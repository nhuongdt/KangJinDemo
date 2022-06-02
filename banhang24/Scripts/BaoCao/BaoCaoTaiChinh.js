var ViewModel = function () {
    var kieu_time = "tháng"
    var self = this;
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    var _id_NhanVien = $('.idnhanvien').text();
    var _id_DonVi = $('#hd_IDdDonVi').val();
    self.MangChiNhanh = ko.observableArray();
    self.MangChiNhanh_BieuDo = ko.observableArray();
    self.DonVis = ko.observableArray();
    self.searchDonVi = ko.observableArray()
    var _idDonViSeach = $('#hd_IDdDonVi').val();
    var _nameDonViSeach = null;
    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/'
    var _IDDoiTuong = $('.idnguoidung').text();
    var _year;
    self.TodayBC1 = ko.observable("Theo tháng");
    self.TenChiNhanh = ko.observable($('.branch label').text());
    $('#home').removeClass("active")
    $('#info').addClass("active")
    self.check_kieubang = ko.observable('2');
    self.Loc_TonKho = ko.observable('1')
    //trinhpv phân quyền
    self.BCTaiChinh = ko.observable();
    self.BCTC_TheoNam = ko.observable();
    self.BCTC_TheoNam_XuatFile = ko.observable();
    self.BCTC_TheoQuy = ko.observable();
    self.BCTC_TheoQuy_XuatFile = ko.observable();
    self.BCTC_TheoThang = ko.observable();
    self.BCTC_TheoThang_XuatFile = ko.observable();
    function getQuyen_NguoiDung() {
        //quyền xem báo cáo
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCTaiChinh", "GET").done(function (data) {
            self.BCTaiChinh(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCTC_TheoNam", "GET").done(function (data) {
            self.BCTC_TheoNam(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCTC_TheoNam_XuatFile", "GET").done(function (data) {
            self.BCTC_TheoNam_XuatFile(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCTC_TheoQuy", "GET").done(function (data) {
            self.BCTC_TheoQuy(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCTC_TheoQuy_XuatFile", "GET").done(function (data) {
            self.BCTC_TheoQuy_XuatFile(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCTC_TheoThang", "GET").done(function (data) {
            self.BCTC_TheoThang(data);
            self.getListTaiChinh_TheoThang();
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCTC_TheoThang_XuatFile", "GET").done(function (data) {
            self.BCTC_TheoThang_XuatFile(data);
        })
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
        self.MangChiNhanh_BieuDo(self.MangChiNhanh());
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
            self.MangChiNhanh_BieuDo(self.searchDonVi());
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
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListTaiChinh_TheoThang_BieuDo();
            else
                self.getListTaiChinh_TheoThang();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListTaiChinh_TheoQuy_BieuDo();
            else
                self.getListTaiChinh_TheoQuy();
        }
        else if (_kieubang == 3) {
            if (self.check_kieubang() == 1)
                self.getListTaiChinh_TheoNam_BieuDo();
            else
                self.getListTaiChinh_TheoNam();
        }

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
            if (_year != undefined) {
                if (_kieubang == 1) {
                    if (self.check_kieubang() == 1)
                        self.getListTaiChinh_TheoThang_BieuDo();
                    else
                        self.getListTaiChinh_TheoThang();
                }
                else if (_kieubang == 2) {
                    if (self.check_kieubang() == 1)
                        self.getListTaiChinh_TheoQuy_BieuDo();
                    else
                        self.getListTaiChinh_TheoQuy();
                }
                else if (_kieubang == 3) {
                    if (self.check_kieubang() == 1)
                        self.getListTaiChinh_TheoNam_BieuDo();
                    else
                        self.getListTaiChinh_TheoNam();
                }
            }
        }
        self.MangChiNhanh_BieuDo(self.MangChiNhanh());
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
            if (self.check_kieubang() == 1)
                self.getListTaiChinh_TheoThang_BieuDo();
            else
                self.getListTaiChinh_TheoThang();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListTaiChinh_TheoQuy_BieuDo();
            else
                self.getListTaiChinh_TheoQuy();
        }
        else if (_kieubang == 3) {
            if (self.check_kieubang() == 1)
                self.getListTaiChinh_TheoNam_BieuDo();
            else
                self.getListTaiChinh_TheoNam();
        }
    })
    var _kieubang = 1;
    $('.chose_Time input').on('click', function () {
        self.hideTableReport();
        _kieubang = $(this).val();
        if ($(this).val() == 1) {
            kieu_time = "tháng";
            self.TodayBC1("Theo tháng");
            $('.table_TheoThang').show();
            if (self.check_kieubang() == 1)
                self.getListTaiChinh_TheoThang_BieuDo();
            else
                self.getListTaiChinh_TheoThang();
        }
        else if ($(this).val() == 2) {
            kieu_time = "quý";
            self.TodayBC1("Theo quý");
            $('.table_TheoQuy').show();
            if (self.check_kieubang() == 1)
                self.getListTaiChinh_TheoQuy_BieuDo();
            else
                self.getListTaiChinh_TheoQuy();
        }
        else if ($(this).val() == 3) {
            kieu_time = "năm";
            self.TodayBC1("Theo năm");
            $('.table_TheoNam').show();
            if (self.check_kieubang() == 1)
                self.getListTaiChinh_TheoNam_BieuDo();
            else
                self.getListTaiChinh_TheoNam();
        }
    })
    self.hideTableReport = function () {
        $('.table_TheoThang').hide();
        $('.table_TheoQuy').hide();
        $('.table_TheoNam').hide();
    }
    self.hideTableReport();
    $('.table_TheoThang').show();
    self.newYear = ko.observable();
    self.listReportTaiChinh_TheoThang = ko.observableArray();
    $(".PhanQuyen").hide();
    self.getListTaiChinh_TheoThang = function () {
        if (self.BCTC_TheoThang() == "BCTC_TheoThang")
        {
            $(".PhanQuyen").hide();
            hidewait('table_h');
            ajaxHelper(ReportUri + "getListTaiChinh_TheoThang?year=" + _year + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
                for (var i = 0; i < data.length; i++) {
                    data[i].Padding33 = "";
                    data[i].ColorText = "text-right itemRight";
                    if (i === 2 || i === 3 || i === 8 || i == 9 || i == 10 || i == 13 || i == 14 || i == 15) {
                        data[i].Padding33 = "mahang padding33";
                        data[i].ColorText = "text-right color_text itemRight";
                    }
                }
                self.listReportTaiChinh_TheoThang(data);
                $("div[id ^= 'wait']").text("");
            });
        }
        else
        {
            $(".PhanQuyen").show();
        }
    }

    self.listReportTaiChinh_TheoQuy = ko.observableArray();
    self.getListTaiChinh_TheoQuy = function () {
        if (self.BCTC_TheoQuy() == "BCTC_TheoQuy")
        {
            $(".PhanQuyen").hide();
            hidewait('table_h');
            ajaxHelper(ReportUri + "getListTaiChinh_TheoQuy?year=" + _year + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
                for (var i = 0; i < data.length; i++) {
                    data[i].Padding33 = "";
                    data[i].ColorText = "text-right itemRight";
                    if (i === 2 || i === 3 || i === 8 || i == 9 || i == 10 || i == 13 || i == 14 || i == 15) {
                        data[i].Padding33 = "mahang padding33";
                        data[i].ColorText = "text-right color_text itemRight";
                    }
                }
                self.listReportTaiChinh_TheoQuy(data);
                $("div[id ^= 'wait']").text("");
            });
        }
        else {
            $(".PhanQuyen").show();
        }
    }

    self.listReportTaiChinh_TheoNam = ko.observableArray();
    self.getListTaiChinh_TheoNam = function () {
        if (self.BCTC_TheoNam() == "BCTC_TheoNam")
        {
            $(".PhanQuyen").hide();
            hidewait('table_h');
            ajaxHelper(ReportUri + "getListTaiChinh_TheoNam?year=" + _year + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
                console.log(data);
                for (var i = 0; i < data.length; i++) {
                    data[i].Padding33 = "";
                    data[i].ColorText = "text-right itemRight";
                    if (i === 2 || i === 3 || i === 8 || i == 9 || i == 10 || i == 13 || i == 14 || i == 15) {
                        data[i].Padding33 = "mahang padding33";
                        data[i].ColorText = "text-right color_text itemRight";
                    }
                }
                self.listReportTaiChinh_TheoNam(data);
                $("div[id ^= 'wait']").text("");
            });
        }
        else {
            $(".PhanQuyen").show();
        }
       
    }

    self.ArrayYear = ko.observableArray();
    self.getListYear = function () {
        ajaxHelper(ReportUri + "getListYear", "GET").done(function (data) {
            self.ArrayYear(data);
            _year = self.ArrayYear()[0].Year;
            self.newYear(_year);
            if (self.check_kieubang() == 1)
                self.getListTaiChinh_TheoThang_BieuDo();
            else
                self.getListTaiChinh_TheoThang();
        });
    }
    self.getListYear();

    self.SelectYearReport = function (item) {
        _year = item.Year;
        self.newYear(_year);
        $('#selec-all-Year li').each(function () {
            $(this).removeClass('SelectReport');
            if (parseInt($(this).attr('id')) === parseInt(_year)) {
                $(this).addClass('SelectReport');
            }
        });
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListTaiChinh_TheoThang_BieuDo();
            else
                self.getListTaiChinh_TheoThang();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListTaiChinh_TheoQuy_BieuDo();
            else
                self.getListTaiChinh_TheoQuy();
        }
        else if (_kieubang == 3) {
            if (self.check_kieubang() == 1)
                self.getListTaiChinh_TheoNam_BieuDo();
            else
                self.getListTaiChinh_TheoNam();
        }
    }
    // xuat excel
    self.ExportExcel = function () {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo tài chính",
            NoiDung: "Xuất báo cáo kết quả hoạt động kinh doanh theo " + kieu_time + " năm " + self.newYear(),
            NoiDungChiTiet: "Xuất báo cáo kết quả hoạt động kinh doanh theo " + kieu_time + " năm " + self.newYear(),
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
                if (_kieubang == 1) {
                    if (self.BCTC_TheoThang() == "BCTC_TheoThang" && self.BCTC_TheoThang_XuatFile() == "BCTC_TheoThang_XuatFile")
                    {
                        var url = ReportUri + "ExportExcelTaiChinh_TheoThang?year=" + _year + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC1() + "&ChiNhanh=" + self.TenChiNhanh();
                        window.location.href = url;
                    }
                    else
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                }
                else if (_kieubang == 2) {
                    if (self.BCTC_TheoQuy() == "BCTC_TheoQuy" && self.BCTC_TheoQuy_XuatFile() == "BCTC_TheoQuy_XuatFile")
                    {
                        var url = ReportUri + "ExportExcelTaiChinh_TheoQuy?year=" + _year + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC1() + "&ChiNhanh=" + self.TenChiNhanh();
                        window.location.href = url;
                    }
                    else 
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                }
                else if (_kieubang == 3) {
                    if (self.BCTC_TheoNam() == "BCTC_TheoNam" && self.BCTC_TheoNam_XuatFile() == "BCTC_TheoNam_XuatFile")
                    {
                        var url = ReportUri + "ExportExcelTaiChinh_TheoNam?year=" + _year + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC1() + "&ChiNhanh=" + self.TenChiNhanh();
                        window.location.href = url;
                    }
                    else
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
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

    // load biểu đồ
    self.TodayBC = ko.observable();
    self.DoanhThuBanHang = ko.observableArray();
    self.DoanhThuBanHang_BieuDo = ko.observableArray();
    self.DonViTinhDS = ko.observable();
    self.arrNT = ko.observableArray();
    self.arrDS = ko.observableArray();
    self.data_DoanhSo = ko.observableArray();
    self.DonViTinh_DoanhSo = ko.observable();
    self.getListTaiChinh_TheoThang_BieuDo = function () {
        self.DoanhThuBanHang([]);
        self.TodayBC('tháng');
        hidewait('table_h');
        self.arrNT([]);
        self.arrDS([]);
        self.data_DoanhSo([]);
        ajaxHelper(ReportUri + "getListTaiChinh_TheoThang?year=" + _year + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            if (data[17].Thang1 > 0)
                self.DoanhThuBanHang().push({ 'Columnss': 'T1.' + _year, 'Rowsn': data[17].Thang1 });
            if (data[17].Thang2 > 0)
                self.DoanhThuBanHang().push({ 'Columnss': 'T2.' + _year, 'Rowsn': data[17].Thang2 });
            if (data[17].Thang3 > 0)
                self.DoanhThuBanHang().push({ 'Columnss': 'T3.' + _year, 'Rowsn': data[17].Thang3 });
            if (data[17].Thang4 > 0)
                self.DoanhThuBanHang().push({ 'Columnss': 'T4.' + _year, 'Rowsn': data[17].Thang4 });
            if (data[17].Thang5 > 0)
                self.DoanhThuBanHang().push({ 'Columnss': 'T5.' + _year, 'Rowsn': data[17].Thang5 });
            if (data[17].Thang6 > 0)
                self.DoanhThuBanHang().push({ 'Columnss': 'T6.' + _year, 'Rowsn': data[17].Thang6 });
            if (data[17].Thang7 > 0)
                self.DoanhThuBanHang().push({ 'Columnss': 'T7.' + _year, 'Rowsn': data[17].Thang7 });
            if (data[17].Thang8 > 0)
                self.DoanhThuBanHang().push({ 'Columnss': 'T8.' + _year, 'Rowsn': data[17].Thang8 });
            if (data[17].Thang9 > 0)
                self.DoanhThuBanHang().push({ 'Columnss': 'T9.' + _year, 'Rowsn': data[17].Thang9 });
            if (data[17].Thang10 > 0)
                self.DoanhThuBanHang().push({ 'Columnss': 'T10.' + _year, 'Rowsn': data[17].Thang10 });
            if (data[17].Thang11 > 0)
                self.DoanhThuBanHang().push({ 'Columnss': 'T11.' + _year, 'Rowsn': data[17].Thang11 });
            if (data[17].Thang12 > 0)
                self.DoanhThuBanHang().push({ 'Columnss': 'T12.' + _year, 'Rowsn': data[17].Thang12 });
            if (self.MangChiNhanh_BieuDo().length > 1) {
                for (var m = 0; m < self.DoanhThuBanHang().length; m++) {
                    self.arrNT.push(self.DoanhThuBanHang()[m].Columnss);
                }
                for (var i = 0; i < self.MangChiNhanh_BieuDo().length; i++) {
                    var objMangChiNhanh = self.MangChiNhanh_BieuDo()[i];
                    $.ajax({
                        type: 'GET',
                        url: ReportUri + "getListTaiChinh_TheoThang?year=" + _year + "&ID_ChiNhanh=" + objMangChiNhanh.ID,
                        async: false,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: null,
                        success: function (data2) {
                            if (data2[17].Thang1 > 0)
                                self.DoanhThuBanHang_BieuDo().push({ 'Columnss': 'T1.' + _year, 'Rowsn': data2[17].Thang1 });
                            if (data2[17].Thang2 > 0)
                                self.DoanhThuBanHang_BieuDo().push({ 'Columnss': 'T2.' + _year, 'Rowsn': data2[17].Thang2 });
                            if (data2[17].Thang3 > 0)
                                self.DoanhThuBanHang_BieuDo().push({ 'Columnss': 'T3.' + _year, 'Rowsn': data2[17].Thang3 });
                            if (data2[17].Thang4 > 0)
                                self.DoanhThuBanHang_BieuDo().push({ 'Columnss': 'T4.' + _year, 'Rowsn': data2[17].Thang4 });
                            if (data2[17].Thang5 > 0)
                                self.DoanhThuBanHang_BieuDo().push({ 'Columnss': 'T5.' + _year, 'Rowsn': data2[17].Thang5 });
                            if (data2[17].Thang6 > 0)
                                self.DoanhThuBanHang_BieuDo().push({ 'Columnss': 'T6.' + _year, 'Rowsn': data2[17].Thang6 });
                            if (data2[17].Thang7 > 0)
                                self.DoanhThuBanHang_BieuDo().push({ 'Columnss': 'T7.' + _year, 'Rowsn': data2[17].Thang7 });
                            if (data2[17].Thang8 > 0)
                                self.DoanhThuBanHang_BieuDo().push({ 'Columnss': 'T8.' + _year, 'Rowsn': data2[17].Thang8 });
                            if (data2[17].Thang9 > 0)
                                self.DoanhThuBanHang_BieuDo().push({ 'Columnss': 'T9.' + _year, 'Rowsn': data2[17].Thang9 });
                            if (data2[17].Thang10 > 0)
                                self.DoanhThuBanHang_BieuDo().push({ 'Columnss': 'T10.' + _year, 'Rowsn': data2[17].Thang10 });
                            if (data2[17].Thang11 > 0)
                                self.DoanhThuBanHang_BieuDo().push({ 'Columnss': 'T11.' + _year, 'Rowsn': data2[17].Thang11 });
                            if (data2[17].Thang12 > 0)
                                self.DoanhThuBanHang_BieuDo().push({ 'Columnss': 'T12.' + _year, 'Rowsn': data2[17].Thang12 });
                            for (var k = 0; k < self.DoanhThuBanHang().length; k++) {
                                for (var j = 0; j < self.DoanhThuBanHang_BieuDo().length; j++) {
                                    if (self.DoanhThuBanHang_BieuDo()[j].Columnss == self.DoanhThuBanHang()[k].Columnss) {
                                        self.arrDS.push(self.DoanhThuBanHang_BieuDo()[j].Rowsn);
                                        break;
                                    }
                                    if (j == self.DoanhThuBanHang_BieuDo().length - 1)
                                        self.arrDS.push(0);
                                }
                            }
                            var obj = {
                                name: objMangChiNhanh.TenDonVi,
                                turboThreshold: i,
                                _colorIndex: i,
                                data: self.arrDS()
                            }
                            self.data_DoanhSo.push(obj);
                            console.log(self.data_DoanhSo())
                            self.arrDS([]);
                            self.DoanhThuBanHang_BieuDo([]);
                        },
                        statusCode: {
                            404: function () {
                            },
                        }
                    })
                }
            }
            else {
                for (var i = 0; i < self.DoanhThuBanHang().length; i++) {
                    _loadDate = self.DoanhThuBanHang()[i].Columnss;
                    self.arrNT.push(_loadDate);
                    self.arrDS.push(self.DoanhThuBanHang()[i].Rowsn);
                }
                var obj = {
                    name: self.MangChiNhanh_BieuDo()[0].TenDonVi,
                    turboThreshold: 0,
                    _colorIndex: 0,
                    data: self.arrDS()
                }
                self.data_DoanhSo.push(obj);
            }
            var dvt = 0;
            var TongTienDS = 0;
            for (var i = 0; i < self.DoanhThuBanHang().length; i++) {
                if (dvt < self.DoanhThuBanHang()[i].Rowsn) {
                    dvt = self.DoanhThuBanHang()[i].Rowsn;
                }
            }
            if (dvt >= 1000000000) {
                self.DonViTinh_DoanhSo(1000000000);
                self.DonViTinhDS(" tỷ")
            }
            if (dvt >= 1000000 & dvt < 1000000000) {
                self.DonViTinh_DoanhSo(1000000);
                self.DonViTinhDS(" tr")
            }
            if (dvt >= 1000 & dvt < 1000000) {
                self.DonViTinh_DoanhSo(1000);
                self.DonViTinhDS(" k")
            }
            self.loadDoanhThuBanHang();
            $("div[id ^= 'wait']").text("");
        });
    }
    self.getListTaiChinh_TheoQuy_BieuDo = function () {
        self.DoanhThuBanHang([]);
        self.data_DoanhSo([]);
        self.TodayBC('quý');
        hidewait('table_h');
        self.arrNT([]);
        self.arrDS([]);
        ajaxHelper(ReportUri + "getListTaiChinh_TheoQuy?year=" + _year + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            if (data[17].Quy1 > 0)
                self.DoanhThuBanHang().push({ 'Columnss': 'Q1.' + _year, 'Rowsn': data[17].Quy1 });
            if (data[17].Quy2 > 0)
                self.DoanhThuBanHang().push({ 'Columnss': 'Q2.' + _year, 'Rowsn': data[17].Quy2 });
            if (data[17].Quy3 > 0)
                self.DoanhThuBanHang().push({ 'Columnss': 'Q3.' + _year, 'Rowsn': data[17].Quy3 });
            if (data[17].Quy4 > 0)
                self.DoanhThuBanHang().push({ 'Columnss': 'Q4.' + _year, 'Rowsn': data[17].Quy4 });

            if (self.MangChiNhanh_BieuDo().length > 1) {
                for (var m = 0; m < self.DoanhThuBanHang().length; m++) {
                    self.arrNT.push(self.DoanhThuBanHang()[m].Columnss);
                }
                for (var i = 0; i < self.MangChiNhanh_BieuDo().length; i++) {
                    var objMangChiNhanh = self.MangChiNhanh_BieuDo()[i];
                    $.ajax({
                        type: 'GET',
                        url: ReportUri + "getListTaiChinh_TheoQuy?year=" + _year + "&ID_ChiNhanh=" + objMangChiNhanh.ID,
                        async: false,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: null,
                        success: function (data2) {
                            if (data2[17].Quy1 > 0)
                                self.DoanhThuBanHang_BieuDo().push({ 'Columnss': 'Q1.' + _year, 'Rowsn': data2[17].Quy1 });
                            if (data2[17].Quy2 > 0)
                                self.DoanhThuBanHang_BieuDo().push({ 'Columnss': 'Q2.' + _year, 'Rowsn': data2[17].Quy2 });
                            if (data2[17].Quy3 > 0)
                                self.DoanhThuBanHang_BieuDo().push({ 'Columnss': 'Q3.' + _year, 'Rowsn': data2[17].Quy3 });
                            if (data2[17].Quy4 > 0)
                                self.DoanhThuBanHang_BieuDo().push({ 'Columnss': 'Q4.' + _year, 'Rowsn': data2[17].Quy4 });

                            for (var k = 0; k < self.DoanhThuBanHang().length; k++) {
                                for (var j = 0; j < self.DoanhThuBanHang_BieuDo().length; j++) {
                                    if (self.DoanhThuBanHang_BieuDo()[j].Columnss == self.DoanhThuBanHang()[k].Columnss) {
                                        self.arrDS.push(self.DoanhThuBanHang_BieuDo()[j].Rowsn);
                                        break;
                                    }
                                    if (j == self.DoanhThuBanHang_BieuDo().length - 1)
                                        self.arrDS.push(0);
                                }
                            }
                            var obj = {
                                name: objMangChiNhanh.TenDonVi,
                                turboThreshold: i,
                                _colorIndex: i,
                                data: self.arrDS()
                            }
                            self.data_DoanhSo.push(obj);
                            self.arrDS([]);
                            self.DoanhThuBanHang_BieuDo([]);
                        },
                        statusCode: {
                            404: function () {
                            },
                        }
                    })
                }
            }
            else {
                for (var i = 0; i < self.DoanhThuBanHang().length; i++) {
                    _loadDate = self.DoanhThuBanHang()[i].Columnss;
                    self.arrNT.push(_loadDate);
                    self.arrDS.push(self.DoanhThuBanHang()[i].Rowsn);
                }
                var obj = {
                    name: self.MangChiNhanh_BieuDo()[0].TenDonVi,
                    turboThreshold: 0,
                    _colorIndex: 0,
                    data: self.arrDS()
                }
                self.data_DoanhSo.push(obj);
            }
            var dvt = 0;
            for (var i = 0; i < self.DoanhThuBanHang().length; i++) {
                if (dvt < self.DoanhThuBanHang()[i].Rowsn) {
                    dvt = self.DoanhThuBanHang()[i].Rowsn;
                }
            }
            if (dvt >= 1000000000) {
                self.DonViTinh_DoanhSo(1000000000);
                self.DonViTinhDS(" tỷ")
            }
            if (dvt >= 1000000 & dvt < 1000000000) {
                self.DonViTinh_DoanhSo(1000000);
                self.DonViTinhDS(" tr")
            }
            if (dvt >= 1000 & dvt < 1000000) {
                self.DonViTinh_DoanhSo(1000);
                self.DonViTinhDS(" k")
            }
            self.loadDoanhThuBanHang();
            $("div[id ^= 'wait']").text("");
        });
    }
    self.getListTaiChinh_TheoNam_BieuDo = function () {
        self.DoanhThuBanHang([]);
        self.TodayBC('năm: ' + _year);
        hidewait('table_h');
        self.data_DoanhSo([]);
        self.arrNT([]);
        self.arrDS([]);
        ajaxHelper(ReportUri + "getListTaiChinh_TheoNam?year=" + _year + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            if (data[17].Nam > 0)
                self.DoanhThuBanHang().push({ 'Columnss': 'Năm ' + _year, 'Rowsn': data[17].Nam });
            if (self.MangChiNhanh_BieuDo().length > 1) {
                for (var m = 0; m < self.DoanhThuBanHang().length; m++) {
                    self.arrNT.push(self.DoanhThuBanHang()[m].Columnss);
                }
                for (var i = 0; i < self.MangChiNhanh_BieuDo().length; i++) {
                    var objMangChiNhanh = self.MangChiNhanh_BieuDo()[i];
                    $.ajax({
                        type: 'GET',
                        url: ReportUri + "getListTaiChinh_TheoNam?year=" + _year + "&ID_ChiNhanh=" + objMangChiNhanh.ID,
                        async: false,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: null,
                        success: function (data2) {
                            if (data2[17].Nam > 0)
                            {
                                //self.DoanhThuBanHang_BieuDo().push({ 'Columnss': 'Năm ' + _year, 'Rowsn': data2[17].Nam });
                                self.arrDS.push(data2[17].Nam);
                                var obj = {
                                    name: objMangChiNhanh.TenDonVi,
                                    turboThreshold: i,
                                    _colorIndex: i,
                                    data: self.arrDS()
                                }
                                self.data_DoanhSo.push(obj);
                                console.log(self.data_DoanhSo());
                                self.arrDS([]);
                                self.DoanhThuBanHang_BieuDo([]);
                            }
                        },
                        statusCode: {
                            404: function () {
                            },
                        }
                    })
                }
            }
            else {
                _loadDate = self.DoanhThuBanHang()[0].Columnss;
                self.arrNT.push(_loadDate);
                self.arrDS.push(self.DoanhThuBanHang()[0].Rowsn);
                var obj = {
                    name: self.MangChiNhanh_BieuDo()[0].TenDonVi,
                    turboThreshold: 0,
                    _colorIndex: 0,
                    data: self.arrDS()
                }
                self.data_DoanhSo.push(obj);
            }
            var dvt = 0;
            for (var i = 0; i < self.DoanhThuBanHang().length; i++) {
                if (dvt < self.DoanhThuBanHang()[i].Rowsn) {
                    dvt = self.DoanhThuBanHang()[i].Rowsn;
                }
            }
            if (dvt >= 1000000000) {
                self.DonViTinh_DoanhSo(1000000000);
                self.DonViTinhDS(" tỷ")
            }
            if (dvt >= 1000000 & dvt < 1000000000) {
                self.DonViTinh_DoanhSo(1000000);
                self.DonViTinhDS(" tr")
            }
            if (dvt >= 1000 & dvt < 1000000) {
                self.DonViTinh_DoanhSo(1000);
                self.DonViTinhDS(" k")
            }
            self.loadDoanhThuBanHang();
            $("div[id ^= 'wait']").text("");
        });
    }
    self.loadDoanhThuBanHang = function () {
        var viewPrint = true;
        if (self.data_DoanhSo().length > 0)
            viewPrint = true;
        else
            viewPrint = false;
        var chart = Highcharts.chart('chart', {
            chart: {
                type: 'column',
                polar: false
            },
            title: {
                text: 'Lợi nhuận theo ' + self.TodayBC()
            },
            subtitle: {
                text: ''
            },
            xAxis: {
                categories: self.arrNT(),
                crosshair: true
            },
            yAxis: {
                min: 0,
                title: {
                    text: ''
                },
                labels: {
                    formatter: function () {
                        if (this.value == 0)
                            return 0
                        else
                            return Highcharts.numberFormat(this.value / self.DonViTinh_DoanhSo(), 0, '.', ',') + self.DonViTinhDS();
                    }
                }
            },
            tooltip: {
                footerFormat: "</table>",
                shared: false,
                useHTML: true,
                formatter: function () {
                    return "</span><table>" + '<b style=\"color:{series.color};padding:0; text-align:Left; text-transform: capitalize;\">' + this.series.name + ': ' + Highcharts.numberFormat(this.y, 0, '.', ',') + "</b>";
                }
            },
            plotOptions: {
                series: {
                    stacking: "normal",
                    animation: false,
                    // pointWidth: 30,
                    maxPointWidth: 30,
                    dataLabels: {
                        style: {
                            color: "contrast",
                            fontSize: "11px",
                            fontWeight: "bold",
                            textOutline: "1px 1px contrast"
                        }
                    }
                },
                column: {
                    pointPadding: 0.2,
                    borderWidth: 0
                }
            },
            navigation: {
                buttonOptions: {
                    align: 'right',
                    verticalAlign: 'bottom',
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
            series: self.data_DoanhSo(),
            colors: [
                "#32b7b3",
                "#ef6c00",
                "#8085e9",
                "#2979ff",
                "#8085e9",
                "#f15c80",
                "#e4d354",
                "#2b908f",
                "#f45b5b",
                "#91e8e1"
            ],
            legend: {
                align: "center",
                verticalAlign: "bottom",
                x: 20
            },
            credits: {
                enabled: false,
            }
        });
        //var chart = Highcharts.chart('chart', {
        //    chart: {
        //        type: 'column'
        //    },
        //    title: {
        //        text: 'Lợi nhuận theo ' + self.TodayBC()
        //    },
        //    xAxis: {
        //        categories: self.arrNT()
        //    },
        //    yAxis: {
        //        min: 0,
        //        title: {
        //            text: 'Total fruit consumption'
        //        },
        //        stackLabels: {
        //            enabled: true,
        //            style: {
        //                fontWeight: 'bold',
        //                color: (Highcharts.theme && Highcharts.theme.textColor) || 'black'
        //            }
        //        }
        //    },

        //    plotOptions: {
        //        column: {
        //            stacking: 'normal',
        //            dataLabels: {
        //            }
        //        }
        //    },
        //    series: [{
        //        name: "Doanh thu thuần",
        //        data: self.arrDS(),
        //        maxPointWidth: 30
        //    }],

        //});
    };
}
ko.applyBindings(new ViewModel());
function hidewait(o) {
    $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /></div>')
}