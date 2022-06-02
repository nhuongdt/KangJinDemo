var ViewModel = function () {
    var self = this;
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var NhanVienUri = '/api/DanhMuc/NS_NhanVienAPI/';
    var BH_KhuyenMaiUri = '/api/DanhMuc/BH_KhuyenMaiAPI/';
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    var _id_NhanVien = $('.idnhanvien').text();
    var _id_DonVi = $('#hd_IDdDonVi').val();
    self.TenChiNhanh = ko.observable($('#_txtTenDonVi').text());
    //self.TenChiNhanh = ko.observable($('.branch label').text());
    console.log(self.TenChiNhanh());
    $('#txtNoiDung').focus();
    var _tenNguoiBanSeach;
    var _noidung = null;
    var _chucnang = null;
    self.ListLichSuThaoTac = ko.observableArray();
    self.SumNumberPageReport = ko.observableArray();
    self.SumRowsHangHoa = ko.observable();
    self.HangHoa_XemGiaVon = ko.observable();
    self.NhatKy_XemDS_PhongBan = ko.observable();
    self.NhatKy_XemDS_HeThong = ko.observable();
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var _IDDoiTuong = $('.idnguoidung').text();
    var _pageNumber = 1;
    var _pageSize = 10;
    var AllPage;
    var thisDateTo;
    var thisDateFrom;
    var _thaotac1 = null;
    var _thaotac2 = null;
    var _thaotac3 = null;
    var _thaotac4 = null;
    var _thaotac5 = null;
    var _thaotac6 = null;
    var _thaotac7 = null;
    self.RowsStart = ko.observable('1');
    self.RowsEnd = ko.observable('10');
    self.MangNguoiBan = ko.observableArray();
    self.searchNguoiban = ko.observableArray()
    self.NguoiBans = ko.observableArray();
    self.filterNgayPhieuHuy = ko.observable('0');
    var dt1 = new Date();
    var _timeStart = dt1.getFullYear() + "-" + (dt1.getMonth() + 1) + "-" + dt1.getDate();
    var _timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 1))).format('YYYY-MM-DD');
    self.TodayBC = ko.observable('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
    //self.TenChiNhanh = ko.observable($('.branch label').text());
    self.CheckTime = ko.observable('1');
    self.pageSizes = [10, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(1);
    self.ResetCurrentPage = function () {
        _pageSize = self.pageSize();
        _pageNumber = 1;
        
        self.currentPage(1);
        self.getList_LichSuThaoTac();
    };
    // phân quyền
    function loadQuyenIndex() {
        //quyền xem hệ thống
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "NhatKy_XemDS_HeThong", "GET").done(function (data) {
            self.NhatKy_XemDS_HeThong(data);
            console.log(data);
            getAllNSNhanVien();
        })
        // quyền xem phòng ban
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "NhatKy_XemDS_PhongBan", "GET").done(function (data) {
            self.NhatKy_XemDS_PhongBan(data);
            console.log(data);
        })
        
    }
    loadQuyenIndex();


    self.SelectChucNang = function () {
        _chucnang = $('#txtChucNang').val();
        console.log(_chucnang);
    }
    $('#txtChucNang').keypress(function (e) {
        if (e.keyCode == 13) {
            self.getList_LichSuThaoTac();
        }
    })
    self.SelectNoiDung = function () {
        _noidung = $('#txtNoiDung').val();
        console.log(_noidung);
    }
    $('#txtNoiDung').keypress(function (e) {
        if (e.keyCode == 13) {
            self.getList_LichSuThaoTac();
        }
    });
    self.ClickIconSearch = function () {
        self.getList_LichSuThaoTac();
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
            //console.log(self.NguoiBans())
        }

    }
    $('#NoteNameNguoiBan').keypress(function (e) {
        if (e.keyCode == 13 && self.NguoiBans().length > 0) {
            self.SelectedNguoiBan(self.NguoiBans()[0]);
        }
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
        // remove check
        $('#selec-all-NguoiBan li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        console.log(self.MangNguoiBan())
        _pageNumber = 1;
        self.getList_LichSuThaoTac();

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

        $('#NoteNameNguoiBan').val('');
        self.NguoiBans(self.searchNguoiban());
        //đánh dấu check
        for (var i = 0; i < self.MangNguoiBan().length; i++) {
            $('#selec-all-NguoiBan li').each(function () {
                if ($(this).attr('id') === self.MangNguoiBan()[i].ID) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>')
                }
            });
        }
        console.log(_tenNguoiBanSeach)
        _pageNumber = 1;
        self.getList_LichSuThaoTac();
    }
    $('.choose_Time input').on('click', function () {
        _pageNumber = 1;
        $(".op-filter-container ul li").css("display", "none");
        $(".open-li").show();
        $(".close-li").hide();
        $(".op-filter-container ul li").eq(0).css("display", "block");
        $(".op-filter-container ul li").eq(1).css("display", "block");
        self.CheckTime($(this).val());
        var datime = new Date();
        if ($(this).val() == 1) {
            _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            self.getList_LichSuThaoTac();
        }
        else if ($(this).val() == 2) {
            var currentWeekDay = datime.getDay();
            var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
            self.getList_LichSuThaoTac();
        }
        else if ($(this).val() == 3) {
            _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
            self.getList_LichSuThaoTac();
        }
        else {
            _timeStart = '2015-09-26'
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            self.getList_LichSuThaoTac();
        }
    })
    $('.hideCheck').on('click', function () {
        self.CheckTime(1);
    });
    $('.showCheck').on('click', function () {
        self.CheckTime('1');
        _timeStart = dt1.getFullYear() + "-" + (dt1.getMonth() + 1) + "-" + dt1.getDate();
        _timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 1))).format('YYYY-MM-DD');
    });
    $('#txtDateTo').on('dp.change', function (e) {
        thisDateTo = $(this).val();
        var t = thisDateTo.split(" ");
        var t1 = t[0].split("/").reverse().join("-")
        thisDateTo = moment(t1).format('MM/DD/YYYY')
        console.log(thisDateTo);
        _timeStart = moment(new Date(thisDateTo)).format('YYYY-MM-DD');
        //var dt = new Date(thisDateTo);
        //_timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
        //_pageNumber = 1;

    });
    $('#txtDateFrom').on('dp.change', function (e) {
        thisDateFrom = $(this).val();
        var t = thisDateFrom.split(" ");
        var t1 = t[0].split("/").reverse().join("-")
        thisDateFrom = moment(t1).format('MM/DD/YYYY')
        console.log(thisDateFrom);
        _timeEnd = moment(new Date(thisDateFrom)).format('YYYY-MM-DD');
        //var dt = new Date(thisDateFrom);
        //_timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
        //_pageNumber = 1;

    });
    self.filterLichSuThaoTac = function () {
        if (new Date(_timeStart) > new Date(_timeEnd)) {
            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Ngày bắt đầu không được lớn hơn ngày kết thúc", "danger");
        }
        else {
            self.getList_LichSuThaoTac();
        }
    }
    var thaotac = "1,2,3,4,5,6,7";
    $('#checkThaoTac1').on('click', function () {
        if (_thaotac1 == null)
            _thaotac1 = 1;
        else
            _thaotac1 = null;
        console.log(_thaotac1);
        if (_thaotac1 == null & _thaotac2 == null & _thaotac3 == null & _thaotac4 == null & _thaotac5 == null & _thaotac6 == null & _thaotac7 == null) {
            thaotac = "1,2,3,4,5,6,7";
        }
        else {
            thaotac = _thaotac1 + "," + _thaotac2 + "," + _thaotac3 + "," + _thaotac4 + "," + _thaotac5 + "," + _thaotac6 + "," + _thaotac7;
        }
        self.getList_LichSuThaoTac();
    });
    $('#checkThaoTac2').on('click', function () {
        if (_thaotac2 == null)
            _thaotac2 = 2;
        else
            _thaotac2 = null;
        if (_thaotac1 == null & _thaotac2 == null & _thaotac3 == null & _thaotac4 == null & _thaotac5 == null & _thaotac6 == null & _thaotac7 == null) {
            thaotac = "1,2,3,4,5,6,7";
        }
        else {
            thaotac = _thaotac1 + "," + _thaotac2 + "," + _thaotac3 + "," + _thaotac4 + "," + _thaotac5 + "," + _thaotac6 + "," + _thaotac7;
        }
        self.getList_LichSuThaoTac();
    });
    $('#checkThaoTac3').on('click', function () {
        if (_thaotac3 == null)
            _thaotac3 = 3;
        else
            _thaotac3 = null;
        if (_thaotac1 == null & _thaotac2 == null & _thaotac3 == null & _thaotac4 == null & _thaotac5 == null & _thaotac6 == null & _thaotac7 == null) {
            thaotac = "1,2,3,4,5,6,7";
        }
        else {
            thaotac = _thaotac1 + "," + _thaotac2 + "," + _thaotac3 + "," + _thaotac4 + "," + _thaotac5 + "," + _thaotac6 + "," + _thaotac7;
        }
        self.getList_LichSuThaoTac();
    });
    $('#checkThaoTac4').on('click', function () {
        if (_thaotac4 == null)
            _thaotac4 = 4;
        else
            _thaotac4 = null;
        if (_thaotac1 == null & _thaotac2 == null & _thaotac3 == null & _thaotac4 == null & _thaotac5 == null & _thaotac6 == null & _thaotac7 == null) {
            thaotac = "1,2,3,4,5,6,7";
        }
        else {
            thaotac = _thaotac1 + "," + _thaotac2 + "," + _thaotac3 + "," + _thaotac4 + "," + _thaotac5 + "," + _thaotac6 + "," + _thaotac7;
        }
        self.getList_LichSuThaoTac();
    });
    $('#checkThaoTac5').on('click', function () {
        if (_thaotac5 == null)
            _thaotac5 = 5;
        else
            _thaotac5 = null;
        if (_thaotac1 == null & _thaotac2 == null & _thaotac3 == null & _thaotac4 == null & _thaotac5 == null & _thaotac6 == null & _thaotac7 == null) {
            thaotac = "1,2,3,4,5,6,7";
        }
        else {
            thaotac = _thaotac1 + "," + _thaotac2 + "," + _thaotac3 + "," + _thaotac4 + "," + _thaotac5 + "," + _thaotac6 + "," + _thaotac7;
        }
        self.getList_LichSuThaoTac();
    });
    $('#checkThaoTac6').on('click', function () {
        if (_thaotac6 == null)
            _thaotac6 = 6;
        else
            _thaotac6 = null;
        if (_thaotac1 == null & _thaotac2 == null & _thaotac3 == null & _thaotac4 == null & _thaotac5 == null & _thaotac6 == null & _thaotac7 == null) {
            thaotac = "1,2,3,4,5,6,7";
        }
        else {
            thaotac = _thaotac1 + "," + _thaotac2 + "," + _thaotac3 + "," + _thaotac4 + "," + _thaotac5 + "," + _thaotac6 + "," + _thaotac7;
        }
        self.getList_LichSuThaoTac();
    });
    $('#checkThaoTac7').on('click', function () {
        if (_thaotac7 == null)
            _thaotac7 = 7;
        else
            _thaotac7 = null;
        if (_thaotac1 == null & _thaotac2 == null & _thaotac3 == null & _thaotac4 == null & _thaotac5 == null & _thaotac6 == null & _thaotac7 == null) {
            thaotac = "1,2,3,4,5,6,7";
        }
        else {
            thaotac = _thaotac1 + "," + _thaotac2 + "," + _thaotac3 + "," + _thaotac4 + "," + _thaotac5 + "," + _thaotac6 + "," + _thaotac7;
        }
        self.getList_LichSuThaoTac();
    });
    function getAllNSNhanVien() {
        ajaxHelper(BH_KhuyenMaiUri + "getNhanViens?nameChinhanh=" + _id_DonVi, 'GET').done(function (data) {
            self.NguoiBans(data);
            //console.log(data);
            self.searchNguoiban(data);
            for (var i = 0; i < self.NguoiBans().length; i++) {
                if (i == 0) {
                    _tenNguoiBanSeach = self.NguoiBans()[i].ID;
                }
                else {
                    _tenNguoiBanSeach = self.NguoiBans()[i].ID + "," + _tenNguoiBanSeach;
                }
            }
            self.getList_LichSuThaoTac();
            //console.log(_tenNguoiBanSeach)
        });

    }
    //getAllNSNhanVien();
    self.getList_LichSuThaoTac = function () {
        _pageNumber = 1;
        $('.table-reponsive').gridLoader();
        var array_Seach = {
            ID_NhanVien: _tenNguoiBanSeach,
            ID_ChiNhanh: _id_DonVi,
            NoiDung: _noidung,
            ChucNang: _chucnang,
            timeStart: _timeStart,
            timeEnd: _timeEnd,
            ThaoTac: thaotac,
            pageNumber: _pageNumber,
            pageSize: _pageSize,
            XemDS_HeThong: self.NhatKy_XemDS_HeThong(),
            XemDS_PhongBan: self.NhatKy_XemDS_PhongBan(),
            ID_NguoiDung: _IDDoiTuong
        }
        console.log(array_Seach)
        ajaxHelper(ReportUri + "getList_LichSuThaoTac", "POST", array_Seach).done(function (data) {
            console.log(data);
            for (var i = 0; i < data.LstData.length; i++) {
                if (data.LstData[i].NoiDung.length > 100) {
                    var tr = data.LstData[i].NoiDung.substr(0, 105);
                    var mangTr = tr.split(" ");
                    var chuoi = mangTr[0];
                    for (var j = 1; j < mangTr.length - 1; j++) {
                        chuoi = chuoi + " " + mangTr[j];
                    }
                    data.LstData[i].NoiDung_MR = chuoi + "..."
                }
                else {
                    data.LstData[i].NoiDung_MR = data.LstData[i].NoiDung;
                }
            }
            self.ListLichSuThaoTac(data.LstData)
            console.log(data.LstData);
            self.ListLichSuThaoTac(data.LstData);
            if (self.ListLichSuThaoTac().length != 0) {
                self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                self.RowsEnd((_pageNumber - 1) * _pageSize + self.ListLichSuThaoTac().length)
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
            $('.table-reponsive').gridLoader({ show: false });
            $("div[id ^= 'wait']").text("");
        });
    };
    self.SelectPage_LichSuThaoTac = function () {
        $('.table-reponsive').gridLoader();
        var array_Seach = {
            ID_NhanVien: _tenNguoiBanSeach,
            ID_ChiNhanh: _id_DonVi,
            NoiDung: _noidung,
            ChucNang: _chucnang,
            timeStart: _timeStart,
            timeEnd: _timeEnd,
            ThaoTac: thaotac,
            pageNumber: _pageNumber,
            pageSize: _pageSize,
            XemDS_HeThong: self.NhatKy_XemDS_HeThong(),
            XemDS_PhongBan: self.NhatKy_XemDS_PhongBan(),
            ID_NguoiDung: _IDDoiTuong
        }
        ajaxHelper(ReportUri + "getList_LichSuThaoTac", "POST", array_Seach).done(function (data) {
            for (var i = 0; i < data.LstData.length; i++) {
                if (data.LstData[i].NoiDung.length > 100) {
                    var tr = data.LstData[i].NoiDung.substr(0, 105);
                    var mangTr = tr.split(" ");
                    var chuoi = mangTr[0];
                    for (var j = 1; j < mangTr.length - 1; j++) {
                        chuoi = chuoi + " " + mangTr[j];
                    }
                    data.LstData[i].NoiDung_MR = chuoi + "..."
                }
                else {
                    data.LstData[i].NoiDung_MR = data.LstData[i].NoiDung;
                }
            }
            self.ListLichSuThaoTac(data.LstData)
            self.RowsStart((_pageNumber - 1) * _pageSize + 1);
            self.RowsEnd((_pageNumber - 1) * _pageSize + self.ListLichSuThaoTac().length)
            $("div[id ^= 'wait']").text("");
            $('.table-reponsive').gridLoader({ show: false });
        });

    };
    //Phân trang
    self.GetClass = function (page) {
        return (page.SoTrang === self.currentPage()) ? "click" : "";
    };
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
            self.SelectPage_LichSuThaoTac();
            self.ReserPage();

        }
    };
    self.BackPage = function (item) {
        if (_pageNumber > 1) {
            _pageNumber = _pageNumber - 1;
            self.SelectPage_LichSuThaoTac();
            self.ReserPage();

        }
    };
    self.EndPage = function (item) {
        _pageNumber = AllPage;
        self.SelectPage_LichSuThaoTac();
        self.ReserPage();

    };
    self.StartPage = function (item) {
        _pageNumber = 1;
        self.SelectPage_LichSuThaoTac();
        self.ReserPage();

    };
    self.gotoNextPage = function (item) {
        _pageNumber = item.SoTrang;
        self.SelectPage_LichSuThaoTac();
        self.ReserPage();

    }
    $('.ip_DateReport').attr('disabled', 'false');
    $('.choose_TimeReport input').on('click', function () {
        _rdTime = $(this).val()
        if ($(this).val() == 0) {
            $('.ip_TimeReport').removeAttr('disabled');
            $('.dr_TimeReport').attr("data-toggle", "dropdown");
            $('.ip_DateReport').attr('disabled', 'false');
            var _rdoNgayPage = $('.ip_TimeReport').val();
            console.log(_rdoNgayPage);
            var datime = new Date();
            var datimeBC = new Date();
            //Toàn thời gian
            if (_rdoNgayPage === "Toàn thời gian") {
                _timeStart = '2015-09-26'
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                self.TodayBC('Ngày bán: Toàn thời gian');
            }
            //Hôm nay
            else if (_rdoNgayPage === "Hôm nay") {
                _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                self.TodayBC('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
            }
            //Hôm qua
            else if (_rdoNgayPage === "Hôm qua") {
                var dt1 = new Date();
                var dt2 = new Date();
                _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
                _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
                self.TodayBC('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
            }
            //Tuần này
            else if (_rdoNgayPage === "Tuần này") {
                var currentWeekDay = datime.getDay();
                var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
                var _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate() + 6))).format('YYYY-MM-DD');
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
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
                var _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate()))).format('YYYY-MM-DD');
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));

            }
            //Tháng này
            else if (_rdoNgayPage === "Tháng này") {
                _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
                var dtBC = new Date(_timeEnd);
                var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
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
            self.currentPage(1);
            self.getList_LichSuThaoTac();
        }
        // lựa chọn daterange
        else if ($(this).val() == 1) {
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
                _pageNumber = 1;
                self.currentPage(1);
                self.getList_LichSuThaoTac();
            }
        }
    })
    $('.newDateTime').on('apply.daterangepicker', function (ev, picker) {
        debugger;
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        var dt = new Date(picker.endDate.format('MM/DD/YYYY'));
        var dtBC = new Date(picker.endDate.format('MM/DD/YYYY'));
        _timeStart = picker.startDate.format('YYYY-MM-DD');
        _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
        var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate()))).format('YYYY-MM-DD');
        if (_timeStart == _timeBC)
            self.TodayBC('Ngày tạo: ' + moment(_timeStart).format('DD/MM/YYYY'));
        else
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        self.currentPage(1);
        if (self.filterNgayPhieuHuy() === '1') {
            _pageNumber = 1;
            self.getList_LichSuThaoTac();
        }
    });
    $('.choseNgayTaoPhieuHuy li').on('click', function () {
        var _rdoNgayPage = $(this).val();
        var datimeBC = new Date();
        _pageNumber = 1;
        var datime = new Date();
        self.currentPage(1);
        if (_rdoNgayPage === 17) {
            _timeStart = '2015-09-26'
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            self.TodayBC('Ngày bán: Toàn thời gian');
        }
        //Hôm nay
        else if (_rdoNgayPage === 1) {
            _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            self.TodayBC('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
        }
        //Hôm qua
        else if (_rdoNgayPage === 2) {
            var dt1 = new Date();
            var dt2 = new Date();
            _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
            _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
            self.TodayBC('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
        }
        //Tuần này
        else if (_rdoNgayPage === 3) {
            var currentWeekDay = datime.getDay();
            var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
            var _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate() + 6))).format('YYYY-MM-DD');
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //Tuần trước
        else if (_rdoNgayPage === 4) {
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
            var _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate() - datimeBC.getDay()))).format('YYYY-MM-DD');
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //7 ngày qua
        else if (_rdoNgayPage === 5) {
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
            var newtime = new Date();
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            var _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate()))).format('YYYY-MM-DD');
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
        else if (_rdoNgayPage === 10) {
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
            var newtime = new Date();
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            var dtBC = new Date(_timeEnd);
            var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //Quý này
        else if (_rdoNgayPage === 11) {
            _timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
            var newtime = new Date(moment().endOf('quarter'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            var dtBC = new Date(_timeEnd);
            var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        // Quý trước
        else if (_rdoNgayPage === 12) {
            var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
            _timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
            var newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            var dtBC = new Date(_timeEnd);
            var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //Năm này
        else if (_rdoNgayPage === 13) {
            _timeStart = moment().startOf('year').format('YYYY-MM-DD');
            var newtime = new Date(moment().endOf('year'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            var dtBC = new Date(_timeEnd);
            var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //năm trước
        else if (_rdoNgayPage === 14) {
            var prevYear = moment().year() - 1;
            _timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
            var newtime = new Date(moment().year(prevYear).endOf('year'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            var dtBC = new Date(_timeEnd);
            var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        self.getList_LichSuThaoTac();
    });
    //Download file excel format (*.xlsx)
    self.DownloadFileTeamplateXLSX = function (item) {
        var url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + item;
        window.location.href = url;
    }
    // xuất excel
    self.exportToExcelHangHoas = function () {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Lịch sử thao tác",
            NoiDung: "Xuất báo cáo chi tiết lịch sử thao tác",
            NoiDungChiTiet: "Xuất báo cáo chi tiết lịch sử thao tác",
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
                var array_Seach = {
                    ID_NhanVien: _tenNguoiBanSeach,
                    ID_ChiNhanh: _id_DonVi,
                    NoiDung: _noidung,
                    ChucNang: _chucnang,
                    timeStart: _timeStart,
                    timeEnd: _timeEnd,
                    ThaoTac: thaotac,
                    columnsHide: columnHide,
                    txtTime: self.TodayBC(),
                    nameChiNhanh: self.TenChiNhanh(),
                    XemDS_HeThong: self.NhatKy_XemDS_HeThong(),
                    XemDS_PhongBan: self.NhatKy_XemDS_PhongBan(),
                    ID_NguoiDung: _IDDoiTuong
                }
                console.log(array_Seach);
                $.ajax({
                    type: "POST",
                    dataType: "json",
                    url: ReportUri + "ExportExcel_LichSuThaoTac",
                    data: { objExcel: array_Seach},
                    success: function (url) {
                        self.DownloadFileTeamplateXLSX(url)
                    }
                });
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
    $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /><div class="happy-wait">' +
        ' </div>' +
        '</div>')
}