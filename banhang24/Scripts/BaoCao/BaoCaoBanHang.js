var ViewModelBaoCaoBanHang = function () {
    var self = this;
    self.RowsStart = ko.observable('1');
    self.RowsEnd = ko.observable('10');
    self.NgayTao = ko.observable();
    self.BaoCaoBanHangs = ko.observableArray();
    self.TongHangBaoCaoBanHangs = ko.observable();
    self.TongTrangBaoCaoBanHangs = ko.observableArray();
    self.SelectTrangBC = ko.observableArray([{ name: "SoTrang" }]);
    self.soHang = ko.observable();
    self.rdoBaoCao = ko.observable(true);
    //self.filterBaoCao = ko.observable('1');
    self.filterBieuBaoCao = ko.observable('0')
    self.filterNgayBaoCao = ko.observable('0');
    self.filterQuanTam = ko.observable('0');
    self.NgayBaoCao = ko.observable('0');
    self.Ngaypage = ko.observable('0');
    self.filterDangBang = ko.observable('0')
    var nextPage = 1;
    var _IDchinhanh = $('.branch label').attr('id'); // lấy ID chi nhánh _header.cshtml
    var datime1 = new Date();
    var timeStart = '2017-01-01';
    var timeEnd = moment(new Date(datime1.getFullYear(), datime1.getMonth() + 1, 1)).format('YYYY-MM-DD');
    var timeBC = '';
    var numberPage = 1;
    var numberRows = 10;
    var AllPage = 1;
    var thisDate = '';
    var itemSoTrang = 1;
    self.MoneyBanHang = ko.observableArray();
    self._ThanhTien = ko.observable(0);
    self._TienVon = ko.observable(0);
    self._LaiLo = ko.observable();
    self.TodayBC = ko.observable('Toàn thời gian');
    self.TenChiNhanh = ko.observable($('.branch label').text());
    var _time = new Date();
    self.thoigian = ko.observable(moment(_time).format('DD/MM/YYYY'))
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    // Khai báo ajaxHelper để kết nối tới API
    function ajaxHelper(uri, method, data) {
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            //data: { loaiDoiTuong: loaiDoiTuong }, // add
            contentType: 'application/json',
            data: data ? JSON.stringify() : null,
            success: function (data) {
                data: data ? JSON.stringify() : null;
            },
            statusCode: {
                404: function () {
                },
            }
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
            });
    }

    self.currentPage = ko.observable(1); //load trang số 1

    self.ResetCurrentPage = function () {
        self.currentPage(1);
    };
   

    self.selecPage = function () {
        AllPage = self.TongTrangBaoCaoBanHangs().length;
        if (AllPage > 4) {
            for (var i = 5; i < AllPage; i++) {
                self.TongTrangBaoCaoBanHangs.pop(i + 1);
            }
        }
        else {
        }
    }
    self.ReserPage = function (item) {
        //self.selecPage();
        if (nextPage > 1 && AllPage > 5/* && nextPage < AllPage - 1*/) {
            if (nextPage > 3 && nextPage < AllPage - 1) {
                for (var i = 0; i < 5; i++) {
                    self.TongTrangBaoCaoBanHangs.replace(self.TongTrangBaoCaoBanHangs()[i], { SoTrang: parseInt(nextPage) + i - 2 });
                }
            }
            else if (parseInt(nextPage) === parseInt(AllPage) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.TongTrangBaoCaoBanHangs.replace(self.TongTrangBaoCaoBanHangs()[i], { SoTrang: parseInt(nextPage) + i - 3 });
                }
            }
            else if (nextPage == AllPage) {
                for (var i = 0; i < 5; i++) {
                    self.TongTrangBaoCaoBanHangs.replace(self.TongTrangBaoCaoBanHangs()[i], { SoTrang: parseInt(nextPage) + i - 4 });
                }
            }
        }
        if (nextPage == 1)
        {
            for (var i = 0; i < 5; i++) {
                self.TongTrangBaoCaoBanHangs.replace(self.TongTrangBaoCaoBanHangs()[i], { SoTrang: parseInt(nextPage) + i});
            }
        }
        self.currentPage(parseInt(nextPage));

        
    }
    //format phân trang 
    self.GoToPage = function (item) {
        if (nextPage < AllPage) {
            nextPage = nextPage + 1;
            self.ReserPage();
            self.NextandBackPage();
        }
    };
    self.BackPage = function (item) {
        if (nextPage > 1) {
            nextPage = nextPage - 1;
            self.ReserPage();
            self.NextandBackPage();
        }
    };
    self.EndPage = function (item) {
        nextPage = AllPage;
        self.ReserPage();
        self.NextandBackPage();
    };
    self.StartPage = function (item) {
        nextPage = 1;
        self.ReserPage();
        self.NextandBackPage();
        self.currentPage(1);
    };

    self.GetClass = function (page) {
        return (page.SoTrang === self.currentPage()) ? "click" : "";
    };
    function getList() {
        self.RowsStart('1');
        self.RowsEnd('10');
        self.currentPage(1);
        nextPage = 1;
        self.TodayBC("Từ " + moment(timeStart).format('DD/MM/YYYY') + "  đến " + moment(timeBC).format('DD/MM/YYYY'));
        //var _IDchinhanh = $('#checkcty').text();
        ajaxHelper(BH_HoaDonUri + "getBC_BanHang_inday?DateStart=" + timeStart + "&DateEnd=" + timeEnd + "&sohang=" + numberRows + "&Page=" + numberPage + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
            self.BaoCaoBanHangs(data);
        });
        ajaxHelper(BH_HoaDonUri + "getRowBCBHinday?DateStart=" + timeStart + "&DateEnd=" + timeEnd + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
            self.TongHangBaoCaoBanHangs(data);
        });
        ajaxHelper(BH_HoaDonUri + "getPageBCBHinday?DateStart=" + timeStart + "&DateEnd=" + timeEnd + "&sohang=" + numberRows + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
            self.TongTrangBaoCaoBanHangs(data);
            self.selecPage();
        });
        self.NgayBaoCao = ko.observable('0');
        self.getMoneyBanHang();
    }
    function getNextPages() {
        ajaxHelper(BH_HoaDonUri + "getBC_BanHang_inday?DateStart=" + timeStart + "&DateEnd=" + timeEnd + "&sohang=" + numberRows + "&Page=" + numberPage + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
            self.BaoCaoBanHangs(data);
            if (self.BaoCaoBanHangs().length != 0) {
                self.RowsStart((numberPage - 1) * numberRows + 1);
                self.RowsEnd((numberPage - 1) * numberRows + self.BaoCaoBanHangs().length)
            }
            else {
                self.RowsStart('0');
                self.RowsEnd('0');
            }
        });
    }

    // trả về List báo cáo bán hàng từ API
    //function getBaocaoBanHang() {
    //    ajaxHelper(BH_HoaDonUri + "GetBC_BanHang?TuNgayLapBaoCao=" + "2017-09-01" + "&DenNgayLapBaoCao=" + "2018-09-01" + "&sohang=" + "15" + "&Page=" + "1", "GET").done(function (data) {
    //        self.BaoCaoBanHangs(data);
    //    });
    //}
    function getAllBC_BanHang() {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('BCCN_TongHop', lc_CTQuyen) > -1) {
            ajaxHelper(BH_HoaDonUri + "getAllBC_BanHang?sohang=" + "10" + "&Page=" + "1" + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                self.BaoCaoBanHangs(data);
                if (self.BaoCaoBanHangs().length > 0) {
                    self.filterDangBang('2');
                    $('#home').removeClass("active")
                    $('#info').addClass("active")
                }
                else {
                    self.filterDangBang('1');
                    $('#info').removeClass("active")
                    $('#home').addClass("active")
                }
            });
            self.getMoneyBanHang();
        } else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Không có quyền xem DS báo cáo bán hàng!", "danger");
        }
    }

    self.getMoneyBanHang = function () {
        self._ThanhTien(0);
        self._TienVon(0);
        self._LaiLo(0);
        ajaxHelper(BH_HoaDonUri + "getMoneyBanHang?dayStart=" + timeStart + "&dayEnd=" + timeEnd + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
            self.MoneyBanHang(data);
            //self._ThanhTien(parseFloat(self.MoneyBanHang()[0].ThanhTien).toFixed(2));
            //self._TienVon(parseFloat(self.MoneyBanHang()[0].TienVon).toFixed(2));
            //self._LaiLo(parseFloat(self.MoneyBanHang()[0].LaiLo).toFixed(2));
            self._ThanhTien(self.MoneyBanHang()[0].ThanhTien);
            self._TienVon(self.MoneyBanHang()[0].TienVon);
            self._LaiLo(self.MoneyBanHang()[0].LaiLo);
        });
    }

    function getHang_BCBanHang() {
        ajaxHelper(BH_HoaDonUri + "getRowAllBaocaoBanHang?IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
            self.TongHangBaoCaoBanHangs(data);
        });
    }
    function getTrang_BCBanHang() {
        ajaxHelper(BH_HoaDonUri + "getPageAllBaoCaoBanHang?sohang=" + "10" + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
            self.TongTrangBaoCaoBanHangs(data);
            self.selecPage();
        });
        self.NgayBaoCao = ko.observable('0');
    }

    // lọc báo cáo
    $('.choseHienThi a').on('click', function () {
        self.filterDangBang($(this).val());

    });

    $('.choseNgayTaoBC li').on('click', function () {
        self.NgayBaoCao($(this).val());
        self.Ngaypage($(this).val());
        $
    });
    $('.choseQuanTam input').on('click', function () {
        self.filterQuanTam($(this).val());
        console.log(1, self.filterQuanTam())
    });

    $('.setcheck input').on('click', function () {
        //console.log(2, self.filterNgayBaoCao());
        //$('#txtTime').val($(this).text());
        //self.NgayTao($(this).text());
        //console.log('self.NgayTao ' + self.NgayTao())
    });

    $('#txtTime').on('dp.change', function (e) {
        thisDate = $(this).val();
        var t = thisDate.split(" ");
        var t1 = t[0].split("/").reverse().join("-")
        thisDate = moment(t1).format('MM/DD/YYYY')
        //thisDate = moment(thisDate).format('MM/DD/YYYY');
        console.log(thisDate);
        self.currentPage(1);
        if (self.filterNgayBaoCao() === '1') {
            var datime1 = moment(thisDate).format('DD/MM/YYYY');
            self.TodayBC(datime1);
            ajaxHelper(BH_HoaDonUri + "getBC_HanHang_Today?today=" + thisDate + "&sohang=" + "10" + "&Page=" + "1" + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                self.BaoCaoBanHangs(data);
                self.RowsStart('1');
                self.RowsEnd('10');
            });
            ajaxHelper(BH_HoaDonUri + "getRowBCBHtoday?today=" + thisDate + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                self.TongHangBaoCaoBanHangs(data);
            });
            ajaxHelper(BH_HoaDonUri + "getPageBCBHtoday?today=" + thisDate + "&sohang=" + "10" + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                self.TongTrangBaoCaoBanHangs(data);
                self.selecPage();
            });
            self.NgayBaoCao = ko.observable('0');

            timeStart = moment(new Date(thisDate)).format('YYYY-MM-DD');
            var dt = new Date(thisDate);
            timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
            self.getMoneyBanHang();
        }
    });

    //function getAllBC_BanHang() {
    self.FilteredDMBaoCao = ko.computed(function () {
        self.RowsStart('1');
        self.RowsEnd('10');
        var _rdoNgayBaoCao = self.NgayBaoCao();
        var datime = new Date();
        if (self.filterNgayBaoCao() === '0') {
            //Toàn thời gian
            if (_rdoNgayBaoCao === 0) {
                self.currentPage(1);
                nextPage = 1;
                self.TodayBC('Toàn thời gian');
                ajaxHelper(BH_HoaDonUri + "getAllBC_BanHang?sohang=" + "10" + "&Page=" + "1" + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                    self.BaoCaoBanHangs(data);
                });
                ajaxHelper(BH_HoaDonUri + "getRowAllBaocaoBanHang?IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                    self.TongHangBaoCaoBanHangs(data);
                });
                ajaxHelper(BH_HoaDonUri + "getPageAllBaoCaoBanHang?sohang=" + "10" + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                    self.TongTrangBaoCaoBanHangs(data);
                    self.selecPage();
                });
                self.NgayBaoCao = ko.observable('0');
                timeStart = "2017-01-01";
                timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                self.getMoneyBanHang();
            }
            //Hôm nay
            if (_rdoNgayBaoCao === 1) {
                self.currentPage(1);
                nextPage = 1;
                self.TodayBC(moment(datime).format('DD/MM/YYYY'));
                ajaxHelper(BH_HoaDonUri + "getBC_HanHang_Today?today=" + datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate() + "&sohang=" + "10" + "&Page=" + "1" + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                    self.BaoCaoBanHangs(data);
                });
                ajaxHelper(BH_HoaDonUri + "getRowBCBHtoday?today=" + datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate() + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                    self.TongHangBaoCaoBanHangs(data);
                });
                ajaxHelper(BH_HoaDonUri + "getPageBCBHtoday?today=" + datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate() + "&sohang=" + "10" + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                    self.TongTrangBaoCaoBanHangs(data);
                    self.selecPage();
                });
                self.NgayBaoCao = ko.observable('0');
                timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
                timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                self.getMoneyBanHang();
            }
            //Hôm qua
            if (_rdoNgayBaoCao === 2) {
                self.currentPage(1);
                nextPage = 1;
                var lastday = new Date(datime.setDate(datime.getDate() - 1));
                self.TodayBC(moment(lastday).format('DD/MM/YYYY'));
                ajaxHelper(BH_HoaDonUri + "getBC_HanHang_Today?today=" + lastday.getFullYear() + "-" + (lastday.getMonth() + 1) + "-" + lastday.getDate() + "&sohang=" + "10" + "&Page=" + "1" + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                    self.BaoCaoBanHangs(data);
                });
                ajaxHelper(BH_HoaDonUri + "getRowBCBHtoday?today=" + lastday.getFullYear() + "-" + (lastday.getMonth() + 1) + "-" + lastday.getDate() + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                    self.TongHangBaoCaoBanHangs(data);
                });
                ajaxHelper(BH_HoaDonUri + "getPageBCBHtoday?today=" + lastday.getFullYear() + "-" + (lastday.getMonth() + 1) + "-" + lastday.getDate() + "&sohang=" + "10" + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                    self.TongTrangBaoCaoBanHangs(data);
                    self.selecPage();
                });
                self.NgayBaoCao = ko.observable('0');

                var dt1 = new Date();
                var dt2 = new Date();
                timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
                timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
                self.getMoneyBanHang();
            }
            //Tuần này
            if (_rdoNgayBaoCao === 3) {
                var currentWeekDay = datime.getDay();
                var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
                timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
                timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
                var newtime = new Date();
                timeBC = moment(new Date(newtime.setDate(newtime.getDate() - lessDays + 6))).format('YYYY-MM-DD');
                numberPage = 1;
                getList();
            }
            //Tuần trước
            if (_rdoNgayBaoCao === 4) {
                timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
                timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
                var newtime = new Date();
                timeBC = moment(new Date(newtime.setDate(newtime.getDate() - newtime.getDay()))).format('YYYY-MM-DD');
                numberPage = 1;
                getList();
            }
            //7 ngày qua
            if (_rdoNgayBaoCao === 5) {
                timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
                var newtime = new Date();
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                timeBC = moment(new Date).format('YYYY-MM-DD');
                numberPage = 1;
                getList();
            }
            //Tháng này
            if (_rdoNgayBaoCao === 6) {
                timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
                timeBC = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 0)).format('YYYY-MM-DD');
                numberPage = 1;
                getList();
            }
            //Tháng trước
            if (_rdoNgayBaoCao === 7) {
                timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
                timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                timeBC = moment(new Date(datime.getFullYear(), datime.getMonth(), 0)).format('YYYY-MM-DD');
                numberPage = 1;
                getList();
            }
            if (_rdoNgayBaoCao === 8) {

            }
            if (_rdoNgayBaoCao === 9) {

            }
            //30 ngày qua
            if (_rdoNgayBaoCao === 10) {
                timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
                var newtime = new Date();
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                timeBC = moment(new Date()).format('YYYY-MM-DD');
                numberPage = 1;
                getList();
            }
            //Quý này
            if (_rdoNgayBaoCao === 11) {
                timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
                var newtime = new Date(moment().endOf('quarter'));
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                timeBC = moment().endOf('quarter').format('YYYY-MM-DD');
                numberPage = 1;
                getList();
            }
            //Quý trước
            if (_rdoNgayBaoCao === 12) {
                var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
                timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                var newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                timeBC = moment().quarter(prevQuarter).endOf('quarter').format('YYYY-MM-DD');
                numberPage = 1;
                getList();
            }
            //Năm này
            if (_rdoNgayBaoCao === 13) {
                timeStart = moment().startOf('year').format('YYYY-MM-DD');
                var newtime = new Date(moment().endOf('year'));
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                timeBC = moment().endOf('year').format('YYYY-MM-DD');
                numberPage = 1;
                getList()
            }
            //Năm trước
            if (_rdoNgayBaoCao === 14) {
                var prevYear = moment().year() - 1;
                timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                var newtime = new Date(moment().year(prevYear).endOf('year'));
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                timeBC = moment().year(prevYear).endOf('year').format('YYYY-MM-DD');
                numberPage = 1;
                getList();

            }
            if (_rdoNgayBaoCao === 15) {

            }
            if (_rdoNgayBaoCao === 16) {

            }
        }
        else {

        }
        return self.BaoCaoBanHangs();
    });

    self.gotoNextPage = function (item) {
        self.currentPage(item.SoTrang);
        nextPage = item.SoTrang;
        var _rdoNgayPage = self.Ngaypage();
        var datime = new Date();
        if (self.filterNgayBaoCao() === '0') {
            //Toàn thời gian
            if (_rdoNgayPage === 0 | _rdoNgayPage === '0') {
                //  TodayBC = 'Toàn thời gian';
                ajaxHelper(BH_HoaDonUri + "getAllBC_BanHang?sohang=" + "10" + "&Page=" + item.SoTrang + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                    self.BaoCaoBanHangs(data);
                    if (self.BaoCaoBanHangs().length != 0) {
                        self.RowsStart((item.SoTrang - 1) * 15 + 1);
                        self.RowsEnd((item.SoTrang - 1) * 15 + self.BaoCaoBanHangs().length)
                    }
                    else {
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }

                });

            }
            //Hôm nay
            if (_rdoNgayPage === 1) {
                TodayBC = moment(new Date(datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate())).format('DD/MM/YYYY');
                ajaxHelper(BH_HoaDonUri + "getBC_HanHang_Today?today=" + datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate() + "&sohang=" + "10" + "&Page=" + item.SoTrang + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                    self.BaoCaoBanHangs(data);
                    if (self.BaoCaoBanHangs().length != 0) {
                        self.RowsStart((item.SoTrang - 1) * 15 + 1);
                        self.RowsEnd((item.SoTrang - 1) * 15 + self.BaoCaoBanHangs().length)
                    }
                    else {
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                });
            }
            //Hôm qua
            if (_rdoNgayPage === 2) {
                var lastday = new Date(datime.setDate(datime.getDate() - 1));
                ajaxHelper(BH_HoaDonUri + "getBC_HanHang_Today?today=" + lastday.getFullYear() + "-" + (lastday.getMonth() + 1) + "-" + lastday.getDate() + "&sohang=" + "10" + "&Page=" + item.SoTrang + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                    self.BaoCaoBanHangs(data);
                    if (self.BaoCaoBanHangs().length != 0) {
                        self.RowsStart((item.SoTrang - 1) * 15 + 1);
                        self.RowsEnd((item.SoTrang - 1) * 15 + self.BaoCaoBanHangs().length)
                    }
                    else {
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                });
            }
            //Tuần này
            if (_rdoNgayPage === 3) {
                var currentWeekDay = datime.getDay();
                var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
                timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
                timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
                numberPage = item.SoTrang;
                getNextPages();
            }
            //Tuần trước
            if (_rdoNgayPage === 4) {
                timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
                timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
                numberPage = item.SoTrang;
                getNextPages();
            }
            //7 ngày qua
            if (_rdoNgayPage === 5) {
                timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
                var newtime = new Date();
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                numberPage = item.SoTrang;
                getNextPages();
            }
            //Tháng này
            if (_rdoNgayPage === 6) {
                timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
                numberPage = item.SoTrang;
                getNextPages();
            }
            //Tháng trước
            if (_rdoNgayPage === 7) {
                timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
                timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                numberPage = item.SoTrang;
                getNextPages();
            }
            if (_rdoNgayPage === 8) {

            }
            if (_rdoNgayPage === 9) {

            }
            //30 ngày qua
            if (_rdoNgayPage === 10) {
                timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
                var newtime = new Date();
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                numberPage = item.SoTrang;
                getNextPages();
            }
            //Quý này
            if (_rdoNgayPage === 11) {
                timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
                var newtime = new Date(moment().endOf('quarter'));
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                numberPage = item.SoTrang;
                getNextPages();
            }
            // Quý trước
            if (_rdoNgayPage === 12) {
                var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
                timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                var newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                numberPage = item.SoTrang;
                getNextPages();
            }
            //Năm này
            if (_rdoNgayPage === 13) {
                timeStart = moment().startOf('year').format('YYYY-MM-DD');
                var newtime = new Date(moment().endOf('year'));
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                numberPage = item.SoTrang;
                getNextPages();
            }
            //năm trước
            if (_rdoNgayPage === 14) {
                var prevYear = moment().year() - 1;
                timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                var newtime = new Date(moment().year(prevYear).endOf('year'));
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                numberPage = item.SoTrang;
                getNextPages();
            }
            if (_rdoNgayPage === 15) {

            }
            if (_rdoNgayPage === 16) {

            }
        }
        else {
            ajaxHelper(BH_HoaDonUri + "getBC_HanHang_Today?today=" + thisDate + "&sohang=" + "10" + "&Page=" + item.SoTrang + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                self.BaoCaoBanHangs(data);
                if (self.BaoCaoBanHangs().length != 0) {
                    self.RowsStart((item.SoTrang - 1) * 15 + 1);
                    self.RowsEnd((item.SoTrang - 1) * 15 + self.BaoCaoBanHangs().length)
                }
                else {
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
            });
        }
        self.ReserPage();
    }

    self.NextandBackPage = function () {
        self.currentPage(nextPage);
        var _rdoNgayPage = self.Ngaypage();
        var datime = new Date();
        if (self.filterNgayBaoCao() === '0') {
            //Toàn thời gian
            if (_rdoNgayPage === 0 | _rdoNgayPage === '0') {
                //  TodayBC = 'Toàn thời gian';
                ajaxHelper(BH_HoaDonUri + "getAllBC_BanHang?sohang=" + "10" + "&Page=" + nextPage + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                    self.BaoCaoBanHangs(data);
                    if (self.BaoCaoBanHangs().length != 0) {
                        self.RowsStart((nextPage - 1) * 15 + 1);
                        self.RowsEnd((nextPage- 1) * 15 + self.BaoCaoBanHangs().length)
                    }
                    else {
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }

                });
            }
            //Hôm nay
            if (_rdoNgayPage === 1) {
                TodayBC = moment(new Date(datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate())).format('DD/MM/YYYY');
                ajaxHelper(BH_HoaDonUri + "getBC_HanHang_Today?today=" + datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate() + "&sohang=" + "10" + "&Page=" + nextPage + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                    self.BaoCaoBanHangs(data);
                    if (self.BaoCaoBanHangs().length != 0) {
                        self.RowsStart((nextPage - 1) * 15 + 1);
                        self.RowsEnd((nextPage - 1) * 15 + self.BaoCaoBanHangs().length)
                    }
                    else {
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                });
            }
            //Hôm qua
            if (_rdoNgayPage === 2) {
                var lastday = new Date(datime.setDate(datime.getDate() - 1));
                ajaxHelper(BH_HoaDonUri + "getBC_HanHang_Today?today=" + lastday.getFullYear() + "-" + (lastday.getMonth() + 1) + "-" + lastday.getDate() + "&sohang=" + "10" + "&Page=" + nextPage + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                    self.BaoCaoBanHangs(data);
                    if (self.BaoCaoBanHangs().length != 0) {
                        self.RowsStart((nextPage - 1) * 15 + 1);
                        self.RowsEnd((nextPage - 1) * 15 + self.BaoCaoBanHangs().length)
                    }
                    else {
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                });
            }
            //Tuần này
            if (_rdoNgayPage === 3) {
                var currentWeekDay = datime.getDay();
                var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
                timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
                timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
                numberPage = nextPage;
                getNextPages();
            }
            //Tuần trước
            if (_rdoNgayPage === 4) {
                timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
                timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
                numberPage = nextPage;
                getNextPages();
            }
            //7 ngày qua
            if (_rdoNgayPage === 5) {
                timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
                var newtime = new Date();
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                numberPage = nextPage;
                getNextPages();
            }
            //Tháng này
            if (_rdoNgayPage === 6) {
                timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
                numberPage = nextPage;
                getNextPages();
            }
            //Tháng trước
            if (_rdoNgayPage === 7) {
                timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
                timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                numberPage = nextPage;
                getNextPages();
            }
            if (_rdoNgayPage === 8) {

            }
            if (_rdoNgayPage === 9) {

            }
            //30 ngày qua
            if (_rdoNgayPage === 10) {
                timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
                var newtime = new Date();
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                numberPage = nextPage;
                getNextPages();
            }
            //Quý này
            if (_rdoNgayPage === 11) {
                timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
                var newtime = new Date(moment().endOf('quarter'));
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                numberPage = nextPage;
                getNextPages();
            }
            // Quý trước
            if (_rdoNgayPage === 12) {
                var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
                timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                var newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                numberPage = nextPage;
                getNextPages();
            }
            //Năm này
            if (_rdoNgayPage === 13) {
                timeStart = moment().startOf('year').format('YYYY-MM-DD');
                var newtime = new Date(moment().endOf('year'));
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                numberPage = nextPage;
                getNextPages();
            }
            //năm trước
            if (_rdoNgayPage === 14) {
                var prevYear = moment().year() - 1;
                timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                var newtime = new Date(moment().year(prevYear).endOf('year'));
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                numberPage = nextPage;
                getNextPages();
            }
            if (_rdoNgayPage === 15) {

            }
            if (_rdoNgayPage === 16) {

            }
        }
        else {
            ajaxHelper(BH_HoaDonUri + "getBC_HanHang_Today?today=" + thisDate + "&sohang=" + "10" + "&Page=" + nextPage + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
                self.BaoCaoBanHangs(data);
                if (self.BaoCaoBanHangs().length != 0) {
                    self.RowsStart((nextPage - 1) * 15 + 1);
                    self.RowsEnd((nextPage - 1) * 15 + self.BaoCaoBanHangs().length)
                }
                else {
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
            });
        }
    }

    // chạy hàm lấy List báo cáo bán hàng
    getAllBC_BanHang();
    getHang_BCBanHang();
    getTrang_BCBanHang();
}
ko.applyBindings(new ViewModelBaoCaoBanHang());