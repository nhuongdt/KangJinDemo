// validate ky tu dac biet
function isValid(str) {
    return !/[~`!@#$%\^&*()+=\-\[\]\\';,/{}|\\":<>\?]/g.test(str);
};
var Key_Form = "Key_TuVan";
$('#selectColumn').on('click', '.dropdown-list li input[type = checkbox]', function (i) {
    var valueCheck = $(this).val();
    LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, i);
    $('.' + valueCheck).toggle();
});
function loadHtmlGrid() {
    LocalCaches.LoadFirstColumnGrid(Key_Form, $('#selectColumn ul li input[type = checkbox]'), []);
}
loadHtmlGrid();
var FormModel_LoaiTV = function () {
    var self = this;
    self.ID = ko.observable();
    self.TenLoaiTuVanLichHen = ko.observable();
    self.ID_NguoiTao = ko.observable();
    self.TuVan_LichHen = ko.observable();
    self.NgayTao = ko.observable();
    self.ID_NguoiSua = ko.observable();
    self.NgaySua = ko.observable();

    self.setdata = function (item) {
        self.ID(item.ID);
        self.TenLoaiTuVanLichHen(item.TenLoaiTuVanLichHen);
        self.ID_NguoiTao(item.ID_NguoiTao);
        self.TuVan_LichHen(item.TuVan_LichHen);
        self.NgayTao(item.NgayTao);
        self.ID_NguoiSua(item.ID_NguoiSua);
        self.NgaySua(item.NgaySua);
    }
};

var FormModel_NewTuVan = function () {
    var self = this;
    self.ID = ko.observable();
    self.ID_LoaiTuVan = ko.observable();
    self.ID_KhachHang = ko.observable();
    self.ID_NhanVien = ko.observable();
    self.Ma_TieuDe = ko.observable();
    var time = new Date();
    self.NgayGio = ko.observable(time);
    self.ThoiGianHenLai = ko.observable();
    self.TrangThai = ko.observable("1");
    self.NoiDung = ko.observable();
    self.TraLoi = ko.observable();

    self.SetData = function (item) {
        self.ID(item.ID);
        self.Ma_TieuDe(item.Ma_TieuDe);
        self.ID_LoaiTuVan(item.ID_LoaiTuVan);
        self.ID_KhachHang(item.ID_KhachHang);
        self.ID_NhanVien(item.ID_NhanVien);
        self.NgayGio(item.NgayGio);
        self.ThoiGianHenLai(item.ThoiGianHenLai);
        self.TrangThai(item.TrangThai);
        self.NoiDung(item.NoiDung);
        self.TraLoi(item.TraLoi);
    };
};
var ViewModel = function () {
    var CSKHUri = '/api/DanhMuc/ChamSocKhachHangAPI/';
    var DM_LoaiTVLHUri = '/api/DanhMuc/DM_LoaiTuVanLichHenAPI/';
    var DMDoiTuongUri = '/api/DanhMuc/DM_DoiTuongAPI/';
    var NhanVienUri = '/api/DanhMuc/NS_NhanVienAPI/';
    var self = this;

    var idNhanVien = $('.idnhanvien').text();
    var idDonVi = $('#hd_IDdDonVi').val();
    self.deletePhieuTuVan = ko.observable();
    self.deleteID = ko.observable();
    self._ThemLoaiTV = ko.observable(true);
    self.LoaiTuVanLichHens = ko.observableArray();

    self.deleteTenLoaiTuVan = ko.observable();

    self.filterNV = ko.observable();
    self.NhanViens = ko.observableArray();
    self.selectedNV = ko.observable();

    self.filterKH = ko.observable();
    self.DoiTuongs = ko.observableArray();
    self.selectedKH = ko.observable();

    self.ckThamKhao = ko.observable(true);
    self.ckTiemNang = ko.observable(true);
    //self.ckHoanThanh = ko.observable(true);
    self.ckHuy = ko.observable(true);

    self.filterNgayLapHD = ko.observable("0");
    self.filterNgayLapHD_Input = ko.observable(); // ngày cụ thể
    self.filterNgayLapHD_Quy = ko.observable(0); // Theo quý
    //phân trang
    self.pageSizes = [10, 20, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();
    self.arrPagging = ko.observableArray();


    self.error = ko.observable();
    self.filter = ko.observable();
    self.TuVans = ko.observableArray();
    self.LoaiTuVans = ko.observableArray();
    self.booleanAdd = ko.observable(true);
    self.selectedLoaiTuVan = ko.observable();
    self.newTuVan = ko.observable(new FormModel_NewTuVan());
    self.newLoaiTuVan = ko.observable(new FormModel_LoaiTV());

    //self.TrangThai = ko.observableArray([
    //    { name: "Hoàn thành", value: "1" },
    //    { name: "Tham khảo", value: "2" },
    //    { name: "Tiềm năng", value: "3" },
    //    { name: "Hủy", value: "4" }
    //]);
    //self.selectedTrangThai = ko.observable();

    self.resetTextBox = function () {
        self.newTuVan(new FormModel_NewTuVan());
        //self.selectedTrangThai(1);
        self.selectedLoaiTuVan(undefined);
        self.selectedKH(undefined);
        self.selectedNV(undefined);
        $('#txtKhachHang').text("--- Chọn khách hàng ---");
        $('#lstKhachHang span').each(function () {
            $(this).empty();
        });
        $('#txtNhanVienChiaSe').text("--- Chọn nhân viên ---");
        $('#lstNhanVien span').each(function () {
            $(this).empty();
        });
    }

    self.ListIDNhanVienQuyen = ko.observableArray();
    function LoadID_NhanVien() {
        ajaxHelper(CSKHUri + 'GetListNhanVienLienQuanByIDLoGin_inDepartment?idnvlogin=' + idNhanVien 
                + '&idChiNhanh=' + idDonVi + '&funcName=' + funcName, 'GET').done(function (data) {
            self.ListIDNhanVienQuyen(data);
            var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
            if ($.inArray('TuVan_ThemMoi', lc_CTQuyen) > -1) {
                $('.txtThemMoiTuVan').show();
            }
            else {
                $('.txtThemMoiTuVan').hide();
            }
            SearchTuVan();
        })
    }
    LoadID_NhanVien();

    self.LoadQuyen = function () {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('TuVan_Capnhat', lc_CTQuyen) > -1) {
            $('.txtTuVanCapNhat').show();
        }
        else {
            $('.txtTuVanCapNhat').hide();
        }
        if ($.inArray('TuVan_Xoa', lc_CTQuyen) > -1) {
            $('.txtTuVanXoa').show();
        }
        else {
            $('.txtTuVanXoa').hide();
        }

    }
    // Reset
    self.resetLoaiTVLH = function () {
        self.newLoaiTuVan(new FormModel_LoaiTV());

    }

    self.themmoituvan = function () {
        self.resetTextBox();
        self.booleanAdd(true);
        $('#myModaltuvan').modal('show');
        $('#txtThoiGianHenLaiTV').val("");
        $('#txtMaPhieu').focus();
    }

    self.editTuVan = function (item) {
        ajaxHelper(CSKHUri + "GetPhieuTuVan/" + item.ID, 'GET').done(function (data) {
            self.booleanAdd(false);
            self.newTuVan().SetData(data);
            self.newTuVan().TrangThai(data.TrangThai);
            self.selectedLoaiTuVan(data.ID_LoaiTuVan);
            self.selectedKH(data.ID_KhachHang);
            self.selectedNV(data.ID_NhanVien);
            $('#lstKhachHang span').each(function () {
                $(this).empty();
            });
            $('#lstNhanVien span').each(function () {
                $(this).empty();
            });
            if (data.ID_KhachHang !== null) {
                $('#txtKhachHang').text(data.TenKhachHang);
                $(function () {
                    $('span[id=spanCheckKhachHang_' + data.ID_KhachHang + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
                });
            }
            else {
                $('#txtKhachHang').text("--- Chọn khách hàng ---");
            }
            if (data.ID_NhanVien !== null) {
                $('#txtNhanVienChiaSe').text(data.TenNV);
                $(function () {
                    $('span[id=spanCheckNhanVien_' + data.ID_NhanVien + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
                });
            }
            else {
                $('#txtNhanVienChiaSe').text("--- Chọn nhân viên ---");
            }
            $('#txtNgayGio').val(moment(data.NgayGio, "YYYY-MM-DD HH:mm").format('DD/MM/YYYY HH:mm'));
            $('#txtThoiGianHenLaiTV').val(moment(data.ThoiGianHenLai, "YYYY-MM-DD HH:mm").format('DD/MM/YYYY HH:mm'));
        });
        $('#myModaltuvan').modal('show');
    }

    self.themmoiloaituvan = function () {
        self.resetLoaiTVLH();
        self._ThemLoaiTV(true);
        $('#myModalphanloai').modal('show');
    }

    self.editloaituvan = function (item) {
        ajaxHelper(DM_LoaiTVLHUri + "GetLoaiTuVan/" + this.selectedLoaiTuVan(), 'GET').done(function (data) {
            self.newLoaiTuVan().setdata(data);
            self._ThemLoaiTV(false);
        });
        $('#myModalphanloai').modal('show');
    }

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
                }
            }
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            });
    }

    self.MangLoaiTuVan = ko.observableArray();

    self.selectedLoaiTuVanFilter = function (item) {
        var arrLCV = [];
        for (var i = 0; i < self.MangLoaiTuVan().length; i++) {
            if ($.inArray(self.MangLoaiTuVan()[i], arrLCV) === -1) {
                arrLCV.push(self.MangLoaiTuVan()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrLCV) === -1) {
            self.MangLoaiTuVan.push(item);
        }
        SearchTuVan();
        // thêm dấu check vào đối tượng được chọn
        $('#selec-all-TuVan li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
            }
        });
        $('#choose_LoaiTuVan input').remove();
    }

    self.CloseTuVan = function (item) {
        self.MangLoaiTuVan.remove(item);
        if (self.MangLoaiTuVan().length === 0) {
            $('#choose_LoaiTuVan').append('<input type="text" id="dllTuVan" readonly="readonly" class="dropdown" placeholder="Chọn loại tư vấn">');
        }
        SearchTuVan();
        // remove checks
        $('#selec-all-TuVan li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }


    self.MangLoaiTuVanFilter = ko.observableArray();
    self.TotalRecord = ko.observableArray();
    self.PageCount = ko.observableArray();

    self.TodayBC = ko.observable();
    self.MangIDNhanVien = ko.observableArray();
    function SearchTuVan() {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('TuVan_XemDS', lc_CTQuyen) > -1) {
            var arrLCV = [];
            for (var i = 0; i < self.MangLoaiTuVan().length; i++) {
                if ($.inArray(self.MangLoaiTuVan()[i], arrLCV) === -1) {
                    arrLCV.push(self.MangLoaiTuVan()[i].ID);
                }
            }

            self.MangLoaiTuVanFilter(arrLCV);

            var arrIDNV = [];
            for (var i = 0; i < self.ListIDNhanVienQuyen().length; i++) {
                if ($.inArray(self.ListIDNhanVienQuyen()[i], arrIDNV) === -1) {
                    arrIDNV.push(self.ListIDNhanVienQuyen()[i]);
                }
            }
            self.MangIDNhanVien(arrIDNV);

            var statusInvoice = 1;
            if (self.ckHuy()) {
                if (self.ckTiemNang()) {
                    if (self.ckThamKhao()) {
                        statusInvoice = 6;
                    } else {
                        statusInvoice = 5;
                    }
                }
                else {
                    if (self.ckThamKhao()) {
                        statusInvoice = 4;
                    } else {
                        statusInvoice = 1; // HT
                    }
                }
            }
            else {
                if (self.ckTiemNang()) {
                    if (self.ckThamKhao()) {
                        statusInvoice = 0;
                    } else {
                        statusInvoice = 3;
                    }
                } else {
                    if (self.ckThamKhao()) {
                        statusInvoice = 2;
                    } else {
                        statusInvoice = 7;
                    }
                }
            }
            var _now = new Date();  //current date of week
            var currentWeekDay = _now.getDay();
            var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1;
            var dayStart = '';
            var dayEnd = '';

            if (self.filterNgayLapHD() === '0') {

                switch (self.filterNgayLapHD_Quy()) {
                    case 0:
                        // all
                        self.TodayBC('Toàn thời gian');
                        dayStart = '2016-01-01';
                        dayEnd = '9999-01-01';
                        break;
                    case 1:
                        // hom nay
                        self.TodayBC('Hôm nay');
                        dayStart = moment(_now).format('YYYY-MM-DD');
                        dayEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
                        break;
                    case 2:
                        // hom qua
                        self.TodayBC('Hôm qua');
                        dayEnd = moment(_now).format('YYYY-MM-DD');
                        dayStart = moment(new Date(_now.setDate(_now.getDate() - 1))).format('YYYY-MM-DD');
                        break;
                    case 3:
                        // tuan nay
                        self.TodayBC('Tuần này');
                        dayStart = moment(new Date(_now.setDate(_now.getDate() - lessDays - 1))).format('YYYY-MM-DD'); // start of wwek
                        dayEnd = moment(new Date(_now.setDate(_now.getDate() + 6))).add('days', 1).format('YYYY-MM-DD'); // end of week
                        break;
                    case 4:
                        // tuan truoc
                        self.TodayBC('Tuần trước');
                        dayStart = moment().weekday(-6).format('YYYY-MM-DD');
                        dayEnd = moment(dayStart, 'YYYY-MM-DD').add(6, 'days').add('days', 1).format('YYYY-MM-DD'); // add day in moment.js
                        break;
                    case 5:
                        // 7 ngay qua
                        self.TodayBC('7 ngày qua');
                        dayEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
                        dayStart = moment(new Date(_now.setDate(_now.getDate() - 7))).format('YYYY-MM-DD');
                        break;
                    case 6:
                        // thang nay
                        self.TodayBC('Tháng này');
                        dayStart = moment(new Date(_now.getFullYear(), _now.getMonth(), 1)).format('YYYY-MM-DD');
                        dayEnd = moment(new Date(_now.getFullYear(), _now.getMonth() + 1, 0)).add('days', 1).format('YYYY-MM-DD');
                        break;
                    case 7:
                        // thang truoc

                        self.TodayBC('Tháng trước');
                        dayStart = moment(new Date(_now.getFullYear(), _now.getMonth() - 1, 1)).format('YYYY-MM-DD');
                        dayEnd = moment(new Date(_now.getFullYear(), _now.getMonth(), 0)).add('days', 1).format('YYYY-MM-DD');
                        break;
                    case 10:
                        // quy nay
                        self.TodayBC('Quý này');
                        dayStart = moment().startOf('quarter').format('YYYY-MM-DD');
                        dayEnd = moment().endOf('quarter').add('days', 1).format('YYYY-MM-DD');
                        break;
                    case 11:
                        // quy truoc = currQuarter -1; // if (currQuarter -1 == 0) --> (assign = 1)
                        self.TodayBC('Quý trước');
                        var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
                        dayStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                        dayEnd = moment().quarter(prevQuarter).endOf('quarter').add('days', 1).format('YYYY-MM-DD');
                        break;
                    case 12:
                        // nam nay
                        self.TodayBC('Năm nay');
                        dayStart = moment().startOf('year').format('YYYY-MM-DD');
                        dayEnd = moment().endOf('year').add('days', 1).format('YYYY-MM-DD');
                        break;
                    case 13:
                        // nam truoc
                        self.TodayBC('Năm trước');
                        var prevYear = moment().year() - 1;
                        dayStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                        dayEnd = moment().year(prevYear).endOf('year').add('days', 1).format('YYYY-MM-DD');
                        break;
                }
            }
            else {
                // chon ngay cu the
                var arrDate = self.filterNgayLapHD_Input().split('-');
                dayStart = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
                dayEnd = moment(arrDate[1], 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD');
                self.TodayBC('Từ ' + moment(arrDate[0], 'DD/MM/YYYY').format('DD/MM/YYYY') + ' đến ' + moment(arrDate[1], 'DD/MM/YYYY').format('DD/MM/YYYY'));
            }
            $('.table-reponsive').gridLoader();
            ajaxHelper(CSKHUri + 'GetAllTuVanWhere?currentPage=' + self.currentPage() + '&pageSize=' + self.pageSize() + '&arrLoaiTuVan=' + self.MangLoaiTuVanFilter() + '&dayStart=' + dayStart + '&dayEnd=' + dayEnd + '&txtSearch=' + self.filter() + '&trangthai=' + statusInvoice + '&arrMangIDNhanVien=' + self.MangIDNhanVien() + '&iddonvi=' + idDonVi, 'GET').done(function (data) {
                self.TuVans(data.LstData);
                self.TotalRecord(data.TotalRow);
                self.PageCount(data.PageCount);
                $('.table-reponsive').gridLoader({ show: false });
            })
        }
    }

    self.clickSearchTV = function () {
        SearchTuVan();
    }

    self.ckHuy.subscribe(function (newVal) {
        self.currentPage(0);
        SearchTuVan();
    });

    self.ckThamKhao.subscribe(function (newVal) {
        self.currentPage(0);
        SearchTuVan();
    });

    self.ckTiemNang.subscribe(function (newVal) {
        self.currentPage(0);
        SearchTuVan();
    });

    $('#txtFilterTuVan').keypress(function (e) {
        if (e.keyCode === 13) {
            self.currentPage(0);
            SearchTuVan();
        }
    })
    self.ResetCurrentPage = function () {
        self.currentPage(0);
        SearchTuVan();
    };

    self.filterNgayLapHD.subscribe(function (newVal) {
        self.currentPage(0);
        SearchTuVan();
    });
    $('#txtNgayTaoInput').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        SearchTuVan();
    });

    $('.choseNgayTao li').on('click', function () {
        $('#txtNgayTao').val($(this).text());
        self.filterNgayLapHD_Quy($(this).val());
        self.currentPage(0);
        SearchTuVan();
    });

    self.PageResults = ko.computed(function () {
        if (self.TuVans() !== null) {

            self.fromitem((self.currentPage() * self.pageSize()) + 1);

            if (((self.currentPage() + 1) * self.pageSize()) > self.TuVans().length) {
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
                    if (currentPage == 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
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
            SearchTuVan();
        }
    };

    self.StartPage = function () {
        self.currentPage(0);
        SearchTuVan();
    }

    self.BackPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
            SearchTuVan();
        }
    }

    self.GoToNextPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.currentPage() + 1);
            SearchTuVan();
        }
    }

    self.EndPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
            SearchTuVan();
        }
    }
    self.GetClassHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage()) ? "click" : "";
    };

    function getAllDMLoaiTuVanLichHens() {
        ajaxHelper(DM_LoaiTVLHUri + "GetDM_LoaiTuVan", 'GET').done(function (data) {
            self.LoaiTuVanLichHens(data);
        });
    }
    getAllDMLoaiTuVanLichHens();

    self.addLoaiTuVanLichHen = function (formElement) {
        var _idLoaiTuVanLichHen = self.newLoaiTuVan().ID();
        var _tenLoaiTuVanLichHen = self.newLoaiTuVan().TenLoaiTuVanLichHen();

        if (_tenLoaiTuVanLichHen == null || _tenLoaiTuVanLichHen == "" || _tenLoaiTuVanLichHen == "undefined") {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Không được để trống tên loại tư vấn!", "danger");
            $('#txtTenLoaiTuVan').focus();
            return false;
        }
        var objLoaiTuVan = {
            ID: _idLoaiTuVanLichHen,
            TenLoaiTuVanLichHen: _tenLoaiTuVanLichHen
        };
        if (self._ThemLoaiTV() === true) {
            var myData = {};
            myData.objLoaiTVLH = objLoaiTuVan;
            //ajaxHelper(DM_LoaiTVLHUri + "Check_TenLoaiTuVanLichHenExist?tenLoaiTuVan=" + _tenLoaiTuVanLichHen, 'POST').done(function (data) {
            //    if (data) {
            //        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Tên loại tư vấn đã tồn tại!", "danger");
            //        getAllDMLoaiTuVanLichHens();
            //        return false;
            //    }
            //else {
            $.ajax({
                url: DM_LoaiTVLHUri + "PostLoaiTuVan",
                type: 'POST',
                dataType: 'json',
                data: myData,
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (item) {
                    self.LoaiTuVanLichHens.push(item);
                    self.selectedLoaiTuVan(item.ID);
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Thêm mới loại tư vấn thành công!", "success");
                },
                statusCode: {
                    404: function () {
                        self.error("page not found");
                    },
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    $("#myModalphanloai").modal("hide");
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Thêm mới loại tư vấn thất bại!", "danger");
                },
                complete: function () {
                    $("#myModalphanloai").modal("hide");
                    self.resetLoaiTVLH();

                }
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    $("#myModalphanloai").modal("hide");
                });
            //    }
            //});
        }
        else {
            var myData = {};
            myData.id = _idLoaiTuVanLichHen;
            myData.objLoaiTVLH = objLoaiTuVan;
            $.ajax({
                url: DM_LoaiTVLHUri + "PutLoaiTuVan",
                type: 'PUT',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: myData,
                success: function () {
                    $("#myModalphanloai").modal("hide");
                    getAllDMLoaiTuVanLichHens();
                },
                statusCode: {
                    404: function () {
                        self.error("page not found");
                    },
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                },
                complete: function () {
                    bottomrightnotify('Cập nhật loại tư vấn thành công !', 'success');
                }
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                });
        }
    }

    //THem/Sua
    self.addPhieuTuVan = function (formElement) {

        var _matieude = self.newTuVan().Ma_TieuDe();
        _matieude = _matieude === "" ? undefined : (_matieude === null ? undefined : _matieude);
        var _id = self.newTuVan().ID();
        var _idloatv = self.selectedLoaiTuVan();
        var _idkhachhang = self.selectedKH();
        var _idnhanvien = self.selectedNV();
        var _ngaygio = $('#txtNgayGio').val();
        var _ngaygiohenlai = $('#txtThoiGianHenLaiTV').val();
        var _noidung = self.newTuVan().NoiDung();
        var _traloi = self.newTuVan().TraLoi();
        var _trangthai = self.newTuVan().TrangThai();
        if (!isValid(_matieude)) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Mã phiếu không được nhập kí tự đặc biệt!", "danger");
            return false;
        }
        if (_idkhachhang === undefined) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng chọn khách hàng", "danger");
            $('#txtKhachHang').focus();
            return false;
        }

        if (moment(_ngaygio, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') === "Invalid date") {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng chọn ngày tư vấn", "danger");
            $('#txtNgayGio').select();
            return false;
        }

        if (_idnhanvien === undefined) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng chọn nhân viên tư vấn", "danger");
            $('#txtAuto').focus();
            return false;
        }

        if (_idloatv === undefined || _idloatv === null) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng chọn loại tư vấn", "danger");
            $('#ddlLoaiTVLH1').focus();
            return false;
        }

        if ((moment(_ngaygio, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') > moment(_ngaygiohenlai, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm')) && moment(_ngaygiohenlai, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') !== "Invalid date") {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Thời gian hẹn lại phải lớn hơn thời gian bắt đầu", "danger");
            $('#txtThoiGianHenLaiTV').focus();
            return false;
        }

        var Tu_Van = {
            ID: _id,
            Ma_TieuDe: _matieude,
            NgayGio: moment(_ngaygio, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm'),
            ThoiGianHenLai: moment(_ngaygiohenlai, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') === "Invalid date" ? null : moment(_ngaygiohenlai, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm'),
            ID_LoaiTuVan: _idloatv,
            ID_KhachHang: _idkhachhang,
            ID_NhanVien: _idnhanvien,
            NoiDung: _noidung,
            TraLoi: _traloi,
            TrangThai: _trangthai
        };
        if (self.booleanAdd() === true) {
            var myData = {};
            myData.objTuVan = Tu_Van
            // check Exist code, phone (check in server code)
            $.ajax({
                data: Tu_Van,
                url: CSKHUri + "Check_Exist",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",

                success: function (item) {
                    // khong dong popup neu item !== ''
                    if (item === '') {
                        callAjaxAdd(myData);
                    }
                    else {
                        bottomrightnotify(item, "danger");
                    }
                },
                statusCode: {
                    404: function () {
                        self.error("page not found");
                    },
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                },
            })
        }
        //Sua
        else {
            var myData = {};
            myData.id = _id;
            myData.objTuVan = Tu_Van;
            $.ajax({
                data: Tu_Van,
                url: CSKHUri + "Check_Exist",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",

                success: function (item) {
                    // khong dong popup neu item !== ''
                    if (item === '') {
                        callAjaxUpdate(myData);
                    }
                    else {
                        bottomrightnotify(item, "danger");
                    }
                },
                statusCode: {
                    404: function () {
                        self.error("page not found");
                    },
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                },
            })

        }
    };

    function callAjaxAdd(myData) {
        $.ajax({
            data: myData,
            url: CSKHUri + "PostPhieuTuVan",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (item) {
                //self.TuVans.unshift(item);
                bottomrightnotify("Thêm mới phiếu tư vấn thành công!", "success");
                SearchTuVan();
            },
            statusCode: {
                404: function () {
                    self.error("page not found");
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                bottomrightnotify("Thêm mới phiếu tư vấn thất bại!", "danger");
            },
            complete: function () {
                $("#myModaltuvan").modal("hide");
            }
        })
    }

    function callAjaxUpdate(myData) {
        //console.log('myData' + JSON.stringify(myData))
        $.ajax({
            url: CSKHUri + "PutPhieuTuVan",
            type: 'PUT',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myData,
            success: function () {
                $("#myModaltuvan").modal("hide");
                SearchTuVan();
            },
            statusCode: {
                404: function () {
                    self.error("page not found");
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            },
            complete: function () {
                bottomrightnotify('Cập nhật phiếu tư vấn thành công !', 'success');
            }
        })
    }

    self.selectedLoaiCV = ko.observable();
    self.selectedLoaiCV.subscribe(function (newValue) {
    })

    self.modalDelete = function (item) {
        self.deletePhieuTuVan(item.Ma_TieuDe);
        self.deleteID(item.ID);
        $('#modalpopup_deleteTuVan').modal('show');
    };

    self.modalDeleteLoaiTuVan = function (LoaiTuVans) {
        self.deleteTenLoaiTuVan(self.newLoaiTuVan().TenLoaiTuVanLichHen());
        self.deleteID(self.newLoaiTuVan().ID());
        $('#modalpopup_deleteLoaiTuVan').modal('show');
    };


    self.XoaTuVanLichSu = function (tieude) {
        var objDiary = {
            ID_NhanVien: idNhanVien,
            ID_DonVi: idDonVi,
            ChucNang: "Tư vấn",
            NoiDung: "Xóa tư vấn : " + tieude,
            NoiDungChiTiet: "Xóa tư vấn : " + tieude,
            LoaiNhatKy: 3 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        var myData = {};
        myData.objDiary = objDiary;
        $.ajax({
            url: '/api/DanhMuc/SaveDiary/' + "post_NhatKySuDung",
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
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại", "danger");
            },
            complete: function () {

            }
        })
    }
    //Xóa tu van
    self.xoaTuVan = function (TuVans) {
        $.ajax({
            type: "DELETE",
            url: CSKHUri + "Delete_PhieuTuVan/" + TuVans.deleteID(),
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa phiếu tư vấn thành công !", "success");
                self.XoaTuVanLichSu(TuVans.deletePhieuTuVan());
                SearchTuVan();
            },
            error: function (error) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Xóa phiếu tư vấn thất bại!", "danger");
            }
        })
    };
    // Xoa loai tu van
    self.xoaLoaiTuVan = function () {
        //console.log(JSON.stringify(LoaiTuVans));
        $.ajax({
            type: "DELETE",
            url: DM_LoaiTVLHUri + "Delete_LoaiTuVan/" + self.newLoaiTuVan().ID(),
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa loại tư vấn thành công !", "success");
                getAllDMLoaiTuVanLichHens();
            },
            error: function (error) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Xóa loại tư vấn thất bại!", "danger");
            }
        })
    };

    //auto search khách hàng
    function getAllDMDoiTuong() {
        ajaxHelper(DMDoiTuongUri + "GetListKhachHang?loaiDoiTuong=1", 'GET').done(function (data) {
            self.DoiTuongs(data);
        });
    }
    getAllDMDoiTuong();

    // select id khách hàng được chọn
    self.selectedChooseKhachHang = ko.observable();
    self.selectedChooseKhachHang.subscribe(function (newValue) {
        if (newValue !== undefined) {
            self.selectedKH(newValue.ID);
            $('#txtKhachHang').text(newValue.TenDoiTuong);
            $('#lstKhachHang span').each(function () {
                $(this).empty();
            });
            $(function () {
                $('span[id=spanCheckKhachHang_' + newValue.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
            });
        }
    })
    //khách hàng filter
    self.filterKhachHang = ko.observable();
    self.arrFilterKhachHang = ko.computed(function () {
        var _filter = self.filterKhachHang();

        return arrFilter = ko.utils.arrayFilter(self.DoiTuongs(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenDoiTuong).toLowerCase().split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            var arr1 = locdau(prod.MaDoiTuong).toLowerCase().split(/\s+/);
            var sSearch1 = '';

            for (var i = 0; i < arr1.length; i++) {
                sSearch1 += arr1[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = (locdau(prod.TenDoiTuong).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    sSearch.indexOf(locdau(_filter).toLowerCase()) >= 0 || locdau(prod.MaDoiTuong).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    sSearch1.indexOf(locdau(_filter).toLowerCase()) >= 0
                );
            }
            return chon;
        });
    });

    self.clickFistChonKhachHang = function () {
        self.selectedKH(undefined);
        $('#lstKhachHang span').each(function () {
            $(this).empty();
        });
        $('#txtKhachHang').text('--- Chọn khách hàng ---');
    };

    //auto search nhân viên
    function getAllNSNhanVien() {
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetNS_NhanVien_DaTaoND?idDonVi=" + idDonVi, 'GET').done(function (data) {
            if (data !== null) {
                self.NhanViens(data);
            }
        });
    }
    getAllNSNhanVien();

    // select nhân viên được chọn
    self.selectedChooseNhanVien = ko.observable();
    self.selectedChooseNhanVien.subscribe(function (newValue) {
        if (newValue !== undefined) {
            self.selectedNV(newValue.ID);
            $('#txtNhanVienChiaSe').text(newValue.TenNhanVien);
            $('#lstNhanVien span').each(function () {
                $(this).empty();
            });
            $(function () {
                $('span[id=spanCheckNhanVien_' + newValue.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>');
            });
        }
    })
    //nhân viên chia sẻ
    self.filterNhanVien = ko.observable();
    self.arrFilterNhanVien = ko.computed(function () {
        var _filter = self.filterNhanVien();

        return arrFilter = ko.utils.arrayFilter(self.NhanViens(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenNhanVien).toLowerCase().split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            var arr1 = locdau(prod.MaNhanVien).toLowerCase().split(/\s+/);
            var sSearch1 = '';

            for (var i = 0; i < arr1.length; i++) {
                sSearch1 += arr1[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = (locdau(prod.TenNhanVien).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    sSearch.indexOf(locdau(_filter).toLowerCase()) >= 0 || locdau(prod.MaNhanVien).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    sSearch1.indexOf(locdau(_filter).toLowerCase()) >= 0
                );
            }
            return chon;
        });
    });

    self.clickFistChonNhanVien = function () {
        self.selectedNV(undefined);
        $('#lstNhanVien span').each(function () {
            $(this).empty();
        });
        $('#txtNhanVienChiaSe').text('--- Chọn nhân viên ---');
    }

    function GetTuVanByLoaiTV(id) {
        ajaxHelper(CSKHUri + "GetListTuVanByLoaiTuVan/" + id, 'GET').done(function (data) {
            self.TuVans(data);
        });
    }
    self.changeddlLoaiTuVan = function (item) {
        //console.log('idloaituvan: ' + item.ID);
        GetTuVanByLoaiTV(item.ID);
    }


    function locdau(obj) {
        if (!obj)
            return "";
        var str = obj;
        str = str.toString().toLowerCase();
        str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
        str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
        str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
        str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
        str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
        str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
        str = str.replace(/đ/g, "d");
        str = str.replace(/^\-+|\-+$/g, "");
        return str;
    }



    // getList loại tư vấn
    //GetListLoaiTuVanHoa
    self.LoaiTuVans = ko.observableArray();
    function getLoaiTuVan() {
        ajaxHelper(DM_LoaiTVLHUri + "GetDM_LoaiTuVan", 'GET').done(function (data) {
            self.LoaiTuVans(data);
            console.log(data);
        })
    }
    getLoaiTuVan();
    self.NoteLoaiTuVan = function () {
        var tk = $('#SeachLoaiTuVan').val();
        console.log(tk);
        if (tk.trim() != '') {
            ajaxHelper(DM_LoaiTVLHUri + "GetDM_LoaiTuVanWhere?TenLoaiTV=" + tk, 'GET').done(function (data) {
                self.LoaiTuVans(data);
                console.log(data);
            })
        }
        else {
            ajaxHelper(DM_LoaiTVLHUri + "GetDM_LoaiTuVan", 'GET').done(function (data) {
                self.LoaiTuVans(data);
                console.log(data);
            })
        }
    }
    self.SelectRepoert_LoaiTuVan = function (item) {
        _ID_LoaiTuVan = item.ID;
        $('.SelectLoaiTuVan li').each(function () {
            if ($(this).attr('id_LoaiTuVan') === item.ID) {
                $(this).addClass('SelectReport');
            }
            else {
                $(this).removeClass('SelectReport');
            }
        });
        $('.SelectALLLoaiTuVan li').removeClass('SelectReport');
        _pageNumber = 1;
        console.log(_ID_LoaiTuVan);
        //self.getListReportHangBan_NhanVien();
    }
    $('.SelectALLLoaiTuVan').on('click', function () {
        $('.SelectALLLoaiTuVan li').addClass('SelectReport')
        $('.SelectLoaiTuVan li').each(function () {
            $(this).removeClass('SelectReport');
        });
        _ID_LoaiTuVan = null;
        _pageNumber = 1;
        //self.getListReportHangBan_NhanVien();
    });
    // Key Event maHH
    var _maPhieu = null
    self.SelectMaHH = function () {
        _maPhieu = $('#txtMaHH').val();
        console.log(_maPhieu);
    }
    $('#txtMaHH').keypress(function (e) {
        if (e.keyCode == 13) {

        }
    })
};

ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {

        //initialize datepicker with some optional options
        var options = {
            format: 'DD/MM/YYYY HH:mm',
            defaultDate: new Date()
        };

        if (allBindingsAccessor() !== undefined) {
            if (allBindingsAccessor().datepickerOptions !== undefined) {
                options.format = allBindingsAccessor().datepickerOptions.format !== undefined ? allBindingsAccessor().datepickerOptions.format : options.format;
            }
        }

        $(element).datetimepicker(options);

        //when a user changes the date, update the view model
        ko.utils.registerEventHandler(element, "dp.change", function (event) {
            var value = valueAccessor();
            if (ko.isObservable(value)) {
                value(event.date);
            }
        });

        var defaultVal = $(element).val();
        var value = valueAccessor();
        value(moment(defaultVal, options.format));
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var thisFormat = 'DD/MM/YYYY HH:mm';

        if (allBindingsAccessor() !== undefined) {
            if (allBindingsAccessor().datepickerOptions !== undefined) {
                thisFormat = allBindingsAccessor().datepickerOptions.format !== undefined ? allBindingsAccessor().datepickerOptions.format : thisFormat;
            }
        }

        var value = valueAccessor();
        var unwrapped = ko.utils.unwrapObservable(value());

        if (unwrapped === undefined || unwrapped === null) {
            element.value = new moment(new Date()).format(thisFormat);
        } else {
            element.value = moment(unwrapped).format(thisFormat);
        }
    }


};
ko.applyBindings(new ViewModel());

$('.daterange').daterangepicker({
    locale: {
        "format": 'DD/MM/YYYY',
        "separator": " - ",
        "applyLabel": "Tìm kiếm",
        "cancelLabel": "Hủy",
        "fromLabel": "Từ",
        "toLabel": "Đến",
        "customRangeLabel": "Custom",
        "daysOfWeek": [
            "CN",
            "T2",
            "T3",
            "T4",
            "T5",
            "T6",
            "T7"
        ],
        "monthNames": [
            "Tháng 1",
            "Tháng 2",
            "Tháng 3",
            "Tháng 4",
            "Tháng 5",
            "Tháng 6",
            "Tháng 7",
            "Tháng 8",
            "Tháng 9",
            "Tháng 10",
            "Tháng 11",
            "Tháng 12"
        ],
        "firstDay": 1
    }
});