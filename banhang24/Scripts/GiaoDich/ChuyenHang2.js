ko.bindingHandlers.iframeContent = {
    update: function (element, valueAccessor) {
        var value = ko.unwrap(valueAccessor());
        element.contentWindow.document.close(); // Clear the content
        element.contentWindow.document.write(value);
    }
};


var FormModel_NewHoaDon = function () {
    var self = this;
    self.ID = ko.observable();
    self.MaHoaDon = ko.observable();
    self.TongTienHang = ko.observable();
    self.NgayLapHoaDon = ko.observable();
    self.ID_NhanVien = ko.observable();
    self.ID_DonVi = ko.observable();
    self.ID_CheckIn = ko.observable();
    self.NguoiTao = ko.observable();
    self.NgayTao = ko.observable();
    self.DienGiai = ko.observable(); // ghi chu
    self.BH_HoaDon_ChiTiet = ko.observableArray(); // List chi tiet hoa don
    self.LoaiHoaDon = loaiHoaDon;
    self.TonCNNhap = ko.observable();
    self.YeuCau = ko.observableArray();
    self.SoLuong = ko.observable();

    self.SetData = function (item) {
        self.ID(item.ID);
        self.ID_DonVi(item.ID_DonVi);
        self.ID_CheckIn(item.ID_CheckIn);
        self.MaHoaDon(item.MaHoaDon);
        self.TonCNNhap(item.TonCNNhap);
        self.TongTienHang(item.TongTienHang);
        self.NgayLapHoaDon(item.NgayLapHoaDon);
        self.ID_NhanVien(item.ID_NhanVien);
        self.NguoiTao(item.NguoiTao);
        self.NgayTao(item.NgayTao);
        self.DienGiai(item.DienGiai);
        self.SoLuong(item.SoLuong);
        self.YeuCau(item.YeuCau);
        self.BH_HoaDon_ChiTiet(item.BH_HoaDon_ChiTiet);

    }
}
var loaiHoaDon = '';
var ViewModelHD = function () {
    var self = this;
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var DMDonViUri = "/api/DanhMuc/DM_DonViAPI/"
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';
    var _id_NhanVien = $('.idnhanvien').text();
    var _IDchinhanh = $('#hd_IDdDonVi').val();
    var _IDNguoiDung = $('.idnguoidung').text();
    var Key_Form = 'Key_Tranfer';
    loaiHoaDon = $('#loaiHoaDon').val();
    $('#txtNgayTao').val('Tháng này');

    self.TodayBC = ko.observable('Toàn thời gian');
    self.TenChiNhanh = ko.observableArray();
    self.filterNgayLapHD = ko.observable("0");
    self.filterNgayLapHD_Input = ko.observable(); // ngày cụ thể
    self.filterNgayLapHD_Quy = ko.observable(6); // Theo tháng
    self.PageCount = ko.observable();
    self.TotalRecord = ko.observable(0);

    self.HoaDons = ko.observableArray();
    self.HangHoas = ko.observableArray();
    self.DonVis = ko.observableArray();
    self.ChiTietDoiTuong = ko.observableArray();
    self.BH_HoaDon_ChiTiet = ko.observableArray();
    self.NhanViens = ko.observableArray();

    self.ckPhieuTam = ko.observable(true);
    self.ckDangChuyen = ko.observable(true);
    self.ckDaNhan = ko.observable(true);
    self.ckDahuy = ko.observable(false);

    self.filter = ko.observable();
    self.deleteMaHoaDon = ko.observable();
    self.deleteID = ko.observable();

    self.selectedHHChuyenHang = ko.observable();
    self.selectedNV = ko.observable();
    self.selectedDonVi = ko.observable();
    self.error = ko.observable();
    self.booleanAdd = ko.observable(true);
    self.InforHDprintf = ko.observableArray();
    self.CTHoaDonPrint = ko.observableArray();
    self.CongTy = ko.observableArray(); // get infor CongTy
    self.TongTienHang = ko.observable();
    self.TongChiPhi = ko.observable();
    self.TongSLChuyen = ko.observable(0);
    self.TongSLNhan = ko.observable(0);
    self.newHoaDon = ko.observable(new FormModel_NewHoaDon());
    self.dataIframe = ko.observable();
    self.ChotSo_ChiNhanh = ko.observableArray();
    self.ChiNhanhs = ko.observableArray();
    self.MangNhomDV = ko.observableArray();
    self.MangIDDV = ko.observableArray();
    self.BH_HoaDonChiTiets = ko.observableArray();
    self.BH_HoaDonChiTietsThaoTac = ko.observableArray();
    self.ListTypeMauIn = ko.observableArray();
    self.ListLoHang = ko.observableArray();
    self.ListCheckBox = ko.observableArray();
    self.NumberColum_Div2 = ko.observableArray();
    self.columsort = ko.observable(null);
    self.sort = ko.observable(null);
    self.checkThietLapLoHang = ko.observable();

    self.ChuyenHang_ThayDoiThoiGian = ko.observable();
    self.ChuyenHang_ThayDoiNhanVien = ko.observable();
    self.HangHoa_XemGiaVon = ko.observable();
    self.Quyen_NguoiDung = ko.observableArray();
    self.DonViTinhs = ko.observableArray();
    self.selectIDDonViTinh = ko.observable();
    //phân trang
    self.pageSizes = [10, 20, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();
    self.arrPagging = ko.observableArray();

    self.currentPageCTNH = ko.observable();
    self.pageSizeCTNH = ko.observable(10);
    self.fromitemCTNH = ko.observable(0);
    self.toitemCTNH = ko.observable(0);
    self.PageCountCTNH = ko.observable();
    self.TotalRecordCTNH = ko.observable();

    self.ThietLap = ko.observableArray();
    self.Show_BtnInsert = ko.observable(false);
    self.Show_BtnUpdate = ko.observable(false);
    self.Show_BtnCopy = ko.observable(false);
    self.Show_BtnEdit = ko.observable(false);
    self.Show_BtnDelete = ko.observable(false);
    self.Show_BtnExcelDetail = ko.observable(false);
    self.Show_BtnOpenNhanHang = ko.observable(false);
    self.Show_BtnUpdateHDTam = ko.observable(false);
    self.Enable_NgayLapHD = ko.observable(true);

    this.YeuCau = ko.observableArray([
        { name: "Phiếu tạm", value: "1" },
        { name: "Đang hủy", value: "2" },
        { name: "Đã nhận", value: "3" },
        { name: "Đã hủy", value: "4" }
    ]);
    this.selectedYeuCau = ko.observable();

    function PageLoad() {
        LoadColumnCheck();
        GetHT_Quyen_ByNguoiDung();
        Check_QuyenXemGiaVon();
        GetDataChotSo();
        GetCauHinhHeThong();
        getAllChiNhanh();
        getListDonVi();
        getListNhanVien();
        SearchHoaDon();
        loadMauIn();
        GetInforCongTy();
    }

    PageLoad();
    console.log('ch')

    $('.modal-backdrop').remove();

    function LoadColumnCheck() {
        ajaxHelper('/api/DanhMuc/BaseAPI/' + "GetListColumnInvoices?loaiHD=10", 'GET').done(function (data) {
            self.ListCheckBox(data);
            self.NumberColum_Div2(Math.ceil(data.length / 2));
            LoadHtmlGrid();
        });
    }

    function LoadHtmlGrid() {
        LocalCaches.LoadFirstColumnGrid(Key_Form, $('#myList ul li input[type = checkbox]'), self.ListCheckBox());
    }

    // hide/show column from checkbox
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

    function CheckQuyenExist(maquyen) {
        var role = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen === maquyen;
        });
        if (role.length > 0) {
            return true;
        }
        return false;
    }

    function GetHT_Quyen_ByNguoiDung() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetListQuyen_OfNguoiDung", 'GET').done(function (data) {
                if (data !== "" && data.length > 0) {
                    self.Quyen_NguoiDung(data);
                    localStorage.setItem('lc_CTQuyen', JSON.stringify(data));
                    self.ChuyenHang_ThayDoiThoiGian(CheckQuyenExist('ChuyenHang_ThayDoiThoiGian'));
                    self.ChuyenHang_ThayDoiNhanVien(CheckQuyenExist('ChuyenHang_ThayDoiNhanVien'));
                    self.Show_BtnExcelDetail(CheckQuyenExist('ChuyenHang_XuatFile'));
                    self.Show_BtnCopy(CheckQuyenExist('ChuyenHang_SaoChep'));
                    self.Show_BtnDelete(CheckQuyenExist('ChuyenHang_Xoa'));
                    self.Show_BtnInsert(CheckQuyenExist('ChuyenHang_ThemMoi'));
                }
                else {
                    ShowMessage_Danger('Không có quyền');
                }
            });
        }
    }

    function Check_QuyenXemGiaVon() {
        ajaxHelper('/api/DanhMuc/ReportAPI/' + "getQuyenXemGiaVon?ID_NguoiDung=" + _IDNguoiDung + "&MaQuyen=" + "HangHoa_XemGiaVon", "GET").done(function (data) {
            if (data !== '') {
                self.HangHoa_XemGiaVon(true);
            }
            else {
                self.HangHoa_XemGiaVon(false);
            }
        });
    }

    function GetCauHinhHeThong() {
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + 'GetCauHinhHeThong/' + _IDchinhanh, 'GET').done(function (data) {
            localStorage.setItem('lc_CTThietLap', JSON.stringify(data));
            self.ThietLap(data);
        });
    }

    function GetDataChotSo() {
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/GetDataChotSo?idChiNhanh=' + _IDchinhanh, 'GET').done(function (data) {
            if (data !== null && data.length > 0) {
                self.ChotSo_ChiNhanh(data);
            }
        });
    }

    self.editHD = function (formElement) {
        var _maHoaDon = formElement.MaHoaDon;
        var _id = formElement.ID;
        var _idNhanVien = formElement.ID_NhanVien;
        var _ngayLapHoaDon = formElement.NgayLapHoaDon;
        var _dienGiai = formElement.DienGiai;
        var _tongTienhang = formElement.TongTienHang;
        var _tennhanvien = formElement.TenNhanVien;
        var _tendonvi = formElement.TenDonVi;
        var _tendoituong = formElement.TenDoiTuong;
        var _yeucau = formElement.NguoiSua;// muon tam {nguoisua <=> yeucau(int)}

        var HoaDon = {
            ID: _id,
            MaHoaDon: _maHoaDon,
            ID_NhanVien: _idNhanVien,
            TongTienHang: _tongTienhang,
            TongChiPhi: formElement.TongChiPhi,
            NgayLapHoaDon: _ngayLapHoaDon,
            NgaySua: formElement.NgaySua,
            NgayTao: self.newHoaDon().NgayTao(),
            DienGiai: _dienGiai,
            YeuCau: _yeucau,
            NguoiSua: _yeucau,
            TenNhanVien: _tennhanvien,
            TenDonVi: _tendonvi,
            TenDonViChuyen: formElement.TenDonViChuyen,
            TenDoiTuong: _tendoituong,
            BH_HoaDon_ChiTiet: self.newHoaDon().BH_HoaDon_ChiTiet(),
            LoaiHoaDon: loaiHoaDon,
            ChoThanhToan: false,
            NguoiTao: $('#txtTenTaiKhoan').text(),
            NguoiTaoHD: $('#txtTenTaiKhoan').text(),
        };

        var myData = {};
        myData.id = _id;
        myData.objNewHoaDon = HoaDon;

        $.ajax({
            data: myData,
            url: BH_HoaDonUri + "PutBH_HDChuyenHang",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                self.HoaDons.push(HoaDon);
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Cập nhật hóa đơn thành công", "success");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            },
            complete: function (item) {
                SearchHoaDon();
            }
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            });
    };

    function getAllChiNhanh() {
        ajaxHelper('/api/DanhMuc/DM_DonViAPI/' + "GetListDonViByIDNguoiDung?idnhanvien=" + _id_NhanVien, 'GET').done(function (data) {
            data = data.sort((a, b) => a.TenDonVi.localeCompare(b.TenDonVi, undefined, { caseFirst: "upper" }));
            self.ChiNhanhs(data);
            var obj = {
                ID: _IDchinhanh,
                TenDonVi: $('#_txtTenDonVi').html()
            }
            self.selectedCN(obj);
        });
    }

    self.selectedCN = function (item) {
        ResetSearch();
        var arrDV = [];
        for (var i = 0; i < self.MangNhomDV().length; i++) {
            if ($.inArray(self.MangNhomDV()[i], arrDV) === -1) {
                arrDV.push(self.MangNhomDV()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrDV) === -1) {
            self.MangNhomDV.push(item);
        }
        SearchHoaDon();
        //$('#choose_DonVi input').remove();
        // thêm dấu check vào đối tượng được chọn
        $('#selec-all-DV li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
            }
        });
        $('#choose_TenDonVi input').remove();
    }

    self.CloseDV = function (item) {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        self.MangNhomDV.remove(item);
        if (self.MangNhomDV().length === 0) {
            $('#choose_TenDonVi').append('<input type="text" id="dllChiNhanh" readonly="readonly" class="dropdown" placeholder="Chọn chi nhánh">');
        }
        SearchHoaDon();
        // remove check
        $('#selec-all-DV li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }

    function CheckKhoaSo(itemHD) {
        let chotso = false;
        switch (parseInt(itemHD.NguoiSua)) {// NguoiSua = YeuCau in BH_HoaDon
            case 2:// tamluu
                break;
            case 1:// dang chuyen
                break;
            case 4:// da nhan
                chotso = VHeader.CheckKhoaSo(moment(itemHD.NgayLapHoaDon).format('YYYY-MM-DD'), itemHD.ID_DonVi);
                chotso = chotso || VHeader.CheckKhoaSo(moment(itemHD.NgayLapHoaDon).format('YYYY-MM-DD'), itemHD.ID_CheckIn);
                break;
        }
        self.Enable_NgayLapHD(!chotso);
    }

    self.LoadChiTietHD = function (item, e) {
        CheckKhoaSo(item);

        self.currentPageCTNH(0);
        self.BH_HoaDonChiTiets([]);
        var tongmathangchuyen = 0;
        var tongmathangnhan = 0;
        $('.table-detal').gridLoader({
            style: "left: 460px;top: 200px;"
        });
        ajaxHelper(BH_HoaDonUri + 'GetChiTietHD_byIDHoaDonChuyenHangThaoTac?idHoaDon=' + item.ID + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
            $('.table-detal').gridLoader({ show: false });
            for (let i = 0; i < data.length; i++) {
                if (data[i].MaHangHoa.indexOf('{DEL}') > -1) {
                    data[i].MaHangHoa = data[i].MaHangHoa.substr(0, data[i].MaHangHoa.length - 5);
                    data[i].Del = '{Xóa}';
                } else {
                    data[i].Del = "";
                }
            }
            self.BH_HoaDonChiTietsThaoTac(data);
            searchCTHN();

            SetHeightShowDetail($(e.currentTarget));
            if (_IDchinhanh === item.ID_CheckIn) { // thisCN = chinhanh nhan
                // chua nhanhang
                if (item.NguoiSua !== "4") {
                    if (CheckQuyenExist('ChuyenHang_MoPhieu')) {
                        if (item.NguoiSua === "1" || item.NguoiSua === "2") {// dangchuyen
                            self.Show_BtnOpenNhanHang(true);
                        }
                        else {
                            self.Show_BtnOpenNhanHang(false);
                        }
                    }
                    else {
                        self.Show_BtnOpenNhanHang(false);
                    }
                }
                else {
                    self.Show_BtnOpenNhanHang(false);
                }
                self.Show_BtnUpdateHDTam(false);
                self.Show_BtnEdit(false);
                self.Show_BtnUpdate(false);
            }
            else {
                self.Show_BtnOpenNhanHang(false);
                if (item.NguoiSua === "2") {// tamluu
                    self.Show_BtnUpdateHDTam(true);
                    self.Show_BtnEdit(false);
                }
                else {
                    self.Show_BtnUpdateHDTam(false);
                    if (item.NguoiSua === "1") {// dangchuyen
                        if (_IDchinhanh === item.ID_DonVi) {//  thisCN = chinhanh chuyen 
                            self.Show_BtnEdit(true);
                        }
                        else {
                            self.Show_BtnEdit(false);
                        }
                    }
                    else {
                        self.Show_BtnEdit(false);
                    }
                }
            }

            var lc_CTThietLap = JSON.parse(localStorage.getItem('lc_CTThietLap'));
            self.checkThietLapLoHang(lc_CTThietLap.LoHang);
            for (let i = 0; i < self.BH_HoaDonChiTietsThaoTac().length; i++) {
                tongmathangchuyen += self.BH_HoaDonChiTietsThaoTac()[i].SoLuong;
                tongmathangnhan += self.BH_HoaDonChiTietsThaoTac()[i].GiamGia;
            }
            self.TongSLChuyen(tongmathangchuyen);
            self.TongSLNhan(tongmathangnhan);
        });
    }

    self.linkLoHangHoa = function (item) {
        localStorage.setItem('FindLoHang', item.MaLoHang);
        var url = "/#/Shipment";
        window.open(url);
    };

    self.FilterHangHoaChildren = function (item) {
        var txtSearch = $('#txtSearch' + item.ID).val();
        var objCT = [];
        if (txtSearch !== "") {
            for (var i = 0; i < self.BH_HoaDonChiTietsThaoTac().length; i++) {
                var sSearch = '';
                var arr = locdau(self.BH_HoaDonChiTietsThaoTac()[i].TenHangHoa).toLowerCase().split(/\s+/);
                for (var j = 0; j < arr.length; j++) {
                    sSearch += arr[j].toString().split('')[0];
                }
                var locdauMHH = locdau(self.BH_HoaDonChiTietsThaoTac()[i].MaHangHoa).toLowerCase();
                var locdauTenHH = locdau(self.BH_HoaDonChiTietsThaoTac()[i].TenHangHoa).toLowerCase();
                var MHH = locdauMHH.split(txtSearch);
                var THH = locdauTenHH.split(txtSearch);
                if (MHH.length > 1 || THH.length > 1 || locdau(self.BH_HoaDonChiTietsThaoTac()[i].TenHangHoa).indexOf(locdau(txtSearch)) >= 0 || sSearch.indexOf(locdau(txtSearch)) >= 0) {
                    objCT.push(self.BH_HoaDonChiTietsThaoTac()[i]);
                }
            }
            self.BH_HoaDonChiTiets(objCT);
        }
        else {
            searchCTHN();
        }
    }

    // not use (version old --> keep avoid err)
    self.xoaHD = function () {

    }

    self.modalDelete = function (item) {
        dialogConfirm('Thông báo xóa ', 'Có muốn hủy phiếu chuyển hàng <b>' + item.MaHoaDon + '</b> không?', function () {
            $.ajax({
                type: "Post",
                url: BH_HoaDonUri + "UPdate_YeuCauChuyenHang?id=" + item.ID + '&idnhanvien=' + _id_NhanVien + '&iddonvi=' + _IDchinhanh,
                dataType: 'json',
                contentType: 'application/json',
                success: function (result) {
                    SearchHoaDon();
                    bottomrightnotify("Xóa hóa đơn thành công!", "success");
                },
                error: function (error) {
                    bottomrightnotify("Xóa hóa đơn thất bại.", "danger");
                },
                complete: function () {
                    $('#modalPopuplgDelete').modal('hide');
                }
            });
        });
    };

    function getListDonVi() {
        ajaxHelper(DMDonViUri + "GetListDonVi1", 'GET').done(function (data) {
            for (let i = 0; i < data.length; i++) {
                if (_IDchinhanh === data[i].ID) {
                    data.splice(i, 1);
                }
            }
            self.DonVis(data);
        });
    }

    function getListNhanVien() {
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetNS_NhanVien_InforBasic?idDonVi=" + _IDchinhanh, 'GET').done(function (data) {
            self.NhanViens(data);
        });
    }

    function getChiTietHoaDonByID(id) {
        if (id !== undefined) {
            ajaxHelper("/api/DanhMuc/BH_HoaDonAPI/" + "GetChiTietHoaDon/" + id, 'GET').done(function (data) {
                self.newHoaDon().BH_HoaDon_ChiTiet(data);
            });
        }
    }

    function ResetSearch() {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        self.currentPage(0);
    }

    $('.choseNgayTao li').on('click', function () {
        $('#txtNgayTao').val($(this).text());
        self.filterNgayLapHD_Quy($(this).val());
        ResetSearch();
        SearchHoaDon();
    });

    function GetParamSearch() {
        var maHDFind = localStorage.getItem('FindHD');
        if (maHDFind !== null) {
            self.filter(maHDFind);
        }
        var txtMaHDon = self.filter();
        if (txtMaHDon === undefined) {
            txtMaHDon = "";
        }
        // trang thai: 
        var statusInvoice = 1;
        if (self.ckDangChuyen()) {
            if (self.ckPhieuTam()) {
                if (self.ckDahuy()) {
                    if (self.ckDaNhan()) {
                        statusInvoice = 15; // all
                    }
                    else {
                        statusInvoice = 11; // dangchuyen + tamluu + dahuy
                    }

                } else {
                    if (self.ckDaNhan()) {
                        statusInvoice = 12; // dangchuyen + tamluu + da nhanhang
                    }
                    else {
                        statusInvoice = 5; // dangchuyen + tamluu
                    }
                }
            }
            else {
                if (self.ckDahuy()) {
                    if (self.ckDaNhan()) {
                        statusInvoice = 14; // dangchuyen + dahuy + danhan
                    }
                    else {
                        statusInvoice = 6; // dangchuyen + dahuy
                    }

                } else {
                    if (self.ckDaNhan()) {
                        statusInvoice = 7; // dangchuyen + danhan
                    }
                    else {
                        statusInvoice = 1; // dangchuyen
                    }
                }
            }
        }
        else {
            if (self.ckPhieuTam()) {
                if (self.ckDahuy()) {
                    if (self.ckDaNhan()) {
                        statusInvoice = 13; // tamluu + dahuy + danhan
                    }
                    else {
                        statusInvoice = 8; // tamluu + dahuy
                    }

                } else {
                    if (self.ckDaNhan()) {
                        statusInvoice = 9; // tamluu + danhan
                    }
                    else {
                        statusInvoice = 2; // tamluu
                    }
                }
            }
            else {
                if (self.ckDahuy()) {
                    if (self.ckDaNhan()) {
                        statusInvoice = 10; // dahuy + danhan
                    }
                    else {
                        statusInvoice = 3; // dahuy
                    }

                } else {
                    if (self.ckDaNhan()) {
                        statusInvoice = 4; // danhan
                    }
                    else {
                        statusInvoice = 16; // khong check all
                    }
                }
            }
        }

        // NgayLapHoaDon
        var _now = new Date();  //current date of week
        var currentWeekDay = _now.getDay();
        var lessDays = currentWeekDay === 0 ? 6 : currentWeekDay - 1;
        var dayStart = '';
        var dayEnd = '';
        var dateChose = '';

        var arrDV = [];
        self.TenChiNhanh([]);
        for (var i = 0; i < self.MangNhomDV().length; i++) {
            if ($.inArray(self.MangNhomDV()[i], arrDV) === -1) {
                self.TenChiNhanh.push(self.MangNhomDV()[i].TenDonVi);
                arrDV.push(self.MangNhomDV()[i].ID);
            }
        }
        self.MangIDDV(arrDV);
        if (self.filterNgayLapHD() === '0') {

            switch (parseInt(self.filterNgayLapHD_Quy())) {
                case 0:
                    // all
                    self.TodayBC('Toàn thời gian');
                    dayStart = '2016-01-01';
                    dayEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
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
                    dayEnd = moment(_now).format('YYYY-MM-DD').add('days', 1);
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
                case 12:
                    // nam nay
                    self.TodayBC('Năm này');
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
            dayEnd = moment(arrDate[1], 'DD/MM/YYYY').add('days', 1).format('YYYY-MM-DD');
            self.TodayBC('Từ ' + moment(arrDate[0], 'DD/MM/YYYY').format('DD/MM/YYYY') + ' đến ' + moment(arrDate[1], 'DD/MM/YYYY').format('DD/MM/YYYY'));
        }

        return {
            LoaiHoaDon: loaiHoaDon,
            MaHoaDon: txtMaHDon,
            TrangThai: statusInvoice,
            DayStart: dayStart,
            DayEnd: dayEnd,
            ArrIDDonVi: self.MangIDDV(),
        }
    }

    function SearchHoaDon(isGoToNext) {
        isGoToNext = isGoToNext || false;
        $('.content-table').gridLoader();
        var param = GetParamSearch();

        ajaxHelper(BH_HoaDonUri + 'GetListHoaDonsChuyenHang_Where?currentPage=' + self.currentPage() + '&pageSize=' + self.pageSize() + '&loaiHoaDon=' + loaiHoaDon
            + '&maHoaDon=' + param.MaHoaDon + '&trangThai=' + param.TrangThai
            + '&dayStart=' + param.DayStart + '&dayEnd=' + param.DayEnd + '&id_donvi=' + _IDchinhanh + '&arrChiNhanh=' + param.ArrIDDonVi + '&columsort=' + self.columsort() + '&sort=' + self.sort(),
            'GET').done(function (data) {
                $('.content-table').gridLoader({ show: false });
                self.HoaDons(data.lstCH);

                self.TongTienHang(data.TongTienHang);
                self.TongChiPhi(data.TongChiPhi); // tra hang
                LoadHtmlGrid();
                if (!isGoToNext) {
                    self.TotalRecord(data.Rowcount);
                    self.PageCount(data.pageCount);
                }
            });
        localStorage.removeItem('FindHD');
    }

    self.clickiconSearch = function () {
        ResetSearch();
        SearchHoaDon();
    }

    $('#txtSearchCH').keypress(function (e) {
        ResetSearch();
        if (e.keyCode === 13) {
            SearchHoaDon();
        }
    });

    self.ckDangChuyen.subscribe(function (newVal) {
        ResetSearch();
        SearchHoaDon();
    });

    self.ckPhieuTam.subscribe(function (newVal) {
        ResetSearch();
        SearchHoaDon();
    });

    self.ckDahuy.subscribe(function (newVal) {
        ResetSearch();
        SearchHoaDon();
    });
    self.ckDaNhan.subscribe(function (newVal) {
        ResetSearch();
        SearchHoaDon();
    });

    self.filterNgayLapHD.subscribe(function (newVal) {
        ResetSearch();
        SearchHoaDon();
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
                    for (let i = allPage - 5; i < allPage; i++) {
                        let obj = {
                            pageNumber: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    if (currentPage === 1) {
                        for (let j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (let j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            let obj = {
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
                        let obj = {
                            pageNumber: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        let obj = {
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
                for (let i = 0; i < allPage; i++) {
                    let obj = {
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
            SearchHoaDon(true);
        }
    };

    self.StartPage = function () {
        self.currentPage(0);
        SearchHoaDon(true);
    }

    self.BackPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
            SearchHoaDon(true);
        }
    }

    self.GoToNextPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.currentPage() + 1);
            SearchHoaDon(true);
        }
    }

    self.EndPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
            SearchHoaDon(true);
        }
    }
    self.GetClassHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage()) ? "click" : "";
    };

    //sort colum chuyển hàng
    $('#tb thead tr').on('click', 'th', function () {
        var id = $(this).attr('id');
        if (id === "txtMaHoaDon") {
            self.columsort("MaHoaDon");
            SortGrid(id);
        }
        if (id === "txtTuChiNhanh") {
            self.columsort("TuChiNhanh");
            SortGrid(id);
        }
        if (id === "txtNguoiTao") {
            self.columsort("NguoiTao");
            SortGrid(id);
        }
        if (id === "txtToiChiNhanh") {
            self.columsort("ToiChiNhanh");
            SortGrid(id);
        }
        if (id === "txtNguoiNhan") {
            self.columsort("NguoiNhan");
            SortGrid(id);
        }
        if (id === "txtNgayChuyen") {
            self.columsort("NgayChuyen");
            SortGrid(id);
        }
        if (id === "txtNgayNhan") {
            self.columsort("NgayNhan");
            SortGrid(id);
        }
        if (id === "txtGiaChuyen") {
            self.columsort("GiaChuyen");
            SortGrid(id);
        }
        if (id === "txtGiaNhan") {
            self.columsort("GiaNhan");
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
        SearchHoaDon();
    };

    function searchCTHN(isGoToNext) {
        var pagecount = Math.ceil(self.BH_HoaDonChiTietsThaoTac().length / self.pageSizeCTNH());
        self.PageCountCTNH(pagecount);
        self.TotalRecordCTNH(self.BH_HoaDonChiTietsThaoTac().length);
        self.BH_HoaDonChiTiets(self.BH_HoaDonChiTietsThaoTac().slice(self.currentPageCTNH() * self.pageSizeCTNH(), self.currentPageCTNH() * self.pageSizeCTNH() + self.pageSizeCTNH()));
    };

    self.PageListCTNH = ko.computed(function () {
        var arrPage = [];
        var allPage = self.PageCountCTNH();
        var currentPage = self.currentPageCTNH();

        if (allPage > 4) {

            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPageCTNH()) + 1;
            }
            else {
                i = self.currentPageCTNH();
            }

            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (let i = allPage - 5; i < allPage; i++) {
                        let obj = {
                            pageNumberCTN: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    if (currentPage === 1) {
                        for (let j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            let obj = {
                                pageNumberCTN: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (let j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            let obj = {
                                pageNumberCTN: j + 1,
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
                            pageNumberCTN: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        let obj = {
                            pageNumberCTN: i,
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
                        pageNumberCTN: i + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }
        return arrPage;
    });

    self.PageResultCTNH = ko.computed(function () {
        if (self.BH_HoaDonChiTiets() !== null) {

            self.fromitemCTNH((self.currentPageCTNH() * self.pageSizeCTNH()) + 1);

            if (((self.currentPageCTNH() + 1) * self.pageSizeCTNH()) > self.BH_HoaDonChiTiets().length) {
                var fromItem = (self.currentPageCTNH() + 1) * self.pageSizeCTNH();
                if (fromItem < self.TotalRecordCTNH()) {
                    self.toitemCTNH((self.currentPageCTNH() + 1) * self.pageSizeCTNH());
                }
                else {
                    self.toitemCTNH(self.TotalRecordCTNH());
                }
            } else {
                self.toitemCTNH((self.currentPageCTNH() * self.pageSizeCTNH()) + self.pageSizeCTNH());
            }
        }
    });

    self.VisibleStartPageCTNH = ko.computed(function () {
        if (self.PageListCTNH().length > 0) {
            return self.PageListCTNH()[0].pageNumberCTN !== 1;
        }
    });

    self.VisibleEndPageCTNH = ko.computed(function () {
        if (self.PageListCTNH().length > 0) {
            return self.PageListCTNH()[self.PageListCTNH().length - 1].pageNumberCTN !== self.PageCountCTNH();
        }
    });

    self.GoToPageCTNH = function (page) {
        if (page.pageNumberCTN !== '.') {
            self.currentPageCTNH(page.pageNumberCTN - 1);
            searchCTHN();
        }
    };

    self.GetClassCTNH = function (page) {
        return ((page.pageNumberCTN - 1) === self.currentPageCTNH()) ? "click" : "";
    };

    self.StartPageCTNH = function () {
        self.currentPageCTNH(0);
        searchCTHN();
    }
    self.BackPageCTNH = function () {
        if (self.currentPageCTNH() > 1) {
            self.currentPageCTNH(self.currentPageCTNH() - 1);
            searchCTHN();
        }
    }
    self.GoToNextPageCTNH = function () {
        if (self.currentPageCTNH() < self.PageCountCTNH() - 1) {
            self.currentPageCTNH(self.currentPageCTNH() + 1);
            searchCTHN();
        }
    }
    self.EndPageCTNH = function () {
        if (self.currentPageCTNH() < self.PageCountCTNH() - 1) {
            self.currentPageCTNH(self.PageCountCTNH() - 1);
            searchCTHN();
        }
    }

    self.PageResults = ko.computed(function () {
        if (self.HoaDons() !== null) {

            self.fromitem((self.currentPage() * self.pageSize()) + 1);

            if (((self.currentPage() + 1) * self.pageSize()) > self.HoaDons().length) {
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

    self.PageList = ko.computed(function () {
        if (self.PageCount() > 1) {
            return Array.apply(null, {
                length: self.PageCount()
            }).map(Number.call, Number);
        }
    });

    self.ResetCurrentPage = function () {
        /**/
        self.currentPage(0);
        SearchHoaDon();
    };

    self.GoToPage = function (page) {
        self.currentPage(page);
    };

    self.GetClass = function (page) {
        return (page === self.currentPage()) ? "click" : "";
    };

    self.clickChuyenHang = function () {
        localStorage.removeItem('lcCH_EditOpen');
        localStorage.removeItem('createfrom');
        GoToChiTietNhap();
    }

    function GetCTHD_andSaveCache(arrCT, itemHD, type) {
        isSaoChep = false;
        if (type === 1) {
            isSaoChep = true;
            itemHD.ID = const_GuidEmpty;
        }
        let ngaylapHD = null;
        if (type === 3) {//  update chuyenhang
            ngaylapHD = itemHD.NgayLapHoaDon;
        }

        var cthdLoHang = [];
        var arrIDQuiDoi = [];
        var sumCT = 0;
        for (let i = 0; i < arrCT.length; i++) {
            var ctNew = $.extend({}, arrCT[i]);
            ctNew.ID_HoaDon = itemHD.ID;
            ctNew.MaHoaDon = itemHD.MaHoaDon;
            ctNew.ID_CheckIn = itemHD.ID_CheckIn;
            ctNew.ID_DonVi = itemHD.ID_DonVi;
            ctNew.DienGiai = itemHD.DienGiai;
            ctNew.NgayLapHoaDon = ngaylapHD;
            ctNew.NgaySua = itemHD.NgaySua;

            ctNew.GiaVon = ctNew.DonGia;
            ctNew.TonKho = parseFloat((ctNew.TonKho / ctNew.TyLeChuyenDoi).toFixed(3));
            ctNew.SoLuongChuyen = ctNew.SoLuong;
            ctNew.ThanhTien = ctNew.SoLuong * ctNew.GiaVon;
            sumCT += ctNew.ThanhTien;

            ctNew.DM_LoHang = [];
            ctNew.LotParent = ctNew.QuanLyTheoLoHang;
            ctNew.SoThuTu = cthdLoHang.length + 1;

            let ngaysx = ctNew.NgaySanXuat !== null ? moment(ctNew.NgaySanXuat).format('DD/MM/YYYY') : '';
            let hethan = ctNew.NgayHetHan !== null ? moment(ctNew.NgayHetHan).format('DD/MM/YYYY') : '';

            if (ngaysx === 'Invalid date') {
                ngaysx = '';
            }
            if (hethan === 'Invalid date') {
                hethan = '';
            }
            ctNew.NgaySanXuat = ngaysx;
            ctNew.NgayHetHan = hethan;

            if ($.inArray(ctNew.ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                arrIDQuiDoi.unshift(ctNew.ID_DonViQuiDoi);

                ctNew.ID_Random = CreateIDRandom('IDRandom_');
                if (ctNew.QuanLyTheoLoHang) {
                    // push DM_Lo
                    let objLot = $.extend({}, ctNew);
                    objLot.HangCungLoais = [];
                    objLot.DM_LoHang = [];
                    ctNew.DM_LoHang.push(objLot);
                }
                cthdLoHang.push(ctNew);
            }
            else {
                // find in cthdLoHang with same ID_QuiDoi
                for (let j = 0; j < cthdLoHang.length; j++) {
                    if (cthdLoHang[j].ID_DonViQuiDoi === ctNew.ID_DonViQuiDoi) {
                        if (ctNew.QuanLyTheoLoHang) {
                            // push DM_Lo
                            let objLot = $.extend({}, ctNew);
                            objLot.LotParent = false;
                            objLot.HangCungLoais = [];
                            objLot.DM_LoHang = [];
                            objLot.ID_Random = CreateIDRandom('IDRandom_');
                            cthdLoHang[j].DM_LoHang.push(objLot);
                        }
                        break;
                    }
                }
            }
        }
        cthdLoHang[0].TongTienHang = sumCT;
        localStorage.setItem('lcCH_EditOpen', JSON.stringify(cthdLoHang));
    }

    function GoToChiTietNhap() {
        window.location.href = '/#/TransfersCT2';
    }

    self.ChiTietMoPhieu = function (item) {
        GetCTHD_andSaveCache(self.BH_HoaDonChiTietsThaoTac(), item, 2);
        localStorage.setItem('createfrom', 2);// nhanhang
        GoToChiTietNhap();
    }

    self.SaoChep = function (item) {
        if (item.ID_DonVi !== _IDchinhanh) {
            ShowMessage_Danger("Phiếu chuyển hàng bạn chọn thuộc chi nhánh " + item.TenDonVi + ". Xin vui lòng chọn lại chi nhánh làm việc là " + item.TenDonVi + " để thực hiện Sao chép giao dịch này");
            return false;
        }
        else {
            GetCTHD_andSaveCache(self.BH_HoaDonChiTietsThaoTac(), item, 1);
            localStorage.setItem('createfrom', 1); // chuyenhang
            GoToChiTietNhap();
        }
    }
    // mophieu tamluu
    self.ChiTietMoPhieuTL = function (item, type) {
        if (type === 1) { // mo hdtam
            localStorage.setItem('createfrom', 3);
        }
        else {// update phieuchuyen
            localStorage.setItem('createfrom', 4);
        }
        GetCTHD_andSaveCache(self.BH_HoaDonChiTietsThaoTac(), item, 3);
        GoToChiTietNhap();
    }

    self.ExportExcel_ChuyenHang = function () {
        var param = GetParamSearch();
        //load columnhide from cache
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
        console.log('columnHide', columnHide);

        var url = BH_HoaDonUri + 'ExportExcel_ChuyenHang?loaiHoaDon=' + loaiHoaDon
            + '&maHoaDon=' + param.MaHoaDon + '&trangThai=' + param.TrangThai
            + '&dayStart=' + param.DayStart + '&dayEnd=' + param.DayEnd + '&id_donvi=' + _IDchinhanh + "&ColumnsHide=" + columnHide
            + '&arrChiNhanh=' + param.ArrIDDonVi + '&time=' + self.TodayBC() + '&TenChiNhanh=' + self.TenChiNhanh();
        window.location.href = url;

        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Chuyển hàng",
            NoiDung: "Xuất báo cáo danh sách phiếu chuyển hàng",
            NoiDungChiTiet: "Xuất báo cáo danh sách phiếu chuyển hàng",
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        Insert_NhatKyThaoTac_1Param(objDiary);
    };

    self.ExportExcel_ChiTietChuyenHang = function (item) {

        var url = BH_HoaDonUri + 'ExportExcel__ChiTietPhieuChuyenHang?ID_HoaDon=' + item.ID;
        window.location.href = url;

        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Chuyển hàng",
            NoiDung: "Xuất báo cáo phiếu chuyển hàng chi tiết theo mã: " + item.MaHoaDon,
            NoiDungChiTiet: "Xuất báo cáo phiếu chuyển hàng chi tiết theo mã: <a onclick=\"FindMaHD('" + item.MaHoaDon + "')\"> " + item.MaHoaDon + "</a>",
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        Insert_NhatKyThaoTac_1Param(objDiary);
    };

    function loadMauIn() {
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetListMauIn?typeChungTu=' + TeamplateMauIn + '&idDonVi=' + _IDchinhanh,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                self.ListTypeMauIn(result);
            }
        });
    }

    self.InHoaDon = function (item) {
        var cthdFormat = GetCTHDPrint_Format(self.BH_HoaDonChiTietsThaoTac());
        self.CTHoaDonPrint(cthdFormat);

        var itemHDFormat = GetInforHDPrint(item);
        self.InforHDprintf(itemHDFormat);

        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=' + TeamplateMauIn + '&idDonVi=' + _IDchinhanh,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                var data = '<script src="/Scripts/knockout-3.4.2.js"></script>';
                data = data.concat("<script > var item1=" + JSON.stringify(self.CTHoaDonPrint())
                    + "; var item4=[], item5=[]; var item2=" + JSON.stringify(self.CTHoaDonPrint())
                    + ";var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(result, data, 1);
            }
        });
    }

    self.PrintHoaDon = function (item, key) {
        var cthdFormat = GetCTHDPrint_Format(self.BH_HoaDonChiTietsThaoTac());
        self.CTHoaDonPrint(cthdFormat);

        var itemHDFormat = GetInforHDPrint(item);
        self.InforHDprintf(itemHDFormat);

        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrint?idMauIn=' + key,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                var data = '<script src="/Scripts/knockout-3.4.2.js"></script>';
                data = data.concat("<script > var item1=" + JSON.stringify(self.CTHoaDonPrint())
                    + "; var item4=[], item5=[]; var item2=" + JSON.stringify(self.CTHoaDonPrint())
                    + "; var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(result, data, 1);
            }
        });
    }

    function GetInforHDPrint(objHD) {
        var hdPrint = $.extend({}, objHD);
        hdPrint.NgayLapHoaDon = moment(objHD.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss');
        hdPrint.DiaChiKhachHang = hdPrint.DiaChi;
        hdPrint.DienThoaiKhachHang = hdPrint.DienThoai;
        hdPrint.TenCuaHang = hdPrint.TenDonVi;
        hdPrint.ChiNhanhChuyen = hdPrint.TenDonVi;
        hdPrint.ChiNhanhNhan = hdPrint.TenDonViChuyen;
        hdPrint.NguoiChuyen = hdPrint.NguoiTao;
        hdPrint.NguoiNhan = hdPrint.NguoiTaoHD;

        hdPrint.TongSoLuongChuyen = formatNumber3Digit(self.TongSLChuyen());
        hdPrint.TongSoLuongNhan = formatNumber3Digit(self.TongSLNhan());
        hdPrint.TongTienChuyen = formatNumber3Digit(objHD.TongTienHang, 2);
        hdPrint.TongTienNhan = formatNumber3Digit(objHD.TongChiPhi, 2);

        // logo cong ty
        if (self.CongTy().length > 0) {
            hdPrint.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
            hdPrint.TenCuaHang = self.CongTy()[0].TenCongTy;
            hdPrint.DiaChiCuaHang = self.CongTy()[0].DiaChi;
        }
        return hdPrint;
    }

    function GetCTHDPrint_Format(arrCTHD) {
        for (var i = 0; i < arrCTHD.length; i++) {
            arrCTHD[i].SoThuTu = i + 1;
            arrCTHD[i].TenHangHoa = arrCTHD[i].TenHangHoa + (arrCTHD[i].TenDonViTinh !== "" && arrCTHD[i].TenDonViTinh !== null ? "(" + arrCTHD[i].TenDonViTinh + ")" : "") + (arrCTHD[i].ThuocTinh_GiaTri !== null ? arrCTHD[i].ThuocTinh_GiaTri : "") + (arrCTHD[i].MaLoHang !== "" && arrCTHD[i].MaLoHang !== null ? "(Lô: " + arrCTHD[i].MaLoHang + ")" : "");
            arrCTHD[i].GiaChuyen = formatNumber3Digit(arrCTHD[i].DonGia, 2);
            arrCTHD[i].SoLuongChuyen = formatNumber3Digit(arrCTHD[i].SoLuong);
            arrCTHD[i].SoLuongNhan = (arrCTHD[i].YeuCau === '1' || arrCTHD[i].YeuCau === '2') ? 0 : formatNumber(arrCTHD[i].GiamGia);
            arrCTHD[i].ThanhTien = formatNumber3Digit(arrCTHD[i].ThanhTien, 2);
        }
        return arrCTHD;
    }

    function GetInforCongTy() {
        ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CongTy', 'GET').done(function (data) {
            if (data !== null) {
                self.CongTy(data);
            }
        });
    }

    $('#txtNgayTaoInput').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        SearchHoaDon();
    });
    //trinhpv import dieuchuyen
    self.DownloadFileTeamplateXLS = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_DanhSachHangDieuChuyen.xls";
        window.location.href = url;
    }
    //Download file teamplate excel format (*.xlsx)
    self.DownloadFileTeamplateXLSX = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_DanhSachHangDieuChuyen.xlsx";
        window.location.href = url;
    }
};

var modelChuyenHang = new ViewModelHD();
ko.applyBindings(modelChuyenHang);

$('input[type=text]').click(function () {
    $(this).select();
});


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
                "Tháng Một _ ",
                "Tháng Hai _ ",
                "Tháng Ba _ ",
                "Tháng Tư _ ",
                "Tháng Năm _ ",
                "Tháng Sáu _ ",
                "Tháng Bảy _ ",
                "Tháng Tám _ ",
                "Tháng Chín _ ",
                "Tháng Mười _ ",
                "Tháng Mười Một _ ",
                "Tháng Mười Hai _ "
            ],
            "firstDay": 1
        }
    });
});