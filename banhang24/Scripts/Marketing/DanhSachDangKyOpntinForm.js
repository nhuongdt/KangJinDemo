var FormModel_NewKhachHang = function () {
    var self = this;
    self.ID = ko.observable();
    self.MaDoiTuong = ko.observable();
    self.ID_NhomDoiTuong = ko.observable();
    self.ID_QuanHuyen = ko.observable();
    self.ID_TinhThanh = ko.observable();
    self.ID_NguonKhach = ko.observable();
    self.ID_NguoiGioiThieu = ko.observable();
    self.ID_NhanVienPhuTrach = ko.observable();
    self.LaCaNhan = ko.observable();

    self.TenDoiTuong = ko.observable();
    self.Email = ko.observable();
    self.DiaChi = ko.observable();
    self.DienThoai = ko.observable();
    self.NgaySinh_NgayTLap = ko.observable();
    self.GioiTinhNam = ko.observable(true);
    self.MaSoThue = ko.observable();
    self.GhiChu = ko.observable();
    self.LoaiDoiTuong = loaiDoiTuong;
    self.NoHienTai = ko.observable();
    self.NoCanTra = ko.observable();
    self.CongTy = ko.observable();
    self.DiaChiChiNhanh = ko.observable();
    self.DienThoaiChiNhanh = ko.observable();
    self.TongTichDiem = ko.observable();
    self.GhiChu = ko.observable();

    self.ID_VungMien = ko.observable();
    self.ID_KhuVuc = ko.observable();
    self.DinhDang_NgaySinh = ko.observable();
    self.TongBan = ko.observable();
    self.TongMua = ko.observable();
    self.TongBanTruTraHang = ko.observable();
    self.SoLanMuaHang = ko.observable();
    $('#SL_TrangThaiKH').val(undefined);// add new: reset text TrangThai

    self.SetData = function (item) {
        self.ID(item.ID);
        self.CongTy(item.CongTy); // TenCuaHang
        self.DienThoaiChiNhanh(item.DienThoaiChiNhanh);
        self.DiaChiChiNhanh(item.DiaChiChiNhanh);

        // ID_NhomDoiTuong.toString() --> avoid ID_DoiTuong = 0 (nhom mac dinh)
        //if (item.ID_NhomDoiTuong !== null && item.ID_NhomDoiTuong.toString().indexOf('0000') === -1) {
        //    self.ID_NhomDoiTuong(item.ID_NhomDoiTuong);
        //}
        //else {
        //    self.ID_NhomDoiTuong(undefined);
        //}
        self.ID_NhomDoiTuong(undefined);
        if (item.ID_QuanHuyen !== null && item.ID_QuanHuyen.indexOf('0000') === -1) {
            self.ID_QuanHuyen(item.ID_QuanHuyen);
        }
        else {
            self.ID_QuanHuyen(undefined);
        }

        if (item.ID_TinhThanh !== null && item.ID_TinhThanh.indexOf('0000') === -1) {
            self.ID_TinhThanh(item.ID_TinhThanh);
        }
        else {
            self.ID_TinhThanh(undefined);
        }
        self.ID_NguonKhach(undefined);
        self.ID_NguoiGioiThieu(undefined);
        self.ID_NhanVienPhuTrach(undefined);
        $('#SL_TrangThaiKH').val(undefined);
        self.LaCaNhan(item.boolLaCaNhan);
        self.MaDoiTuong(item.MaDoiTuong);
        self.TenDoiTuong(item.TenDoiTuong);
        self.Email(item.Email);
        self.DiaChi(item.DiaChi);
        self.DienThoai(item.SoDienThoai);
        console.log(item.boolGioiTinh);
        if (item.boolGioiTinh == 0)
            self.GioiTinhNam(false);
        else
            self.GioiTinhNam(true);
        self.GhiChu(item.GhiChu);
        var ngaysinh = item.NgaySinh;
        if (ngaysinh === null || ngaysinh === undefined) {
            ngaysinh = "";
        }
        else {
            ngaysinh = moment(item.NgaySinh, "YYYY-MM-DD hh:mm:ss").format("DD/MM/YYYY");
        }
        self.NgaySinh_NgayTLap(ngaysinh);
        self.MaSoThue(item.MaSoThue);
        self.NoHienTai(item.NoHienTai);
        self.NoCanTra(item.NoCanTra);
        self.CongTy(item.CongTy);
        self.DienThoaiChiNhanh(item.DienThoaiChiNhanh);
        self.DiaChiChiNhanh(item.DiaChiChiNhanh);
        self.TongTichDiem(item.TongTichDiem);
        self.DinhDang_NgaySinh(item.DinhDang_NgaySinh);
        self.TongBan(item.TongBan);
        self.TongMua(item.TongMua);
        self.TongBanTruTraHang(item.TongBanTruTraHang);
        self.SoLanMuaHang(item.SoLanMuaHang);
    }
};
var FormModel_NewNhomDoiTuong = function () {
    var self = this;
    self.ID = ko.observable();
    self.TenNhomDoiTuong = ko.observable();
    self.LoaiDoiTuong = loaiDoiTuong;
    self.GhiChu = ko.observable();

    self.SetData = function (item) {
        self.ID(item.ID);
        self.TenNhomDoiTuong(item.TenNhomDoiTuong);
        self.GhiChu(item.GhiChu);
    }
}

var FormModel_NewNguonKhach = function () {
    var self = this;
    self.ID = ko.observable();
    self.TenNguonKhach = ko.observable();

    self.SetData = function (item) {
        self.ID(item.ID);
        self.TenNguonKhach(item.TenNguonKhach);
    }
}
var ViewModal = function () {
    var self = this;
    self.Check_TrangThaiXuLy = ko.observable(1);
    self.Check_TrangThaiChuaXuLy = ko.observable(1);
    self.Check_TrangThaiHuyBo = ko.observable(0);
    self.Check_TrangThaiFormBat = ko.observable(1);
    self.Check_TrangThaiFormTat = ko.observable(1);
    self.Check_TrangThaiFormXoa = ko.observable(0);
    self.Loai_OptinForm = ko.observable(1);
    self.MangOptinFormKhachHang = ko.observableArray();
    self.MangOptinFormLichHen = ko.observableArray();
    self.searchOptinFormKhachHang = ko.observableArray();
    self.searchOptinFormLichHen = ko.observableArray();
    self.DM_OptinFromKhachHang = ko.observableArray();
    self.DM_OptinFromLichHen = ko.observableArray();
    self.DM_DoiTuongKhachHang = ko.observableArray();
    self.RowsStart = ko.observable('1');
    self.RowsEnd = ko.observable('10');
    self.ID_KhuVuc = ko.observable();
    self.ID_ViTri = ko.observable();
    self.ID_TinhThanh = ko.observable();
    self.ID_QuanHuyen = ko.observable();
    self.IsOpenModalCus = ko.observable(false);
    var AllPage;
    self.SumNumberPageReport = ko.observableArray();
    self.SumRowsDoiTuongOF = ko.observable();
    var OptinFormUri = '/api/DanhMuc/OptinFormAPI/';
    var DMDoiTuongUri = '/api/DanhMuc/DM_DoiTuongAPI/';
    var DMNguonKhachUri = '/api/DanhMuc/DM_NguonKhachAPI/';
    var DMNhomDoiTuongUri = '/api/DanhMuc/DM_NhomDoiTuongAPI/';
    var CSKHUri = '/api/DanhMuc/ChamSocKhachHangAPI/';
    var _id_NhanVien = $('.idnhanvien').text();
    var _IDDoiTuong = $('.idnguoidung').text();
    var _id_DonVi = $('#hd_IDdDonVi').val();
    var idNhanVien = $('.idnhanvien').text();
    var DateNgaySinh;
    self.newNguonKhach = ko.observable(new FormModel_NewNguonKhach());
    self.newDoiTuong = ko.observable(new FormModel_NewKhachHang());
    self.newNhomDoiTuong = ko.observable(new FormModel_NewNhomDoiTuong());
    var ID_OptinForm = null;
    var TenKhachHang = null;
    var Ten_OptinForm = null;
    var lc_DoiTuongOF = null
    self.NgaySinhOld_KhachHang = ko.observable();
    self.ListTypeNgaySinh = ko.observableArray([
        { Value: 'dd/MM/yyyy', Text: 'Theo ngày/tháng/năm' },
        { Value: 'dd/MM', Text: 'Theo ngày/tháng' },
        { Value: 'MM/yyyy', Text: 'Theo tháng/năm' },
        { Value: 'yyyy', Text: 'Theo năm' },
    ])

    self.pageSize = ko.observable(10);
    var _pageNumber = 1;
    var _pageSize = 10;
    self.pageSizes = [10, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.pageNumber_KH = ko.observable(1);
    self.pageNumber_LH = ko.observable(1);
    //localStorage.removeItem('lc_DoiTuongOF');
    lc_DoiTuongOF = JSON.parse(localStorage.getItem('lc_DoiTuongOF'));
    console.log(lc_DoiTuongOF);
    if (lc_DoiTuongOF != null) {
        //self.Check_TrangThaiXuLy(lc_DoiTuongOF.Check_TrangThaiXuLy);
        //self.Check_TrangThaiChuaXuLy(lc_DoiTuongOF.Check_TrangThaiChuaXuLy);
        //self.Check_TrangThaiHuyBo(lc_DoiTuongOF.Check_TrangThaiHuyBo);
        //self.Check_TrangThaiFormBat(lc_DoiTuongOF.Check_TrangThaiFormBat);
        //self.Check_TrangThaiFormTat(lc_DoiTuongOF.Check_TrangThaiFormTat);
        //self.Check_TrangThaiFormXoa(lc_DoiTuongOF.Check_TrangThaiFormXoa);
        ID_OptinForm = lc_DoiTuongOF.ID;
        Ten_OptinForm = lc_DoiTuongOF.TenOptinForm;
        self.Loai_OptinForm(lc_DoiTuongOF.LoaiOptinForm);
        localStorage.removeItem('lc_DoiTuongOF');
    }

    var datime = new Date();
    var _timeStart = '2015-09-26'
    var _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
    self.TodayBC = ko.observable('Ngày đăng ký: Toàn thời gian');
    //trinhpv phân quyền
    self.OF_BaoCaoKhachHang = ko.observable();
    self.OF_BaoCaoLichHen = ko.observable();
    self.OF_XuatFile = ko.observable();
    function getQuyen_NguoiDung() {
        //quyền xem báo cáo
        ajaxHelper('/api/DanhMuc/ReportAPI/' + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "OF_BaoCaoKhachHang", "GET").done(function (data) {
            self.OF_BaoCaoKhachHang(data);
            console.log(data);
        })
        //quyền xem xuất báo cáo
        ajaxHelper('/api/DanhMuc/ReportAPI/' + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "OF_BaoCaoLichHen", "GET").done(function (data) {
            self.OF_BaoCaoLichHen(data);
            console.log(data);
        })
        ajaxHelper('/api/DanhMuc/ReportAPI/' + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "OF_XuatFile", "GET").done(function (data) {
            self.OF_XuatFile(data);
            console.log(data);
        })
    }
    getQuyen_NguoiDung();
    function LoadingForm(IsShow) {
        $('.tab-show .tab-pane').each(function () {
            if ($(this).hasClass('active')) {
                var top = $(this).find('.table-reponsive').height() / 2;
                var style = "top:" + (top > 30 ? top - 30 : top) + "px";
                $(this).find('.table-reponsive').gridLoader({ show: IsShow, style: style });
            }
        });
    }
    self.select_CheckTrangThaiXuLy = function () {
        if (self.Check_TrangThaiXuLy() == 1)
            self.Check_TrangThaiXuLy(0);
        else
            self.Check_TrangThaiXuLy(1);
        // console.log(self.Check_TrangThaiXuLy());
        self.LoadReport();
    }
    self.select_CheckTrangThaiChuaXuLy = function () {
        if (self.Check_TrangThaiChuaXuLy() == 1)
            self.Check_TrangThaiChuaXuLy(0);
        else
            self.Check_TrangThaiChuaXuLy(1);
        self.LoadReport();
    }
    self.select_CheckTrangThaiHuyBo = function () {
        if (self.Check_TrangThaiHuyBo() == 1)
            self.Check_TrangThaiHuyBo(0);
        else
            self.Check_TrangThaiHuyBo(1);
        self.LoadReport();
    }
    self.select_CheckTrangThaiFormBat = function () {
        if (self.Check_TrangThaiFormBat() == 1)
            self.Check_TrangThaiFormBat(0);
        else
            self.Check_TrangThaiFormBat(1);
        //console.log(self.Check_TrangThaiFormBat());
        self.LoadReport();
    }
    self.select_CheckTrangThaiFormTat = function () {
        if (self.Check_TrangThaiFormTat() == 1)
            self.Check_TrangThaiFormTat(0);
        else
            self.Check_TrangThaiFormTat(1);
        self.LoadReport();
    }
    self.select_CheckTrangThaiFormXoa = function () {
        if (self.Check_TrangThaiFormXoa() == 1)
            self.Check_TrangThaiFormXoa(0);
        else
            self.Check_TrangThaiFormXoa(1);
        self.LoadReport();
    }
    self.Select_Text = function () {
        TenKhachHang = $('#txt_search').val();
        console.log(TenKhachHang);
    }
    $('#txt_search').keypress(function (e) {
        if (e.keyCode == 13) {
            _pageNumber = 1;
            self.LoadReport();
        }
    })
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
                self.TodayBC('Ngày đăng ký: Toàn thời gian');
            }
                //Hôm nay
            else if (_rdoNgayPage === "Hôm nay") {
                _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                self.TodayBC('Ngày đăng ký: ' + moment(_timeStart).format('DD/MM/YYYY'));
            }
                //Hôm qua
            else if (_rdoNgayPage === "Hôm qua") {
                var dt1 = new Date();
                var dt2 = new Date();
                _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
                _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
                self.TodayBC('Ngày đăng ký: ' + moment(_timeStart).format('DD/MM/YYYY'));
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
                    self.TodayBC('Ngày đăng ký: ' + moment(_timeStart).format('DD/MM/YYYY'));
                else
                    self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                _pageNumber = 1;
                self.LoadReport();
            }
        }
    })
    $('.choose_txtTime li').on('click', function () {
        //self.TodayBC($(this).text())
        $('.ip_TimeReport').val($(this).text());
        var _rdoNgayPage = $(this).val();
        var datime = new Date();
        var datimeBC = new Date();
        //Toàn thời gian
        if (_rdoNgayPage === 13) {
            _timeStart = '2015-09-26'
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            self.TodayBC('Ngày đăng ký: Toàn thời gian');
        }
            //Hôm nay
        else if (_rdoNgayPage === 1) {
            _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            self.TodayBC('Ngày đăng ký: ' + moment(_timeStart).format('DD/MM/YYYY'));
        }
            //Hôm qua
        else if (_rdoNgayPage === 2) {
            var dt1 = new Date();
            var dt2 = new Date();
            _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
            _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
            self.TodayBC('Ngày đăng ký: ' + moment(_timeStart).format('DD/MM/YYYY'));
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
            self.TodayBC('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
        else
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        _pageNumber = 1;
        self.LoadReport();
    });
    self.CloseOptinFormKhachHang = function (item) {
        ID_OptinForm = null;
        self.MangOptinFormKhachHang.remove(item);
        for (var i = 0; i < self.MangOptinFormKhachHang().length; i++) {
            if (ID_OptinForm == null) {
                ID_OptinForm = self.MangOptinFormKhachHang()[i].ID;
            }
            else {
                ID_OptinForm = self.MangOptinFormKhachHang()[i].ID + "," + ID_OptinForm;
            }
        }
        if (self.MangOptinFormKhachHang().length === 0) {
            $("#NoteNameOFKhachHang").attr("placeholder", "Chọn OptinForm...");
        }
        $('#selec-all-OFKhachHang li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        self.LoadReport();
    }

    self.SelectedOptinFormKhachHang = function (item) {
        ID_OptinForm = null;
        var arrIDDonVi = [];
        for (var i = 0; i < self.MangOptinFormKhachHang().length; i++) {
            if ($.inArray(self.MangOptinFormKhachHang()[i], arrIDDonVi) === -1) {
                arrIDDonVi.push(self.MangOptinFormKhachHang()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDDonVi) === -1) {
            self.MangOptinFormKhachHang.push(item);
            $('#NoteNameOFKhachHang').removeAttr('placeholder');
            for (var i = 0; i < self.MangOptinFormKhachHang().length; i++) {
                if (ID_OptinForm == null) {
                    ID_OptinForm = self.MangOptinFormKhachHang()[i].ID;
                }
                else {
                    ID_OptinForm = self.MangOptinFormKhachHang()[i].ID + "," + ID_OptinForm;
                }
            }
            self.LoadReport();
        }
        //thêm dấu check vào đối tượng được chọn
        $('#selec-all-OFKhachHang li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });

    }
    //lọc OptinFrom
    self.NoteNameOFKhachHang = function () {
        var arrOFKhachHang = [];
        var itemSearch = locdau($('#NoteNameOFKhachHang').val().toLowerCase());
        for (var i = 0; i < self.searchOptinFormKhachHang().length; i++) {
            var locdauInput = locdau(self.searchOptinFormKhachHang()[i].TenOptinForm).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrOFKhachHang.push(self.searchOptinFormKhachHang()[i]);
            }
        }
        self.DM_OptinFromKhachHang(arrOFKhachHang);
        if ($('#NoteNameOFKhachHang').val() == "") {
            self.DM_OptinFromKhachHang(self.searchOptinFormKhachHang());
        }
    }
    $('#NoteNameOFKhachHang').keypress(function (e) {
        if (e.keyCode == 13 && self.DM_OptinFromKhachHang().length > 0) {
            self.SelectedOptinFormKhachHang(self.DM_OptinFromKhachHang()[0]);
        }
    });
    self.CloseOptinFormLichHen = function (item) {
        ID_OptinForm = null;
        self.MangOptinFormLichHen.remove(item);
        for (var i = 0; i < self.MangOptinFormLichHen().length; i++) {
            if (ID_OptinForm == null) {
                ID_OptinForm = self.MangOptinFormLichHen()[i].ID;
            }
            else {
                ID_OptinForm = self.MangOptinFormLichHen()[i].ID + "," + ID_OptinForm;
            }
        }
        if (self.MangOptinFormLichHen().length === 0) {
            $("#NoteNameOFLichHen").attr("placeholder", "Chọn OptinForm...");
        }
        $('#selec-all-OFLichHen li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        self.LoadReport();
    }

    self.SelectedOptinFormLichHen = function (item) {
        ID_OptinForm = null;
        var arrIDDonVi = [];
        for (var i = 0; i < self.MangOptinFormLichHen().length; i++) {
            if ($.inArray(self.MangOptinFormLichHen()[i], arrIDDonVi) === -1) {
                arrIDDonVi.push(self.MangOptinFormLichHen()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDDonVi) === -1) {
            self.MangOptinFormLichHen.push(item);
            $('#NoteNameOFLichHen').removeAttr('placeholder');
            for (var i = 0; i < self.MangOptinFormLichHen().length; i++) {
                if (ID_OptinForm == null) {
                    ID_OptinForm = self.MangOptinFormLichHen()[i].ID;
                }
                else {
                    ID_OptinForm = self.MangOptinFormLichHen()[i].ID + "," + ID_OptinForm;
                }
            }
            self.LoadReport();
        }
        //thêm dấu check vào đối tượng được chọn
        $('#selec-all-OFLichHen li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });

    }
    //lọc OptinFrom
    self.NoteNameOFLichHen = function () {
        var arrOFLichHen = [];
        var itemSearch = locdau($('#NoteNameOFLichHen').val().toLowerCase());
        for (var i = 0; i < self.searchOptinFormLichHen().length; i++) {
            var locdauInput = locdau(self.searchOptinFormLichHen()[i].TenOptinForm).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrOFLichHen.push(self.searchOptinFormLichHen()[i]);
            }
        }
        self.DM_OptinFromLichHen(arrOFLichHen);
        if ($('#NoteNameOFLichHen').val() == "") {
            self.DM_OptinFromLichHen(self.searchOptinFormLichHen());
        }
    }
    $('#NoteNameOFLichHen').keypress(function (e) {
        if (e.keyCode == 13 && self.DM_OptinFromLichHen().length > 0) {
            self.SelectedOptinFormLichHen(self.DM_OptinFromLichHen()[0]);
        }
    });
    self.getList_OptinFromKhachHang = function () {
        ajaxHelper(OptinFormUri + "getList_OptinForm?LoaiOF=" + self.Loai_OptinForm()).done(function (data) {
            self.DM_OptinFromKhachHang(data.LstData);
            self.searchOptinFormKhachHang(data.LstData);
            if (ID_OptinForm != null) {
                var obj = {
                    ID: ID_OptinForm,
                    TenOptinForm: Ten_OptinForm
                }
                self.MangOptinFormKhachHang.push(obj)
            }

        });
    };
    self.getList_OptinFromLichHen = function () {
        ajaxHelper(OptinFormUri + "getList_OptinForm?LoaiOF=" + self.Loai_OptinForm()).done(function (data) {
            self.DM_OptinFromLichHen(data.LstData);
            self.searchOptinFormLichHen(data.LstData);
            if (ID_OptinForm != null) {
                var obj = {
                    ID: ID_OptinForm,
                    TenOptinForm: Ten_OptinForm
                }
                self.MangOptinFormLichHen.push(obj)
            }
        });
    };
    if (self.Loai_OptinForm() == 1) {
        self.getList_OptinFromKhachHang();
    }
    else {
        self.getList_OptinFromLichHen();
    }
    self.LoadReport = function () {
        LoadingForm(true);
        $('.table-reponsive').css('display', 'none');
        $("#iconSort").remove();
        self.pageNumber_KH(1);
        self.pageNumber_LH(1);
        var array_Seach = {
            ID_OptinFrom: ID_OptinForm,
            TenKhachHang: TenKhachHang,
            TimeStart: _timeStart,
            TimeEnd: _timeEnd,
            LoaiOptinForm: self.Loai_OptinForm(),
            TrangThaiXuLy: self.Check_TrangThaiXuLy() != 1 ? 4 : 1,
            TrangThaiChuaXuLy: self.Check_TrangThaiChuaXuLy() != 1 ? 4 : 2,
            TrangThaiHuyBo: self.Check_TrangThaiHuyBo() != 1 ? 4 : 3,
            TrangThaiFromBat: self.Check_TrangThaiFormBat() != 1 ? 4 : 1,
            TrangThaiFromTat: self.Check_TrangThaiFormTat() != 1 ? 4 : 0,
            TrangThaiFromXoa: self.Check_TrangThaiFormXoa() != 1 ? 4 : 3,
            paperSize: _pageSize,
            columnsHide: null,
            TodayBC: null,
            TenChiNhanh: null
        }
        console.log(array_Seach);
        if (self.Loai_OptinForm() == 1) {
            $('#OptinKH').show();
            $('#OptinLH').hide();
            if (self.OF_BaoCaoKhachHang() != "OF_BaoCaoKhachHang") {
                $(".PhanQuyen").hide();
                ajaxHelper(OptinFormUri + "getList_KhachHangOptinForm", "POST", array_Seach).done(function (data) {
                    self.DM_DoiTuongKhachHang(data.LstData);
                    console.log(data);
                    if (self.DM_DoiTuongKhachHang().length != 0) {
                        $(".Report_Empty").hide();
                        $(".page").show();
                        self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                        self.RowsEnd((_pageNumber - 1) * _pageSize + self.DM_DoiTuongKhachHang().length)
                    }
                    else {
                        $(".Report_Empty").show();
                        $(".page").hide();
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                    AllPage = data.numberPage;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsDoiTuongOF(data.Rowcount);
                    LoadingForm(false);
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
        else if (self.Loai_OptinForm() == 2) {
            $('#OptinKH').hide();
            $('#OptinLH').show();
        }
    }
    self.LoadReport();
    self.ResetCurrentPage = function () {
        $("#iconSort").remove();
        _pageSize = self.pageSize();
        _pageNumber = 1;
        if (self.Loai_OptinForm() == 1)
            self.pageNumber_KH(_pageNumber);
        else
            self.pageNumber_LH(_pageNumber);
        self.LoadReport();
    };
    self.DM_DoiTuongKhachHang_Page = ko.computed(function (x) {
        var first = (self.pageNumber_KH() - 1) * self.pageSize();
        if (self.DM_DoiTuongKhachHang() !== null) {
            if (self.DM_DoiTuongKhachHang().length != 0) {
                $(".Report_Empty").hide();
                $(".page").show();
                self.RowsStart((self.pageNumber_KH() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_KH() - 1) * self.pageSize() + self.DM_DoiTuongKhachHang().slice(first, first + self.pageSize()).length)
            }
            else {
                $(".Report_Empty").show();
                $(".page").hide();
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.DM_DoiTuongKhachHang().slice(first, first + _pageSize);
        }
        return null;
    })
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
        //loadHtmlGrid();
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
            if (self.Loai_OptinForm() == 1)
                self.pageNumber_KH(_pageNumber);
            else if (self.Loai_OptinForm() == 2)
                self.pageNumber_LH(_pageNumber);
            self.ReserPage();
        }
    };
    self.BackPage = function (item) {
        if (_pageNumber > 1) {
            _pageNumber = _pageNumber - 1;
            if (self.Loai_OptinForm() == 1)
                self.pageNumber_KH(_pageNumber);
            else if (self.Loai_OptinForm() == 2)
                self.pageNumber_LH(_pageNumber);
            self.ReserPage();
        }
    };
    self.EndPage = function (item) {
        _pageNumber = AllPage;
        if (self.Loai_OptinForm() == 1)
            self.pageNumber_KH(_pageNumber);
        else if (self.Loai_OptinForm() == 2)
            self.pageNumber_LH(_pageNumber);
        self.ReserPage();
    };
    self.StartPage = function (item) {
        _pageNumber = 1;
        if (self.Loai_OptinForm() == 1)
            self.pageNumber_KH(_pageNumber);
        else if (self.Loai_OptinForm() == 2)
            self.pageNumber_LH(_pageNumber);
        self.ReserPage();
    };
    self.gotoNextPage = function (item) {
        _pageNumber = item.SoTrang;
        if (self.Loai_OptinForm() == 1)
            self.pageNumber_KH(_pageNumber);
        else if (self.Loai_OptinForm() == 2)
            self.pageNumber_LH(_pageNumber);
        self.ReserPage();
    }
    // xóa
    var itemOF_Delete = null;
    self.deleteTenDoiTuong = ko.observable();
    self.modalDelete = function (item) {
        itemOF_Delete = item;
        self.deleteTenDoiTuong(item.TenDoiTuong);
        $('#modalpopup_deleteHD').modal('show');
    };
    self.Delete_TrangThaiDoiTuongOF = function () {
        var myData = {};
        myData.OptinForm_DoiTuong = itemOF_Delete;
        $.ajax({
            data: myData,
            url: OptinFormUri + "Delete_DoiTuongOF?ID_NhanVien=" + _id_NhanVien + "&ID_DonVi=" + _id_DonVi,
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa đối tượng thành công", "success");
                $('#modalpopup_deleteHD').modal('hide');
                self.LoadReport();
            },
            statusCode: {
                404: function (item) {
                    self.error("page not found");
                },
                500: function (item) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa đối tượng không thành công", "danger");
                }
            },
            complete: function (item) {
            }
        })
    }
    // xử lý
    self.Add_DoiTuongOF = ko.observableArray();
    self.add_LaCaNhan = ko.observable();
    self.add_GioiTinh = ko.observable();

    self.TinhThanhs = ko.observableArray();
    self.QuanHuyens = ko.observableArray();
    self.FileImgs = ko.observableArray();
    self.HaveImage_Select = ko.observable(false);
    self.HaveImage = ko.observable(false);
    self.FilesSelect = ko.observableArray();
    self.NguonKhachChosed = ko.observableArray();
    self.NguonKhachs = ko.observableArray();
    self.TrangThaiKhachHang = ko.observableArray();
    self.ListAllDoiTuong = ko.observableArray();
    self.NhomDoiTuongChosed = ko.observableArray();
    self.NhomDoiTuongDB = ko.observableArray();
    self.NhanViens = ko.observableArray();

    self.ID = ko.observable();
    self.MaDoiTuong = ko.observable();
    self.ID_NhomDoiTuong = ko.observable();
    self.ID_NguonKhach = ko.observable();
    self.ID_NguoiGioiThieu = ko.observable();
    self.ID_NhanVienPhuTrach = ko.observable();
    self.LaCaNhan = ko.observable();

    self.TenDoiTuong = ko.observable();
    self.Email = ko.observable();
    self.DiaChi = ko.observable();
    self.DienThoai = ko.observable();
    self.NgaySinh_NgayTLap = ko.observable();
    self.GioiTinhNam = ko.observable(true);
    self.MaSoThue = ko.observable();
    self.GhiChu = ko.observable();
    self.LoaiDoiTuong = loaiDoiTuong;
    self.NoHienTai = ko.observable();
    self.NoCanTra = ko.observable();
    self.CongTy = ko.observable();
    self.DiaChiChiNhanh = ko.observable();
    self.DienThoaiChiNhanh = ko.observable();
    self.TongTichDiem = ko.observable();
    self.GhiChu = ko.observable();
    self.AnhDaiDien = ko.observableArray();
    self.ID_VungMien = ko.observable();
    self.ID_KhuVuc = ko.observable();
    self.DinhDang_NgaySinh = ko.observable();
    self.NhomDoiTuongs = ko.observableArray();
    var idDonVi = $('#hd_IDdDonVi').val();
    $('#addNguonKhach').hide();
    $('#btnThemMoiNhom').hide();
    var user = $('#txtUserLogin').val(); // get at ViewBag
    var sLoai = 'khách hàng';
    self.modalAddDoiTuong = function (item) {
        // set default NV Phu Trach = NV login
        itemOF_Delete = item;
        if (item.ID_NhanVienPhuTrach == null || item.ID_NhanVienPhuTrach == const_GuidEmpty) {
            item.ID_NhanVienPhuTrach = idNhanVien;
        }
        self.newDoiTuong().SetData(item);
        self.newDoiTuong().ID_NhanVienPhuTrach(_id_NhanVien);
        self.IsOpenModalCus(true);

        var ngaysinhOld = item.ngaysinh;
        if (ngaysinhOld !== '') {
            self.NgaySinhOld_KhachHang(moment(ngaysinhOld, 'DD/MM/YYYY').format('MM-DD'));
        }
        getListTinhThanh();
        getListQuanHuyen(item.ID_TinhThanh);
        GetDM_NguonKHang();
        getListNhanVien();
        getListNhomDT();
        getListNhanVienNguoiDung();
        LoadTrangThai();
        GetAllKhachHang();
        self.HaveImage_Select(false);
        if (item.AnhDaiDien != null) {
            self.HaveImage_Select(true);
            var url = [{
                ID: null,
                URLAnh: item.AnhDaiDien,
                //URLAnh: '/ImageUpload/AnhKhachHang/20032019_KH01/9C.png',
                SoThuTu: 1
            }];
            self.FileImgs(url);
        }
        if (self.FileImgs() != null) {
            self.HaveImage_Select(true);
        }
        // khong can check quyen vi da an/hide button
        if (item.boolLaCaNhan) {
            $(".check-personal").show();
            $(".cheack-conpany").removeClass("tgg")
        }
        else {
            $(".check-personal").hide();
            $(".cheack-conpany").addClass("tgg");
        }
        $('#modalThemMoiKhachHang').modal('show');
        $('#lblTitleKH').text('Thêm mới khách hàng');
        self.FilesSelect(self.FileImgs());
    };
    function GetImages_DoiTuong(id) {
        ajaxHelper(DMDoiTuongUri + 'GetImages_DoiTuong/' + id, 'GET').done(function (data) {
            if (data !== null && data.length > 0) {
                self.HaveImage(true);

                // xóa những thẻ li có class = bx-clone (thẻ tự phát sinh của bx-slider)
                $('.bx-clone').remove();
                // nếu update ảnh: sử dung 2 lệnh này mới load được ảnh đại diện (OK)
                self.AnhDaiDien([]);
                self.AnhDaiDien.push(data[0]);
                self.FileImgs(data);
            }
            else {
                self.HaveImage(false);
                self.AnhDaiDien([]);
                self.FileImgs([]);
            }
        })
    }
    self.DeleteImg = function (item, event) {
        if (item.ID !== undefined && item.ID !== null) {
            dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn xóa ảnh của khách hàng <b>' + self.newDoiTuong().MaDoiTuong() + '</b> không?', function () {
                $.ajax({
                    type: "DELETE",
                    url: DMDoiTuongUri + "DeleteDM_DoiTuong_Anh/" + item.ID,
                    dataType: 'json',
                    contentType: 'application/json',
                    success: function (result) {
                        ShowMessage_Success("Xóa ảnh khách hàng thành công");
                    },
                    error: function (error) {
                        $('#modalPopuplgDelete').modal('hide');
                        ShowMessage_Danger("Xóa ảnh " + sLoai + " thất bại");
                    }
                });

                self.FilesSelect.remove(item);
                self.FileImgs.remove(item);

                if (self.FilesSelect().length == 0) {
                    self.HaveImage_Select(false);
                    self.AnhDaiDien([]);
                    $('#file').val('');
                }
            })

        } else {
            //self.FileImgs.remove(item);
            self.FilesSelect.remove(item);

            if (self.FilesSelect().length == 0) {
                self.HaveImage_Select(false);
                self.AnhDaiDien([]);
                $('#file').val('');
            }
        }
    }

    self.ChangeImage = function (item) {
        self.AnhDaiDien(item);
        $("img[id^='imgKH_']").removeClass('border');
        $('#imgKH_' + item.ID).addClass("border");
    }

    self.ZoomImage = function (item) {
        self.ImageIsZoom(item);
        $(".model-images").show();
        $(".modal-ontop").show();
    }

    self.CloseZoom = function () {
        $(".model-images").hide();
        $(".modal-ontop").hide();
    }
    self.ShowModalAddNhomKH_popup = function () {
        if (self.RoleInsert_Cus()) {
            $('#modalAddGroup').modal('show');
            self.booleanAddNhomDT(true);
            self.IsAddAtModal(true);
            self.autoUpdate(false);
            self.newNhomDoiTuong(new FormModel_NewNhomDoiTuong());// use at add nhom NCC
            self.DM_NhomDoiTuong_Tr([{ ID: null, TenNhomDoiTuong: null, GiamGia: null, GiamGiaTheoPhanTram: true, GhiChu: "" }]);// add nhomKH

            self.DieuKienNangNhom([]);
            self.refreshTab();
            _id_NhomDoiTuong = null;
        }
        else {
            ShowMessage_Danger('Không có quyền thêm mới nhóm khách hàng');
        }
    }
    self.ShowModal_InsertNguon_modalKH = function () {
        self.newNguonKhach(new FormModel_NewNguonKhach());
        self.IsModalNguon_modalKH(true);
        self.IsInsertNguon(true);
        $('#NguonKhach').modal('show');
    }
    self.InsertNguonKhach = function () {
        var tenNguon = self.newNguonKhach().TenNguonKhach();
        var _id = self.selectedNguonKhach();

        var DM_NguonKhach = {
            TenNguonKhach: tenNguon,
            NguoiTao: user,
        };

        if (self.IsInsertNguon()) {
            ajaxHelper(DMNguonKhachUri + 'PostDM_NguonKhachHang', 'POST', DM_NguonKhach).done(function (item) {

                self.NguonKhachs.unshift(item);

                if (self.IsModalNguon_modalKH()) {
                    self.ChoseNguonKhach(item);
                }
                else {
                    self.selectedNguonKhach(item.ID);
                }

                $('#NguonKhach').modal('hide');
                ShowMessage_Success('Thêm mới ngồn khách thành công');

                // Assign IsModalNguon_modalKH = false after insert/update Nguon
                self.IsModalNguon_modalKH(false);
            });
        }
        else {

            DM_NguonKhach.NguoiSua = userID;
            DM_NguonKhach.ID = _id;

            ajaxHelper(DMNguonKhachUri + 'PutDM_NguonKhachHang', 'PUT', DM_NguonKhach).done(function (item) {

                for (var i = 0; i < self.NguonKhachs().length; i++) {
                    if (self.NguonKhachs()[i].ID === _id) {
                        self.NguonKhachs.splice(i, 1);
                        break;
                    }
                }

                self.NguonKhachs.unshift(DM_NguonKhach);
                self.selectedNguonKhach(_id);

                $('#NguonKhach').modal('hide');
                ShowMessage_Success('Cập nhật ngồn khách thành công');
            });

            // after update Nguon, assign again IsInsertNguon = true;
            self.IsInsertNguon(true);
        }
    }
    self.filterNhanVien = function (item, inputString) {

        var itemSearch = locdau(item.TenNhanVien);
        var itemSearch2 = locdau(item.MaNhanVien);
        var locdauInput = locdau(inputString);
        // nen bat chat khong cho nhap dau cach o MaKH
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';

        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }

        return itemSearch.indexOf(locdauInput) > -1 || itemSearch2.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    }
    self.filterDoiTuong = function (item, inputString) {
        var itemSearch = locdau(item.TenDoiTuong);
        var itemSearch2 = locdau(item.MaDoiTuong);
        var itemSearch3 = locdau(item.DienThoai);
        var locdauInput = locdau(inputString);
        // nen bat chat khong cho nhap dau cach o MaKH
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';

        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }

        return itemSearch.indexOf(locdauInput) > -1 || itemSearch2.indexOf(locdauInput) > -1 || itemSearch3.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    }
    self.filterProvince = function (item, inputString) {
        var itemSearch = locdau(item.TenTinhThanh);
        var locdauInput = locdau(inputString);
        // nen bat chat khong cho nhap dau cach o MaKH
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';

        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }

        return itemSearch.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    }
    self.filterDistrict = function (item, inputString) {
        var itemSearch = locdau(item.TenQuanHuyen);
        var locdauInput = locdau(inputString);
        // nen bat chat khong cho nhap dau cach o MaKH
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';

        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }

        return itemSearch.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    }
    self.newDoiTuong().ID_TinhThanh.subscribe(function (newValue) {
        if (newValue !== undefined) {
            getListQuanHuyen(newValue);
            self.newDoiTuong().ID_TinhThanh(newValue);
        }
    });
    self.newDoiTuong().ID_NguoiGioiThieu.subscribe(function (newVal) {
        self.newDoiTuong().ID_NguoiGioiThieu(newVal);
    });

    self.newDoiTuong().ID_NhanVienPhuTrach.subscribe(function (newVal) {
        self.newDoiTuong().ID_NhanVienPhuTrach(newVal);
    });

    self.newDoiTuong().ID_QuanHuyen.subscribe(function (newVal) {
        self.newDoiTuong().ID_QuanHuyen(newVal);
    });
    function LoadSearchNhomDT() {
        var indexNhomDT = -1;
        // search auto Nhom DOiTuong in popup ChuyenNhom
        var model_KHPT = new Vue({
            el: '#divSearchNhomDT',
            data: function () {
                return {
                    query_NhomDT: '',
                    data_kh: self.NhomDoiTuongs()
                }
            },
            methods: {
                reset: function (item) {
                    this.data_kh = item;
                    this.query_NhomDT = '';
                },
                click: function (item) {
                    self.selectNhomDT_MoveGroup(item.ID);
                    $('#txtSearchNhomDT').val(item.TenNhomDoiTuong);
                    $('#showseach_NhomDT').hide();
                },
                submit: function (event) {
                    if (event.keyCode === 13) {
                        var result = this.fillter_KH(this.query_NhomDT);
                        var focus = false;
                        $('#showseach_NhomDT ul li').each(function (i) {
                            if ($(this).hasClass('hoverenabled')) {
                                self.selectNhomDT_MoveGroup(result[i].ID)
                                $('#showseach_NhomDT').hide();
                                focus = true;
                            }
                        });
                        if (result.length > 0 && this.query_NhomDT !== '' && focus === false) {
                            self.selectNhomDT_MoveGroup(result[0].ID);
                            $('#showseach_NhomDT').hide();
                        }
                    }
                    else if (event.keyCode === 40)//dows
                    {
                        indexNhomDT = indexNhomDT + 1;
                        if (indexNhomDT >= ($("#showseach_NhomDT ul li").length)) {
                            indexNhomDT = 0;
                            $('#showseach_NhomDT').stop().animate({
                                scrollTop: $('#showseach_NhomDT').offset().top - 600
                            }, 1000);
                        }
                        else if (indexNhomDT > 9 && indexNhomDT < $("#showseach_NhomDT ul li").length) {
                            $('#showseach_NhomDT').stop().animate({
                                scrollTop: $('#showseach_NhomDT').offset().top + 500
                            }, 1000);
                        }
                        this.loadFocus();

                    }
                    else if (event.keyCode === 38)//up
                    {
                        indexNhomDT = indexNhomDT - 1;
                        if (indexNhomDT < 0) {
                            indexNhomDT = $("#showseach_NhomDT ul li").length - 1;
                            $('#showseach_NhomDT').stop().animate({
                                scrollTop: $('#showseach_NhomDT').offset().top + 500
                            }, 1000);
                        }
                        else if (indexNhomDT > 0 && indexNhomDT < 10) {
                            $('#showseach_NhomDT').stop().animate({
                                scrollTop: $('#showseach_NhomDT').offset().top - 600
                            }, 1000);
                        }
                        this.loadFocus();

                    }
                },
                loadFocus: function () {
                    $('#showseach_NhomDT ul li').each(function (i) {
                        $(this).removeClass('hoverenabled');
                        if (indexNhomDT === i) {
                            $(this).addClass('hoverenabled');
                        }
                    });
                },
                // Tìm kiếm khách hàg
                fillter_KH: function (value) {
                    if (value === '') return this.data_kh.slice(0, 20);
                    return this.data_kh.filter(function (item) {
                        return containsAll(value.split(" "), item['Text_Search']) === true;
                    }).slice(0, 20);
                },

            },
            computed: {
                // Return Khách hàng
                searchResult: function () {

                    var result = this.fillter_KH(this.query_NhomDT);
                    if (result.length < 1 || this.query_NhomDT === '') {
                        $('#showseach_NhomDT').hide();
                    }
                    else {
                        indexNhomDT = -1;
                        $('#showseach_NhomDT').show();
                    }
                    $('#showseach_NhomDT ul li').each(function (i) {
                        $(this).removeClass('hoverenabled');
                    });
                    $('#showseach_NhomDT').stop().animate({
                        scrollTop: $('#showseach_NhomDT').offset().top - 600
                    }, 1000);
                    return result;
                }
            }
        });
    }
    function getListTinhThanh() {
        ajaxHelper(DMDoiTuongUri + "GetListTinhThanh", 'GET').done(function (x) {
            if (x.res === true) {
                self.TinhThanhs(x.data);
                //newModal_LienHe.listProvince(data);
            }
        });
    }
    function getListQuanHuyen(id) {
        if (id !== undefined && id !== null) {
            ajaxHelper(DMDoiTuongUri + "GetListQuanHuyen?idTinhThanh=" + id, 'GET').done(function (data) {
                if (data != null && data.length > 0) {
                    self.QuanHuyens(data);
                }
                else
                    self.newDoiTuong().ID_QuanHuyen(null);
            });
        }
    }
    $(window.document).on('shown.bs.modal', '.modal', function () {
        window.setTimeout(function () {
            $('[autofocus]', this).focus();
            $('[autofocus]').select();
        }.bind(this), 100);

        $('.datepicker_mask').datetimepicker({
            timepicker: false,
            mask: false,
            format: 'd/m/Y H:i',
        });
        DateNgaySinh = $("#txtNgaySinh").datepicker({
            showOn: 'focus',
            altFormat: "dd/mm/yy",
            buttonImage: '/Content/images/icon/ngaysinh.png',
            showOn: "button",
            buttonImageOnly: true,
            dateFormat: "dd/mm/yy"
        }).mask('99/99/9999').on("change", function (e) {
            console.log("Date changed: ", e.target.value);
            self.newDoiTuong().NgaySinh_NgayTLap(e.target.value);
        });
    });
    $.datetimepicker.setLocale('vi');

    function refreshDate() {
        DateNgaySinh = $("#txtNgaySinh").datepicker({
            showOn: 'focus',
            altFormat: "dd/mm/yy",
            buttonImage: '/Content/images/icon/ngaysinh.png',
            showOn: "button",
            buttonImageOnly: true,
            dateFormat: "dd/mm/yy"
        });
    }
    self.DinhDang_NgaySinh = ko.observable();
    self.GetTypeNgaySinh = function () {
        var type = self.DinhDang_NgaySinh();
        $('#lstTypeNgaySinh li').each(function () {
            $(this).children('i').remove();
        });
        switch (type) {
            case 'dd/MM':
                $('#lstTypeNgaySinh li:eq(1)').append('<i class="fa fa-check check-after-li"></i>');
                break;
            case 'MM/yyyy':
                $('#lstTypeNgaySinh li:eq(2)').append('<i class="fa fa-check check-after-li"></i>');
                break;
            case 'yyyy':
                $('#lstTypeNgaySinh li:eq(3)').append('<i class="fa fa-check check-after-li"></i>');
                break;
            default:
                $('#lstTypeNgaySinh li:eq(0)').append('<i class="fa fa-check check-after-li"></i>');
                break;
        }

    }
    self.ChangeType_NgaySinh = function (item) {
        self.DinhDang_NgaySinh(item.Value);
        self.GetTypeNgaySinh();
        switch (item.Value) {
            case 'dd/MM':
                DateNgaySinh.val(null);
                DateNgaySinh.datepicker("destroy");
                DateNgaySinh.mask('99/99').focus();
                break;
            case 'MM/yyyy':
                DateNgaySinh.val(null);
                DateNgaySinh.datepicker("destroy");
                DateNgaySinh.mask('99/9999').focus();
                break;
            case 'yyyy':
                DateNgaySinh.val(null);
                DateNgaySinh.datepicker("destroy");
                DateNgaySinh.mask('9999').focus();
                break;
            default:
                DateNgaySinh.val(null);
                refreshDate();
                DateNgaySinh.mask('99/99/9999').focus();
                break;
        }
    }
    function GetDM_NguonKHang() {
        if (navigator.onLine) {
            ajaxHelper(DMNguonKhachUri + 'GetDM_NguonKhach', 'GET').done(function (data) {
                var lc_NguonKhach = JSON.stringify(data);
                localStorage.setItem('lc_NguonKhachs', lc_NguonKhach);
                self.NguonKhachs(JSON.parse(lc_NguonKhach));
            });
        }
        else {
            var lc_NguonKhach = localStorage.getItem('lc_NguonKhachs');
            self.NguonKhachs(JSON.parse(lc_NguonKhach));
        }
    }
    function getListNhanVien() {
        // get all NhanVien all ChiNhanh --> because, share many ChiNhanh
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetNS_NhanVien_InforBasic?idDonVi=" + const_GuidEmpty, 'GET').done(function (data) {
            if (data !== null) {
                self.NhanViens(data);
            }
        });
    }
    function getListNhomDT() {
        ajaxHelper(DMNhomDoiTuongUri + "GetDM_NhomDoiTuong?loaiDoiTuong=" + loaiDoiTuong, 'GET').done(function (data) {
            if (data !== '' && data !== null) {
                self.NhomDoiTuongs(data);
                self.NhomDoiTuongDB(data);
            }
            var newObj = {
                ID: 0,
                TenNhomDoiTuong: 'Nhóm mặc định',
                Text_Search: 'nhom mac dinh nmd'
            }
            self.NhomDoiTuongs.unshift(newObj);

            // use search when ChuyenNhom
            LoadSearchNhomDT();
        });
    }
    function getListNhanVienNguoiDung() {
        // get all NhanVien all ChiNhanh --> because, share many ChiNhanh
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + 'GetNhanVien_NguoiDung', 'GET').done(function (x) {
            if (x.res === true) {
                let data = x.data;
                var lstNV_byDonVi = $.grep(data, function (x) {
                    return x.ID_DonVi === idDonVi;
                });
                self.NhanViens(lstNV_byDonVi);
            }
        });
    }
    function LoadTrangThai() {
        ajaxHelper(CSKHUri + 'GetTrangThaiTimKiem', 'GET').done(function (data) {
            if (data.res === true) {
                var lst = data.dataSoure.ttkhachhang;
                self.TrangThaiKhachHang(lst);
            }
        });
    };
    function GetAllKhachHang() {
        ajaxHelper(DMDoiTuongUri + 'GetListKH_InforBasic?idChiNhanh=' + idDonVi + '&loaiDoiTuong=' + loaiDoiTuong, 'GET').done(function (data) {
            if (data != null) {
                self.ListAllDoiTuong(data);
            }
        })
    }
    self.CloseNguonKhach = function () {
        self.NguonKhachChosed([]);

        $('#ddlNguonKhach li').each(function () {
            $(this).find('.fa-check').remove();
        });

        $('#choose_NguonKhach').append('<input type="text" class="dropdown form-control" placeholder="Chọn nguồn">');
    }

    self.ChoseNguonKhach = function (item) {

        self.newDoiTuong().ID_NguonKhach(item.ID); // assign ID_NguonKhach --> add new KhachHang

        self.NguonKhachChosed(item);

        var idNguon = item.ID;
        // thêm dấu check vào đối tượng được chọn (OK)
        $('#ddlNguonKhach li').each(function () {

            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>');
            }
            else {
                $(this).find('.fa-check').remove();
            }
        });

        // add class 'choose-person' : overflow, set width li
        $('#choose_NguonKhach input').remove();
        $('#choose_NguonKhach').addClass('choose-person');
    }
    self.selectManyNhomDT = function (item) {
        var arr = [];
        for (var i = 0; i < self.NhomDoiTuongChosed().length; i++) {
            if ($.inArray(self.NhomDoiTuongChosed()[i], arr) === -1) {
                arr.push(self.NhomDoiTuongChosed()[i].ID);
            }
        }
        if ($.inArray(item.ID, arr) === -1) {
            self.NhomDoiTuongChosed.push(item);
        }

        var arrID_After = [];
        for (var i = 0; i < self.NhomDoiTuongChosed().length; i++) {
            arrID_After.push(self.NhomDoiTuongChosed()[i].ID)
        }

        // thêm dấu check vào đối tượng được chọn (OK)
        $('#ddlNhomDT li').each(function () {

            if ($.inArray($(this).attr('id'), arrID_After) > -1) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
            }
            else {
                $(this).find('.fa-check').remove();
            }
        });

        // add class 'choose-person' : overflow, set width li
        $('#choose_NhomDT').addClass('choose-person');
        $('#choose_NhomDT input').remove();
    }

    self.CloseNhomDT = function (item) {
        self.NhomDoiTuongChosed.remove(item);
        if (self.NhomDoiTuongChosed().length === 0) {
            $('#choose_NhomDT').append('<input type="text" class="dropdown form-control" placeholder="Chọn nhóm">');
        }

        // remove check
        $('#ddlNhomDT li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
    }
    self.fileSelect = function (elemet, event) {
        var files = event.target.files;// FileList object
        // Loop through the FileList and render image files as thumbnails.

        var countErrType = 0;
        var countErrSize = 0;
        var errFileSame = '';
        var err = '';

        // Check Type file & Size
        for (var i = 0; i < files.length; i++) {

            if (!files[i].type.match('image.*')) {
                countErrType += 1;
            }

            var size = parseFloat(files[i].size / 1024).toFixed(2);
            if (size > 2048) {
                countErrSize += 1;
            }

            // check trung ten file
            for (var j = 0; j < self.FileImgs().length; j++) {

                var arrPath = self.FileImgs()[j].URLAnh.split('/');
                var fileName = arrPath[arrPath.length - 1];

                if (fileName === files[i].name) {
                    errFileSame += files[i].name + ', ';
                }
            }
        }

        // remove comma ,
        if (errFileSame !== '') {
            errFileSame = errFileSame.substr(0, errFileSame.length - 2)
        }

        if (countErrType > 0) {
            err = countErrType + ' file chưa đúng định dạng. ';
        }

        if (countErrSize > 0) {
            if (countErrType > 0) {
                if (errFileSame === '') {
                    // err type + error size
                    err += '<br />' + countErrSize + ' file có dung lượng > 2MB';
                }
                else {
                    // err type + error size + error exist file
                    err += '<br />' + countErrSize + ' file có dung lượng > 2MB' + '<br />' + ' File ' + errFileSame + ' đã tồn tại';
                }
            }
            else {
                // err size
                if (errFileSame === '') {
                    err = countErrSize + ' file có dung lượng > 2MB'
                }
                else {
                    // err size + error exist file
                    err = countErrSize + ' file có dung lượng > 2MB' + '<br />' + 'File ' + errFileSame + ' đã tồn tại';
                }
            }
        }
        else {
            if (countErrType > 0) {
                if (errFileSame === '') {
                    // err type
                    err = err;
                }
                else {
                    // err type + error exist file
                    err += '<br />' + 'File ' + errFileSame + ' đã tồn tại';
                }
            }
            else {
                // not err
                if (errFileSame === '') {
                    err = '';
                }
                else {
                    // error exist file
                    err = 'File ' + errFileSame + ' đã tồn tại';
                }
            }
        }

        if (err !== '') {
            ShowMessage_Danger(err);
        }

        for (var i = 0; i < files.length; i++) {
            var f = files[i];

            // Only process image files.
            if (!f.type.match('image.*')) {
                continue;
            }
            var size = parseFloat(f.size / 1024).toFixed(2);

            if (size <= 2048) {
                var reader = new FileReader();
                // Closure to capture the file information.
                reader.onload = (function (theFile) {
                    return function (e) {
                        self.FilesSelect.push(new FileModel(theFile, e.target.result));
                    };
                })(f);

                // Read in the image file as a data URL.
                reader.readAsDataURL(f);
                self.HaveImage_Select(true);
            }
        }
    };
    self.addKhachHang = function (formElement) {
        document.getElementById("btnLuuDoiTuong").disabled = true;
        document.getElementById("btnLuuDoiTuong").lastChild.data = " Đang lưu";
        var _id = self.newDoiTuong().ID();
        var _tenDoiTuong = self.newDoiTuong().TenDoiTuong();
        var _ngaySinh = self.newDoiTuong().NgaySinh_NgayTLap();
        var _maDT = self.newDoiTuong().MaDoiTuong();
        var _laCaNhan = self.newDoiTuong().LaCaNhan();
        var _typeNgaySinh = self.newDoiTuong().DinhDang_NgaySinh();
        var _noHienTai = self.newDoiTuong().NoHienTai();
        var _tongTichDiem = self.newDoiTuong().TongTichDiem();
        var _tongBan = self.newDoiTuong().TongBan();
        var _tongMua = self.newDoiTuong().TongMua();
        var _tongBanTruTraHang = self.newDoiTuong().TongBanTruTraHang();
        var _solanMuahang = self.newDoiTuong().SoLanMuaHang();

        var _idTinhThanh = self.newDoiTuong().ID_TinhThanh();
        _idTinhThanh = (_idTinhThanh === undefined || _idTinhThanh === '' ? null : _idTinhThanh);

        var _idQuanHuyen = self.newDoiTuong().ID_QuanHuyen();
        _idQuanHuyen = (_idQuanHuyen === undefined || _idQuanHuyen === '' ? null : _idQuanHuyen);

        var _idNguonKhach = self.newDoiTuong().ID_NguonKhach();
        _idNguonKhach = (_idNguonKhach === undefined || _idNguonKhach === '' ? null : _idNguonKhach);

        var _idNguoigioiThieu = self.newDoiTuong().ID_NguoiGioiThieu();
        _idNguoigioiThieu = (_idNguoigioiThieu === undefined || _idNguoigioiThieu === '' ? null : _idNguoigioiThieu);

        var _idNVienPhuTrach = self.newDoiTuong().ID_NhanVienPhuTrach();
        _idNVienPhuTrach = (_idNVienPhuTrach === undefined || _idNVienPhuTrach === '' ? null : _idNVienPhuTrach);

        var dtNow = new Date();
        var _yearNow = dtNow.getFullYear();

        // cho phep nhap NgaySinh = null
        if (_ngaySinh === undefined || _ngaySinh === 'Invalid date' || _ngaySinh === '') {
            self.newDoiTuong().NgaySinh_NgayTLap(null);
            _typeNgaySinh = null;
        }

        var msgCheck = CheckInput(self.newDoiTuong());
        if (msgCheck !== '') {
            ShowMessage_Danger(msgCheck);
            Enable_btnSaveDoiTuong();
            return false;
        }

        // if update: ngaysinh = dd/MM/yyyy
        if (_id !== null && _id !== undefined) {
            if (_ngaySinh !== null && _ngaySinh !== undefined) {
                if (_ngaySinh.length === 10) {
                    switch (_typeNgaySinh) {
                        case 'dd/MM':
                            _ngaySinh = _ngaySinh.substr(0, 5);
                            break;
                        case 'MM/yyyy':
                            _ngaySinh = _ngaySinh.substr(3, 7);
                            break;
                        case 'yyyy':
                            _ngaySinh = _ngaySinh.substr(6, 4);
                            break;
                        default:
                            _ngaySinh = _ngaySinh;
                            _typeNgaySinh = 'dd/MM/yyyy';
                    }
                }
            }
        }

        switch (_typeNgaySinh) {
            case 'dd/MM':
                _ngaySinh = _ngaySinh + '/' + _yearNow;
                break;
            case 'MM/yyyy':
                _ngaySinh = '01/' + _ngaySinh;
                break;
            case 'yyyy':
                _ngaySinh = '01/01/' + _ngaySinh;
                break;
            case null:
                _ngaySinh = null;
            default:
                _typeNgaySinh = 'dd/MM/yyyy';
                break;
        }

        if (_ngaySinh != null) {
            _ngaySinh = moment(_ngaySinh, 'DD/MM/YYYY').format('YYYY-MM-DD');

            var checkNS = CheckNgaySinh(_ngaySinh);
            if (!checkNS) {
                Enable_btnSaveDoiTuong();
                return;
            }
        }

        if (_idNguonKhach === undefined) {
            _idNguonKhach = null;
        }
        var tenTrangThai = '';
        var idTrangThai = $('#SL_TrangThaiKH').val();
        var itemCusType = $.grep(self.TrangThaiKhachHang(), function (x) {
            return x.ID === idTrangThai;
        });
        if (itemCusType.length > 0) {
            tenTrangThai = itemCusType[0].Name;
        }

        var DM_DoiTuong = {
            ID: _id,
            ID_NhomDoiTuong: null, // not use this field
            MaDoiTuong: self.newDoiTuong().MaDoiTuong(),
            TenDoiTuong: _tenDoiTuong,
            DienThoai: self.newDoiTuong().DienThoai(),
            Email: self.newDoiTuong().Email(),
            DiaChi: self.newDoiTuong().DiaChi(),
            GioiTinhNam: self.newDoiTuong().GioiTinhNam(),
            NgaySinh_NgayTLap: _ngaySinh,
            MaSoThue: self.newDoiTuong().MaSoThue(),
            LoaiDoiTuong: loaiDoiTuong,
            GhiChu: self.newDoiTuong().GhiChu(),
            ID_NguonKhach: _idNguonKhach,  // get ID_NguonKhach from NguonKach Insert
            ID_NguoiGioiThieu: _idNguoigioiThieu,
            ID_NhanVienPhuTrach: _idNVienPhuTrach,
            LaCaNhan: _laCaNhan,
            ID_TinhThanh: _idTinhThanh,
            ID_QuanHuyen: _idQuanHuyen,
            ID_DonVi: idDonVi,
            NguoiTao: user, // user dang nhap
            DinhDang_NgaySinh: _typeNgaySinh,

            // get to do bind after update
            TongBan: _tongBan,
            TongMua: _tongMua,
            NoHienTai: _noHienTai,
            TongBanTruTraHang: _tongBanTruTraHang,
            TongTichDiem: _tongTichDiem,
            SoLanMuaHang: _solanMuahang,
            ID_TrangThai: idTrangThai,
            TrangThaiKhachHang: tenTrangThai, // bind in list
        };

        console.log('DM_DoiTuong', DM_DoiTuong);
        callAjaxAdd(DM_DoiTuong);
        $('.line-right').height(0).css("margin-top", "0px");
    }
    function CheckInput(obj) {
        var sReturn = '';
        var id = obj.ID();
        var maDT = obj.MaDoiTuong();
        var tenDT = obj.TenDoiTuong();
        var date1 = obj.NgaySinh_NgayTLap();
        var date2 = moment(new Date()).format('YYYY-MM-DD');
        var email = obj.Email();
        var phone = obj.DienThoai();
        var idTinhThanh = obj.ID_TinhThanh();
        var idQuanHuyen = obj.ID_QuanHuyen();
        var idNguoiGioiThieu = obj.ID_NguoiGioiThieu();
        var idNguoiQuanLy = obj.ID_NhanVienPhuTrach();
        if (tenDT === null || tenDT === "" || tenDT === undefined) {
            sReturn = 'Vui lòng nhập tên ' + sLoai + '  <br />';
        }
        // insert
        var lstDoiTuong = self.ListAllDoiTuong();
        if (id === undefined) {
            for (var i = 0; i < lstDoiTuong.length; i++) {
                if (maDT !== undefined && lstDoiTuong[i].MaDoiTuong.toLowerCase() === maDT.trim().toLowerCase()) {
                    sReturn += 'Mã ' + sLoai + ' tồn tại <br />';
                    break;
                }

                var dienThoai = lstDoiTuong[i].DienThoai;
                if (dienThoai !== null && phone !== undefined
                    && dienThoai !== '' && phone !== '') {

                    if (dienThoai.trim() === phone.trim()) {
                        sReturn += 'Số điện thoại đã tồn tại <br />';
                        break;
                    }
                    else {
                        if (CheckIsNumber(phone) === false) {
                            sReturn += 'Số điện thoại phải là số <br />';
                            break;
                        }
                    }
                }

                if (lstDoiTuong[i].Email !== null && email !== undefined
                    && lstDoiTuong[i].Email !== '' && email !== ''
                    && lstDoiTuong[i].Email === email.trim()) {
                    sReturn += 'Email đã tồn tại <br />';
                    break;
                }
            }
        }
            // update
        else {
            for (var i = 0; i < lstDoiTuong.length; i++) {
                if (id !== lstDoiTuong[i].ID) {
                    if (maDT !== undefined && lstDoiTuong[i].MaDoiTuong === maDT.trim()) {
                        sReturn += 'Mã ' + sLoai + ' đã tồn tại <br />';
                        break;
                    }

                    var dienThoai2 = lstDoiTuong[i].DienThoai;
                    if (dienThoai2 !== null && phone !== undefined
                        && phone !== null && dienThoai2 !== '' && phone !== '') {

                        if (dienThoai2.trim() === phone.trim()) {
                            sReturn += 'Số điện thoại đã tồn tại <br />';
                            break;
                        }
                        else {
                            if (CheckIsNumber(phone) === false) {
                                sReturn += 'Số điện thoại phải là số <br />';
                                break;
                            }
                        }
                    }

                    if (lstDoiTuong[i].Email !== null && email !== undefined
                        && email !== null && lstDoiTuong[i].Email !== '' && email !== ''
                        && lstDoiTuong[i].Email === email.trim()) {
                        sReturn += 'Email đã tồn tại <br />';
                        break;
                    }
                }
            }
        }

        // check MaKhachHang nhập kí tự đặc biệt 
        if (CheckChar_Special(maDT)) {
            sReturn += 'Mã ' + sLoai + ' không được chứa kí tự đặc biệt <br />';
        }

        if (email != '' && email !== undefined && email !== null) {
            var re = /^(([^<>()[\]\\.,;:\s@#ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ\"]+(\.[^<>()[\]\\.,;:\s@#ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i;
            var valReturn = re.test(email);
            if (valReturn === false) {
                sReturn += 'Email không hợp lệ <br />';
            }
        }

        if (Is_undefined_empty_GuidEmpty(idTinhThanh) === false) {
            var itemTT = $.grep(self.TinhThanhs(), function (item) {
                return item.ID === idTinhThanh;
            });
            if (itemTT.length === 0) {
                sReturn += 'Tỉnh thành không tồn tại trong hệ thống <br />';
            }
        }

        if (Is_undefined_empty_GuidEmpty(idQuanHuyen) === false) {
            var itemQH = $.grep(self.QuanHuyens(), function (item) {
                return item.ID === idQuanHuyen;
            });
            if (itemQH.length === 0) {
                sReturn += 'Quận huyện không tồn tại trong hệ thống <br />';
            }
        }

        if (Is_undefined_empty_GuidEmpty(idNguoiGioiThieu) === false) {
            var itemKH = $.grep(self.ListAllDoiTuong(), function (item) {
                return item.ID === idNguoiGioiThieu;
            });
            if (itemKH.length === 0) {
                sReturn += 'Người giới thiệu không tồn tại trong hệ thống <br />';
            }
        }

        if (Is_undefined_empty_GuidEmpty(idNguoiQuanLy) === false) {
            var itemNV = $.grep(self.NhanViens(), function (item) {
                return item.ID === idNguoiQuanLy;
            });
            if (itemNV.length === 0) {
                sReturn += 'Người quản lý không tồn tại trong hệ thống <br />';
            }
        }
        return sReturn;
    }

    function CheckNgaySinh(valDate) {

        var dateNow = moment(new Date()).format('YYYY-MM-DD');

        if (valDate !== null && valDate !== '') {

            var check = isValidDateYYYYMMDD(valDate);
            if (!check) {
                ShowMessage_Danger("Ngày sinh chưa đúng định dạng");
                return false;
            }

            // if type ='dd/MM' --> not compare date
            var typeNgaySinh = self.newDoiTuong().DinhDang_NgaySinh();
            if (typeNgaySinh !== null && typeNgaySinh !== 'dd/MM') {
                if (valDate > dateNow) {
                    ShowMessage_Danger("Ngày sinh không được lớn hơn ngày hiện tại");
                    return false;
                }
            }
        }
        return true;
    }
    function callAjaxAdd(DM_DoiTuong) {
        $.ajax({
            data: DM_DoiTuong,
            url: DMDoiTuongUri + "PostDM_DoiTuong",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (obj) {
                if (obj.res === true) {
                    var item = obj.data;
                    ShowMessage_Success("Thêm mới " + sLoai + " thành công");

                    // insert DM_DoiTuong_Nhom (many Group)
                    var lstDM_DoiTuong_Nhom = [];
                    var sNhoms = '';
                    var idNhoms = '';

                    for (var i = 0; i < self.NhomDoiTuongChosed().length; i++) {
                        var itemFor = self.NhomDoiTuongChosed()[i];
                        var objDTNhom = {
                            ID_DoiTuong: item.ID,
                            ID_NhomDoiTuong: itemFor.ID,
                        }

                        lstDM_DoiTuong_Nhom.push(objDTNhom);
                        sNhoms += itemFor.TenNhomDoiTuong + ', ';
                        idNhoms += itemFor.ID + ', ';
                    }
                    self.InsertImage(item.ID, item.MaDoiTuong);
                    DM_DoiTuong.ID = item.ID; 
                    DM_DoiTuong.MaDoiTuong = item.MaDoiTuong; 
                    DM_DoiTuong.ID_NhomDoiTuong = idNhoms; 
                    DM_DoiTuong.ListAllDoiTuong.unshift(DM_DoiTuong); // unshift--> check nang nhom
                    self.Update_TrangThaiDoiTuongOF();
                    UpdateNhomKH_DB(item.ID);
                    Insert_NhatKyThaoTac(DM_DoiTuong, 1, 1);
                    //LoadHtmlGridKH();
                }
                else {
                    ShowMessage_Danger(obj.mes);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                ShowMessage_Danger("Thêm mới " + sLoai + " thất bại");
            },
            complete: function () {
                $("#modalThemMoiKhachHang").modal("hide");
                self.LoadReport();
                Enable_btnSaveDoiTuong();
            }
        })
    }
    self.Update_TrangThaiDoiTuongOF = function () {
        var myData = {};
        myData.OptinForm_DoiTuong = itemOF_Delete;
        $.ajax({
            data: myData,
            url: OptinFormUri + "Update_DoiTuongOF",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
            },
            statusCode: {
                404: function (item) {
                    self.error("page not found");
                },
                500: function (item) {
                }
            },
            complete: function (item) {
            }
        })
    }
    function UpdateNhomKH_DB(idDoiTuong) {
        var itemDT = $.grep(self.ListAllDoiTuong(), function (x) {
            return x.ID === idDoiTuong;
        });

        if (itemDT.length > 0) {
            var lstNhomNang = [];
            var arrIDNhom = $.unique(itemDT[0].ID_NhomDoiTuong.trim().split(','));
            for (var i = 0; i < arrIDNhom.length; i++) {
                if (arrIDNhom[i].trim() !== '') {
                    var objDTNhom = {
                        ID_DoiTuong: idDoiTuong,
                        ID_NhomDoiTuong: arrIDNhom[i].trim(),
                    }
                    lstNhomNang.push(objDTNhom);
                }
            }

            if (lstNhomNang.length > 0) {
                Update_ManyNhom(lstNhomNang, false);
            }
            else {
                // delete all nhom of DoiTuon in DB
                ajaxHelper(DMDoiTuongUri + 'DeleteAllNhom_ofDoiTuong?idDoiTuong=' + idDoiTuong, 'PUT').done(function (x) {

                })
            }
        }
    }
    function Update_ManyNhom(lstNhom, isMoveGroup) {

        lstNhom = $.unique(lstNhom);

        var myData = {};
        myData.lstDM_DoiTuong_Nhom = lstNhom;

        console.log('lstNhom ', lstNhom)

        $.ajax({
            data: myData,
            url: DMDoiTuongUri + "PutDM_DoiTuong_Nhom",
            type: 'PUT',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (item) {
                if (isMoveGroup) {
                    SearchKhachHang(false, false);;
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                ShowMessage_Danger("Cập nhật " + sLoai + " thất bại");
            },
            complete: function () {
            }
        })
    }
    var style1 = '<a style= \"cursor: pointer\" onclick = \"';
    var style2 = "('";
    var style3 = "')\" >";
    var style4 = '</a>';
    function Insert_NhatKyThaoTac(objUsing, chucNang, loaiNhatKy) {
        // chuc nang (1.DoiTuong, 2.NhomDoiTuong, 3.PhieuThu, 4.Export, 5.Import)
        var tenChucNang = '';
        var noiDung = '';
        var noiDungChiTiet = '';
        var txtFirst = '';
        var tenChucNangLowercase = '';

        var funcNameKH = 'LoadKhachHang_byMaKH';
        var funcNameSoQuy = 'LoadQuyHD_byMa';

        switch (loaiNhatKy) {
            case 1:
                txtFirst = 'Thêm mới ';
                break;
            case 2:
                txtFirst = 'Cập nhật ';
                break;
            case 3:
                txtFirst = 'Xóa ';
                break;
            case 5:
                txtFirst = 'Import ';
                break;
            case 6:
                txtFirst = 'Xuất file ';
                break;
        }

        if (loaiDoiTuong === 2) {
            tenChucNangLowercase = 'nhà cung cấp ';
        }
        else {
            tenChucNangLowercase = 'khách hàng ';
        }

        if (chucNang === 1) {
            if (loaiDoiTuong === 2) {
                tenChucNang = 'Nhà cung cấp';
            }
            else {
                tenChucNang = 'Khách hàng';
            }

            if (loaiNhatKy < 4) {
                // them, sua, xoa
                var maDoiTuong = objUsing.MaDoiTuong;
                var ngaySinh = '';
                var tenNhom = '';
                var dienThoai = '';

                if (objUsing.NgaySinh_NgayTLap !== null && objUsing.NgaySinh_NgayTLap !== undefined) {
                    ngaySinh = 'Ngày sinh: ' + moment(objUsing.NgaySinh_NgayTLap, 'YYYY-MM-DD HH:mm:ss').format('DD/MM/YYYY') + ', ';
                }
                if (objUsing.TenNhomDT !== '') {
                    tenNhom = 'Nhóm: ' + objUsing.TenNhomDT + ', ';
                }
                if (objUsing.DienThoai !== null && objUsing.DienThoai !== undefined) {
                    dienThoai = 'Điện thoại: ' + objUsing.DienThoai + ', ';
                }
                noiDung = txtFirst.concat(tenChucNangLowercase, maDoiTuong, ', Tên: ', objUsing.TenDoiTuong, ', ', ngaySinh, dienThoai, tenNhom);
                noiDungChiTiet = txtFirst.concat(tenChucNangLowercase, style1, funcNameKH, style2, maDoiTuong, style3, maDoiTuong, style4, ', tên: ', objUsing.TenDoiTuong, ', ',
                    ngaySinh, dienThoai, tenNhom);
                noiDungChiTiet = Remove_LastComma(noiDungChiTiet);
                noiDungChiTiet = noiDungChiTiet.concat('<br /> Nhân viên thực hiện: ', user);
            }
            else {
                // import, export
                noiDung = txtFirst.concat('danh sách ', tenChucNangLowercase);
                noiDungChiTiet = noiDung.concat('<br /> Nhân viên thực hiện: ', user)
            }
        }

        if (chucNang === 2) {
            if (loaiDoiTuong === 2) {
                tenChucNang = 'Nhà cung cấp';
            }
            else {
                tenChucNang = 'Khách hàng';
            }
            noiDung = txtFirst.concat('Nhóm ', tenChucNangLowercase, objUsing.TenNhomDoiTuong);
            noiDungChiTiet = noiDung.concat('<br /> Nhân viên thực hiện: ', user)
        }

        if (chucNang === 3) {
            var phaiTT = formatNumber(objUsing.TongTienThu);
            var ngaylapHD = moment(objUsing.NgayLapHoaDon, 'YYYY-MM-DD HH:mm:ss').format('DD/MM/YYYY HH:mm:ss');
            var styleMaHD = ''.concat(style1, funcNameSoQuy, style2, objUsing.MaHoaDon, style3, objUsing.MaHoaDon, style4);
            // 11.Thu, 12.Chi
            if (objUsing.LoaiHoaDon === 11) {
                tenChucNang = 'Phiếu thu ';
            }
            else {
                tenChucNang = 'Phiếu chi ';
            }

            noiDung = txtFirst.concat(tenChucNang, objUsing.MaHoaDon, ' với giá trị: ', phaiTT, ', Phương thức thanh toán: ', objUsing.PhuongThucTT, ', Thời gian: ', ngaylapHD);
            noiDungChiTiet = txtFirst.concat(tenChucNang, styleMaHD, '<br/ > Giá trị: ', phaiTT, '<br/ > Phương thức thanh toán: ', objUsing.PhuongThucTT, '<br/ > Thời gian: ', ngaylapHD)
        }

        // insert HT_NhatKySuDung
        var objNhatKy = {
            ID_NhanVien: idNhanVien,
            ID_DonVi: idDonVi,
            ChucNang: tenChucNang,
            LoaiNhatKy: loaiNhatKy,
            NoiDung: noiDung,
            NoiDungChiTiet: noiDungChiTiet,
        };
        Insert_NhatKyThaoTac_1Param(objNhatKy);
    }
    self.InsertImage = function (idDoiTuong, maDoiTuong) {
        for (var i = 0; i < self.FilesSelect().length; i++) {
            var formData = new FormData();
            formData.append("file", self.FilesSelect()[i].file);
            $.ajax({
                type: "POST",
                url: DMDoiTuongUri + "ImageUpload?id=" + idDoiTuong + '&pathFolder=AnhKhachHang%2f' + maDoiTuong,
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                async: false,
                success: function (response) {

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log('err');
                }
            });
        }
    }
    function Enable_btnSaveDoiTuong() {
        document.getElementById("btnLuuDoiTuong").disabled = false;
        document.getElementById("btnLuuDoiTuong").lastChild.data = "Lưu";
    }
    // xử lý số điện thoại trùng
    self.DoiTuongXuLy = ko.observableArray();
    self.Show_XuLyKhachHang = function (item) {
        self.DoiTuongXuLy(item);
        $('#modalXuLyKhachHang').modal('show');
    }
    var i = 0;
    self.Update_KhachHangSDT = function () {
        var item = self.DoiTuongXuLy();
        $('.form-xu-ly-lich-hen-dk .form-group').each(function () {
            // var isGenderMale = $('.checkbox-modal').prop('checked');
            //$(this).find('.checkbox-modal').prop('checked', check)
            var check = $(this).is(':checked');
            var isGenderMale = $(this).find('.checkbox-modal').is(':checked')
            console.log(i, isGenderMale);
            if (i == 1 && isGenderMale == false)
                item.TenDoiTuong = item.TenKhachHang;
            if (i == 2 && isGenderMale == false)
                item.SoDienThoai = item.DienThoaiKhachHang
            if (i == 3 && isGenderMale == false)
                item.boolGioiTinh = item.GioiTinhKhachHang
            if (i == 4 && isGenderMale == false)
                item.NgaySinh = item.NgaySinhKhachHang;
            if (i == 5 && isGenderMale == false)
                item.Email = item.EmailKhachHang;
            if (i == 6 && isGenderMale == false)
                item.DiaChi = ite.DiaChiKhachHang;
            if (i == 7 && isGenderMale == false)
                item.MaSoThue = item.MaSoThueKhachHang;
            if (i == 8 && isGenderMale == false)
                item.boolLaCaNhan = item.LaCaNhanKhachHang;
            i = i + 1;
        });
        // set default NV Phu Trach = NV login

        if (item.ID_NhanVienPhuTrach == null || item.ID_NhanVienPhuTrach == const_GuidEmpty) {
            item.ID_NhanVienPhuTrach = idNhanVien;
        }
        self.newDoiTuong().SetData(item);
        self.newDoiTuong().ID_NhanVienPhuTrach(_id_NhanVien);
        self.IsOpenModalCus(true);

        // used to check remind birthday customer (ngaySinhOld != ngaySinhNew)
        var ngaysinhOld = item.ngaysinh;
        if (ngaysinhOld !== '') {
            self.NgaySinhOld_KhachHang(moment(ngaysinhOld, 'DD/MM/YYYY').format('MM-DD'));
        }
        getListTinhThanh();
        getListQuanHuyen(item.ID_TinhThanh);
        GetDM_NguonKHang();
        getListNhanVien();
        getListNhomDT();
        getListNhanVienNguoiDung();
        LoadTrangThai();
        GetAllKhachHang();
        self.HaveImage_Select(false);
        if (item.AnhDaiDien != null) {
            self.HaveImage_Select(true);
            var url = [{
                ID: null,
                URLAnh: item.AnhDaiDien,
                SoThuTu: 1
            }];
            self.FileImgs(url);
        }
        if (self.FileImgs() != null) {
            self.HaveImage_Select(true);
        }
        // khong can check quyen vi da an/hide button
        if (item.LaCaNhan) {
            $(".check-personal").show();
            $(".cheack-conpany").removeClass("tgg")
        }
        else {
            $(".check-personal").hide();
            $(".cheack-conpany").addClass("tgg");
        }
        $('#modalThemMoiKhachHang').modal('show');
        $('#lblTitleKH').text('Thêm mới khách hàng');
        self.FilesSelect(self.FileImgs());
    };
};
ko.applyBindings(new ViewModal());
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
var FileModel = function (filef, srcf) {
    var self = this;
    this.file = filef;
    this.URLAnh = srcf;
};