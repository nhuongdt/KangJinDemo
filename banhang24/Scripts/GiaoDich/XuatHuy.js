
var loaiHoaDon = 8;
function ViewModel() {
    var self = this;
    var BH_XuatHuyUri = '/api/DanhMuc/BH_XuatHuyAPI/';
    var ReportUri = '/api/DanhMuc/ReportAPI/';

    var _shopCookies = VHeader.IdNganhNgheKinhDoanh;
    var _userLogin = VHeader.UserLogin;
    var _id_DonVi = VHeader.IdDonVi;
    var _id_NhanVien = VHeader.IdNhanVien;
    var _IDNguoiDung = VHeader.IdNguoiDung;
    var Key_Form = 'Key_DamageItems';
    //loaiHoaDon = 8; //  1.xuat sudung gdv, 2.xuat banle, 3. xuat suachua, 8.xuatkho

    self.TenChiNhanh = ko.observable();
    self.searchDonVi = ko.observableArray();
    self.MaHangHoa_Search = ko.observable();
    self.SumNumberPageReportHangHoa = ko.observableArray();
    self.RowsStart = ko.observable('0');
    self.RowsEnd = ko.observable('10');
    self.RowsStart_XHCT = ko.observable('0');
    self.RowsEnd_XHCT = ko.observable('10');
    self.SumRowsXHCTChuyenHang = ko.observable();
    self.SumNumberPageReportXHCT = ko.observableArray();
    self.PageList = ko.observableArray();
    self.fromitem = ko.observable();
    self.toitem = ko.observable();
    self.HoaDons = ko.observableArray();
    self.CongTy = ko.observableArray();

    self.numberPG = ko.observable(1);
    self.columsort = ko.observable(null);
    self.sort = ko.observable(null);
    self.checkTamLuu = ko.observable(true);
    self.checkHoanThanh = ko.observable(false);
    self.checkHuy = ko.observable(false);
    self.RowsHangHoas = ko.observable();
    self.pageHangHoas = ko.observableArray();
    self.BH_HoaDon_ChiTiet = ko.observableArray();
    self.NhanViens = ko.observableArray();
    self.NguoiDungs = ko.observableArray();
    self.selectIDNV = ko.observable();
    self.InforHDprintf = ko.observableArray();
    self.CTHoaDonPrint = ko.observableArray();
    self.ListCheckBox = ko.observableArray();
    self.IsGara = ko.observable(false);

    //Phân quyền 
    self.HangHoa_XemDS = ko.observable(false);
    self.XuatHuy_XuatFile = ko.observable(false);
    self.XuatHuy_ThemMoi = ko.observable(false);
    self.XuatHuy_CapNhat = ko.observable(false);
    self.XuatHuy_SaoChep = ko.observable(false);
    self.XuatHuy_MoPhieu = ko.observable(false);
    self.XuatHuy_Xoa = ko.observable(false);
    self.XuatHuy_XemDS_PhongBan = ko.observable(false);
    self.XuatHuy_XemDS_HeThong = ko.observable(false);
    self.XuatHuy_ThayDoiThoiGian = ko.observable(false);
    self.HangHoa_XemGiaVon = ko.observable();
    self.XuatHuy_SuaDoi = ko.observable(false);

    $('#txtSelectHT').attr('disabled', 'false');
    $('.modal-backdrop').remove();// used to when go to back this page at xuatkhochitiet

    console.log('_shopCookies ', _shopCookies)
    switch (_shopCookies.toUpperCase()) {
        case 'C16EDDA0-F6D0-43E1-A469-844FAB143014':
            self.IsGara(true);
            break;
        default:
            self.IsGara(false);
            break;
    }

    //load quyền
    self.ThietLap = ko.observableArray();
    function loadQuyenIndex() {
        var arrQuyen = [];
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetHT_NhomNguoiDung?idnguoidung=" + _IDNguoiDung + '&iddonvi=' + _id_DonVi, 'GET').done(function (data) {

            if (data.HT_Quyen_NhomDTO.length > 0) {
                for (var i = 0; i < data.HT_Quyen_NhomDTO.length; i++) {
                    arrQuyen.push(data.HT_Quyen_NhomDTO[i].MaQuyen);
                }
            }
            localStorage.setItem('lc_CTQuyen', JSON.stringify(arrQuyen));
        });
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + 'GetCauHinhHeThong/' + _id_DonVi, 'GET').done(function (data) {
            localStorage.setItem('lc_CTThietLap', JSON.stringify(data));
            self.ThietLap(data);
        });
    }

    function LoadColumnCheck() {
        ajaxHelper('/api/DanhMuc/BaseAPI/' + "GetListColumnInvoices?loaiHD=8", 'GET').done(function (data) {
            self.ListCheckBox(data);
            LoadHtmlGrid();
        });
    }

    function LoadHtmlGrid() {
        LocalCaches.LoadFirstColumnGrid(Key_Form, $('#myList ul li input[type = checkbox]'), self.ListCheckBox());
    }

    // hide/show column from checkbox
    $('#myList').on('change', 'ul li input[type = checkbox]', function () {
        var valueCheck = $(this).val();
        LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, valueCheck);
        $('.' + valueCheck).toggle();
    });
    $('#myList').on('click', 'ul li', function (i) {
        if ($(this).find('input[type = checkbox]').is(':checked')) {
            $(this).find('input[type = checkbox]').prop("checked", false);
        }
        else {
            $(this).find('input[type = checkbox]').prop("checked", true);
        }
        var valueCheck = $(this).find('input[type = checkbox]').val();
        LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, valueCheck);
        $('.' + valueCheck).toggle();
    });

    function CheckRole(maquyen) {
        return VHeader.Quyen.indexOf(maquyen) > -1;
    }

    function getQuyen_NguoiDung() {
        ajaxHelper(ReportUri + "getQuyenXemGiaVon?ID_NguoiDung=" + _IDNguoiDung + "&MaQuyen=" + "HangHoa_XemGiaVon", "GET").done(function (data) {
            self.HangHoa_XemGiaVon(data);
        })

        self.XuatHuy_XuatFile(CheckRole('XuatHuy_XuatFile'));
        self.XuatHuy_ThemMoi(CheckRole('XuatHuy_ThemMoi'));
        self.XuatHuy_CapNhat(CheckRole('XuatHuy_CapNhat'));
        self.XuatHuy_SuaDoi(CheckRole('XuatHuy_SuaDoi'));
        self.XuatHuy_SaoChep(CheckRole('XuatHuy_SaoChep'));
        self.XuatHuy_MoPhieu(CheckRole('XuatHuy_MoPhieu'));
        self.XuatHuy_Xoa(CheckRole('XuatHuy_Xoa'));
        self.XuatHuy_ThayDoiThoiGian(CheckRole('XuatHuy_ThayDoiThoiGian'));
        self.HangHoa_XemDS(CheckRole('HangHoa_XemDS'));
        self.XuatHuy_XemDS_HeThong(CheckRole('XuatHuy_XemDS_HeThong'));
        self.XuatHuy_XemDS_PhongBan(CheckRole('XuatHuy_XemDS_PhongBan'));

    };

    var timeStart = null;
    var timeEnd = null;
    self.timeValue = ko.observable('Tháng này');
    self.TodayBC = ko.observable('');
    self.DonVis = ko.observableArray();
    self.MangChiNhanh = ko.observableArray();
    self.filterNgayPhieuHuy = ko.observable('0');
    self.NgayTaoLH = ko.observable('6');
    self.pageSizes = [10, 20, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.PageCount = ko.observable(1);
    self.currentPageHangHoa = ko.observable(1);
    self.currentPageXHCT = ko.observable(1);

    self.GetClass = function (page) {
        return ((page.SoTrang - 1) === self.currentPage()) ? "click" : "";
    };
    self.GetClassHangHoa = function (page) {
        return (page.SoTrang === self.currentPageHangHoa()) ? "click" : "";
    };
    self.GetClassXHCT = function (page) {
        return (page.SoTrang === self.currentPageXHCT()) ? "click" : "";
    };
    //load đơn vị
    function getDonVi() {
        ajaxHelper('/api/DanhMuc/DM_DonViAPI/' + "GetListDonViByIDNguoiDung?idnhanvien=" + _id_NhanVien, 'GET').done(function (data) {
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

    self.CloseDonVi = function (item) {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);

        self.MangChiNhanh.remove(item);
        if (self.MangChiNhanh().length === 0) {
            $("#NoteNameDonVi").attr("placeholder", "Chọn chi nhánh...");
        }
        getAllHoaDon();
    }

    self.SelectedDonVi = function (item) {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);

        var arrIDDonVi = [];
        for (var i = 0; i < self.MangChiNhanh().length; i++) {
            if ($.inArray(self.MangChiNhanh()[i], arrIDDonVi) === -1) {
                arrIDDonVi.push(self.MangChiNhanh()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDDonVi) === -1) {
            self.MangChiNhanh.push(item);
            getAllHoaDon();
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

    self.PhieuXuatHuy = function () {
        localStorage.setItem('XK_createfrom', 0);
        GoToThemMoiXuatKho();
    }
    //load Loai Chung Tu
    self.MangChungTu = ko.observableArray();
    self.ChungTus = ko.observableArray([
        { ID: 8, TenChungTu: 'Xuất kho' },
        { ID: 38, TenChungTu: 'Xuất bán lẻ' },
        { ID: 39, TenChungTu: 'Xuất bảo hành' },
        { ID: 35, TenChungTu: 'Xuất nguyên vật liệu' },
        { ID: 40, TenChungTu: 'Xuất hỗ trợ chung' },
        { ID: 37, TenChungTu: 'Xuất hỗ trợ ngày thuốc' },
    ]);

    self.CloseChungTu = function (item) {
        self.MangChungTu.remove(item);

        if (self.MangChungTu().length === 0) {
            $("#NoteNameChungTu").attr("placeholder", "Chọn chứng từ...");
        }
        $('#selec-all-ChungTu li').each(function () {
            if ($(this).attr('id') == item.ID) {
                $(this).find('i').remove();
            }
        });
        getAllHoaDon();
    }
    self.SelectedChungTu = function (item) {
        var arrIDChungTu = [];
        for (var i = 0; i < self.MangChungTu().length; i++) {
            if ($.inArray(self.MangChungTu()[i], arrIDChungTu) === -1) {
                arrIDChungTu.push(self.MangChungTu()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDChungTu) === -1) {
            self.MangChungTu.push(item);
            $('#NoteNameChungTu').removeAttr('placeholder');
            getAllHoaDon();
        }
        //thêm dấu check vào đối tượng được chọn
        $('#selec-all-ChungTu li').each(function () {
            if ($(this).attr('id') == item.ID) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });

    }

    function LoadingForm(IsShow) {
        $('.tab-show .tab-pane').each(function () {
            if ($(this).hasClass('active')) {
                var top = $(this).find('.table-reponsive').height() / 2;
                var style = "top:" + (top > 30 ? top - 30 : top) + "px";
                $(this).find('.table-reponsive').gridLoader({ show: IsShow, style: style });
            }
        });
    }
    self._ThanhTien = ko.observable();

    function GetParamSearch() {
        var txtSeach = self.MaHangHoa_Search();
        var _now = new Date();
        var FindHD = localStorage.getItem('FindHD');
        if (FindHD !== null) {
            txtSeach = FindHD;
            timeStart = '2015-09-26'
            timeEnd = moment(_now).add(1, 'days').format('YYYY-MM-DD');
            localStorage.removeItem('FindHD');
            self.MaHangHoa_Search(txtSeach);
        }
        else {
            if (self.filterNgayPhieuHuy() === '0') {
                switch (parseInt(self.NgayTaoLH())) {
                    case 0:
                        // all
                        self.TodayBC('Toàn thời gian');
                        timeStart = '2016-01-01';
                        timeEnd = moment(_now).add(1, 'days').format('YYYY-MM-DD');
                        break;
                    case 1:
                        // hom nay
                        self.TodayBC('Hôm nay');
                        timeStart = moment(_now).format('YYYY-MM-DD');
                        timeEnd = moment(_now).add(1, 'days').format('YYYY-MM-DD');
                        break;
                    case 2:
                        // hom qua
                        self.TodayBC('Hôm qua');
                        timeEnd = moment(_now).format('YYYY-MM-DD');
                        timeStart = moment(_now).subtract(1, 'days').format('YYYY-MM-DD');
                        break;
                    case 3:
                        // tuan nay
                        self.TodayBC('Tuần này');
                        timeStart = moment().startOf('week').add('days', 1).format('YYYY-MM-DD');
                        timeEnd = moment().endOf('week').add(2, 'days').format('YYYY-MM-DD');
                        break;
                    case 4:
                        // tuan truoc
                        self.TodayBC('Tuần trước');
                        timeStart = moment().weekday(-6).format('YYYY-MM-DD');
                        timeEnd = moment(timeStart, 'YYYY-MM-DD').add(7, 'days').format('YYYY-MM-DD'); // add day in moment.js
                        break;
                    case 5:
                        // 7 ngay qua
                        self.TodayBC('7 ngày qua');
                        timeEnd = moment(_now).format('YYYY-MM-DD');
                        timeStart = moment(_now).subtract(7, 'days').format('YYYY-MM-DD');
                        break;
                    case 6:
                        // thang nay
                        self.TodayBC('Tháng này');
                        timeStart = moment().startOf('month').format('YYYY-MM-DD');
                        timeEnd = moment().endOf('month').add(1, 'days').format('YYYY-MM-DD'); // add them 1 ngày 01-month-year --> compare in SQL
                        break;
                    case 7:
                        // thang truoc
                        self.TodayBC('Tháng trước');
                        timeStart = moment().subtract(1, 'months').startOf('month').format('YYYY-MM-DD');
                        timeEnd = moment().subtract(1, 'months').endOf('month').add(1, 'days').format('YYYY-MM-DD');
                        break;
                    case 11:
                        // quy nay
                        self.TodayBC('Quý này');
                        timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
                        timeEnd = moment().endOf('quarter').add(1, 'days').format('YYYY-MM-DD');
                        break;
                    case 12:
                        // quy truoc = currQuarter -1; // if (currQuarter -1 === 0) --> (assign = 1)
                        self.TodayBC('Quý trước');
                        var prevQuarter = moment().quarter() - 1;
                        if (prevQuarter === 0) {
                            // get quy 4 cua nam truoc 01/10/... -->  31/21/...
                            let prevYear = moment().year() - 1;
                            timeStart = prevYear + '-' + '10-01';
                            timeEnd = moment().year() + '-' + '01-01';
                        }
                        else {
                            timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                            timeEnd = moment().quarter(prevQuarter).endOf('quarter').add(1, 'days').format('YYYY-MM-DD');
                        }
                        break;
                    case 13:
                        // nam nay
                        self.TodayBC('Năm này');
                        timeStart = moment().startOf('year').format('YYYY-MM-DD');
                        timeEnd = moment().endOf('year').add(1, 'days').format('YYYY-MM-DD');
                        break;
                    case 14:
                        // nam truoc
                        self.TodayBC('Năm trước');
                        var prevYear = moment().year() - 1;
                        timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                        timeEnd = moment().year(prevYear).endOf('year').add(1, 'days').format('YYYY-MM-DD');
                        break;
                }
            }
        }

        let sTenChiNhanhs = '', arrDV = [];;
        for (let i = 0; i < self.MangChiNhanh().length; i++) {
            if ($.inArray(self.MangChiNhanh()[i], arrDV) === -1) {
                arrDV.push(self.MangChiNhanh()[i].ID);
                sTenChiNhanhs += self.MangChiNhanh()[i].TenDonVi + ',';
            }
        }
        sTenChiNhanhs = Remove_LastComma(sTenChiNhanhs);// use when export excel
        if (self.MangChiNhanh().length === 0) {
            arrDV = [_id_DonVi];
        }

        let arrTT = [];
        if (self.checkTamLuu()) {
            arrTT.push(1);
        }
        if (self.checkHoanThanh()) {
            arrTT.push(2);
        }
        if (self.checkHuy()) {
            arrTT.push(3);
        }

        let arrLoaiHD = self.MangChungTu().map(function (x) {
            return x.ID;
        })

        if (arrLoaiHD.length === 0) {
            arrLoaiHD = self.ChungTus().map(function (x) {
                return x.ID;
            })
        }

        var param = {
            MaHoaDon: txtSeach,
            ID_ChiNhanhs: arrDV,
            NgayTaoHD_TuNgay: timeStart,
            NgayTaoHD_DenNgay: timeEnd,
            TrangThaiHDs: arrTT,
            LaHoaDonSuaChua: arrLoaiHD, // loaihoadon
            CurrentPage: self.currentPage(),
            PageSize: self.pageSize(),
            TrangThai_SapXep: 0,
        }
        return param;
    }
    function getAllHoaDon() {
        $('.line-right').height(0).css("margin-top", "0px");
        LoadingForm(true);

        $('#table-reponsive').gridLoader();

        var param = GetParamSearch();
        ajaxHelper(BH_XuatHuyUri + 'getDmXuatHuy', 'POST', param).done(function (x) {
            console.log('x ', x)
            $('#table-reponsive').gridLoader({ show: false });
            if (x.res) {
                self.HoaDons(x.LstData);
                self._ThanhTien(x._thanhtien);
                self.RowsHangHoas(x.Rowcount);
                let totalPage = Math.ceil(x.Rowcount / self.pageSize())
                self.PageCount(totalPage);
                LoadingForm(false);
            }
            LoadHtmlGrid();
        })
    }

    self.ClickInconSearch = function () {
        getAllHoaDon();
    }

    self.XH_HangHoaChiTiet = ko.observableArray();
    self.XH_HangHoaChiTiet_Search = ko.observableArray();

    self.checkTamLuu.subscribe(function () {
        self.currentPage(0);
        getAllHoaDon();
    });
    self.checkHoanThanh.subscribe(function () {
        self.currentPage(0);
        getAllHoaDon();
    });
    self.checkHuy.subscribe(function () {
        self.currentPage(0);
        getAllHoaDon();
    });

    self.ResetCurrentPage = function () {
        self.currentPage(0);
        getAllHoaDon();
    };

    $('.newDateTime').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        var dt = new Date(picker.endDate.format('MM/DD/YYYY'));
        timeStart = picker.startDate.format('YYYY-MM-DD');
        timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
        self.TodayBC($(this).val());

        self.currentPage(0);
        getAllHoaDon();
    });

    $('.choseNgayTaoPhieuHuy li').on('click', function () {
        self.NgayTaoLH($(this).val());
        let txt = $(this).children('a').text();
        self.timeValue(txt);
        self.currentPage(0);
        getAllHoaDon();
    });

    //sort colum 
    $('#tb thead tr').on('click', 'th', function () {
        var id = $(this).attr('id');
        if (id === "txtMaHoaDon") {
            self.columsort("MaHoaDon");
            SortGrid(id);
        }
        if (id === "txtThoiGian") {
            self.columsort("ThoiGian");
            SortGrid(id);
        }
        if (id === "txtChiNhanh") {
            self.columsort("ChiNhanh");
            SortGrid(id);
        }
        if (id === "txtTongGiaTri") {
            self.columsort("TongTienHang");
            SortGrid(id);
        }
        if (id === "txtGhiChu") {
            self.columsort("GhiChu");
            SortGrid(id);
        }
    });

    function SortGrid(item) {
        $("#iconSort").remove();
        if (self.sort() === 0) {
            self.sort(1);
            $('#' + item).append(' ' + "<i id='iconSort' class='fa fa-caret-down' aria-hidden='true'></i>");
        }
        else {
            self.sort(0);
            $('#' + item).append(' ' + "<i id='iconSort' class='fa fa-caret-up' aria-hidden='true'></i>");
        }
        getAllHoaDon();
    };

    self.pageHangHoas = ko.computed(function () {
        var arrPage = [];
        var allPage = self.PageCount();
        var currentPage = self.currentPage();
        if (allPage > 0) {
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
                        for (let i = allPage - 5; i < allPage; i++) {
                            let obj = {
                                SoTrang: i + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                    else {
                        // get currentPage - 2 , currentPage, currentPage + 2
                        if (currentPage === 1) {
                            for (let j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                                let obj = {
                                    SoTrang: j + 1,
                                };
                                arrPage.push(obj);
                            }
                        } else {
                            for (let j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                                let obj = {
                                    SoTrang: j + 1,
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
                            let obj = {
                                SoTrang: i - 1,
                            };
                            arrPage.push(obj);
                            i = i + 1;
                        }
                    }
                    else {
                        while (arrPage.length < 5) {
                            let obj = {
                                SoTrang: i,
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
                    for (let i = 0; i < allPage; i++) {
                        let obj = {
                            SoTrang: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
            }
            self.RowsStart((self.currentPage() * self.pageSize()) + 1);
            if (((self.currentPage() + 1) * self.pageSize()) > self.HoaDons().length) {
                var RowsStart = (self.currentPage() + 1) * self.pageSize();
                if (RowsStart < self.RowsHangHoas()) {
                    self.RowsEnd((self.currentPage() + 1) * self.pageSize());
                }
                else {
                    self.RowsEnd(self.RowsHangHoas());
                }
            } else {
                self.RowsEnd((self.currentPage() * self.pageSize()) + self.pageSize());
            }
        }
        return arrPage;
    });

    self.VisibleStartPage = ko.computed(function () {
        if (self.pageHangHoas().length > 0) {
            return self.pageHangHoas()[0].SoTrang !== 1;
        }
    });
    self.VisibleEndPage = ko.computed(function () {
        if (self.pageHangHoas().length > 0) {
            return self.pageHangHoas()[self.pageHangHoas().length - 1].SoTrang !== self.PageCount();
        }
    });

    self.NextPage = function (item) {
        self.currentPage(0);
        getAllHoaDon();
    };
    self.BackPage = function (item) {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
            getAllHoaDon();
        }
    };
    self.EndPage = function (item) {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
            getAllHoaDon();
        }
    };
    self.StartPage = function (item) {
        self.currentPage(0);
        getAllHoaDon();
    };
    self.gotoNextPage = function (item) {
        self.currentPage(item.SoTrang - 1);
        getAllHoaDon();
    }

    $('#datetimepicker_mask').on('changedatetime.xdsoft', function (e) {
        timeNow = $(this).val();
    });

    self.NoteMaHD = function () {
        var keyCode = event.keyCode || event.which;
        if (keyCode === 13) {
            self.currentPage(0);
            getAllHoaDon();
        }
    }

    self.SelectIDNV = function (item) {
        self.selectIDNV(item.ID_NhanVien);
    }

    function getAllNSNhanVien() {
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetNS_NhanVien_InforBasic?idDonVi=" + _id_DonVi, 'GET').done(function (data) {
            self.NhanViens(data);
        });
    }

    function getAllNguoiDung() {
        ajaxHelper(BH_XuatHuyUri + "GetListNguoiDung", 'GET').done(function (data) {
            self.NguoiDungs(data);
        });
    }

    self.Enable_NgayLapHD = ko.observable(true);
    self.sum_SoLuongXuat = ko.observable();
    self.sum_GiaTriXuat = ko.observable();
    self.NgayLapHD_Update = ko.observable();

    function CheckNgayLapHD_format(valDate, idDonVi = null) {
        if (idDonVi === null) {
            idDonVi = VHeader.IdDonVi;
        }
        var dateNow = moment(new Date()).format('YYYY-MM-DD HH:mm');
        var ngayLapHD = moment(valDate, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
        if (valDate === '') {
            ShowMessage_Danger("Vui lòng nhập ngày lập phiếu xuất kho");
            return false;
        }
        if (valDate.indexOf('_') > -1) {
            ShowMessage_Danger("Ngày lập phiếu chưa đúng định dạng");
            return false;
        }
        if (ngayLapHD > dateNow) {
            ShowMessage_Danger("Ngày lập phiếu vượt quá thời gian hiện tại");
            return false;
        }
        let chotSo = VHeader.CheckKhoaSo(moment(ngayLapHD, 'YYYY-MM-DD HH:mm').format('YYYY-MM-DD'), idDonVi);
        if (chotSo) {
            ShowMessage_Danger(VHeader.warning.ChotSo.Update);
            return false;
        }
        return true;
    }

    self.isXuatKhoGDV = ko.observable(false);

    self.LoadHangHoaChiTiet = function (item) {
        var chotso = VHeader.CheckKhoaSo(moment(item.NgayLapHoaDon).format('YYYY-MM-DD'), item.ID_DonVi);
        self.Enable_NgayLapHD(!chotso);

        var $this = $(event.currentTarget);
        $this.next().find('.datetimepicker_maskedit').datetimepicker({
            timepicker: true,
            mask: true,
            format: 'd/m/Y H:i',
            maxDate: new Date(),
            onChangeDateTime: function (dp, $input) {
                CheckNgayLapHD_format($input.val(), item.ID_DonVi);
                self.NgayLapHD_Update($input.val());
            }
        });

        self.HDGoc_hasChange(false);

        var lc_HangHoaChiTiet = [];
        $.getJSON(BH_XuatHuyUri + "getList_HangHoaXuatHuybyID?ID_HoaDon=" + item.ID + "&ID_ChiNhanh=" + _id_DonVi).done(function (data) {
            let arr = data.LstDataPrint;
            let xkSuDung = $.grep(data.LstDataPrint, function (x) {
                return x.ChatLieu === '4';
            })
            self.isXuatKhoGDV(xkSuDung.length > 0)

            self.XH_HangHoaChiTiet(arr);
            self.XH_HangHoaChiTiet_Search(data.LstDataPrint);

            let sumSL = arr.reduce(function (_this, val) {
                return _this + val.SoLuong;
            }, 0);
            let sumGT = arr.reduce(function (_this, val) {
                return _this + val.GiaTriHuy;
            }, 0);
            self.sum_SoLuongXuat(RoundDecimal(sumSL, 3));
            self.sum_GiaTriXuat(RoundDecimal(sumGT, 3));
            lc_HangHoaChiTiet = arr;
            localStorage.setItem('lc_HangHoaChiTiet', JSON.stringify(lc_HangHoaChiTiet));
            SetHeightShowDetail($this);

            let trangThaiPhieu = 0;
            if (data.LstDataPrint.length > 0) {
                trangThaiPhieu = parseInt(data.LstDataPrint[0].TrangThaiMoPhieu);
            }
            self.HDGoc_hasChange(trangThaiPhieu === 1 || trangThaiPhieu === 2);
        })
    }

    async function LoadChiTietHD(idHoaDon) {
        let xx = await $.getJSON(BH_XuatHuyUri + "getList_HangHoaXuatHuybyID?ID_HoaDon=" + idHoaDon + "&ID_ChiNhanh=" + _id_DonVi).done(function (data) { })
            .then(function (data) {
                return data.LstDataPrint
            })
        return xx;
    }

    function SetDataToCache_gotoDetail(hd, loai) {
        if (self.XH_HangHoaChiTiet_Search().length > 0) {

            var cthd = [];
            var arrIDQuiDoi = [];

            for (let i = 0; i < self.XH_HangHoaChiTiet_Search().length; i++) {
                let itFor = self.XH_HangHoaChiTiet_Search()[i];
                let ctNew = $.extend({}, itFor);
                let quanlytheolo = itFor.QuanLyTheoLoHang;

                let ngaysx = moment(itFor.NgaySanXuat).format('DD/MM/YYYY');
                let hethan = moment(itFor.NgayHetHan).format('DD/MM/YYYY');
                if (ngaysx === 'Invalid date') {
                    ngaysx = '';
                }
                if (hethan === 'Invalid date') {
                    hethan = '';
                }
                ctNew.NgaySanXuat = ngaysx;
                ctNew.NgayHetHan = hethan;
                ctNew.DM_LoHang = [];
                if (commonStatisJs.CheckNull(ctNew.DonViTinh)) {
                    ctNew.DonViTinh = [];
                }
                ctNew.HangCungLoais = [];
                ctNew.LotParent = quanlytheolo;
                ctNew.ThanhTien = itFor.GiaTriHuy;
                ctNew.MaLoHang = itFor.TenLoHang;
                ctNew.SoLuongConLai = 0;

                // get from hd
                ctNew.ID_DonVi = hd.ID_DonVi;
                ctNew.TongTienHang = hd.TongTienHang;
                ctNew.ID_PhieuTiepNhan = hd.ID_PhieuTiepNhan;
                ctNew.ID_HoaDon = loai === 1 ? '00000000-0000-0000-0000-000000000000' : hd.ID;
                ctNew.ID_HoaDonSC = hd.ID_HoaDon;
                ctNew.MaHoaDonSuaChua = hd.MaHoaDonGoc;
                ctNew.MaPhieuTiepNhan = hd.MaPhieuTiepNhan;
                ctNew.BienSo = hd.BienSo;
                ctNew.DienGiai = hd.DienGiai;
                ctNew.CssWarning = false;

                ctNew.ThanhPhan_DinhLuong = [];
                ctNew.HasTPDinhLuong = false;
                if (itFor.ThanhPhan_DinhLuong !== null && itFor.ThanhPhan_DinhLuong.length > 0) {
                    ctNew.ThanhPhan_DinhLuong = itFor.ThanhPhan_DinhLuong;
                    ctNew.HasTPDinhLuong = true;
                }

                if ($.inArray(ctNew.ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                    arrIDQuiDoi.unshift(ctNew.ID_DonViQuiDoi);
                    // push CTHD
                    ctNew.SoThuTu = cthd.length + 1;
                    ctNew.ID_Random = CreateIDRandom('RandomCT_');
                    if (quanlytheolo) {
                        let objLot = $.extend({}, ctNew);
                        objLot.HangCungLoais = [];
                        objLot.DM_LoHang = [];
                        objLot.ThanhPhan_DinhLuong = [];
                        ctNew.DM_LoHang.push(objLot);
                    }
                    cthd.unshift(ctNew);
                }
                else {
                    // find in cthd with same ID_QuiDoi
                    for (let j = 0; j < cthd.length; j++) {
                        if (cthd[j].ID_DonViQuiDoi === ctNew.ID_DonViQuiDoi) {
                            if (quanlytheolo) {
                                let objLot = $.extend({}, ctNew);
                                objLot.HangCungLoais = [];
                                objLot.DM_LoHang = [];
                                objLot.LotParent = false;
                                objLot.ThanhPhan_DinhLuong = [];
                                objLot.ID_Random = CreateIDRandom('RandomCT_');
                                cthd[j].DM_LoHang.push(objLot);
                            }
                            else {
                                cthd[j].SoLuong = cthd[j].SoLuong + ctNew.SoLuong;
                                cthd[j].ThanhTien = cthd[j].SoLuong * cthd[j].GiaVon;
                            }
                            break;
                        }
                    }
                }
            }
            localStorage.setItem('lcXK_EditOpen', JSON.stringify(cthd));
            localStorage.setItem('XK_createfrom', loai);
            GoToThemMoiXuatKho();
        }
    }
    function GoToThemMoiXuatKho() {
        window.open('/#/XuatKhoChitiet');
    }
    self.ClickSaoChep = function (item) {
        SetDataToCache_gotoDetail(item, 1);
    }

    self.ClickMoPhieuHuy = function (item) {
        SetDataToCache_gotoDetail(item, 2);
    }

    self.UpdateAgain_PhieuXK = function (item) {
        SetDataToCache_gotoDetail(item, 4);
    }

    // chỉnh sửa phiếu xuất hủy
    self.editHD = function (item) {
        var maHoaDon = item.MaHoaDon;
        var idHoaDon = item.ID;
        var ngaylapHDOld = item.NgayLapHoaDon;
        if (self.NgayLapHD_Update() === undefined) {
            self.NgayLapHD_Update(moment(ngaylapHDOld).format('DD/MM/YYYY HH:mm'));
        }
        var check = CheckNgayLapHD_format(self.NgayLapHD_Update(), item.ID_DonVi);
        if (!check) {
            return;
        }
        var ngaylapHDNew = moment(self.NgayLapHD_Update(), 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm:ss');
        ngaylapHDOld = moment(ngaylapHDOld).format('YYYY-MM-DD HH:mm:ss');

        var HoaDon = {
            ID: idHoaDon,
            MaHoaDon: maHoaDon,
            ID_NhanVien: self.selectIDNV(),
            DienGiai: item.DienGiai,
            NguoiSua: _userLogin,
            NgayLapHoaDon: ngaylapHDNew,
        };

        var myData = {
            id: idHoaDon,
            objNewHoaDon: HoaDon,
        };

        ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + "PutBH_HoaDon2", 'POST', myData).done(function (x) {
            if (x.res) {
                ShowMessage_Success("Cập nhật phiếu xuất kho thàng công");

                let diary = {
                    ID_NhanVien: _id_NhanVien,
                    ID_DonVi: _id_DonVi,
                    ChucNang: 'Danh sách phiếu xuất kho',
                    NoiDung: "Cập nhật phiếu xuất kho ".concat(maHoaDon, ', Người sửa: ', _userLogin),
                    NoiDungChiTiet: "Cập nhật phiếu xuất kho ".concat(maHoaDon,
                        '<br /> - Ngày lập hóa đơn cũ:, ', ngaylapHDOld,
                        '<br /> - Ngày lập hóa đơn mới:, ', ngaylapHDNew),
                    LoaiNhatKy: 3,
                    LoaiHoaDon: 8,
                };
                if (item.ChoThanhToan === false) {
                    diary.ID_HoaDon = idHoaDon;
                    diary.ThoiGianUpdateGV = ngaylapHDOld;
                    Post_NhatKySuDung_UpdateGiaVon(diary);
                }
                else {
                    Insert_NhatKyThaoTac_1Param(diary);
                }
            }
            else {
                ShowMessage_Danger('Cập nhật phiếu xuất kho thất bại');
            }
        }).fail(function () {
            ShowMessage_Danger('Cập nhật phiếu xuất kho thất bại');
        })
    };

    self.xoaHD = function (item) {
        var idHoaDon = item.ID;
        var maHoaDon = item.MaHoaDon;
        console.log('ite ', item)
        dialogConfirm('Thông báo xóa', ' Bạn có chắc chắn muốn hủy bỏ phiếu xuất kho <b>' + maHoaDon + ' </b>  không?', function () {
            ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + "Huy_HoaDon?id=" + idHoaDon + '&nguoiSua=' + _userLogin
                + '&iddonvi=' + _id_DonVi).done(function (mes) {
                    if (mes === '') {
                        ShowMessage_Success("Hủy phiếu xuất kho thành công");

                        let diary = {
                            ID_NhanVien: _id_NhanVien,
                            ID_DonVi: _id_DonVi,
                            ChucNang: 'Danh sách phiếu xuất kho',
                            NoiDung: "Xóa phiếu xuất kho " + maHoaDon,
                            NoiDungChiTiet: "Xóa phiếu xuất kho ".concat(maHoaDon, ', Người xóa: ', _userLogin),
                            LoaiNhatKy: 3,
                            LoaiHoaDon: 8,
                        };
                        if (item.ChoThanhToan === false) {
                            diary.ID_HoaDon = idHoaDon;
                            diary.ThoiGianUpdateGV = item.NgayLapHoaDon;
                            Post_NhatKySuDung_UpdateGiaVon(diary);
                        }
                        else {
                            Insert_NhatKyThaoTac_1Param(diary);
                        }
                    }
                    else {
                        ShowMessage_Danger('Hủy phiếu xuất kho thất bại');
                    }
                    getAllHoaDon();
                })
        })
    };

    // xuất excel 
    self.ExportExcel = function () {
        if (self.HoaDons().length === 0) {
            ShowMessage_Danger('Không có dữ liệu để xuất file excel');
            return;
        }
        var columnHide = '';
        var columns = localStorage.getItem(Key_Form);
        if (columns !== null) {
            columns = JSON.parse(columns);
            for (let i = 0; i < self.ListCheckBox().length; i++) {
                for (let j = 0; j < columns.length; j++) {
                    if (columns[j].Value === self.ListCheckBox()[i].Key) {
                        columnHide += i + '_';
                        break;
                    }
                }
            }
        }

        var param = GetParamSearch();
        param.CurrentPage = 0;
        param.PageSize = self.RowsHangHoas();
        param.ColumnsHide = columnHide;
        console.log('columnHide ', columnHide)

        ajaxHelper(BH_XuatHuyUri + 'ExportExelXH', 'POST', param).done(function (x) {
            if (x.res) {
                self.DownloadFileTeamplateXLSX(x.url);
                let diary = {
                    ID_NhanVien: _id_NhanVien,
                    ID_DonVi: _id_DonVi,
                    ChucNang: "Xuất kho",
                    NoiDung: "Xuất danh sách phiếu xuất kho",
                    NoiDungChiTiet: "Xuất danh sách phiếu xuất kho",
                    LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                };
                Insert_NhatKyThaoTac_1Param(diary);
            }
        })
    };

    self.ExportExcel_ChiTiet = function (item) {
        var url = BH_XuatHuyUri + "ExportExcelXH_ChiTiet?ID_HoaDon=" + item.ID + "&ColumnsHide=null&time="
            + self.TodayBC() + "&ChiNhanh=" + _id_DonVi;
        window.location.href = url;

        var diary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Xuất kho",
            NoiDung: "Xuất danh sách chi tiết hàng hóa theo phiếu xuất kho: " + item.MaHoaDon,
            NoiDungChiTiet: "Xuất danh sách chi tiết hàng hóa theo phiếu xuất kho: " + item.MaHoaDon,
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        Insert_NhatKyThaoTac_1Param(diary);
    }

    self.XH_HangHoaChiTiet_Print = ko.observableArray();
    self.InHoaDon = function (item) {
        self.XH_HangHoaChiTiet_Print($.extend(true, [], self.XH_HangHoaChiTiet_Search()));
        var cthdFormat = GetCTHDPrint_Format(self.XH_HangHoaChiTiet_Search());
        self.CTHoaDonPrint(cthdFormat);
        var itemHDFormat = GetInforHDPrint(item);
        self.InforHDprintf(itemHDFormat);
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=' + TeamplateXuatHuy + '&idDonVi=' + _id_DonVi,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                var data1 = '<script src="/Scripts/knockout-3.4.2.js"></script>';
                data1 = data1.concat("<script > var item1=" + JSON.stringify(self.CTHoaDonPrint())
                    + "; var item4=[], item5=[]; var item2=" + JSON.stringify(self.CTHoaDonPrint())
                    + " ;var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
                data1 = data1.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(result, data1, self.numberPG());
            }
        });
    }
    self.ListTypeMauIn = ko.observableArray();
    function loadMauIn() {
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetListMauIn?typeChungTu=' + TeamplateXuatHuy + '&idDonVi=' + _id_DonVi,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                self.ListTypeMauIn(result);
            }
        });
    }
    self.PrinXuatHuy = function (item, key) {
        self.XH_HangHoaChiTiet_Print($.extend(true, [], self.XH_HangHoaChiTiet_Search()));
        var cthdFormat = GetCTHDPrint_Format(self.XH_HangHoaChiTiet_Search());
        self.CTHoaDonPrint(cthdFormat);

        var itemHDFormat = GetInforHDPrint(item);
        self.InforHDprintf(itemHDFormat);
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrint?idMauIn=' + key,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                var data = result;
                data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                data = data.concat("<script > var item1=" + JSON.stringify(self.CTHoaDonPrint())
                    + "; var item2=[], item4=[], item5=[];var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(data);
            }
        });
    };
    function GetInforHDPrint(objHD) {
        var objPrint = $.extend({}, true, objHD)
        objPrint.MaHoaDon = objPrint.MaHoaDon;
        objPrint.NhanVienBanHang = objPrint.TenNhanVien;
        objPrint.NgayLapHoaDon = moment(objPrint.NgayLapHoaDon, 'YYYY-MM-DD HH:mm:ss').format('DD/MM/YYYY HH:mm:ss');

        objPrint.TongSoLuongHang = formatNumber3Digit(self.sum_SoLuongXuat());
        objPrint.TongTienHang = formatNumber3Digit(objPrint.TongTienHang, 2);
        objPrint.TenChiNhanh = objPrint.TenChiNhanh;
        objPrint.NguoiTao = objPrint.NguoiTaoHD;
        objPrint.GhiChu = objPrint.DienGiai;
        var cn = self.DonVis().find(x => x.ID == objPrint.ID_DonVi);
        objPrint.DiaChiChiNhanh = cn.DiaChi;
        objPrint.DienThoaiChiNhanh = cn.SoDienThoai;

        // logo cong ty
        if (self.CongTy().length > 0) {
            objPrint.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
            objPrint.TenCuaHang = self.CongTy()[0].TenCongTy;
            objPrint.DiaChiCuaHang = self.CongTy()[0].DiaChi;
            objPrint.DienThoaiCuaHang = self.CongTy()[0].SoDienThoai;
        }

        return objPrint;
    }

    function GetCTHDPrint_Format(arrCTHD) {
        var arr = [];
        arrCTHD = arrCTHD.sort(function (a, b) {
            let x = a.SoThuTu, y = b.SoThuTu;
            return x > y ? 1 : x < y ? -1 : 0;
        });
        for (var i = 0; i < arrCTHD.length; i++) {
            let itFor = $.extend({}, arrCTHD[i]);
            itFor.GiaVon = formatNumber3Digit(itFor.GiaVon, 0);
            itFor.SoLuongHuy = formatNumber3Digit(itFor.SoLuong);
            itFor.GiaTriHuy = formatNumber3Digit(itFor.GiaTriHuy, 2);
            itFor.MaLoHang = itFor.TenLoHang;
            arr.push(itFor);
        }
        return arr;
    }

    function GetInforCongTy() {
        ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CongTy', 'GET').done(function (data) {
            if (data != null) {
                self.CongTy(data);
            }
        });
    }

    self.LoadLoHang_byMaLH = function (item) {
        localStorage.setItem('FindLoHang', item.TenLoHang);
        var url = "/#/Shipment";
        window.open(url);
    };

    shortcut.add('F10', function (e) {
        if ($('.bgwhite').css('display') === "none") {
            if (loaiHoaDon === 4) {
                if (window.location.hash !== "#/PurchaseOrder") {
                    $('.bgwhite').show();
                    modelGiaoDich.addHoaDon();
                }
            }
            if (loaiHoaDon === 7) {
                if (window.location.hash !== "#/PurchaseReturns") {
                    $('.bgwhite').show();
                    modelGiaoDich.addHoaDonTH();
                }
            }
            if (loaiHoaDon === "10") {
                if (window.location.hash !== "#/Transfers") {
                    $('.bgwhite').show();
                    modelChuyenHang.addHoaDon();
                }
            }
            if (loaiHoaDon === "9") {
                if (window.location.hash !== "#/StockTakes") {
                    $('.bgwhite').show();
                    modelHangHoa.addKiemKho('StockTakes');
                }
            }
            if (loaiHoaDon === 8) {
                if (window.location.hash !== "#/DamageItems") {
                    $('.bgwhite').show();
                    self.addHoaDon_HoanThanh();
                }
            }
        }
        e.stopImmediatePropagation();
    });

    self.DownloadFileTeamplateXLSX = function (pathFile) {
        var url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + pathFile;
        window.location.href = url;
    }

    var timeCT = null;
    self.NoteHangHoa_ChiTiet = function (hd) {
        clearTimeout(timeCT);
        var value = $(event.target).val();
        timeCT = setTimeout(function () {
            var arrHangHoaChiTiet = [];
            var itemSearch = locdau(value.toLowerCase());
            for (let i = 0; i < self.XH_HangHoaChiTiet_Search().length; i++) {
                let locdauMHH = locdau(self.XH_HangHoaChiTiet_Search()[i].MaHangHoa).toLowerCase();
                let locdauTenHH = locdau(self.XH_HangHoaChiTiet_Search()[i].TenHangHoaFull).toLowerCase();
                let MHH = locdauMHH.split(itemSearch);
                let THH = locdauTenHH.split(itemSearch);
                if (MHH.length > 1 || THH.length > 1) {
                    arrHangHoaChiTiet.push(self.XH_HangHoaChiTiet_Search()[i]);
                }
            }
            if (value == "") {
                arrHangHoaChiTiet = self.XH_HangHoaChiTiet_Search();
            }

            if (hd.LoaiHoaDon === 1) {// sudung gdv
                arrHangHoaChiTiet = arrHangHoaChiTiet.filter(x => x.ChatLieu === '4')
            }
            else {
                arrHangHoaChiTiet = arrHangHoaChiTiet.filter(x => x.ChatLieu !== '4')
            }
            self.XH_HangHoaChiTiet(arrHangHoaChiTiet);

        }, 500);
    }

    function PageLoad() {
        loadQuyenIndex();
        LoadColumnCheck();
        getDonVi();
        getQuyen_NguoiDung();
        GetInforCongTy();
        getAllNSNhanVien();
        getAllNguoiDung();
        loadMauIn();
    }

    PageLoad();

    self.XemThanhPhanDinhLuong = function (item) {

    }

    self.HDGoc_hasChange = ko.observable(false);
    self.XacNhanXuat = async function (item) {
        self.HDGoc_hasChange(false);

        var dataX = await LoadChiTietHD(item.ID);
        if (dataX.length === 0) {
            ShowMessage_Danger('Chi tiết hóa đơn rỗng');
            return;
        }

        let mes = '';
        switch (parseInt(dataX[0].TrangThaiMoPhieu)) {
            case 1:
                self.HDGoc_hasChange(true);
                mes = 'Phiếu xuất đã bị hủy do cập nhật lại nguyên vật liệu';
                break;
            case 2:
                self.HDGoc_hasChange(true);
                mes = 'Hóa đơn gốc đã được hủy. Không thể xuất kho';
                break;
        };
        if (mes != '') {
            ShowMessage_Danger(mes);
            return;
        }

        // check het tonkho (tai thoidiem hientai)
        for (let i = 0; i < dataX.length; i++) {
            let itFor = dataX[i];
            let tk = RoundDecimal(itFor.TonKho);
            let sl = RoundDecimal(itFor.SoLuong);
            if (sl > tk) {
                mes += itFor.MaHangHoa + ' ,';
            }
        }
        if (!commonStatisJs.CheckNull(mes)) {
            ShowMessage_Danger('Không đủ tồn kho cho sản phẩm: ' + Remove_LastComma(mes));
            return;
        }

        $.getJSON(BH_XuatHuyUri + 'PhieuXuatKho_XacNhanXuat?idHoaDon=' + item.ID).done(function (x) {
            if (x.res) {
                let diary = {
                    ID_DonVi: item.ID_DonVi,
                    ID_NhanVien: _id_NhanVien,
                    LoaiNhatKy: 1,
                    ID_HoaDon: item.ID,
                    LoaiHoaDon: item.LoaiHoaDon,
                    ThoiGianUpdateGV: x.dataSoure, // get date at server
                    ChucNang: 'Xác nhận xuất kho',
                    NoiDung: 'Xác nhận xuất kho, Mã phiếu xuất '.concat(item.MaHoaDon),
                    NoiDungChiTiet: 'Thông tin chi tiết '.concat('<br /> Mã phiếu xuất: ', item.MaHoaDon,
                        '<br /> Ngày xác nhận: ', moment(x.dataSoure).format('DD/MM/YYYY HH:mm'),
                        '<br /> User xác nhận: ', _userLogin,
                        '<br /> Ghi chú: ', item.DienGiai
                    ),
                }
                Post_NhatKySuDung_UpdateGiaVon(diary);

                self.currentPage(0);
                getAllHoaDon();
            }
        })

        self.XH_HangHoaChiTiet_Search(dataX)
        self.InHoaDon(item);
    }

    self.gotoPageOther = function (item, type) {
        switch (type) {
            case 1:
                var url = '';
                if (!commonStatisJs.CheckNull(item.MaHoaDonGoc)) {
                    localStorage.setItem('FindHD', item.MaHoaDonGoc);
                    switch (item.LoaiHoaDon) {
                        case 40:// xuat hotro chung
                        case 38:// xuat banle
                        case 37: //Xuất hỗ trợ ngày thuốc
                        case 35:// xuat NVL
                            url = '/#/Invoices';
                            break;
                        case 39:
                            url = '/#/HoaDonBaoHanh';
                            break;

                    }
                    if (url !== '') {
                        window.open(url, '_blank');
                    }
                }
                break;
            case 2:
                if (!commonStatisJs.CheckNull(item.MaPhieuTiepNhan)) {
                    window.open('/#/DanhSachPhieuTiepNhan?' + item.MaPhieuTiepNhan, '_blank');
                }
                else {
                    self.LoadHangHoaChiTiet(item);
                }
                break;
            case 3:
                if (!commonStatisJs.CheckNull(item.BienSo)) {
                    window.open('/#/DanhSachXe?' + item.BienSo, '_blank');
                }
                else {
                    self.LoadHangHoaChiTiet(item);
                }
                break;
        }
    }
}
var vmXuatHuy = new ViewModel();
ko.applyBindings(vmXuatHuy);



