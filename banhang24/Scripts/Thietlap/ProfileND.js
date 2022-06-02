var FormModel_ChuyenTien = function () {
    var self = this;

    self.ID = ko.observable();
    self.ID_NguoiNhanTien = ko.observable();
    self.SoTien = ko.observable();
    self.GhiChu = ko.observable();

    self.Setdata = function (item) {
        self.ID(item.ID);
        self.ID_NguoiNhanTien(item.ID_NguoiNhanTien);
        self.SoTien(item.SoTien);
        self.GhiChu(item.GhiChu);
    };
};

var FormModel_NapTien = function () {
    var self = this;

    self.ID = ko.observable();
    self.SoTien = ko.observable();
    self.GhiChu = ko.observable();

    self.Setdata = function (item) {
        self.ID(item.ID);
        self.SoTien(item.SoTien);
        self.GhiChu(item.GhiChu);
    };
};


var ViewModel = function () {
    var self = this;

    var ThietLapApis = '/api/DanhMuc/ThietLapApi/';
    var _txtTenTaiKhoan = $('#txtTenTaiKhoan').text();
    var _IDNguoiDung = $('.idnguoidung').text();
    var _IDNhanVien = $('.idnhanvien').text();
    var _IDchinhanh = $('#hd_IDdDonVi').val();
    var userLogin = $('#txtTenTaiKhoan').text();
    self.error = ko.observable();
    self.TenTaiKhoanLogin = ko.observable(userLogin);
    self.IDNguoiDungCheck = ko.observable(_IDNguoiDung);

    self.newChuyenTien = ko.observable(new FormModel_ChuyenTien());
    self.newNapTien = ko.observable(new FormModel_NapTien());
    //pagging
    self.pageSizes = [10, 20, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();
    self.arrPagging = ko.observableArray();

    self.TotalRecord = ko.observable(0);
    self.PageCount = ko.observable();

    self.pageSizes1 = [10, 20, 30, 40, 50];
    self.pageSize1 = ko.observable(self.pageSizes[0]);
    self.currentPage1 = ko.observable(0);
    self.fromitem1 = ko.observable(1);
    self.toitem1 = ko.observable();
    self.arrPagging1 = ko.observableArray();

    self.TotalRecord1 = ko.observable(0);
    self.PageCount1 = ko.observable();

    function ajaxHelper(uri, method, data) {
        self.error(''); // Clear error message
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null,
            statusCode: {
                404: function () {
                    self.error("Page not found");
                },
            }
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            });
    }

    self.ThongTinNguoiDung = ko.observableArray();
    self.ListNguoiDungNotLogin = ko.observableArray();

    function getThongTinNguoiDung() {
        ajaxHelper(ThietLapApis + 'GetThongTinNguoiDungByIDNV?idnv=' + _IDNhanVien, 'GET').done(function (data) {
            self.ThongTinNguoiDung(data);
        });
    }

    getThongTinNguoiDung();

    function getListNguoiDungOrtherIDLogin() {
        ajaxHelper(ThietLapApis + 'GetListNguoiDungNotLogin?id_nd=' + _IDNguoiDung, 'GET').done(function (data) {
            self.ListNguoiDungNotLogin(data);
        });
    }
    getListNguoiDungOrtherIDLogin();

    self.ListChuyenNhanTien = ko.observableArray();
    self.TongTienNap = ko.observable();
    self.TongTienChuyen = ko.observable();
    self.TongTienDungGuiTien = ko.observable();
    self.TongTienNhan = ko.observable();
    self.TongTienConLai = ko.observable();

    function SearchChuyenNhanTien(isGoToNext = false) {
        $('.table_h').gridLoader();
        ajaxHelper(ThietLapApis + 'GetListChuyenNhanTien?currentpage=' + self.currentPage() + '&pageSize=' + self.pageSize() + '&id_nd=' + _IDNguoiDung,
            'GET').done(function (data) {
                $('.table_h').gridLoader({ show: false });
                self.ListChuyenNhanTien(data.lstChuyenNhan);
                self.TotalRecord(data.Rowcount);
                self.PageCount(data.pageCount);
                self.TongTienNap(data.TongTienNap);
                self.TongTienChuyen(data.TongTienChuyen);
                self.TongTienDungGuiTien(data.TongTienDungGuiTien);
                self.TongTienNhan(data.TongTienNhan);
                self.TongTienConLai(data.TongTienConLai);
            });
    }
    SearchChuyenNhanTien();

    self.PageResults = ko.computed(function () {
        if (self.ListChuyenNhanTien() !== null) {

            self.fromitem((self.currentPage() * self.pageSize()) + 1);

            if (((self.currentPage() + 1) * self.pageSize()) > self.ListChuyenNhanTien().length) {
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
                    for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                        var obj = {
                            pageNumber: j + 1,
                        };
                        arrPage.push(obj);
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

    self.GoToPageHD = function (page) {
        if (page.pageNumber !== '.') {
            self.currentPage(page.pageNumber - 1);
            SearchChuyenNhanTien(true);
        }
    };

    self.StartPage = function () {
        self.currentPage(0);
        SearchChuyenNhanTien(true);
    }

    self.BackPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
            SearchChuyenNhanTien(true);
        }
    }

    self.GoToNextPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.currentPage() + 1);
            SearchChuyenNhanTien(true);
        }
    }

    self.EndPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
            SearchChuyenNhanTien(true);
        }
    }
    self.GetClassHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage()) ? "click" : "";
    };

    self.LaAdmin = ko.observable();
    function CheckLaAdminND() {
        ajaxHelper(ThietLapApis + 'CheckAdminLogin?id_nd=' + _IDNguoiDung, 'GET').done(function (data) {
            self.LaAdmin(data);
            if (data === false) {
                //$('.thongtinnaptien').hide();
                $('.thongtinchuyentien').click();
            }
        });
    }
    CheckLaAdminND();

    self.ListNapTien = ko.observableArray();

    function GetListNapTienByCuaHang() {
        ajaxHelper(ThietLapApis + 'GetListNapTienByCuaHang?currentpage=' + self.currentPage1() + '&pageSize=' + self.pageSize1(), 'GET').done(function (data) {
            if (self.LaAdmin() === true) {
                self.ListNapTien(data.lst);
                self.TotalRecord1(data.Rowcount);
                self.PageCount1(data.pageCount);
            }
            else {
                self.ListNapTien([]);
                self.TotalRecord1(0);
                self.PageCount1(0);
            }
        });
    }
    GetListNapTienByCuaHang();

    self.PageResults1 = ko.computed(function () {
        if (self.ListNapTien() !== null) {

            self.fromitem1((self.currentPage1() * self.pageSize1()) + 1);

            if (((self.currentPage1() + 1) * self.pageSize1()) > self.ListNapTien().length) {
                var fromItem = (self.currentPage1() + 1) * self.pageSize1();
                if (fromItem < self.TotalRecord1()) {
                    self.toitem1((self.currentPage1() + 1) * self.pageSize1());
                }
                else {
                    self.toitem1(self.TotalRecord1());
                }
            } else {
                self.toitem1((self.currentPage1() * self.pageSize1()) + self.pageSize1());
            }
        }
    });

    self.PageList_Display1 = ko.computed(function () {
        var arrPage = [];

        var allPage = self.PageCount1();
        var currentPage = self.currentPage1();

        if (allPage > 4) {

            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPage1()) + 1;
            }
            else {
                i = self.currentPage1();
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
                    for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                        var obj = {
                            pageNumber: j + 1,
                        };
                        arrPage.push(obj);
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
        return arrPage;
    });

    self.VisibleStartPage1 = ko.computed(function () {
        if (self.PageList_Display1().length > 0) {
            return self.PageList_Display1()[0].pageNumber !== 1;
        }
    });

    self.VisibleEndPage1 = ko.computed(function () {
        if (self.PageList_Display1().length > 0) {
            return self.PageList_Display1()[self.PageList_Display1().length - 1].pageNumber !== self.PageCount1();
        }
    });

    self.GoToPageHD1 = function (page) {
        if (page.pageNumber !== '.') {
            self.currentPage1(page.pageNumber - 1);
            GetListNapTienByCuaHang(true);
        }
    };

    self.StartPage1 = function () {
        self.currentPage1(0);
        GetListNapTienByCuaHang(true);
    }

    self.BackPage1 = function () {
        if (self.currentPage1() > 1) {
            self.currentPage1(self.currentPage1() - 1);
            GetListNapTienByCuaHang(true);
        }
    }

    self.GoToNextPage1 = function () {
        if (self.currentPage1() < self.PageCount1() - 1) {
            self.currentPage1(self.currentPage1() + 1);
            GetListNapTienByCuaHang(true);
        }
    }

    self.EndPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
            SearchChuyenNhanTien(true);
        }
    }
    self.GetClassHD1 = function (page) {
        return ((page.pageNumber - 1) === self.currentPage1()) ? "click" : "";
    };

    self.showThongTinNap = function () {

    };

    self.showThongTinChuyen = function () {

    };

    self.showPopupChuyenTien = function () {
        $('#modalPopup_chuyentien').modal('show');
        self.newChuyenTien(new FormModel_ChuyenTien());
    };

    self.showpopupnaptien = function () {
        $('#modalPopup_naptien').modal('show');
        self.newNapTien(new FormModel_NapTien());
    }

    self.addChuyenTien = function () {
        var _idnguoinhan = self.newChuyenTien().ID_NguoiNhanTien();
        var _sotien = self.newChuyenTien().SoTien();
        var _ghiChu = self.newChuyenTien().GhiChu();

        if (_idnguoinhan === undefined) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng chọn người nhận tiền", "danger");
            return false;
        }

        if (_sotien === 0 || _sotien === "" || _sotien === undefined || _sotien === null) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng nhập số tiền", "danger");
            $('#txtSoTienNhapChuyen').select();
            return false;
        }

        if (formatNumberToInt(_sotien) > self.TongTienConLai()) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Tiền chuyển không được lớn hơn số dư hiện tại", "danger");
            $('#txtSoTienNhapChuyen').select();
            return false;
        }

        var ChuyenTien = {
            ID_NguoiChuyenTien: _IDNguoiDung,
            ID_NguoiNhanTien: _idnguoinhan,
            SoTien: _sotien,
            GhiChu: _ghiChu
        };

        var myData = {};
        myData.objChuyenTien = ChuyenTien;

        $.ajax({
            data: myData,
            url: ThietLapApis + "Post_ChuyenTienND",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (item) {
            },
            statusCode: {
                404: function () {
                    self.error("page not found");
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            },
            complete: function (item) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Chuyển tiền thành công", "success");
                $('#modalPopup_chuyentien').modal('hide');
                SearchChuyenNhanTien();
            }
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            });
    };

    self.addNapTien = function () {
        var _sotien = self.newNapTien().SoTien();
        var _ghiChu = self.newNapTien().GhiChu();

        if (_sotien === 0 || _sotien === "" || _sotien === undefined || _sotien === null) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng nhập số tiền", "danger");
            $('#txtSoTienNap').select();
            return false;
        }

        var NapTien = {
            TenKhachHang: userLogin,
            SoTien: _sotien,
            GhiChu: _ghiChu,
            TrangThai: 2
        };

        var myData = {};
        myData.objNapTien = NapTien;
        myData.ID_NguoiNhanTien = _IDNguoiDung;

        $.ajax({
            data: myData,
            url: ThietLapApis + "Post_NaptienND",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (item) {
            },
            statusCode: {
                404: function () {
                    self.error("page not found");
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            },
            complete: function (item) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Mạp tiền thành công", "success");
                $('#modalPopup_naptien').modal('hide');
                GetListNapTienByCuaHang();
            }
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            });
    }

    self.editThongTinNguoiDung = function () {
        $('#myModal').modal('show');
        $('.notyfi-check').css('display', 'none');
        document.getElementById("passNew").disabled = true;
        document.getElementById("passRepeat").disabled = true;
        $('#passNew').val("");
        $('#passRepeat').val("");
        $('#passOld').val("");

        document.getElementById("txtTenNhanVien").readOnly = true;
        document.getElementById("txtTenDangNhap").readOnly = true;
        document.getElementById("passOld").disabled = false;

        $('#clicksua').css('display', 'none');
        $('#clickluu').css('display', 'block');
    }
};

ko.applyBindings(new ViewModel());