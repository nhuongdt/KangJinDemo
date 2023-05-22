var ViewModelHD = function () {
    var self = this;
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var DMDonViUri = "/api/DanhMuc/DM_DonViAPI/"
    var Quy_HoaDonUri = '/api/DanhMuc/Quy_HoaDonAPI/';
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';

    var _id_NhanVien = $('.idnhanvien').text();
    var _IDchinhanh = $('#hd_IDdDonVi').val();
    var _IDNguoiDung = $('.idnguoidung').text();
    var userLogin = $('#txtTenTaiKhoan').text();
    var Key_Form = 'Key_PurchaseOrder';
    $('#txtNgayTao').val('Tháng này');

    var _now = new Date();
    var _nowFormat = moment(_now).format('YYYY-MM-DD');

    self.LoaiHoaDonMenu = ko.observable(parseInt($('#txtLoaiHoaDon').val()));
    self.shopCookies = ko.observable($('#shopCookies').val().toUpperCase())
    self.TodayBC = ko.observable('Toàn thời gian');
    self.TenChiNhanh = ko.observableArray();
    self.filterNgayLapHD = ko.observable("0");
    self.filterNgayLapHD_Input = ko.observable(); // ngày cụ thể
    self.filterNgayLapHD_Quy = ko.observable('6'); // Theo tháng

    var sLoai = 'nhập hàng';
    if (self.LoaiHoaDonMenu() === 31) {
        sLoai = 'đặt hàng nhà cung cấp';
    }

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
    self.GiaBans = ko.observableArray();
    self.KhoanChis = ko.observableArray();
    self.ListCheckBox = ko.observableArray();
    self.TT_TamLuu = ko.observable(true);
    self.TT_DangXuLy = ko.observable(true);
    self.TT_DaLuu = ko.observable(true);
    self.TT_HoanThanh = ko.observable(true);
    self.TT_DaHuy = ko.observable(false);
    self.filter = ko.observable();

    self.selectedNV = ko.observable();
    self.selectedDonVi = ko.observable();
    self.error = ko.observable();
    self.InforHDprintf = ko.observableArray();
    self.CTHoaDonPrint = ko.observableArray();
    self.CongTy = ko.observableArray(); // get infor CongTy
    self.ThanhTienChuaCK = ko.observable();
    self.GiamGiaCT = ko.observable();
    self.TongTienHang = ko.observable();
    self.TongChiPhi = ko.observable();
    self.TongKhachTra = ko.observable();
    self.TongTienMat = ko.observable();
    self.TongPOS = ko.observable();
    self.TongChuyenKhoan = ko.observable();
    self.TongTienCoc = ko.observable();
    self.TongGiamGia = ko.observable();
    self.TongKhachNo = ko.observable();
    self.TongPhaiTraKhach = ko.observable();
    self.TongTienThue = ko.observable();
    self.TongTienHangChuaCK = ko.observable();
    self.TongGiamGiaHang = ko.observable();
    self.PTChietKhauHH = ko.observable();
    self.MangNhomDV = ko.observableArray();
    self.MangIDDV = ko.observableArray();
    self.NumberColum_Div2 = ko.observableArray();
    self.columsort = ko.observable(null);
    self.sort = ko.observable(null);
    self.checkThietLapLoHang = ko.observable();
    self.MangIDNhanVien = ko.observable();
    self.HangHoa_XemGiaVon = ko.observable();
    self.TraHangNhap_ThemMoi = ko.observable();
    self.ChuyenHang_ThemMoi = ko.observable();
    self.HoaDon_ThemMoi = ko.observable();
    self.NhapHang_ThayDoiThoiGian = ko.observable();
    self.NhapHang_ThayDoiNhanVien = ko.observable();
    self.roleNhapHang_Insert = ko.observable(false);
    self.roleNhapHang_Export = ko.observable(false);

    self.TongSLuong = ko.observable();
    self.NgayLapHD_Update = ko.observable();
    self.selectedGiaBan = ko.observable();
    self.CheckInBangGia1 = ko.observable();
    self.CheckInMaHang1 = ko.observable();
    self.selectID_KhoanThu = ko.observable();
    self.arrPrintBarCode = ko.observableArray();
    self.selectID_KhoanThu = ko.observableArray();
    self.filterKhoanThuChi = ko.observable();

    self.Show_BtnUpdate = ko.observable(false);
    self.Show_BtnCopy = ko.observable(false);
    self.Show_BtnEdit = ko.observable(false);
    self.Show_BtnDelete = ko.observable(false);
    self.Show_BtnExcelDetail = ko.observable(false);
    self.Show_BtnThanhToanCongNo = ko.observable(false);
    self.Show_BtnUpdateHDTam = ko.observable(false);
    self.Show_BtnUpdateSoQuy = ko.observable(false);
    self.Show_BtnDeleteSoQuy = ko.observable(false);
    self.Allow_ChangeTimeSoQuy = ko.observable(false);
    self.role_NhapKhoNoiBo = ko.observable(VHeader.Quyen.indexOf('NhapKhoNoiBo') > -1);
    self.role_NhapHangKhachThua = ko.observable(VHeader.Quyen.indexOf('NhapHangKhachThua') > -1);
    self.Role_ChangeInvoice_ifOtherDate = ko.observable(false);

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
    self.LoaiInMaVach = ko.observableArray([
        { TenLoai: "In 3 tem", value: "3" },
        { TenLoai: "In 2 tem", value: "2" },
        { TenLoai: "In 65 tem", value: "65" }
    ]);

    self.LoaiHoaDon_4 = ko.observable(true);
    self.LoaiHoaDon_13 = ko.observable(true);
    self.LoaiHoaDon_14 = ko.observable(true);
    self.LoaiHoaDon_31 = ko.observable(true);

    switch (self.LoaiHoaDonMenu()) {
        case 4:
            self.LoaiHoaDon_31(false);
            self.LoaiHoaDon_13(false);
            self.LoaiHoaDon_14(false);
            break;
        case 13:
            self.LoaiHoaDon_4(false);
            self.LoaiHoaDon_31(false);
            self.LoaiHoaDon_14(false);
            break;
        case 31:
            self.LoaiHoaDon_4(false);
            self.LoaiHoaDon_13(false);
            self.LoaiHoaDon_14(false);
            break;
        case 14:
            self.LoaiHoaDon_4(false);
            self.LoaiHoaDon_13(false);
            self.LoaiHoaDon_31(false);
            break;
    }

    function PageLoad() {
        LoadColumnCheck();
        Check_QuyenXemGiaVon();
        LoadID_NhanVien();
        GetHT_Quyen_ByNguoiDung();
        GetDataChotSo();
        GetCauHinhHeThong();
        getAllChiNhanh();
        getListDonVi();
        getListNhanVien();
        loadMauIn();
        GetInforCongTy();
        getAllGiaBan();
        GetAllQuy_KhoanThuChi();
        GetDM_TaiKhoanNganHang();
    }

    PageLoad();

    function LoadColumnCheck() {
        let sLoai = 4;
        switch (self.LoaiHoaDonMenu()) {
            case 13:
            case 14:
                sLoai = self.LoaiHoaDonMenu();
                break;
        }
        ajaxHelper('/api/DanhMuc/BaseAPI/' + "GetListColumnInvoices?loaiHD=" + sLoai, 'GET').done(function (data) {
            self.ListCheckBox(data);
            self.NumberColum_Div2(Math.ceil(data.length / 2));
            LoadHtmlGrid();
        });
    }

    function SetDefault_HideColumn() {
        var arrHideColumn = ['loaichungtu', 'makhachhang', 'diachi', 'sodienthoai', 'tenchinhanh', 'nguoiban', 'tonggiamgia', 'ghichu', 'trangthai', 'tienthue'];
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

    function GetHT_Quyen_ByNguoiDung() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetListQuyen_OfNguoiDung", 'GET').done(function (data) {
                if (data !== "" && data.length > 0) {
                    self.Quyen_NguoiDung(data);
                    self.TraHangNhap_ThemMoi(CheckQuyenExist('TraHangNhap_ThemMoi'));
                    self.ChuyenHang_ThemMoi(CheckQuyenExist('ChuyenHang_ThemMoi'));
                    self.HoaDon_ThemMoi(CheckQuyenExist('HoaDon_ThemMoi'));
                    self.Show_BtnUpdateSoQuy(CheckQuyenExist('SoQuy_CapNhat'));
                    self.Show_BtnDeleteSoQuy(CheckQuyenExist('SoQuy_Xoa'));
                    self.Allow_ChangeTimeSoQuy(CheckQuyenExist('SoQuy_ThayDoiThoiGian'));

                    switch (self.LoaiHoaDonMenu()) {
                        case 4:
                            self.NhapHang_ThayDoiThoiGian(CheckQuyenExist('NhapHang_ThayDoiThoiGian'));
                            self.NhapHang_ThayDoiNhanVien(CheckQuyenExist('NhapHang_ThayDoiNhanVien'));
                            self.roleNhapHang_Insert(CheckQuyenExist('NhapHang_ThemMoi'));
                            self.roleNhapHang_Export(CheckQuyenExist('NhapHang_XuatFile'));
                            self.Show_BtnCopy(CheckQuyenExist('NhapHang_SaoChep'));
                            self.Show_BtnEdit(CheckQuyenExist('NhapHang_CapNhat'));
                            self.Show_BtnUpdate(CheckQuyenExist('NhapHang_CapNhat'));
                            self.Show_BtnDelete(CheckQuyenExist('NhapHang_Xoa'));
                            self.Show_BtnExcelDetail(CheckQuyenExist('NhapHang_XuatFile'));
                            break;
                        case 14:
                            self.NhapHang_ThayDoiThoiGian(CheckQuyenExist('NhapHang_ThayDoiThoiGian'));
                            self.NhapHang_ThayDoiNhanVien(CheckQuyenExist('NhapHang_ThayDoiNhanVien'));
                            self.roleNhapHang_Insert(CheckQuyenExist('NhapHang_ThemMoi'));
                            self.roleNhapHang_Export(CheckQuyenExist('NhapHang_XuatFile'));
                            self.Show_BtnCopy(CheckQuyenExist('NhapHang_SaoChep'));
                            self.Show_BtnEdit(CheckQuyenExist('NhapHang_CapNhat'));
                            self.Show_BtnUpdate(CheckQuyenExist('NhapHang_CapNhat'));
                            self.Show_BtnDelete(CheckQuyenExist('NhapHang_Xoa'));
                            self.Show_BtnExcelDetail(CheckQuyenExist('NhapHang_XuatFile'));
                            break;
                        case 13:
                            self.NhapHang_ThayDoiThoiGian(CheckQuyenExist('NhapNoiBo_ThayDoiThoiGian'));
                            self.NhapHang_ThayDoiNhanVien(CheckQuyenExist('NhapNoiBo_ThayDoiNhanVien'));
                            self.roleNhapHang_Insert(CheckQuyenExist('NhapNoiBo_ThemMoi'));
                            self.roleNhapHang_Export(CheckQuyenExist('NhapNoiBo_XuatFile'));
                            self.Show_BtnCopy(CheckQuyenExist('NhapNoiBo_SaoChep'));
                            self.Show_BtnEdit(CheckQuyenExist('NhapNoiBo_CapNhat'));
                            self.Show_BtnUpdate(CheckQuyenExist('NhapNoiBo_CapNhat'));
                            self.Show_BtnDelete(CheckQuyenExist('NhapNoiBo_Xoa'));
                            self.Show_BtnExcelDetail(CheckQuyenExist('NhapNoiBo_XuatFile'));
                            break;
                        case 31:
                            self.NhapHang_ThayDoiThoiGian(CheckQuyenExist('DatHangNCC_ThayDoiThoiGian'));
                            self.NhapHang_ThayDoiNhanVien(CheckQuyenExist('DatHangNCC_ThayDoiNhanVien'));
                            self.roleNhapHang_Insert(CheckQuyenExist('DatHangNCC_ThemMoi'));
                            self.roleNhapHang_Export(CheckQuyenExist('DatHangNCC_XuatFile'));
                            self.Show_BtnCopy(CheckQuyenExist('DatHangNCC_SaoChep'));
                            self.Show_BtnEdit(CheckQuyenExist('DatHangNCC_CapNhat'));
                            self.Show_BtnUpdate(CheckQuyenExist('DatHangNCC_CapNhat'));
                            self.Show_BtnDelete(CheckQuyenExist('DatHangNCC_Xoa'));
                            self.Show_BtnExcelDetail(CheckQuyenExist('DatHangNCC_XuatFile'));
                            break;
                    }
                }
                else {
                    ShowMessage_Danger('Không có quyền');
                }
            });
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


    function GetInforCongTy() {
        ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CongTy', 'GET').done(function (data) {
            if (data !== null) {
                self.CongTy(data);
                vmThanhToanNCC.inforCongTy = {
                    TenCongTy: self.CongTy()[0].TenCongTy,
                    DiaChiCuaHang: self.CongTy()[0].DiaChi,
                    LogoCuaHang: Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang,
                    TenChiNhanh: VHeader.TenDonVi,
                };
            }
        });
    }

    function getAllGiaBan() {
        ajaxHelper("/api/DanhMuc/DM_GiaBanAPI/" + "GetDM_GiaBanByIDDonVi?iddonvi=" + _IDchinhanh, 'GET').done(function (data) {
            if (data !== null && data.length > 0) {
                self.GiaBans(data);
            }
        });
    }

    function GetAllQuy_KhoanThuChi() {
        ajaxHelper(Quy_HoaDonUri + 'GetQuy_KhoanThuChi', 'GET').done(function (x) {
            if (x.res === true) {
                vmThanhToanNCC.listData.KhoanThuChis = x.data;
            }
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
            NguoiSua: userLogin,
            NgayLapHoaDon: ngaylapHD,
        };

        var myData = {
            id: id,
            objNewHoaDon: HoaDon,
        };

        ajaxHelper(BH_HoaDonUri + "PutBH_HoaDon2", 'post', myData).done(function () {
            ShowMessage_Success('Cập nhật hóa đơn thành công');
            SearchHoaDon();

            var diary = {
                ID_NhanVien: _id_NhanVien,
                ID_DonVi: _IDchinhanh,
                ChucNang: 'Danh sách phiếu nhập hàng',
                NoiDung: "Cập nhật phiếu nhập hàng ".concat(maHoaDon),
                NoiDungChiTiet: "Cập nhật phiếu nhập hàng ".concat(maHoaDon,
                    '<br /> Ngày lập hóa đơn cũ: ', ngaylapHDOld, ', Ngày lập hóa đơn mới :', ngaylapHD),
                LoaiNhatKy: 2
            };
            if (formElement.ChoThanhToan === false) {
                diary.ID_HoaDon = id;
                diary.LoaiHoaDon = self.LoaiHoaDonMenu();
                diary.ThoiGianUpdateGV = ngaylapHDOld;
                Post_NhatKySuDung_UpdateGiaVon(diary);
            }
            else {
                Insert_NhatKyThaoTac_1Param(diary);
            }

        }).fail(function (err) {
            ShowMessage_Danger(err);
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
        ajaxHelper(Quy_HoaDonUri + 'GetQuyHoaDon_byIDHoaDon?idHoaDon=' + item.ID + '&idHoaDonParent=' + item.ID_HoaDon, 'GET').done(function (data) {
            self.LichSuThanhToan(data);
        });
    }

    self.Enable_NgayLapHD = ko.observable(true);

    self.LoadChiTietHD = function (item, e) {
        self.Enable_NgayLapHD(!VHeader.CheckKhoaSo(moment(item.NgayLapHoaDon).format('YYYY-MM-DD'), item.ID_DonVi));

        let ngaylapFormat = moment(item.NgayLapHoaDon).format('YYYY-MM-DD');
        let role = CheckQuyenExist('GiaoDich_ChoPhepSuaDoiChungTu_NeuKhacNgayHienTai');// bat buoc chay lai sau khi gan quyen o ben duoi
        if (_nowFormat === ngaylapFormat) {// neu trung ngay: luon co quyen sua
            role = true;
        }
        self.Role_ChangeInvoice_ifOtherDate(role);

        var roleInsertQuy = CheckQuyenExist('SoQuy_ThemMoi');
        if (roleInsertQuy) {
            let conno = item.PhaiThanhToan - item.KhachDaTra;
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

            var tongtienchuaCK = data.reduce(function (_this, x) {
                return _this + (x.SoLuong * x.DonGia)
            }, 0);
            self.TongTienHangChuaCK(tongtienchuaCK);

            var tonggiamgiaHang = data.reduce(function (_this, x) {
                return _this + (x.SoLuong * x.TienChietKhau)
            }, 0);
            self.TongGiamGiaHang(tonggiamgiaHang);
        });
        self.GetLichSuThanhToan(item);

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

    function newLot(itemCTHD, itemLot) {
        if (itemLot === null || itemLot === undefined) {
            return objLot = {
                SoThuTu: 1,
                ID: itemCTHD.ID,
                DonGia: itemCTHD.DonGia,
                GiaVon: itemCTHD.GiaVon,
                SoLuong: itemCTHD.SoLuong,
                TonKho: itemCTHD.TonKho,
                TenHangHoa: itemCTHD.TenHangHoa,
                TyLeChuyenDoi: itemCTHD.TyLeChuyenDoi,
                ThanhTien: itemCTHD.ThanhTien,
                ID_DonViQuiDoi: itemCTHD.ID_DonViQuiDoi,
                TenDonViTinh: itemCTHD.TenDonViTinh,
                MaHangHoa: itemCTHD.MaHangHoa,
                TienChietKhau: itemCTHD.TienChietKhau,// tien giam gia
                DVTinhGiam: '%',
                GiaBan: itemCTHD.GiaBan,
                GiaBanHH: itemCTHD.GiaBanHH,
                GiamGia: itemCTHD.GiamGia,
                GiaTraLai: itemCTHD.GiaTraLai,
                SoLuongTra: itemCTHD.SoLuong,
                ThuocTinh_GiaTri: itemCTHD.ThuocTinh_GiaTri,
                DonViTinh: itemCTHD.DonViTinh,
                QuanLyTheoLoHang: true,
                GhiChu: itemCTHD.GhiChu,
                LotParent: true,
                ID_LoHang: null,
                ID_HangHoa: itemCTHD.ID_HangHoa,
                ID_Random: itemCTHD.ID_Random,
                NgaySanXuat: itemCTHD.NgaySanXuat ? (isCorrectFormat(moment(itemCTHD.NgaySanXuat).format('DD/MM/YYYY'), 'DD/MM/YYYY') === true ? moment(itemCTHD.NgaySanXuat, 'DD/MM/YYYY').format('DD/MM/YYYY') : itemCTHD.NgaySanXuat) : "",
                NgayHetHan: itemCTHD.NgayHetHan ? (isCorrectFormat(moment(itemCTHD.NgayHetHan).format('DD/MM/YYYY'), 'DD/MM/YYYY') === true ? moment(itemCTHD.NgayHetHan, 'DD/MM/YYYY').format('DD/MM/YYYY') : itemCTHD.NgayHetHan) : "",
                MaLoHang: itemCTHD.MaLoHang,
                NguoiTao: '',
                LoaiHoaDon: 1,
                DM_LoHang: [],
            };
        }
        else {
            var ngaysx = (itemLot.NgaySanXuat !== "" && itemLot.NgaySanXuat !== null) ? moment(itemLot.NgaySanXuat).format('DD/MM/YYYY') : "";
            var hethan = (itemLot.NgayHetHan !== "" && itemLot.NgayHetHan !== null) ? moment(itemLot.NgayHetHan).format('DD/MM/YYYY') : "";

            return objLot = {
                SoThuTu: 1,
                ID: itemCTHD.ID,
                DonGia: itemCTHD.DonGia,
                GiaVon: itemCTHD.GiaVon,
                SoLuong: itemCTHD.SoLuong,
                TonKho: itemCTHD.TonKho,
                TenHangHoa: itemCTHD.TenHangHoa,
                TyLeChuyenDoi: itemCTHD.TyLeChuyenDoi,
                ThanhTien: itemCTHD.ThanhTien,
                ID_DonViQuiDoi: itemCTHD.ID_DonViQuiDoi,
                TenDonViTinh: itemCTHD.TenDonViTinh,
                MaHangHoa: itemCTHD.MaHangHoa,
                TienChietKhau: itemCTHD.TienChietKhau,// tien giam gia
                DVTinhGiam: '%',
                GiaBan: itemCTHD.GiaBan,
                GiaBanHH: itemCTHD.GiaBanHH,
                GiamGia: itemCTHD.GiamGia,
                GiaTraLai: itemCTHD.GiaTraLai,
                SoLuongTra: itemCTHD.SoLuong,
                ThuocTinh_GiaTri: itemCTHD.ThuocTinh_GiaTri,
                DonViTinh: itemCTHD.DonViTinh,
                QuanLyTheoLoHang: true,
                GhiChu: itemCTHD.GhiChu,
                LotParent: true,
                ID_LoHang: itemLot.ID,
                ID_HangHoa: itemCTHD.ID_HangHoa,
                ID_Random: itemCTHD.ID_Random,
                NgaySanXuat: ngaysx,
                NgayHetHan: hethan,
                MaLoHang: itemLot.MaLoHang,
                NguoiTao: '',
                LoaiHoaDon: itemCTHD.LoaiHoaDon,
                DM_LoHang: [],
            }
        }
    }

    self.linkLoHangHoa = function (item) {
        localStorage.setItem('FindLoHang', item.MaLoHang);
        var url = "/#/Shipment";
        window.open(url);
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
        ajaxHelper(BH_HoaDonUri + 'GetDSHoaDon_chuaHuy_byIDDatHang?id=' + item.ID , 'GET').done(function (x) {
            if (x === true) {
                switch (item.LoaiHoaDon) {
                    case 4:
                        ShowMessage_Danger('Có trả hàng nhập từ phiếu nhập, không thể xóa');
                        break;
                    case 31:
                        ShowMessage_Danger('Phiếu đặt hàng đã nhập mua, không thể hủy');
                        break;
                }
            }
            else {
                dialogConfirm('Thông báo xóa ', 'Bạn có chắc chắn muốn hủy phiếu nhập hàng <b>' + item.MaHoaDon + '</b> không?', function () {
                    $.getJSON(BH_HoaDonUri + "Huy_HoaDon?id=" + item.ID + '&nguoiSua=' + userLogin + '&iddonvi=' + _IDchinhanh).done(function (x) {
                        if (x === '') {
                            ShowMessage_Success('Hủy hóa đơn thành công');
                            SearchHoaDon();

                            let diary = {
                                ID_NhanVien: _id_NhanVien,
                                ID_DonVi: item.ID_DonVi,
                                LoaiNhatKy: 3,
                                ChucNang: commonStatisJs.FirstChar_UpperCase(sLoai),
                                NoiDung: "Hủy phiếu ".concat(sLoai, ' ', item.MaHoaDon),
                                NoiDungChiTiet: "Hủy phiếu ".concat(sLoai, ' ', item.MaHoaDon,
                                    ' <br />- Người hủy: ', userLogin,
                                    ' <br />- Chi nhánh hủy: ', VHeader.TenDonVi),
                                LoaiHoaDon: item.LoaiHoaDon,
                                ID_HoaDon: item.ChoThanhToan === false ? item.ID : null,
                                ThoiGianUpdateGV: item.ChoThanhToan === false ? item.NgayLapHoaDon : null,
                            }
                            Post_NhatKySuDung_UpdateGiaVon(diary);

                            UpdateStatudHD(item.ID_HoaDon);
                        }
                        else {
                            ShowMessage_Danger('Hủy hóa đơn thất bại');
                        }
                    }).fail(function () {
                        ShowMessage_Danger('Hủy hóa đơn thất bại');
                    });
                });
            }
        });
    };

    function UpdateStatudHD(idDatHang) {
        if (!commonStatisJs.CheckNull(idDatHang)) {
            ajaxHelper(BH_HoaDonUri + 'UpdateStatus_HDDatHang?id=' + idDatHang + '&nguoiSua=' + VHeader.UserLogin
                + '&loaihoadon=4', 'GET').done(function (data) {
                    if (data === '') {
                    }
                }).fail(function () {
                });
        }
    }

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
            vmThanhToanNCC.listData.NhanViens = data;
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
        }
        var txtMaHDon = self.filter();
        if (txtMaHDon === undefined) {
            txtMaHDon = "";
        }

        var arrStatus = [];
        if (self.TT_TamLuu()) {
            arrStatus.push('0');
        }
        if (self.TT_DaLuu()) {
            arrStatus.push('1');
        }
        if (self.TT_DangXuLy()) {
            arrStatus.push('2');
        }
        if (self.TT_HoanThanh()) {
            arrStatus.push('3');
        }
        if (self.TT_DaHuy()) {
            arrStatus.push('4');
        }

        let arrLoaiHD = [];
        if (self.LoaiHoaDon_4()) {
            arrLoaiHD.push(4);
        }
        if (self.LoaiHoaDon_13()) {
            arrLoaiHD.push(13);
        }
        if (self.LoaiHoaDon_14()) {
            arrLoaiHD.push(14);
        }
        if (self.LoaiHoaDon_31()) {
            arrLoaiHD.push(31);
        }

        // NgayLapHoaDon
        var currentWeekDay = _now.getDay();
        var lessDays = currentWeekDay === 0 ? 6 : currentWeekDay - 1;
        var dayStart = '';
        var dayEnd = '';

        var arrDV = [];
        self.TenChiNhanh([]);
        for (let i = 0; i < self.MangNhomDV().length; i++) {
            if ($.inArray(self.MangNhomDV()[i], arrDV) === -1) {
                self.TenChiNhanh.push(self.MangNhomDV()[i].TenDonVi);
                arrDV.push(self.MangNhomDV()[i].ID);
            }
        }
        self.MangIDDV(arrDV);
        if (arrDV.length === 0) {
            arrDV = [_IDchinhanh];
        }

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
                    dayStart = moment(new Date(_now.setDate(_now.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
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
            loaiHoaDon: self.LoaiHoaDonMenu(),
            arrLoaiHoaDon: arrLoaiHD,
            maHoaDon: txtMaHDon,
            maHDGoc: '',
            dayStart: dayStart,
            dayEnd: dayEnd,
            id_donvi: _IDchinhanh,
            arrChiNhanh: arrDV,
            id_NhanViens: arrIDNV,
            id_ViTris: arrStatus,
            ArrTrangThai: arrStatus,
            id_BangGias: [],
            columsort: self.columsort(),
            sort: self.sort(),
            currentPage: self.currentPage(),
            pageSize: self.pageSize(),
        }
    }

    function SearchHoaDon(isGoToNext) {
        isGoToNext = isGoToNext || false;
        $('.content-table').gridLoader();
        var param = GetParamSearch();
        console.log(param)
        ajaxHelper(BH_HoaDonUri + 'GetList_HoaDonNhapHang', 'post', param).done(function (x) {
            $('.content-table').gridLoader({ show: false });

            if (x.res) {
                var data = x.dataSoure;
                self.HoaDons(data);
                if (data.length > 0) {
                    var firstR = data[0];
                    self.ThanhTienChuaCK(firstR.SumThanhTienChuaCK);
                    self.GiamGiaCT(firstR.SumGiamGiaCT);
                    self.TongTienHang(firstR.SumTongTienHang);
                    self.TongChiPhi(firstR.SumTongChiPhi);
                    self.TongTienThue(firstR.SumTongTienThue);
                    self.TongGiamGia(firstR.SumTongGiamGia);
                    self.TongPhaiTraKhach(firstR.SumTongThanhToan);
                    self.TongKhachTra(firstR.SumDaThanhToan);
                    self.TongTienMat(firstR.SumTienMat);
                    self.TongPOS(firstR.SumPOS);
                    self.TongChuyenKhoan(firstR.SumChuyenKhoan);
                    self.TongTienCoc(firstR.SumTienCoc);
                    self.TongKhachNo(firstR.SumConNo);
                    self.TotalRecord(firstR.TotalRow);
                    self.PageCount(firstR.TotalPage);
                }
                else {
                    self.TotalRecord(0);
                    self.PageCount(0);
                }
                self.HoaDons(data);
                LoadHtmlGrid();
            }
            else {
                self.TotalRecord(0);
                self.PageCount(0);
            }
        });
        localStorage.removeItem('FindHD');
    }

    self.clickiconSearch = function () {
        ResetSearch();
        SearchHoaDon();
    }

    $('#txtMaHoaDonNH').keypress(function (e) {
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

    self.TT_DangXuLy.subscribe(function (newVal) {
        ResetSearch();
        SearchHoaDon();
    });

    self.TT_DaLuu.subscribe(function (newVal) {
        ResetSearch();
        SearchHoaDon();
    });

    self.LoaiHoaDon_4.subscribe(function (newVal) {
        ResetSearch();
        SearchHoaDon();
    });

    self.LoaiHoaDon_13.subscribe(function (newVal) {
        ResetSearch();
        SearchHoaDon();
    });

    self.LoaiHoaDon_14.subscribe(function (newVal) {
        ResetSearch();
        SearchHoaDon();
    });

    self.LoaiHoaDon_31.subscribe(function (newVal) {
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

    self.clickNhapHang = function () {
        GoToChiTietNhap();
    }

    function GetCTHD_andSaveCache(arrCT, itemHD, type) {
        isSaoChep = false;
        let ngaylapHD = new Date();
        if (type === 1) {
            isSaoChep = true;
            itemHD.ID = const_GuidEmpty;
        }
        if (type === 2 || type === 3) {// mophieutam
            ngaylapHD = itemHD.NgayLapHoaDon;
        }

        var cthdLoHang = [];
        var arrIDQuiDoi = [];
        for (let i = 0; i < arrCT.length; i++) {
            arrCT[i].ID_HoaDon = itemHD.ID;
            arrCT[i].MaHoaDon = type === 1 ? 'Copy' + itemHD.MaHoaDon : itemHD.MaHoaDon;
            arrCT[i].ID_DonVi = itemHD.ID_DonVi;
            arrCT[i].ID_DoiTuong = itemHD.ID_DoiTuong;
            arrCT[i].ID_NhanVien = itemHD.ID_NhanVien;
            arrCT[i].DienGiai = itemHD.DienGiai;
            arrCT[i].TongTienHang = itemHD.TongTienHang;
            arrCT[i].TongGiamGia = itemHD.TongGiamGia;
            arrCT[i].TongChietKhau = itemHD.TongChietKhau;
            arrCT[i].PhaiThanhToan = itemHD.PhaiThanhToan;
            arrCT[i].KhachDaTra = itemHD.KhachDaTra;
            arrCT[i].NgayLapHoaDon = ngaylapHD;

            arrCT[i].HangCungLoais = [];
            arrCT[i].LaConCungLoai = false;
            arrCT[i].GiaNhap = arrCT[i].DonGia;
            arrCT[i].TonKho = parseFloat((arrCT[i].TonKho / arrCT[i].TyLeChuyenDoi).toFixed(3));
            arrCT[i].ThanhTien = arrCT[i].SoLuong * arrCT[i].DonGia;

            let idLoHang = arrCT[i].ID_LoHang;
            let itemLot = [];
            if (idLoHang !== null) {
                itemLot = $.grep(self.ListLoHang(), function (x) {
                    return x.ID === idLoHang;
                });
            }
            arrCT[i].DM_LoHang = [];
            arrCT[i].ID_LoHang = idLoHang;
            arrCT[i].LotParent = arrCT[i].QuanLyTheoLoHang;
            arrCT[i].SoThuTu = cthdLoHang.length + 1;

            if ($.inArray(arrCT[i].ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                arrIDQuiDoi.unshift(arrCT[i].ID_DonViQuiDoi);

                // push CTHD
                arrCT[i].MaLoHang = arrCT[i].QuanLyTheoLoHang ? (itemLot.length > 0 ? itemLot[0].MaLoHang : null) : null; // assign MaLoHang for CTHD parent  --> save NhatKyThaotac

                arrCT[i].ID_Random = CreateIDRandom('IDRandom_');
                if (idLoHang !== null) {
                    // push DM_Lo
                    let objLot = newLot(arrCT[i], itemLot[0]);
                    arrCT[i].DM_LoHang.push(objLot);
                }
                cthdLoHang.push(arrCT[i]);
            }
            else {
                // find in cthdLoHang with same ID_QuiDoi
                for (let j = 0; j < cthdLoHang.length; j++) {
                    if (cthdLoHang[j].ID_DonViQuiDoi === arrCT[i].ID_DonViQuiDoi) {
                        if (idLoHang !== null) {
                            // push DM_Lo
                            let objLot = newLot(arrCT[i], itemLot[0]);
                            objLot.LotParent = false;
                            objLot.ID_Random = CreateIDRandom('IDRandom_');
                            cthdLoHang[j].DM_LoHang.push(objLot);
                        }
                        break;
                    }
                }
            }

            if (cthdLoHang.length > 0) {
                cthdLoHang[0].LoaiHoaDon = item.LoaiHoaDon;
            }
        }
        localStorage.setItem('THN_Chitiet', JSON.stringify(cthdLoHang));
    }

    function GoToChiTietNhap() {
        switch (self.LoaiHoaDonMenu()) {
            case 4:
                window.open('/#/PurchaseOrderItem2', '_blank');
                break;
            case 31:
                window.open('/#/DatHangNCCItem', '_blank');
                break;
            case 13:
                window.open('/#/NhapNoiBoItem', '_blank');
                break;
            case 14:
                window.open('/#/NhapHangThuaItem', '_blank');
                break;
        }
    }

    async function CheckHD_DaXuatKho(item, type) {
        if (type === 2) {
            let loaiHDCheck = item.LoaiHoaDon === 31 ? 4 : 7;
            let xx = $.getJSON('/api/DanhMuc/GaraAPI/' + 'CheckHoaDon_DaXuLy?idHoaDon=' + item.ID + '&loaiHoaDon=' + loaiHDCheck).done(function (x) {
            }).then(function (x) {
                if (x == true) {
                    return !x;
                }
                return true;
            });
            return xx;
        }
        else {
            return true;
        }
    }

    function GetPTThue_PTChietKhauHang(item) {
        var ptCKHang = 0;
        var arrCTsort = self.BH_HoaDonChiTietsThaoTac();
        var arr = $.grep(arrCTsort, function (x) {
            return x.PTChietKhau === arrCTsort[0].PTChietKhau;
        });
        if (arr.length === arrCTsort.length) {
            ptCKHang = arrCTsort[0].PTChietKhau;
        }

        var ptThue = item.PTThueHoaDon;
        return {
            PTChietKhauHH: ptCKHang,
            PTThueHD: ptThue,
        }
    }

    self.SaoChep_Edit = async function (item, type) {
        let check = await CheckHD_DaXuatKho(item, type);
        if (!check) {
            if (item.LoaiHoaDon === 31) {
                ShowMessage_Danger('Phiếu đặt hàng đã nhập mua. Không thể sửa đổi');
                return;
            }
            if (item.LoaiHoaDon === 4) {
                ShowMessage_Danger('Đã tồn tại phiếu trả hàng từ phiếu nhập. Không thể sửa đổi');
                return;
            }
        }

        var hd = $.extend({}, item);
        var arrCTsort = self.BH_HoaDonChiTietsThaoTac();
        var arrIDQuiDoi = [];
        var cthdLoHang = [];
        var khachdatra = 0;
        var phaitt = hd.TongThanhToan;
        var idHoaDon = const_GuidEmpty;
        let isSaochep = false;
        // 0. saochep, 1.updateHDTam, 2.updateHD truoc do
        if (type !== 0) {
            khachdatra = hd.KhachDaTra;
            idHoaDon = hd.ID;
        }
        else {
            khachdatra = 0;
            isSaochep = true;
        }

        var obj = GetPTThue_PTChietKhauHang(item);

        for (let i = 0; i < arrCTsort.length; i++) {
            var ctNew = $.extend({}, arrCTsort[i]);
            ctNew.ID_HoaDon = idHoaDon;
            ctNew.ID_HoaDonGoc = type !== 0 ? item.ID_HoaDon : null;// update again nhaphang (keep ID_HoaDon)
            ctNew.TenDoiTuong = item.TenDoiTuong;
            ctNew.TongTienHangChuaCK = self.TongTienHangChuaCK();
            ctNew.TongGiamGiaHang = self.TongGiamGiaHang();
            ctNew.PTChietKhauHH = obj.PTChietKhauHH;
            ctNew.PTThueHD = obj.PTThueHD;
            ctNew.TongTienHang = item.TongTienHang;
            ctNew.TongGiamGia = item.TongGiamGia;
            ctNew.TongTienThue = item.TongTienThue;
            ctNew.TongChietKhau = item.TongChietKhau;
            ctNew.PhaiThanhToan = phaitt;
            ctNew.TongThanhToan = phaitt;
            ctNew.KhachDaTra = khachdatra;
            ctNew.TongChiPhi = item.TongChiPhi;// chiphi tra NCC
            ctNew.DaThanhToan = 0;
            ctNew.ID_NhanVien = item.ID_NhanVien;
            ctNew.NgayLapHoaDon = type !== 0 ? item.NgayLapHoaDon : null;
            ctNew.MaHoaDon = isSaochep ? "Copy" + item.MaHoaDon : item.MaHoaDon;
            ctNew.DienGiai = item.DienGiai;
            ctNew.YeuCau = item.YeuCau;
            if (hd.ID_DoiTuong !== '00000000-0000-0000-0000-000000000000') {
                ctNew.ID_DoiTuong = hd.ID_DoiTuong;
            }

            let idLoHang = ctNew.ID_LoHang;
            let quanLiTheoLo = ctNew.QuanLyTheoLoHang;
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

            ctNew.DM_LoHang = [];
            ctNew.ID_LoHang = idLoHang;
            ctNew.LotParent = quanLiTheoLo;
            ctNew.SoThuTu = cthdLoHang.length + 1;
            ctNew.HangCungLoais = [];
            ctNew.LaConCungLoai = false;
            ctNew.DVTinhGiam = '%';
            if (ctNew.PTChietKhau === 0 && ctNew.TienChietKhau !== 0) {
                ctNew.DVTinhGiam = 'VND';
            }

            if ($.inArray(ctNew.ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                arrIDQuiDoi.unshift(ctNew.ID_DonViQuiDoi);
                ctNew.IDRandom = CreateIDRandom('CTHD_');
                if (quanLiTheoLo) {
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
                        if (quanLiTheoLo) {
                            // push DM_Lo
                            let objLot = $.extend({}, ctNew);
                            objLot.LotParent = false;
                            objLot.HangCungLoais = [];
                            objLot.DM_LoHang = [];
                            objLot.IDRandom = CreateIDRandom('RandomCT_');
                            cthdLoHang[j].DM_LoHang.push(objLot);
                        }
                        else {
                            ctNew.IDRandom = CreateIDRandom('RandomCT_');
                            ctNew.LaConCungLoai = true;
                            cthdLoHang[j].HangCungLoais.push(ctNew);
                        }
                        break;
                    }
                }
            }
        }

        if (cthdLoHang.length > 0) {
            cthdLoHang[0].LoaiHoaDon = item.LoaiHoaDon;
            cthdLoHang[0].ID_DonVi = isSaochep ? VHeader.IdDonvi : item.ID_DonVi;// keep idDonVi if update hoadon
        }
        localStorage.setItem('lc_CTSaoChep', JSON.stringify(cthdLoHang));
        localStorage.setItem('isSaoChep', isSaochep);
        localStorage.setItem('isEditNH', !isSaochep);
        GoToChiTietNhap();
    }

    self.XuatChuyenHang = function (item) {
        var arrCTsort = self.BH_HoaDonChiTietsThaoTac();
        var arrIDQuiDoi = [];
        var cthdLoHang = [];
        var sumCT = 0;
        for (let i = 0; i < arrCTsort.length; i++) {
            let ctNew = $.extend({}, arrCTsort[i]);
            delete ctNew["ID"];
            delete ctNew["ID_HoaDon"];
            ctNew.MaHoaDon = '';
            ctNew.ID_CheckIn = item.ID_CheckIn;
            ctNew.ID_DonVi = item.ID_DonVi;
            ctNew.ID_NhanVien = item.ID_NhanVien;
            ctNew.DienGiai = item.DienGiai;
            ctNew.NgayLapHoaDon = new Date();

            ctNew.PTThue = 0;
            ctNew.DM_LoHang = [];
            ctNew.TienThue = 0;
            ctNew.ThanhToan = 0;
            ctNew.TonKho = parseFloat((ctNew.TonKho / ctNew.TyLeChuyenDoi).toFixed(3));
            ctNew.SoLuongChuyen = ctNew.SoLuong;
            ctNew.ThanhTien = ctNew.SoLuong * ctNew.GiaVon;
            sumCT += ctNew.ThanhTien;

            let quanLiTheoLo = ctNew.QuanLyTheoLoHang;
            ctNew.LotParent = ctNew.QuanLyTheoLoHang;
            ctNew.SoThuTu = cthdLoHang.length + 1;

            if ($.inArray(ctNew.ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                arrIDQuiDoi.unshift(ctNew.ID_DonViQuiDoi);

                ctNew.ID_Random = CreateIDRandom('IDRandom_');
                if (quanLiTheoLo) {
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
                        if (quanLiTheoLo) {
                            let objLot = $.extend({}, ctNew);
                            objLot.ID_Random = CreateIDRandom('IDRandom_');
                            objLot.LotParent = false;
                            objLot.DM_LoHang = [];
                            objLot.HangCungLoais = [];
                            cthdLoHang[j].DM_LoHang.push(objLot);
                        }
                        else {
                            // find in ctlohang (same quidoi)
                            for (let k = 0; k < cthdLoHang.length; k++) {
                                if (cthdLoHang[k].ID_DonViQuiDoi === ctNew.ID_DonViQuiDoi) {
                                    cthdLoHang[k].SoLuongChuyen += ctNew.SoLuong;
                                    cthdLoHang[k].ThanhTien = cthdLoHang[k].SoLuongChuyen * cthdLoHang[k].DonGia;
                                    cthdLoHang[k].ThanhToan = 0;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }
        cthdLoHang[0].TongTienHang = sumCT;

        localStorage.setItem('lcCH_EditOpen', JSON.stringify(cthdLoHang));
        localStorage.setItem('createfrom', 4);// xuat chuyenhang
        window.open('/#/TransfersCT2', '_blank');
    };

    self.ChiTietTraHang = function (item) {
        var arrCTsort = self.BH_HoaDonChiTietsThaoTac();
        var arrIDQuiDoi = [];
        var cthdLoHang = [];

        for (let i = 0; i < arrCTsort.length; i++) {
            let ctNew = $.extend({}, arrCTsort[i]);
            // get variable of hoadon
            ctNew.ID_HoaDon = item.ID;// save id phieu nhaphang
            ctNew.ID_DoiTuong = item.ID_DoiTuong;
            ctNew.ID_NhanVien = item.ID_NhanVien;
            ctNew.TenDoiTuong = item.TenDoiTuong;
            ctNew.TongTienHang = item.TongTienHang;
            ctNew.TongGiamGia = item.TongGiamGia;
            ctNew.TongChietKhau = item.TongChietKhau;
            ctNew.TongTienThue = item.TongTienThue;
            ctNew.PTThueHD = item.PTThueHoaDon;
            ctNew.PhaiThanhToan = item.PhaiThanhToan;
            ctNew.TongThanhToan = item.TongThanhToan;
            ctNew.NgayLapHoaDon = new Date();
            if (item.ID_DoiTuong !== '00000000-0000-0000-0000-000000000000') {
                ctNew.ID_DoiTuong = item.ID_DoiTuong;
            }

            ctNew.ID = ctNew.ID_HangHoa;
            ctNew.TienChietKhau = 0;
            ctNew.PTChietKhau = 0;
            ctNew.DonGia = ctNew.DonGia - ctNew.GiamGia;
            ctNew.ThanhToan = ctNew.SoLuong * (ctNew.DonGia + ctNew.TienThue);
            ctNew.GiaNhap = ctNew.DonGia;
            ctNew.DVTinhGiam = '%';

            let quanLiTheoLo = ctNew.QuanLyTheoLoHang;
            ctNew.DM_LoHang = [];
            ctNew.HangCungLoais = [];
            ctNew.LotParent = quanLiTheoLo;
            ctNew.LaConCungLoai = false;
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

                if (quanLiTheoLo) {
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
                        if (quanLiTheoLo) {
                            let objLot = $.extend({}, ctNew);
                            objLot.ID_Random = CreateIDRandom('IDRandom_');
                            objLot.LotParent = false;
                            objLot.DM_LoHang = [];
                            objLot.HangCungLoais = [];
                            cthdLoHang[j].DM_LoHang.push(objLot);
                        }
                        else {
                            // find in ctlohang (same quidoi)
                            for (let k = 0; k < cthdLoHang.length; k++) {
                                if (cthdLoHang[k].ID_DonViQuiDoi === ctNew.ID_DonViQuiDoi) {
                                    cthdLoHang[k].SoLuong += ctNew.SoLuong;
                                    cthdLoHang[k].ThanhTien = cthdLoHang[k].SoLuong * cthdLoHang[k].DonGia;
                                    cthdLoHang[k].ThanhToan = cthdLoHang[k].SoLuong * (cthdLoHang[k].DonGia + cthdLoHang[k].TienThue);
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }
        localStorage.setItem('THN_Chitiet', JSON.stringify(cthdLoHang));
        localStorage.setItem('THN_Thaotac', 3);
        window.open('/#/PurchaseReturnsCT2', '_blank');
    }

    self.XuatBanHang = function (item) {
        var hd = $.extend({}, item);
        if (self.BH_HoaDonChiTietsThaoTac() !== null) {
            localStorage.setItem('createHDfrom', 5);
            hd.MaHoaDonDB = null;
            hd.ID_HoaDon = null;
            hd.IsSaoChep = false;
            hd.IsHDDatHang = false; // assign IsHDDatHang = false --> deleteNCC at BanLe_TraHang
            hd.Status = 1;
            hd.LoaiHoaDon = 1;
            hd.StatusOffline = false;
            hd.DVTinhGiam = '%';
            hd.TongChietKhau = 0;
            hd.TongGiamGia = 0;
            hd.DiemGiaoDichDB = 0;
            hd.PhaiThanhToanDB = 0;
            hd.TongGiaGocHangTra = 0;
            hd.TongChiPhi = 0;
            hd.TongTienTra = 0;
            hd.PTGiamDB = 0;
            hd.TongGiamGiaDB = 0;
            hd.TienGui = 0;
            hd.TienATM = 0;
            hd.HoanTraTamUng = 0;
            hd.KhachDaTra = 0;
            hd.IsKhuyenMaiHD = false;
            hd.IsOpeningKMaiHD = false;
            hd.KhuyeMai_GiamGia = 0;
            hd.TongGiamGiaKM_HD = 0;
            hd.ID_DoiTuong = null;
            hd.ID_BangGia = '00000000-0000-0000-0000-000000000000';
            hd.NguoiTao = userLogin.trim();
            hd.TongThueDB = 0;
            hd.PTThueHoaDon = 0;
            hd.TongTienThue = 0;
            hd.TongChiPhiHangTra = 0;

            // TichDiem
            hd.TTBangDiem = 0;
            hd.DiemGiaoDich = 0;
            hd.DiemQuyDoi = 0;
            hd.DiemHienTai = 0;
            // use when KM_Cong diem
            hd.DiemCong = 0;
            hd.DiemKhuyenMai = 0;

            // apply giam gia theo nhom
            hd.ID_NhomDTApplySale = null;

            // Goi dich vu
            hd.NgayApDungGoiDV = null;
            hd.HanSuDungGoiDV = null;

            hd.CreateTime = 0; // bắt đầu chọn bàn (phòng) lúc HH:mm
            hd.ID_ViTri = null;
            hd.TenViTriHD = '';

            hd.BH_NhanVienThucHiens = [];
            hd.TienTheGiaTri = 0;
            hd.ThoiGianThucHien = 0;
            hd.TrangThaiHD = 1;
            hd.IsActive = '';

            hd.PTChietKhauHH = 0;
            hd.TongGiamGiaHang = '';
            hd.TongTienKhuyenMai_CT = '';
            hd.TongGiamGiaKhuyenMai_CT = '';
            hd.DuyetBaoGia = false;
            hd.XuatKhoAll = false;
            hd.TenBaoHiem = '';
            hd.PhaiThanhToanBaoHiem = 0;
            hd.MaPhieuTiepNhan = '';

            hd.SoVuBaoHiem = '';
            hd.KhauTruTheoVu = 0;
            hd.GiamTruBoiThuong = 0;
            hd.PTGiamTruBoiThuong = 0;
            hd.TongTienThueBaoHiem = 0;
            hd.PTThueBaoHiem = 0;
            hd.BHThanhToanTruocThue = 0;
            hd.TongTienBHDuyet = 0;
            hd.GiamTruThanhToanBaoHiem = 0;
            hd.CongThucBaoHiem = 0;
            hd.TongThueKhachHang = 0;
            hd.HeaderBH_GiaTriPtram = 0;
            hd.HeaderBH_Type = 1;

            // order by SoThuTu ASC --> group Hang Hoa by LoHang
            var arrCTsort = self.BH_HoaDonChiTietsThaoTac().sort(function (a, b) {
                var x = a.SoThuTu,
                    y = b.SoThuTu;
                return x < y ? -1 : x > y ? 1 : 0;
            });

            var tongTienHang = 0;
            var arrIDQuiDoi = [];
            var cthdLoHang = [];

            for (let i = 0; i < arrCTsort.length; i++) {
                var ctNew = $.extend({}, arrCTsort[i]);
                delete ctNew["ID"];

                ctNew.MaHoaDon = "";
                ctNew.LoaiHoaDon = 1;
                ctNew.SoLuongDaMua = 0;
                ctNew.TienChietKhau = 0;
                ctNew.DVTinhGiam = '%';
                ctNew.PTChietKhau = 0;
                ctNew.GiaBan = ctNew.GiaBanMaVach;
                ctNew.ThanhTien = ctNew.GiaBan * ctNew.SoLuong;
                ctNew.ThanhToan = ctNew.GiaBan * ctNew.SoLuong;
                ctNew.SrcImage = null;
                ctNew.CssWarning = false;
                ctNew.IsKhuyenMai = false;
                ctNew.IsOpeningKMai = false;
                ctNew.TenKhuyenMai = '';
                ctNew.HangHoa_KM = [];
                ctNew.RoleChangePrice = true;

                ctNew.ID_ChiTietDinhLuong = null;
                ctNew.ID_ChiTietGoiDV = null;
                ctNew.UsingService = false;
                ctNew.ListDonViTinh = [];
                ctNew.ShowEditQuyCach = false;
                ctNew.ShowWarningQuyCach = false;
                ctNew.SoLuongQuyCach = 0;
                ctNew.ThanhPhan_DinhLuong = [];
                ctNew.ThanhPhanComBo = [];
                ctNew.PTThue = 0;
                ctNew.TienThue = 0;
                ctNew.HangCungLoais = [];
                ctNew.LaConCungLoai = false;
                ctNew.DiemKhuyenMai = 0;

                ctNew.BH_NhanVienThucHien = [];
                ctNew.GhiChu_NVThucHien = '';
                ctNew.GhiChu_NVTuVan = '';
                ctNew.GhiChu_NVThucHienPrint = '';
                ctNew.GhiChu_NVTuVanPrint = '';
                ctNew.ID_LichBaoDuong = null;

                ctNew.ID_ViTri = null;
                ctNew.TenViTri = '';
                ctNew.TimeStart = 0;
                ctNew.QuaThoiGian = 0;
                ctNew.TimeRemain = 0;
                ctNew.ThoiGianThucHien = 0;
                ctNew.ThoiGian = moment(new Date()).format('YYYY-MM-DD HH:mm');

                tongTienHang += ctNew.ThanhTien;

                let quanLiTheoLo = ctNew.QuanLyTheoLoHang;
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

                ctNew.DM_LoHang = [];
                ctNew.LotParent = quanLiTheoLo;
                ctNew.SoThuTu = cthdLoHang.length + 1;

                if (ctNew.PTChietKhau === 0 && ctNew.TienChietKhau !== 0) {
                    ctNew.DVTinhGiam = 'VND';
                }

                if ($.inArray(ctNew.ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                    arrIDQuiDoi.unshift(ctNew.ID_DonViQuiDoi);
                    ctNew.IDRandom = CreateIDRandom('CTHD_');
                    if (quanLiTheoLo) {
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
                            if (quanLiTheoLo) {
                                // push DM_Lo
                                let objLot = $.extend({}, ctNew);
                                objLot.LotParent = false;
                                objLot.HangCungLoais = [];
                                objLot.DM_LoHang = [];
                                objLot.IDRandom = CreateIDRandom('RandomCT_');
                                cthdLoHang[j].DM_LoHang.push(objLot);
                            }
                            else {
                                ctNew.IDRandom = CreateIDRandom('RandomCT_');
                                ctNew.LaConCungLoai = true;
                                cthdLoHang[j].HangCungLoais.push(ctNew);
                            }
                            break;
                        }
                    }
                }
            }

            // caculator TongTienHang from HDNhap (because get GiaBan from DonViQuiDoi)
            hd.TongTienHang = tongTienHang;
            hd.TongThanhToan = tongTienHang;
            hd.PhaiThanhToan = tongTienHang;
            hd.DaThanhToan = tongTienHang;
            hd.TienMat = tongTienHang;
            hd.TongTienHangChuaCK = tongTienHang;

            localStorage.setItem('lcHDForNH', JSON.stringify(hd));
            localStorage.setItem('lcCTHDForNH', JSON.stringify(cthdLoHang));
            if (self.shopCookies() === 'C16EDDA0-F6D0-43E1-A469-844FAB143014') {
                localStorage.setItem('gara_CreateFrom', 'NH_XuatBan');
                let newwindow = window.open('/g/Gara', '_blank');
                let popupTick = setInterval(function () {
                    if (newwindow.closed) {
                        clearInterval(popupTick);
                        SearchHoaDon();
                    }
                }, 500);
            }
            else {
                window.open('/$/BanLe', '_blank');
            }
        }
        else {
            ShowMessage_Danger('Không có chi tiết hóa đơn');
            return false;
        }
    };

    self.NhapMua_fromPO = function (item) {

        let hd = $.extend({}, item);
        let obj = GetPTThue_PTChietKhauHang(item);

        let dathanhtoan = item.KhachDaTra;
        if (item.YeuCau === '2') {// daxuly it nhat 1 lan
            dathanhtoan = 0;
        }
        let ptGiamHD = item.TongChietKhau;
        if (ptGiamHD === 0) {
            if (item.TongTienHang > 0) {
                ptGiamHD = item.TongGiamGia / (item.TongTienHang + item.TongTienThue) * 100;
            }
        }

        var arrIDQuiDoi = [];
        var cthdLoHang = [];

        ajaxHelper(BH_HoaDonUri + 'GetCTHoaDon_afterDatHang?idHoaDon=' + item.ID, 'GET').done(function (data) {
            if (data !== null) {
                let ctConLai = $.grep(data, function (x) {
                    return x.SoLuongConLai > 0;
                });
                console.log('ctConLai ', ctConLai)

                if (ctConLai.length === 0) {
                    ShowMessage_Danger('Đã nhập mua đủ đơn hàng');
                    return;
                }

                let tongtienhang = 0, tongthueHD = 0, thanhtienchuaCK = 0, tongCKHang = 0, tonggiamHD = 0,
                    phaithanhtoan = 0, tongthanhtoan = 0;

                for (let i = 0; i < ctConLai.length; i++) {
                    let ctNew = $.extend({}, ctConLai[i]);

                    ctNew.ID_ChiTietGoiDV = ctNew.ID;
                    ctNew.SoLuong = ctNew.SoLuongConLai;

                    let ck1SP = ctNew.TienChietKhau, thue1SP = ctNew.TienThue;
                    if (ctNew.PTChietKhau > 0) {
                        ck1SP = ctNew.PTChietKhau * ctNew.DonGia / 100;
                    }
                    if (ctNew.PTThue > 0) {
                        thue1SP = ctNew.PTThue * (ctNew.DonGia - ck1SP) / 100;
                    }
                    ctNew.ThanhTien = ctNew.SoLuong * (ctNew.DonGia - ck1SP);
                    ctNew.ThanhToan = ctNew.SoLuong * (ctNew.DonGia - ck1SP + thue1SP);

                    thanhtienchuaCK += ctNew.SoLuong * ctNew.DonGia;
                    tongtienhang += ctNew.ThanhTien;
                    tongthueHD += ctNew.SoLuong * thue1SP;
                    tongCKHang += ctNew.SoLuong * ck1SP;

                    let idLoHang = ctNew.ID_LoHang;
                    let quanLiTheoLo = ctNew.QuanLyTheoLoHang;
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

                    ctNew.DM_LoHang = [];
                    ctNew.ID_LoHang = idLoHang;
                    ctNew.LotParent = quanLiTheoLo;
                    ctNew.SoThuTu = cthdLoHang.length + 1;
                    ctNew.HangCungLoais = [];
                    ctNew.LaConCungLoai = false;
                    ctNew.DVTinhGiam = '%';
                    if (ctNew.PTChietKhau === 0 && ctNew.TienChietKhau !== 0) {
                        ctNew.DVTinhGiam = 'VND';
                    }

                    if ($.inArray(ctNew.ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                        arrIDQuiDoi.unshift(ctNew.ID_DonViQuiDoi);
                        ctNew.IDRandom = CreateIDRandom('CTHD_');
                        if (quanLiTheoLo) {
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
                                if (quanLiTheoLo) {
                                    let objLot = $.extend({}, ctNew);
                                    objLot.LotParent = false;
                                    objLot.HangCungLoais = [];
                                    objLot.DM_LoHang = [];
                                    objLot.IDRandom = CreateIDRandom('RandomCT_');
                                    cthdLoHang[j].DM_LoHang.push(objLot);
                                }
                                else {
                                    ctNew.IDRandom = CreateIDRandom('RandomCT_');
                                    cthdLoHang.push(ctNew);
                                }
                                break;
                            }
                        }
                    }
                }

                if (cthdLoHang.length > 0) {
                    if (hd.ID_DoiTuong !== '00000000-0000-0000-0000-000000000000') {
                        cthdLoHang[0].ID_DoiTuong = hd.ID_DoiTuong;
                    }
                    tonggiamHD = ptGiamHD * (tongtienhang + tongthueHD) / 100;
                    cthdLoHang[0].ID = const_GuidEmpty;
                    cthdLoHang[0].ID_HoaDon = item.ID;
                    cthdLoHang[0].ID_DonVi = item.ID_DonVi;
                    cthdLoHang[0].LoaiHoaDon = 4;
                    cthdLoHang[0].NgayLapHoaDon = null;
                    cthdLoHang[0].TenDoiTuong = hd.TenDoiTuong;
                    cthdLoHang[0].TongTienHangChuaCK = thanhtienchuaCK;
                    cthdLoHang[0].TongGiamGiaHang = tongCKHang;
                    cthdLoHang[0].PTChietKhauHH = obj.PTChietKhauHH;
                    cthdLoHang[0].PTThueHD = obj.PTThueHD;
                    cthdLoHang[0].TongTienHang = tongtienhang;
                    cthdLoHang[0].TongGiamGia = tonggiamHD;
                    cthdLoHang[0].TongTienThue = tongthueHD;
                    cthdLoHang[0].TongChietKhau = item.TongChietKhau;
                    cthdLoHang[0].TongChiPhi = item.TongChiPhi;
                    cthdLoHang[0].PhaiThanhToan = tongtienhang + tongthueHD - tonggiamHD + item.TongChiPhi;
                    cthdLoHang[0].TongThanhToan = cthdLoHang[0].PhaiThanhToan;
                    cthdLoHang[0].KhachDaTra = dathanhtoan;
                    cthdLoHang[0].DaThanhToan = 0;
                    cthdLoHang[0].ID_NhanVien = _id_NhanVien;
                    cthdLoHang[0].MaHoaDon = '';
                    cthdLoHang[0].DienGiai = item.DienGiai;
                }
                localStorage.setItem('lc_CTSaoChep', JSON.stringify(cthdLoHang));
                localStorage.setItem('typeCacheNhapHang', 4);
                window.open('/#/PurchaseOrderItem2', '_blank');
            }
        })
    }

    self.DownloadFileExportXLSX = function (url) {
        var url1 = DMHangHoaUri + "Download_fileExcel?fileSave=" + url;
        window.location.href = url1;
    }

    self.ExportExcel_PhieuNhapHang = function () {
        var param = GetParamSearch();
        param.currentPage = 0;
        param.pageSize = self.TotalRecord();
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

        let url = 'ExportExcel_PhieuNhapHang';
        if (self.LoaiHoaDon_13() || self.LoaiHoaDon_14()) {
            url = 'ExportExcel_PhieuNhapKhoNoiBo';
        }

        ajaxHelper(BH_HoaDonUri + url, 'post', { objExcel: param }).done(function (url) {
            self.DownloadFileExportXLSX(url);
            var objDiary = {
                ID_NhanVien: _id_NhanVien,
                ID_DonVi: _IDchinhanh,
                ChucNang: "Nhập hàng",
                NoiDung: "Xuất excel danh sách phiếu nhập hàng",
                NoiDungChiTiet: "Xuất excel danh sách phiếu nhập hàng".concat('<br />- Người xuất: ', VHeader.UserLogin),
                LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
            };
            Insert_NhatKyThaoTac_1Param(objDiary);
        });
    };

    self.ExportExcel_ChiTietPhieuNhapHang = function (item) {

        var url = BH_HoaDonUri + 'ExportExcel__ChiTietPhieuNhapHang?ID_HoaDon=' + item.ID;
        window.location.href = url;

        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Nhập hàng",
            NoiDung: "Xuất excel chi tiết phiếu nhập hàng theo mã: " + item.MaHoaDon,
            NoiDungChiTiet: "Xuất excel chi tiết phiếu nhập hàng theo mã: <a onclick=\"FindMaHD('" + item.MaHoaDon + "')\"> " + item.MaHoaDon + "</a>",
            LoaiNhatKy: 6
        };
        Insert_NhatKyThaoTac_1Param(objDiary);
    };

    function loadMauIn() {
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetListMauIn?typeChungTu=PNK' + '&idDonVi=' + _IDchinhanh,
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
        var soluong = 0;
        for (let i = 0; i < self.BH_HoaDonChiTietsThaoTac().length; i++) {
            let itFor = self.BH_HoaDonChiTietsThaoTac()[i];
            soluong += formatNumberToFloat(itFor.SoLuong);
        }
        itemHDFormat.TongSoLuongHang = formatNumber3Digit(soluong);
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
        var soluong = 0;
        for (let i = 0; i < self.BH_HoaDonChiTietsThaoTac().length; i++) {
            let itFor = self.BH_HoaDonChiTietsThaoTac()[i];
            soluong += formatNumberToFloat(itFor.SoLuong);
        }
        itemHDFormat.TongSoLuongHang = formatNumber3Digit(soluong);
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

    function GetInforChiNhanh(idDonVi) {
        let diachi = '', dienthoai = '', ten = '';
        var chinhanh = $.grep(self.ChiNhanhs(), function (x) {
            return x.ID === idDonVi;
        });
        if (chinhanh.length > 0) {
            ten = chinhanh[0].TenDonVi;
            diachi = chinhanh[0].DiaChi;
            dienthoai = chinhanh[0].SoDienThoai;
        }
        return {
            TenChiNhanh: ten,
            DiaChiChiNhanh: diachi,
            DienThoaiChiNhanh: dienthoai,
        }
    }

    function GetInforHDPrint(objHD) {
        var hd = $.extend({}, objHD);
        var datehoadon = moment(objHD.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss');
        hd.NgayLapHoaDon = datehoadon;
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
        hd.TongTienThue = formatNumber3Digit(objHD.TongTienThue, 2);
        hd.TongThanhToan = formatNumber3Digit(objHD.TongThanhToan, 2);
        hd.TongGiamGia = formatNumber3Digit(objHD.TongGiamGia, 2);
        hd.PhaiThanhToan = formatNumber3Digit(phaiTT, 2);
        hd.DaThanhToan = formatNumber3Digit(daTT, 2);
        hd.TongCong = formatNumber3Digit(phaiTT, 2);
        hd.NoSau = formatNumber3Digit(phaiTT - daTT, 2);
        hd.TienBangChu = DocSo(phaiTT);
        hd.GhiChu = hd.DienGiai;
        hd.NoTruoc = 0;
        hd.PhiTraHang = 0;
        hd.TongTienHangChuaCK = formatNumber3Digit(self.TongTienHangChuaCK(), 2);
        hd.TongGiamGiaHang = formatNumber3Digit(self.TongGiamGiaHang(), 2);
        hd.TienMat = formatNumber3Digit(hd.TienMat, 2);
        hd.TienKhachThieu = formatNumber3Digit(hd.ConNo, 2);

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

        var obj = GetInforChiNhanh(hd.ID_DonVi);
        hd.DiaChiChiNhanh = obj.DiaChiChiNhanh;
        hd.DienThoaiChiNhanh = obj.DienThoaiChiNhanh;
        hd.TenChiNhanh = obj.TenChiNhanh;

        if (self.CongTy().length > 0) {
            hd.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
            hd.TenCuaHang = self.CongTy()[0].TenCongTy;
            hd.DiaChiCuaHang = self.CongTy()[0].DiaChi;
            hd.DienThoaiCuaHang = self.CongTy()[0].SoDienThoai;
        }
        return hd;
    }

    function GetCTHDPrint_Format(arrCTHD) {
        var arr = [];
        for (let i = 0; i < arrCTHD.length; i++) {
            let ct = $.extend({}, arrCTHD[i]);
            ct.SoThuTu = i + 1;
            ct.DonGia = formatNumber3Digit(ct.DonGia, 2);
            ct.SoLuong = formatNumber3Digit(ct.SoLuong, 2);
            ct.TienThue = formatNumber3Digit(ct.TienThue * ct.SoLuong);
            ct.HH_ThueTong = formatNumber3Digit(ct.TienThue);
            ct.ThanhTien = formatNumber3Digit(ct.ThanhTien, 2);
            ct.ThanhToan = formatNumber3Digit(ct.ThanhToan, 2);
            ct.TienChietKhau = formatNumber3Digit(ct.TienChietKhau, 2);
            ct.TongChietKhau = formatNumber3Digit(ct.TienChietKhau * ct.SoLuong, 2);
            arr.push(ct);
        }
        return arr;
    }

    self.CheckInTenHang1 = ko.observable();
    self.CheckInTenCuaHang1 = ko.observable();
    self.selectedLoaIn = ko.observable();
    self.selectedMauInMaVach = ko.observable();
    self.ListMauInMaVach = ko.observableArray();

    self.BarCodeChooseHH = function (item) {
        $('#myModalprintChooseHH').modal('show');
        self.CheckInBangGia1(true);
        self.CheckInMaHang1(true);
        $('#myModalprintChooseHH').on('shown.bs.modal', function () {
            $('.khongingiacheck1 input').prop('checked', false);
            $('.khonginmahangcheck1 input').prop('checked', false);
        })
        self.selectedGiaBan(undefined);

        $.getJSON('/api/DanhMuc/ThietLapApi/GetListMauIn?typeChungTu=IMV' + '&idDonVi=' + _IDchinhanh, function (result) {
            self.ListMauInMaVach(result);
        });
    }

    self.PrintBarcodeChoose1 = function () {
        inmavach(3);
    }
    self.PrintBarcodeChoose2 = function () {
        inmavach2tem(2);
    }
    self.PrintBarcodeChoose3 = function () {
        inmavach(6);
    }
    self.PrintBarcodeChoose4 = function () {
        inmavach1(5);
    }

    self.InTheoMauInThaoTac = function () {
        var id_mauIn = self.selectedMauInMaVach();
        var sobanghi = self.selectedLoaIn();
        InListMaVach(self.BH_HoaDonChiTietsThaoTac(), self.selectedGiaBan(), id_mauIn, sobanghi);
    };

    function inmavach(sobanghi) {
        if ($('.khongingiacheck1 input').is(':checked')) {
            self.CheckInBangGia1(false);
        }
        else {
            self.CheckInBangGia1(true);
        }
        if ($('.khonginmahangcheck1 input').is(':checked')) {
            self.CheckInMaHang1(false);
        }
        else {
            self.CheckInMaHang1(true);
        }
        if ($('.khongintenhangcheck1 input').is(':checked')) {
            self.CheckInTenHang1(false);
        }
        else {
            self.CheckInTenHang1(true);
        }
        if ($('.khongintencuahang1 input').is(':checked')) {
            self.CheckInTenCuaHang1(false);
        }
        else {
            self.CheckInTenCuaHang1(true);
        }
        var model = {
            listHH: self.BH_HoaDonChiTietsThaoTac(),
            InGia: self.CheckInBangGia1(),
            InMaHH: self.CheckInMaHang1(),
            InTenHH: self.CheckInTenHang1(),
            InTenCH: self.CheckInTenCuaHang1(),
            ID_BangGia: self.selectedGiaBan(),
            SoBanGhi: sobanghi
        };
        ajaxHelper(DMHangHoaUri + 'PrintBarcodeThaoTacNhapHang', 'POST', model).done(function (data) {
            printJS({ printable: data, type: 'pdf', showModal: false });
        });
    }

    function inmavach2tem(sobanghi) {
        if ($('.khongingiacheck1 input').is(':checked')) {
            self.CheckInBangGia1(false);
        }
        else {
            self.CheckInBangGia1(true);
        }
        if ($('.khonginmahangcheck1 input').is(':checked')) {
            self.CheckInMaHang1(false);
        }
        else {
            self.CheckInMaHang1(true);
        }
        if ($('.khongintenhangcheck1 input').is(':checked')) {
            self.CheckInTenHang1(false);
        }
        else {
            self.CheckInTenHang1(true);
        }
        if ($('.khongintencuahang1 input').is(':checked')) {
            self.CheckInTenCuaHang1(false);
        }
        else {
            self.CheckInTenCuaHang1(true);
        }
        var model = {
            listHH: self.BH_HoaDonChiTietsThaoTac(),
            InGia: self.CheckInBangGia1(),
            InMaHH: self.CheckInMaHang1(),
            InTenHH: self.CheckInTenHang1(),
            InTenCH: self.CheckInTenCuaHang1(),
            ID_BangGia: self.selectedGiaBan(),
            SoBanGhi: sobanghi
        };
        ajaxHelper(DMHangHoaUri + 'PrintBarcodeThaoTacNhapHang2Tem', 'POST', model).done(function (data) {
            printJS({ printable: data, type: 'pdf', showModal: false });
        });
    }

    function inmavach1(sobanghi) {
        if ($('.khongingiacheck1 input').is(':checked')) {
            self.CheckInBangGia1(false);
        }
        else {
            self.CheckInBangGia1(true);
        }
        if ($('.khonginmahangcheck1 input').is(':checked')) {
            self.CheckInMaHang1(false);
        }
        else {
            self.CheckInMaHang1(true);
        }
        if ($('.khongintenhangcheck1 input').is(':checked')) {
            self.CheckInTenHang1(false);
        }
        else {
            self.CheckInTenHang1(true);
        }
        if ($('.khongintencuahang1 input').is(':checked')) {
            self.CheckInTenCuaHang1(false);
        }
        else {
            self.CheckInTenCuaHang1(true);
        }
        var model = {
            listHH: self.BH_HoaDonChiTietsThaoTac(),
            InGia: self.CheckInBangGia1(),
            InMaHH: self.CheckInMaHang1(),
            InTenHH: self.CheckInTenHang1(),
            InTenCH: self.CheckInTenCuaHang1(),
            ID_BangGia: self.selectedGiaBan(),
            SoBanGhi: sobanghi
        };
        console.log('model ', model)
        ajaxHelper(DMHangHoaUri + 'PrintBarcodeThaoTacNhapHang1', 'POST', model).done(function (data) {
            printJS({ printable: data, type: 'pdf', showModal: false });
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

    function GetDM_TaiKhoanNganHang() {
        if (navigator.onLine) {
            ajaxHelper(Quy_HoaDonUri + 'GetAllTaiKhoanNganHang_ByDonVi?idDonVi=' + _IDchinhanh, 'GET').done(function (x) {
                if (x.res === true) {
                    vmThanhToanNCC.listData.AccountBanks = x.data;
                }
            })
        }
    }

    self.showPopThanhToan = function (item) {
        vmThanhToanNCC.showModalThanhToan(item);
    }

    self.formatDateTime = function () {
        $('.datepicker_mask').datetimepicker(
            {
                format: "d/m/Y H:i",
                mask: true,
                timepicker: false,
            });
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