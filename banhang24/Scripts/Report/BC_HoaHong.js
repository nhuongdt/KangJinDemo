var ModelReportDiscount = function () {
    var self = this;

    var _userLogin = $('#txtTenTaiKhoan').text();
    var _idDonVi = $('#hd_IDdDonVi').val();
    var _idNhanVien = $('.idnhanvien').text();
    var _userID = $('.idnguoidung').text();
    var _elmCheckAfterLi = '<i class="fa fa-check check-after-li" style="display:block"></i>';
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var Key_Form = 'Key_RpDiscountProduct';
    var typeCheck = 7;// use when load checkbox and check hide/show colum after load grid

    self.TypeReport = ko.observable(4);
    self.TypeReport_Parent = ko.observable(4);
    self.RdoTypeTime = ko.observable('1');
    self.TodayBC = ko.observable();
    self.TenChiNhanhs = ko.observable($('#_txtTenDonVi').text());
    self.LiInput_Time = ko.observable(6);
    self.ChiNhanhs = ko.observableArray();
    self.ChiNhanhChosed = ko.observableArray();
    self.ListCheckBox = ko.observableArray();
    self.NumberColum_Div2 = ko.observable();
    self.IsReportDetail = ko.observable(false);
    self.Quyen_NguoiDung = ko.observableArray();
    self.NhomHangHoas = ko.observableArray();
    self.fiterNhomHH = ko.observable();
    self.IDNhomHangChosed = ko.observableArray();
    self.ArrDonVi = ko.observableArray();
    self.LstIDDonVi = ko.observableArray([_idDonVi]);

    self.LaHangHoa = ko.observable(true);
    self.LaDichVu = ko.observable(true);
    self.LaCombo = ko.observable(true);
    self.MangChungTuChosed = ko.observableArray();

    self.MangChungTu = ko.observableArray([
        {
            ID: 0, TenChungTu: 'Tất cả',
        },
        {
            ID: 1, TenChungTu: 'Hóa đơn bán lẻ',
        },
        {
            ID: 6, TenChungTu: 'Hóa đơn trả hàng',
        },
        {
            ID: 19, TenChungTu: 'Gói dịch vụ',
        },
        {
            ID: 22, TenChungTu: 'Thẻ giá trị',
        },
        {
            ID: 25, TenChungTu: 'Hóa đơn sửa chữa',
        },
    ])

    if (VHeader.IdNganhNgheKinhDoanh.toUpperCase() !== 'C16EDDA0-F6D0-43E1-A469-844FAB143014') {
        self.MangChungTu.splice(5, 1);
    }
    self.ArrMangChungTu = ko.observableArray($.extend([], true, self.MangChungTu()));

    self.IsCheck_DoanhThu = ko.observable(true);
    self.IsCheck_ThucThu = ko.observable(true);
    self.IsCheck_VND = ko.observable(true);
    self.TrangThaiHD = ko.observable('1'); // 1. HoanThanh, 2. PhieuTam

    self.RpSale_CheckDoanhThu = ko.observable(true);
    self.RpSale_CheckThucThu = ko.observable(true);

    self.ReportProduct_General = ko.observableArray();
    self.ReportProduct_Detail = ko.observableArray();
    self.ReportInvoice_General = ko.observableArray();
    self.ReportInvoice_Detail = ko.observableArray();
    self.ReportSales_General = ko.observableArray();
    self.ReportSales_Detail = ko.observableArray();
    self.ReportSales_ItemChosing = ko.observableArray([]);
    self.ReportDiscount_All = ko.observableArray();

    // sum footer
    self.ReportProduct_SumSoLuong = ko.observable(0);
    self.ReportProduct_SumThucHien = ko.observable(0);
    self.ReportProduct_SumThucHien_TheoYC = ko.observable(0);
    self.ReportProduct_SumTuVan = ko.observable(0);
    self.ReportProduct_SumBanGoi = ko.observable(0);
    self.ReportProduct_SumAll = ko.observable(0);
    self.ReportProduct_SumGiatriCK = ko.observable(0);
    self.ReportProduct_SumGiatriSauHeSo = ko.observable(0);

    self.ReportInvoice_TongGtriThucThu = ko.observable(0);
    self.ReportInvoice_SumDoanhThu = ko.observable(0);
    self.ReportInvoice_SumThucThu = ko.observable(0);
    self.ReportInvoice_SumVND = ko.observable(0);
    self.ReportInvoice_SumAll = ko.observable(0);

    self.ReportSales_SumDoanhThu = ko.observable(0);
    self.ReportSales_SumThucThu = ko.observable(0);
    self.ReportSales_SumHoaHongDT = ko.observable(0);
    self.ReportSales_SumHoaHongTT = ko.observable(0);
    self.ReportSales_SumAll = ko.observable(0);
    self.ReportSales_SumHHDoanhSo = ko.observable(0);

    self.role_XemBaoCao = ko.observable(false);

    // paging
    self.TotalRow = ko.observable(0);
    self.TotalPage = ko.observable(0);
    self.PageSizes = ko.observableArray([10, 20, 30]);
    self.CurrentPageSize = ko.observable(self.PageSizes()[0]);
    self.CurrentPage = ko.observable(0);
    self.FromItem = ko.observable(0);
    self.ToItem = ko.observable(0);

    function Page_Load() {
        SetDefault_HideColumn();
        LoadCheckBox(typeCheck);
        GetAllChiNhanh();
        GetHT_Quyen_ByNguoiDung();
        GetTree_NhomHangHoa();
        GetAllPhongBan();
        console.log('bchh')
    }

    function SetDefault_HideColumn() {
        var arrHideColumn = [];

        arrHideColumn = ['donvitinh', 'lohang', 'mahang', 'makhachhang', 'tenkhachhang', 'dienthoaikh', 'tennhomhang'];
        var cacheHideColumn = localStorage.getItem('Key_RpDiscountProduct_Detail');
        if (cacheHideColumn == null || cacheHideColumn === '[]') {
            // hide default some column
            for (var i = 0; i < arrHideColumn.length; i++) {
                LocalCaches.AddColumnHidenGrid('Key_RpDiscountProduct_Detail', arrHideColumn[i], arrHideColumn[i]);
            }
        }

        arrHideColumn = ['manhanvien', 'makhachhang', 'tenkhachhang', 'dienthoaikh'];
        cacheHideColumn = localStorage.getItem('Key_RpDiscountInvoice_Detail');
        if (cacheHideColumn == null || cacheHideColumn === '[]') {
            // hide default some column
            for (var i = 0; i < arrHideColumn.length; i++) {
                LocalCaches.AddColumnHidenGrid('Key_RpDiscountInvoice_Detail', arrHideColumn[i], arrHideColumn[i]);
            }
        }
    }

    function LoadCheckBox(typeCheck) {
        $.getJSON("api/DanhMuc/BaseApi/GetCheckedStatic?type=" + typeCheck, function (data) {
            self.ListCheckBox(data);
            self.NumberColum_Div2(Math.ceil(data.length / 2));
            LoadHtmlGrid();
        });
    }

    function LoadHtmlGrid() {
        LocalCaches.LoadFirstColumnGrid(Key_Form, $('#select-column ul li input[type = checkbox]'), self.ListCheckBox());
    }

    function GetAllChiNhanh() {
        ajaxHelper('/api/DanhMuc/DM_DonViAPI/' + "GetListDonViByIDNguoiDung?idnhanvien=" + _idNhanVien, 'GET').done(function (data) {

            var arrSortbyName = data.sort((a, b) => a.TenDonVi.localeCompare(b.TenDonVi, undefined, { caseFirst: "upper" }));
            self.ChiNhanhs(arrSortbyName);
            self.ArrDonVi(arrSortbyName);

            var obj = {
                ID: _idDonVi,
                TenDonVi: $('#_txtTenDonVi').html()
            }
            self.ChiNhanhChosed.push(obj);

            AddCheckAfterLi(_idDonVi);
        });
    }

    function GetHT_Quyen_ByNguoiDung() {
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetHT_NhomNguoiDung?idnguoidung=" + _userID + '&iddonvi=' + _idDonVi, 'GET').done(function (data) {
            if (data != null) {
                var allRole = data.HT_Quyen_NhomDTO;
                self.Quyen_NguoiDung(allRole);

                SearchReport();
            }
            else {
                ShowMessage_Danger('Không có quyền xem báo cáo');
            }
        });
    }

    function GetTree_NhomHangHoa() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/DM_NhomHangHoaAPI/' + 'GetTree_NhomHangHoa', 'GET').done(function (obj) {
                if (obj.res === true) {
                    let data = obj.data;
                    if (data.length > 0) {
                        data = data.sort((a, b) => a.text.localeCompare(b.text, undefined, { caseFirst: "upper" }));
                    }
                    self.NhomHangHoas(data);
                }
            })
        }
    }

    var tree = '';
    self.ListDepartment = ko.observableArray();
    self.ListDepartmentID = ko.observableArray([]);
    function GetAllPhongBan() {
        $.getJSON('/api/DanhMuc/NS_NhanVienAPI/' + "GetTreePhongBan?chinhanhId=" + VHeader.IdDonVi, function (data) {
            self.ListDepartment(data);

            tree = $('#treePhongBan1').tree({
                primaryKey: 'id',
                uiLibrary: 'bootstrap',
                dataSource: data,
                checkboxes: false,
            }).on('select', function (e, node, id) {
                reportDiscount.PhongBan_getAllChild(id);
            })
        });
    }

    self.PhongBan_getAllChild = function (idNhom) {
        var arrID = [];
        var nhom = $.grep(self.ListDepartment(), function (x) {
            return x.id === idNhom;
        });
        if (nhom.length > 0) {
            for (let i = 0; i < nhom[0].children.length; i++) {
                arrID.push(nhom[0].children[i].id);

                for (let j = 0; j < nhom[0].children[i].children.length; j++) {
                    arrID.push(nhom[0].children[i].children[j].id);
                }
            }
        }
        arrID.push(idNhom);
        self.ListDepartmentID(arrID);
        SearchReport();
    }

    function GetChildren_Phong(arrParent, arrJson, txtSearch, arr, isRoot) {
        if (txtSearch === '') {
            //return self.ListDepartment();
        }
        for (let i = 0; i < arrJson.length; i++) {
            let tenNhom = locdau(arrJson[i].text);
            if (tenNhom.indexOf(txtSearch) > -1) {
                if (isRoot) {
                    arr.push(arrJson[i]);
                }
                else {
                    var ex = $.grep(arr, function (x) {
                        return x.id === arrParent.id;
                    })
                    if (ex.length === 0) {
                        arr.push(arrParent);
                    }
                    else {
                        // neu da ton tai, thoat vong for of children
                        return;
                    }
                }
            }
            if (arrJson[i].children.length > 0) {
                GetChildren_Phong(arrJson[i], arrJson[i].children, txtSearch, arr, false);
            }
        }
        return arr;
    }

    $('#txtSearchPB1').keypress(function (e) {
        if (e.keyCode === 13) {
            var filter = locdau($(this).val());
            var arr = GetChildren_Phong([], self.ListDepartment(), filter, [], true);
            tree.destroy();
            tree = $('#treePhongBan1').tree({
                primaryKey: 'id',
                uiLibrary: 'bootstrap',
                dataSource: arr,
                checkboxes: false,
            }).on('select', function (e, node, id) {
                reportDiscount.PhongBan_getAllChild(id);
            });
        }
    });

    self.selectAllPhongBan = function () {
        self.ListDepartmentID([]);
        SearchReport();
    }

    function AddCheckAfterLi(idChiNhanh) {
        $('#selec-all-DonVi li').each(function () {
            if ($(this).attr('id') === idChiNhanh) {
                $(this).find('.fa-check').remove();
                $(this).append(_elmCheckAfterLi);
            }
        });
        $('#choose_TenDonVi input').remove();
    }

    self.SearchNhomHH = function (item) {
        var txtSearch = locdau(self.fiterNhomHH());
        var itemSearch = locdau(item.text);

        let txtChild = '';
        for (let i = 0; i < item.children.length; i++) {
            txtChild += locdau(item.children[i].text) + ' ';
        }
        let arr = itemSearch.indexOf(txtSearch) > -1 ||
            txtChild.indexOf(txtSearch) > -1;
    }

    self.arrFilterNhomHH = ko.computed(function () {
        var _filter = locdau(self.fiterNhomHH());
        return ko.utils.arrayFilter(self.NhomHangHoas(), function (prop) {
            var chon = true;
            let parent = locdau(prop.text);
            var txtChilds = '';

            for (var i = 0; i < prop.children.length; i++) {
                txtChilds += locdau(prop.children[i].text) + ' ';
            }

            if (_filter) {
                chon = (parent.indexOf(_filter) >= 0 ||
                    txtChilds.indexOf(_filter) >= 0
                )
            }
            return chon;
        })
    });

    self.ClickNhomHang = function (item) {
        let $this = event.currentTarget;
        let isCheck = $($this).find('input').is(':checked');

        if (item.id == undefined) {
            $('.li-oo').removeClass("yellow")
            $('#tatcanhh a').css("display", "block");
            $('#tatcanhh').addClass("yellow");
            self.IDNhomHangChosed([]);

            $('#treeviewnhomhang input').prop("checked", isCheck);
        }
        else {
            let arrID = [];
            $('.ss-li .li-oo').removeClass("yellow");
            $('#tatcanhh').removeClass("yellow")
            $('.li-pp').removeClass("yellow");
            $('#tatcanhh a').css("display", "none");
            $('#' + item.id).addClass("yellow");

            arrID.push(item.id);
            for (let i = 0; i < item.children.length; i++) {
                arrID.push(item.children[i].id);
                for (let j = 0; j < item.children[i].children.length; j++) {
                    arrID.push(item.children[i].children[j].id);
                }
            }

            if (isCheck) {
                for (let i = 0; i < arrID.length; i++) {
                    if ($.inArray(arrID[i], self.IDNhomHangChosed()) === -1) {
                        self.IDNhomHangChosed.push(arrID[i]);
                    }
                }
            }
            else {
                for (let i = 0; i < arrID.length; i++) {
                    if ($.inArray(arrID[i], self.IDNhomHangChosed()) > -1) {
                        self.IDNhomHangChosed.remove(arrID[i]);
                    }
                }
            }

            // set check child
            for (let i = 0; i < arrID.length; i++) {
                $('#' + arrID[i]).children('label').children('input').prop("checked", isCheck);
            }
        }
        ResetInforSearch();
        SearchReport();
    }

    self.selectedCN = function (item) {
        event.stopPropagation();
        var sTenChiNhanhs = '';
        var arrIDSearch = [];
        var arrDV = [];
        if (item.ID === undefined) {
            arrIDSearch = $.map(self.ChiNhanhs(), function (x) {
                return x.ID;
            });
            arrDV = self.ArrDonVi().map(function (x) {
                return x.ID;
            })
            // push again lstDV has chosed
            for (let i = 0; i < self.ChiNhanhChosed().length; i++) {
                if ($.inArray(self.ChiNhanhChosed()[i].ID, arrDV) === -1 && self.ChiNhanhChosed()[i].ID !== '00000000-0000-0000-0000-0000-000000000000') {
                    self.ArrDonVi().unshift(self.ChiNhanhChosed()[i]);
                }
            }
            self.ChiNhanhChosed([{
                ID: '00000000-0000-0000-0000-0000-000000000000', TenDonVi: 'Tất cả chi nhánh'
            }]);
            sTenChiNhanhs = 'Tất cả chi nhánh';
        }
        else {

            for (var i = 0; i < self.ChiNhanhChosed().length; i++) {
                sTenChiNhanhs += self.ChiNhanhChosed()[i].TenDonVi + ',';
                if ($.inArray(self.ChiNhanhChosed()[i].ID, arrDV) === -1) {
                    arrDV.push(self.ChiNhanhChosed()[i].ID);
                }
                if (self.ChiNhanhChosed()[i].ID === '00000000-0000-0000-0000-0000-000000000000') {
                    self.ChiNhanhChosed().splice(i, 1);
                }
            }
            if ($.inArray(item.ID, arrDV) === -1) {
                self.ChiNhanhChosed.push(item);
                sTenChiNhanhs += item.TenDonVi + ', '; // used to bind at head Report
            }
            sTenChiNhanhs = Remove_LastComma(sTenChiNhanhs);
            arrIDSearch = $.map(self.ChiNhanhChosed(), function (x) {
                return x.ID;
            });
        }
        // remove donvi has chosed in lst
        var arr = $.grep(self.ArrDonVi(), function (x) {
            return x.ID !== item.ID;
        });
        self.ArrDonVi(arr);
        self.LstIDDonVi(arrIDSearch);
        self.TenChiNhanhs(sTenChiNhanhs);
        event.preventDefault();
        return false;
    }

    self.CloseDV = function (item) {
        self.ChiNhanhChosed.remove(item);
        var arrID = [];
        var sTenChiNhanhs = '';
        if (item.ID === '00000000-0000-0000-0000-0000-000000000000') {
            arrID = $.map(self.DonVis(), function (x) {
                return x.ID;
            });
            sTenChiNhanhs = 'Tất cả chi nhánh';
        }
        else {
            self.ArrDonVi.unshift(item);

            if (self.ChiNhanhChosed().length === 0) {
                $('#choose_TenDonVi').append('<input type="text" id="NoteNameDonVi" readonly="readonly" class="dropdown" placeholder="Chọn chi nhánh">');

                arrID = $.map(self.DonVis(), function (x) {
                    return x.ID;
                });
                sTenChiNhanhs = 'Tất cả chi nhánh';
            }
            else {
                for (var i = 0; i < self.ChiNhanhChosed().length; i++) {
                    sTenChiNhanhs += self.ChiNhanhChosed()[i].TenDonVi + ', ';
                }
                sTenChiNhanhs = Remove_LastComma(sTenChiNhanhs);
                arrID = $.map(self.ChiNhanhChosed(), function (x) {
                    return x.ID;
                });
            }
            // remove check
            $('#selec-all-DonVi li').each(function () {
                if ($(this).attr('id') === item.ID) {
                    $(this).find('.fa-check').remove();
                }
            });
        }
        self.TenChiNhanhs(sTenChiNhanhs);
        self.LstIDDonVi(arrID);
        SearchReport();
    }

    self.RdoTypeTime.subscribe(function (newVal) {
        SearchReport();
    });

    $('.choose_txtTime li').on('click', function () {
        $('.ip_TimeReport').val($(this).text());
        self.LiInput_Time($(this).val());
        SearchReport();
    });

    $('.ip_DateReport ').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        SearchReport();
    });

    $('#txtSearch').keypress(function (e) {
        var keyPress = e.keyCode || e.which;
        if (keyPress === 13) {
            self.CurrentPage(0);
            SearchReport();
        }
    })

    $('#txtSearchHH').keypress(function (e) {
        var keyPress = e.keyCode || e.which;
        if (keyPress === 13) {
            self.CurrentPage(0);
            SearchReport();
        }
    })

    self.ClickBtnSearch = function () {
        SearchReport();
    }

    $('#select-column').on('change', 'ul li input[type = checkbox]', function () {
        var valueCheck = $(this).val();
        // valueCheck (1) = class name, valueCheck(2) = value  --> pass to func 
        // add/remove class is hidding in list cache {NameClass, Value}
        LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, valueCheck);
        //$('.' + valueCheck).toggle();
        SearchReport(false, valueCheck);
    });


    $('#select-column').on('click', 'ul li', function (i) {
        if ($(this).find('input[type = checkbox]').is(':checked')) {
            $(this).find('input[type = checkbox]').prop("checked", false);
        }
        else {
            $(this).find('input[type = checkbox]').prop("checked", true);
        }
        var valueCheck = $(this).find('input[type = checkbox]').val();
        LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, valueCheck);
        //$('.' + valueCheck).toggle();
        SearchReport(false, valueCheck);
    });

    function ResetInforSearch() {
        self.CurrentPage(0);
        self.CurrentPageSize(self.PageSizes()[0]);
    }

    function Load_ReprotProduct_General(array_Seach, valueCheck) {
        valueCheck = '' || valueCheck;
        HideShow_btnExportExcel('BCCKHangHoa_XuatTongHop');
        $('#table_hanghoa_tonghop').gridLoader();
        ajaxHelper(ReportUri + "ReportDiscountProduct_General", "POST", array_Seach).done(function (data) {
            $('#table_hanghoa_tonghop').gridLoader({ show: false });
            if (data.res == true) {
                self.ReportProduct_General(data.LstData);

                self.TotalRow(data.TotalRow);
                self.TotalPage(data.TotalPage);

                self.ReportProduct_SumThucHien(data.SumThucHien);
                self.ReportProduct_SumThucHien_TheoYC(data.SumThucHien_TheoYC);
                self.ReportProduct_SumTuVan(data.SumTuVan);
                self.ReportProduct_SumBanGoi(data.SumBanGoi);
                self.ReportProduct_SumAll(data.SumAll);

                GetListNumberPaging();
                Caculator_FromToPaging(data.LstData);
                LoadCheckBox(typeCheck);// hide/show colum after finish load data
                self.role_XemBaoCao(CheckRoleExist('BCCKHangHoa_TongHop'));
            }
        });
    }

    function Load_ReprotProduct_Detail(array_Seach, valueCheck) {
        valueCheck = '' || valueCheck;
        HideShow_btnExportExcel('BCCKHangHoa_XuatChiTiet');
        $('#table_hanghoa_chitiet').gridLoader();
        ajaxHelper(ReportUri + "ReportDiscountProduct_Detail", "POST", array_Seach).done(function (data) {
            $('#table_hanghoa_chitiet').gridLoader({ show: false });
            if (data.res == true) {
                self.ReportProduct_Detail(data.LstData);

                self.TotalRow(data.TotalRow);
                self.TotalPage(data.TotalPage);

                self.ReportProduct_SumSoLuong(data.TongSoLuong);
                self.ReportProduct_SumThucHien(data.SumThucHien);
                self.ReportProduct_SumThucHien_TheoYC(data.SumThucHien_TheoYC);
                self.ReportProduct_SumTuVan(data.SumTuVan);
                self.ReportProduct_SumBanGoi(data.SumBanGoi);
                self.ReportProduct_SumAll(data.TongAll);
                self.ReportProduct_SumGiatriCK(data.SumGtriCK);
                self.ReportProduct_SumGiatriSauHeSo(data.SumThanhTienSauHS);

                GetListNumberPaging();
                Caculator_FromToPaging(data.LstData);
                LoadCheckBox(typeCheck);
                self.role_XemBaoCao(CheckRoleExist('BCCKHangHoa_ChiTiet'));

                $('.' + valueCheck).toggle();
            }
        });
    }

    function Load_ReprotInvoice_General(array_Seach, valueCheck) {
        valueCheck = '' || valueCheck;
        HideShow_btnExportExcel('BCCKHoaDon_XuatTongHop');
        $('#table_hoadon_tonghop').gridLoader();
        ajaxHelper(ReportUri + "ReportDiscountInvoice_General", "POST", array_Seach).done(function (data) {
            $('#table_hoadon_tonghop').gridLoader({ show: false });
            if (data.res == true) {
                self.ReportInvoice_General(data.LstData);

                self.TotalRow(data.TotalRow);
                self.TotalPage(data.TotalPage);

                self.ReportInvoice_SumDoanhThu(data.SumDoanhThu);
                self.ReportInvoice_SumThucThu(data.SumThucThu);
                self.ReportInvoice_SumVND(data.SumVND);
                self.ReportInvoice_SumAll(data.SumAll);

                GetListNumberPaging();
                Caculator_FromToPaging(data.LstData);
                LoadCheckBox(typeCheck);

                self.role_XemBaoCao(CheckRoleExist('BCCKHoaDon_TongHop'));

                $('.' + valueCheck).toggle();
            }
        });
    }

    function Load_ReprotInvoice_Detail(array_Seach, valueCheck) {
        valueCheck = '' || valueCheck;

        HideShow_btnExportExcel('BCCKHoaDon_XuatChiTiet');
        $('#table_hoadon_chitiet').gridLoader();
        ajaxHelper(ReportUri + "ReportDiscountInvoice_Detail", "POST", array_Seach).done(function (data) {
            $('#table_hoadon_chitiet').gridLoader({ show: false });
            if (data.res == true) {
                self.ReportInvoice_Detail(data.LstData);

                self.TotalRow(data.TotalRow);
                self.TotalPage(data.TotalPage);

                self.ReportProduct_SumGiatriCK(data.DoanhThuHD);
                self.ReportInvoice_TongGtriThucThu(data.TongThucThuHD);
                self.ReportInvoice_SumDoanhThu(data.SumCPNganHang);// muon truong
                self.ReportProduct_SumGiatriSauHeSo(data.SumThucThu_ThucTinh);// muon truong
                self.ReportInvoice_SumThucThu(data.SumThucThu);
                self.ReportInvoice_SumVND(data.SumVND);
                self.ReportInvoice_SumAll(data.SumAll);

                GetListNumberPaging();
                Caculator_FromToPaging(data.LstData);
                LoadCheckBox(typeCheck);

                self.role_XemBaoCao(CheckRoleExist('BCCKHoaDon_ChiTiet'));

                $('.' + valueCheck).toggle();
            }
        });
    }

    function Load_ReprotAll(array_Seach) {
        //HideShow_btnExportExcel('BCCKAll_XuatExcel');
        $('.table-reponsive').gridLoader();
        ajaxHelper(ReportUri + "ReportDiscountAll", "POST", array_Seach).done(function (data) {
            $('.table-reponsive').gridLoader({ show: false });
            if (data.res == true) {
                self.ReportDiscount_All(data.LstData);

                self.TotalRow(data.TotalRow);
                self.TotalPage(data.TotalPage);

                self.ReportProduct_SumThucHien(data.TongHHThucHien);
                self.ReportProduct_SumThucHien_TheoYC(data.TongHHThucHien_TheoYC);
                self.ReportProduct_SumTuVan(data.TongHHTuVan);
                self.ReportProduct_SumBanGoi(data.TongHHBanGoiDV);
                self.ReportProduct_SumAll(data.TongHH_HangHoa);

                self.ReportInvoice_SumDoanhThu(data.TongHHDoanhThu);
                self.ReportInvoice_SumThucThu(data.TongHHThucThu);
                self.ReportInvoice_SumVND(data.TongHHVND);
                self.ReportInvoice_SumAll(data.TongHH_HoaDon);

                self.ReportSales_SumDoanhThu(data.TongDoanhThu);
                self.ReportSales_SumThucThu(data.TongThucThu);
                self.ReportSales_SumHoaHongDT(data.TongHHDoanhThuDS);
                self.ReportSales_SumHoaHongTT(data.TongHHThucThuDS);
                self.ReportSales_SumHHDoanhSo(data.TongHH_DoanhSo);
                self.ReportSales_SumAll(data.TongHH_All);

                GetListNumberPaging();
                Caculator_FromToPaging(data.LstData);
                //LoadCheckBox(typeCheck);

                self.role_XemBaoCao(CheckRoleExist('BCCK_TongHop'));
            }
        });
    }

    // báo cáo doanh số nhân viên
    function Load_ReprotSales(array_Seach) {
        HideShow_btnExportExcel('BCCKDoanhThu_XuatTongHop');
        $('#theodoanhso').gridLoader();
        ajaxHelper(ReportUri + "ReportDiscountSales", "POST", array_Seach).done(function (data) {
            $('#theodoanhso').gridLoader({ show: false });
            if (data.res == true) {
                self.ReportSales_General(data.LstData);

                self.TotalRow(data.TotalRow);
                self.TotalPage(data.TotalPage);

                self.ReportSales_SumDoanhThu(data.SumDoanhThu);
                self.ReportSales_SumThucThu(data.SumThucThu);
                self.ReportSales_SumHoaHongDT(data.SumHHDoanhThu);
                self.ReportSales_SumHoaHongTT(data.SumHHThucThu);
                self.ReportSales_SumAll(data.SumAll);

                GetListNumberPaging();
                Caculator_FromToPaging(data.LstData);
                LoadCheckBox(typeCheck);

                self.role_XemBaoCao(CheckRoleExist('BCCKDoanhThu'));
            }
        });
    }

    function Load_ReprotSales_Detail(array_Seach) {
        var role = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen === 'BCCKDoanhThu_XuatChiTiet';
        });
        if (role.length > 0) {
            $('.btnExportDetail').show();
        }
        else {
            $('.btnExportDetail').hide();
        }

        ajaxHelper(ReportUri + "ReportDiscountSales_Detail", "POST", array_Seach).done(function (data) {
            if (data.res == true) {
                self.ReportSales_Detail(data.LstData);

                self.TotalRow(data.TotalRow);
                self.TotalPage(data.TotalPage);

                GetListNumberPaging();
                Caculator_FromToPaging(data.LstData);
                LoadCheckBox(typeCheck);

                self.role_XemBaoCao(CheckRoleExist('BCCKDoanhThu_ChiTiet'));
            }
        });
    }

    function HideShow_btnExportExcel(maQuyen) {
        var role = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen === maQuyen;
        });
        if (role.length > 0) {
            $('#btnExport').show();
        }
        else {
            $('#btnExport').hide();
        }
    }

    function GetParamSearch() {
        var idChiNhanhs = '';
        for (var i = 0; i < self.ChiNhanhChosed().length; i++) {
            idChiNhanhs += self.ChiNhanhChosed()[i].ID + ',';
        }
        // avoid error in Store procedure
        if (idChiNhanhs === '') {
            idChiNhanhs = _idDonVi;
        }
        idChiNhanhs = Remove_LastComma(idChiNhanhs);

        var _now = new Date();  //current date of week
        var currentWeekDay = _now.getDay();
        var lessDays = currentWeekDay === 0 ? 6 : currentWeekDay - 1;
        var dayStart = '';
        var dayEnd = '';

        if (self.RdoTypeTime() === '1') {
            var dateFrom = '';
            var dateTo = '';

            switch (self.LiInput_Time()) {
                case 0:
                    // all
                    self.TodayBC('Toàn thời gian');
                    dayStart = '20160101';
                    dayEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
                    break;
                case 1:
                    // hom nay
                    dayStart = dayEnd = moment(_now).format('YYYY-MM-DD');
                    self.TodayBC('Ngày: '.concat(moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY')));
                    break;
                case 2:
                    // hom qua
                    dayStart = dayEnd = moment(_now).subtract('days', 1).format('YYYY-MM-DD');
                    self.TodayBC('Ngày: '.concat(moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY')));
                    break;
                case 3:
                    // tuan nay
                    dayStart = moment().startOf('week').add('days', 1).format('YYYY-MM-DD');
                    dayEnd = moment().endOf('week').add('days', 1).format('YYYY-MM-DD');

                    dateFrom = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    dateTo = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.TodayBC('Từ ' + dateFrom + ' Đến ' + dateTo);
                    break;
                case 4:
                    // tuan truoc
                    dayStart = moment().weekday(-6).format('YYYY-MM-DD');
                    dayEnd = moment(dayStart, 'YYYY-MM-DD').add(6, 'days').format('YYYY-MM-DD'); // add day in moment.js

                    dateFrom = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    dateTo = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.TodayBC('Từ ' + dateFrom + ' Đến ' + dateTo);
                    break;
                case 5:
                    // 7 ngay qua
                    dayEnd = moment(_now).format('YYYY-MM-DD');
                    dayStart = moment(_now).subtract('days', 6).format('YYYY-MM-DD');

                    dateFrom = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    dateTo = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.TodayBC('Từ ' + dateFrom + ' Đến ' + dateTo);
                    break;
                case 6:
                    // thang nay
                    dayStart = moment().startOf('month').format('YYYY-MM-DD');
                    dayEnd = moment().endOf('month').format('YYYY-MM-DD'); // add them 1 ngày 01-month-year --> compare in SQL

                    dateFrom = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    dateTo = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.TodayBC('Từ ' + dateFrom + ' Đến ' + dateTo);
                    break;
                case 7:
                    // thang truoc
                    dayStart = moment().subtract('months', 1).startOf('month').format('YYYY-MM-DD');
                    dayEnd = moment().subtract('months', 1).endOf('month').format('YYYY-MM-DD');

                    dateFrom = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    dateTo = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.TodayBC('Từ ' + dateFrom + ' Đến ' + dateTo);
                    break;
                case 10:
                    // quy nay
                    dayStart = moment().startOf('quarter').format('YYYY-MM-DD');
                    dayEnd = moment().endOf('quarter').format('YYYY-MM-DD');

                    dateFrom = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    dateTo = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.TodayBC('Từ ' + dateFrom + ' Đến ' + dateTo);
                    break;
                case 11:
                    // quy truoc = currQuarter -1; // if (currQuarter -1 == 0) --> (assign = 1)
                    var prevQuarter = moment().quarter() - 1;
                    if (prevQuarter === 0) {
                        // get quy 4 cua nam truoc 01/10/... -->  31/21/...
                        var prevYear1 = moment().year() - 1;
                        dayStart = prevYear1 + '' + '1001';
                        dayEnd = prevYear1 + '' + '1231';
                    }
                    else {
                        dayStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                        dayEnd = moment().quarter(prevQuarter).endOf('quarter').format('YYYY-MM-DD');
                    }

                    dateFrom = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    dateTo = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.TodayBC('Từ ' + dateFrom + ' Đến ' + dateTo);
                    break;
                case 12:
                    // nam nay
                    dayStart = moment().startOf('year').format('YYYY-MM-DD');
                    dayEnd = moment().endOf('year').add('days').format('YYYY-MM-DD');

                    dateFrom = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    dateTo = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.TodayBC('Từ ' + dateFrom + ' Đến ' + dateTo);
                    break;
                case 13:
                    // nam truoc
                    var prevYear = moment().year() - 1;
                    dayStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                    dayEnd = moment().year(prevYear).endOf('year').format('YYYY-MM-DD');

                    dateFrom = moment(dayStart, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    dateTo = moment(dayEnd, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    self.TodayBC('Từ ' + dateFrom + ' Đến ' + dateTo);
                    break;
            }
        }
        else {
            var arrDate = $('.ip_DateReport').val().split('-');
            dayStart = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
            dayEnd = moment(arrDate[1], 'DD/MM/YYYY').format('YYYY-MM-DD');

            self.TodayBC('Từ ' + moment(arrDate[0], 'DD/MM/YYYY').format('DD/MM/YYYY') + ' đến ' + moment(arrDate[1], 'DD/MM/YYYY').format('DD/MM/YYYY'));
        }
        return {
            ID_ChiNhanhs: idChiNhanhs,
            FromDate: dayStart,
            ToDate: dayEnd,
        }
    }

    self.SearchReport = function () {
        SearchReport(false);
    }

    function CheckRoleExist(maquyen) {
        var role = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen.indexOf(maquyen) > -1;
        });
        return role.length > 0;
    }

    //$('#btnExport').click(function () {
    //    let tblHtml = document.getElementById('tblHoaHongHHCT');
    //    console.log('wb ', tblHtml)
    //    // keep all cell format as like html table: { raw: true },  { cellStyles: true }
    //    // use https://unpkg.com/xlsx@0.18.2/dist/xlsx.full.min.js
    //    var wb = XLSX.utils.table_to_book(tblHtml, { raw: true });
    //    delete (wb['O1'])
    //    XLSX.writeFile(wb, 'sample.xlsx', { cellStyles: true });
    //    return false;
    //});

    function SearchReport(isExport, valueHideColum, itemDetail) {
        isExport = isExport || false;
        valueHideColum = '' || valueHideColum;
        itemDetail = itemDetail || null;// BC doanhthu chitiet

        var txtSearch = $('#txtSearch').val();
        if (txtSearch == null || txtSearch == undefined) {
            txtSearch = '';
        }

        var txtSearchHH = $('#txtSearchHH').val();
        if (txtSearchHH == null || txtSearchHH == undefined) {
            txtSearchHH = '';
        }
        txtSearch = txtSearch.trim();

        var commonParam = GetParamSearch();

        // chỉ xem báo cáo theo DoanhThu, ThucThu, hoac VND
        var status = 0;
        if (self.IsCheck_DoanhThu()) {
            if (self.IsCheck_ThucThu()) {
                if (self.IsCheck_VND()) {
                    status = 0;
                }
                else {
                    status = 1;
                }
            }
            else {
                if (self.IsCheck_VND()) {
                    status = 2;
                }
                else {
                    status = 3;
                }
            }
        }
        else {
            if (self.IsCheck_ThucThu()) {
                if (self.IsCheck_VND()) {
                    status = 4;
                }
                else {
                    status = 5;
                }
            }
            else {
                if (self.IsCheck_VND()) {
                    status = 6;
                }
                else {
                    status = 7;
                }
            }
        }

        // tính lại tổng theo trạng thái cột ẩn
        var typeReport = parseInt(self.TypeReport());
        if (typeReport === 3) {
            if (self.RpSale_CheckThucThu()) {
                if (self.RpSale_CheckDoanhThu()) {
                    status = 0;
                }
                else {
                    status = 1;
                }
            }
            else {
                if (self.RpSale_CheckDoanhThu()) {
                    status = 2;
                }
                else {
                    status = 3;
                }
            }
        }

        var status_columhide = 0;
        var cacheHideColumn2 = localStorage.getItem(Key_Form);
        if (cacheHideColumn2 !== null) {
            cacheHideColumn2 = JSON.parse(cacheHideColumn2);

            var arrcolumn = [];
            for (var i = 0; i < cacheHideColumn2.length; i++) {
                arrcolumn.push(cacheHideColumn2[i].Value)
            }

            switch (typeReport) {
                case 1:
                    if ($.inArray('hoahongthuchien', arrcolumn) > -1) {
                        if ($.inArray('hoahongtuvan', arrcolumn) > -1) {
                            if ($.inArray('hoahongdichvu', arrcolumn) > -1) {
                                if ($.inArray('hoahongthuchien_theoyc', arrcolumn) > -1) {
                                    status_columhide = 1;// tong = 0
                                }
                                else {
                                    status_columhide = 2;// thuchien_theoyc
                                }
                            }
                            else {
                                if ($.inArray('hoahongthuchien_theoyc', arrcolumn) > -1) {
                                    status_columhide = 3;// bangoi
                                }
                                else {
                                    status_columhide = 4;// thuchien_theoyc + bangoi
                                }
                            }
                        }
                        else {
                            if ($.inArray('hoahongdichvu', arrcolumn) > -1) {
                                if ($.inArray('hoahongthuchien_theoyc', arrcolumn) > -1) {
                                    status_columhide = 5;// tuvan
                                }
                                else {
                                    status_columhide = 6;// thuchien_theoyc + tuvan
                                }
                            }
                            else {
                                if ($.inArray('hoahongthuchien_theoyc', arrcolumn) > -1) {
                                    status_columhide = 7;// bangoi + tuvan
                                }
                                else {
                                    status_columhide = 8;// bangoi + tuvan + thuchien_theoyc
                                }
                            }
                        }
                    }
                    else {
                        if ($.inArray('hoahongtuvan', arrcolumn) > -1) {
                            if ($.inArray('hoahongdichvu', arrcolumn) > -1) {
                                if ($.inArray('hoahongthuchien_theoyc', arrcolumn) > -1) {
                                    status_columhide = 9;// thuchien
                                }
                                else {
                                    status_columhide = 10;// thuchien + thuchien_theoyc
                                }
                            }
                            else {
                                if ($.inArray('hoahongthuchien_theoyc', arrcolumn) > -1) {
                                    status_columhide = 11;// thuchien + bangoi
                                }
                                else {
                                    status_columhide = 12;// thuchien + bangoi + thuchien_theoyc
                                }
                            }
                        }
                        else {
                            if ($.inArray('hoahongdichvu', arrcolumn) > -1) {
                                if ($.inArray('hoahongthuchien_theoyc', arrcolumn) > -1) {
                                    status_columhide = 13;// thuchien + tuvan
                                }
                                else {
                                    status_columhide = 14;// thuchien + tuvan + thuchien_theoyc
                                }
                            }
                            else {
                                if ($.inArray('hoahongthuchien_theoyc', arrcolumn) > -1) {
                                    status_columhide = 15;// thuchien + tuvan + bangoi
                                }
                                else {
                                    status_columhide = 16;// all
                                }
                            }
                        }
                    }
                    break;
                case 2:
                    if ($.inArray('hoahongdoanhthu', arrcolumn) > -1) {
                        if ($.inArray('hoahongthucthu', arrcolumn) > -1) {
                            if ($.inArray('hoahongvnd', arrcolumn) > -1) {
                                status_columhide = 1; // tong = 0
                            }
                            else {
                                status_columhide = 2; // vnd
                            }
                        }
                        else {
                            if ($.inArray('hoahongvnd', arrcolumn) > -1) {
                                status_columhide = 3;// chi thucthu
                            }
                            else {
                                status_columhide = 4;// thucthu + vnd
                            }
                        }
                    }
                    else {
                        if ($.inArray('hoahongthucthu', arrcolumn) > -1) {
                            if ($.inArray('hoahongvnd', arrcolumn) > -1) {
                                status_columhide = 5; // doanh thu
                            }
                            else {
                                status_columhide = 6; // doanhthu + vnd
                            }
                        }
                        else {
                            if ($.inArray('hoahongvnd', arrcolumn) > -1) {
                                status_columhide = 7; // doanh thu + thuc thu
                            }
                            else {
                                status_columhide = 8; // doanhthu + thucthu +vnd
                            }
                        }
                    }
                    break;
            }
        }

        let idNhomHang = '';
        if (self.IDNhomHangChosed().length > 0) {
            for (let i = 0; i < self.IDNhomHangChosed().length; i++) {
                idNhomHang += self.IDNhomHangChosed()[i] + ', ';
            }
            idNhomHang = Remove_LastComma(idNhomHang);
        }
        else {
            idNhomHang = '%%';
        }

        var arr = [];
        if (self.LaHangHoa()) {
            arr.push(1);
        }
        if (self.LaDichVu()) {
            arr.push(2);
        }
        if (self.LaCombo()) {
            arr.push(3);
        }

        var arrChungTu = [];
        if (self.MangChungTuChosed().length === 0
            || (self.MangChungTuChosed().length === 1 && self.MangChungTuChosed()[0].ID === 0)) {
            arrChungTu = self.MangChungTu().map(function (x) {
                return x.ID;
            })
        }
        else {
            arrChungTu = self.MangChungTuChosed().map(function (x) {
                return x.ID;
            })
        }

        var array_Seach = {
            TextSearch: txtSearch,
            HangHoaSearch: txtSearchHH,
            DateFrom: commonParam.FromDate,
            DateTo: commonParam.ToDate,
            ID_ChiNhanhs: commonParam.ID_ChiNhanhs,
            ID_NhomHang: idNhomHang,
            TrangThai: status,
            CurrentPage: self.CurrentPage(),
            PageSize: self.CurrentPageSize(),
            TodayBC: self.TodayBC(),
            TextReport: self.TenChiNhanhs(),
            ColumnsHide: _columnHide,
            TypeReport: 0,
            Status_ColumnHide: status_columhide,
            StatusInvoice: parseInt(self.TrangThaiHD()),
            LstIDChiNhanh: self.LstIDDonVi(),
            ID_NhanVienLogin: _idNhanVien,
            LaHangHoas: arr,
            LoaiChungTus: arrChungTu,
            DepartmentIDs: self.ListDepartmentID(),
        }

        if (isExport) {
            $('.table-reponsive').gridLoader();
            var funcExcel = '';
            var txtFunc = '';
            var detail = '';

            var lenData = 0;
            switch (typeReport) {
                case 1:
                    funcExcel = 'ExportExcel_ReportDiscountProduct';
                    if (self.IsReportDetail()) {
                        array_Seach.TypeReport = 2;
                        txtFunc = 'chi tiết theo hàng hóa';
                    }
                    else {
                        array_Seach.TypeReport = 1;//
                        txtFunc = 'tổng hợp theo hàng hóa';
                    }
                    break;
                case 2:
                    funcExcel = 'ExportExcel_ReportDiscountInvoice';
                    if (self.IsReportDetail()) {
                        array_Seach.TypeReport = 4;
                        txtFunc = 'chi tiết theo hóa đơn';
                    }
                    else {
                        array_Seach.TypeReport = 3;
                        txtFunc = 'tổng hợp theo hóa đơn';
                    }
                    break;
                case 3:
                    funcExcel = 'ExportExcel_ReportDiscountSales';
                    if (itemDetail == null) {
                        array_Seach.TypeReport = 5;
                        txtFunc = 'tổng hợp theo doanh thu';
                    }
                    else {
                        array_Seach.TextSearch = itemDetail.ID_NhanVien;// mượn trường
                        array_Seach.TextReport += ' (Nhân viên: '.concat(itemDetail.MaNhanVien, ' - ', itemDetail.TenNhanVien, ')');
                        array_Seach.TypeReport = 6;
                        txtFunc = 'chi tiết theo doanh thu';
                    }
                    break;
                case 4:
                    funcExcel = 'ExportExcel_ReportDiscountAll';
                    array_Seach.TypeReport = 7;
                    txtFunc = 'tổng hợp';
                    break;

            }

            array_Seach.PageSize = self.TotalRow();// export all row
            ajaxHelper(ReportUri + funcExcel, 'POST', array_Seach).done(function (obj) {
                $('.table-reponsive').gridLoader({ show: false });
                if (obj.res === true) {
                    self.DownloadFileTeamplateXLSX(obj.data);
                }
            })

            detail = 'Xuất báo cáo hoa hồng '.concat(txtFunc, ' .Thời gian: ', self.TodayBC(), ' .Chi nhánh: ', self.TenChiNhanhs(), ' .Người xuất: ', _userLogin);
            var objDiary = {
                ID_NhanVien: _idNhanVien,
                ID_DonVi: _idDonVi,
                ChucNang: "Báo cáo hoa hồng ".concat(txtFunc),
                NoiDung: 'Xuất báo cáo hoa hồng '.concat(txtFunc),
                NoiDungChiTiet: detail,
                LoaiNhatKy: 6
            };
            Insert_NhatKyThaoTac_1Param(objDiary);
        }
        else {
            $('#select-column').show();
            switch (typeReport) {
                case 1:
                    if (self.IsReportDetail()) {
                        Load_ReprotProduct_Detail(array_Seach, valueHideColum);
                    }
                    else {
                        Load_ReprotProduct_General(array_Seach, valueHideColum);
                    }
                    break;
                case 2:
                    if (self.IsReportDetail()) {
                        Load_ReprotInvoice_Detail(array_Seach, valueHideColum);
                    }
                    else {
                        Load_ReprotInvoice_General(array_Seach, valueHideColum);
                    }
                    break;
                case 3:
                    if (itemDetail == null) {
                        Load_ReprotSales(array_Seach);
                    }
                    else {
                        array_Seach.TextSearch = itemDetail.ID_NhanVien;
                        Load_ReprotSales_Detail(array_Seach);
                    }
                    break;
                case 4:
                    $('#select-column').hide();// xuat TongHop: khong cho an/hien cot
                    Load_ReprotAll(array_Seach);
                    break;
            }
        }
    }

    self.TrangThaiHD.subscribe(function () {
        self.CurrentPage(0);
        SearchReport(false);
    })

    self.LaHangHoa.subscribe(function () {
        self.CurrentPage(0);
        SearchReport(false);
    })
    self.LaDichVu.subscribe(function () {
        self.CurrentPage(0);
        SearchReport(false);
    })
    self.LaCombo.subscribe(function () {
        self.CurrentPage(0);
        SearchReport(false);
    })

    self.ChoseLoaiChungTu = function (item, event) {
        event.stopPropagation();
        var objAll = { ID: 0, TenChungTu: 'Tất cả' };
        if (item.ID === 0) {
            self.MangChungTuChosed([objAll]);
            self.ArrMangChungTu($.extend([], true, self.MangChungTu()));
        }
        else {
            self.MangChungTuChosed.push(item);
            let arr = $.grep(self.MangChungTuChosed(), function (x) {
                return x.ID !== 0;
            })
            self.MangChungTuChosed(arr);

            // push all if nost exists
            let exAll = $.grep(self.ArrMangChungTu(), function (x) {
                return x.ID === 0;
            })
            if (exAll.length === 0) {
                self.ArrMangChungTu.push(objAll);
            }
        }
        self.ArrMangChungTu.remove(item);
        event.preventDefault();
    }

    self.CloseLoaiChungTu = function (item) {
        self.MangChungTuChosed.remove(item);
        self.ArrMangChungTu.push(item);
        SearchReport(false);
    }

    self.ChangeTab = function (val) {
        self.IsReportDetail(val);
        ResetInforSearch();
        $('.divSearchHH').hide();
        $('.divSearchNhomHH').hide();
        $('.trangthaiHD').show();

        switch (parseInt(self.TypeReport())) {
            case 1:
                if (val == true) {
                    $('.divSearchHH').show();
                    $('.divSearchNhomHH').show();
                    $('.showChungTu, .jsLoaiHang').show();
                    Key_Form = 'Key_RpDiscountProduct_Detail';
                    typeCheck = 8;
                }
                else {
                    $('.showChungTu, .jsLoaiHang').hide();
                    Key_Form = 'Key_RpDiscountProduct';
                    typeCheck = 7;
                }
                break;
            case 2:
                //$('.trangthaiHD').show();
                if (val == true) {
                    Key_Form = 'Key_RpDiscountInvoice_Detail';
                    typeCheck = 10;

                }
                else {
                    Key_Form = 'Key_RpDiscountInvoice';
                    typeCheck = 9;
                }
                break;
        }
        SearchReport();
    }

    self.Load_ReportDiscountSale_Detail = function (item) {
        self.IsReportDetail(true);
        self.ReportSales_ItemChosing(item);
        SearchReport(false, '', item);
    }

    // paging
    function Caculator_FromToPaging(arrGird) {
        self.FromItem((self.CurrentPage() * self.CurrentPageSize()) + 1);

        if (((self.CurrentPage() + 1) * self.CurrentPageSize()) > arrGird.length) {
            var fromItem = (self.CurrentPage() + 1) * self.CurrentPageSize();
            if (fromItem < self.TotalRow()) {
                self.ToItem((self.CurrentPage() + 1) * self.CurrentPageSize());
            }
            else {
                self.ToItem(self.TotalRow());
            }
        } else {
            self.ToItem((self.CurrentPage() * self.CurrentPageSize()) + self.CurrentPageSize());
        }
    }

    self.ChangePageSize = function (item) {
        self.CurrentPage(0);
        SearchReport();
    };

    //self.TypeReport_Parent.subscribe(function (newVal) {
    //    if (newVal == '4') {
    //        self.TypeReport(4);
    //        $('#hoahongtonghop').addClass('active');
    //        $('#theohanghoa').removeClass('active');
    //    }
    //    else {
    //        self.TypeReport(1);
    //        $('#hoahongtonghop').removeClass('active');
    //        $('#theohanghoa').addClass('active');
    //    }
    //})
    /*$('ul.chose_kieubang li').unbind("click");*/
    $('ul.chose_kieubang li').click(function () {
        $('.chi-tiet-hoa-hong').hide();
        $(".chose_kieubang li").each(function (i) {
            $(this).find('a').removeClass('box-tab');
        });
        $(this).find('a').addClass('box-tab');
        $(this).find('.chi-tiet-hoa-hong').show();
        var thisVal = $(this).val();
        switch (thisVal) {
            case 1:
                $("#theohanghoa").show();
                $("#theohanghoa").siblings().hide();
                break;
            case 2:
                $("#theohoadon").show();
                $("#theohoadon").siblings().hide();
                break;
            case 3:
                $("#theodoanhso").show();
                $("#theodoanhso").siblings().hide();
                break;
            case 4:
                $("#hoahongtonghop").show();
                $("#hoahongtonghop").siblings().hide();
                break;

            default:
                $("#hoahongtonghop").show();
                $("#hoahongtonghop").siblings().hide();
                break;

        }
        ////if (thisVal === 5) {
        ////    $(".chose_kieubang li").each(function (i) {
        ////        if (i > 1) {
        ////            $(this).show();
        ////        }
        ////    });
        ////}
        ////else if (thisVal === 4) {
        ////    $(".chose_kieubang li.subReport").each(function (i) {

        ////            $(this).hide();

        ////    });
        ////}
        //// neu thay doi loai bao cao--> reset tab, nguoc lai khong reset
        $('.divSearchHH').hide();
        $('.divSearchNhomHH').hide();
        var loaiBC = self.TypeReport();
        switch (thisVal) {
            case 1:
            case 5:
                $('a[href = "#theohanghoa"]').addClass('box-tab');
                $('a[href = "#hoahongchitiet"]').addClass('box-tab');
                $('#theohanghoa').addClass('active');
                $('#hoahongtonghop').removeClass('active');
                self.IsReportDetail(false);
                $('#hdReport').text('Báo cáo tổng hợp hoa hồng nhân viên theo hàng hóa');
                Key_Form = 'Key_RpDiscountProduct';
                typeCheck = 7;

                $('#theohanghoa ul li').removeClass('active');
                $('#theohanghoa ul li:eq(0)').addClass('active');
                $('#table_hanghoa_chitiet').removeClass('active');
                $('#table_hanghoa_tonghop').addClass('active');
                $('.showChungTu,.jsLoaiHang, .trangthaiHD').hide();
                loaiBC = 1;
                break;
            case 2:
                if (loaiBC !== thisVal) {
                    $('#hdReport').text('Báo cáo tổng hợp hoa hồng nhân viên theo hóa đơn');
                    Key_Form = 'Key_RpDiscountInvoice';
                    typeCheck = 9;

                    $('#theohoadon ul li').removeClass('active');
                    $('#theohoadon ul li:eq(0)').addClass('active');
                    $('#table_hoadon_chitiet').removeClass('active');
                    $('#table_hoadon_tonghop').addClass('active');
                    $('.trangthaiHD, .showChungTu, .jsLoaiHang').hide();
                    loaiBC = 2;
                }
                break;
            case 3:
                if (loaiBC !== thisVal) {
                    $('#hdReport').text('Báo cáo tổng hợp hoa hồng nhân viên theo doanh số');
                    Key_Form = 'Key_RpDiscountSales';
                    typeCheck = 11;
                    loaiBC = 3;
                    $('.trangthaiHD, .showChungTu, .jsLoaiHang').hide();
                }
                break;
            case 4:
                if (loaiBC !== thisVal) {
                    $('#hdReport').text('Báo cáo tổng hợp hoa hồng nhân viên');
                    Key_Form = 'Key_RpDiscountAll';
                    typeCheck = 12;
                    loaiBC = 4;
                    $('.trangthaiHD, .showChungTu, .jsLoaiHang').hide();
                }
                break;
        }

        // nếu click checkbox DoanhThu/ThucThu/VND --> không reset tab
        if (self.TypeReport() !== thisVal) {
            self.IsReportDetail(false);
        }
        self.TypeReport(loaiBC);
        ResetInforSearch();
        SearchReport();
        /*return false;*/
    });

    self.PageListPaging = ko.observableArray();

    function GetListNumberPaging() {
        var arrPage = [];
        var allPage = self.TotalPage();
        var currentPage = self.CurrentPage();
        var totalRow = self.TotalRow();

        if (allPage > 4) {

            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.CurrentPage()) + 1;
            }
            else {
                i = self.CurrentPage();
            }

            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var k = allPage - 5; k < allPage; k++) {
                        var obj1 = {
                            pageNumber: k + 1,
                        };
                        arrPage.push(obj1);
                    }
                }
                else {
                    if (currentPage === 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj2 = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj2);
                        }
                    }
                    else {
                        // get currentPage - 2 , currentPage, currentPage + 2
                        for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj3 = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj3);
                        }
                    }
                }
            }
            else {
                // get 5 trang dau
                if (i >= 2) {
                    while (arrPage.length < 5) {
                        var obj4 = {
                            pageNumber: i - 1,
                        };
                        arrPage.push(obj4);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj5 = {
                            pageNumber: i,
                        };
                        arrPage.push(obj5);
                        i = i + 1;
                    }
                }
            }
        }
        else {
            // neu chi co 1 trang --> khong hien thi DS trang
            if (allPage > 1) {
                for (var m = 0; m < allPage; m++) {
                    var obj = {
                        pageNumber: m + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }

        self.PageListPaging(arrPage);
    }

    self.VisibleStartPage = ko.computed(function () {
        if (self.PageListPaging().length > 0) {
            return self.PageListPaging()[0].pageNumber !== 1;
        }
    });

    self.VisibleEndPage = ko.computed(function () {
        if (self.PageListPaging().length > 0) {
            return self.PageListPaging()[self.PageListPaging().length - 1].pageNumber !== self.TotalPage();
        }
    })

    self.ResetCurrentPage = function () {
        self.CurrentPage(0);
        SearchReport();
    };

    self.GoToPage = function (page) {
        self.CurrentPage(page.pageNumber - 1);
        SearchReport();
    };

    self.GetClass = function (page) {
        return ((page.pageNumber - 1) === self.CurrentPage()) ? "click" : "";
    };

    self.StartPage = function () {
        self.CurrentPage(0);
        SearchReport();
    }

    self.BackPage = function () {
        if (self.CurrentPage() > 1) {
            self.CurrentPage(self.CurrentPage() - 1);
            SearchReport();
        }
    }

    self.GoToNextPage = function () {
        if (self.CurrentPage() < self.TotalPage() - 1) {
            self.CurrentPage(self.CurrentPage() + 1);
            SearchReport();
        }
    }

    self.EndPage = function () {
        if (self.CurrentPage() < self.TotalPage() - 1) {
            self.CurrentPage(self.TotalPage() - 1);
            SearchReport();
        }
    }

    Page_Load();

    self.DownloadFileTeamplateXLSX = function (pathFile) {
        var url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + pathFile;
        window.location.href = url;
    }

    var _columnHide = '';
    self.ExportExcel = function () {

        _columnHide = '';
        var tblName = '';
        var cacheHideColumn2 = localStorage.getItem(Key_Form);
        if (cacheHideColumn2 !== null) {
            cacheHideColumn2 = JSON.parse(cacheHideColumn2);

            var arrColumn = [];
            for (var i = 0; i < cacheHideColumn2.length; i++) {
                var itemFor = cacheHideColumn2[i];

                // find in list checkbox
                for (var j = 0; j < self.ListCheckBox().length; j++) {
                    if (self.ListCheckBox()[j].Key === itemFor.Value && $.inArray(itemFor.Value, arrColumn) == -1) {
                        arrColumn.push(itemFor.Value);
                        _columnHide += j + '_';
                        break;
                    }
                }
            }
        }

        var lenData = 0;
        switch (parseInt(self.TypeReport())) {
            case 1:// hanghoa
                if (self.IsReportDetail()) {
                    lenData = self.ReportProduct_Detail().length;
                    let cloumAdd = '';
                    // neu an cot hoahongthuchien --> an luon 2 cot % + VND
                    if (_columnHide.indexOf('16') > -1) {
                        cloumAdd += '16_17_';
                    }
                    if (_columnHide.indexOf('17') > -1) {
                        cloumAdd += '18_19_';
                    }
                    if (_columnHide.indexOf('18') > -1) {
                        cloumAdd += '20_21_';
                    }
                    if (_columnHide.indexOf('19') > -1) {
                        cloumAdd += '22_23_';
                    }

                    if (_columnHide.indexOf('17') > -1 && cloumAdd.indexOf('16') == -1) {
                        _columnHide = _columnHide.replace('17', '');
                    }
                    if (_columnHide.indexOf('18') > -1 && cloumAdd.indexOf('19') == -1) {
                        _columnHide = _columnHide.replace('18', '');
                    }
                    if (_columnHide.indexOf('19') > -1 && cloumAdd.indexOf('18') == -1) {
                        _columnHide = _columnHide.replace('19', '');
                    }

                    _columnHide = _columnHide + cloumAdd;
                }
                else {
                    lenData = self.ReportProduct_General().length;
                }
                break;
            case 2:// hoadon
                if (self.IsReportDetail()) {
                    lenData = self.ReportInvoice_Detail().length;

                    let cloumAdd2 = '';
                    
                    if (_columnHide.indexOf('10') > -1) {
                        cloumAdd2 += '10_11_';
                    }
                    if (_columnHide.indexOf('13') > -1) {
                        cloumAdd2 += '14_15_';
                    }

                    if (_columnHide.indexOf('14') > -1 && cloumAdd2.indexOf('15') == -1) {
                        _columnHide = _columnHide.replace('14', '16');
                    }

                    if (_columnHide.indexOf('12') > -1) {
                        _columnHide = _columnHide.replace('12', '13');
                    }

                    if (_columnHide.indexOf('11') > -1 && cloumAdd2.indexOf('10') == -1) {
                        _columnHide = _columnHide.replace('11', '12');
                    }

                    _columnHide = _columnHide + cloumAdd2;
                }
                else {
                    lenData = self.ReportInvoice_General().length;
                }
                break;
            case 3:// doanhthu (tongquan)
                lenData = self.ReportSales_General().length;
                break;
            case 4:// chietkhau all
                lenData = self.ReportDiscount_All().length;
                break;
        }

        if (lenData == 0) {
            ShowMessage_Danger('Báo cáo không có dữ liệu');
            return false;
        }

        SearchReport(true);
    }

    self.ExportExcel_SalesDetail = function () {
        if (self.IsReportDetail() && self.ReportSales_General().length === 0) {
            ShowMessage_Danger('Báo cáo không có dữ liệu');
            return false;
        }
        SearchReport(true, '', self.ReportSales_ItemChosing());
    }

    // infor HD
    self.LoaiHoaDon_MoPhieu = ko.observable();
    self.MaHoaDon_MoPhieu = ko.observable();
    self.Modal_HoaDons = ko.observableArray();
    self.TongSLHang = ko.observable();

    self.ShowPopup_InforHD = function (item) {
        var maHoaDon = Remove_LastComma(item.MaHoaDon);

        self.MaHoaDon_MoPhieu(maHoaDon);

        ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'Get_InforHoaDon_byMaHoaDon?maHoaDon=' + maHoaDon, 'GET').done(function (data) {
            if (data !== null) {
                self.Modal_HoaDons(data);
                self.LoaiHoaDon_MoPhieu(data.LoaiHoaDon);

                var slHang = data.BH_HoaDon_ChiTiet.reduce(function (_this, val) {
                    return _this + val.SoLuong;
                }, 0);

                self.TongSLHang(slHang);

                $('#modalpopup_PhieuBH').modal('show');
            }
        })
    }

    self.GotoKhachHang = function (item) {
        if (item.MaKhachHang !== '') {
            localStorage.setItem('FindKhachHang', item.MaKhachHang);
            var url = "/#/Customers";
            window.open(url);
        }
    }

    self.ClickMoPhieu = function () {
        localStorage.setItem('FindHD', self.MaHoaDon_MoPhieu());

        var url = '';

        switch (self.LoaiHoaDon_MoPhieu()) {
            case 1:
                url = "/#/Invoices";
                break;
            case 3:
                url = "/#/Order";
                break;
            case 6:
                url = "/#/Returns";
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
            case 22:
                url = "/#/RechargeValueCard";
                break;
        }
        if (url !== '') {
            window.open(url);
        }
    }
}
var reportDiscount = new ModelReportDiscount();
ko.applyBindings(reportDiscount);
$('#selec-all-DonVi').parent().on('hide.bs.dropdown', function () {
    reportDiscount.SearchReport();
});

$('#selec-all-loaiCT').parent().on('hide.bs.dropdown', function () {
    reportDiscount.SearchReport();
});