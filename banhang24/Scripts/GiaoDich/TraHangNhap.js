var ViewModelHD = function () {
    var self = this;
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var DMDonViUri = "/api/DanhMuc/DM_DonViAPI/"
    var Quy_HoaDonUri = '/api/DanhMuc/Quy_HoaDonAPI/';
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';

    var _id_NhanVien = VHeader.IdNhanVien;
    var _IDchinhanh = VHeader.IdDonVi;
    var _userLogin = VHeader.UserLogin;
    var Key_Form = 'Key_PurchaseReturns';
    $('#txtNgayTao').val('Tháng này');

    self.TodayBC = ko.observable('Toàn thời gian');
    self.TenChiNhanh = ko.observableArray();
    self.filterNgayLapHD = ko.observable("0");
    self.filterNgayLapHD_Input = ko.observable(); // ngày cụ thể
    self.filterNgayLapHD_Quy = ko.observable('6'); // Theo tháng

    self.HoaDons = ko.observableArray();
    self.DonVis = ko.observableArray();
    self.BH_HoaDon_ChiTiet = ko.observableArray();
    self.NhanViens = ko.observableArray();
    self.LichSuThanhToan = ko.observableArray();
    self.ChotSo_ChiNhanh = ko.observableArray();
    self.ChiNhanhs = ko.observableArray();
    self.Quyen_NguoiDung = ko.observableArray();
    self.ListIDNhanVienQuyen = ko.observableArray();
    self.BH_HoaDonChiTiets = ko.observableArray();
    self.BH_HoaDonChiTietsThaoTac = ko.observableArray();
    self.ListTypeMauIn = ko.observableArray();
    self.ListLoHang = ko.observableArray();
    self.ListCheckBox = ko.observableArray();
    self.TT_TamLuu = ko.observable(true);
    self.TT_HoanThanh = ko.observable(true);
    self.TT_DaHuy = ko.observable(false);
    self.filter = ko.observable();

    self.selectedNV = ko.observable();
    self.selectedDonVi = ko.observable();
    self.error = ko.observable();
    self.InforHDprintf = ko.observableArray();
    self.CTHoaDonPrint = ko.observableArray();
    self.CongTy = ko.observableArray(); // get infor CongTy
    self.TongTienHang = ko.observable();
    self.TongThanhToan = ko.observable(0);
    self.TongTienThue = ko.observable();
    self.TongKhachTra = ko.observable();
    self.TongGiamGia = ko.observable();
    self.TongKhachNo = ko.observable();
    self.TongPhaiTraKhach = ko.observable();
    self.MangNhomDV = ko.observableArray();
    self.MangIDDV = ko.observableArray();
    self.NumberColum_Div2 = ko.observableArray();
    self.columsort = ko.observable(null);
    self.sort = ko.observable(null);
    self.checkThietLapLoHang = ko.observable();
    self.MangIDNhanVien = ko.observable();
    self.TraHangNhap_ThayDoiThoiGian = ko.observable();
    self.TraHangNhap_ThayDoiNhanVien = ko.observable();
    self.TongSLuong = ko.observable();

    //phân trang
    self.PageCount = ko.observable();
    self.TotalRecord = ko.observable(0);
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

    // role
    self.Show_BtnThanhToanCongNo = ko.observable(false);
    self.Show_BtnUpdateSoQuy = ko.observable(false);
    self.Show_BtnDeleteSoQuy = ko.observable(false);
    self.Allow_ChangeTimeSoQuy = ko.observable(false);
    self.Show_BtnInsert = ko.observable(false);
    self.Show_BtnCopy = ko.observable(false);
    self.Show_BtnUpdate = ko.observable(false);
    self.Show_BtnDelete = ko.observable(false);
    self.Show_BtnExport = ko.observable(false);
    self.Role_ChangeNhanVien = ko.observable(false);
    self.Role_ChangeThoiGian = ko.observable(false);

    $('.modal-backdrop').remove();

    function PageLoad() {
        LoadColumnCheck();
        LoadID_NhanVien();
        GetHT_Quyen_ByNguoiDung();
        GetDataChotSo();
        GetCauHinhHeThong();
        getAllChiNhanh();
        getListDonVi();
        getListNhanVien();
        SearchHoaDon();
        loadMauIn();
        GetInforCongTy();
        GetAllQuy_KhoanThuChi();
        GetDM_TaiKhoanNganHang();
        console.log(2)
    }

    PageLoad();

    function LoadColumnCheck() {
        ajaxHelper('/api/DanhMuc/BaseAPI/' + "GetListColumnInvoices?loaiHD=7", 'GET').done(function (data) {
            self.ListCheckBox(data);
            self.NumberColum_Div2(Math.ceil(data.length / 2));
            LoadHtmlGrid();
        });
    }

    function SetDefault_HideColumn() {
        var arrHideColumn = ['makhachhang', 'diachi', 'sodienthoai', 'tenchinhanh', 'nguoiban', 'tonggiamgia', 'ghichu', 'trangthai'];
        var cacheHideColumn = localStorage.getItem(Key_Form);
        if (cacheHideColumn === null || cacheHideColumn === '[]') {
            // hide default some column
            for (let i = 0; i < arrHideColumn.length; i++) {
                LocalCaches.AddColumnHidenGrid(Key_Form, arrHideColumn[i], arrHideColumn[i]);
            }
        }
    }

    function LoadHtmlGrid() {
        SetDefault_HideColumn();
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
        return role.length > 0;
    }

    function GetHT_Quyen_ByNguoiDung() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetListQuyen_OfNguoiDung", 'GET').done(function (data) {
                if (data !== "" && data.length > 0) {
                    self.Quyen_NguoiDung(data);
                    self.TraHangNhap_ThayDoiThoiGian(CheckQuyenExist('TraHangNhap_ThayDoiThoiGian'));
                    self.TraHangNhap_ThayDoiNhanVien(CheckQuyenExist('TraHangNhap_ThayDoiNhanVien'));
                    self.Show_BtnUpdateSoQuy(CheckQuyenExist('SoQuy_CapNhat'));
                    self.Show_BtnDeleteSoQuy(CheckQuyenExist('SoQuy_Xoa'));
                    self.Allow_ChangeTimeSoQuy(CheckQuyenExist('SoQuy_ThayDoiThoiGian'));

                    self.Show_BtnUpdate(CheckQuyenExist('TraHangNhap_CapNhat'));
                    self.Show_BtnCopy(CheckQuyenExist('TraHangNhap_SaoChep'));
                    self.Show_BtnDelete(CheckQuyenExist('TraHangNhap_Xoa'));
                    self.Show_BtnExport(CheckQuyenExist('TraHangNhap_XuatFile'));
                    self.Show_BtnInsert(CheckQuyenExist('TraHangNhap_ThemMoi'));
                }
                else {
                    ShowMessage_Danger('Không có quyền');
                }
            });
        }
    }

    function GetAllQuy_KhoanThuChi() {
        ajaxHelper(Quy_HoaDonUri + 'GetQuy_KhoanThuChi', 'GET').done(function (x) {
            if (x.res === true) {
                vmThanhToanNCC.listData.KhoanThuChis = x.data;
            }
        });
    }
    function GetDM_TaiKhoanNganHang() {
        if (navigator.onLine) {
            ajaxHelper(Quy_HoaDonUri + 'GetAllTaiKhoanNganHang_ByDonVi?idDonVi=' + _IDchinhanh, 'GET').done(function (x) {
                if (x.res === true) {
                    vmThanhToanNCC.listData.AccountBanks = x.data;
                }
            })
        }
    }

    function LoadID_NhanVien() {
        ajaxHelper('/api/DanhMuc/ChamSocKhachHangAPI/' + 'GetListNhanVienLienQuanByIDLoGin_inDepartment?idnvlogin=' + _id_NhanVien
            + '&idChiNhanh=' + _IDchinhanh + '&funcName=' + funcName, 'GET').done(function (data) {
                self.ListIDNhanVienQuyen(data);
                SearchHoaDon();
            });
    }

    self.ThietLap = ko.observableArray();

    function GetCauHinhHeThong() {
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + 'GetCauHinhHeThong/' + _IDchinhanh, 'GET').done(function (data) {
            self.checkThietLapLoHang(data.LoHang);
            self.ThietLap(data);
        });
    }

    function GetDataChotSo() {
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/GetDataChotSo?idChiNhanh=' + _IDchinhanh, 'GET').done(function (data) {
            if (data !== null && data.length > 0) {
                self.ChotSo_ChiNhanh(data);
            }
            getAllDMLoHang();
        });
    }

    function getAllDMLoHang() {
        var timeChotSo = '2016-01-01';
        if (self.ChotSo_ChiNhanh().length > 0) {
            timeChotSo = self.ChotSo_ChiNhanh()[0].NgayChotSo;
        }

        if (navigator.onLine) {
            ajaxHelper(DMHangHoaUri + "SP_GetAll_DMLoHang?iddonvi=" + _IDchinhanh + '&timeChotSo=' + timeChotSo, 'GET').done(function (data) {
                if (data !== null) {
                    self.ListLoHang(data);
                }
            });
        }
    }

    function CheckNgayLapHD_format(valDate, idDonVi = null) {

        var dateNow = moment(new Date()).format('YYYY-MM-DD HH:mm');
        var ngayLapHD = moment(valDate, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');

        if (valDate === '') {
            ShowMessage_Danger('Vui lòng nhập ngày lập hóa đơn');
            return false;
        }

        if (valDate.indexOf('_') > -1) {
            ShowMessage_Danger('Ngày lập hóa đơn chưa đúng định dạng');
            return false;
        }

        if (ngayLapHD > dateNow) {
            ShowMessage_Danger('Ngày lập phiếu nhập vượt quá thời gian hiện tại');
            return false;
        }

        let chotSo = VHeader.CheckKhoaSo(moment(ngayLapHD, 'YYYY-MM-DD HH:mm').format('YYYY-MM-DD'), idDonVi);
        if (chotSo) {
            ShowMessage_Danger(VHeader.warning.ChotSo.Update);
            return false;
        }
        return true;
    }

    self.ID_NhanVieUpdateHD = ko.observable();
    self.NgayLapHD_Update = ko.observable();

    self.GetID_NhanVien = function (item) {
        self.ID_NhanVieUpdateHD(item.ID_NhanVien); //--> get to do updateHoaDon
    }

    self.editHD = function (formElement) {
        var id = formElement.ID;
        var maHoaDon = formElement.MaHoaDon;
        var ngaylapHDOld = formElement.NgayLapHoaDon;
        var idNhanVien = self.ID_NhanVieUpdateHD();

        if (commonStatisJs.CheckNull(idNhanVien)) {
            idNhanVien = formElement.ID_NhanVien;
        }
        if (commonStatisJs.CheckNull(self.NgayLapHD_Update())) {
            self.NgayLapHD_Update(moment(formElement.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'));
        }
        var check = CheckNgayLapHD_format(self.NgayLapHD_Update(), formElement.ID_DonVi);

        if (!check) {
            return;
        }
        var ngaylapHD = moment(self.NgayLapHD_Update(), 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm:ss');
        ngaylapHDOld = moment(ngaylapHDOld).format('YYYY-MM-DD HH:mm:ss');
        var HoaDon = {
            ID: formElement.ID,
            MaHoaDon: maHoaDon,
            ID_NhanVien: idNhanVien,
            DienGiai: formElement.DienGiai,
            NguoiSua: _userLogin,
            NgayLapHoaDon: ngaylapHD,
        };

        var myData = {
            id: id,
            objNewHoaDon: HoaDon,
        };

        ajaxHelper(BH_HoaDonUri + "PutBH_HoaDon2", 'put', myData).done(function () {
            ShowMessage_Success('Cập nhật hóa đơn thành công');
            SearchHoaDon();

            var diary = {
                ID_NhanVien: _id_NhanVien,
                ID_DonVi: _IDchinhanh,
                ChucNang: 'Danh sách phiếu trả hàng nhập',
                NoiDung: "Cập nhật phiếu trả hàng nhập ".concat(maHoaDon),
                NoiDungChiTiet: "Cập nhật phiếu trả hàng nhập ".concat(maHoaDon,
                    '<br /> Ngày lập hóa đơn cũ: ', ngaylapHDOld, ', Ngày lập hóa đơn mới :', ngaylapHD),
                LoaiNhatKy: 2
            };
            if (formElement.ChoThanhToan === false) {
                diary.ID_HoaDon = id;
                diary.LoaiHoaDon = loaiHoaDon;
                diary.ThoiGianUpdateGV = ngaylapHDOld;
                Post_NhatKySuDung_UpdateGiaVon(diary);
            }
            else {
                Insert_NhatKyThaoTac_1Param(diary);
            }
        });
    };

    function getAllChiNhanh() {
        ajaxHelper('/api/DanhMuc/DM_DonViAPI/' + "GetListDonViByIDNguoiDung?idnhanvien=" + _id_NhanVien, 'GET').done(function (data) {
            data = data.sort((a, b) => a.TenDonVi.localeCompare(b.TenDonVi, undefined, { caseFirst: "upper" }));
            self.ChiNhanhs(data);
            vmThanhToanNCC.listData.ChiNhanhs = data;

            var obj = {
                ID: _IDchinhanh,
                TenDonVi: $('#_txtTenDonVi').html()
            }
            self.MangNhomDV.push(obj);
            $('#selec-all-DV li').each(function () {
                if ($(this).attr('id') === _IDchinhanh) {
                    $(this).find('.fa-check').remove();
                    $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                }
            });
            $('#choose_TenDonVi input').remove();
        });
    }

    self.selectedCN = function (item) {
        ResetSearch();
        var arrDV = [];
        for (let i = 0; i < self.MangNhomDV().length; i++) {
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

    self.GetLichSuThanhToan = function (item) {
        ajaxHelper(Quy_HoaDonUri + 'GetQuyHoaDon_byIDHoaDon?idHoaDon=' + item.ID, 'GET').done(function (data) {
            self.LichSuThanhToan(data);
        });
    }

    self.formatDateTime = function () {
        $('.datepicker_mask').datetimepicker(
            {
                format: "d/m/Y H:i",
                mask: true,
                timepicker: false,
            });
    }

    self.showPopThanhToan = function (item) {
        console.log('item ', item)

        if (self.CongTy().length > 0) {
            vmThanhToanNCC.inforCongTy = {
                TenCongTy: self.CongTy()[0].TenCongTy,
                DiaChiCuaHang: self.CongTy()[0].DiaChi,
                LogoCuaHang: Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang,
                TenChiNhanh: $('#_txtTenDonVi').text(),
            };
        }

        vmThanhToanNCC.showModalThanhToan(item);
    }

    self.InPhieuThu = function (item) {
        var itemHDFormat = GetInforPhieuThu(item);
        self.InforHDprintf(itemHDFormat);
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=SQPT&idDonVi=' + _IDchinhanh,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                var data = result;
                data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                data = data.concat("<script > var item1=[], item4=[], item5=[] ; var item2=[] ;var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(data);
            }
        });
    }
    function GetInforPhieuThu(objHD) {
        objHD.TenCuaHang = self.CongTy()[0].TenCongTy;
        objHD.DiaChiCuaHang = self.CongTy()[0].DiaChi;
        objHD.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
        objHD.ChiNhanhBanHang = objHD.TenChiNhanh;
        objHD.MaPhieu = objHD.MaHoaDon;
        objHD.NgayLapHoaDon = moment(objHD.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss');
        objHD.NguoiNhanTien = objHD.NguoiNopTien;
        objHD.DiaChiKhachHang = self.ItemHoaDon().DiaChiKhachHang;
        objHD.DienThoaiKhachHang = self.ItemHoaDon().DienThoai;
        objHD.TienBangChu = DocSo(formatNumberToInt(objHD.TongTienThu));
        objHD.GiaTriPhieu = formatNumber3Digit(objHD.TongTienThu, 2);
        return objHD;
    }

    self.Enable_NgayLapHD = ko.observable(true);

    self.LoadChiTietHD = function (item, e) {
        self.Enable_NgayLapHD(!VHeader.CheckKhoaSo(moment(item.NgayLapHoaDon).format('YYYY-MM-DD'), item.ID_DonVi));

        var thisObj = event.currentTarget;
        var ulTab = '';
        if (loaiHoaDon === 1 || loaiHoaDon === 0) {
            ulTab = $(thisObj).parent().next().find('.op-object-detail.nav-tabs');
        }
        else {
            ulTab = $(thisObj).next().find('.op-object-detail.nav-tabs');
        }
        ulTab.children('li').removeClass('active');
        ulTab.children('li').eq(0).addClass('active');
        ulTab.next().children('.tab-pane').removeClass('active');
        ulTab.next().children('.tab-pane:eq(0)').addClass('active');

        var roleInsertQuy = CheckQuyenExist('SoQuy_ThemMoi');
        if (roleInsertQuy) {
            let conno = item.TongTienHang - item.TongGiamGia - item.KhachDaTra;
            if (conno > 0) {
                self.Show_BtnThanhToanCongNo(true);
            }
            else {
                self.Show_BtnThanhToanCongNo(false);
            }
        }
        else {
            self.Show_BtnThanhToanCongNo(false);
        }

        self.currentPageCTNH(0);
        self.BH_HoaDonChiTiets([]);
        var tongsoluong = 0;
        $('.table-detal').gridLoader({
            style: "left: 460px;top: 200px;"
        });
        ajaxHelper(BH_HoaDonUri + 'GetChiTietHD_byIDHoaDon?idHoaDon=' + item.ID + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
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
            for (let i = 0; i < self.BH_HoaDonChiTietsThaoTac().length; i++) {
                tongsoluong += self.BH_HoaDonChiTietsThaoTac()[i].SoLuong;
            }
            self.TongSLuong(tongsoluong);
        });
        $('.txtNgayLapHD').datetimepicker({
            timepicker: true,
            mask: true,
            format: 'd/m/Y H:i',
            maxDate: new Date(),
            onChangeDateTime: function (dp, $input) {
                self.NgayLapHD_Update($input.val());
                CheckNgayLapHD_format(self.NgayLapHD_Update(), item.ID_DonVi);
            }
        });
    }

    self.linkLoHangHoa = function (item) {
        localStorage.setItem('FindLoHang', item.MaLoHang);
        window.open('/#/Shipment', '_blank');
    };

    self.gotoKhachHang = function (item) {
        localStorage.setItem('FindKhachHang', item.MaDoiTuong);
        window.open('/#/Suppliers', '_blank');
    };

    self.FilterHangHoaChildren = function (item) {
        var txtSearch = $('#txtSearch' + item.ID).val();
        var objCT = [];
        if (txtSearch !== "") {
            for (let i = 0; i < self.BH_HoaDonChiTietsThaoTac().length; i++) {
                var sSearch = '';
                var arr = locdau(self.BH_HoaDonChiTietsThaoTac()[i].TenHangHoa).toLowerCase().split(/\s+/);
                for (let j = 0; j < arr.length; j++) {
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

    self.modalDelete = function (item) {
        dialogConfirm('Thông báo xóa ', 'Có muốn hủy phiếu trả hàng hàng <b>' + item.MaHoaDon + '</b> không?', function () {
            $.getJSON(BH_HoaDonUri + "Huy_HoaDon?id=" + item.ID + '&nguoiSua=' + _userLogin + '&iddonvi=' + _IDchinhanh).done(function (x) {
                if (x === '') {
                    ShowMessage_Success('Hủy hóa đơn thành công');
                    SearchHoaDon();

                    let diary = {
                        ID_NhanVien: _id_NhanVien,
                        ID_DonVi: item.ID_DonVi,
                        LoaiNhatKy: 3,
                        ChucNang: 'Trả hàng nhập',
                        NoiDung: "Hủy phiếu trả hàng nhập ".concat(item.MaHoaDon),
                        NoiDungChiTiet: "Hủy phiếu trả hàng nhập ".concat(item.MaHoaDon,
                            ' <br />- Người hủy: ', VHeader.UserLogin,
                            ' <br />- Chi nhánh hủy: ', VHeader.TenDonVi),
                        LoaiHoaDon: item.LoaiHoaDon,
                        ID_HoaDon: item.ChoThanhToan === false ? item.ID : null,
                        ThoiGianUpdateGV: item.ChoThanhToan === false ? item.NgayLapHoaDon : null,
                    }
                    Post_NhatKySuDung_UpdateGiaVon(diary);
                }
                else {
                    ShowMessage_Danger('Hủy hóa đơn thất bại');
                }
            }).fail(function () {
                ShowMessage_Danger('Hủy hóa đơn thất bại');
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
            vmThanhToanNCC.listData.NhanViens = self.NhanViens();
        });
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
            self.filterNgayLapHD('0');
            self.filterNgayLapHD_Quy(0);
            localStorage.removeItem('FindHD');
        }
        var txtMaHDon = self.filter();
        if (txtMaHDon === undefined) {
            txtMaHDon = "";
        }
        // trang thai: 

        var arrStatus = [];
        if (self.TT_TamLuu()) {
            arrStatus.push('1');
        }
        if (self.TT_HoanThanh()) {
            arrStatus.push('0');
        }
        if (self.TT_DaHuy()) {
            arrStatus.push('4');
        }
        var statusInvoice = 1;
        if (self.TT_TamLuu()) {
            if (self.TT_HoanThanh()) {
                if (self.TT_DaHuy()) {
                    statusInvoice = 0; // all

                } else {
                    statusInvoice = 9; // tamluu + hoanthanh
                }
            }
            else {
                if (self.TT_DaHuy()) {
                    statusInvoice = 5; // tamluu + huy

                } else {
                    statusInvoice = 13; // tamluu
                }
            }
        }
        else {
            if (self.TT_HoanThanh()) {
                if (self.TT_DaHuy()) {
                    statusInvoice = 3; // hoanthanh + huy

                } else {
                    statusInvoice = 11; // hoanthanh
                }
            }
            else {
                if (self.TT_DaHuy()) {
                    statusInvoice = 7; // huy

                } else {
                    statusInvoice = 0; // not check (all)
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
        for (let i = 0; i < self.MangNhomDV().length; i++) {
            if ($.inArray(self.MangNhomDV()[i], arrDV) === -1) {
                self.TenChiNhanh.push(self.MangNhomDV()[i].TenDonVi);
                arrDV.push(self.MangNhomDV()[i].ID);
            }
        }
        if (arrDV.length === 0) {
            arrDV = [_IDchinhanh];
        }
        self.MangIDDV(arrDV);

        var arrIDNV = [];
        for (let i = 0; i < self.ListIDNhanVienQuyen().length; i++) {
            if ($.inArray(self.ListIDNhanVienQuyen()[i], arrIDNV) === -1) {
                arrIDNV.push(self.ListIDNhanVienQuyen()[i]);
            }
        }

        self.MangIDNhanVien(arrIDNV);

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
            maHDGoc: '',
            TrangThai: statusInvoice,
            DayStart: dayStart,
            DayEnd: dayEnd,
            arrChiNhanh: self.MangIDDV(),
            ArrIDNhanVien: arrIDNV,
            ArrTrangThai: arrStatus,
            id_BangGias: [],
            columsort: self.columsort(),
            sort: self.sort(),
            currentPage: self.currentPage(),
            pageSize: self.pageSize(),
        }
    }

    function ResetSumFooter() {
        self.TongTienHang(0);
        self.TongTienThue(0);
        self.TongGiamGia(0);
        self.TongKhachTra(0);
        self.TongKhachNo(0);
        self.TongThanhToan(0);
    }

    function SearchHoaDon(isGoToNext) {
        isGoToNext = isGoToNext || false;
        $('.content-table').gridLoader();
        var param = GetParamSearch();
        ajaxHelper(BH_HoaDonUri + 'GetList_HoaDonNhapHang', 'POST', param).done(function (x) {
            $('.content-table').gridLoader({ show: false });

            if (x.res) {
                let data = x.dataSoure;
                self.HoaDons(data);

                if (x.dataSoure.length > 0) {
                    let firstR = data[0];
                    self.TongTienHang(firstR.SumTongTienHang);
                    self.TongTienThue(firstR.SumTongTienThue);
                    self.TongGiamGia(firstR.SumTongGiamGia);
                    self.TongKhachTra(firstR.SumDaThanhToan);
                    self.TongKhachNo(firstR.SumConNo);
                    self.TongThanhToan(firstR.SumTongThanhToan);

                    if (!isGoToNext) {
                        self.TotalRecord(firstR.TotalRow);
                        self.PageCount(firstR.TotalPage);
                    }
                }
                else {
                    ResetSumFooter();
                }
            }
            else {
                ResetSumFooter();
            }
            LoadHtmlGrid();
        });
        localStorage.removeItem('FindHD');
    }

    self.clickiconSearch = function () {
        ResetSearch();
        SearchHoaDon();
    }

    $('#txtMaHoaDonTH').keypress(function (e) {
        ResetSearch();
        if (e.keyCode === 13) {
            SearchHoaDon();
        }
    });

    self.TT_TamLuu.subscribe(function (newVal) {
        ResetSearch();
        SearchHoaDon();
    });

    self.TT_HoanThanh.subscribe(function (newVal) {
        ResetSearch();
        SearchHoaDon();
    });

    self.TT_DaHuy.subscribe(function (newVal) {
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

        self.currentPage(0);
        SearchHoaDon();
    };

    self.GoToPage = function (page) {
        self.currentPage(page);
    };

    self.GetClass = function (page) {
        return (page === self.currentPage()) ? "click" : "";
    };

    self.clickTraHang = function () {
        localStorage.removeItem('lcCH_EditOpen');
        localStorage.removeItem('createfrom');
        GoToChiTietNhap();
    }

    function GetNgaySX_NgayHH(ctDoing) {
        var ngaysx = ctDoing.NgaySanXuat;
        if (!commonStatisJs.CheckNull(ngaysx)) {
            ngaysx = moment(ngaysx).format('DD/MM/YYYY');
        }
        var hansd = ctDoing.NgayHetHan;
        if (!commonStatisJs.CheckNull(hansd)) {
            hansd = moment(hansd).format('DD/MM/YYYY');
        }
        return {
            NgaySanXuat: ngaysx,
            NgayHetHan: hansd,
        }
    }

    function GetCTHD_andSaveCache(arrCT, itemHD, type) {
        isSaoChep = false;
        let ngaylapHD = new Date();
        let idHoaDon = itemHD.ID;
        if (type === 1) {
            isSaoChep = true;
            idHoaDon = const_GuidEmpty;
        }
        if (type === 2 || type === 3 || type === 4) {// mophieutam + update trahang
            ngaylapHD = itemHD.NgayLapHoaDon;
        }

        var cthdLoHang = [];
        var arrIDQuiDoi = [];
        for (let i = 0; i < arrCT.length; i++) {
            let ctNew = $.extend({}, arrCT[i]);
            ctNew.ID_HoaDon = idHoaDon;
            ctNew.MaHoaDon = type === 1 ? 'Copy' + itemHD.MaHoaDon : itemHD.MaHoaDon;
            ctNew.ID_DonVi = itemHD.ID_DonVi;
            ctNew.ID_DoiTuong = itemHD.ID_DoiTuong;
            ctNew.ID_NhanVien = itemHD.ID_NhanVien;
            ctNew.DienGiai = itemHD.DienGiai;
            ctNew.TongTienHang = itemHD.TongTienHang;
            ctNew.TongGiamGia = itemHD.TongGiamGia;
            ctNew.TongChietKhau = itemHD.TongChietKhau;
            ctNew.PhaiThanhToan = itemHD.PhaiThanhToan;
            ctNew.KhachDaTra = type === 1 ? 0 : itemHD.KhachDaTra;
            ctNew.NgayLapHoaDon = ngaylapHD;
            ctNew.PTThueHD = itemHD.PTThueHoaDon;
            ctNew.TongTienThue = itemHD.TongTienThue;
            ctNew.TongThanhToan = itemHD.TongThanhToan;
            ctNew.ID_PhieuNhapHang = itemHD.ID_HoaDon;

            ctNew.HangCungLoais = [];
            ctNew.LaConCungLoai = false;
            ctNew.GiaNhap = ctNew.DonGia;
            ctNew.ThanhTien = ctNew.SoLuong * ctNew.DonGia;

            let dateLot = GetNgaySX_NgayHH(ctNew);
            ctNew.DM_LoHang = [];
            ctNew.LotParent = ctNew.QuanLyTheoLoHang;
            ctNew.SoThuTu = cthdLoHang.length + 1;
            ctNew.NgaySanXuat = dateLot.NgaySanXuat;;
            ctNew.NgayHetHan = dateLot.NgayHetHan;;

            if ($.inArray(arrCT[i].ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                arrIDQuiDoi.unshift(arrCT[i].ID_DonViQuiDoi);
                ctNew.ID_Random = CreateIDRandom('IDRandom_');
                if (ctNew.QuanLyTheoLoHang) {
                    let objLot = $.extend({}, ctNew);
                    objLot.DM_LoHang = [];
                    objLot.HangCungLoais = [];
                    ctNew.DM_LoHang.push(objLot);
                }
                cthdLoHang.push(ctNew);
            }
            else {
                // find in cthdLoHang with same ID_QuiDoi
                for (let j = 0; j < cthdLoHang.length; j++) {
                    if (cthdLoHang[j].ID_DonViQuiDoi === ctNew.ID_DonViQuiDoi) {
                        if (ctNew.QuanLyTheoLoHang) {
                            let objLot = $.extend({}, ctNew);
                            objLot.LotParent = false;
                            objLot.DM_LoHang = [];
                            objLot.HangCungLoais = [];
                            objLot.ID_Random = CreateIDRandom('IDRandom_');
                            cthdLoHang[j].DM_LoHang.push(objLot);
                        }
                        break;
                    }
                }
            }
        }
        localStorage.setItem('THN_Chitiet', JSON.stringify(cthdLoHang));
    }

    function GoToChiTietNhap() {
        window.open('/#/PurchaseReturnsCT2', '_blank');
    }

    self.SaoChepTH = function (item) {
        GetCTHD_andSaveCache(self.BH_HoaDonChiTietsThaoTac(), item, 1);
        localStorage.setItem('THN_Thaotac', 1); // saochep
        GoToChiTietNhap();
    }
    // mophieu tamluu
    self.openHD = function (item) {
        localStorage.setItem('THN_Thaotac', 2);
        GetCTHD_andSaveCache(self.BH_HoaDonChiTietsThaoTac(), item, 2);
        GoToChiTietNhap();
    }
    // capnhat HD
    self.updateHD = function (item) {
        localStorage.setItem('THN_Thaotac', 4);
        GetCTHD_andSaveCache(self.BH_HoaDonChiTietsThaoTac(), item, 4);
        GoToChiTietNhap();
    }

    self.DownloadFileExportXLSX = function (url) {
        var url1 = DMHangHoaUri + "Download_fileExcel?fileSave=" + url;
        window.location.href = url1;
    }

    self.ExportExcel_TraHangNhap = function () {
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

        param.columnsHide = columnHide;
        param.currentPage = 0;
        param.pageSize = self.TotalRecord();
        ajaxHelper(BH_HoaDonUri + "ExportExcel_PhieuTraHangNhap", 'post', { objExcel: param }).done(function (url) {
            self.DownloadFileExportXLSX(url);
            var objDiary = {
                ID_NhanVien: _id_NhanVien,
                ID_DonVi: _IDchinhanh,
                ChucNang: "Trả hàng nhập",
                NoiDung: "Xuất excel danh sách phiếu trả hàng nhập",
                NoiDungChiTiet: "Xuất excel danh sách phiếu trả hàng nhập",
                LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
            };
            Insert_NhatKyThaoTac_1Param(objDiary);
        });
    };

    self.ExportExcel_ChiTietPhieuTraHangNhap = function (item) {

        var url = BH_HoaDonUri + 'ExportExcel__ChiTietPhieuTraHangNhap?ID_HoaDon=' + item.ID;
        window.location.href = url;

        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Trả hàng nhập",
            NoiDung: "Xuất excel phiếu trả hàng nhập chi tiết theo mã: " + item.MaHoaDon,
            NoiDungChiTiet: "Xuất excel phiếu trả hàng nhập chi tiết theo mã: <a onclick=\"FindMaHD('" + item.MaHoaDon + "')\"> " + item.MaHoaDon + "</a>",
            LoaiNhatKy: 6
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
                    + ";var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(result, data, 1);
            }
        });
    }

    function GetInforHDPrint(objHD) {
        var hd = $.extend({}, objHD);
        hd.NgayLapHoaDon = moment(hd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss');;
        var phaiTT = formatNumberToFloat(hd.PhaiThanhToan);
        var daTT = formatNumberToFloat(hd.KhachDaTra);
        var tongtien = formatNumberToFloat(hd.TongTienHang);
        hd.MaKhachHang = hd.MaDoiTuong;
        hd.TenNhaCungCap = hd.TenDoiTuong;
        hd.DiaChiKhachHang = hd.DiaChi;
        hd.DienThoaiKhachHang = hd.DienThoai;
        hd.NhanVienBanHang = hd.TenNhanVien;
        hd.TenChiNhanh = hd.TenDonVi;
        hd.TongTienHang = formatNumber3Digit(tongtien, 2);
        hd.PhaiThanhToan = formatNumber3Digit(phaiTT, 2);
        hd.DaThanhToan = formatNumber3Digit(daTT, 2);
        hd.TongCong = formatNumber3Digit(phaiTT, 2);
        hd.NoSau = formatNumber3Digit(phaiTT - daTT, 2);
        hd.TienBangChu = DocSo(phaiTT);
        hd.GhiChu = hd.DienGiai;
        hd.NoTruoc = 0;
        hd.PhiTraHang = 0;
        hd.TongSoLuongHang = formatNumber3Digit(self.TongSLuong());

        let pthuc = '';
        if (formatNumberToFloat(objHD.TienMat) > 0) {
            pthuc = 'Tiền mặt, ';
        }
        if (formatNumberToFloat(objHD.TienATM) > 0) {
            pthuc += 'POS, ';
        }
        if (formatNumberToFloat(objHD.ChuyenKhoan) > 0) {
            pthuc += 'Chuyển khoản, ';
        }
        if (formatNumberToFloat(objHD.TienDatCoc) > 0) {
            pthuc += 'Tiền cọc, ';
        }
        hd.PhuongThucTT = Remove_LastComma(pthuc);

        if (self.CongTy().length > 0) {
            hd.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
            hd.TenCuaHang = self.CongTy()[0].TenCongTy;
            hd.DiaChiCuaHang = self.CongTy()[0].DiaChi;
        }
        return hd;
    }

    function GetCTHDPrint_Format(arrCTHD) {
        var arr = [];
        for (let i = 0; i < arrCTHD.length; i++) {
            let ct = $.extend({}, arrCTHD[i]);
            ct.SoThuTu = i + 1;
            ct.TenHangHoa = ct.TenHangHoa
                + (ct.TenDonViTinh !== "" && ct.TenDonViTinh !== null ? "(" + ct.TenDonViTinh + ")" : "") + (ct.ThuocTinh_GiaTri !== null ? ct.ThuocTinh_GiaTri : "") + (ct.MaLoHang !== "" && ct.MaLoHang !== null ? "(Lô: " + ct.MaLoHang + ")" : "");
            ct.DonGia = formatNumber3Digit(ct.DonGia, 2);
            ct.SoLuong = formatNumber3Digit(ct.SoLuong);
            ct.ThanhTien = formatNumber3Digit(ct.ThanhTien, 2);
            ct.ThanhToan = formatNumber3Digit(ct.ThanhToan, 2);
            ct.TienThue = formatNumber3Digit(ct.TienThue, 2);
            arr.push(ct);
        }
        return arr;
    }

    function GetInforCongTy() {
        ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CongTy', 'GET').done(function (data) {
            if (data !== null) {
                self.CongTy(data);
            }
        });
    }

    $('#txtNgayTaoInput').on('apply.daterangepicker', function (ev, picker) {
        console.log(22423434)
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

    self.ShowPopup_InforHD_PhieuThu = function (item) {
        vmThanhToanNCC.showModalUpdate(item.ID);
    }
};

var modelTraHangNhap = new ViewModelHD();
ko.applyBindings(modelTraHangNhap);

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