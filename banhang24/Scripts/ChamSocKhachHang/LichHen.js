// validate ky tu dac biet
function isValid(str) {
    return !/[~`!@#$%\^&*()+=\-\[\]\\';,/{}|\\":<>\?]/g.test(str);
};
var Key_Form = "Key_LichHen";
$('#selectColumn').on('click', '.dropdown-list li input[type = checkbox]', function (i) {
    var valueCheck = $(this).val();
    LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, i);
    $('.' + valueCheck).toggle();
});
function loadHtmlGrid() {
    LocalCaches.LoadFirstColumnGrid(Key_Form, $('#selectColumn ul li input[type = checkbox]'), []);
}
loadHtmlGrid();
var FormModel_LoaiLH = function () {
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

var FormModel_NewLichHen = function () {
    var self = this;
    self.ID = ko.observable();
    self.ID_KhachHang = ko.observable();
    self.ID_NhanVien = ko.observable();
    self.Ma_TieuDe = ko.observable();
    self.NgayGio = ko.observable();
    self.NgayGioKetThuc = ko.observable();
    self.TrangThai = ko.observable("1");
    self.NoiDung = ko.observable();// ghi chu
    self.NhacNho = ko.observableArray();//nhac nho

    self.SetData = function (item) {
        self.ID(item.ID);
        self.Ma_TieuDe(item.Ma_TieuDe);
        self.ID_KhachHang(item.ID_KhachHang);
        self.ID_NhanVien(item.ID_NhanVien);
        self.NgayGio(item.NgayGio);
        self.NgayGioKetThuc(item.NgayGioKetThuc);
        self.TrangThai(item.TrangThai);
        self.NoiDung(item.NoiDung);
        self.NhacNho(item.NhacNho);
    };
};

var ViewModel = function () {

    var CSKHUri = '/api/DanhMuc/ChamSocKhachHangAPI/';
    var DMDoiTuongUri = '/api/DanhMuc/DM_DoiTuongAPI/';
    var NhanVienUri = '/api/DanhMuc/NS_NhanVienAPI/';
    var DM_LoaiTVLHUri = '/api/DanhMuc/DM_LoaiTuVanLichHenAPI/';
    var self = this;
    $('#info').removeClass("active");
    $('#home').addClass("active");
    self.check_kieubang = ko.observable('1');
    $('.chose_kieubang li').on('click', function () {
        self.check_kieubang($(this).val());
        self.getluoi();
    })
    var thisDate;
    self.RowsStart = ko.observable('0');
    self.RowsEnd = ko.observable('15');
    var _IDchinhanh = JSON.parse(localStorage.getItem('lc_CTThieTLap')).ID_DonVi; // lấy ID chi nhánh _header.cshtml
    var _ID_ChiNhanh = JSON.parse(localStorage.getItem('lc_CTThieTLap')).ID_DonVi;
    console.log(_IDchinhanh);
    self.error = ko.observable();

    var idNhanVien = $('.idnhanvien').text();
    var idDonVi = $('#hd_IDdDonVi').val();
    self.ListIDNhanVienQuyen = ko.observableArray();
    function LoadID_NhanVien() {
        ajaxHelper(CSKHUri + 'GetListNhanVienLienQuanByIDLoGin_inDepartment?idnvlogin=' + idNhanVien
            + '&idChiNhanh=' + idDonVi + '&funcName=' + funcName, 'GET').done(function (data) {
                self.ListIDNhanVienQuyen(data);
                var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
                if ($.inArray('LichHen_ThemMoi', lc_CTQuyen) > -1) {
                    $('.txtLichHenThemMoi').show();
                }
                else {
                    $('.txtLichHenThemMoi').hide();
                }
                SearchLichHen();
            });
    }
    LoadID_NhanVien();

    self.LoadQuyen = function () {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('LichHen_CapNhat', lc_CTQuyen) > -1) {
            $('.txtLichHenCapNhat').show();
        }
        else {
            $('.txtLichHenCapNhat').hide();
        }
        if ($.inArray('LichHen_Xoa', lc_CTQuyen) > -1) {
            $('.txtLichHenXoa').show();
        }
        else {
            $('.txtLichHenXoa').hide();
        }

    }

    self.DMLichHens = ko.observableArray();
    self.TongHangLichHens = ko.observableArray();
    self.TongTrangLichHens = ko.observableArray();
    self.NgayTaoLH = ko.observable('0');
    self.NgaypageLH = ko.observable();
    self.filterNgayLichHen = ko.observable('0');
    var datime = new Date();
    var _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
    var _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
    //var numberPage = 1;
    var _pageNumber = 1
    var _pageSize = 10;
    self.pageSizes = [10, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();
    self.arrPagging = ko.observableArray();
    self.filterNgayLapHD = ko.observable("0");
    self.filterNgayLapHD_Input = ko.observable(); // ngày cụ thể
    self.filterNgayLapHD_Quy = ko.observable(0); // Theo quý
    //phân trang
    self.TodayBC = ko.observable("Tháng này");
    var _rdoNgayPage = '0';
    var nextPage = 1;
    var AllPage = 1;
    self.ckTrangthai = ko.observable();
    var _rdTrangThai = '';
    var SuKien = null;
    self.checkThamKhao = ko.observable(true);
    self.checkTiemNang = ko.observable(true);
    self.checkHuy = ko.observable(true);
    self.PhanLoaiLichHen = ko.observableArray();
    var _giatriSeach = '';
    self.filterDMLichHen = ko.observableArray();
    var _TenLoaiTV = '';
    var _trangthai1 = 1;
    var _trangthai2 = 2;
    var _trangthai3 = 3;
    var _allPhanLoai = 1;
    self.filterDangBang = ko.observable('2');
    self.filterDangLuoi = ko.observable(true);
    self.TieuDeLichHen = ko.observable('Cập nhật lịch hẹn');
    let start = moment().startOf('week').format('YYYY-MM-DD');
    let end = moment().endOf('week').format('YYYY-MM-DD');
    self.dateloadLuoiStart = ko.observable(start);
    self.dateloadluoiEnd = ko.observable(end);
    self.CalendarHeader = ko.observable('2019 -11')
    self.selecteMaTieuDe = ko.observable();

    self.deleteLichHen = ko.observable();
    self.deleteID = ko.observable();
    self.LoaiTuVanLichHens = ko.observableArray();

    self.NhanViens = ko.observableArray();
    self.selectedNV = ko.observable();
    self.filterNV = ko.observable();


    self.selectedKH = ko.observable();
    self.DoiTuongs = ko.observableArray();
    self.filterKH = ko.observable();

    self.ckThamKhao = ko.observable(true);
    self.ckTiemNang = ko.observable(true);
    //self.ckHoanThanh = ko.observable(true);
    self.ckHuy = ko.observable(true);

    self.deleteTenLoaiLichHen = ko.observable();
    self.filter = ko.observable();
    self.LichHens = ko.observableArray();
    self.LoaiLichHens = ko.observableArray();
    self.selectedLoaiLichHen = ko.observable();
    self.booleanAdd = ko.observable(true);
    self._ThemLoaiLH = ko.observable(true);
    self.newLichHen = ko.observable(new FormModel_NewLichHen());
    self.newLoaiLichHen = ko.observable(new FormModel_LoaiLH());


    //self.TrangThai = ko.observableArray([
    //    { name: "Chưa thực hiện", value: "1" },
    //    { name: "Đang thực hiện", value: "2" },
    //    { name: "Hoàn thành", value: "3" },
    //    { name: "Hủy", value: "4" }
    //]);
    //self.selectedTrangThai = ko.observable();

    //$(function () {
    //    var lc_LichHenNgay = localStorage.getItem("lc_LichHenNgay");
    //    var mang = JSON.parse(lc_LichHenNgay);
    //});


    self.NhacNho = ko.observableArray([
        { name: "5 phút", value: "5" },
        { name: "10 phút", value: "10" },
        { name: "15 phút", value: "15" },
        { name: "30 phút", value: "30" },
        { name: "1 giờ", value: "60" },
        { name: "2 giờ", value: "120" },
        { name: "3 giờ", value: "180" },
        { name: "6 giờ", value: "360" },
        { name: "12 giờ", value: "720" },
        { name: "24 giờ", value: "1440" }
    ]);
    self.selectedNhacNho = ko.observable();

    self.themmoilichhen = function () {
        self.resetTextBox();
        self.booleanAdd(true);
        self.selectedKH(undefined);
        self.selectedNV(undefined);
        self.TieuDeLichHen("Thêm mới lịch hẹn");
        $('#ngaybd').val("");
        $('#ngaykt').val("");
        $('#myModallichhen').modal('show');
        $('.modal-backdrop').hide();
        $('#txtTieuDe').focus();
        $('#txtNhanVienChiaSe').text("--- Chọn nhân viên ---");
        $('#lstNhanVien span').each(function () {
            $(this).empty();
        });

        $('#txtKhachHang').text(" --- Chọn khách hàng ---");
        $('#lstKhachHang span').each(function () {
            $(this).empty();
        });

    }

    self.editLichHen = function (item) {
        ajaxHelper(CSKHUri + "GetLichHen/" + item.ID, 'GET').done(function (data) {
            $('#myModallichhen').modal('show');
            $('.modal-backdrop').hide();
            self.newLichHen().SetData(data);
            self.booleanAdd(false);
            self.newLichHen().TrangThai(data.TrangThai);
            self.selectedLoaiLichHen(data.ID_LoaiTuVan);
            self.selectedNhacNho(data.NhacNho);
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
            $('#ngaybd').val(moment(data.NgayGio, "YYYY-MM-DD HH:mm").format('DD/MM/YYYY HH:mm'));
            $('#ngaykt').val(moment(data.NgayGioKetThuc, "YYYY-MM-DD HH:mm").format('DD/MM/YYYY HH:mm'));
            self.TieuDeLichHen('Cập nhật: ' + data.Ma_TieuDe);
        });
        //self.TieuDeLichHen('Cập nhật lịch hẹn');
    };
    // phương
    self.CapNhatLichHen = function (id) {
        //ajaxHelper(CSKHUri + "GetLichHen/" + id, 'GET').done(function (data) {
        //    self.newLichHen().SetData(data);
        //    self.booleanAdd(false);
        //    self.newLichHen().TrangThai(data.TrangThai);
        //    self.selectedLoaiLichHen(data.ID_LoaiTuVan);
        //    self.selectedNhacNho(data.NhacNho);
        //    self.selectedKH(data.ID_KhachHang);
        //    self.selectedNV(data.ID_NhanVien);
        //    self.TieuDeLichHen('Cập nhật: ' + data.Ma_TieuDe);
        //});
        // self.TieuDeLichHen('Cập nhật: ' + self.newLichHen().Ma_TieuDe());
        $('#myModallichhen').modal('show');

        // find event in calendar
        var event = $.grep(calendar.options.events, function (x) {
            return x.ID === id;
        });
        if (event.length > 0) {
            let ngaygio = moment(new Date(parseFloat(event[0].start))).format('DD/MM/YYYY HH:mm');
            let ngaygiokt = moment(new Date(parseFloat(event[0].end))).format('DD/MM/YYYY HH:mm');
            event[0].NgayGio = ngaygio;
            event[0].NgayGioKetThuc = ngaygiokt;
            self.newLichHen().SetData(event[0]);
            self.booleanAdd(false);
            self.newLichHen().TrangThai(event[0].TrangThai);
            self.selectedLoaiLichHen(event[0].ID_LoaiTuVan);
            self.selectedNhacNho(event[0].NhacNho);
            self.selectedKH(event[0].ID_KhachHang);
            $('#txtKhachHang').val(event[0].TenDoiTuong)
            self.selectedChooseNhanVien({ ID: event[0].ID_NhanVien, TenNhanVien: event[0].TenNhanVien });
            console.log(event[0])
        }
    };
    // thêm mới theo ngày
    self.TaoMoiLichHen = function (item, type) {
        switch (type) {
            case 1:   // day
                console.log('newdate ', calendar.getStartDate());
                break;
            case 2:   // week
                break;
            case 3:   // month
                break;
        }
        self.booleanAdd(true);
        self.TieuDeLichHen("Thêm mới lịch hẹn")

        $('#myModallichhen').modal('show');
        $('#txtTieuDe').focus();
    };
    self.resetTextBox = function () {
        self.selectedLoaiLichHen(undefined);
        self.selectedNhacNho(undefined);
        self.selectedKH(undefined);
        self.selectedNV(undefined);
        self.newLichHen(new FormModel_NewLichHen());
    }

    // Reset
    self.resetLoaiTVLH = function () {
        self.newLoaiLichHen(new FormModel_LoaiLH());

    }
    self.themmoiloailichhen = function () {

        self.resetLoaiTVLH();
        self._ThemLoaiLH(true);
        $('#myModalphanloai').modal('show');
    }

    self.editloailichhen = function (item) {
        ajaxHelper(DM_LoaiTVLHUri + "GetLoaiTuVan/" + this.selectedLoaiLichHen(), 'GET').done(function (data) {
            self.newLoaiLichHen().setdata(data);
            self._ThemLoaiLH(false);
            $('#myModalphanloai').modal('show');
        });
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


    function getAllDMLoaiTuVanLichHens() {
        ajaxHelper(DM_LoaiTVLHUri + "GetDM_LoaiLichHen", 'GET').done(function (data) {
            self.LoaiTuVanLichHens(data);
        });
    }
    getAllDMLoaiTuVanLichHens();

    self.addLoaiTuVanLichHen = function (formElement) {

        var _idLoaiTuVanLichHen = self.newLoaiLichHen().ID();
        var _tenLoaiTuVanLichHen = self.newLoaiLichHen().TenLoaiTuVanLichHen();

        if (_tenLoaiTuVanLichHen == null || _tenLoaiTuVanLichHen == "" || _tenLoaiTuVanLichHen == "undefined") {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Không được để trống tên loại tư vấn!", "danger");
            $('#txtTenLoaiLichHen').focus();
            return false;
        }
        var objLoaiTuVan = {
            ID: _idLoaiTuVanLichHen,
            TenLoaiTuVanLichHen: _tenLoaiTuVanLichHen
        };
        if (self._ThemLoaiLH() === true) {
            var myData = {};
            myData.objLoaiTVLH = objLoaiTuVan;
            $.ajax({
                url: DM_LoaiTVLHUri + "PostLoaiLichHen",
                type: 'POST',
                dataType: 'json',
                data: myData,
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (item) {
                    self.LoaiTuVanLichHens.push(item);
                    self.selectedLoaiLichHen(item.ID);
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Thêm mới loại phản hồi thành công!", "success");
                },
                statusCode: {
                    404: function () {
                        self.error("page not found");
                    },
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    $("#myModalphanloai").modal("hide");
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Thêm mới loại phản hồi thất bại!", "danger");
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
                url: DM_LoaiTVLHUri + "PutLoaiLichHen",
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
                    bottomrightnotify('Cập nhật loại phản hồi thành công !', 'success');
                }
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                });
        }
    }

    //THem/Sua
    self.addLichHen = function (formElement) {

        var _tieude = self.newLichHen().Ma_TieuDe();
        var _id = self.newLichHen().ID();
        var _idkhachhang = self.selectedKH();;
        var _idnhanvien = self.selectedNV();
        //var _ngaygio = $('#ngaybd').val();
        //var _ngaygioketthuc = $('#ngaykt').val();
        var _ngaygio = self.newLichHen().NgayGio();
        var _ngaygioketthuc = self.newLichHen().NgayGioKetThuc();
        var _noidung = self.newLichHen().NoiDung();// ghi chú
        var _nhacnho = self.selectedNhacNho();
        var _trangthai = self.newLichHen().TrangThai();
        var _idloailh = self.selectedLoaiLichHen();
        if (_tieude === null || _tieude === "" || _tieude === undefined) {
            commonStatisJs.ShowMessageDanger('Vui lòng nhập tiêu đề');
            $('#txtTieuDe').select();
            return false;
        }

        if (_idkhachhang === undefined) {
            commonStatisJs.ShowMessageDanger('Vui lòng chọn khách hàng');
            $('#txtKhachHang').focus();
            return false;
        }

        if (moment(_ngaygio, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') === "Invalid date") {
            commonStatisJs.ShowMessageDanger('Vui lòng chọn thời gian');
            $('#ngaybd').select();
            return false;
        }

        //if ((moment(_ngaygio, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') > moment(_ngaygioketthuc, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm')) && moment(_ngaygioketthuc, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') !== "Invalid date") {
        //    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Thời gian kết thúc phải lớn hơn thời gian bắt đầu", "danger");
        //    return false;
        //}

        var Lich_Hen = {
            ID: _id,
            Ma_TieuDe: _tieude,
            ID_DonVi: _IDchinhanh,
            ID_KhachHang: _idkhachhang,
            ID_NhanVien: _idnhanvien,
            ID_LoaiTuVan: _idloailh,
            NgayGio: moment(_ngaygio, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm'),
            NgayGioKetThuc: moment(_ngaygioketthuc, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') === "Invalid date" ? moment(_ngaygio, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') : moment(_ngaygioketthuc, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm'),
            NoiDung: _noidung,
            NhacNho: _nhacnho,
            TrangThai: _trangthai
        };
        //Them
        if (self.booleanAdd() === true) {
            var myData = {};
            myData.objLichHen = Lich_Hen;
            callAjaxAdd(myData);
        }
        else {
            var myData = {};
            myData.id = _id;
            myData.objLichHen = Lich_Hen;
            callAjaxUpdate(myData);
        }
        //self.gotoNextPage();
    };

    function callAjaxAdd(myData) {
        $.ajax({
            data: myData,
            url: CSKHUri + "PostLichHen",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (item) {
                //self.LichHens.unshift(item);
                bottomrightnotify("Thêm mới lịch hẹn thành công!", "success");
                SearchLichHen();
                calendar.getTitle();
            },
            statusCode: {
                404: function () {
                    self.error("page not found");
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                bottomrightnotify("Thêm mới lịch hẹn thất bại!", "danger");
            },
            complete: function () {
                $("#myModallichhen").modal("hide");
            }
        })
    }

    function callAjaxUpdate(myData) {
        $.ajax({
            url: CSKHUri + "PutLichHen",
            type: 'PUT',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myData,
            success: function () {
                $("#myModallichhen").modal("hide");
                SearchLichHen();
                calendar.getTitle(); // bind again calendar
            },
            statusCode: {
                404: function () {
                    self.error("page not found");
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                bottomrightnotify("Cập nhật lịch hẹn thất bại!", "danger");
            },
            complete: function () {
                self.LichHens([]);
                bottomrightnotify('Cập nhật lịch hẹn thành công !', 'success');
                SearchLichHen();
            }
        })
    }

    self.modalDelete = function (item) {
        self.deleteLichHen(item.Ma_TieuDe);
        self.deleteID(item.ID);
        $('#modalpopup_deleteLichHen').modal('show');
    };
    //Xóa
    self.modalDeleteLoaiLichHen = function (LoaiLichHens) {
        self.deleteTenLoaiLichHen(self.newLoaiLichHen().TenLoaiTuVanLichHen());
        self.deleteID(self.newLoaiLichHen().ID());
        $('#modalpopup_deleteLoaiTuVan').modal('show');
    };

    self.XoaLichHenLichSu = function (tieude) {
        var objDiary = {
            ID_NhanVien: idNhanVien,
            ID_DonVi: idDonVi,
            ChucNang: "Lịch hẹn",
            NoiDung: "Xóa lịch hẹn : " + tieude,
            NoiDungChiTiet: "Xóa lịch hẹn : " + tieude,
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

    self.xoaLichHen = function (LichHens) {
        $.ajax({
            type: "DELETE",
            url: CSKHUri + "Delete_LichHen/" + LichHens.deleteID(),
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa lịch hẹn thành công !", "success");
                self.XoaLichHenLichSu(LichHens.deleteLichHen());
                SearchLichHen();
            },
            error: function (error) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Xóa lịch hẹn thất bại!", "danger");
            }
        })
    };
    // Xoa loai lịch hẹn
    self.xoaLoaiLichHen = function (item) {
        $.ajax({
            type: "DELETE",
            url: DM_LoaiTVLHUri + "Delete_LoaiTuVan/" + self.newLoaiLichHen().ID(),
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa loại lịch hẹn thành công !", "success");
                getAllDMLoaiTuVanLichHens();
            },
            error: function (error) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Xóa loại lịch hẹn thất bại!", "danger");
            }
        })
    };

    function getAllDMDoiTuong() {
        ajaxHelper(DMDoiTuongUri + "GetListKhachHang?loaiDoiTuong=1", 'GET').done(function (data) {
            self.DoiTuongs(data);
        });
    }
    getAllDMDoiTuong();

    //self.selectedChooseKhachHang = ko.observable();
    //self.selectedChooseKhachHang.subscribe(function (newValue) {
    //    if (newValue !== undefined) {
    //        self.selectedKH(newValue.ID);
    //        $('#txtKhachHang').text(newValue.TenDoiTuong);
    //        $('#lstKhachHang span').each(function () {
    //            $(this).empty();
    //        });
    //        $(function () {
    //            $('span[id=spanCheckKhachHang_' + newValue.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
    //        });
    //    }
    //})

    //self.selectedKH.subscribe(function () {
    //    var tenKH = $('.txtKhachHang').val();
    //    console.log(tenKH);
    //    $('#txtKhachHang').text(tenKH);
    //})

    //self.filterKhachHang = ko.observable();
    //self.arrFilterKhachHang = ko.computed(function () {
    //    var _filter = self.filterKhachHang();

    //    return arrFilter = ko.utils.arrayFilter(self.DoiTuongs(), function (prod) {
    //        var chon = true;
    //        var arr = locdau(prod.TenDoiTuong).toLowerCase().split(/\s+/);
    //        var sSearch = '';

    //        for (var i = 0; i < arr.length; i++) {
    //            sSearch += arr[i].toString().split('')[0];
    //        }

    //        var arr1 = locdau(prod.MaDoiTuong).toLowerCase().split(/\s+/);
    //        var sSearch1 = '';

    //        for (var i = 0; i < arr1.length; i++) {
    //            sSearch1 += arr1[i].toString().split('')[0];
    //        }

    //        if (chon && _filter) {
    //            chon = (locdau(prod.TenDoiTuong).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
    //                sSearch.indexOf(locdau(_filter).toLowerCase()) >= 0 || locdau(prod.MaDoiTuong).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
    //                sSearch1.indexOf(locdau(_filter).toLowerCase()) >= 0
    //            );
    //        }
    //        return chon;
    //    });
    //});

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


    //function getChiTietNCCByID(id) {
    //    ajaxHelper(DMDoiTuongUri + "GetDM_DoiTuong/" + id, 'GET').done(function (data) {
    //        self.ChiTietDoiTuong(data);
    //    })
    //}

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

    //phân trang

    self.MangLoaiLichHen = ko.observableArray();

    self.selectedLoaiTuVanFilter = function (item) {
        var arrLCV = [];
        for (var i = 0; i < self.MangLoaiLichHen().length; i++) {
            if ($.inArray(self.MangLoaiLichHen()[i], arrLCV) === -1) {
                arrLCV.push(self.MangLoaiLichHen()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrLCV) === -1) {
            self.MangLoaiLichHen.push(item);
        }
        SearchLichHen();
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
        self.MangLoaiLichHen.remove(item);
        if (self.MangLoaiLichHen().length === 0) {
            $('#choose_LoaiTuVan').append('<input type="text" id="dllTuVan" readonly="readonly" class="dropdown" placeholder="Chọn loại lịch hẹn">');
        }
        SearchLichHen();
        // remove checks
        $('#selec-all-TuVan li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }

    self.MangLoaiLichHenFilter = ko.observableArray();
    self.TotalRecord = ko.observableArray();
    self.PageCount = ko.observableArray();

    self.TodayBC = ko.observable();
    self.MangIDNhanVien = ko.observableArray();
    function SearchLichHen() {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('LichHen_XemDS', lc_CTQuyen) > -1) {
            var arrLCV = [];
            for (var i = 0; i < self.MangLoaiLichHen().length; i++) {
                if ($.inArray(self.MangLoaiLichHen()[i], arrLCV) === -1) {
                    arrLCV.push(self.MangLoaiLichHen()[i].ID);
                }
            }

            self.MangLoaiLichHenFilter(arrLCV);

            var arrIDNV = [];
            for (var i = 0; i < self.ListIDNhanVienQuyen().length; i++) {
                if ($.inArray(self.ListIDNhanVienQuyen()[i], arrIDNV) === -1) {
                    arrIDNV.push(self.ListIDNhanVienQuyen()[i]);
                }
            }
            self.MangIDNhanVien(arrIDNV);
            var statusInvoice = 1;
            if (self.checkHuy()) {
                if (self.checkTiemNang()) {
                    if (self.checkThamKhao()) {
                        statusInvoice = 6;
                    } else {
                        statusInvoice = 5;
                    }
                }
                else {
                    if (self.checkThamKhao()) {
                        statusInvoice = 4;
                    } else {
                        statusInvoice = 1; // HT
                    }
                }
            }
            else {
                if (self.checkTiemNang()) {
                    if (self.checkThamKhao()) {
                        statusInvoice = 0;
                    } else {
                        statusInvoice = 3;
                    }
                } else {
                    if (self.checkThamKhao()) {
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
            ajaxHelper(CSKHUri + 'GetAllLichHenWhere?currentPage=' + self.currentPage() + '&pageSize=' + self.pageSize() + '&arrLoaiLichHen=' + self.MangLoaiLichHenFilter() + '&dayStart=' + dayStart + '&dayEnd=' + dayEnd + '&txtSearch=' + self.filter() + '&trangthai=' + statusInvoice + '&arrMangIDNhanVien=' + self.MangIDNhanVien() + '&iddonvi=' + idDonVi, 'GET').done(function (data) {
                self.LichHens(data.LstData);
                self.TotalRecord(data.TotalRow);
                self.PageCount(data.PageCount);
                $('.table-reponsive').gridLoader({ show: false });
            })
        }
    }

    self.clickSearchLH = function () {
        SearchLichHen();
    }

    self.checkHuy.subscribe(function (newVal) {
        self.currentPage(0);
        SearchLichHen();
    });

    self.checkThamKhao.subscribe(function (newVal) {
        self.currentPage(0);
        SearchLichHen();
    });

    self.checkTiemNang.subscribe(function (newVal) {
        self.currentPage(0);
        SearchLichHen();
    });

    $('#txtFilterLichHen').keypress(function (e) {
        if (e.keyCode === 13) {
            self.currentPage(0);
            SearchLichHen();
        }
    })
    self.ResetCurrentPage = function () {
        self.currentPage(0);
        SearchLichHen();
    };

    self.filterNgayLapHD.subscribe(function (newVal) {
        self.currentPage(0);
        SearchLichHen();
    });
    $('#txtNgayTaoInput').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        SearchLichHen();
    });

    $('.choseNgayTao li').on('click', function () {
        $('#txtNgayTao').val($(this).text());
        self.filterNgayLapHD_Quy($(this).val());
        self.currentPage(0);
        SearchLichHen();
    });

    self.PageResults = ko.computed(function () {
        if (self.LichHens() !== null) {

            self.fromitem((self.currentPage() * self.pageSize()) + 1);

            if (((self.currentPage() + 1) * self.pageSize()) > self.LichHens().length) {
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
            SearchLichHen();
        }
    };

    self.StartPage = function () {
        self.currentPage(0);
        SearchLichHen();
    }

    self.BackPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
            SearchLichHen();
        }
    }

    self.GoToNextPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.currentPage() + 1);
            SearchLichHen();
        }
    }

    self.EndPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
            SearchLichHen();
        }
    }
    self.GetClassHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage()) ? "click" : "";
    };

    // tim kiem JqAuto khách hàng
    self.filterKH = function (item, inputString) {
        var itemSearch = locdau(item.TenDoiTuong).toLowerCase();
        var locdauInput = locdau(inputString).toLowerCase();
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';
        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }
        return itemSearch.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    }


    // tim kiem JqAuto nhan vien
    self.filterNV = function (item, inputString) {
        var itemSearch = locdau(item.TenNhanVien).toLowerCase();
        var locdauInput = locdau(inputString).toLowerCase();
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';
        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }
        return itemSearch.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    }

    function GetListLichHen() {
        return ajaxHelper('/s/GetListLichHen_FromTo?daySart=' + self.dateloadLuoiStart() + '&dayEnd=' + self.dateloadluoiEnd() + '&IDchinhanh=' + idDonVi, 'POST').done(function (x) {
            return x;
        })
    }

    var calendar = $("#calendar").calendar({
        modal: "#myModallichhen",
        modal_type: "userdefine",
        modal_title: function (e) { return e.title },
        view: 'week',
        tmpl_path: "/Content/calendar/tmpls/",
        events_source: '/s/GetListLichHen_FromTo?daySart=' + self.dateloadLuoiStart() + '&dayEnd=' + self.dateloadluoiEnd() + '&IDchinhanh=' + idDonVi
    });

    self.getluoi = function (nav, type) {
        switch (type) {
            case 1:
                calendar.navigate(nav);
                break;
            case 2:
                calendar.view(nav);
                break;
        }
        self.CalendarHeader(calendar.getTitle());
    }

    // overide func calendar.getTitle at calendar.js
    calendar.getTitle = function () {
        var p = this.options.position.start;
        let dateFrom = '';
        let dateTo = '';

        switch (this.options.view) {
            case 'year':
                dateFrom = moment(p).startOf('year').format('YYYY-MM-DD');
                dateTo = moment(p).endOf('year').add('days', 1).format('YYYY-MM-DD');
                console.log('year ', dateFrom, dateTo);
                calendar.options.events_source = '/s/GetListLichHen_FromTo?daySart=' + dateFrom + '&dayEnd=' + dateTo + '&IDchinhanh=' + idDonVi;
                calendar._loadEvents();
                calendar._render();
                return 'Năm '.concat(this.locale.title_year.format(p.getFullYear()));
                break;
            case 'month':
                dateFrom = moment(p).startOf('month').format('YYYY-MM-DD');
                dateTo = moment(p).endOf('month').add('days', 1).format('YYYY-MM-DD');
                console.log('month ', dateFrom, dateTo);
                calendar.options.events_source = '/s/GetListLichHen_FromTo?daySart=' + dateFrom + '&dayEnd=' + dateTo + '&IDchinhanh=' + idDonVi;
                calendar._loadEvents();
                calendar._render();
                return 'Tháng '.concat(p.getMonth() + 1, ' - ', p.getFullYear());
                break;
            case 'week':
                dateFrom = moment(p).startOf('week').format('YYYY-MM-DD');
                dateTo = moment(p).endOf('week').add('days', 1).format('YYYY-MM-DD');
                console.log('week ', dateFrom, dateTo);
                let start = moment(p).format('DD/MM/YYYY');
                let end = moment(p).endOf('week').format('DD/MM/YYYY');
                calendar.options.events_source = '/s/GetListLichHen_FromTo?daySart=' + dateFrom + '&dayEnd=' + dateTo + '&IDchinhanh=' + idDonVi;
                calendar._loadEvents();
                calendar._render();
                return start.concat(' - ', end);
                break;
            case 'day':
                dateFrom = moment(p).format('YYYY-MM-DD');
                dateTo = moment(p).add('days', 1).format('YYYY-MM-DD');
                console.log('day ', dateFrom, dateTo);
                calendar.options.events_source = '/s/GetListLichHen_FromTo?daySart=' + dateFrom + '&dayEnd=' + dateTo + '&IDchinhanh=' + idDonVi;
                calendar._loadEvents();
                calendar._render();
                return this.locale['d' + p.getDay()].concat(', ', p.getDate(), '/', p.getMonth() + 1, '/', p.getFullYear())
                break;
        }
        return;
    }

    $('.cal-day-inmonth, .cal-day-hour-part').on('click', function () {
        console.log(5);
    })

    self.ClickPrev = function () {
        var $this = event.currentTarget;
        var valBtn = $($this).attr('data-calendar-nav');
        self.getluoi(valBtn, 1);
    }

    self.ChangeView = function () {
        var $this = event.currentTarget;
        var valBtn = $($this).attr('data-calendar-view');
        $($this).parent('div').find('button').removeClass('active');
        $($this).addClass('active');
        self.getluoi(valBtn, 2);
    }

    function update() {
        document.getElementById("title-up").disabled = false;
        document.getElementById("date-begin").disabled = false;
        document.getElementById("title-up").disabled = false;
        document.getElementById("date-end").disabled = false;
        document.getElementById("note").disabled = false;
        document.getElementById("txtAuto1").disabled = false;
        document.getElementById("status").disabled = false;
    }

    $(".op-js-themmoinhomhang").click(function () {
        $(".modal-ontop").show();
        $(".op-js-modal").modal('show');
    });
    $(".close-modal,.save-modal").click(function () {
        $(".modal-ontop").hide();
        $(".op-js-modal").modal('hide');
    });
    $(".modal-ontop").click(function () {
        $(this).hide();
        $(".op-js-modal").modal('hide');
    });
    $(document).on('click', '.choose-date .dropdown-menu ul li', function () {
        var date = $(this).find("a").html();
        $(".choose-date-show").val(date);
    });
}
var lichhenvm = new ViewModel();
ko.applyBindings(lichhenvm);
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

function hidewait(o) {
    $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /><div class="happy-wait">' +
        ' </div>' +
        '</div>')
}
function update_lichhen(id) {
    lichhenvm.CapNhatLichHen(id);
};
function creat_lichhen(data, type) {
    lichhenvm.TaoMoiLichHen(data, type);
};

//var calendar = $("#calendar").calendar({

//    //modal: "#myModallichhen",
//    modal_type: "userdefine",
//    modal_title: function (e) { return e.title },
//    tmpl_path: "/Content/calendar/tmpls/",
//    events_source: function (start, end, timezone, callback) {
//        $.ajax({
//            url: 's/GetListLichHen_FromTo',
//            type: 'GET',
//            dataType: 'json',
//            data: {
//                daySart: '2016-01-01',
//                dayEnd: '2019-12-02',
//                IDchinhanh:'d93b17ea-89b9-4ecf-b242-d03b8cde71de'
//            },
//            success: function (doc) {
//                return doc;
//                //var events = [];
//                //if (!!doc.result) {
//                //    $.map(doc.result, function (r) {
//                //        events.push({
//                //            id: r.id,
//                //            title: r.title,
//                //            start: r.date_start,
//                //            end: r.date_end
//                //        });
//                //    });
//                //}
//                //callback(events);
//            }
//        });
//    }
//});


//$('.btn-group button[data-calendar-nav]').each(function () {
//    var $this = $(this);
//    $this.click(function () {
//        //calendar.navigate($this.data('calendar-nav'));
//        lichhenvm.getluoi();
//    });
//});
//$('.btn-group button[data-calendar-view]').each(function () {
//    var $this = $(this);
//    $this.click(function () {
//        calendar.view($this.data('calendar-view'));
//    });
//});
//$(function () {
//    $('.cal-day-hour-part').each(function () {
//        var $this = $(this);
//        $this.click(function () {
//            console.log('view');
//        });
//    }
//    )
//})

//function update() {
//    document.getElementById("title-up").disabled = false;
//    document.getElementById("date-begin").disabled = false;
//    document.getElementById("title-up").disabled = false;
//    document.getElementById("date-end").disabled = false;
//    document.getElementById("note").disabled = false;
//    document.getElementById("txtAuto1").disabled = false;
//    document.getElementById("status").disabled = false;

//}

//$(".close-modal,.save-modal").click(function () {
//    $(".modal-ontop").hide();
//    $(".op-js-modal").modal('hide');
//});
//$(".modal-ontop").click(function () {
//    $(this).hide();
//    $(".op-js-modal").modal('hide');
//});
//$(document).on('click', '.choose-date .dropdown-menu ul li', function () {
//    var date = $(this).find("a").html();
//    $(".choose-date-show").val(date);
//});

$('#myModallichhen').on('shown.bs.modal', function (e) {
    $('.datetimepicker4').datetimepicker({
        format: 'd/m/Y H:i',
        timepicker: true,
        step: 30,
        mask: true
    });
});