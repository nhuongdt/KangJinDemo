var ViewModelKH = function () {
    var self = this;
    var DMDoiTuongUri = '/api/DanhMuc/DM_DoiTuongAPI/';
    var DMNguonKhachUri = '/api/DanhMuc/DM_NguonKhachAPI/';
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';
    var Quy_HoaDonUri = '/api/DanhMuc/Quy_HoaDonAPI/';
    var CSKHUri = '/api/DanhMuc/ChamSocKhachHangAPI/';
    var ThietLapAPI = '/api/DanhMuc/ThietLapApi/';
    $('#lblCode').text('');
    $('#lblPhone').text('');
    var user = $('#txtUserLogin').val(); // get at ViewBag
    var userID = $('#txtUserID').val();
    var idNhanVien = $('.idnhanvien').text();
    var idDonVi = $('#hd_IDdDonVi').val();
    var today = new Date();
    self.index_NVienBanGoi = ko.observable(0);
    self.ListNVien_BanGoi = ko.observableArray();
    self.GridNVienBanGoi_Chosed = ko.observableArray();
    self.DoiTuongs = ko.observableArray();
    self.NhomDoiTuongs = ko.observableArray();
    self.AllNhomDoiTuongs = ko.observableArray();
    self.TinhThanhs = ko.observableArray();
    self.QuanHuyens = ko.observableArray();
    self.AllQuanHuyens = ko.observableArray();
    self.NguonKhachs = ko.observableArray();
    self.NhanViens = ko.observableArray();
    self.NhanVienQuanLy = ko.observableArray();// only get NV in PhongBan/HeThong (if have role)
    self.CongTy = ko.observableArray();
    self.VungMiens = ko.observableArray();
    self.error = ko.observable();
    // anh khach hang
    self.FileImgs = ko.observableArray();
    self.HaveImage = ko.observable(false);
    self.AnhDaiDien = ko.observableArray();
    self.ImageIsZoom = ko.observableArray();
    self.filter = ko.observable();
    self.filterNgayTao = ko.observable("0");
    self.filterNgayTao_Quy = ko.observable("0");
    self.filterNgayTao_Input = ko.observable();
    self.filterTongBan = ko.observable("0");
    self.filterDateTongBan_Quy = ko.observable("0");
    self.filterDateTongBan_Input = ko.observable();
    self.selectedNhomDT = ko.observable();
    self.selectedNguonKhach = ko.observable();
    self.selectedNguoiGT = ko.observable();
    self.selectedNguoiQL = ko.observable();
    self.selectNhomDT = ko.observable(); // use at modal ThemMoi DoiTuong
    self.selectNhomDT_MoveGroup = ko.observable(); // use at modal ChuyenNhom
    self.DoiTuong_Old = ko.observableArray();// get infor old of KH when update
    self.pageSizes = [10, 20, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();
    self.TotalRecord = ko.observable();
    self.PageCount = ko.observable();
    self.NoHienTaiAll = ko.observable();
    self.TongBanAll = ko.observable();
    self.TongBanTruTraHangAll = ko.observable();
    self.TongPhiDichVu = ko.observable(0);
    self.TongTichDiemAll = ko.observable();
    self.ContinueImport = ko.observable(false);
    self.GhiChuQuyHD = ko.observable();
    self.TongThuDieuChinh = ko.observable();
    self.NguoiNopTien = ko.observable();
    self.ID_NguoiNopTien = ko.observable();
    self.ListAllDoiTuong = ko.observableArray();
    self.PageKH_KHDoing = ko.observableArray();// use to bind infor KH of modal in page KhachHang, but not in modalNewKhachHang
    // trang thai tiem nang
    self.TrangThaiKhachHang = ko.observableArray();
    self.TrangThaiKH_chosed = ko.observable();
    // filter input at modal add Customer
    self.IsOpenModalCus = ko.observable(false);
    self.searchProvince = ko.observable();
    self.searchProvinceModal = ko.observable();
    self.customerType = ko.observable('0');
    self.customerSex = ko.observable('0');
    self.customerBirthday = ko.observable('0');
    self.customerBirthday_Quy = ko.observable('0'); // loc ngay sinh theo thang, quy, nam
    self.customerBirthday_Input = ko.observable(); // loc ngay sinh from date --> to date
    self.ProvinceChosed = ko.observableArray();
    self.customerDebit = ko.observable('0');
    self.ManagerChosed = ko.observableArray();
    self.searchManager = ko.observable();
    self.searchNguoiGT = ko.observable();
    self.searchDistrict = ko.observable();
    self.CusStatus = ko.observable('0');

    self.trangthaiTienCoc = ko.observable('0');
    self.SoDuTienCocFrom = ko.observable();
    self.SoDuTienCocTo = ko.observable();

    // dieu chinh Diem
    self.DiemDieuChinh = ko.observable();
    self.ThietLap_TichDiem = ko.observableArray();
    // Lich su 
    self.LichSuBanHang = ko.observableArray();
    self.LichSuDatHang = ko.observableArray();
    self.LichSuTichDiem = ko.observableArray();
    self.CongNoKhachHang = ko.observableArray();
    self.selectIDDoiTuong = ko.observable();
    // paging His
    self.pageSizeHis = ko.observable(5);
    self.currentPageHis = ko.observable(0);
    self.fromitemHis = ko.observable(1);
    self.toitemHis = ko.observable();
    self.TotalRecordHis = ko.observable(0);
    self.PageCountHis = ko.observable();
    // paging His Dat Hang
    self.pageSizeHisDH = ko.observable(5);
    self.currentPageHisDH = ko.observable(0);
    self.fromitemHisDH = ko.observable(1);
    self.toitemHisDH = ko.observable();
    self.TotalRecordHisDH = ko.observable(0);
    self.PageCountHisDH = ko.observable();
    // paging His Tich Diem
    self.pageSizeHisTD = ko.observable(5);
    self.currentPageHisTD = ko.observable(0);
    self.fromitemHisTD = ko.observable(1);
    self.toitemHisTD = ko.observable();
    self.TotalRecordHisTD = ko.observable(0);
    self.PageCountHisTD = ko.observable();
    // paging No KH
    self.pageSizeNoKH = ko.observable(10);
    self.currentPageNoKH = ko.observable(0);
    self.fromitemNoKH = ko.observable(1);
    self.toitemNoKH = ko.observable();
    self.TotalRecordNoKH = ko.observable();
    self.PageCountNoKH = ko.observable();
    // paging usercontact
    self.pageSizeUserContact = ko.observable(5);
    self.currentPageUserContact = ko.observable(0);
    self.fromitemUserContact = ko.observable(1);
    self.toitemUserContact = ko.observable();
    self.TotalRecordUserContact = ko.observable();
    self.PageCountUserContact = ko.observable();
    // paging task work
    self.pageSizeSchedule = ko.observable(5);
    self.currentPageSchedule = ko.observable(0);
    self.fromitemSchedule = ko.observable(1);
    self.toitemSchedule = ko.observable();
    self.TotalRecordSchedule = ko.observable();
    self.PageCountSchedule = ko.observable();
    // Thanh toan No
    self.filterKhoanThuChi = ko.observable();
    self.KhoanThuChis = ko.observableArray();
    self.ID_NVienLapPhieu = ko.observable(idNhanVien);
    self.ThoiGian_ThanhToan = ko.observable();
    self.ThuTuKhach = ko.observable();
    self.NoSau = ko.observable();
    self.SelectPT = ko.observable();
    self.GhiChu_PhieuThu = ko.observable();
    self.TongThanhToan = ko.observable();
    self.CongVaoTK = ko.observable();
    self.ListHDisDebit = ko.observableArray();
    self.InforHDprintf = ko.observableArray();
    // chon nhieu ChiNhanh 
    self.ChiNhanhs = ko.observableArray();
    self.ChiNhanhChosed = ko.observableArray();
    self.selectedChiNhanh = ko.observable();
    // Check Quyen
    self.RoleUpdateImg_Invoice = ko.observable(false);
    self.RoleView_Cus = ko.observable(false);
    self.RoleInsert_Cus = ko.observable(false);
    self.RoleInsert_CusGroup = ko.observable(false);
    self.RoleUpdate_Cus = ko.observable(false);
    self.RoleDelete_Cus = ko.observable(false);
    self.RoleExport_Cus = ko.observable(false);
    self.RoleImport_Cus = ko.observable(false);
    self.RoleView_Vendor = ko.observable(false);
    self.RoleInsert_Vendor = ko.observable(false);
    self.RoleUpdate_Vendor = ko.observable(false);
    self.RoleDelete_Vendor = ko.observable(false);
    self.RoleExport_Vendor = ko.observable(false);
    self.RoleView_Debit = ko.observable(true);
    self.Show_BtnEdit = ko.observable(false);
    self.Show_BtnDelete = ko.observable(false);
    self.Show_BtnDieuChinhNo = ko.observable(false);
    self.Show_BtnThanhToanCongNo = ko.observable(false);
    self.Show_BtnExportDetail = ko.observable(false);
    self.Show_BtnDieuChinhDiem = ko.observable(false);
    self.Show_BtnAddContact = ko.observable(false);
    self.Show_BtnAddWork = ko.observable(false);
    self.Show_BtnUpdateSoQuy = ko.observable(false);
    self.Show_BtnDeleteSoQuy = ko.observable(false);
    self.Allow_ChangeTimeSoQuy = ko.observable(false);
    self.RoleDieuChinhSoDuThe = ko.observable(false);
    self.RoleThemMoiTheGiaTri = ko.observable(false);
    self.RoleXemTongDoanhThu = ko.observable(false);
    self.RoleXemDSTheGiaTri = ko.observable(false);
    self.Allow_ChangeTimeTheGiaTri = ko.observable(false);
    self.ShopCookie = ko.observable($('#shopCookies').val());
    self.IsGara = ko.observable(false);
    self.ListCheckBox = ko.observableArray();
    self.NumberColum_Div2 = ko.observable();
    // filter by column
    self.ComparisonFields = ko.observableArray();
    self.ListAllColumn = ko.observableArray();
    self.ListFilterColumn = ko.observableArray();
    self.ResetColumnSearch = function () {
        // only search if close search advance
        if ($('.tr-filter-head').css('display') === 'none') {
            self.ListFilterColumn([]);
            self.currentPage(0);
            $('.number-search, .search-grid input').val('');
            SearchKhachHang();
        }
    }
    switch (VHeader.IdNganhNgheKinhDoanh.toUpperCase()) {
        case 'C16EDDA0-F6D0-43E1-A469-844FAB143014':
            self.IsGara(true);
            break;
        case 'C1D14B5A-6E81-4893-9F73-E11C63C8E6BC':
            break;
        default:
            break;
    }
    self.selecttedKeyCompare = function (key, type) {
        for (let i = 0; i < self.ListFilterColumn().length; i++) {
            if (self.ListFilterColumn()[i].Key === key) {
                self.ListFilterColumn()[i].type = parseInt(type);
                break;
            }
        }
        SearchKhachHang();
    }
    $('.list-kv ul').on('click', 'li', function () {
        $(".list-kv").each(function () {
            $(this).hide();
        });
        $(this).closest('.list-kv').parent().find('.kv1').find('span').text($(this).text().split(':')[0]);
        var value = $(this).closest('.list-kv').parent().next('.col-md-9').find('input').val();
        if (value !== null && value !== '' && value !== undefined) {
            SearchKhachHang(false);
        }
    });
    function LoadKeySearchColumn() {
        ajaxHelper(DMDoiTuongUri + 'GetListKeyColumKhachHang', 'GET').done(function (data1) {
            self.ComparisonFields(data1.compareFile);
            self.ListAllColumn(data1.keycolumn);
        });
    }
    function AssignValueColumnSearch(obj, isInput) {
        isInput = isInput || false;
        let id = $(obj).data('id');
        let val = $(obj).val().toLowerCase();
        let type = 0;
        let itemCl = $.grep(self.ListAllColumn(), function (x) {
            return x.Key === id;
        });
        if ($(obj).hasClass('number-search')) {
            let valType = $(obj).closest('td').find('.kv1 span').text();
            valType = valType.trim();
            switch (valType) {
                case '=':
                    type = 0;
                    break;
                case '<':
                    type = 1;
                    break;
                case '≤':
                    type = 2;
                    break;
                case '>':
                    type = 3;
                    break;
                case '≥':
                    type = 4;
                    break;
            }
        }
        if (itemCl.length > 0) {
            // find ex
            itemCl[0].Value = val;
            itemCl[0].type = type;
            let ex = $.grep(self.ListFilterColumn(), function (x) {
                return x.Key === id;
            });
            if (ex.length > 0) {
                for (let i = 0; i < self.ListFilterColumn().length; i++) {
                    if (self.ListFilterColumn()[i].Key === id) {
                        if (val === '') {
                            self.ListFilterColumn.splice(i, 1);
                        }
                        else {
                            self.ListFilterColumn()[i].Value = val;
                            self.ListFilterColumn()[i].type = type;
                        }
                        break;
                    }
                }
            }
            else {
                if (val !== '') {
                    console.log(1, itemCl[0])
                    self.ListFilterColumn.push(itemCl[0]);
                }
            }
            if (isInput === false) {
                SearchKhachHang(false, false);
            }
        }
    }
    $(".search-grid").keypress(function (e) {
        if (e.keyCode === 13) {
            self.currentPage(0);
            AssignValueColumnSearch($(this));
        }
    });
    $(".search-grid").focusout(function () {
        if ($(this).data('id') !== 11) {
            self.currentPage(0);
            AssignValueColumnSearch($(this), true);
        }
    });
    //$('.gioitinh select, .tinhthanh select, .quanhuyen select .nguonkhach select, .nguoigioithieu select, .trang').on('change', function () {
    $('.gioitinh select').on('change', function () {
        AssignValueColumnSearch($(this));
    })
    $('.number-search').keyup(function () {
        var val = this.value.replace(/\D/g, '');
        this.value = formatNumber(val);
    });
    $(".number-search").keypress(function (e) {
        if (e.keyCode === 13) {
            self.currentPage(0);
            if ($(this).val() !== '') {
                AssignValueColumnSearch($(this));
            }
        }
    });
    $(".number-search").focusout(function () {
        self.currentPage(0);
        AssignValueColumnSearch($(this));
    });
    function AssignValueColumnSearch_jqAuto(id, val) {
        let itemCl = $.grep(self.ListAllColumn(), function (x) {
            return x.Key === id;
        });
        if (itemCl.length > 0) {
            // find ex
            itemCl[0].Value = val;
            let ex = $.grep(self.ListFilterColumn(), function (x) {
                return x.Key === id;
            });
            if (ex.length > 0) {
                for (let i = 0; i < self.ListFilterColumn().length; i++) {
                    if (self.ListFilterColumn()[i].Key === id) {
                        self.ListFilterColumn()[i].Value = val;
                        break;
                    }
                }
            }
            else {
                self.ListFilterColumn.push(itemCl[0]);
            }
            SearchKhachHang(false, false);
        }
    }
    self.searchNguoiGT.subscribe(function (newVal) {
        AssignValueColumnSearch_jqAuto(13, newVal);
    });
    self.searchDistrict.subscribe(function (newVal) {
        AssignValueColumnSearch_jqAuto(11, newVal);
    });
    //sort
    self.columsort = ko.observable(null);
    self.sort = ko.observable(0);
    self.ListIDNhanVienQuyen = ko.observableArray();
    self.ArrKhachHangCoSDT = ko.observableArray();
    self.PTThanhToan = ko.observableArray([
        { text: 'Tiền mặt', value: 0 },
        { text: 'Thẻ', value: 1 },
    ]);
    var sLoai = 'khách hàng ';
    var Key_Form = "Key_ListCustomer";
    if (loaiDoiTuong == 2) {
        sLoai = 'nhà cung cấp ';
        Key_Form = "Key_ListVendor"
    }
    var style1 = '<a style= \"cursor: pointer\" onclick = \"';
    var style2 = "('";
    var style3 = "')\" >";
    var style4 = '</a>';
    function Page_Load() {
        getListTinhThanh();
        GetDM_NguonKHang();
        GetInforCongTy();
        GetHT_TichDiem();
        getList_VungMien();
        GetAllChiNhanh();
        GetListIDNhanVien_byUserLogin();
        GetDM_TaiKhoanNganHang();
        loadCheckbox();
        LoadTrangThai();
        getListNhanVienNguoiDung();
        GetAllQuy_KhoanThuChi();
        GetCauHinhHeThong();
        getAllNganHang();
        GetAllSMSMau(); // tinhlv
        getAllBrandName();
        LoadKeySearchColumn();
        GetAllQuanHuyen();
        getallLoaiCongViec();
        //GetAllDichVu();
        if (VHeader) {
            if (loaiDoiTuong === 1) {
                vmThemMoiKhach.inforLogin = {
                    ID_NhanVien: VHeader.IdNhanVien,
                    ID_User: VHeader.IdNguoiDung,
                    UserLogin: VHeader.UserLogin,
                    ID_DonVi: VHeader.IdDonVi,
                    TenNhanVien: VHeader.TenNhanVien,
                };
                vmThemMoiNhomKhach.inforLogin = vmThemMoiKhach.inforLogin;
                vmTrangThaiKhach.inforLogin = vmThemMoiKhach.inforLogin;
                vmNguonKhach.inforLogin = vmThemMoiKhach.inforLogin;
            }
            else {
                vmThemMoiNCC.inforLogin = {
                    ID_NhanVien: VHeader.IdNhanVien,
                    ID_User: VHeader.IdNguoiDung,
                    UserLogin: VHeader.UserLogin,
                    ID_DonVi: VHeader.IdDonVi,
                    TenNhanVien: VHeader.TenNhanVien,
                };
                vmThemMoiNhomNCC.inforLogin = vmThemMoiNCC.inforLogin;
            }
        }
    }
    function CheckRole_VHeader(maquyen) {
        var role = $.grep(VHeader.Quyen, function (x) {
            return x === maquyen;
        });
        return role.length > 0;
    }
    function SetDefault_HideColumn() {
        var arrHideColumn = [];
        if (loaiDoiTuong === 1) {
            arrHideColumn = ['tinhthanh', 'quanhuyen', 'gioitinh', 'email', 'nguoigioithieu',
                'nguoitao', 'nguonkhach', 'trangthaikhachhang', 'ghichu', 'ngaytao', 'sotaikhoan'];
        }
        var cacheHideColumn = localStorage.getItem(Key_Form);
        if (cacheHideColumn == null || cacheHideColumn === '[]') {
            // hide default some column
            for (var i = 0; i < arrHideColumn.length; i++) {
                LocalCaches.AddColumnHidenGrid(Key_Form, arrHideColumn[i], arrHideColumn[i]);
            }
        }
    }
    function loadCheckbox() {
        $.getJSON("api/DanhMuc/BaseApi/GetCheckedStatic?type=" + $('#pageID').val(), function (data) {
            self.ListCheckBox(data);
            self.NumberColum_Div2(Math.ceil(data.length / 2));
        });
    }
    function RoleViewDebit_HideShowColumn() {
        var arrKey = localStorage.getItem(Key_Form);
        if (arrKey !== null) {
            arrKey = JSON.parse(arrKey);
        }
        else {
            arrKey = [];
        }
        // !!! quyen nay check cho xem CongNo (@Thuy)
        if (self.RoleView_Debit() === false) {
            // check if not exist--> add
            let key1 = $.grep(arrKey, function (x) {
                return x.Value === 'nohientai';
            });
            if (key1.length === 0) {
                LocalCaches.AddColumnHidenGrid(Key_Form, 'nohientai', 'nohientai');
            }
            let key2 = $.grep(arrKey, function (x) {
                return x.Value === 'tongban';
            });
            if (key2.length === 0) {
                LocalCaches.AddColumnHidenGrid(Key_Form, 'tongban', 'tongban');
            }
            let key3 = $.grep(arrKey, function (x) {
                return x.Value === 'tongbantrutrahang';
            });
            if (key1.length === 0) {
                LocalCaches.AddColumnHidenGrid(Key_Form, 'tongbantrutrahang', 'tongbantrutrahang');
            }
        }
    }
    function HideShowColumn() {
        SetDefault_HideColumn();
        RoleViewDebit_HideShowColumn();
        loadHtmlGrid();
    }
    function loadHtmlGrid() {
        LocalCaches.LoadFirstColumnGrid(Key_Form, $('#myList ul li input[type = checkbox]'), self.ListCheckBox());
    }
    $('#myList').on('change', 'ul li input[type = checkbox]', function () {
        var valueCheck = $(this).val();
        // valueCheck (1) = class name, valueCheck(2) = value  --> pass to func 
        // add/remove class is hidding in list cache {NameClass, Value}
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
    function GetListIDNhanVien_byUserLogin() {
        ajaxHelper(CSKHUri + 'GetListNhanVienLienQuanByIDLoGin_inDepartment?idnvlogin=' + idNhanVien
            + '&idChiNhanh=' + idDonVi + '&funcName=' + funcName, 'GET').done(function (data) {
                self.ListIDNhanVienQuyen(data);
                GetHT_Quyen_ByNguoiDung();
            })
    }
    function getList_VungMien() {
        ajaxHelper(DMDoiTuongUri + "getList_VungMien", "GET").done(function (data) {
            if (loaiDoiTuong === 1) {
                vmThemMoiNhomKhach.listData.VungMiens = data;
            }
        });
    };
    function getListTinhThanh() {
        ajaxHelper(DMDoiTuongUri + "GetListTinhThanh", 'GET').done(function (x) {
            if (x.res === true) {
                let data = x.data;
                self.TinhThanhs(data);
                var province = data.map(function (p) {
                    return {
                        ID: p.ID,
                        val2: p.TenTinhThanh
                    }
                });
                if (loaiDoiTuong === 2) {
                    vmThemMoiNCC.listData.TinhThanhs = province;
                    vmThemMoiNCC.listData.ListTinhThanhSearch = province;
                }
                newModal_LienHe.listProvince(data);
                newModal_LienHe.UserLogin(user);
            }
        });
    }
    // used to search jqAuto header list Customer
    function GetAllQuanHuyen() {
        ajaxHelper(DMDoiTuongUri + "GetAllQuanHuyen", 'GET').done(function (x) {
            if (x.res === true) {
                self.AllQuanHuyens(x.data);
                newModal_LienHe.listDistrict(x.data);
                vueDistrict.reset();
            }
        });
    }
    function GetNhomDoiTuong_DonVi() {
        ajaxHelper(DMDoiTuongUri + 'GetNhomDoiTuong_DonVi?loaiDT=' + loaiDoiTuong, 'GET').done(function (obj) {
            if (obj.res === true) {
                let data = obj.data;
                for (var i = 0; i < data.length; i++) {
                    let tenNhom = data[i].TenNhomDoiTuong;
                    tenNhom = tenNhom.concat(' ', locdau(tenNhom), ' ', GetChartStart(tenNhom));
                    data[i].Text_Search = tenNhom;
                }
                self.AllNhomDoiTuongs(data);// used to chuyennhom khach

                if (self.ThietLap().QuanLyKhachHangTheoDonVi) {
                    // only get Nhom chua cai dat ChiNhanh or in this ChiNhanh
                    var arrNhom = [];
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].NhomDT_DonVi.length > 0) {
                            let ex = $.grep(data[i].NhomDT_DonVi, function (x) {
                                return x.ID === idDonVi;
                            });
                            if (ex.length) {
                                arrNhom.push(data[i]);
                            }
                        }
                        else {
                            arrNhom.push(data[i]);
                        }
                    }
                    self.NhomDoiTuongs(arrNhom);
                }
                else {
                    self.NhomDoiTuongs(data);
                }
                if (loaiDoiTuong === 1) {
                    vmThemMoiKhach.listData.NhomKhachs = self.NhomDoiTuongs();
                }
                else {
                    vmThemMoiNCC.listData.NhomKhachs = self.NhomDoiTuongs();
                }
            }
            var newObj = {
                ID: const_GuidEmpty,
                TenNhomDoiTuong: 'Nhóm mặc định',
                Text_Search: 'nhom mac dinh nmd',
                NhomDT_DonVi: [],
            }
            self.NhomDoiTuongs.unshift(newObj);
            LoadSearchNhomDT();
        });
    }

    function GetAllQuy_KhoanThuChi() {
        ajaxHelper(Quy_HoaDonUri + 'GetQuy_KhoanThuChi', 'GET').done(function (x) {
            if (x.res === true) {
                vmThemPhieuThuChi.listData.AllKhoanThuChis = x.data;
                vmNapTienDatCoc.listData.AllKhoanThuChis = x.data;
                if (loaiDoiTuong === 1) {
                    vmThanhToan.listData.KhoanThuChis = x.data;
                }
                else {
                    vmThanhToanNCC.listData.KhoanThuChis = x.data;
                }
            }
        })
    }
    function getallLoaiCongViec() {
        ajaxHelper('/api/DanhMuc/DM_LoaiTuVanLichHenAPI/' + 'GetDM_LoaiCongViec', 'GET').done(function (x) {
            if (x.res === true) {
                partialWork.LoaiCongViecs(x.data);
            }
        });
    }
    self.showPopupAddKH = function () {
        vmThemMoiKhach.showModalAdd();
    }
    self.showPopupNCC = function () {
        vmThemMoiNCC.showModalAdd();
    };
    self.editNCC = function (item) {
        item.TenNhomKhachs = item.TenNhomDT;
        vmThemMoiNCC.showModalUpdate(item);
    };
    function GetChartStart(input) {
        if (input === '' || input === null || input.replace(/\s+/g, '') === '') {
            return null;
        }
        else {
            var inputLocdau = locdau(input);
            var result = inputLocdau.split(' ').filter(word => word !== '');
            var output = '';
            $.each(result, function (index, value) {
                output += value.substring(0, 1);
            });
            return output.toLowerCase();
        }
    }
    // click add NhomDT at page Khachhang
    self.ShowModalAddNhomKH = function () {
        vmThemMoiNhomKhach.showModalAdd();
    }
    self.modalDelete = function (item) {
        // khong can check quyen vi da an button roi
        var _id = item.ID;
        var _maDT = item.MaDoiTuong;
        dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn xóa ' + sLoai + ' có mã  <b> ' + _maDT + '</b> không?', function () {
            $.ajax({
                type: "DELETE",
                url: DMDoiTuongUri + "DeleteDM_DoiTuong/" + _id,
                dataType: 'json',
                contentType: 'application/json',
                success: function (result) {
                    Insert_NhatKyThaoTac(item, 1, 3);
                    SearchKhachHang(false, false);
                    ShowMessage_Success("Xóa " + sLoai + " thành công");
                },
                error: function (error) {
                    $('#modalPopuplgDelete').modal('hide');
                    ShowMessage_Danger("Xóa " + sLoai + " thất bại");
                }
            })
        })
    };
    self.editNhomDTuong = function () {
        var nhom = $.grep(self.NhomDoiTuongs(), function (x) {
            return x.ID === self.selectNhomDT();
        });
        if (nhom.length > 0) {
            if (loaiDoiTuong === 1) {
                vmThemMoiNhomKhach.showModalUpdate(nhom[0]);
            }
            else {
                vmThemMoiNhomNCC.showModalUpdate(nhom[0]);
            }
        }
    }
    // add nhom NCC
    self.showpopAddNhomDT = function () {
        vmThemMoiNhomNCC.showModalAdd();
    };
    self.editKH = function (item) {
        item.TenNguoiGioiThieu = item.NguoiGioiThieu;
        item.TenTrangThai = item.TrangThaiKhachHang;
        item.LoaiDoiTuong = 1;
        vmThemMoiKhach.showModalUpdate(item);
    };
    self.RestoreCus = function (item) {
        dialogConfirm('Khôi phục ' + sLoai, 'Bạn có chắc chắn muốn khôi phục '.concat(sLoai, ' <b> ', item.MaDoiTuong + ' </b> không?'), function () {
            ajaxHelper(DMDoiTuongUri + 'RestoreCus/' + item.ID, 'GET').done(function (x) {
                if (x) {
                    ShowMessage_Success('Khôi phục ' + sLoai + ' thành công');
                    let diary = {
                        ID_NhanVien: idNhanVien,
                        ID_DonVi: idDonVi,
                        ChucNang: 'Khôi phục ' + sLoai,
                        NoiDung: 'Khôi phục '.concat(sLoai, ' ', item.TenDoiTuong, ' (', item.MaDoiTuong, ')'),
                        NoiDungChiTiet: 'Khôi phục '.concat(sLoai, ': ', item.TenDoiTuong, ' (', item.MaDoiTuong, ')',
                            '<br/> Người khôi phục: ', user),
                        LoaiNhatKy: 2
                    };
                    Insert_NhatKyThaoTac_1Param(diary);
                }
            })
        })
    }
    self.importFromExcelDoiTuong = function () {
        $("#fileLoader").click();
    }
    function GetDM_NguonKHang() {
        if (navigator.onLine) {
            ajaxHelper(DMNguonKhachUri + 'GetDM_NguonKhach', 'GET').done(function (data) {
                self.NguonKhachs(data);
                if (loaiDoiTuong === 1) {
                    vmThemMoiKhach.listData.NguonKhachs = data;
                }
            });
        }
    }
    // sort when click header
    $('#tblList thead tr').on('click', 'th', function () {
        var id = $(this).attr('id');
        if (id !== undefined) {
            switch (id) {
                case "madoituong":
                    self.columsort("MaDoiTac");
                    break;
                case "tendoituong":
                    self.columsort("MaDoiTac");
                    break;
                case "dienthoai":
                    self.columsort("MaDoiTac");
                    break;
                case "gioitinh":
                    self.columsort("TenDoiTac");
                    break;
                case "nhomkhach":
                    self.columsort("NhomDoiTac");
                    break;
                case "ngaysinh":
                    self.columsort("NgaySinh");
                    break;
                case "email":
                    self.columsort("Email");
                    break;
                case "tinhthanh":
                    self.columsort("TinhThanh");
                    break;
                case "quanhuyen":
                    self.columsort("QuanHuyen");
                    break;
                case "diachi":
                    self.columsort("DiaChi");
                    break;
                case "nguoitao":
                    self.columsort("NguoiTao");
                    break;
                case "ngaytao":
                    self.columsort("NgayTao");
                    break;
                case "nguoigioithieu":
                    self.columsort("NguoiGioiThieu");
                    break;
                case "nguonkhach":
                    self.columsort("NguonKhach");
                    break;
                case "nohientai":
                    self.columsort("NoHienTai");
                    break;
                case "tongban":
                    self.columsort("TongBan");
                    break;
                case "tongmua":
                    self.columsort("TongMua");
                    break;
                case "tongbantrutrahang":
                    self.columsort("TongBanTruTraHang");
                    break;
                case "tongtichdiem":
                    self.columsort("TongTichDiem");
                    break;
                case "ngaytao":
                    self.columsort("NgayTao");
                    break;
                case "ngaygiaodichgannhat":
                    self.columsort("NgayGiaoDichGanNhat");
                    break;
                case "cpDichvu":
                    self.columsort("PhiDichVu");
                    break;
                case "napcoc":
                    self.columsort("NapCoc");
                    break;
                case "sudungcoc":
                    self.columsort("SuDungCoc");
                    break;
                case "soducoc":
                    self.columsort("SoDuCoc");
                    break;
            }
            SortGrid(id);
        }
    })
    function SortGrid(item) {
        $("#iconSort").remove();
        if (self.sort() === 1) {
            self.sort(2);
            $('#' + item).append(' ' + "<i id='iconSort' class='fa fa-caret-down' aria-hidden='true'></i>");
        }
        else {
            self.sort(1);
            $('#' + item).append(' ' + "<i id='iconSort' class='fa fa-caret-up' aria-hidden='true'></i>");
        }
        SearchKhachHang(false, false);
    };
    //=============== Search and Paging ===============
    $('.choseNgayTao li').on('click', function () {
        $('#txtNgayTao').val($(this).text());
        self.filterNgayTao_Quy($(this).val());
        ResetSort();
        SearchKhachHang(false, false);
    });
    $('.choseTongBan li').on('click', function () {
        $('#txtTongBan').val($(this).text());
        self.filterDateTongBan_Quy($(this).val());
        ResetSort();
        SearchKhachHang(false, false);
    });
    $('.choseNgaySinh li').on('click', function () {
        $('#txtCusBirthDay').val($(this).text());
        self.customerBirthday_Quy($(this).val());
        ResetSort();
        SearchKhachHang(false, false);
    });
    self.filterNgayTao.subscribe(function (newVal) {
        ResetSort();
        SearchKhachHang(false, false);
    });
    $('#txtTongBanInput').on('apply.daterangepicker', function (e, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        var thisDate = $(this).val();
        self.filterDateTongBan_Input(thisDate);
        ResetSort();
        SearchKhachHang(false, false);
    });
    $('#txtNgayTaoInput').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        var thisDate = $(this).val();
        self.filterNgayTao_Input(thisDate);
        ResetSort();
        SearchKhachHang(false, false);
    });
    $('#txtCusBirthDay_Input').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        var thisDate = $(this).val();
        self.customerBirthday_Input(thisDate);
        ResetSort();
        SearchKhachHang(false, false);
    });
    self.filterTongBan.subscribe(function () {
        ResetSort();
        SearchKhachHang(false, false);
    });
    self.customerType.subscribe(function () {
        ResetSort();
        SearchKhachHang(false, false);
    });
    self.customerSex.subscribe(function () {
        ResetSort();
        SearchKhachHang(false, false);
    });
    self.customerDebit.subscribe(function (newVal) {
        ResetSort();
        // 0.Tat ca , 1. Con no, 2.Het no
        if (newVal === '2') {
            $('#divDebitValue').css('display', 'none');
            $('#txtNoHienTaiFrom').val('');
            $('#txtNoHienTaiTo').val('');
        }
        else {
            $('#divDebitValue').css('display', '');
        }
        SearchKhachHang(false, false);
    });
    self.trangthaiTienCoc.subscribe(function (newVal) {
        ResetSort();
        if (newVal === '2') {
            self.SoDuTienCocFrom(0);
            self.SoDuTienCocTo(0);
        }
        SearchKhachHang(false, false);
    });
    self.customerBirthday.subscribe(function () {
        ResetSort();
        SearchKhachHang(false, false);
    });
    self.CusStatus.subscribe(function () {
        ResetSort();
        SearchKhachHang(false, false);
    });
    self.Click_IconSearch = function () {
        ResetSort();
        self.currentPage(0);
        SearchKhachHang(false, false);
    }
    function SearchKhachHang(firstload, isXuatExcel) {
        firstload = firstload || false;
        isXuatExcel = isXuatExcel || false;
        $('.line-right').height(0).css("margin-top", "0px");
        var idNhomDT = self.selectNhomDT();
        // all nhom
        if (idNhomDT == undefined) {
            idNhomDT = null;
        }
        // nhom mac dinh
        if (idNhomDT === 0) {
            idNhomDT = '00000000-0000-0000-0000-000000000000';
        }
        var KhachHangLoad = localStorage.getItem('FindKhachHang');
        if (KhachHangLoad !== null) {
            $('#txtMaDoiTuong').val(KhachHangLoad);
        }
        var maKH = $('#txtMaDoiTuong').val();
        if (maKH == undefined) {
            maKH = '';
        }
        else {
            maKH = locdau(maKH.trim());
        }
        // tongban 
        var tongBanFrom = $('#txtTongBanFrom').val();
        if (tongBanFrom === '') {
            tongBanFrom = 0;
        }
        else {
            tongBanFrom = formatNumberToInt(tongBanFrom);
        }
        var tongBanTo = $('#txtTongBanTo').val();
        // NCC: tongBanTo == undefined
        if (tongBanTo === undefined) {
            tongBanTo = 0;
        }
        if (tongBanTo !== '') {
            tongBanTo = formatNumberToInt(tongBanTo);
        }
        else {
            tongBanTo = 0;
        }
        // no hien tai
        var debitFrom = $('#txtNoHienTaiFrom').val();
        if (debitFrom === '') {
            debitFrom = null;
        }
        else {
            debitFrom = formatNumberToInt(debitFrom);
        }
        var debitTo = $('#txtNoHienTaiTo').val();
        if (debitTo !== '') {
            debitTo = formatNumberToInt(debitTo);
        }
        else {
            debitTo = null;
        }
        // loai khach
        var loaiKhach = self.customerType();
        var gioiTinh = parseInt(self.customerSex());
        // nguonKhach
        var idNguonKhach = self.selectedNguonKhach();
        if (idNguonKhach == undefined) {
            idNguonKhach = null;
        }
        // tinhThanh
        var arrIDTinhThanh = [];
        for (var i = 0; i < self.ProvinceChosed().length; i++) {
            arrIDTinhThanh.push(self.ProvinceChosed()[i].ID);
        }
        if (arrIDTinhThanh.length === 0) {
            arrIDTinhThanh = null;
        }
        var arrIDManager = [];
        if (self.ManagerChosed().length > 0) {
            for (var i = 0; i < self.ManagerChosed().length; i++) {
                arrIDManager.push(self.ManagerChosed()[i].ID);
            }
        }
        else {
            for (var i = 0; i < self.ListIDNhanVienQuyen().length; i++) {
                if ($.inArray(self.ListIDNhanVienQuyen()[i], arrIDManager) === -1) {
                    arrIDManager.push(self.ListIDNhanVienQuyen()[i]);
                }
            }
        }
        if (arrIDManager.length === 0) {
            arrIDManager = null;
        }
        var _now = new Date();  //current date of week
        var dayStart = '';
        var dayEnd = '';
        var dateSellStart = '';
        var dateSellEnd = '';
        var createDate = localStorage.getItem('KhachHangTaoMoi');
        if (createDate !== null) {
            self.filterNgayTao('1');
            createDate = JSON.parse(createDate);
            self.filterNgayTao_Input(createDate.FromDate + ' - ' + createDate.ToDate);
            localStorage.removeItem('KhachHangTaoMoi');
        }
        if (self.filterNgayTao() === '0') {
            switch (parseInt(self.filterNgayTao_Quy())) {
                case 0:
                    // all
                    //dayStart = '2016-01-01';
                    //dayEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
                    break;
                case 1:
                    // hom nay
                    dayStart = moment(_now).format('YYYY-MM-DD');
                    dayEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
                    break;
                case 2:
                    // hom qua : dayEnd must set infont of dayStart
                    dayEnd = moment(_now).format('YYYY-MM-DD');
                    dayStart = moment(_now).subtract('days', 1).format('YYYY-MM-DD');
                    break;
                case 3:
                    // tuan nay (start: monday, end: sunday)
                    dayStart = moment().startOf('week').add('days', 1).format('YYYY-MM-DD');
                    dayEnd = moment().endOf('week').add('days', 2).format('YYYY-MM-DD');
                    break;
                case 4:
                    // tuan truoc (OK)
                    dayStart = moment().startOf('week').subtract('days', 6).format('YYYY-MM-DD');
                    dayEnd = moment().startOf('week').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 5:
                    // thang nay
                    dayStart = moment().startOf('month').format('YYYY-MM-DD');
                    dayEnd = moment().endOf('month').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 6:
                    // thang truoc
                    dayStart = moment().subtract('months', 1).startOf('month').format('YYYY-MM-DD');
                    dayEnd = moment().subtract('months', 1).endOf('month').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 7:
                    // quy nay
                    dayStart = moment().startOf('quarter').format('YYYY-MM-DD');
                    dayEnd = moment().endOf('quarter').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 8:
                    var prevQuarter = moment().quarter() - 1;
                    if (prevQuarter === 0) {
                        // get quy 4 cua nam truoc 01/10/... -->  31/21/...
                        let prevYear = moment().year() - 1;
                        dayStart = prevYear + '-' + '10-01';
                        dayEnd = moment().year() + '-' + '01-01';
                    }
                    else {
                        dayStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                        dayEnd = moment().quarter(prevQuarter).endOf('quarter').add(1, 'days').format('YYYY-MM-DD');
                    }
                    break;
                case 9:
                    // nam nay
                    dayStart = moment().startOf('year').format('YYYY-MM-DD');
                    dayEnd = moment().endOf('year').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 10:
                    // nam truoc
                    var prevYear = moment().year() - 1;
                    dayStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                    dayEnd = moment().year(prevYear).endOf('year').add('days', 1).format('YYYY-MM-DD');
                    break;
            }
        }
        else {
            var arrDate = self.filterNgayTao_Input().split('-');
            dayStart = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
            dayEnd = moment(arrDate[1], 'DD/MM/YYYY').add('days', 1).format('YYYY-MM-DD');
        }
        if (self.filterTongBan() === '0') {
            switch (parseInt(self.filterDateTongBan_Quy())) {
                case 0:
                    // all
                    //dateSellStart = '2016-01-01';
                    //dateSellEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
                    break;
                case 1:
                    // hom nay
                    dateSellStart = moment(_now).format('YYYY-MM-DD');
                    dateSellEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
                    break;
                case 2:
                    // hom qua
                    dateSellEnd = moment(_now).format('YYYY-MM-DD');
                    dateSellStart = moment(_now).subtract('days', 1).format('YYYY-MM-DD');
                    break;
                case 3:
                    // tuan nay
                    dateSellStart = moment().startOf('week').add('days', 1).format('YYYY-MM -DD');
                    dateSellEnd = moment().endOf('week').add('days', 2).format('YYYY-MM-DD');
                    break;
                case 4:
                    // tuan truoc
                    dateSellStart = moment().startOf('week').subtract('days', 6).format('YYYY-MM-DD');
                    dateSellEnd = moment().startOf('week').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 5:
                    // thang nay
                    dateSellStart = moment().startOf('month').format('YYYY-MM-DD');
                    dateSellEnd = moment().endOf('month').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 6:
                    // thang truoc
                    dateSellStart = moment().subtract('months', 1).startOf('month').format('YYYY-MM-DD');
                    dateSellEnd = moment().subtract('months', 1).endOf('month').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 7:
                    // quy nay
                    dateSellStart = moment().startOf('quarter').format('YYYY-MM-DD');
                    dateSellEnd = moment().endOf('quarter').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 8:
                    var prevQuarter = moment().quarter() - 1;
                    if (prevQuarter === 0) {
                        // get quy 4 cua nam truoc 01/10/... -->  31/21/...
                        let prevYear = moment().year() - 1;
                        dateSellStart = prevYear + '-' + '10-01';
                        dateSellEnd = moment().year() + '-' + '01-01';
                    }
                    else {
                        dateSellStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                        dateSellEnd = moment().quarter(prevQuarter).endOf('quarter').add(1, 'days').format('YYYY-MM-DD');
                    }
                    break;
                case 9:
                    // nam nay
                    dateSellStart = moment().startOf('year').format('YYYY-MM-DD');
                    dateSellEnd = moment().endOf('year').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 10:
                    // nam truoc
                    var prevYear = moment().year() - 1;
                    dateSellStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                    dateSellEnd = moment().year(prevYear).endOf('year').add('days', 1).format('YYYY-MM-DD');
                    break;
            }
        }
        else {
            var arrDate = self.filterDateTongBan_Input().split('-');
            dateSellStart = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
            dateSellEnd = moment(arrDate[1], 'DD/MM/YYYY').add('days', 1).format('YYYY-MM-DD');
        }
        // search customer birthday
        var ngaySinhFrom = '';
        var ngaySinhTo = '';
        var typeNgaySinh = 0; // 0. so sanh ngay/thang; 1. so sanh nam
        var cacheSN = localStorage.getItem('SinhNhatKhachHang');
        if (cacheSN !== null) {
            cacheSN = JSON.parse(cacheSN);
            self.customerBirthday('1');
            self.customerBirthday_Input(cacheSN.FromDate + ' - ' + cacheSN.ToDate);
            localStorage.removeItem('SinhNhatKhachHang');
        }
        if (self.customerBirthday() === '0') {
            switch (parseInt(self.customerBirthday_Quy())) {
                case 0:
                    // all
                    ngaySinhFrom = '1918-01-01';
                    ngaySinhTo = moment(_now).add('days', 1).format('YYYY-MM-DD');
                    break;
                case 1:
                    // hom nay
                    ngaySinhFrom = ngaySinhTo = moment(_now).format('YYYY-MM-DD');
                    break;
                case 2: // hom qua
                    ngaySinhFrom = ngaySinhTo = moment(_now).subtract('day', 1).format('YYYY-MM-DD');
                    break;
                case 3:
                    // tuan nay
                    ngaySinhFrom = moment().startOf('week').add('days', 1).format('YYYY-MM -DD');
                    ngaySinhTo = moment().endOf('week').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 4:
                    // tuan truoc
                    ngaySinhFrom = moment().startOf('week').subtract('days', 6).format('YYYY-MM-DD');
                    ngaySinhTo = moment().startOf('week').format('YYYY-MM-DD');
                    break;
                case 5:
                    // thang nay
                    ngaySinhFrom = moment().startOf('month').format('YYYY-MM -DD');
                    ngaySinhTo = moment().endOf('month').format('YYYY-MM -DD');
                    break;
                case 6:
                    // thang truoc
                    ngaySinhFrom = moment().subtract('months', 1).startOf('month').format('YYYY-MM-DD');
                    ngaySinhTo = moment().subtract('months', 1).endOf('month').format('YYYY-MM-DD');
                    break;
                case 7:
                    // quy nay
                    ngaySinhFrom = moment().startOf('quarter').format('YYYY-MM-DD');
                    ngaySinhTo = moment().endOf('quarter').format('YYYY-MM-DD');
                    break;
                case 8:
                    var prevQuarter = moment().quarter() - 1;
                    if (prevQuarter === 0) {
                        // get quy 4 cua nam truoc 01/10/... -->  31/21/...
                        let prevYear = moment().year() - 1;
                        ngaySinhFrom = prevYear + '-' + '10-01';
                        ngaySinhTo = moment().year() + '-' + '01-01';
                    }
                    else {
                        ngaySinhFrom = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                        ngaySinhTo = moment().quarter(prevQuarter).endOf('quarter').add(1, 'days').format('YYYY-MM-DD');
                    }
                    typeNgaySinh = 1;
                    break;
                case 9:
                    // nam nay
                    ngaySinhFrom = moment().startOf('year').format('YYYY-MM-DD');
                    ngaySinhTo = moment().endOf('year').format('YYYY-MM-DD');
                    typeNgaySinh = 1;
                    break;
                case 10:
                    // nam truoc
                    var prevYear = moment().year() - 1;
                    ngaySinhFrom = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                    ngaySinhTo = moment().year(prevYear).endOf('year').format('YYYY-MM-DD');
                    typeNgaySinh = 1;
                    break;
                    break;
                case 11:
                    // ngay mai
                    ngaySinhFrom = ngaySinhTo = moment(_now).add('day', 1).format('YYYY-MM-DD');
                    break;
                case 12:
                    // tuan toi
                    ngaySinhFrom = moment().startOf('week').add('days', 8).format('YYYY-MM-DD');
                    ngaySinhTo = moment().endOf('week').add('days', 8).format('YYYY-MM-DD');
                    break;
                case 13:// thang toi
                    ngaySinhFrom = moment().add('months', 1).startOf('month').format('YYYY-MM-DD');
                    ngaySinhTo = moment().add('months', 1).endOf('month').format('YYYY-MM-DD');
                    break;
                case 14:
                    // quy toi
                    var nextQuater = (moment().quarter() + 1 === 5) ? 1 : moment().quarter() + 1;
                    ngaySinhFrom = moment().quarter(nextQuater).startOf('quarter').format('YYYY-MM-DD');
                    ngaySinhTo = moment().quarter(nextQuater).endOf('quarter').format('YYYY-MM-DD');
                    break;
            }
        }
        else {
            // chon ngay cu the
            var arrDate = self.customerBirthday_Input().split('-');
            ngaySinhFrom = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
            ngaySinhTo = moment(arrDate[1], 'DD/MM/YYYY').format('YYYY-MM-DD');
        }
        var idTrangThai = self.TrangThaiKH_chosed();
        if (idTrangThai == undefined) {
            idTrangThai = null;
        }
        // remove if value =''
        for (let k = 0; k < self.ListFilterColumn().length; k++) {
            if (self.ListFilterColumn()[k].Value === '') {
                self.ListFilterColumn.splice(k, 1);
            }
        }
        // chinhanh
        let arrDV = [];
        for (var i = 0; i < self.ChiNhanhChosed().length; i++) {
            if ($.inArray(self.ChiNhanhChosed()[i], arrDV) === -1) {
                arrDV.push(self.ChiNhanhChosed()[i].ID);
            }
        }
        if (arrDV.length === 0) {
            arrDV = [idDonVi];
        }

        // trangthaikhach (key = 22)     
        self.ListFilterColumn(self.ListFilterColumn().filter(x => x.Key !== 22));
        switch (parseInt(self.CusStatus())) {
            case 0:
                self.ListFilterColumn.push({ Key: 22, Value: '0', type: 0 })
                break;
            case 1:
                self.ListFilterColumn.push({ Key: 22, Value: '1', type: 0 })
                break;
        }

        // trangthaiTienCoc (key = 24)
        self.ListFilterColumn(self.ListFilterColumn().filter(x => x.Key !== 24));
        switch (parseInt(self.trangthaiTienCoc())) {
            case 1:
                self.ListFilterColumn.push({ Key: 24, Value: '0', type: 1 })
                break;
            case 2:
                self.ListFilterColumn.push({ Key: 24, Value: '0', type: 2 })
                break;
        }

        if (loaiDoiTuong === 1) {
            if ($('#txtSearchDistr').val().trim() === '') {
                self.ListFilterColumn(self.ListFilterColumn().filter(x => x.Key !== 11));
            }
            if ($('#txtSearchNguoiGT').val().trim() === '') {
                self.ListFilterColumn(self.ListFilterColumn().filter(x => x.Key !== 13));
            }
        }

        let typeSort = self.sort();// 0.No sort, 1.ASC, 2.DESC
        if (typeSort === 0) {
            typeSort = 2;
        }
        if (loaiDoiTuong === 2) {
            switch (self.columsort()) {
                case 'NoHienTai':
                    typeSort = typeSort == 1 ? 2 : 1;// nhacungcap sắp xếp ngược với khách hàng
                    break;
            }
        }
        else {
            switch (self.columsort()) {
                case 'NapCoc':
                case 'SoDuCoc':
                case 'SuDungCoc':
                    typeSort = typeSort == 1 ? 2 : 1;
                    break;
            }
        }
        let sortBy = typeSort == 2 ? 'DESC' : 'ASC';
        var Params_GetListKhachHang = {
            ID_DonVis: arrDV,
            LoaiDoiTuong: loaiDoiTuong,
            MaDoiTuong: maKH,
            ID_NhomDoiTuong: idNhomDT,
            NgayTao_TuNgay: dayStart,
            NgayTao_DenNgay: dayEnd,
            TongBan_TuNgay: dateSellStart,
            TongBan_DenNgay: dateSellEnd,
            TongBan_Tu: tongBanFrom,
            TongBan_Den: tongBanTo,
            NoHienTai_Tu: debitFrom,
            NoHienTai_Den: debitTo,
            No_TrangThai: self.customerDebit(),
            GioiTinh: gioiTinh,
            LoaiKhach: loaiKhach, // ca nhan, cong ty
            ID_TinhThanhs: arrIDTinhThanh,
            NgaySinh_TuNgay: ngaySinhFrom,
            NgaySinh_DenNgay: ngaySinhTo,
            LoaiNgaySinh: typeNgaySinh, // 0.Ngay/Thang, 1.Nam
            ID_NguonKhach: idNguonKhach,
            ID_NhanVienQuanLys: arrIDManager,
            TrangThai_SapXep: typeSort,  
            Cot_SapXep: self.columsort(),
            NguoiTao: user,
            ID_TrangThai: idTrangThai,
            ColumnsHide: columnHide,
            CurrentPage: self.currentPage(),
            PageSize: self.pageSize(),
            SearchColumns: self.ListFilterColumn(),
            ColumnSort: self.columsort(),
            SortBy: sortBy
        }
        console.log(1, Params_GetListKhachHang)
        if (isXuatExcel) {
            Params_GetListKhachHang.CurrentPage = 0;
            Params_GetListKhachHang.PageSize = self.TotalRecord();
            var funcName = 'ExportExcel_KhachHang';
            if (loaiDoiTuong === 2) {
                funcName = 'ExportExcel_NhaCungCap';
            }
            ajaxHelper(DMDoiTuongUri + funcName, 'POST', Params_GetListKhachHang).done(function (url) {
                if (url !== "") {
                    self.DownloadFileTeamplateXLS_Export(url);
                }
            })
            var objDiary = {
                ID_NhanVien: idNhanVien,
                ID_DonVi: idDonVi,
                ChucNang: sLoai,
                NoiDung: "Xuất báo cáo danh sách " + sLoai,
                LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
            };
            Insert_NhatKyThaoTac_1Param(objDiary);
        }
        else {
            var hasPermisson = false;
            if (loaiDoiTuong == 2) {
                hasPermisson = self.RoleView_Vendor();
            }
            else {
                hasPermisson = self.RoleView_Cus();
            }
            if (hasPermisson) {
                $('.content-table').gridLoader();
                ajaxHelper(DMDoiTuongUri + 'LoadDanhMuc_KhachHangNhaCungCap', 'POST', Params_GetListKhachHang)
                    .done(function (x) {
                        $('.content-table').gridLoader({ show: false });
                        if (x.res === true) {
                            let data = x.data;
                            self.DoiTuongs(data);
                            if (x.data.length > 0) {
                                self.PageCount(x.TotalPage);
                                self.TotalRecord(x.TotalRow);
                                self.TongBanAll(x.TongBanAll);
                                self.TongBanTruTraHangAll(x.TongBanTruTraHangAll);
                                self.TongTichDiemAll(x.TongTichDiemAll);
                                self.NoHienTaiAll(x.NoHienTaiAll);
                                self.TongPhiDichVu(x.TongPhiDichVu);
                            }
                            else {
                                self.PageCount(0);
                                self.TotalRecord(0);
                            }
                            self.ArrKhachHangCoSDT(data.filter(p => p.DienThoai !== ""));// use send SMS
                            //var lenData = data.length;
                            PageListPaging();
                            HideShowColumn();
                            SetCheck_Input();
                            // count again KH is chosing
                            if (idNhomDT !== const_GuidEmpty && idNhomDT !== null) {
                                var arrIDKH = [];
                                for (var i = 0; i < data.length; i++) {
                                    arrIDKH.push(data[i].ID);
                                }
                                // remove KH not exist in lstSearch
                                arrIDDoiTuong = $.grep(arrIDDoiTuong, function (x) {
                                    return $.inArray(x, arrIDKH) > -1;
                                });
                                $('#count').text(arrIDDoiTuong.length);
                                if (arrIDDoiTuong.length > 0) {
                                    $('#divThaoTac').show();
                                    $('.choose-commodity').css("display", "inline-block").trigger("RemoveClassForButtonNew");
                                    $('#count').text(arrIDDoiTuong.length);
                                }
                                else {
                                    $('#divThaoTac').hide();
                                    $('.choose-commodity').hide().trigger("addClassForButtonNew");
                                }
                            }
                        }
                        var addQuick = localStorage.getItem('InsertQuickly');
                        if (addQuick != null) {
                            addQuick = parseInt(addQuick);
                            if (addQuick == 2) {
                                localStorage.removeItem('InsertQuickly');
                                self.showPopupAddKH();
                            }
                        }
                    });
            }
            else {
                $('.content-table').gridLoader({ show: false });
                ShowMessage_Danger('Không có quyền xem danh sách ' + sLoai);
            }
            localStorage.removeItem('FindKhachHang');
        }
    }
    self.selectNhomDT.subscribe(function (newval) {
        ResetSort();
        SearchKhachHang(false, false);
    });
    self.TrangThaiKH_chosed.subscribe(function (newval) {
        ResetSort();
        SearchKhachHang(false, false);
    });
    $('#txtMaDoiTuong').keypress(function (e) {
        if (e.keyCode == 13) {
            ResetSort();
            self.currentPage(0);
            SearchKhachHang(false, false);
        }
    });
    $('#txtTongBanFrom').keypress(function (e) {
        if (e.keyCode == 13) {
            ResetSort();
            SearchKhachHang(false, false);
        }
    });
    $('#txtTongBanTo').keypress(function (e) {
        if (e.keyCode == 13) {
            ResetSort();
            SearchKhachHang(false, false);
        }
    });
    $('#txtNoHienTaiFrom').keypress(function (e) {
        if (e.keyCode == 13) {
            ResetSort();
            SearchKhachHang(false, false);
        }
    });
    $('#txtNoHienTaiTo').keypress(function (e) {
        if (e.keyCode == 13) {
            self.currentPage(0);
            ResetSort();
            SearchKhachHang(false, false);
        }
    });
    //self.PageResults = ko.computed(function () {
    //    var first = self.currentPage() * self.pageSize();
    //    if (self.DoiTuongs() !== null) {
    //        return self.DoiTuongs().slice(first, first + self.pageSize());
    //    }
    //});
    self.PageList = ko.observableArray();
    function PageListPaging() {
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
            if (self.DoiTuongs() !== null) {
                self.fromitem((self.currentPage() * self.pageSize()) + 1);
                if (((self.currentPage() + 1) * self.pageSize()) > self.DoiTuongs().length) {
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
        }
        else {
            self.fromitem(0);
            self.toitem(0);
        }
        self.PageList(arrPage);
    }
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
        /**/
        self.currentPage(0);
        SearchKhachHang(false, false);
    };
    self.GoToPage = function (page) {
        self.currentPage(page.pageNumber - 1);
        SearchKhachHang(true, false);
    };
    function SetCheck_Input() {
        // find in list and set check
        var countCheck = 0;
        $('#tblList tr td.check-group input').each(function (x) {
            var id = $(this).attr('id');
            if ($.inArray(id, arrIDDoiTuong) > -1) {
                $(this).prop('checked', true);
                countCheck += 1;
            }
            else {
                $(this).prop('checked', false);
            }
        });
        // set again check header
        var ckHeader = $('#tblList thead tr th:eq(0) input')
        if (countCheck == self.DoiTuongs().length) {
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
        SearchKhachHang(true, false);
    }
    self.BackPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
            SearchKhachHang(true, false);
        }
    }
    self.GoToNextPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.currentPage() + 1);
            SearchKhachHang(true, false);
        }
    }
    self.EndPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
            SearchKhachHang(true, false);
        }
    }
    self.DownloadFileTeamplateXLS_Export = function (pathFile) {
        var url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + pathFile;
        window.location.href = url;
    }
    var columnHide = '';
    function GetColumHide() {
        var cacheHideColumn2 = localStorage.getItem(Key_Form);
        if (cacheHideColumn2 !== null) {
            cacheHideColumn2 = JSON.parse(cacheHideColumn2);
            var arrColumn = [];
            columnHide = '';
            var tdClass = $('#tblList thead tr th');
            for (var i = 0; i < cacheHideColumn2.length; i++) {
                var itemFor = cacheHideColumn2[i];
                if (itemFor.Value !== undefined) {
                    $(tdClass).each(function (index) {
                        var className = $(this).attr('class');
                        if (className !== undefined && className.indexOf(itemFor.Value) > -1) {
                            // push if not exist
                            if ($.inArray(itemFor.Value, arrColumn) === -1) {
                                arrColumn.push(itemFor.Value);
                                columnHide += (index - 1) + '_';
                            }
                        }
                    })
                }
            }
        }
        console.log(3, columnHide)
    }
    self.ExportExcel_KhachHang = function () {
        GetColumHide();
        SearchKhachHang(false, true);
    }
    self.ColumnsExcel_KhachHang = ko.observableArray();
    self.ColumnsExcel_KhachHang.push("4");
    self.ColumnsExcel_KhachHang.push("5");
    self.ColumnsExcel_KhachHang.push("8");
    self.addColum_KhachHang = function (item) {
        if (self.ColumnsExcel_KhachHang().length < 1) {
            self.ColumnsExcel_KhachHang.push(item);
        }
        else {
            for (var i = 0; i < self.ColumnsExcel_KhachHang().length; i++) {
                if (self.ColumnsExcel_KhachHang()[i] === item) {
                    self.ColumnsExcel_KhachHang.splice(i, 1);
                    break;
                }
                if (i == self.ColumnsExcel_KhachHang().length - 1) {
                    self.ColumnsExcel_KhachHang.push(item);
                    break;
                }
            }
        }
        self.ColumnsExcel_KhachHang.sort();
    }
    //===============================
    //xuất excel nhà cung cấp
    self.ExportExcel_NhaCungCap = function () {
        GetColumHide();
        SearchKhachHang(false, true);
    }
    self.ColumnsExcel_NhaCungCap = ko.observableArray();
    self.addColum_NhaCungCap = function (item) {
        if (self.ColumnsExcel_NhaCungCap().length < 1) {
            self.ColumnsExcel_NhaCungCap.push(item);
        }
        else {
            for (var i = 0; i < self.ColumnsExcel_NhaCungCap().length; i++) {
                if (self.ColumnsExcel_NhaCungCap()[i] === item) {
                    self.ColumnsExcel_NhaCungCap.splice(i, 1);
                    break;
                }
                if (i == self.ColumnsExcel_NhaCungCap().length - 1) {
                    self.ColumnsExcel_NhaCungCap.push(item);
                    break;
                }
            }
        }
        self.ColumnsExcel_NhaCungCap.sort();
        console.log(self.ColumnsExcel_NhaCungCap());
    }
    self.ShowandHide = function () {
        $('.BangBaoLoi').gridLoader();
        $('#imageUploadFormKH').val('');
        self.insertArticleNews();
    }
    self.ShowandHideNCC = function () {
        self.ImportNhaCungCap();
    }
    self.loiExcel = ko.observableArray();
    self.ImportFile_IndexErr = ko.observableArray();
    $(".BangBaoLoi").hide();
    self.insertArticleNews = function () {
        //hidewait('NoteImport');
        $('.NoteImport').gridLoader();
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadFormKH").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadFormKH").files[i];
            formData.append("imageUploadFormKH", file);
        }
        $.ajax({
            type: "POST",
            url: DMDoiTuongUri + "ImportExcelToKhachHang?ID_NhanVien=" + idNhanVien + "&ID_DonVi=" + idDonVi,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (x) {
                if (x.res === false) {
                    if (x.mes == "") {
                        self.loiExcel(x.data);
                        self.visibleImport(true);
                        let arrIndex = x.data.map(function (xx) { return xx.rowError });
                        arrIndex = arrIndex.filter((x, i, a) => a.indexOf(x) == i);
                        self.ImportFile_IndexErr(arrIndex);
                        $(".BangBaoLoi").show();
                        $(".NoteImport").hide();
                        $(".filterFileSelect").hide();
                        $(".btnImportExcel").hide();
                    }
                    else {
                        ShowMessage_Danger(x.mes);
                    }
                }
                else {
                    Insert_NhatKyThaoTac(null, 1, 5);
                    document.getElementById('imageUploadFormKH').value = "";
                    self.visibleImport(true);
                    $(".NoteImport").show();
                    $(".filterFileSelect").hide();
                    $(".btnImportExcel").hide();
                    $(".BangBaoLoi").hide();
                    $("#myModalinport").modal("hide");
                    GetNhomDoiTuong_DonVi();
                    SearchKhachHang(false, false);
                    ShowMessage_Success("Import " + sLoai + " thành công");
                }
                $('.BangBaoLoi').gridLoader({ show: false });
                $('.NoteImport').gridLoader({ show: false });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $('.BangBaoLoi').gridLoader({ show: false });
                $('.NoteImport').gridLoader({ show: false });
            },
        });
    }
    self.DoneWithError = function () {
        //hidewait('NoteImport');
        $('.BangBaoLoi').gridLoader();
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadFormKH").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadFormKH").files[i];
            formData.append("imageUploadFormKH", file);
            formData.append("ListErr", self.ImportFile_IndexErr());
        }
        $.ajax({
            type: "POST",
            url: DMDoiTuongUri + "ImportKhachHang_WithError?ID_NhanVien=" + idNhanVien + "&ID_DonVi=" + idDonVi,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (x) {
                $(".NoteImport").show();
                if (x.res) {
                    ShowMessage_Success("Import khách hàng thành công");
                    Insert_NhatKyThaoTac(null, 1, 5);
                    document.getElementById('imageUploadFormKH').value = "";
                    $(".filterFileSelect").hide();
                    $(".btnImportExcel").hide();
                    $(".BangBaoLoi").hide();
                    $("#myModalinport").modal("hide");
                    GetNhomDoiTuong_DonVi();
                    SearchKhachHang(false, false);
                    $('.BangBaoLoi').gridLoader({ show: false });
                }
                else {
                    ShowMessage_Danger("Import khách hàng thất bại");
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $('.BangBaoLoi').gridLoader({ show: false });
            },
        });
    }
    self.DownloadFileTeamplateXLS = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_KhachHang.xls";
        window.open(url)
    }
    //Download file teamplate excel format (*.xlsx)
    self.DownloadFileTeamplateXLSX = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_KhachHang.xlsx";
        window.open(url)
    }
    self.fileNameExcel = ko.observable("Lựa chọn file Excel...");
    self.visibleImport = ko.observable(true);
    self.notvisibleImport = ko.computed(function () {
        return !self.visibleImport();
    });
    self.refreshFileSelect = function () {
        self.visibleImport(true);
        self.fileNameExcel("Lựa chọn file Excel...");
        $(".filterFileSelect").hide();
        $(".btnImportExcel").hide();
        document.getElementById('imageUploadFormKH').value = "";
    }
    self.refreshFileSelectNCC = function () {
        self.visibleImport(true);
        self.fileNameExcel("Lựa chọn file Excel...");
        $(".filterFileSelect").hide();
        $(".btnImportExcel").hide();
        document.getElementById('imageUploadFormNCC').value = "";
    }
    $(".filterFileSelect").hide();
    $(".btnImportExcel").hide();
    self.SelectedFileImport = function (vm, evt) {
        self.fileNameExcel(evt.target.files[0].name)
        $(".filterFileSelect").show();
        $(".btnImportExcel").show();
        $(".NoteImport").show();
        $(".BangBaoLoi").hide();
        self.visibleImport(false);
    }
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
    // import nhà cung cấp
    self.ImportNhaCungCap = function () {
        $('.NoteImport').gridLoader();
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadFormNCC").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadFormNCC").files[i];
            formData.append("imageUploadFormNCC", file);
        }
        $.ajax({
            type: "POST",
            url: DMDoiTuongUri + "ImportExcelToNhaCungCap?ID_NhanVien=" + idNhanVien + "&ID_DonVi=" + idDonVi,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (x) {
                if (x.res === false) {
                    if (x.mes == "") {
                        self.visibleImport(true);
                        self.loiExcel(x.data);
                        var arrIndex = $.unique(x.data.map(function (xx) { return xx.rowError }));
                        self.ImportFile_IndexErr(arrIndex);
                        $(".BangBaoLoi").show();
                        $(".NoteImport").hide();
                        $(".filterFileSelect").hide();
                        $(".btnImportExcel").hide();
                    }
                    else {
                        ShowMessage_Success(x.mes);
                    }
                }
                else {
                    ShowMessage_Success("Import nhà cung cấp thành công");
                    document.getElementById('imageUploadFormNCC').value = "";
                    self.visibleImport(true);
                    $(".NoteImport").show();
                    $(".filterFileSelect").hide();
                    $(".btnImportExcel").hide();
                    $(".BangBaoLoi").hide();
                    $("#myModalinport").modal("hide");
                    Insert_NhatKyThaoTac(null, 1, 5);
                    GetNhomDoiTuong_DonVi();
                    SearchKhachHang(false, false);
                }
                $('.NoteImport').gridLoader({ show: false });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $('.NoteImport').gridLoader({ show: false });
                ShowMessage_Danger('Import nhà cung cấp thất bại');
            },
        });
    }
    self.ImportNCCWithError = function () {
        $('.NoteImport').gridLoader();
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadFormNCC").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadFormNCC").files[i];
            formData.append("imageUploadFormNCC", file);
            formData.append("ListErr", self.ImportFile_IndexErr());
        }
        $.ajax({
            type: "POST",
            url: DMDoiTuongUri + "ImportNhaCungCap_WithError?ID_NhanVien=" + idNhanVien + "&ID_DonVi=" + idDonVi,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (x) {
                $('.NoteImport').gridLoader({ show: false });
                if (x.res) {
                    ShowMessage_Success("Import nhà cung cấp thành công");
                    document.getElementById('imageUploadFormNCC').value = "";
                    $(".NoteImport").show();
                    $(".filterFileSelect").hide();
                    $(".btnImportExcel").hide();
                    $(".BangBaoLoi").hide();
                    $("#myModalinport").modal("hide");
                    GetNhomDoiTuong_DonVi();
                    SearchKhachHang(false, false);
                }
                else {
                    ShowMessage_Danger('Import nhà cung cấp thất bại');
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                ShowMessage_Danger('Import nhà cung cấp thất bại ');
                $('.NoteImport').gridLoader({ show: false });
            },
        });
    }
    self.DownloadFileNhaCungCapXLS = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_NhaCungCap.xls";
        window.open(url)
    }
    //Download file teamplate excel format (*.xlsx)
    self.DownloadFileNhaCungCapXLSX = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_NhaCungCap.xlsx";
        window.open(url)
    }
    self.listUserContact = ko.observableArray();
    function getUserContact(idDoiTuong) {
        var sWhere = " DM_LienHe.TrangThai !='0' AND DM_LienHe.ID_DoiTuong like '%25" + idDoiTuong + "%25'";
        ajaxHelper('/api/DanhMuc/DM_LienHeAPI/' + 'GetAllUserContact_byWhere_FilterNhanVien?txtSearch=' + sWhere, 'GET').done(function (result) {
            if (result !== null) {
                self.listUserContact(result);
                var lenData = result.length;
                self.TotalRecordUserContact(lenData);
                self.PageCountUserContact(Math.ceil(lenData / self.pageSizeUserContact()));
                partialWork.listUserContact(result);
            }
        });
    }

    self.customerOld = ko.observable();
    self.GetLichSuBanTraHang = function (item) {
        self.customerOld(item);
        self.IDDT(item.ID);
        // reset tab & set default active tab 0
        var thisObj = $(event.currentTarget);
        var ulTab = thisObj.closest('tr').next().find('.op-object-detail').children('ul');
        ulTab.children('li').removeClass('active');
        ulTab.children('li').eq(0).addClass('active');

        // active tabcontent
        ulTab.next().children('.tab-pane').removeClass('active');
        ulTab.next().children('.tab-pane:eq(0)').addClass('active');
        self.currentPageHis(0);
        self.selectIDDoiTuong(item.ID);
        GetAllHoaDon_ofDoiTuong_andCount();
        self.currentPageHisDH(0);
        GetHDDatHang_ofDoiTuong();
        self.currentPageNoKH(0);
        GetLst_CongNoKH();
        GetImages_DoiTuong(item.ID);
        self.currentPageHisTD(0);
        GetLichSu_TichDiem();
        // slide for img Khachhang
        if (loaiDoiTuong !== 2) {
            $('.bxslider').bxSlider({
                slideWidth: 255,
                minSlides: 1,
                maxSlides: 1,
                slideMargin: 4
            });
        }
        // quyen them moi so quy
        var itemInsertQuy = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen.indexOf('SoQuy_ThemMoi') > -1;
        });
        // update Hoadon
        var itemUpdateHD = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen.indexOf('HoaDon_CapNhat') > -1;
        });
        // Chi co quyen dieu chinh cong no, thanh toan hoa don neu co quyen them moi so quy, update hoa don
        if (loaiDoiTuong === 2) {
            // dieu chinh no
            var itemDCNo = $.grep(self.Quyen_NguoiDung(), function (x) {
                return x.MaQuyen.indexOf('NhaCungCap_DieuChinhNo') > -1;
            });
            // co quyen dc no : neu co quyen them so quy
            if (itemInsertQuy.length > 0 && itemDCNo.length > 0) {
                self.Show_BtnDieuChinhNo(true);
            }
            else {
                self.Show_BtnDieuChinhNo(false);
            }
            // co quyen tt no : neu co quyen them so quy
            if (itemInsertQuy.length > 0) {
                self.Show_BtnThanhToanCongNo(true);
            }
            else {
                self.Show_BtnThanhToanCongNo(false);
            }
        }
        else {
            // Dieu chinh diem
            var itemDCDiem = $.grep(self.Quyen_NguoiDung(), function (x) {
                return x.MaQuyen.indexOf('KhachHang_DieuChinhDiem') > -1;
            });
            if (self.RoleView_Cus() && itemDCDiem.length > 0) {
                self.Show_BtnDieuChinhDiem(true);
            }
            else {
                self.Show_BtnDieuChinhDiem(false);
            }
            // dieu chinh no
            var itemDCNo = $.grep(self.Quyen_NguoiDung(), function (x) {
                return x.MaQuyen.indexOf('KhachHang_DieuChinhNo') > -1;
            });
            // co quyen dc no : neu co quyen them so quy
            if (itemInsertQuy.length > 0 && itemDCNo.length > 0) {
                self.Show_BtnDieuChinhNo(true);
            }
            else {
                self.Show_BtnDieuChinhNo(false);
            }
            // Thanh toan no
            if (CheckQuyenExist('KhachHang_ThanhToanNo')) {
                self.Show_BtnThanhToanCongNo(true);
            }
            else {
                self.Show_BtnThanhToanCongNo(false);
            }
        }
        // check role insert work, usercontact
        var itemViewUserContact = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen.indexOf('LienHe_XemDS') > -1;
        });
        if (itemViewUserContact.length > 0) {
            ulTab.children('li').eq(5).show();
        }
        else {
            ulTab.children('li').eq(5).hide();
        }
        var itemViewWork = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen.indexOf('CongViec_XemDS') > -1;
        });
        if (itemViewWork.length > 0) {
            ulTab.children('li').eq(6).show();
        }
        else {
            ulTab.children('li').eq(6).hide();
        }
    }
    self.NhanViens_IsNguoiDung = ko.observableArray();
    self.LoadTab_UserContact = function (item) {
        getUserContact(item.ID);
        self.ID_DoiTuong_Doing(item.ID);
        // role insert user contact
        var itemInsertUserContact = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen.indexOf('LienHe_ThemMoi') > -1;
        });
        if (itemInsertUserContact.length > 0) {
            self.Show_BtnAddContact(true);
        }
        else {
            self.Show_BtnAddContact(false);
        }
    }
    // get list NhanVien da co Tai khoan dang nhap (de xem cong viec)
    function GetListNhanVien_IsNguoiDung() {
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetNS_NhanVien_DaTaoND?idDonVi=" + idDonVi, 'GET').done(function (data) {
            if (data !== null) {
                self.NhanViens_IsNguoiDung(data);
                partialWork.NhanViens(data);
            }
        });
    }
    self.GetLichSu_DatHang = function (item) {
        self.currentPageHis(0);
        self.selectIDDoiTuong(item.ID);
        GetHDDatHang_ofDoiTuong(false);
    }
    self.PageResult_LichSuBanHang = ko.computed(function () {
        var first = self.currentPageHis() * self.pageSizeHis();
        if (self.LichSuBanHang() !== null) {
            // sort --> paging
            return self.LichSuBanHang().slice(first, first + self.pageSizeHis());
        }
        return null;
    });
    self.GoToPageHis = function (item) {
        self.currentPageHis(item.pageNumber - 1);
    }
    function GetAllHoaDon_ofDoiTuong_andCount() {
        let ids = GetIDDonVis_Chosed();
        ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'GetAllHoaDon_ofDoiTuong?idDoiTuong=' + self.selectIDDoiTuong() + '&idChiNhanh=' + ids, 'GET').done(function (data) {
            if (data !== null) {
                self.LichSuBanHang(data);
                var lenData = data.length;
                self.TotalRecordHis(lenData);
                self.PageCountHis(Math.ceil(lenData / self.pageSizeHis()));
            }
        });
    }
    self.PageResult_LichSuDatHang = ko.computed(function (x) {
        var first = self.currentPageHisDH() * self.pageSizeHisDH();
        if (self.LichSuDatHang() !== null) {
            return self.LichSuDatHang().slice(first, first + self.pageSizeHisDH());
        }
        return null;
    });
    function GetIDDonVis_Chosed() {
        let ids = '';
        if (self.ChiNhanhChosed().length === 0) {
            ids = idDonVi;
        }
        else {
            for (let i = 0; i < self.ChiNhanhChosed().length; i++) {
                ids += self.ChiNhanhChosed()[i].ID + ',';
            }
        }
        ids = Remove_LastComma(ids);
        return ids;
    }
    function GetHDDatHang_ofDoiTuong() {
        let ids = GetIDDonVis_Chosed();
        ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'GetHDDatHang_ofDoiTuong_andCount?idDoiTuong=' + self.selectIDDoiTuong() + '&idChiNhanh=' + ids, 'GET').done(function (data) {
            if (data !== null) {
                var lenData = data.length;
                self.LichSuDatHang(data);
                self.TotalRecordHisDH(lenData);
                self.PageCountHisDH(Math.ceil(lenData / self.pageSizeHisDH()));
            }
        });
    }
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
    self.PageResult_LichSuTichDiem = ko.computed(function (x) {
        var first = self.currentPageHisTD() * self.pageSizeHisTD();
        if (self.LichSuTichDiem() !== null) {
            return self.LichSuTichDiem().slice(first, first + self.pageSizeHis());
        }
        return null;
    })
    function GetLichSu_TichDiem() {
        ajaxHelper(DMDoiTuongUri + 'GetLichSu_TichDiem?idDoiTuong=' + self.selectIDDoiTuong(), 'GET').done(function (data) {
            self.LichSuTichDiem([]);
            self.TotalRecordHisTD(0);
            self.TotalRecordHisTD(0);
            if (data !== null) {
                var lenData = data.length;
                self.LichSuTichDiem(data);
                self.TotalRecordHisTD(lenData);
                self.PageCountHisTD(Math.ceil(lenData / self.pageSizeHisTD()));
            }
        });
    }
    self.VisibleHisSale = ko.computed(function () {
        if (self.LichSuBanHang() === null) {
            return false;
        }
        else {
            if (self.RoleView_Debit() === false) {
                return false;
            }
            return self.LichSuBanHang().length > 0;
        }
    });
    self.VisibleHisOrder = ko.computed(function () {
        if (self.LichSuDatHang() === null) {
            return false;
        }
        else {
            if (self.RoleView_Debit() === false) {
                return false;
            }
            return self.LichSuDatHang().length > 0;
        }
    });
    self.VisibleListDebit = ko.computed(function () {
        return self.Show_BtnDieuChinhNo() || self.Show_BtnThanhToanCongNo();
    });
    // paging lich su ban hang
    self.PageListHistory = ko.computed(function () {
        var arrPage = [];
        var allPage = self.PageCountHis();
        var currentPageHis = self.currentPageHis();
        if (allPage > 4) {
            var i = 0;
            if (currentPageHis === 0) {
                i = parseInt(self.currentPageHis()) + 1;
            }
            else {
                i = self.currentPageHis();
            }
            if (allPage >= 5 && currentPageHis > allPage - 5) {
                if (currentPageHis >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumber: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    if (currentPageHis == 1) {
                        for (var j = currentPageHis - 1; (j <= currentPageHis + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                    else {
                        // get currentPageHis - 2 , currentPageHis, currentPageHis + 2
                        for (var j = currentPageHis - 2; (j <= currentPageHis + 2) && j < allPage; j++) {
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
        if (self.LichSuBanHang() !== null) {
            self.fromitemHis((self.currentPageHis() * self.pageSizeHis()) + 1);
            if (((self.currentPageHis() + 1) * self.pageSizeHis()) > self.LichSuBanHang().length) {
                var fromItem = (self.currentPageHis() + 1) * self.pageSizeHis();
                if (fromItem < self.TotalRecordHis()) {
                    self.toitemHis((self.currentPageHis() + 1) * self.pageSizeHis());
                }
                else {
                    self.toitemHis(self.TotalRecordHis());
                }
            } else {
                self.toitemHis((self.currentPageHis() * self.pageSizeHis()) + self.pageSizeHis());
            }
        }
        return arrPage;
    });
    self.VisibleStartPageHis = ko.computed(function () {
        if (self.PageListHistory().length > 0) {
            return self.PageListHistory()[0].pageNumber !== 1;
        }
    });
    self.VisibleEndPageHis = ko.computed(function () {
        if (self.PageListHistory().length > 0) {
            return self.PageListHistory()[self.PageListHistory().length - 1].pageNumber !== self.PageCountHis();
        }
    })
    self.GetClassHis = function (page) {
        return ((page.pageNumber - 1) === self.currentPageHis()) ? "click" : "";
    };
    self.StartPageHis = function () {
        self.currentPageHis(0);
    }
    self.BackPageHis = function () {
        if (self.currentPageHis() > 1) {
            self.currentPageHis(self.currentPageHis() - 1);
        }
    }
    self.GoToNextPageHis = function () {
        if (self.currentPageHis() < self.PageCountHis() - 1) {
            self.currentPageHis(self.currentPageHis() + 1);
        }
    }
    self.EndPageHis = function () {
        if (self.currentPageHis() < self.PageCountHis() - 1) {
            self.currentPageHis(self.PageCountHis() - 1);
        }
    }
    // paging lich su dat hang
    self.PageListHistoryDH = ko.computed(function () {
        var arrPage = [];
        var allPage = self.PageCountHisDH();
        var currentPageHisDH = self.currentPageHisDH();
        if (allPage > 4) {
            var i = 0;
            if (currentPageHisDH === 0) {
                i = parseInt(self.currentPageHisDH()) + 1;
            }
            else {
                i = self.currentPageHisDH();
            }
            if (allPage >= 5 && currentPageHisDH > allPage - 5) {
                if (currentPageHisDH >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumber: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    if (currentPageHisDH == 1) {
                        for (var j = currentPageHisDH - 1; (j <= currentPageHisDH + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                    else {
                        // get currentPageHisDH - 2 , currentPageHisDH, currentPageHisDH + 2
                        for (var j = currentPageHisDH - 2; (j <= currentPageHisDH + 2) && j < allPage; j++) {
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
        if (self.LichSuDatHang() !== null) {
            self.fromitemHisDH((self.currentPageHisDH() * self.pageSizeHisDH()) + 1);
            if (((self.currentPageHisDH() + 1) * self.pageSizeHisDH()) > self.LichSuDatHang().length) {
                var fromItem = (self.currentPageHisDH() + 1) * self.pageSizeHisDH();
                if (fromItem < self.TotalRecordHisDH()) {
                    self.toitemHisDH((self.currentPageHisDH() + 1) * self.pageSizeHisDH());
                }
                else {
                    self.toitemHisDH(self.TotalRecordHisDH());
                }
            } else {
                self.toitemHisDH((self.currentPageHisDH() * self.pageSizeHisDH()) + self.pageSizeHisDH());
            }
        }
        return arrPage;
    });
    self.VisibleStartPageHisDH = ko.computed(function () {
        if (self.PageListHistoryDH().length > 0) {
            return self.PageListHistoryDH()[0].pageNumber !== 1;
        }
    });
    self.VisibleEndPageHisDH = ko.computed(function () {
        if (self.PageListHistoryDH().length > 0) {
            return self.PageListHistoryDH()[self.PageListHistoryDH().length - 1].pageNumber !== self.PageCountHisDH();
        }
    })
    self.GoToPageHisDH = function (page) {
        self.currentPageHisDH(page.pageNumber - 1);
    };
    self.GetClassHisDH = function (page) {
        return ((page.pageNumber - 1) === self.currentPageHisDH()) ? "click" : "";
    };
    self.StartPageHisDH = function () {
        self.currentPageHisDH(0);
    }
    self.BackPageHisDH = function () {
        if (self.currentPageHisDH() > 1) {
            self.currentPageHisDH(self.currentPageHisDH() - 1);
        }
    }
    self.GoToNextPageHisDH = function () {
        if (self.currentPageHisDH() < self.PageCountHisDH() - 1) {
            self.currentPageHisDH(self.currentPageHisDH() + 1);
        }
    }
    self.EndPageHisDH = function () {
        if (self.currentPageHisDH() < self.PageCountHisDH() - 1) {
            self.currentPageHisDH(self.PageCountHisDH() - 1);
            GetHDDatHang_ofDoiTuong();
        }
    }
    // paging no can thu tu khach
    self.PageResult_CongNoKhachHang = ko.computed(function (x) {
        var first = self.currentPageNoKH() * self.pageSizeNoKH();
        if (self.CongNoKhachHang() !== null) {
            return self.CongNoKhachHang().slice(first, first + self.pageSizeNoKH());
        }
        return null;
    })
    function GetLst_CongNoKH() {
        let ids = GetIDDonVis_Chosed();
        ajaxHelper(DMDoiTuongUri + 'GetHoaDon_SoQuy_ofDoiTuong?idDoiTuong=' + self.selectIDDoiTuong() + '&idChiNhanh=' + ids, 'GET').done(function (data) {
            if (data !== null) {
                var lenData = data.length;
                self.CongNoKhachHang(data);
                self.TotalRecordNoKH(lenData);
                self.PageCountNoKH(Math.ceil(lenData / self.pageSizeNoKH()));
            }
        });
    }
    self.PageListNoKH = ko.computed(function () {
        var arrPage = [];
        var allPage = self.PageCountNoKH();
        var currentPageNoKH = self.currentPageNoKH();
        if (allPage > 4) {
            var i = 0;
            if (currentPageNoKH === 0) {
                i = parseInt(self.currentPageNoKH()) + 1;
            }
            else {
                i = self.currentPageNoKH();
            }
            if (allPage >= 5 && currentPageNoKH > allPage - 5) {
                if (currentPageNoKH >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumber: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    if (currentPageNoKH == 1) {
                        for (var j = currentPageNoKH - 1; (j <= currentPageNoKH + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                    else {
                        // get currentPageNoKH - 2 , currentPageNoKH, currentPageNoKH + 2
                        for (var j = currentPageNoKH - 2; (j <= currentPageNoKH + 2) && j < allPage; j++) {
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
        if (self.CongNoKhachHang() !== null) {
            if (self.CongNoKhachHang().length > 0) {
                self.fromitemNoKH((self.currentPageNoKH() * self.pageSizeNoKH()) + 1);
            }
            else {
                self.fromitemNoKH(0);
            }
            if (((self.currentPageNoKH() + 1) * self.pageSizeNoKH()) > self.CongNoKhachHang().length) {
                var fromItem = (self.currentPageNoKH() + 1) * self.pageSizeNoKH();
                if (fromItem < self.TotalRecordNoKH()) {
                    self.toitemNoKH((self.currentPageNoKH() + 1) * self.pageSizeNoKH());
                }
                else {
                    self.toitemNoKH(self.TotalRecordNoKH());
                }
            } else {
                self.toitemNoKH((self.currentPageNoKH() * self.pageSizeNoKH()) + self.pageSizeNoKH());
            }
        }
        return arrPage;
    });
    self.VisibleStartPageNoKH = ko.computed(function () {
        if (self.PageListNoKH().length > 0) {
            return self.PageListNoKH()[0].pageNumber !== 1;
        }
    });
    self.VisibleEndPageNoKH = ko.computed(function () {
        if (self.PageListNoKH().length > 0) {
            return self.PageListNoKH()[self.PageListNoKH().length - 1].pageNumber !== self.PageCountNoKH();
        }
    })
    self.GoToPageNoKH = function (page) {
        self.currentPageNoKH(page.pageNumber - 1);
    };
    self.GetClassNoKH = function (page) {
        return ((page.pageNumber - 1) === self.currentPageNoKH()) ? "click" : "";
    };
    self.StartPageNoKH = function () {
        self.currentPageNoKH(0);
    }
    self.BackPageNoKH = function () {
        if (self.currentPageNoKH() > 1) {
            self.currentPageNoKH(self.currentPageNoKH() - 1);
        }
    }
    self.GoToNextPageNoKH = function () {
        if (self.currentPageNoKH() < self.PageCountNoKH() - 1) {
            self.currentPageNoKH(self.currentPageNoKH() + 1);
        }
    }
    self.EndPageNoKH = function () {
        if (self.currentPageNoKH() < self.PageCountNoKH() - 1) {
            self.currentPageNoKH(self.PageCountNoKH() - 1);
        }
    }
    // paging His TichDiem
    self.PageListHistoryTD = ko.computed(function () {
        var arrPage = [];
        var allPage = self.PageCountHisTD();
        var currentPageHisTD = self.currentPageHisTD();
        if (allPage > 4) {
            var i = 0;
            if (currentPageHisTD === 0) {
                i = parseInt(self.currentPageHisTD()) + 1;
            }
            else {
                i = self.currentPageHisTD();
            }
            if (allPage >= 5 && currentPageHisTD > allPage - 5) {
                if (currentPageHisTD >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumber: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    if (currentPageHisTD == 1) {
                        for (var j = currentPageHisTD - 1; (j <= currentPageHisTD + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                    else {
                        // get currentPageHisTD - 2 , currentPageHisTD, currentPageHisTD + 2
                        for (var j = currentPageHisTD - 2; (j <= currentPageHisTD + 2) && j < allPage; j++) {
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
        if (self.LichSuTichDiem() !== null) {
            if (self.LichSuTichDiem().length > 0) {
                self.fromitemHisTD((self.currentPageHisTD() * self.pageSizeHisTD()) + 1);
            }
            else {
                self.fromitemHisTD(0);
            }
            if (((self.currentPageHisTD() + 1) * self.pageSizeHisTD()) > self.LichSuDatHang().length) {
                var fromItem = (self.currentPageHisTD() + 1) * self.pageSizeHisTD();
                if (fromItem < self.TotalRecordHisTD()) {
                    self.toitemHisTD((self.currentPageHisTD() + 1) * self.pageSizeHisTD());
                }
                else {
                    self.toitemHisTD(self.TotalRecordHisTD());
                }
            } else {
                self.toitemHisTD((self.currentPageHisTD() * self.pageSizeHisTD()) + self.pageSizeHisTD());
            }
        }
        return arrPage;
    });
    self.VisibleStartPageHisTD = ko.computed(function () {
        if (self.PageListHistoryTD().length > 0) {
            return self.PageListHistoryTD()[0].pageNumber !== 1;
        }
    });
    self.VisibleEndPageHisTD = ko.computed(function () {
        if (self.PageListHistoryTD().length > 0) {
            return self.PageListHistoryTD()[self.PageListHistoryTD().length - 1].pageNumber !== self.PageCountHisTD();
        }
    })
    self.GoToPageHisTD = function (page) {
        self.currentPageHisTD(page.pageNumber - 1);
    };
    self.GetClassHisTD = function (page) {
        return ((page.pageNumber - 1) === self.currentPageHisTD()) ? "click" : "";
    };
    self.StartPageHisTD = function () {
        self.currentPageHisTD(0);
    }
    self.BackPageHisTD = function () {
        if (self.currentPageHisTD() > 1) {
            self.currentPageHisTD(self.currentPageHisTD() - 1);
        }
    }
    self.GoToNextPageHisTD = function () {
        if (self.currentPageHisTD() < self.PageCountHisTD() - 1) {
            self.currentPageHisTD(self.currentPageHisTD() + 1);
        }
    }
    self.EndPageHisTD = function () {
        if (self.currentPageHisTD() < self.PageCountHisTD() - 1) {
            self.currentPageHisTD(self.PageCountHisTD() - 1);
        }
    }
    self.ClickDieuChinh = function (item) {
        let obj = {
            ID_DoiTuong: item.ID,
            LoaiDoiTuong: loaiDoiTuong,
            MaNguoiNop: item.MaDoiTuong,
            NguoiNopTien: item.TenDoiTuong,
        }
        vmDieuChinhCongNo.arrChiNhanhChosed = self.ChiNhanhChosed().map(function (x) { return x.ID });
        vmDieuChinhCongNo.showModal(obj);
    }

    self.LoadTab_CongNo = function (item) {
        GetSoDu_TheGiaTri(item.ID);
    }

    self.ListHoaDon_withChiPhi = ko.observableArray();
    self.CPHoaDon_TongChiPhi = ko.observable(0);
    self.CPHoaDon_DaThanhToan = ko.observable(0);
    self.CPHoaDon_ConNo = ko.observable(0);
    self.LoadTab_ChiPhiDichVu = function (item) {
        var param = {
            TextSearch: item.ID,// muontamtruong
        }
        ajaxHelper(DMDoiTuongUri + 'GetChiPhiDichVu_byVendor', 'POST', param).done(function (x) {
            if (x.res) {
                self.ListHoaDon_withChiPhi(x.dataSoure);
            }
            if (self.ListHoaDon_withChiPhi().length > 0) {
                self.CPHoaDon_TongChiPhi(self.ListHoaDon_withChiPhi()[0].SumTongTienHang);
                self.CPHoaDon_DaThanhToan(self.ListHoaDon_withChiPhi()[0].SumDaThanhToan);
                self.CPHoaDon_ConNo(self.ListHoaDon_withChiPhi()[0].SumConNo);
            }
            else {
                self.CPHoaDon_TongChiPhi(0);
                self.CPHoaDon_DaThanhToan(0);
                self.CPHoaDon_ConNo(0);
            }
        })
    }

    self.showPopThanhToan_ChiPhiDV = function (item) {
        vmThanhToan_assignListData();
        vmThanhToanNCC.showModalThanhToan(item, 2);// ds ncc
    }

    function vmThanhToan_assignListData() {
        let idDVs = GetIDDonVis_Chosed();
        if (loaiDoiTuong === 2) {
            vmThanhToanNCC.listData.arrIDDonVi = idDVs.split(',');
        }
        else {
            vmThanhToan.listData.arrIDDonVi = idDVs.split(',');
            vmHoaHongHoaDon.listData.NhanViens = self.NhanViens();
        }
    }

    self.showPopThanhToan = function (item) {
        vmThanhToan_assignListData();
        if (loaiDoiTuong === 2) {
            vmThanhToanNCC.showModalThanhToan(item, 1);
        }
        else {
            vmThanhToan.showModalThanhToan(item, 1);
        }
    }

    self.InHoaDon = function (item) {
        var temp = 'SQPT';
        if (loaiDoiTuong === 2) {
            temp = 'SQPC';
        }
        var itemHDFormat = GetInforHDPrint(item);
        self.InforHDprintf(itemHDFormat);
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=' + temp + '&idDonVi=' + idDonVi,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                var data = result;
                data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                data = data.concat("<script > var item1=[], item4=[], item5= [] ; var item2=[] ;var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(data);
            }
        });
    }
    function GetInforHDPrint(objHD) {
        if (self.CongTy() !== null && self.CongTy().length > 0) {
            objHD.TenCuaHang = self.CongTy()[0].TenCongTy;
            objHD.DiaChiCuaHang = self.CongTy()[0].DiaChi;
            objHD.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
        }
        objHD.DienThoaiChiNhanh = objHD.DienThoaiChiNhanh;
        objHD.ChiNhanhBanHang = $('#_txtTenDonVi').text();
        objHD.MaPhieu = objHD.MaHoaDon;
        objHD.NgayLapHoaDon = moment(objHD.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss');
        objHD.NguoiNopTien = objHD.NguoiNopTien;
        objHD.NguoiNhanTien = objHD.NguoiNopTien;
        objHD.DiaChiKhachHang = self.PageKH_KHDoing().DiaChi;
        objHD.DienThoaiKhachHang = self.PageKH_KHDoing().DienThoai;
        objHD.NoiDungThu = objHD.NoiDungThu;
        var tongThu = formatNumberToInt(objHD.TongTienThu);
        objHD.TienBangChu = DocSo(tongThu);
        objHD.GiaTriPhieu = formatNumber(tongThu);
        return objHD;
    }
    // get infor CongTy --> get logo cong ty
    function GetInforCongTy() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CongTy', 'GET').done(function (data) {
                if (data != null) {
                    self.CongTy(data);
                    if (self.CongTy().length > 0) {
                        let ifCompany = {
                            TenCongTy: self.CongTy()[0].TenCongTy,
                            DiaChiCuaHang: self.CongTy()[0].DiaChi,
                            LogoCuaHang: Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang,
                            TenChiNhanh: VHeader.TenDonVi,
                        };
                        if (loaiDoiTuong === 2) {
                            vmThanhToanNCC.inforCongTy = ifCompany;
                        }
                        else {
                            vmThanhToan.inforCongTy = ifCompany;
                            vmThemMoiTheNap.inforCongTy = ifCompany;
                        }
                    }
                }
            });
        }
    }
    self.formatDateTime = function () {
        $('.datepicker_mask').datetimepicker(
            {
                format: "d/m/Y H:i",
                mask: true,
                timepicker: true,
                scrollMonth: false,
                maxDate: new Date(),
            });
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
    self.ShowModalMoveGroup = function () {
        var roleUpdate = self.RoleUpdate_Cus();
        if (loaiDoiTuong == 2) {
            roleUpdate = self.RoleUpdate_Vendor();
        }
        if (roleUpdate) {
            $('#modalMoveGroup').modal('show');
            $('#hdChuyenNhom').text('Chuyển nhóm ' + sLoai);
            $('#lblChuyenNhom').text('Nhóm ' + sLoai);
            $('#txtSearchNhomDT').val('');
            self.selectNhomDT_MoveGroup(undefined);
        }
        else {
            ShowMessage_Danger("Không có quyền chuyển nhóm " + sLoai);
        }
    }
    function containsAll(needles, haystack) {
        for (var i = 0, len = needles.length; i < len; i++) {
            if (needles[i] === '' || needles[i] === undefined) continue;
            if (haystack.search(new RegExp(locdau(needles[i]), "i")) < 0) return false;
        }
        return true;
    }
    // search auto Nhom DOiTuong in popup ChuyenNhom
    function LoadSearchNhomDT() {
        var indexNhomDT = -1;
        var model_KHPT = new Vue({
            el: '#divSearchNhomDT',
            data: function () {
                return {
                    query_NhomDT: '',
                    data_kh: self.AllNhomDoiTuongs()
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
                    let keyCode = event.keyCode;
                    switch (keyCode) {
                        case 13:
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
                            break;
                        case 38: // up
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
                            break;
                        case 40:// down
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
                            break;
                        case 13:
                            break;
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
    // vue district
    var indexDistr = -1;
    // search auto Nhom DOiTuong in popup ChuyenNhom
    var vueDistrict = new Vue({
        el: '#divSearchDistr',
        data: function () {
            return {
                query: '',
                data: [],
            }
        },
        methods: {
            reset: function (item) {
                this.data = item;
                this.query = '';
            },
            click: function (item) {
                self.searchDistrict(item.ID);
                $('#txtSearchDistr').val(item.TenQuanHuyen);
                $('#showsearch_Distr').hide();
            },
            submit: function (event) {
                let keyCode = event.keyCode;
                switch (keyCode) {
                    case 13:
                        var result = this.filterDT(this.query);
                        var focus = false;
                        $('#showsearch_Distr ul li').each(function (i) {
                            if ($(this).hasClass('hoverenabled')) {
                                self.searchDistrict(result[i].ID)
                                $('#showsearch_Distr').hide();
                                focus = true;
                            }
                        });
                        if (result.length > 0 && this.query !== '' && focus === false) {
                            self.searchDistrict(result[0].ID);
                            $('#showsearch_Distr').hide();
                        }
                        if (result.length === 0) {
                            AssignValueColumnSearch_jqAuto(11, '');
                        }
                        break;
                    case 38: // up
                        indexDistr = indexDistr - 1;
                        if (indexDistr < 0) {
                            indexDistr = $("#showsearch_Distr ul li").length - 1;
                            $('#showsearch_Distr').stop().animate({
                                scrollTop: $('#showsearch_Distr').offset().top + 500
                            }, 1000);
                        }
                        else if (indexDistr > 0 && indexDistr < 10) {
                            $('#showsearch_Distr').stop().animate({
                                scrollTop: $('#showsearch_Distr').offset().top - 600
                            }, 1000);
                        }
                        this.loadFocus();
                        break;
                    case 40:// down
                        indexDistr = indexDistr + 1;
                        if (indexDistr >= ($("#showsearch_Distr ul li").length)) {
                            indexDistr = 0;
                            $('#showsearch_Distr').stop().animate({
                                scrollTop: $('#showsearch_Distr').offset().top - 600
                            }, 1000);
                        }
                        else if (indexDistr > 9 && indexDistr < $("#showsearch_Distr ul li").length) {
                            $('#showsearch_Distr').stop().animate({
                                scrollTop: $('#showsearch_Distr').offset().top + 500
                            }, 1000);
                        }
                        this.loadFocus();
                        break;
                    case 13:
                        break;
                }
            },
            loadFocus: function () {
                $('#showsearch_Distr ul li').each(function (i) {
                    $(this).removeClass('hoverenabled');
                    if (indexDistr === i) {
                        $(this).addClass('hoverenabled');
                    }
                });
            },
            filterDT: function (value) {
                if (value === '') {
                    return [];
                }
                let txt = locdau(value);
                return self.AllQuanHuyens().filter(function (item) {
                    return locdau(item['TenQuanHuyen']).indexOf(txt) > -1 === true;
                }).slice(0, 20);
            },
        },
        computed: {
            // Return Khách hàng
            searchResult: function () {
                var result = this.filterDT(this.query);
                if (result.length < 1 || this.query === '') {
                    $('#showsearch_Distr').hide();
                }
                else {
                    indexDistr = -1;
                    $('#showsearch_Distr').show();
                }
                $('#showsearch_Distr ul li').each(function (i) {
                    $(this).removeClass('hoverenabled');
                });
                $('#showsearch_Distr').stop().animate({
                    scrollTop: $('#showsearch_Distr').offset().top - 600
                }, 1000);
                return result;
            }
        }
    });
    self.filterNhomDoiTuong = function (item, inputString) {
        var itemSearch = locdau(item.TenNhomDoiTuong);
        var locdauInput = locdau(inputString);
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';
        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }
        return itemSearch.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    }
    self.MoveDoiTuong_toGroup = function () {
        var idNhomNew = self.selectNhomDT_MoveGroup();
        if (arrIDDoiTuong.length == 0) {
            ShowMessage_Danger("Vui lòng chọn " + sLoai + " cần chuyển nhóm");
            return;
        }
        if (idNhomNew === 0 || idNhomNew === undefined) {
            ajaxHelper(DMDoiTuongUri + 'DeleteAllNhom_ofDoiTuong?lstIDDoiTuong=' + arrIDDoiTuong, 'POST', arrIDDoiTuong).done(function (x) {
                console.log('deleteDT in nhom ', x.res)
            });
        }
        else {
            var lst = [];
            for (let i = 0; i < arrIDDoiTuong.length; i++) {
                let obj = {
                    ID_DoiTuong: arrIDDoiTuong[i],
                    ID: idNhomNew,
                };
                lst.push(obj);
            }
            switch (loaiDoiTuong) {
                case 1:
                    vmThemMoiKhach.UpdateNhomKhachHang(lst, true);
                    break;
                case 2:
                    vmThemMoiNCC.UpdateNhomKhachHang(lst, true);
                    break;
            }
        }
        ShowMessage_Success("Cập nhật dữ liệu thành công");
        $('#divThaoTac, .choose-commodity').hide();
        $('#modalMoveGroup').modal('hide');
        RemoveAllCheck();
    }
    self.DeleteManyDoiTuong = function () {
        var roleDelete = false;
        if (loaiDoiTuong === 2) {
            roleDelete = self.RoleDelete_Vendor();
        }
        else {
            roleDelete = self.RoleDelete_Cus();
        }
        if (roleDelete) {
            dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn xóa những ' + sLoai + ' đã chọn không?', function () {
                let sIDs = '';
                let lenArr = arrIDDoiTuong.length;
                for (var i = 0; i < lenArr; i++) {
                    sIDs += arrIDDoiTuong[i] + ',';
                }
                sIDs = Remove_LastComma(sIDs);
                $('#divThaoTac').hide();
                let myData = { IDs: arrIDDoiTuong };
                ajaxHelper(DMDoiTuongUri + 'Delete_ManyDoiTuong?ids=' + sIDs, 'POST', myData).done(function (data) {
                    if (data === '') {
                        ShowMessage_Success("Xóa nhiều " + sLoai + " thành công");
                        let objDiary = {
                            ID_NhanVien: idNhanVien,
                            ID_DonVi: idDonVi,
                            ChucNang: "Khách hàng",
                            NoiDung: "Xóa ".concat(lenArr, ' khách hàng'),
                            NoiDungChiTiet: "Xóa ".concat(lenArr, ' khách hàng'),
                            LoaiNhatKy: 3,
                        }
                        Insert_NhatKyThaoTac_1Param(objDiary);
                    }
                    SearchKhachHang(false, false);
                });
                $('#divThaoTac').hide();
                $('.choose-commodity').hide();
                arrIDDoiTuong = [];
            });
        }
        else {
            ShowMessage_Danger("Không có quyền xóa " + sLoai);
        }
    }
    function GetHT_TichDiem() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CauHinh_TichDiem?idDonVi=' + idDonVi, 'GET').done(function (data) {
                if (data != null) {
                    self.ThietLap_TichDiem(data);
                }
            });
        }
    }
    self.ThietLap = ko.observableArray();
    function GetCauHinhHeThong() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + "GetCauHinhHeThong/" + idDonVi, 'GET').done(function (data) {
                if (data !== null) {
                    self.ThietLap(data);
                    GetNhomDoiTuong_DonVi();// check QuanLyNhom by ChiNhanh
                    if (loaiDoiTuong === 1) {
                        vmThemMoiKhach.QuanLyKhachHangTheoDonVi = data.QuanLyKhachHangTheoDonVi;
                    }
                }
            });
        }
    }

    self.DieuChinhDiem = function (item) {
        self.PageKH_KHDoing(item);
        self.NguoiNopTien(item.TenDoiTuong);
        self.ID_NguoiNopTien(item.ID);
        self.DoiTuong_Old(item);
        self.GhiChuQuyHD('');
        self.NgayDieuChinh();
        self.TongThuDieuChinh('');
        $('#modal_DieuChinhDiem').modal('show');
    }
    self.InsertQuyHoaDon_TichDiem = function () {
        var d = new Date();
        var time = d.getHours() + ":" + d.getMinutes() + ":" + d.getSeconds();
        if (self.DiemDieuChinh() === undefined || self.DiemDieuChinh().indexOf(' ') > -1) {
            ShowMessage_Danger('Vui lòng nhập điểm điều chỉnh');
            return false;
        }
        var loaiHoaDon = 11;
        var diemThanhToan = 0;
        var diemCu = formatNumberToFloat(self.PageKH_KHDoing().TongTichDiem);
        // khach thanh toan = diem --> phieu chi
        if (formatNumberToInt(self.DiemDieuChinh()) < diemCu) {
            loaiHoaDon = 12;
            diemThanhToan = diemCu - formatNumberToInt(self.DiemDieuChinh());
        }
        else {
            // khach mua hang --> + diem
            diemThanhToan = formatNumberToInt(self.DiemDieuChinh()) - diemCu;
        }
        // get date + time
        var ngayLap = moment(self.NgayDieuChinh(), 'DD/MM/YYYY').format("YYYY-MM-DD") + ' ' + time;
        var Quy_HoaDon = {
            NgayLapHoaDon: ngayLap,
            TongTienThu: 0, // khong thu tien
            NguoiNopTien: self.NguoiNopTien(),
            NoiDungThu: self.GhiChuQuyHD(),
            ID_NhanVien: idNhanVien,
            NguoiTao: user,
            ID_DonVi: idDonVi,
            LoaiHoaDon: loaiHoaDon,
            HachToanKinhDoanh: false,
            PhieuDieuChinhCongNo: 0,
        }
        var Quy_HoaDon_ChiTiet = {
            ID_NhanVien: null,
            ID_DoiTuong: self.ID_NguoiNopTien(),
            TienMat: 0,
            TienGui: 0,
            TienThu: 0, // khong thu tien
            DiemThanhToan: diemThanhToan,
            GhiChu: self.GhiChuQuyHD(),
        }
        var myData = {};
        myData.objQuyHoaDon = Quy_HoaDon;
        myData.objCTQuyHoaDon = Quy_HoaDon_ChiTiet;

        ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/PostQuy_HoaDon_DieuChinh', 'POST', myData).done(function (x) {
            if (x.res) {
                ShowMessage_Success("Điều chỉnh điểm thành công");

                var objDiary = {
                    ID_NhanVien: idNhanVien,
                    ID_DonVi: idDonVi,
                    ChucNang: "Điều chỉnh điểm",
                    NoiDung: "Điều chỉnh điểm khách hàng: ".concat(self.DoiTuong_Old().TenDoiTuong, ' (', self.DoiTuong_Old().MaDoiTuong, ')'),
                    NoiDungChiTiet: 'Điểm cũ: '.concat(diemCu, ' , Điểm điều chỉnh: ', self.DiemDieuChinh(), ', Ngày điều chỉnh: ', moment(ngayLap, 'YYYY-MM-DD HH:mm:ss').format('DD/MM/YYYY')),
                    LoaiNhatKy: 2,
                };
                Insert_NhatKyThaoTac_1Param(objDiary);

                GetLichSu_TichDiem();
            }
            else {
                ShowMessage_Danger(x.mess);
            }
            $("#modal_DieuChinhDiem").modal("hide");
        })
    }
    function UpdateDiemKH_toDB(idDoiTuong, diemGiaoDich) {
        ajaxHelper(DMDoiTuongUri + 'HuyHD_UpdateDiem?idDoiTuong=' + idDoiTuong + '&diemGiaoDich=' + diemGiaoDich, 'GET').done(function (err) {
            if (err !== '') {
                ShowMessage_Danger(err);
            }
        });
    }
    self.Export_HisBanHang = function (item) {
        var url = '/api/DanhMuc/BH_HoaDonAPI/' + 'ExportExcel__ChiTietHoaDon?ID_HoaDon=' + item.ID;
        window.location.href = url;
        var objDiary = {
            ID_NhanVien: idNhanVien,
            ID_DonVi: idDonVi,
            ChucNang: "Hóa đơn",
            NoiDung: "Xuất báo cáo hóa đơn chi tiết theo mã: " + item.MaHoaDon,
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        Insert_NhatKyThaoTac_1Param(objDiary);
    }
    $('.datepicker_maskby').datetimepicker(
        {
            format: "d/m/Y",
            //value: moment(time).format('DD/MM/YYYY'),
            mask: true,
            timepicker: false,
        });
    self.Export_HisAllTab = function (item) {
        let ids = GetIDDonVis_Chosed();
        var funcExport = 'ExportExcel_AllHis_ofKH?';
        if (loaiDoiTuong === 2) {
            funcExport = 'ExportExcel_AllHis_ofNCC?';
        }
        ajaxHelper(DMDoiTuongUri + funcExport
            + 'idDoiTuong=' + item.ID + '&maDoiTuog=' + item.MaDoiTuong + '&tenDoiTuong=' + item.TenDoiTuong
            + '&idChiNhanh=' + ids, 'POST').done(function (url) {
                if (url !== "") {
                    self.DownloadFileTeamplateXLS_Export(url);
                }
            })
        var objDiary = {
            ID_NhanVien: idNhanVien,
            ID_DonVi: idDonVi,
            ChucNang: "Danh mục " + sLoai,
            NoiDung: "Xuất báo cáo lịch sử mua, thanh toán, tích điểm của" + sLoai + " : " + item.MaDoiTuong,
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        Insert_NhatKyThaoTac_1Param(objDiary);
    }
    function GetAllChiNhanh() {
        ajaxHelper('/api/DanhMuc/DM_DonViAPI/' + 'GetListDonVi_User?ID_NguoiDung=' + userID, 'GET').done(function (data) {
            if (data !== null && data.length > 0) {
                self.ChiNhanhs(data);
                if (loaiDoiTuong === 1) {
                    vmThemMoiNhomKhach.listData.ChiNhanhs = data;
                    vmThanhToan.listData.ChiNhanhs = data;
                }
                else {
                    vmThanhToanNCC.listData.ChiNhanhs = data;
                }
                var thisCN = $.grep(data, function (x) {
                    return x.ID === idDonVi;
                });
                self.ChiNhanhChosed(thisCN);
                $('#divChiNhanhChosed input').remove();
                // add check after li
                $('#divAllChiNhanh li').each(function () {
                    if ($(this).attr('id') === idDonVi) {
                        $(this).find('i').remove();
                        $(this).append(elmCheck);
                    }
                });
            }
        });
    }
    self.SelectedChiNhanh = function (item) {
        var arrIDPB = [];
        for (var i = 0; i < self.ChiNhanhChosed().length; i++) {
            if ($.inArray(self.ChiNhanhChosed()[i].ID, arrIDPB) === -1) {
                arrIDPB.push(self.ChiNhanhChosed()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDPB) === -1) {
            self.ChiNhanhChosed.push(item);
        }
        $('#divChiNhanhChosed input').remove();
        // add check after li
        $('#divAllChiNhanh li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
                $(this).append(elmCheck);
            }
        });
        self.currentPage(0);
        SearchKhachHang(false, false);
    }
    self.CloseChiNhanh = function (item) {
        self.ChiNhanhChosed.remove(item);
        if (self.ChiNhanhChosed().length === 0) {
            $('#divChiNhanhChosed').append('<input type="text" class="dropdown form-control" placeholder="Chọn chi nhánh">');
        }
        // remove check
        $('#divAllChiNhanh li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        SearchKhachHang(false, false);
    }
    // Check Quyen by ID_User
    self.Quyen_NguoiDung = ko.observableArray();
    function GetHT_Quyen_ByNguoiDung() {
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetHT_NhomNguoiDung?idnguoidung=" + userID + '&iddonvi=' + idDonVi, 'GET').done(function (data) {
            if (data != null) {
                self.Quyen_NguoiDung(data.HT_Quyen_NhomDTO);
                self.Show_BtnUpdateSoQuy(CheckQuyenExist('SoQuy_CapNhat'));
                self.Show_BtnDeleteSoQuy(CheckQuyenExist('SoQuy_Xoa'));
                self.Allow_ChangeTimeSoQuy(CheckQuyenExist('SoQuy_ThayDoiThoiGian'));
                self.RoleDieuChinhSoDuThe(CheckQuyenExist('TheGiaTri_DieuChinhSoDu'));
                self.RoleThemMoiTheGiaTri(CheckQuyenExist('TheGiaTri_ThemMoi'));
                self.Allow_ChangeTimeTheGiaTri(CheckQuyenExist('TheGiaTri_ThayDoiThoiGian'));
                self.RoleXemDSTheGiaTri(CheckQuyenExist('TheGiaTri_XemDS'));
                self.RoleXemTongDoanhThu(CheckQuyenExist('KhachHang_XemTongDoanhThu'));
                self.RoleUpdateImg_Invoice(CheckQuyenExist('HoaDon_CapNhatAnh'));
                self.RoleInsert_CusGroup(CheckQuyenExist('NhomKhachHang_ThemMoi'));

                if (loaiDoiTuong == 2) {
                    HideShowButton_Vendor();
                }
                else {
                    HideShowButton_Customer();
                }
            }
            else {
                ShowMessage_Danger('Không có quyền xem danh sách ' + sLoai);
            }
        });
    }
    function CheckQuyenExist(maquyen) {
        var role = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen === maquyen;
        });
        return role.length > 0;
    }
    function HideShowButton_Customer() {
        vmThemMoiKhach.role.KhachHang.ThemMoi = CheckQuyenExist('KhachHang_ThemMoi');
        vmThemMoiKhach.role.KhachHang.CapNhat = CheckQuyenExist('KhachHang_CapNhat');
        // Xem
        var arrRoleCus = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen.indexOf('KhachHang_') > -1;
        });
        var itemView = $.grep(arrRoleCus, function (x) {
            return x.MaQuyen.indexOf('KhachHang_XemDS') > -1;
        });
        if (itemView.length > 0) {
            $('#tblList').show();
            $('#btnDropdownView').show();
            $('#myList').css('display', '');
            self.RoleView_Cus(true);
            SearchKhachHang(true, false);
        }
        else {
            ShowMessage_Danger('Không có quyền xem danh sách ' + sLoai);
        }
        // Them moi
        var itemInsert = $.grep(arrRoleCus, function (x) {
            return x.MaQuyen.indexOf('KhachHang_ThemMoi') > -1;
        });
        if (self.RoleView_Cus() === true && itemInsert.length > 0) {
            self.RoleInsert_Cus(true);
        }
        // xuat file
        self.RoleExport_Cus(CheckQuyenExist('KhachHang_XuatFile'));
        // import file
        self.RoleImport_Cus(CheckQuyenExist('KhachHang_Import'));
        // update
        var itemUpdate = $.grep(arrRoleCus, function (x) {
            return x.MaQuyen.indexOf('KhachHang_CapNhat') > -1;
        });
        if (self.RoleView_Cus() === true && itemUpdate.length > 0) {
            self.RoleUpdate_Cus(true);
        }
        // xoa
        var itemDelete = $.grep(arrRoleCus, function (x) {
            return x.MaQuyen.indexOf('KhachHang_Xoa') > -1;
        });
        if (self.RoleView_Cus() === true && itemDelete.length > 0) {
            self.RoleDelete_Cus(true);
        }
        // xem congno, tongban,mua
        //var viewDebit = $.grep(arrRoleCus, function (x) {
        //    return x.MaQuyen.indexOf('KhachHang_XemCongNo') > -1; // nho them quyen nay sau todo
        //});
        //self.RoleView_Debit(viewDebit.length > 0);
    }
    function HideShowButton_Vendor() {
        // Xem
        var arrRole = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen.indexOf('NhaCungCap_') > -1;
        });
        var itemView = $.grep(arrRole, function (x) {
            return x.MaQuyen.indexOf('NhaCungCap_XemDS') > -1;
        });
        if (itemView.length > 0) {
            $('#tblList').show();
            $('#btnDropdownView').show();
            $('#myList').css('display', '');
            self.RoleView_Vendor(true);
            SearchKhachHang(false, false);
        }
        else {
            ShowMessage_Danger('Không có quyền xem danh sách ' + sLoai);
        }
        // Them moi
        var itemInsert = $.grep(arrRole, function (x) {
            return x.MaQuyen.indexOf('NhaCungCap_ThemMoi') > -1;
        });
        if (self.RoleView_Vendor() === true && itemInsert.length > 0) {
            self.RoleInsert_Vendor(true);
        }
        // xuat file
        self.RoleExport_Vendor(CheckQuyenExist('NhaCungCap_XuatFile'));
        // import file
        var itemImport = $.grep(arrRole, function (x) {
            return x.MaQuyen.indexOf('NhaCungCap_Import') > -1;
        });
        if (self.RoleView_Vendor() === true && self.RoleInsert_Vendor() && itemImport.length > 0) {
            $('#btnImportVendor').css('display', '');
        }
        // update
        var itemUpdate = $.grep(arrRole, function (x) {
            return x.MaQuyen.indexOf('NhaCungCap_CapNhat') > -1;
        });
        if (self.RoleView_Vendor() === true && itemUpdate.length > 0) {
            self.RoleUpdate_Vendor(true);
        }
        // xoa
        var itemDelete = $.grep(arrRole, function (x) {
            return x.MaQuyen.indexOf('NhaCungCap_Xoa') > -1;
        });
        if (self.RoleView_Vendor() === true && itemDelete.length > 0) {
            self.RoleDelete_Vendor(true);
        }
    }
    function Insert_NhatKyThaoTac(objUsing, chucNang, loaiNhatKy) {
        // chuc nang (1.DoiTuong, 2.NhomDoiTuong, 3.PhieuThu)
        var tenChucNang = '';
        var noiDung = '';
        var noiDungChiTiet = '';
        var txtFirst = '';
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
            tenChucNang = 'Nhà cung cấp ';
        }
        else {
            tenChucNang = 'Khách hàng ';
        }
        switch (chucNang) {
            case 1:
                if (loaiNhatKy < 4) {
                    // them, sua, xoa
                    var maDoiTuong = objUsing.MaDoiTuong.toUpperCase();
                    var ngaySinh = '';
                    var tenNhom = '';
                    var dienThoai = '';
                    var inforOld = '';
                    if (loaiNhatKy === 2) {
                        let maOld = self.DoiTuong_Old().MaDoiTuong.toUpperCase();
                        let nameOld = self.DoiTuong_Old().TenDoiTuong;
                        let dtOld = self.DoiTuong_Old().DienThoai;
                        let nhomOld = self.DoiTuong_Old().ID_NhomDoiTuong;
                        let sInforOld = '';
                        if (maDoiTuong !== maOld) {
                            sInforOld = sInforOld.concat('Mã : ', maOld);
                        }
                        if (locdau(nameOld) !== locdau(objUsing.TenDoiTuong)) {
                            sInforOld = sInforOld.concat(' Tên : ', nameOld);
                        }
                        if (dtOld !== objUsing.DienThoai) {
                            sInforOld = sInforOld.concat(' Điện thoại : ', dtOld == '' ? null : dtOld);
                        }
                        if (sInforOld != '') {
                            inforOld = '<br /> Thông tin cũ: '.concat(sInforOld)
                        }
                    }
                    if (objUsing.NgaySinh_NgayTLap !== null && objUsing.NgaySinh_NgayTLap !== undefined) {
                        ngaySinh = 'Ngày sinh: ' + moment(objUsing.NgaySinh_NgayTLap, 'YYYY-MM-DD HH:mm:ss').format('DD/MM/YYYY') + ', ';
                    }
                    if (objUsing.TenNhomDT !== '') {
                        tenNhom = 'Nhóm: ' + objUsing.TenNhomDT + ', ';
                    }
                    if (objUsing.DienThoai !== null && objUsing.DienThoai !== undefined) {
                        dienThoai = 'Điện thoại: ' + objUsing.DienThoai + ', ';
                    }
                    noiDung = txtFirst.concat(sLoai, maDoiTuong, ', Tên: ', objUsing.TenDoiTuong, ', ', ngaySinh, dienThoai, tenNhom);
                    noiDungChiTiet = txtFirst.concat(sLoai, style1, funcNameKH, style2, maDoiTuong, style3, maDoiTuong, style4, ', tên: ', objUsing.TenDoiTuong, ', ',
                        ngaySinh, dienThoai, tenNhom, inforOld);
                    noiDungChiTiet = Remove_LastComma(noiDungChiTiet);
                    noiDungChiTiet = noiDungChiTiet.concat('<br /> Nhân viên thực hiện: ', user);
                }
                else {
                    // import, export
                    noiDung = txtFirst.concat('danh sách ', sLoai);
                    noiDungChiTiet = noiDung.concat('<br /> Nhân viên thực hiện: ', user)
                }
                break;
            case 2:
                noiDung = txtFirst.concat('Nhóm ', sLoai, objUsing.TenNhomDoiTuong);
                noiDungChiTiet = noiDung.concat('<br /> Nhân viên thực hiện: ', user);
                break;
            case 3:
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
                noiDungChiTiet = txtFirst.concat(tenChucNang, styleMaHD, '<br/ > Giá trị: ', phaiTT, '<br/ > Phương thức thanh toán: ', objUsing.PhuongThucTT, '<br/ > Thời gian: ', ngaylapHD);
                break;
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
    // Them moi nguon khach
    self.selectedNguonKhach = ko.observable();
    self.ShowModal_InsertNguon = function () {
        vmNguonKhach.showModalAdd();
    }
    self.ShowModal_UpdateNguon = function () {
        var nguon = $.grep(self.NguonKhachs(), function (x) {
            return x.ID === self.selectedNguonKhach();
        });
        if (nguon.length > 0) {
            vmNguonKhach.showModalUpdate(nguon[0]);
        }
    }
    self.selectedNguonKhach.subscribe(function (newID) {
        self.selectedNguonKhach(newID);
        ResetSort();
        SearchKhachHang(false, false);
    })
    // filter at menu left
    self.FilterProvince = ko.computed(function () {
        var inputSearch = '';
        if (self.IsOpenModalCus()) {
            inputSearch = self.searchProvinceModal();
        }
        else {
            inputSearch = self.searchProvince();
        }
        var locdauInput = locdau(inputSearch);
        if (inputSearch) {
            return $.grep(self.TinhThanhs(), function (x) {
                var itemSearch = locdau(x.TenTinhThanh);
                return itemSearch.indexOf(locdauInput) > -1;
            });
        }
        else {
            return self.TinhThanhs();
        }
    });
    self.ChoseProvince = function (item) {
        var arr = [];
        for (var i = 0; i < self.ProvinceChosed().length; i++) {
            if ($.inArray(self.ProvinceChosed()[i], arr) === -1) {
                arr.push(self.ProvinceChosed()[i].ID);
            }
        }
        if ($.inArray(item.ID, arr) === -1) {
            self.ProvinceChosed.push(item);
        }
        var arrID_After = [];
        for (var i = 0; i < self.ProvinceChosed().length; i++) {
            arrID_After.push(self.ProvinceChosed()[i].ID)
        }
        // thêm dấu check vào đối tượng được chọn (OK)
        $('#divAllProvince li').each(function () {
            if ($.inArray($(this).attr('id'), arrID_After) > -1) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
            }
            else {
                $(this).find('.fa-check').remove();
            }
        });
        // add class 'choose-person' : overflow, set width li
        $('#divProvinceChosed').addClass('choose-person');
        $('#divProvinceChosed input').remove();
        ResetSort();
        SearchKhachHang(false, false);
    }
    function ResetSort() {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(0);
        self.currentPage(0);
    }
    self.CloseProvince = function (item) {
        self.ProvinceChosed.remove(item);
        if (self.ProvinceChosed().length === 0) {
            $('#divProvinceChosed').
                append('<input type="text" data-bind="valueUpdate: \'afterkeydown\', value: searchProvince" placeholder= "Chọn tỉnh thành" > ');
            self.searchProvince(undefined);
        }
        // remove check
        $('#divAllProvince li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        ResetSort();
        SearchKhachHang(false, false);
    }
    self.FilterManager = ko.computed(function () {
        var inputSearch = self.searchManager();
        var locdauInput = locdau(inputSearch);
        if (inputSearch !== undefined && inputSearch !== '') {
            return $.grep(self.NhanVienQuanLy(), function (x) {
                var itemSearch = locdau(x.TenNhanVien);
                return itemSearch.indexOf(locdauInput) > -1;
            });
        }
        else {
            return self.NhanVienQuanLy();
        }
    });
    self.ChoseManager = function (item) {
        var arr = [];
        for (var i = 0; i < self.ManagerChosed().length; i++) {
            if ($.inArray(self.ManagerChosed()[i], arr) === -1) {
                arr.push(self.ManagerChosed()[i].ID);
            }
        }
        if ($.inArray(item.ID, arr) === -1) {
            self.ManagerChosed.push(item);
        }
        var arrID_After = [];
        for (var i = 0; i < self.ManagerChosed().length; i++) {
            arrID_After.push(self.ManagerChosed()[i].ID)
        }
        // thêm dấu check vào đối tượng được chọn (OK)
        $('#divAllManager li').each(function () {
            if ($.inArray($(this).attr('id'), arrID_After) > -1) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
            }
            else {
                $(this).find('.fa-check').remove();
            }
        });
        // add class 'choose-person' : overflow, set width li
        /* $('#divManagerChosed').addClass('choose-person');*/
        $('#txtSearchManager').val('');
        $('#txtSearchManager').attr('placeholder', '');
        ResetSort();
        SearchKhachHang(false, false);
    }
    self.CloseManager = function (item) {
        self.ManagerChosed.remove(item);
        if (self.ManagerChosed().length === 0) {
            self.searchManager('');
            $('#txtSearchManager').attr('placeholder', 'Chọn quản lí');
        }
        // remove check
        $('#divAllManager li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        ResetSort();
        SearchKhachHang(false, false);
    }
    self.ResetSearchManager = function () {
        self.searchManager('');
    }

    function GetDM_TaiKhoanNganHang() {
        if (navigator.onLine) {
            ajaxHelper(Quy_HoaDonUri + 'GetAllTaiKhoanNganHang_ByDonVi?idDonVi=' + idDonVi, 'GET').done(function (x) {
                if (x.res === true) {
                    vmThemPhieuThuChi.listData.AccountBanks = x.data;
                    vmNapTienDatCoc.listData.AccountBanks = x.data;
                    if (loaiDoiTuong === 2) {
                        vmThanhToanNCC.listData.AccountBanks = x.data;
                    }
                    else {
                        vmThanhToanGara.listData.AccountBanks = x.data;
                        vmThanhToan.listData.AccountBanks = x.data;
                    }
                }
            })
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
            scrollMonth: false,
            maxDate: new Date(),
        });
    });
    $.datetimepicker.setLocale('vi');
    // nguoi lien he
    self.ID_DoiTuong_Doing = ko.observable();
    self.PageResult_UserContact = ko.computed(function (x) {
        var first = self.currentPageUserContact() * self.pageSizeUserContact();
        if (self.listUserContact() !== null) {
            return self.listUserContact().slice(first, first + self.pageSizeUserContact());
        }
        return null;
    })
    self.PageListUserContact = ko.computed(function () {
        var arrPage = [];
        var allPage = self.PageCountUserContact();
        var currentPageUserContact = self.currentPageUserContact();
        if (allPage > 4) {
            var i = 0;
            if (currentPageUserContact === 0) {
                i = parseInt(self.currentPageUserContact()) + 1;
            }
            else {
                i = self.currentPageUserContact();
            }
            if (allPage >= 5 && currentPageUserContact > allPage - 5) {
                if (currentPageUserContact >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumber: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    if (currentPageUserContact == 1) {
                        for (var j = currentPageUserContact - 1; (j <= currentPageUserContact + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                    else {
                        // get currentPageUserContact - 2 , currentPageUserContact, currentPageUserContact + 2
                        for (var j = currentPageUserContact - 2; (j <= currentPageUserContact + 2) && j < allPage; j++) {
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
            if (self.listUserContact().length > 0) {
                self.fromitemUserContact((self.currentPageUserContact() * self.pageSizeUserContact()) + 1);
            }
            else {
                self.fromitemUserContact(0);
            }
            if (((self.currentPageUserContact() + 1) * self.pageSizeUserContact()) > self.listUserContact().length) {
                var fromItem = (self.currentPageUserContact() + 1) * self.pageSizeUserContact();
                if (fromItem < self.TotalRecordUserContact()) {
                    self.toitemUserContact((self.currentPageUserContact() + 1) * self.pageSizeUserContact());
                }
                else {
                    self.toitemUserContact(self.TotalRecordUserContact());
                }
            } else {
                self.toitemUserContact((self.currentPageUserContact() * self.pageSizeUserContact()) + self.pageSizeUserContact());
            }
        }
        return arrPage;
    });
    self.VisibleStartPageUserContact = ko.computed(function () {
        if (self.PageListUserContact().length > 0) {
            return self.PageListUserContact()[0].pageNumber !== 1;
        }
    });
    self.VisibleEndPageUserContact = ko.computed(function () {
        if (self.PageListUserContact().length > 0) {
            return self.PageListUserContact()[self.PageListUserContact().length - 1].pageNumber !== self.PageCountUserContact();
        }
    })
    self.GoToPageUserContact = function (page) {
        self.currentPageUserContact(page.pageNumber - 1);
    };
    self.GetClassUserContact = function (page) {
        return ((page.pageNumber - 1) === self.currentPageUserContact()) ? "click" : "";
    };
    self.StartPageUserContact = function () {
        self.currentPageUserContact(0);
    }
    self.BackPageUserContact = function () {
        if (self.currentPageUserContact() > 1) {
            self.currentPageUserContact(self.currentPageUserContact() - 1);
        }
    }
    self.GoToNextPageUserContact = function () {
        if (self.currentPageUserContact() < self.PageCountUserContact() - 1) {
            self.currentPageUserContact(self.currentPageUserContact() + 1);
        }
    }
    self.EndPageUserContact = function () {
        if (self.currentPageUserContact() < self.PageCountUserContact() - 1) {
            self.currentPageUserContact(self.PageCountUserContact() - 1);
        }
    }
    // hander
    self.ShowModal_AddNguoiLienHe = function (itemKH) {
        if (loaiDoiTuong == 1) {
            $('#lblKhachHang_LienHe').text('Khách hàng');
        }
        else {
            $('#lblKhachHang_LienHe').text('Nhà cung cấp');
        }
        $('#lblTitle').text('Thêm mới liên hệ');
        $('#modalPopuplg_Contact').modal('show');
        $('#txtCustomer_modal').attr('disabled', 'disabled');
        $('#divSearchKH .icon-search-input').remove();
        // assign TenDoiTuong is doing
        $('#txtCustomer_modal').val(itemKH.TenDoiTuong);
        // Reset LienHe old
        newModal_LienHe.ID_DoiTuong(itemKH.ID);
        newModal_LienHe.MaLienHe('');
        newModal_LienHe.TenLienHe('');
        newModal_LienHe.SoDienThoai('');
        newModal_LienHe.DienThoaiCoDinh('');
        newModal_LienHe.NgaySinh(null);
        newModal_LienHe.Email('');
        newModal_LienHe.ChucVu('');
        newModal_LienHe.DiaChi('');
        newModal_LienHe.GhiChu('');
        newModal_LienHe.FilesSelect([]);
        newModal_LienHe.HaveImage_Select(false);
        newModal_LienHe.ID_TinhThanh(null);
        newModal_LienHe.ID_QuanHuyen(null);
        newModal_LienHe.XungHo(0);
        $('#txtProvince_modal').text('--Chọn tỉnh thành--');
        $('#txtDistrict_modal').text('--Chọn quận huyện--');
        newModal_LienHe.booleanAdd_LienHe(true);
    }
    self.Update_NguoiLienHe = function (item) {
        $('#lblTitle').text('Cập nhật liên hệ');
        $('#modalPopuplg_Contact').modal('show');
        $('#txtCustomer_modal').attr('disabled', 'disabled');
        $('#divSearchKH .icon-search-input').remove();
        $('#txtCustomer_modal').val(item.TenDoiTuong);
        newModal_LienHe.SetData(item);
        newModal_LienHe.booleanAdd_LienHe(false);
        newModal_LienHe.HaveImage_Select(newModal_LienHe.HaveImage());
        if (newModal_LienHe.HaveImage_Select() === false) {
            newModal_LienHe.FilesSelect([]);
            $('#file').val('');
        }
        else {
            newModal_LienHe.FilesSelect(newModal_LienHe.FileImgs());
        }
    }
    self.Delete_Usercontact = function (itemKH, itemContact) {
        var id = itemContact.ID;
        var maLienHe = itemContact.MaLienHe;
        var maDoiTuong = itemKH.MaDoiTuong;
        dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn xóa liên hệ có mã  <b> ' + maLienHe + '</b> không?', function () {
            $.ajax({
                type: "DELETE",
                url: '/api/DanhMuc/DM_LienHeAPI/' + "DeleteDM_LienHe/" + id,
                dataType: 'json',
                contentType: 'application/json',
                success: function (result) {
                    for (var i = 0; i < self.listUserContact().length; i++) {
                        if (self.listUserContact()[i].ID === id) {
                            self.listUserContact.remove(self.listUserContact()[i]);
                            break;
                        }
                    }
                    var lenData = self.TotalRecordUserContact() - 1;
                    self.TotalRecordUserContact(lenData);
                    self.PageCountUserContact(Math.ceil(lenData / self.pageSizeUserContact()));
                    var noiDung = 'Xóa liên hệ ' + maLienHe + ' của khách hàng ' + maDoiTuong;
                    var noiDungChiTiet = noiDung.concat('<br /> Nhân viên thực hiện: ', user);
                    var objDiary = {
                        ID_NhanVien: idNhanVien,
                        ID_DonVi: idDonVi,
                        ChucNang: 'Khách hàng - Liên hệ',
                        LoaiNhatKy: 3,
                        NoiDung: noiDung,
                        NoiDungChiTiet: noiDungChiTiet,
                    }
                    Insert_NhatKyThaoTac_1Param(objDiary);
                    ShowMessage_Success('Xóa liên hệ thành công')
                },
                error: function (error) {
                    ShowMessage_Danger('Xóa liên hệ thất bại');
                }, complete: function () {
                    $('#modalPopuplgDelete').modal('hide');
                }
            })
        })
    }
    self.GetImages_LienHe = function (item) {
        var idLienHe = item.ID;
        ajaxHelper('/api/DanhMuc/DM_LienHeAPI/' + 'GetImages_byIDLienHe/' + idLienHe, 'GET').done(function (data) {
            if (data !== null && data.length > 0) {
                newModal_LienHe.HaveImage(true);
                // xóa những thẻ li có class = bx-clone (thẻ tự phát sinh của bx-slider)
                $('.bx-clone').remove();
                // nếu update ảnh: sử dung 2 lệnh này mới load được ảnh đại diện (OK)
                newModal_LienHe.AnhDaiDien([]);
                newModal_LienHe.AnhDaiDien.push(data[0]);
                newModal_LienHe.FileImgs(data);
            }
            else {
                newModal_LienHe.HaveImage(false);
                newModal_LienHe.AnhDaiDien([]);
                newModal_LienHe.FileImgs([]);
            }
        })
        // check role update, delete UserContact
        var roleUpdateUserContact = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen.indexOf('LienHe_CapNhat') > -1;
        })
        if (roleUpdateUserContact.length > 0) {
            $('.editLienHe').show();
        }
        var roleDeleteUserContact = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen.indexOf('LienHe_Xoa') > -1;
        })
        if (roleDeleteUserContact.length > 0) {
            $('.xoaLienHe').show();
        }
    }
    $('#modalPopuplg_Contact').on('hidden.bs.modal', function () {
        getUserContact(self.ID_DoiTuong_Doing());
    })
    // cong viec
    self.ListCongViec_DoiTuong = ko.observableArray();
    self.TabCongViec_Status = ko.observable(0);
    self.LoadTab_CongViec = function (itemKH) {
        let tr = $('#DiaryWork_' + itemKH.ID);
        $(tr).find('.btn-tab').removeClass('active');
        $(tr).find('.btn-tab').eq(0).addClass('active');
        GetListCongViec_byDoiTuong(itemKH.ID, 0);
        var roleInsertWork = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen.indexOf('CongViec_ThemMoi') > -1;
        });
        if (roleInsertWork.length > 0) {
            self.Show_BtnAddWork(true);
        }
        else {
            self.Show_BtnAddWork(false);
        }
    }
    self.ChangeTab_CongViec = function (itemKH, type) {
        var $this = $(event.currentTarget);
        $this.siblings('button').removeClass('active');
        $this.addClass('active');
        $this.css("border-bottom-color");
        self.TabCongViec_Status(0);
        GetListCongViec_byDoiTuong(itemKH.ID, type);
    }
    self.ShowInfor_Work = function () {
        // check role update, delete Work
        var roleUpdateWork = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen.indexOf('CongViec_CapNhat') > -1;
        })
        if (roleUpdateWork.length > 0) {
            $('.editWork').show();
        }
        var roleDeleteWork = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen.indexOf('CongViec_Xoa') > -1;
        })
        if (roleDeleteWork.length > 0) {
            $('.deleteWork').show();
        }
    }
    self.ShowPopup_AddWork = function (itemKH) {
        $('#nameModelThemMoi').text('Thêm mới công việc/lịch hẹn');
        $('#modalPopuplg_Work').modal('show');
        partialWork.DateFrom(moment(dtNow).format('DD/MM/YYYY'));
        partialWork.DateTo(moment(dtNow).format('DD/MM/YYYY'));
        partialWork.TimeFrom(moment(dtNow).format('HH:mm'));
        partialWork.TimeTo(moment(dtNow).add(2, 'hours').format('HH:mm'));
        partialWork.newCongViec(new FormModel_NewCongViec());
        partialWork.booleanAddCV(true);
        partialWork.IsAddTypeWork(true);
        partialWork.ListStaffShare([]);
        partialWork.Calendar_ResetNew();
        self.ID_DoiTuong_Doing(itemKH.ID)
        partialWork.newCongViec().ID_KhachHang(itemKH.ID);
        if (loaiDoiTuong == 2) {
            partialWork.IsCustomer(false);
        }
        else {
            partialWork.IsCustomer(true);
        }
        $('#calendar_txtKH').val(itemKH.TenDoiTuong);
        $('#calendar_txtKH2').val(itemKH.TenDoiTuong);
        $('#calendar_txtKH').attr('disabled', 'disabled');
        $('#calendar_txtKH2').attr('disabled', 'disabled');
    }
    $('#modalPopuplg_Work').on('hidden.bs.modal', function () {
        GetListCongViec_byDoiTuong(self.ID_DoiTuong_Doing(), 1);
    });
    self.Update_Work = function (itemKH, itemCV) {
        $('#nameModelThemMoi').text('Cập nhật công việc/lịch hẹn');
        $('#modalPopuplg_Work').modal('show');
        partialWork.Calendar_ResetText();
        partialWork.Calendar_RemoveCheckAfter();
        partialWork.ResetDateOfWeekChosed();
        partialWork.newCongViec().ID_KhachHang(itemKH.ID);
        partialWork.EventOld(itemCV);
        self.ID_DoiTuong_Doing(itemKH.ID);
        $('#calendar_txtKH2').attr('disabled', 'disabled');
        $('#calendar_txtKH').attr('disabled', 'disabled');
        if (loaiDoiTuong === 2) {
            partialWork.IsCustomer(false);
        }
        else {
            partialWork.IsCustomer(true);
        }
        partialWork.booleanAddCV(false);
        // typeWork
        if (itemCV.ID_LoaiTuVan != null) {
            partialWork.IsAddTypeWork(false);
        }
        else {
            partialWork.IsAddTypeWork(true);
        }
        partialWork.newCongViec().SetData(itemCV);
        partialWork.DateFromOld(itemCV.NgayGio);
    }
    self.Delete_Work = function (itemCV) {
        var id = itemCV.ID;
        var tenCV = itemCV.Ma_TieuDe;
        var tenDoiTuong = itemCV.TenDoiTuong;
        partialWork.DateFromOld(itemCV.NgayGio);
        var sLoai = 'công việc';
        switch (itemCV.PhanLoai) {
            case 3:
                sLoai = 'lịch hẹn';
                break;
        }
        dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn xóa ' + sLoai + ' <b > ' + tenCV + '</b > ngày <b > ' + moment(itemCV.NgayGio).format('DD/MM/YYYY') + ' </b > không ? ', function () {
            partialWork.DeleteWork(itemCV, sLoai);
        })
    }
    function GetListCongViec_byDoiTuong(idDoiTuong, type) {
        let arrIDLoaiTV = partialWork.LoaiCongViecs().map(function (x) { return x.ID });
        arrIDLoaiTV.push(const_GuidEmpty);
        let arrNhanVien = self.NhanViens().map(function (x) { return x.ID });
        let from = '2018-01-01';
        let to = '';
        let status = [1];
        switch (type) {
            case 0:// dangthuchien: time >= today + dangxuly (get cv cuoi thang toi)
                from = moment(new Date()).format('YYYY-MM-DD');
                to = moment().add('months', 1).endOf('month').format('YYYY-MM-DD');
                break;
            case 1:// chuathuchien < today + dangxuly
                to = moment(new Date()).format('YYYY-MM-DD');
                break;
            case 2: // hoanthanh: time <= today & TrangThaiCV= 3
                to = moment(new Date()).add(1, 'days').format('YYYY-MM-DD');
                status = [2];
                break;
        }
        self.TabCongViec_Status(type);
        var arrDV = [];
        for (let i = 0; i < self.ChiNhanhChosed().length; i++) {
            if ($.inArray(self.ChiNhanhChosed()[i], arrDV) === -1) {
                arrDV.push(self.ChiNhanhChosed()[i].ID);
            }
        }
        var model = {
            ID_DonVis: arrDV,
            IDLoaiTuVans: arrIDLoaiTV,
            IDNhanVienPhuTrachs: arrNhanVien,
            TrangThaiCVs: status,
            MucDoUuTien: '%%',
            LoaiDoiTuong: loaiDoiTuong,
            PhanLoai: '%%',
            FromDate: from,
            ToDate: to,
            TextSearch: null,
            CurrentPage: 0,
            PageSize: 10,
            ColumnsHide: null,
            TenChiNhanhs: '',
            TypeShow: 1,
            ID_KhachHang: idDoiTuong,
        };
        console.log(model)
        ajaxHelper('/api/DanhMuc/ChamSocKhachHangAPI/' + 'GetListLichHen_FullCalendar', 'POST', model).done(function (x) {

            if (x.res === true) {
                self.ListCongViec_DoiTuong(x.data);
                var lenData = x.data.length;
                self.TotalRecordSchedule(lenData);
                self.PageCountSchedule(Math.ceil(lenData / self.pageSizeSchedule()));
            }
        });
    }
    self.PageResult_Schedule = ko.computed(function (x) {
        var first = self.currentPageSchedule() * self.pageSizeSchedule();
        if (self.ListCongViec_DoiTuong() !== null) {
            return self.ListCongViec_DoiTuong().slice(first, first + self.pageSizeSchedule());
        }
        return null;
    })
    self.PageListSchedule = ko.computed(function () {
        var arrPage = [];
        var allPage = self.PageCountSchedule();
        var currentPageSchedule = self.currentPageSchedule();
        if (allPage > 4) {
            var i = 0;
            if (currentPageSchedule === 0) {
                i = parseInt(self.currentPageSchedule()) + 1;
            }
            else {
                i = self.currentPageSchedule();
            }
            if (allPage >= 5 && currentPageSchedule > allPage - 5) {
                if (currentPageSchedule >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumber: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    if (currentPageSchedule == 1) {
                        for (var j = currentPageSchedule - 1; (j <= currentPageSchedule + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                    else {
                        // get currentPageSchedule - 2 , currentPageSchedule, currentPageSchedule + 2
                        for (var j = currentPageSchedule - 2; (j <= currentPageSchedule + 2) && j < allPage; j++) {
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
        if (self.ListCongViec_DoiTuong() !== null) {
            if (self.ListCongViec_DoiTuong().length > 0) {
                self.fromitemSchedule((self.currentPageSchedule() * self.pageSizeSchedule()) + 1);
            }
            else {
                self.fromitemSchedule(0);
            }
            if (((self.currentPageSchedule() + 1) * self.pageSizeSchedule()) > self.ListCongViec_DoiTuong().length) {
                var fromItem = (self.currentPageSchedule() + 1) * self.pageSizeSchedule();
                if (fromItem < self.TotalRecordSchedule()) {
                    self.toitemSchedule((self.currentPageSchedule() + 1) * self.pageSizeSchedule());
                }
                else {
                    self.toitemSchedule(self.TotalRecordSchedule());
                }
            } else {
                self.toitemSchedule((self.currentPageSchedule() * self.pageSizeSchedule()) + self.pageSizeSchedule());
            }
        }
        return arrPage;
    });
    self.VisibleStartPageSchedule = ko.computed(function () {
        if (self.PageListSchedule().length > 0) {
            return self.PageListSchedule()[0].pageNumber !== 1;
        }
    });
    self.VisibleEndPageSchedule = ko.computed(function () {
        if (self.PageListSchedule().length > 0) {
            return self.PageListSchedule()[self.PageListSchedule().length - 1].pageNumber !== self.PageCountSchedule();
        }
    })
    self.GoToPageSchedule = function (page) {
        self.currentPageSchedule(page.pageNumber - 1);
    };
    self.GetClassSchedule = function (page) {
        return ((page.pageNumber - 1) === self.currentPageSchedule()) ? "click" : "";
    };
    self.StartPageSchedule = function () {
        self.currentPageSchedule(0);
    }
    self.BackPageSchedule = function () {
        if (self.currentPageSchedule() > 1) {
            self.currentPageSchedule(self.currentPageSchedule() - 1);
        }
    }
    self.GoToNextPageSchedule = function () {
        if (self.currentPageSchedule() < self.PageCountSchedule() - 1) {
            self.currentPageSchedule(self.currentPageSchedule() + 1);
        }
    }
    self.EndPageSchedule = function () {
        if (self.currentPageSchedule() < self.PageCountSchedule() - 1) {
            self.currentPageSchedule(self.PageCountSchedule() - 1);
        }
    }
    // modal infor HoaDon Ban/Dat/Tra/PhieuThu/Chi
    self.Modal_HoaDons = ko.observableArray();
    self.TongSLHang = ko.observable(0);
    self.Modal_SoQuy = ko.observableArray();
    self.LoaiHoaDon_MoPhieu = ko.observable(0);
    self.MaHoaDon_MoPhieu = ko.observable('');
    self.ShowPopup_InforHD = function (item) {
        switch (item.LoaiHoaDon) {
            case 22:
                vmThemMoiTheNap.showModalUpdate(item.ID, 1);
                break;
            case 32:
                if (commonStatisJs.CheckNull(item.ID_SoQuy)) {
                    vmNapTienDatCoc.getQuyHoaDon_byID(item.ID_SoQuy, true, 1);// 1.formType: tra lai tiencoc
                }
                break;
            default:
                vmChiTietHoaDon.showModalChiTietHoaDon(item.ID);
                break;
        }
    }
    self.NKyCoc_ShowPopupInforHD = function (item) {
        vmChiTietHoaDon.showModalChiTietHoaDon(item.ID_HoaDonGoc);
    }
    self.ShowPopup_InforHD_PhieuThu = function (item) {
        self.LoaiHoaDon_MoPhieu(item.LoaiHoaDon);
        self.MaHoaDon_MoPhieu(item.MaHoaDon);
        switch (item.LoaiHoaDon) {
            case 11:
            case 12:
                switch (item.LoaiThanhToan) {
                    case 1: // tiencoc
                        vmNapTienDatCoc.getQuyHoaDon_byID(item.ID, true);
                        break;
                    case 3:// khongbutru congno
                        vmThemPhieuThuChi.getQuyHoaDon_byID(item.ID, true);
                        break;
                    case 2: // dieuchinh congno
                        vmDieuChinhCongNo.getQuyHoaDon_byID(item.ID, true);
                        break;
                    default:// thanhtoan hoadon
                        vmThanhToan_assignListData();
                        if (loaiDoiTuong === 2) {
                            vmThanhToanNCC.showModalUpdate(item.ID, self.customerOld().NoHienTai);
                        }
                        else {
                            vmThanhToan.showModalUpdate(item.ID, self.customerOld().NoHienTai, 1);
                        }
                        break;
                }
                break;
            case 125: // chiphidichvu
                if (loaiDoiTuong === 2) {
                    VueChiPhi.CTHD_GetChiPhiDichVu([item.ID], 2, [self.IDDT()]);
                }
                break;
            case 22: // nap thegiatri
                vmThemMoiTheNap.showModalUpdate(item.ID, 1);
                break;
            case 32: // hoan tra thegiatri
                if (commonStatisJs.CheckNull(item.ID_SoQuy)) {
                    vmNapTienDatCoc.getQuyHoaDon_byID(item.ID_SoQuy, true, 1);// 1.formType: tra lai tiencoc
                }
                break;
            case 42:
                vmTatToanTGT.getData_andShowModalUpdate(item.ID);
                break;
            default:
                vmChiTietHoaDon.showModalChiTietHoaDon(item.ID);
                break;
        }
    }

    self.ClickMoPhieu = function () {
        localStorage.setItem('FindHD', self.MaHoaDon_MoPhieu());
        var url = '';
        console.log('self.LoaiHoaDon_MoPhieu() ', self.LoaiHoaDon_MoPhieu())
        switch (self.LoaiHoaDon_MoPhieu()) {
            case 1:
                url = "/#/Invoices";
                break;
            case 2:
                url = "/#/HoaDonBaoHanh";
                break;
            case 3:
                url = "/#/Order";
                break;
            case 4:
                url = "/#/PurchaseOrder";
                break;
            case 6:
                url = "/#/Returns";
                break;
            case 7:
                url = "/#/PurchaseReturns";
                break;
            case 8:
                url = "/#/DamageItems";
                break;
            case 9:
                url = "/#/StockTakes";
                break;
            case 10:
                url = "/#/Transfers";
                break;
            case 11:
            case 12:
                localStorage.removeItem('FindHD');
                localStorage.setItem('FindMaPhieuChi', self.MaHoaDon_MoPhieu());
                url = '/#/CashFlow';
                break;
            case 19:
                url = "/#/ServicePackage";
                break;
            case 25:
                url = "/#/HoaDonSuaChua";
                break;
        }
        if (url !== '') {
            window.open(url);
        }
    }

    // trang thai tiem nang KH
    function LoadTrangThai() {
        ajaxHelper(CSKHUri + 'GetTrangThaiTimKiem', 'GET').done(function (data) {
            if (data.res === true) {
                var lst = data.dataSoure.ttkhachhang;
                self.TrangThaiKhachHang(lst);
                partialWork.LoaiKhachHang(lst);
                if (loaiDoiTuong === 1) {
                    vmThemMoiKhach.listData.TrangThaiKhachs = lst;
                }
            }
        });
    };
    self.ShowModal_AddTrangThaiKH = function () {
        vmTrangThaiKhach.showModalAdd();
    }
    self.ShowModal_UpdateTrangThaiKH = function () {
        // get trangthai current
        var tt = $.grep(self.TrangThaiKhachHang(), function (x) {
            return x.ID === self.TrangThaiKH_chosed()
        });
        if (tt.length > 0) {
            tt[0].TenTrangThai = tt[0].Name;
            vmTrangThaiKhach.showModalUpdate(tt[0]);
        }
    }
    $('#modalStatus_Customer').on('hidden.bs.modal', function () {
        if (vmTrangThaiKhach.saveOK) {
            var obj = vmTrangThaiKhach.newTrangThai;
            obj.Name = obj.TenTrangThai;
            for (let i = 0; i < self.TrangThaiKhachHang().length; i++) {
                if (self.TrangThaiKhachHang()[i].ID === obj.ID) {
                    self.TrangThaiKhachHang().splice(i, 1);
                    break;
                }
            }
            if (vmTrangThaiKhach.typeUpdate !== 0) {
                self.TrangThaiKhachHang.unshift(obj);
            }
        }
    })
    // the gia tri + chiet khau nv
    self.TienTheGiaTri_PhieuThu = ko.observable();
    self.SoDuTheGiaTri = ko.observable();
    function GetSoDu_TheGiaTri(idDoiTuong) {
        // reset SoDu
        self.SoDuTheGiaTri(0);
        var date = moment(new Date()).format('YYYY-MM-DD HH:mm:ss');
        if (navigator.onLine) {
            ajaxHelper(DMDoiTuongUri + "Get_SoDuTheGiaTri_ofKhachHang_ByTime?idDoiTuong=" + idDoiTuong + '&datetime=' + date, 'GET').done(function (data) {
                self.SoDuTheGiaTri(data);
                if (loaiDoiTuong === 1) {
                    vmThanhToan.HoaDonChosing.SoDuTheGiaTri = data;
                }
            });
        }
    }

    self.SoDuDatCoc = ko.observable(0);
    function GetSoDuDatCoc(idDoiTuong) {
        var soduDatCoc = 0;
        ajaxHelper('/api/DanhMuc/DM_DoiTuongAPI/' + "GetTienDatCoc_ofDoiTuong?idDoiTuong=" + idDoiTuong
            + '&idDonVi=' + idDonVi, 'GET').done(function (x) {
                if (x.res && x.dataSoure.length > 0) {
                    soduDatCoc = x.dataSoure[0].SoDuTheGiaTri;
                }
                self.SoDuDatCoc(soduDatCoc);
            })
    }
    self.LoadTabTienCoc = function () {
        self.typeTab('tiencoc');
        var param = {
            TextSearch: self.IDDT(),// muon tam truong
            IdChiNhanhs: self.ChiNhanhChosed().map(function (x) { return x.ID }),
            NgayTaoFrom: self.NKyTheFrom(),
            NgayTaoTo: self.NKyTheTo(),
            CurrentPage: self.Coc_Currentpage() - 1,
            PageSize: 10,
        }
        ajaxHelper(DMDoiTuongUri + 'GetNhatKyTienCoc_OfDoiTuong', 'POST', param).done(function (x) {
            if (x.res) {
                var dataS = x.dataSoure;
                self.ListNhatKyTienCoc(dataS.data);
                self.Coc_PageList(dataS.ListPage);
                self.Coc_PageView(dataS.PageView);
                self.Coc_TotalRow(dataS.TotalRow);
                self.Coc_TotalPage(dataS.TotalPage);
                // get sodutiencoc
                GetSoDuDatCoc(self.IDDT());
            }
        })
    }
    self.Coc_GotoPage = function (item) {
        self.Coc_Currentpage(item);
        self.LoadTabTienCoc();
    }

    // anh lieu trinh
    self.ListAnhLieuTrinh = ko.observableArray();
    self.AnhLieuTrinh_PageList = ko.observableArray();
    self.AnhLieuTrinh_CurrentPage = ko.observable(1);
    self.AnhLieuTrinh_PageSize = ko.observable(2);
    self.AnhLieuTrinh_PageView = ko.observable();
    self.AnhLieuTrinh_isPrev = ko.observable(false);
    self.AnhLieuTrinh_isNext = ko.observable(false);
    self.AnhLieuTrinh_totalPage = ko.observable(0);

    self.LoadAnhLieuTrinh = function () {
        let param = {
            TextSearch: '',
            IDCustomers: [self.IDDT()],
            CurrentPage: self.AnhLieuTrinh_CurrentPage() - 1,
            PageSize: self.AnhLieuTrinh_PageSize(),
        }

        ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'GetListImgInvoice_byCus', 'POST', param).done(function (x) {
            if (x.res) {
                let dataS = x.dataSoure;
                self.ListAnhLieuTrinh(dataS.data);
                self.AnhLieuTrinh_PageList(dataS.listpage);
                self.AnhLieuTrinh_isPrev(dataS.isprev);
                self.AnhLieuTrinh_isNext(dataS.isnext);
                self.AnhLieuTrinh_totalPage(dataS.countpage);
                self.AnhLieuTrinh_PageView(dataS.pageview);
            }
        });
    }

    self.AnhLieuTrinh_GotoPage = function (item) {
        self.AnhLieuTrinh_CurrentPage(item);
        self.LoadAnhLieuTrinh();
    }
    self.AnhLieuTrinh_Next = function (item) {
        if (self.AnhLieuTrinh_CurrentPage() < self.AnhLieuTrinh_PageList().length) {
            self.AnhLieuTrinh_CurrentPage(self.AnhLieuTrinh_CurrentPage() + 1);
            self.LoadAnhLieuTrinh();
        }
    }
    self.AnhLieuTrinh_Prev = function (item) {
        if (self.AnhLieuTrinh_CurrentPage() > 1) {
            self.AnhLieuTrinh_CurrentPage(self.AnhLieuTrinh_CurrentPage() - 1);
            self.LoadAnhLieuTrinh();
        }
    }
    self.AnhLieuTrinh_ShowHoaDon = function (item) {
        vmChiTietHoaDon.showModalChiTietHoaDon(item.ID);
    }
    self.AnhLieuTrinh_ShowImg = function (item) {
        vmUpAnhHoaDon.isSaveToTemp = false;
        vmUpAnhHoaDon.isView = true;
        vmUpAnhHoaDon.GetListImgInvoiceDB(item.ID, "123");
        vmUpAnhHoaDon.showModalInsert();
    }

    //thẻ giá trị LT
    self.TrangThai_TheGiaTri = ko.observable();
    self.IDDT = ko.observable();
    self.loaiNKSD = ko.observable(1);
    self.loaiLSNT = ko.observable(1);
    self.TenKhachHangNapThe = ko.observable();
    self.GiaTriDieuChinh = ko.observable('');
    self.MaDieuChinh = ko.observable('');
    self.NgayDieuChinh = ko.observable(moment(today).format('DD/MM/YYYY'));
    self.TGT_ID = ko.observable(null);

    self.clickTheGiaTri = function (item) {
        self.typeTab('thegiatri');
        ResetTime_TheGiaTri();
        self.TheGiaTri_IsNhatKy(true);
        GetSoDu_TheGiaTri(item.ID);
        self.IDDT(item.ID);
        self.TenKhachHangNapThe(item.TenDoiTuong);
        self.TrangThai_TheGiaTri(item.TrangThai_TheGiaTri);
        SearchNhatKySDThe();

        // active tab0
        let tabTGT = $('#naptien' + item.ID);
        $(tabTGT).find('ul li').removeClass('active');
        $(tabTGT).find('ul li:eq(0)').addClass('active');

        $('#tab_b1' + item.ID).addClass('active');
        $('#tab_b2' + item.ID).removeClass('active');
    };
    self.clickLichSuNap = function (item) {
        ResetTime_TheGiaTri();
        self.TheGiaTri_IsNhatKy(false);
        self.loaiLSNT(1);
        SearchLichSuNapTien();
    };
    self.DieuChinhSoDu = function (item) {
        self.TGT_ID(null);
        self.GiaTriDieuChinh(0);
        self.TenKhachHangNapThe(item.TenDoiTuong);
        self.TrangThaiTheGiaTriChoose(item.TrangThai_TheGiaTri);
        $('#modalPopup_thegiatri').modal('show');
        $('#txtSoDuPopup').val(formatNumber(self.SoDuTheGiaTri()));
        $('#modalPopup_thegiatri').on('shown.bs.modal', function () {
            $('#txtSoDuPopup').val(formatNumber(self.SoDuTheGiaTri()));
        });
    };
    self.DieuChinhThe_showUpdate = function (item) {
        switch (item.LoaiHoaDon) {
            case 22:// napthe
                vmThemMoiTheNap.showModalUpdate(item.ID, 1);
                break;
            case 42:// tattoan congno
                vmTatToanTGT.getData_andShowModalUpdate(item.ID);
                break;
            case 23:// dieuchinh
                self.TGT_ID(item.ID);
                self.MaDieuChinh(item.MaHoaDon);
                if (item.PhatSinhTang > 0) {
                    self.GiaTriDieuChinh(item.PhatSinhTang);
                }
                else {
                    self.GiaTriDieuChinh(item.PhatSinhGiam);
                }
                self.NgayDieuChinh(moment(item.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'));
                $('#modalPopup_thegiatri').modal('show');
                break;
        }
    }

    self.TGT_HuyPhieuDieuChinh = function () {
        // todo check sudung vuot TGT
        $.getJSON('/api/DanhMuc/BH_HoaDonAPI/' + 'TGT_HuyPhieuDieuChinh?id=' + self.TGT_ID()).done(function (x) {
            console.log(x)
            if (x.res) {
                ShowMessage_Success('Hủy phiếu điều chỉnh thành công');

                $('#modalPopup_thegiatri').modal('hide');

                let diary = {
                    LoaiNhatKy: 3,
                    ID_DonVi: VHeader.IdDonVi,
                    ID_NhanVien: VHeader.IdNhanVien,
                    ChucNang: 'Hủy phiếu điều chỉnh thẻ giá trị',
                    NoiDung: 'Hủy phiếu điều chỉnh thẻ giá trị '.concat(self.MaDieuChinh()),
                    NoiDungChiTiet: 'Thông tin hủy:'.concat('<br /> Mã phiếu: ', self.MaDieuChinh(),
                        '<br /> Khách hàng: ', self.TenKhachHangNapThe(),
                        '<br /> Giá trị điều chỉnh: ', formatNumber3Digit(self.GiaTriDieuChinh()),
                        '<br /> Ngày điều chỉnh: ', self.NgayDieuChinh(),
                        '<br /> User hủy phiếu: ', VHeader.UserLogin),
                }
                Insert_NhatKyThaoTac_1Param(diary);
            }
        })
    }

    self.TrangThaiTheGT = ko.observableArray([
        { TrangThai: "Đang hoạt động", value: "1" },
        { TrangThai: "Ngừng hoạt động", value: "2" }
    ]);
    self.TrangThaiTheGiaTriChoose = ko.observable("1");
    self.showpopupNapTien = function (item) {
        vmThemMoiTheNap.showModalAddNew(1);
        var cus = {
            ID: item.ID,
            MaDoiTuong: item.MaDoiTuong,
            TenDoiTuong: item.TenDoiTuong,
            DienThoai: item.DienThoai,
        }
        vmThemMoiTheNap.ChangeCustomer(cus);
        vmThanhToanGara.listData.NhanViens = self.NhanViens();
        vmThanhToanGara.listData.ChietKhauHoaDons = vmHoaHongHoaDon.listData.ChietKhauHoaDons;
    };
    self.NganHangs = ko.observableArray();
    self.TaiKhoansPOS = ko.observableArray();
    self.TaiKhoansCK = ko.observableArray();
    function getAllNganHang() {
        ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/' + 'GetAllNganHang', 'GET').done(function (data) {
            self.NganHangs(data);
        })
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
                vmNapTienDatCoc.listData.NhanViens = self.NhanViens();

                partialWork.NhanViens(lstNV_byDonVi);
                if (self.ListIDNhanVienQuyen().length > 0) {
                    let arrManager = $.grep(self.NhanViens(), function (x) {
                        return $.inArray(x.ID, self.ListIDNhanVienQuyen()) > -1;
                    });
                    self.NhanVienQuanLy(arrManager);
                }
                else {
                    self.NhanVienQuanLy(lstNV_byDonVi);
                }
                if (loaiDoiTuong === 1) {
                    vmThemMoiKhach.listData.NhanViens = lstNV_byDonVi;
                    vmThanhToan.listData.NhanViens = lstNV_byDonVi;
                    vmHoaHongDV.listData.NhanViens = lstNV_byDonVi;
                }
                else {
                    vmThanhToanNCC.listData.NhanViens = lstNV_byDonVi;
                }
            }
        });
    }
    self.LuuTrangThaiThe = function () {
        var sodu = formatNumberToFloat($('#txtSoDuPopup').val());
        var soduhientai = self.SoDuTheGiaTri();
        var chenhlech = sodu - soduhientai;
        var trangthai = self.TrangThaiTheGiaTriChoose();
        var nguoitao = $('#txtTenTaiKhoan').text();
        self.SoDuTheGiaTri(sodu);
        let sDieuChinh = chenhlech > 0 ? 'Điều chỉnh tăng' : 'Điều chỉnh giảm';
        ajaxHelper(DMDoiTuongUri + 'UpdateDieuChinhSoDu?chenhlech=' + chenhlech + '&trangthai=' + trangthai + '&iddt=' + self.IDDT() + '&iddonvi=' + idDonVi + '&idnhanvien=' + idNhanVien + '&nguoitao=' + nguoitao + '&sodusaunap=' + sodu, 'GET').done(function (data) {
            if (data === "") {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + 'Điều chỉnh thành công thẻ giá trị', 'success');
                $('#modalPopup_thegiatri').modal('hide');
                SearchNhatKySDThe();
                SearchLichSuNapTien();

                let diary = {
                    ID_DonVi: VHeader.IdDonVi,
                    ID_NhanVien: VHeader.IdNhanVien,
                    ChucNang: 'Điều chỉnh số dư thẻ giá trị',
                    LoaiNhatKy: 1,
                    NoiDung: sDieuChinh,
                    NoiDungChiTiet: 'Khách hàng: '.concat(self.TenKhachHangNapThe(),
                        '<br /> ', sDieuChinh, ': ', formatNumber3Digit(chenhlech),
                        '<br /> User điều chỉnh: ', VHeader.UserLogin
                    ),
                }
                Insert_NhatKyThaoTac_1Param(diary);
            }
        });
    };
    //pagging tab nhật ký sử dụng
    self.pageSizesNKThe = [5, 10, 15, 20];
    self.pageSizeNKThe = ko.observable(self.pageSizesNKThe[0]);
    self.currentPageNKThe = ko.observable(0);
    self.fromitemNKThe = ko.observable(1);
    self.toitemNKThe = ko.observable();
    self.ListNhatKyThe = ko.observableArray();
    self.PageCountNKThe = ko.observable();
    self.TotalRecordNKThe = ko.observable();
    self.TongTienTang = ko.observable();
    self.TongTienGiam = ko.observable();
    self.ThoiGianChange = ko.observable();
    self.ThoiGianChangeLSNT = ko.observable();
    self.NKyTheFrom = ko.observable('2016-01-01');
    self.NKyTheTo = ko.observable(moment(today).add('days', 1).format('YYYY-MM-DD'));
    self.TheGiaTri_IsNhatKy = ko.observable(true);
    self.DateRangeThe_From = ko.observable(moment(today).format('DD/MM/YYYY'));
    self.DateRangeThe_To = ko.observable(moment(today).format('DD/MM/YYYY'));
    self.ListNhatKyTienCoc = ko.observableArray();
    self.Coc_PageList = ko.observableArray();
    self.Coc_PageView = ko.observable('');
    self.Coc_Currentpage = ko.observable(1);
    self.Coc_TotalRow = ko.observable(0);
    self.Coc_TotalPage = ko.observable(0);
    self.typeTab = ko.observable('');
    self.TheGiaTri_GetTimeSearch = function (typeTime, typeTab = "thegiatri") {
        var thiObj = $(event.currentTarget);
        var _now = new Date();
        switch (typeTime) {
            case 0:
                // all
                dayStart = '2016-01-01';
                dayEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
                break;
            case 1:
                // hom nay
                dayStart = moment(_now).format('YYYY-MM-DD');
                dayEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
                break;
            case 2:
                // hom qua : dayEnd must set infont of dayStart
                dayEnd = moment(_now).format('YYYY-MM-DD');
                dayStart = moment(_now).subtract('days', 1).format('YYYY-MM-DD');
                break;
            case 3:
                // tuan nay (start: monday, end: sunday)
                dayStart = moment().startOf('week').add('days', 1).format('YYYY-MM-DD');
                dayEnd = moment().endOf('week').add('days', 2).format('YYYY-MM-DD');
                break;
            case 4:
                // tuan truoc (OK)
                dayStart = moment().startOf('week').subtract('days', 6).format('YYYY-MM-DD');
                dayEnd = moment().startOf('week').add('days', 1).format('YYYY-MM-DD');
                break;
            case 5:
                // thang nay
                dayStart = moment().startOf('month').format('YYYY-MM-DD');
                dayEnd = moment().endOf('month').add('days', 1).format('YYYY-MM-DD');
                break;
            case 6:
                // thang truoc
                dayStart = moment().subtract('months', 1).startOf('month').format('YYYY-MM-DD');
                dayEnd = moment().subtract('months', 1).endOf('month').add('days', 1).format('YYYY-MM-DD');
                break;
        }
        self.NKyTheFrom(dayStart);
        self.NKyTheTo(dayEnd);
        switch (typeTab) {
            case "thegiatri":
                if (self.TheGiaTri_IsNhatKy()) {
                    SearchNhatKySDThe();
                    //thiObj.parent().prev().text(thiObj.text());
                }
                else {
                    SearchLichSuNapTien();
                    //thiObj.parent().prev().text(thiObj.text());
                }
                break;
            case "tiencoc":
                self.Coc_Currentpage(1);
                self.LoadTabTienCoc();
                break;
        }
        thiObj.parent().prev().text(thiObj.text());
    }
    function ResetTime_TheGiaTri() {
        $('#dropdownMenuButton').text('Toàn thời gian');
        $('#dropdownMenuButt2on').text('Toàn thời gian');
        self.NKyTheFrom('2016-01-01');
        self.NKyTheTo(moment(new Date()).add('days', 1).format('YYYY-MM-DD'));

    }
    self.SearchTheGiaTri = function () {
        self.NKyTheFrom(moment(self.DateRangeThe_From(), 'DD/MM/YYYY').format('YYYY-MM-DD'));
        self.NKyTheTo(moment(self.DateRangeThe_To(), 'DD/MM/YYYY').add('days', 1).format('YYYY-MM-DD'));
        var textDate = self.DateRangeThe_From().concat(' - ', self.DateRangeThe_To());
        switch (self.typeTab()) {
            case 'thegiatri':
                SearchNhatKySDThe();
                if (self.TheGiaTri_IsNhatKy()) {
                    SearchNhatKySDThe();
                }
                else {
                    SearchLichSuNapTien();
                }
                $('#naptien' + self.IDDT()).find('.tab-pane.active').find('button.dropdown-toggle').text(textDate);
                break;
            case 'tiencoc':
                $('#tiencoc_' + self.IDDT()).find('button.dropdown-toggle').text(textDate);
                self.LoadTabTienCoc();
                break;
        }
        $('#custometimemodal').modal('hide');
    }
    function SearchNhatKySDThe() {
        var model = {
            loai: self.loaiNKSD(),
            iddt: self.IDDT(),
            currentPage: self.currentPageNKThe(),
            pageSize: self.pageSizeNKThe(),
            dayStart: self.NKyTheFrom(),
            dayEnd: self.NKyTheTo(),
        };
        ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'GetListNhatKySDThe', 'POST', model).done(function (obj) {
            if (obj.res) {
                self.ListNhatKyThe(obj.lst);
                self.PageCountNKThe(obj.pageCount);
                self.TotalRecordNKThe(obj.Rowcount);
                self.TongTienTang(obj.TongTienTang);
                self.TongTienGiam(obj.TongTienGiam);
            }
        });
    };
    self.PageResultsNKThe = ko.computed(function () {
        if (self.ListNhatKyThe() !== null) {
            self.fromitemNKThe((self.currentPageNKThe() * self.pageSizeNKThe()) + 1);
            if (((self.currentPageNKThe() + 1) * self.pageSizeNKThe()) > self.ListNhatKyThe().length) {
                var fromItem = (self.currentPageNKThe() + 1) * self.pageSizeNKThe();
                if (fromItem < self.TotalRecordNKThe()) {
                    self.toitemNKThe((self.currentPageNKThe() + 1) * self.pageSizeNKThe());
                }
                else {
                    self.toitemNKThe(self.TotalRecordNKThe());
                }
            } else {
                self.toitemNKThe((self.currentPageNKThe() * self.pageSizeNKThe()) + self.pageSizeNKThe());
            }
        }
    });
    self.PageList_DisplayNKThe = ko.computed(function () {
        var arrPage = [];
        var allPage = self.PageCountNKThe();
        var currentPage = self.currentPageNKThe();
        if (allPage > 4) {
            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPageNKThe()) + 1;
            }
            else {
                i = self.currentPageNKThe();
            }
            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumberNKThe: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    if (currentPage == 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumberNKThe: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj = {
                                pageNumberNKThe: j + 1,
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
                            pageNumberNKThe: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumberNKThe: i,
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
                        pageNumberNKThe: i + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }
        return arrPage;
    });
    self.VisibleStartPageNKThe = ko.computed(function () {
        if (self.PageList_DisplayNKThe().length > 0) {
            return self.PageList_DisplayNKThe()[0].pageNumberNKThe !== 1;
        }
    });
    self.VisibleEndPageNKThe = ko.computed(function () {
        if (self.PageList_DisplayNKThe().length > 0) {
            return self.PageList_DisplayNKThe()[self.PageList_DisplayNKThe().length - 1].pageNumberNKThe !== self.PageCountNKThe();
        }
    });
    self.GoToPageHDNKThe = function (page) {
        if (page.pageNumberNKThe !== '.') {
            self.currentPageNKThe(page.pageNumberNKThe - 1);
            SearchNhatKySDThe();
        }
    };
    self.StartPageNKThe = function () {
        self.currentPageNKThe(0);
        SearchNhatKySDThe();
    };
    self.BackPageNKThe = function () {
        if (self.currentPageNKThe() > 1) {
            self.currentPageNKThe(self.currentPageNKThe() - 1);
            SearchNhatKySDThe();
        }
    };
    self.GoToNextPageNKThe = function () {
        if (self.currentPageNKThe() < self.PageCountNKThe() - 1) {
            self.currentPageNKThe(self.currentPageNKThe() + 1);
            SearchNhatKySDThe();
        }
    };
    self.EndPageNKThe = function () {
        if (self.currentPageNKThe() < self.PageCountNKThe() - 1) {
            self.currentPageNKThe(self.PageCountNKThe() - 1);
            SearchNhatKySDThe();
        }
    };
    self.GetClassHDNKThe = function (page) {
        return ((page.pageNumberNKThe - 1) === self.currentPageNKThe()) ? "click" : "";
    };
    //end nhật ký sử dụng
    //pagging lịch sử nạp tiền
    self.pageSizesNT = [10, 20, 30];
    self.pageSizeNT = ko.observable(self.pageSizesNT[0]);
    self.currentPageNT = ko.observable(0);
    self.fromitemNT = ko.observable(1);
    self.toitemNT = ko.observable();
    self.ListLSNapThe = ko.observableArray();
    self.PageCountNT = ko.observable();
    self.TotalRecordNT = ko.observable();
    function SearchLichSuNapTien() {
        // chinhanh
        let arrDV = [];
        for (var i = 0; i < self.ChiNhanhChosed().length; i++) {
            if ($.inArray(self.ChiNhanhChosed()[i], arrDV) === -1) {
                arrDV.push(self.ChiNhanhChosed()[i].ID);
            }
        }
        if (arrDV.length === 0) {
            arrDV = [idDonVi];
        }
        var model = {
            loai: self.loaiLSNT(),
            iddt: self.IDDT(),
            currentPage: self.currentPageNT(),
            pageSize: self.pageSizeNT(),
            dayStart: self.NKyTheFrom(),
            dayEnd: self.NKyTheTo(),
            arrChiNhanh: arrDV,
        };
        ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'GetHisChargeValueCard', 'POST', model).done(function (x) {
            if (x.res == true) {
                self.ListLSNapThe(x.lst);
                self.PageCountNT(x.pageCount);
                self.TotalRecordNT(x.Rowcount);
                self.TongTienTang(x.TongTang);
                self.TongTienGiam(x.TongGiam);
            }
        });
    }
    self.PageResultsNT = ko.computed(function () {
        if (self.ListLSNapThe() !== null) {
            self.fromitemNT((self.currentPageNT() * self.pageSizeNT()) + 1);
            if (((self.currentPageNT() + 1) * self.pageSizeNT()) > self.ListLSNapThe().length) {
                var fromItem = (self.currentPageNT() + 1) * self.pageSizeNT();
                if (fromItem < self.TotalRecordNT()) {
                    self.toitemNT((self.currentPageNT() + 1) * self.pageSizeNT());
                }
                else {
                    self.toitemNT(self.TotalRecordNT());
                }
            } else {
                self.toitemNT((self.currentPageNT() * self.pageSizeNT()) + self.pageSizeNT());
            }
        }
    });
    self.PageList_DisplayNT = ko.computed(function () {
        var arrPage = [];
        var allPage = self.PageCountNT();
        var currentPage = self.currentPageNT();
        if (allPage > 4) {
            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPageNT()) + 1;
            }
            else {
                i = self.currentPageNT();
            }
            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumberNT: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    if (currentPage == 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumberNT: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj = {
                                pageNumberNT: j + 1,
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
                            pageNumberNT: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumberNT: i,
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
                        pageNumberNT: i + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }
        return arrPage;
    });
    self.VisibleStartPageNT = ko.computed(function () {
        if (self.PageList_DisplayNT().length > 0) {
            return self.PageList_DisplayNT()[0].pageNumberNT !== 1;
        }
    });
    self.VisibleEndPageNT = ko.computed(function () {
        if (self.PageList_DisplayNT().length > 0) {
            return self.PageList_DisplayNT()[self.PageList_DisplayNT().length - 1].pageNumberNT !== self.PageCountNT();
        }
    });
    self.GoToPageHDNT = function (page) {
        if (page.pageNumberNT !== '.') {
            self.currentPageNT(page.pageNumberNT - 1);
            SearchLichSuNapTien();
        }
    };
    self.StartPageNT = function () {
        self.currentPageNT(0);
        SearchLichSuNapTien();
    };
    self.BackPageNT = function () {
        if (self.currentPageNT() > 1) {
            self.currentPageNT(self.currentPageNT() - 1);
            SearchLichSuNapTien();
        }
    };
    self.GoToNextPageNT = function () {
        if (self.currentPageNT() < self.PageCountNT() - 1) {
            self.currentPageNT(self.currentPageNT() + 1);
            SearchLichSuNapTien();
        }
    };
    self.EndPageNT = function () {
        if (self.currentPageNT() < self.PageCountNT() - 1) {
            self.currentPageNT(self.PageCountNT() - 1);
            SearchLichSuNapTien();
        }
    };
    self.GetClassHDNT = function (page) {
        return ((page.pageNumberNT - 1) === self.currentPageNT()) ? "click" : "";
    };
    function GetAllSMSMau() {
        ajaxHelper(ThietLapAPI + 'GetAllMauTin', 'GET').done(function (data) {
            self.SMSMaus(data);
        });
    }
    function getAllBrandName() {
        ajaxHelper(ThietLapAPI + 'GetallBrandName', 'GET').done(function (data) {
            data = data.filter(p => p.Status === 1);
            self.BrandNames(data);
        });
    }
    //SMS Lee Tinh
    self.BrandNames = ko.observableArray();
    self.SMSMaus = ko.observableArray();

    self.Insert_ManyNhom = function (lstNhom) {
        var self = this;
        var myData = {};
        myData.lstDM_DoiTuong_Nhom = lstNhom;
        ajaxHelper(self.UrlDoiTuongAPI + 'PostDM_DoiTuong_Nhom', 'POST', myData).done(function (x) {
        }).fail(function () {
            ShowMessage_Danger("Thêm mới " + sLoai + " thất bại");
        })
    }
    $('#modalMoveGroup').on('hidden.bs.modal', function () {
        SearchKhachHang(false, false);
    })
    // vue khachhang/ncc
    $('#ThemMoiKhachHang').on('hidden.bs.modal', function () {
        var saveOK = vmThemMoiKhach.saveOK;
        var isNew = vmThemMoiKhach.isNew;
        var obj = vmThemMoiKhach.customerDoing;
        var objOld = vmThemMoiKhach.customerOld;
        if (saveOK) {
            obj.TongBan = 0;
            obj.KhuVuc = obj.TenTinhThanh;
            obj.PhuongXa = obj.TenQuanHuyen;
            obj.NguoiGioiThieu = obj.TenNguoiGioiThieu;
            obj.TrangThaiKhachHang = obj.TenTrangThai;
            obj.TenNhomDT = obj.TenNhomKhachs;
            obj.ID_NhomDoiTuong = obj.IDNhomKhachs;
            if (isNew) {
                obj.NoHienTai = 0;
                obj.TongBan = 0;
                obj.TongBanTruTraHang = 0;
                obj.TongTichDiem = 0;
                self.DoiTuongs.unshift(obj);
                self.TotalRecord(self.TotalRecord() + 1);
                HideShowColumn();
            }
            else {
                obj.NoHienTai = objOld.NoHienTai;
                obj.TongBan = objOld.TongBan;
                obj.TongBanTruTraHang = objOld.TongBanTruTraHang;
                obj.TongTichDiem = objOld.TongTichDiem;
                for (let i = 0; i < self.DoiTuongs().length; i++) {
                    if (self.DoiTuongs()[i].ID === obj.ID) {
                        self.DoiTuongs.remove(self.DoiTuongs()[i]);
                        break;
                    }
                }
                self.DoiTuongs.unshift(obj);
                HideShowColumn();
            }
        }
    })
    $('#vmThemMoiNCC').on('hidden.bs.modal', function () {
        var saveOK = vmThemMoiNCC.saveOK;
        var isNew = vmThemMoiNCC.isNew;
        var obj = vmThemMoiNCC.customerDoing;
        var objOld = vmThemMoiNCC.customerOld;
        if (loaiDoiTuong === 2) {
            saveOK = vmThemMoiNCC.saveOK;
            isNew = vmThemMoiNCC.isNew;
            obj = vmThemMoiNCC.newVendor;
            objOld = vmThemMoiNCC.customerOld;
        }
        if (saveOK) {
            obj.TongBan = 0;
            obj.KhuVuc = obj.TenTinhThanh;
            obj.PhuongXa = obj.TenQuanHuyen;
            obj.NguoiGioiThieu = obj.TenNguoiGioiThieu;
            obj.TrangThaiKhachHang = obj.TenTrangThai;
            obj.TenNhomDT = obj.TenNhomKhachs;
            if (isNew) {
                obj.NoHienTai = 0;
                obj.TongBan = 0;
                obj.TongBanTruTraHang = 0;
                obj.TongTichDiem = 0;
                obj.PhiDichVu = 0;
                self.DoiTuongs.unshift(obj);
                self.TotalRecord(self.TotalRecord() + 1);
                HideShowColumn();
            }
            else {
                obj.NoHienTai = objOld.NoHienTai;
                obj.TongBan = objOld.TongBan;
                obj.TongBanTruTraHang = objOld.TongBanTruTraHang;
                obj.TongTichDiem = objOld.TongTichDiem;
                obj.PhiDichVu = objOld.PhiDichVu;
                for (let i = 0; i < self.DoiTuongs().length; i++) {
                    if (self.DoiTuongs()[i].ID === obj.ID) {
                        self.DoiTuongs.remove(self.DoiTuongs()[i]);
                        break;
                    }
                }
                self.DoiTuongs.unshift(obj);
                HideShowColumn();
            }
        }
    })
    $('#modalNguonKhach').on('hidden.bs.modal', function () {
        if (vmNguonKhach.saveOK) {
            var obj = vmNguonKhach.newNguonKhach;
            for (let i = 0; i < self.NguonKhachs().length; i++) {
                if (self.NguonKhachs()[i].ID === obj.ID) {
                    self.NguonKhachs().splice(i, 1);
                    break;
                }
            }
            if (vmNguonKhach.typeUpdate !== 0) {
                self.NguonKhachs.unshift(obj);
            }
        }
    })
    $('#ThemNhomKhachHang').on('hidden.bs.modal', function () {
        var saveOK = vmThemMoiNhomKhach.saveOK;
        var typeUpdate = vmThemMoiNhomKhach.typeUpdate;
        var nhom = vmThemMoiNhomKhach.newGroup;
        if (saveOK) {
            for (let i = 0; i < self.NhomDoiTuongs().length; i++) {
                if (self.NhomDoiTuongs()[i].ID === nhom.ID) {
                    self.NhomDoiTuongs.remove(self.NhomDoiTuongs()[i]);
                    break;
                }
            };
            if (typeUpdate !== 0) {
                self.NhomDoiTuongs.unshift(nhom);
            }
        }
    })
    $('#vmThemNhomNCC').on('hidden.bs.modal', function () {
        var saveOK = vmThemMoiNhomNCC.saveOK;
        var typeUpdate = vmThemMoiNhomNCC.typeUpdate;
        var nhom = vmThemMoiNhomNCC.newGroup;
        if (saveOK) {
            for (let i = 0; i < self.NhomDoiTuongs().length; i++) {
                if (self.NhomDoiTuongs()[i].ID === nhom.ID) {
                    self.NhomDoiTuongs.remove(self.NhomDoiTuongs()[i]);
                    break;
                }
            };
            if (typeUpdate !== 0) {
                self.NhomDoiTuongs.unshift(nhom);
            }
        }
    })
    // vue thegiatri
    $('#vmThemMoiTheNap').on('hidden.bs.modal', function () {
        if (vmThemMoiTheNap.saveOK) {
            SearchLichSuNapTien();
        }
    })

    $('#vmDieuChinhCongNo').on('hidden.bs.modal', function () {
        if (vmDieuChinhCongNo.saveOK) {
            GetLst_CongNoKH();
        }
    })

    Page_Load();
};
var viewModelKhachHang = new ViewModelKH();
ko.applyBindings(viewModelKhachHang, document.getElementById('formCustomer'));
var newModal_LienHe = new PartieView_NewUserContact();
ko.applyBindings(newModal_LienHe, document.getElementById('modalPopuplg_Contact'));
var partialWork = new PartialView_CongViec();
ko.applyBindings(partialWork, document.getElementById('modalWork'));
function hidewait1(o) {
    console.log("aa")
    $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" /></div>')
}
$(function () {
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
                "Tháng 1 ",
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
function HideLostFocust() {
    $('#showseach_NhomDT').delay(300).hide(0, function () {
    });
}
var arrIDDoiTuong = [];
function SetCheckAll(obj) {
    var isChecked = $(obj).is(":checked");
    $('.check-group input[type=checkbox]').each(function () {
        $(this).prop('checked', isChecked);
    })
    if (isChecked) {
        $('.check-group input[type=checkbox]').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined && !(jQuery.inArray(thisID, arrIDDoiTuong) > -1)) {
                arrIDDoiTuong.push(thisID);
            }
        });
    }
    else {
        $('.check-group input[type=checkbox]').each(function () {
            var thisID = $(this).attr('id');
            for (var i = 0; i < arrIDDoiTuong.length; i++) {
                if (arrIDDoiTuong[i] === thisID) {
                    arrIDDoiTuong.splice(i, 1);
                    break;
                }
            }
        })
    }
    if (arrIDDoiTuong.length > 0) {
        $('#divThaoTac').show();
        $('.choose-commodity').css("display", "inline-block").trigger("RemoveClassForButtonNew");
        $('#count').text(arrIDDoiTuong.length);
    }
    else {
        $('#divThaoTac').hide();
        $('.choose-commodity').hide().trigger("addClassForButtonNew");
    }
}
function ChoseDoiTuong(obj) {
    var thisID = $(obj).attr('id');
    if ($(obj).is(':checked')) {
        if ($.inArray(thisID, arrIDDoiTuong) === -1) {
            arrIDDoiTuong.push(thisID);
        }
    }
    else {
        // remove item in arrID
        arrIDDoiTuong = arrIDDoiTuong.filter(x => x !== thisID);
    }
    if (arrIDDoiTuong.length > 0) {
        $('#divThaoTac').show();
        $('.choose-commodity').show().trigger("RemoveClassForButtonNew");
        $('#count').text(arrIDDoiTuong.length);
    }
    else {
        $('#divThaoTac').hide();
        $('.choose-commodity').hide().trigger("addClassForButtonNew");
    }
    // count input is checked
    var countCheck = 0;
    $('#tblList tr td.check-group input').each(function (x) {
        var id = $(this).attr('id');
        if ($.inArray(id, arrIDDoiTuong) > -1) {
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
    arrIDDoiTuong = [];
    $('#divThaoTac').hide();
    $('.choose-commodity').hide();
}
$('#modalPopuplg_Work').on('shown.bs.modal', function (e) {
    $('.datetimepicker4').datetimepicker({
        format: 'd/m/Y H:i',
        timepicker: true,
        step: 30,
        mask: false
    });
});
$('#modalpopup_NapThe').on('shown.bs.modal', function (e) {
    $('.datetimepicker4').datetimepicker({
        format: 'd/m/Y H:i',
        timepicker: true,
        step: 30,
        mask: true
    });
    $('input[type=text]').click(function () {
        $(this).select();
    });
});
$(function () {
    $('.showfiltercolumn').on('click', function () {
        $('.tr-filter-head').toggle();
    });

    $(".kv1").click(function () {
        var display = $(this).next(".list-kv").css('display');
        $(".list-kv").each(function () {
            $(this).hide();
        });
        if (display === 'none') {
            $(this).next(".list-kv").show();
        }
        $(".list-kv").mouseup(function () {
            return false
        });
        $(".kv1").mouseup(function () {
            return false
        });
        $(document).mouseup(function () {
            $(".list-kv").hide();
        });
    });
})