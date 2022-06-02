var FileModel = function (filef, srcf) {
    var self = this;
    this.file = filef;
    this.URLAnh = srcf;
};

var FormModel_NewUserContact = function () {
    var self = this;
    self.ID = ko.observable();
    self.ID_DoiTuong = ko.observable();
    self.MaLienHe = ko.observable();
    self.TenLienHe = ko.observable();
    self.SoDienThoai = ko.observable();
    self.DienThoaiCoDinh = ko.observable();
    self.Email = ko.observable();
    self.NgaySinh = ko.observable();
    self.GhiChu = ko.observable();
    self.NguoiTao = ko.observable();
    self.NguoiSua = ko.observable();
    self.ID_QuanHuyen = ko.observable();
    self.ID_TinhThanh = ko.observable();
    self.DiaChi = ko.observable();
    self.ChucVu = ko.observable();
    self.XungHo = ko.observable();

    $('#txtCustomer_modal').text('--Chọn khách hàng--');
    $('#txtProvince_modal').text('--Chọn tỉnh thành--');
    $('#txtDistrict_modal').text('--Chọn quận huyện--');
    //$('#showseach_DoiTuong ul li span i').remove();
    $('#showseach_DoiTuong span[id^="spanCheckDoiTuong_"]').remove();

    self.SetData = function (item) {
        self.ID(item.ID);
        self.MaLienHe(item.MaLienHe);
        self.TenLienHe(item.TenLienHe);
        self.SoDienThoai(item.SoDienThoai);
        self.DienThoaiCoDinh(item.DienThoaiCoDinh);
        self.Email(item.Email);
        self.GhiChu(item.GhiChu);
        self.DiaChi(item.DiaChi);
        self.ChucVu(item.ChucVu);
        self.XungHo(item.XungHo);

        var ngaysinh = item.NgaySinh;
        if (ngaysinh === null || ngaysinh === undefined) {
            ngaysinh = "";
        }
        else {
            ngaysinh = moment(ngaysinh, "YYYY-MM-DD hh:mm:ss").format("DD/MM/YYYY");
        }
        self.NgaySinh(ngaysinh);

        if (item.ID_DoiTuong !== null && item.ID_DoiTuong.indexOf('0000') === -1) {
            self.ID_DoiTuong(item.ID_DoiTuong);
        }
        else {
            self.ID_DoiTuong(undefined);
        }

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
    }
};

var ViewModal = function () {
    var self = this;
    self.listProvince = ko.observableArray();
    self.searchTT = ko.observable();
    self.listProvinces = ko.computed(function () {
        if (self.searchTT() === null || self.searchTT() === undefined || self.searchTT() === '') {
            return self.listProvince();
        }
        else {
            return self.listProvince().filter(function (item) {
                return SearchTxt_inVue(self.searchTT().split(" "), item['TenTinhThanh']) === true;
            });
        }
    });
    self.searchDistrict = ko.observable(); // search in popup
    self.listDistrict = ko.observableArray();// at list search
    self.District_byProvice = ko.observableArray();// add popup
    self.listUserContact = ko.observableArray();
    self.listCheckbox = ko.observableArray();
    self.booleanAdd = ko.observable();
    self.DoiTuongs = ko.observableArray();
    self.ListIDNhanVienQuyen = ko.observableArray();

    self.FileImgs = ko.observableArray();
    self.FilesSelect = ko.observableArray();
    self.HaveImage = ko.observable(false);
    self.HaveImage_Select = ko.observable(false);
    self.AnhDaiDien = ko.observableArray();
    self.ImageIsZoom = ko.observableArray();
    self.IsCustomer = ko.observable(true);

    self.birthday = ko.observable();
    self.birthday_Quy = ko.observable();
    self.birthday_Input = ko.observable();
    self.filterNgayTao = ko.observable("1");
    self.filterNgayTao_Quy = ko.observable("0");
    self.filterNgayTao_Input = ko.observable();
    self.selectProvince = ko.observable();
    self.selectDistrict = ko.observable();
    self.selectCustomer = ko.observable();
    self.selectedXungHo = ko.observable();

    self.filter = ko.observable();
    self.filterProvince = ko.observable("0");
    self.filterDistrict = ko.observable("0");
    self.filterCreateDate = ko.observable("0");
    self.filterCreateDate_Quy = ko.observable("0");
    self.filterCreateDate_Input = ko.observable();

    self.pageSizes = [10,20, 30,40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();
    self.TotalRecord = ko.observable();
    self.PageCount = ko.observable();

    // Check Quyen
    self.RoleView = ko.observable(false);
    self.RoleInsert = ko.observable(false);
    self.RoleUpdate = ko.observable(false);
    self.RoleDelete = ko.observable(false);
    self.RoleExport = ko.observable(false);
    self.newUserContact = ko.observable(new FormModel_NewUserContact());

    var user = $('#txtTenTaiKhoan').text(); // get at header
    var idNhanVien = $('#txtIDNhanVien').val();
    var userID = $('#txtUserID').val();
    var idDonVi = $('#hd_IDdDonVi').val();
    var DMLienHeUri = '/api/DanhMuc/DM_LienHeAPI/';
    var DMDoiTuongUri = '/api/DanhMuc/DM_DoiTuongAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    var sLoai = 'khách hàng';

    // const search in modal add new
    const txtProvince_modal = '#txtProvince_modal';
    const txtDistrict_modal = '#txtDistrict_modal';
    const txtCustomer_modal = '#txtCustomer_modal';

    self.ListXungHo = ko.observableArray([
        { ID: 0, Text: '- Xưng hô -' },
        { ID: 1, Text: 'Anh' },
        { ID: 2, Text: 'Chị' },
        { ID: 3, Text: 'Cô' },
        { ID: 4, Text: 'Chú' },
    ])

    function LoadPage() {
        loadCheckbox();
        getListProvince();
        GetAll_District();
        GetListIDNhanVien_byUserLogin();
    }
    LoadPage();

    var Key_Form = 'Key_UserContact';
    function loadCheckbox() {
        $.getJSON("api/DanhMuc/BaseApi/GetCheckedStatic?type=" + $('#ID_loaibaocao').val(), function (data) {
            self.listCheckbox(data);
            loadHtmlGrid();
        });
    }
    function loadHtmlGrid() {
        LocalCaches.LoadFirstColumnGrid(Key_Form, $('#select-column .dropdown-list ul li input[type = checkbox]'), self.listCheckbox());
    }
    $('#select-column').on('change', '.dropdown-list ul li input[type = checkbox]', function () {
        var valueCheck = $(this).val();
        LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, valueCheck);
        $('.' + valueCheck).toggle();
    });
    $('#select-column').on('click', '.dropdown-list ul li', function (i) {
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
    function getListProvince() {
        ajaxHelper(DMDoiTuongUri + "GetListTinhThanh", 'GET').done(function (x) {
            if (x.res === true) {
                self.listProvince(x.data);
            }
        });
    }

    function GetListIDNhanVien_byUserLogin() {
        ajaxHelper('/api/DanhMuc/ChamSocKhachHangAPI/' + 'GetListNhanVienLienQuanByIDLoGin_inDepartment?idnvlogin=' + idNhanVien
            + '&idChiNhanh=' + idDonVi + '&funcName=' + funcName, 'GET').done(function (data) {
                self.ListIDNhanVienQuyen(data);

                GetHT_Quyen_ByNguoiDung();
            })
    }

    function getListDistrict(id) {
        if (id !== undefined) {
            var arr = $.grep(self.listDistrict(), function (x) {
                return x.ID_TinhThanh === id;
            })
            self.District_byProvice(arr);
        }
    }

    function GetAll_District() {
        ajaxHelper(DMDoiTuongUri + "GetAllQuanHuyen", 'GET').done(function (x) {
            if (x.res === true) {
                self.listDistrict(x.data);
            }
        });
    }

    self.filterDoiTuong = ko.observable();
    function getAllDMDoiTuong(loaiDoiTuong) {
        var _now = new Date();
        var dayEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
        var dayStart = '2016-01-01';

        var arrIDManager = [];
        for (var i = 0; i < self.ListIDNhanVienQuyen().length; i++) {
            if ($.inArray(self.ListIDNhanVienQuyen()[i], arrIDManager) === -1) {
                arrIDManager.push(self.ListIDNhanVienQuyen()[i]);
            }
        }

        var Params_GetListKhachHang = {
            ID_DonVis: [idDonVi],
            LoaiDoiTuong: loaiDoiTuong,
            MaDoiTuong: '',
            ID_NhomDoiTuong: null,
            NgayTao_TuNgay: dayStart,
            NgayTao_DenNgay: dayEnd,
            TongBan_TuNgay: dayStart,
            TongBan_DenNgay: dayEnd,
            TongBan_Tu: 0,
            TongBan_Den: 0,
            NoHienTai_Tu: null,
            NoHienTai_Den: null,
            No_TrangThai: 0,
            GioiTinh: null,
            LoaiKhach: null, // ca nhan, cong ty
            ID_TinhThanhs: null,
            NgaySinh_TuNgay: null,
            NgaySinh_DenNgay: null,
            LoaiNgaySinh: null, // 0.Ngay/Thang, 1.Nam
            ID_NguonKhach: null,
            ID_NhanVienQuanLys: arrIDManager,
            TrangThai_SapXep: 0,  // 0.No sort, 1.Sort ASC, 2.DESC
            Cot_SapXep: '',
            NguoiTao: user,
        }

        console.log(Params_GetListKhachHang);

        ajaxHelper(DMDoiTuongUri + "GetListKhachHang_Where_PassObject?", 'POST', Params_GetListKhachHang).done(function (data) {
            if (data !== null) {
                self.DoiTuongs(data);
                var itemDT = $.grep(data, function (x) {
                    return x.ID === self.newUserContact().ID_DoiTuong();
                });
                if (itemDT.length > 0) {
                    self.ChoseCustomer_Modal(itemDT[0]);
                }
            }
        });
    }

    self.FilterDoiTuong = ko.computed(function () {
        var filter = self.filterDoiTuong();
        if (filter === null || filter === undefined || filter === '') {
            return self.DoiTuongs();
        }
        else {
            return self.DoiTuongs().filter(function (item) {
                return SearchTxt_inVue(filter.split(" "), item['TenDoiTuong']) === true;
            });
        }
    });

    function LoadSearchKhachHang() {
        var index = -1;
        var model_KH = new Vue({
            el: '#divSearchKH',
            data: function () {

                return {
                    query_Kh: '',
                    data_kh: self.DoiTuongs()
                }
            },
            methods: {
                reset: function (item) {
                    this.data_kh = item;
                    this.query_Kh = '';
                },
                click: function (item) {
                    self.ChoseCustomer_Modal(item);
                    $('#showseach_Kh').hide();
                },
                submit: function (event) {
                    if (event.keyCode === 13) {
                        var result = this.fillter_KH(this.query_Kh);
                        var focus = false;
                        $('#showseach_Kh ul li').each(function (i) {
                            if ($(this).hasClass('hoverenabled')) {
                                $('#showseach_Kh').hide();
                                focus = true;
                                self.ChoseCustomer_Modal(result[i]);
                            }
                        });
                        if (result.length > 0 && this.query_Kh !== '' && focus === false) {
                            $('#showseach_Kh').hide();
                        }
                    }
                    else if (event.keyCode === 40)//dows
                    {
                        index = index + 1;
                        if (index >= ($("#showseach_Kh ul li").length)) {
                            index = 0;
                            $('#showseach_Kh').stop().animate({
                                scrollTop: $('#showseach_Kh').offset().top - 600
                            }, 1000);
                        }
                        else if (index > 9 && index < $("#showseach_Kh ul li").length) {
                            $('#showseach_Kh').stop().animate({
                                scrollTop: $('#showseach_Kh').offset().top + 500
                            }, 1000);
                        }
                        this.loadFocus();

                    }
                    else if (event.keyCode === 38)//up
                    {
                        index = index - 1;
                        if (index < 0) {
                            index = $("#showseach_Kh ul li").length - 1;
                            $('#showseach_Kh').stop().animate({
                                scrollTop: $('#showseach_Kh').offset().top + 500
                            }, 1000);
                        }
                        else if (index > 0 && index < 10) {
                            $('#showseach_Kh').stop().animate({
                                scrollTop: $('#showseach_Kh').offset().top - 600
                            }, 1000);
                        }
                        this.loadFocus();

                    }
                },
                loadFocus: function () {
                    $('#showseach_Kh ul li').each(function (i) {
                        $(this).removeClass('hoverenabled');
                        if (index === i) {
                            $(this).addClass('hoverenabled');
                        }
                    });
                },
                // Tìm kiếm khách hàg
                fillter_KH: function (value) {
                    if (value === '') return this.data_kh.slice(0, 50);
                    return this.data_kh.filter(function (item) {
                        return SearchTxt_inVue(value.split(" "), item['Name_Phone']) === true;
                    }).slice(0, 50);
                },

            },
            computed: {
                // Return Khách hàng
                SearchKhachHang: function () {

                    var result = this.fillter_KH(this.query_Kh);
                    if (result.length < 1 || this.query_Kh === '') {
                        $('#showseach_Kh').hide();
                    }
                    else {
                        index = 0;
                        $('#showseach_Kh').show();
                    }
                    $('#showseach_Kh ul li').each(function (i) {
                        if (i === 0) {
                            $(this).addClass('hoverenabled');
                        }
                        else {
                            $(this).removeClass('hoverenabled');
                        }
                    });
                    $('#showseach_Kh').stop().animate({
                        scrollTop: $('#showseach_Kh').offset().top - 600
                    }, 1000);
                    return result;
                }

            }
        });
    }

    self.FilterDistrict_byProvice = ko.computed(function () {
        if (self.searchDistrict() === null || self.searchDistrict() === undefined || self.searchDistrict() === '') {
            return self.District_byProvice();
        }
        else {
            return self.District_byProvice().filter(function (item) {
                return SearchTxt_inVue(self.searchDistrict().split(" "), item['TenQuanHuyen']) === true;
            });
        }
    });

    // at list LienHe
    self.filterProvince = function (item, inputString) {
        var itemSearch = locdau(item.TenTinhThanh);
        var locdauInput = locdau(inputString);
        var charStart = GetChartStart(itemSearch);

        return itemSearch.indexOf(locdauInput) > -1 ||
            charStart.indexOf(locdauInput) > -1;
    }

    // at list LienHe
    self.filterDistrict = function (item, inputString) {
        var itemSearch = locdau(item.TenQuanHuyen);
        var locdauInput = locdau(inputString);
        var charStart = GetChartStart(itemSearch);

        return itemSearch.indexOf(locdauInput) > -1 ||
            charStart.indexOf(locdauInput) > -1;
    }

    self.ChoseProvince_Modal = function (item) {
        self.searchTT('');
        self.searchDistrict('');

        $(txtDistrict_modal).text('--Chọn quận huyện--');
        self.newUserContact().ID_QuanHuyen(undefined);

        if (item == undefined || item.ID == undefined) {
            $(txtProvince_modal).text('--Chọn Tỉnh thành--');
            self.newUserContact().ID_TinhThanh(undefined);
        }
        else {
            $(txtProvince_modal).text(item.TenTinhThanh);
            self.newUserContact().ID_TinhThanh(item.ID);

            // get list district by ID_Province
            getListDistrict(item.ID);
        }
    }

    self.ChoseDistrict_Modal = function (item) {
        if (item.ID == undefined) {
            $(txtDistrict_modal).text('--Chọn quận huyện--');
            self.newUserContact().ID_QuanHuyen(undefined);
        }
        else {
            $(txtDistrict_modal).text(item.TenQuanHuyen);
            self.newUserContact().ID_QuanHuyen(item.ID);
        }
    }

    self.ChoseCustomer_Modal = function (item) {
        var thisObj = event.currentTarget;
        $('#showseach_DoiTuong span[id^="spanCheckDoiTuong_"] i').remove();

        if (!self.IsCustomer()) {
            sLoai = 'nhà cung cấp';
        }

        if (item.ID == undefined) {
            $(txtCustomer_modal).text('--Chọn' + sLoai + '--');
            self.newUserContact().ID_DoiTuong(undefined);
        }
        else {
            $(txtCustomer_modal).text(item.TenDoiTuong);
            self.newUserContact().ID_DoiTuong(item.ID);
            // append check <i>
            $('#spanCheckDoiTuong_' + item.ID).append(element_appendCheck);
        }
    }

    self.Quyen_NguoiDung = ko.observableArray();
    self.RoleView_Contact = ko.observable();
    self.RoleInsert_Contact = ko.observable();
    self.RoleUpdate_Contact = ko.observable();
    self.RoleDelete_Contact = ko.observable();
    self.RoleExport_Contact = ko.observable();

    function GetHT_Quyen_ByNguoiDung() {
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetHT_NhomNguoiDung?idnguoidung=" + userID + '&iddonvi=' + idDonVi, 'GET').done(function (data) {

            if (data != null) {
                self.Quyen_NguoiDung(data.HT_Quyen_NhomDTO);

                HideShowButton_ByRole();
            }
            else {
                ShowMessage_Danger('Không có quyền xem danh sáchn liên hệ ');
            }
        });
    }

    function HideShowButton_ByRole() {

        // Xem
        var arrRoleCus = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen.indexOf('LienHe_') > -1;
        });

        var itemView = $.grep(arrRoleCus, function (x) {
            return x.MaQuyen.indexOf('LienHe_XemDS') > -1;
        });

        if (itemView.length > 0) {
            $('#tblList').show();
            $('#btnDropdownView').show();
            $('#myList').css('display', '');
            self.RoleView_Contact(true);

            SearchList();
        }
        else {
            ShowMessage_Danger('Không có quyền xem danh sách liên hệ');
        }

        // Them moi
        var itemInsert = $.grep(arrRoleCus, function (x) {
            return x.MaQuyen.indexOf('LienHe_ThemMoi') > -1;
        });

        if (self.RoleView_Contact() === true && itemInsert.length > 0) {
            $('#btnAddUserContact').show();
            self.RoleInsert_Contact(true);
        }

        // xuat file
        var itemExport = $.grep(arrRoleCus, function (x) {
            return x.MaQuyen === 'LienHe_XuatFile';
        });

        if (self.RoleView_Contact() === true && itemExport.length > 0) {
            $('#btnExport').show();
            self.RoleExport_Contact(true);
        }

        // import file
        var itemImport = $.grep(arrRoleCus, function (x) {
            return x.MaQuyen.indexOf('LienHe_Import') > -1;
        });

        if (self.RoleView_Contact() === true && self.RoleInsert_Contact() && itemImport.length > 0) {
            $('#btnImport').show();
        }

        // update
        var itemUpdate = $.grep(arrRoleCus, function (x) {
            return x.MaQuyen.indexOf('LienHe_CapNhat') > -1;
        });

        if (self.RoleView_Contact() === true && itemUpdate.length > 0) {
            self.RoleUpdate_Contact(true);
        }

        // xoa
        var itemDelete = $.grep(arrRoleCus, function (x) {
            return x.MaQuyen.indexOf('LienHe_Xoa') > -1;
        });

        if (self.RoleView_Contact() === true && itemDelete.length > 0) {
            self.RoleDelete_Contact(true);
        }
    }

    function GetWhere() {
        var sWhere = ' DM_LienHe.TrangThai !=0 ';

        // search ngay tao
        var _now = new Date();
        var currentWeekDay = _now.getDay();
        var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
        var dtFrom = '';
        var dtTo = '';

        if (self.filterNgayTao() === '1') {
            switch (parseInt(self.filterNgayTao_Quy())) {
                case 0:
                    // all
                    dtFrom = '1918-01-01'; // 100 years ago
                    dtTo = moment(_now).add('days', 1).format('YYYY-MM-DD');
                    break;
                case 1:
                    // hom nay
                    dtFrom = dtTo = moment(_now).format('YYYY-MM-DD');
                    break;
                case 2: // hom qua
                    dtFrom = dtTo = moment(_now).subtract('day', 1).format('YYYY-MM-DD');
                    break;
                case 3:
                    // tuan nay
                    dtFrom = moment().startOf('week').add('days', 1).format('YYYY-MM -DD');
                    dtTo = moment().endOf('week').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 4:
                    // tuan truoc
                    dtFrom = moment().startOf('week').subtract('days', 6).format('YYYY-MM-DD');
                    dtTo = moment().startOf('week').format('YYYY-MM-DD');
                    break;
                case 5:
                    // thang nay
                    dtFrom = moment().startOf('month').format('YYYY-MM -DD');
                    dtTo = moment().endOf('month').format('YYYY-MM -DD');
                    break;
                case 6:
                    // thang truoc
                    dtFrom = moment().subtract('months', 1).startOf('month').format('YYYY-MM-DD');
                    dtTo = moment().subtract('months', 1).endOf('month').format('YYYY-MM-DD');
                    break;
                case 7:
                    // quy nay
                    dtFrom = moment().startOf('quarter').format('YYYY-MM-DD');
                    dtTo = moment().endOf('quarter').format('YYYY-MM-DD');
                    break;
                case 8:
                    // quy truoc = currQuarter -1; // if (currQuarter -1 == 0) --> (assign = 1)
                    var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
                    dtFrom = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                    dtTo = moment().quarter(prevQuarter).endOf('quarter').format('YYYY-MM-DD');
                    break;
                case 9:
                    // nam nay
                    dtFrom = moment().startOf('year').format('YYYY-MM-DD');
                    dtTo = moment().endOf('year').format('YYYY-MM-DD');
                    break;
                case 10:
                    // nam truoc
                    var prevYear = moment().year() - 1;
                    dtFrom = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                    dtTo = moment().year(prevYear).endOf('year').format('YYYY-MM-DD');
                    break;
            }
        }
        else {
            // chon ngay cu the
            var arrDate = self.filterNgayTao_Input().split('-');
            dtFrom = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
            dtTo = moment(arrDate[1], 'DD/MM/YYYY').add('days', 1).format('YYYY-MM-DD');
        }

        // tinh thanh, quan huyen
        var idProvince = self.selectProvince();
        var idDistrict = self.selectDistrict();
        var txtSearch = $('#txtSearch').val();

        if (idProvince !== undefined) {
            sWhere += " AND DM_LienHe.ID_TinhThanh like '%25" + idProvince + "%25'";
        }

        if (idDistrict !== undefined) {
            if (sWhere === "") {
                sWhere = " DM_LienHe.ID_QuanHuyen like '%25" + idDistrict + "%25'";
            }
            else {
                sWhere += " AND  DM_LienHe.ID_QuanHuyen like '%25" + idDistrict + "%25'";
            }
        }

        if (dtFrom !== '' && dtTo !== '') {
            if (sWhere === "") {
                sWhere = " CAST(DM_LienHe.NgayTao as date) >= '" + dtFrom + "' AND  CAST(DM_LienHe.NgayTao as date) <='" + dtTo + "'";
            }
            else {
                sWhere += " AND  CAST(DM_LienHe.NgayTao as date) >= '" + dtFrom + "' AND  CAST(DM_LienHe.NgayTao as date) <='" + dtTo + "'";
            }
        }

        // not filter NhanVien by where, filter in API

        //var lstNhanVien = '';
        //for (var i = 0; i < self.ListIDNhanVienQuyen().length; i++) {
        //    lstNhanVien += self.ListIDNhanVienQuyen()[i] + ',';
        //}
        //lstNhanVien = Remove_LastComma(lstNhanVien);

        //if (lstNhanVien !== '') {
        //    sWhere += " AND (dt.ID_NhanVienPhuTrach IN (Select * from splitstring('" + lstNhanVien + "')) " +
        //        " OR dt.ID_NhanVienPhuTrach IS NULL OR DM_LienHe.NguoiTao like '%" + user.trim() + "%' )";
        //}

        console.log('sWhere', sWhere)

        if (txtSearch !== '') {
            var locdauInput = locdau(txtSearch);
            if (sWhere === "") {
                sWhere = " MaLienHe like '%25" + txtSearch + "%25' OR TenLienHe like N'%25" + txtSearch + "%25' OR TenDoiTuong like N'%25" + txtSearch + "%25'" +
                    " OR dbo.Func_ConvertStringToUnsign(TenLienHe) like N%25'" + locdauInput + "%25')" +
                    " OR TenDoiTuong_KhongDau like N'%25" + locdauInput + "%25'";
            }
            else {
                sWhere += " AND (MaLienHe like '%25" + txtSearch + "%25' OR TenLienHe like N'%25" + txtSearch + "%25' OR TenDoiTuong like N'%25" + txtSearch + "%25'" +
                    "  OR dbo.Func_ConvertStringToUnsign(TenLienHe) like N'%25" + locdauInput + "%25'" +
                    " OR TenDoiTuong_KhongDau like N'%25" + locdauInput + "%25')";
            }
        }
        return sWhere;
    }

    self.Click_IconSearch = function () {
        self.currentPage(0);
        SearchList();
    }

    function SearchList() {

        var hasPermisson = true;

        var sWhere = GetWhere();

        var idManagers = '';
        for (var i = 0; i < self.ListIDNhanVienQuyen().length; i++) {
            idManagers += self.ListIDNhanVienQuyen()[i] + ',';
        }

        console.log('idManagers', idManagers)

        if (hasPermisson) {
            ajaxHelper(DMLienHeUri + 'GetAllUserContact_byWhere_FilterNhanVien?txtSearch=' + sWhere + '&idManagers=' + idManagers, 'GET').done(function (data) {
                $('#wait').remove();
                if (data !== null) {
                    self.listUserContact(data);
                    var lenData = data.length;
                    self.PageCount(Math.ceil(lenData / self.pageSize()));
                    self.TotalRecord(lenData);
                    //$("#tblList").colResizable({
                    //    liveDrag: true,
                    //    gripInnerHtml: "<div class='grip'></div>",
                    //    draggingClass: "dragging",
                    //    resizeMode: 'overflow',
                    //    hoverCursor: "col-resize",
                    //    dragCursor: "col-resize",
                    //    disabledColumns: [0],
                    //});
                }
            });
        }
        else {
            $('#wait').remove();
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + 'Không có quyền xem danh sách liên hệ', 'danger');
        }
    }

    self.showPopupAddNguoiLienHe = function () {
        $('#lblTitle').text('Thêm mới liên hệ');
        $('#modalPopuplg_Contact').modal('show');
        $('#lblDoiTuong').text('Khách hàng');
        self.booleanAdd(true);
        self.newUserContact(new FormModel_NewUserContact());
        getAllDMDoiTuong(1);
    }

    self.showPopupEditNguoiLienHe = function (item) {
        $('#lblTitle').text('Cập nhật liên hệ');
        $('#modalPopuplg_Contact').modal('show');
        self.newUserContact().SetData(item);
        self.booleanAdd(false);

        if (item.LoaiLienHe == 1) {
            self.IsCustomer(true);
            getAllDMDoiTuong(1);
        }
        else {
            self.IsCustomer(false);
            getAllDMDoiTuong(2);
        }

        // find DoiTuong by ID
        var itemDT = $.grep(self.DoiTuongs(), function (x) {
            return x.ID === item.ID_DoiTuong;
        });
        if (itemDT.length > 0) {
            $(txtCustomer_modal).text(itemDT[0].TenDoiTuong)
        }

        // find province by ID
        var itemPr = $.grep(self.listProvince(), function (x) {
            return x.ID === item.ID_TinhThanh;
        });
        if (itemPr.length > 0) {
            //$(txtProvince_modal).text(itemPr[0].TenTinhThanh);
            self.ChoseProvince_Modal(itemPr[0]);
        }
        else {
            self.ChoseProvince_Modal();
        }
        // find district by ID
        var itemQH = $.grep(self.listDistrict(), function (x) {
            return x.ID === item.ID_QuanHuyen;
        });
        if (itemQH.length > 0) {
            $(txtDistrict_modal).text(itemQH[0].TenQuanHuyen);
        }

        self.HaveImage_Select(self.HaveImage());
        if (self.HaveImage_Select() === false) {
            self.FilesSelect([]);
            $('#file').val('');
        }
        else {
            self.FilesSelect(self.FileImgs());
        }
    }

    function Enable_btnSave() {
        $('#btnSave').removeAttr('disabled');
        $('#btnSave').text('Lưu');
    }

    self.addUpdate_UserContract = function (formElement) {

        $('#btnSave').attr('disabled', 'disabled');
        $('#btnSave').text('Đang lưu');

        var id = self.newUserContact().ID();
        var maLienHe = self.newUserContact().MaLienHe();
        var tenLienHe = self.newUserContact().TenLienHe();
        var idQuanHuyen = self.newUserContact().ID_QuanHuyen();
        var idTinhThanh = self.newUserContact().ID_TinhThanh();
        var ngaysinh = self.newUserContact().NgaySinh();
        var email = self.newUserContact().Email();
        var ghichu = self.newUserContact().GhiChu();
        var dienthoai = self.newUserContact().SoDienThoai();
        var dienthoaiCoDinh = self.newUserContact().DienThoaiCoDinh();
        var idDoiTuong = self.newUserContact().ID_DoiTuong();
        var diaChi = self.newUserContact().DiaChi();
        var chucVu = self.newUserContact().ChucVu();
        var xungHo = self.newUserContact().XungHo();

        if (idTinhThanh == undefined) {
            idTinhThanh = null;
        }

        if (idQuanHuyen == undefined) {
            idQuanHuyen = null;
        }

        var msgCheck = CheckInput(self.newUserContact());
        if (msgCheck !== '') {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + msgCheck, 'danger');
            Enable_btnSave();
            return false;
        }

        if (ngaysinh !== null && ngaysinh !== undefined && ngaysinh !== '') {
            ngaysinh = moment(ngaysinh, 'DD/MM/YYYY').format('YYYY-MM-DD');

            var checkNS = isValidDateYYYYMMDD(ngaysinh);
            if (!checkNS) {
                Enable_btnSave();
                return;
            }
        }

        var DM_LienHe = {
            ID: id,
            ID_DoiTuong: idDoiTuong,
            MaLienHe: maLienHe,
            XungHo: xungHo, // 0: anh, chi, co, bac, chu (todo)
            TenLienHe: tenLienHe,
            SoDienThoai: dienthoai,
            DienThoaiCoDinh: dienthoaiCoDinh, //(todo)
            Email: email,
            DiaChi: diaChi,
            ChucVu: chucVu,
            NgaySinh: ngaysinh,
            GhiChu: ghichu,
            ID_TinhThanh: idTinhThanh,
            ID_QuanHuyen: idQuanHuyen,
            NguoiTao: user,
        };

        console.log('DM_LienHe', DM_LienHe);

        if (navigator.onLine) {
            // insert LienHe
            if (self.booleanAdd() === true) {
                $.ajax({
                    url: DMLienHeUri + "AddDM_LienHe",
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    data: DM_LienHe,
                    success: function (item) {
                        // push in to list
                        self.listUserContact.unshift(item);
                        self.TotalRecord(self.TotalRecord() + 1);

                        Insert_NhatKyThaoTac(item, 1, 1);

                        self.InsertImage(item.ID, item.MaLienHe);

                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i> Thêm mới người liên hệ thành công', 'success');
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> Thêm mới người liên hệ thất bại', 'danger');
                    },
                    complete: function () {
                        $('#modalPopuplg_Contact').modal('hide');
                        Enable_btnSave();
                    }
                })
            }
            // update LienHe
            else {

                DM_LienHe.NguoiSua = user;

                $.ajax({
                    url: DMLienHeUri + "UpdateDM_LienHe",
                    type: 'PUT',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    data: DM_LienHe,
                    success: function (item) {
                        for (var i = 0; i < self.listUserContact().length; i++) {
                            if (self.listUserContact()[i].ID === id) {
                                self.listUserContact.remove(self.listUserContact()[i]);
                                break;
                            }
                        }
                        self.listUserContact.unshift(item);
                        Insert_NhatKyThaoTac(item, 1, 2);

                        self.InsertImage(item.ID, item.MaLienHe);

                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i> Cập nhật người liên hệ thành công', 'success');
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> Cập nhật người liên hệ thất bại', 'danger');
                    },
                    complete: function () {
                        $('#modalPopuplg_Contact').modal('hide');
                        Enable_btnSave();
                    }
                })
            }
        }
    }

    self.modalDelete = function (item) {

        // khong can check quyen vi da an button roi
        var id = item.ID;

        dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn xóa người liên hệ có mã  <b> ' + item.MaLienHe + '</b> không?', function () {
            $.ajax({
                type: "POST",
                url: DMLienHeUri + "DeleteDM_LienHe/" + id,
                dataType: 'json',
                contentType: 'application/json',
                success: function (result) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa người liên hệ thành công", "success");

                    Insert_NhatKyThaoTac(item, 1, 3);

                    SearchList();
                },
                error: function (error) {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Xóa người liên hệ thất bại.", "danger");
                }, complete: function () {
                    $('#modalPopuplgDelete').modal('hide');
                }
            })
        })
    };

    self.DeleteMany = function () {

        var roleDelete = true;
        if (roleDelete) {
            dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn xóa những liên hệ đã chọn không?', function () {
                for (var i = 0; i < arrIDCheck.length; i++) {
                    ajaxHelper(DMLienHeUri + 'DeleteDM_LienHe/' + arrIDCheck[i], 'POST').done(function (data) {
                        $('#divThaoTac').hide();
                        SearchList();
                    });
                }
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa nhiều liên hệ thành công!", "success");
                $('#divThaoTac').hide();
                $('.choose-commodity').hide();
                arrIDCheck = [];
            });
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Không có quyền xóa liên hệ", "danger");
        }
    }

    function Insert_NhatKyThaoTac(objUsing, chucNang, loaiNhatKy) {
        chucNang = chucNang || 1;
        loaiNhatKy = loaiNhatKy || 1;
        // chuc nang (1.DM_LienHe, 4.Export, 5.Import)
        var tenChucNang = '';
        var noiDung = '';
        var noiDungChiTiet = '';
        var txtFirst = '';
        var tenChucNangLowercase = ' người liên hệ ';

        var style1 = '<a style= \"cursor: pointer\" onclick = \"';
        var style2 = "('";
        var style3 = "')\" >";
        var style4 = '</a>';

        var funcNameKH = 'LoadNguoiLienHe_byMaKH';

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

        if (chucNang === 1) {
            tenChucNang = 'Người liên hệ';

            if (loaiNhatKy < 4) {
                // them, sua, xoa
                var maLienHe = objUsing.MaLienHe;
                var ngaySinh = '';
                var tenDoiTuong = '';
                var dienThoai = '';

                if (objUsing.NgaySinh !== null && objUsing.NgaySinh !== undefined) {
                    ngaySinh = 'Ngày sinh: ' + moment(objUsing.NgaySinh, 'YYYY-MM-DD HH:mm:ss').format('DD/MM/YYYY') + ', ';
                }
                if (objUsing.SoDienThoai !== null && objUsing.SoDienThoai !== undefined) {
                    dienThoai = 'Điện thoại: ' + objUsing.SoDienThoai + ', ';
                }
                noiDung = txtFirst.concat(tenChucNangLowercase, maLienHe, ', Tên: ', objUsing.TenLienHe, ', ', ngaySinh, dienThoai, tenDoiTuong);
                noiDungChiTiet = txtFirst.concat(tenChucNangLowercase, style1, funcNameKH, style2, maLienHe, style3, maLienHe, style4, ', tên: ', objUsing.TenLienHe, ', ',
                    ngaySinh, dienThoai, tenDoiTuong);
                noiDungChiTiet = Remove_LastComma(noiDungChiTiet);
                noiDungChiTiet = noiDungChiTiet.concat('<br /> Nhân viên thực hiện: ', user);
            }
            else {
                // import, export
                noiDung = txtFirst.concat('danh sách ', tenChucNangLowercase);
                noiDungChiTiet = noiDung.concat('<br /> Nhân viên thực hiện: ', user)
            }
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

        var myDataNK = {};
        myDataNK.objDiary = objNhatKy;

        $.ajax({
            url: '/api/DanhMuc/SaveDiary/' + "post_NhatKySuDung",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myDataNK,
            success: function (x) {

            },
        });
    }

    function CheckInput(obj) {
        var sReturn = '';

        var id = obj.ID();
        var maLienHe = obj.MaLienHe();
        var tenLienHe = obj.TenLienHe();
        var email = obj.Email();
        var phone = obj.SoDienThoai();
        var idTinhThanh = obj.ID_TinhThanh();
        var idQuanHuyen = obj.ID_QuanHuyen();
        var idDoiTuong = obj.ID_DoiTuong();

        if (tenLienHe === null || tenLienHe === "" || tenLienHe === undefined) {
            sReturn = 'Vui lòng nhập tên liên hệ  <br />';
        }

        // insert
        var lst = self.listUserContact();
        if (id === undefined) {
            for (var i = 0; i < lst.length; i++) {
                if (maLienHe !== undefined && lst[i].MaLienHe.toLowerCase() === maLienHe.trim().toLowerCase()) {
                    sReturn += 'Mã người liên hệ tồn tại <br />';
                    break;
                }

                if (lst[i].SoDienThoai !== null && phone !== undefined
                    && lst[i].SoDienThoai !== '' && phone !== ''
                    && lst[i].SoDienThoai.trim() === phone.trim()) {
                    sReturn += 'Số điện thoại đã tồn tại <br />';
                    break;
                }

                if (lst[i].Email !== null && email !== undefined
                    && lst[i].Email !== '' && email !== ''
                    && lst[i].Email === email.trim()) {
                    sReturn += 'Email đã tồn tại <br />';
                    break;
                }
            }
        }
        // update
        else {
            for (var i = 0; i < lst.length; i++) {
                if (id !== lst[i].ID) {
                    if (maLienHe !== undefined && lst[i].MaLienHe === maLienHe.trim()) {
                        sReturn += 'Mã người liên hệ đã tồn tại <br />';
                        break;
                    }

                    if (lst[i].SoDienThoai !== null && lst[i].SoDienThoai !== undefined
                        && phone !== null && lst[i].SoDienThoai !== '' && phone !== ''
                        && lst[i].SoDienThoai.trim() === phone.trim()) {
                        sReturn += 'Số điện thoại đã tồn tại <br />';
                        break;
                    }

                    if (lst[i].Email !== null && email !== undefined
                        && email !== null && lst[i].Email !== '' && email !== ''
                        && lst[i].Email === email.trim()) {
                        sReturn += 'Email đã tồn tại <br />';
                        break;
                    }
                }
            }
        }

        if (CheckChar_Special(maLienHe)) {
            sReturn += 'Mã người liên hệ không được chứa kí tự đặc biệt <br />';
        }

        if (idDoiTuong == null || idDoiTuong == undefined) {
            sReturn += 'Vui lòng chọn ' + sLoai + ' liên hệ <br />';
        }

        if (email !== '' && email !== undefined && email !== null) {
            var valReturn = ValidateEmail(email);
            if (valReturn === false) {
                sReturn += 'Email không hợp lệ <br />';
            }
        }

        if (idTinhThanh !== undefined && idTinhThanh !== null && idTinhThanh.indexOf('000') === -1) {
            var itemTT = $.grep(self.listProvince(), function (item) {
                return item.ID === idTinhThanh;
            });
            if (itemTT.length === 0) {
                sReturn += 'Tỉnh thành không có trong hệ thống <br />';
            }
        }
        else {
            // find province in list
            var txtTenTT = $(txtProvince_modal).val();
            if (txtTenTT !== '') {
                var itemTT = $.grep(self.listProvince(), function (x) {
                    return x.TenTinhThanh.trim().toLowerCase() === txtTenDT.trim().toLowerCase();
                });
                if (itemTT.length === 0) {
                    sReturn += 'Tỉnh thành không có trong hệ thống <br />';
                }
            }
        }

        if (idQuanHuyen !== undefined && idQuanHuyen !== null && idQuanHuyen.indexOf('000') === -1) {
            var itemQH = $.grep(self.listDistrict(), function (item) {
                return item.ID === idQuanHuyen;
            });
            if (itemQH.length === 0) {
                sReturn += 'Quận huyện không có trong hệ thống <br />';
            }
        }
        else {
            // find province in list
            var txtTenQH = $(txtDistrict_modal).val();
            if (txtTenQH !== '') {
                var itemQH = $.grep(self.listDistrict(), function (x) {
                    return x.TenQuanHuyen.trim().toLowerCase() === txtTenQH.trim().toLowerCase();
                });
                if (itemQH.length === 0) {
                    sReturn += 'Quận huyện không có trong hệ thống <br />';
                }
            }
        }
        return sReturn;
    }

    self.formatDateTime = function () {
        $('.datepicker_mask').datetimepicker(
            {
                format: "d/m/Y",
                mask: true,
                timepicker: false,
                //datepicker: false, // hide calander when click
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
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + err, 'danger');
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
                        //self.FilesSelect.push(new FileModel(theFile, e.target.result)); // if many image
                        self.FilesSelect([]); // only get 1 image
                        self.FilesSelect.push(new FileModel(theFile, e.target.result));
                    };
                })(f);

                // Read in the image file as a data URL.
                reader.readAsDataURL(f);
                self.HaveImage_Select(true);
            }
        }
    };

    self.InsertImage = function (idUser, codeUser) {
        for (var i = 0; i < self.FilesSelect().length; i++) {

            var formData = new FormData();
            formData.append("file", self.FilesSelect()[i].file);

            // %2f = /
            $.ajax({
                type: "POST",
                url: DMLienHeUri + "ImageUpload?id=" + idUser + '&pathFolder=ImgUserContact%2f' + codeUser, // ImgUserContact + MaLienHe
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

    self.GetDetail_InforLienHe = function (item) {
        GetImages_LienHe(item.ID);

        if (self.RoleUpdate_Contact()) {
            $('.edit').show();
        }

        if (self.RoleDelete_Contact()) {
            $('.xoa').show();
        }
    }

    function GetImages_LienHe(id) {
        ajaxHelper(DMLienHeUri + 'GetImages_byIDLienHe/' + id, 'GET').done(function (data) {
            if (data !== null && data.length > 0) {
                self.HaveImage(true);

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

    self.ResetCurrentPage = function () {
        self.currentPage(0);
    };

    // find with conditional
    self.selectProvince.subscribe(function (newID) {
        self.selectProvince(newID);
        SearchList();
    });

    self.selectDistrict.subscribe(function (newID) {
        self.selectDistrict(newID);
        SearchList();
    });

    $('#txtSearchProvince').keypress(function () {
        if (event.keyCode === 13) {
            if ($('#txtSearchProvince').val() === '') {
                self.selectProvince(undefined);
                SearchList();
            }
        }
    })

    $('#txtSearchDistrict').keypress(function () {
        if (event.keyCode === 13) {
            if ($('#txtSearchDistrict').val() === '') {
                self.selectDistrict(undefined);
                SearchList();
            }
        }
    })

    $('.choose_txtTime li').on('click', function () {
        $('.ip_TimeReport').val($(this).text());
        self.filterNgayTao_Quy($(this).val());
        SearchList();
    });

    $('#filterDate').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        var thisDate = $(this).val();
        self.filterNgayTao_Input(thisDate);
        SearchList();
    });

    self.filterNgayTao.subscribe(function (newVal) {
        SearchList();
    });

    $('#txtSearch').keypress(function () {
        if (event.keyCode === 13) {
            SearchList();
        }
    });

    // paging
    self.PageResults = ko.computed(function () {
        var first = self.currentPage() * self.pageSize();
        if (self.listUserContact() !== null) {
            return self.listUserContact().slice(first, first + self.pageSize());
        }
    });

    self.PageList = ko.computed(function () {
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
                    if (currentPage == 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
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

        if (self.listUserContact() !== null) {

            self.fromitem((self.currentPage() * self.pageSize()) + 1);

            if (((self.currentPage() + 1) * self.pageSize()) > self.listUserContact().length) {
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
        loadHtmlGrid();
        SetCheck_Input();
        return arrPage;
    });

    self.VisibleStartPage = ko.computed(function () {
        if (self.PageList().length > 0) {
            return self.PageList()[0].pageNumber !== 1;
        }
    });

    self.VisibleEndPage = ko.computed(function () {
        if (self.PageList().length > 0) {
            return self.PageList()[self.PageList().length - 1].pageNumber !== self.PageCount();
        }
    })

    self.ResetCurrentPage = function () {
        var lenData = self.listUserContact().length;
        self.currentPage(0);
        self.PageCount(Math.ceil(lenData / self.pageSize()));
    };

    self.GoToPage = function (page) {
        self.currentPage(page.pageNumber - 1);
    };

    function SetCheck_Input() {
        // find in list and set check
        var countCheck = 0;
        $('#tblList tr td.check-group input').each(function (x) {
            var id = $(this).attr('id');
            if ($.inArray(id, arrIDCheck) > -1) {
                $(this).prop('checked', true);
                countCheck += 1;
            }
            else {
                $(this).prop('checked', false);
            }
        });

        // set again check header
        var ckHeader = $('#tblList thead tr th:eq(0) input')
        if (countCheck == self.PageResults().length) {
            ckHeader.prop('checked', true);
        }
        else {
            ckHeader.prop('checked', false);
        }
    }

    self.GetClass = function (page) {
        return ((page.pageNumber - 1) === self.currentPage()) ? "click" : "";
    };

    self.StartPage = function () {
        self.currentPage(0);
    }

    self.BackPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
        }
    }

    self.GoToNextPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.currentPage() + 1);
        }
    }

    self.EndPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
        }
    }

    var columnHide = '';
    self.GetIndexColumnHide = function (item) {
        // get list column is hide
        $('#tblList thead tr th').each(function (index) {
            if ($(this).css('display') === 'none') {
                // find index of th
                columnHide += parseInt(index - 1) + '_';
            }
        })

        // remove char '_' last
        if (columnHide) {
            columnHide = columnHide.substr(0, columnHide.length - 1);
        }
    }

    self.ExportExcel = function () {
        self.GetIndexColumnHide();

        var sWhere = GetWhere();
        var arrIDManager = [];
        for (var i = 0; i < self.ListIDNhanVienQuyen().length; i++) {
            if ($.inArray(self.ListIDNhanVienQuyen()[i], arrIDManager) === -1) {
                arrIDManager.push(self.ListIDNhanVienQuyen()[i]);
            }
        }

        var url = DMLienHeUri + 'ExportExcel_ListLienHe?txtSearch=' + sWhere + '&idManagers=' + arrIDManager + "&columnsHide=" + columnHide;
        window.location.href = url;

        Insert_NhatKyThaoTac(null, 1, 6);
    }

    // import file
    self.ContinueImport = ko.observable(false);
    self.loiExcel = ko.observableArray();
    self.addRownError = ko.observableArray();
    self.visibleImport = ko.observable(true);
    self.fileNameExcel = ko.observable("Lựa chọn file Excel...");
    $(".BangBaoLoi").hide();

    self.notvisibleImport = ko.computed(function () {
        return !self.visibleImport();

    });

    self.DownloadFileTeamplateXLS = function () {
        var url = '/api/DanhMuc/DM_HangHoaAPI/' + "Download_TeamplateImport?fileSave=" + "Temp_Import_DMLienHe.xls";
        window.open(url)
    }
    //Download file teamplate excel format (*.xlsx)
    self.DownloadFileTeamplateXLSX = function () {
        var url = '/api/DanhMuc/DM_HangHoaAPI/' + "Download_TeamplateImport?fileSave=" + "Temp_Import_DMLienHe.xlsx";
        window.open(url)
    }

    $(".filterFileSelect").hide();
    $(".btnImportExcel").hide();
    //check ignore error
    $('.startImport').attr('disabled', 'false');
    $('.startImport').removeClass("btn-green");
    $('.startImport').addClass("StartImport");

    $('.choseContinue input').on('click', function () {

        if ($(this).val() == 0) {
            $(this).val(1);
            $('.startImport').removeAttr('disabled');
            $('.startImport').addClass("btn-green");
            $('.startImport').removeClass("StartImport");
        }
        else {
            $(this).val(0);
            $('.startImport').attr('disabled', 'false');
            $('.startImport').removeClass("btn-green");
            $('.startImport').addClass("StartImport");
        }
    });

    self.SelectedFileImport = function (vm, evt) {
        self.fileNameExcel(evt.target.files[0].name)
        $(".filterFileSelect").show();
        $(".btnImportExcel").show();
        $(".NoteImport").show();
        $(".BangBaoLoi").hide();
        self.visibleImport(false);
    }

    self.refreshFileSelect = function () {
        self.visibleImport(true);
        self.fileNameExcel("Lựa chọn file Excel...");
        $(".filterFileSelect").hide();
        $(".btnImportExcel").hide();
        document.getElementById('imageUploadFormKH').value = "";
    }

    self.ShowandHide = function () {
        self.Execute_Import();
    }

    self.Execute_Import = function () {
        hidewait('printinport');
        // $("#wait").remove();
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadFormKH").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadFormKH").files[i];
            formData.append("imageUploadFormKH", file);
        }
        $.ajax({
            type: "POST",
            url: DMLienHeUri + "ImportExcel_DMLienHe?nguoitao=" + user,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (item) {
                self.loiExcel(item);
                if (self.loiExcel() != null) {
                    self.visibleImport(true);
                    $(".BangBaoLoi").show();
                    $(".NoteImport").hide();
                    $(".filterFileSelect").hide();
                    $(".btnImportExcel").hide();
                }
                else {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Import liên hệ thành công!", "success");

                    Insert_NhatKyThaoTac(null, 1, 5);

                    document.getElementById('imageUploadFormKH').value = "";
                    $(".NoteImport").show();
                    $(".filterFileSelect").hide();
                    $(".btnImportExcel").hide();
                    $(".BangBaoLoi").hide();
                    $("#myModalinport").modal("hide");
                    SearchList();
                }
                $("div[id ^= 'wait']").text("");
            },
            error: function (jqXHR, textStatus, errorThrown) {
            },
        });
    }

    self.Import_PassError = function () {
        var rownError = null;
        for (var i = 0; i < self.loiExcel().length; i++) {
            if (self.addRownError().length < 1) {
                self.addRownError.push(self.loiExcel()[i].rowError);
            }
            else {
                for (var j = 0; j < self.addRownError().length; j++) {
                    if (self.addRownError()[j] === self.loiExcel()[i].rowError) {
                        break;
                    }
                    if (j == self.addRownError().length - 1) {
                        self.addRownError.push(self.loiExcel()[i].rowError);
                        break;
                    }
                }
            }
        }
        // self.addRownError.sort();
        self.addRownError = self.addRownError.sort(function (a, b) {
            var x = a, y = b;
            return x > y ? 1 : x < y ? -1 : 0;
        })
        //console.log(self.addRownError());
        for (var i = 0; i < self.addRownError().length; i++) {
            if (i == 0)
                rownError = self.addRownError()[i];
            else
                rownError = rownError + "_" + self.addRownError()[i];
        }
        console.log(rownError);
        hidewait('printinport');
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadFormKH").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadFormKH").files[i];
            formData.append("imageUploadFormKH", file);
        }
        $.ajax({
            type: "POST",
            url: DMLienHeUri + "ImportDMLienHe_PassError?RowsError=" + rownError + '&nguoitao=' + user,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (item) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Import liên hệ thành công!", "success");

                Insert_NhatKyThaoTac(null, 1, 5);

                document.getElementById('imageUploadFormKH').value = "";
                $(".NoteImport").show();
                $(".filterFileSelect").hide();
                $(".btnImportExcel").hide();
                $(".BangBaoLoi").hide();
                self.visibleImport(true);
                $("#myModalinport").modal("hide");
                SearchList();
                $("div[id ^= 'wait']").text("");
            },
            statusCode: {
                404: function () {
                    $("div[id ^= 'wait']").text("");
                },
                500: function () {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Import liên hệ thất bại!", "danger");
                    $("div[id ^= 'wait']").text("");
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $("div[id ^= 'wait']").text("");
            },
        });

    }

    self.DeleteImg = function (item, event) {
        if (item.ID !== undefined) {
            dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn xóa ảnh của khách hàng ' + self.newUserContact().MaLienHe() + '</b> không?', function () {
                $.ajax({
                    type: "DELETE",
                    url: DMLienHeUri + "DeleteDM_LienHe_Anh/" + item.ID,
                    dataType: 'json',
                    contentType: 'application/json',
                    success: function (result) {
                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa ảnh thành công!", "success");
                    },
                    error: function (error) {
                        $('#modalPopuplgDelete').modal('hide');
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Xóa ảnh liên hệ thất bại.", "danger");
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
            self.FilesSelect.remove(item);

            if (self.FilesSelect().length == 0) {
                self.HaveImage_Select(false);
                self.AnhDaiDien([]);
                $('#file').val('');
            }
        }
    }

    self.IsCustomer.subscribe(function (newValue) {
        self.filterDoiTuong('');
        if (newValue === true) {
            sLoai = 'khách hàng';
            $('#lblDoiTuong').text('Khách hàng');
            getAllDMDoiTuong(1);
        }
        else {
            sLoai = 'nhà cung cấp';
            $('#lblDoiTuong').text('Nhà cung cấp');
            getAllDMDoiTuong(2);
        }
        $(txtCustomer_modal).text('--Chọn ' + sLoai + '--');
        $('#showseach_DoiTuong ul li:eq(0)').text('--Chọn ' + sLoai + '--');
    })

}
ko.applyBindings(new ViewModal());

$(function () {
    $('#filterDate').daterangepicker({
        locale: {
            "format": 'DD/MM/YYYY',
            "separator": " - ",
            "applyLabel": "Tìm kiếm",
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
});

$('.time-select').on('change', 'input[type=radio][name=rd_TimeReport]', function () {
    if ($(this).val() === "2") {
        $('#filterDate').removeAttr('disabled');
        $('.choose-date-show').attr('disabled', "disabled");
    }
    else {
        $('#filterDate').attr('disabled', "disabled");
        $('.choose-date-show').removeAttr('disabled');
    }
})

function HideLostFocust() {
    $('.divSearchVue').delay(300).hide(0, function () {
    });
}

var arrIDCheck = [];

function SetCheckAll(obj) {
    var isChecked = $(obj).is(":checked");
    $('.check-group input[type=checkbox]').each(function () {
        $(this).prop('checked', isChecked);
    })
    if (isChecked) {
        $('.check-group input[type=checkbox]').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined && !(jQuery.inArray(thisID, arrIDCheck) > -1)) {
                arrIDCheck.push(thisID);
            }
        });
    }
    else {
        $('.check-group input[type=checkbox]').each(function () {
            var thisID = $(this).attr('id');
            for (var i = 0; i < arrIDCheck.length; i++) {
                if (arrIDCheck[i] === thisID) {
                    arrIDCheck.splice(i, 1);
                    break;
                }
            }
        })
    }

    if (arrIDCheck.length > 0) {
        $('#divThaoTac').show();
        $('.choose-commodity').show().trigger("RemoveClassForButtonNew");
        $('#count').text(arrIDCheck.length);
    }
    else {
        $('#divThaoTac').hide();
        $('.choose-commodity').hide().trigger("addClassForButtonNew");
    }
}

function ChoseDoiTuong(obj) {
    var thisID = $(obj).attr('id');

    if ($(obj).is(':checked')) {
        if ($.inArray(thisID, arrIDCheck) === -1) {
            arrIDCheck.push(thisID);
        }
    }
    else {
        //remove item in arrID
        arrIDCheck = arrIDCheck.filter(x => x !== thisID);
    }

    if (arrIDCheck.length > 0) {
        $('#divThaoTac').show();
        $('.choose-commodity').show().trigger("RemoveClassForButtonNew");
        $('#count').text(arrIDCheck.length);
    }
    else {
        $('#divThaoTac').hide();
        $('.choose-commodity').hide().trigger("addClassForButtonNew");
    }

    // count input is checked
    var countCheck = 0;
    $('#tblList tr td.check-group input').each(function (x) {
        var id = $(this).attr('id');
        if ($.inArray(id, arrIDCheck) > -1) {
            countCheck += 1;
        }
    });

    // set check for header
    var ckHeader = $('#tblList thead tr th:eq(0) input');
    var lenList = $('#tblList tbody tr.prev-tr-hide').length;
    if (countCheck === lenList) {
        ckHeader.prop('checked', true);
    }
    else {
        ckHeader.prop('checked', false);
    }
}
function RemoveAllCheck() {
    $('input[type=checkbox]').prop('checked', false);
    arrIDCheck = [];
    $('#divThaoTac').hide();
    $('.choose-commodity').hide();
}
