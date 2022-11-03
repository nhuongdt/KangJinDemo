var ViewModelHD = function () {
    var self = this;

    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var DMDoiTuongUri = "/api/DanhMuc/DM_DoiTuongAPI/";
    var Quy_HoaDonUri = '/api/DanhMuc/Quy_HoaDonAPI/';
    var CSKHUri = '/api/DanhMuc/ChamSocKhachHangAPI/';
    var id_donvi = $('#hd_IDdDonVi').val();// get from @Html.Hidden
    var userLogin = $('#txtUserLogin').val();
    var LoaiHoaDonMenu = $('#txtLoaiHoaDon').val();
    var _id_NhanVien = $('.idnhanvien').text();
    var _IDNguoiDung = $('.idnguoidung').text();
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    var _now = new Date();
    var _nowFormat = moment(_now).format('YYYY-MM-DD');

    self.TodayBC = ko.observable('Toàn thời gian');
    self.ShopCookie = ko.observable($('#txtShopCookie').val());
    self.TenChiNhanh = ko.observableArray();
    self.HoaDons = ko.observableArray();
    self.BH_HoaDon_ChiTiet = ko.observableArray();
    self.NhanViens = ko.observableArray();
    self.PhongBans = ko.observableArray();
    self.GiaBans = ko.observableArray();
    self.NgayLapHD_Update = ko.observable();
    self.InforHDprintf = ko.observableArray();
    self.CTHoaDonPrint = ko.observableArray();
    self.CTHoaDonPrintMH = ko.observableArray();
    // lọc hàng hóa
    self.TT_HoanThanh = ko.observable(true);
    self.TT_DaHuy = ko.observable();
    self.TT_TamLuu = ko.observable(true);
    self.TT_GiaoHang = ko.observable(true);
    self.TT_DaDuyet = ko.observable(true);
    self.La_HDBan = ko.observable(LoaiHoaDonMenu === '1');
    self.La_HDSuaChua = ko.observable(LoaiHoaDonMenu === '25');
    self.La_HDHoTro = ko.observable(true);
    self.LoaiHoaDonMenu = ko.observable(parseInt($('#txtLoaiHoaDon').val()));
    self.Quyen_NguoiDung = ko.observableArray();
    // hoa don ban
    self.RoleView_Invoice = ko.observable(false);
    self.RoleInsert_Invoice = ko.observable(false);
    self.RoleInsert_HoaDonBaoHanh = ko.observable(false);
    self.RoleUpdate_Invoice = ko.observable(false);
    self.RoleDelete_Invoice = ko.observable(false);
    self.RoleExport_Invoice = ko.observable(false);
    self.RoleUpdateImg_Invoice = ko.observable(false);
    self.Role_ChangeInvoice_ifOtherDate = ko.observable(false);
    self.Role_DeleteInvoice_ifOtherDate = ko.observable(false);
    // hd dat
    self.RoleView_Order = ko.observable(false);
    self.RoleInsert_Order = ko.observable(false);
    self.RoleUpdate_Order = ko.observable(false);
    self.RoleDelete_Order = ko.observable(false);
    self.RoleExport_Order = ko.observable(false);
    self.RoleApprove_Order = ko.observable(false);
    // hd tra
    self.RoleView_Return = ko.observable(false);
    self.RoleInsert_Return = ko.observable(false);
    self.RoleUpdate_Return = ko.observable(false);
    self.RoleDelete_Return = ko.observable(false);
    self.RoleExport_Return = ko.observable(false);

    // hd hotro
    self.RoleView_HDHoTro = ko.observable(false);
    self.RoleUpdate_HDHoTro = ko.observable(false);
    self.RoleDelete_HDHoTro = ko.observable(false);

    self.Show_BtnUpdate = ko.observable(false);
    self.Show_BtnCopy = ko.observable(false);
    self.Show_BtnEdit = ko.observable(false);
    self.Show_BtnDelete = ko.observable(false);
    self.Show_BtnExcelDetail = ko.observable(false);
    self.Show_BtnThanhToanCongNo = ko.observable(false);
    self.Show_BtnOpenHD = ko.observable(false);
    self.Show_BtnXulyDH = ko.observable(false);
    self.Role_PrintHoaDon = ko.observable(false);
    self.Role_HoaHongDichVu_Edit = ko.observable(false);
    self.Role_HoaHongHoaDon_Edit = ko.observable(false);
    self.Role_SuaChiPhiDV = ko.observable(false);
    self.Role_NhapHangTuHoaDon = ko.observable(false);
    self.Role_XuatKho = ko.observable(false);
    self.Show_BtnInsertSoQuy = ko.observable(false);
    self.Show_BtnUpdateSoQuy = ko.observable(false);
    self.Show_BtnDeleteSoQuy = ko.observable(false);
    self.Allow_ChangeTimeSoQuy = ko.observable(false);

    self.BaoHiem = ko.observable(3);
    self.BaoHiemCo = ko.observable(true);
    self.BaoHiemKhong = ko.observable(true);
    self.filter = ko.observable();
    self.filterMaHDGoc = ko.observable();
    self.filterFind = ko.observable();
    self.selectedNV = ko.observable(_id_NhanVien);// NVien lap phieuthu
    self.ID_NhanVieUpdateHD = ko.observable();
    self.error = ko.observable();
    self.booleanAdd = ko.observable(true);
    self.ID_DonViQuiDoi = ko.observable();
    self.filterBangGia = ko.observable();
    self.filterPhongBan = ko.observable();
    self.PhongBanChosed = ko.observableArray();
    self.GiaBanChosed = ko.observableArray();
    self.filterNgayLapHD = ko.observable("0");
    self.filterNgayLapHD_Input = ko.observable(); // ngày cụ thể
    self.filterNgayLapHD_Quy = ko.observable(6); // Theo tháng
    self.BH_HoaDonChiTiets = ko.observableArray(); // split HoaDon = Hoa Don + Chi Tiet
    self.HoaDonDoiTra = ko.observableArray();
    self.LichSuThanhToan = ko.observableArray();
    self.LichSuTraHang = ko.observableArray();
    self.LichSuThanhToanDH = ko.observableArray();
    self.TongSLuong = ko.observable();
    self.MaHoaDonParent = ko.observable();
    self.CongTy = ko.observableArray(); // get infor congty
    self.ThietLap = ko.observableArray(); // ThietLapTinhNang HeThong
    self.ChotSo_ChiNhanh = ko.observableArray();
    self.ListIDNhanVienQuyen = ko.observableArray();
    self.ThietLap_TichDiem = ko.observableArray();
    self.ListCheckBox = ko.observableArray();
    self.NumberColum_Div2 = ko.observable();
    self.NumberColum_Div3 = ko.observable();
    self.columsort = ko.observable('NgayLapHoaDon');
    self.sort = ko.observable(0);
    self.PThucChosed = ko.observableArray();
    self.PTThanhToan = ko.observableArray([
        { ID: '1', TenPhuongThuc: 'Tiền mặt' },
        { ID: '2', TenPhuongThuc: 'POS' },
        { ID: '3', TenPhuongThuc: 'Chuyển khoản' },
        { ID: '4', TenPhuongThuc: 'Thẻ giá trị' }
    ]);
    // sum at footer
    self.TongTienBHDuyet = ko.observable(0);
    self.KhauTruTheoVu = ko.observable(0);
    self.GiamTruBoiThuong = ko.observable(0);
    self.BHThanhToanTruocThue = ko.observable(0);
    self.TongTienThueBaoHiem = ko.observable(0);

    self.TongTienHang = ko.observable();
    self.TongThanhToan = ko.observable();
    self.PhaiThanhToanBaoHiem = ko.observable();
    self.BaoHiemDaTra = ko.observable();
    self.TienDatCoc = ko.observable();
    self.TongChiPhi = ko.observable();
    self.TongKhachTra = ko.observable();
    self.KhachCanTra = ko.observable();
    self.TongGiamGia = ko.observable();
    self.TongGiamGiaKM = ko.observable();
    self.TongKhachNo = ko.observable();
    self.TongPhaiTraKhach = ko.observable(0);
    self.TongTienThue = ko.observable(0);
    self.TongNoKhach = ko.observable(0);
    self.TongTienDoiDiem = ko.observable(0);
    self.TongTienTheGTri = ko.observable(0);
    self.TongTienMat = ko.observable(0);
    self.ThanhTienChuaCK = ko.observable(0);
    self.GiamGiaCT = ko.observable(0);
    self.TongChuyenKhoan = ko.observable(0);
    self.TongPOS = ko.observable(0);
    self.TongGiaTriSDDV = ko.observable(0);
    self.ThuTuKhach = ko.observable();
    self.SelectPT = ko.observable();
    self.NoSau = ko.observable();
    self.ThoiGian_ThanhToan = ko.observable(moment(new Date()).format('DD/MM/YYYY HH:mm'));
    self.ListHDisDebit = ko.observableArray();
    self.GhiChu_PhieuThu = ko.observable();
    self.NoHienTai = ko.observable();
    self.TongTT_PhieuThu = ko.observable(0);
    self.ItemHoaDon = ko.observableArray();
    self.TienThua_PT = ko.observable(0);
    //lọc theo đơn vị
    self.ChiNhanhs = ko.observableArray();
    self.MangNhomDV = ko.observableArray();
    self.MangIDDV = ko.observableArray();
    self.filterHangHoa_ChiTietHD = ko.observable();
    // phan trang CTHD
    self.PageSize_CTHD = ko.observable(10);
    self.currentPage_CTHD = ko.observable(0);
    self.fromitem_CTHD = ko.observable(1);
    self.toitem_CTHD = ko.observable();
    self.PageCount_CTHD = ko.observable();
    self.TotalRecord_CTHD = ko.observable(0);
    // the gia tri + chiet khau nv
    self.TienTheGiaTri_PhieuThu = ko.observable();
    self.SoDuTheGiaTri = ko.observable();
    self.TongNapThe = ko.observable();
    self.SuDungThe = ko.observable(0);
    self.HoanTraTheGiaTri = ko.observable(0);

    self.IsGara = ko.observable(false);
    self.NganhKinhDoanh = ko.observable(1);//1.banle, 2.gara, 3.nhahang
    self.TongSoLuongHang = ko.observable(0);
    self.TongTienHangChuaCK = ko.observable(0);
    self.TongGiamGiaHang = ko.observable(0);
    self.TongTienPhuTung = ko.observable(0);
    self.TongTienDichVu = ko.observable(0);
    self.TongTienPhuTung_TruocCK = ko.observable(0);
    self.TongTienDichVu_TruocCK = ko.observable(0);
    self.TongTienPhuTung_TruocVAT = ko.observable(0);
    self.TongTienDichVu_TruocVAT = ko.observable(0);

    self.TongThue_PhuTung = ko.observable(0);
    self.TongCK_PhuTung = ko.observable(0);
    self.TongThue_DichVu = ko.observable(0);
    self.TongCK_DichVu = ko.observable(0);
    self.TongSL_DichVu = ko.observable(0);
    self.TongSL_PhuTung = ko.observable(0);

    self.ChiTietDoiTuong = ko.observableArray();
    self.InforBaoHiem = ko.observableArray();

    var urlBanHang = '';
    switch (self.ShopCookie()) {
        case 'C16EDDA0-F6D0-43E1-A469-844FAB143014':
            urlBanHang = '/g/Gara';
            self.IsGara(true);
            self.NganhKinhDoanh(2);
            break;
        case 'C1D14B5A-6E81-4893-9F73-E11C63C8E6BC':
            urlBanHang = '/$/NhaHang';
            self.NganhKinhDoanh(3);
            break;
        default:
            urlBanHang = '/$/BanLe';
            break;
    }

    var sLoai = '';
    var Key_Form = "KeyInvoices";
    switch (loaiHoaDon) {
        case 0:
        case 1:
            loaiHoaDon = self.LoaiHoaDonMenu();
            switch (self.LoaiHoaDonMenu()) {
                case 1:
                    sLoai = 'hóa đơn bán hàng';
                    break;
                case 2:
                    sLoai = 'hóa đơn bảo hành';
                    Key_Form = "KeyHDBaoHanh";
                    break;
                case 25:
                    sLoai = 'hóa đơn sửa chữa';
                    Key_Form = "KeyHDSuaChua";
                    break;
            }
            break;
        case 3:
            if (self.LoaiHoaDonMenu() === 0) {
                Key_Form = "KeyOrders";
                sLoai = 'hóa đơn đặt hàng';
            }
            else {
                Key_Form = "KeyBGSuaChua";
                sLoai = 'báo giá sửa chữa';
            }
            break;
        case 6:
            sLoai = 'hóa đơn trả hàng';
            Key_Form = "KeyReturns";
            break;
        case 25:
            sLoai = 'hóa đơn sửa chữa';
            Key_Form = "KeyHDSuaChua";
            break;
    }

    function PageLoad() {
        console.log('bdt')
        loadCheckbox();
        GetListIDNhanVien_byUserLogin();
        getAllChiNhanh();
        GetCauHinhHeThong();
        getListNhanVien();
        getAllPhongBan();
        getAllGiaBan();
        GetInforCongTy();
        loadMauIn();
        GetDM_NhomDoiTuong_ChiTiets();
        GetAllMauIn_byChiNhanh();
        GetKM_CTKhuyenMai();
        GetAllQuy_KhoanThuChi();
        GetDM_TaiKhoanNganHang();
        GetHT_TichDiem();
        GetAllNhomHangHoas();
    }
    PageLoad();

    function SetDefault_HideColumn() {
        var arrHideColumn = [];
        switch (loaiHoaDon) {
            case 1:
            case 25:
                arrHideColumn = ['madathang', 'email', 'diachi', 'sodienthoai', 'khuvuc', 'phuongxa', 'tenchinhanh', 'nguoiban', 'nguoitao',
                    'thanhtienchuack', 'giamgiact', 'giatrisudung', 'tienthue', 'pos', 'chuyenkhoan', 'tiendoidiem', 'thegiatri', 'trangthai'];
                break;
            case 3:
                arrHideColumn = ['email', 'diachi', 'sodienthoai', 'khuvuc', 'phuongxa', 'tenchinhanh', 'nguoiban', 'nguoitao', 'tonggiamgia', 'trangthai'];
                break;
            case 6:
                arrHideColumn = ['mahoadon', 'diachi', 'sodienthoai', 'tenchinhanh', 'phitrahang', 'nguoiban', 'nguoitao', 'tongsaugiamgia', 'trangthai'];
                break;
        }

        var cacheHideColumn = localStorage.getItem(Key_Form);
        if (cacheHideColumn === null || cacheHideColumn === '[]') {
            // hide default some column
            for (let i = 0; i < arrHideColumn.length; i++) {
                LocalCaches.AddColumnHidenGrid(Key_Form, arrHideColumn[i], arrHideColumn[i]);
            }
        }
    }
    function loadCheckbox() {
        if (loaiHoaDon === 1) {
            loaiHoaDon = self.LoaiHoaDonMenu();
        }
        $.getJSON("api/DanhMuc/BaseApi/GetListColumnInvoices?loaiHD=" + loaiHoaDon, function (data) {
            if (loaiHoaDon === 25) {
                self.NumberColum_Div2(Math.ceil(data.length / 3));
            }
            else {
                if (loaiHoaDon === 3) {
                    if (self.LoaiHoaDonMenu() === 0) {
                        data = $.grep(data, function (x) {
                            return $.inArray(x.Key, ['maphieutiepnhan', 'bienso']) === -1;
                        })
                    }
                }
                self.NumberColum_Div2(Math.ceil(data.length / 2));
            }
            self.NumberColum_Div3(Math.ceil(data.length / 3 * 2));
            self.ListCheckBox(data);
        });
    }
    function HideShowColumn() {
        SetDefault_HideColumn();
        LocalCaches.LoadFirstColumnGrid(Key_Form, $('#myList ul li input[type = checkbox]'), self.ListCheckBox());
    }
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
    function loadQuyenIndex() {
        var arrQuyen = [];
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetHT_NhomNguoiDung?idnguoidung=" + _IDNguoiDung + '&iddonvi=' + id_donvi, 'GET').done(function (data) {
            if (data.ID !== null) {
                self.Quyen_NguoiDung(data.HT_Quyen_NhomDTO);

                CheckRole_Invoice();

                // check role update/delete soquy
                self.Show_BtnInsertSoQuy(CheckQuyenExist('SoQuy_ThemMoi'));
                self.Show_BtnUpdateSoQuy(CheckQuyenExist('SoQuy_CapNhat'));
                self.Show_BtnDeleteSoQuy(CheckQuyenExist('SoQuy_Xoa'));
                self.Allow_ChangeTimeSoQuy(CheckQuyenExist('SoQuy_ThayDoiThoiGian'));
                self.Role_HoaHongDichVu_Edit(CheckQuyenExist('BanHang_HoaDongDichVu_CapNhat'))
                self.Role_HoaHongHoaDon_Edit(CheckQuyenExist('BanHang_HoaDongHoaDon_CapNhat'));
                self.Show_BtnThanhToanCongNo(CheckQuyenExist('KhachHang_ThanhToanNo'));

                self.Role_SuaChiPhiDV(CheckQuyenExist('HoaDon_SuaChiPhiDichVu'))
                self.Role_NhapHangTuHoaDon(CheckQuyenExist('NhapHang_ThemMoi'))
                self.Role_XuatKho(CheckQuyenExist('XuatHuy_ThemMoi'));
                self.RoleUpdateImg_Invoice(CheckQuyenExist('HoaDon_CapNhatAnh'));
            }
            else {
                ShowMessage_Danger('Không có quyền xem danh sách ' + sLoai);
            }
        });
    }
    function GetListIDNhanVien_byUserLogin() {
        ajaxHelper(CSKHUri + 'GetListNhanVienLienQuanByIDLoGin_inDepartment?idnvlogin=' + _id_NhanVien
            + '&idChiNhanh=' + id_donvi + '&funcName=' + funcName, 'GET').done(function (data) {
                self.ListIDNhanVienQuyen(data);
                loadQuyenIndex();
            })
    }
    function UpdateIDDoiTuong_inHoaDon_andPhieuThu() {
        ajaxHelper(BH_HoaDonUri + 'UpdateIDDoiTuong_inHoaDon_andPhieuThu', 'GET').done(function (data) {
        })
    }

    self.gotoGara = function () {
        if (self.IsGara()) {
            var newwindow = window.open(urlBanHang, '_blank');
            var popupTick = setInterval(function () {
                if (newwindow.closed) {
                    clearInterval(popupTick);
                    SearchHoaDon();
                }
            }, 500);
        }
        else {
            window.open(urlBanHang, '_blank');
        }
    }

    self.clickbanhang = function () {
        switch (loaiHoaDon) {
            case 1:
            case 25:
                localStorage.setItem('fromHoaDon', true);
                break;
            case 2:
                localStorage.setItem('fromBaoHanh', true);
                break;
        }
        self.gotoGara();
    }
    self.selectedCN = function (item) {
        $("#iconSort").remove();
        self.columsort('NgayLapHoaDon');
        self.sort(0);
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
    self.showPopupNCC = function () {
        self.resetNhaCungCap();
        $('#modalPopuplg_NCC').modal('show');
        $('#modalPopuplg_NCC').on('shown.bs.modal', function () {
            $('#txtTenDoiTuong').select();
        })
        $('#lblTitleNCC').html("Thêm nhà cung cấp")
    };
    self.HuyHoaDon_updateChoThanhToan = function (item) {
        var msgBottom = '';
        var msgDialog = '';
        var idHoaDon = item.ID;
        var urlCheck = BH_HoaDonUri + 'GetDSHoaDon_chuaHuy_byIDDatHang/' + idHoaDon;
        switch (item.LoaiHoaDon) {
            case 1:
            case 0:
                msgBottom = "Hóa đơn đã có trả hàng, không thể hủy";
                break;
            case 3:
                msgBottom = "Phiếu đặt hàng đã có hóa đơn, không thể hủy";
                break;
            case 6:
                msgBottom = "Phiếu trả hàng đã có hóa đơn, không thể hủy";
                break;
            case 25:
                urlCheck = '/api/DanhMuc/GaraAPI/CheckHoaDon_DaXuLy?idHoaDon=' + idHoaDon + '&loaiHoaDon=8';
                msgBottom = "Hóa đơn đã có phiếu xuất kho, không thể hủy";
                break;
            case 36:
                msgBottom = "Hóa đơn đã tạo phiếu xuất kho, không thể hủy";
                break;
        }
        // huy hoadon : neu dang co tra hang --> khong duoc huy 
        // huy dat hang: neu dang co HD tao tu HD dat hang --> khong duoc huy
        ajaxHelper(urlCheck, 'GET').done(function (x) {
            if (x === true) {
                ShowMessage_Danger(msgBottom);
                return;
            }
            else {
                if (item.LoaiHoaDon !== 3) {
                    msgDialog = 'Có muốn hủy hóa đơn <b>' + item.MaHoaDon + '</b> cùng những phiếu liên quan không?'
                }
                else {
                    if (item.KhachDaTra > 0) {
                        msgDialog = 'Có muốn hủy hóa đơn <b>' + item.MaHoaDon + '</b> cùng tiền đặt cọc không?'
                    }
                    else {
                        msgDialog = 'Có muốn hủy hóa đơn <b>' + item.MaHoaDon + '</b> cùng những phiếu liên quan không?'
                    }
                }
                // move dialogConfirm() in this
                dialogConfirm('Thông báo xóa', msgDialog, function () {
                    $.ajax({
                        type: "POST",
                        url: BH_HoaDonUri + "Huy_HoaDon?id=" + idHoaDon + '&nguoiSua=' + userLogin + '&iddonvi=' + id_donvi,
                        dataType: 'json',
                        contentType: 'application/json',
                        success: function (result) {
                            ShowMessage_Success("Cập nhật " + sLoai + " thành công");
                            SearchHoaDon();
                            var objDiary = {
                                ID_NhanVien: _id_NhanVien,
                                ID_DonVi: id_donvi,
                                ChucNang: 'Danh mục ' + sLoai,
                                NoiDung: "Xóa " + sLoai + ": " + item.MaHoaDon,
                                NoiDungChiTiet: "Xóa ".concat(sLoai, ": ", item.MaHoaDon, ', Người xóa: ', userLogin),
                                LoaiNhatKy: 3
                            };
                            if (item.LoaiHoaDon !== 3) {
                                // HuyHD : tru diem (cong diem am)
                                // Huy TraHang: cong diem
                                // HuyDatHang: khong thuc hien gi ca
                                var diemGiaoDich = item.DiemGiaoDich;
                                if (diemGiaoDich > 0 && item.ID_DoiTuong !== null) {
                                    if (item.LoaiHoaDon === 1 || item.LoaiHoaDon === 25) {
                                        diemGiaoDich = -diemGiaoDich;
                                    }
                                    ajaxHelper(DMDoiTuongUri + 'HuyHD_UpdateDiem?idDoiTuong=' + item.ID_DoiTuong + '&diemGiaoDich=' + diemGiaoDich, 'POST').done(function (data) {
                                    });
                                }
                                objDiary.ID_HoaDon = idHoaDon;
                                objDiary.LoaiHoaDon = item.LoaiHoaDon;
                                objDiary.ThoiGianUpdateGV = item.NgayLapHoaDon;
                                Post_NhatKySuDung_UpdateGiaVon(objDiary);
                                vmThanhToan.NangNhomKhachHang(item.ID_DoiTuong);

                                switch (item.LoaiHoaDon) {                                 
                                    case 25:
                                        HuyHoaDon_UpdateLichBaoDuong(idHoaDon);
                                        break;
                                }
                            }
                            else {
                                Insert_NhatKyThaoTac_1Param(objDiary);
                            }
                        },
                        error: function (error) {
                            ShowMessage_Danger('Cập nhật trạng thái thất bại');
                        },
                        complete: function () {
                            $('#wait').remove();
                            $('#modalPopuplgDelete').modal('hide');
                        }
                    });
                })
            }
        });
    }

    function HuyHoaDon_UpdateLichBaoDuong(idHoaDon) {
        ajaxHelper('/api/DanhMuc/GaraAPI/' + 'HuyHoaDon_UpdateLichBaoDuong?idHoaDon=' + idHoaDon, 'GET').done(function (x) {
        });
    }
   
    function UpdateLichBD_whenChangeNgayLapHD(idHoaDon, ngaylapOld, ngaylapNew) {
        ajaxHelper('/api/DanhMuc/GaraAPI/UpdateLichBD_whenChangeNgayLapHD?idHoaDon=' + idHoaDon +
            '&ngaylapOld=' + ngaylapOld + '&ngaylapNew=' + ngaylapNew, 'GET')
            .done(function (x) {
            })
    }

    // huy hdDoi, khong huy hdTra
    self.HuyHD_DoiTraHang = function (parent, item) {
        var idDoiTra = item.ID;

        var msg = 'Hóa đơn <b>' + item.MaHoaDon + ' </b> có liên quan đến giao dịch trả hàng <b>' + parent.MaHoaDon + ' </b> . Bạn có chắc chắn muốn hủy không?';
        dialogConfirm('Xác nhận hủy', msg, function () {
            $.ajax({
                type: "POST",
                url: BH_HoaDonUri + "Huy_HoaDon?id=" + idDoiTra + '&nguoiSua=' + userLogin + '&iddonvi=' + id_donvi,
                dataType: 'json',
                contentType: 'application/json',
                success: function (result) {
                    SearchHoaDon();

                    var diemGiaoDich = - item.DiemGiaoDich;
                    if (diemGiaoDich !== 0 && parent.ID_DoiTuong !== null) {
                        ajaxHelper(DMDoiTuongUri + 'HuyHD_UpdateDiem?idDoiTuong=' + parent.ID_DoiTuong + '&diemGiaoDich=' + diemGiaoDich, 'POST').done(function (data) {
                        });
                    }
                    // insert Ht_NhatKySuDung (HDDoiTra)
                    var objDiary = {
                        ID_NhanVien: _id_NhanVien,
                        ID_DonVi: id_donvi,
                        ChucNang: "Hóa đơn đổi trả",
                        NoiDung: "Xóa hóa đơn đổi trả: " + item.MaHoaDon,
                        NoiDungChiTiet: "Xóa hóa đơn đổi trả: " + item.MaHoaDon,
                        LoaiNhatKy: 3,
                        ID_HoaDon: idDoiTra,
                        LoaiHoaDon: 1,
                        ThoiGianUpdateGV: item.NgayLapHoaDon,
                    };
                    Post_NhatKySuDung_UpdateGiaVon(objDiary);
                    ShowMessage_Success("Hủy hóa đơn thành công");
                },
                error: function (error) {
                    ShowMessage_Danger('Cập nhật hóa đơn đổi trả trạng thái thất bại');
                },
                complete: function () {
                    $('#modalPopuplgDelete').modal('hide');
                }
            })
        });
    }

    self.GetID_NhanVien = function (item) {
        self.ID_NhanVieUpdateHD(item.ID_NhanVien); //--> get to do updateHoaDon
    }
    self.updateHoaDon = function (formElement) {
        var id = formElement.ID;
        var maHoaDon = formElement.MaHoaDon;
        var idNhanVien = self.ID_NhanVieUpdateHD();
        var ngaylapHDOld = formElement.NgayLapHoaDon;
        var loaiHoaDon = formElement.LoaiHoaDon;
        if (idNhanVien === undefined) {
            // if not change ID_NhanVien --> get from DB
            idNhanVien = formElement.ID_NhanVien;
            // if ID_NhanVien in DB = null --> get ID_NhanVien login
            if (idNhanVien === null) {
                idNhanVien = _id_NhanVien;
            }
        }
        if (self.NgayLapHD_Update() === undefined) {
            self.NgayLapHD_Update(moment(formElement.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'));
        }
        var check = CheckNgayLapHD_format(self.NgayLapHD_Update(), formElement.ID_DonVi);
        if (!check) {
            return;
        }
        var ngaylapHD = moment(self.NgayLapHD_Update(), 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm:ss');
        var HoaDon = {
            ID: id,
            MaHoaDon: maHoaDon,
            ID_NhanVien: idNhanVien,
            DienGiai: formElement.DienGiai,
            NguoiSua: userLogin,
            NgayLapHoaDon: ngaylapHD,
        };
        // compare to update GiaVon (alway Ngay min)
        ngaylapHDOld = moment(ngaylapHDOld).format('YYYY-MM-DD HH:mm:ss'); // alway NgayLapHoaDon old 
       
        var myData = {};
        myData.id = id;
        myData.objNewHoaDon = HoaDon;
        $.ajax({
            data: myData,
            url: BH_HoaDonUri + "PutBH_HoaDon2",
            type: 'PUT',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (x) {
                if (x.res === true) {
                    SearchHoaDon();
                    ShowMessage_Success("Cập nhật " + sLoai + " thành công")
                    var objDiary = {
                        ID_NhanVien: _id_NhanVien,
                        ID_DonVi: id_donvi,
                        ChucNang: 'Danh mục ' + sLoai,
                        NoiDung: "Cập nhật  ".concat(sLoai, ": ", maHoaDon),
                        NoiDungChiTiet: "Cập nhật  ".concat(sLoai, ": ", maHoaDon, 
                            '<br /> Ngày lập hóa đơn cũ: ', ngaylapHDOld,
                            '<br /> Ngày lập hóa đơn mới: ', ngaylapHD,
                        ),
                        LoaiNhatKy: 2
                    };
                    if (loaiHoaDon !== 3) {
                        objDiary.ID_HoaDon = id;
                        objDiary.LoaiHoaDon = loaiHoaDon;
                        objDiary.ThoiGianUpdateGV = ngaylapHDOld;
                        Post_NhatKySuDung_UpdateGiaVon(objDiary);

                        switch (formElement.LoaiHoaDon) {
                            case 1:
                            case 36:
                                vmApDungNhomHoTro.ChangeNgayLapHD_UpdatePhieuXuatKho(id);
                                break;
                            case 25:
                                UpdateLichBD_whenChangeNgayLapHD(id, ngaylapHDOld, ngaylapHD);
                                break;
                        }
                    }
                    else {
                        Insert_NhatKyThaoTac_1Param(objDiary);
                    }
                }
                else {
                    ShowMessage_Danger("Cập nhật " + sLoai + " thất bại");
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            },
        })
    }
    self.exportToExcelDoiTuong = function () {
        tableToExcel('tblDanhMucHangHoa', 'dmHangHoas.xls');
    }
    self.importFromExcelDoiTuong = function () {
        $("#fileLoader").click();
    }

    $('#txtMaHD, #txtMaHDGoc').keypress(function (e) {
        $("#iconSort").remove();
        ResetColumnSort();
        if (e.keyCode === 13 || e.which === 13) {
            // reset currentPage if is finding at other page > 1
            self.currentPage(0);
            SearchHoaDon();
        }
    })
    self.Click_IconSearch = function () {
        ResetColumnSort();
        self.currentPage(0);
        SearchHoaDon();
    }
    function ResetColumnSort() {
        self.columsort('NgayLapHoaDon');
        self.sort(0);
    }
    //phân trang
    self.pageSizes = [10, 20, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(0);
    self.toitem = ko.observable(0);
    self.PageCount = ko.observable(0);
    self.TotalRecord = ko.observable(0);
    // tim kiem JqAuto hàng hóa
    self.filterFind = function (item, inputString) {
        var itemSearch = locdau(item.TenHangHoa);
        var itemSearch1 = locdau(item.MaHangHoa);
        var locdauInput = locdau(inputString);
        var arr = itemSearch.split(/\s+/);
        var arr1 = itemSearch1.split(/\s+/);
        var sThreechars = '';
        var sThreechars1 = '';
        for (let i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }
        for (let i = 0; i < arr1.length; i++) {
            sThreechars1 += arr1[i].toString().split('')[0];
        }
        return itemSearch.indexOf(locdauInput) > -1 ||
            itemSearch1.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1 ||
            sThreechars1.indexOf(locdauInput) > -1;
    }
    self.ResetCurrentPage = function () {
        $("#iconSort").remove();
        ResetColumnSort();

        self.currentPage(0);
        SearchHoaDon();
    };
    function getAllPhongBan() {
        ajaxHelper('/api/DanhMuc/DM_ViTriAPI/' + "GetListViTris", 'GET').done(function (data) {
            self.PhongBans(data);
        });
    }
    function getAllGiaBan() {
        ajaxHelper("/api/DanhMuc/DM_GiaBanAPI/" + "GetDM_GiaBanByIDDonVi?iddonvi=" + id_donvi, 'GET').done(function (data) {
            if (data !== null && data.length > 0) {
                self.GiaBans(data);
            }
            var objBGChung = {
                ID: '00000000-0000-0000-0000-000000000000',
                //ID: null,
                TenGiaBan: 'Bảng giá chung',
            }
            self.GiaBans.unshift(objBGChung);
        })
    }
    function getListNhanVien() {
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetNS_NhanVien_InforBasic?idDonVi=" + id_donvi, 'GET').done(function (data) {
            self.NhanViens(data);

            vmThanhToan.listData.NhanViens = self.NhanViens();
            vmHoaHongHoaDon.listData.NhanViens = self.NhanViens();
            vmHoaHongDV.listData.NhanViens = self.NhanViens();
            vmHoaHongDV.inforHoaDon.ID_DonVi = id_donvi;
        });
    }
    self.ShowColumn_LoHang = ko.observable(false);
    function GetCauHinhHeThong() {
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + "GetCauHinhHeThong/" + id_donvi, 'GET').done(function (data) {
            self.ThietLap(data);
            self.ShowColumn_LoHang(data.LoHang);
        });
    }
    function getAllChiNhanh() {
        ajaxHelper('/api/DanhMuc/DM_DonViAPI/' + "GetListDonViByIDNguoiDung?idnhanvien=" + _id_NhanVien, 'GET').done(function (data) {
            var arrSortbyName = data.sort((a, b) => a.TenDonVi.localeCompare(b.TenDonVi, undefined, { caseFirst: "upper" }));
            self.ChiNhanhs(arrSortbyName);
            vmThanhToan.listData.ChiNhanhs = arrSortbyName;

            var obj = {
                ID: id_donvi,
                TenDonVi: $('#_txtTenDonVi').html()
            }
            // assign mangChiNhanh, and set check: avoid load douple list HoaDon
            self.MangNhomDV.push(obj);
            $('#selec-all-DV li').each(function () {
                if ($(this).attr('id') === id_donvi) {
                    $(this).find('.fa-check').remove();
                    $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                }
            });
            $('#choose_TenDonVi input').remove();
        });
    }
    self.filterProvince = function (item, inputString) {
        var itemSearch = locdau(item.TenTinhThanh);
        var locdauInput = locdau(inputString);
        // nen bat chat khong cho nhap dau cach o MaKH
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';
        for (let i = 0; i < arr.length; i++) {
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
        for (let i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }
        return itemSearch.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    }
    self.GetClassHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage()) ? "click" : "";
    };
    self.SelectedPB = function (item) {
        $("#iconSort").remove();
        ResetColumnSort();
        var arrIDPB = [];
        for (let i = 0; i < self.PhongBanChosed().length; i++) {
            if ($.inArray(self.PhongBanChosed()[i].ID, arrIDPB) === -1) {
                arrIDPB.push(self.PhongBanChosed()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDPB) === -1) {
            self.PhongBanChosed.push(item);
        }
        $('#choose-PB input').remove();
        // add check after li
        $('#selec-all-PB li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });
        self.currentPage(0);
        SearchHoaDon();
    }
    self.ClosePhongBan = function (item) {
        $("#iconSort").remove();
        ResetColumnSort();
        self.PhongBanChosed.remove(item);
        if (self.PhongBanChosed().length === 0) {
            $('#choose-PB').append('<input type="text" class="dropdown form-control" placeholder="Chọn phòng/bàn">');
        }
        // remove check
        $('#selec-all-PB li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        SearchHoaDon();
    }
    self.SelectedGiaBan = function (item) {
        $("#iconSort").remove();
        ResetColumnSort();
        var arrID_BangGia = [];
        for (let i = 0; i < self.GiaBanChosed().length; i++) {
            if ($.inArray(self.GiaBanChosed()[i].ID, arrID_BangGia) === -1) {
                arrID_BangGia.push(self.GiaBanChosed()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrID_BangGia) === -1) {
            self.GiaBanChosed.push(item);
        }
        $('#choose-GB input').remove();
        // add check after li
        $('#selec-all-GB li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });
        self.currentPage(0);
        SearchHoaDon();
    }
    self.CloseBangGia = function (item) {
        $("#iconSort").remove();
        ResetColumnSort();
        self.GiaBanChosed.remove(item);
        if (self.GiaBanChosed().length === 0) {
            $('#choose-GB').append('<input type="text" class="dropdown form-control" placeholder="Chọn bảng giá">');
        }
        // remove check
        $('#selec-all-GB li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        SearchHoaDon();
    }
    self.ChosePTThanhToan = function (item) {
        $("#iconSort").remove();
        ResetColumnSort();
        var arrIDPB = [];
        for (let i = 0; i < self.PThucChosed().length; i++) {
            if ($.inArray(self.PThucChosed()[i].ID, arrIDPB) === -1) {
                arrIDPB.push(self.PThucChosed()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDPB) === -1) {
            self.PThucChosed.push(item);
        }
        $('#choose-PThuc input').remove();
        // add check after li
        $('#selec-pt-ThanhToan li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });
        self.currentPage(0);
        SearchHoaDon();
    }
    self.ClosePhuongThuc = function (item) {
        $("#iconSort").remove();
        ResetColumnSort();
        self.PThucChosed.remove(item);
        if (self.PThucChosed().length === 0) {
            $('#choose-PThuc').append('<input type="text" class="dropdown form-control" placeholder="Chọn phương thức">');
        }
        // remove check
        $('#selec-pt-ThanhToan li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        SearchHoaDon();
    }
    $('.choseNgayTao li').on('click', function () {
        $("#iconSort").remove();
        ResetColumnSort();
        $('#txtNgayTao').val($(this).text());
        self.filterNgayLapHD_Quy($(this).val());
        self.currentPage(0);
        SearchHoaDon();
    });

    function GetColumHide(firsColumn = 0) {// hdban: colum 1= checkbox
        var cacheHideColumn2 = localStorage.getItem(Key_Form);
        if (cacheHideColumn2 !== null) {
            cacheHideColumn2 = JSON.parse(cacheHideColumn2);
            switch (Key_Form) {
                case 'KeyInvoices':
                    break;
                case 'KeyOrders':
                    break;
                case 'KeyReturns':
                    cacheHideColumn2 = cacheHideColumn2.filter(x => $.inArray(['email', 'khuvuc', 'phuongxa'], x) == -1);
                    break;
                case 25:
                    break;
            }
            var arrColumn = [];
            columnHide = '';
            var tdClass = $('#tb thead tr th');
            for (var i = 0; i < cacheHideColumn2.length; i++) {
                var itemFor = cacheHideColumn2[i];
                if (itemFor.Value !== undefined) {
                    $(tdClass).each(function (index) {
                        var className = $(this).attr('class');
                        if (className !== undefined && className.indexOf(itemFor.Value) > -1) {
                            // push if not exist
                            if ($.inArray(itemFor.Value, arrColumn) === -1) {
                                arrColumn.push(itemFor.Value);
                                columnHide += (index - firsColumn) + '_';
                            }
                        }
                    })
                }
            }
        }

        var lstColumn = columnHide.split('_');
        lstColumn = lstColumn.filter(x => x !== '');
        var lstAfter = [];
        switch (loaiHoaDon) {
            case 1:
            case 2:
            case 3:
            case 25:
            case 6:
                for (let i = 0; i < lstColumn.length; i++) {
                    lstAfter.push(formatNumberToFloat(lstColumn[i]));
                }
                break;
        }

        columnHide = '';
        for (var i = 0; i < lstAfter.length; i++) {
            columnHide += lstAfter[i].toString() + '_';
        }
    }

    var dayStart_Excel, dayEnd_Excel;// used to export many hoadon

    function SearchHoaDon(isExport = false) {
        var arrDV = [];
        $('.line-right').height(0).css("margin-top", "0px");
        var maHDFind = localStorage.getItem('FindHD');
        if (maHDFind !== null) {
            self.filter(maHDFind);
            self.filterNgayLapHD('0');
            self.filterNgayLapHD_Quy(0);
        }
        var txtMaHDon = self.filter();
        var txtMaHDgoc = self.filterMaHDGoc(); // HD tra hang
        var arrIDPB = [];
        for (let i = 0; i < self.PhongBanChosed().length; i++) {
            arrIDPB.push(self.PhongBanChosed()[i].ID);
        }
        var arrIDBangGia = [];
        for (let i = 0; i < self.GiaBanChosed().length; i++) {
            arrIDBangGia.push(self.GiaBanChosed()[i].ID);
        }
        var ptThanhToan = $.map(self.PThucChosed(), function (x) { return x.ID });

        if (txtMaHDon === undefined) {
            txtMaHDon = "";
        }
        if (txtMaHDgoc === undefined) {
            txtMaHDgoc = "";
        }
        var sTenChiNhanhs = '';
        for (let i = 0; i < self.MangNhomDV().length; i++) {
            if ($.inArray(self.MangNhomDV()[i], arrDV) === -1) {
                arrDV.push(self.MangNhomDV()[i].ID);
                sTenChiNhanhs += self.MangNhomDV()[i].TenDonVi + ',';
            }
        }
        sTenChiNhanhs = Remove_LastComma(sTenChiNhanhs);// use when export excel
        self.MangIDDV(arrDV);
        // avoid error in Store procedure
        if (self.MangIDDV().length === 0) {
            self.MangIDDV([id_donvi]);
        }
        // trang thai hoadon
        var arrStatus = [];
        if (self.TT_DaDuyet()) {
            arrStatus.push('0');
        }
        if (self.TT_TamLuu()) {
            arrStatus.push('1');
        }
        if (self.TT_GiaoHang()) {
            arrStatus.push('2');
        }
        if (self.TT_HoanThanh()) {
            arrStatus.push('3');
        }
        if (self.TT_DaHuy()) {
            arrStatus.push('4');
        }

        var arrLoaiHD = [];
        if (self.LoaiHoaDonMenu() === 1) {
            if (self.La_HDBan()) {
                arrLoaiHD.push(1);
            }
            if (self.La_HDHoTro()) {
                arrLoaiHD.push(36);
            }

            if (arrLoaiHD.length === 0) {
                arrLoaiHD = [1, 36];
            }
        }
        else {
            arrLoaiHD = [self.LoaiHoaDonMenu()];
        }

        // NgayLapHoaDon
        var dayStart = '';
        var dayEnd = '';
        if (self.filterNgayLapHD() === '0') {
            switch (self.filterNgayLapHD_Quy()) {
                case 0:
                    // all
                    self.TodayBC('Toàn thời gian');
                    dayStart = '2010-01-01';
                    dayEnd = moment(_now).add(1, 'days').format('YYYY-MM-DD');
                    break;
                case 1:
                    // hom nay
                    self.TodayBC('Hôm nay');
                    dayStart = moment(_now).format('YYYY-MM-DD');
                    dayEnd = moment(_now).add(1, 'days').format('YYYY-MM-DD');
                    break;
                case 2:
                    // hom qua
                    self.TodayBC('Hôm qua');
                    dayEnd = moment(_now).format('YYYY-MM-DD');
                    dayStart = moment(_now).subtract(1, 'days').format('YYYY-MM-DD');
                    break;
                case 3:
                    // tuan nay
                    self.TodayBC('Tuần này');
                    dayStart = moment().startOf('week').add('days', 1).format('YYYY-MM-DD');
                    dayEnd = moment().endOf('week').add(2, 'days').format('YYYY-MM-DD');
                    break;
                case 4:
                    // tuan truoc
                    self.TodayBC('Tuần trước');
                    dayStart = moment().weekday(-6).format('YYYY-MM-DD');
                    dayEnd = moment(dayStart, 'YYYY-MM-DD').add(7, 'days').format('YYYY-MM-DD'); // add day in moment.js
                    break;
                case 5:
                    // 7 ngay qua
                    self.TodayBC('7 ngày qua');
                    dayEnd = moment(_now).format('YYYY-MM-DD');
                    dayStart = moment(_now).subtract(7, 'days').format('YYYY-MM-DD');
                    break;
                case 6:
                    // thang nay
                    self.TodayBC('Tháng này');
                    dayStart = moment().startOf('month').format('YYYY-MM-DD');
                    dayEnd = moment().endOf('month').add(1, 'days').format('YYYY-MM-DD'); // add them 1 ngày 01-month-year --> compare in SQL
                    break;
                case 7:
                    // thang truoc
                    self.TodayBC('Tháng trước');
                    dayStart = moment().subtract(1, 'months').startOf('month').format('YYYY-MM-DD');
                    dayEnd = moment().subtract(1, 'months').endOf('month').add(1, 'days').format('YYYY-MM-DD');
                    break;
                case 10:
                    // quy nay
                    self.TodayBC('Quý này');
                    dayStart = moment().startOf('quarter').format('YYYY-MM-DD');
                    dayEnd = moment().endOf('quarter').add(1, 'days').format('YYYY-MM-DD');
                    break;
                case 11:
                    // quy truoc = currQuarter -1; // if (currQuarter -1 === 0) --> (assign = 1)
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
                    dayEnd = moment().endOf('year').add(1, 'days').format('YYYY-MM-DD');
                    break;
                case 13:
                    // nam truoc
                    self.TodayBC('Năm trước');
                    var prevYear = moment().year() - 1;
                    dayStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                    dayEnd = moment().year(prevYear).endOf('year').add(1, 'days').format('YYYY-MM-DD');
                    break;
            }
        }
        else {
            var arrDate = self.filterNgayLapHD_Input().split('-');
            dayStart = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
            dayEnd = moment(arrDate[1], 'DD/MM/YYYY').add('days', 1).format('YYYY-MM-DD');

            self.TodayBC('Từ ' + moment(arrDate[0], 'DD/MM/YYYY').format('DD/MM/YYYY') + ' đến ' + moment(arrDate[1], 'DD/MM/YYYY').format('DD/MM/YYYY'));
        }
        dayStart_Excel = dayStart;
        dayEnd_Excel = dayEnd;
        var Params_GetListHoaDon = {
            CurrentPage: self.currentPage(),
            PageSize: self.pageSize(),
            LoaiHoaDon: loaiHoaDon,
            LaHoaDonSuaChua: arrLoaiHD,
            MaHoaDon: locdau(txtMaHDon).trim(),
            MaHoaDonGoc: locdau(txtMaHDgoc.trim()),
            ID_ChiNhanhs: self.MangIDDV(),
            ID_ViTris: arrIDPB,
            ID_BangGias: arrIDBangGia,
            ID_NhanViens: [_id_NhanVien], // nvlogin
            NguoiTao: userLogin,
            TrangThai: '',
            NgayTaoHD_TuNgay: dayStart,
            NgayTaoHD_DenNgay: dayEnd,
            TrangThai_SapXep: self.sort(), // 1. Tang dan, 2. Giamdan
            Cot_SapXep: self.columsort(),
            //PTThanhToan: '',
            ColumnsHide: '',
            SortBy: self.sort() == 1 ? 'ASC' : 'DESC',
            ValueText: sTenChiNhanhs,
            TrangThaiHDs: arrStatus,
            PhuongThucTTs: ptThanhToan,
            BaoHiem: self.BaoHiem()
        }

        if (isExport) {
            $('.content-table').gridLoader();
            var txtLoaiHD = 'Hóa đơn';
            var funcName = 'ExportExcel_HoaDonBanLe'; // loai 1, 19, 2
            var noidungNhatKy = "Xuất excel danh sách hóa đơn";

            switch (loaiHoaDon) {
                case 1:
                    GetColumHide(1);
                    funcName = 'ExportExcel_HoaDonBanLe';
                    break;
                case 25:
                    funcName = 'ExportExcel_HoaDonSuaChua';
                    GetColumHide(1);
                    break;
                case 2:
                    GetColumHide(1);
                    txtLoaiHD = 'Hóa đơn bảo hành';
                    funcName = 'ExportExcel_HoaDonBaoHanh';
                    noidungNhatKy = "Xuất excel danh sách hóa đơn bảo hành";
                    break;
                case 3:
                    GetColumHide(0);
                    txtLoaiHD = 'Đặt hàng';
                    funcName = 'ExportExcel_DatHang';
                    noidungNhatKy = "Xuất excel danh sách hóa đơn đặt hàng";
                    break;
                case 6:
                    GetColumHide(0);
                    txtLoaiHD = 'Trả hàng';
                    funcName = 'ExportExcel_PhieuTraHang';
                    noidungNhatKy = "Xuất excel danh sách hóa đơn trả hàng";
                    break;
            }
            Params_GetListHoaDon.currentPage = 0;
            Params_GetListHoaDon.PageSize = self.TotalRecord();
            Params_GetListHoaDon.ColumnsHide = columnHide;

            ajaxHelper(BH_HoaDonUri + funcName, 'POST', Params_GetListHoaDon).done(function (url) {
                $('.content-table').gridLoader({ show: false });
                if (url !== "") {
                    self.DownloadFileTeamplateXLSX(url);
                }
            })
            var objDiary = {
                ID_NhanVien: _id_NhanVien,
                ID_DonVi: id_donvi,
                ChucNang: txtLoaiHD,
                NoiDung: noidungNhatKy,
                LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
            };
            Insert_NhatKyThaoTac_1Param(objDiary);
        }
        else {
            var hasPermission = false;
            switch (loaiHoaDon) {
                case 0:
                case 1:
                case 2:
                case 25:
                    hasPermission = self.RoleView_Invoice();
                    break;
                case 3:
                    hasPermission = self.RoleView_Order();
                    break;
                case 6:
                    hasPermission = self.RoleView_Return();
                    break;
            }
            if (hasPermission) {
                $('.content-table').gridLoader();
                ajaxHelper(BH_HoaDonUri + 'GetListInvoice_Paging', 'POST', Params_GetListHoaDon).done(function (x) {
                    $('.content-table').gridLoader({ show: false });

                    if (x.res && x.dataSoure.length > 0) {
                        var first = x.dataSoure[0];

                        self.HoaDons(x.dataSoure);
                        self.TotalRecord(first.TotalRow);
                        self.PageCount(first.TotalPage);
                        self.TongTienHang(first.SumTongTienHang);
                        self.TongChiPhi(first.SumTongChiPhi);
                        self.TongThanhToan(first.SumTongThanhToan);
                        self.TongGiamGia(first.SumTongGiamGia);
                        self.TongGiamGiaKM(first.SumKhuyeMai_GiamGia);
                        self.TongKhachTra(first.SumKhachDaTra);
                        self.KhachCanTra(first.SumPhaiThanhToan);
                        self.PhaiThanhToanBaoHiem(first.SumPhaiThanhToanBaoHiem);
                        self.BaoHiemDaTra(first.SumBaoHiemDaTra);
                        self.TienDatCoc(first.SumTienCoc);
                        self.TongTienDoiDiem(first.SumTienDoiDiem);
                        self.TongTienTheGTri(first.SumThuTuThe);
                        self.ThanhTienChuaCK(first.SumThanhTienChuaCK);
                        self.GiamGiaCT(first.SumGiamGiaCT);
                        self.TongPhaiTraKhach(first.TongPhaiTraKhach);

                        self.TongTienBHDuyet(first.SumTongTienBHDuyet);
                        self.KhauTruTheoVu(first.SumKhauTruTheoVu);
                        self.GiamTruBoiThuong(first.SumGiamTruBoiThuong);
                        self.BHThanhToanTruocThue(first.SumBHThanhToanTruocThue);
                        self.TongTienThueBaoHiem(first.SumTongTienThueBaoHiem);

                        self.TongTienMat(first.SumTienMat); // tra hang
                        self.TongChuyenKhoan(first.SumChuyenKhoan);
                        self.TongPOS(first.SumPOS);
                        self.TongTienThue(first.SumTongTienThue);
                        self.TongGiaTriSDDV(first.TongGiaTriSDDV);
                        self.TongKhachNo(first.SumConNo);
                        self.TongNoKhach(first.SumConNo);// trahang
                    }
                    else {
                        // if not data, reset 
                        self.HoaDons([]);
                        self.TotalRecord(0);
                        self.PageCount(0);
                        self.TongTienHang(0);
                        self.TongChiPhi(0);
                        self.TongThanhToan(0);
                        self.TongGiamGia(0);
                        self.GiamGiaCT(0);
                        self.TongGiamGiaKM(0);
                        self.TongKhachTra(0);
                        self.TongPhaiTraKhach(0);
                        self.TongKhachNo(0);
                        self.TongTienDoiDiem(0);
                        self.TongTienTheGTri(0);
                        self.KhachCanTra(0);
                        self.TongTienMat(0);
                        self.TongPOS(0);
                    }
                    HideShowColumn();
                    SetCheck_Input();
                });
            }
            localStorage.removeItem('FindHD');
        }
    }
    self.La_HDSuaChua.subscribe(function () {
        $("#iconSort").remove();
        ResetColumnSort();
        self.currentPage(0);
        SearchHoaDon();
    });
    self.La_HDBan.subscribe(function () {
        $("#iconSort").remove();
        ResetColumnSort();
        self.currentPage(0);
        SearchHoaDon();
    });
    self.La_HDHoTro.subscribe(function () {
        $("#iconSort").remove();
        ResetColumnSort();
        self.currentPage(0);
        SearchHoaDon();
    });
    self.TT_HoanThanh.subscribe(function (newVal) {
        $("#iconSort").remove();
        ResetColumnSort();
        self.currentPage(0);
        SearchHoaDon();
    });
    self.TT_DaHuy.subscribe(function (newVal) {
        $("#iconSort").remove();
        ResetColumnSort();
        self.currentPage(0);
        SearchHoaDon();
    });
    self.TT_TamLuu.subscribe(function (newVal) {
        $("#iconSort").remove();
        ResetColumnSort();
        self.currentPage(0);
        SearchHoaDon();
    });
    self.BaoHiemCo.subscribe(function (newVal) {
        self.CalcBaoHiem();
        $("#iconSort").remove();
        ResetColumnSort();
        self.currentPage(0);
        SearchHoaDon();
    });
    self.BaoHiemKhong.subscribe(function (newVal) {
        self.CalcBaoHiem();
        $("#iconSort").remove();
        ResetColumnSort();
        self.currentPage(0);
        SearchHoaDon();
    });

    self.CalcBaoHiem = function () {
        let intBaoHiemCo = 1;
        let intBaoHiemKhong = 2;
        if (self.BaoHiemCo() === true) {
            intBaoHiemCo = 1;
        }
        else {
            intBaoHiemCo = 0;
        }
        if (self.BaoHiemKhong() === true) {
            intBaoHiemKhong = 2;
        }
        else {
            intBaoHiemKhong = 0;
        }
        self.BaoHiem(intBaoHiemCo + intBaoHiemKhong);
    }

    self.TT_DaDuyet.subscribe(function (newVal) {
        $("#iconSort").remove();
        ResetColumnSort();
        self.currentPage(0);
        SearchHoaDon();
    });
    self.TT_GiaoHang.subscribe(function (newVal) {
        $("#iconSort").remove();
        ResetColumnSort();
        self.currentPage(0);
        SearchHoaDon();
    });
    self.filterNgayLapHD.subscribe(function (newVal) {
        $("#iconSort").remove();
        ResetColumnSort();
        self.currentPage(0);
        SearchHoaDon();
    });
    self.PageResults_HoaDon = ko.computed(function () {
        var first = self.currentPage() * self.pageSize();
        if (self.HoaDons() !== null) {
            return self.HoaDons().slice(first, first + self.pageSize());
        }
    });
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
                                let obj = {
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
            SearchHoaDon();
        }
    };
    function SetCheck_Input() {
        // find in list and set check
        var countCheck = 0;
        $('#tb tr td.check-group input').each(function (x) {
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
        var ckHeader = $('#tb thead tr th:eq(0) input')
        if (countCheck === self.HoaDons().length) {
            ckHeader.prop('checked', true);
        }
        else {
            ckHeader.prop('checked', false);
        }
    }
    self.StartPage = function () {
        self.currentPage(0);
        SearchHoaDon();
    }
    self.BackPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
            SearchHoaDon();
        }
    }
    self.GoToNextPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.currentPage() + 1);
            SearchHoaDon();
        }
    }
    self.EndPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
            SearchHoaDon();
        }
    }
    //sort by cột trong bảng hóa đơn
    $('#tb thead tr').on('click', 'th', function () {
        var id = $(this).attr('class');
        if ($(this).hasClass('check-group')) {
            return;
        }
        switch (id) {
            case "mahoadon":
                self.columsort("MaHoaDon");
                break;
            case "ngaylaphoadon":
                self.columsort("NgayLapHoaDon");
                break;
            case "madathang":
                self.columsort("MaHoaDonGoc");
                break;
            case "makhachhang":
                self.columsort("MaKhachHang");
                break;
            case "tenkhachhang":
                self.columsort("TenKhachHang");
                break;
            case "email":
                self.columsort("Email");
                break;
            case "sodienthoai":
                self.columsort("SoDienThoai");
                break;
            case "diachi":
                self.columsort("DiaChi");
                break;
            case "khuvuc":
                self.columsort("KhuVuc");
                break;
            case "phuongxa":
                self.columsort("PhuongXa");
                break;
            case "nguoiban":
                self.columsort("NguoiBan");
                break;
            case "nguoitao":
                self.columsort("NguoiTao");
                break;
            case "ghichu":
                self.columsort("GhiChu");
                break;
            case "tongtienhang":
                self.columsort("TongTienHang");
                break;
            case "tonggiamgia":
                self.columsort("GiamGia");
                break;
            case "khachcantra":
                self.columsort("PhaiThanhToan");
                break;
            case "thanhtienchuack":
                self.columsort("ThanhTienChuaCK");
                break;
            case "giamgiact":
                self.columsort("GiamGiaCT");
                break;
            case "khachdatra":
                self.columsort("KhachDaTra");
                break;
            case "phitrahang":
                self.columsort("TongChiPhi");
                break;
            case "conno":
                self.columsort("ConNo");
                break;
            case "tienthue":
                self.columsort("VAT");
                break;
            case "tiendoidiem":
                self.columsort("TienDoiDiem");
                break;
            case "giatrisudung":
                self.columsort("GiaTriSDDV");
                break;
            case "tienmat":
                self.columsort("TienMat");
                break;
            case "chuyenkhoan":
                self.columsort("ChuyenKhoan");
                break;
            case "pos":
                self.columsort("TienATM");
                break;
            case "thegiatri":
                self.columsort("ThuTuThe");
                break;
            case "tiencoc":
                self.columsort("TienDatCoc");
                break;
            case "baohiemcantra":
                self.columsort("PhaiThanhToanBaoHiem");
                break;
            case "baohiemdatra":
                self.columsort("BaoHiemDaTra");
                break;
            case "tongtienBHduyet":
                self.columsort("TongTienBHDuyet");
                break;
            case "khautrutheovu":
                self.columsort("KhauTruTheoVu");
                break;
            case "giamtruboithuong":
                self.columsort("GiamTruBoiThuong");
                break;
            case "BHchitratruocVAT":
                self.columsort("BHThanhToanTruocThue");
                break;
            case "tongthueBH":
                self.columsort("TongTienThueBaoHiem");
                break;
        }
        SortGrid(id);
    });
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
        SearchHoaDon();
    };
    // Tra Hang
    self.gotoBanLe = function () {
        localStorage.setItem('fromTraHang', true);
        self.gotoGara();
    }

    self.headerTraHangs = [
        { title: 'Mã trả hàng', sortPropertyName: 'MaTraHang', asc: true, arrowDown: true, arrowUp: false, headerID: 'hdCodeTH' },
        { title: 'Mã hóa đơn', sortPropertyName: 'MaHoaDon', asc: true, arrowDown: false, arrowUp: false, headerID: 'hdCode' },
        { title: 'Thời gian', sortPropertyName: 'NgayLapHoaDon', asc: true, arrowDown: false, arrowUp: false, headerID: 'hdDate' },
        { title: 'Khách hàng', sortPropertyName: 'TenDoiTuong', asc: true, arrowDown: true, arrowUp: false, headerID: 'hdCusNam' },
        { title: 'Chi nhánh', sortPropertyName: '', asc: true, arrowDown: false, arrowUp: false, headerID: 'hdBranch' },
        { title: 'Người trả nhận', sortPropertyName: '', asc: true, arrowDown: false, arrowUp: false, headerID: 'hdPayer' },
        { title: 'Ghi chú', sortPropertyName: 'DienGiai', asc: true, arrowDown: true, arrowUp: false, headerID: 'hdNote' },
        { title: 'Tổng tiền hàng', sortPropertyName: 'TongTienHang', asc: true, arrowDown: false, arrowUp: false, headerID: 'hdSum' },
        { title: 'Giảm giá', sortPropertyName: 'TongGiamGia', asc: true, arrowDown: true, arrowUp: false, headerID: 'hdSale' }, // branch: chi nhanh
        { title: 'Tổng sau giảm giá', sortPropertyName: '', asc: true, arrowDown: false, arrowUp: false, headerID: 'hdSaleafter' },
        { title: 'Phí trả hàng', sortPropertyName: '', asc: true, arrowDown: true, arrowUp: false, headerID: 'hdFee' },
        { title: 'Cần trả khách', sortPropertyName: 'PhaiThanhToan', asc: true, arrowDown: false, arrowUp: false, headerID: 'hdSumPay' },
        { title: 'Đã trả khách', sortPropertyName: 'DaThanhToan', asc: true, arrowDown: false, arrowUp: false, headerID: 'hdPayed' },
        { title: 'Trạng thái', sortPropertyName: '', asc: true, arrowDown: false, arrowUp: false, headerID: 'hdStatus' },
    ];
    self.activeSortTH = self.headerTraHangs[1];
    self.sortHoaDon_TraHang = function (header, event) {
        if (self.activeSortTH === header) {
            header.asc = !header.asc;
            header.arrowDown = !header.arrowDown;
            header.arrowUp = !header.arrowUp;
        } else {
            self.activeSortTH.arrowDown = false;
            self.activeSortTH.arrowUp = false;
            self.activeSortTH = header;
            header.arrowDown = true;
        }
        if (header.arrowDown === true) {
            $('th[id^=hd]').each(function () {
                $(this).html($(this).text())
            });
            $('#' + header.headerID).html('');
            $('#' + header.headerID).html(header.title).append('<i class="fa fa-caret-down" aria-hidden="true"></i>');
        }
        else {
            $('th[id^=hd]').each(function () {
                $(this).html($(this).text())
            });
            $('#' + header.headerID).html('');
            $('#' + header.headerID).html(header.title).append('<i class="fa fa-caret-up" aria-hidden="true"></i>');
        }
        var prop = header.sortPropertyName;
        var ascSort = function (a, b) {
            if (typeof a[prop] === "number" || typeof a[prop] === "boolean") {
                return a[prop] < b[prop] ? -1 : a[prop] > b[prop] ? 1 : a[prop] === b[prop] ? 0 : 0;
            }
            else {
                if (a[prop] === null || a[prop] === undefined || b[prop] === null || b[prop] === undefined) {
                    if (a[prop] === null || b[prop] === null) {
                        // compare(null, string)= -1, compare(string, null)= 1
                        return (a[prop] === null && b[prop] !== null) ? -1 : (a[prop] !== null && b[prop] === null) ? 1 : 0;
                    }
                    else {
                        return a[prop] < b[prop] ? -1 : a[prop] > b[prop] ? 1 : a[prop] === b[prop] ? 0 : 0;
                    }
                }
                else {
                    return locdau(a[prop]) < locdau(b[prop]) ? -1 : locdau(a[prop]) > locdau(b[prop]) ? 1 : locdau(a[prop]) === locdau(b[prop]) ? 0 : 0;
                }
            }
        };
        var descSort = function (a, b) {
            if (typeof a[prop] === "number" || typeof a[prop] === "boolean") {
                return a[prop] > b[prop] ? -1 : a[prop] < b[prop] ? 1 : a[prop] === b[prop] ? 0 : 0;
            }
            else {
                if (a[prop] !== null && a[prop] !== undefined && b[prop] !== null && b[prop] !== undefined) {
                    return locdau(a[prop]) > locdau(b[prop]) ? -1 : locdau(a[prop]) < locdau(b[prop]) ? 1 : locdau(a[prop]) === locdau(b[prop]) ? 0 : 0;
                }
                else {
                    if (a[prop] === null || b[prop] === null) {
                        // compare(null, string)= 1, compare(string, null)= -1
                        return (a[prop] === null && b[prop] !== null) ? 1 : (a[prop] !== null && b[prop] === null) ? -1 : 0;
                    }
                    else {
                        return a[prop] > b[prop] ? -1 : a[prop] < b[prop] ? 1 : a[prop] === b[prop] ? 0 : 0;
                    }
                }
            }
        };
        var sortFunc = header.asc ? ascSort : descSort;
        self.HoaDons.sort(sortFunc);
    };
    self.gotoHoaDonGoc = function (item) {
        localStorage.setItem('FindHD', item.MaHoaDonGoc);
        switch (item.LoaiHoaDonGoc) {
            case 1:
                window.location.href = '/#/Invoices';
                break;
            case 2:
                window.location.href = '/#/HoaDonBaoHanh';
                break;
            case 19:
                window.location.href = '/#/ServicePackage';
                break;
            case 25:
                window.location.href = '/#/HoaDonSuaChua';
                break;
        }
    }
    self.gotoSoQuy = function (item) {
        localStorage.setItem('FindMaPhieuChi', item.MaPhieuChi);
        window.location.href = '/#/CashFlow';
    }
    self.gotoHoaDonTH = function (item) {
        switch (loaiHoaDon) {
            case 1:
                localStorage.setItem('FindHD', item.MaHoaDonGoc);
                window.location.href = '/#/Returns';
                break;
            case 6:
                // if click from HDDoiTra --> get MaHDparent
                localStorage.setItem('FindHD', self.MaHoaDonParent());
                window.location.href = '/#/Returns';
                break;
        }
    }
    self.gotoKhachHang = function (item, type) {
        switch (type) {
            case 1:
                localStorage.setItem('FindKhachHang', item.MaDoiTuong);
                window.location.href = '/#/Customers';
                break;
            case 2:
                if (!commonStatisJs.CheckNull(item.MaPhieuTiepNhan)) {
                    window.open('/#/DanhSachPhieuTiepNhan?' + item.MaPhieuTiepNhan, '_blank');
                }
                else {
                    self.LoadChiTietHD(item);
                }
                break;
            case 3:
                if (!commonStatisJs.CheckNull(item.BienSo)) {
                    window.open('/#/DanhSachXe?' + item.BienSo, '_blank');
                }
                else {
                    self.LoadChiTietHD(item);
                }
                break;
        }
    }
    //Trinhpv xuất excel HoaDon
    self.ColumnsExcel = ko.observableArray();
    self.addColum = function (item) {
        if (self.ColumnsExcel().length < 1) {
            self.ColumnsExcel.push(item);
        }
        else {
            for (let i = 0; i < self.ColumnsExcel().length; i++) {
                if (self.ColumnsExcel()[i] === item) {
                    self.ColumnsExcel.splice(i, 1);
                    break;
                }
                if (i === self.ColumnsExcel().length - 1) {
                    self.ColumnsExcel.push(item);
                    break;
                }
            }
        }
        self.ColumnsExcel.sort();
    }
    //===============================
    // triger khi đặt hàng thành công
    // đặt hàng
    //===============================
    $("body").on('ChangeDatHang', function () {
        SearchHoaDon();
    });
    //===============================
    // triger khi đặt hàng thành công
    // Hóa đơn
    //===============================
    $("body").on('ChangeHoaDon', function () {
        SearchHoaDon();
    });
    //===============================
    // triger khi đặt hàng thành công
    // TraHang
    //===============================
    $("body").on('ChangeTraHang', function () {
        SearchHoaDon();
    });
    var columnHide = null;
    self.loadColumnsHide = function () {
        //columnHide = null;
        //for (let i = 0; i < self.ColumnsExcel().length; i++) {
        //    if (i === 0) {
        //        columnHide = self.ColumnsExcel()[i];
        //    }
        //    else {
        //        columnHide = self.ColumnsExcel()[i] + "_" + columnHide;
        //    }
        //}
    }
    self.DownloadFileTeamplateXLSX = function (pathFile) {
        var url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + pathFile;
        window.location.href = url;
    }
    //xuất excel hóa đơn
    self.ExportMany_HD = function () {
        var arrDV = [];
        var sTenChiNhanhs = '';
        for (let i = 0; i < self.MangNhomDV().length; i++) {
            if ($.inArray(self.MangNhomDV()[i], arrDV) === -1) {
                arrDV.push(self.MangNhomDV()[i].ID);
                sTenChiNhanhs += self.MangNhomDV()[i].TenDonVi + ',';
            }
        }
        sTenChiNhanhs = sTenChiNhanhs.substr(0, sTenChiNhanhs.length - 1);

        ajaxHelper(BH_HoaDonUri + 'GetListHDbyIDs?lstID=' + arrIDCheck, 'POST', arrIDCheck).done(function (x) {
            if (x.res) {
                GetColumHide(1);
                let myData = {
                    LstExport: x.lstHD,
                    LoaiHoaDon: loaiHoaDon,
                    DayStart: dayStart_Excel,
                    DayEnd: dayEnd_Excel,
                    ColumnsHide: columnHide,
                    ChiNhanhs: sTenChiNhanhs,
                }

                ajaxHelper(BH_HoaDonUri + 'XuatFileHD_TongQuan', 'POST', myData).done(function (url) {
                    if (url !== "") {
                        self.DownloadFileTeamplateXLSX(url);
                        var objDiary = {
                            ID_NhanVien: _id_NhanVien,
                            ID_DonVi: id_donvi,
                            ChucNang: "Hóa đơn",
                            NoiDung: "Xuất file tổng quan danh sách hóa đơn ",
                            NoiDungChiTiet: "Xuất file danh sách hóa đơn gồm " + $.map(x.lstHD, function (x) { return x.MaHoaDon }).toString(),
                            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                        };
                        Insert_NhatKyThaoTac_1Param(objDiary)
                    }
                })
            }
        })
    }

    self.ExportExcel_HoaDon = function () {
        SearchHoaDon(true);
    }
    self.ExportExcel_ChiTietHoaDon = function (item) {
        //let table = $('#home_' + item.ID);
        //    TableToExcel.convert(table[0], { // html code may contain multiple tables so here we are refering to 1st table tag
        //        name: `export.xlsx`, // fileName you could use any name
        //        sheet: {
        //            name: 'Sheet 1' // sheetName
        //        }
        //    });
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: id_donvi,
            ChucNang: "Hóa đơn",
            NoiDung: "Xuất báo cáo hóa đơn chi tiết theo mã: " + item.MaHoaDon,
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        var myData = {};
        myData.objDiary = objDiary;
        $.ajax({
            url: DiaryUri + "post_NhatKySuDung",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myData,
            success: function (item) {
            },
            error: function (jqXHR, textStatus, errorThrown) {
                ShowMessage_Danger("Ghi nhật ký sử dụng thất bại");
            },
            complete: function () {
                var columnHide = '';
                if (self.ThietLap().LoHang === false) {
                    columnHide = '3'; // hide: LoHang
                }
                var url = BH_HoaDonUri + 'ExportExcel__ChiTietHoaDon?ID_HoaDon=' + item.ID + '&loaiHoaDon=' + loaiHoaDon + '&columHides=' + columnHide;
                window.location.href = url;
            }
        })
    }
    // xuất excel phiếu trả hàng
    self.ExportExcel_PhieuTraHang = function () {
        SearchHoaDon(true);
    }
    self.ExportExcel_ChiTietPhieuTraHang = function (item) {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: id_donvi,
            ChucNang: "Trả hàng",
            NoiDung: "Xuất báo cáo phiếu trả hàng chi tiết theo mã: " + item.MaHoaDon,
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        var myData = {};
        myData.objDiary = objDiary;
        $.ajax({
            url: DiaryUri + "post_NhatKySuDung",
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
                var url = BH_HoaDonUri + 'ExportExcel__ChiTietPhieuTraHang?ID_HoaDon=' + item.ID;
                window.location.href = url;
            }
        })
    }
    //xuất excel phiếu đặt hàng
    self.ExportExcel_DatHang = function () {
        SearchHoaDon(true);
    }
    self.ExportExcel_ChiTietPhieuDatHang = function (item) {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: id_donvi,
            ChucNang: "Đặt hàng",
            NoiDung: "Xuất báo cáo phiếu đặt hàng chi tiết theo mã: " + item.MaHoaDon,
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        var myData = {};
        myData.objDiary = objDiary;
        $.ajax({
            url: DiaryUri + "post_NhatKySuDung",
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
                var url = BH_HoaDonUri + 'ExportExcel_ChiTietPhieuDatHang?ID_HoaDon=' + item.ID;
                window.location.href = url;
            }
        })
    }
    // use enable/disable txtNgayLapHD, dropdown NVien
    self.ThayDoi_NgayLapHD = ko.observable(false);
    self.ThayDoi_NVienBan = ko.observable(false);
    self.RdoKhauTru = ko.observable(0);
    self.RdoCheTai = ko.observable(0);
    self.InVoiceChosing = ko.observable();
    self.LoadChiTietHD = function (item, e) {
        self.InVoiceChosing(item);
        self.Enable_NgayLapHD(item.ChoThanhToan === null || !VHeader.CheckKhoaSo(moment(item.NgayLapHoaDon).format('YYYY-MM-DD'), item.ID_DonVi));

        let ngaylapFormat = moment(item.NgayLapHoaDon).format('YYYY-MM-DD');
        let role = CheckQuyenExist('GiaoDich_ChoPhepSuaDoiChungTu_NeuKhacNgayHienTai');// bat buoc chay lai sau khi gan quyen o ben duoi
        let role2 = CheckQuyenExist('GiaoDich_ChoPhepHuyChungTu_NeuKhacNgayHienTai');// bat buoc chay lai sau khi gan quyen o ben duoi
        if (_nowFormat === ngaylapFormat) {// neu trung ngay: luon co quyen sua
            role = true;
            role2 = true;
        }
        self.Role_ChangeInvoice_ifOtherDate(role);
        self.Role_DeleteInvoice_ifOtherDate(role2);

        self.currentPage_CTHD(0);
        var congthucBH = item.CongThucBaoHiem;

        if (commonStatisJs.CheckNull(congthucBH)) {
            congthucBH = '0';
        }
        congthucBH = congthucBH.toString().split('');
        if (congthucBH.length > 1) {
            self.RdoKhauTru(parseInt(congthucBH[0]));
            self.RdoCheTai(parseInt(congthucBH[1]));
        }
        else {
            self.RdoKhauTru(0);
            self.RdoCheTai(0);
        }

        // reset tab & set default active tab 1
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
        // active tabcontent
        ulTab.next().children('.tab-pane').removeClass('active');
        ulTab.next().children('.tab-pane:eq(0)').addClass('active');
        self.NgayLapHD_Update(undefined);
        self.filterHangHoa_ChiTietHD(undefined);

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
        var $thiss = $(event.currentTarget).closest('tr').next().find('td').find('.op-object-detail').find('.loadcthd');
        var css = $(event.currentTarget).closest('tr').next().css('display');
        $(".op-js-tr-hide").hide();
        if (css === 'none') {
            $(event.currentTarget).closest('tr').next().toggle();
        }
        $thiss.gridLoader();
        self.BH_HoaDonChiTiets([]);

        ajaxHelper(BH_HoaDonUri + 'SP_GetChiTietHD_byIDHoaDon_ChietKhauNV?idHoaDon=' + item.ID, 'GET').done(function (data) {
            $thiss.gridLoader({ show: false });
            if (data !== null) {
                var sluong = 0;
                for (let i = 0; i < data.length; i++) {
                    sluong += data[i].SoLuong;
                    if (data[i].MaHangHoa.indexOf('{DEL}') > -1) {
                        data[i].MaHangHoa = data[i].MaHangHoa.substr(0, data[i].MaHangHoa.length - 5);
                        data[i].Del = '{Xóa}';
                    } else {
                        data[i].Del = "";
                    }
                    // USE FOR SPA (add BH_NhanVienThucHien) (only BanHang, not (DatHang + TraHang))
                    let lstNV_TH = '';
                    let lstNV_TV = '';
                    let listBH_NVienThucHienOld = data[i].BH_NhanVienThucHien;
                    // remove BH_NhanVienThucHien old, and add again
                    data[i].BH_NhanVienThucHien = [];
                    for (let j = 0; j < listBH_NVienThucHienOld.length; j++) {
                        let itemFor = listBH_NVienThucHienOld[j];
                        // addNvienTuVan_ThucHien
                        let isNVThucHien = itemFor.ThucHien_TuVan;
                        let tienCK = itemFor.TienChietKhau;
                        let gtriPtramCK = itemFor.PT_ChietKhau;
                        let isPTram = gtriPtramCK > 0 ? true : false;
                        let gtriCK_TH = 0;
                        let gtriCK_TV = 0;
                        let tacVu = 1;
                        if (isNVThucHien) {
                            if (isPTram) {
                                gtriCK_TH = gtriPtramCK;
                                lstNV_TH += itemFor.TenNhanVien + ', ';
                            }
                            else {
                                gtriCK_TH = tienCK;
                                lstNV_TH += itemFor.TenNhanVien + ', ';
                            }
                        }
                        else {
                            tacVu = 2;
                            if (isPTram) {
                                gtriCK_TV = gtriPtramCK;
                                lstNV_TV += itemFor.TenNhanVien + ', ';
                            }
                            else {
                                gtriCK_TV = tienCK;
                                lstNV_TV += itemFor.TenNhanVien + ', ';
                            }
                        }
                        // add to do fun SaoChep
                        var idRandom = CreateIDRandom('CKNV_');
                        var itemNV = {
                            IDRandom: idRandom,
                            ID_NhanVien: itemFor.ID_NhanVien,
                            TenNhanVien: itemFor.TenNhanVien,
                            ThucHien_TuVan: isNVThucHien,
                            TacVu: tacVu,
                            TienChietKhau: tienCK,
                            TheoYeuCau: itemFor.TheoYeuCau,
                            PT_ChietKhau: gtriPtramCK,
                            HeSo: itemFor.HeSo,
                            TinhChietKhauTheo: itemFor.TinhChietKhauTheo,
                            TinhHoaHongTruocCK: itemFor.TinhHoaHongTruocCK,
                        }
                        data[i].BH_NhanVienThucHien.push(itemNV);
                    }

                    data[i].GhiChu_NVThucHien = (lstNV_TH === '' ? '' : '- Thực hiện: ' + Remove_LastComma(lstNV_TH));
                    data[i].GhiChu_NVTuVan = (lstNV_TV === '' ? '' : '- Tư vấn: ' + Remove_LastComma(lstNV_TV));
                    data[i].GhiChu_NVThucHienPrint = (lstNV_TH === '' ? '' : Remove_LastComma(lstNV_TH));
                    data[i].GhiChu_NVTuVanPrint = (lstNV_TV === '' ? '' : Remove_LastComma(lstNV_TV));
                }
                self.BH_HoaDonChiTiets(data);
                self.TongSLuong(sluong);

                var tongsoluong = self.BH_HoaDonChiTiets().reduce(function (x, item) {
                    return x + item.SoLuong;
                }, 0);
                var tonggiamgiahang = self.BH_HoaDonChiTiets().reduce(function (x, item) {
                    return x + (item.TienChietKhau * item.SoLuong);
                }, 0);
                var tongtienhangchuaCK = self.BH_HoaDonChiTiets().reduce(function (x, item) {
                    return x + (item.SoLuong * item.DonGia);
                }, 0);
                var arrHH = self.BH_HoaDonChiTiets().filter(x => x.LaHangHoa);
                var arrDV = self.BH_HoaDonChiTiets().filter(x => x.LaHangHoa === false);

                var tongDV = 0, tongDV_truocVAT = 0, tongDV_truocCK = 0;
                var tongHH = 0, tongHH_truocVAT = 0, tongHH_truocCK = 0;
                var DV_tongthue = 0, DV_tongCK = 0, DV_tongSL = 0;
                var HH_tongthue = 0, HH_tongCK = 0, HH_tongSL = 0;
                for (let k = 0; k < arrHH.length; k++) {
                    let itFor = arrHH[k];
                    let soluong = formatNumberToFloat(itFor.SoLuong);
                    HH_tongSL += soluong;
                    tongHH += formatNumberToFloat(itFor.ThanhToan);
                    tongHH_truocVAT += RoundDecimal(formatNumberToFloat(itFor.ThanhTien), 3);
                    tongHH_truocCK += soluong * formatNumberToFloat(itFor.DonGia);
                    HH_tongthue += soluong * formatNumberToFloat(itFor.TienThue);
                    HH_tongCK += soluong * formatNumberToFloat(itFor.TienChietKhau);
                }
                for (let k = 0; k < arrDV.length; k++) {
                    let itFor = arrDV[k];
                    let soluong = formatNumberToFloat(itFor.SoLuong);
                    DV_tongSL += soluong;
                    tongDV += formatNumberToFloat(itFor.ThanhToan);
                    tongDV_truocVAT += RoundDecimal(formatNumberToFloat(itFor.ThanhTien), 3);
                    tongDV_truocCK += soluong * formatNumberToFloat(itFor.DonGia);
                    DV_tongthue += soluong * formatNumberToFloat(itFor.TienThue);
                    DV_tongCK += soluong * formatNumberToFloat(itFor.TienChietKhau);
                }

                self.TongSoLuongHang(tongsoluong);
                self.TongGiamGiaHang(tonggiamgiahang);
                self.TongTienHangChuaCK(RoundDecimal(tongtienhangchuaCK, 3));

                self.TongTienPhuTung(tongHH);
                self.TongTienDichVu(tongDV);
                self.TongTienPhuTung_TruocCK(tongHH_truocCK);
                self.TongTienDichVu_TruocCK(tongDV_truocCK);
                self.TongTienPhuTung_TruocVAT(tongHH_truocVAT);
                self.TongTienDichVu_TruocVAT(tongDV_truocVAT);

                self.TongThue_PhuTung(HH_tongthue);
                self.TongCK_PhuTung(HH_tongCK);
                self.TongThue_DichVu(DV_tongthue);
                self.TongCK_DichVu(DV_tongCK);
                self.TongSL_DichVu(DV_tongSL);
                self.TongSL_PhuTung(HH_tongSL);
            }
            else {
                self.BH_HoaDonChiTiets(data);
            }
        });
        var roleInsertQuy = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen.indexOf('SoQuy_ThemMoi') > -1;
        });

        vmThanhPhanCombo.GetAllCombo_byIDHoaDon(item.ID);
        vmThanhToan.GetSoDuTheGiaTri(item.ID_DoiTuong);// used to get print

        switch (loaiHoaDon) {
            case 0:
            case 1:
            case 2:
            case 25:
                // chietkhau NV --> use when SaoChep/MoPhieu
                if (navigator.onLine) {
                    ajaxHelper(BH_HoaDonUri + 'GetChietKhauNV_byIDHoaDon?idHoaDon=' + item.ID, 'GET').done(function (x) {
                        if (x.res === true) {
                            item.BH_NhanVienThucHiens = x.data;
                        }
                    });
                }
                VueChiPhi.CTHD_GetChiPhiDichVu([item.ID]);

                break;
            case 3:
                GetLichSuThanhToan_ofDatHang(item.ID);

                var roleXuLiDH = $.grep(self.Quyen_NguoiDung(), function (x) {
                    return x.MaQuyen.indexOf('DatHang_TaoHoaDon') > -1;
                });
                var roleInsert_Invoice = $.grep(self.Quyen_NguoiDung(), function (x) {
                    return x.MaQuyen.indexOf('HoaDon_ThemMoi') > -1;
                });
                if (roleInsert_Invoice.length > 0 && roleXuLiDH.length > 0) {
                    if (item.YeuCau === '1' || item.YeuCau === '2' || item.YeuCau === '') {
                        self.Show_BtnXulyDH(true);
                    }
                    else {
                        self.Show_BtnXulyDH(false);
                    }
                }
                break;
            case 6:
                self.MaHoaDonParent(item.MaHoaDon); // get MaHoaDon Parent --> go to gotoHoaDonTH (go to itself)
                GetLichSuThanhToan(item.ID, null);
                break;
        }
        GetInfor_PhieuTiepNhan(item.ID_PhieuTiepNhan);
        GetInforKhachHangFromDB_ByID(item.ID_DoiTuong, 1);
        GetInforKhachHangFromDB_ByID(item.ID_BaoHiem, 3);
    }

    function CheckQuyen_HoaDonMua() {
        self.RoleUpdate_Invoice(CheckQuyenExist('HoaDon_CapNhat'));
        self.ThayDoi_NgayLapHD(CheckQuyenExist('HoaDon_ThayDoiThoiGian'));
        self.ThayDoi_NVienBan(CheckQuyenExist('HoaDon_ThayDoiNhanVien'));
        self.RoleExport_Invoice(CheckQuyenExist('HoaDon_XuatFile'));
        self.RoleDelete_Invoice(CheckQuyenExist('HoaDon_Xoa'));
        self.Show_BtnExcelDetail(self.RoleExport_Invoice());
    }

    self.GetLichSuThanhToan = function (item) {
        // neu hdDoiTra (tra > mua): khong get lichsu thanhtoan
        if (item.PhaiThanhToan !== 0 || item.TongThanhToan !== 0) {
            GetLichSuThanhToan(item.ID, item.ID_HoaDon);
        }
        else {
            self.LichSuThanhToan([]);
        }
    }
    self.GetLichSuTraHang = function () {
        GetLichSuTraHang(item.ID);
    }

    self.Enable_NgayLapHD = ko.observable(true);

    function CheckNgayLapHD_format(valDate, idDonVi = null) {
        if (idDonVi === null) {
            idDonVi = VHeader.IdDonVi;
        }
        var dateNow = moment(new Date()).format('YYYY-MM-DD HH:mm');
        var ngayLapHD = moment(valDate, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
        if (valDate === '') {
            ShowMessage_Danger("Vui lòng nhập ngày lập " + sLoai);
            return false;
        }
        if (valDate.indexOf('_') > -1) {
            ShowMessage_Danger("Ngày lập " + sLoai + ' chưa đúng định dạng');
            return false;
        }
        if (ngayLapHD > dateNow) {
            ShowMessage_Danger("Ngày lập " + sLoai + ' vượt quá thời gian hiện tại');
            return false;
        }
        let chotSo = VHeader.CheckKhoaSo(moment(ngayLapHD, 'YYYY-MM-DD HH:mm').format('YYYY-MM-DD'), idDonVi);
        if (chotSo) {
            ShowMessage_Danger(VHeader.warning.ChotSo.Update);
            return false;
        }
        return true;
    }

    function GetLichSuThanhToan(idHoaDon, idParent) {
        // load data from Quy_HoaDon
        ajaxHelper(Quy_HoaDonUri + 'GetQuyHoaDon_byIDHoaDon?idHoaDon=' + idHoaDon + '&idHoaDonParent=' + idParent, 'GET').done(function (data) {
            self.LichSuThanhToan(data);
        });
    }
    function GetLichSuTraHang(idHoaDon) {
        ajaxHelper(BH_HoaDonUri + 'GetHDTraHang_byIDHoaDon?idHoaDon=' + idHoaDon, 'GET').done(function (data) {
            self.LichSuTraHang(data); // = L/su HoaDon of HD Dathang
        });
    }
    function GetLichSuThanhToan_ofDatHang(id) {
        ajaxHelper(Quy_HoaDonUri + 'GetLichSuThanhToan_ofDatHang?id=' + id, 'GET').done(function (data) {
            self.LichSuThanhToanDH(data);
        });
    }
    function GetInfor_ofHDDoiTra(item) {
        // reset HoaDonDoiTra: if data null
        self.HoaDonDoiTra([]);
        ajaxHelper(BH_HoaDonUri + 'GetHoaDon_ByID?id=' + item.ID, 'GET').done(function (data) {
            // show hdDoiTra if chua huy
            if (data !== null && data.ChoThanhToan !== null) {
                var cthd = data.BH_HoaDon_ChiTiet;
                // remove tp dinh luong in CTHD
                data.BH_HoaDon_ChiTiet = $.grep(cthd, function (x) {
                    return x.ID_ChiTietDinhLuong === x.ID || x.ID_ChiTietDinhLuong === null;
                })
                self.HoaDonDoiTra(data);

                vmThanhPhanCombo.GetAllCombo_byIDHoaDon(data.ID);
            }
            else {
                self.HoaDonDoiTra([]);
            }
        });
    }
    self.VisibleHisTraHang = ko.computed(function () {
        if (self.LichSuTraHang() === null) {
            return false;
        }
        else {
            return self.LichSuTraHang().length > 0;
        }
    })
    self.VisibleHis_HisHDofDH = ko.computed(function () {
        if (self.LichSuThanhToanDH() === null) {
            return false;
        }
        else {
            var count = 0;
            for (let i = 0; i < self.LichSuThanhToanDH().length; i++) {
                if (self.LichSuThanhToanDH()[i].LoaiHoaDon === 1) {
                    // Da tao HD from Dat hang
                    count += 1;
                }
            }
            if (count === 0) {
                return false;
            }
            else {
                return true;
            }
        }
    })
    self.VisibleHuyHD = ko.computed(function () {
        // if have TraHang --> hide btnHuyHoaDon
        if (self.LichSuTraHang() === null) {
            return true;
        }
        else {
            return false;
        }
    })
    self.VisibleHDDoiTra = ko.computed(function () {
        if (self.HoaDonDoiTra() === null) {
            return false;
        }
        else {
            return true;
        }
    })
    self.arrFilterBangGia = ko.computed(function () {
        var _filter = self.filterBangGia();
        return arrFilter = ko.utils.arrayFilter(self.GiaBans(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenGiaBan).split(/\s+/);
            var sSearch = '';
            for (let i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }
            if (chon && _filter) {
                chon = (locdau(prod.TenGiaBan).indexOf(locdau(_filter)) >= 0 ||
                    sSearch.indexOf(locdau(_filter)) >= 0
                );
            }
            return chon;
        });
    });
    self.arrFilterPhongBan = ko.computed(function () {
        var _filter = self.filterPhongBan();
        return arrFilter = ko.utils.arrayFilter(self.PhongBans(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenViTri).split(/\s+/);
            var sSearch = '';
            for (let i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }
            if (chon && _filter) {
                chon = (locdau(prod.TenViTri).indexOf(locdau(_filter)) >= 0 ||
                    sSearch.indexOf(locdau(_filter)) >= 0
                );
            }
            return chon;
        });
    });

    self.PrintMany = function () {
        let arHD = [];
        ajaxHelper(BH_HoaDonUri + 'GetListHDbyIDs?lstID=' + arrIDCheck, 'POST', arrIDCheck).done(function (x) {
            if (x.res) {
                let lstHD = x.lstHD;
                let lstCTHD = x.lstCTHD;

                for (let i = 0; i < lstHD.length; i++) {
                    let forHD = lstHD[i];
                    let arrCT = $.grep(lstCTHD, function (x) {
                        return x.ID_HoaDon === forHD.ID;
                    });

                    if (arrCT.length > 0) {
                        let objHD = GetInforHDPrint(forHD);
                        let cthdPrint = GetCTHDPrint_Format(arrCT);
                        objHD.BH_HoaDon_ChiTiet = cthdPrint;
                        objHD.CTHoaDonPrintMH = [];
                        arHD.push(objHD);
                    }
                }

                $.ajax({
                    url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=HDBL&idDonVi='
                        + VHeader.IdDonVi + '&printMultiple=true',
                    type: 'GET',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (result) {
                        let data = result;
                        data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                        data = data.concat(`<script> function formatNumber(number, decimalDot = 2) {
                                            if (number === undefined || number === null) {
                                                return 0;
                                            }
                                            else {
                                                number = formatNumberToFloat(number);
                                                number = Math.round(number * Math.pow(10, decimalDot)) / Math.pow(10, decimalDot);
                                                if (number !== null) {
                                                    let lastone = number.toString().split('').pop();
                                                    if (lastone !== '.') {
                                                        number = parseFloat(number);
                                                    }
                                                }
                                                if (isNaN(number)) {
                                                    number = 0;
                                                }
                                                let xxxx=number.toString().replace(/\\B(?=(\\d{3})+(?!\\d))/g, ',');
                                                return xxxx;
                                            }
                                        }
                                    function formatNumberToFloat(objVal) {
                                            let value = parseFloat(objVal.toString().replace(/,/g, ''));
                                            if (isNaN(value)) {
                                                return 0;
                                            }
                                            else {
                                             
                                                return value;
                                            }
                                       
                                    } </script>`);
                        data = data.concat("<script > var item1=" + JSON.stringify(arHD) + "; </script>");
                        data = data.concat('<script> var dataMauIn = function () {' +
                            'var self = this;' +
                            'self.ListHoaDon_ChiTietHoaDonPrint = ko.observableArray(item1);' +
                            'self.Count_ListHoaDons = ko.computed(function () { ' +
                            'return self.ListHoaDon_ChiTietHoaDonPrint().length;' +
                            '})' +
                            '};' +
                            'ko.applyBindings(new dataMauIn()) </script>');
                        PrintExtraReport_Multiple(data);
                    }
                });
            }
        })
    }

    self.PrintMerger = async function () {
        var idCus = null, idBaoHiem = null;
        if (arrIDCheck.length > 0) {
            // find hd by id --> get customer
            let hd = $.grep(self.HoaDons(), function (x) {
                return $.inArray(x.ID, arrIDCheck) > -1;
            });
            if (hd.length > 0) {
                idCus = hd[0].ID_DoiTuong;
                idBaoHiem = hd[0].ID_BaoHiem;
            }
        }

        var cus = await GetInforCus(idCus);
        var baohiem = await GetInforCus(idBaoHiem);

        ajaxHelper(BH_HoaDonUri + 'GetListHDbyIDs?lstID=' + arrIDCheck, 'POST', arrIDCheck).done(function (x) {
            if (x.res) {
                // khac PTN --> canh bao
                if ($.unique($.map(x.lstHD, function (x) { return x.ID_PhieuTiepNhan })).length > 1) {
                    commonStatisJs.ShowMessageDanger('Không in gộp hóa đơn nếu khác phiếu tiếp nhận');
                    return;
                }

                let lstHD = x.lstHD;
                let lstCTHD = x.lstCTHD;

                let multipleHD = lstHD.length > 1;
                if (lstHD.length > 0) {
                    let itFirstHD = lstHD[0];

                    let tn = {};
                    $.getJSON('/api/DanhMuc/GaraAPI/' + 'PhieuTiepNhan_GetThongTinChiTiet?id=' + itFirstHD.ID_PhieuTiepNhan).done(function (x) {
                        if (x.res && x.dataSoure.length > 0) {
                            tn = x.dataSoure[0];
                        }

                        let arrID_Khach = [], arrID_NhanVien = [], arrID_BaoHiem = [];
                        let maHD = '', maBG = '', nguoitaoHD = '', ngaylapHD = '', diengiai = '',
                            maKhachs = '', tenKhachs = '', tenNVienBans = '', maBaoHiems = '', tenBaoHiems = '';
                        let diemgiaodich = 0, tongthueKH = 0;

                        for (let i = 0; i < lstHD.length; i++) {
                            let forHD = lstHD[i];

                            maHD += forHD.MaHoaDon + ', ';
                            maBG += forHD.MaHoaDonGoc + ', ';
                            diengiai += forHD.DienGiai + ' <br /> ';
                            ngaylapHD += moment(forHD.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss') + ', ';
                            nguoitaoHD = forHD.NguoiTaoHD;

                            if ($.inArray(forHD.ID_DoiTuong, arrID_Khach) === -1) {
                                arrID_Khach.push(forHD.ID_DoiTuong);
                                maKhachs += forHD.MaDoiTuong + ', ';
                                tenKhachs += forHD.TenDoiTuong + ', ';
                            }

                            if ($.inArray(forHD.ID_BaoHiem, arrID_BaoHiem) === -1) {
                                arrID_BaoHiem.push(forHD.ID_BaoHiem);
                                maBaoHiems += forHD.MaBaoHiem + ', ';
                                tenBaoHiems += forHD.TenBaoHiem + ', ';
                            }

                            if ($.inArray(forHD.ID_NhanVien, arrID_NhanVien) === -1) {
                                arrID_NhanVien.push(forHD.ID_NhanVien);
                                tenNVienBans += forHD.TenNhanVien + ', ';
                            }

                            diemgiaodich += forHD.DiemGiaoDich;
                            tongthueKH += forHD.TongThueKhachHang;
                        }

                        let objPrint = {
                            MaHoaDon: Remove_LastComma(maHD),
                            MaHoaDonTraHang: Remove_LastComma(maBG),
                            NgayLapHoaDon: Remove_LastComma(ngaylapHD),
                            Ngay: moment(itFirstHD.NgayLapHoaDon).format('DD'),
                            Thang: moment(itFirstHD.NgayLapHoaDon).format('MM'),
                            Nam: moment(itFirstHD.NgayLapHoaDon).format('YYYY'),

                            LoaiHoaDon: itFirstHD.LoaiHoaDon,
                            NguoiTao: nguoitaoHD,
                            DienGiai: diengiai,
                            NhanVienBanHang: tenNVienBans,
                            TenGiaBan: 'Bảng giá chuẩn',

                            ChoThanhToan: false,
                            NgayApDungGoiDV: null,
                            HanSuDungGoiDV: null,
                            DiemGiaoDich: diemgiaodich,
                            TongChiPhi: itFirstHD.SumTongChiPhi,

                            TongTienHang: itFirstHD.SumTongChiPhi,
                            PhaiThanhToan: itFirstHD.SumPhaiThanhToan,
                            TongGiamGiaKM_HD: itFirstHD.SumTongGiamGia,
                            TongGiamGia: itFirstHD.SumTongGiamGia,
                            DaThanhToan: itFirstHD.SumDaThanhToan,
                            KhachDaTra: itFirstHD.SumKhachDaTra,
                            BaoHiemDaTra: itFirstHD.SumBaoHiemDaTra,
                            TongTienThue: itFirstHD.SumTongTienThue,

                            TongTienThueBaoHiem: itFirstHD.SumTongTienThueBaoHiem,
                            PhaiThanhToanBaoHiem: itFirstHD.SumPhaiThanhToanBaoHiem,
                            TongThanhToan: itFirstHD.SumTongThanhToan,
                            KhauTruTheoVu: itFirstHD.SumKhauTruTheoVu,
                            GiamTruBoiThuong: 0,
                            TongTienThueBaoHiem: itFirstHD.SumTongTienThueBaoHiem,
                            BHThanhToanTruocThue: itFirstHD.SumBHThanhToanTruocThue,
                            TongTienBHDuyet: itFirstHD.SumTongTienBHDuyet,
                            TongThueKhachHang: tongthueKH,
                            TongCong: itFirstHD.SumTongThanhToan,

                            PTThueHoaDon: multipleHD ? 0 : itFirstHD.PTThueHoaDon,
                            SoVuBaoHiem: multipleHD ? 0 : itFirstHD.SoVuBaoHiem,
                            PTThueBaoHiem: multipleHD ? 0 : itFirstHD.PTThueBaoHiem,
                            PTGiamTruBoiThuong: multipleHD ? 0 : itFirstHD.PTGiamTruBoiThuong,
                            TongChietKhau: multipleHD ? 0 : itFirstHD.TongChietKhau,

                            CongThucBaoHiem: 0,
                            PTThueKhachHang: 0,
                            GiamTruThanhToanBaoHiem: 0,
                            KhuyeMai_GiamGia: 0,
                            TongTienTra: 0,
                            TongTienMua: 0,
                            TongGiaGocHangTra: 0,
                            TongChiPhiHangTra: 0,
                            HoanTraThuKhac: 0,
                            DaTraKhach: 0,
                            PhaiTraKhach: 0,
                            DiemQuyDoi: 0,
                            TienThua: 0,

                            TienMat: itFirstHD.SumTienMat,
                            TienATM: itFirstHD.SumPOS,
                            TienGui: itFirstHD.SumChuyenKhoan,
                            TienTheGiaTri: itFirstHD.SumThuTuThe,
                            TTBangDiem: itFirstHD.SumTienDoiDiem,
                            MaDoiTuong: Remove_LastComma(maKhachs),
                            TenDoiTuong: Remove_LastComma(tenKhachs),
                            DienThoaiKhachHang: itFirstHD.DienThoai,
                            DiaChiKhachHang: itFirstHD.DiaChiKhachHang,
                            MaPhieuTiepNhan: '',
                            TenBaoHiem: Remove_LastComma(tenBaoHiems),
                            MaBaoHiem: Remove_LastComma(maBaoHiems),

                            NgayVaoXuong: '',
                            NgayXuatXuongDuKien: '',
                            LienHeBaoHiem: '',
                            SoDienThoaiLienHeBaoHiem: '',
                            PTN_GhiChu: '',
                            CoVan_SDT: '',
                            CoVanDichVu: '',
                            NhanVienTiepNhan: '',
                            SoKhung: '',
                            SoMay: '',
                            HopSo: '',
                            DungTich: '',
                            NamSanXuat: '',
                            MauSon: '',
                            TenLoaiXe: '',
                            TenMauXe: '',
                            TenHangXe: '',
                            ChuXe: '',
                            MaSoThue: '',
                            TaiKhoanNganHang: '',

                            BH_SDT: '',
                            BH_Email: '',
                            BH_DiaChi: '',
                            BH_TenLienHe: '',
                            BH_SDTLienHe: '',

                            ChiPhi_GhiChu: '',
                            PTChietKhauHH: 0,
                            TongGiamGiaHang: 0,
                            TongTienHangChuaCK: 0,
                            TongTienKhuyenMai_CT: 0,
                            TongGiamGiaKhuyenMai_CT: 0,
                            SoDuDatCoc: 0,
                            NoTruoc: 0,
                            BH_NoTruoc: 0,
                            BH_NoSau: 0,
                            HD_ConThieu: itFirstHD.SumConNo,
                            TienKhachThieu: itFirstHD.SumPhaiThanhToan - itFirstHD.SumKhachDaTra,
                            BH_ConThieu: itFirstHD.SumPhaiThanhToanBaoHiem - itFirstHD.SumBaoHiemDaTra,
                        }

                        let notruoc = 0, nosau = 0;
                        if (cus) {
                            nosau = cus.NoHienTai;
                            notruoc = nosau - objPrint.TienKhachThieu;
                            notruoc = notruoc < 0 ? 0 : notruoc;
                        }
                        else {
                            nosau = itFirstHD.SumConNo;// khachle
                        }
                        objPrint.NoTruoc = formatNumber(notruoc);
                        objPrint.NoSau = formatNumber(nosau);

                        let pthuc = '';
                        if (itFirstHD.SumTienMat > 0) {
                            pthuc += 'Tiền mặt, ';
                        }
                        if (itFirstHD.SumPOS > 0) {
                            pthuc += 'POS, ';
                        }
                        if (itFirstHD.SumChuyenKhoan > 0) {
                            pthuc += 'Chuyển khoản, ';
                        }
                        if (itFirstHD.SumThuTuThe > 0) {
                            pthuc += 'Thẻ giá trị, ';
                        }
                        if (itFirstHD.SumTienDoiDiem > 0) {
                            pthuc += 'Điểm, ';
                        }
                        if (itFirstHD.SumTienCoc > 0) {
                            pthuc += 'Tiền cọc, ';
                        }

                        objPrint.PhuongThucTT = Remove_LastComma(pthuc);
                        objPrint.TienBangChu = DocSo(objPrint.TongCong);
                        objPrint.KH_TienBangChu = DocSo(itFirstHD.SumKhachDaTra);
                        objPrint.BH_TienBangChu = DocSo(itFirstHD.SumBaoHiemDaTra);

                        if (tn) {
                            objPrint.MaPhieuTiepNhan = tn.MaPhieuTiepNhan;
                            objPrint.BienSo = tn.BienSo;
                            objPrint.ChuXe = tn.ChuXe;
                            objPrint.ChuXe_DiaChi = tn.ChuXe_DiaChi;
                            objPrint.ChuXe_Email = tn.ChuXe_Email;
                            objPrint.ChuXe_SDT = tn.ChuXe_SDT;
                            objPrint.CoVanDichVu = tn.CoVanDichVu;
                            objPrint.CoVan_SDT = tn.CoVan_SDT;
                            objPrint.DungTich = tn.DungTich;
                            objPrint.PTN_GhiChu = tn.GhiChu;
                            objPrint.HopSo = tn.HopSo;
                            objPrint.NhanVienTiepNhan = tn.NhanVienTiepNhan;
                            objPrint.MauSon = tn.MauSon;
                            objPrint.NamSanXuat = tn.NamSanXuat;
                            objPrint.NgayVaoXuong = moment(tn.NgayVaoXuong).format('DD/MM/YYYY HH:mm');
                            objPrint.NgayXuatXuong = tn.NgayXuatXuong ? moment(tn.NgayXuatXuong).format('DD/MM/YYYY HH:mm') : '';
                            objPrint.NgayXuatXuongDuKien = tn.NgayXuatXuongDuKien ? moment(tn.NgayXuatXuongDuKien).format('DD/MM/YYYY HH:mm') : '';
                            objPrint.SoDienThoaiLienHe = tn.SoDienThoaiLienHe;
                            objPrint.SoKhung = tn.SoKhung;
                            objPrint.SoMay = tn.SoMay;
                            objPrint.SoKmRa = tn.SoKmRa;
                            objPrint.SoKmVao = tn.SoKmVao;
                            objPrint.TenHangXe = tn.TenHangXe;
                            objPrint.TenLoaiXe = tn.TenLoaiXe;
                            objPrint.TenMauXe = tn.TenMauXe;
                            objPrint.TenLienHe = tn.TenLienHe;
                        }

                        let sumSoLuong = 0, sumGiamGiaHang = 0, tongtienhang_truocCK = 0;
                        let cthdPrint = [];
                        for (let j = 0; j < lstCTHD.length; j++) {
                            let ctFor = $.extend({}, true, lstCTHD[j]);
                            ctFor.SoThuTu = j + 1;
                            ctFor.BH_ThanhTien = ctFor.SoLuong * ctFor.DonGiaBaoHiem;
                            ctFor.HH_ThueTong = ctFor.SoLuong * ctFor.TienThue;
                            ctFor.TongChietKhau = ctFor.SoLuong * ctFor.GiamGia;
                            ctFor.ThanhTienTruocCK = ctFor.SoLuong * ctFor.DonGia;
                            cthdPrint.push(ctFor);

                            sumSoLuong += ctFor.SoLuong;
                            sumGiamGiaHang += ctFor.TongChietKhau;
                            tongtienhang_truocCK += ctFor.ThanhTienTruocCK;
                        }
                        objPrint.TongGiamGiaHang = sumGiamGiaHang;
                        objPrint.TongTienHangChuaCK = tongtienhang_truocCK;
                        objPrint.TongSoLuongHang = sumSoLuong;
                        objPrint.TongGiamGiaHD_HH = sumGiamGiaHang + itFirstHD.SumTongGiamGia;

                        let arrHH = lstCTHD.filter(x => x.LaHangHoa);
                        let arrDV = lstCTHD.filter(x => x.LaHangHoa === false);

                        let tongDV = 0, tongDV_truocVAT = 0, tongDV_truocCK = 0;
                        let tongHH = 0, tongHH_truocVAT = 0, tongHH_truocCK = 0;
                        let DV_tongthue = 0, DV_tongCK = 0, DV_tongSL = 0;
                        let HH_tongthue = 0, HH_tongCK = 0, HH_tongSL = 0;
                        for (let k = 0; k < arrHH.length; k++) {
                            let itFor = arrHH[k];
                            let soluong = formatNumberToFloat(itFor.SoLuong);
                            HH_tongSL += soluong;
                            tongHH += formatNumberToFloat(itFor.ThanhToan);
                            tongHH_truocVAT += formatNumberToFloat(itFor.ThanhTien);
                            tongHH_truocCK += soluong * formatNumberToFloat(itFor.DonGia);
                            HH_tongthue += soluong * formatNumberToFloat(itFor.TienThue);
                            HH_tongCK += soluong * formatNumberToFloat(itFor.TienChietKhau);
                        }
                        for (let k = 0; k < arrDV.length; k++) {
                            let itFor = arrDV[k];
                            let soluong = formatNumberToFloat(itFor.SoLuong);
                            DV_tongSL += soluong;
                            tongDV += formatNumberToFloat(itFor.ThanhToan);
                            tongDV_truocVAT += formatNumberToFloat(itFor.ThanhTien);
                            tongDV_truocCK += soluong * formatNumberToFloat(itFor.DonGia);
                            DV_tongthue += soluong * formatNumberToFloat(itFor.TienThue);
                            DV_tongCK += soluong * formatNumberToFloat(itFor.TienChietKhau);
                        }

                        objPrint.TongSL_DichVu = DV_tongSL;
                        objPrint.TongTienDichVu = tongDV;
                        objPrint.TongThue_DichVu = DV_tongthue;
                        objPrint.TongCK_DichVu = DV_tongCK;
                        objPrint.TongTienDichVu_TruocCK = tongDV_truocCK;
                        objPrint.TongTienDichVu_TruocVAT = tongDV_truocVAT;

                        objPrint.TongSL_PhuTung = HH_tongSL;
                        objPrint.TongTienPhuTung = tongHH;
                        objPrint.TongThue_PhuTung = HH_tongthue;
                        objPrint.TongCK_PhuTung = HH_tongCK;
                        objPrint.TongTienPhuTung_TruocCK = tongHH_truocCK;
                        objPrint.TongTienPhuTung_TruocVAT = tongHH_truocVAT;

                        $.ajax({
                            url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=HDBL&idDonVi=' + VHeader.IdDonVi,
                            type: 'GET',
                            dataType: 'json',
                            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                            success: function (result) {
                                let data = result;
                                data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                                data = data.concat("<script > var item1=", JSON.stringify(cthdPrint)
                                    , "; var item2= [] "
                                    , "; var item3=", JSON.stringify(objPrint)
                                    , "; var item4 =", JSON.stringify(self.HangMucSuaChuas())
                                    , "; var item5 =", JSON.stringify(self.VatDungKemTheos())
                                    , "; </script>");
                                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>"); // MauInTeamplate.js: used to bind data in knockout
                                PrintExtraReport(data);
                            }
                        });

                    })
                }
            }
        })
    }

    function PrintExtraReport_Multiple(dataContent) {
        var frame1 = $('<iframe />');
        frame1[0].name = "frame1";
        frame1.css({ "position": "absolute", "top": "-100000px" });
        $("body").append(frame1);
        var frameDoc = frame1[0].contentWindow ? frame1[0].contentWindow : frame1[0].contentDocument.document ? frame1[0].contentDocument.document : frame1[0].contentDocument;
        frameDoc.document.open();
        frameDoc.document.write('<html><head>');
        frameDoc.document.write(`<style> tr.mauin-tr-netlien td { border-bottom: 1px solid #ccc; } 
                                tr.mauin-tr-netdut td { border-bottom: 1px dashed #ccc; }
                                table.mauin-table-baoquanh{
                                    border: 1px solid black;
                                }
                                table.mauin-table-baoquanh td,table.mauin-table-baoquanh th{
                                    border: none;
                                } </style>`);
        frameDoc.document.write('</head><body><div style="width:96%">');
        frameDoc.document.write(dataContent);
        frameDoc.document.write('</div></body></html>');
        frameDoc.document.close();
        setTimeout(function () {
            window.frames["frame1"].focus();
            window.frames["frame1"].print();
            frame1.remove();
        }, 500);
    }
    self.Change_LoaiMauIn = function (maChungTu, item) {
        dathangTeamplate = maChungTu;
        loadMauIn();
        if (maChungTu === 'DTH') {
            GetInfor_ofHDDoiTra(item);
        }
    }
    self.InHoaDon = function (item) {
        var cthdFormat = GetCTHDPrint_Format(self.BH_HoaDonChiTiets());
        self.CTHoaDonPrint(cthdFormat);
        var itemHDFormat = GetInforHDPrint(item, false);
        self.InforHDprintf(itemHDFormat);
        GetMauIn_ByMaLoaiChunghTu(dathangTeamplate);
    }
    function GetMauIn_ByMaLoaiChunghTu(maChungTu) {
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=' + maChungTu + '&idDonVi=' + id_donvi,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                var data = result;
                data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                data = data.concat("<script > var item1=" + JSON.stringify(self.CTHoaDonPrint())
                    + "; var item2=" + JSON.stringify(self.CTHoaDonPrintMH())
                    + ";var item3=" + JSON.stringify(self.InforHDprintf())
                    + ";var item4=" + JSON.stringify(self.HangMucSuaChuas())
                    + ";var item5=" + JSON.stringify(self.VatDungKemTheos())
                    + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>"); // MauInTeamplate.js: used to bind data in knockout
                PrintExtraReport(data); // assign content HTML into frame
            }
        });
    }
    self.InHoaDonDoiTra = function (item) {
        var cthdTraHang = GetCTHDPrint_Format(self.BH_HoaDonChiTiets());
        self.CTHoaDonPrint(cthdTraHang);
        var cthdDoiTra = GetCTHDPrint_Format(item.BH_HoaDon_ChiTiet);
        self.CTHoaDonPrintMH(cthdDoiTra);
        var tongTienHDTra = 0;
        var phiTraHang = 0;
        // get tongTienTra, phiTraHang from HD Tra
        for (let i = 0; i < self.HoaDons().length; i++) {
            let itFor = self.HoaDons()[i];
            if (itFor.ID === item.ID_HoaDon) {
                tongTienHDTra = itFor.TongThanhToan;
                phiTraHang = self.HoaDons()[i].TongChiPhi;
                break;
            }
        }
        item.TongTienTraHang = tongTienHDTra;
        item.PhiTraHang = phiTraHang;
        var phaiTT = formatNumberToFloat(item.PhaiThanhToan) - tongTienHDTra;
        if (phaiTT > 0) {
            // khach phai tra them tien
            item.TongCong = phaiTT;
            item.PhaiTraKhach = 0;
        }
        else {
            item.TongCong = tongTienHDTra;
            item.PhaiTraKhach = tongTienHDTra;
        }
        var sumGiamGiaHang = cthdDoiTra.reduce(function (_this, val) {
            return formatNumberToFloat(_this) + formatNumberToFloat(val.TienChietKhau);
        }, 0);
        var itemHDFormat = GetInforHDPrint(item, true);
        itemHDFormat.TongGiamGiaHang = formatNumber(sumGiamGiaHang);
        self.InforHDprintf(itemHDFormat);
        GetMauIn_ByMaLoaiChunghTu('DTH');
    }
    self.InPhieuThu = function (item) {
        var temp = phieuThuTeamplate;
        if (item.LoaiHoaDon === 12) {
            temp = phieuChiTeamplate;
        }
        var itemHDFormat = GetInforPhieuThu(item);
        self.InforHDprintf(itemHDFormat);
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=' + temp + '&idDonVi=' + id_donvi,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                var data = result;
                data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                data = data.concat("<script > var item1=[], item4 =[], item5 =[] ; var item2=[] ;var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(data);
            }
        });
    }
    function GetInforPhieuThu(objHD) {
        objHD.TenCuaHang = self.CongTy()[0].TenCongTy;
        objHD.DiaChiCuaHang = self.CongTy()[0].DiaChi;
        objHD.DienThoaiCuaHang = self.CongTy()[0].SoDienThoai;
        objHD.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
        objHD.ChiNhanhBanHang = objHD.TenChiNhanh;
        objHD.MaPhieu = objHD.MaHoaDon;
        objHD.NgayLapHoaDon = moment(objHD.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss');
        objHD.NguoiNhanTien = objHD.NguoiNopTien;
        objHD.DiaChiKhachHang = self.ItemHoaDon().DiaChiKhachHang;
        objHD.DienThoaiKhachHang = self.ItemHoaDon().DienThoai;
        objHD.TienBangChu = DocSo(formatNumberToFloat(objHD.TongTienThu));
        objHD.GiaTriPhieu = formatNumber(objHD.TongTienThu);
        return objHD;
    }
    function GetInforHDPrint(objHD, isDoiTraHang) {
        var objPrint = $.extend({}, objHD);
        var phaiThanhToan = formatNumberToFloat(objHD.PhaiThanhToan);
        var daThanhToan = formatNumberToFloat(objHD.KhachDaTra);
        objPrint.MaHoaDonTraHang = objPrint.MaHoaDonGoc;
        objPrint.TenNhaCungCap = objPrint.TenDoiTuong;
        objPrint.DienThoaiKhachHang = objPrint.DienThoai;
        objPrint.TongTichDiem = formatNumber(objPrint.DiemSauGD);
        objPrint.NhanVienBanHang = objPrint.TenNhanVien;
        objPrint.TongGiamGia = formatNumber(objHD.TongGiamGia + objHD.KhuyeMai_GiamGia);
        if (objPrint.NgaySinh_NgayTLap !== null) {
            objPrint.NgaySinh_NgayTLap = moment(objPrint.NgaySinh_NgayTLap).format('DD/MM/YYYY');
        }
        var tongcong = formatNumberToFloat(objPrint.TongTienHang) - formatNumberToFloat(objPrint.TongGiamGia)
            - formatNumberToFloat(objPrint.KhuyeMai_GiamGia)
            + formatNumberToFloat(objPrint.TongTienThue);

        objPrint.NgayLapHoaDon = moment(objHD.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss');
        objPrint.Ngay = moment(objHD.NgayLapHoaDon).format('DD');
        objPrint.Thang = moment(objHD.NgayLapHoaDon).format('MM');
        objPrint.Nam = moment(objHD.NgayLapHoaDon).format('YYYY');

        objPrint.TongTienHang = formatNumber3Digit(objPrint.TongTienHang);
        objPrint.PhaiThanhToan = formatNumber(phaiThanhToan);
        objPrint.DaThanhToan = formatNumber(daThanhToan);
        objPrint.PhaiThanhToanBaoHiem = formatNumber(objHD.PhaiThanhToanBaoHiem);
        objPrint.DiemGiaoDich = formatNumber(objPrint.DiemGiaoDich);
        objPrint.TienThua = 0;
        objPrint.TongTienThue = formatNumber(objPrint.TongTienThue);

        var conno = formatNumberToFloat(objPrint.PhaiThanhToan) - daThanhToan;
        if (isDoiTraHang) {
            // doi tra hang
            tongcong = tongcong - formatNumberToFloat(objPrint.TongTienTraHang);
            // {TienTraKhach} --> PhaiTraKhach
            // {KhachCanTra} --> PhaiThanhToan
            // {DaThanhToan} --> DaThanhToan
            if (tongcong < 0) {
                // mua < tra
                objPrint.PhaiTraKhach = formatNumber(-tongcong);
                objPrint.TongCong = formatNumber(-tongcong);
                objPrint.PhaiThanhToan = 0;
            }
            else {
                // mua > tra --> khach phai tra
                objPrint.PhaiTraKhach = 0;
                objPrint.PhaiThanhToan = formatNumber(tongcong);
                objPrint.TongCong = formatNumber(tongcong);
            }
            objPrint.TongTienTraHang = formatNumber3Digit(objHD.TongTienTraHang);
            objPrint.TongChiPhi = formatNumber(objHD.PhiTraHang);
            objPrint.TongTienTra = objHD.TongTienTraHang - objHD.TongTienThue;
            objPrint.TongTienHoaDonMua = objPrint.TongTienHang;
        }
        else {
            // tra hang
            objPrint.TongTienTraHang = objPrint.TongTienHang;
            objPrint.TongChiPhi = formatNumber(objPrint.TongChiPhi);
            objPrint.TongCong = formatNumber(tongcong);
            objPrint.TongTienTra = formatNumber(tongcong);
        }
        var notruoc = 0, nosau = 0;
        if (self.ChiTietDoiTuong() !== null && self.ChiTietDoiTuong().length > 0
            && self.ChiTietDoiTuong()[0].ID !== "00000000-0000-0000-0000-000000000000") {
            nosau = self.ChiTietDoiTuong()[0].NoHienTai;
            notruoc = nosau - conno;
            notruoc = notruoc < 0 ? 0 : notruoc;
        }
        else {
            nosau = conno;// khachle
        }
        objPrint.NoTruoc = formatNumber(notruoc);
        objPrint.NoSau = formatNumber(nosau);
        objPrint.ChiPhiNhap = objPrint.TongChiPhi;
        objPrint.GhiChu = objPrint.DienGiai;
        objPrint.TienBangChu = DocSo(formatNumberToFloat(objPrint.TongCong));
        objPrint.TenChiNhanh = objPrint.TenDonVi; // chi nhanh ban hang
        // logo cong ty
        if (self.CongTy().length > 0) {
            objPrint.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
            objPrint.TenCuaHang = self.CongTy()[0].TenCongTy;
            objPrint.DiaChiCuaHang = self.CongTy()[0].DiaChi;
            objPrint.DienThoaiCuaHang = self.CongTy()[0].SoDienThoai;
        }

        objPrint.TienMat = formatNumber3Digit(objPrint.TienMat);
        objPrint.TienATM = formatNumber3Digit(objPrint.TienATM);// in store: assign TienGui = TienATM
        objPrint.TienKhachThieu = formatNumber(conno);
        // the gia tri
        objPrint.TongTaiKhoanThe = formatNumber(vmThanhToan.theGiaTriCus.TongNapThe);
        objPrint.TongSuDungThe = formatNumber(vmThanhToan.theGiaTriCus.SuDungThe);
        objPrint.SoDuConLai = formatNumber(vmThanhToan.theGiaTriCus.SoDuTheGiaTri);
        objPrint.TienDoiDiem = formatNumber(objPrint.TienDoiDiem);
        objPrint.TienTheGiaTri = formatNumber(objPrint.ThuTuThe);

        let pthuc = '';
        if (formatNumberToFloat(objHD.TienMat) > 0) {
            pthuc += 'Tiền mặt, ';
        }
        if (formatNumberToFloat(objHD.TienATM) > 0) {
            pthuc += 'POS, ';
        }
        if (formatNumberToFloat(objHD.ChuyenKhoan) > 0) {
            pthuc += 'Chuyển khoản, ';
        }
        if (formatNumberToFloat(objHD.ThuTuThe) > 0) {
            pthuc += 'Thẻ giá trị, ';
        }
        if (formatNumberToFloat(objHD.TienDoiDiem) > 0) {
            pthuc += 'Điểm, ';
        }
        if (formatNumberToFloat(objHD.TienDatCoc) > 0) {
            pthuc += 'Tiền cọc, ';
        }
        objPrint.PhuongThucTT = Remove_LastComma(pthuc);

        let nvHoaDon = '';
        let nvHoaDon_inCK = '';
        if (objPrint.BH_NhanVienThucHiens !== null && objPrint.BH_NhanVienThucHiens.length > 0) {
            for (let i = 0; i < objPrint.BH_NhanVienThucHiens.length; i++) {
                let nv = objPrint.BH_NhanVienThucHiens[i];
                nvHoaDon += nv.TenNhanVien + ', ';
                nvHoaDon_inCK += nv.TenNhanVien.concat(nv.PT_ChietKhau > 0 ? ' ('.concat(nv.PT_ChietKhau, ' %)') : ' ('.concat(formatNumber(nv.TienChietKhau), ')'), ', ');
            }
        }
        objPrint.ChietKhauNVHoaDon = Remove_LastComma(nvHoaDon);
        objPrint.ChietKhauNVHoaDon_InGtriCK = Remove_LastComma(nvHoaDon_inCK);

        objPrint.TongSoLuongHang = formatNumber(self.TongSoLuongHang());
        objPrint.TongGiamGiaHang = formatNumber(self.TongGiamGiaHang());
        objPrint.TongTienDichVu = formatNumber(self.TongTienDichVu());
        objPrint.TongTienDichVu_TruocVAT = formatNumber(self.TongTienDichVu_TruocVAT());
        objPrint.TongTienDichVu_TruocCK = formatNumber(self.TongTienDichVu_TruocCK());
        objPrint.TongTienPhuTung = formatNumber(self.TongTienPhuTung());
        objPrint.TongTienPhuTung_TruocVAT = formatNumber(self.TongTienPhuTung_TruocVAT());
        objPrint.TongTienPhuTung_TruocCK = formatNumber(self.TongTienPhuTung_TruocCK());
        objPrint.TongTienHangChuaCK = formatNumber(self.TongTienHangChuaCK());
        objPrint.TongGiamGiaHD_HH = formatNumber(self.TongGiamGiaHang() + objHD.TongGiamGia + objHD.KhuyeMai_GiamGia);
        objPrint.TongTienHDSauGiamGia = formatNumber3Digit(formatNumberToFloat(objPrint.TongTienHang) - objPrint.TongGiamGiaKM_HD);

        objPrint.TongThue_PhuTung = self.TongThue_PhuTung();
        objPrint.TongCK_PhuTung = self.TongCK_PhuTung();
        objPrint.TongThue_DichVu = self.TongThue_DichVu();
        objPrint.TongCK_DichVu = self.TongCK_DichVu();
        objPrint.TongSL_DichVu = self.TongSL_DichVu();
        objPrint.TongSL_PhuTung = self.TongSL_PhuTung();

        // gara
        objPrint.BH_TenLienHe = objHD.LienHeBaoHiem;
        objPrint.BH_SDTLienHe = objHD.SoDienThoaiLienHeBaoHiem;

        objPrint.TongTienBHDuyet = formatNumber(objHD.TongTienBHDuyet);
        objPrint.KhauTruTheoVu = formatNumber(objHD.KhauTruTheoVu);
        objPrint.GiamTruBoiThuong = formatNumber(objHD.GiamTruBoiThuong);
        objPrint.BHThanhToanTruocThue = formatNumber(objHD.BHThanhToanTruocThue);
        objPrint.TongTienThueBaoHiem = formatNumber(objHD.TongTienThueBaoHiem);
        objPrint.BH_TienBangChu = DocSo(objHD.BaoHiemDaTra);
        objPrint.KH_TienBangChu = DocSo(daThanhToan);
        objPrint.BH_ConThieu = formatNumber3Digit(objHD.PhaiThanhToanBaoHiem - objHD.BaoHiemDaTra);

        let bh_notruoc = 0, bh_nosau = 0;
        if (self.InforBaoHiem() !== null && self.InforBaoHiem().length > 0) {
            bh_nosau = self.InforBaoHiem()[0].NoHienTai;
            bh_notruoc = bh_nosau - objHD.PhaiThanhToanBaoHiem - objHD.BaoHiemDaTra;
            bh_notruoc = bh_notruoc < 0 ? 0 : bh_notruoc;
        }
        objPrint.BH_NoTruoc = bh_notruoc;
        objPrint.BH_NoSau = bh_nosau;

        // phieutiepnhan
        var tn = self.ThongTinPhieuTiepNhan();
        if (tn) {
            objPrint.BienSo = tn.BienSo;
            objPrint.ChuXe = tn.ChuXe;
            objPrint.ChuXe_DiaChi = tn.ChuXe_DiaChi;
            objPrint.ChuXe_Email = tn.ChuXe_Email;
            objPrint.ChuXe_SDT = tn.ChuXe_SDT;
            objPrint.CoVanDichVu = tn.CoVanDichVu;
            objPrint.CoVan_SDT = tn.CoVan_SDT;
            objPrint.DungTich = tn.DungTich;
            objPrint.PTN_GhiChu = tn.GhiChu;
            objPrint.HopSo = tn.HopSo;
            objPrint.NhanVienTiepNhan = tn.NhanVienTiepNhan;
            objPrint.MauSon = tn.MauSon;
            objPrint.NamSanXuat = tn.NamSanXuat;
            objPrint.NgayVaoXuong = moment(tn.NgayVaoXuong).format('DD/MM/YYYY HH:mm');
            objPrint.NgayXuatXuong = tn.NgayXuatXuong ? moment(tn.NgayXuatXuong).format('DD/MM/YYYY HH:mm') : '';
            objPrint.NgayXuatXuongDuKien = tn.NgayXuatXuongDuKien ? moment(tn.NgayXuatXuongDuKien).format('DD/MM/YYYY HH:mm') : '';
            objPrint.SoDienThoaiLienHe = tn.SoDienThoaiLienHe;
            objPrint.SoKhung = tn.SoKhung;
            objPrint.SoMay = tn.SoMay;
            objPrint.SoKmRa = tn.SoKmRa;
            objPrint.SoKmVao = tn.SoKmVao;
            objPrint.TenHangXe = tn.TenHangXe;
            objPrint.TenLoaiXe = tn.TenLoaiXe;
            objPrint.TenMauXe = tn.TenMauXe;
            objPrint.TenLienHe = tn.TenLienHe;
        }
        return objPrint;
    }
    function GetInforCongTy() {
        ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CongTy', 'GET').done(function (data) {
            if (data !== null) {
                self.CongTy(data);
            }
        });
    }
    function GetCTHDPrint_Format(arr) {
        var arrCTHD = [];
        var thuocTinh = '';
        for (let i = 0; i < arr.length; i++) {
            let itFor = $.extend({}, arr[i]);
            let price = formatNumberToFloat(itFor.DonGia);
            let sale = formatNumberToFloat(itFor.GiamGia);
            let giaban = formatNumberToFloat(itFor.GiaBan);
            let bh_tt = itFor.SoLuong * formatNumberToFloat(itFor.DonGiaBaoHiem);

            thuocTinh = itFor.ThuocTinh_GiaTri;
            thuocTinh = thuocTinh === null || thuocTinh === '' ? '' : thuocTinh.substr(1);
            itFor.DonGia = formatNumber(price);
            itFor.DonGiaBaoHiem = formatNumber(itFor.DonGiaBaoHiem);
            itFor.BH_ThanhTien = formatNumber3Digit(bh_tt);
            itFor.TienChietKhau = formatNumber(sale);
            itFor.GiaBan = formatNumber(giaban);
            itFor.SoLuong = formatNumber3Digit(itFor.SoLuong);
            itFor.ThanhTien = formatNumber3Digit(itFor.ThanhTien);
            itFor.ThuocTinh_GiaTri = thuocTinh;

            let sophutTH = itFor.ThoiGianThucHien;
            let quathoigian = itFor.QuaThoiGian;
            if (sophutTH > 0) {
                itFor.TimeStart = moment(itFor.ThoiGian).format('HH:mm');
                itFor.ThoiGianThucHien = sophutTH + ' phút';
                itFor.QuaThoiGian = quathoigian + ' phút';
            }

            // nvthuchien, tuvan co in %ck 
            let th_CoCK = '';
            let tv_CoCK = '';
            if (itFor.BH_NhanVienThucHien != null) {
                for (let j = 0; j < itFor.BH_NhanVienThucHien.length; j++) {
                    let for2 = itFor.BH_NhanVienThucHien[j];
                    if (for2.ThucHien_TuVan) {
                        th_CoCK += for2.TenNhanVien.concat(for2.PT_ChietKhau > 0 ? ' ('.concat(for2.PT_ChietKhau, ' %)') : ' ('.concat(formatNumber(for2.TienChietKhau), ')'), ', ');
                    }
                    else {
                        tv_CoCK += for2.TenNhanVien.concat(for2.PT_ChietKhau > 0 ? ' ('.concat(for2.PT_ChietKhau, ' %)') : ' ('.concat(formatNumber(for2.TienChietKhau), ')'), ', ');
                    }
                }
            }
            itFor.NVThucHienDV_CoCK = Remove_LastComma(th_CoCK);
            itFor.NVTuVanDV_CoCK = Remove_LastComma(tv_CoCK);

            let lstCombo = $.grep(vmThanhPhanCombo.AllComBo_ofHD, function (x) {
                return x.ID_ParentCombo === itFor.ID_ParentCombo;
            });
            if (lstCombo.length > 0) {
                itFor.ThanhPhanComBo = lstCombo;
                itFor = AssignThanhPhanComBo_toCTHD(itFor);
            }
            else {
                itFor.ThanhPhanComBo = [];
            }
            arrCTHD.push(itFor);
        }
        return arrCTHD;
    }


    self.XuLiDonHang = function (item) {
        localStorage.setItem('createHDfrom', 2);
        var phaiTT = item.PhaiThanhToan - item.KhachDaTra;
        var dvtGiam = 'VND';
        if (item.TongChietKhau > 0) {
            dvtGiam = '%';
        }
        let objTax = GetPTChietKhauHang_HeaderBH(item);
        var itemNew = {
            ID: item.ID,
            ID_HoaDon: null,
            ID_DonVi: item.ID_DonVi,
            MaHoaDon: item.MaHoaDon,
            MaHoaDonDB: item.MaHoaDon,
            LoaiHoaDon: loaiHoaDon,
            ID_DoiTuong: item.ID_DoiTuong,
            NguoiTao: userLogin,
            ID_BangGia: item.ID_BangGia,
            ID_NhanVien: item.ID_NhanVien,
            NgayLapHoaDon: moment(item.NgayLapHoaDon).format('YYYY-MM-DD HH:mm:ss'),
            TongTienHang: item.TongTienHang,
            PhaiThanhToan: item.PhaiThanhToan,
            TongGiamGia: item.TongGiamGia,
            KhachDaTra: item.KhachDaTra,
            PTThueHoaDon: item.PTThueHoaDon,
            TongTienThue: item.TongTienThue,
            PTThueBaoHiem: 0,
            TongTienThueBaoHiem: 0,
            ChoThanhToan: false,
            DienGiai: item.DienGiai,
            TienGui: 0,
            TienThua: 0,
            Status: 1,
            StatusOffline: false,
            DVTinhGiam: dvtGiam,
            TongChietKhau: item.TongChietKhau, // PTGiam
            // gan DaThanhToan : Tien con lai phai TT --> to do bind in BanLe.js
            DaThanhToan: self.IsGara() ? 0 : phaiTT,
            // tien khach con phai TT : tong PhaiTT - KhachDaTra
            PhaiThanhToan: self.IsGara() ? item.PhaiThanhToan : phaiTT,
            TienMat: self.IsGara() ? 0 : phaiTT,
            TienATM: 0,
            PhaiThanhToanBaoHiem: 0,
            BaoHiemDaTra: 0,
            // Tra Hang
            PhaiThanhToanDB: 0,
            TongGiaGocHangTra: 0,
            TongChiPhiHangTra: 0,
            TongChiPhi: item.TongChiPhi,
            HoanTraThuKhac: 0,
            TongTienTra: 0,
            PhaiTraKhach: 0,
            DaTraKhach: 0,
            GiaoHang: false,
            TongGiamGiaDB: item.TongGiamGia,
            DiemGiaoDichDB: 0,
            PTGiamDB: 0,
            TongTienMua: 0,
            PTThueDB: 0,
            TongThueDB: 0,
            // DatHang
            TrangThaiHD: 6, // HD DatHang dang xuly
            IsActive: '',
            YeuCau: item.YeuCau,
            IsChose: true, // tức là đang thao tác với hóa đơn này
            HoanTraTamUng: 0,
            IsKhuyenMaiHD: false,
            IsOpeningKMaiHD: false,
            KhuyeMai_GiamGia: 0,
            TongGiamGiaKM_HD: item.TongGiamGia + item.KhuyeMai_GiamGia, // to do bind GiamGia at BanHang
            // Tich Diem
            TTBangDiem: 0,
            DiemGiaoDich: 0,
            DiemQuyDoi: 0,
            DiemHienTai: 0,
            DiemCong: 0,  // use when KM_Cong diem
            DiemKhuyenMai: 0,

            ID_NhomDTApplySale: null,
            // Goi dich vu
            NgayApDungGoiDV: null,
            HanSuDungGoiDV: null,
            CreateTime: 0,
            ID_ViTri: null,
            TenViTriHD: '',
            BH_NhanVienThucHiens: [],
            TienTheGiaTri: 0,
            ThoiGianThucHien: 0,

            MaPhieuTiepNhan: item.MaPhieuTiepNhan,
            BienSo: item.BienSo,
            ID_PhieuTiepNhan: item.ID_PhieuTiepNhan,
            ID_BaoHiem: item.ID_BaoHiem,
            LienHeBaoHiem: item.LienHeBaoHiem,
            SoDienThoaiLienHeBaoHiem: item.SoDienThoaiLienHeBaoHiem,
            PhaiThanhToanBaoHiem: 0,
            ChiPhi_GhiChu: item.ChiPhi_GhiChu,
            TongThanhToan: item.TongThanhToan,
            TenBaoHiem: item.TenBaoHiem,
            XuatKhoAll: false,
            DuyetBaoGia: !item.ChoThanhToan,

            PTChietKhauHH: objTax.PTChietKhauHH,
            TongGiamGiaHang: self.TongGiamGiaHang(),
            TongTienHangChuaCK: self.TongTienHangChuaCK(),
            TongTienKhuyenMai_CT: 0,
            TongGiamGiaKhuyenMai_CT: 0,

            TongTienBHDuyet: 0,
            GiamTruBoiThuong: 0,
            SoVuBaoHiem: '',
            KhauTruTheoVu: 0,
            PTGiamTruBoiThuong: 0,
            BHThanhToanTruocThue: 0,
            TongThueKhachHang: item.TongTienThue,
            CongThucBaoHiem: 13,
            GiamTruThanhToanBaoHiem: 0,
            HeaderBH_GiaTriPtram: 0,
            HeaderBH_Type: 1,
        }
        // save cache lcXuLiDonHang to arrayJson --> xu li nhieu DH cung 1 luc
        var lcXuLiDonHang = localStorage.getItem('lcXuLiDonHang');
        if (lcXuLiDonHang === null) {
            lcXuLiDonHang = [];
        }
        else {
            lcXuLiDonHang = JSON.parse(lcXuLiDonHang);
            // remove and add new
            lcXuLiDonHang = $.grep(lcXuLiDonHang, function (x) {
                return x.MaHoaDon !== item.MaHoaDon;
            });
        }
        lcXuLiDonHang.push(itemNew);
        ajaxHelper(BH_HoaDonUri + 'GetCTHoaDon_afterDatHang?idHoaDon=' + item.ID, 'GET').done(function (data) {
            var lcCTDatHang = localStorage.getItem('lcCTDatHang');
            if (lcCTDatHang === null) {
                lcCTDatHang = [];
            }
            else {
                lcCTDatHang = JSON.parse(lcCTDatHang);
                lcCTDatHang = $.grep(lcCTDatHang, function (x) {
                    return x.MaHoaDon !== item.MaHoaDon;
                })
            }
            if (data !== null) {
                // order by SoThuTu ASC --> group Hang Hoa by LoHang
                var arrCTsort = data.sort(function (a, b) {
                    var x = a.SoThuTu,
                        y = b.SoThuTu;
                    return x < y ? -1 : x > y ? 1 : 0;
                });
                var arrIDQuiDoi1 = [];
                var cthdLoHang = [];
                for (let i = 0; i < arrCTsort.length; i++) {
                    let ctNew = $.extend({}, arrCTsort[i]);
                    ctNew = SetDefaultPropertiesCTHD(arrCTsort[i], item.MaHoaDon, item.LoaiHoaDon);
                    ctNew.SoLuongDaMua = 0;
                    ctNew.TienChietKhau = ctNew.GiamGia;
                    ctNew.DVTinhGiam = '%';
                    ctNew.GiaBan = ctNew.DonGia;
                    ctNew.ThanhTien = (arrCTsort[i].GiaBan - ctNew.TienChietKhau) * ctNew.SoLuong;
                    if (arrCTsort[i].TienChietKhau > 0) {
                        if (arrCTsort[i].PTChietKhau === 0) {
                            ctNew.DVTinhGiam = 'VND';
                        }
                    }
                    ctNew.CssWarning = false;
                    ctNew.ChatLieu = '3';
                    ctNew.ID_ChiTietGoiDV = null;
                    ctNew.GhiChu_NVThucHien = '';
                    ctNew.BH_NhanVienThucHien = [];// dathang: khong co NVThucHien
                    ctNew.GhiChu_NVTuVan = '';
                    ctNew.GhiChu_NVThucHienPrint = '';
                    ctNew.GhiChu_NVTuVanPrint = '';
                    let quycach = ctNew.QuyCach === null || ctNew.QuyCach === 0 ? 1 : ctNew.QuyCach;
                    ctNew.QuyCach = quycach;
                    // DatHang: phi DV = 0
                    ctNew.ID_ViTri = null;
                    ctNew.TenViTri = '';
                    ctNew.ThoiGianThucHien = 0;

                    ctNew = AssignTPDinhLuong_toCTHD(ctNew);
                    ctNew = AssignNVThucHien_toCTHD(ctNew);

                    let quanLiTheoLo = ctNew.QuanLyTheoLoHang;
                    ctNew.DM_LoHang = [];
                    ctNew.LotParent = quanLiTheoLo;

                    let dateLot = GetNgaySX_NgayHH(ctNew);
                    ctNew.NgaySanXuat = dateLot.NgaySanXuat;
                    ctNew.NgayHetHan = dateLot.NgayHetHan;

                    // get tpcombo
                    let combo = $.grep(vmThanhPhanCombo.AllComBo_ofHD, function (x) {
                        return x.ID_ParentCombo === ctNew.ID_ParentCombo;
                    });
                    if (combo.length > 0) {
                        ctNew.ThanhPhanComBo = combo;
                        ctNew = AssignThanhPhanComBo_toCTHD(ctNew);
                    }
                    else {
                        ctNew.ThanhPhanComBo = [];
                    }

                    // check exist in cthdLoHang
                    if ($.inArray(arrCTsort[i].ID_DonViQuiDoi, arrIDQuiDoi1) === -1) {
                        arrIDQuiDoi1.unshift(arrCTsort[i].ID_DonViQuiDoi);
                        // push CTHD
                        ctNew.SoThuTu = cthdLoHang.length + 1;
                        ctNew.IDRandom = CreateIDRandom('RandomCT_');
                        if (quanLiTheoLo) {
                            // push DM_Lo
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
                                if (quanLiTheoLo) {
                                    let objLot = $.extend({}, ctNew);
                                    objLot.LotParent = false;
                                    objLot.IDRandom = CreateIDRandom('RandomCT_');
                                    objLot.DM_LoHang = [];
                                    objLot.HangCungLoais = [];
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
                // sort CTHD by SoThuTu desc
                cthdLoHang = cthdLoHang.sort(function (a, b) {
                    var x = a.SoThuTu, y = b.SoThuTu;
                    return x < y ? 1 : x > y ? -1 : 0;
                });
                // push in cache CTHD (xu li nhieu CT cung luc)
                for (let i = 0; i < cthdLoHang.length; i++) {
                    lcCTDatHang.push(cthdLoHang[i]);
                }

                localStorage.setItem('lcXuLiDonHang', JSON.stringify(lcXuLiDonHang));
                localStorage.setItem('lcCTDatHang', JSON.stringify(lcCTDatHang));
                SetCache_ifGara('TN_xulyBG');
                if (self.IsGara()) {
                    localStorage.setItem('maHDCache', item.MaHoaDon);// used to get at gara.js (phieu dang xuly)
                }
                self.gotoGara();
            }
            else {
                ShowMessage_Danger('Không có dữ liệu');
                return false;
            }
        });
    }
    self.DatHang = function () {
        localStorage.setItem('fromDatHang', true);
        self.gotoGara();
    }

    self.DuyetBaoGia = function (item) {
        var $this = $(event.currentTarget);
        ajaxHelper('/api/DanhMuc/GaraAPI/' + 'Duyet_HuyBaoGia?id=' + item.ID + '&trangthai=' + false, 'GET').done(function (x) {

            if (x.res) {
                ShowMessage_Success('Đã duyệt báo giá thành công');
                // savediary
                var diary = {
                    ID_DonVi: id_donvi,
                    ID_NhanVien: _id_NhanVien,
                    ChucNang: 'Danh sách báo giá',
                    LoaiNhatKy: 1,
                    NoiDung: 'Duyệt báo giá '.concat(item.MaHoaDon),
                    NoiDungChiTiet: 'Duyệt báo giá '.concat(' báo giá ', item.MaHoaDon,
                        ', người duyệt: ', userLogin),
                }
                Insert_NhatKyThaoTac_1Param(diary);

                $this.next().show();
                $this.hide();
            }
            else {
                ShowMessage_Danger('Duyệt báo giá thất bại');
            }
        })
    }

    self.CopyDatHang = function (item) {
        localStorage.setItem('createHDfrom', 1);
        SetCache_ifGara('TN_copyDH');

        var phaiTT = item.PhaiThanhToan;
        var dvtGiam = 'VND';
        if (item.TongChietKhau > 0) {
            dvtGiam = '%';
        }

        var obj = GetPTChietKhauHang_HeaderBH(item);
        var itemNew = {
            ID: const_GuidEmpty,
            ID_HoaDon: null,
            MaHoaDon: item.MaHoaDon,
            MaHoaDonDB: 'Copy' + item.MaHoaDon,
            LoaiHoaDon: loaiHoaDon,
            ID_DonVi: item.ID_DonVi,
            ID_DoiTuong: item.ID_DoiTuong,
            ID_BangGia: item.ID_BangGia,
            ID_NhanVien: item.ID_NhanVien,
            NguoiTao: userLogin,
            NgayLapHoaDon: null, // assign = null: auto update time

            TongTienHang: item.TongTienHang,
            TongGiamGia: item.TongGiamGia,
            KhachDaTra: 0,
            PTThueHoaDon: item.PTThueHoaDon,
            TongTienThue: item.TongTienThue,
            PTThueBaoHiem: 0,
            TongTienThueBaoHiem: 0,
            ChoThanhToan: false,
            DienGiai: item.DienGiai,
            TienGui: 0,
            TienATM: 0,
            TienThua: 0,
            Status: 1,
            StatusOffline: false,
            DVTinhGiam: dvtGiam,
            TongChietKhau: item.TongChietKhau, // PTGiam
            DaThanhToan: 0,
            PhaiThanhToan: self.IsGara() ? item.PhaiThanhToan : phaiTT,
            TienMat: self.IsGara() ? 0 : phaiTT,
            // Tra Hang
            TongGiaGocHangTra: 0,
            TongChiPhiHangTra: 0,
            TongChiPhi: item.TongChiPhi,
            HoanTraThuKhac: 0,
            TongTienTra: 0,
            PhaiTraKhach: 0,
            DaTraKhach: 0,
            GiaoHang: false,
            PhaiThanhToanDB: 0,
            TongGiamGiaDB: 0,
            DiemGiaoDichDB: 0,
            PTGiamDB: 0,
            TongTienMua: 0,
            PTThueDB: 0,
            TongThueDB: 0,

            TrangThaiHD: 1,// hd new
            IsActive: '',
            YeuCau: "1",
            IsChose: true, // chỉ sử dụng ở Xử lí đơn hàng
            HoanTraTamUng: 0,
            IsKhuyenMaiHD: false,
            IsOpeningKMaiHD: false,
            KhuyeMai_GiamGia: 0,
            TongGiamGiaKM_HD: item.TongGiamGia + item.KhuyeMai_GiamGia,
            // TichDiem
            TTBangDiem: 0,
            DiemGiaoDich: 0,
            DiemQuyDoi: 0,
            DiemHienTai: 0,
            // use when KM_Cong diem
            DiemCong: 0,
            ID_NhomDTApplySale: null,
            // Goi dich vu
            NgayApDungGoiDV: null,
            HanSuDungGoiDV: null,
            CreateTime: 0,
            ID_ViTri: null,
            TenViTriHD: '',
            BH_NhanVienThucHiens: [],
            TienTheGiaTri: 0,
            ThoiGianThucHien: 0,

            MaPhieuTiepNhan: item.MaPhieuTiepNhan,
            BienSo: item.BienSo,
            ID_PhieuTiepNhan: item.ID_PhieuTiepNhan,
            ID_BaoHiem: item.ID_BaoHiem,
            LienHeBaoHiem: item.LienHeBaoHiem,
            SoDienThoaiLienHeBaoHiem: item.SoDienThoaiLienHeBaoHiem,
            PhaiThanhToanBaoHiem: 0,
            ChiPhi_GhiChu: item.ChiPhi_GhiChu,
            TongThanhToan: item.TongThanhToan,
            TenBaoHiem: item.TenBaoHiem,
            XuatKhoAll: false,
            DuyetBaoGia: false,

            PTChietKhauHH: obj.PTChietKhauHH,
            TongGiamGiaHang: self.TongGiamGiaHang(),
            TongTienHangChuaCK: self.TongTienHangChuaCK(),
            TongTienKhuyenMai_CT: 0,
            TongGiamGiaKhuyenMai_CT: 0,
            SoDuDatCoc: 0,

            TongTienBHDuyet: 0,
            GiamTruBoiThuong: 0,
            SoVuBaoHiem: '',
            KhauTruTheoVu: 0,
            PTGiamTruBoiThuong: 0,
            BHThanhToanTruocThue: 0,
            TongThueKhachHang: item.TongTienThue,
            CongThucBaoHiem: 0,
            GiamTruThanhToanBaoHiem: 0,
            HeaderBH_GiaTriPtram: 0,
            HeaderBH_Type: 1,
        }
        localStorage.setItem('lcCopyDatHang', JSON.stringify(itemNew));
        // order by SoThuTu ASC --> group Hang Hoa by LoHang
        var arrCTsort = self.BH_HoaDonChiTiets().sort(function (a, b) {
            var x = a.SoThuTu,
                y = b.SoThuTu;
            return x < y ? -1 : x > y ? 1 : 0;
        });
        var arrIDQuiDoi = [];
        var cthdLoHang = [];
        for (let i = 0; i < arrCTsort.length; i++) {
            let ctNew = $.extend({}, arrCTsort[i]);
            ctNew = SetDefaultPropertiesCTHD(ctNew, item.MaHoaDon, 3);
            ctNew.SoLuongDaMua = 0;
            ctNew.TienChietKhau = ctNew.GiamGia;
            ctNew.DVTinhGiam = '%';
            ctNew.GiaBan = ctNew.DonGia;
            ctNew.ThanhTien = (ctNew.GiaBan - ctNew.TienChietKhau) * ctNew.SoLuong;
            if (ctNew.TienChietKhau > 0 && ctNew.PTChietKhau === 0) {
                ctNew.DVTinhGiam = 'VND';
            }
            ctNew.CssWarning = false;
            ctNew.GhiChu_NVThucHien = '';
            ctNew.BH_NhanVienThucHien = [];
            ctNew.GhiChu_NVTuVan = '';
            ctNew.GhiChu_NVThucHienPrint = '';
            ctNew.GhiChu_NVTuVanPrint = '';
            ctNew.ID_ChiTietGoiDV = null;
            ctNew.ChatLieu = '';
            ctNew.LaPTPhiDichVu = false;
            ctNew.PhiDichVu = 0;
            ctNew.TongPhiDichVu = 0;
            ctNew.ID_ViTri = null;
            ctNew.TenViTri = '';

            ctNew = AssignTPDinhLuong_toCTHD(ctNew);
            ctNew = AssignNVThucHien_toCTHD(ctNew);

            // lo hang
            let quanLiTheoLo = ctNew.QuanLyTheoLoHang;
            ctNew.DM_LoHang = [];
            ctNew.LotParent = quanLiTheoLo;

            let dateLot = GetNgaySX_NgayHH(ctNew);
            ctNew.NgaySanXuat = dateLot.NgaySanXuat;
            ctNew.NgayHetHan = dateLot.NgayHetHan;

            // get tpcombo
            let combo = $.grep(vmThanhPhanCombo.AllComBo_ofHD, function (x) {
                return x.ID_ParentCombo === ctNew.ID_ParentCombo;
            });
            if (combo.length > 0) {
                ctNew.ThanhPhanComBo = combo;
                ctNew = AssignThanhPhanComBo_toCTHD(ctNew);
            }
            else {
                ctNew.ThanhPhanComBo = [];
            }

            // check exist in cthdLoHang
            if ($.inArray(ctNew.ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                arrIDQuiDoi.unshift(ctNew.ID_DonViQuiDoi);

                ctNew.SoThuTu = cthdLoHang.length + 1;
                ctNew.IDRandom = CreateIDRandom('RandomCT_');
                if (quanLiTheoLo) {
                    // push DM_Lo
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
                        if (quanLiTheoLo) {
                            // push DM_Lo
                            let objLot = $.extend({}, ctNew);
                            objLot.LotParent = false;
                            objLot.IDRandom = CreateIDRandom('RandomCT_');
                            objLot.DM_LoHang = [];
                            objLot.HangCungLoais = [];
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
        // sort CTHD by SoThuTu desc
        cthdLoHang = cthdLoHang.sort(function (a, b) {
            var x = a.SoThuTu, y = b.SoThuTu;
            return x < y ? 1 : x > y ? -1 : 0;
        });
        localStorage.setItem('lcCTDatHangCopy', JSON.stringify(cthdLoHang));
        self.gotoGara();
    }

    function SetDefaultPropertiesCTHD(itemCT, mahoadon, loaiHD) {
        itemCT.MaHoaDon = mahoadon;
        itemCT.LoaiHoaDon = loaiHD;
        itemCT.SrcImage = null;
        itemCT.CssWarning = loaiHD === 1 ? itemCT.SoLuong > itemCT.TonKho : false;
        itemCT.IsKhuyenMai = false;
        itemCT.IsOpeningKMai = false;
        itemCT.TenKhuyenMai = '';
        itemCT.HangHoa_KM = [];
        itemCT.DiemKhuyenMai = 0;
        itemCT.ID_ChiTietDinhLuong = null;
        itemCT.UsingService = false;
        itemCT.ListDonViTinh = [];
        itemCT.ShowEditQuyCach = false;
        itemCT.ShowWarningQuyCach = false;
        itemCT.SoLuongQuyCach = 0;
        itemCT.HangCungLoais = [];
        itemCT.ThanhPhanComBo = [];
        itemCT.LaConCungLoai = false;
        itemCT.TimeStart = 0;
        itemCT.QuaThoiGian = 0;
        itemCT.TimeRemain = 0;
        itemCT.ThoiGian = moment(new Date()).format('YYYY-MM-DD HH:mm');
        if (!commonStatisJs.CheckNull(itemCT.DonViTinh)) {
            itemCT.ListDonViTinh = itemCT.DonViTinh;
        }
        return itemCT;
    }

    function SetCache_ifGara(cacheName) {
        if (self.IsGara()) {
            localStorage.setItem('gara_CreateFrom', cacheName);
        }
    }

    function SetCache_ifNotGara(val) {
        if (!self.IsGara()) {
            localStorage.setItem('createHDfrom', val);
        }
    }
    self.SaoChepTraHang = function (item) {
        if (item.TongTienHDDoiTra > 0) {
            ShowMessage_Danger('Hóa đơn có hàng đổi trả, không thể sao chép');
            return;
        }
        var hdNew = $.extend({}, item);
        // assign again ID = constGuid_Empty at BanLe.js
        SetCache_ifNotGara(4);
        SetCache_ifGara('TN_copyTH');
        hdNew.MaHoaDonDB = 'Copy' + hdNew.MaHoaDon;
        hdNew.Status = 1;
        hdNew.LoaiHoaDon = loaiHoaDon;
        hdNew.NgayLapHoaDon = null; // assign = null: auto update time
        hdNew.StatusOffline = false;
        hdNew.DVTinhGiam = 'VND';
        hdNew.NguoiTao = userLogin;
        if (hdNew.TongChietKhau > 0) {
            hdNew.DVTinhGiam = '%';
        }
        // Goi dich vu
        hdNew.CreateTime = 0; // bắt đầu chọn bàn (phòng) lúc HH:mm
        hdNew.TenViTriHD = '';
        hdNew.BH_NhanVienThucHiens = [];
        hdNew.TienTheGiaTri = 0;
        hdNew.DiemGiaoDichDB = 0;

        hdNew.ThoiGianThucHien = 0;
        hdNew.TrangThaiHD = 1;// saochep
        hdNew.YeuCau = 1;
        hdNew.ChoThanhToan = false;
        hdNew.DiemKhuyenMai = 0;
        hdNew.IsActive = '';
        hdNew.XuatKhoAll = false;
        hdNew.DuyetBaoGia = false;

        hdNew.MaPhieuTiepNhan = '';
        hdNew.BienSo = '';
        hdNew.SoDuDatCoc = 0;
        // Tra Nhanh
        if (hdNew.ID_HoaDon === null) {
            hdNew.MaHoaDonTraHang = '';
            hdNew.TongGiaGocHangTra = hdNew.PhaiThanhToan;
            hdNew.TongTienTra = hdNew.PhaiThanhToan - hdNew.TongChiPhi;
            hdNew.PTGiamDB = 0;
            hdNew.TongGiamGiaDB = 0;
            hdNew.TongChiPhiHangTra = 0;
            hdNew.PhaiThanhToanDB = 0;
            hdNew.DaThanhToan = self.IsGara() ? 0 : hdNew.PhaiThanhToan;
            hdNew.DaTraKhach = self.IsGara() ? 0 : hdNew.PhaiThanhToan;
            hdNew.MaHoaDonTH_NVien = 'Trả hàng ';
            hdNew.PTThueDB = 0;
            hdNew.TongThueDB = 0;

            // order by SoThuTu ASC --> group Hang Hoa by LoHang
            var arrCTsort = self.BH_HoaDonChiTiets().sort(function (a, b) {
                var x = a.SoThuTu,
                    y = b.SoThuTu;
                return x < y ? -1 : x > y ? 1 : 0;
            });
            var arrIDQuiDoi = [];
            var cthdLoHang = [];
            for (let i = 0; i < arrCTsort.length; i++) {
                let ctNew = $.extend({}, arrCTsort[i]);
                ctNew = SetDefaultPropertiesCTHD(arrCTsort[i], hdNew.MaHoaDon, hdNew.LoaiHoaDon);
                ctNew.SoLuongDaMua = 0;
                ctNew.DVTinhGiam = '%'; // default DVTGiam ='%'
                ctNew.GiaBan = ctNew.DonGia - ctNew.TienChietKhau;// get thue from DB
                if (arrCTsort[i].TienChietKhau > 0) {
                    if (arrCTsort[i].PTChietKhau === 0) {
                        ctNew.DVTinhGiam = 'VND';
                    }
                }
                ctNew.CssWarning = true; // default TraHang = false
                ctNew.GhiChu_NVThucHien = '';
                ctNew.BH_NhanVienThucHien = [];
                ctNew.GhiChu_NVTuVan = '';
                ctNew.GhiChu_NVThucHienPrint = '';
                ctNew.GhiChu_NVTuVanPrint = '';
                ctNew.ThanhPhan_DinhLuong = [];
                ctNew.ThanhPhanComBo = [];
                ctNew.ChatLieu = '1';
                ctNew.ID_ChiTietGoiDV = null;
                ctNew.LaPTPhiDichVu = false;
                ctNew.PhiDichVu = 0;
                ctNew.TongPhiDichVu = 0;
                ctNew.TenViTri = '';
                ctNew.ThoiGianThucHien = 0;

                // lo hang
                var quanLiTheoLo = ctNew.QuanLyTheoLoHang;
                let dateLot = GetNgaySX_NgayHH(ctNew);
                ctNew.NgaySanXuat = dateLot.NgaySanXuat;
                ctNew.NgayHetHan = dateLot.NgayHetHan;

                ctNew.DM_LoHang = [];
                ctNew.LotParent = quanLiTheoLo;

                // check exist in cthdLoHang
                if ($.inArray(arrCTsort[i].ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                    arrIDQuiDoi.unshift(arrCTsort[i].ID_DonViQuiDoi);
                    ctNew.SoThuTu = cthdLoHang.length + 1;
                    ctNew.IDRandom = CreateIDRandom('RandomCT_');
                    if (quanLiTheoLo) {
                        // push DM_Lo
                        let objLot = $.extend({}, ctNew);
                        objLot.DM_LoHang = [];
                        objLot.HangCungLoais = [];
                        ctNew.DM_LoHang.push(objLot);
                    }
                    cthdLoHang.push(arrCTsort[i]);
                }
                else {
                    // find in cthdLoHang with same ID_QuiDoi
                    for (let j = 0; j < cthdLoHang.length; j++) {
                        if (cthdLoHang[j].ID_DonViQuiDoi === ctNew.ID_DonViQuiDoi) {
                            if (quanLiTheoLo) {
                                let objLot = $.extend({}, ctNew);
                                objLot.LotParent = false;
                                objLot.IDRandom = CreateIDRandom('RandomCT_');
                                objLot.DM_LoHang = [];
                                objLot.HangCungLoais = [];
                                cthdLoHang[j].DM_LoHang.push(objLot);
                            }
                            else {
                                ctNew.IDRandom = CreateIDRandom('RandomCT_');
                                ctNew.LaConCungLoai = true;
                                cthdLoHang[j].HangCungLoais.push(arrCTsort[i]);
                            }
                            break;
                        }
                    }
                }
            }
            // sort CTHD by SoThuTu desc
            cthdLoHang = cthdLoHang.sort(function (a, b) {
                var x = a.SoThuTu, y = b.SoThuTu;
                return x < y ? 1 : x > y ? -1 : 0;
            });
            localStorage.setItem('lcCTHDTHSaoChep', JSON.stringify(cthdLoHang));
            self.gotoGara();
        }
        else {
            // Tra theo HD san co
            hdNew.MaHoaDonTraHang = hdNew.MaHoaDonGoc;
            hdNew.PhaiThanhToanDB = 0;// todo check when giamtruhd muagoc
            hdNew.TongGiaGocHangTra = 0;
            hdNew.TongChiPhiHangTra = 0;
            hdNew.TongChiPhi = 0;
            hdNew.TongTienTra = 0;
            hdNew.DaThanhToan = 0;
            hdNew.TongGiamGiaDB = 0;
            hdNew.PhaiThanhToan = 0;
            hdNew.PTGiamDB = RoundDecimal(hdNew.TongGiamGia === null ? 0 : hdNew.TongGiamGia / (item.TongTienHang - item.TongTienThue) * 100);
            hdNew.MaHoaDonTH_NVien = 'Trả hàng ' + hdNew.MaHoaDonGoc + ' - ' + hdNew.TenNhanVien;
            hdNew.PTThueDB = item.PTThueHoaDon;
            hdNew.TongThueDB = item.TongTienThue;


            ajaxHelper(BH_HoaDonUri + 'GetCTHoaDon_afterTraHang?idHoaDon=' + hdNew.ID_HoaDon, 'GET').done(function (data) {
                if (data === null || (data !== null && data.length === 0)) {
                    ShowMessage_Danger('Không còn mặt hàng để hoàn trả cho hóa đơn');
                    return;
                }
                // data contains {TyLeChuyenDoi, QuyCach, LaDonViChuan}
                if (data !== null && data.length > 0) {
                    let ctAfter = $.grep(data, function (x) {
                        return x.SoLuongConLai > 0;
                    });
                    if (ctAfter.length === 0) {
                        localStorage.removeItem('lcHDTHSaoChep');
                        ShowMessage_Danger('Không còn mặt hàng để hoàn trả cho hóa đơn');
                        return false;
                    }
                    var countThis = 0;
                    // update soluong damua to crhd
                    for (let i = 0; i < ctAfter.length; i++) {
                        for (let j = 0; j < self.BH_HoaDonChiTiets().length; j++) {
                            let itFor = self.BH_HoaDonChiTiets()[j];
                            if (itFor.ID_ChiTietGoiDV === ctAfter[i].ID) {
                                countThis += 1;
                                self.BH_HoaDonChiTiets()[j].SoLuongConLai = ctAfter[i].SoLuongConLai;
                                break;
                            }
                        }
                    }
                    if (countThis === 0) {
                        localStorage.removeItem('lcHDTHSaoChep');
                        ShowMessage_Danger('Không còn mặt hàng để hoàn trả cho hóa đơn');
                        return false;
                    }
                    // order by SoThuTu ASC --> group Hang Hoa by LoHang
                    ctAfter = self.BH_HoaDonChiTiets().sort(function (a, b) {
                        var x = a.SoThuTu,
                            y = b.SoThuTu;
                        return x < y ? -1 : x > y ? 1 : 0;
                    });

                    ctAfter = $.grep(ctAfter, function (x) {
                        return x.SoLuongConLai > 0;
                    })
                    var arrIDQuiDoi = [];
                    var cthdLoHang = [];
                    for (let i = 0; i < ctAfter.length; i++) {
                        let ctNew = $.extend({}, ctAfter[i]);
                        ctNew = SetDefaultPropertiesCTHD(ctNew, hdNew.MaHoaDon, hdNew.LoaiHoaDon);
                        ctNew.SoLuong = 0;
                        ctNew.GhiChu_NVThucHien = '';
                        ctNew.GhiChu_NVThucHienPrint = '';
                        ctNew.GhiChu_NVTuVan = '';
                        ctNew.GhiChu_NVTuVanPrint = '';
                        ctNew.SoLuongDaMua = ctNew.SoLuongConLai;
                        ctNew.DVTinhGiam = '%';
                        ctNew.GiaBan = ctNew.DonGia;// trahang: lay giaban chua tinh thue
                        ctNew.ThanhTien = 0;
                        ctNew.ThanhToan = 0;
                        if (ctNew.TienChietKhau > 0) {
                            if (ctNew.PTChietKhau === 0) {
                                ctNew.DVTinhGiam = 'VND';
                            }
                        }
                        ctNew.CssWarning = false;
                        ctNew.ChatLieu = '1';
                        if (hdNew.LoaiHoaDonGoc === 19) {
                            ctNew.ChatLieu = '2';
                        }
                        ctNew.ThanhPhan_DinhLuong = [];
                        ctNew.TongPhiDichVu = 0;
                        ctNew.LaPTPhiDichVu = false;
                        ctNew.PhiDichVu = false;
                        ctNew.TenViTri = '';
                        ctNew.ThoiGianThucHien = 0;
                        // lo hang
                        let quanLiTheoLo = ctNew.QuanLyTheoLoHang;
                        ctNew.QuanLyTheoLoHang = quanLiTheoLo;
                        ctNew.DM_LoHang = [];
                        ctNew.LotParent = quanLiTheoLo;

                        let dateLot = GetNgaySX_NgayHH(ctNew);
                        ctNew.NgaySanXuat = dateLot.NgaySanXuat;
                        ctNew.NgayHetHan = dateLot.NgayHetHan;

                        // get tpcombo
                        let combo = $.grep(vmThanhPhanCombo.AllComBo_ofHD, function (x) {
                            return x.ID_ParentCombo === ctNew.ID_ParentCombo;
                        });
                        if (combo.length > 0) {
                            ctNew.ThanhPhanComBo = combo;
                            ctNew = AssignThanhPhanComBo_toCTHD(ctNew);
                            for (let k = 0; k < ctNew.ThanhPhanComBo.length; k++) {
                                let forCb = ctNew.ThanhPhanComBo[k];
                                forCb.SoLuong = 0;
                                forCb.ThanhTien = 0;
                                forCb.PTChietKhau = 0;
                                forCb.TienChietKhau = 0;
                                forCb.ThanhToan = 0;
                                forCb.SoLuongDaMua = forCb.SoLuongConLai;
                                forCb.GiaBan = forCb.DonGia - forCb.GiamGia;
                                forCb.DonGia = forCb.GiaBan;
                                forCb.ID_ChiTietGoiDV = forCb.ID;
                            }
                        }
                        else {
                            ctNew.ThanhPhanComBo = [];
                        }

                        if ($.inArray(ctNew.ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                            arrIDQuiDoi.unshift(ctNew.ID_DonViQuiDoi);
                            // push CTHD
                            ctNew.SoThuTu = cthdLoHang.length + 1;
                            ctNew.IDRandom = CreateIDRandom('RandomCT_');
                            if (quanLiTheoLo) {
                                // push DM_Lo
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
                                    if (quanLiTheoLo) {
                                        // push DM_Lo
                                        let objLot = $.extend({}, ctNew);
                                        objLot.DM_LoHang = [];
                                        objLot.HangCungLoais = [];
                                        objLot.LotParent = false;
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
                    // sort CTHD by SoThuTu desc
                    cthdLoHang = cthdLoHang.sort(function (a, b) {
                        var x = a.SoThuTu, y = b.SoThuTu;
                        return x < y ? 1 : x > y ? -1 : 0;
                    });

                    localStorage.setItem('lcCTHDTHSaoChep', JSON.stringify(cthdLoHang));
                    self.gotoGara();
                }
            });
        }

        // HoaDon
        hdNew.TongTienThue = 0;// do tienthue nay bind at hd doitra
        hdNew.TongThueKhachHang = 0;
        hdNew.GiamTruThanhToanBaoHiem = 0;
        hdNew.HeaderBH_GiaTriPtram = 0;
        hdNew.HeaderBH_Type = 1;
        hdNew.CongThucBaoHiem = 0;
        hdNew.HoanTraThuKhac = 0;
        hdNew.TongTienHang = 0;
        hdNew.TongGiamGia = 0;
        hdNew.HoanTraTamUng = 0;
        hdNew.KhachDaTra = 0;
        hdNew.IsKhuyenMaiHD = false;
        hdNew.IsOpeningKMaiHD = false;
        hdNew.KhuyeMai_GiamGia = 0;
        hdNew.TTBangDiem = 0;
        hdNew.DiemGiaoDich = 0;
        hdNew.DiemQuyDoi = 0;
        hdNew.DiemHienTai = 0;
        hdNew.TienATM = 0;
        hdNew.DiemCong = 0; // use when KM_Cong diem
        hdNew.ID_NhomDTApplySale = null;  // apply giam gia theo nhom
        localStorage.setItem('lcHDTHSaoChep', JSON.stringify(hdNew));
    }
    function ResetPhieuThuChi() {
        self.GhiChu_PhieuThu('');
        self.TongTT_PhieuThu(0);
        self.NoSau(0);
        self.TienThua_PT(0);
        self.selectID_KhoanThu(undefined);
        self.ThuTuKhach('');// at list TraHang
        $(txtTienMat).val(0);
        $(txtTienATM).val(0);
        $(txtTienGui).val(0);
        $(txtTienTheGiaTri).val(0);
        $('#txtBillCode').val('');
    }
    self.showPopThanhToan = function (item) {
        item.DienThoaiBaoHiem = item.BH_SDT;

        if (self.CongTy().length > 0) {
            vmThanhToan.inforCongTy = {
                TenCongTy: self.CongTy()[0].TenCongTy,
                DiaChiCuaHang: self.CongTy()[0].DiaChi,
                LogoCuaHang: Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang,
                TenChiNhanh: $('#_txtTenDonVi').text(),
            };
        }
        vmThanhToan.showModalThanhToan(item);
    }
    $('#txtNgayTaoInput').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        SearchHoaDon();
    });
    // ThanhToanHD: POS, ChuyenKhoan
    self.ListAccountPOS = ko.observableArray();
    self.ListAccountChuyenKhoan = ko.observableArray();
    self.filterAcCK = ko.observable();
    self.filterAcPOS = ko.observable();
    self.selectID_POS = ko.observable();
    self.selectID_ChuyenKhoan = ko.observable();
    self.KhoanThuChis = ko.observableArray();
    self.KhoanChis = ko.observableArray();
    self.AllKhoanThuChis = ko.observableArray();
    self.selectID_KhoanThu = ko.observableArray();
    self.AllAccountBank = ko.observableArray();
    const txtTienMat = '#txtTienMat';
    const txtTienATM = '#txtTienATM';
    const txtTienGui = '#txtTienGui';
    const txtTienTheGiaTri = '#txtTienTheGiaTri_PhieuThu';
    function GetDM_TaiKhoanNganHang() {
        if (navigator.onLine) {
            ajaxHelper(Quy_HoaDonUri + 'GetAllTaiKhoanNganHang_ByDonVi?idDonVi=' + id_donvi, 'GET').done(function (x) {
                if (x.res === true) {
                    let data = x.data;
                    self.AllAccountBank(data);
                    vmThanhToan.listData.AccountBanks = data;

                    for (let i = 0; i < data.length; i++) {
                        if (data[i].TaiKhoanPOS === true) {
                            self.ListAccountPOS.push(data[i]);
                        }
                        else {
                            self.ListAccountChuyenKhoan.push(data[i]);
                        }
                    }
                }
            })
        }
    }
    function GetAllQuy_KhoanThuChi() {
        ajaxHelper(Quy_HoaDonUri + 'GetQuy_KhoanThuChi', 'GET').done(function (x) {
            if (x.res === true) {
                let data = x.data;
                var khoanthu = $.grep(data, function (x) {
                    return x.LaKhoanThu === true;
                });
                self.KhoanThuChis(khoanthu);
                var khoanchi = $.grep(data, function (x) {
                    return x.LaKhoanThu === false;
                });
                self.KhoanChis(khoanchi);
                vmThanhToan.listData.KhoanThuChis = data;
            }
        })
    }

    function AssignMoney_InHoaDonDebit(thuTuKhach) {
        // update TienThu for List HoaDonDebit 
        for (let i = 0; i < self.ListHDisDebit().length; i++) {
            if (thuTuKhach <= self.ListHDisDebit()[i].TienMat) {
                self.ListHDisDebit()[i].TienThu = thuTuKhach;
                $('#tienthu_' + self.ListHDisDebit()[i].ID).val(formatNumber(thuTuKhach));
                if (i === 0 && self.ListHDisDebit().length > 0) {
                    for (let j = 1; j < self.ListHDisDebit().length; j++) {
                        self.ListHDisDebit()[j].TienThu = 0;
                        $('#tienthu_' + self.ListHDisDebit()[j].ID).val("0");
                    }
                }
                break;
            }
            else {
                self.ListHDisDebit()[i].TienThu = self.ListHDisDebit()[i].TienMat;
                thuTuKhach = thuTuKhach - self.ListHDisDebit()[i].TienMat;
                $('#tienthu_' + self.ListHDisDebit()[i].ID).val(formatNumber(self.ListHDisDebit()[i].TienThu));
            }
        }
    }

    // use at HDTra
    self.CongVaoTK = ko.observable();
    self.TinhNoSau = function () {
        var noHienTai = self.NoHienTai();
        if (noHienTai === undefined) {
            noHienTai = 0;
        }
        formatNumberObj($('#txtThuTuKhach'));
        var thuTuKhach = formatNumberToFloat(self.ThuTuKhach());
        var tienThua = Math.round(thuTuKhach - noHienTai);
        self.TienThua_PT(tienThua);
        var noSau = Math.round(noHienTai - thuTuKhach);
        self.NoSau(noSau);
        AssignMoney_InHoaDonDebit(thuTuKhach);
        // update TienThu for List HoaDonDebit 
        for (let i = 0; i < self.ListHDisDebit().length; i++) {
            if (thuTuKhach <= self.ListHDisDebit()[i].TienMat) {
                self.ListHDisDebit()[i].TienThu = thuTuKhach;
                $('#tienthu_' + self.ListHDisDebit()[i].ID).val(formatNumber(thuTuKhach));
                if (i === 0 && self.ListHDisDebit().length > 0) {
                    for (let j = 1; j < self.ListHDisDebit().length; j++) {
                        self.ListHDisDebit()[j].TienThu = 0;
                        $('#tienthu_' + self.ListHDisDebit()[j].ID).val("0");
                    }
                }
                break;
            }
            else {
                self.ListHDisDebit()[i].TienThu = self.ListHDisDebit()[i].TienMat;
                thuTuKhach = thuTuKhach - self.ListHDisDebit()[i].TienMat;
                $('#tienthu_' + self.ListHDisDebit()[i].ID).val(formatNumber(self.ListHDisDebit()[i].TienThu));
            }
        }
        var tongTT = 0;
        for (let i = 0; i < self.ListHDisDebit().length; i++) {
            tongTT += self.ListHDisDebit()[i].TienThu;
        }
        self.TongTT_PhieuThu(tongTT);
    }

    self.formatDateTime = function () {
        $('.datepicker_mask').datetimepicker(
            {
                format: "d/m/Y H:i",
                mask: true,
                timepicker: false,
            });
    }

    function CheckQuyenExist(maquyen) {
        var role = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen === maquyen;
        });
        return role.length > 0;
    }

    function CheckRole_Invoice() {
        switch (loaiHoaDon) {
            case 1:
            case 25:
                self.RoleView_Invoice(CheckQuyenExist('HoaDon_XemDS'));
                self.RoleInsert_Invoice(CheckQuyenExist('HoaDon_ThemMoi'));
                self.RoleUpdate_Invoice(CheckQuyenExist('HoaDon_CapNhat'));
                self.RoleDelete_Invoice(CheckQuyenExist('HoaDon_Xoa'));
                self.RoleExport_Invoice(CheckQuyenExist('HoaDon_XuatFile'));
                self.Show_BtnEdit(CheckQuyenExist('HoaDon_SuaDoi'));
                self.Show_BtnOpenHD(CheckQuyenExist('HoaDon_CapNhatHDTamLuu'));
                self.RoleInsert_HoaDonBaoHanh(CheckQuyenExist('HoaDonBaoHanh_ThemMoi'));

                // hdHoTro
                self.RoleUpdate_HDHoTro(CheckQuyenExist('HoaDonHoTro_SuaDoi'));
                self.RoleDelete_HDHoTro(CheckQuyenExist('HoaDonHoTro_Xoa'));

                self.Show_BtnExcelDetail(self.RoleExport_Invoice());
                self.Show_BtnCopy(CheckQuyenExist('HoaDon_SaoChep'));
                self.Role_PrintHoaDon(CheckQuyenExist('HoaDon_In'));
                self.ThayDoi_NgayLapHD(CheckQuyenExist('HoaDon_ThayDoiThoiGian'));
                self.ThayDoi_NVienBan(CheckQuyenExist('HoaDon_ThayDoiNhanVien'));

                if (self.RoleView_Invoice()) {
                    SearchHoaDon();
                }
                else {
                    ShowMessage_Danger('Không có quyền xem danh sách ' + sLoai)
                }
                break;
            case 2:
                self.RoleView_Invoice(CheckQuyenExist('HoaDonBaoHanh_XemDS'));
                self.RoleInsert_HoaDonBaoHanh(CheckQuyenExist('HoaDonBaoHanh_ThemMoi'));
                self.RoleUpdate_Invoice(CheckQuyenExist('HoaDonBaoHanh_CapNhat'));
                self.RoleDelete_Invoice(CheckQuyenExist('HoaDonBaoHanh_Xoa'));

                self.Show_BtnCopy(CheckQuyenExist('HoaDonBaoHanh_SaoChep'));
                self.RoleExport_Invoice(CheckQuyenExist('HoaDonBaoHanh_XuatFile'));
                self.Role_PrintHoaDon(CheckQuyenExist('HoaDonBaoHanh_In'));

                if (self.RoleView_Invoice()) {
                    SearchHoaDon();
                }
                else {
                    ShowMessage_Danger('Không có quyền xem danh sách ' + sLoai)
                }
                break;
            case 3:
                self.RoleView_Order(CheckQuyenExist('DatHang_XemDS'));
                self.RoleInsert_Order(CheckQuyenExist('DatHang_ThemMoi'));
                self.RoleUpdate_Order(CheckQuyenExist('DatHang_CapNhat'));
                self.RoleDelete_Order(CheckQuyenExist('DatHang_Xoa'));
                self.RoleExport_Order(CheckQuyenExist('DatHang_XuatFile'));
                self.RoleApprove_Order(CheckQuyenExist('DatHang_DuyetBaoGia'));

                self.Show_BtnCopy(CheckQuyenExist('DatHang_SaoChep'));
                self.Role_PrintHoaDon(CheckQuyenExist('DatHang_In'));
                self.Show_BtnExcelDetail(self.RoleExport_Order());
                self.ThayDoi_NgayLapHD(CheckQuyenExist('DatHang_ThayDoiThoiGian'));
                self.ThayDoi_NVienBan(CheckQuyenExist('DatHang_ThayDoiNhanVien'));

                if (self.RoleView_Order()) {
                    SearchHoaDon();
                }
                else {
                    ShowMessage_Danger('Không có quyền xem danh sách ' + sLoai)
                }
                break;
            case 6:
                self.RoleView_Return(CheckQuyenExist('TraHang_XemDS'));
                self.RoleInsert_Return(CheckQuyenExist('TraHang_ThemMoi'));
                self.RoleUpdate_Return(CheckQuyenExist('TraHang_CapNhat'));
                self.RoleDelete_Return(CheckQuyenExist('TraHang_Xoa'));
                self.RoleExport_Return(CheckQuyenExist('TraHang_XuatFile'));

                self.Show_BtnCopy(CheckQuyenExist('TraHang_SaoChep'));
                self.Role_PrintHoaDon(CheckQuyenExist('TraHang_In'));
                self.Show_BtnExcelDetail(self.RoleExport_Return());

                CheckQuyen_HoaDonMua();

                if (self.RoleView_Return()) {
                    SearchHoaDon();
                }
                else {
                    ShowMessage_Danger('Không có quyền xem danh sách ' + sLoai)
                }
                break;
        }

        if (self.NganhKinhDoanh() === 3) {
            self.Show_BtnCopy(false);
        }
    }

    self.ListTypeMauIn = ko.observableArray();
    function loadMauIn() {
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetListMauIn?typeChungTu=' + dathangTeamplate + '&idDonVi=' + id_donvi,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                self.ListTypeMauIn(result);
            }
        });
    }
    self.PrinDatHang = function (item, key) {
        var cthdFormat = GetCTHDPrint_Format(self.BH_HoaDonChiTiets());
        self.CTHoaDonPrint(cthdFormat);

        var itemHDFormat = GetInforHDPrint(item, false);
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
                    + "; var item2=" + JSON.stringify(self.CTHoaDonPrintMH())
                    + "; var item3=" + JSON.stringify(itemHDFormat)
                    + "; var item4=" + JSON.stringify(self.HangMucSuaChuas())
                    + "; var item5=" + JSON.stringify(self.VatDungKemTheos())
                    + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(data);
            }
        });
    }
    self.Print_ListTempDoiTra = function (item, key) {
        var cthdTraHang = GetCTHDPrint_Format(self.BH_HoaDonChiTiets());
        self.CTHoaDonPrint(cthdTraHang);
        var cthdDoiTra = GetCTHDPrint_Format(item.BH_HoaDon_ChiTiet);

        for (let i = 0; i < cthdDoiTra.length; i++) {
            let nvth = '';
            let nvtv = '';
            for (let j = 0; j < cthdDoiTra[i].BH_NhanVienThucHien.length; j++) {
                let nvien = cthdDoiTra[i].BH_NhanVienThucHien[j];
                switch (nvien.ThucHien_TuVan) {
                    case true:
                        nvth += nvien.TenNhanVien + ', ';
                        break;
                    case false:
                        nvtv += nvien.TenNhanVien + ', ';
                        break;
                }
            }
            cthdDoiTra[i].GhiChu_NVThucHien = Remove_LastComma(nvth);
            cthdDoiTra[i].GhiChu_NVTuVan = Remove_LastComma(nvtv);
            cthdDoiTra[i].GhiChu_NVThucHienPrint = Remove_LastComma(nvth);
            cthdDoiTra[i].GhiChu_NVTuVanPrint = Remove_LastComma(nvtv);
        }

        self.CTHoaDonPrintMH(cthdDoiTra);
        var tongTienHDTra = 0;
        var phiTraHang = 0;
        // get tongTienTra, phiTraHang from HD Tra
        for (let i = 0; i < self.HoaDons().length; i++) {
            let itFor = self.HoaDons()[i];
            if (itFor.ID === item.ID_HoaDon) {
                tongTienHDTra = itFor.TongTienHang + itFor.TongTienThue - itFor.TongGiamGia;
                phiTraHang = itFor.TongChiPhi;
                break;
            }
        }
        item.TongTienTraHang = tongTienHDTra;
        item.PhiTraHang = phiTraHang;
        item.TongTienTra = tongTienHDTra - phiTraHang;
        var phaiTT = formatNumberToFloat(item.PhaiThanhToan) - tongTienHDTra;
        if (phaiTT > 0) {
            // khach phai tra them tien
            item.TongCong = phaiTT;
            item.PhaiTraKhach = 0;
        }
        else {
            item.TongCong = tongTienHDTra;
            item.PhaiTraKhach = tongTienHDTra;
        }
        var sumGiamGiaHang = item.BH_HoaDon_ChiTiet.reduce(function (_this, val) {
            return formatNumberToFloat(_this) + formatNumberToFloat(val.TienChietKhau);
        }, 0);
        var itemHDFormat = GetInforHDPrint(item, true);
        item.TongGiamGiaHang = formatNumber(sumGiamGiaHang);
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
                    + "; var item4=[], item5=[]; var item2=" + JSON.stringify(self.CTHoaDonPrintMH())
                    + ";var item3=" + JSON.stringify(self.InforHDprintf()) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>"); // MauInTeamplate.js: used to bind data in knockout
                PrintExtraReport(data); // assign content HTML into frame
            }
        });
    }
    // check format date
    const isCorrectFormat = (dateString, format) => {
        return moment(dateString, format, true).isValid()
    }
    // auto update NhomKhach(TraHang --> nâng nhóm , HuyHD --> hạ nhóm)
    self.DM_NhomDoiTuong_ChiTiets = ko.observableArray();
    function GetDM_NhomDoiTuong_ChiTiets() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/DM_NhomDoiTuongAPI/' + 'GetDM_NhomDoiTuong_ChiTiets?idDonVi=' + id_donvi, 'GET').done(function (x) {
                let data = x.data;
                if (data.length > 0) {
                    self.DM_NhomDoiTuong_ChiTiets(data);
                }
            });
        }
    }

    async function GetInforCus(id) {
        if (!commonStatisJs.CheckNull(id)) {
            var date = moment(new Date()).format('YYYY-MM-DD HH:mm');
            var xx = await ajaxHelper(DMDoiTuongUri + "GetInforKhachHang_ByID?idDoiTuong=" + id + '&idChiNhanh=' + VHeader.IdDonVi
                + '&timeStart=' + date + '&timeEnd=' + date + '&wasChotSo=false', 'GET').done(function () {
                })
                .then(function (data) {
                    if (data !== null && data.length > 0) {
                        return data[0];
                    }
                    return {};
                })
            return xx;
        }
        return {};
    }

    // used to get NoHienTai when pritn hoadon
    function GetInforKhachHangFromDB_ByID(id, loaiDoiTuong = 1) {
        var date = moment(new Date()).format('YYYY-MM-DD HH:mm');
        if (navigator.onLine && id !== null && id !== const_GuidEmpty) {
            ajaxHelper(DMDoiTuongUri + "GetInforKhachHang_ByID?idDoiTuong=" + id + '&idChiNhanh=' + id_donvi
                + '&timeStart=' + date + '&timeEnd=' + date + '&wasChotSo=false', 'GET').done(function (data) {
                    if (data !== null) {
                        switch (loaiDoiTuong) {
                            case 1:
                                self.ChiTietDoiTuong(data);
                                break;
                            case 3:
                                self.InforBaoHiem(data);
                                break;
                        }
                    }
                    else {
                        self.InforBaoHiem([]);
                        self.ChiTietDoiTuong([]);
                    }
                });
        }
        else {
            self.InforBaoHiem([]);
            self.ChiTietDoiTuong([]);
        }
    }

    function Insert_ManyNhom(lstNhom) {
        var myData = {};
        myData.lstDM_DoiTuong_Nhom = lstNhom;
        $.ajax({
            data: myData,
            url: DMDoiTuongUri + "PostDM_DoiTuong_Nhom",
            type: 'POST',
            async: false,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
            },
            error: function (jqXHR, textStatus, errorThrown) {
                ShowMessage_Danger('Thêm mới nhóm khách hàng thất bại');
            },
        })
    }
    // search and paging CTHD
    self.PageResult_CTHoaDons = ko.computed(function (item) {
        // filter
        var filter = self.filterHangHoa_ChiTietHD();
        var arrFilter = ko.utils.arrayFilter(self.BH_HoaDonChiTiets(), function (prod) {
            var chon = true;
            var ipLodau = locdau(filter);
            var maHH = locdau(prod.MaHangHoa);
            var tenHH = locdau(prod.TenHangHoa);
            var maLoHang = locdau(prod.MaLoHang);
            var kitudau = GetChartStart(tenHH);
            if (chon && filter) {
                chon = maHH.indexOf(ipLodau) > -1 || tenHH.indexOf(ipLodau) > -1
                    || maLoHang.indexOf(ipLodau) > -1 || kitudau.indexOf(ipLodau) > -1
                    ;
            }
            return chon;
        });
        var lenData = arrFilter.length;
        self.PageCount_CTHD(Math.ceil(lenData / self.PageSize_CTHD()));
        self.TotalRecord_CTHD(lenData);
        // paging
        var first = self.currentPage_CTHD() * self.PageSize_CTHD();
        if (arrFilter !== null) {
            return arrFilter.slice(first, first + self.PageSize_CTHD());
        }
    })
    self.PageList_CTHD = ko.computed(function () {
        var arrPage = [];
        var allPage = self.PageCount_CTHD();
        var currentPage = self.currentPage_CTHD();
        if (allPage > 4) {
            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPage_CTHD()) + 1;
            }
            else {
                i = self.currentPage_CTHD();
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
                    if (currentPage === 1) {
                        for (let j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            let obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                    else {
                        // get currentPage - 2 , currentPage, currentPage + 2
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
        if (self.PageResult_CTHoaDons().length > 0) {
            self.fromitem_CTHD((self.currentPage_CTHD() * self.PageSize_CTHD()) + 1);
            if (((self.currentPage_CTHD() + 1) * self.PageSize_CTHD()) > self.PageResult_CTHoaDons().length) {
                var ss = (self.currentPage_CTHD() + 1) * self.PageSize_CTHD();
                var fromItem = (self.currentPage_CTHD() + 1) * self.PageSize_CTHD();
                if (fromItem < self.TotalRecord_CTHD()) {
                    self.toitem_CTHD((self.currentPage_CTHD() + 1) * self.PageSize_CTHD());
                }
                else {
                    self.toitem_CTHD(self.TotalRecord_CTHD());
                }
            } else {
                self.toitem_CTHD((self.currentPage_CTHD() * self.PageSize_CTHD()) + self.PageSize_CTHD());
            }
        }
        return arrPage;
    });
    self.VisibleStartPage_CTHD = ko.computed(function () {
        if (self.PageList_CTHD().length > 0) {
            return self.PageList_CTHD()[0].pageNumber !== 1;
        }
    });
    self.VisibleEndPage_CTHD = ko.computed(function () {
        if (self.PageList_CTHD().length > 0) {
            return self.PageList_CTHD()[self.PageList_CTHD().length - 1].pageNumber !== self.PageCount_CTHD();
        }
    })
    self.ResetCurrentPage_CTHD = function () {
        self.currentPage_CTHD(0);
    };
    self.GoToPage_CTHD = function (page) {
        self.currentPage_CTHD(page.pageNumber - 1);
    };
    self.GetClass_CTHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage_CTHD()) ? "click" : "";
    };
    self.StartPage_CTHD = function () {
        self.currentPage_CTHD(0);
    }
    self.BackPage_CTHD = function () {
        if (self.currentPage_CTHD() > 1) {
            self.currentPage_CTHD(self.currentPage_CTHD() - 1);
        }
    }
    self.GoToNextPage_CTHD = function () {
        if (self.currentPage_CTHD() < self.PageCount_CTHD() - 1) {
            self.currentPage_CTHD(self.currentPage_CTHD() + 1);
        }
    }
    self.EndPage_CTHD = function () {
        if (self.currentPage_CTHD() < self.PageCount_CTHD() - 1) {
            self.currentPage_CTHD(self.PageCount_CTHD() - 1);
        }
    }
    self.DM_MauIn = ko.observableArray();
    function GetAllMauIn_byChiNhanh() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/ThietLapApi/GetAllMauIn_ByChiNhanh?idChiNhanh=' + id_donvi, 'GET').done(function (data) {
                if (data !== null) {
                    self.DM_MauIn(data);
                }
            });
        }
    }
    self.DMKhuyenMai = ko.observableArray();
    function GetKM_CTKhuyenMai() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/BH_KhuyenMaiAPI/' + 'GetKM_CTKhuyenMai?idDonVi=' + id_donvi, 'GET').done(function (data) {
                if (data !== null) {
                    self.DMKhuyenMai(data);
                    GetList_KMApDung();
                }
            });
        }
    }
    function Check_KhuyenMai_Active(idKhuyenMai) {
        var itemKM = $.grep(self.DMKhuyenMai(), function (x) {
            return x.ID_KhuyenMai === idKhuyenMai;
        });
        if (itemKM.length > 0) {
            // check Han su dung
            var now = moment(new Date()).format('YYYY-MM-DD');
            var ngayHetHan = moment(itemKM[0].ThoiGianKetThuc).format('YYYY-MM-DD');
            if (ngayHetHan >= now) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    }
    // saochep/trahang co KM
    self.KM_KMApDung = ko.observableArray();
    self.NhomHangHoas = ko.observableArray();
    function GetList_KMApDung() {
        for (let i = 0; i < self.DMKhuyenMai().length; i++) {
            var itemKM = self.DMKhuyenMai()[i];
            var objKhuyenMai = {
                ID_KhuyenMai: null,
                TenKhuyenMai: "",
                HinhThuc: "", // 21: Mua hang giam hang; 22: mua hang tang hang; 23: mua hang tang diem; 24: gia ban theo SL Mua
                HinhThucKhuyenMai: "",
                Months: [],
                Dates: [],
                Hours: [],
                Days: [],
                ApDungNgaySinh: 0,
                ID_QuyDoiMuas: [],
                ID_QuyDoiTangs: [],
                ID_NhomHHMuas: [],
                ID_NhomHHTangs: [],
                ID_NhomKHs: [],
                ID_NhanViens: [],
                DM_KhuyenMai_ChiTiet: [],
                Note_HinhThuc: '', // ghi chu tuong ung voi tung loai hinh thuc KM
            };
            objKhuyenMai.ID_KhuyenMai = itemKM.ID;
            objKhuyenMai.TenKhuyenMai = itemKM.TenKhuyenMai;
            objKhuyenMai.HinhThucKhuyenMai = itemKM.TenHinhThucKM;
            objKhuyenMai.HinhThuc = itemKM.HinhThuc;
            objKhuyenMai.ApDungNgaySinh = itemKM.ApDungNgaySinhNhat;
            if (itemKM.ThangApDung !== '') {
                objKhuyenMai.Months = itemKM.ThangApDung.split('_');
            }
            if (itemKM.NgayApDung !== '') {
                objKhuyenMai.Dates = itemKM.NgayApDung.split('_');
            }
            if (itemKM.ThuApDung !== '') {
                objKhuyenMai.Days = itemKM.ThuApDung.split('_');
            }
            if (itemKM.GioApDung !== '') {
                objKhuyenMai.Hours = itemKM.GioApDung.split('_');
            }
            for (let j = 0; j < itemKM.DM_KhuyenMai_ApDung.length; j++) {
                var itemKM_AD = itemKM.DM_KhuyenMai_ApDung[j];
                if (itemKM_AD.ID_NhomKhachHang !== null && $.inArray(itemKM_AD.ID_NhomKhachHang, objKhuyenMai.ID_NhomKHs) === -1) {
                    objKhuyenMai.ID_NhomKHs.push(itemKM_AD.ID_NhomKhachHang);
                }
                if (itemKM_AD.ID_NhanVien !== null && $.inArray(itemKM_AD.ID_NhanVien, objKhuyenMai.ID_NhanViens) === -1) {
                    objKhuyenMai.ID_NhanViens.push(itemKM_AD.ID_NhanVien);
                }
            }
            for (let j = 0; j < itemKM.DM_KhuyenMai_ChiTiet.length; j++) {
                var itemKMCT = itemKM.DM_KhuyenMai_ChiTiet[j];
                if (itemKMCT.ID_DonViQuiDoiMua !== null && $.inArray(itemKMCT.ID_DonViQuiDoiMua, objKhuyenMai.ID_QuyDoiMuas) === -1) {
                    objKhuyenMai.ID_QuyDoiMuas.push(itemKMCT.ID_DonViQuiDoiMua);
                }
                if (itemKMCT.ID_DonViQuiDoi !== null && $.inArray(itemKMCT.ID_DonViQuiDoi, objKhuyenMai.ID_QuyDoiTangs) === -1) {
                    objKhuyenMai.ID_QuyDoiTangs.push(itemKMCT.ID_DonViQuiDoi);
                }
                if (itemKMCT.ID_NhomHangHoa !== null) {
                    var arrNhomChildTang = GetAll_IDNhomChild_ofNhomHH([itemKMCT.ID_NhomHangHoa]);
                    if (arrNhomChildTang.length > 0) {
                        for (let k = 0; k < arrNhomChildTang.length; k++) {
                            if ($.inArray(arrNhomChildTang[k], objKhuyenMai.ID_NhomHHTangs) === -1) {
                                objKhuyenMai.ID_NhomHHTangs.push(arrNhomChildTang[k]);
                            }
                        }
                    }
                }
                if (itemKMCT.ID_NhomHangHoaMua !== null) {
                    var arrNhomChildMua = GetAll_IDNhomChild_ofNhomHH([itemKMCT.ID_NhomHangHoaMua]);
                    if (arrNhomChildMua.length > 0) {
                        for (let k = 0; k < arrNhomChildMua.length; k++) {
                            if ($.inArray(arrNhomChildMua[k], objKhuyenMai.ID_NhomHHMuas) === -1) {
                                objKhuyenMai.ID_NhomHHMuas.push(arrNhomChildMua[k]);
                            }
                        }
                    }
                }
            }
            // sort to do check SoLuongMua tương ứng với GiaKhuyenMai
            itemKM.DM_KhuyenMai_ChiTiet = itemKM.DM_KhuyenMai_ChiTiet.sort(function (a, b) {
                var x = a.SoLuongMua,
                    y = b.SoLuongMua;
                return x < y ? -1 : x > y ? 1 : 0;
            });
            objKhuyenMai.DM_KhuyenMai_ChiTiet = itemKM.DM_KhuyenMai_ChiTiet;
            self.KM_KMApDung.push(objKhuyenMai);
        }
        // remove KMai tang diem if HeThong khong cai dat tinh nang tich diem
        if (self.ThietLap().TinhNangTichDiem === false) {
            var listKM = $.grep(self.KM_KMApDung(), function (x) {
                return x.HinhThuc !== 14 && x.HinhThuc !== 23;
            });
            self.KM_KMApDung(listKM);
        }
    }
    function CheckKM_IsApDung(idNhanVien) {
        var isApDung = false;
        var dtNow = new Date();
        var _month = (dtNow.getMonth() + 1).toString(); // 1-12 (+1 because getMoth() return 0-11)
        var _date = (dtNow.getDate()).toString(); // 1- 31
        var _hours = (dtNow.getHours()).toString(); // 1-24
        var _day = (dtNow.getDay() + 1).toString(); // mon:2, tues:3, wed:4, thur:5, fri:6, sat: 7, sun: 8
        var _weekofMonth = Math.ceil(dtNow.getDate() / 7); // get week of Month ( 1- 5);
        var idNhomKH = null;
        var ngaysinhFull = 0;
        var ngaysinh = 0;
        var thangsinh = 0;
        var tuansinh = 0;
        var ctDoiTuong = self.ChiTietDoiTuong();
        // if chose KH --> get idNhomKH + ngaysinh
        if (ctDoiTuong !== null && ctDoiTuong.length > 0) {
            idNhomKH = ctDoiTuong[0].ID_NhomDoiTuong.toLowerCase();
            ngaysinhFull = ctDoiTuong[0].NgaySinh_NgayTLap;
        }
        if (ngaysinhFull !== 0 && ngaysinhFull !== null) {
            var dtNgaySinh = new Date(moment(ngaysinhFull).format('YYYY-MM-DD')); // must format 'YYYY-MM-DD'
            // get day, moth from dayFull
            ngaysinh = dtNgaySinh.getDate();
            thangsinh = (dtNgaySinh.getMonth() + 1).toString();
            tuansinh = Math.ceil(dtNgaySinh.getDate() / 7);
        }
        // get list KM by ID_DoiTuong and ID_NhanVien
        var arrKM_forDT = [];
        for (let i = 0; i < self.KM_KMApDung().length; i++) {
            // get KM apply for HangHoa
            var xItem = self.KM_KMApDung()[i];
            if (xItem.ApDungNgaySinh !== 0) {
                // if chose KH
                if (ctDoiTuong !== null && ctDoiTuong.length > 0) {
                    // check ID_NhanVien, ID_nhomKH
                    if ((xItem.Months.length === 0 || $.inArray(_month, xItem.Months) > -1)
                        && (xItem.Dates.length === 0 || $.inArray(_date, xItem.Dates) > -1)
                        && (xItem.Days.length === 0 || $.inArray(_day, xItem.Days) > -1)
                        && (xItem.Hours.length === 0 || $.inArray(_hours, xItem.Hours) > -1)
                        && (xItem.ID_NhanViens.length === 0 || $.inArray(idNhanVien, xItem.ID_NhanViens) > -1)
                        && (xItem.ID_NhomKHs.length === 0 || $.inArray(idNhomKH, xItem.ID_NhomKHs) > -1)) {
                        switch (xItem.ApDungNgaySinh) {
                            case 1: // ap dung ngay sinh theo ngay
                                if (ngaysinh.toString() === _date) {
                                    isApDung = true;
                                }
                                break;
                            case 2: // ap dung ngay sinh theo tuan
                                if (tuansinh.toString() === _weekofMonth) {
                                    isApDung = true;
                                }
                                break;
                            case 3: // ap dung ngay sinh theo thang
                                if (thangsinh.toString() === _month) {
                                    isApDung = true;// esc for KM_KMApDung
                                }
                                break;
                        }
                    }
                }
            }
            else {
                // if ApDungNgaySinh = 0
                if ((xItem.Months.length === 0 || $.inArray(_month, xItem.Months) > -1)
                    && (xItem.Dates.length === 0 || $.inArray(_date, xItem.Dates) > -1)
                    && (xItem.Days.length === 0 || $.inArray(_day, xItem.Days) > -1)
                    && (xItem.Hours.length === 0 || $.inArray(_hours, xItem.Hours) > -1)
                    && (xItem.ID_NhanViens.length === 0 || $.inArray(idNhanVien, xItem.ID_NhanViens) > -1)
                    && (xItem.ID_NhomKHs.length === 0 || $.inArray(idNhomKH, xItem.ID_NhomKHs) > -1)) {
                    isApDung = true;
                }
            }
        }
        return isApDung;
    }
    function GetAllNhomHangHoas() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/DM_NhomHangHoaAPI/' + 'GetDM_NhomHangHoa', 'GET').done(function (data) {
                if (data !== null) {
                    for (let i = 0; i < data.length; i++) {
                        if (data[i].ID_Parent === null) {
                            var objParent = {
                                ID: data[i].ID,
                                TenNhomHangHoa: data[i].TenNhomHangHoa,
                                Childs: [],
                            }
                            for (let j = 0; j < data.length; j++) {
                                if (data[j].ID !== data[i].ID && data[j].ID_Parent === data[i].ID) {
                                    var objChild =
                                    {
                                        ID: data[j].ID,
                                        TenNhomHangHoa: data[j].TenNhomHangHoa,
                                        ID_Parent: data[i].ID,
                                        Child2s: []
                                    };
                                    for (let k = 0; k < data.length; k++) {
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
                }
            });
        }
    };
    function GetAll_IDNhomChild_ofNhomHH(arrIDNhomHang) {
        var arrNhomHHChilds = [];
        // get all IDChild of ID_Parent
        var lc_NhomHangHoas = self.NhomHangHoas();
        for (let j = 0; j < lc_NhomHangHoas.length; j++) {
            if (lc_NhomHangHoas[j].Childs.length > 0 && $.inArray(lc_NhomHangHoas[j].Childs[0].ID_Parent, arrIDNhomHang) > -1) {
                for (let k = 0; k < lc_NhomHangHoas[j].Childs.length; k++) {
                    arrNhomHHChilds.push(lc_NhomHangHoas[j].Childs[k].ID);
                    if (lc_NhomHangHoas[j].Childs[k].Child2s.length > 0) {
                        for (let i = 0; i < lc_NhomHangHoas[j].Childs[k].Child2s.length; i++) {
                            arrNhomHHChilds.push(lc_NhomHangHoas[j].Childs[k].Child2s[i].ID);
                        }
                    }
                }
            }
        }
        // add ID_Parent into arrNhomHHChilds
        for (let i = 0; i < arrIDNhomHang.length; i++) {
            arrNhomHHChilds.push(arrIDNhomHang[i]);
        }
        return arrNhomHHChilds;
    }

    self.CheckHD_DaXuatKho = function (item, type) {
        if (type === 2) {
            var loaiCheck = item.LoaiHoaDon == 3 ? 25 : 8;
            $.getJSON('/api/DanhMuc/GaraAPI/' + 'CheckHoaDon_DaXuLy?idHoaDon=' + item.ID + '&loaiHoaDon=' + loaiCheck).done(function (x) {
                if (x == true) {
                    if (item.LoaiHoaDon === 3) {
                        commonStatisJs.ShowMessageDanger('Báo giá đã tạo hóa đơn. Không thể sửa đổi');
                    }
                    else {
                        commonStatisJs.ShowMessageDanger('Hóa đơn đã xuất kho. Không thể sửa đổi');
                    }
                    return;
                }
                modelGiaoDich.SaoChepHD_KhuyenMai(item, type);
            });
        }
        else {
            self.SaoChepHD_KhuyenMai(item, type);
        }
    }

    self.UpdateThongTinBaoHiem = function (item) {
        vmThongTinThanhToanBaoHiem.inforLogin = {
            ID_DonVi: VHeader.IdDonVi,
            ID_NhanVien: VHeader.IdNhanVien,
            UserLogin: VHeader.UserLogin,
        };
        vmThongTinThanhToanBaoHiem.showModaUpdate(item);
    }

    function GetPTChietKhauHang_HeaderBH() {
        var ptCKHang = 0;
        var arrCTsort = self.BH_HoaDonChiTiets();
        var arr = $.grep(arrCTsort, function (x) {
            return x.PTChietKhau === arrCTsort[0].PTChietKhau;
        });
        if (arrCTsort.length > 0 && arr.length === arrCTsort.length) {
            ptCKHang = arrCTsort[0].PTChietKhau;
        }

        var gtri = 0;
        var arrDonGiaBH = $.grep(arrCTsort, function (x) {
            return x.DonGiaBaoHiem > 0;
        });
        if (arrDonGiaBH.length === arrCTsort.length) {
            gtri = RoundDecimal(arrDonGiaBH[0].DonGiaBaoHiem / arrDonGiaBH[0].GiaBan * 100, 3);
        }
        return {
            PTChietKhauHH: ptCKHang,
            HeaderBH: gtri,
        }
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

    self.SaoChepHD_KhuyenMai = function (item, type) {
        var newHD = $.extend({}, item);

        var gtrisudung = formatNumberToFloat(newHD.GiaTriSDDV);
        if (gtrisudung > 0) {
            if (type === 0) {
                ShowMessage_Danger('Vui lòng không sao chép hóa đơn sử dụng gói dịch vụ');
                return false;
            }
        }

        var maHD = 'Copy' + newHD.MaHoaDon;
        var phaiTT = formatNumberToFloat(newHD.PhaiThanhToan);
        var khachdatra = formatNumberToFloat(newHD.KhachDaTra);
        // nếu TT= điểm --> caculator DiemQuyDoi from TTBangDiem
        let diemquydoi = 0;
        let tiendoidiem = 0;
        if (self.ThietLap_TichDiem() !== null && self.ThietLap_TichDiem().length > 0) {
            tiendoidiem = newHD.TienDoiDiem;
            diemquydoi = Math.floor(tiendoidiem * self.ThietLap_TichDiem()[0].DiemThanhToan / self.ThietLap_TichDiem()[0].TienThanhToan);
        }

        var obj = GetPTChietKhauHang_HeaderBH(item);
        newHD.PTChietKhauHH = obj.PTChietKhauHH;
        newHD.HeaderBH_GiaTriPtram = 0;
        newHD.HeaderBH_Type = 1;
      
        newHD.TienMat = 0;
        newHD.TienATM = 0;
        newHD.TienGui = 0;
        newHD.TienTheGiaTri = 0;
        newHD.TTBangDiem = 0;

        if (commonStatisJs.CheckNull(item.CongThucBaoHiem)) {
            newHD.CongThucBaoHiem = 0;
        }
        if (commonStatisJs.CheckNull(item.GiamTruThanhToanBaoHiem)) {
            newHD.GiamTruThanhToanBaoHiem = 0;
        }

        // assign again ID = constGuid_Empty at BanLe.js
        switch (type) {
            case 0:// saochep
                newHD.KhachDaTra = 0;
                newHD.BaoHiemDaTra = 0;
                newHD.DaThanhToan = self.IsGara() ? 0 : newHD.PhaiThanhToan;
                newHD.TTBangDiem = tiendoidiem;
                newHD.DiemQuyDoi = diemquydoi;
                newHD.DiemGiaoDichDB = 0;
                newHD.DiemGiaoDich = item.DiemGiaoDich;
                newHD.TrangThaiHD = 1;
                newHD.TienMat = newHD.PhaiThanhToan;
                newHD.TienTheGiaTri = newHD.ThuTuThe;
                SetCache_ifGara('TN_copyHD');
                SetCache_ifNotGara(3);
                break;
            case 1:// update HDTamLuu
                maHD = newHD.MaHoaDon;
                newHD.TrangThaiHD = 3;
                newHD.TTBangDiem = 0;
                newHD.DiemQuyDoi = 0;
                newHD.DiemGiaoDichDB = 0;
                newHD.DaThanhToan = self.IsGara() ? 0 : phaiTT - khachdatra; // số tiền còn lại phaiTT --> bind at BanHang
                newHD.TienMat = newHD.DaThanhToan;
                newHD.TienATM = 0;
                newHD.TienGui = 0;
                newHD.TienTheGiaTri = 0;
                newHD.ID_TaiKhoanPos = null;
                newHD.ID_TaiKhoanChuyenKhoan = null;
                SetCache_ifGara('TN_updateHD');
                SetCache_ifNotGara(7);
                break;
            case 2:// updateHD daTT
                maHD = newHD.MaHoaDon;
                newHD.TrangThaiHD = 8;
                newHD.TTBangDiem = 0;
                newHD.DiemQuyDoi = 0;
                newHD.DiemGiaoDichDB = newHD.DiemGiaoDich; // tru diem giaodich HD cu
                newHD.DaThanhToan = self.IsGara() ? 0 : phaiTT - khachdatra;
                newHD.TienMat = self.IsGara() ? 0 : newHD.DaThanhToan; // = số tiền còn lại phaiTT
                newHD.TienATM = 0;
                newHD.TienGui = 0;
                newHD.TienTheGiaTri = 0;
                newHD.ID_TaiKhoanPos = null;
                newHD.ID_TaiKhoanChuyenKhoan = null;

                SetCache_ifGara('TN_updateHD');
                SetCache_ifNotGara(8);
                break;
            case 3:// baohanh
                maHD = newHD.MaHoaDon;
                newHD.LoaiHoaDon = 2;
                newHD.TrangThaiHD = 1;
                newHD.ID_HoaDon = newHD.ID;
                newHD.TongTienHang = 0;
                newHD.TongGiamGia = 0;
                newHD.TongChietKhau = 0;
                newHD.TongTienThue = 0;
                newHD.TongThanhToan = 0;
                newHD.PTThueHoaDon = 0;
                newHD.PhaiThanhToan = 0;
                newHD.DaThanhToan = 0;
                newHD.KhachDaTra = 0;
                newHD.TienMat = 0;
                newHD.TienGui = 0;
                newHD.TienATM = 0;
                newHD.TienTheGiaTri = 0;
                newHD.TienDoiDiem = 0;
                newHD.TienDatCoc = 0;
                SetCache_ifGara('TN_copyHD');
                SetCache_ifNotGara(9);
                break;
        }
        var lstKMCTHD = localStorage.getItem('productKM_HoaDon');
        if (lstKMCTHD !== null) {
            lstKMCTHD = JSON.parse(lstKMCTHD);
        }
        else {
            lstKMCTHD = [];
        }
        // remove item old in KhuyenMai if same MaHoaDon && add new
        lstKMCTHD = $.grep(lstKMCTHD, function (x) {
            return x.MaHoaDon !== maHD;
        });

        var note_KMaiHD = '';
        if (self.BH_HoaDonChiTiets() !== null) {
            var giamgiaKM_HD = newHD.KhuyeMai_GiamGia;
            newHD.Status = 1;
            newHD.MaHoaDonDB = type == 3 ? '' : maHD;
            newHD.YeuCau = 1;
            newHD.ChoThanhToan = false;
            newHD.StatusOffline = false;
            newHD.DVTinhGiam = 'VND';
            if (newHD.TongChietKhau > 0) {
                newHD.DVTinhGiam = '%';
            }
            newHD.HoanTraThuKhac = 0;
            newHD.PhaiThanhToanDB = 0;
            newHD.TongGiaGocHangTra = 0;
            newHD.TongChiPhiHangTra = 0;
            newHD.TongTienTra = 0;
            newHD.PTGiamDB = 0;
            newHD.TongGiamGiaDB = 0;
            newHD.HoanTraTamUng = 0;
            newHD.IsKhuyenMaiHD = false;
            newHD.IsOpeningKMaiHD = false;
            newHD.KhuyeMai_GiamGia = 0;
            newHD.TongGiamGiaKM_HD = newHD.TongGiamGia + giamgiaKM_HD;
            newHD.PTThueDB = 0;
            newHD.TongThueDB = 0;
            newHD.TongTienHangChuaCK = type === 3 ? 0 : self.TongTienHangChuaCK();
            newHD.TongGiamGiaHang = type === 3 ? 0 : self.TongGiamGiaHang();

            newHD.DiemHienTai = 0;
            newHD.DiemCong = 0;
            newHD.SoDuDatCoc = 0;

            newHD.TongTienKhuyenMai_CT = 0;
            newHD.TongGiamGiaKhuyenMai_CT = 0;

            newHD.DiemKhuyenMai = 0;
            newHD.ID_NhomDTApplySale = null;
            newHD.IsActive = '';
            // Goi dich vu
            newHD.NgayApDungGoiDV = null;
            newHD.HanSuDungGoiDV = null;
            if (newHD.ID_ViTri !== const_GuidEmpty) {
                newHD.CreateTime = FormatDatetime_AMPM(new Date());
            }
            else {
                newHD.CreateTime = 0;
                newHD.ID_ViTri = null;
            }
            newHD.TenViTriHD = newHD.TenPhongBan;
            newHD.ChoThanhToan = false; // set default = false
            // order by SoThuTu ASC --> group Hang Hoa by LoHang
            var arrCTsort = self.BH_HoaDonChiTiets().sort(function (a, b) {
                var x = a.SoThuTu,
                    y = b.SoThuTu;
                return x < y ? -1 : x > y ? 1 : 0;
            });
            var arrIDQuiDoi = [];
            var cthdLoHang = [];
            for (let i = 0; i < arrCTsort.length; i++) {
                let cthd = $.extend({}, arrCTsort[i]);
                let idKhuyenMai = cthd.ID_KhuyenMai;

                // if muahang - giamhang (chinh no): push , nguoclai not push
                if (cthd.ID_TangKem !== null) {
                    if (idKhuyenMai === null || idKhuyenMai === const_GuidEmpty) {
                        continue;
                    }
                }
                cthd = SetDefaultPropertiesCTHD(cthd, newHD.MaHoaDon, newHD.LoaiHoaDon);
                cthd.SoLuongDaMua = 0;
                cthd.TienChietKhau = cthd.GiamGia;
                cthd.DVTinhGiam = '%';
                cthd.GiaBan = formatNumberToFloat(cthd.DonGia);
                if (cthd.TienChietKhau > 0 && cthd.PTChietKhau === 0) {
                    cthd.DVTinhGiam = 'VND';
                }
                cthd.ID_ViTri = newHD.ID_ViTri;
                cthd.TenViTri = newHD.TenPhongBan;
                if (cthd.ChatLieu === '4') {
                    cthd.SoLuongConLai = cthd.SoLuongDVConLai;
                }

                let quanLiTheoLo = cthd.QuanLyTheoLoHang;
                cthd.QuanLyTheoLoHang = quanLiTheoLo;
                cthd.DM_LoHang = [];
                cthd.LotParent = quanLiTheoLo;
                if (type === 2) {
                    cthd.TonKho = cthd.TonKho + cthd.SoLuong;
                }
                //saochep: khong gan id_baoduong
                if (type === 0) {
                    cthd.ID_LichBaoDuong = null;
                }

                let dateLot = GetNgaySX_NgayHH(cthd);
                cthd.NgaySanXuat = dateLot.NgaySanXuat;
                cthd.NgayHetHan = dateLot.NgayHetHan;

                // PhiDichVu, LaPTPhiDichVu (get from DB store)
                cthd.TongPhiDichVu = cthd.SoLuong * cthd.PhiDichVu;
                if (cthd.LaPTPhiDichVu) {
                    cthd.TongPhiDichVu
                        = Math.round(cthd.SoLuong * cthd.GiaBan * cthd.PhiDichVu / 100);
                }

                // get tpcombo
                let combo = $.grep(vmThanhPhanCombo.AllComBo_ofHD, function (x) {
                    return x.ID_ParentCombo === cthd.ID_ParentCombo;
                });
                if (combo.length > 0) {
                    cthd.ThanhPhanComBo = combo;
                    cthd = AssignThanhPhanComBo_toCTHD(cthd);
                }
                else {
                    cthd.ThanhPhanComBo = [];
                }

                cthd = AssignNVThucHien_toCTHD(cthd);
                cthd = AssignTPDinhLuong_toCTHD(cthd);

                // check KhuyenMai
                if (idKhuyenMai !== null && idKhuyenMai !== '00000000-0000-0000-0000-000000000000') {
                    let itemKM = $.grep(self.KM_KMApDung(), function (x) {
                        return x.ID_KhuyenMai === idKhuyenMai;
                    });
                    if (self.ThietLap().KhuyenMai === true && CheckKM_IsApDung(newHD.ID_NhanVien) && itemKM.length > 0) {
                        // if TangKem of HoaDon (ID_TangKem = null, ID_KhuyenMai !=null)
                        // if TangKem of HangHoa (ID_TangKem = ID_QuiDoi of HangTang, ID_KhuyenMai == null)
                        // save cache KhuyenMai of HoaDon
                        if (cthd.TangKem && $.inArray(itemKM[0].HinhThuc, [11, 12, 13, 14]) > -1) {// neu khuyenmai hoadon
                            // if was push KhuyenMai HoaDon --> not push 
                            for (let m = 0; m < lstKMCTHD.length; m++) {
                                if (lstKMCTHD[m].ID_KhuyenMai === idKhuyenMai && lstKMCTHD[m].MaHoaDon === cthd.MaHoaDon) {
                                    continue;
                                }
                            }
                            // find all Hang KhuyenMai of HoaDon
                            let hangTangHoaDon = $.grep(cthd, function (x) {
                                return x.ID_KhuyenMai === idKhuyenMai;
                            });
                            let exitsKM = $.grep(lstKMCTHD, function (x) {
                                return x.ID_KhuyenMai === newHD.ID_KhuyenMai;
                            })
                            if (exitsKM.length === 0) {
                                let noteDetail = '';
                                for (let m = 0; m < hangTangHoaDon.length; m++) {
                                    // assign proprties hangTangHoaDon
                                    hangTangHoaDon[m] = SetDefaultPropertiesCTHD(hangTangHoaDon[m], maHD, newHD.LoaiHoaDon);
                                    hangTangHoaDon[m].SoLuongDaMua = 0;
                                    hangTangHoaDon[m].TienChietKhau = hangTangHoaDon[m].GiamGia;
                                    hangTangHoaDon[m].DVTinhGiam = '%';
                                    hangTangHoaDon[m].GiaBan = hangTangHoaDon[m].DonGia;
                                    hangTangHoaDon[m].ThanhTien = (hangTangHoaDon[m].GiaBan - hangTangHoaDon[m].TienChietKhau) * hangTangHoaDon[m].SoLuong;
                                    if (hangTangHoaDon[m].TienChietKhau > 0 && hangTangHoaDon[m].PTChietKhau === 0) {
                                        hangTangHoaDon[m].DVTinhGiam = 'VND';
                                    }
                                    hangTangHoaDon[m].CssWarning = false;
                                    hangTangHoaDon[m].ID_ChiTietGoiDV = null;
                                    hangTangHoaDon[m].ThanhPhan_DinhLuong = [];
                                    hangTangHoaDon[m].TongPhiDichVu = 0;
                                    hangTangHoaDon[m].PhiDichVu = 0;
                                    hangTangHoaDon[m].LaPTPhiDichVu = false;
                                    hangTangHoaDon[m].ID_ViTri = null;
                                    hangTangHoaDon[m].TenViTri = '';
                                    hangTangHoaDon[m].ThoiGian = moment(new Date()).format('YYYY-MM-DD HH:mm');
                                    hangTangHoaDon[m].ThoiGianThucHien = 0;
                                    // lo hang
                                    hangTangHoaDon[m].QuanLyTheoLoHang = quanLiTheoLo;
                                    hangTangHoaDon[m].DM_LoHang = [];
                                    hangTangHoaDon[m].LotParent = quanLiTheoLo;
                                    noteDetail += hangTangHoaDon[m].SoLuong + ' ' + hangTangHoaDon[m].MaHangHoa + ', ';
                                    lstKMCTHD.push(hangTangHoaDon[m]);
                                    localStorage.setItem('productKM_HoaDon', JSON.stringify(lstKMCTHD))
                                }
                                // find CTKhuyenMai was ApDung in this HoaDon
                                let tongTienHang = 0;
                                let isGiamGiaPT = 0;
                                let gtriGiamGia = 0;
                                // sort DM_KhuyenMai_ChiTiet by TongTienHang (get DM gan nhat voi TongTienHang)
                                itemKM[0].DM_KhuyenMai_ChiTiet = itemKM[0].DM_KhuyenMai_ChiTiet.sort(function (a, b) {
                                    let x = a.TongTienHang, y = b.TongTienHang;
                                    return x > y ? 1 : x < y ? -1 : 0;
                                });
                                for (let j = 0; j < itemKM[0].DM_KhuyenMai_ChiTiet.length; j++) {
                                    if (itemKM[0].DM_KhuyenMai_ChiTiet[j].TongTienHang <= newHD.TongTienHang) {
                                        gtriGiamGia = itemKM[0].DM_KhuyenMai_ChiTiet[j].GiamGia;
                                        tongTienHang = itemKM[0].DM_KhuyenMai_ChiTiet[j].TongTienHang;
                                        isGiamGiaPT = itemKM[0].DM_KhuyenMai_ChiTiet[j].GiamGiaTheoPhanTram;
                                    }
                                }
                                note_KMaiHD = itemKM[0].TenKhuyenMai + ': Tổng tiền hàng từ ' + formatNumber(tongTienHang);
                                switch (itemKM[0].HinhThuc) {
                                    case 12:// tang hang
                                        note_KMaiHD += ' tặng ' + Remove_LastComma(noteDetail);
                                        break;
                                    case 13:// giam gia hang
                                        newHD.KhuyeMai_GiamGia = giamgiaKM_HD;
                                        note_KMaiHD += ' giảm giá '.concat(isGiamGiaPT ? gtriGiamGia + ' %' : formatNumber(gtriGiamGia) + ' Đ', ' cho ', Remove_LastComma(noteDetail));
                                        break;
                                }
                                // update infor cache HoaDon when apply KhuyenMai
                                newHD.KhuyenMai_GhiChu = note_KMaiHD;
                                newHD.IsKhuyenMaiHD = true;
                                newHD.IsOpeningKMaiHD = true;
                            }
                            continue; // not add hangtangHoaDon in cache CTHD
                        }
                        else {
                            // find hang tang kem of this HangHoa
                            let hhTangKem = $.grep(cthd, function (x) {
                                return x.ID_TangKem === cthd.ID_DonViQuiDoi;
                            });
                            let txtKhuyenMai_Last = '';
                            // kmai theo hanghoa or diemcong
                            if (hhTangKem.length > 0 || cthd.DiemKhuyenMai > 0) {
                                // chi add neu khong phai khuyenmai congdiem
                                if (cthd.TangKem === false && cthd.DiemKhuyenMai === 0) {
                                    // get all HangTang of this HangHoa
                                    for (let m = 0; m < hhTangKem.length; m++) {
                                        // assign proprties hhTangKem
                                        hhTangKem[m] = SetDefaultPropertiesCTHD(hhTangKem[m], newHD.MaHoaDon, newHD.LoaiHoaDon);
                                        hhTangKem[m].SoLuongDaMua = 0;
                                        hhTangKem[m].TienChietKhau = hhTangKem[m].GiamGia;
                                        hhTangKem[m].DVTinhGiam = '%';
                                        hhTangKem[m].GiaBan = hhTangKem[m].DonGia;
                                        hhTangKem[m].ThanhTien = (hhTangKem[m].GiaBan - hhTangKem[m].TienChietKhau) * hhTangKem[m].SoLuong;
                                        if (hhTangKem[m].TienChietKhau > 0 && hhTangKem[m].PTChietKhau === 0) {
                                            hhTangKem[m].DVTinhGiam = 'VND';
                                        }
                                        hhTangKem[m].CssWarning = false;
                                        hhTangKem[m].ID_ChiTietGoiDV = null;
                                        hhTangKem[m].ThanhPhan_DinhLuong = [];
                                        hhTangKem[m].TongPhiDichVu = 0;
                                        hhTangKem[m].PhiDichVu = 0;
                                        hhTangKem[m].LaPTPhiDichVu = false;
                                        hhTangKem[m].ID_ViTri = null;
                                        hhTangKem[m].TenViTri = '';
                                        hhTangKem[m].ThoiGian = moment(new Date()).format('YYYY-MM-DD HH:mm');
                                        hhTangKem[m].ThoiGianThucHien = 0;

                                        hhTangKem[m].QuanLyTheoLoHang = quanLiTheoLo;
                                        hhTangKem[m].DM_LoHang = [];
                                        hhTangKem[m].LotParent = quanLiTheoLo;
                                        cthd.HangHoa_KM.push(hhTangKem[m]);
                                        // VD: 3 hang 001, 1 hang 002
                                        txtKhuyenMai_Last += hhTangKem[m].SoLuong + ' ' + hhTangKem[m].MaHangHoa + ', ';
                                    }
                                    txtKhuyenMai_Last = Remove_LastComma(txtKhuyenMai_Last);
                                }

                                // find all Hang with same ID_KhuyenMai
                                let cthd_sameKM = $.grep(cthd, function (x) {
                                    return x.ID_KhuyenMai === idKhuyenMai;
                                });
                                let textKM_First = '';
                                for (let k = 0; k < cthd_sameKM.length; k++) {
                                    textKM_First += cthd_sameKM[k].SoLuong + ' ' + cthd_sameKM[k].MaHangHoa + ', ';
                                }
                                textKM_First = 'Khi mua ' + Remove_LastComma(textKM_First);
                                // set ghichu KhuyenMai for CTHD
                                let isGiamGiaPTram = false;
                                let gtriGiamGia = 0;
                                let giaKhuyenMai = 0;
                                let loaiKM_note = '';
                                let soluongMua_ThucTe = 0;
                                let itemCTApDung = [];
                                let arrNhomChilds = [];
                                // sort DM_KhuyenMai_ChiTiet by SoLuongMua (get DM gan nhat voi SoLuongMua)
                                itemKM[0].DM_KhuyenMai_ChiTiet = itemKM[0].DM_KhuyenMai_ChiTiet.sort(function (a, b) {
                                    var x = a.SoLuongMua, y = b.SoLuongMua;
                                    return x > y ? 1 : x < y ? -1 : 0;
                                });

                                for (let k = 0; k < itemKM[0].DM_KhuyenMai_ChiTiet.length; k++) {
                                    let itemCT = itemKM[0].DM_KhuyenMai_ChiTiet[k];
                                    // check ID_QuiDoi Mua thuoc KMai {ID_QuiDoiMua OR ID_NhomHHMua }
                                    arrNhomChilds = GetAll_IDNhomChild_ofNhomHH([itemCT.ID_NhomHangHoaMua]);
                                    if ($.inArray(cthd.ID_DonViQuiDoi, itemKM[0].ID_QuyDoiMuas) > -1
                                        || $.inArray(cthd.ID_NhomHangHoa, arrNhomChilds) > -1) {
                                        // if Kmai by ID_QuiDoi
                                        if (cthd.ID_DonViQuiDoi === itemCT.ID_DonViQuiDoiMua) {
                                            //isTangNhom = false;
                                            itemCTApDung = itemCT;
                                        }
                                        else {
                                            // Kmai by Nhom
                                            itemCTApDung = itemCT;
                                        }
                                        if (itemCTApDung != []) {
                                            break;
                                        }
                                    }
                                }
                                if (itemCTApDung != []) {
                                    isGiamGiaPTram = itemCTApDung.GiamGiaTheoPhanTram;
                                    gtriGiamGia = itemCTApDung.GiamGia;
                                    giaKhuyenMai = itemCTApDung.GiaKhuyenMai;
                                    isGiamGiaPTram = itemCTApDung.GiamGiaTheoPhanTram;
                                    // KMai by Nhom
                                    if (itemCTApDung.ID_NhomHangHoaMua !== null) {
                                        // get all CTHD thuoc nhom KM --> tinh soluong mua
                                        for (let n = 0; n < cthd.length; n++) {
                                            if ($.inArray(cthd[n].ID_NhomHangHoa, arrNhomChilds) > -1) {
                                                soluongMua_ThucTe += cthd[n].SoLuong;
                                            }
                                        }
                                    }
                                    else {
                                        // Kmai by ID_QuiDoi
                                        soluongMua_ThucTe = cthd.SoLuong;
                                    }
                                }
                                loaiKM_note = textKM_First;
                                switch (itemKM[0].HinhThuc) {
                                    case 21:
                                        if (isGiamGiaPTram) {
                                            loaiKM_note += ' giảm giá ' + gtriGiamGia + '% cho ' + txtKhuyenMai_Last;
                                        }
                                        else {
                                            loaiKM_note += ' giảm giá ' + gtriGiamGia + ' cho ' + txtKhuyenMai_Last;
                                        }
                                        break;
                                    case 22:
                                        loaiKM_note += ' tặng ' + txtKhuyenMai_Last;
                                        break;
                                    case 23:
                                        if (isGiamGiaPTram) {
                                            loaiKM_note += ' tặng ' + gtriGiamGia + '% điểm ';
                                        }
                                        else {
                                            // nhân theo Soluong Mua (chi tinh Soluong tai thoi diem mua hien tai)
                                            gtriGiamGia = Math.floor(gtriGiamGia * Math.floor(soluongMua_ThucTe / itemCTApDung.SoLuongMua));
                                            loaiKM_note += ' tặng ' + gtriGiamGia + ' điểm ';
                                        }
                                        break;
                                    case 24:
                                        loaiKM_note += ' giá ' + formatNumber(giaKhuyenMai);
                                        break;
                                }
                                // if hanghoa same ID_KhuyenMai --> neu da add KMai: not assign IsOpeningKMai, ISKhuyenMai = true
                                let existKM = $.grep(cthd, function (x) {
                                    return x.ID_KhuyenMai === idKhuyenMai && x.IsKhuyenMai === true;
                                });
                                if (existKM.length === 0) {
                                    cthd.IsOpeningKMai = true;
                                    cthd.IsKhuyenMai = true;
                                    cthd.TenKhuyenMai = itemKM[0].TenKhuyenMai + ': ' + loaiKM_note;
                                }
                            }// end if hangTangKem > 0
                        } // end else KM HangHoa
                    } // end if IsApDung
                }
                // check exist in cthdLoHang
                if ($.inArray(cthd.ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                    arrIDQuiDoi.unshift(cthd.ID_DonViQuiDoi);
                    cthd.SoThuTu = cthdLoHang.length + 1;
                    cthd.IDRandom = CreateIDRandom('RandomCT_');
                    if (quanLiTheoLo) {
                        // push DM_Lo
                        let objLot = $.extend({}, cthd);
                        objLot.HangCungLoais = [];
                        objLot.DM_LoHang = [];
                        cthd.DM_LoHang.push(objLot);
                    }
                    cthdLoHang.push(cthd);
                }
                else {
                    // find in cthdLoHang with same ID_QuiDoi
                    for (let j = 0; j < cthdLoHang.length; j++) {
                        if (cthdLoHang[j].ID_DonViQuiDoi === cthd.ID_DonViQuiDoi) {
                            if (quanLiTheoLo) {
                                // push DM_Lo
                                let objLot = $.extend({}, cthd);
                                objLot.LotParent = false;
                                objLot.HangCungLoais = [];
                                objLot.DM_LoHang = [];
                                objLot.IDRandom = CreateIDRandom('RandomCT_');
                                cthdLoHang[j].DM_LoHang.push(objLot);
                            }
                            else {
                                cthd.IDRandom = CreateIDRandom('RandomCT_');
                                cthd.LaConCungLoai = true;
                                cthdLoHang[j].HangCungLoais.push(cthd);
                            }
                            break;
                        }
                    }
                }
            }

            let cacheCP = localStorage.getItem('lcChiPhi');
            if (cacheCP !== null) {
                cacheCP = JSON.parse(cacheCP);
                // remove & add again
                cacheCP = $.grep(cacheCP, function (x) {
                    return x.ID_HoaDon !== item.ID;
                });
            }
            else {
                cacheCP = [];
            }
            if (VueChiPhi.ListChiPhi.length > 0) {
                let arrCP = $.extend([], true, VueChiPhi.ListChiPhi);
                for (let m = 0; m < arrCP.length; m++) {
                    let for1 = arrCP[m];
                    for (let n = for1.ChiTiets.length - 1; n > -1; n--) {
                        let for2 = for1.ChiTiets[n];
                        if (for2.ID_NhaCungCap === null) {
                            for1.ChiTiets.splice(n, 1);
                        }
                    }
                    cacheCP.push(for1);
                }
                for (let m = cacheCP.length - 1; m > -1; m--) {
                    if (cacheCP[m].ChiTiets.length === 0) {
                        cacheCP.splice(m, 1);
                    }
                }
            }
            if (cacheCP.length > 0) {
                localStorage.setItem('lcChiPhi', JSON.stringify(cacheCP));
            }

            // sort CTHD by SoThuTu desc
            cthdLoHang = cthdLoHang.sort(function (a, b) {
                var x = a.SoThuTu, y = b.SoThuTu;
                return x < y ? 1 : x > y ? -1 : 0;
            });

            if (type === 3) {// baohanh --> reset dongia, thanhtien
                for (let i = 0; i < cthdLoHang.length; i++) {
                    cthdLoHang[i].DonGia = 0;
                    cthdLoHang[i].GiaBan = 0;
                    cthdLoHang[i].PTThue = 0;
                    cthdLoHang[i].TienThue = 0;
                    cthdLoHang[i].PTChietKhau = 0;
                    cthdLoHang[i].TienChietKhau = 0;
                    cthdLoHang[i].ThanhToan = 0;
                    cthdLoHang[i].ThanhTien = 0;
                    cthdLoHang[i].ID_ChiTietGoiDV = cthdLoHang[i].ID;

                    for (let j = 0; j < cthdLoHang[i].DM_LoHang.length; j++) {
                        cthdLoHang[i].DM_LoHang[j].DonGia = 0;
                        cthdLoHang[i].DM_LoHang[j].GiaBan = 0;
                        cthdLoHang[i].DM_LoHang[j].PTThue = 0;
                        cthdLoHang[i].DM_LoHang[j].TienThue = 0;
                        cthdLoHang[i].DM_LoHang[j].PTChietKhau = 0;
                        cthdLoHang[i].DM_LoHang[j].TienChietKhau = 0;
                        cthdLoHang[i].DM_LoHang[j].ThanhTien = 0;
                        cthdLoHang[i].DM_LoHang[j].ThanhToan = 0;
                        cthdLoHang[i].DM_LoHang[j].ID_ChiTietGoiDV = cthdLoHang[i].DM_LoHang[j].ID;
                    }
                    for (let j = 0; j < cthdLoHang[i].HangCungLoais.length; j++) {
                        cthdLoHang[i].HangCungLoais[j].DonGia = 0;
                        cthdLoHang[i].HangCungLoais[j].GiaBan = 0;
                        cthdLoHang[i].HangCungLoais[j].PTThue = 0;
                        cthdLoHang[i].HangCungLoais[j].TienThue = 0;
                        cthdLoHang[i].HangCungLoais[j].PTChietKhau = 0;
                        cthdLoHang[i].HangCungLoais[j].TienChietKhau = 0;
                        cthdLoHang[i].HangCungLoais[j].ThanhTien = 0;
                        cthdLoHang[i].HangCungLoais[j].ThanhToan = 0;
                        cthdLoHang[i].HangCungLoais[j].ID_ChiTietGoiDV = cthdLoHang[i].HangCungLoais[j].ID;
                    }
                    for (let j = 0; j < cthdLoHang[i].ThanhPhanComBo.length; j++) {
                        cthdLoHang[i].ThanhPhanComBo[j].DonGia = 0;
                        cthdLoHang[i].ThanhPhanComBo[j].GiaBan = 0;
                        cthdLoHang[i].ThanhPhanComBo[j].PTThue = 0;
                        cthdLoHang[i].ThanhPhanComBo[j].TienThue = 0;
                        cthdLoHang[i].ThanhPhanComBo[j].PTChietKhau = 0;
                        cthdLoHang[i].ThanhPhanComBo[j].TienChietKhau = 0;
                        cthdLoHang[i].ThanhPhanComBo[j].ThanhTien = 0;
                        cthdLoHang[i].ThanhPhanComBo[j].ThanhToan = 0;
                        cthdLoHang[i].ThanhPhanComBo[j].ID_ChiTietGoiDV = cthdLoHang[i].ThanhPhanComBo[j].ID;
                    }
                }
            }
            // tinh ThoiGianThucHien DichVu
            var totalTime = 0;
            if (newHD.ID_ViTri !== null) {
                for (let i = 0; i < cthdLoHang.length; i++) {
                    totalTime += cthdLoHang[i].ThoiGianThucHien;
                }
            }
            newHD.ThoiGianThucHien = totalTime;
            // chietkhau NV hoadon
            if (type !== 1) {
                for (let k = 0; k < newHD.BH_NhanVienThucHiens.length; k++) {
                    newHD.BH_NhanVienThucHiens[k].IDRandom = CreateIDRandom('CKHD_');
                    newHD.BH_NhanVienThucHiens[k].ChietKhauMacDinh = newHD.BH_NhanVienThucHiens[k].PT_ChietKhau;
                    if (newHD.BH_NhanVienThucHiens[k].TinhChietKhauTheo === 3)
                        newHD.BH_NhanVienThucHiens[k].ChietKhauMacDinh = newHD.BH_NhanVienThucHiens[k].TienChietKhau / newHD.BH_NhanVienThucHiens[k].HeSo;
                }
            }
            else {
                // update HDTamLuu: khong get chietkhau NV, vi ThucThu HoaDon co the bi thay doi --> chietkhau old se khong con dung
                newHD.BH_NhanVienThucHiens = [];
            }
            // khuyenMai: hinhthuc 11, 14: giam gia HD, cong diem HD (todo 14: congdiem)
            if (newHD.ID_KhuyenMai !== null && newHD.ID_KhuyenMai !== '00000000-0000-0000-0000-000000000000') {
                let itemKM = $.grep(self.KM_KMApDung(), function (x) {
                    return x.ID_KhuyenMai === newHD.ID_KhuyenMai;
                });
                var exitsKM2 = $.grep(lstKMCTHD, function (x) {
                    return x.ID_KhuyenMai === newHD.ID_KhuyenMai;
                })
                // if not exist KM in hangKM _HoaDon
                if (exitsKM2.length === 0 && self.ThietLap().KhuyenMai === true && CheckKM_IsApDung(newHD.ID_NhanVien) && itemKM.length > 0) {
                    // find CTKhuyenMai was ApDung in this HoaDon
                    let tongTienHang = 0;
                    let isGiamGiaPT = 0;
                    let gtriGiamGia = 0;
                    // sort DM_KhuyenMai_ChiTiet by TongTienHang (get DM gan nhat voi TongTienHang)
                    itemKM[0].DM_KhuyenMai_ChiTiet = itemKM[0].DM_KhuyenMai_ChiTiet.sort(function (a, b) {
                        var x = a.TongTienHang, y = b.TongTienHang;
                        return x > y ? 1 : x < y ? -1 : 0;
                    });
                    for (let j = 0; j < itemKM[0].DM_KhuyenMai_ChiTiet.length; j++) {
                        if (itemKM[0].DM_KhuyenMai_ChiTiet[j].TongTienHang <= newHD.TongTienHang) {
                            gtriGiamGia = itemKM[0].DM_KhuyenMai_ChiTiet[j].GiamGia;
                            tongTienHang = itemKM[0].DM_KhuyenMai_ChiTiet[j].TongTienHang;
                            isGiamGiaPT = itemKM[0].DM_KhuyenMai_ChiTiet[j].GiamGiaTheoPhanTram;
                        }
                    }
                    note_KMaiHD = itemKM[0].TenKhuyenMai + ': Tổng tiền hàng từ ' + formatNumber(tongTienHang);
                    switch (itemKM[0].HinhThuc) {
                        case 11:
                            note_KMaiHD += ' giảm giá ' + gtriGiamGia + ' ' + (isGiamGiaPT ? '%' : 'Đ') + ' cho hóa đơn';
                            break;
                        case 14:
                            note_KMaiHD += ' tặng ' + gtriGiamGia + ' ' + (isGiamGiaPT ? ' % điểm' : ' điểm') + ' cho hóa đơn';
                            break;
                    }
                    // update infor cache HoaDon when apply KhuyenMai
                    newHD.KhuyenMai_GhiChu = note_KMaiHD;
                    newHD.IsKhuyenMaiHD = true;
                    newHD.IsOpeningKMaiHD = true;
                    newHD.KhuyeMai_GiamGia = giamgiaKM_HD;
                }
            }
            delete newHD['BH_HoaDon_ChiTiet']
            // save cache HoaDon after check KhuyenMai
            localStorage.setItem('lcHDSaoChep', JSON.stringify(newHD));
            localStorage.setItem('lcCTHDSaoChep', JSON.stringify(cthdLoHang));
            self.gotoGara();
        }
        else {
            ShowMessage_Danger('Không có chi tiết hóa đơn');
            return false;
        }
    }
    function AssignNVThucHien_toCTHD(itemCT) {
        var listBH_NVienThucHienOld = itemCT.BH_NhanVienThucHien;
        itemCT.BH_NhanVienThucHien = [];// reset BH_NhanVienThucHien old, and add again
        var nvTH = '';
        var nvTV = '';
        var nvTH_Print = '';
        var nvTV_Print = '';
        for (let j = 0; j < listBH_NVienThucHienOld.length; j++) {
            let itemFor = listBH_NVienThucHienOld[j];
            let isNVThucHien = itemFor.ThucHien_TuVan;
            let tienCK = itemFor.TienChietKhau;
            let gtriPtramCK = itemFor.PT_ChietKhau;
            let isPTram = gtriPtramCK > 0 ? true : false;
            let gtriCK_TH = 0;
            let gtriCK_TV = 0;
            let tacVu = 1;
            let ckMacDinh = gtriPtramCK;
            if (isNVThucHien) {
                if (itemFor.TheoYeuCau) {
                    tacVu = 3;  // thuchien theo yeucau
                }
                else {
                    tacVu = 1;
                }
                if (isPTram) {
                    gtriCK_TH = gtriPtramCK;
                    nvTH += itemFor.TenNhanVien + ' (' + gtriCK_TH + ' %), ';
                }
                else {
                    gtriCK_TH = tienCK;
                    nvTH += itemFor.TenNhanVien + ' (' + formatNumber(gtriCK_TH) + ' đ), ';
                    ckMacDinh = tienCK / itemFor.HeSo / itemCT.SoLuong;
                }
                nvTH_Print += itemFor.TenNhanVien + ', ';
            }
            else {
                tacVu = 2;
                if (isPTram) {
                    gtriCK_TV = gtriPtramCK;
                    nvTV += itemFor.TenNhanVien + ' (' + gtriCK_TV + ' %), ';
                }
                else {
                    gtriCK_TV = tienCK;
                    nvTV += itemFor.TenNhanVien + ' (' + formatNumber(gtriCK_TV) + ' đ), ';
                    ckMacDinh = tienCK / itemFor.HeSo / itemCT.SoLuong;
                }
                nvTV_Print += itemFor.TenNhanVien + ', ';
            }
            var idRandom = CreateIDRandom('IDRandomCK_');
            var itemNV = {
                IDRandom: idRandom,
                ID_NhanVien: itemFor.ID_NhanVien,
                TenNhanVien: itemFor.TenNhanVien,
                ThucHien_TuVan: isNVThucHien,
                TienChietKhau: tienCK,
                PT_ChietKhau: gtriPtramCK,
                TheoYeuCau: itemFor.TheoYeuCau,
                TacVu: tacVu,
                HeSo: itemFor.HeSo,
                TinhChietKhauTheo: itemFor.TinhChietKhauTheo,
                ChietKhauMacDinh: ckMacDinh,
                TinhHoaHongTruocCK: itemFor.TinhHoaHongTruocCK,
            }
            itemCT.BH_NhanVienThucHien.push(itemNV);
        }

        itemCT.GhiChu_NVThucHien = nvTH === '' ? '' : 'Thực hiện: ' + Remove_LastComma(nvTH);
        itemCT.GhiChu_NVThucHienPrint = nvTH_Print === '' ? '' : Remove_LastComma(nvTH_Print);
        itemCT.GhiChu_NVTuVan = nvTV === '' ? '' : 'Tư vấn: ' + Remove_LastComma(nvTV);
        itemCT.GhiChu_NVTuVanPrint = nvTV_Print === '' ? '' : Remove_LastComma(nvTV_Print);
        itemCT.HoaHongTruocChietKhau =
            listBH_NVienThucHienOld != null && listBH_NVienThucHienOld.length > 0 ?
                listBH_NVienThucHienOld[0].TinhHoaHongTruocCK : itemCT.HoaHongTruocChietKhau;
        return itemCT;
    }
    function AssignTPDinhLuong_toCTHD(itemCT) {
        if (itemCT.ThanhPhan_DinhLuong !== null && itemCT.ThanhPhan_DinhLuong !== undefined) {
            for (let k = 0; k < itemCT.ThanhPhan_DinhLuong.length; k++) {
                let tpDL = itemCT.ThanhPhan_DinhLuong[k];
                itemCT.ThanhPhan_DinhLuong[k].STT = k + 1;
                itemCT.ThanhPhan_DinhLuong[k].isDefault = false;
                itemCT.ThanhPhan_DinhLuong[k].IDRandom = CreateIDRandom('TPDL_');
                itemCT.ThanhPhan_DinhLuong[k].SoLuongQuyCach = tpDL.SoLuong * tpDL.QuyCach;
                itemCT.ThanhPhan_DinhLuong[k].SoLuongDinhLuong_BanDau = tpDL.SoLuong / itemCT.SoLuong;
                itemCT.ThanhPhan_DinhLuong[k].SoLuongMacDinh = itemCT.ThanhPhan_DinhLuong[k].SoLuongDinhLuong_BanDau; // assign = SoLuongDinhLuong_BanDau
                itemCT.ThanhPhan_DinhLuong[k].GiaVonAfter = tpDL.SoLuong * tpDL.GiaVon;
            }
        }
        else {
            itemCT.ThanhPhan_DinhLuong = [];
        }
        return itemCT;
    }

    function AssignThanhPhanComBo_toCTHD(itemCT) {
        if (itemCT.ThanhPhanComBo !== null && itemCT.ThanhPhanComBo !== undefined) {
            for (let k = 0; k < itemCT.ThanhPhanComBo.length; k++) {
                let combo = itemCT.ThanhPhanComBo[k];
                combo.IDRandom = CreateIDRandom('combo_');
                combo = AssignNVThucHien_toCTHD(combo);
                combo = AssignTPDinhLuong_toCTHD(combo);
                let dateLot1 = GetNgaySX_NgayHH(combo);
                combo.NgaySanXuat = dateLot1.NgaySanXuat;
                combo.NgayHetHan = dateLot1.NgayHetHan;
                combo.LotParent = false;
                combo.DM_LoHang = [];

                combo.TongPhiDichVu = combo.PhiDichVu * combo.SoLuong;
                if (combo.LaPTPhiDichVu) {
                    combo.TongPhiDichVu = RoundDecimal(combo.PhiDichVu * combo.SoLuong * combo.DonGia / 100, 3);
                }

                combo.LoaiHoaDon = itemCT.LoaiHoaDon;
                combo.MaHoaDon = itemCT.MaHoaDon;
                combo.ID_ViTri = itemCT.ID_ViTri;
                combo.TenViTri = itemCT.TenPhongBan;

                combo.SoLuongMacDinh = itemCT.SoLuong === 0 ? combo.SoLuong : combo.SoLuong / itemCT.SoLuong;
                combo.SoLuongDaMua = 0;
                combo.CssWarning = false;
                combo.IsKhuyenMai = false;
                combo.IsOpeningKMai = false;
                combo.TenKhuyenMai = '';
                combo.HangHoa_KM = [];
                combo.UsingService = false;
                combo.ListDonViTinh = [];
                combo.ShowWarningQuyCach = false;
                combo.SoLuongQuyCach = 0;
                combo.HangCungLoais = [];
                combo.LaConCungLoai = false;
                combo.TimeStart = 0;
                combo.QuaThoiGian = 0;
                combo.TimeRemain = 0;
                combo.ThoiGian = moment(new Date()).format('YYYY-MM-DD HH:mm');
            }
        }
        else {
            itemCT.ThanhPhanComBo = [];
        }
        return itemCT;
    }

    // show infor HoaDon/PhieuThu/Chi
    self.Modal_HoaDons = ko.observableArray();
    self.TongSLHang = ko.observable(0);
    self.LoaiHoaDon_MoPhieu = ko.observable(0);
    self.MaHoaDon_MoPhieu = ko.observable('');

    self.ShowPopup_InforHD_PhieuThu = function (item, itHD) {
        self.LoaiHoaDon_MoPhieu(item.LoaiHoaDon);
        vmThanhToan.showModalUpdate(item.ID, itHD.ConNo);
    }

    self.ClickMoPhieu = function () {
        localStorage.setItem('FindHD', self.MaHoaDon_MoPhieu());
        var url = '';
        switch (self.LoaiHoaDon_MoPhieu()) {
            case 6:
                localStorage.setItem('FindHD', self.MaHoaDon_MoPhieu());
                url = "/#/Returns";
                break;
            case 11:
            case 12:
                localStorage.removeItem('FindHD');
                localStorage.setItem('FindMaPhieuChi', self.MaHoaDon_MoPhieu());
                url = '/#/CashFlow';
                break;
        }
        if (url !== '') {
            window.open(url);
        }
    }

    self.Goto_LoHang = function (item) {
        localStorage.setItem('FindLoHang', item.MaLoHang);
        var url = "/#/Shipment";
        window.open(url);
    }

    function GetHT_TichDiem() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CauHinh_TichDiemChiTiet?idDonVi=' + id_donvi, 'GET').done(function (obj) {
                if (obj.res === true) {
                    self.ThietLap_TichDiem(obj.data);
                }
            });
        }
    }

    self.ThongTinPhieuTiepNhan = ko.observable();
    self.HangMucSuaChuas = ko.observableArray();
    self.VatDungKemTheos = ko.observableArray();

    function GetInfor_PhieuTiepNhan(id) {
        if (id !== null) {
            $.getJSON('/api/DanhMuc/GaraAPI/' + 'PhieuTiepNhan_GetThongTinChiTiet?id=' + id).done(function (x) {
                if (x.res && x.dataSoure.length > 0) {
                    self.ThongTinPhieuTiepNhan(x.dataSoure[0]);
                }
                $.getJSON('/api/DanhMuc/GaraAPI/' + "PhieuTiepNhan_GetTinhTrangXe?id=" + id).done(function (o) {
                    if (o.res) {
                        let hm = o.dataSoure.hangmuc.map(function (item, index) {
                            return {
                                STT: index + 1,
                                TenHangMuc: item.TenHangMuc,
                                TinhTrang: item.TinhTrang,
                                PhuongAnSuaChua: item.PhuongAnSuaChua,
                                Anh: item.Anh,
                            }
                        });

                        let vd = o.dataSoure.vatdung.map(function (item, index) {
                            return {
                                STT: index + 1,
                                TieuDe: item.TieuDe,
                                SoLuong: item.SoLuong,
                                FileDinhKem: item.FileDinhKem,
                            }
                        });
                        self.HangMucSuaChuas(hm);
                        self.VatDungKemTheos(vd);
                    }
                });
            })
        }
        else {
            self.HangMucSuaChuas([]);
            self.VatDungKemTheos([]);
        }
    }

    self.showModalEditCKHoaDon = function (item) {
        let tongTT = formatNumberToFloat(item.TongThanhToan);
        let daTT = formatNumberToFloat(item.KhachDaTra) + formatNumberToFloat(item.BaoHiemDaTra)
            - formatNumberToFloat(item.TienDoiDiem) - formatNumberToFloat(item.ThuTuThe);
        let obj = {
            ID: item.ID,
            LoaiHoaDon: item.LoaiHoaDon,
            MaHoaDon: item.MaHoaDon,
            TongThanhToan: tongTT,
            TongTienThue: item.TongTienThue,
            ThucThu: daTT,
            DaThuTruoc: daTT,
            ConNo: tongTT - daTT,
            TongPhiNganHang: 0,
        }
        vmHoaHongHoaDon.GetChietKhauHoaDon_byID(obj);
    }

    self.showModalEditCKDichVu = function (item) {
        vmChiTietHoaDon.showModalChiTietHoaDon(item.ID);
    }

    $('#ThuTienHoaDonModal').on('hidden.bs.modal', function () {
        if (vmThanhToan.saveOK) {
            SearchHoaDon();
        }
    })

    self.showTPComBo = function (item) {
        item.LoaiHoaDon = loaiHoaDon;
        if (item.LoaiHangHoa === 3) {
            vmThanhPhanCombo.showModalUpdate(item);
        }
        else {
            vmHDSC_chitietXuat.showModal(item);
        }
    }

    self.NhapHang = function (item) {
        let cthd = self.BH_HoaDonChiTiets();
        // get tpdluong 
        var arr = [];
        for (let i = 0; i < cthd.length; i++) {
            let forOut = cthd[i];
            switch (parseInt(forOut.LoaiHangHoa)) {
                case 1:
                    arr.push(forOut);
                    break;
                case 2:
                    if (!commonStatisJs.CheckNull(forOut.ThanhPhan_DinhLuong)) {
                        for (let j = 0; j < forOut.ThanhPhan_DinhLuong.length; j++) {
                            let forIn = forOut.ThanhPhan_DinhLuong[j];
                            arr.push(forIn);
                        }
                    }
                    break;
                case 3:
                    let combo = $.grep(vmThanhPhanCombo.AllComBo_ofHD, function (x) {
                        return x.ID_ParentCombo === forOut.ID_ParentCombo;
                    });

                    if (combo.length > 0) {
                        for (let k = 0; k < combo.length; k++) {
                            let for1 = combo[k];
                            switch (parseInt(for1.LoaiHangHoa)) {
                                case 1:
                                    arr.push(for1);
                                    break;
                                case 2:
                                    if (!commonStatisJs.CheckNull(for1.ThanhPhan_DinhLuong)) {
                                        for (let j = 0; j < for1.ThanhPhan_DinhLuong.length; j++) {
                                            let forIn = for1.ThanhPhan_DinhLuong[j];
                                            arr.push(forIn);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    break;
            }
        }
        if (arr.length === 0) {
            commonStatisJs.ShowMessageDanger('Hóa đơn chỉ bao gồm dịch vụ. Không thể xuất kho');
            return;
        }

        let sum = arr.reduce(function (x, item) {
            return x + (item.SoLuong * item.GiaNhap);
        }, 0);

        let cthdLoHang = [], arrIDQuiDoi = [];
        for (let i = 0; i < arr.length; i++) {
            var ctNew = $.extend({}, arr[i]);
            delete ctNew["ID"];
            ctNew.TenDoiTuong = '';
            ctNew.TongTienHangChuaCK = 0;
            ctNew.TongGiamGiaHang = 0;
            ctNew.PTChietKhauHH = 0;
            ctNew.PTThueHD = 0;
            ctNew.TongGiamGia = 0;
            ctNew.TongTienThue = 0;
            ctNew.TongChietKhau = 0;
            ctNew.MaHoaDon = '';
            ctNew.DienGiai = item.DienGiai;
            ctNew.ID_DoiTuong = null;
            ctNew.NgayLapHoaDon = new Date();
            ctNew.ID_HoaDon = const_GuidEmpty;

            ctNew.TongTienHang = sum;
            ctNew.PhaiThanhToan = sum;
            ctNew.TongThanhToan = sum;
            ctNew.KhachDaTra = sum;
            ctNew.DaThanhToan = sum;
            ctNew.ID_NhanVien = item.ID_NhanVien;

            if (commonStatisJs.CheckNull(ctNew.ThuocTinh_GiaTri)) {
                ctNew.ThuocTinh_GiaTri = '';
            }
            if (commonStatisJs.CheckNull(ctNew.DonViTinh)) {
                ctNew.DonViTinh = [];
            }
            if (commonStatisJs.CheckNull(ctNew.TyLeChuyenDoi)) {
                ctNew.TyLeChuyenDoi = 1;
            }
            if (commonStatisJs.CheckNull(ctNew.MaLoHang)) {
                ctNew.MaLoHang = '';
            }

            let idLoHang = ctNew.ID_LoHang;
            let quanLiTheoLo = !commonStatisJs.CheckNull(ctNew.ID_LoHang);
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
            ctNew.QuanLyTheoLoHang = quanLiTheoLo;
            ctNew.SoThuTu = cthdLoHang.length + 1;
            ctNew.HangCungLoais = [];
            ctNew.ThanhPhanComBo = [];
            ctNew.ThanhPhan_DinhLuong = [];
            ctNew.LaConCungLoai = false;
            ctNew.ID_ChiTietGoiDV = null;
            ctNew.ID_ChiTietDinhLuong = null;
            ctNew.DVTinhGiam = '%';
            ctNew.PTChietKhau = 0;
            ctNew.TienChietKhau = 0;
            ctNew.PTThue = 0;
            ctNew.TienThue = 0;
            ctNew.DonGia = ctNew.GiaNhap;
            ctNew.ThanhTien = ctNew.SoLuong * ctNew.GiaNhap;
            ctNew.ThanhToan = ctNew.ThanhTien;

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
                            let exLo = false;
                            for (let k = 0; k < cthdLoHang[j].DM_LoHang.length; k++) {
                                let forLot = cthdLoHang[j].DM_LoHang[k];
                                if (forLot.ID_LoHang === ctNew.ID_LoHang) {
                                    exLo = true;
                                    cthdLoHang[j].DM_LoHang[k].SoLuong = forLot.SoLuong + ctNew.SoLuong;
                                    cthdLoHang[j].DM_LoHang[k].ThanhTien = cthdLoHang[j].DM_LoHang[k].SoLuong * forLot.GiaNhap;
                                }
                            }
                            if (!exLo) {
                                let objLot = $.extend({}, ctNew);
                                objLot.LotParent = false;
                                objLot.HangCungLoais = [];
                                objLot.DM_LoHang = [];
                                objLot.IDRandom = CreateIDRandom('RandomCT_');
                                cthdLoHang[j].DM_LoHang.push(objLot);
                            }
                        }
                        else {
                            cthdLoHang[j].SoLuong = cthdLoHang[j].SoLuong + ctNew.SoLuong;
                            cthdLoHang[j].ThanhTien = cthdLoHang[j].SoLuong * cthdLoHang[j].GiaNhap;
                        }
                        break;
                    }
                }
            }
        }

        localStorage.setItem('lc_CTSaoChep', JSON.stringify(cthdLoHang));
        localStorage.setItem('typeCacheNhapHang', 3);
        window.open('/#/PurchaseOrderItem2', '_blank');
    }

    self.CapNhatChiPhi = function (item) {
        VueChiPhi.ShowModal(1);
    }

    self.XuatKho = function (item) {
        localStorage.setItem('lcXK_EditOpen', JSON.stringify([item]));
        localStorage.setItem('XK_createfrom', 3);
        window.open('/#/XuatKhoChiTiet');
    }

    self.Invoice_UpdateImg = function (item) {
        vmUpAnhHoaDon.InvoiceChosing = item;
        vmUpAnhHoaDon.isSaveToTemp = false;
        vmUpAnhHoaDon.GetListImgInvoiceDB(item.ID, "123");
        vmUpAnhHoaDon.showModalInsert();
    }

    self.showModalApDungHoTro = function (item) {
        let toDate = moment(item.NgayLapHoaDon).add(2, 'seconds',).format('YYYY-MM-DD HH:mm');
        vmApDungNhomHoTro.GetTongGiaTriSuDung_ofKhachHang(item.ID_DoiTuong, toDate);

        vmApDungNhomHoTro.showModalUpdate(item.ID, self.BH_HoaDonChiTiets(),);
    }

};
var modelGiaoDich = new ViewModelHD();
ko.applyBindings(modelGiaoDich, document.getElementById('divPage'));
function hidewait(o) {
    $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /><div class="happy-wait">' +
        ' </div>' +
        '</div>')
}
var arrIDchose = [];
function selectedVT(obj) {
    if ($(obj).children().length === 0) {
        $(obj).append('<i class="fa fa-check" aria-hidden="true"></i><i class="fa fa-times"></i>');
    }
    if ($(obj).children().length > 0) {
        arrIDchose.push($(obj).attr('id'));
    }
}
$(function () {
    $('input[type=text]').click(function () {
        $(this).select();
    });
})
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
            for (let i = 0; i < arrIDCheck.length; i++) {
                if (arrIDCheck[i] === thisID) {
                    arrIDCheck.splice(i, 1);
                    break;
                }
            }
        })
    }
    if (arrIDCheck.length > 0) {
        $('#divThaoTac').css("display", "inline-block");
        $('.choose-commodity').css("display", "inline-block").trigger("RemoveClassForButtonNew");
        $('#count').text(arrIDCheck.length);
    }
    else {
        $('#divThaoTac').hide();
        $('.choose-commodity').hide().trigger("addClassForButtonNew");
    }
}
function ChoseHoaDon(obj) {
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
        $('.choose-commodity').css("display", "inline-block").trigger("RemoveClassForButtonNew");
        $('#count').text(arrIDCheck.length);
    }
    else {
        $('#divThaoTac').hide();
        $('.choose-commodity').hide().trigger("addClassForButtonNew");
    }
    // count input is checked
    var countCheck = 0;
    $('#tb tr td.check-group input').each(function (x) {
        var id = $(this).attr('id');
        if ($.inArray(id, arrIDCheck) > -1) {
            countCheck += 1;
        }
    });
    // set check for header
    var ckHeader = $('#tb thead tr th:eq(0) input');
    var lenList = $('#tb tbody tr.prev-tr-hide').length;
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

function ConvertMinutes_ToHourMinutes(sophut) {
    var div = RoundDecimal(sophut / 60);
    var hours = Math.floor(div);
    var minutes = formatNumber3Digit((div - hours) * 60);
    if (hours > 0) {
        return hours.toString().concat(' giờ ', minutes, ' phút');
    }
    return minutes.toString().concat(' phút');
}

$('#vmChiPhiHoaDon').on('hidden.bs.modal', function () {
    if (!commonStatisJs.CheckNull(modelGiaoDich.InVoiceChosing())) {
        switch (modelGiaoDich.InVoiceChosing().LoaiHoaDon) {
            case 1:
            case 25:
                VueChiPhi.CTHD_GetChiPhiDichVu([modelGiaoDich.InVoiceChosing().ID])
                break;
        }
    }
})