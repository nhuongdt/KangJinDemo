var ViewModelDiscount = function () {
    var self = this;
    var NhanVienUri = '/api/DanhMuc/NS_NhanVienAPI/';
    var NhomHHUri = '/api/DanhMuc/DM_NhomHangHoaAPI/';
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';

    var _IDchinhanh = $('#hd_IDdDonVi').val();
    var _idNhanVien = $('.idnhanvien').text();

    var laPT_ThucHien = true;
    var laPT_TuVan = true;
    var laPT_YeuCau = true;
    var laPT_BanGoi = true;
    var choose_Update = 0;
    var up_giaban = 0;

    self.IsGara = ko.observable(VHeader.IdNganhNgheKinhDoanh.toUpperCase() === 'C16EDDA0-F6D0-43E1-A469-844FAB143014')
    self.NhanViens = ko.observableArray();
    self.ListNhanVien_Chosed = ko.observableArray();
    self.selectedNhanVien = ko.observable(_idNhanVien);
    self.MaHangHoa_Search = ko.observable();
    self.selectedHH = ko.observable();
    self.selectedTenNhanVien = ko.observable();
    self.HangHoas_seach = ko.observableArray();
    self.filterSeach = ko.observable();
    self.DiscountDetail_Invoice = ko.observableArray();
    self.DiscountDetail_Product = ko.observableArray();
    self.AllDiscountProduct_NhanViens = ko.observableArray();
    self.CountDiscountProduct_byNhom = ko.observableArray();
    self.CountNhomHangHoa = ko.observable(0);
    self.searchAuto_NhomHang = ko.observable();
    self.searchNVienChosed = ko.observable();
    self.DiscountProduct_GroupNameChosing = ko.observable();
    self.CKYeuCau_ApplyGiaBan = ko.observable(false);
    self.ItemOld = ko.observable();

    // paging discount product
    self.pageSizes = [10, 20, 30, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(1);
    self.TotalRecord = ko.observable(0);
    self.PageCount = ko.observable(0);
    self.fromItem = ko.observable(0);
    self.toItem = ko.observable(0);
    self.CheckLoaiNVApDung = ko.observable('0');// 0.all, 1.nvbanhang, 2.nv tuvan, 3.nvlaphoadon
    self.TrangThaiConHan = ko.observable('1');//0: all, 1.conhan, 2. hethan

    self.NhomHangHoas = ko.observableArray();
    self.ListIDNhomHang_Chosed = ko.observableArray([]);
    self.TreeNhomHangHoa = ko.observableArray();// menu left
    self.txtSearchNhomHang = ko.observable();

    // dùng chung: {ChietKhau theo Hoá đơn + Doanh thu}
    self.AllPhongBans = ko.observableArray();
    self.searchAuto_Department = ko.observable();
    self.searchAuto_NVienDepartment = ko.observable();
    self.searchAuto_NVienChosed = ko.observable();

    self.NhanVien_inDepartment = ko.observableArray();
    self.ID_PhongBanChosing = ko.observable();

    // hoa hong hoa don
    self.ChungTuApDung = ko.observableArray();
    self.ChungTuChosed = ko.observableArray([]);
    self.HoaHongHD_ListNhanVienChosed = ko.observableArray();
    self.HoaHongHD_TinhCKTheo = ko.observable('1'); // 1. ThucThu, 2. DoanhThu, 3. VND
    self.HoaDongHD_GiaTriCK = ko.observable();
    self.HoaDongHD_GhiChu = ko.observable();
    self.HoaHongHD_Table = ko.observableArray();
    self.HoaHongHD_IsUpdate = ko.observable(false);
    self.HoaHongHD_IDUpdate = ko.observable(null);

    self.HoaHongDoanhThu_GtriKieuNVApDung = ko.observable(1);// 1.nv laphd, 2.nv theo dichvu, 3.nv theo hoadon
    self.HoaHongDoanhThu_TextKieuNVApDung = ko.observable('1. Nhân viên bán hàng');
    self.HoaHongDoanhThu_GhiChu = ko.observable();
    self.HoaHongDoanhThu_ApDungTuNgay = ko.observableArray();
    self.HoaHongDoanhThu_ApDungDenNgay = ko.observableArray();
    self.HoaHongDoanhThu_TinhCKTheo = ko.observable('1'); // 1. ThucThu, 2. DoanhThu
    self.HoaHongDoanhThu_Table = ko.observableArray();
    self.HoaHongDoanhThu_TableDetail = ko.observableArray();
    self.HoaHongDoanhThu_ApplyAll = ko.observable(false);
    self.HoaHongDoanhThu_IsUpdate = ko.observable(false);
    self.HoaHongDoanhThu_IDUpdate = ko.observable(null);

    // sao chep
    var _nhanvienchuaCD = null;
    var _nhanviendaCD = null;
    self.checkCapNhat = ko.observable('2');
    self.NhanVienSaoChep1 = ko.observableArray();
    self.NhanVienSaoChep2 = ko.observableArray();

    self.NghiepVuADs = ko.observableArray([
        { TenNghiepVu: "Bán hàng", value: "1" },
        { TenNghiepVu: "Đặt hàng", value: "3" },
        { TenNghiepVu: "Trả hàng", value: "6" },
        { TenNghiepVu: "Gói dịch vụ", value: "19" }
    ]);

    self.HoaHongDoanhThu_KieuNVApDungs = ko.observableArray([
        { Text: "1. Nhân viên bán hàng", ID: 1 },
        { Text: "2. Nhân viên thực hiện/ tư vấn dịch vụ", ID: 2 },
        { Text: "3. Nhân viên lập hóa đơn", ID: 3 },
    ]);

    function PageLoad() {
        GetNhanVien_byChiNhanh();
        GetAllChietKhau_ByNhanVien();
        GetAllNhomHangHoas();
        GetAllPhongBan();
        Get_DMChungTuApDung();
        getlist_NhanVienChuaCaiDat();
        getlist_NhanVienDaCaiDat();
        GetTree_NhomHangHoa();
    }
    PageLoad();

    function GetNhanVien_byChiNhanh() {
        ajaxHelper(NhanVienUri + "getListNhanVien_DonVi?ID_ChiNhanh=" + _IDchinhanh + "&nameNV=null", 'GET').done(function (data) {
            self.NhanViens(data);

            vm_NhanVien.reset(data);

            data.map(function (x) {
                x['ID_NhanVien'] = x.ID
            });
            vmSaoChepHoaHongDV.listData.AllNhanViens = data;
            console.log(3, data)
        });
    }

    var tree = '';
    function GetTree_NhomHangHoa() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/DM_NhomHangHoaAPI/' + 'GetTree_NhomHangHoa', 'GET').done(function (obj) {
                if (obj.res === true) {
                    let data = obj.data;
                    if (data.length > 0) {
                        data = data.sort((a, b) => a.text.localeCompare(b.text, undefined, { caseFirst: "upper" }));
                    }
                    self.TreeNhomHangHoa(data);
                    //  bind data on tree
                    tree = $('#treeProductGroup').tree({
                        primaryKey: 'id',
                        uiLibrary: 'bootstrap',
                        dataSource: data,
                        checkboxes: false,
                    }).on('select', function (e, node, id) {
                        newModelDiscount.GetAll_IDNhomChild_ofNhomHH(id);
                    })
                }
            });
        }
    }

    function GetAllNhomHangHoas() {
        ajaxHelper(NhomHHUri + 'GetDM_NhomHangHoa', 'GET').done(function (data) {
            if (data !== null) {
                for (var i = 0; i < data.length; i++) {
                    if (data[i].ID_Parent === null) {
                        var objParent = {
                            ID: data[i].ID,
                            TenNhomHangHoa: data[i].TenNhomHangHoa,
                            // set default child 
                            Childs: [],
                        }

                        for (var j = 0; j < data.length; j++) {
                            if (data[j].ID !== data[i].ID && data[j].ID_Parent === data[i].ID) {
                                var objChild =
                                {
                                    ID: data[j].ID,
                                    TenNhomHangHoa: data[j].TenNhomHangHoa,
                                    ID_Parent: data[i].ID,
                                    Child2s: []
                                };

                                for (var k = 0; k < data.length; k++) {
                                    if (data[k].ID_Parent !== null && data[k].ID_Parent === data[j].ID) {
                                        var objChild2 =
                                        {
                                            ID: data[k].ID,
                                            TenNhomHangHoa: data[k].TenNhomHangHoa,
                                            ID_Parent: data[j].ID,
                                        };
                                        objChild.Child2s.push(objChild2);
                                    }
                                }
                                objParent.Childs.push(objChild);
                            }
                        }
                        self.NhomHangHoas.push(objParent);
                    }
                }

                var arrSort = self.NhomHangHoas().sort(function (a, b) {
                    var x = locdau(a.TenNhomHangHoa), y = locdau(b.TenNhomHangHoa);
                    return x > y ? 1 : x < y ? -1 : 0;
                })
                self.NhomHangHoas(arrSort);
            }
        });
    };

    function GetChildren_NhomHH(arrParent, arrJson, txtSearch, arr, isRoot) {
        if (txtSearch === '') {
            return self.NhomHangHoas();
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
                GetChildren_NhomHH(arrJson[i], arrJson[i].children, txtSearch, arr, false);
            }
        }
        return arr;
    }

    $('#txtSearchNhomHH').keypress(function (e) {
        if (e.keyCode === 13) {
            var filter = locdau($(this).val());
            var arr = GetChildren_NhomHH([], self.TreeNhomHangHoa(), filter, [], true);
            tree.destroy();
            tree = $('#treeProductGroup').tree({
                primaryKey: 'id',
                uiLibrary: 'bootstrap',
                dataSource: arr,
                checkboxes: false,
            }).on('select', function (e, node, id) {
                newModelDiscount.GetAll_IDNhomChild_ofNhomHH(id);
            });
        }
    });

    self.GetAll_IDNhomChild_ofNhomHH = function (idNhom) {
        var arrID = [];
        var nhom = $.grep(self.TreeNhomHangHoa(), function (x) {
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
        self.ListIDNhomHang_Chosed(arrID);
        self.ResetCurrentPage();
    }

    self.FilterNhomHang_ClickAll = function () {
        self.ListIDNhomHang_Chosed([]);
        self.ResetCurrentPage();
    }

    self.AddNhanVien_ToList = function (item) {

        var nv = {
            ID: item.ID_NhanVien,
            MaNhanVien: item.MaNhanVien,
            TenNhanVien: item.TenNhanVien,
        }
        var idNhanVien = nv.ID;
        var itemEx = $.grep(self.ListNhanVien_Chosed(), function (x) {
            return x.ID === idNhanVien;
        });
        if (itemEx.length === 0) {
            self.ListNhanVien_Chosed.unshift(nv);
            self.selectedNhanVien(idNhanVien);
            self.selectedTenNhanVien(nv.TenNhanVien);

            GetAllChietKhau_ByNhanVien();

            var nvAfterChosed = $.grep(self.NhanVienSaoChep1(), function (x) {
                return x.ID !== idNhanVien;
            });
            self.NhanVienSaoChep1($.extend(true, [], nvAfterChosed));
        }
        else {
            ShowMessage_Danger('Nhân viên đã được chọn');
        }
    }

    self.RemoveNhanVien_Chosed = function (item) {
        // add again NVien removed to list NVien
        self.NhanViens.unshift(item);

        for (var i = 0; i < self.ListNhanVien_Chosed().length; i++) {
            if (self.ListNhanVien_Chosed()[i].ID === item.ID) {
                self.ListNhanVien_Chosed.remove(self.ListNhanVien_Chosed()[i]);
                break;
            }
        }
        if (self.ListNhanVien_Chosed().length == 0) {
            self.selectedNhanVien(null);
            self.DiscountDetail_Product([]);
        }
        else {
            // if remove NVien first --> get NVien next
            self.selectedNhanVien(self.ListNhanVien_Chosed()[0].ID);
            GetAllChietKhau_ByNhanVien();
        }
    }

    self.ChoseNhanVien = function (item) {
        var thisObj = event.currentTarget;
        self.selectedNhanVien(item.ID);
        GetAllChietKhau_ByNhanVien();
    }

    self.ListNhanVienChosed_Computed = ko.computed(function () {
        var filter = self.searchNVienChosed();

        var arrFilter = ko.utils.arrayFilter(self.ListNhanVien_Chosed(), function (x) {
            var chon = true;
            var ipLodau = locdau(filter);
            var name = locdau(x.TenNhanVien);
            var code = x.MaNhanVien.toLowerCase();
            var kitudau = GetChartStart(name);

            if (chon && filter) {
                chon = name.indexOf(ipLodau) > -1 || kitudau.indexOf(ipLodau) > -1 || code.indexOf(ipLodau) > -1;
            }
            return chon;
        });
        return arrFilter;
    })

    function LoadingForm(IsShow) {
        $('.table-show').gridLoader({ show: IsShow });
    }

    var indexNVien = 0;
    var vm_NhanVien = new Vue({
        el: '#el_NVien',
        data: function () {
            return {
                query: '',
                filters: self.NhanVienSaoChep1(),
            }
        },
        methods: {
            reset: function (item) {
                this.filters = item;
                this.query = '';
            },
            click: function (item) {
                self.AddNhanVien_ToList(item);
                $('#showseach').hide();
            },
            submit: function (event) {
                var keyPress = event.keyCode;
                switch (keyPress) {
                    case 13:
                        var result = this.findBy(this.filters, this.query.trim());
                        var focus = false;
                        $('#showseach ul li').each(function (i) {
                            if ($(this).hasClass('hoverenabled')) {
                                self.AddNhanVien_ToList(result[i]);
                                $('#showseach').hide();
                                focus = true;
                            }
                        });
                        if (result.length > 0 && this.query !== '' && focus === false) {
                            self.AddNhanVien_ToList(result[0]);
                        }
                        //$('#txtSearchNV').focus();
                        break;
                    case 37:
                    case 38:
                        indexNVien = indexNVien + 1;
                        if (indexNVien < 0) {
                            indexNVien = $("#showseach ul li").length - 1;
                        }
                        this.loadfocus();
                        break;
                    case 39:
                    case 40:
                        indexNVien += 1;
                        if (indexNVien >= $("#showseach ul li").length) {
                            indexNVien = 0;
                        }
                        this.loadfocus();
                        break;
                }
            },
            findBy: function (list, value) {
                if (value === '') return list;
                return list.filter(function (item) {
                    item.NameFull = item.MaNhanVien + " " + item.TenNhanVien;
                    return containsAll(value.split(" "), item['NameFull']) === true;
                });
            },
            loadfocus: function () {
                $('#showseach uli li').each(function (i) {
                    $(this).removeClass('hoverenabled');
                    if (i === indexNVien) {
                        $(this).addClass('hoverenabled');
                    }
                });
                //$('#txtSearchNV').focus();
            }
        },
        computed: {
            searchResult: function () {
                // chi get ds nv chua caidat
                this.filters = self.NhanVienSaoChep1();
                var result = this.findBy(this.filters, this.query.trim());
                if (result.length < 1 || this.query === '') {
                    $('#showseach').hide();
                }
                else {
                    $('#showseach').show();
                    indexNVien = 0;
                }
                $('#showseach ul li').each(function (i) {
                    if (i === 0) {
                        $(this).addClass('hoverenabled');
                    }
                    else {
                        $(this).removeClass('hoverenabled');
                    }
                });
                return result;
            }
        }
    });

    self.SelectedHHEnterkey = function () {
        self.MaHangHoa_Search($('#txtAutoHangHoa').val().toUpperCase());
        AddChietKhauHangHoa_forNVien();
    }

    function AddChietKhauHangHoa_forNVien(MaHH) {
        var maHangHoa = $('#txtAutoHangHoa').val().toUpperCase();
        var idNhanVien = self.selectedNhanVien();
        if (idNhanVien === undefined || idNhanVien === null || idNhanVien === '') {
            ShowMessage_Danger('Vui lòng chọn nhân viên để cài đặt chiết khấu ');
            return false;
        }
        if (maHangHoa == '') {
            ShowMessage_Danger('Vui lòng chọn hàng hóa để cài đặt chiết khấu ');
            return false;
        }
        console.log('maHHChosed', maHangHoa)

        if (self.selectedNhanVien() != undefined) {
            $.ajax({
                type: "GET",
                url: NhanVienUri + "AddChiTietbyMaHH?maHH=" + maHangHoa + "&idnhanvien=" + self.selectedNhanVien() + "&iddonvi=" + _IDchinhanh,
                success: function (result) {
                    if (result === true) {
                        self.GetAllChietKhau_ByNhanVien();
                        ShowMessage_Success('Cập nhật hàng hóa cài đặt hoa hồng thành công');
                    }
                    else {
                        ShowMessage_Danger("Mã hàng hóa: " + self.MaHangHoa_Search() + " đã tồn tại trong danh sách cài đặt hoa hồng nhân viên!");
                    }
                    self.selectedHH(undefined);
                    $('#txtAutoHangHoa').val(self.MaHangHoa_Search());
                    $('#text_MaHangHoa').focus();
                    $('#txtAutoHangHoa').focus().select();
                }
            });
        }
        else
            ShowMessage_Danger("Bạn chưa chọn nhân viên cài đặt hoa hồng");
    }

    self.GetAllChietKhau_ByNhanVien = function () {
        // cai dat Hoa hong cho NVien  ABBB
        GetAllChietKhau_ByNhanVien();
    };

    function GetAllChietKhau_ByNhanVien() {
        var maHangHoa = $('#text_MaHangHoa').val().toUpperCase();
        var idNhanVien = self.selectedNhanVien();
        if (idNhanVien === undefined || idNhanVien === null) {
            idNhanVien = _idNhanVien;
        }

        var param = {
            ID_DonVi: _IDchinhanh,
            ID_NhanVien: idNhanVien,
            ID_NhomHangs: self.ListIDNhomHang_Chosed(),
            TextSearch: maHangHoa,
            CurrentPage: self.currentPage() - 1,
            PageSize: self.pageSize(),
        }
        console.log('param', param)
        $('#tableTheoHangHoa').gridLoader();

        ajaxHelper(NhanVienUri + "GetListNhanVienNhomHang", 'POST', param).done(function (data) {
            $('#tableTheoHangHoa').gridLoader({ show: false });
            if (data.res) {
                self.AllDiscountProduct_NhanViens(data.LstData);
                self.DiscountDetail_Product(data.LstData);
                self.TotalRecord(data.Rowcount);
                self.PageCount(data.numberPage);

                var currentPage = self.currentPage() - 1;
                self.fromItem(currentPage * self.pageSize() + 1);
                if ((self.currentPage() * self.pageSize()) > data.LstData.length) {
                    var fromItem = self.currentPage() * self.pageSize();
                    if (fromItem < self.TotalRecord()) {
                        self.toItem(self.currentPage() * self.pageSize());
                    }
                    else {
                        self.toItem(self.TotalRecord());
                    }
                } else {
                    self.toItem(currentPage * self.pageSize() + self.pageSize());
                }
            }
            else {
                self.fromItem(0);
                self.toItem(0);
                self.PageCount(0);
                self.TotalRecord(0);
            }
            $("div[id ^= 'wait']").remove();
            LoadingForm(false);
        });
    }

    // paging
    self.GetClass = function (page) {
        return ((page.pageNumber) === self.currentPage()) ? "click" : "";
    };

    self.PageList_Display = ko.computed(function () {
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

            // load cache hide/show colum at here
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

    self.GoToPage = function (page) {
        if (page.pageNumber !== '.') {
            self.currentPage(page.pageNumber);
            GetAllChietKhau_ByNhanVien();
        }
    };

    self.StartPage = function () {
        self.currentPage(1);
        GetAllChietKhau_ByNhanVien();
    }

    self.BackPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
        }
        GetAllChietKhau_ByNhanVien();
    }

    self.GoToNextPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.currentPage() + 1);
        }
        GetAllChietKhau_ByNhanVien();
    }

    self.EndPage = function () {
        if (self.currentPage() < self.PageCount()) {
            self.currentPage(self.PageCount());
        }
        GetAllChietKhau_ByNhanVien();
    }

    self.ResetCurrentPage = function () {
        self.currentPage(1);
        GetAllChietKhau_ByNhanVien();
    };

    // search
    $('#text_MaHangHoa').keypress(function (e) {
        if (e.keyCode == 13) {
            self.currentPage(1);
            GetAllChietKhau_ByNhanVien();
        }
    });

    self.ClickIconSearch = function () {
        self.currentPage(1);
        GetAllChietKhau_ByNhanVien();
    }

    self.item_UpdateChietKhau = ko.observable();

    self.ApplyChietKhau_ThucHien = function (item) {
        var idChietKhau = item.ID;

        var giatri_CK = formatNumberToFloat($('#number_' + idChietKhau).val());
        if (giatri_CK > 100 && laPT_ThucHien == true) {
            $("#modalpopup_Update").modal("show");
        }
        else {
            var mydata = {};
            if ($('#cbapdung_' + idChietKhau).is(":checked")) {
                mydata.objData = self.DiscountDetail_Product();
            }
            else {
                mydata.objData = [self.item_UpdateChietKhau()];
            }

            $.ajax({
                url: NhanVienUri + "update_ChietKhau?ChietKhau=" + giatri_CK + "&LaPhanTram=" + laPT_ThucHien,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                data: mydata,
                success: function (result) {
                    ShowMessage_Success('Cập nhật cài đặt hoa hồng thành công');
                    GetAllChietKhau_ByNhanVien();
                },
                error: function (error) {
                    ShowMessage_Danger('Cập nhật cài đặt hoa hồng thất bại');
                    $('#modalpopup_Update').modal('hide');
                }
            })
        }
    }

    self.ApplyChietKhau_ThucHien_ByNhom = function (item) {
        var idChietKhau = item.ID;
        var idNhomHH = item.ID_NhomHang;

        var giatri_CK = formatNumberToFloat($('#number_' + idChietKhau).val());
        if (giatri_CK > 100 && laPT_ThucHien == true) {
            $("#modalpopup_Update").modal("show");
        }
        else {
            var mydata = {};
            if ($('#cbapdung_' + idChietKhau).is(":checked")) {
                // get all ChietKhau by Nhom
                var arrCK_byNhom = $.grep(self.AllDiscountProduct_NhanViens(), function (x) {
                    return x.ID_NhomHang === idNhomHH;
                });
                mydata.objData = arrCK_byNhom;
            }
            else {
                mydata.objData = [self.item_UpdateChietKhau()];
            }

            $.ajax({
                url: NhanVienUri + "update_ChietKhau?ChietKhau=" + giatri_CK + "&LaPhanTram=" + laPT_ThucHien,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                data: mydata,
                success: function (result) {
                    ShowMessage_Success('Cập nhật cài đặt hoa hồng thành công');
                    GetAllChietKhau_ByNhanVien();
                },
                error: function (error) {
                    ShowMessage_Danger('Cập nhật cài đặt hoa hồng thất bại');
                    $('#modalpopup_Update').modal('hide');
                }
            })
        }
    }

    self.ApplyChietKhau_TuVan = function (item) {
        var idChietKhau = item.ID;

        var giatri_CK = formatNumberToFloat($('#number2_' + idChietKhau).val());
        if (giatri_CK > 100 && laPT_TuVan == true) {
            $("#modalpopup_Update").modal("show");
        }
        else {
            var mydata = {};
            if ($('#cbapdung2_' + idChietKhau).is(":checked")) {
                mydata.objData = self.DiscountDetail_Product();
            }
            else {
                mydata.objData = [self.item_UpdateChietKhau()];
            }
            console.log('mydata.objData ', mydata.objData)
            $.ajax({
                url: NhanVienUri + "update_ChietKhau_TuVan?ChietKhau=" + giatri_CK + "&LaPhanTram=" + laPT_TuVan,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                data: mydata,
                success: function (result) {
                    ShowMessage_Success('Cập nhật cài đặt hoa hồng thành công');
                    GetAllChietKhau_ByNhanVien();
                },
                error: function (error) {
                    ShowMessage_Danger('Cập nhật cài đặt hoa hồng thất bại')
                    $('#modalpopup_Update').modal('hide');
                }
            })
        }
    }

    self.ApplyChietKhau_TuVan_ByNhom = function (item) {
        var idChietKhau = item.ID;
        var idNhomHH = item.ID_NhomHang;

        var giatri_CK = formatNumberToFloat($('#number2_' + idChietKhau).val());
        if (giatri_CK > 100 && laPT_TuVan == true) {
            $("#modalpopup_Update").modal("show");
        }
        else {
            var mydata = {};
            if ($('#cbapdung2_' + idChietKhau).is(":checked")) {
                var arrCK_byNhom = $.grep(self.AllDiscountProduct_NhanViens(), function (x) {
                    return x.ID_NhomHang === idNhomHH;
                });
                mydata.objData = arrCK_byNhom;
            }
            else {
                mydata.objData = [self.item_UpdateChietKhau()];
            }
            console.log('mydata.objData ', mydata.objData)
            $.ajax({
                url: NhanVienUri + "update_ChietKhau_TuVan?ChietKhau=" + giatri_CK + "&LaPhanTram=" + laPT_TuVan,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                data: mydata,
                success: function (result) {
                    ShowMessage_Success('Cập nhật cài đặt hoa hồng thành công');
                    GetAllChietKhau_ByNhanVien();
                },
                error: function (error) {
                    ShowMessage_Danger('Cập nhật cài đặt hoa hồng thất bại')
                    $('#modalpopup_Update').modal('hide');
                }
            })
        }
    }

    self.ApplyChietKhau_BanGoi = function (item) {
        var idChietKhau = item.ID;

        var giatri_CK = formatNumberToFloat($('#number1_' + idChietKhau).val());
        if (giatri_CK > 100 && laPT_BanGoi == true) {
            $("#modalpopup_Update").modal("show");
        }
        else {
            var mydata = {};
            if ($('#cbapdung1_' + idChietKhau).is(":checked")) {
                mydata.objData = self.DiscountDetail_Product();
            }
            else {
                mydata.objData = [self.item_UpdateChietKhau()];
            }
            $.ajax({
                url: NhanVienUri + "update_ChietKhau_BanGoi?ChietKhau=" + giatri_CK + "&LaPhanTram=" + laPT_BanGoi,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                data: mydata,
                success: function (result) {
                    ShowMessage_Success('Cập nhật cài đặt hoa hồng thành công');
                    GetAllChietKhau_ByNhanVien();
                },
                error: function (error) {
                    ShowMessage_Danger('Cập nhật cài đặt hoa hồng thất bại')
                    $('#modalpopup_deleteGB').modal('hide');
                }
            })
        }
    }

    self.ApplyChietKhau_BanGoi_ByNhom = function (item) {
        var idChietKhau = item.ID;
        var idNhomHH = item.ID_NhomHang;

        var giatri_CK = formatNumberToFloat($('#number1_' + idChietKhau).val());
        if (giatri_CK > 100 && laPT_BanGoi == true) {
            $("#modalpopup_Update").modal("show");
        }
        else {
            var mydata = {};
            if ($('#cbapdung1_' + idChietKhau).is(":checked")) {
                var arrCK_byNhom = $.grep(self.AllDiscountProduct_NhanViens(), function (x) {
                    return x.ID_NhomHang === idNhomHH;
                });
                mydata.objData = arrCK_byNhom;
            }
            else {
                mydata.objData = [self.item_UpdateChietKhau()];
            }
            $.ajax({
                url: NhanVienUri + "update_ChietKhau_BanGoi?ChietKhau=" + giatri_CK + "&LaPhanTram=" + laPT_BanGoi,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                data: mydata,
                success: function (result) {
                    ShowMessage_Success('Cập nhật cài đặt hoa hồng thành công');
                    GetAllChietKhau_ByNhanVien();
                },
                error: function (error) {
                    ShowMessage_Danger('Cập nhật cài đặt hoa hồng thất bại')
                    $('#modalpopup_deleteGB').modal('hide');
                }
            })
        }
    }

    self.ApplyChietKhau_YeuCau_ByNhom = function (item) {
        var idChietKhau = item.ID;
        var idNhomHH = item.ID_NhomHang;

        var giatri_CK = formatNumberToFloat($('#number4_' + idChietKhau).val());
        if (giatri_CK > 100 && laPT_YeuCau == true) {
            $("#modalpopup_Update").modal("show");
        }
        else {
            var mydata = {};
            if ($('#cbapdung4_' + idChietKhau).is(":checked")) {
                // get all ChietKhau by Nhom
                var arrCK_byNhom = $.grep(self.AllDiscountProduct_NhanViens(), function (x) {
                    return x.ID_NhomHang === idNhomHH;
                });
                mydata.objData = arrCK_byNhom;
            }
            else {
                mydata.objData = [self.item_UpdateChietKhau()];
            }

            let applyGiaBan = 0;  // mac dinh tinh theo giaban
            //if (self.CKYeuCau_ApplyGiaBan() === true) applyGiaBan = 0;
            $.ajax({
                url: NhanVienUri + "update_ChietKhau_YeuCau?ChietKhau=" + giatri_CK + "&LaPhanTram=" + laPT_YeuCau + '&theoCKThucHien=' + applyGiaBan,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                data: mydata,
                success: function (result) {
                    ShowMessage_Success('Cập nhật cài đặt hoa hồng thành công');
                    GetAllChietKhau_ByNhanVien();
                },
                error: function (error) {
                    ShowMessage_Danger('Cập nhật cài đặt hoa hồng thất bại');
                    $('#modalpopup_Update').modal('hide');
                }
            })
        }
    }

    function DiscountProduct_CountByGroup(idNhom, tenNhom) {
        var arrbyIDNhom = $.grep(self.AllDiscountProduct_NhanViens(), function (x) {
            return x.ID_NhomHang === idNhom;
        });
        self.CountDiscountProduct_byNhom(arrbyIDNhom.length);
        self.DiscountProduct_GroupNameChosing(tenNhom);
    }

    function SetOverflowTableFrame() {
        /*$('.table-frame').css("overflow", "inherit");*/
    }

    self.ShowDiv_ThucHien = function (item) {
        SetOverflowTableFrame();
        var idChietKhau = item.ID;
        var gtriCK = item.ChietKhau;

        choose_Update = 3;
        up_giaban = item.GiaBan;
        self.item_UpdateChietKhau(item);

        $(function () {
            $('#number_' + idChietKhau).focus().select();
        })
        if (item.LaPTChietKhau) {
            laPT_ThucHien = true;
            $('#number_' + idChietKhau).val(gtriCK);

            $('#pt_' + idChietKhau).addClass('gb');
            $('#vnd_' + idChietKhau).removeClass('gb');
        }
        else {
            if (gtriCK > 0) {
                laPT_ThucHien = false;
                $('#number_' + idChietKhau).val(formatNumber(gtriCK));

                $('#pt_' + idChietKhau).removeClass('gb');
                $('#vnd_' + idChietKhau).addClass('gb');
            }
            else {
                laPT_ThucHien = true;
                $('#number_' + idChietKhau).val(0);

                $('#pt_' + idChietKhau).addClass('gb');
                $('#vnd_' + idChietKhau).removeClass('gb');
            }
        }

        // count product by group
        DiscountProduct_CountByGroup(item.ID_NhomHang, item.TenNhomHangHoa);
    }

    self.ShowDiv_TuVan = function (item) {
        SetOverflowTableFrame();
        var idChietKhau = item.ID;
        var gtriCK = item.TuVan;
        choose_Update = 2;
        self.item_UpdateChietKhau(item);
        up_giaban = item.GiaBan;

        $(function () {
            $('#number2_' + idChietKhau).focus().select();
        })

        if (item.LaPTTuVan) {
            laPT_TuVan = true;
            $('#number2_' + idChietKhau).val(gtriCK);

            $('#pt2_' + idChietKhau).addClass('gb');
            $('#vnd2_' + idChietKhau).removeClass('gb');
        }
        else {
            if (gtriCK > 0) {
                laPT_TuVan = false;
                $('#number2_' + idChietKhau).val(formatNumber(gtriCK));

                $('#pt2_' + idChietKhau).removeClass('gb');
                $('#vnd2_' + idChietKhau).addClass('gb');
            }
            else {
                laPT_TuVan = true;
                $('#number2_' + idChietKhau).val(0);

                $('#pt2_' + idChietKhau).addClass('gb');
                $('#vnd2_' + idChietKhau).removeClass('gb');
            }
        }
        DiscountProduct_CountByGroup(item.ID_NhomHang, item.TenNhomHangHoa);
    }

    self.ShowDiv_BanGoi = function (item) {
        SetOverflowTableFrame();
        var idChietKhau = item.ID;
        var gtriCK = item.BanGoi;
        choose_Update = 2;
        self.item_UpdateChietKhau(item);
        up_giaban = item.GiaBan;

        $(function () {
            $('#number1_' + idChietKhau).focus().select();
        })

        if (item.LaPTBanGoi) {
            laPT_BanGoi = true;
            $('#number1_' + idChietKhau).val(gtriCK);

            $('#pt1_' + idChietKhau).addClass('gb');
            $('#vnd1_' + idChietKhau).removeClass('gb');
        }
        else {
            if (gtriCK > 0) {
                laPT_BanGoi = false;
                $('#number1_' + idChietKhau).val(formatNumber(gtriCK));

                $('#pt1_' + idChietKhau).removeClass('gb');
                $('#vnd1_' + idChietKhau).addClass('gb');
            }
            else {
                laPT_BanGoi = true;
                $('#number1_' + idChietKhau).val(0);

                $('#pt1_' + idChietKhau).addClass('gb');
                $('#vnd1_' + idChietKhau).removeClass('gb');
            }
        }
        DiscountProduct_CountByGroup(item.ID_NhomHang, item.TenNhomHangHoa);
    }

    self.ShowDiv_YeuCau = function (item) {
        SetOverflowTableFrame();
        var idChietKhau = item.ID;
        var gtriCK = item.YeuCau;
        var theoGB = item.TheoChietKhau_ThucHien == 1 ? false : true;// if AppyThucHien = 1 --> Apply GiaBan = false
        choose_Update = 2;
        self.item_UpdateChietKhau(item);
        up_giaban = item.GiaBan;
        $(function () {
            $('#number4_' + idChietKhau).focus().select();
        })
        if (item.LaPTYeuCau) {
            laPT_YeuCau = true;
            $('#number4_' + idChietKhau).val(gtriCK);

            $('#pt4_' + idChietKhau).addClass('gb');
            $('#vnd4_' + idChietKhau).removeClass('gb');
        }
        else {
            if (gtriCK > 0) {
                laPT_YeuCau = false;
                $('#number1_' + idChietKhau).val(formatNumber(gtriCK));

                $('#pt4_' + idChietKhau).removeClass('gb');
                $('#vnd4_' + idChietKhau).addClass('gb');
            }
            else {
                laPT_YeuCau = true;
                $('#number4_' + idChietKhau).val(0);

                $('#pt4_' + idChietKhau).addClass('gb');
                $('#vnd4_' + idChietKhau).removeClass('gb');
            }
        }

        //set text by theoCKThucHien
        self.CKYeuCau_ApplyGiaBan(theoGB);
        var $this = event.currentTarget;
        var lblYC = $($this).closest('td').find('.lblYeuCau');
        if (theoGB) {
            $(lblYC).text(' Hoa hồng thực hiện theo yêu cầu = ');
        }
        else {
            $(lblYC).text(' Hoa hồng thực hiện theo yêu cầu =  Hoa hồng thực hiện +');
        }

        DiscountProduct_CountByGroup(item.ID_NhomHang, item.TenNhomHangHoa);
    }

    self.EditChietKhau_Product = function (item, loaiChietKhau) {
        var thisObj = event.currentTarget;
        formatNumberObj(thisObj);

        var isPTram = true;
        switch (loaiChietKhau) {
            case 1:
                isPTram = laPT_ThucHien;
                break;
            case 2:
                isPTram = laPT_TuVan;
                break;
            case 3:
                isPTram = laPT_BanGoi;
                break;
            case 4:
                isPTram = laPT_YeuCau;
                break;
        }

        var valThis = $(thisObj).val();
        if (valThis === '') {
            valThis = '0';
        }

        var ptram = 0;
        var gtriTien = 0;
        var giaBan = item.GiaBan;

        if (isPTram) {
            // chietkhau > 100 % --> reset = 100
            if (valThis.indexOf('.') == -1) {
                // neu khong nhap dau .
                valThis = formatNumberToFloat(valThis);
            }
            if (valThis > 100) {
                $(thisObj).val(100);
                valThis = 100;
            }
            gtriTien = Math.round(giaBan * parseFloat(valThis) / 100);
            ptram = formatNumberToFloat(valThis);
            $(thisObj).val(valThis);
        }
        else {
            gtriTien = Math.round(formatNumberToFloat(valThis));
            ptram = 0;
            $(thisObj).val(formatNumber(gtriTien));
        }
    }

    function AddClass_VND_Percent(thisObj) {
        $(thisObj).removeClass('gb');
        $(thisObj).prev().removeClass('gb');
        $(thisObj).next().removeClass('gb');
        $(thisObj).addClass('gb');
    }

    self.ClickVND_ThucHien = function (isClickPtram, itemCK) {
        var thisObj = event.currentTarget;
        AddClass_VND_Percent(thisObj);

        var id = itemCK.ID;
        var objInput = $('#number_' + id);
        var giaBan = itemCK.GiaBan;
        var isPtram = itemCK.LaPTChietKhau;
        var gtriCK_new = formatNumberToFloat(objInput.val());

        var ptUpdate = 0;
        var tienUpdate = 0;

        if (isClickPtram) {
            // neu dang vnd ma click %
            if (laPT_ThucHien == false) {
                ptUpdate = RoundDecimal(gtriCK_new / giaBan * 100); // %
                tienUpdate = gtriCK_new; // vnd

                objInput.val(ptUpdate);
            }
        }
        else {
            // neu dang % ma click vnd
            if (laPT_ThucHien) {
                ptUpdate = 0;
                tienUpdate = Math.round(gtriCK_new * giaBan / 100); // vnd

                objInput.val(formatNumber(tienUpdate));
            }
        }
        laPT_ThucHien = isClickPtram;
    }

    self.ClickVND_TuVan = function (isClickPtram, itemCK) {
        var thisObj = event.currentTarget;
        AddClass_VND_Percent(thisObj);

        var id = itemCK.ID;
        var objInput = $('#number2_' + id);
        var giaBan = itemCK.GiaBan;
        var isPtram = itemCK.LaPTTuVan;
        var gtriCK_new = formatNumberToFloat(objInput.val());

        var ptUpdate = 0;
        var tienUpdate = 0;

        if (isClickPtram) {
            // neu dang vnd ma click %
            if (laPT_TuVan == false) {
                ptUpdate = RoundDecimal(gtriCK_new / giaBan * 100); // %
                tienUpdate = gtriCK_new; // vnd

                objInput.val(ptUpdate);
            }
        }
        else {
            // neu dang % ma click vnd
            if (laPT_TuVan) {
                ptUpdate = 0;
                tienUpdate = Math.round(gtriCK_new * giaBan / 100); // vnd

                objInput.val(formatNumber(tienUpdate));
            }
        }

        laPT_TuVan = isClickPtram;
    }

    self.clickVND_BanGoi = function (isClickPtram, itemCK) {
        var thisObj = event.currentTarget;
        AddClass_VND_Percent(thisObj);

        var id = itemCK.ID;
        var objInput = $('#number1_' + id);
        var giaBan = itemCK.GiaBan;
        var isPtram = itemCK.LaPTBanGoi;
        var gtriCK_new = formatNumberToFloat(objInput.val());

        var ptUpdate = 0;
        var tienUpdate = 0;

        if (isClickPtram) {
            // neu dang vnd ma click %
            if (laPT_BanGoi == false) {
                ptUpdate = RoundDecimal(gtriCK_new / giaBan * 100); // %
                tienUpdate = gtriCK_new; // vnd

                objInput.val(ptUpdate);
            }
        }
        else {
            // neu dang % ma click vnd
            if (laPT_BanGoi) {
                ptUpdate = 0;
                tienUpdate = Math.round(gtriCK_new * giaBan / 100); // vnd

                objInput.val(formatNumber(tienUpdate));
            }
        }

        laPT_BanGoi = isClickPtram;
    }

    self.clickVND_YeuCau = function (isClickPtram, itemCK) {
        var thisObj = event.currentTarget;
        AddClass_VND_Percent(thisObj);

        var id = itemCK.ID;
        var objInput = $('#number1_' + id);
        var giaBan = itemCK.GiaBan;
        var isPtram = itemCK.LaPTYeuCau;
        var gtriCK_new = formatNumberToFloat(objInput.val());

        var ptUpdate = 0;
        var tienUpdate = 0;

        if (isClickPtram) {
            // neu dang vnd ma click %
            if (laPT_YeuCau == false) {
                ptUpdate = RoundDecimal(gtriCK_new / giaBan * 100); // %
                tienUpdate = gtriCK_new; // vnd

                objInput.val(ptUpdate);
            }
        }
        else {
            // neu dang % ma click vnd
            if (laPT_YeuCau) {
                ptUpdate = 0;
                tienUpdate = Math.round(gtriCK_new * giaBan / 100); // vnd

                objInput.val(formatNumber(tienUpdate));
            }
        }

        laPT_YeuCau = isClickPtram;
    }

    self.CKYeuCau_ChangeLoaiApDung = function () {
        var $this = event.currentTarget;
        var lblYC = $($this).closest('.callprice').find('.lblYeuCau');

        var applyThucHien = self.CKYeuCau_ApplyGiaBan();
        if (applyThucHien) {
            $(lblYC).text(' Hoa hồng thực hiện theo yêu cầu =  Hoa hồng thực hiện +');
            self.CKYeuCau_ApplyGiaBan(false);
        }
        else {
            $(lblYC).text(' Hoa hồng thực hiện theo yêu cầu = ');
            self.CKYeuCau_ApplyGiaBan(true);
        }
    }

    self.Delete_DiscountDetailProduct = function (item) {
        var id = item.ID;
        dialogConfirm('Xác nhận xóa', 'Bạn có chắc chắn muốn xóa <b>' + item.TenHangHoa
            + ' </b> khỏi danh sách chiết khấu không?', function () {
                ajaxHelper(NhanVienUri + "deleteChiTiet/" + id, 'GET').done(function (result) {
                    if (result === true) {
                        ShowMessage_Success("Xóa cài đặt hoa hồng hàng hóa thành công", "success");
                        for (var i = 0; i < self.DiscountDetail_Product().length; i++) {
                            if (self.DiscountDetail_Product()[i].ID === id) {
                                self.DiscountDetail_Product.remove(self.DiscountDetail_Product()[i]);
                                break;
                            }
                        }
                    }
                    else {
                        ShowMessage_Danger("Xóa cài đặt hoa hồng hàng hóa thất bại!", "danger");
                    }
                })
            })
    }

    self.DeleteAll_DiscountDetailProduct = function (item) {
        var id = self.selectedNhanVien();
        var tenNhanVienChosed = '';
        if (id == null || id == undefined) {
            id = _idNhanVien;
            tenNhanVienChosed = $('#txtTenNhanVien').val();
        }
        else {
            let itemNV = $.grep(self.NhanViens(), function (x) {
                return x.ID === id;
            })
            if (itemNV.length > 0) {
                tenNhanVienChosed = itemNV[0].TenNhanVien;
            }
        }

        var maHangHoa = $('#text_MaHangHoa').val();
        if (!commonStatisJs.CheckNull(maHangHoa)) {
            maHangHoa = maHangHoa.trim();
        }
        // delete all HangHoa from list after search
        dialogConfirm('Xác nhận xóa', 'Bạn có chắc chắn xóa tất cả hàng hóa của nhân viên <b>' + tenNhanVienChosed
            + ' </b> khỏi danh sách chiết khấu không?', function () {
                let param = {
                    ID_DonVi: VHeader.IdDonVi,
                    ID_NhanVien: id,
                    ID_NhomHangs: self.ListIDNhomHang_Chosed(),
                    TextSearch: maHangHoa,
                    CurrentPage: self.currentPage() - 1,
                    PageSize: self.TotalRecord(),
                }
                ajaxHelper(NhanVienUri + "deleteAllChietKhau", 'POST', param).done(function (x) {
                    if (x.res) {
                        ShowMessage_Success("Xóa cài đặt hoa hồng hàng hóa thành công", "success");
                        GetAllChietKhau_ByNhanVien();
                        let diary = {
                            ID_DonVi: VHeader.IdDonVi,
                            ID_NhanVien: VHeader.IdNhanVien,
                            LoaiNhatKy: 3,
                            ChucNang: 'Xóa cài đặt hoa hồng',
                            NoiDung: 'Xóa cài đặt hoa hồng của nhân viên '.concat(tenNhanVienChosed),
                            NoiDungChiTiet: 'Xóa cài đặt hoa hồng của nhân viên '.concat(tenNhanVienChosed,
                                ' <br /> Nhóm hàng: ', self.ListIDNhomHang_Chosed(),
                                ' <br /> Mã hàng: ', maHangHoa,
                                ' <br /> Người xóa: ', VHeader.UserLogin),
                        }
                        Insert_NhatKyThaoTac_1Param(diary);
                    }
                })
            })
    }

    self.ShowModal_NhomHangHoa = function () {
        if (self.selectedNhanVien() === undefined || self.selectedNhanVien() === null) {
            ShowMessage_Danger('Vui lòng chọn nhân viên để cài đặt chiết khấu');
            return false;
        }
        self.RemoveAll_NhomHangHoa();
        $('#modalChonNhomHangHoa').modal('show');
    }

    self.PageResult_NhomHHs = ko.computed(function (item) {
        var filter = self.searchAuto_NhomHang();
        var arrFilter = ko.utils.arrayFilter(self.NhomHangHoas(), function (item) {

            var chon = true;
            var ipLodau = locdau(filter);
            var tenNhom = locdau(item.TenNhomHangHoa);
            var kitudau = GetChartStart(tenNhom);

            var tenNhomCon1 = '';
            if (item.Childs.length > 0) {
                for (var i = 0; i < item.Childs.length; i++) {
                    tenNhomCon1 += locdau(item.Childs[i].TenNhomHangHoa) + ' ';
                }
            }
            if (chon && filter) {
                chon = tenNhom.indexOf(ipLodau) > -1 || kitudau.indexOf(ipLodau) > -1 || tenNhomCon1.indexOf(ipLodau) > -1;
            }
            return chon;
        });
        return arrFilter;
    })

    // use to show at list Nhom HangHoa (2 column)
    self.CountNhomHangHoa = ko.computed(function () {
        return self.PageResult_NhomHHs().length / 2;
    })

    var arrIDNhomHang_Chosed = [];

    self.CheckAll_NhomHangHoa = function () {
        var thisObj = event.currentTarget;
        var isChecked = $(thisObj).is(':checked');
        $('#divGroupProduct input').prop('checked', isChecked);

        // get all ID_NhomHang
        if (isChecked) {
            arrIDNhomHang_Chosed = [];
            for (var i = 0; i < self.NhomHangHoas().length; i++) {
                arrIDNhomHang_Chosed.push(self.NhomHangHoas()[i].ID);
            }
        }
    }

    self.RemoveAll_NhomHangHoa = function () {
        $('#modalChonNhomHangHoa input[type=checkbox]').prop('checked', false);
        arrIDNhomHang_Chosed = [];
    }

    self.ChoseNhomHangHoa = function (item) {
        var thisObj = event.currentTarget;
        var idChosed = item.ID;
        var isChecked = $(thisObj).is(':checked');

        if (isChecked) {
            arrIDNhomHang_Chosed.push(idChosed);
        }
        else {
            // remove
            arrIDNhomHang_Chosed = $.grep(arrIDNhomHang_Chosed, function (x) {
                return x !== idChosed;
            });
        }

        // check all child
        $(thisObj).parent('._content-tree').find('input[type=checkbox]').prop('checked', isChecked);

        // find parent and check
        var parentLi = $(thisObj).closest('ul');
        var liChild = parentLi.children('li');
        var lenLi = liChild.length;
        var countCheck = 0;

        liChild.find('input').each(function () {
            if ($(this).is(':checked')) {
                countCheck += 1;
            }
        })

        $(function () {
            var inputParent = parentLi.siblings('input');
            inputParent.prop('checked', countCheck == lenLi);

            if (countCheck == lenLi) {
                arrIDNhomHang_Chosed.push(inputParent.attr('id'));
            }
            else {
                arrIDNhomHang_Chosed = $.grep(arrIDNhomHang_Chosed, function (x) {
                    return x !== inputParent.attr('id');
                });
            }

            // check parent root
            var parentRoot = $(parentLi).closest('.divChild1');
            var liRoot = parentRoot.children('ul').children('li');
            var lenRoot = liRoot.length;
            var countCheck2 = 0;
            liRoot.find('._content-tree').children('input').each(function () {
                if ($(this).is(':checked')) {
                    countCheck2 += 1;
                }
            })
            $(function () {
                var inputRoot = parentRoot.children('input');
                inputRoot.prop('checked', countCheck2 == lenRoot);

                if (countCheck2 == lenRoot) {
                    arrIDNhomHang_Chosed.push(inputRoot.attr('id'));
                }
                else {
                    arrIDNhomHang_Chosed = $.grep(arrIDNhomHang_Chosed, function (x) {
                        return x !== inputRoot.attr('id');
                    });
                }
                //console.log('arrIDNhomHang_Chosed', arrIDNhomHang_Chosed)
            })
        })
    }

    self.AddChietKhau_ByNhomHang = function (item) {
        var allIDNhom = GetAll_IDNhomChild_ofNhomHH(arrIDNhomHang_Chosed);
        allIDNhom = $.grep(allIDNhom, function (x) {
            return x !== undefined;
        })
        allIDNhom = $.unique(allIDNhom);
        //allIDNhom = Remove_LastComma(allIDNhom.toString());
        console.log('allIDNhom ', allIDNhom, 'nvID ', self.selectedNhanVien())

        if (allIDNhom.length === 0) {
            ShowMessage_Danger('Vui lòng chọn nhóm hàng đê thực hiện cài đặt');
            return false;
        }

        var tenNhanVien = '';
        var idNhanVien = self.selectedNhanVien();
        if (self.ListNhanVien_Chosed().length == 0) {
            ShowMessage_Danger('Vui lòng chọn nhân viên để thực hiện cài đặt hoa hồng');
            return false;
        }
        tenNhanVien = self.ListNhanVien_Chosed()[0].TenNhanVien;

        var objParam = {
            ID_DonVi: _IDchinhanh,
            ID_NhanVien: idNhanVien,
            ID_NhomHangs: allIDNhom
        };
        $('#tableTheoHangHoa').gridLoader();
        if (idNhanVien != null && idNhanVien !== undefined) {
            ajaxHelper(NhanVienUri + "AddChietKhau_ByIDNhom", "POST", objParam).done(function (result) {
                $('#tableTheoHangHoa').gridLoader({ show: false });

                if (result === true) {
                    arrIDNhomHang_Chosed = [];

                    GetAllChietKhau_ByNhanVien();
                    ShowMessage_Success('Cài đặt hoa hồng theo nhóm thành công');

                    var objDiary = {
                        ID_NhanVien: idNhanVien,
                        ID_DonVi: _IDchinhanh,
                        ChucNang: "Cài đặt hoa hồng theo nhóm hàng",
                        NoiDung: "Cài đặt hoa hồng theo nhóm hàng cho nhân viên: " + tenNhanVien,
                        NoiDungChiTiet: "Cài đặt hoa hồng theo nhóm cho nhân viên: " + tenNhanVien,
                        LoaiNhatKy: 2 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                    };
                    Insert_NhatKyThaoTac_1Param(objDiary);
                }
                else {
                    ShowMessage_Danger('Cài đặt hoa hồng theo nhóm thất bại');
                }
            })
            $('#modalChonNhomHangHoa').modal('hide');
        }
        else {
            ShowMessage_Danger('Vui lòng chọn nhân viên để thực hiện cài đặt hoa hồng');
        }
    };

    function GetAll_IDNhomChild_ofNhomHH(arrIDNhomHang) {
        var arrNhomHHChilds = [];
        // get all IDChild of ID_Parent
        var lc_NhomHangHoas = self.NhomHangHoas();
        for (var j = 0; j < lc_NhomHangHoas.length; j++) {
            if (lc_NhomHangHoas[j].Childs.length > 0 && $.inArray(lc_NhomHangHoas[j].Childs[0].ID_Parent, arrIDNhomHang) > -1) {
                for (var k = 0; k < lc_NhomHangHoas[j].Childs.length; k++) {
                    arrNhomHHChilds.push(lc_NhomHangHoas[j].Childs[k].ID);
                    if (lc_NhomHangHoas[j].Childs[k].Child2s.length > 0) {
                        for (var i = 0; i < lc_NhomHangHoas[j].Childs[k].Child2s.length; i++) {
                            arrNhomHHChilds.push(lc_NhomHangHoas[j].Childs[k].Child2s[i].ID);
                        }
                    }
                }
            }
        }

        // add ID_Parent into arrNhomHHChilds
        for (var i = 0; i < arrIDNhomHang.length; i++) {
            arrNhomHHChilds.push(arrIDNhomHang[i]);
        }
        return arrNhomHHChilds;
    }

    // import
    $(".btnImportExcel").hide();
    $(".filterFileSelect").hide();
    self.fileNameExcel = ko.observable("Lựa chọn file Excel...");
    self.loiExcel = ko.observableArray();

    self.ShowModalImport = function () {
        console.log(1, self.selectedNhanVien())
        if (self.selectedNhanVien() != undefined) {
            document.getElementById('imageUploadForm').value = "";
            $(".NoteImport").show();
            $(".filterFileSelect").hide();
            $(".btnImportExcel").hide();
            $(".BangBaoLoi").hide();
            $('#myModalinport').modal('show');
        }
        else
            ShowMessage_Danger('Bạn chưa chọn nhân viên cài đặt hoa hồng')
    }

    self.DownloadFileTeamplateXLS = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_CaiDatHoaHong.xls";
        window.location.href = url;
    }

    self.DownloadFileTeamplateXLSX = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_CaiDatHoaHong.xlsx";
        window.location.href = url;
    }

    self.Export_DownLoafFile = function (pathFile) {
        var url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + pathFile;
        window.location.href = url;
    }

    self.deleteFileSelect = function () {
        self.fileNameExcel("Lựa chọn file Excel...");
        $(".filterFileSelect").hide();
        $(".btnImportExcel").hide();
        $(".NoteImport").show();
        document.getElementById('imageUploadForm').value = "";
    }

    self.refreshFileSelect = function () {
        self.importChietKhau();
    }

    self.SelectedFileImport = function (vm, evt) {
        self.fileNameExcel(evt.target.files[0].name)
        $(".NoteImport").hide();
        $(".btnImportExcel").show();
        $(".filterFileSelect").show();
        self.loiExcel([]);
    }

    self.importChietKhau = function () {
        LoadingForm(true);
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadForm").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadForm").files[i];
            formData.append("imageUploadForm", file);
        }
        $.ajax({
            type: "POST",
            url: NhanVienUri + "ImfortExcelChietKhau",
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (item) {
                self.loiExcel(item);
                if (self.loiExcel().length > 0) {
                    $(".BangBaoLoi").show();
                    $(".btnImportExcel").hide();
                    $(".refreshFile").show();

                    $(".deleteFile").hide();
                    LoadingForm(false);
                }
                else {
                    $.ajax({
                        type: "POST",
                        url: NhanVienUri + "getList_DanhSachHangChietKhau?ID_ChiNhanh=" + _IDchinhanh + "&ID_NhanVien=" + self.selectedNhanVien(),
                        data: formData,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        success: function (item) {
                            $('#myModalinport').modal('hide');
                            GetAllChietKhau_ByNhanVien();
                            ShowMessage_Success('Import danh sách hàng cài đặt hoa hồng thành công');
                        },
                        statusCode: {
                            500: function (item) {
                                LoadingForm(false);
                                ShowMessage_Danger('Import danh sách hàng cài đặt hoa hồng thất bại')
                            }
                        },
                    })
                }
                LoadingForm(false);
            },
            statusCode: {
                406: function (item) {
                    ShowMessage_Danger(item.responseJSON.Message)
                    LoadingForm(false);
                },
                500: function (item) {
                    LoadingForm(false);
                    ShowMessage_Danger('Import danh sách hàng cài đặt hoa hồng thất bại')
                }
            },
        });
    }

    self.ExportExcel = function () {
        var idNhanVien = self.selectedNhanVien();
        if (idNhanVien == undefined) {
            idNhanVien = _idNhanVien
        }

        var maHangHoa = $('#text_MaHangHoa').val().toUpperCase();
        // get TenNhanVien
        var tenNvien = '';
        var itemNV = $.grep(self.NhanViens(), function (x) {
            return x.ID === idNhanVien;
        });
        if (itemNV.length == 0) {
            itemNV = $.grep(self.ListNhanVien_Chosed(), function (x) {
                return x.ID === idNhanVien;
            });
        }
        if (itemNV.length > 0) {
            tenNvien = itemNV[0].TenNhanVien;
        }

        if (self.DiscountDetail_Product().length < 1) {
            ShowMessage_Danger('Không có dữ liệu để xuất file Excel');
        }
        else {
            var param = {
                ID_DonVi: _IDchinhanh,
                ID_NhanVien: idNhanVien,
                ID_NhomHangs: self.ListIDNhomHang_Chosed(),
                TextSearch: maHangHoa,
                CurrentPage: self.currentPage() - 1,
                PageSize: self.TotalRecord(),
            }

            var columnHide = null;
            ajaxHelper(NhanVienUri + 'ExportExcelChietKhau_NhanVien?columnsHide=null&TenChiNhanh='
                + $('#_txtTenDonVi').text() + '&TenNhanVien=' + tenNvien, 'POST', param).done(function (url) {
                    $('.table-reponsive').gridLoader({ show: false });
                    if (url !== "") {
                        self.Export_DownLoafFile(url);

                        var objDiary = {
                            ID_NhanVien: idNhanVien,
                            ID_DonVi: _IDchinhanh,
                            ChucNang: "Cài đặt hoa hồng",
                            NoiDung: "Xuất báo cáo hàng hóa cài đặt hoa hồng theo nhân viên: " + tenNvien,
                            NoiDungChiTiet: "Xuất báo cáo hàng hóa cài đặt hoa hồng theo nhân viên: " + tenNvien,
                            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                        };
                        Insert_NhatKyThaoTac_1Param(objDiary);
                    }
                });
        }
    }

    self.ShowModalSaoChep = function () {
        vmSaoChepHoaHongDV.showModal();
    }

    self.ApDung_SaoChep = function () {
        document.getElementById("add_HoaHong").disabled = true;
        document.getElementById("add_HoaHong").lastChild.data = " Đang lưu";
        if (arrID_NhanVien.length != 0) {
            var _ID_NhanVien;
            for (var i = 0; i < arrID_NhanVien.length; i++) {
                if (i == 0)
                    _ID_NhanVien = arrID_NhanVien[i];
                else
                    _ID_NhanVien = _ID_NhanVien + "," + arrID_NhanVien[i];
            }
            SaoChep_CaiDatHoaHong(_ID_NhanVien);
        }
        else {
            ShowMessage_Danger('Bạn chưa chọn nhân viên để thực hiện sao chép')
            document.getElementById("add_HoaHong").disabled = false;
            document.getElementById("add_HoaHong").lastChild.data = " Áp dụng";
        }
    };

    function SaoChep_CaiDatHoaHong(item) {
        $('.table-reponsive').gridLoader();
        var array_Seach = {
            ID_DonVi: _IDchinhanh,
            ID_NhanVien: self.selectedNhanVien(),
            ID_NhanVien_new: item,
            PhuongThuc: parseInt(self.checkCapNhat()),
        }
        ajaxHelper(NhanVienUri + "SaoChep_CaiDatHoaHong", "POST", array_Seach).done(function (data) {
            console.log(data);
            if (data != "SCCDHH")
                ShowMessage_Danger('Sao chép cài đặt hoa hồng không thành công')
            else
                ShowMessage_Success('Sao chép cài đặt hoa hồng thành công')

            document.getElementById("add_HoaHong").disabled = false;
            document.getElementById("add_HoaHong").lastChild.data = " Áp dụng";

            $('#modalSaoChepNhanVien').modal("hide");
            getlist_NhanVienChuaCaiDat();
            getlist_NhanVienDaCaiDat();
            self.clearn_ChuaCaiDat();
            self.clearn_DaCaiDat();
            $('.table-reponsive').gridLoader({ show: false });
            $("div[id ^= 'wait']").text("");
        });
    }

    self.clearn_ChuaCaiDat = function () {
        $('#all_nhanvienchuacaidat').prop('checked', false);
        $('#all_nhanvienchuacaidat').removeClass('squarevt');
        $('.NhanVien_ChuaCaiDat input').each(function () {
            $(this).prop('checked', false);
            var thisID = $(this).attr('id');
            $.map(arrID_NhanVien, function (item, i) {
                if (item === thisID) {
                    arrID_NhanVien.splice(i, 1);
                }
            })
        })
        console.log(arrID_NhanVien);
    }

    self.clearn_DaCaiDat = function () {
        $('#all_nhanviendacaidat').prop('checked', false);
        $('#all_nhanviendacaidat').removeClass('squarevt');
        $('.NhanVien_DaCaiDat input').each(function () {
            $(this).prop('checked', false);
            var thisID = $(this).attr('id');
            $.map(arrID_NhanVien, function (item, i) {
                if (item === thisID) {
                    arrID_NhanVien.splice(i, 1);
                }
            })
        })
        console.log(arrID_NhanVien);
    }

    $('.select_CapNhat input').on('click', function () {
        self.checkCapNhat($(this).val());
        console.log(self.checkCapNhat());
    });

    $('#txtSearchNV_ChuaCaiDat').keypress(function (e) {
        if (e.keyCode == 13) {
            _nhanvienchuaCD = $(this).val();
            getlist_NhanVienChuaCaiDat();
        }
    });

    $('#txtSearchNV_DaCaiDat').keypress(function (e) {
        if (e.keyCode == 13) {
            _nhanviendaCD = $(this).val();
            getlist_NhanVienDaCaiDat();
        }
    });

    function getlist_NhanVienChuaCaiDat() {
        if (_nhanvienchuaCD == null) {
            _nhanvienchuaCD = '';
        }
        ajaxHelper(NhanVienUri + "getlistNhanVien_CaiDatChietKhau?ID_DonVi=" + _IDchinhanh + "&MaNhanVien=" + _nhanvienchuaCD + "&TrangThai=" + 0, 'GET').done(function (data) {
            self.NhanVienSaoChep1(data);
        });
    };

    function getlist_NhanVienDaCaiDat() {
        if (_nhanviendaCD == null) {
            _nhanviendaCD = '';
        }

        ajaxHelper(NhanVienUri + "getlistNhanVien_CaiDatChietKhau?ID_DonVi=" + _IDchinhanh + "&MaNhanVien=" + _nhanviendaCD + "&TrangThai=" + 1, 'GET').done(function (data) {
            self.NhanVienSaoChep2(data);

            // change ID_NhanVien --> ID (same list NhanViens {ID, MaNhanVien,..)
            var arr = $.extend(true, [], data);
            for (var i = 0; i < arr.length; i++) {
                arr[i].ID = arr[i].ID_NhanVien;
            }
            self.ListNhanVien_Chosed(arr);
        });
    };

    function GetAllPhongBan() {
        $.getJSON(NhanVienUri + "GetTreePhongBan?chinhanhId=" + _IDchinhanh, function (data) {
            // mượn tạm trường TenNhanVien_GC (TenPhongBan)
            self.AllPhongBans(data);
            vmSaoChepHoaHongDV.listData.PhongBans = data;
        });
    }

    function Get_DMChungTuApDung() {
        ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + "GetChungTuApDung", 'GET').done(function (data) {
            if (!self.IsGara()) {
                data = data.filter(x => x.Key !== 25)
            }
            self.ChungTuApDung(data);
        });
    }

    function GetAll_IDNhomChild_ofDepartment() {
        // find li with id chosing
        var arrIDReturn = [];
        var liChosing = $('#deptID_' + self.ID_PhongBanChosing());
        liChosing.find('ul li').each(function () {
            var thisID = $(this).attr('id');
            arrIDReturn.push(thisID.split('_')[1]);
        });
        arrIDReturn.push(self.ID_PhongBanChosing());

        return arrIDReturn;
    }

    function CreateIDRandom(firstChar) {
        var uniqueId = Math.random().toString(36).substring(2)
            + (new Date()).getTime().toString(36);
        return firstChar + uniqueId;
    }

    function HoaHongHD_ResetInfor() {
        self.ChungTuChosed([]);
        self.HoaHongHD_TinhCKTheo('1');
        self.HoaDongHD_GiaTriCK('');
        self.HoaHongHD_ListNhanVienChosed([]);
        self.NhanVien_inDepartment([]);
        self.ID_PhongBanChosing();
        self.HoaHongHD_IsUpdate(false);
        self.HoaHongHD_IDUpdate(null);
    }

    self.HoaHongHD_EditChietKhau = function () {
        var thisObj = event.currentTarget;
        if (self.HoaHongHD_TinhCKTheo() === '3') {
            formatNumberObj(thisObj);
        }
        else {
            var thisVal = parseFloat($(thisObj).val());
            if (thisVal > 100) {
                $(thisObj).val(100);
            }
        }
    }

    self.PageResult_Department = ko.computed(function (item) {
        var filter = self.searchAuto_Department();
        var arrFilter = ko.utils.arrayFilter(self.AllPhongBans(), function (item) {

            var chon = true;
            var ipLodau = locdau(filter);
            var parentName = locdau(item.text); // ite.text = TenPhongBan
            var kitudau = GetChartStart(parentName);

            var childName = '';
            var childName2 = '';
            for (var i = 0; i < item.children.length; i++) {
                childName += locdau(item.children[i].text) + ' ';

                for (var j = 0; j < item.children[i].children.length; j++) {
                    childName2 += locdau(item.children[i].children[j].text) + ' ';
                }
            }

            if (chon && filter) {
                chon = parentName.indexOf(ipLodau) > -1 || kitudau.indexOf(ipLodau) > -1
                    || childName.indexOf(ipLodau) > -1 || childName2.indexOf(ipLodau) > -1;
            }
            return chon;
        });
        return arrFilter;
    })

    self.PageResult_NVienDepartment = ko.computed(function (item) {
        var filter = self.searchAuto_NVienDepartment();
        var arrFilter = ko.utils.arrayFilter(self.NhanVien_inDepartment(), function (item) {

            var chon = true;
            var ipLodau = locdau(filter);
            var code = locdau(item.MaNhanVien);
            var name = locdau(item.TenNhanVien);
            var kitudau = GetChartStart(name);

            if (chon && filter) {
                chon = code.indexOf(ipLodau) > -1 || kitudau.indexOf(ipLodau) > -1
                    || name.indexOf(ipLodau) > -1;
            }
            return chon;
        });
        return arrFilter;
    })

    self.PageResult_NVienChosed = ko.computed(function (item) {
        var filter = self.searchAuto_NVienChosed();
        var arrFilter = ko.utils.arrayFilter(self.HoaHongHD_ListNhanVienChosed(), function (item) {

            var chon = true;
            var ipLodau = locdau(filter);
            var code = locdau(item.MaNhanVien);
            var name = locdau(item.TenNhanVien);
            var kitudau = GetChartStart(name);

            if (chon && filter) {
                chon = code.indexOf(ipLodau) > -1 || kitudau.indexOf(ipLodau) > -1
                    || name.indexOf(ipLodau) > -1;
            }
            return chon;
        });
        return arrFilter;
    })

    // chi get DS nhan vien chau cai dat chiet khau (HoaDon/PhongBan)

    self.GetNhanVien_byDepartment = function (item) {
        var idPhongBan = '00000000-0000-0000-0000-000000000000';
        if (item !== undefined) {
            idPhongBan = item.id;
        }
        self.ID_PhongBanChosing(idPhongBan);// used to check chose NVien in PhongBan
        ajaxHelper(NhanVienUri + "GetListNhanVien_inDepartment?idPhongBan=" + idPhongBan + "&idDonVi=" + _IDchinhanh, 'GET').done(function (data) {
            if (data != null) {
                //self.NhanVien_inDepartment(data);

                // count NVien chosed
                var arrAfter = [];
                if (idPhongBan == undefined || idPhongBan === const_GuidEmpty) {
                    // if click into all Department
                    arrAfter = self.HoaHongHD_ListNhanVienChosed();
                }
                else {
                    var arrAllNhomChild = GetAll_IDNhomChild_ofDepartment();
                    arrAfter = $.grep(self.HoaHongHD_ListNhanVienChosed(), function (x) {
                        return $.inArray(x.ID_PhongBan, arrAllNhomChild) > -1;
                    });
                }
                // set check for input[checkbox] header
                $('.table-chiet-khau thead input').prop('checked', arrAfter.length === data.length && arrAfter.length != 0);

                // get arrID_NhaVien chosed
                var arrIDNhanVien = [];
                for (var i = 0; i < arrAfter.length; i++) {
                    arrIDNhanVien.push(arrAfter[i].ID);
                }
                // set check[input] for each tr
                //for (var i = 0; i < self.NhanVien_inDepartment().length; i++) {
                //    var itemFor = self.NhanVien_inDepartment()[i];
                //    if ($.inArray(itemFor.ID, arrIDNhanVien) > -1) {
                //        $('input[id$=' + itemFor.ID + ']').prop('checked', true);
                //    }
                //}

                // only get list NhanVien don't chosed
                var arr = $.grep(data, function (x) {
                    return $.inArray(x.ID, arrIDNhanVien) === -1;
                });

                self.NhanVien_inDepartment(arr);
            }
            else {
                self.NhanVien_inDepartment([]);
                $('.table-chiet-khau thead input').prop('checked', false);
            }
        });
    }

    self.Check_UncheckNhanVien = function () {
        var thisObj = event.currentTarget;
        var liData = $(thisObj).closest('ul').children('li');
        var lenLi = liData.length;

        var countCheck = 0;
        liData.each(function () {
            var ip = $(this).find('input');
            if (ip.is(':checked')) {
                countCheck += 1;
            }
        });
        $(function () {
            $(liData).closest('.cai-dat-hoa-hong-r').find('.checkbox-modal').prop('checked', countCheck === lenLi);
        })
    }

    self.HoaHongHD_Chose1NhanVien = function (item) {
        self.HoaHongHD_ListNhanVienChosed.push(item);

        for (var i = 0; i < self.NhanVien_inDepartment().length; i++) {
            if (self.NhanVien_inDepartment()[i].ID === item.ID) {
                self.NhanVien_inDepartment.remove(self.NhanVien_inDepartment()[i]);
                break;
            }
        }
    }

    self.HoaHongHD_Remove1NhanVien = function (item) {
        var idNVien = item.ID;
        for (var i = 0; i < self.HoaHongHD_ListNhanVienChosed().length; i++) {
            if (self.HoaHongHD_ListNhanVienChosed()[i].ID === idNVien) {
                self.HoaHongHD_ListNhanVienChosed.splice(i, 1);
                break;
            }
        }
        self.NhanVien_inDepartment.unshift(item);
    }

    // not use
    self.HoaHongHD_ChoseAllNhanVien1 = function (item) {
        var thisObj = event.currentTarget;

        // get list ID_NVien chosed
        var arrIDNhanVien = [];
        for (var i = 0; i < self.HoaHongHD_ListNhanVienChosed().length; i++) {
            arrIDNhanVien.push(self.HoaHongHD_ListNhanVienChosed()[i].ID);
        }

        var isChecked = $(thisObj).is(':checked');
        if (isChecked) {
            //  find all check box in table and check
            for (var i = 0; i < self.NhanVien_inDepartment().length; i++) {
                if ($.inArray(self.NhanVien_inDepartment()[i].ID, arrIDNhanVien) === -1) {
                    self.HoaHongHD_ListNhanVienChosed.unshift(self.NhanVien_inDepartment()[i]);
                }
            }
        }
        else {
            var arrAllNhomChild = GetAll_IDNhomChild_ofDepartment();
            var arrAfter = $.grep(self.HoaHongHD_ListNhanVienChosed(), function (x) {
                return $.inArray(x.ID_PhongBan, arrAllNhomChild) === -1;
            });
            self.HoaHongHD_ListNhanVienChosed($.extend(true, [], arrAfter));
        }

        $('.table-chiet-khau input[type=checkbox]').prop('checked', isChecked);
    }

    self.HoaHongHD_ChoseAllNhanVien_Left = function () {
        var thisObj = event.currentTarget;
        var isChecked = $(thisObj).is(':checked');

        // find ul in this modal
        //var idUl = $(thisObj).closest('.cai-dat-hoa-hong-r').find('.ul-cai-dat-hoa-hong-r');
        //$(idUl).find('li input').each(function () {
        //    $(this).prop('checked', isChecked);
        //})
        $("#NV_Apdung_L input").each(function () {
            $(this).prop('checked', isChecked);
        })
    }

    self.HoaHongHD_ChoseAllNhanVien_Right = function () {
        var thisObj = event.currentTarget;
        var isChecked = $(thisObj).is(':checked');

        // find ul in this modal
        //var idUl = $(thisObj).closest('.cai-dat-hoa-hong-r').find('.ul-nhan-vien-ap-dung');
        //$(idUl).find('li input').each(function () {
        //    $(this).prop('checked', isChecked);
        //})
        $("#NV_Apdung_R input").each(function () {
            $(this).prop('checked', isChecked);
        })

    }

    self.HoaHongHD_ClickBtnChose = function (item) {
        var thisObj = event.currentTarget;
        // find div content ul
        var divUl = $("#listNV_L");
        var ulData = $("#NV_Apdung_L");

        var arrIDNhanVien = [];
        ulData.find('li input').each(function () {
            if ($(this).is(':checked')) {
                arrIDNhanVien.push($(this).attr('id'));
            }
        })

        $(function () {
            // push in list chosed
            for (var i = 0; i < self.NhanVien_inDepartment().length; i++) {
                if ($.inArray(self.NhanVien_inDepartment()[i].ID, arrIDNhanVien) > -1) {
                    self.HoaHongHD_ListNhanVienChosed.unshift(self.NhanVien_inDepartment()[i]);
                }
            }

            // remove from list nv_department
            var arrAfter = $.grep(self.NhanVien_inDepartment(), function (x) {
                return $.inArray(x.ID, arrIDNhanVien) === -1;
            });
            self.NhanVien_inDepartment(arrAfter);
        })
        // remove check all at header
        $(divUl).find('.checkbox-modal').prop('checked', false);
    }

    self.HoaHongHD_ClickBtnRemoveChose = function () {
        var thisObj = event.currentTarget;
        //var divUl = $(thisObj).closest('.cai-dat-hoa-hong-l').next();
        //var ulData = $(divUl).find('.ul-nhan-vien-ap-dung');
        var divUl = $("#listNV_R");
        var ulData = $("#NV_Apdung_R");
        var arrIDNhanVien = [];
        ulData.find('li input').each(function () {
            if ($(this).is(':checked')) {
                arrIDNhanVien.push($(this).attr('id'));
            }
        })

        $(function () {
            // push again list NhanVien_inDepartment
            var arrRemove = $.grep(self.HoaHongHD_ListNhanVienChosed(), function (x) {
                return $.inArray(x.ID, arrIDNhanVien) > -1;
            });
            for (var i = 0; i < arrRemove.length; i++) {
                self.NhanVien_inDepartment.unshift(arrRemove[i]);
            }
            // remove from list NhanVienChosed
            var arrAfter = $.grep(self.HoaHongHD_ListNhanVienChosed(), function (x) {
                return $.inArray(x.ID, arrIDNhanVien) === -1
            });

            self.HoaHongHD_ListNhanVienChosed(arrAfter);
        })

        // remove check all at header
        $(divUl).find('.checkbox-modal').prop('checked', false);
    }

    // not use
    self.HoaHongHD_RemoveStaff = function (item) {
        var idNVien = item.ID;
        for (var i = 0; i < self.HoaHongHD_ListNhanVienChosed().length; i++) {
            if (self.HoaHongHD_ListNhanVienChosed()[i].ID === idNVien) {
                self.HoaHongHD_ListNhanVienChosed.splice(i, 1);
                break;
            }
        }

        // remove check at input list NhanVien
        $('#idNVienHD_' + idNVien).prop('checked', false);
    }

    self.HoaHongHD_ChoseChungTu = function (item) {
        // get all key chosed
        var allKey = [];
        for (var i = 0; i < self.ChungTuChosed().length; i++) {
            allKey.push(self.ChungTuChosed()[i].Key);
        }

        if ($.inArray(item.Key, allKey) === -1) {
            self.ChungTuChosed.push(item);
        }

        // set check after li
        $('#chungtu_' + item.Key).append(element_appendCheck);
    }

    self.HoaHongHD_RemoveChungTu = function (item) {
        for (var i = 0; i < self.ChungTuChosed().length; i++) {
            if (self.ChungTuChosed()[i].Key === item.Key) {
                self.ChungTuChosed.splice(i, 1);
            }
        }
        $('#chungtu_' + item.Key).empty();

        // keep value of list ChungTu
        var lstChungTu = self.ChungTuApDung();
        self.ChungTuApDung($.extend(true, [], lstChungTu));
    }

    self.HoaHongHD_ThemMoi = function () {
        $('#modalCaiDatHoaDon').modal('show');
        $('.table-chiet-khau thead input').prop('checked', false);

        HoaHongHD_ResetInfor();
        $('#ddlChungTu ul li i').remove();
    }

    self.HoaHongHD_ShowFormUpdate = function (item) {
        $('#modalCaiDatHoaDon').modal('show');
        self.HoaHongHD_IsUpdate(true);

        // assign value 
        self.HoaDongHD_GiaTriCK(item.GiaTriChietKhau);
        self.HoaDongHD_GhiChu(item.GhiChu);
        self.ChungTuChosed(item.ListChungTuApDung);
        self.HoaHongHD_ListNhanVienChosed(item.NhanViens);
        self.HoaHongHD_IDUpdate(item.ID);
        self.HoaHongHD_TinhCKTheo('' + item.TinhChietKhauTheo);

        self.GetNhanVien_byDepartment();
    }

    function CaculatorAgain_TotalRow(totalRow) {
        // caculator again PageCount, fromItem, ToItem
        var totalPage = Math.ceil(totalRow / self.pageSize());
        self.TotalRecordHD(totalRow);
        self.TotalPageHD(totalPage);
    }

    self.HoaHongHD_Agree = function () {

        var gtriCK = self.HoaDongHD_GiaTriCK();
        var tinhCKTheo = self.HoaHongHD_TinhCKTheo();
        var chungtu = self.ChungTuChosed();
        var nhanvien = self.HoaHongHD_ListNhanVienChosed();
        var laPTram = true;
        var strTinhCKTheo = '';

        if (gtriCK == undefined || gtriCK == '' || gtriCK === null) {
            ShowMessage_Danger('Vui lòng nhập giá trị chiết khấu');
            return false;
        }

        if (nhanvien.length === 0) {
            ShowMessage_Danger('Vui lòng chọn nhân viên');
            return false;
        }

        if (chungtu.length === 0) {
            ShowMessage_Danger('Vui lòng chọn chứng từ áp dụng');
            return false;
        }

        switch (tinhCKTheo) {
            case '1':
                strTinhCKTheo = 'thực thu';
                break;
            case '2':
                strTinhCKTheo = 'doanh thu';
                break;
            case '3':
                strTinhCKTheo = 'VNĐ';
                laPTram = false;
                break;
        }

        var sChungTu = '';
        for (var i = 0; i < chungtu.length; i++) {
            sChungTu += chungtu[i].Key + ',';
        }
        sChungTu = Remove_LastComma(sChungTu);

        var objNew = {
            IDRandom: CreateIDRandom('HoaHongHD_'),
            ID_DonVi: _IDchinhanh,
            GiaTriChietKhau: gtriCK,
            TinhChietKhauTheo: tinhCKTheo,
            LaPhanTram: laPTram,
            Text_TinhChietKhauTheo: strTinhCKTheo,
            ChungTuApDung: sChungTu,
            ListChungTuApDung: chungtu,// used to bind in grid
            NhanViens: nhanvien,
            GhiChu: self.HoaDongHD_GhiChu(),
            TrangThai: 1,
        }
        console.log('add_ckHD ', objNew)

        var sID_NhanVien = '';
        for (var i = 0; i < nhanvien.length; i++) {
            sID_NhanVien += nhanvien[i].ID + ',';
        }
        sID_NhanVien = Remove_LastComma(sID_NhanVien);

        var idChietKhauHD = const_GuidEmpty;
        if (self.HoaHongHD_IsUpdate()) {
            idChietKhauHD = self.HoaHongHD_IDUpdate();
        }

        var Param_GetChietKhauHoaDon = {
            ID_DonVi: _IDchinhanh,
            ID_NhanViens: sID_NhanVien,
            ChungTuApDung: sChungTu,
            ID_ChietKhauHoaDon: idChietKhauHD,
        }

        console.log(3, Param_GetChietKhauHoaDon)

        // check if ChietKhauHD_NhanVien exist
        ajaxHelper(NhanVienUri + 'CheckExist_ChietKhauHD_NhanVien', 'POST', Param_GetChietKhauHoaDon).done(function (data1) {
            console.log(data1)
            if (data1.res == true) {
                var sTenNhanVien = '';
                for (var i = 0; i < data1.DataSoure.length; i++) {
                    sTenNhanVien += data1.DataSoure[i].TenNhanVien + ', ';
                }
                sTenNhanVien = Remove_LastComma(sTenNhanVien);

                // if not exist --> insert/update DB
                if (sTenNhanVien == '') {
                    Save_ChietKhauHoaDon(objNew);
                    HoaHongHD_ResetInfor();
                    $('#modalCaiDatHoaDon').modal('hide');
                }
                else {
                    ShowMessage_Danger('Nhân viên ' + sTenNhanVien + ' đã cài đặt chiết khấu hóa đơn');
                }
            }
            else {
                Save_ChietKhauHoaDon(objNew);
            }
        })
    }

    function Save_ChietKhauHoaDon(objNew) {

        if (self.HoaHongHD_IsUpdate() === false) {
            ajaxHelper(NhanVienUri + 'Add_ChietKhauHoaDon', 'POST', objNew).done(function (data) {
                console.log('data2 ', data)
                if (data.res == true) {
                    objNew.ID = data.DataSoure.ID;
                    self.HoaHongHD_Table.unshift(objNew);
                    ShowMessage_Success('Cài đặt chiết khấu hóa đơn thành công');

                    CaculatorAgain_TotalRow(self.TotalRecordHD() + 1);
                }
                else {
                    console.log(data.mess);
                    ShowMessage_Danger('Cài đặt chiết khấu hóa đơn thất bại');
                }
            })
        }
        else {
            objNew.ID = self.HoaHongHD_IDUpdate();

            ajaxHelper(NhanVienUri + 'Update_ChietKhauHoaDon', 'POST', objNew).done(function (data) {
                if (data.res == true) {
                    for (var i = 0; i < self.HoaHongHD_Table().length; i++) {
                        if (self.HoaHongHD_Table()[i].ID === objNew.ID) {
                            self.HoaHongHD_Table.remove(self.HoaHongHD_Table()[i]);
                            break;
                        }
                    }
                    self.HoaHongHD_Table.unshift(objNew);
                    ShowMessage_Success('Cập nhật cài đặt chiết khấu hóa đơn thành công');
                }
                else {
                    console.log(data.mess);
                    ShowMessage_Danger('Cập nhật cài đặt chiết khấu hóa đơn thất bại');
                }
            })
        }
    }

    self.HoaHongHD_Remove = function (item) {
        var thisObj = event.currentTarget;
        var idRandom = item.IDRandom;
        var id = item.ID;

        dialogConfirm('Xác nhận xóa', 'Bạn có chắc chắn muốn xóa chiết khấu này không?', function () {
            ajaxHelper(NhanVienUri + 'Delete_ChietKhauHoaDon/' + id, 'POST').done(function (data) {
                if (data.res == true) {
                    for (var i = 0; i < self.HoaHongHD_Table().length; i++) {
                        if (self.HoaHongHD_Table()[i].IDRandom === idRandom) {
                            self.HoaHongHD_Table.splice(i, 1);
                        }
                    }
                    ShowMessage_Success('Xóa cài đặt chiết khấu hóa đơn thành công');

                    CaculatorAgain_TotalRow(self.TotalRecordHD() - 1);
                }
                else {
                    console.log(data.mess);
                    ShowMessage_Danger('Xóa cài đặt chiết khấu hóa đơn thất bại');
                }
            })
        })
    }

    // not use
    self.RemoveChungTuApDung_InRow = function (dataItem, parentItem) {
        for (var i = 0; i < self.HoaHongHD_Table().length; i++) {
            var itemOut = self.HoaHongHD_Table()[i];
            if (itemOut.IDRandom === parentItem.IDRandom) {
                for (var j = 0; j < itemOut.ChungTuApDung.length; j++) {
                    if (itemOut.ChungTuApDung[j].Key === dataItem.Key) {
                        self.HoaHongHD_Table()[i].ChungTuApDung.splice(j, 1);
                        break;
                    }
                }
                break;
            }
        }
    }

    // hoa hong theo doanh thu
    function HoaHongDoanhThu_ResetInfor() {
        self.HoaHongDoanhThu_IsUpdate(false);
        self.HoaHongDoanhThu_TinhCKTheo('1');
        self.HoaHongHD_ListNhanVienChosed([]);
        self.NhanVien_inDepartment([]);
        self.ID_PhongBanChosing();
        self.HoaHongDoanhThu_GhiChu('');
        self.HoaHongDoanhThu_GtriKieuNVApDung(1);
        self.HoaHongDoanhThu_TextKieuNVApDung(self.HoaHongDoanhThu_KieuNVApDungs()[0].Text);

        self.HoaHongDoanhThu_ApDungTuNgay('');
        self.HoaHongDoanhThu_ApDungDenNgay('');
        self.HoaHongDoanhThu_TableDetail(
            [{
                IDRandom: CreateIDRandom('ChiTietDT_'),
                DoanhThuTu: 0,
                DoanhThuDen: 0,
                GiaTriChietKhau: 0,
                LaPhanTram: 1,
            }
            ]);
        self.HoaHongDoanhThu_IDUpdate(null);
    }

    function HoaHongDoangThu_ActiveTab() {
        $('.table-chiet-khau thead input').prop('checked', false);
        $('#ulTab_ChietKhauDoanhThu li').removeClass('active');
        $('#ulTab_ChietKhauDoanhThu li:eq(0)').addClass('active');

        $('._tab-chieu-khau-doanh-thu .tab-content .tab-pane').removeClass('active');
        $('._tab-chieu-khau-doanh-thu .tab-content .tab-pane:eq(0)').addClass('active');
    }

    self.HoaHongDoanhThu_ChoseKieuNV = function (item) {
        self.HoaHongDoanhThu_GtriKieuNVApDung(item.ID);
        self.HoaHongDoanhThu_TextKieuNVApDung(item.Text);

        switch (item.ID) {
            case 1:
            case 3:
                self.HoaHongDoanhThu_TinhCKTheo('1');
                break;
            case 2:
                self.HoaHongDoanhThu_TinhCKTheo('2');
                break;
        }
    }

    self.HoaHongDoanhThu_ThemMoi = function () {
        $('#modalCaiDatDoanhThu').modal('show');
        HoaHongDoanhThu_ResetInfor();
        HoaHongDoangThu_ActiveTab();
    }

    self.HoaHongDoanhThu_EditDoanhThu = function (item) {
        var thisObj = event.currentTarget;
        formatNumberObj(thisObj);

        var idRandom = item.IDRandom;
        for (var i = 0; i < self.HoaHongDoanhThu_TableDetail().length; i++) {
            // update value revenue (Doanh Thu) for 1 row
            if (self.HoaHongDoanhThu_TableDetail()[i].IDRandom === idRandom) {
                var tr = $('#' + self.HoaHongDoanhThu_TableDetail()[i].IDRandom);
                var doanhThuTu = formatNumberToFloat(tr.children('td').eq(1).find('input').val());
                var doanhThuDen = formatNumberToFloat(tr.children('td').eq(3).find('input').val());

                self.HoaHongDoanhThu_TableDetail()[i].DoanhThuTu = doanhThuTu;
                self.HoaHongDoanhThu_TableDetail()[i].DoanhThuDen = doanhThuDen;
                break;
            }
        }
    }

    self.HoaHongDoanhThu_EditGtriCK = function (item) {
        var thisObj = event.currentTarget;
        var objPtram = $('#laPTDT_' + item.IDRandom);
        var laPtram = objPtram.hasClass('gb');
        if (laPtram) {
            var gtriCK = formatNumberToFloat($(thisObj).val());
            if (gtriCK > 100) {
                $(thisObj).val(100);
            }
        }
        else {
            formatNumberObj(thisObj);
        }
    }

    self.Add_DoanhThuChiTiet = function () {
        var check = CheckKhoangDoanhThu();
        if (check === false) {
            return;
        }
        var newObj = {
            IDRandom: CreateIDRandom('ChiTietDT_'),
            DoanhThuTu: 0,
            DoanhThuDen: 0,
            GiaTriChietKhau: 0,
            LaPhanTram: 1,
        }
        self.HoaHongDoanhThu_TableDetail.push(newObj)
    }

    self.CountRow_HoaHongDoanhThu_TableDetail = ko.computed(function () {
        return self.HoaHongDoanhThu_TableDetail().length;
    })

    self.ShowDiv_ChietKhauCT = function (item) {
        var thisObj = event.currentTarget;
        var objInput = $(thisObj).next().find('.number-price');
        $(function () {
            objInput.focus().select();
        })

        var idRandom = item.IDRandom;
        var laPTram = item.LaPhanTram;
        var gtriCK = item.GiaTriChietKhau;

        // set class 'gb' for %
        var objPTram = $(thisObj).next().find('#laPTDT_' + idRandom);
        objPTram.prev().removeClass('gb');
        objPTram.addClass('gb');

        if (!laPTram) {
            gtriCK = formatNumber3Digit(gtriCK);
            objPTram.removeClass('gb');
            objPTram.prev().addClass('gb');
        }
        objInput.val(gtriCK);
        self.HoaHongDoanhThu_ApplyAll(false);
    }

    self.HoaDongDoanhThu_ClickVND = function (item) {
        var thisObj = event.currentTarget;
        $(thisObj).prev().removeClass('gb');
        $(thisObj).next().removeClass('gb');
        $(thisObj).addClass('gb');

        // focus input
        $('#gtriCKDT_' + item.IDRandom).focus().select();
    }

    self.HoaDongDoanhThu_AgreeGiaTriCK = function (item) {
        var idRandom = item.IDRandom;
        var isApplyAll = self.HoaHongDoanhThu_ApplyAll();
        var gtriCK = $('#gtriCKDT_' + idRandom).val();
        var laPTram = $('#laPTDT_' + idRandom).hasClass('gb');
        if (laPTram) {
            laPTram = 1;
        }
        else {
            laPTram = 0;
        }

        if (isApplyAll) {
            for (var i = 0; i < self.HoaHongDoanhThu_TableDetail().length; i++) {
                // assign value for DoanThuTu/Den
                var tr = $('#' + self.HoaHongDoanhThu_TableDetail()[i].IDRandom);
                var doanhThuTu = formatNumberToFloat(tr.children('td').eq(1).find('input').val());
                var doanhThuDen = formatNumberToFloat(tr.children('td').eq(3).find('input').val());

                self.HoaHongDoanhThu_TableDetail()[i].DoanhThuTu = doanhThuTu;
                self.HoaHongDoanhThu_TableDetail()[i].DoanhThuDen = doanhThuDen;

                self.HoaHongDoanhThu_TableDetail()[i].GiaTriChietKhau = gtriCK;
                self.HoaHongDoanhThu_TableDetail()[i].LaPhanTram = laPTram;
            }
        }
        else {
            for (var i = 0; i < self.HoaHongDoanhThu_TableDetail().length; i++) {
                // update value revenue (Doanh Thu) for all row
                var tr = $('#' + self.HoaHongDoanhThu_TableDetail()[i].IDRandom);
                var doanhThuTu = formatNumberToFloat(tr.children('td').eq(1).find('input').val());
                var doanhThuDen = formatNumberToFloat(tr.children('td').eq(3).find('input').val());

                self.HoaHongDoanhThu_TableDetail()[i].DoanhThuTu = doanhThuTu;
                self.HoaHongDoanhThu_TableDetail()[i].DoanhThuDen = doanhThuDen;

                if (self.HoaHongDoanhThu_TableDetail()[i].IDRandom === idRandom) {
                    self.HoaHongDoanhThu_TableDetail()[i].GiaTriChietKhau = gtriCK;
                    self.HoaHongDoanhThu_TableDetail()[i].LaPhanTram = laPTram;
                }
            }
        }

        // bind again
        var arrDetail = self.HoaHongDoanhThu_TableDetail();
        self.HoaHongDoanhThu_TableDetail($.extend(true, [], arrDetail));
    }

    // check condition for ChietKhauDoanhThu
    function CheckKhoangDoanhThu() {
        // check DoanhThuTu > DoanhThuDen
        for (var i = 0; i < self.HoaHongDoanhThu_TableDetail().length; i++) {
            let itemFor = self.HoaHongDoanhThu_TableDetail()[i];
            if (itemFor.DoanhThuTu >= itemFor.DoanhThuDen) {
                ShowMessage_Danger("Doanh thu từ phải nhỏ hơn doanh thu đến");
                return false;
            }
        }

        // kiem tra khoang doanh thu bị gối lên nhau không
        for (var i = 0; i < self.HoaHongDoanhThu_TableDetail().length; i++) {
            let itemFor = self.HoaHongDoanhThu_TableDetail()[i];
            let arr = $.grep(self.HoaHongDoanhThu_TableDetail(), function (x) {
                return x.DoanhThuDen >= itemFor.DoanhThuTu && x.DoanhThuTu <= itemFor.DoanhThuDen && x.IDRandom !== itemFor.IDRandom;
            });
            if (arr.length > 0) {
                ShowMessage_Danger('Khoảng doanh thu không được gối trùng lên nhau');
                return false;
            }
        }
    }

    self.ChangeApDungTuNgay = function (x) {
        var dateFrom = self.HoaHongDoanhThu_ApDungTuNgay();
        if (dateFrom !== '' && dateFrom !== undefined) {
            var dateTo = self.HoaHongDoanhThu_ApDungDenNgay();

            dateFrom = moment(dateFrom, 'DD/MM/YYYY').format('YYYYMMDD');
            dateTo = moment(dateTo, 'DD/MM/YYYY').format('YYYYMMDD');
            console.log(dateFrom, dateTo)

            if (dateTo < dateFrom) {
                ShowMessage_Danger("Ngày kết thúc không được nhỏ hơn ngày bắt đầu");
            }
        }
    }

    // blur: di chuot ra khoi o input
    self.Blur_DoanhThuDen = function (item) {
        var idRandom = item.IDRandom;
        for (var i = 0; i < self.HoaHongDoanhThu_TableDetail().length; i++) {
            let itemFor = self.HoaHongDoanhThu_TableDetail()[i];
            if (itemFor.IDRandom === idRandom) {
                if (itemFor.DoanhThuDen <= itemFor.DoanhThuTu) {
                    ShowMessage_Danger("Doanh thu từ phải nhỏ hơn doanh thu đến");
                }
                break;
            }
        }
    }

    self.Blur_DoanhThuTu = function (item) {
        var thisObj = event.currentTarget;
        var idRandom = item.IDRandom;
        var doanhThuTu = formatNumberToInt($(thisObj).val());

        let arr = $.grep(self.HoaHongDoanhThu_TableDetail(), function (x) {
            return x.DoanhThuDen >= doanhThuTu && x.IDRandom !== idRandom;
        });
        if (arr.length > 0) {
            ShowMessage_Danger('Khoảng doanh thu không được gối trùng lên nhau');
            return false;
        }
    }

    self.HoaHongDoanhThu_RemoveDetail = function (item) {
        var idRandom = item.IDRandom;
        for (var i = 0; i < self.HoaHongDoanhThu_TableDetail().length; i++) {
            let itemFor = self.HoaHongDoanhThu_TableDetail()[i];
            if (itemFor.IDRandom === idRandom) {
                self.HoaHongDoanhThu_TableDetail.splice(i, 1);
                break;
            }
        }
    }

    self.HoaHongDoanhThu_Agree = function () {

        // check ngay apdung
        var nhanvien = self.HoaHongHD_ListNhanVienChosed();
        var dateFrom = self.HoaHongDoanhThu_ApDungTuNgay();
        var dateTo = self.HoaHongDoanhThu_ApDungDenNgay();
        if (dateFrom == undefined || dateFrom == null || dateFrom == '') {
            ShowMessage_Danger('Vui lòng nhập ngày bắt đầu áp dụng');
            return false;
        }

        if (dateTo == undefined || dateTo == null || dateTo == '') {
            dateTo = null;
        }
        else {
            dateTo = moment(dateTo, 'DD/MM/YYYY').format('YYYY-MM-DD');
        }

        dateFrom = moment(dateFrom, 'DD/MM/YYYY').format('YYYY-MM-DD');

        var check = isValidDateYYYYMMDD(dateFrom);
        if (!check) {
            ShowMessage_Danger("Ngày áp dụng chưa đúng định dạng");
            return false;
        }

        if (dateFrom !== 'Invalid date' && dateTo !== null) {
            if (dateFrom > dateTo) {
                ShowMessage_Danger("Ngày bắt đầu áp dụng không được lớn hơn Ngày kết thúc áp dụng");
                return false;
            }
        }

        // check chua nhap gtri ap dung
        if (self.HoaHongDoanhThu_TableDetail().length === 0) {
            ShowMessage_Danger("Vui lòng nhập khoảng doanh thu áp dụng");
            return false;
        }

        // check NhanVien
        if (nhanvien.length === 0) {
            ShowMessage_Danger("Vui lòng chọn nhân viên áp dụng");
            return false;
        }

        var check = CheckKhoangDoanhThu();
        if (check === false) {
            return;
        }

        var tinhCKTheo = self.HoaHongDoanhThu_TinhCKTheo();
        var strTinhCKTheo = '';

        switch (tinhCKTheo) {
            case '1':
                strTinhCKTheo = 'thực thu';
                break;
            case '2':
                strTinhCKTheo = 'doanh thu';
                break;
        }

        var objNew = {
            IDRandom: CreateIDRandom('HoaHongDT_'),
            ID_DonVi: _IDchinhanh,
            TinhChietKhauTheo: tinhCKTheo,
            Text_TinhChietKhauTheo: strTinhCKTheo,
            Text_LoaiNhanVienApDung: self.HoaHongDoanhThu_TextKieuNVApDung(),
            ApDungTuNgay: dateFrom,
            ApDungDenNgay: dateTo,
            NhanViens: nhanvien,
            DoanhThuChiTiet: self.HoaHongDoanhThu_TableDetail(),
            GhiChu: self.HoaHongDoanhThu_GhiChu(),
            TrangThai: 1,
            LoaiNhanVienApDung: self.HoaHongDoanhThu_GtriKieuNVApDung(),
        }

        var sID_NhanVien = '';
        for (var i = 0; i < nhanvien.length; i++) {
            sID_NhanVien += nhanvien[i].ID + ',';
        }
        sID_NhanVien = Remove_LastComma(sID_NhanVien);

        var idChietKhau = const_GuidEmpty;
        if (self.HoaHongDoanhThu_IsUpdate()) {
            idChietKhau = self.HoaHongDoanhThu_IDUpdate();
        }

        var Param_GetChietKhauHoaDon = {
            ID_DonVi: _IDchinhanh,
            ID_NhanViens: sID_NhanVien,
            ApDungTuNgay: moment(dateFrom, 'YYYY-MM-DD').format('YYYYMMDD'),
            ApDungDenNgay: dateTo == null ? '' : moment(dateTo, 'YYYY-MM-DD').format('YYYYMMDD'),
            ID_ChietKhauHoaDon: idChietKhau, // ~ ID_ChietKhauDoanhThu
            TinhChietKhauTheo: parseInt(tinhCKTheo),
            LoaiNhanVienApDung: objNew.LoaiNhanVienApDung,
        }

        ajaxHelper(NhanVienUri + 'CheckExist_ChietKhauDT_NhanVien', 'POST', Param_GetChietKhauHoaDon).done(function (data1) {
            let arrCode = [];
            if (data1.res == false) {
                var sTenNhanVien = '';
                for (var i = 0; i < data1.DataSoure.length; i++) {
                    if ($.inArray(data1.DataSoure[i].MaNhanVien, arrCode) == -1) {
                        arrCode.push(data1.DataSoure[i].MaNhanVien);
                        sTenNhanVien += data1.DataSoure[i].TenNhanVien + ', ';
                    }
                }
                sTenNhanVien = Remove_LastComma(sTenNhanVien);

                // if not exist --> insert/update DB
                ShowMessage_Danger('Nhân viên ' + sTenNhanVien + ' đã cài đặt chiết khấu hóa đơn trong khoảng thời gian này');
            }
            else {
                $('#modalCaiDatDoanhThu').modal('hide');
                Save_ChietKhauDoanhThu(objNew);
            }
        })
    }

    function Save_ChietKhauDoanhThu(objNew) {
        var sLoai = '', sDetail = '', sKhoangDT = '', sNVien = '', sOld = '';
        var sLoai = '';
        switch (parseInt(self.LoaiChietKhau())) {
            case 1:
                sLoai = 'theo hàng hóa - dịch vụ';
                break;
            case 2:
                sLoai = 'theo hóa đơn';
                break;
            case 3:
                sLoai = 'theo doanh thu - thực thu';
                break;
        }

        for (let i = 0; i < objNew.DoanhThuChiTiet.length; i++) {
            let itFor = objNew.DoanhThuChiTiet[i];
            sKhoangDT += formatNumber(itFor.DoanhThuTu).concat(' - ', formatNumber(itFor.DoanhThuDen), ' (',
                itFor.LaPhanTram ? itFor.GiaTriChietKhau + '%); ' : formatNumber(itFor.GiaTriChietKhau) + ' VND) ; ')
        }

        for (let i = 0; i < objNew.NhanViens.length; i++) {
            let itFor = objNew.NhanViens[i];
            sNVien += itFor.TenNhanVien.concat('(', itFor.MaNhanVien, '), ');
        }

        sDetail = '<br /> - Loại nhân viên áp dụng: '.concat(self.HoaHongDoanhThu_TextKieuNVApDung(),
            '<br /> - Tính chiết khấu theo: ', objNew.Text_TinhChietKhauTheo,
            '<br /> - Áp dụng từ: ', moment(objNew.ApDungTuNgay, 'YYYY-MM-DD').format('DD/MM/YYYY'),
            objNew.ApDungDenNgay ? ' - ' + moment(objNew.ApDungDenNgay, 'YYYY-MM-DD').format('DD/MM/YYYY') : '',
            '<br /> - Khoảng doanh thu: ', sKhoangDT,
            '<br /> - NViên áp dụng: ', sNVien,
        );
        var diary = {
            ID_DonVi: _IDchinhanh,
            ID_NhanVien: _idNhanVien,
            ChucNang: 'Cài đặt hoa hồng - '.concat(sLoai),
        };
        console.log(objNew)

        if (!self.HoaHongDoanhThu_IsUpdate()) {
            ajaxHelper(NhanVienUri + 'Add_ChietKhauDoanhThu', 'POST', objNew).done(function (data) {
                if (data.res == true) {
                    GetChietKhau_HoaDon_orDoanhThu();
                    ShowMessage_Success('Cài đặt chiết khấu doanh thu thành công');
                    CaculatorAgain_TotalRow(self.TotalRecordHD() + 1);

                    diary.LoaiNhatKy = 1;
                    diary.NoiDung = 'Thêm mới '.concat('Cài đặt hoa hồng - ', sLoai);
                    diary.NoiDungChiTiet = 'Thêm mới '.concat('Cài đặt hoa hồng - ', sLoai, sDetail);
                    Insert_NhatKyThaoTac_1Param(diary);
                }
                else {
                    console.log(data.mess);
                    ShowMessage_Danger('Cài đặt chiết khấu doanh thu thất bại');
                }
            })
        }
        else {
            objNew.ID = self.HoaHongDoanhThu_IDUpdate();
            ajaxHelper(NhanVienUri + 'Update_ChietKhauDoanhThu', 'POST', objNew).done(function (data) {
                if (data.res == true) {
                    sKhoangDT = '';
                    for (let i = 0; i < self.ItemOld().DoanhThuChiTiet.length; i++) {
                        let itFor = self.ItemOld().DoanhThuChiTiet[i];
                        sKhoangDT += formatNumber(itFor.DoanhThuTu).concat(' - ', formatNumber(itFor.DoanhThuDen), ' (',
                            itFor.LaPhanTram ? itFor.GiaTriChietKhau + '%); ' : formatNumber(itFor.GiaTriChietKhau) + ' VND) ; ')
                    }

                    sNVien = '';
                    for (let i = 0; i < self.ItemOld().NhanViens.length; i++) {
                        let itFor = self.ItemOld().NhanViens[i];
                        sNVien += itFor.TenNhanVien.concat('(', itFor.MaNhanVien, '), ');
                    }

                    sOld = '<br /> - Loại nhân viên áp dụng: '.concat(self.ItemOld().Text_LoaiNhanVienApDung,
                        '<br /> - Tính chiết khấu theo: ', self.ItemOld().Text_TinhChietKhauTheo,
                        '<br /> - Áp dụng từ: ', moment(self.ItemOld().ApDungTuNgay).format('DD/MM/YYYY'),
                        self.ItemOld().ApDungDenNgay ? ' - ' + moment(self.ItemOld().ApDungDenNgay).format('DD/MM/YYYY') : '',
                        '<br /> - Khoảng doanh thu: ', sKhoangDT,
                        '<br /> - NViên áp dụng: ', sNVien,
                    );

                    diary.LoaiNhatKy = 2;
                    diary.NoiDung = 'Cập nhật '.concat('Cài đặt hoa hồng - ', sLoai);
                    diary.NoiDungChiTiet = 'Cập nhật '.concat('Cài đặt hoa hồng - ', sLoai, sDetail,
                    );
                    Insert_NhatKyThaoTac_1Param(diary);

                    GetChietKhau_HoaDon_orDoanhThu();
                    ShowMessage_Success('Cập nhật cài đặt chiết khấu doanh thu thành công');
                }
                else {
                    console.log(data.mess);
                    ShowMessage_Danger('Cập nhật cài đặt chiết khấu doanh thu thất bại');
                }
            })
        }
    }

    self.HoaHongDoanhThu_Remove = function (item) {
        var thisObj = event.currentTarget;
        var id = item.ID;

        dialogConfirm('Xác nhận xóa', 'Bạn có chắc chắn muốn xóa cài đặt chiết khấu này không?', function () {
            ajaxHelper(NhanVienUri + 'Delete_ChietKhauDoanhThu/' + id, 'POST').done(function (data) {
                console.log(data);

                if (data.res == true) {
                    for (var i = 0; i < self.HoaHongDoanhThu_Table().length; i++) {
                        if (self.HoaHongDoanhThu_Table()[i].ID === id) {
                            self.HoaHongDoanhThu_Table.splice(i, 1);
                        }
                    }
                    ShowMessage_Success('Xóa cài đặt chiết khấu doanh thu thành công');

                    CaculatorAgain_TotalRow(self.TotalRecordHD() - 1);
                }
                else {
                    ShowMessage_Danger('Xóa cài đặt chiết khấu doanh thu thất bại');
                }
            })
        })
    }

    var khoangDT = []; nvApDung = [];
    self.HoaHongDoanhThu_ShowFormUpdate = function (item) {
        $('#modalCaiDatDoanhThu').modal('show');
        self.HoaHongDoanhThu_IsUpdate(true);
        HoaHongDoangThu_ActiveTab();

        // assign value
        var dateFrom = item.ApDungTuNgay;
        var dateTo = item.ApDungDenNgay;
        if (dateTo != null) {
            dateTo = moment(dateTo).format('DD/MM/YYYY');
        }
        dateFrom = moment(dateFrom).format('DD/MM/YYYY');

        self.HoaHongDoanhThu_GhiChu(item.GhiChu);
        self.HoaHongDoanhThu_ApDungTuNgay(dateFrom);
        self.HoaHongDoanhThu_ApDungDenNgay(dateTo);
        self.HoaHongDoanhThu_TinhCKTheo('' + item.TinhChietKhauTheo);

        for (var i = 0; i < item.DoanhThuChiTiet.length; i++) {
            item.DoanhThuChiTiet[i].IDRandom = CreateIDRandom('ChiTietDT_')
        }
        self.HoaHongDoanhThu_TableDetail(item.DoanhThuChiTiet);
        self.HoaHongHD_ListNhanVienChosed(item.NhanViens);
        self.HoaHongDoanhThu_IDUpdate(item.ID);

        self.HoaHongDoanhThu_GtriKieuNVApDung(item.LoaiNhanVienApDung);
        self.HoaHongDoanhThu_TextKieuNVApDung(item.Text);

        self.GetNhanVien_byDepartment();

        self.ItemOld(item);
        console.log('itemOld ', item)
    }

    self.ChangeTab = function () {

        //self.LoaiChietKhau(loaiDoanhThu);
        //console.log(self.LoaiChietKhau());
        self.currentPageHD(1);
        //console.log(self.LoaiChietKhau);
        switch (self.LoaiChietKhau()) {
            case '1':
                $('#hdDiscount').text('Cài đặt / Hoa hồng theo hàng hóa - dịch vụ');
                $('#hanghoadichvu').addClass('active');
                $('#theohoadon').removeClass('active');
                $('#doanhthuthucthu').removeClass('active');
                break;
            case '2':
                $('#hdDiscount').text('Cài đặt / Hoa hồng theo hóa đơn');
                $('#hanghoadichvu').removeClass('active');
                $('#theohoadon').addClass('active');
                $('#doanhthuthucthu').removeClass('active');
                $('#_cdhoahonghd').addClass('active');
                GetChietKhau_HoaDon_orDoanhThu();
                break;
            case '3':
                $('#hdDiscount').text('Cài đặt / Hoa hồng theo doanh thu - thực thu');
                $('#hanghoadichvu').removeClass('active');
                $('#theohoadon').removeClass('active');
                $('#doanhthuthucthu').addClass('active');
                GetChietKhau_HoaDon_orDoanhThu();
                break;
        }
    }

    function GetListChungTu_byKey(sKeys) {
        var arrKey = sKeys.split(',');

        var lst = [];
        for (var i = 0; i < arrKey.length; i++) {
            if (arrKey[i] !== '') {
                var chungtu = $.grep(self.ChungTuApDung(), function (x) {
                    return x.Key === parseInt(arrKey[i]);
                });
                if (chungtu.length > 0) {
                    lst.push(chungtu[0]);
                }
            }
        }
        return lst;
    }

    function GetChietKhauHoaDon_ByDonVi() {

        $('#tableTheoHoaDon').gridLoader();

        var Param_GetChietKhauHoaDon = {
            ID_NhanVien: self.selectedNhanVien(),
            ID_DonVi: _IDchinhanh,
            CurrentPage: self.currentPageHD(),
            PageSize: self.pageSize(),
        }

        ajaxHelper(NhanVienUri + 'Get_ChietKhauHoaDon_byDonVi', 'POST', Param_GetChietKhauHoaDon).done(function (data) {
            console.log(data)
            $('#tableTheoHoaDon').gridLoader({ show: false });
            if (data.res == true) {
                for (var i = 0; i < data.DataSoure.length; i++) {
                    var item = data.DataSoure[i];
                    var sTinhCKtheo = '';
                    data.DataSoure[i].IDRandom = CreateIDRandom('HoaHongHD_');
                    switch (item.TinhChietKhauTheo) {
                        case 1:
                            sTinhCKtheo = 'thực thu';
                            break;
                        case 2:
                            sTinhCKtheo = 'doanh thu';
                            break;
                        case 3:
                            sTinhCKtheo = 'VNĐ';
                            break;

                    }
                    data.DataSoure[i].Text_TinhChietKhauTheo = sTinhCKtheo;
                    data.DataSoure[i].ListChungTuApDung = GetListChungTu_byKey(item.ChungTuApDung);
                }
                self.HoaHongHD_Table(data.DataSoure);
                self.TotalRecordHD(data.TotalRecord);
                self.TotalPageHD(data.TotalPage);

                var currentPage = self.currentPageHD() - 1;
                self.FromItemHD(currentPage * self.pageSize() + 1);
                if ((self.currentPageHD() * self.pageSize()) > data.DataSoure.length) {
                    var fromItem = self.currentPageHD() * self.pageSize();
                    if (fromItem < self.TotalRecordHD()) {
                        self.ToItemHD(self.currentPageHD() * self.pageSize());
                    }
                    else {
                        self.ToItemHD(self.TotalRecordHD());
                    }
                } else {
                    self.ToItemHD(currentPage * self.pageSize() + self.pageSize());
                }
            }
        })
    }

    // paging ChietKhauHoaDon/DoanhThu
    self.currentPageHD = ko.observable(1);
    self.TotalRecordHD = ko.observable(0);
    self.TotalPageHD = ko.observable(0);
    self.LoaiChietKhau = ko.observable('1');
    self.FromItemHD = ko.observable(0);
    self.ToItemHD = ko.observable(0);

    self.GetClassHD = function (page) {
        return ((page.pageNumber) === self.currentPageHD()) ? "click" : "";
    };

    self.PageList_DisplayHD = ko.computed(function () {
        var arrPage = [];

        var allPage = self.TotalPageHD();
        var currentPage = self.currentPageHD();

        if (allPage > 0) {
            if (allPage > 4) {

                var i = 0;
                if (currentPage === 0) {
                    i = parseInt(currentPage) + 1;
                }
                else {
                    i = currentPage;
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

            // load cache hide/show colum at here
        }

        return arrPage;
    });

    self.VisibleStartPageHD = ko.computed(function () {
        if (self.PageList_DisplayHD().length > 0) {
            return self.PageList_DisplayHD()[0].pageNumber !== 1;
        }
    });

    self.VisibleEndPageHD = ko.computed(function () {
        if (self.PageList_DisplayHD().length > 0) {
            return self.PageList_DisplayHD()[self.PageList_DisplayHD().length - 1].pageNumber !== self.TotalPageHD();
        }
    });

    self.GoToPageHD = function (page) {
        if (page.pageNumber !== '.') {
            self.currentPageHD(page.pageNumber);
        }
        GetChietKhau_HoaDon_orDoanhThu();
    };

    self.StartPageHD = function () {
        self.currentPageHD(1);
        GetChietKhau_HoaDon_orDoanhThu();
    }

    self.BackPageHD = function () {
        if (self.currentPageHD() > 1) {
            self.currentPageHD(self.currentPageHD() - 1);
        }
        GetChietKhau_HoaDon_orDoanhThu();
    }

    self.GoToNextPageHD = function () {
        if (self.currentPageHD() < self.TotalPageHD() - 1) {
            self.currentPageHD(self.currentPageHD() + 1);
        }
        GetChietKhau_HoaDon_orDoanhThu();
    }

    self.EndPageHD = function () {
        if (self.currentPageHD() < self.TotalPageHD()) {
            self.currentPageHD(self.TotalPageHD());
        }
        GetChietKhau_HoaDon_orDoanhThu();
    }

    self.ResetCurrentPageHD = function () {
        self.currentPageHD(1);
        GetChietKhau_HoaDon_orDoanhThu();
    };

    function GetChietKhau_HoaDon_orDoanhThu() {
        $('.line-right').height(0).css("margin-top", "0px");
        switch (self.LoaiChietKhau()) {
            case '2':
                GetChietKhauHoaDon_ByDonVi();
                break;
            case '3':
                Get_ChietKhauDoanhThu_byDonVi();
                break;
        }
    }

    self.CheckLoaiNVApDung.subscribe(function () {
        self.currentPageHD(1);
        Get_ChietKhauDoanhThu_byDonVi();
    });

    self.TrangThaiConHan.subscribe(function () {
        self.currentPageHD(1);
        Get_ChietKhauDoanhThu_byDonVi();
    });

    function Get_ChietKhauDoanhThu_byDonVi() {
        var trangthai = '%%';
        var loainv = "%%";
        switch (parseInt(self.TrangThaiConHan())) {
            case 1:
                trangthai = '%0%';
                break;
            case 2:
                trangthai = '%2%';
                break;
        }

        switch (parseInt(self.CheckLoaiNVApDung())) {
            case 1:
                loainv = '%1%';
                break;
            case 2:
                loainv = '%2%';
                break;
            case 3:
                loainv = '%3%';
                break;
        }
        var Param_GetChietKhauHoaDon = {
            ID_NhanVien: self.selectedNhanVien(),
            ID_DonVi: _IDchinhanh,
            ApDungTuNgay: loainv,// muontamtruong
            ChungTuApDung: trangthai,// muontamtruong
            CurrentPage: self.currentPageHD() - 1,
            PageSize: self.pageSize(),
        }

        $('.popup-chiet-khau').gridLoader();
        ajaxHelper(NhanVienUri + 'Get_ChietKhauDoanhThu_byDonVi', 'POST', Param_GetChietKhauHoaDon).done(function (data) {
            $('.popup-chiet-khau').gridLoader({ show: false });

            if (data.res == true) {
                for (let i = 0; i < data.DataSoure.length; i++) {
                    let item = data.DataSoure[i];
                    let sTinhCKtheo = '';
                    data.DataSoure[i].IDRandom = CreateIDRandom('HoaHongDT_');
                    switch (item.TinhChietKhauTheo) {
                        case 1:
                            sTinhCKtheo = 'thực thu';
                            break;
                        case 2:
                            sTinhCKtheo = 'doanh thu';
                            break;
                    }
                    data.DataSoure[i].Text_TinhChietKhauTheo = sTinhCKtheo;
                }
                self.HoaHongDoanhThu_Table(data.DataSoure);
                self.TotalRecordHD(data.TotalRecord);
                self.TotalPageHD(data.TotalPage);

                var currentPage = self.currentPageHD() - 1;
                self.FromItemHD(currentPage * self.pageSize() + 1);
                if ((self.currentPageHD() * self.pageSize()) > data.DataSoure.length) {
                    var fromItem = self.currentPageHD() * self.pageSize();
                    if (fromItem < self.TotalRecordHD()) {
                        self.ToItemHD(self.currentPageHD() * self.pageSize());
                    }
                    else {
                        self.ToItemHD(self.TotalRecordHD());
                    }
                } else {
                    self.ToItemHD(currentPage * self.pageSize() + self.pageSize());
                }
            }
            else {
                console.log(data.mess)
            }
        })
    }
}

var newModelDiscount = new ViewModelDiscount();
ko.applyBindings(newModelDiscount);

newModelDiscount.LoaiChietKhau.subscribe(function (newValue) {
    newModelDiscount.ChangeTab();
});

function keypressEnterSelected(e) {
    if (e.keyCode == 13) {
        newModelDiscount.SelectedHHEnterkey();
    }
}

function itemSelected() {
    newModelDiscount.SelectedHHEnterkey();
}

//function itemSelected_LoHang(item) {
//    ctgiavm.SelectedHHEnterkey_LoHang(item);
//}

function HideLostFocust() {
    $('#showseach').delay(300).hide(0, function () {
    });
}

function containsAll(needles, haystack) {
    for (var i = 0, len = needles.length; i < len; i++) {
        if (needles[i] === '') continue;
        if (locdau(haystack).search(new RegExp(locdau(needles[i]), "i")) < 0) return false;
    }
    return true;
}

//$(document).on('click', '.newprice', function () {
//    var rowid = $(this).attr('id');
//    var oldPrice = formatNumberToInt($("#" + rowid).val());
//    $(this).next(".callprice").toggle();
//    $(".arrow-left2").mouseup(function () {
//        return false
//    });
//    $(".callprice").mouseup(function () {

//        return false
//    });
//    $(document).mouseup(function () {
//        $(".callprice").hide();
//    });
//});

//function fomartNumberDiscount(obj) {
//    var rowid = $(obj).attr('id');
//    var objsoluong = formatNumberObj($("#" + rowid))
//    objsoluong = formatNumber($("#" + rowid).val());
//    console.log(objsoluong, rowid);
//    $($("#" + rowid)).val(objsoluong);
//}

function fomartNumberDiscount(e, obj) {
    var elementAfer = $(obj).next();
    if (elementAfer.hasClass('gb')) {
        // chi cho phep nhap so
        return keypressNumber(e);
    }
    else {
        // cho phep nhap 3 so sau dau .
        keypressNumber_limitNumber(e, obj);
    }
}

// used to SaoChep_HoaDong
var arrID_NhanVien = [];
function SetCheckAllChildsCCD(obj) {
    var thisID = $(obj).attr('id');
    $(obj).removeClass('squarevt');
    var isChecked = $(obj).is(":checked");
    $('.NhanVien_ChuaCaiDat input').each(function () {
        $(this).prop('checked', isChecked);
    })

    if (isChecked) {
        $('.NhanVien_ChuaCaiDat input').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined && !(jQuery.inArray(thisID, arrID_NhanVien) > -1)) {
                arrID_NhanVien.push(thisID);
            }
        })
    }
    else {
        $('.NhanVien_ChuaCaiDat input').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined && (jQuery.inArray(thisID, arrID_NhanVien) > -1)) {
                $.map(arrID_NhanVien, function (item, i) {
                    if (item === thisID) {
                        arrID_NhanVien.splice(i, 1);
                    }
                })
            }
        })
    }
    console.log(arrID_NhanVien);
}
function SetCheckAllChild2sCCD(obj) {
    $('#all_nhanvienchuacaidat').prop('checked', false);
    var count = 0;
    var countcheck = 0;
    var thisID = $(obj).attr('id');
    $('#all_nhanvienchuacaidat').addClass('squarevt');
    $('.NhanVien_ChuaCaiDat input').each(function () {
        if ($(this).is(':checked')) {
            countcheck = countcheck + 1;
        }
        count += 1;
    })
    if (count === countcheck) {
        $('#all_nhanvienchuacaidat').removeClass('squarevt');
        $('#all_nhanvienchuacaidat').prop('checked', true);
    }
    if (countcheck === 0) {
        $('#all_nhanvienchuacaidat').removeClass('squarevt');
        $('#all_nhanvienchuacaidat').prop('checked', false);
    }
    if ($(obj).is(':checked')) {
        if (thisID !== undefined && !(jQuery.inArray(thisID, arrID_NhanVien) > -1)) {
            arrID_NhanVien.push(thisID);
        }
    }
    else {
        //remove item in arrID_NhanVien
        $.map(arrID_NhanVien, function (item, i) {
            if (item === thisID) {
                arrID_NhanVien.splice(i, 1);
            }
        })
    }
    console.log(arrID_NhanVien);
}
function SetCheckAllChildsDCD(obj) {
    var thisID = $(obj).attr('id');
    $(obj).removeClass('squarevt');
    var isChecked = $(obj).is(":checked");
    $('.NhanVien_DaCaiDat input').each(function () {
        $(this).prop('checked', isChecked);
    })

    if (isChecked) {
        $('.NhanVien_DaCaiDat input').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined && !(jQuery.inArray(thisID, arrID_NhanVien) > -1)) {
                arrID_NhanVien.push(thisID);
            }
        })
    }
    else {
        $('.NhanVien_DaCaiDat input').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined && (jQuery.inArray(thisID, arrID_NhanVien) > -1)) {
                $.map(arrID_NhanVien, function (item, i) {
                    if (item === thisID) {
                        arrID_NhanVien.splice(i, 1);
                    }
                })
            }
        })
    }
    console.log(arrID_NhanVien);
}
function SetCheckAllChild2sDCD(obj) {
    $('#all_nhanviendacaidat').prop('checked', false);
    var count = 0;
    var countcheck = 0;
    var thisID = $(obj).attr('id');
    $('#all_nhanviendacaidat').addClass('squarevt');
    $('.NhanVien_DaCaiDat input').each(function () {
        if ($(this).is(':checked')) {
            countcheck = countcheck + 1;
        }
        count += 1;
    })
    if (count === countcheck) {
        $('#all_nhanviendacaidat').removeClass('squarevt');
        $('#all_nhanviendacaidat').prop('checked', true);
    }
    if (countcheck === 0) {
        $('#all_nhanviendacaidat').removeClass('squarevt');
        $('#all_nhanviendacaidat').prop('checked', false);
    }
    if ($(obj).is(':checked')) {
        if (thisID !== undefined && !(jQuery.inArray(thisID, arrID_NhanVien) > -1)) {
            arrID_NhanVien.push(thisID);
        }
    }
    else {
        //remove item in arrID_NhanVien
        $.map(arrID_NhanVien, function (item, i) {
            if (item === thisID) {
                arrID_NhanVien.splice(i, 1);
            }
        })
    }
    console.log(arrID_NhanVien);
}