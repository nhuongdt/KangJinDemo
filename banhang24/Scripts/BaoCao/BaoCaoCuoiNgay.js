var ViewModel = function () {
    var self = this;
    self.ColumnsExcelTC = ko.observableArray();
    self.ColumnsExcelHH = ko.observableArray();
    self.ColumnsExcelBH = ko.observableArray();
    var cacheExcelBH = true;
    var cacheExcelTC = true;
    var cacheExcelHH = true;

    self.MangChiNhanh = ko.observableArray();
    self.DonVis = ko.observableArray();
    self.searchDonVi = ko.observableArray()
    var _idDonViSeach = $('#hd_IDdDonVi').val();
    var _nameDonViSeach = null;
    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/'
    var _IDDoiTuong = $('.idnguoidung').text();
    self.Loc_TonKho = ko.observable('1')
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var NhanVienUri = '/api/DanhMuc/NS_NhanVienAPI/';
    var BH_KhuyenMaiUri = '/api/DanhMuc/BH_KhuyenMaiAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    var _id_DonVi = $('#hd_IDdDonVi').val();
    var _id_NhanVien = $('.idnhanvien').text();
    var _pageNumber = 1;
    var _pageNumberBanHang = 1;
    var _pageNumberHangHoa = 1;
    var AllPageHangHoa;
    var _pageSize = 10;
    var AllPage;
    var AllPageBanHang;
    self.txtShow = ko.observable();
    self.currentPage = ko.observable(1);
    self.currentPageBanHang = ko.observable(1);
    self.currentPageHangHoa = ko.observable(1);
    self.TenChiNhanh = ko.observable($('.branch label').text());
    self.MangNguoiBan = ko.observableArray();
    self.searchNguoiban = ko.observableArray()
    self.searchLoaiThuChi = ko.observableArray()
    self.NguoiBans = ko.observableArray();
    self.LoaiSP_HH = ko.observable(true);
    self.LoaiSP_DV = ko.observable(true);
    self.NhomHangHoas = ko.observableArray();
    self.SumNumberPageReport = ko.observableArray();
    self.SumRowsHangHoa = ko.observable();
    self.RowsStart = ko.observable('1');
    self.RowsEnd = ko.observable('10');
    self.RowsStartBanHang = ko.observable('1');
    self.RowsEndBanHang = ko.observable('10');
    self.RowsStartHangHoa = ko.observable('1');
    self.RowsEndHangHoa = ko.observable('10');
    self.MoiQuanTam = ko.observable('Báo cáo cuối ngày theo bán hàng');
    $('#txtMaKH').focus();
    var dt1 = new Date();
    var _timeStart = dt1.getFullYear() + "-" + (dt1.getMonth() + 1) + "-" + dt1.getDate();
    var _timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 1))).format('YYYY-MM-DD');
    self.TodayBC = ko.observable(moment(_timeStart).format('DD/MM/YYYY'));
    var _maHH = null;
    var _maKH = null;
    var _kieubang = 1;
    var _tenNguoiBanSeach = null;
    var _tenLoaiThuChiSeach = "1,2,3,4,5,6";
    var mang1 = { ID: 1, TenLoaiThuChi: "Phiếu thu khác" };
    var mang2 = { ID: 2, TenLoaiThuChi: "Phiếu chi khác" };
    var mang3 = { ID: 3, TenLoaiThuChi: "Thu tiền khách trả" };
    var mang4 = { ID: 4, TenLoaiThuChi: "Chi tiền trả khách" };
    var mang5 = { ID: 5, TenLoaiThuChi: "Thu tiền NCC trả" };
    var mang6 = { ID: 6, TenLoaiThuChi: "Chi tiền trả NCC" };
    self.LoaiThuChis = ko.observableArray();
    self.MangLoaiThuChi = ko.observableArray();
    self.LoaiThuChis.push(mang1);
    self.LoaiThuChis.push(mang2);
    self.LoaiThuChis.push(mang3);
    self.LoaiThuChis.push(mang4);
    self.LoaiThuChis.push(mang5);
    self.LoaiThuChis.push(mang6);
    console.log(self.LoaiThuChis());
    self.searchLoaiThuChi(self.LoaiThuChis());
    self.ReportCuoiNgay_BanHang = ko.observableArray();
    //trinhpv phân quyền
    self.BCCuoiNgay = ko.observable();
    self.BCCN_BanHang = ko.observable();
    self.BCCN_BanHang_XuatFile = ko.observable();
    self.BCCN_HangHoa = ko.observable();
    self.BCCN_HangHoa_XuatFile = ko.observable();
    self.BCCN_ThuChi = ko.observable();
    self.BCCN_ThuChi_XuatFile = ko.observable();
    self.BCCN_TongHop = ko.observable();
    self.BCCN_TongHop_XuatFile = ko.observable();

    function getQuyen_NguoiDung() {
        //quyền xem báo cáo
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCCuoiNgay", "GET").done(function (data) {
            self.BCCuoiNgay(data);
            console.log(data);
        });
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCCN_BanHang", "GET").done(function (data) {
            self.BCCN_BanHang(data);
            console.log(data);
        });
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCCN_BanHang_XuatFile", "GET").done(function (data) {
            self.BCCN_BanHang_XuatFile(data);
            console.log(data);
        });
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCCN_HangHoa", "GET").done(function (data) {
            self.BCCN_HangHoa(data);
            console.log(data);
        });
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCCN_HangHoa_XuatFile", "GET").done(function (data) {
            self.BCCN_HangHoa_XuatFile(data);
            console.log(data);
        });
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCCN_ThuChi", "GET").done(function (data) {
            self.BCCN_ThuChi(data);
            console.log(data);
        });
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCCN_ThuChi_XuatFile", "GET").done(function (data) {
            self.BCCN_ThuChi_XuatFile(data);
            console.log(data);
        });
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCCN_TongHop", "GET").done(function (data) {
            self.BCCN_TongHop(data);
            console.log(data);
        });
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCCN_TongHop_XuatFile", "GET").done(function (data) {
            self.BCCN_TongHop_XuatFile(data);
            console.log(data);
        });
    }
    getQuyen_NguoiDung();
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
        getAllNSNhanVien();
        $('#selec-all-DonVi li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });

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
            getAllNSNhanVien();
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

    function getAllNSNhanVien() {
        ajaxHelper(BH_KhuyenMaiUri + "getNhanViens?nameChinhanh=" + _idDonViSeach, 'GET').done(function (data) {
            self.NguoiBans(data);
            self.searchNguoiban(data);
            for (var i = 0; i < self.NguoiBans().length; i++) {
                if (i == 0) {
                    _tenNguoiBanSeach = self.NguoiBans()[i].ID;
                }
                else {
                    _tenNguoiBanSeach = self.NguoiBans()[i].ID + "," + _tenNguoiBanSeach;
                }
            }
            nextPage = 1;
            if (_kieubang == 1) {
                self.getListCuoiNgay_BanHang();
            }
            else if (_kieubang == 2) {
                self.getListCuoiNgay_ThuChi();
            }
            else if (_kieubang == 3) {
                self.getListCuoiNgay_HangHoa();
            }

            else if (_kieubang == 4) {
                self.getListCuoiNgay_TongKetThuChi();
                self.getListCuoiNgay_TongKetBanHang();
                self.getListCuoiNgay_SoLuongGiaoDich();
                self.getListCuoiNgay_SoLuongHangHoa();
            }
        });
    }
    //getAllNSNhanVien();
    self.hideTableReport = function () {
        $('.table_BanHang').hide();
        $('.table_ThuChi').hide();
        $('.table_HangHoa').hide();
        $('.table_TongHop1').hide();
        $('.table_TongHop2').hide();
        $('.table_TongHop3').hide();
        $('.table_TongHop4').hide();
        $('.PhanTrangTH').hide();
        $('.Export_Excel').hide();
    }
    self.hideTableReport();
    $('.table_BanHang').show();
    $('.Export_Excel').show();

    self.hideRadioReport = function () {
        $('.rd_KhachHang').hide();
        $('.rd_HangHoa').hide();
        $('.rd_LoaiHang').hide();
        $('.rd_LoaiThuChi').hide();
        $('.rd_NhomHang').hide();
    }
    self.hideRadioReport();
    $('.rd_KhachHang').show();
    $('.chose_Time input').on('click', function () {
        $(".ShowColumn").show();
        self.hideTableReport();
        self.hideRadioReport();
        _kieubang = $(this).val();
        console.log(_kieubang);
        if ($(this).val() == 1) {
            $('.table_BanHang').show();
            $('.rd_KhachHang').show();
            $('.Export_Excel').show();
            $(".list_BanHang").show();
            $(".list_HangHoa").hide();
            $(".list_ThuChi").hide();
            self.MoiQuanTam('Báo cáo cuối ngày theo bán hàng');
            self.getListCuoiNgay_BanHang();
        }
        else if ($(this).val() == 2) {
            $('.table_ThuChi').show();
            $(".list_BanHang").hide();
            $(".list_ThuChi").show();
            $(".list_HangHoa").hide();
            $('.rd_KhachHang').show();
            $('.rd_LoaiThuChi').show();
            $('.PhanTrangTH').show();
            $('.Export_Excel').show();
            self.txtShow(" Phiếu")
            self.MoiQuanTam('Báo cáo cuối ngày theo thu chi');
            self.getListCuoiNgay_ThuChi();
        }
        else if ($(this).val() == 3) {
            $(".list_BanHang").hide();
            $(".list_ThuChi").hide();
            $(".list_HangHoa").show();
            $('.table_HangHoa').show();
            $('.rd_KhachHang').show();
            $('.rd_HangHoa').show();
            $('.rd_LoaiHang').show();
            $('.rd_NhomHang').show();
            $('.PhanTrangTH').show();
            $('.Export_Excel').show();
            self.txtShow(" Hàng hóa")
            self.MoiQuanTam('Báo cáo cuối ngày theo hàng hóa');
            self.getListCuoiNgay_HangHoa();
        }
        else if ($(this).val() == 4) {
            $(".ShowColumn").hide();
            //$('.PhanTrangTH').show();
            self.MoiQuanTam('Báo cáo tổng hợp cuối ngày');
            if (self.BCCN_TongHop() == 'BCCN_TongHop') {
                $(".PhanQuyen").hide();
                $('.table_TongHop1').show();
                $('.table_TongHop2').show();
                $('.table_TongHop3').show();
                $('.table_TongHop4').show();
                self.getListCuoiNgay_TongKetThuChi();
                self.getListCuoiNgay_TongKetBanHang();
                self.getListCuoiNgay_SoLuongGiaoDich();
                self.getListCuoiNgay_SoLuongHangHoa();
            }
            else {
                $(".PhanQuyen").show();
            }
        }
    })
    //$('#datetimepicker_mask').datetimepicker().on('dp.change', function (e) {
    //    console.log("abc")
    //});
    $('#datetimepicker_mask').keypress(function (e) {
        if (e.keyCode == 13) {
            dktime = $(this).val();
            thisDate = $(this).val();
            var t = thisDate.split(" ");
            var t1 = t[0].split("/").reverse().join("-")
            thisDate = moment(t1).format('MM/DD/YYYY')
            _timeStart = moment(new Date(thisDate)).format('YYYY-MM-DD');
            var dt = new Date(thisDate);
            _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
            console.log(_timeEnd);
            if (thisDate != 'Invalid date') {
                self.TodayBC($(this).val())
                _pageNumber = 1;
                if (_kieubang == 1) {
                    self.getListCuoiNgay_BanHang();
                }
                else if (_kieubang == 2) {
                    self.getListCuoiNgay_ThuChi();
                }
                else if (_kieubang == 3) {
                    self.getListCuoiNgay_HangHoa();
                }
                else if (_kieubang == 4) {
                    self.getListCuoiNgay_TongKetThuChi();
                    self.getListCuoiNgay_TongKetBanHang();
                    self.getListCuoiNgay_SoLuongGiaoDich();
                    self.getListCuoiNgay_SoLuongHangHoa();
                }
                self.ReserPage();
            }
            else {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Định dạng thời gian không chính xác!", "danger");
            }
        }
    });
    $('#datetimepicker_mask').on('change.dp', function (e) {
        console.log('123');
        dktime = $(this).val();
        thisDate = $(this).val();
        var t = thisDate.split(" ");
        var t1 = t[0].split("/").reverse().join("-")
        thisDate = moment(t1).format('MM/DD/YYYY')
        _timeStart = moment(new Date(thisDate)).format('YYYY-MM-DD');
        var dt = new Date(thisDate);
        _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
        if (thisDate != 'Invalid date') {
            self.TodayBC($(this).val())
            _pageNumber = 1;
            if (_kieubang == 1) {
                self.getListCuoiNgay_BanHang();
            }
            else if (_kieubang == 2) {
                self.getListCuoiNgay_ThuChi();
            }
            else if (_kieubang == 3) {
                self.getListCuoiNgay_HangHoa();
            }
            else if (_kieubang == 4) {
                self.getListCuoiNgay_TongKetThuChi();
                self.getListCuoiNgay_TongKetBanHang();
                self.getListCuoiNgay_SoLuongGiaoDich();
                self.getListCuoiNgay_SoLuongHangHoa();
            }
            self.ReserPage();
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Định dạng thời gian không chính xác!", "danger");
        }
    });

    $('#txtDate').on('dp.change', function (e) {
        dktime = $(this).val();
        thisDate = $(this).val();
        var t = thisDate.split(" ");
        var t1 = t[0].split("/").reverse().join("-")
        thisDate = moment(t1).format('MM/DD/YYYY')
        console.log(thisDate);
        _timeStart = moment(new Date(thisDate)).format('YYYY-MM-DD');
        var dt = new Date(thisDate);
        _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
        self.TodayBC($(this).val())
        _pageNumber = 1;
        if (_kieubang == 1) {
            self.getListCuoiNgay_BanHang();
        }
        else if (_kieubang == 2) {
            self.getListCuoiNgay_ThuChi();
        }
        else if (_kieubang == 3) {
            self.getListCuoiNgay_HangHoa();
        }
        else if (_kieubang == 4) {
            self.getListCuoiNgay_TongKetThuChi();
            self.getListCuoiNgay_TongKetBanHang();
            self.getListCuoiNgay_SoLuongGiaoDich();
            self.getListCuoiNgay_SoLuongHangHoa();
        }
        self.ReserPage();
    });
    self.CloseNguoiBan = function (item) {
        _tenNguoiBanSeach = null;
        self.MangNguoiBan.remove(item);
        for (var i = 0; i < self.MangNguoiBan().length; i++) {
            _tenNguoiBanSeach = self.MangNguoiBan()[i].ID + "," + _tenNguoiBanSeach;
        }
        if (self.MangNguoiBan().length === 0) {
            getAllNSNhanVien();
        }
        else {
            nextPage = 1;
            if (_kieubang == 1) {
                self.getListCuoiNgay_BanHang();
            }
            else if (_kieubang == 2) {
                self.getListCuoiNgay_ThuChi();
            }
            else if (_kieubang == 3) {
                self.getListCuoiNgay_HangHoa();
            }

            else if (_kieubang == 4) {
                self.getListCuoiNgay_TongKetThuChi();
                self.getListCuoiNgay_TongKetBanHang();
                self.getListCuoiNgay_SoLuongGiaoDich();
                self.getListCuoiNgay_SoLuongHangHoa();
            }
        }
        // remove check
        $('#selec-all-NguoiBan li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        console.log(self.MangNguoiBan())
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
                if (_tenNguoiBanSeach == null)
                    _tenNguoiBanSeach = self.MangNguoiBan()[i].ID;
                else
                    _tenNguoiBanSeach = self.MangNguoiBan()[i].ID + "," + _tenNguoiBanSeach;
            }
        }
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
        console.log(_tenNguoiBanSeach);
        if (_kieubang == 1) {
            self.getListCuoiNgay_BanHang();
        }
        else if (_kieubang == 2) {
            self.getListCuoiNgay_ThuChi();
        }
        else if (_kieubang == 3) {
            self.getListCuoiNgay_HangHoa();

        }
        else if (_kieubang == 4) {
            self.getListCuoiNgay_TongKetThuChi();
            self.getListCuoiNgay_TongKetBanHang();
            self.getListCuoiNgay_SoLuongGiaoDich();
            self.getListCuoiNgay_SoLuongHangHoa();
        }
    }

    self.CloseLoaiThuChi = function (item) {
        _tenLoaiThuChiSeach = null;
        self.MangLoaiThuChi.remove(item);
        for (var i = 0; i < self.MangLoaiThuChi().length; i++) {
            _tenLoaiThuChiSeach = self.MangLoaiThuChi()[i].ID + "," + _tenLoaiThuChiSeach;
        }
        if (self.MangLoaiThuChi().length === 0) {
            // $('#choose_LoaiThuChi').append('<input type="text" class="dropdown" placeholder="Chọn người bán...">');
            _tenLoaiThuChiSeach = "1,2,3,4,5,6";
        }
        // remove check
        $('#selec-all-LoaiThuChi li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        console.log(self.MangLoaiThuChi());
        console.log(_tenLoaiThuChiSeach);
        nextPage = 1;
        if (_kieubang == 1) {
            self.getListCuoiNgay_BanHang();
        }
        else if (_kieubang == 2) {
            self.getListCuoiNgay_ThuChi();
        }
        else if (_kieubang == 3) {
            self.getListCuoiNgay_HangHoa();
        }

        else if (_kieubang == 4) {
            self.getListCuoiNgay_TongKetThuChi();
            self.getListCuoiNgay_TongKetBanHang();
            self.getListCuoiNgay_SoLuongGiaoDich();
            self.getListCuoiNgay_SoLuongHangHoa();
        }
    }
    self.SelectedLoaiThuChi = function (item) {
        _tenLoaiThuChiSeach = null;
        var arrIDLoaiThuChi = [];
        for (var i = 0; i < self.MangLoaiThuChi().length; i++) {
            if ($.inArray(self.MangLoaiThuChi()[i], arrIDLoaiThuChi) === -1) {
                arrIDLoaiThuChi.push(self.MangLoaiThuChi()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDLoaiThuChi) === -1) {
            self.MangLoaiThuChi.push(item);
            for (var i = 0; i < self.MangLoaiThuChi().length; i++) {
                if (_tenLoaiThuChiSeach == null)
                    _tenLoaiThuChiSeach = self.MangLoaiThuChi()[i].ID;
                else
                    _tenLoaiThuChiSeach = self.MangLoaiThuChi()[i].ID + "," + _tenLoaiThuChiSeach;
            }
        }
        $('#NoteNameLoaiThuChi').val('');
        self.LoaiThuChis(self.searchLoaiThuChi());

        //đánh dấu check
        for (var i = 0; i < self.MangLoaiThuChi().length; i++) {
            $('#selec-all-LoaiThuChi li').each(function () {
                if ($(this).attr('id') === self.MangLoaiThuChi()[i].ID) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>')
                }
            });
        }
        nextPage = 1;
        console.log(_tenLoaiThuChiSeach);
        if (_kieubang == 1) {
            //self.getListCuoiNgay_BanHang();
        }
        else if (_kieubang == 2) {
            self.getListCuoiNgay_ThuChi();
        }
        else if (_kieubang == 3) {
            self.getListCuoiNgay_HangHoa();
        }
        else if (_kieubang == 4) {
            self.getListCuoiNgay_TongKetThuChi();
            self.getListCuoiNgay_TongKetBanHang();
            self.getListCuoiNgay_SoLuongGiaoDich();
            self.getListCuoiNgay_SoLuongHangHoa();
        }
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
    $('#NoteNameNguoiBan').keypress(function (e) {
        if (e.keyCode == 13 && self.NguoiBans().length > 0) {
            self.SelectedNguoiBan(self.NguoiBans()[0]);
        }
    });

    self.NoteNameThuChi = function () {
        var arrLoaiThuChi = [];
        var itemSearch = locdau($('#NoteNameLoaiThuChi').val().toLowerCase());
        for (var i = 0; i < self.searchLoaiThuChi().length; i++) {
            var locdauInput = locdau(self.searchLoaiThuChi()[i].TenLoaiThuChi).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrLoaiThuChi.push(self.searchLoaiThuChi()[i]);
            }
        }
        self.LoaiThuChis(arrLoaiThuChi);
        if ($('#NoteNameLoaiThuChi').val() == "") {
            self.LoaiThuChis(self.searchLoaiThuChi());
        }

    }
    $('#NoteNameLoaiThuChi').keypress(function (e) {
        if (e.keyCode == 13 && self.LoaiThuChis().length > 0) {
            self.SelectedLoaiThuChi(self.LoaiThuChis()[0]);
        }
    });

    //function getNhomHangHoa() {
    //    ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetDM_NhomHangHoa", 'GET').done(function (data) {
    //        self.NhomHangHoas(data);
    //        //console.log(self.NhomHangHoas());
    //    })
    //}
    //getNhomHangHoa();
    //getNhomHangHoa();
    var tk = null;
    function GetAllNhomHH() {
        ajaxHelper(ReportUri + "GetListID_NhomHangHoa?TenNhomHang=" + tk, 'GET').done(function (data) {
            console.log(data);
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
                        console.log(data);
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
    var _ID_NhomHang = null;
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
        self.getListCuoiNgay_HangHoa();
    }
    $('.SelectALLNhomHang').on('click', function () {
        _ID_NhomHang = null;
        _pageNumber = 1;
        self.getListCuoiNgay_HangHoa();
    });

    //self.NoteNhomHang = function () {
    //    var tk = $('#SeachNhomHang').val();
    //    console.log(tk);
    //    if (tk.trim() != '') {
    //        ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "SeachDM_NhomHangHoa?TenNhom=" + tk, 'GET').done(function (data) {
    //            self.NhomHangHoas(data);
    //        })
    //    }
    //    else {
    //        ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetDM_NhomHangHoa", 'GET').done(function (data) {
    //            self.NhomHangHoas(data);
    //        })
    //    }
    //}

    //var _ID_NhomHang = null;
    //self.SelectRepoert_NhomHangHoa = function (item) {
    //    _ID_NhomHang = item.ID;
    //    $('.SelectNhomHang li').each(function () {
    //        if ($(this).attr('id_NhomHang') === item.ID) {
    //            $(this).addClass('SelectReport');
    //        }
    //        else {
    //            $(this).removeClass('SelectReport');
    //        }
    //    });
    //    $('.SelectALLNhomHang li').removeClass('SelectReport');
    //    _pageNumber = 1;
    //    self.getListCuoiNgay_HangHoa();
    //}
    //$('.SelectALLNhomHang').on('click', function () {
    //    $('.SelectALLNhomHang li').addClass('SelectReport')
    //    $('.SelectNhomHang li').each(function () {
    //        $(this).removeClass('SelectReport');
    //    });
    //    _ID_NhomHang = null;
    //    _pageNumber = 1;
    //    self.getListCuoiNgay_HangHoa();
    //});
    // Select LoaiHang
    var _laHangHoa = 2;
    var _ckHangHoa = 1;
    var _ckDichVu = 1;
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
            self.getListCuoiNgay_BanHang();
        }
        else if (_kieubang == 2) {
            self.getListCuoiNgay_ThuChi();
        }
        else if (_kieubang == 3) {
            self.getListCuoiNgay_HangHoa();
        }
        else if (_kieubang == 4) {
            self.getListCuoiNgay_TongKetThuChi();
            self.getListCuoiNgay_TongKetBanHang();
            self.getListCuoiNgay_SoLuongGiaoDich();
            self.getListCuoiNgay_SoLuongHangHoa();
        }
    })
    // Key Event maKH
    self.SelectMaKH = function () {
        _maKH = $('#txtMaKH').val();
        console.log(_maKH);
    }
    $('#txtMaKH').keypress(function (e) {
        if (e.keyCode == 13) {
            console.log("a");
            if (_kieubang == 1) {
                self.getListCuoiNgay_BanHang();
            }
            else if (_kieubang == 2) {
                self.getListCuoiNgay_ThuChi();
            }
            else if (_kieubang == 3) {
                self.getListCuoiNgay_HangHoa();
            }
            else if (_kieubang == 4) {
                self.getListCuoiNgay_TongKetThuChi();
                self.getListCuoiNgay_TongKetBanHang();
                self.getListCuoiNgay_SoLuongGiaoDich();
                self.getListCuoiNgay_SoLuongHangHoa();
            }
        }
    })
    // Key Event maHH
    self.SelectMaHH = function () {
        _maHH = $('#txtMaHH').val();
        console.log(_maHH);
    }
    $('#txtMaHH').keypress(function (e) {
        if (e.keyCode == 13) {
            _pageNumber = 1;
            if (_kieubang == 1) {

            }
            else if (_kieubang == 2) {
                self.getListCuoiNgay_ThuChi();
            }
            else if (_kieubang == 3) {
                self.getListCuoiNgay_HangHoa();
            }
            else if (_kieubang == 4) {
                self.getListCuoiNgay_TongKetThuChi();
                self.getListCuoiNgay_TongKetBanHang();
                self.getListCuoiNgay_SoLuongGiaoDich();
                self.getListCuoiNgay_SoLuongHangHoa();
            }
        }
    })




    self.GetClass = function (page) {
        return (page.SoTrang === self.currentPage()) ? "click" : "";
    };
    self.GetClassBanHang = function (page) {
        return (page.SoTrang === self.currentPageBanHang()) ? "click" : "";
    };
    self.GetClassHangHoa = function (page) {
        return (page.SoTrang === self.currentPageHangHoa()) ? "click" : "";
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
            if (_kieubang == 1) {

            }
            else if (_kieubang == 2) {
                self.SelectListCuoiNgay_ThuChi();
            }
            else if (_kieubang == 3) {
                self.SelectListCuoiNgay_HangHoa();
            }
            else if (_kieubang == 4) {

            }
        }
    };
    self.BackPage = function (item) {
        if (_pageNumber > 1) {
            _pageNumber = _pageNumber - 1;
            self.ReserPage();
            if (_kieubang == 1) {

            }
            else if (_kieubang == 2) {
                self.SelectListCuoiNgay_ThuChi();
            }
            else if (_kieubang == 3) {
                self.SelectListCuoiNgay_HangHoa();
            }
            else if (_kieubang == 4) {

            }
        }
    };
    self.EndPage = function (item) {
        _pageNumber = AllPage;
        self.ReserPage();
        if (_kieubang == 1) {

        }
        else if (_kieubang == 2) {
            self.SelectListCuoiNgay_ThuChi();
        }
        else if (_kieubang == 3) {
            self.SelectListCuoiNgay_HangHoa();
        }
        else if (_kieubang == 4) {

        }
    };
    self.StartPage = function (item) {
        _pageNumber = 1;
        self.ReserPage();
        if (_kieubang == 1) {

        }
        else if (_kieubang == 2) {
            self.SelectListCuoiNgay_ThuChi();
        }
        else if (_kieubang == 3) {
            self.SelectListCuoiNgay_HangHoa();
        }
        else if (_kieubang == 4) {

        }
    };
    self.gotoNextPage = function (item) {
        _pageNumber = item.SoTrang;
        self.ReserPage();
        if (_kieubang == 1) {

        }
        else if (_kieubang == 2) {
            self.SelectListCuoiNgay_ThuChi();
        }
        else if (_kieubang == 3) {
            self.SelectListCuoiNgay_HangHoa();
        }
        else if (_kieubang == 4) {

        }
    }
    // phân trang chi tiết bán hàng
    self.selecPageBanHang = function () {
        // AllPageBanHang = self.SumNumberPageReport().length;

        if (AllPageBanHang > 4) {
            for (var i = 3; i < AllPageBanHang; i++) {
                self.SumNumberPageReportBanHang.pop(i + 1);
            }
            self.SumNumberPageReportBanHang.push({ SoTrang: 4 });
            self.SumNumberPageReportBanHang.push({ SoTrang: 5 });
        }
        else {
            for (var i = 0; i < 6; i++) {
                self.SumNumberPageReportBanHang.pop(i);
            }
            for (var j = 0; j < AllPageBanHang; j++) {
                self.SumNumberPageReportBanHang.push({ SoTrang: j + 1 });
            }
        }
        $('#StartPageBanHang').hide();
        $('#BackPageBanHang').hide();
        $('#NextPageBanHang').show();
        $('#EndPageBanHang').show();
    }
    self.ReserPageBanHang = function (item) {
        //self.selecPage();
        if (_pageNumberBanHang > 1 && AllPageBanHang > 5/* && nextPage < AllPageBanHang - 1*/) {
            if (_pageNumberBanHang > 3 && _pageNumberBanHang < AllPageBanHang - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportBanHang.replace(self.SumNumberPageReportBanHang()[i], { SoTrang: parseInt(_pageNumberBanHang) + i - 2 });
                }
            }
            else if (parseInt(_pageNumberBanHang) === parseInt(AllPageBanHang) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportBanHang.replace(self.SumNumberPageReportBanHang()[i], { SoTrang: parseInt(_pageNumberBanHang) + i - 3 });
                }
            }
            else if (_pageNumberBanHang == AllPageBanHang) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportBanHang.replace(self.SumNumberPageReportBanHang()[i], { SoTrang: parseInt(_pageNumberBanHang) + i - 4 });
                }
            }
            else if (_pageNumberBanHang < 4) {
                for (var i = 0; i < 5; i++) {
                    //console.log(_pageNumberBanHang)
                    self.SumNumberPageReportBanHang.replace(self.SumNumberPageReportBanHang()[i], { SoTrang: 1 + i });
                }
            }
        }
        else if (_pageNumberBanHang == 1 && AllPageBanHang > 5) {
            for (var i = 0; i < 5; i++) {
                self.SumNumberPageReportBanHang.replace(self.SumNumberPageReportBanHang()[i], { SoTrang: parseInt(_pageNumberBanHang) + i });
            }
        }
        if (_pageNumberBanHang > 1) {
            $('#StartPageBanHang').show();
            $('#BackPageBanHang').show();
        }
        else {
            $('#StartPageBanHang').hide();
            $('#BackPageBanHang').hide();
        }
        if (_pageNumberBanHang == AllPageBanHang) {
            $('#NextPageBanHang').hide();
            $('#EndPageBanHang').hide();
        }
        else {
            $('#NextPageBanHang').show();
            $('#EndPageBanHang').show();
        }
        self.currentPageBanHang(parseInt(_pageNumberBanHang));
    }
    self.NextPageBanHang = function (item) {
        if (_pageNumberBanHang < AllPageBanHang) {
            _pageNumberBanHang = _pageNumberBanHang + 1;
            self.ReserPageBanHang();
            self.SelectedPageNumberReportNCC_BanHangChiTiet();
        }
    };
    self.BackPageBanHang = function (item) {
        if (_pageNumberBanHang > 1) {
            _pageNumberBanHang = _pageNumberBanHang - 1;
            self.ReserPageBanHang();
            self.SelectedPageNumberReportNCC_BanHangChiTiet();
        }
    };
    self.EndPageBanHang = function (item) {
        _pageNumberBanHang = AllPageBanHang;
        self.ReserPageBanHang();
        self.SelectedPageNumberReportNCC_BanHangChiTiet();
    };
    self.StartPageBanHang = function (item) {
        _pageNumberBanHang = 1;
        self.ReserPageBanHang();
        self.SelectedPageNumberReportNCC_BanHangChiTiet();
    };
    self.gotoNextPageBanHang = function (item) {
        _pageNumberBanHang = item.SoTrang;
        self.ReserPageBanHang();
        self.SelectedPageNumberReportNCC_BanHangChiTiet();
    }

    // phân trang chi tiết HangHoa
    self.selecPageHangHoa = function () {
        // AllPageHangHoa = self.SumNumberPageReport().length;

        if (AllPageHangHoa > 4) {
            for (var i = 3; i < AllPageHangHoa; i++) {
                self.SumNumberPageReportHangHoa.pop(i + 1);
            }
            self.SumNumberPageReportHangHoa.push({ SoTrang: 4 });
            self.SumNumberPageReportHangHoa.push({ SoTrang: 5 });
        }
        else {
            for (var i = 0; i < 6; i++) {
                self.SumNumberPageReportHangHoa.pop(i);
            }
            for (var j = 0; j < AllPageHangHoa; j++) {
                self.SumNumberPageReportHangHoa.push({ SoTrang: j + 1 });
            }
        }
        $('#StartPageHangHoa').hide();
        $('#BackPageHangHoa').hide();
        $('#NextPageHangHoa').show();
        $('#EndPageHangHoa').show();
    }
    self.ReserPageHangHoa = function (item) {
        //self.selecPage();
        if (_pageNumberHangHoa > 1 && AllPageHangHoa > 5/* && nextPage < AllPageHangHoa - 1*/) {
            if (_pageNumberHangHoa > 3 && _pageNumberHangHoa < AllPageHangHoa - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportHangHoa.replace(self.SumNumberPageReportHangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoa) + i - 2 });
                }
            }
            else if (parseInt(_pageNumberHangHoa) === parseInt(AllPageHangHoa) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportHangHoa.replace(self.SumNumberPageReportHangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoa) + i - 3 });
                }
            }
            else if (_pageNumberHangHoa == AllPageHangHoa) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportHangHoa.replace(self.SumNumberPageReportHangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoa) + i - 4 });
                }
            }
            else if (_pageNumberHangHoa < 4) {
                for (var i = 0; i < 5; i++) {
                    //console.log(_pageNumberHangHoa)
                    self.SumNumberPageReportHangHoa.replace(self.SumNumberPageReportHangHoa()[i], { SoTrang: 1 + i });
                }
            }
        }
        else if (_pageNumberHangHoa == 1 && AllPageHangHoa > 5) {
            for (var i = 0; i < 5; i++) {
                self.SumNumberPageReportHangHoa.replace(self.SumNumberPageReportHangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoa) + i });
            }
        }
        if (_pageNumberHangHoa > 1) {
            $('#StartPageHangHoa').show();
            $('#BackPageHangHoa').show();
        }
        else {
            $('#StartPageHangHoa').hide();
            $('#BackPageHangHoa').hide();
        }
        if (_pageNumberHangHoa == AllPageHangHoa) {
            $('#NextPageHangHoa').hide();
            $('#EndPageHangHoa').hide();
        }
        else {
            $('#NextPageHangHoa').show();
            $('#EndPageHangHoa').show();
        }
        self.currentPageHangHoa(parseInt(_pageNumberHangHoa));
    }
    self.NextPageHangHoa = function (item) {
        if (_pageNumberHangHoa < AllPageHangHoa) {
            _pageNumberHangHoa = _pageNumberHangHoa + 1;
            self.ReserPageHangHoa();
            self.SelectedPageNumberReportNCC_HangHoaChiTiet();
        }
    };
    self.BackPageHangHoa = function (item) {
        if (_pageNumberHangHoa > 1) {
            _pageNumberHangHoa = _pageNumberHangHoa - 1;
            self.ReserPageHangHoa();
            self.SelectedPageNumberReportNCC_HangHoaChiTiet();
        }
    };
    self.EndPageHangHoa = function (item) {
        _pageNumberHangHoa = AllPageHangHoa;
        self.ReserPageHangHoa();
        self.SelectedPageNumberReportNCC_HangHoaChiTiet();
    };
    self.StartPageHangHoa = function (item) {
        _pageNumberHangHoa = 1;
        self.ReserPageHangHoa();
        self.SelectedPageNumberReportNCC_HangHoaChiTiet();
    };
    self.gotoNextPageHangHoa = function (item) {
        console.log(2);
        _pageNumberHangHoa = item.SoTrang;
        self.ReserPageHangHoa();
        self.SelectedPageNumberReportNCC_HangHoaChiTiet();
    }
    self.CN_SoLuong = ko.observable();
    self.CN_DoanhThu = ko.observable();
    self.CN_TongTienHang = ko.observable();
    self.CN_GiamGiaHD = ko.observable();
    self.CN_PhiTraHang = ko.observable();
    self.CN_ThucThu = ko.observable();
    //$('.TongCongBanHang').hide();
    //$(".PhanQuyen").hide();
    self.getListCuoiNgay_BanHang = function () {
        hidewait('table_h');
        if (self.BCCN_BanHang() == 'BCCN_BanHang') {
            $(".PhanQuyen").hide();
            ajaxHelper(ReportUri + "getListCuoiNgay_BanHang?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
                self.ReportCuoiNgay_BanHang(data.LstData);
                self.CN_SoLuong(data.TongGiaVon);
                self.CN_DoanhThu(data.DoanhThu);
                self.CN_TongTienHang(data.TongTienHang);
                self.CN_GiamGiaHD(data.GiamGiaHD);
                self.CN_PhiTraHang(data.GiaTriTra);
                self.CN_ThucThu(data.LoiNhuanGop);
                $("div[id ^= 'wait']").text("");
                if (self.ReportCuoiNgay_BanHang().length > 0) {
                    $('.Report_Empty').hide();
                    $('.TongCongBanHang').show();
                    $('.page').show();
                }
                else {
                    $('.TongCongBanHang').hide();
                    $('.Report_Empty').show();
                    $('.page').hide();
                }
                $('.PhanTrangTH').hide();
                LoadHtmlGrid(cacheExcelBH, 1, "banhangCN");
            });
        }
        else {
            $(".PhanQuyen").show();
            $('.TongCongBanHang').hide();
            $('.Report_Empty').hide();
            $('.page').hide();
            $("div[id ^= 'wait']").text("");
        }
    }
    var _loaihoadon;
    self.reportCuoiNgay_ChiTietBanHang = ko.observableArray();
    //self.reportCuoiNgay_ChiTietBanHangPrint = ko.observableArray();
    self.SumNumberPageReportBanHang = ko.observableArray();
    self.SumRowsHangHoaBanHang = ko.observable();
    self.selectCuoiNgay_ChiTietBanHang = function (item) {
        console.log('1');
        _loaihoadon = item.LoaiHoaDon;
        //var $this = $('#ChiTiet' + item.LoaiHoaDon);
        //$this.toggle();

        console.log(_idDonViSeach);
        hidewait('table_h');
        _pageNumberBanHang = 1;
        ajaxHelper(ReportUri + "getListCuoiNgay_ChiTietBanHang?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&loaihoadon=" + _loaihoadon + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberBanHang + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            console.log(data);
            self.reportCuoiNgay_ChiTietBanHang(data.LstData);
            //self.reportCuoiNgay_ChiTietBanHangPrint(data.LstDataPrint);
            if (self.reportCuoiNgay_ChiTietBanHang().length != 0) {
                self.RowsStartBanHang((_pageNumberBanHang - 1) * _pageSize + 1);
                self.RowsEndBanHang((_pageNumberBanHang - 1) * _pageSize + self.reportCuoiNgay_ChiTietBanHang().length)
            }
            else {
                self.RowsStartBanHang('0');
                self.RowsEndBanHang('0');
            }
            self.SumNumberPageReportBanHang(data.LstPageNumber);
            AllPageBanHang = self.SumNumberPageReportBanHang().length;
            self.selecPageBanHang();
            self.ReserPageBanHang();
            self.SumRowsHangHoaBanHang(data.Rowcount);
            $("div[id ^= 'wait']").text("");
        });
    }
    self.SelectedPageNumberReportNCC_BanHangChiTiet = function (item) {
        hidewait('table_h');
        //_pageNumberBanHang = 1;
        ajaxHelper(ReportUri + "getListCuoiNgay_ChiTietBanHang?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&loaihoadon=" + _loaihoadon + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberBanHang + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.reportCuoiNgay_ChiTietBanHang(data.LstData);
            self.RowsStartBanHang((_pageNumberBanHang - 1) * _pageSize + 1);
            self.RowsEndBanHang((_pageNumberBanHang - 1) * _pageSize + self.reportCuoiNgay_ChiTietBanHang().length);
            $("div[id ^= 'wait']").text("");
        });
    }
    self.ReportCuoiNgay_ThuChi = ko.observableArray();
    self.ReportCuoiNgay_ThuChiPrint = ko.observableArray();
    self.TC_ThuChi = ko.observable();
    $('.TongCongThuChi').hide();
    self.getListCuoiNgay_ThuChi = function () {
        hidewait('table_h');
        if (self.BCCN_ThuChi() == 'BCCN_ThuChi') {
            $(".PhanQuyen").hide();
            ajaxHelper(ReportUri + "getListCuoiNgay_ThuChi?loaithuchi=" + _tenLoaiThuChiSeach + "&ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
                self.ReportCuoiNgay_ThuChi(data.LstData);
                self.ReportCuoiNgay_ThuChiPrint(data.LstDataPrint)
                console.log(data);
                self.TC_ThuChi(data._lailo);
                if (self.ReportCuoiNgay_ThuChi().length != 0) {
                    self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                    self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportCuoiNgay_ThuChi().length)
                }
                else {
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                self.SumNumberPageReport(data.LstPageNumber);
                AllPage = self.SumNumberPageReport().length;
                self.selecPage();
                self.ReserPage();
                self.SumRowsHangHoa(data.Rowcount);
                $("div[id ^= 'wait']").text("");
                if (self.ReportCuoiNgay_ThuChi().length > 0) {
                    $('.TongCongThuChi').show();
                    $(".PhanTrangTH").show();
                    $('.Report_Empty').hide();

                }

                else {
                    $('.TongCongThuChi').hide();
                    $(".PhanTrangTH").hide();
                    $('.Report_Empty').show();
                }

                LoadHtmlGrid(cacheExcelTC, 2, "thuchiCN");
            });
        }
        else {
            $(".PhanQuyen").show();
            $(".PhanTrangTH").hide();
            $('.TongCongThuChi').hide();
            $('.Report_Empty').hide();
            $("div[id ^= 'wait']").text("");
        }
    }
    self.SelectListCuoiNgay_ThuChi = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListCuoiNgay_ThuChi?loaithuchi=" + _tenLoaiThuChiSeach + "&ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReportCuoiNgay_ThuChi(data.LstData);
            self.RowsStart((_pageNumber - 1) * _pageSize + 1);
            self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportCuoiNgay_ThuChi().length)
            $("div[id ^= 'wait']").text("");
            LoadHtmlGrid(cacheExcelTC, 2, "thuchiCN");
        });
    }
    self.ReportCuoiNgay_HangHoa = ko.observableArray();
    self.ReportCuoiNgay_HangHoaPrint = ko.observableArray();
    self.HH_SoLuongBan = ko.observable();
    self.HH_GiaTriBan = ko.observable();
    self.HH_SoLuongTra = ko.observable();
    self.HH_GiaTriTra = ko.observable();
    self.HH_DoanhThu = ko.observable();
    $('.TongCongHangHoa').hide();
    self.getListCuoiNgay_HangHoa = function () {
        hidewait('table_h');
        if (self.BCCN_HangHoa() == 'BCCN_HangHoa') {
            $(".PhanQuyen").hide();
           
            ajaxHelper(ReportUri + "getListCuoiNgay_HangHoa?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
                self.ReportCuoiNgay_HangHoa(data.LstData);
                self.ReportCuoiNgay_HangHoaPrint(data.LstDataPrint);
                self.HH_SoLuongBan(data.SoLuongBan);
                self.HH_GiaTriBan(data.GiaTriBan);
                self.HH_SoLuongTra(data.SoLuongTra);
                self.HH_GiaTriTra(data.GiaTriTra);
                self.HH_DoanhThu(data.DoanhThuThuan);
                if (self.ReportCuoiNgay_HangHoa().length != 0) {
                    self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                    self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportCuoiNgay_HangHoa().length)
                }
                else {
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                self.SumNumberPageReport(data.LstPageNumber);
                AllPage = self.SumNumberPageReport().length;
                self.selecPage();
                self.ReserPage();
                self.SumRowsHangHoa(data.Rowcount);
                $("div[id ^= 'wait']").text("");
                if (self.ReportCuoiNgay_HangHoa().length > 0)
                {
                    $('.TongCongHangHoa').show();
                    $(".PhanTrangTH").show();
                    $('.Report_Empty').hide();
                }
                else
                {
                    $('.TongCongHangHoa').hide();
                    $(".PhanTrangTH").hide();
                    $('.Report_Empty').show();
                }
                LoadHtmlGrid(cacheExcelHH, 3, "hanghoaCN");
            });
        }
        else {
            $(".PhanQuyen").show();
            $(".PhanTrangTH").hide();
            $('.TongCongHangHoa').hide();
            $('.Report_Empty').hide();
            $("div[id ^= 'wait']").text("");
        }
    }
    self.SelectListCuoiNgay_HangHoa = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListCuoiNgay_HangHoa?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReportCuoiNgay_HangHoa(data.LstData);
            self.RowsStart((_pageNumber - 1) * _pageSize + 1);
            self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportCuoiNgay_HangHoa().length);
            $("div[id ^= 'wait']").text("");
            LoadHtmlGrid(cacheExcelHH, 3, "hanghoaCN");
        });
    }
    var _maHangHoa;
    self.ReportCuoiNgay_HangHoaChiTiet = ko.observableArray();
    self.ReportCuoiNgay_HangHoaChiTietPrint = ko.observableArray();
    self.SumNumberPageReportHangHoa = ko.observableArray();
    self.SumRowsHangHoaHangHoa = ko.observable();
    self.HHCT_SoLuong = ko.observable();
    self.HHCT_TongTienHang = ko.observable();
    self.HHCT_GiamGiaHD = ko.observable();
    self.HHCT_DoanhThu = ko.observable();
    self.MaHangPrint = ko.observable();
    self.TenHangPrint = ko.observable();
    self.ThuocTinh_GiaTriPrint = ko.observable();
    self.TenDonViTinhPrint = ko.observable();
    self.getListCuoiNgay_HangHoaChiTiet = function (item) {
        self.HHCT_SoLuong(item.SoLuongBan - item.SoLuongTra);
        self.HHCT_TongTienHang(item.GiaTriBan + item.GiaTriTra);
        self.HHCT_GiamGiaHD(item.DoanhThuThuan - item.GiaTriBan - item.GiaTriTra);
        self.HHCT_DoanhThu(item.DoanhThuThuan);
        self.MaHangPrint(item.MaHangHoa);
        self.TenHangPrint(item.TenHangHoa);
        self.ThuocTinh_GiaTriPrint(item.ThuocTinh_GiaTri);
        self.TenDonViTinhPrint(item.TenDonViTinh);
        hidewait('table_h');
        _maHangHoa = item.MaHangHoa;
        ajaxHelper(ReportUri + "getListCuoiNgay_HangHoaChiTiet?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&maHH=" + _maHangHoa + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberHangHoa + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            console.log(data);
            self.ReportCuoiNgay_HangHoaChiTiet(data.LstData);
            self.ReportCuoiNgay_HangHoaChiTietPrint(data.LstDataPrint);
            if (self.ReportCuoiNgay_HangHoaChiTiet().length != 0) {
                self.RowsStartHangHoa((_pageNumberHangHoa - 1) * _pageSize + 1);
                self.RowsEndHangHoa((_pageNumberHangHoa - 1) * _pageSize + self.ReportCuoiNgay_HangHoaChiTiet().length)
            }
            else {
                self.RowsStartHangHoa('0');
                self.RowsEndHangHoa('0');
            }
            self.SumNumberPageReportHangHoa(data.LstPageNumber);
            AllPageHangHoa = self.SumNumberPageReportHangHoa().length;
            self.selecPageHangHoa();
            self.ReserPageHangHoa();
            self.SumRowsHangHoaHangHoa(data.Rowcount);
            $("div[id ^= 'wait']").text("");
        });
    }
    self.SelectedPageNumberReportNCC_HangHoaChiTiet = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListCuoiNgay_HangHoaChiTiet?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&maHH=" + _maHangHoa + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberHangHoa + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReportCuoiNgay_HangHoaChiTiet(data.LstData);
            self.RowsStartHangHoa((_pageNumberHangHoa - 1) * _pageSize + 1);
            self.RowsEndHangHoa((_pageNumberHangHoa - 1) * _pageSize + self.ReportCuoiNgay_HangHoaChiTiet().length)
            $("div[id ^= 'wait']").text("");
        });
    }

    self.ReportCuoiNgay_TongKetThuChi = ko.observableArray();
    self.TKTC_TienMat = ko.observable();
    self.TKTC_ChuyenKhoan = ko.observable();
    self.TKTC_The = ko.observable();
    self.TKTC_Diem = ko.observable();
    self.TKTC_TongThucThu = ko.observable();
    self.getListCuoiNgay_TongKetThuChi = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListCuoiNgay_TongKetThuChi?ID_NhanVien=" + _tenNguoiBanSeach + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReportCuoiNgay_TongKetThuChi(data.LstData);
            self.TKTC_TienMat(data.TongGiaVon);
            self.TKTC_ChuyenKhoan(data.DoanhThu);
            self.TKTC_The(data.DoanhThuThuan);
            self.TKTC_Diem(data.GiaTriTra);
            self.TKTC_TongThucThu(data.LoiNhuanGop);
            $("div[id ^= 'wait']").text("");
        });
    }

    self.ReportCuoiNgay_TongKetBanHang = ko.observableArray();
    self.TKBH_GiaTri = ko.observable();
    self.TKBH_TienMat = ko.observable();
    self.TKBH_ChuyenKhoan = ko.observable();
    self.TKBH_The = ko.observable();
    self.TKBH_Diem = ko.observable();
    self.TKBH_TongThucThu = ko.observable();
    self.getListCuoiNgay_TongKetBanHang = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListCuoiNgay_TongKetBanHang?ID_NhanVien=" + _tenNguoiBanSeach + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            console.log(data);
            self.ReportCuoiNgay_TongKetBanHang(data.LstData);
            self.TKBH_GiaTri(data.TongTienHang);
            self.TKBH_TienMat(data.TongGiaVon);
            self.TKBH_ChuyenKhoan(data.DoanhThu);
            self.TKBH_The(data.DoanhThuThuan);
            self.TKBH_Diem(data.GiaTriTra);
            self.TKBH_TongThucThu(data.LoiNhuanGop);
            $("div[id ^= 'wait']").text("");
        });
    }

    self.ReportCuoiNgay_SoLuongGiaoDich = ko.observableArray();
    self.SLGD_SoGiaoDich = ko.observable();
    self.SLGD_TienMat = ko.observable();
    self.SLGD_ChuyenKhoan = ko.observable();
    self.SLGD_The = ko.observable();
    self.SLGD_Diem = ko.observable();
    self.getListCuoiNgay_SoLuongGiaoDich = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListCuoiNgay_SoLuongGiaoDich?ID_NhanVien=" + _tenNguoiBanSeach + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            console.log(data);
            self.ReportCuoiNgay_SoLuongGiaoDich(data.LstData);
            self.SLGD_SoGiaoDich(data.TongTienHang);
            self.SLGD_TienMat(data.TongGiaVon);
            self.SLGD_ChuyenKhoan(data.DoanhThu);
            self.SLGD_The(data.DoanhThuThuan);
            self.SLGD_Diem(data.GiaTriTra);
            $("div[id ^= 'wait']").text("");
        });
    }
    self.ReportCuoiNgay_SoLuongHangHoa = ko.observableArray();
    self.CNHH_SoLuongMatHang = ko.observable();
    self.CNHH_SoLuongSanPham = ko.observable();
    self.getListCuoiNgay_SoLuongHangHoa = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListCuoiNgay_SoLuongHangHoa?ID_NhanVien=" + _tenNguoiBanSeach + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            console.log(data);
            self.ReportCuoiNgay_SoLuongHangHoa(data.LstData);
            self.CNHH_SoLuongMatHang(data.TongTienHang);
            self.CNHH_SoLuongSanPham(data.TongGiaVon);
            $("div[id ^= 'wait']").text("");
        });
    }
    //xuất excel
    //===============================
    // Load lai form lưu cache bộ lọc 
    // trên grid 
    //===============================
    function LoadHtmlGrid(cacheExcel, vals, caches) {
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
                            self.addColumBH(current[i].Value);
                        else if (vals === 2)
                            self.addColumTC(current[i].Value);
                        else if (vals === 3)
                            self.addColumHH(current[i].Value);

                    }
                }
                if (vals === 1)
                    cacheExcelBH = false;
                else if (vals === 2)
                    cacheExcelTC = false;
                else if (vals === 3)
                    cacheExcelHH = false;
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
    $('#bhma').click(function () {
        $(".bhma").toggle();
        addClass(".bhma", "bhma", $(this).val(), "banhangCN");
    });
    $('#bhsanpham').click(function () {
        $(".bhsanpham").toggle();
        addClass(".bhsanpham", "bhsanpham", $(this).val(), "banhangCN");
    });
    $('#bhtongtien').click(function () {
        $(".bhtongtien").toggle();
        addClass(".bhtongtien", "bhtongtien", $(this).val(), "banhangCN");
    });
    $('#bhgiamgia').click(function () {
        $(".bhgiamgia").toggle();
        addClass(".bhgiamgia", "bhgiamgia", $(this).val(), "banhangCN");
    });
    $('#bhdoanhthu').click(function () {
        $(".bhdoanhthu").toggle();
        addClass(".bhdoanhthu", "bhdoanhthu", $(this).val(), "banhangCN");
    });
    $('#bhphi').click(function () {
        $(".bhphi").toggle();
        addClass(".bhphi", "bhphi", $(this).val(), "banhangCN");
    });
    $('#bhthucthu').click(function () {
        $(".bhthucthu").toggle();
        addClass(".bhthucthu", "bhthucthu", $(this).val(), "banhangCN");
    });

    $('#tcma').click(function () {
        $(".tcma").toggle();
        self.addColumTC($(this).val())
        addClass(".tcma", "tcma", $(this).val(), "thuchiCN");
    });
    $('#tcnop').click(function () {
        $(".tcnop").toggle();
        self.addColumTC($(this).val())
        addClass(".tcnop", "tcnop", $(this).val(), "thuchiCN");
    });
    $('#tctime').click(function () {
        $(".tctime").toggle();
        self.addColumTC($(this).val())
        addClass(".tctime", "tctime", $(this).val(), "thuchiCN");
    });
    $('#tcthuchi').click(function () {
        $(".tcthuchi").toggle();
        self.addColumTC($(this).val())
        addClass(".tcthuchi", "tcthuchi", $(this).val(), "thuchiCN");
    });
    $('#tcmact').click(function () {
        $(".tcmact").toggle();
        self.addColumTC($(this).val())
        addClass(".tcmact", "tcmact", $(this).val(), "thuchiCN");
    });

    $('#hhma').click(function () {
        $(".hhma").toggle();
        self.addColumHH($(this).val())
        addClass(".hhma", "hhma", $(this).val(), "hanghoaCN");
    });
    $('#hhname').click(function () {
        $(".hhname").toggle();
        self.addColumHH($(this).val())
        addClass(".hhname", "hhname", $(this).val(), "hanghoaCN");
    });
    $('#hhsoluongban ').click(function () {
        $(".hhsoluongban ").toggle();
        self.addColumHH($(this).val())
        addClass(".hhsoluongban", "hhsoluongban", $(this).val(), "hanghoaCN");
    });
    $('#hhgiatriban ').click(function () {
        $(".hhgiatriban ").toggle();
        self.addColumHH($(this).val())
        addClass(".hhgiatriban", "hhgiatriban", $(this).val(), "hanghoaCN");
    });
    $('#hhsoluongtra ').click(function () {
        $(".hhsoluongtra ").toggle();
        self.addColumHH($(this).val())
        addClass(".hhsoluongtra", "hhsoluongtra", $(this).val(), "hanghoaCN");
    });
    $('#hhgiatritra ').click(function () {
        $(".hhgiatritra ").toggle();
        self.addColumHH($(this).val())
        addClass(".hhgiatritra", "hhgiatritra", $(this).val(), "hanghoaCN");
    });
    $('#hhdoanhthu ').click(function () {
        $(".hhdoanhthu ").toggle();
        self.addColumHH($(this).val())
        addClass(".hhdoanhthu", "hhdoanhthu", $(this).val(), "hanghoaCN");
    });

    self.addColumBH = function (item) {
        if (self.ColumnsExcelBH().length < 1) {
            self.ColumnsExcelBH.push(item);
        }
        else {
            for (var i = 0; i < self.ColumnsExcelBH().length; i++) {
                if (self.ColumnsExcelBH()[i] === item) {
                    self.ColumnsExcelBH.splice(i, 1);
                    break;
                }
                if (i == self.ColumnsExcelBH().length - 1) {
                    self.ColumnsExcelBH.push(item);
                    break;
                }
            }
        }
        self.ColumnsExcelBH.sort();
    }
    self.addColumTC = function (item) {
        if (self.ColumnsExcelTC().length < 1) {
            self.ColumnsExcelTC.push(item);
        }
        else {
            for (var i = 0; i < self.ColumnsExcelTC().length; i++) {
                if (self.ColumnsExcelTC()[i] === item) {
                    self.ColumnsExcelTC.splice(i, 1);
                    break;
                }
                if (i == self.ColumnsExcelTC().length - 1) {
                    self.ColumnsExcelTC.push(item);
                    break;
                }
            }
        }
        self.ColumnsExcelTC.sort();
    }
    self.addColumHH = function (item) {
        if (self.ColumnsExcelHH().length < 1) {
            self.ColumnsExcelHH.push(item);
        }
        else {
            for (var i = 0; i < self.ColumnsExcelHH().length; i++) {
                if (self.ColumnsExcelHH()[i] === item) {
                    self.ColumnsExcelHH.splice(i, 1);
                    break;
                }
                if (i == self.ColumnsExcelHH().length - 1) {
                    self.ColumnsExcelHH.push(item);
                    break;
                }
            }
        }
        self.ColumnsExcelHH.sort();
    }
    // load table
    self.loadTable_Grid = function (itemHoa) {
        console.log(itemHoa);
        for (var i = 0; i < itemHoa.length; i++) {
            var a = itemHoa[i].classCL;
            $(a).addClass("operation");
        }
    }
    self.ExportExcel = function () {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo cuối ngày",
            NoiDung: "Xuất " + self.MoiQuanTam() + ", ngày bán: " + self.TodayBC(),
            NoiDungChiTiet: "Xuất " + self.MoiQuanTam() + ", ngày bán: " + self.TodayBC(),
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
                if (_kieubang == 1 && self.ReportCuoiNgay_BanHang().length != 0) {
                    if (self.BCCN_BanHang() != "BCCN_BanHang" || self.BCCN_BanHang_XuatFile() != "BCCN_BanHang_XuatFile") {
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelBH().length; i++) {
                            if (i == 0) {
                                columnHide = self.ColumnsExcelBH()[i];
                            }
                            else {
                                columnHide = self.ColumnsExcelBH()[i] + "_" + columnHide;
                            }
                        }
                        var url = ReportUri + "ExportExcelCuoiNgay?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                        window.location.href = url;
                    }
                }
                else if (_kieubang == 2 && self.ReportCuoiNgay_ThuChi().length != 0) {
                    if (self.BCCN_ThuChi() != "BCCN_ThuChi" || self.BCCN_ThuChi_XuatFile != "BCCN_ThuChi_XuatFile") {
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelTC().length; i++) {
                            if (i == 0) {
                                columnHide = self.ColumnsExcelTC()[i];
                            }
                            else {
                                columnHide = self.ColumnsExcelTC()[i] + "_" + columnHide;
                            }
                        }
                        var url = ReportUri + "ExportExcelCuoiNgay_ThuChi?loaithuchi=" + _tenLoaiThuChiSeach + "&ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                        window.location.href = url;
                    }
                }
                else if (_kieubang == 3 && self.ReportCuoiNgay_HangHoa().length != 0) {
                    if (self.BCCN_HangHoa() != "BCCN_HangHoa" || self.BCCN_HangHoa_XuatFile() != "BCCN_HangHoa_XuatFile") {
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                    }
                    else {
                        for (var i = 0; i < self.ColumnsExcelHH().length; i++) {
                            if (i == 0) {
                                columnHide = self.ColumnsExcelHH()[i];
                            }
                            else {
                                columnHide = self.ColumnsExcelHH()[i] + "_" + columnHide;
                            }
                        }
                        var url = ReportUri + "ExportExcelCuoiNgay_HangHoa?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                        window.location.href = url;
                    }
                }
                else {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Không có dữ liệu để xuất file excel", "danger");
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
    self.ExportChiTietHangHoa = function () {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo cuối ngày",
            NoiDung: "Xuất báo cáo cuối ngày theo chi tiết hàng hóa, Mã hàng: " + _maHangHoa,
            NoiDungChiTiet: "Xuất báo cáo cuối ngày theo chi tiết hàng hóa, Mã hàng: " + _maHangHoa,
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
                var url = ReportUri + "ExportExcelCuoiNgay_HangHoaChiTiet?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&maHH=" + _maHangHoa + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                window.location.href = url;
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
}
ko.applyBindings(new ViewModel());
function hidewait(o) {
    $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /></div>')
}