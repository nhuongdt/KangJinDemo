function getDateNow() {
    return moment(new Date()).format('DD/MM/YYYY HH:mm');
}

var FormModel_NewNCC = function () {
    var self = this;
    var dateNow = new Date();
    self.ID = ko.observable();
    self.MaDoiTuong = ko.observable();
    self.ID_NhomDoiTuong = ko.observable();
    self.ID_QuanHuyen = ko.observable();
    self.ID_TinhThanh = ko.observable();

    self.TenDoiTuong = ko.observable();
    self.Email = ko.observable();
    self.DiaChi = ko.observable();
    self.DienThoai = ko.observable();
    self.NgaySinh_NgayTLap = ko.observable();
    self.GioiTinhNam = ko.observable(true);
    self.MaSoThue = ko.observable();
    self.GhiChu = ko.observable();
    self.LoaiDoiTuong = 2;
    self.NoHienTai = ko.observable();

    self.SetData = function (item) {
        self.ID(item.ID);
        self.ID_NhomDoiTuong(item.ID_NhomDoiTuong);
        self.ID_QuanHuyen(item.ID_QuanHuyen);
        self.ID_TinhThanh(item.ID_TinhThanh);
        self.MaDoiTuong(item.MaDoiTuong);
        self.TenDoiTuong(item.TenDoiTuong);
        self.Email(item.Email);
        self.DiaChi(item.DiaChi);
        self.DienThoai(item.DienThoai);
        self.GioiTinhNam(item.GioiTinhNam);
        self.GhiChu(item.GhiChu);
        self.NgaySinh_NgayTLap(moment(item.NgaySinh_NgayTLap, "YYYY-MM-DD hh:mm:ss").format("DD/MM/YYYY"));
        self.MaSoThue(item.MaSoThue);
        self.NoHienTai(item.NoHienTai);
    };
};


var FormMode_LoaiChungTu = function () {
    var self = this;
    self.TenLoaiChungTu = ko.observable();
    self.ID = ko.observable();
    self.GhiChu = ko.observable();
    self.setdata = function (item) {
        self.ID(item.ID);
        self.TenLoaiChungTu(item.TenLoaiChungTu);
        self.GhiChu(item.GhiChu);
    }
};

var FormModel_NewQuyHoaDon = function () {
    var self = this;
    self.ID = ko.observable();
    self.MaHoaDon = ko.observable();
    self.NgayLapHoaDon = ko.observable(getDateNow());
    self.ID_NhanVien = ko.observable();
    self.NguoiNopTien = ko.observable();
    self.TenNhanVien = ko.observable();
    self.NgayTao = ko.observable();
    self.TongTienThu = ko.observable();
    self.Quy_HoaDon_ChiTiet = ko.observableArray(); // List chi tiet hoa don
    self.LoaiHoaDon = ko.observable();
    self.NoiDungThu = ko.observable();
    self.HachToanKinhDoanh = ko.observable(true);
    self.NguoiTao = ko.observable(VHeader.UserLogin);

    self.SetData = function (item) {
        self.ID(item.ID);
        self.MaHoaDon(item.MaHoaDon);
        self.NgayLapHoaDon(moment(item.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'));
        self.ID_NhanVien(item.ID_NhanVien);
        self.TenNhanVien(item.TenNhanVien);
        self.NgayTao(item.NgayTao);
        self.Quy_HoaDon_ChiTiet(item.Quy_HoaDon_ChiTiet);
        self.NguoiNopTien(item.NguoiNopTien);
        self.TongTienThu(item.TongTienThu);
        self.NoiDungThu(item.NoiDungThu);
        self.HachToanKinhDoanh(item.HachToanKinhDoanh);
        self.NguoiTao(item.NguoiTao);
    };
};

var FormModel_Quy_KhoanThuChi = function () {
    var self = this;
    self.ID = ko.observable();
    self.MaKhoanThuChi = ko.observable();
    self.NoiDungThuChi = ko.observable();
    self.GhiChu = ko.observable();
    self.TinhLuong = ko.observable(false);

    self.SetData = function (item) {
        self.ID(item.ID);
        self.MaKhoanThuChi(item.MaKhoanThuChi);
        self.NoiDungThuChi(item.NoiDungThuChi);
        self.GhiChu(item.GhiChu);
        self.TinhLuong(item.TinhLuong);
    };
};
//var loaiHoaDon = '';
var ViewModelQuyHD = function () {
    var self = this;
    var Quy_HoaDonUri = '/api/DanhMuc/Quy_HoaDonAPI/';
    var DMLoaiChungTuUri = '/api/DanhMuc/DM_LoaiChungTuAPI/';
    var _IDNguoiDung = $('.idnguoidung').text();
    var _IDchinhanh = $('#hd_IDdDonVi').val();
    var _IDNhanVien = $('.idnhanvien').text();
    var userLogin = VHeader.UserLogin;
    var Key_Form = 'KeyForm_CashFlow';
    self.LoaiHoaDonMenu = $('#txtLoaiHoaDon').val();

    self.TenChiNhanh = ko.observable($('#_txtTenDonVi').html());
    self.TodayBC = ko.observable('Tháng này');
    self.QuyHoaDons = ko.observableArray();
    self.Loc_PhieuThu = ko.observable(self.LoaiHoaDonMenu === '0' || self.LoaiHoaDonMenu === '2');
    self.Loc_PhieuChi = ko.observable(self.LoaiHoaDonMenu === '1' || self.LoaiHoaDonMenu === '2');
    self.Loc_HoatDongKinhDoanh = ko.observable('0');
    self.TT_DaThanhToan = ko.observable(true);
    self.TT_DaHuy = ko.observable(false);
    self.DoiTuongs = ko.observableArray();
    self.LoaiChungTus = ko.observableArray();
    self.Quy_HoaDon_ChiTiet = ko.observableArray();
    self.NhanViens = ko.observableArray();
    self._ThemMoiLoaiHoaDon = ko.observable(true);
    self.loai = ko.observable(1);
    self.loaiSoQuyShow = ko.observable();
    self.ChotSo_ChiNhanh = ko.observableArray();
    self.NgayLapHoaDonIDLQ = ko.observable();
    self.TaiKhoans = ko.observableArray();
    self.selectedTaiKhoan = ko.observable();
    self.filterLoaiThu = ko.observable();
    self.filterLoaiChi = ko.observable();

    self.Filter_DatCoc = ko.observable('0');// coc, all

    self.filter = ko.observable();
    self.filterGC = ko.observable();
    self.filterTenNN = ko.observable();
    self.deleteMaHoaDon = ko.observable();
    self.deleteID = ko.observable();
    self.selectedTenDoiTuong = ko.observable();
    self.selectedTenNhanVien = ko.observable();
    self.selectedID_NV = ko.observable();
    self.error = ko.observable();
    self.booleanAdd = ko.observable(true);
    self.ThemMoiLoaiThu = ko.observable(true);
    self.newDoiTuong = ko.observable(new FormModel_NewNCC());
    self.newQuyHoaDon = ko.observable(new FormModel_NewQuyHoaDon());
    self.newQuy_KhoanThuChi = ko.observable(new FormModel_Quy_KhoanThuChi());
    self.newLoaiChungTu = ko.observable(new FormMode_LoaiChungTu());

    self._ThemMoiLoaiChungTu = ko.observable(true);
    self.LoaiHoaDon = ko.observable(11);
    self.InforHDprintf = ko.observableArray();

    self.PageList_Display = ko.observableArray();
    self.PageCount = ko.observable();
    self.TotalRecord = ko.observable(0);
    self.filterNgayLapHD = ko.observable("0");
    self.filterNgayLapHD_Input = ko.observable(); // ngày cụ thể
    self.filterNgayLapHD_Quy = ko.observable('thangnay');

    $('.txtNgayTao').val('Tháng này');

    self.ColumnsExcel = ko.observableArray();
    self.ColumnsExcelNN = ko.observableArray();
    self.ColumnsExcelTQ = ko.observableArray();

    self.dataIframe = ko.observable();

    self.columsort = ko.observable(null);
    self.sort = ko.observable(null);
    self.pageSizes = [10, 20, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();

    self.ID_KhoanThuChi = ko.observable(null);
    self.selectedNhomNN = ko.observable();
    self.CTNhomNguoiNop = ko.observableArray();
    self.DM_NhomDoiTuong_ChiTiets = ko.observableArray();
    self.TenCuaNguoiNopTien = ko.observable();
    self.ID_DoiTuong_QCT = ko.observable();
    self.ID_NhanVien_QCT = ko.observable();
    self.CheckLoaiDoiTuong = ko.observable(0);
    self.selectedTenDoiTuongKH = ko.observable();
    self.ID_HoaDonLienQuan = ko.observable();
    self.ChiTietQuys = ko.observableArray();
    self.DoiTuongs = ko.observableArray();
    self.ChiTietDoiTuong = ko.observableArray();
    self.ChiTietDoiTuongNCC = ko.observableArray();
    self.ChiTietDoiTuongNV = ko.observableArray();
    self.selectedNV = ko.observable();
    self.selectedLoaiThuChi = ko.observable('');
    self.ChiNhanhs = ko.observableArray();
    self.ArrDonVi = ko.observableArray();
    self.MangNhomDV = ko.observableArray();
    self.MangIDDV = ko.observableArray([_IDchinhanh]);
    self.KhoanThus = ko.observableArray();
    self.KhoanChis = ko.observableArray();
    self.KhoanThuChis = ko.observableArray();
    self.KhoanThuChiSearch = ko.observable('');
    self.arrMauIn = ko.observableArray();
    self.arrMauInPhieuThu = ko.observableArray();
    self.arrMauInPhieuChi = ko.observableArray();
    self.LoaiHDMauIn = ko.observable();
    self.arrMauInTQ = ko.observableArray();
    self.IDMauIn = ko.observable();
    self.CongTy = ko.observableArray();
    self.NganHangs = ko.observableArray();
    self.filterNH = ko.observable();
    self.selectIDNH = ko.observable();
    self.selectIDNganHang = ko.observable();
    self.ThemTaiKhoan = ko.observable(false);
    self.GanIDTaiKhoan = ko.observable();
    self.selectedTaiKhoanAddPhieu = ko.observable();
    self.Quyen_NguoiDung = ko.observableArray();
    self.Allow_ChangeTimeSoQuy = ko.observable(false);
    self.Role_UpdateSoQuy = ko.observable(false);
    self.Role_XemDS = ko.observable(false);
    self.Role_SoQuy_XoaNeuKhacNgay = ko.observable(false);
    self.Role_SoQuy_Xoa = ko.observable(false);
    self.Role_KhoanThuChi_ThemMoi = ko.observable(false);
    self.Role_KhoanThuChi_CapNhat = ko.observable(false);
    self.Role_KhoanThuChi_Xoa = ko.observable(false);
    self.ListCheckBox = ko.observableArray();
    self.NumberColum_Div2 = ko.observable(0);

    self.TonDauKy = ko.observable(0);
    self.TonCuoiKy = ko.observable(0);
    self.ThuTrongKy = ko.observable(0);
    self.ChiTrongKy = ko.observable(0);
    self.TonTrongKy = ko.observable();

    var _shopCookies = $('#shopCookies').val();
    self.isGara = ko.observable(false);
    if (_shopCookies === 'C16EDDA0-F6D0-43E1-A469-844FAB143014') {
        self.isGara(true);
    }

    self.NhomNguoiNops = ko.observableArray([
        { name: "Chọn nhóm người", value: "0" },
        { name: "Khách hàng", value: "1" },
        { name: "Nhà cung cấp", value: "2" },
        { name: "Nhân viên", value: "3" }
    ]);

    function PageLoad() {
        CheckRole_byVHeader();
        loadCheckbox();
        loadQuyenIndex();
        GetAllNhanVien();
        getAllChiNhanh();
        GetDataChotSo();
        GetDM_NhomDoiTuong_ChiTiets();
        getAllDMLoaiChungTus();
        GetAllQuy_KhoanThuChi();
        getallloaiMauIn();
        GetInforCongTy();
        getAllNganHang();
        getAllTaiKhoanNganHang();
    }

    PageLoad();

    function loadCheckbox() {
        $.getJSON("api/DanhMuc/BaseApi/GetCheckedStatic?type=" + $('#pageID').val(), function (data) {
            switch (parseInt(self.loai())) {
                case 0:// nganhang
                    data = $.grep(data, function (x) {
                        return $.inArray(x.Key, ['hinhthuc']) === -1;
                    })
                    break;
                case 1:// mat
                    data = $.grep(data, function (x) {
                        return $.inArray(x.Key, ['hinhthuc', 'taikhoanchuyen', 'taikhoanpos']) === -1;
                    })
                    break;
                case 2://all
                    break;
            }
            self.ListCheckBox(data);
            self.NumberColum_Div2(Math.ceil(data.length / 2));

            let cacheColumnOld = localStorage.getItem(Key_Form)
            if (cacheColumnOld !== null) {
                cacheColumnOld = JSON.parse(cacheColumnOld);
                cacheColumnOld = $.grep(cacheColumnOld, function (x) {
                    return $.inArray(x.Key, ['chinhanh']) === -1;
                });
                localStorage.setItem(Key_Form, JSON.stringify(cacheColumnOld));
            }
        });
    }

    function LoadHtmlGrid() {
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

    function CheckQuyenExist(maquyen) {
        var role = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen === maquyen;
        });
        return role.length > 0;
    }

    var roleView = $.grep(VHeader.Quyen, function (x) {
        return x.indexOf('SoQuy_XemDS') > -1;
    })
    self.Role_XemDS(roleView.length > 0);

    function CheckRole(maquyen) {
        return VHeader.Quyen.indexOf(maquyen) > -1;
    }

    function CheckRole_byVHeader() {
        self.Role_KhoanThuChi_ThemMoi(CheckRole('KhoanThuChi_ThemMoi'));
        self.Role_KhoanThuChi_CapNhat(CheckRole('KhoanThuChi_CapNhat'));
        self.Role_KhoanThuChi_Xoa(CheckRole('KhoanThuChi_Xoa'));       
    }

    //load quyền
    function loadQuyenIndex() {
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetHT_NhomNguoiDung?idnguoidung=" + _IDNguoiDung + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
            self.Quyen_NguoiDung(data.HT_Quyen_NhomDTO);
            self.Allow_ChangeTimeSoQuy(CheckQuyenExist('SoQuy_ThayDoiThoiGian'));
            self.Role_UpdateSoQuy(CheckQuyenExist('SoQuy_CapNhat'));
            self.Role_SoQuy_Xoa(CheckQuyenExist('SoQuy_Xoa'));
        });
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + 'GetCauHinhHeThong/' + _IDchinhanh, 'GET').done(function (data) {
            localStorage.setItem('lc_CTThietLap', JSON.stringify(data));
        });
    }

    function getAllDMLoaiChungTus() {
        ajaxHelper(DMLoaiChungTuUri + "GetDM_LoaiChungTu", 'GET').done(function (data) {
            self.LoaiChungTus(data);
        });
    }

    function GetDataChotSo() {
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/GetDataChotSo?idChiNhanh=' + _IDchinhanh, 'GET').done(function (data) {
            if (data !== null && data.length > 0) {
                self.ChotSo_ChiNhanh(data);
            }
        })
    }

    function GetDM_NhomDoiTuong_ChiTiets() {
        ajaxHelper('/api/DanhMuc/DM_NhomDoiTuongAPI/' + 'GetDM_NhomDoiTuong_ChiTiets?idDonVi=' + _IDchinhanh, 'GET').done(function (x) {
            let data = x.data;
            if (data.length > 0) {
                self.DM_NhomDoiTuong_ChiTiets(data);
            }
        });
    }

    self.XoaLSSoQuy = function (mahanghoa) {
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Sổ quỹ",
            NoiDung: "Xóa sổ quỹ : " + mahanghoa,
            NoiDungChiTiet: "Xóa sổ quỹ : " + mahanghoa,
            LoaiNhatKy: 3 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        var myData = {};
        myData.objDiary = objDiary;
        $.ajax({
            url: '/api/DanhMuc/SaveDiary/' + "post_NhatKySuDung",
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
                ShowMessage_Danger("Ghi nhật ký sử dụng thất bại");
            },
            complete: function () {

            }
        })
    }


    function actiontab($thisLi, $thisContent, val) {
        $thisLi.each(function () {
            if (val === $(this).data('id')) {
                $(this).addClass('active');
                $(this).css('pointer-events', 'visible');
            }
            else {
                $(this).removeClass('active');
                if (self.TenCuaNguoiNopTien() !== undefined && self.TenCuaNguoiNopTien() !== null && self.TenCuaNguoiNopTien() !== "") {
                    $(this).css('pointer-events', 'none');
                }
                else {
                    $(this).css('pointer-events', 'visible');
                }
            }
        });
        $thisContent.each(function () {

            if (val === $(this).data('id')) {
                $(this).addClass('active in');
                if (self.TenCuaNguoiNopTien() !== undefined && self.TenCuaNguoiNopTien() !== null && self.TenCuaNguoiNopTien() !== "") {
                    $(this).find('.seach-supplier input').hide();
                }
                else {
                    $(this).find('.seach-supplier input').show();
                }
            }
            else {
                $(this).removeClass('active').removeClass('in');
                $(this).find('.seach-supplier input').show();
            }
        });
    }

    self.ChangeTab_Modal = function (val) {
        self.selectedNhomNN(val);
    }
    self.selectedNhomNN.subscribe(function (val) {
        switch (val) {
            case 1:
                self.CheckLoaiDoiTuong(1);
                break;
            case 2:
                self.CheckLoaiDoiTuong(2);
                break;
            case 3:
                self.CheckLoaiDoiTuong(0);
                break;
            case 4:
                self.CheckLoaiDoiTuong(3);
                break;
        }
    });

    self.DeleteDT = function () {
        self.selectedTenDoiTuong(undefined);
        self.ChiTietDoiTuongNCC(undefined);
        $('.xoaCTDT').hide();
    }

    self.DeleteDTKH = function () {
        self.selectedTenDoiTuongKH(undefined);
        self.ChiTietDoiTuong(undefined);
        $('.xoaCTDTKH').hide();
    }

    self.DeleteNV = function () {
        self.selectedTenNhanVien(undefined);
        self.ChiTietDoiTuongNV(undefined);
        $('.xoaCTNV').hide();
    }

    function GetAllNhanVien() {
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetNS_NhanVien_InforBasic?idDonVi=" + _IDchinhanh, 'GET').done(function (data) {
            self.NhanViens(data);
            vmNapTienDatCoc.listData.NhanViens = data;
            vmThemPhieuThuChi.listData.NhanViens = data;
            vmThanhToan.listData.NhanViens = data;
            vmThanhToanNCC.listData.NhanViens = data;
            vmHoaHongHoaDon.listData.NhanViens = self.NhanViens();
        });
    }

    self.copySoQuy = function (item) {
        vmThemPhieuThuChi.showModalUpdate(item, self.ChiTietQuys(), true);
    }

    self.editSQ = async function (item) {
        let qct = await $.getJSON(Quy_HoaDonUri + "GetQuyChiTiet_byIDQuy/" + item.ID).done(function () {
        }).then(function (x) {
            if (x.res) {
                return x.dataSoure;
            }
            return [];
        })

        if (item.PhieuDieuChinhCongNo === 2) {
            // nap tien coc
            if (item.LoaiHoaDon === 12) {
                vmNapTienDatCoc.loaiMenu = 1;
            }
            else {
                vmNapTienDatCoc.loaiMenu = 0;
            }
            vmNapTienDatCoc.showModalUpdate(item, qct);
        }
        else {
            if (qct.length > 0 && commonStatisJs.CheckNull(qct[0].ID_HoaDonLienQuan)) {
                if (qct[0].LaTienCoc === 1) {
                    // chi tra coc
                    vmNapTienDatCoc.showModalUpdate(item, qct);
                }
                else {
                    if (commonStatisJs.CheckNull(qct[0].ID_HoaDonLienQuan)) {
                        // thuchi khong lienquan hoadon
                        vmThemPhieuThuChi.showModalUpdate(item, qct);
                    }
                }
            }
            else {
                switch (item.LoaiDoiTuong) {
                    case 0:
                    case 1:
                    case 3:
                        vmThanhToan.showModalUpdate(item.ID);
                        break;
                    case 2:
                    case 4:// nguoi gt
                    case 5:// hoahong nv
                        vmThanhToanNCC.showModalUpdate(item.ID);
                        break;
                }
            }
        }
    };

    self.selectedNV.subscribe(function (val) {
        self.ChangePageSize();
    })

    self.selectedLoaiThuChi.subscribe(function (val) {
        self.ChangePageSize();
    });

    self.modalDelete = function (item) {
        if (item.PhieuDieuChinhCongNo === 2) {
            vmNapTienDatCoc.newPhieuThu.ID = item.ID;
            vmNapTienDatCoc.newPhieuThu.MaHoaDon = item.MaHoaDon;
            vmNapTienDatCoc.newPhieuThu.LoaiHoaDon = item.LoaiHoaDon;
            vmNapTienDatCoc.HuyPhieu();
        }
        else {
            self.deleteMaHoaDon(item.MaHoaDon);
            self.deleteID(item.ID);
            $('#modalpopup_deleteHH').modal('show');

            dialogConfirm('Xác nhận xóa', ' Bạn có chắc chắn muốn xóa sổ quỹ <b> ' + item.MaHoaDon + ' </b> không?', function () {
                ajaxHelper(Quy_HoaDonUri + "DeleteQuy_HoaDon/" + item.ID, 'DELETE').done(function (x) {
                    if (x === "") {
                        ShowMessage_Success("Xóa sổ quỹ thành công");

                        let diary = {
                            ID_NhanVien: _IDNhanVien,
                            ID_DonVi: _IDchinhanh,
                            ChucNang: 'Sổ quỹ',
                            NoiDung: 'Xóa sổ quỹ '.concat(item.MaHoaDon, ', nhân viên hủy: ', userLogin),
                            NoiDungChiTiet: 'Xóa sổ quỹ '.concat(item.MaHoaDon, ', nhân viên hủy: ', userLogin),
                            LoaiNhatKy: 3
                        }
                        Insert_NhatKyThaoTac_1Param(diary);
                        SearchHoaDon();
                    }
                    else {
                        ShowMessage_Danger("Giá trị thẻ nạp đã được sử dụng không thể xóa phiếu này");
                    }
                    $('#modalpopup_SoQuy').modal('hide');
                });
            });
        }
    };

    self.reset_SoQuy = function () {
        self.newQuyHoaDon(new FormModel_NewQuyHoaDon());
        self.newQuyHoaDon().NgayLapHoaDon(getDateNow());
        self.selectedTenDoiTuong(undefined);
        self.selectedTenDoiTuongKH(undefined);
        self.selectedTenNhanVien(undefined);
        $('.xoaCTDT').hide();
        $('.xoaCTNV').hide();
        $('.xoaCTDTKH').hide();
    };

    self.showPopupAddSoQuy = function () {
        vmThemPhieuThuChi.showModalAddNew(self.loai(), true);
    };

    self.showPopupAddSoQuyChi = function () {
        vmThemPhieuThuChi.showModalAddNew(self.loai(), false);
    };

    self.getAllQuyHoaDon = function (loai) {
        self.loai(loai);
        loadCheckbox();
        self.ChangePageSize();
    };

    function ResetCurrentPage() {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        self.currentPage(0);
    }

    self.ChangePageSize = function () {
        ResetCurrentPage();
        SearchHoaDon();
    };

    self.GoToPage = function (page) {
        self.currentPage(page);
    };

    self.GetClass = function (page) {
        return (page === self.currentPage()) ? "click" : "";
    };

    function getAllChiNhanh() {
        var quyen = "SoQuy_XemDS";
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetDonViXemDsGiaoDich?quyen=" + quyen + '&idnhanvien=' + _IDNhanVien + '&idnguoidung=' + _IDNguoiDung, 'GET').done(function (data) {
            data = data.sort((a, b) => a.TenDonVi.localeCompare(b.TenDonVi, undefined, { caseFirst: "upper" }));
            self.ChiNhanhs(data);
            self.ArrDonVi(self.ChiNhanhs());
            vmThanhToanNCC.listData.ChiNhanhs = data;

            var cn = $.grep(self.ChiNhanhs(), function (x) {
                return x.ID === _IDchinhanh;
            });
            if (cn.length > 0) {
                var obj = {
                    ID: _IDchinhanh,
                    TenDonVi: cn[0].TenDonVi,
                }
                self.MangNhomDV.push(obj);
                vmNapTienDatCoc.inforCongTy.TenChiNhanh = obj.TenDonVi;
                vmNapTienDatCoc.inforCongTy.DienThoaiChiNhanh = cn[0].DienThoai;

            }
            SearchHoaDon();
        });
    }

    self.selectedCN = function (item) {
        ResetCurrentPage();

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
            for (let i = 0; i < self.MangNhomDV().length; i++) {
                if ($.inArray(self.MangNhomDV()[i].ID, arrDV) === -1 && self.MangNhomDV()[i].ID !== '00000000-0000-0000-0000-0000-000000000000') {
                    self.ArrDonVi().unshift(self.MangNhomDV()[i]);
                }
            }
            self.MangNhomDV([{
                ID: '00000000-0000-0000-0000-0000-000000000000', TenDonVi: 'Tất cả chi nhánh'
            }]);
            sTenChiNhanhs = 'Tất cả chi nhánh';
        }
        else {
            for (var i = 0; i < self.MangNhomDV().length; i++) {
                sTenChiNhanhs += self.MangNhomDV()[i].TenDonVi + ',';
                if ($.inArray(self.MangNhomDV()[i].ID, arrDV) === -1) {
                    arrDV.push(self.MangNhomDV()[i].ID);
                }
                if (self.MangNhomDV()[i].ID === '00000000-0000-0000-0000-0000-000000000000') {
                    self.MangNhomDV().splice(i, 1);
                }
            }
            if ($.inArray(item.ID, arrDV) === -1) {
                self.MangNhomDV.push(item);
                sTenChiNhanhs += item.TenDonVi + ', '; // used to bind at head Report
            }
            sTenChiNhanhs = Remove_LastComma(sTenChiNhanhs);
            arrIDSearch = $.map(self.MangNhomDV(), function (x) {
                return x.ID;
            });
        }
        // remove donvi has chosed in lst
        var arr = $.grep(self.ArrDonVi(), function (x) {
            return x.ID !== item.ID;
        });
        self.ArrDonVi(arr);
        self.MangIDDV(arrIDSearch);
        self.TenChiNhanh(sTenChiNhanhs);
        event.preventDefault();
        return false;
    };

    self.CloseDV = function (item) {
        ResetCurrentPage();

        self.MangNhomDV.remove(item);
        var arrID = [];
        var sTenChiNhanhs = '';
        if (item.ID === '00000000-0000-0000-0000-0000-000000000000') {
            arrID = $.map(self.ChiNhanhs(), function (x) {
                return x.ID;
            });
            sTenChiNhanhs = 'Tất cả chi nhánh';
        }
        else {
            self.ArrDonVi.unshift(item);
            if (self.MangNhomDV().length === 0) {
                arrID = $.map(self.ChiNhanhs(), function (x) {
                    return x.ID;
                });
                sTenChiNhanhs = 'Tất cả chi nhánh';
            }
            else {
                for (var i = 0; i < self.MangNhomDV().length; i++) {
                    sTenChiNhanhs += self.MangNhomDV()[i].TenDonVi + ', ';
                }
                sTenChiNhanhs = Remove_LastComma(sTenChiNhanhs);
                arrID = $.map(self.MangNhomDV(), function (x) {
                    return x.ID;
                });
            }
        }
        self.TenChiNhanh(sTenChiNhanhs);
        self.MangIDDV(arrID);
        SearchHoaDon();
    };

    //sort kiểm kho
    $('#tableSQ thead tr').on('click', 'th', function () {
        var id = $(this).attr('id');
        if (id === "txtMaPhieu") {
            self.columsort("MaPhieu");
            SortGrid(id);
        }
        if (id === "txtThoiGian") {
            self.columsort("ThoiGian");
            SortGrid(id);
        }
        if (id === "txtLoaiThuChi") {
            self.columsort("LoaiThuChi");
            SortGrid(id);
        }
        if (id === "txtNguoiNopTien") {
            self.columsort("NguoiNopTien");
            SortGrid(id);
        }
        if (id === "txtChiNhanh") {
            self.columsort("ChiNhanh");
            SortGrid(id);
        }
        if (id === "txtGiaTri") {
            self.columsort("GiaTri");
            SortGrid(id);
        }
    });

    $('#tblSQNH thead tr').on('click', 'th', function () {
        var id = $(this).attr('id');
        if (id === "txtMaPhieuNH") {
            self.columsort("MaPhieu");
            SortGrid(id);
        }
        if (id === "txtThoiGianNH") {
            self.columsort("ThoiGian");
            SortGrid(id);
        }
        if (id === "txtLoaiThuChiNH") {
            self.columsort("LoaiThuChi");
            SortGrid(id);
        }
        if (id === "txtNguoiNopTienNH") {
            self.columsort("NguoiNopTien");
            SortGrid(id);
        }
        if (id === "txtChiNhanhNH") {
            self.columsort("ChiNhanh");
            SortGrid(id);
        }
        if (id === "txtGiaTriNH") {
            self.columsort("GiaTri");
            SortGrid(id);
        }
    });

    $('#tableSQTQ thead tr').on('click', 'th', function () {
        var id = $(this).attr('id');
        if (id === "txtMaPhieuTQ") {
            self.columsort("MaPhieu");
            SortGrid(id);
        }
        if (id === "txtThoiGianTQ") {
            self.columsort("ThoiGian");
            SortGrid(id);
        }
        if (id === "txtLoaiThuChiTQ") {
            self.columsort("LoaiThuChi");
            SortGrid(id);
        }
        if (id === "txtNguoiNopTienTQ") {
            self.columsort("NguoiNopTien");
            SortGrid(id);
        }
        if (id === "txtChiNhanhTQ") {
            self.columsort("ChiNhanh");
            SortGrid(id);
        }
        if (id === "txtGiaTriTQ") {
            self.columsort("GiaTri");
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
    }

    self.addLoaiThu = function () {
        $('#modalPopup_LoaiThu').modal('show');
        self.ThemMoiLoaiThu(true);
        $('#modalPopup_LoaiThu').on('shown.bs.modal', function () {
            $('#txtNoiDungThu').select();
        })
        $('#txtNoiDungThu').val("");
        $('#txtMoTaThu').val("");
    };

    self.addLoaiChi = function () {
        $('#modalPopup_LoaiChi').modal('show');
        self.ThemMoiLoaiThu(true);
        $('#modalPopup_LoaiChi').on('shown.bs.modal', function () {
            $('#txtNoiDungChi').select();
        })
        $('#txtNoiDungChi').val("");
        $('#txtMoTaChi').val("");
    };

    self.showpopuploaithuchi = function () {
        vmKhoanThuChi.showModalAdd();
    };


    self.editLoaiThuChi = function () {
        if (!self.Role_KhoanThuChi_CapNhat()) {
            ShowMessage_Danger('Không có quyền cập nhật khoản thu chi');
            return;
        }

        let khoanTC = $.grep(self.KhoanThuChis(), function (x) {
            return x.ID === self.selectedLoaiThuChi();
        })
        if (khoanTC.length > 0) {
            vmKhoanThuChi.showModalUpdate(khoanTC[0]);
        }
    };


    self.clickFistChonLT = function () {
        self.ID_KhoanThuChi(null);
        $('#lstLoaiThu span').each(function () {
            $(this).empty();
        });
        $('#txtLoaiThu').text('--Chọn loại thu--');
        $('.seach-company').hide();
    }

    self.clickFistChonLC = function () {
        self.ID_KhoanThuChi(null);
        $('#lstLoaiChi span').each(function () {
            $(this).empty();
        });
        $('#txtLoaiChi').text('--Chọn loại chi--');
        $('.seach-company').hide();
    }

    self.clickFistChonLTNH = function () {
        self.ID_KhoanThuChi(null);
        $('#lstLoaiThuNH span').each(function () {
            $(this).empty();
        });
        $('#txtLoaiThuNH').text('--Chọn loại thu--');
        $('.seach-company').hide();
    }

    self.clickFistChonLCNH = function () {
        self.ID_KhoanThuChi(null);
        $('#lstLoaiChiNH span').each(function () {
            $(this).empty();
        });
        $('#txtLoaiChiNH').text('--Chọn loại chi--');
        $('.seach-company').hide();
    }

    function GetAllQuy_KhoanThuChi() {
        ajaxHelper(Quy_HoaDonUri + 'GetQuy_KhoanThuChi/', 'GET').done(function (x) {
            if (x.res === true) {
                let data = x.data;
                for (var i = 0; i < data.length; i++) {
                    self.KhoanThuChis(data);
                    vmNapTienDatCoc.listData.AllKhoanThuChis = data;
                    vmThemPhieuThuChi.listData.AllKhoanThuChis = data;
                    vmThanhToan.listData.KhoanThuChis = data;
                    vmThanhToanNCC.listData.KhoanThuChis = data;

                    if (data[i].LaKhoanThu === true) {
                        self.KhoanThus.push(data[i]);
                    } else {
                        self.KhoanChis.push(data[i]);
                    }
                }
            }
        })
    }

    self.arrFilterKhoanThuChi = ko.computed(function () {
        var _filter = self.KhoanThuChiSearch();
        if (commonStatisJs.CheckNull(_filter)) {
            return self.KhoanThuChis().slice(0, 15);
        }
        return arrFilter = ko.utils.arrayFilter(self.KhoanThuChis(), function (prod) {
            let chon = true;

            let arr = locdau(prod.NoiDungThuChi).toLowerCase().split(/\s+/);
            let sSearch = '';

            for (let i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = (locdau(prod.NoiDungThuChi).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    sSearch.indexOf(locdau(_filter).toLowerCase()) >= 0
                );
            }
            return chon;
        }).slice(0, 15);
    });

    self.KhoanThuChiSelect = function (value, text) {
        self.selectedLoaiThuChi(value);
        $('#ddKhoanThuChi').val(text);
        $('#ddKhoanThuChiSearch').focus();
    }
    self.arrFilterLoaiThu = ko.computed(function () {
        var _filter = self.filterLoaiThu();

        return arrFilter = ko.utils.arrayFilter(self.KhoanThus(), function (prod) {
            var chon = true;
            var arr = locdau(prod.NoiDungThuChi).toLowerCase().split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = (locdau(prod.NoiDungThuChi).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    sSearch.indexOf(locdau(_filter).toLowerCase()) >= 0
                );
            }
            return chon;
        });
    });

    self.arrFilterLoaiChi = ko.computed(function () {
        var _filter = self.filterLoaiChi();
        return arrFilter = ko.utils.arrayFilter(self.KhoanChis(), function (prod) {
            var chon = true;
            var arr = locdau(prod.NoiDungThuChi).toLowerCase().split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = (locdau(prod.NoiDungThuChi).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    sSearch.indexOf(locdau(_filter).toLowerCase()) >= 0
                );
            }
            return chon;
        });
    });

    self.LinkKhachHang = function (item) {
        localStorage.setItem('FindKhachHang', item.MaDoiTuong);
        if (item.LoaiDoiTuong === 1) {
            var url = "/#/Customers";
            window.open(url);
        }
        if (item.LoaiDoiTuong === 2) {
            var url1 = "/#/Suppliers";
            window.open(url1);
        }
        if (item.LoaiDoiTuong === 3) {
            localStorage.setItem('FindNhanVien', item.MaDoiTuong);
            var url2 = "/#/User";
            window.open(url2);
        }
    };

    //===Paging====

    self.GetClassHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage()) ? "click" : "";
    };

    self.SearchText = function () {
        if (event.keyCode === 13) {
            ResetCurrentPage();
            SearchHoaDon(self.loai());
        }
    }

    $('.choseNgayTao li').on('click', function () {
        self.TodayBC($(this).text());
        self.filterNgayLapHD_Quy($(this).attr('value'));
        self.currentPage(0);
        SearchHoaDon();
    });

    $('#txtNgayTaoInput').on('apply.daterangepicker', function (ev, picker) {
        ResetCurrentPage();
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        SearchHoaDon();
    });

    $('#txtNgayTaoInputNH').on('apply.daterangepicker', function (ev, picker) {
        ResetCurrentPage();
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        SearchHoaDon();
    });

    $('#txtNgayTaoInputTQ').on('apply.daterangepicker', function (ev, picker) {
        ResetCurrentPage();
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        SearchHoaDon();
    });

    self.CallSearchHoaDon = function (e) {
        ResetCurrentPage();
        var thisDate = e.val();
        var thisDateFormat = moment(thisDate, 'DD/MM/YYYY HH:mm A').format('YYYY-MM-DD');
        self.filterNgayLapHD_Input(thisDateFormat);
        SearchHoaDon();
    };

    function GetParamSearch() {
        $('.line-right').height(0).css("margin-top", "0px");
        var maHDFind = localStorage.getItem('FindMaPhieuChi');
        if (maHDFind !== null) {
            self.filter(maHDFind);
            self.filterNgayLapHD('0');
            self.filterNgayLapHD_Quy('toanthoigian');
        }

        var txtMaHDon = self.filter();
        var txtGhiChu = self.filterGC();
        var txtTenNN = self.filterTenNN();
        if (commonStatisJs.CheckNull(txtMaHDon)) {
            txtMaHDon = "";
        }
        txtMaHDon = txtMaHDon.trim();
        if (txtGhiChu === undefined) {
            txtGhiChu = "";
        }
        if (txtTenNN === undefined) {
            txtTenNN = "";
        }
        // trang thai: Phiếu thu (1), Phiếu chi (2), Phiếu thu + Phiếu chi (3), ChuaHT + Chua Huy (0)
        // trang thai: Phiếu thu (1), Phiếu chi (2), Phiếu thu + Phiếu chi (3), ChuaHT + Chua Huy (0)
        var loaiChungTu = 1;
        var locThanhToan = 1;
        if (self.Loc_PhieuThu()) {
            if (self.Loc_PhieuChi()) {
                loaiChungTu = 3; // HT + DH
            }
            else {
                loaiChungTu = 1; // HT
            }
        }
        else {
            if (self.Loc_PhieuChi()) {
                loaiChungTu = 2; // Huy
            }
            else {
                loaiChungTu = 0; // Chua (HT + Huy)
            }
        }
        if (self.TT_DaThanhToan()) {
            if (self.TT_DaHuy()) {
                locThanhToan = 3;
            } else {
                locThanhToan = 1;
            }
        } else {
            if (self.TT_DaHuy()) {
                locThanhToan = 2;
            } else {
                locThanhToan = 4;
            }
        }

        var kinhdoanh = 0;
        switch (self.Loc_HoatDongKinhDoanh()) {
            case '0':
                kinhdoanh = 0;
                break;
            case '1':
                kinhdoanh = 1;
                break;
            case '2':
                kinhdoanh = 2;
                break;
        }
        var tiencoc = 0;
        switch (parseInt(self.Filter_DatCoc())) {
            case 0:
                tiencoc = '1';
                break;
            case 1:
                tiencoc = '11';
                break;
            case 2:
                tiencoc = '10';
                break;
            case 3:
                tiencoc = '13';
                break;
        }

        // NgayLapHoaDon
        var dayStart = '';
        var dayEnd = '';
        if (self.filterNgayLapHD() === '0') {
            let objdate = GetDate_FromTo(self.filterNgayLapHD_Quy());
            dayStart = objdate.FromDate;
            dayEnd = moment(objdate.ToDate).add('days', 1).format('YYYY-MM-DD');
        }
        else {
            // chon ngay cu the
            var arrDate = self.filterNgayLapHD_Input().split('-');
            dayStart = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
            dayEnd = moment(arrDate[1], 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD');
            self.TodayBC('Từ ' + moment(arrDate[0], 'DD/MM/YYYY').format('DD/MM/YYYY') + ' đến ' + moment(arrDate[1], 'DD/MM/YYYY').format('DD/MM/YYYY'));
        }
        var idnhanvien = self.selectedNV();
        if (idnhanvien === null || idnhanvien === undefined) {
            idnhanvien = '%%';
        }
        let idKhoanThuChi = self.selectedLoaiThuChi();
        if (idKhoanThuChi === null || idKhoanThuChi === undefined || idKhoanThuChi == "") {
            idKhoanThuChi = '%%';
        }
        let tkNganHang = self.selectedTaiKhoan();
        if (tkNganHang === null || tkNganHang === undefined) {
            tkNganHang = '%%';
        }

        return {
            LoaiSoQuy: self.loai(),
            LoaiChungTu: loaiChungTu,
            LoaiNapTien: tiencoc,
            IDDonVis: self.MangIDDV(),
            ID_NhanVien: idnhanvien,
            ID_NhanVienLogin: _IDNhanVien,
            ID_KhoanThuChi: idKhoanThuChi,
            ID_TaiKhoanNganHang: tkNganHang,
            DateFrom: dayStart,
            DateTo: dayEnd,
            TxtSearch: txtMaHDon,
            TrangThaiHachToan: kinhdoanh,
            TrangThaiSoQuy: locThanhToan,
            ColumnHides: '',
            ColumSort: '',
            SortBy: '',
        }
    }

    function SearchHoaDon(isNext = false) {
        if (self.Role_XemDS()) {

            $('.line-right').height(0).css("margin-top", "0px");
            $('#tableSQ').gridLoader();

            var objSerach = GetParamSearch();
            var ParamCashBook = {
                LoaiSoQuy: objSerach.LoaiSoQuy,
                LoaiChungTu: objSerach.LoaiChungTu,
                LoaiNapTien: objSerach.LoaiNapTien,
                IDDonVis: objSerach.IDDonVis,
                ID_NhanVien: objSerach.ID_NhanVien,
                ID_NhanVienLogin: objSerach.ID_NhanVienLogin,
                ID_KhoanThuChi: objSerach.ID_KhoanThuChi,
                ID_TaiKhoanNganHang: objSerach.ID_TaiKhoanNganHang,
                DateFrom: objSerach.DateFrom,
                DateTo: objSerach.DateTo,
                TxtSearch: objSerach.TxtSearch,
                TrangThaiHachToan: objSerach.TrangThaiHachToan,
                TrangThaiSoQuy: objSerach.TrangThaiSoQuy,
                ColumSort: objSerach.ColumSort,
                SortBy: objSerach.SortBy,
                CurrentPage: self.currentPage(),
                PageSize: self.pageSize(),
            }

            ajaxHelper(Quy_HoaDonUri + 'GetListCashFlow_Paging2', 'POST', ParamCashBook).done(function (x) {
                $('#tableSQ').gridLoader({ show: false });
                console.log('x', x)
                if (x.res === true) {
                    self.QuyHoaDons(x.data);
                    self.TotalRecord(x.TotalRow);
                    self.PageCount(x.TotalPage);
                    GetListPage();
                    LoadHtmlGrid();

                    switch (self.loai()) {
                        case 0:
                            self.ThuTrongKy(x.TongThuCK);
                            self.ChiTrongKy(x.TongChiCK);
                            break;
                        case 1:
                            self.ThuTrongKy(x.TongThuMat);
                            self.ChiTrongKy(x.TongChiMat);
                            break;
                        default:
                            self.ThuTrongKy(x.TongThuAll);
                            self.ChiTrongKy(x.TongChiAll);
                            break;
                    }
                    self.TonTrongKy(self.ThuTrongKy() - self.ChiTrongKy());

                    if (isNext === false) {
                        if (self.LoaiHoaDonMenu === '2') {
                            ajaxHelper(Quy_HoaDonUri + 'GetListCashFlow_Before', 'POST', ParamCashBook).done(function (x1) {
                                var mat = 0, ck = 0, all = 0;
                                if (x.res && x1.TonDauKyAll != null) {
                                    ck = x1.TonDauKyNH;
                                    mat = x1.TonDauKyMat;
                                    all = x1.TonDauKyAll;
                                }

                                switch (self.loai()) {
                                    case 0:
                                        self.TonDauKy(ck);
                                        break;
                                    case 1:
                                        self.TonDauKy(mat);
                                        break;
                                    default:
                                        self.TonDauKy(all);
                                        break;
                                }
                                self.TonCuoiKy(self.TonDauKy() + self.TonTrongKy());
                            });
                        }
                    }
                }
            });
            localStorage.removeItem('FindMaPhieuChi');
        }
    }

    self.ClickIconSearch = function () {
        ResetCurrentPage();
        SearchHoaDon();
    }

    self.Loc_PhieuThu.subscribe(function (newVal) {
        ResetCurrentPage();
        SearchHoaDon();
    });

    self.Filter_DatCoc.subscribe(function (newVal) {
        ResetCurrentPage();
        SearchHoaDon();
    });

    self.Loc_HoatDongKinhDoanh.subscribe(function (newVal) {
        ResetCurrentPage();
        SearchHoaDon();
    });

    self.Loc_PhieuChi.subscribe(function (newVal) {
        ResetCurrentPage();
        SearchHoaDon();
    });

    self.TT_DaThanhToan.subscribe(function (newVal) {
        ResetCurrentPage();
        SearchHoaDon();
    });

    self.TT_DaHuy.subscribe(function (newVal) {
        ResetCurrentPage();
        SearchHoaDon();
    });

    self.filterNgayLapHD.subscribe(function (newVal) {
        ResetCurrentPage()
        SearchHoaDon();
    });

    function GetListPage() {
        var allPage = self.PageCount();
        var currentPage = self.currentPage();
        var arrPage = [];

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
        self.PageList_Display(arrPage);

        self.fromitem((self.currentPage() * self.pageSize()) + 1);
        if (((self.currentPage() + 1) * self.pageSize()) > self.QuyHoaDons().length) {
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

    self.selecttabfilter = ko.observable(1);
    $('.tabs-main ').on('click', 'li', function () {
        self.selecttabfilter($(this).data('id'));
    })
    //Trinhpv xuất excel sổ quỹ

    self.addColum = function (item, isChecked) {
        isChecked = isChecked === undefined ? false : isChecked;
        if (isChecked === false) {// khong check: hide column
            self.ColumnsExcel.push(item);
        }
        else {
            for (let i = 0; i < self.ColumnsExcel().length; i++) {
                if (self.ColumnsExcel()[i] === item) {
                    self.ColumnsExcel.splice(i, 1);
                }
            }
        }
    }

    self.addColumNN = function (item, isChecked) {
        isChecked = isChecked === undefined ? false : isChecked;
        if (isChecked === false) {// khong check: hide column
            self.ColumnsExcelNN.push(item);
        }
        else {
            for (let i = 0; i < self.ColumnsExcelNN().length; i++) {
                if (self.ColumnsExcelNN()[i] === item) {
                    self.ColumnsExcelNN.splice(i, 1);
                }
            }
        }
    }

    self.addColumTQ = function (item, isChecked) {
        isChecked = isChecked === undefined ? false : isChecked;
        if (isChecked === false) {// khong check: hide column
            self.ColumnsExcelTQ.push(item);
        }
        else {
            for (let i = 0; i < self.ColumnsExcelTQ().length; i++) {
                if (self.ColumnsExcelTQ()[i] === item) {
                    self.ColumnsExcelTQ.splice(i, 1);
                }
            }
        }
        self.ColumnsExcelTQ.sort();
    }
    //===============================
    // Load lai form lưu cache bộ lọc 
    // trên grid khách hàng
    //===============================
    function loadFirst() {
        if (!localStorage.getItem('soquy_2')) {
            localStorage.setItem('soquy_2', JSON.stringify([{
                NameClass: ".n11",
                NameId: ".f11",
                Value: 11
            },
            {
                NameClass: ".n12",
                NameId: ".f12",
                Value: 12
            }]));
        }
        if (!localStorage.getItem('soquy_3')) {

            localStorage.setItem('soquy_3', JSON.stringify([{
                NameClass: ".l12",
                NameId: ".g12",
                Value: 3
            },
            {
                NameClass: ".l11",
                NameId: ".g11",
                Value: 3
            }]));
        }
    }

    function GetText_byLoai() {
        var txt = '';
        switch (self.loai()) {
            case 0:
                txt = 'ngân hàng';
                break;
            case 1:
                txt = 'tiền mặt';
                break;
            case 2:
                txt = 'tổng quỹ';
                break;

        }
        return txt;
    }

    self.DownloadFileTeamplateXLSX = function (pathFile) {
        var url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + pathFile;
        window.location.href = url;
    }

    self.ExportExcel = async function () {
        var txtLoai = GetText_byLoai();
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Sổ quỹ",
            NoiDung: "Xuất báo cáo sổ quỹ ".concat(txtLoai),
            NoiDungChiTiet: "Xuất báo cáo sổ quỹ ".concat(txtLoai),
            LoaiNhatKy: 6
        };

        var obj = GetParamSearch();
        obj.CurrentPage = 0;
        obj.PageSize = self.TotalRecord();
        obj.TextTime = self.TodayBC();
        obj.TextChiNhanhs = self.TenChiNhanh();

        // get columhide by loaiSQ: mat,nganhang,all
        let arr = [];
        let clHide = localStorage.getItem(Key_Form);
        if (clHide !== null) {
            clHide = JSON.parse(clHide);
            for (let i = 0; i < clHide.length; i++) {
                let forOut = clHide[i];
                for (let j = 0; j < self.ListCheckBox().length; j++) {
                    let forIn = self.ListCheckBox()[j];
                    if (forOut.NameClass === forIn.Key) {
                        arr.push(j);
                        break;
                    }
                }
            }
        }
        let sClHide = '';
        for (let i = 0; i < arr.length; i++) {
            sClHide += arr[i] + '_';
        }
        obj.ColumnHides = sClHide;
        obj.TonDauKy = self.TonDauKy();

        let fileNameExport;
        switch (obj.LoaiSoQuy) {
            case 0: // chuyenkhoan
                fileNameExport = 'SoQuyNganHang.xlsx';
                break;
            case 1: // mat
                fileNameExport = 'SoQuyTienMat.xlsx';
                break;
            default: // all
                fileNameExport = 'SoQuyTongQuy.xlsx';
                break;
        }

        $('#tableSQ').gridLoader({ show: false });
        const exportOK = await commonStatisJs.NPOI_ExportExcel(Quy_HoaDonUri + 'ExportExcel_SoQuy', 'POST', obj, fileNameExport);

        if (exportOK) {

            Insert_NhatKyThaoTac_1Param(objDiary);
        }
       
    }

    function getallloaiMauIn() {
        ajaxHelper(Quy_HoaDonUri + 'GetListMauInByLoaiHoaDonSQ?iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
            self.arrMauIn(data);
            for (var i = 0; i < self.arrMauIn().length; i++) {
                if (self.arrMauIn()[i].ID_LoaiChungTu === 11) {
                    self.arrMauInPhieuThu.push(self.arrMauIn()[i]);
                }
                else {
                    self.arrMauInPhieuChi.push(self.arrMauIn()[i]);
                }
            }
        })
    }

    self.wasChotSo = ko.observable(false);

    self.loadLoaiHD = function (item) {
        let dtNow = moment(new Date()).format('YYYY-MM-DD')
        let ngayLapPhieu = moment(item.NgayLapHoaDon).format('YYYY-MM-DD');
        let role = CheckRole('SoQuy_Xoa_NeuKhacNgay');
        if (dtNow === ngayLapPhieu) {
            role = self.Role_SoQuy_Xoa();
        }
        self.Role_SoQuy_XoaNeuKhacNgay(role);

        self.wasChotSo(VHeader.CheckKhoaSo(ngayLapPhieu, item.ID_DonVi));
        if (item.LoaiHoaDon === 11) {
            self.arrMauInTQ(self.arrMauInPhieuThu());
        }
        else {
            self.arrMauInTQ(self.arrMauInPhieuChi());
        }

        ajaxHelper(Quy_HoaDonUri + "GetQuyChiTiet_byIDQuy/" + item.ID, 'GET').done(function (x) {
            if (x.res) {
                self.ChiTietQuys(x.dataSoure);
            }
        })
    }

    self.getIDMauIn = function (item) {
        self.IDMauIn(item.ID);
    }

    function GetInforCongTy() {
        ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CongTy', 'GET').done(function (data) {
            if (data != null) {
                self.CongTy(data);
                vmNapTienDatCoc.inforCongTy = {
                    TenCongTy: data[0].TenCongTy,
                    DiaChiCuaHang: data[0].DiaChi,
                    LogoCuaHang: Open24FileManager.hostUrl + data[0].DiaChiNganHang
                };
            }
        });
    }

    self.InHoaDon = function (item) {
        var itemHDFormat = GetInforHDPrint(item);
        self.InforHDprintf(itemHDFormat);
        var loaiin = "";
        if (item.LoaiHoaDon === 11) {
            loaiin = TeamplateThu;
        }
        else {
            loaiin = TeamplateChi;
        }
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=' + loaiin + '&idDonVi=' + _IDchinhanh,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                var data = result;
                data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                data = data.concat("<script > var item1=[]; var item2=[]; var item4=[]; var item5 =[];var item3=" + JSON.stringify(self.InforHDprintf()) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(data);
            }
        });
    }

    self.PrintSoQuy = function (item, key) {
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
                data = data.concat("<script > var item1=[]; var item2=[], item4=[], item5=[];var item3=" + JSON.stringify(self.InforHDprintf()) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(data);
            }
        });
    }

    const isCorrectFormat = (dateString, format) => {
        return moment(dateString, format, true).isValid()
    }

    function GetInforHDPrint(objHD) {
        var hd = $.extend({}, objHD);

        let cn = VHeader.GetInforChiNhanh(objHD.ID_DonVi);
        hd.TenChiNhanh = cn.TenChiNhanh;
        hd.DiaChiChiNhanh = cn.DiaChiChiNhanh;
        hd.DienThoaiChiNhanh = cn.DienThoaiChiNhanh;
        hd.ChiNhanhBanHang = cn.TenChiNhanh;

        hd.NoiDungThu = hd.NoiDungThu;
        hd.MaPhieu = hd.MaHoaDon;
        var datehoadon = moment(objHD.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss');
        if (isCorrectFormat(datehoadon, 'DD/MM/YYYY HH:mm:ss')) {
            hd.NgayLapHoaDon = datehoadon;
        }
        hd.Ngay = moment(datehoadon, 'DD/MM/YYYY HH:mm:ss').format('DD');
        hd.Thang = moment(datehoadon, 'DD/MM/YYYY HH:mm:ss').format('MM');
        hd.Nam = moment(datehoadon, 'DD/MM/YYYY HH:mm:ss').format('YYYY');
        hd.NguoiNopTien = hd.NguoiNopTien;
        hd.NguoiNhanTien = hd.NguoiNopTien;
        hd.DiaChi = hd.DiaChi;
        hd.DienThoaiKhachHang = hd.SoDienThoai;
        hd.DiaChiKhachHang = hd.DiaChiKhachHang;
        hd.GiaTriPhieu = formatNumber3Digit(objHD.TongTienThu, 2);
        hd.TienBangChu = DocSo(objHD.TongTienThu);
        hd.TienMat = formatNumber3Digit(objHD.TienMat, 2);
        hd.TienATM = formatNumber3Digit(objHD.TienPOS, 2);
        hd.KhoanMucThuChi = hd.NoiDungThuChi;

        let pthucTT = '';
        if (objHD.TienMat > 0) {
            pthucTT = 'Tiền mặt, ';
        }
        if (objHD.TienPOS > 0) {
            pthucTT += 'POS, ';
        }
        if (objHD.ChuyenKhoan > 0) {
            pthucTT += 'Chuyển khoản, ';
        }
        if (objHD.TienDoiDiem > 0) {
            pthucTT = 'Đổi điểm, ';
        }
        if (objHD.TienTheGiaTri > 0) {
            pthucTT += 'Thu từ thẻ, ';
        }
        if (objHD.TTBangTienCoc > 0) {
            pthucTT += 'Tiền cọc, ';
        }
        hd.PhuongThucTT = Remove_LastComma(pthucTT);;

        if (self.CongTy().length > 0) {
            hd.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
            hd.TenCuaHang = self.CongTy()[0].TenCongTy;
            hd.DiaChiCuaHang = self.CongTy()[0].DiaChi;
            hd.DienThoaiCuaHang = self.CongTy()[0].SoDienThoai;
        }
        // get hoadon lienquan
        let sMaHD = '', arrMaHD = [];
        for (let i = 0; i < self.ChiTietQuys().length; i++) {
            let itFor = self.ChiTietQuys()[i];
            if ($.inArray(itFor.MaHoaDonHD, arrMaHD) === -1) {
                arrMaHD.push(itFor.MaHoaDonHD);
                sMaHD += itFor.MaHoaDonHD + ',';
            }
        }
        hd.HoaDonLienQuan = Remove_LastComma(sMaHD);
        return hd;
    }

    self.showpopupTaiKhoanNH = function () {
        vmTaiKhoanNganHang.showModalAdd();
    };

    self.editTK = function () {
        let tk = $.grep(self.TaiKhoans(), function (x) {
            return x.ID === self.selectedTaiKhoan();
        })
        if (tk.length > 0) {
            vmTaiKhoanNganHang.showModalUpdate(tk[0]);
        }
    }

    function getAllNganHang() {
        ajaxHelper(Quy_HoaDonUri + 'GetAllNganHang', 'GET').done(function (data) {
            self.NganHangs(data);
        })
    }

    function getAllTaiKhoanNganHang() {
        ajaxHelper(Quy_HoaDonUri + 'GetAllTaiKhoanNganHang_ByDonVi?idDonVi=' + _IDchinhanh, 'GET').done(function (x) {
            if (x.res) {
                self.TaiKhoans(x.data);
                vmNapTienDatCoc.listData.AccountBanks = x.data;
                vmThemPhieuThuChi.listData.AccountBanks = x.data;
                vmThanhToan.listData.AccountBanks = x.data;
                vmThanhToanNCC.listData.AccountBanks = x.data;
            }
        })
    }

    self.selectedTaiKhoan.subscribe(function (newValue) {
        ResetCurrentPage();
        SearchHoaDon(false);
    })

    // nap tien coc
    self.ShowModalNapTienCoc = function (isNapTien) {
        if (isNapTien) {
            vmNapTienDatCoc.loaiMenu = 1;//chi
        }
        else {
            vmNapTienDatCoc.loaiMenu = 0;// thu
        }
        vmNapTienDatCoc.showModalAddNew(isNapTien);
    }

    $('#vmKhoanThuChi').on('hidden.bs.modal', function (e) {
        if (!$('#vmThemPhieuThuChi').hasClass('in')) {
            if (vmKhoanThuChi.saveOK) {
                if (vmKhoanThuChi.typeUpdate !== 1) {
                    for (let i = 0; i < self.KhoanThuChis().length; i++) {
                        let itFor = self.KhoanThuChis()[i];
                        if (itFor.ID === vmKhoanThuChi.newKhoanThuChi.ID) {
                            self.KhoanThuChis.splice(i, 1);
                            break;
                        }
                    }

                    if (vmKhoanThuChi.newKhoanThuChi.LaKhoanThu) {
                        for (let i = 0; i < self.KhoanThus().length; i++) {
                            let itFor = self.KhoanThus()[i];
                            if (itFor.ID === vmKhoanThuChi.newKhoanThuChi.ID) {
                                self.KhoanThus.splice(i, 1);
                                break;
                            }
                        }
                    }
                    else {
                        for (let i = 0; i < self.KhoanChis().length; i++) {
                            let itFor = self.KhoanChis()[i];
                            if (itFor.ID === vmKhoanThuChi.newKhoanThuChi.ID) {
                                self.KhoanChis.splice(i, 1);
                                break;
                            }
                        }
                    }
                }

                if (vmKhoanThuChi.typeUpdate !== 3) {
                    self.KhoanThuChis.unshift(vmKhoanThuChi.newKhoanThuChi);

                    if (vmKhoanThuChi.newKhoanThuChi.LaKhoanThu) {
                        self.KhoanChis.unshift(vmKhoanThuChi.newKhoanThuChi);
                    }
                    else {
                        self.KhoanThus.unshift(vmKhoanThuChi.newKhoanThuChi);
                    }
                    $('#ddKhoanThuChi').val(vmKhoanThuChi.newKhoanThuChi.NoiDungThuChi);
                    self.selectedLoaiThuChi(vmKhoanThuChi.newKhoanThuChi.ID);
                }
            }
        }
    })

    $('#vmTaiKhoanNganHang').on('hidden.bs.modal', function (e) {
        if (vmTaiKhoanNganHang.saveOK) {
            if (vmTaiKhoanNganHang.typeUpdate !== 1) {
                for (let i = 0; i < self.TaiKhoans().length; i++) {
                    let itFor = self.TaiKhoans()[i];
                    if (itFor.ID === vmTaiKhoanNganHang.newAccountBank.ID) {
                        self.TaiKhoans.splice(i, 1);
                        break;
                    }
                }
            }
            if (vmTaiKhoanNganHang.typeUpdate !== 3) {
                self.TaiKhoans.unshift(vmTaiKhoanNganHang.newAccountBank);
                $('#ddlTaiKhoan').val(vmTaiKhoanNganHang.newAccountBank.TenChuThe);
                self.selectedTaiKhoan(vmTaiKhoanNganHang.newAccountBank.ID);
            }
        }
    })

    $('#vmThemPhieuThuChi').on('hidden.bs.modal', function (e) {
        if (vmThemPhieuThuChi.saveOK) {
            SearchHoaDon();
        }
    })
};
var modelSoQuy = new ViewModelQuyHD();
ko.applyBindings(modelSoQuy);

$('#selec-all-DV').parent().on('hide.bs.dropdown', function () {
    modelSoQuy.ChangePageSize();
});

//format number
function formatNumberObj(obj) {
    var objVal = $(obj).val();
    $(obj).val(objVal.toString().replace(/,/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    return objVal;
}

function formatNumberToInt(objVal) {
    return parseInt(objVal.replace(/,/g, ''));
}

function hidewait(o) {
    $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /><div class="happy-wait">' +
        ' </div>' +
        '</div>')
}
var arrIDchose = [];

$('#modalPopuplg_SoQuy, #modalPopuplg_SoQuyChi, #modalPopuplg_SoQuyNH, #modalPopuplg_SoQuyChiNH').on('shown.bs.modal', function () {
    $('input[type=text]').click(function () {
        $(this).select();
    });
    $('#txtMaPhieuChi, #txtMaPhieuThu, #txtMaPhieuThuNH, #txtMaPhieuChiNH').focus();
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
