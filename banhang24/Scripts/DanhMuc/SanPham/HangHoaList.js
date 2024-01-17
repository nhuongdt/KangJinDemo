
function formatCurrency(munber) {
    var munber_new = munber.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",")
    return munber_new;
}

var FromModel_ThuocTinhSP = function () {
    var self = this;
    self.ID = ko.observable();
    self.TenThuocTinh = ko.observable();
    self.setdata = function (item) {
        self.ID(item.ID);
        self.TenThuocTinh(item.TenThuocTinh);
    };
};

var FormModel_NhomHHDV = function () {
    var self = this;
    self.TenNhomHangHoa = ko.observable();
    self.ID_Parent = ko.observable();
    self.ID = ko.observable();
    self.LaNhomHangHoa = ko.observable(true);
    self.setdata = function (item) {
        self.ID(item.ID);
        self.TenNhomHangHoa(item.TenNhomHangHoa);
        self.ID_Parent(item.ID_Parent);
        self.LaNhomHangHoa(item.LaNhomHangHoa);
    };
};

var FormModel_HangHoaDichVu = function () {
    var self = this;
    self.MaHangHoa = ko.observable();

    self.ID = ko.observable();
    self.ID_HangHoa = ko.observable();
    self.ID_NhomHangHoa = ko.observable();
    self.TenNhomHangHoa = ko.observable(true);
    self.TenHangHoa = ko.observable();
    self.LaHangHoa = ko.observable();
    self.LoaiHangHoa = ko.observable(1);
    self.LoaiBaoDuong = ko.observable();
    self.SoKmBaoHanh = ko.observable();
    self.QuanLyBaoDuong = ko.observable();
    self.HoaHongTruocChietKhau = ko.observable();
    self.GiaBan = ko.observable(0);
    self.GiaVon = ko.observable(0);
    self.TonKho = ko.observable(0);
    self.GhiChu = ko.observable();
    self.QuyCach = ko.observable();
    self.TonToiDa = ko.observable();
    self.TonToiThieu = ko.observable();
    self.DuocBanTrucTiep = ko.observable(true);
    self.QuanLyTheoLoHang = ko.observable(false);
    self.DonViTinh = ko.observableArray();
    self.HangHoaCungLoaiArr = ko.observableArray();
    self.DonViTinhChuan = ko.observable();
    self.DinhLuongDichVu = ko.observableArray();
    self.ThoiGianBaoHanh = ko.observable();
    self.DonViTinhQuyCach = ko.observable();
    self.ChiPhiThucHien = ko.observable(0);
    self.ChiPhiTinhTheoPT = ko.observable(true);
    self.SoPhutThucHien = ko.observable(0);
    self.DichVuTheoGio = ko.observable(0);// 1.dichvu theogio
    self.DuocTichDiem = ko.observable(0);
    self.ID_Xe = ko.observable();
    self.BienSo = ko.observable();
    self.ChietKhauMD_NV = ko.observable(0);
    self.ChietKhauMD_NVTheoPT = ko.observable(true);

    self.SetData = function (item) {
        self.ID(item.ID);
        self.ID_HangHoa(item.ID_HangHoa);
        self.ID_NhomHangHoa(item.ID_NhomHangHoa);
        self.TenNhomHangHoa(item.TenNhomHangHoa);
        self.QuyCach(item.QuyCach);
        self.TonToiDa(item.TonToiDa);
        self.TonToiThieu(item.TonToiThieu);
        self.DuocBanTrucTiep(item.DuocBanTrucTiep);
        self.QuanLyTheoLoHang(item.QuanLyTheoLoHang);
        self.MaHangHoa(item.MaHangHoa);
        self.TenHangHoa(item.TenHangHoa);
        self.LaHangHoa(item.LaHangHoa);
        self.LoaiHangHoa(item.LoaiHangHoa);
        self.LoaiBaoDuong(item.LoaiBaoDuong);
        self.SoKmBaoHanh(item.SoKmBaoHanh);
        self.QuanLyBaoDuong(item.QuanLyBaoDuong);
        self.HoaHongTruocChietKhau(item.HoaHongTruocChietKhau);
        self.GiaBan(item.GiaBan);
        self.GiaVon(item.GiaVon);
        self.TonKho(item.TonKho);
        self.GhiChu(item.GhiChu);
        self.DonViTinh(item.DonViTinh);
        self.HangHoaCungLoaiArr(item.HangHoaCungLoaiArr);
        self.DinhLuongDichVu(item.DinhLuongDichVu);
        self.DonViTinhChuan(item.DonViTinhChuan);
        self.ThoiGianBaoHanh(item.ThoiGianBaoHanh);
        self.DonViTinhQuyCach(item.DonViTinhQuyCach);
        self.ChiPhiThucHien(item.ChiPhiThucHien);
        self.ChiPhiTinhTheoPT(item.ChiPhiTinhTheoPT);
        self.SoPhutThucHien(item.SoPhutThucHien);
        self.DichVuTheoGio(item.DichVuTheoGio);
        self.DuocTichDiem(item.DuocTichDiem);
        self.ID_Xe(item.ID_Xe);
        self.BienSo(item.BienSo);
        self.ChietKhauMD_NV(item.ChietKhauMD_NV);
        self.ChietKhauMD_NVTheoPT(item.ChietKhauMD_NVTheoPT);
    };
};

var FormModel_KiemKho = function () {
    var self = this;
    self.ID = ko.observable();
    self.ID_HoaDon = ko.observable();
    self.MaHoaDon = ko.observable();
    self.NgayLapHoaDon = ko.observable();
    self.ID_NhanVien = ko.observable(VHeader.IdNhanVien);
    self.NguoiTao = ko.observable();
    self.NgayTao = ko.observable();
    self.DienGiai = ko.observable(); // ghi chu
    self.BH_KiemKho_ChiTiet = ko.observableArray(); // List chi tiet hoa don
    self.LoaiHoaDon = ko.observable();
    self.ChoThanhToan = ko.observable(true);

    self.SetData = function (item) {
        self.ID(item.ID);
        self.ID_HoaDon(item.ID_HoaDon);
        self.MaHoaDon(item.MaHoaDon);
        self.NgayLapHoaDon(item.NgayLapHoaDon);
        self.ID_NhanVien(item.ID_NhanVien);
        self.NguoiTao(item.NguoiTao);
        self.NgayTao(item.NgayTao);
        self.DienGiai(item.DienGiai);
        self.BH_KiemKho_ChiTiet(item.BH_KiemKho_ChiTiet);
        self.LoaiHoaDon(item.LoaiHoaDon);
        self.ChoThanhToan(item.ChoThanhToan);
    };
};

var FormModel_HoaDonChiTiet = function () {
    var self = this;
    self.ID_HoaDon = ko.observable();
    self.MaHoaDon = ko.observable();
    self.NgayLapHoaDon = ko.observable();
    self.MaDoiTuong = ko.observable();
    self.TenDoiTuong = ko.observable();
    self.TenBangGia = ko.observable();
    self.ChoThanhToan = ko.observable();
    self.TenDonVi = ko.observable(0);
    self.BH_HoaDon_ChiTiet = ko.observableArray();
    self.TongTienHang = ko.observable();
    self.TongGiamGia = ko.observable();
    self.PhaiThanhToan = ko.observable();
    self.KhachDaTra = ko.observable();
    self.LoaiHoaDon = ko.observable();
    self.DienGiai = ko.observable();
    self.TenNhanVien = ko.observable();
    self.NguoiTaoHD = ko.observable();
    self.TongTienHDTra = ko.observable();
    self.MaPhieuChi = ko.observable();
    self.TenDonViChuyen = ko.observable();
    self.TenDonViNhan = ko.observable();
    self.NgaySua = ko.observable();
    self.YeuCau = ko.observable();
    self.NguoiTao = ko.observable();

    self.SetData = function (item) {
        self.ID_HoaDon(item.ID_HoaDon);
        self.MaHoaDon(item.MaHoaDon);
        self.NgayLapHoaDon(item.NgayLapHoaDon);
        self.MaDoiTuong(item.MaDoiTuong);
        self.TenDoiTuong(item.TenDoiTuong);
        self.TenBangGia(item.TenBangGia);
        self.ChoThanhToan(item.ChoThanhToan);
        self.TenDonVi(item.TenDonVi);
        self.BH_HoaDon_ChiTiet(item.BH_HoaDon_ChiTiet);
        self.TongTienHang(item.TongTienHang);
        self.TongGiamGia(item.TongGiamGia);
        self.PhaiThanhToan(item.PhaiThanhToan);
        self.KhachDaTra(item.KhachDaTra);
        self.LoaiHoaDon(item.LoaiHoaDon);
        self.DienGiai(item.DienGiai);
        self.TenNhanVien(item.TenNhanVien);
        self.NguoiTaoHD(item.NguoiTaoHD);
        self.TongTienHDTra(item.TongTienHDTra);
        self.MaPhieuChi(item.MaPhieuChi);
        self.TenDonViChuyen(item.TenDonViChuyen);
        self.TenDonViNhan(item.TenDonViNhan);
        self.NgaySua(item.NgaySua);
        self.YeuCau(item.YeuCau);
        self.NguoiTao(item.NguoiTao);
    };
};
var loaiHoaDon = '';
var ViewModel = function () {
    var self = this;
    var NhomHHUri = '/api/DanhMuc/DM_NhomHangHoaAPI/';
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';
    var DonViQuiDoiUri = '/api/DanhMuc/DonViTinhsAPI/';
    var NSNhanVienUri = "/api/DanhMuc/NS_NhanVienAPI/";
    var ReportUri = '/api/DanhMuc/ReportAPI/';

    var _IDNguoiDung = $('.idnguoidung').text();
    var _IDNhanVien = $('.idnhanvien').text();
    var _IDchinhanh = $('#hd_IDdDonVi').val();
    var _txtTenTaiKhoan = $('#txtTenTaiKhoan').text();
    self.TenChiNhanh = ko.observableArray();
    self.TodayBC = ko.observable('Toàn thời gian');
    self.NhomHangHoas = ko.observableArray();
    self.NhomHangHoasFilter = ko.observableArray();
    self.deleteMaHoaDon = ko.observable();
    self.selectedThuocTinh = ko.observable();
    self.selectedIDThuocTinh - ko.observable();
    self.NhanViens = ko.observableArray();
    self.selectedNV = ko.observable();
    self.HangHoas = ko.observableArray();
    self.KiemKhos = ko.observableArray();
    self.KiemGanDays = ko.observableArray();
    loaiHoaDon = $('#loaiHoaDon').val();
    self.TheKhos = ko.observableArray();
    self.RowErrKho = ko.observable();
    self.ListChooseHH = ko.observableArray();
    self.ListChooseHHInTemThuocTinh = ko.observableArray();
    self.error = ko.observable();
    self.LaHangHoa = ko.observable(true);
    self.selectedHH = ko.observable();
    self.selectIDNHHAdd = ko.observable();
    self.selectIDNHHChuyenNhom = ko.observable();
    self.selectIDNHHKiemKho = ko.observable();
    self.selectIDNHHAddDV = ko.observable();
    self.selectedNHomHH = ko.observable();
    self.selectedNHomHHByLaHH = ko.observable();
    self.deleteMaHangHoa = ko.observable();
    self.deleteID = ko.observable();
    self.deleteTenNhomHang = ko.observable();
    self.selectedNHomHHSua = ko.observable();
    self.header_filterNhomHang = ko.observable();
    self.number_GioiHanSoMatHang = ko.observable();
    self.CheckLocNhomHangCap3 = ko.observable(false);
    self.ID_NganhKinhDoanh = ko.observable($('#nganhkinhdoanh').val());
    self.IsGara = ko.observable(VHeader.IdNganhNgheKinhDoanh.toUpperCase() === 'C16EDDA0-F6D0-43E1-A469-844FAB143014');
    self.getCTHH = ko.observable();
    self.ThuocTinhCuaHH = ko.observableArray();
    self.shouldShowTitleTT = ko.computed(function () {
        if (self.ThuocTinhCuaHH() !== null && self.ThuocTinhCuaHH() !== undefined && self.ThuocTinhCuaHH().length > 0)
            return true;
        else
            return false;
    }, this);
    self.printThuocTinhCuaHH = ko.observableArray();
    self.Loc_TrangThaiPT = ko.observable(true);
    self.Loc_TrangThaiHT = ko.observable(true);
    self.Loc_TrangThaiDH = ko.observable(false);
    self.ContinueImport = ko.observable(false);
    self.LoaiSP_HH = ko.observable(true);
    self.LoaiSP_DV = ko.observable(true);
    self.LoaiSP_CB = ko.observable(true);
    self.Loc_TonKho = ko.observable('0');
    self.Loc_TinhTrangKD = ko.observable('1');
    self.Loc_TrangThaiXoa = ko.observable('0');
    self.filter = ko.observable();
    self.filterKK = ko.observable();
    self.filterHH = ko.observable();
    self.LaHangHoaNha = ko.observable(false);
    self.booleanAdd = ko.observable(false);
    self.newHangHoa = ko.observable(new FormModel_HangHoaDichVu());
    self.newHoaDon = ko.observable(new FormModel_HoaDonChiTiet());
    self.newKiemKho = ko.observable(new FormModel_KiemKho());
    self.newThuocTinh = ko.observable(new FromModel_ThuocTinhSP());
    self.productOld = ko.observable();
    //
    self.newNhomHangHoa = ko.observable(new FormModel_NhomHHDV());
    self._ThemMoiNhomHH = ko.observable(true);
    self.filterNgayLapHD = ko.observable("0");
    self.filterNgayLapHD_Input = ko.observable(); // ngày cụ thể
    self.filterNgayLapHD_Quy = ko.observable(); // Theo quý

    // paging list ChietKhau NV
    self.pageSizeListCK = ko.observable(10);
    self.currentPageListCK = ko.observable(0);
    self.fromitemListCK = ko.observable(1);
    self.toitemListCK = ko.observable();
    self.TotalRecordListCK = ko.observable(0);
    self.PageCountListCK = ko.observable();

    self.columsort = ko.observable(null);
    self.sort = ko.observable(null);

    self.sumError = ko.observable();
    var itemcheckhh = '';

    self.ArrViTriHH = ko.observableArray();

    self.LoaiThoiGianBaoHanhs = ko.observableArray([
        { TenLoai: "Ngày", value: "1" },
        { TenLoai: "Tháng", value: "2" },
        { TenLoai: "Năm", value: "3" },
        { TenLoai: "Giờ", value: "4" },
    ]);

    self.LoaiInMaVach = ko.observableArray([
        { TenLoai: "In 3 tem", value: "3" },
        { TenLoai: "In 2 tem", value: "2" },
        { TenLoai: "In 12 tem", value: "12" },
        { TenLoai: "In 65 tem", value: "65" }
    ]);

    self.ListTypeMauIn = ko.observableArray();
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
    self.selectedMauInMaVach = ko.observable();
    self.selectedLoaIn = ko.observable();
    self.IDViTriEdit = ko.observable();

    self.Quyen_NguoiDung = ko.observableArray();
    self.ThietLap = ko.observableArray();
    self.HangHoa_XemGiaVon = ko.observable(false);
    self.HangHoa_GiaBan = ko.observable(false);
    self.HangHoa_GiaVon = ko.observable(false);
    self.role_InsertProduct = ko.observable(false);
    self.role_DeleteProduct = ko.observable(false);
    self.role_UpdateProduct = ko.observable(false);// = role chuyen nhomhang, role ngungkinhdoanh
    self.role_UpdateMoTaProduct = ko.observable(false);// = sửa đổi thông tin: qunr lý DVT, thuộc tính, mô tả chi tiết
    self.LaAdmin = ko.observable(VHeader.LaAdmin === 'Admin');
    self.role_CaiDatDinhLuong = ko.observable(false);
    self.KiemKho_Insert = ko.observable(false);
    self.KiemKho_Update = ko.observable(false);
    self.KiemKho_Delete = ko.observable(false);
    self.KiemKho_Export = ko.observable(false);
    self.KiemKho_Copy = ko.observable(false);

    self.Role_AddCar = ko.observable(false);
    self.Role_UpdateCar = ko.observable(false);
    self.Role_HoatDongXe = ko.observable(false);

    self.selectedMauInMaVach.subscribe(function () {
    });
    self.selectedLoaIn.subscribe(function () {
    })

    function loadQuyenIndex() {
        var arrQuyen = [];
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetHT_NhomNguoiDung?idnguoidung=" + _IDNguoiDung + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
            self.Quyen_NguoiDung(data.HT_Quyen_NhomDTO);
            if (CheckQuyenExist('HangHoa_GiaBan')) {
                self.HangHoa_GiaBan('HangHoa_GiaBan');
            }
            else {
                self.HangHoa_GiaBan('');
            }

            self.role_InsertProduct(CheckQuyenExist('HangHoa_ThemMoi'));
            self.role_DeleteProduct(CheckQuyenExist('HangHoa_Xoa'));
            self.role_UpdateProduct(CheckQuyenExist('HangHoa_CapNhat'));
            self.role_UpdateMoTaProduct(CheckQuyenExist('HangHoa_SuaDoiThongTinMoTaHangHoa'));
            self.role_CaiDatDinhLuong(CheckQuyenExist('HangHoa_CaiTPDinhLuong'));

            self.KiemKho_Insert(CheckQuyenExist('KiemKho_ThemMoi'));
            self.KiemKho_Copy(CheckQuyenExist('KiemKho_SaoChep'));
            self.KiemKho_Delete(CheckQuyenExist('KiemKho_Xoa'));
            self.KiemKho_Export(CheckQuyenExist('KiemKho_XuatFile'));

            self.Role_AddCar(CheckQuyenExist('DanhMucXe_ThemMoi'));
            self.Role_UpdateCar(CheckQuyenExist('DanhMucXe_CapNhat'));

            let roleHDX = $.grep(self.Quyen_NguoiDung(), function (x) {
                return x.MaQuyen.indexOf('HoatDongXe') > -1;
            });
            self.Role_HoatDongXe(roleHDX.length > 0);

            if (data.HT_Quyen_NhomDTO.length > 0) {
                for (var i = 0; i < data.HT_Quyen_NhomDTO.length; i++) {
                    arrQuyen.push(data.HT_Quyen_NhomDTO[i].MaQuyen);
                }
            }
            localStorage.setItem('lc_CTQuyen', JSON.stringify(arrQuyen));
        });
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + 'GetCauHinhHeThong/' + _IDchinhanh, 'GET').done(function (data) {
            localStorage.setItem('lc_CTThietLap', JSON.stringify(data));
            self.ThietLap(data);
            if (parseInt(loaiHoaDon) !== 9) {
                vmImportDinhLuong.QuanLyTheoLoHang = data.LoHang;
            }
        });
    }


    function CheckQuyenExist(maquyen) {
        var role = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen === maquyen;
        });
        return role.length > 0;
    }

    function getQuyen_NguoiDung() {
        ajaxHelper(ReportUri + "getQuyenXemGiaVon?ID_NguoiDung=" + _IDNguoiDung + "&MaQuyen=" + "HangHoa_XemGiaVon", "GET").done(function (data) {
            self.HangHoa_XemGiaVon(data);
        });
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDNguoiDung + "&ID_DonVi=" + _IDchinhanh + "&MaQuyen=" + "HangHoa_GiaVon", "GET").done(function (data) {
            //self.HangHoa_GiaVon('HangHoa_GiaVon');
            self.HangHoa_GiaVon(data);
        });
    };

    function getallViTri() {
        ajaxHelper(DMHangHoaUri + "getAllViTri", "GET").done(function (data) {
            self.ArrViTriHH(data);
        });
    }

    self.selectedLoaiThoiGianBH = ko.observable();

    self.resetThuocTinh = function () {
        self.newThuocTinh(new FromModel_ThuocTinhSP());
    };

    self.Indexx = ko.observable();
    self.showAddThuocTinh = function (item) {
        self.Indexx(item.index);
        $('#modalThemThuocTinh').modal('show');
        $('#modalThemThuocTinh').on('shown.bs.modal', function () {
            $('#txtTenThuocT').select();
        });
        self.resetThuocTinh();
        $('.thuoctinhten').html('Thêm thuộc tính');
        self.selectedThuocTinh(undefined);
    };
    self.arrThuocTinh = ko.observableArray();
    self.AddThuocTinh = function (formElement) {
        var _id = self.newThuocTinh().ID();
        var _tenThuocTinh = self.newThuocTinh().TenThuocTinh();

        if (_tenThuocTinh === null || _tenThuocTinh === "" || _tenThuocTinh === "undefined") {
            ShowMessage_Danger("Tên thuộc tính không được để trống");
            $('#txtTenThuocT').focus();
            return false;
        }

        var objThuocTinh = {
            ID: _id,
            TenThuocTinh: _tenThuocTinh
        };
        if (_id === undefined) {
            ajaxHelper(DMHangHoaUri + "PostDM_ThuocTinh", 'POST', objThuocTinh).done(function (item) {
                self.arrThuocTinh.push(item);
                var obj = {
                    index: self.Indexx(),
                    ID_ThuocTinh: item.ID,
                    TenThuocTinh: "",
                    GiaTri: [{
                        ID_ThuocTinh: item.ID,
                        TenGiaTri: '',
                        ID: const_GuidEmpty
                    }]
                };

                for (var i = 0; i < self.ThuocTinhCuaHH().length; i++) {
                    if (self.ThuocTinhCuaHH()[i].index === self.Indexx()) {
                        self.ThuocTinhCuaHH.splice(i, 1);
                        self.ThuocTinhCuaHH.push(obj);
                    }
                }
                for (var i = 0; i < self.ThuocTinhCuaHHEdit().length; i++) {
                    if (self.ThuocTinhCuaHHEdit()[i].index === self.Indexx()) {
                        self.ThuocTinhCuaHHEdit.splice(i, 1);
                        self.ThuocTinhCuaHHEdit.push(obj);
                    }
                }
                self.clickChooseTT(obj);
                $('#ddlThuocTinh' + self.Indexx()).val(item.ID);
                ShowMessage_Success("Thêm thuộc tính thành công!");
            }).fail(function () {
                ShowMessage_Danger("Thêm thuộc tính thất bại!");
            }).always(function () {
                Enable_btnSaveThuocTinh();
                $('#modalThemThuocTinh').modal('hide');
            })

        }
        else {
            var myData = {};
            myData.id = _id;
            myData.ThuocTinh = objThuocTinh;

            ajaxHelper(DMHangHoaUri + "PutDM_ThuocTinh", 'POST', myData).done(function () {
                getallThuocTinh();
                ShowMessage_Success("Cập nhật thuộc tính thành công!");
            }).fail(function () {
                ShowMessage_Danger("Cập nhật thuộc tính thất bại!");
            }).always(function () {
                Enable_btnSaveThuocTinh();
                $('#modalThemThuocTinh').modal('hide');
            })
        }
    };

    self.editThuocTinh = function (item) {
        self.deleteID(item.ID_ThuocTinh);
        if (item.ID_ThuocTinh !== undefined) {
            ajaxHelper(DMHangHoaUri + "GetDM_ThuocTinh/" + item.ID_ThuocTinh, 'GET').done(function (data) {
                self.newThuocTinh().setdata(data);
                $('#modalThemThuocTinh').modal('show');
                $('.thuoctinhten').html('Sửa thuộc tính');
            });
        }
    };
    function getNumber_GioiHanSoMatHang() {
        ajaxHelper(DMHangHoaUri + "number_GioiHanSoMatHang", 'GET').done(function (data) {
            self.number_GioiHanSoMatHang(data);
        });
    };
    getNumber_GioiHanSoMatHang();
    self.modaldeleteTT = function (item) {
        $('#modalpopup_deleteThuocTinh').modal('show');
    };

    self.deleteThuocTinh = function (item) {
        $.ajax({
            type: "DELETE",
            url: DMHangHoaUri + "DeleteDM_ThuocTinh/" + self.deleteID(),
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                ShowMessage_Success("Xóa thuộc tính thành công!");
                getallThuocTinh();
            },
            error: function (error) {
                ShowMessage_Danger("Xóa thuộc tính thất bại");
                getallThuocTinh();
            }
        });
    };

    function getallThuocTinh() {
        ajaxHelper(DMHangHoaUri + "GetallThuocTinh", 'GET').done(function (data) {
            self.arrThuocTinh(data);
            if (self.arrThuocTinh().length > 0) {
                $(".close-goods").click(function () {
                    $(this).prev(".op-filter-container ").toggleClass("scoll-tt");
                });
            }
        });
    }


    $('.hideloadGiatri').hide();
    self.arrGiaTriThuocTinh = ko.observableArray();
    self.TenThuocTinhLoad = ko.observable();
    self.LoadGiaTri = function (item) {
        self.TenThuocTinhLoad(item.TenThuocTinh);
        ajaxHelper(DMHangHoaUri + "getGiaTriTTByID_ThuocTinh?idthuoctinh=" + item.ID, 'GET').done(function (data) {
            self.arrGiaTriThuocTinh(data);
        });
        $(".op-filter-container  ul").click(function () {
            var hClass = $(this).parent(".op-filter-container").hasClass('scoll-tt');
            if (hClass == "true") {
                $(this).parent(".op-filter-container").addClass('scoll-tt');
            }
            else {
                $(this).parent(".op-filter-container").removeClass('scoll-tt');
            }

        });
    };
    self.arrListThuocTinh = ko.observableArray();
    self.ListThuocTinh = ko.observableArray();
    self.ListGiaTri_ThuocTinh = ko.observableArray();
    self.hienThiGiaTri = function (item) {
        if (item.ID_ThuocTinh !== undefined) {
            $('span[id=txtTenThuocTinh_' + item.ID_ThuocTinh + ']').html(item.GiaTri);
            var index = self.arrListThuocTinh().map(function (e) { return e.ID_ThuocTinh; }).indexOf(item.ID_ThuocTinh);
            if (index >= 0) {
                self.arrListThuocTinh.remove(self.arrListThuocTinh()[index]);
                self.arrListThuocTinh.push(item);
            }
            else {
                self.arrListThuocTinh.push(item);
            }
        }
        else {
            let arrThuocTinh = self.arrListThuocTinh().map(function (e) { return e.ID_ThuocTinh; });
            let index1 = self.arrListThuocTinh().map(function (e) { return e.ID_ThuocTinh; }).indexOf(item.ID);
            if (index1 >= 0) {
                self.arrListThuocTinh.remove(self.arrListThuocTinh()[index1]);
            }
            $('span[id=txtTenThuocTinh_' + item.ID + ']').html('---' + item.TenThuocTinh + '---');

        }
        self.ListThuocTinh([]);
        for (var i = 0; i < self.arrListThuocTinh().length; i++) {
            self.ListThuocTinh.push(self.arrListThuocTinh()[i].GiaTri + self.arrListThuocTinh()[i].ID_ThuocTinh.toUpperCase())
        }

        SearchHangHoa();
    };

    self.loadData = function () {
        //var lcCT = localStorage.getItem('lc_CTKiemKho');
        //$("#op_js_Table").show();
        var objectStore = db.transaction(table, "readwrite").objectStore(table);
        var req = objectStore.openCursor(key_Add);
        req.onsuccess = function (evt) {
            if (req.result !== null && req.result.value !== null) {
                var lcCT = JSON.parse(req.result.value.Value);
                if (lcCT !== null) {
                    self.newKiemKho().BH_KiemKho_ChiTiet(lcCT);
                    $('#modalpopuploadDaTaKK').modal('hide');
                    $('#txtHangHoaauto').focus();
                    self.SLKhops([]);
                    self.SLLechs([]);
                    self.SLChuaKiems([]);
                    $('#tongitemkhop').text(0);
                    $('#tongitemlech').text(0);
                    $('#tongitem').text(0);
                    $('#tongitemchuakiem').text(0);
                    for (var i = 0; i < self.newKiemKho().BH_KiemKho_ChiTiet().length; i++) {
                        if (parseFloat(self.newKiemKho().BH_KiemKho_ChiTiet()[i].ThanhTien) === parseFloat(self.newKiemKho().BH_KiemKho_ChiTiet()[i].SoLuong)) {
                            self.SLKhops.push(self.newKiemKho().BH_KiemKho_ChiTiet()[i]);
                        }
                        if (self.newKiemKho().BH_KiemKho_ChiTiet()[i].ThanhTien === null) {
                            self.SLChuaKiems.push(self.newKiemKho().BH_KiemKho_ChiTiet()[i]);
                        }
                        if (self.newKiemKho().BH_KiemKho_ChiTiet()[i].ThanhTien !== null && parseFloat(self.newKiemKho().BH_KiemKho_ChiTiet()[i].ThanhTien) !== parseFloat(self.newKiemKho().BH_KiemKho_ChiTiet()[i].SoLuong)) {
                            self.SLLechs.push(self.newKiemKho().BH_KiemKho_ChiTiet()[i]);
                        }
                    }
                    if (self.newKiemKho().BH_KiemKho_ChiTiet().length > 0) {
                        $('#importKiemKho').hide();
                    }
                    else {
                        $('#importKiemKho').show();
                    }
                    $('#tongitemkhop').text(self.SLKhops().length);
                    $('#tongitemlech').text(self.SLLechs().length);
                    $('#tongitem').text(self.newKiemKho().BH_KiemKho_ChiTiet().length);
                    $('#tongitemchuakiem').text(self.SLChuaKiems().length);
                    self.TinhLaiLech();
                    self.TinhLaiSLKhop();
                    self.TinhLaiSLThuc();
                    UpdateAgain_ListDVT();
                    self.currentPageChuaK(self.pagechuaK());
                    self.currentPageLech(self.pagelech());
                    self.currentPageKhop(self.pageKhop());
                }
            }
            $('#wait').remove();
        };
    }

    function getallThietLap() {
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + "GetCauHinhHeThong/" + _IDchinhanh, 'GET').done(function (data) {
            localStorage.setItem('lc_CTThietLap', JSON.stringify(data));
        });
    };


    self.NgungKinhDoanh = function (item) {
        $.ajax({
            type: "DELETE",
            url: DMHangHoaUri + "NgungKinhDoanh_HH?id=" + item.ID_DonViQuiDoi,
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                SearchHangHoa();
                ShowMessage_Success("Cập nhật hàng hóa thành công!");
            },
            error: function (error) {
                ShowMessage_Danger("Cập nhật hàng hóa thất bại!");
            }
        })
    }
    //Cho phép kinh doanh
    self.ChoPhepKinhDoanh = function (item) {
        $.ajax({
            type: "DELETE",
            url: DMHangHoaUri + "ChoKinhDoanh_HH?id=" + item.ID_DonViQuiDoi,
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                SearchHangHoa();
                ShowMessage_Success("Cập nhật hàng hóa thành công!");
            },
            error: function (error) {
                ShowMessage_Danger("Cập nhật hàng hóa thất bại!");
            }
        })
    }

    self.RestoreProduct = function (item) {
        let sloai = '';
        switch (parseInt(item.LoaiHangHoa)) {
            case 1:
                sloai = 'hàng hóa';
                break;
            case 2:
                sloai = 'dịch vụ';
                break;
            case 3:
                sloai = 'combo';
                break;
        }

        dialogConfirm('Khôi phục ' + sloai, 'Bạn có chắc chắn muốn khôi phục '.concat(sloai,
            ' có mã <b> ', item.MaHangHoa, ' </b> không?'), function () {
                ajaxHelper(DMHangHoaUri + 'RestoreProduct/' + item.ID_DonViQuiDoi, 'GET').done(function (x) {
                    if (x) {
                        ShowMessage_Success('Khôi phục ' + sloai + ' thành công');
                        let diary = {
                            ID_NhanVien: _IDNhanVien,
                            ID_DonVi: _IDchinhanh,
                            ChucNang: "Khôi phục " + sloai,
                            NoiDung: "Khôi phục ".concat(sloai, ': ', item.TenHangHoa, ' (', item.MaHangHoa, ')',),
                            NoiDungChiTiet: "Khôi phục ".concat(sloai, ': ', item.TenHangHoa, ' (', item.MaHangHoa, ')',
                                '<br /> Người khôi phục: ', _txtTenTaiKhoan),
                            LoaiNhatKy: 3
                        }
                        Insert_NhatKyThaoTac_1Param(diary);
                        SearchHangHoa();
                    }
                    else {
                        ShowMessage_Danger('Khôi phục ' + sloai + ' thất bại');
                    }
                })
            })
    }

    function getListNhanVien() {
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetNS_NhanVien_InforBasic?idDonVi=" + _IDchinhanh, 'GET').done(function (data) {
            self.NhanViens(data);
            $('#txtNhanVienKK').val(_IDNhanVien);

            self.NhanViens_ChiNhanh(data);

            // find nvlogin
            let nvien = $.grep(self.NhanViens(), function (x) {
                return x.ID === _IDNhanVien;
            });
            if (nvien.length > 0) {
                self.textSearch(nvien[0].TenNhanVien);
                self.newKiemKho().ID_NhanVien(_IDNhanVien);
            }
        });
    }

    self.selectedThuocTinh.subscribe(function (newValue) {
        ajaxHelper(DMHangHoaUri + "GetDM_ThuocTinh/" + newValue, 'GET').done(function (data) {
            var replaceArr = {
                ID: newValue,
                TenThuocTinh: data.TenThuocTinh
            }
            self.ThuocTinhCuaHH.replace(SaveItem, replaceArr);
            self.arrThuocTinhSave(self.ThuocTinhCuaHH());
            self.ThuocTinhCuaHH([]);
            for (var i = 0; i < self.arrThuocTinhSave().length; i++) {
                self.ThuocTinhCuaHH.push(self.arrThuocTinhSave()[i]);
            }
        });
    });
    var SaveItem;
    self.clickChooseTT = function (item) {
        self.btnEditTTEnable(item);
        SaveItem = item;
        self.ThuocTinhCuaHH.refresh();
        if (self.ThuocTinhCuaHH().filter(p => p.ID_ThuocTinh === item.ID_ThuocTinh).length > 1) {
            self.ThuocTinhCuaHH.remove(item);
            ShowMessage_Danger("Thuộc tính này đã có trong hàng hóa. Xin vui lòng chọn thuộc tính khác");
        }
    };

    self.clickChooseTTOld = function (item) {
        self.btnEditTTEnable(item);
        SaveItem = item;
        self.ThuocTinhCuaHHEdit.refresh();
        if (self.ThuocTinhCuaHHEdit().filter(p => p.ID_ThuocTinh === item.ID_ThuocTinh).length > 1) {
            self.ThuocTinhCuaHHEdit.remove(item);
            ShowMessage_Danger("Thuộc tính này đã có trong hàng hóa. Xin vui lòng chọn thuộc tính khác");
        }
    };

    self.btnEditTTEnable = function (item) {
        if (item.ID_ThuocTinh !== undefined) {
            $(function () {
                $('#enableEdit_' + item.index).removeAttr('disabled');
            })
        } else {
            $(function () {
                $('#enableEdit_' + item.index).attr('disabled', 'disabled');
            })
        }
    }


    self.filterFindThuocTinh = function (item, inputString) {
        var itemSearch = locdau(item.TenThuocTinh).toLowerCase();

        var locdauInput = locdau(inputString).toLowerCase();
        var arr = itemSearch.split(/\s+/);

        var sThreechars = '';
        var sThreechars1 = '';
        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }
        return itemSearch.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    }

    self.xoacache = function () {
        var objectStore = db.transaction(table, "readwrite").objectStore(table);
        var req = objectStore.delete(key_Add);
        req.onsuccess = function (evt) {
            $('#txtHangHoaauto').focus();
        };
    };


    self.deleteID_CungLoai = ko.observable();
    self.modalDelete = function (item) {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('HangHoa_Xoa', lc_CTQuyen) > -1) {
            self.deleteMaHangHoa(item.MaHangHoa);
            self.deleteID(item.ID_DonViQuiDoi);
            self.deleteID_CungLoai(item.ID_HangHoaCungLoai);
            $('#modalpopup_deleteHH').modal('show');
        }
    };
    self.modalDeleteNHH = function (NhomHangHoas) {
        self.deleteTenNhomHang(self.newNhomHangHoa().TenNhomHangHoa());
        self.deleteID(self.newNhomHangHoa().ID());
        $('#modalpopup_deleteNHH').modal('show');
    };
    self.keyPressEvent = ko.observable(false);


    self.selectSearchHH = function () {
        getChiTietHangHoaByMaLoHang($('#txtHangHoaauto').val().trim().toUpperCase());
    }

    self.selectHHDM_LoHang = function (item) {
        getChiTietHangHoaByID(item);
    }

    self.selectIDParent = ko.observable(null);
    self.selectIDNhomHHAddHH = ko.observable(null);
    self.selectIDNhomHHChuyenNhom = ko.observable(null);

    self.selectIDNhomHHKiemKho = ko.observable(null);
    self.tenNhomHangChosed = ko.observable('');

    self.selectedNHomHH.subscribe(function (newValue) {
        self.selectIDParent(newValue.ID);
        $('#choose_TenNHH').text(newValue.TenNhomHangHoa);
        $('#lstNhomHang span').each(function () {
            $(this).empty();
        });
        $(function () {
            $('span[id=spanCheckNhom_' + newValue.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
        });
    });

    self.selectedNHomHHByLaHH.subscribe(function (newValue) {
        self.selectIDParent(newValue.ID);
        $('#choose_TenNHHLHH').text(newValue.TenNhomHangHoa);
        $('#lstNhomHangLHH span').each(function () {
            $(this).empty();
        });
        $(function () {
            $('span[id=spanCheckNhom_' + newValue.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
        });
    })

    self.selectedNHomHHSua.subscribe(function (newValue) {
        self.selectIDParent(newValue.ID);
        $('#choose_TenNHHSua').text(newValue.TenNhomHangHoa);
        $('#lstNhomHangSua span').each(function () {
            $(this).empty();
        });
        $(function () {
            $('span[id=spanCheckNhomSua_' + newValue.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
        });
    });

    self.selectIDNHHAdd.subscribe(function (newValue) {
        if (newValue !== undefined) {
            self.selectIDNhomHHAddHH(newValue.ID);
            $('#choose_TenNHHAddHH').text(newValue.TenNhomHangHoa);
            self.tenNhomHangChosed(newValue.TenNhomHangHoa);
            $('#lstNhomHangAddHH span').each(function () {
                $(this).empty();
            });
            $(function () {
                $('span[id=spanCheckNhomAddHH_' + newValue.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
            });
        }
    });

    self.selectIDNHHChuyenNhom.subscribe(function (newValue) {
        if (newValue !== undefined) {
            self.selectIDNhomHHChuyenNhom(newValue.ID);
            $('#choose_TenNHHChuyenNhom').text(newValue.TenNhomHangHoa);
            $('#lstNhomHangChuyenNhom span').each(function () {
                $(this).empty();
            });
            $(function () {
                $('span[id=spanCheckNhomChuyenNhom_' + newValue.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
            });
        }
    });

    self.selectIDNHHKiemKho.subscribe(function (newValue) {
        if (newValue !== undefined) {
            self.selectIDNhomHHKiemKho(newValue.ID);
            $('#choose_TenNHHKiemKho').text(newValue.TenNhomHangHoa);
            $('#lstNhomHangKiemKho span').each(function () {
                $(this).empty();
            });
            $(function () {
                $('span[id=spanCheckNhomKiemKho_' + newValue.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
            });
        }
    });
    self.selectIDNHHAddDV.subscribe(function (newValue) {
        if (newValue !== undefined) {
            self.selectIDNhomHHAddHH(newValue.ID);
            $('#choose_TenNHHAddDV').text(newValue.TenNhomHangHoa);
            self.tenNhomHangChosed(newValue.TenNhomHangHoa);
            $('#lstNhomHangAddDV span').each(function () {
                $(this).empty();
            });
            $(function () {
                $('span[id=spanCheckNhomAddDV_' + newValue.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
            });
        }
    });

    self.clickFistChon = function () {
        self.selectIDParent(null);
        $('#lstNhomHang span').each(function () {
            $(this).empty();
        });
        $('#choose_TenNHH').text('---Chọn nhóm---');
    }

    self.clickFistChonAddHH = function () {
        self.selectIDNhomHHAddHH(null);
        self.tenNhomHangChosed('Nhóm hàng hóa mặc định');
        $('#lstNhomHangAddHH span').each(function () {
            $(this).empty();
        });
        $('#choose_TenNHHAddHH').text('---Chọn nhóm---');
    }

    self.clickFistChonChuyenNhom = function () {
        self.selectIDNhomHHChuyenNhom(null);
        $('#lstNhomHangChuyenNhom span').each(function () {
            $(this).empty();
        });
        $('#choose_TenNHHChuyenNhom').text('---Chọn nhóm---');
    }

    self.clickFistChonNHHKiemKho = function () {
        self.selectIDNhomHHKiemKho(null);
        $('#lstNhomHangKiemKho span').each(function () {
            $(this).empty();
        });
        $('#choose_TenNHHKiemKho').text('---Chọn tất cả nhóm hàng hóa---');
    }

    self.clickFistChonAddDV = function () {
        self.selectIDNhomHHAddHH(null);
        self.tenNhomHangChosed('Nhóm dịch vụ mặc định');
        $('#lstNhomHangAddDV span').each(function () {
            $(this).empty();
        });
        $('#choose_TenNHHAddDV').text('---Chọn nhóm---');
    }

    $("#clickChonNhom").click(function () {
        self.selectIDNhomHHKiemKho(null);
        $('#choose_TenNHHKiemKho').text('---Chọn tất cả nhóm hàng hóa---');
        $('#lstNhomHangKiemKho span').each(function () {
            $(this).empty();
        });
    })


    self.clickFistChonSua = function () {
        self.selectIDParent(null);
        $('#lstNhomHangSua span').each(function () {
            $(this).empty();
        });
        $('#choose_TenNHHSua').text('---Chọn nhóm---');
    }

    //function focusinput() {
    //    if (self.selectIDNhomHHAddHH() !== null) {
    //        $('#txtGiaBan').focus().select();
    //        $('#txtGiaBanDV').focus().select();
    //    }
    //}

    self.clickKiemHang = function (str) {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('KiemKho_ThemMoi', lc_CTQuyen) > -1) {

            localStorage.removeItem('lc_UpdateKK');
            localStorage.setItem('isUpdate', false);
            localStorage.setItem('isSaoChepKK', false);
            clickloadForm(str);
        }
        else {
            ShowMessage_Danger("Không có quyền thêm mới phiếu kiểm kê!");
        }
    }
    var _MaHangHoaNhap;
    var checkinputnhanh = 0;
    self.DonViTinhs = ko.observableArray();
    var indexedDB = window.indexedDB || window.mozIndexedDB || window.webkitIndexedDB || window.msIndexedDB || window.shimIndexedDB;
    // Open (or create) the database
    var open = indexedDB.open("BanHang24-Cache");
    // Create the schema
    var db;
    var table = "Storage";
    open.onupgradeneeded = function () {
        db = open.result;
        var store = db.createObjectStore(table, { keyPath: "Key" });
        store.createIndex("Key", "Key", { unique: false });
        store.createIndex("Value", "Value", { unique: false });
    };
    var CheckOpenStore = false;
    open.onsuccess = function () {
        //localStorage.removeItem('lc_UpdateKK');
        CheckOpenStore = true;
        db = open.result;
        if (loaiHoaDon === "9") {
            getallKiemKho();
        } else {
            getAllDMHangHoas();
            var addQuick = localStorage.getItem('InsertQuickly');
            if (addQuick !== null) {
                addQuick = parseInt(addQuick);
                if (addQuick === 1) {
                    localStorage.removeItem('InsertQuickly');
                    self.themmoicapnhat();
                }
            }
        }

        var objectStore = db.transaction(table, "readonly").objectStore(table);
        var req = objectStore.openCursor(key_Add);
        req.onsuccess = function (evt) {
            if (req.result !== null && req.result.value !== null && req.result.value.Value !== null) {
                var lcCT = JSON.parse(req.result.value.Value);
                //var lcCT = localStorage.getItem('lc_CTKiemKho');
                if (lcCT !== null && lcCT.length > 0 && lcCT.indexOf('[]') !== 0 && localStorage.getItem('isUpdate') === 'false' && localStorage.getItem('isSaoChepKK') === 'false') {
                    $('#modalpopuploadDaTaKK').modal('show');
                }
            }
        };
    };

    var key_Add = "key_Add";
    var key_Update = "key_Update";

    var itemNhap_Cham = null;
    function getChiTietHangHoaByID(itemHH) {
        _MaHangHoaNhap = itemHH.MaHangHoa;
        var checkblock = $('.number-fast').css('display');
        checkinputnhanh = 0;
        if (checkblock == 'block') {
            checkinputnhanh = 1;
        } else {
            checkinputnhanh = 0;
        }
        if (checkinputnhanh == 0) {
            //var lc_CTKiemKho = localStorage.getItem('lc_CTKiemKho');
            if (itemHH !== null) {
                var _ngayKk = $('#datetimepicker').val();
                if (_ngayKk === "") {
                    _ngayKk = moment(new Date()).format('DD/MM/YYYY HH:mm');
                }
                var check = CheckNgayLapHD_format(_ngayKk);
                if (!check) {
                    return false;
                }
                var _ngayKiemKe = moment(_ngayKk, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
                ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "TonCuaHangHoaNotByLoHangKiemKho?idhanghoa=" + itemHH.ID + '&id_donvi=' + _IDchinhanh + '&timeKK=' + _ngayKiemKe + '&idlohang=' + itemHH.ID_LoHang, 'GET').done(function (slton) {
                    var trans = db.transaction(table, "readwrite");
                    var store = trans.objectStore(table);
                    var req = store.openCursor(key_Add);
                    //Open indexDB key_add
                    req.onsuccess = function (e) {
                        var found = -1;
                        var rd = Math.floor(Math.random() * 1000000 + 1);
                        var idRandom = 'IDRandom' + rd + '_';
                        itemHH.MaLoHang = itemHH.MaLoHang === '' ? null : itemHH.MaLoHang;
                        var quanLiTheoLo = itemHH.QuanLyTheoLoHang;
                        var tonkhoDB = parseFloat((slton / itemHH.TyLeChuyenDoi).toFixed(3));

                        var ob1 = {
                            ID: itemHH.ID,
                            ID_HangHoa: itemHH.ID,
                            TenHangHoa: itemHH.TenHangHoa,
                            ID_DonViQuiDoi: itemHH.ID_DonViQuiDoi,
                            SoLuong: tonkhoDB, //TonKho
                            TenDonViTinh: itemHH.TenDonViTinh,
                            MaHangHoa: itemHH.MaHangHoa,
                            ThanhTien: tonkhoDB, //SLThuc
                            ThucTe: tonkhoDB,
                            TienChietKhau: 0, //SLLech
                            ThanhToan: 0, // GiaTriLech
                            GiaVon: itemHH.GiaVon,
                            ThuocTinh_GiaTri: itemHH.ThuocTinh_GiaTri,
                            TyLeChuyenDoi: itemHH.TyLeChuyenDoi,
                            QuanLyTheoLoHang: quanLiTheoLo,
                            ID_LoHang: itemHH.ID_LoHang,
                            LotParent: quanLiTheoLo,
                            ID_Random: idRandom,
                            NgaySanXuat: itemHH.NgaySanXuat !== '' ? (itemHH.NgaySanXuat) : '',
                            NgayHetHan: itemHH.NgayHetHan !== '' ? (itemHH.NgayHetHan) : '',
                            MaLoHang: itemHH.MaLoHang,
                            DonViTinh: itemHH.DonViTinh

                            //LoHangHoa: false
                        };
                        //if (quanLiTheoLo) {
                        //    var objLot = newLot(ob1, null);
                        //    ob1.DM_LoHang.push(objLot);
                        //}
                        var slThuc = tonkhoDB;
                        var thucte = tonkhoDB;
                        var slLech = parseFloat((slThuc - slton / itemHH.TyLeChuyenDoi).toFixed(3));
                        var thanhTien = parseFloat(Math.ceil(itemHH.GiaVon * (slThuc - slton / itemHH.TyLeChuyenDoi)).toFixed());
                        var data_Add;
                        var lc_CTKiemKho = null;
                        if (req.result !== null && req.result.value != null) {
                            data_Add = req.result.value;
                            lc_CTKiemKho = JSON.parse(req.result.value.Value);
                            store.delete(key_Add);
                        }

                        if (lc_CTKiemKho === null) {
                            lc_CTKiemKho = [];

                            var lc_UpdateKK = localStorage.getItem('lc_UpdateKK');
                            if (lc_UpdateKK !== null) {
                                store.add({ Key: key_Add, Value: lc_UpdateKK });
                                lc_CTKiemKho = JSON.parse(lc_UpdateKK);
                                for (var i = 0; i < lc_CTKiemKho.length; i++) {
                                    if (lc_CTKiemKho[i].MaHangHoa == _MaHangHoaNhap && lc_CTKiemKho[i].MaLoHang === itemHH.MaLoHang) {
                                        found = i;
                                        slThuc = parseFloat(lc_CTKiemKho[i].ThanhTien) + 1;
                                        thucte = lc_CTKiemKho[i].SoLuong;
                                        slLech = parseFloat((slThuc - thucte).toFixed(3));
                                        thanhTien = Math.ceil((slLech * parseFloat(lc_CTKiemKho[i].GiaVon)).toFixed());
                                        break;
                                    }
                                }
                            }
                        }
                        else {
                            for (var i = 0; i < lc_CTKiemKho.length; i++) {
                                if (lc_CTKiemKho[i].MaHangHoa === _MaHangHoaNhap && lc_CTKiemKho[i].MaLoHang === itemHH.MaLoHang) {
                                    found = i;
                                    slThuc = parseFloat(lc_CTKiemKho[i].ThanhTien) + 1;
                                    thucte = lc_CTKiemKho[i].SoLuong;
                                    slLech = parseFloat((slThuc - thucte).toFixed(3));
                                    thanhTien = Math.ceil((slLech * parseFloat(lc_CTKiemKho[i].GiaVon)).toFixed());
                                    break;
                                }
                            }
                        }
                        var isUpdate = localStorage.getItem('isUpdate');
                        if (found < 0) {
                            ob1.ThanhTien = slThuc;
                            ob1.TienChietKhau = slLech;
                            ob1.ThanhToan = thanhTien;
                            ob1.ThucTe = thucte;
                            lc_CTKiemKho.unshift(ob1);
                            store.add({ Key: key_Add, Value: JSON.stringify(lc_CTKiemKho) });
                            //localStorage.setItem('lc_CTKiemKho', JSON.stringify(lc_CTKiemKho));
                        }
                        else {
                            for (var i = 0; i < lc_CTKiemKho.length; i++) {
                                if (lc_CTKiemKho[i].MaHangHoa === _MaHangHoaNhap && lc_CTKiemKho[i].MaLoHang === itemHH.MaLoHang) {
                                    // remove 1 item at index = i
                                    lc_CTKiemKho.splice(i, 1);
                                    break;
                                }
                            }
                            //store.add({ Key: key_Add, Value: JSON.stringify(lc_CTKiemKho) });
                            //localStorage.setItem('lc_CTKiemKho', JSON.stringify(lc_CTKiemKho));
                            ob1.ThanhTien = slThuc;
                            ob1.TienChietKhau = slLech;
                            ob1.ThanhToan = thanhTien;
                            ob1.ThucTe = thucte;
                            lc_CTKiemKho.unshift(ob1);
                            store.add({ Key: key_Add, Value: JSON.stringify(lc_CTKiemKho) });

                        }
                        self.newKiemKho().BH_KiemKho_ChiTiet(lc_CTKiemKho);
                        UpdateAgain_ListDVT();
                        if (self.newKiemKho().BH_KiemKho_ChiTiet().length > 0) {
                            $('#importKiemKho').hide();
                        }
                        else {
                            $('#importKiemKho').show();
                        }
                        $('#tongitem').text(self.newKiemKho().BH_KiemKho_ChiTiet().length);
                        self.loadData();
                        $('#txtHangHoaauto').val(itemHH.MaHangHoa);
                        $('#makiemkho').focus();
                        $('#txtHangHoaauto').focus()
                        $(document).ready(function () {
                            document.getElementById('txtHangHoaauto').select();
                        });
                    };
                });
            }
            else {
                $('#txtHangHoaauto').select();
            }
            //});
        }
        else {
            itemNhap_Cham = itemHH;
            ajaxHelper('/api/DanhMuc/DM_HangHoaAPI/' + 'getdonviquidoibymahanghoa?maHangHoa=' + itemHH.MaHangHoa, 'GET').done(function (data) {
                self.DonViTinhs(data);
                if (self.DonViTinhs().length > 1) {
                    document.getElementById("ddlDonViTinh").disabled = false;
                } else {
                    document.getElementById("ddlDonViTinh").disabled = true;
                }
                for (var i = 0; i < self.DonViTinhs().length; i++) {
                    if (self.DonViTinhs()[i].MaHangHoa == itemHH.MaHangHoa) {
                        self.selectIDDonViTinh(self.DonViTinhs()[i].ID);
                    }
                }
            })
            $('#txtSoLuongHang').val(1);
            $('#txtSoLuongHang').focus().select();
        }



    }

    function getChiTietHangHoaByMaLoHang(mahh) {
        _MaHangHoaNhap = mahh;
        var checkblock = $('.number-fast').css('display');
        checkinputnhanh = 0;
        ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetHangHoa_ByMaHangHoa?mahh=" + commonStatisJs.URLEncoding(mahh) + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
            data = data.filter(p => p.LaHangHoa === true);
            itemNhap_Cham = data[0];
        });
        if (checkblock === 'block') {
            checkinputnhanh = 1;
        } else {
            checkinputnhanh = 0;
        }
        if (checkinputnhanh == 0) {
            //var lc_CTKiemKho = localStorage.getItem('lc_CTKiemKho');
            ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetHangHoa_ByMaHangHoa?mahh=" + commonStatisJs.URLEncoding(mahh) + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
                data = data.filter(p => p.LaHangHoa === true);
                if (data[0] !== undefined) {
                    var _ngayKk = $('#datetimepicker').val();
                    if (_ngayKk === "") {
                        _ngayKk = moment(new Date()).format('DD/MM/YYYY HH:mm');
                    }
                    var check = CheckNgayLapHD_format(_ngayKk);
                    if (!check) {
                        return false;
                    }
                    var _ngayKiemKe = moment(_ngayKk, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
                    ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "TonCuaHangHoaNotByLoHangKiemKho?idhanghoa=" + data[0].ID + '&id_donvi=' + _IDchinhanh + '&timeKK=' + _ngayKiemKe + '&idlohang=' + data[0].ID_LoHang, 'GET').done(function (slton) {
                        var trans = db.transaction(table, "readwrite");
                        var store = trans.objectStore(table);
                        var req = store.openCursor(key_Add);
                        //Open indexDB key_add
                        req.onsuccess = function (e) {
                            var found = -1;
                            var rd = Math.floor(Math.random() * 1000000 + 1);
                            var idRandom = 'IDRandom' + rd + '_';

                            var quanLiTheoLo = data[0].QuanLyTheoLoHang;
                            var tonkhoDB = parseFloat((slton / data[0].TyLeChuyenDoi).toFixed(3));
                            var ob1 = {
                                ID: data[0].ID,
                                TenHangHoa: data[0].TenHangHoa,
                                ID_DonViQuiDoi: data[0].ID_DonViQuiDoi,
                                SoLuong: tonkhoDB,
                                TenDonViTinh: data[0].TenDonViTinh,
                                MaHangHoa: data[0].MaHangHoa,
                                ThanhTien: tonkhoDB, //SLThuc
                                ThucTe: tonkhoDB,
                                TienChietKhau: 0, //SLLech
                                ThanhToan: 0, // GiaTriLech
                                GiaVon: data[0].GiaVon,
                                ThuocTinh_GiaTri: data[0].ThuocTinh_GiaTri,
                                TyLeChuyenDoi: data[0].TyLeChuyenDoi,
                                QuanLyTheoLoHang: quanLiTheoLo,
                                ID_LoHang: data[0].ID_LoHang,
                                LotParent: quanLiTheoLo,
                                ID_Random: idRandom,
                                NgaySanXuat: data[0].NgaySanXuat !== '' ? data[0].NgaySanXuat : '',
                                NgayHetHan: data[0].NgayHetHan !== '' ? data[0].NgayHetHan : '',
                                MaLoHang: data[0].MaLoHang,
                                DonViTinh: data[0].DonViTinh

                                //LoHangHoa: false
                            }
                            //if (quanLiTheoLo) {
                            //    var objLot = newLot(ob1, null);
                            //    ob1.DM_LoHang.push(objLot);
                            //}
                            var slThuc = tonkhoDB;
                            var thucte = tonkhoDB;
                            var slLech = parseFloat((slThuc - slton / data[0].TyLeChuyenDoi).toFixed(3));
                            var thanhTien = parseFloat((data[0].GiaVon * (slThuc - slton / data[0].TyLeChuyenDoi)).toFixed());
                            var data_Add;
                            var lc_CTKiemKho = null;
                            if (req.result !== null && req.result.value != null) {
                                data_Add = req.result.value;
                                lc_CTKiemKho = JSON.parse(req.result.value.Value);
                                store.delete(key_Add);
                            }
                            if (lc_CTKiemKho === null) {
                                lc_CTKiemKho = [];

                                var lc_UpdateKK = localStorage.getItem('lc_UpdateKK');
                                if (lc_UpdateKK != null) {
                                    store.add({ Key: key_Add, Value: lc_UpdateKK });
                                    lc_CTKiemKho = JSON.parse(lc_UpdateKK);
                                    for (var i = 0; i < lc_CTKiemKho.length; i++) {
                                        if (lc_CTKiemKho[i].MaHangHoa == mahh && lc_CTKiemKho[i].MaLoHang === data[0].MaLoHang) {
                                            found = i;
                                            slThuc = parseFloat(lc_CTKiemKho[i].ThanhTien) + 1;
                                            thucte = lc_CTKiemKho[i].SoLuong;
                                            slLech = parseFloat((slThuc - thucte).toFixed(3));
                                            thanhTien = (slLech * parseFloat(lc_CTKiemKho[i].GiaVon)).toFixed();
                                            break;
                                        }
                                    }
                                }
                            }
                            else {
                                for (var i = 0; i < lc_CTKiemKho.length; i++) {
                                    if (lc_CTKiemKho[i].MaHangHoa === mahh && lc_CTKiemKho[i].MaLoHang === data[0].MaLoHang) {
                                        found = i;
                                        slThuc = parseFloat(lc_CTKiemKho[i].ThanhTien) + 1;
                                        thucte = lc_CTKiemKho[i].SoLuong;
                                        slLech = parseFloat((slThuc - thucte).toFixed(3));
                                        thanhTien = (slLech * parseFloat(lc_CTKiemKho[i].GiaVon)).toFixed();
                                        break;
                                    }
                                }
                            }
                            var isUpdate = localStorage.getItem('isUpdate');
                            if (found < 0) {
                                ob1.ThanhTien = slThuc;
                                ob1.TienChietKhau = slLech;
                                ob1.ThanhToan = thanhTien;
                                ob1.ThucTe = thucte;
                                lc_CTKiemKho.unshift(ob1);
                                store.add({ Key: key_Add, Value: JSON.stringify(lc_CTKiemKho) });
                                //localStorage.setItem('lc_CTKiemKho', JSON.stringify(lc_CTKiemKho));
                            }
                            else {

                                for (var i = 0; i < lc_CTKiemKho.length; i++) {
                                    if (lc_CTKiemKho[i].MaHangHoa == mahh && lc_CTKiemKho[i].MaLoHang === data[0].MaLoHang) {
                                        // remove 1 item at index = i
                                        lc_CTKiemKho.splice(i, 1);
                                        break;
                                    }
                                }
                                //store.add({ Key: key_Add, Value: JSON.stringify(lc_CTKiemKho) });
                                //localStorage.setItem('lc_CTKiemKho', JSON.stringify(lc_CTKiemKho));
                                ob1.ThanhTien = slThuc;
                                ob1.TienChietKhau = slLech;
                                ob1.ThanhToan = thanhTien;
                                ob1.ThucTe = thucte;
                                lc_CTKiemKho.unshift(ob1);
                                store.add({ Key: key_Add, Value: JSON.stringify(lc_CTKiemKho) });

                            }
                            self.newKiemKho().BH_KiemKho_ChiTiet(lc_CTKiemKho);
                            UpdateAgain_ListDVT();
                            if (self.newKiemKho().BH_KiemKho_ChiTiet().length > 0) {
                                $('#importKiemKho').hide();
                            }
                            else {
                                $('#importKiemKho').show();
                            }
                            $('#tongitem').text(self.newKiemKho().BH_KiemKho_ChiTiet().length);
                            self.loadData();
                            $('#txtHangHoaauto').val(data[0].MaHangHoa);
                            $('#makiemkho').focus();
                            $('#txtHangHoaauto').focus()
                            $(document).ready(function () {
                                document.getElementById('txtHangHoaauto').select();
                            })
                        }
                    });
                }
                else {
                    $('#txtHangHoaauto').select();
                }
            });
        }
        else {
            ajaxHelper('/api/DanhMuc/DM_HangHoaAPI/' + 'getdonviquidoibymahanghoa?maHangHoa=' + mahh, 'GET').done(function (data) {
                self.DonViTinhs(data);
                if (self.DonViTinhs().length > 1) {
                    document.getElementById("ddlDonViTinh").disabled = false;
                } else {
                    document.getElementById("ddlDonViTinh").disabled = true;
                }
                for (var i = 0; i < self.DonViTinhs().length; i++) {
                    if (self.DonViTinhs()[i].MaHangHoa == mahh) {
                        self.selectIDDonViTinh(self.DonViTinhs()[i].ID);
                    }
                }
            })
            $('#txtSoLuongHang').val(1);
            $('#txtSoLuongHang').focus().select();
        }
    }

    $('#txtSoLuongHang').keypress(function (e) {
        if (e.keyCode === 13) {
            if (checkinputnhanh == 1) {
                var soluongNhap = $('#txtSoLuongHang').val();
                if (soluongNhap >= 0 && soluongNhap !== "") {
                    if (itemNhap_Cham !== null) {
                        var _ngayKk = $('#datetimepicker').val();
                        if (_ngayKk === "") {
                            _ngayKk = moment(new Date()).format('DD/MM/YYYY HH:mm');
                        }
                        var check = CheckNgayLapHD_format(_ngayKk);
                        if (!check) {
                            return false;
                        }
                        var _ngayKiemKe = moment(_ngayKk, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
                        ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "TonCuaHangHoaNotByLoHangKiemKho?idhanghoa=" + itemNhap_Cham.ID + '&id_donvi=' + _IDchinhanh + '&timeKK=' + _ngayKiemKe + '&idlohang=' + itemNhap_Cham.ID_LoHang, 'GET').done(function (slton) {
                            var trans = db.transaction(table, "readwrite");
                            var store = trans.objectStore(table);
                            var req = store.openCursor(key_Add);

                            //Open indexDB key_add
                            req.onsuccess = function (e) {
                                var found = -1;
                                var rd = Math.floor(Math.random() * 1000000 + 1);
                                var idRandom = 'IDRandom' + rd + '_';

                                itemNhap_Cham.MaLoHang = itemNhap_Cham.MaLoHang === '' ? null : itemNhap_Cham.MaLoHang;
                                var quanLiTheoLo = itemNhap_Cham.QuanLyTheoLoHang;
                                var tonkhoDB = parseFloat((slton / itemNhap_Cham.TyLeChuyenDoi).toFixed(3));
                                var ob1 = {
                                    ID: itemNhap_Cham.ID, // ID_DonViQuyDoi
                                    TenHangHoa: itemNhap_Cham.TenHangHoa,
                                    ID_DonViQuiDoi: itemNhap_Cham.ID_DonViQuiDoi,
                                    SoLuong: tonkhoDB, //TonKho
                                    TenDonViTinh: itemNhap_Cham.TenDonViTinh,
                                    MaHangHoa: itemNhap_Cham.MaHangHoa,
                                    ThanhTien: tonkhoDB, //SLThuc
                                    ThucTe: tonkhoDB,
                                    TienChietKhau: 0, //SLLech
                                    ThanhToan: 0, // GiaTriLech
                                    GiaVon: itemNhap_Cham.GiaVon,
                                    ThuocTinh_GiaTri: itemNhap_Cham.ThuocTinh_GiaTri,
                                    TyLeChuyenDoi: itemNhap_Cham.TyLeChuyenDoi,
                                    QuanLyTheoLoHang: quanLiTheoLo,
                                    ID_LoHang: itemNhap_Cham.ID_LoHang,
                                    LotParent: quanLiTheoLo,
                                    ID_Random: idRandom,
                                    NgaySanXuat: itemNhap_Cham.NgaySanXuat,
                                    NgayHetHan: itemNhap_Cham.NgayHetHan,
                                    MaLoHang: itemNhap_Cham.MaLoHang,
                                    DonViTinh: itemNhap_Cham.DonViTinh
                                };

                                var slThuc = parseFloat(soluongNhap);
                                var thucte = 1;
                                var slLech = parseFloat((parseFloat(soluongNhap) - slton / itemNhap_Cham.TyLeChuyenDoi).toFixed(3));
                                var thanhTien = parseFloat((itemNhap_Cham.GiaVon * (parseFloat(soluongNhap) - slton / itemNhap_Cham.TyLeChuyenDoi)).toFixed());
                                var data_Add;
                                var lc_CTKiemKho = null;
                                if (req.result !== null && req.result.value != null) {
                                    data_Add = req.result.value;
                                    lc_CTKiemKho = JSON.parse(req.result.value.Value);
                                    store.delete(key_Add);
                                }
                                if (lc_CTKiemKho == null) {
                                    lc_CTKiemKho = [];

                                    var store2 = db.transaction(table, "readwrite").objectStore(table);
                                    var req2 = store2.openCursor(key_Update);

                                    req2.onsuccess = function () {
                                        //var lc_UpdateKK = localStorage.getItem('lc_UpdateKK');
                                        if (req2.result !== null && req2.result.value != null) {
                                            var lc_UpdateKK = req2.result.value.Value;
                                            store2.add({ Key: key_Add, Value: lc_UpdateKK });

                                            //localStorage.setItem('lc_CTKiemKho', lc_UpdateKK);
                                            lc_CTKiemKho = JSON.parse(lc_UpdateKK);
                                            for (var i = 0; i < lc_CTKiemKho.length; i++) {
                                                if (lc_CTKiemKho[i].MaHangHoa == _MaHangHoaNhap && lc_CTKiemKho[i].MaLoHang === itemNhap_Cham.MaLoHang) {
                                                    found = i;
                                                    slThuc = parseFloat(lc_CTKiemKho[i].ThanhTien) + parseFloat(soluongNhap);
                                                    thucte = lc_CTKiemKho[i].SoLuong;
                                                    slLech = parseFloat((slThuc - thucte).toFixed(3));
                                                    thanhTien = (slLech * parseFloat(lc_CTKiemKho[i].GiaVon)).toFixed();
                                                    break;
                                                }
                                            }
                                        }
                                    };

                                }
                                else {

                                    for (var i = 0; i < lc_CTKiemKho.length; i++) {
                                        if (lc_CTKiemKho[i].MaHangHoa === _MaHangHoaNhap && lc_CTKiemKho[i].MaLoHang === itemNhap_Cham.MaLoHang) {
                                            found = i;
                                            slThuc = parseFloat(lc_CTKiemKho[i].ThanhTien) + parseFloat(soluongNhap);
                                            thucte = lc_CTKiemKho[i].SoLuong;
                                            slLech = parseFloat((slThuc - thucte).toFixed(3));
                                            thanhTien = (slLech * parseFloat(lc_CTKiemKho[i].GiaVon)).toFixed();
                                            break;
                                        }
                                    }
                                }
                                var isUpdate = localStorage.getItem('isUpdate');
                                if (found < 0) {
                                    ob1.ThanhTien = slThuc;
                                    ob1.TienChietKhau = slLech;
                                    ob1.ThanhToan = thanhTien;
                                    ob1.ThucTe = thucte;
                                    lc_CTKiemKho.unshift(ob1);
                                    store.add({ Key: key_Add, Value: JSON.stringify(lc_CTKiemKho) });
                                    //localStorage.setItem('lc_CTKiemKho', JSON.stringify(lc_CTKiemKho));
                                }
                                else {

                                    for (var i = 0; i < lc_CTKiemKho.length; i++) {
                                        if (lc_CTKiemKho[i].MaHangHoa === _MaHangHoaNhap && lc_CTKiemKho[i].MaLoHang === itemNhap_Cham.MaLoHang) {
                                            // remove 1 item at index = i
                                            lc_CTKiemKho.splice(i, 1);
                                            break;
                                        }
                                    }
                                    //store.add({ Key: key_Add, Value: JSON.stringify(lc_CTKiemKho) });
                                    //localStorage.setItem('lc_CTKiemKho', JSON.stringify(lc_CTKiemKho));
                                    ob1.ThanhTien = slThuc;
                                    ob1.TienChietKhau = slLech;
                                    ob1.ThanhToan = thanhTien;
                                    ob1.ThucTe = thucte;
                                    lc_CTKiemKho.unshift(ob1);
                                    //for (var i = 0; i < lc_CTKiemKho.length; i++) {
                                    //    if (lc_CTKiemKho[i].TienChietKhau == null || lc_CTKiemKho[i].ThanhToan == null) {
                                    //        lc_CTKiemKho.splice(i, 1);

                                    //    }
                                    //}
                                    store.add({ Key: key_Add, Value: JSON.stringify(lc_CTKiemKho) });

                                    //localStorage.setItem('lc_CTKiemKho', JSON.stringify(lc_CTKiemKho));
                                }
                                self.newKiemKho().BH_KiemKho_ChiTiet(lc_CTKiemKho);
                                self.loadData();
                                $('#txtHangHoaauto').keypress(function (e) {
                                    if (e.keyCode === 13) {
                                        self.keyPressEvent(true);
                                    } else {
                                        self.keyPressEvent(false);
                                    }
                                });
                                $('#txtHangHoaauto').val(itemNhap_Cham.MaHangHoa);
                                $('#makiemkho').focus();
                                $('#txtHangHoaauto').focus();
                                $(document).ready(function () {
                                    document.getElementById('txtHangHoaauto').select();
                                });
                            };
                            UpdateAgain_ListDVT();

                        });
                    }
                    else {
                        $('#txtHangHoaauto').select();
                    }
                }
                else {
                    ShowMessage_Danger("Số lượng phải lớn hơn 0!");
                    $('#txtSoLuongHang').select();
                }
            }
        }
    });

    self.ChangeNgayKiemKho = function () {
        var _ngayKk = $('#datetimepicker').val();
        if (_ngayKk === "") {
            _ngayKk = moment(new Date()).format('DD/MM/YYYY HH:mm');
        }
        var check = CheckNgayLapHD_format(_ngayKk);
        if (!check) {
            return false;
        }

        var arrIDQuiDoi = [], arrIDLoHang = [];
        for (let i = 0; i < self.newKiemKho().BH_KiemKho_ChiTiet().length; i++) {
            let itFor = self.newKiemKho().BH_KiemKho_ChiTiet()[i];
            arrIDQuiDoi.push(itFor.ID_DonViQuiDoi);
            arrIDLoHang.push(itFor.ID_LoHang);
        }
        arrIDLoHang = $.grep(arrIDLoHang, function (x) {
            return x !== null;
        })

        var obj = {
            ID_ChiNhanh: _IDchinhanh,
            ToDate: moment(_ngayKk, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm'),
            ListIDQuyDoi: arrIDQuiDoi,
            ListIDLoHang: arrIDLoHang
        }

        if (arrIDQuiDoi.length > 0) {
            ajaxHelper(DMHangHoaUri + 'GetTonKho_byIDQuyDois', 'POST', obj).done(function (x) {
                if (x.res) {
                    let arrCT = [];
                    for (let i = 0; i < self.newKiemKho().BH_KiemKho_ChiTiet().length; i++) {
                        let forOut = self.newKiemKho().BH_KiemKho_ChiTiet()[i];
                        for (let j = 0; j < x.data.length; j++) {
                            let forIn = x.data[j];
                            if (forIn.ID_DonViQuiDoi === forOut.ID_DonViQuiDoi && forIn.ID_LoHang === forOut.ID_LoHang) {
                                self.newKiemKho().BH_KiemKho_ChiTiet()[i].SoLuong = forIn.TonKho;
                                self.newKiemKho().BH_KiemKho_ChiTiet()[i].TienChietKhau = forOut.ThanhTien - forIn.TonKho;// chenhlech = thucte- tonkho DB
                                self.newKiemKho().BH_KiemKho_ChiTiet()[i].ThanhToan = forOut.TienChietKhau * forIn.GiaVon;// gtri lech
                                break;
                            }
                        }
                        arrCT.push(self.newKiemKho().BH_KiemKho_ChiTiet()[i]);
                    }
                    self.newKiemKho().BH_KiemKho_ChiTiet($.extend(true, [], arrCT));

                    var trans = db.transaction(table, "readwrite");
                    var store = trans.objectStore(table);
                    var req = store.openCursor(key_Add);
                    req.onsuccess = function (e) {
                        store.put({ Key: key_Add, Value: JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet()) });
                    }
                    self.loadData();
                }
                else {
                    ShowMessage_Danger(x.mes);
                }
            })
        }
    }

    self.ClickDVTOrther = function (itemclick, itemold) {
        var _ngayKk = $('#datetimepicker').val();
        if (_ngayKk === "") {
            _ngayKk = moment(new Date()).format('DD/MM/YYYY HH:mm');
        }
        var check = CheckNgayLapHD_format(_ngayKk);
        if (!check) {
            return false;
        }
        var _ngayKiemKe = moment(_ngayKk, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
        ajaxHelper(DMHangHoaUri + 'GetDonViTinhKhacGiaoDich?iddvqd=' + itemclick.ID_DonViQuiDoi + '&iddv=' + _IDchinhanh + '&ngaykk=' + _ngayKiemKe, 'GET').done(function (data) {
            data = data.filter(p => p.ID_LoHang === itemold.ID_LoHang);
            var store = db.transaction(table, "readwrite").objectStore(table);
            var req = store.openCursor(key_Add);
            req.onsuccess = function (e) {
                var lc_CTHoaDon = null;
                if (req.result !== null && req.result.value !== null) {
                    lc_CTHoaDon = JSON.parse(req.result.value.Value);
                    store.delete(key_Add);
                }
                $.map(lc_CTHoaDon, function (item, i) {
                    if (item.ID_DonViQuiDoi === itemold.ID_DonViQuiDoi) {
                        if (lc_CTHoaDon[i].ID_LoHang !== null) {
                            var mahanghoa = data[0].MaHangHoa;
                            var giaNhap = data[0].GiaNhap;
                            var giaVon = data[0].GiaVon;
                            var tonKho = data[0].TonKho;
                            lc_CTHoaDon[i].ID_DonViQuiDoi = itemclick.ID_DonViQuiDoi;
                            lc_CTHoaDon[i].MaHangHoa = mahanghoa;
                            lc_CTHoaDon[i].GiaVon = lc_CTHoaDon[i].ID_LoHang !== null ? parseFloat(giaVon.toFixed()) : lc_CTHoaDon[i].GiaVon;
                            lc_CTHoaDon[i].SoLuong = lc_CTHoaDon[i].ID_LoHang !== null ? parseFloat(tonKho.toFixed(3)) : lc_CTHoaDon[i].SoLuong;
                            lc_CTHoaDon[i].TienChietKhau = lc_CTHoaDon[i].ThanhTien - lc_CTHoaDon[i].SoLuong;
                            lc_CTHoaDon[i].TenDonViTinh = itemclick.TenDonViTinh;
                            lc_CTHoaDon[i].TyLeChuyenDoi = itemclick.TyLeChuyenDoi;
                            lc_CTHoaDon[i].ThanhToan = parseFloat((lc_CTHoaDon[i].GiaVon * lc_CTHoaDon[i].TienChietKhau).toFixed());
                        }
                        else {
                            var mahh = data[0].MaHangHoa;
                            var giaNhap1 = data[0].GiaNhap;
                            var giaVon1 = data[0].GiaVon;
                            var tonKho1 = data[0].TonKho;
                            lc_CTHoaDon[i].ID_DonViQuiDoi = itemclick.ID_DonViQuiDoi;
                            lc_CTHoaDon[i].MaHangHoa = mahh;
                            lc_CTHoaDon[i].GiaVon = parseFloat(giaVon1.toFixed());
                            lc_CTHoaDon[i].SoLuong = parseFloat(tonKho1.toFixed(3));
                            lc_CTHoaDon[i].TienChietKhau = lc_CTHoaDon[i].ThanhTien - lc_CTHoaDon[i].SoLuong;
                            lc_CTHoaDon[i].TenDonViTinh = itemclick.TenDonViTinh;
                            lc_CTHoaDon[i].TyLeChuyenDoi = itemclick.TyLeChuyenDoi;
                            lc_CTHoaDon[i].ThanhToan = parseFloat((lc_CTHoaDon[i].GiaVon * lc_CTHoaDon[i].TienChietKhau).toFixed());
                        }
                    }
                });
                store.add({ Key: key_Add, Value: JSON.stringify(lc_CTHoaDon) });
                self.newKiemKho().BH_KiemKho_ChiTiet(lc_CTHoaDon);
                UpdateAgain_ListDVT();
            };
        });
    };

    function UpdateAgain_ListDVT() {
        var store = db.transaction(table, "readwrite").objectStore(table);
        var req = store.openCursor(key_Add);
        req.onsuccess = function (e) {
            var lc_CTHoaDon = null;
            var objChiTiet = [];
            if (req.result !== null && req.result.value !== null) {
                lc_CTHoaDon = JSON.parse(req.result.value.Value);
                store.delete(key_Add);
            }
            objChiTiet = lc_CTHoaDon;
            for (var z = 0; z < objChiTiet.length; z++) {
                var lstHangHoa_sameIDHangHoa = $.grep(objChiTiet, function (x) {
                    return x.ID === objChiTiet[z].ID;
                });
                var arrIDQuiDoi1 = [];
                for (var k = 0; k < lstHangHoa_sameIDHangHoa.length; k++) {
                    if ($.inArray(lstHangHoa_sameIDHangHoa[k].ID_DonViQuiDoi, arrIDQuiDoi1) === -1) {
                        arrIDQuiDoi1.push(lstHangHoa_sameIDHangHoa[k].ID_DonViQuiDoi);
                    }
                }
                arrIDQuiDoi1 = arrIDQuiDoi1.filter(p => p !== objChiTiet[z].ID_DonViQuiDoi);
                if (objChiTiet[z].DonViTinh.length > 0) {
                    for (var m = 0; m < objChiTiet[z].DonViTinh.length; m++) {
                        objChiTiet[z].DonViTinh[m].Xoa = true;
                        for (var n = 0; n < arrIDQuiDoi1.length; n++) {
                            if (objChiTiet[z].DonViTinh[m].ID_DonViQuiDoi === arrIDQuiDoi1[n]) {
                                objChiTiet[z].DonViTinh[m].Xoa = false;
                            }
                        }
                    }
                }
            };
            store.add({ Key: key_Add, Value: JSON.stringify(objChiTiet) });
            self.newKiemKho().BH_KiemKho_ChiTiet(objChiTiet);
        };
    }

    //lô hàng
    function newLot(itemCTHD, itemLot) {
        if (itemLot === null) {
            return objLot = {
                ID: itemCTHD.ID, // ID_DonViQuyDoi
                TenHangHoa: itemCTHD.TenHangHoa,
                ID_DonViQuiDoi: itemCTHD.ID_DonViQuiDoi,
                SoLuong: itemCTHD.TonKho, //TonKho
                TenDonViTinh: itemCTHD.TenDonViTinh,
                MaHangHoa: itemCTHD.MaHangHoa,
                ThanhTien: itemCTHD.ThanhTien, //SLThuc
                ThucTe: 0,
                TienChietKhau: itemCTHD.TienChietKhau, //SLLech
                ThanhToan: itemCTHD.ThanhToan, // GiaTriLech
                GiaVon: itemCTHD.GiaVon,
                HangHoa_ThuocTinh: itemCTHD.HangHoa_ThuocTinh,

                QuanLyTheoLoHang: true,
                LotParent: true,
                ID_LoHang: null,
                ID_Random: itemCTHD.ID_Random,
                NgaySanXuat: '',
                NgayHetHan: '',
                MaLoHang: ''
            };
        }
        else {
            var ngaysx = moment(itemLot.NgaySanXuat).format('DD/MM/YYYY');
            var hethan = moment(itemLot.NgayHetHan).format('DD/MM/YYYY');

            return objLot = {
                ID: itemCTHD.ID, // ID_DonViQuyDoi
                TenHangHoa: itemCTHD.TenHangHoa,
                ID_DonViQuiDoi: itemCTHD.ID_DonViQuiDoi,
                SoLuong: itemCTHD.TonKho, //TonKho
                TenDonViTinh: itemCTHD.TenDonViTinh,
                MaHangHoa: itemCTHD.MaHangHoa,
                ThanhTien: null, //SLThuc
                ThucTe: 0,
                TienChietKhau: null, //SLLech
                ThanhToan: null, // GiaTriLech
                GiaVon: itemCTHD.GiaVon,
                HangHoa_ThuocTinh: itemCTHD.HangHoa_ThuocTinh,

                QuanLyTheoLoHang: true,
                LotParent: true,
                ID_LoHang: itemLot.ID,
                ID_Random: itemCTHD.ID_Random,
                NgaySanXuat: ngaysx,
                NgayHetHan: hethan,
                MaLoHang: itemLot.MaLoHang
            };
        }
    }

    self.ListLoHang = ko.observableArray();
    self.ListLot_ofProduct = ko.observableArray();
    self.ChotSo_ChiNhanh = ko.observableArray();

    function getAllDMLoHang() {
        var timeChotSo = '2016-01-01';
        if (self.ChotSo_ChiNhanh().length > 0) {
            timeChotSo = self.ChotSo_ChiNhanh()[0].NgayChotSo;
        }

        ajaxHelper(DMHangHoaUri + "SP_GetAll_DMLoHang?iddonvi=" + _IDchinhanh + '&timeChotSo=' + timeChotSo, 'GET').done(function (data) {
            if (data !== null) {
                self.ListLoHang(data);
            }
        });
    }

    self.loadMaLoHang = function (item) {
        var _ngayKk = $('#datetimepicker').val();
        if (_ngayKk === "") {
            _ngayKk = moment(new Date()).format('DD/MM/YYYY HH:mm');
        }
        var check = CheckNgayLapHD_format(_ngayKk);
        if (!check) {
            return false;
        }
        var _ngayKiemKe = moment(_ngayKk, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');

        var $this = event.currentTarget;
        var ngaysx = item.NgaySanXuat !== null ? (item.NgaySanXuat) : "";
        var hethan = item.NgayHetHan !== null ? (item.NgayHetHan) : "";
        var idRandom = $($this.closest('.name-lot1')).find('span').eq(0).attr('id');
        // add cache CTHD
        var arrIDLotChosed = [];
        //Open indexDB key_add
        ajaxHelper(DMHangHoaUri + 'TinhTonTheoLoHang?idhanghoa=' + item.ID_HangHoa + '&id_lohang=' + item.ID + '&id_donvi=' + _IDchinhanh + '&ngaykk=' + _ngayKiemKe, 'GET').done(function (data) {
            var objectStore = db.transaction(table, "readwrite").objectStore(table);
            var req = objectStore.openCursor(key_Add);
            req.onsuccess = function (e) {
                if (req.result !== null && req.result.value !== null) {
                    var lc_CTHoaDon = JSON.parse(req.result.value.Value);
                    if (lc_CTHoaDon !== null) {
                        for (var i = 0; i < lc_CTHoaDon.length; i++) {
                            var itemCTHD = lc_CTHoaDon[i];
                            if (itemCTHD.ID_DonViQuiDoi === item.ID_DonViQuiDoi && itemCTHD.ID_Random === idRandom) {
                                // if change ID_LoHang --> update, not push
                                // if first DM_LoHang--> update ID_LoHang for CTHD if LotParent
                                lc_CTHoaDon[i].ID_LoHang = item.ID;
                                lc_CTHoaDon[i].SoLuong = parseFloat((data[0].TonKho / lc_CTHoaDon[i].TyLeChuyenDoi).toFixed(3));
                                lc_CTHoaDon[i].TienChietKhau = lc_CTHoaDon[i].ThanhTien - lc_CTHoaDon[i].SoLuong;
                                lc_CTHoaDon[i].ThanhToan = lc_CTHoaDon[i].TienChietKhau * lc_CTHoaDon[i].GiaVon;
                                lc_CTHoaDon[i].ID_LoHang = item.ID;
                                lc_CTHoaDon[i].MaLoHang = item.MaLoHang;
                                lc_CTHoaDon[i].NgaySanXuat = ngaysx;
                                lc_CTHoaDon[i].NgayHetHan = hethan;
                            }
                        }
                        var cursor = e.target.result;
                        var updateData = cursor.value;
                        updateData.Value = JSON.stringify(lc_CTHoaDon);
                        var request = cursor.update(updateData);
                        self.loadData();
                    }
                }
            }
        });
    }

    self.RestLoHangHoa = function (item) {
        ajaxHelper(DMHangHoaUri + 'TonCuaHangHoaNotByLoHang?idhanghoa=' + item.ID + '&id_donvi=' + _IDchinhanh, 'GET').done(function (soluonglo) {
            var objectStore = db.transaction(table, "readwrite").objectStore(table);
            var req = objectStore.openCursor(key_Add);
            req.onsuccess = function (e) {
                if (req.result !== null && req.result.value !== null) {
                    var lc_CTHoaDon = JSON.parse(req.result.value.Value);
                    if (lc_CTHoaDon !== null) {
                        for (var i = 0; i < lc_CTHoaDon.length; i++) {
                            if (item.ID_Random === lc_CTHoaDon[i].ID_Random) {
                                lc_CTHoaDon[i].ID_LoHang = null;
                                lc_CTHoaDon[i].SoLuong = parseFloat((soluonglo / lc_CTHoaDon[i].TyLeChuyenDoi).toFixed(3));
                                lc_CTHoaDon[i].TienChietKhau = lc_CTHoaDon[i].ThanhTien - soluonglo;
                                lc_CTHoaDon[i].ThanhToan = lc_CTHoaDon[i].TienChietKhau * lc_CTHoaDon[i].GiaVon;
                                lc_CTHoaDon[i].MaLoHang = "";
                                lc_CTHoaDon[i].NgaySanXuat = "";
                                lc_CTHoaDon[i].NgayHetHan = "";
                            }
                        }
                        var cursor = e.target.result;
                        var updateData = cursor.value;
                        updateData.Value = JSON.stringify(lc_CTHoaDon);
                        var request = cursor.update(updateData);
                        self.loadData();
                    }
                }
            }
        })
    }

    self.ArrMangLo = ko.observableArray();
    self.LoadListLoHang = function (item) {
        var iddvqd = item.ID_DonViQuiDoi;
        //ajaxHelper(DMHangHoaUri + "getListDM_LoHang?ID_DonViQuiDoi=" + iddvqd + '&ID_ChiNhanh=' + _IDchinhanh, "GET").done(function (data) {
        //    self.ListLoHang(data.LstData);
        var arrLot = $.grep(self.ListLoHang(), function (x) {
            return x.ID_DonViQuiDoi === iddvqd;
        });
        var arrIDLotChosed = [];
        var objectStore = db.transaction(table, "readwrite").objectStore(table);
        var req = objectStore.openCursor(key_Add);
        //Open indexDB key_add
        req.onsuccess = function (e) {
            if (req.result !== null && req.result.value !== null) {
                var lc_CTHoaDon = JSON.parse(req.result.value.Value);
                if (lc_CTHoaDon !== null) {

                    var arrCTHD = $.grep(lc_CTHoaDon, function (x) {
                        return x.ID_DonViQuiDoi === item.ID_DonViQuiDoi;
                    });

                    if (arrCTHD.length > 0) {
                        for (var i = 0; i < arrCTHD.length; i++) {
                            arrIDLotChosed.push(arrCTHD[i].ID_LoHang);
                        }
                    }
                }
                // get arr Lot chua chon
                arrLot = $.grep(arrLot, function (x) {
                    return $.inArray(x.ID, arrIDLotChosed) === -1;
                });

                self.ListLot_ofProduct(arrLot);
                self.ArrMangLo(arrLot);
            }
        }

        //})
    }

    self.SearchLoHangHoa = function (item) {
        var txtSearch = $('#AddNewLo' + item.ID_Random).val();
        var objCT = [];
        if (txtSearch !== "") {
            for (var i = 0; i < self.ArrMangLo().length; i++) {
                if (self.ArrMangLo()[i].MaLoHang.toLowerCase().includes(txtSearch) === true) {
                    objCT.push(self.ArrMangLo()[i]);
                }
            }
            self.ListLot_ofProduct(objCT);
        }
        else {
            self.ListLot_ofProduct(self.ArrMangLo());
        }
    }

    self.ChangeNullSearchLo = function (item) {
        var txtSearch = $('#AddNewLo' + item.ID_Random).val();
        if (txtSearch === "") {
            var objectStore = db.transaction(table, "readwrite").objectStore(table);
            var req = objectStore.openCursor(key_Add);
            //Open indexDB key_add
            req.onsuccess = function (e) {
                if (req.result !== null && req.result.value !== null) {
                    var lc_CTHoaDon = JSON.parse(req.result.value.Value);
                    if (lc_CTHoaDon !== null) {
                        for (var i = 0; i < lc_CTHoaDon.length; i++) {
                            if (item.ID_Random === lc_CTHoaDon[i].ID_Random) {
                                $('#AddNewLo' + item.ID_Random).val(lc_CTHoaDon[i].MaLoHang);
                            }
                        }
                    }
                }
            }
        }
    }

    self.selectIDDonViTinh = ko.observable();
    self.selectIDDonViTinh.subscribe(function (val) {
        if (val !== undefined) {
            ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetHangHoa_ByIDQuyDoi?id=" + val, 'GET').done(function (data) {
                $('#txtHangHoaauto').val(data.MaHangHoa);
                getChiTietHangHoaByID(itemNhap_Cham);
            })
        }
    })

    self.TinhLaiSLThuc = function () {
        if (self.newKiemKho().BH_KiemKho_ChiTiet() == "") {
            $('#lbltongslthucall').html('0');
        } else {
            var sum = 0;
            var store = db.transaction(table, "readwrite").objectStore(table);
            var req = store.openCursor(key_Add);
            //Open indexDB key_add
            req.onsuccess = function (event) {
                var cursor = event.target.result;
                if (cursor) {
                    var lc_CTKiemKho = cursor.value.Value;
                    //var lc_CTKiemKho = localStorage.getItem('lc_CTKiemKho');
                    if (lc_CTKiemKho !== null) {
                        lc_CTKiemKho = JSON.parse(lc_CTKiemKho);
                        for (var i = 0; i < lc_CTKiemKho.length; i++) {
                            if (lc_CTKiemKho[i].ThanhTien == null) {
                                lc_CTKiemKho[i].ThanhTien = 0;
                            }
                            sum += parseFloat(lc_CTKiemKho[i].ThanhTien);
                        }
                        if (isNaN(sum)) {
                            sum = 0;
                        }
                        $('#lbltongslthucall').html(formatNumber3Digit(parseFloat(sum.toFixed(3))));
                    }
                }
            };

        }
    }

    self.TinhLaiSLKhop = function () {
        var sum = 0;
        if (self.SLKhops().length > 0) {
            for (var i = 0; i < self.SLKhops().length; i++) {
                sum += parseFloat(self.SLKhops()[i].ThanhTien);
                $('#lbltongslthuckhop').html(formatNumber3Digit(parseFloat(sum)));
            }
        } else {
            $('#lbltongslthuckhop').html(sum);
        }
    }

    self.SoLuongLechTrungGian = ko.observableArray();
    self.TinhLaiLech = function () {
        self.SoLuongLechTrungGian([]);
        var tonglechtang = 0;
        var tonglechgiam = 0;
        var tonglech = 0;
        var tonggiatritang = 0;
        var tonggiatrigiam = 0;
        var tonggiatri = 0;
        var objectStore = db.transaction(table, "readwrite").objectStore(table);
        var req = objectStore.openCursor(key_Add);
        //Open indexDB key_add
        req.onsuccess = function (e) {
            if (req.result !== null && req.result.value != null) {
                data_Add = req.result.value;
                lc_CTKiemKho = JSON.parse(req.result.value.Value);
                self.newKiemKho().BH_KiemKho_ChiTiet(lc_CTKiemKho);
                for (var i = 0; i < self.newKiemKho().BH_KiemKho_ChiTiet().length; i++) {
                    if (self.newKiemKho().BH_KiemKho_ChiTiet()[i].ThanhTien !== null && parseFloat(self.newKiemKho().BH_KiemKho_ChiTiet()[i].ThanhTien) !== parseFloat(self.newKiemKho().BH_KiemKho_ChiTiet()[i].SoLuong)) {
                        self.SoLuongLechTrungGian.push(self.newKiemKho().BH_KiemKho_ChiTiet()[i]);
                    }
                }
                if (self.SoLuongLechTrungGian().length > 0) {
                    for (var i = 0; i < self.SoLuongLechTrungGian().length; i++) {
                        if (self.SoLuongLechTrungGian()[i].TienChietKhau > 0) {

                            tonglechtang = parseFloat(tonglechtang) + parseFloat(self.SoLuongLechTrungGian()[i].TienChietKhau);
                        }
                        if (self.SoLuongLechTrungGian()[i].TienChietKhau <= 0) {
                            tonglechgiam = parseFloat(tonglechgiam) + parseFloat(self.SoLuongLechTrungGian()[i].TienChietKhau);
                        }
                        if (self.SoLuongLechTrungGian()[i].ThanhToan > 0) {
                            tonggiatritang = parseFloat(tonggiatritang) + parseFloat(self.SoLuongLechTrungGian()[i].ThanhToan);
                        }
                        if (self.SoLuongLechTrungGian()[i].ThanhToan <= 0) {
                            tonggiatrigiam = parseFloat(tonggiatrigiam) + parseFloat(self.SoLuongLechTrungGian()[i].ThanhToan);
                        }
                    }
                    tonglech = parseFloat(tonglechtang + tonglechgiam);
                    tonggiatri = parseFloat(tonggiatritang + tonggiatrigiam);
                    $('#sltang').html(formatNumber3Digit(parseFloat(tonglechtang.toFixed(3))));
                    $('#slgiam').html(formatNumber3Digit(parseFloat(tonglechgiam.toFixed(3))));
                    $('#tongchenhlech').html(formatNumber3Digit(parseFloat(tonglech.toFixed(3))));
                    $('#giatritang').html(formatNumber3Digit(parseFloat(tonggiatritang.toFixed())));
                    $('#giatrigiam').html(formatNumber3Digit(parseFloat(tonggiatrigiam.toFixed())));
                    $('#tonggiatrilech').html(formatNumber3Digit(parseFloat(tonggiatri.toFixed())));
                }
                else {
                    $('#sltang').html(formatNumber3Digit(parseFloat(tonglechtang.toFixed(3))));
                    $('#slgiam').html(formatNumber3Digit(parseFloat(tonglechgiam.toFixed(3))));
                    $('#tongchenhlech').html(formatNumber3Digit(parseFloat(tonglech.toFixed(3))));
                    $('#giatritang').html(formatNumber3Digit(parseFloat(tonggiatritang.toFixed())));
                    $('#giatrigiam').html(formatNumber3Digit(parseFloat(tonggiatrigiam.toFixed())));
                    $('#tonggiatrilech').html(formatNumber3Digit(parseFloat(tonggiatri.toFixed())));
                }

            }
        }
    }

    self.deleteChiTietHD = function (item) {

        self.newKiemKho().BH_KiemKho_ChiTiet.remove(item);
        if (self.newKiemKho().BH_KiemKho_ChiTiet().length > 0) {
            $('#importKiemKho').hide();
        }
        else {
            $('#importKiemKho').show();
            self.deleteFileSelect();
        }

        //var lstCT = JSON.parse(localStorage.getItem('lc_CTKiemKho'));
        var store = db.transaction(table, "readwrite").objectStore(table);
        var req = store.openCursor(key_Add);

        //Open indexDB key_add
        req.onsuccess = function (event) {
            var cursor = event.target.result;
            if (cursor) {
                var updateData = cursor.value;
                var lstCT = JSON.parse(updateData.Value);
                lstCT = $.grep(lstCT, function (x) {
                    return x.ID_DonViQuiDoi !== item.ID_DonViQuiDoi;
                })
                updateData.Value = JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet());
                var request = cursor.update(updateData);
                request.onsuccess = function () {
                };
            }
            for (var i = 0; i < self.SLKhops().length; i++) {
                if (self.SLKhops()[i].ID == item.ID) {
                    self.SLKhops.remove(self.SLKhops()[i]);
                }
            }
            for (var i = 0; i < self.SLLechs().length; i++) {
                if (self.SLLechs()[i].ID_DonViQuiDoi == item.ID_DonViQuiDoi) {
                    self.SLLechs.remove(self.SLLechs()[i]);
                }
            }
            for (var i = 0; i < self.SLChuaKiems().length; i++) {
                if (self.SLChuaKiems()[i].ID_DonViQuiDoi == item.ID_DonViQuiDoi) {
                    self.SLChuaKiems.remove(self.SLChuaKiems()[i]);
                }
            }
            self.TinhLaiSLThuc();
            self.TinhLaiSLKhop();
            self.TinhLaiLech();
            UpdateAgain_ListDVT();
            $('#tongitemkhop').text(self.SLKhops().length);
            $('#tongitemlech').text(self.SLLechs().length);
            $('#tongitem').text((self.SLLechs().length) + (self.SLKhops().length) + self.SLChuaKiems().length);
            $('#tongitemchuakiem').text((self.SLChuaKiems().length));
        };
    }

    self.deleteCTKhop = function (item) {
        var tongslthuckhop = 0;
        self.SLKhops.remove(item);
        for (var i = 0; i < self.newKiemKho().BH_KiemKho_ChiTiet().length; i++) {
            if (self.newKiemKho().BH_KiemKho_ChiTiet()[i].ID_DonViQuiDoi == item.ID_DonViQuiDoi) {
                self.newKiemKho().BH_KiemKho_ChiTiet.remove(self.newKiemKho().BH_KiemKho_ChiTiet()[i]);
            }
        }
        if (self.newKiemKho().BH_KiemKho_ChiTiet().length > 0) {
            $('#importKiemKho').hide();
        }
        else {
            $('#importKiemKho').show();
            self.deleteFileSelect();
        }
        var store = db.transaction(table, "readwrite").objectStore(table);
        var req = store.openCursor(key_Add);
        //Open indexDB key_add
        req.onsuccess = function (event) {
            var cursor = event.target.result;
            if (cursor) {
                var updateData = cursor.value;
                updateData.Value = JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet());
                var request = cursor.update(updateData);
                request.onsuccess = function () {
                };
            }
            else {
                store.add({ Key: key_Add, Value: JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet()) })
            }
            self.TinhLaiSLKhop();
            self.TinhLaiSLThuc();
            $('#tongitemkhop').text(self.SLKhops().length);
            $('#tongitemlech').text(self.SLLechs().length);
            $('#tongitem').text((self.SLLechs().length) + (self.SLKhops().length) + self.SLChuaKiems().length);
            $('#tongitemchuakiem').text((self.SLChuaKiems().length));
        };
        //localStorage.setItem('lc_CTKiemKho', JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet()));


    }

    self.deleteCTLech = function (item) {
        self.SLLechs.remove(item);
        for (var i = 0; i < self.newKiemKho().BH_KiemKho_ChiTiet().length; i++) {
            if (self.newKiemKho().BH_KiemKho_ChiTiet()[i].ID_DonViQuiDoi == item.ID_DonViQuiDoi) {
                self.newKiemKho().BH_KiemKho_ChiTiet.remove(self.newKiemKho().BH_KiemKho_ChiTiet()[i]);
            }
        }
        if (self.newKiemKho().BH_KiemKho_ChiTiet().length > 0) {
            $('#importKiemKho').hide();
        }
        else {
            $('#importKiemKho').show();
            self.deleteFileSelect();
        }
        var store = db.transaction(table, "readwrite").objectStore(table);
        var req = store.openCursor(key_Add);
        //Open indexDB key_add
        req.onsuccess = function (event) {
            var cursor = event.target.result;
            if (cursor) {
                var updateData = cursor.value;
                updateData.Value = JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet());
                var request = cursor.update(updateData);
                request.onsuccess = function () {
                };
            }
            else {
                store.add({ Key: key_Add, Value: JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet()) })
            }
            self.TinhLaiSLThuc();
            self.TinhLaiLech();
            $('#tongitemkhop').text(self.SLKhops().length);
            $('#tongitemlech').text(self.SLLechs().length);
            $('#tongitem').text((self.SLLechs().length) + (self.SLKhops().length) + self.SLChuaKiems().length);
            $('#tongitemchuakiem').text((self.SLChuaKiems().length));
        };
    }

    self.deleteCTChuaKiem = function (item) {
        self.SLChuaKiems.remove(item);
        for (var i = 0; i < self.newKiemKho().BH_KiemKho_ChiTiet().length; i++) {
            if (self.newKiemKho().BH_KiemKho_ChiTiet()[i].ID_DonViQuiDoi == item.ID_DonViQuiDoi) {
                self.newKiemKho().BH_KiemKho_ChiTiet.remove(self.newKiemKho().BH_KiemKho_ChiTiet()[i]);
            }
        }
        if (self.newKiemKho().BH_KiemKho_ChiTiet().length > 0) {
            $('#importKiemKho').hide();
        }
        else {
            $('#importKiemKho').show();
            self.deleteFileSelect();
        }
        var store = db.transaction(table, "readwrite").objectStore(table);
        var req = store.openCursor(key_Add);
        //Open indexDB key_add
        req.onsuccess = function (event) {
            var cursor = event.target.result;
            if (cursor) {
                var updateData = cursor.value;
                updateData.Value = JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet());
                var request = cursor.update(updateData);
                request.onsuccess = function () {
                };
            }
            else {
                store.add({ Key: key_Add, Value: JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet()) })
            }
            self.TinhLaiSLThuc();
            self.TinhLaiLech();
            $('#tongitemkhop').text(self.SLKhops().length);
            $('#tongitemlech').text(self.SLLechs().length);
            $('#tongitem').text((self.SLLechs().length) + (self.SLKhops().length) + self.SLChuaKiems().length);
            $('#tongitemchuakiem').text((self.SLChuaKiems().length));
        };


    }

    shortcut.add("F6", function () {
        $(".number-fast").toggle();
        if ($('.number-fast').css('display') === 'block') {
            ShowMessage_Success("Chế độ nhập thường!");
            $('#txtHangHoaauto').focus().select();
        } else {
            ShowMessage_Success("Chế độ nhập nhanh!");
            $('#txtHangHoaauto').focus().select();
        }
        $('#txtSoLuongHang').val(1);
    });

    shortcut.add("F3", function () {
        $('#txtHangHoaauto').focus();
    });

    //shortcut.add("F9", function () {
    //    self.addKiemKho('StockTakes');
    //});

    shortcut.add("F10", function (e) {
        if ($('.bgwhite').css('display') === "none") {
            if (loaiHoaDon === 4) {
                if (window.location.hash !== "#/PurchaseOrder") {
                    $('.bgwhite').show();
                    modelGiaoDich.AddHoaDonWhereLoHang();
                }
            }
            if (loaiHoaDon === 7) {
                if (window.location.hash !== "#/PurchaseReturns") {
                    $('.bgwhite').show();
                    modelGiaoDich.AddHoaDonTHWhereLoHang();
                }
            }
            if (loaiHoaDon === "10") {
                if (window.location.hash !== "#/Transfers") {
                    $('.bgwhite').show();
                    modelChuyenHang.AddHoaDonWhereLoHang();
                }
            }
            if (loaiHoaDon === "9") {
                if (window.location.hash !== "#/StockTakes") {
                    $('.bgwhite').show();
                    modelHangHoa.AddKiemKhoWhereLoHang('StockTakes');
                }
            }
            if (loaiHoaDon === 8) {
                if (window.location.hash !== "#/DamageItems") {
                    $('.bgwhite').show();
                    vmXuatHuy.addHoaDon_HoanThanh();
                }
            }
        }
        e.stopImmediatePropagation();
    });
    //$(document).keyup(function (e) {
    //    var code = (e.keyCode ? e.keyCode : e.which);
    //    if (code === 121) {
    //        if ($('.bgwhite').css('display') === "none") {
    //            if (loaiHoaDon === 4) {
    //                if (window.location.hash !== "#/PurchaseOrder") {
    //                    $('.bgwhite').show();
    //                    modelGiaoDich.addHoaDon();
    //                }
    //            }
    //            if (loaiHoaDon === 7) {
    //                if (window.location.hash !== "#/PurchaseReturns") {
    //                    $('.bgwhite').show();
    //                    modelGiaoDich.addHoaDonTH();
    //                }
    //            }
    //            if (loaiHoaDon === "10") {
    //                if (window.location.hash !== "#/Transfers") {
    //                    $('.bgwhite').show();
    //                    modelChuyenHang.addHoaDon();
    //                }
    //            }
    //            if (loaiHoaDon === "9") {
    //                if (window.location.hash !== "#/StockTakes") {
    //                    $('.bgwhite').show();
    //                    self.addKiemKho('StockTakes');
    //                }
    //            }
    //            if (loaiHoaDon === 8) {
    //                if (window.location.hash !== "#/DamageItems") {
    //                    $('.bgwhite').show();
    //                    vmXuatHuy.addHoaDon_HoanThanh();
    //                }
    //            }
    //        }
    //    }
    //    e.stopImmediatePropagation();
    //})
    //shortcut.add("F10", function () {
    //    self.addKiemKhoTL('StockTakes');
    //});

    self.SLKhops = ko.observableArray();
    self.SLLechs = ko.observableArray();
    self.SLChuaKiems = ko.observableArray();

    self.xulySLChuaKiem = function (item) {
        var slchuakiemedit = parseFloat($('#slthucchuakiem_' + item.ID_Random + item.ID_DonViQuiDoi).val());
        if (isNaN(slchuakiemedit)) {
            slchuakiemedit = "";
        }
        if (self.SLChuaKiems().length > 0) {
            for (var i = 0; i < self.SLChuaKiems().length; i++) {
                if (self.SLChuaKiems()[i].ID_DonViQuiDoi === item.ID_DonViQuiDoi) {
                    for (var k = 0; k < self.newKiemKho().BH_KiemKho_ChiTiet().length; k++) {
                        if (self.newKiemKho().BH_KiemKho_ChiTiet()[k].ID_DonViQuiDoi === item.ID_DonViQuiDoi) {
                            self.newKiemKho().BH_KiemKho_ChiTiet.remove(self.newKiemKho().BH_KiemKho_ChiTiet()[k]);
                            if (slchuakiemedit === "") {
                                self.SLChuaKiems()[i].ThanhTien = null;
                                self.SLChuaKiems()[i].TienChietKhau = null;
                                self.SLChuaKiems()[i].ThanhToan = null;
                            } else {
                                self.SLChuaKiems()[i].ThanhTien = slchuakiemedit;
                                self.SLChuaKiems()[i].TienChietKhau = slchuakiemedit - self.SLChuaKiems()[i].SoLuong;
                                self.SLChuaKiems()[i].ThanhToan = self.SLChuaKiems()[i].GiaVon * (slchuakiemedit - self.SLChuaKiems()[i].SoLuong);
                            }
                            self.KiemGanDays.unshift(self.SLChuaKiems()[i]);
                            self.newKiemKho().BH_KiemKho_ChiTiet.splice(k, 0, self.SLChuaKiems()[i]);

                            var store = db.transaction(table, "readwrite").objectStore(table);
                            var req = store.openCursor(key_Add);
                            req.onsuccess = function (event) {
                                var cursor = event.target.result;
                                if (cursor) {
                                    var updateData = cursor.value;
                                    updateData.Value = JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet());
                                    var request = cursor.update(updateData);
                                    request.onsuccess = function () {
                                    };
                                }
                                else {
                                    store.add({ Key: key_Add, Value: JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet()) })
                                }
                            };
                            //localStorage.setItem('lc_CTKiemKho', JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet()));
                        }
                    }
                    if (slchuakiemedit !== item.SoLuong && slchuakiemedit !== "") {
                        self.SLChuaKiems()[i].TienChietKhau = slchuakiemedit - self.SLChuaKiems()[i].SoLuong;;
                        self.SLChuaKiems()[i].ThanhToan = self.SLChuaKiems()[i].GiaVon * (slchuakiemedit - self.SLChuaKiems()[i].SoLuong);
                        self.KiemGanDays.unshift(self.SLChuaKiems()[i]);
                        self.SLLechs.push(self.SLChuaKiems()[i]);
                        for (var j = 0; j < self.SLChuaKiems().length; j++) {
                            if (self.SLChuaKiems()[j].ID_DonViQuiDoi === item.ID_DonViQuiDoi) {
                                self.SLChuaKiems.remove((self.SLChuaKiems()[j]));
                            }
                        }
                    }
                    if (slchuakiemedit === item.SoLuong) {
                        self.SLChuaKiems()[i].TienChietKhau = 0;
                        self.SLChuaKiems()[i].ThanhToan = 0;
                        self.SLKhops.push(self.SLChuaKiems()[i]);
                        for (var j = 0; j < self.SLChuaKiems().length; j++) {
                            if (self.SLChuaKiems()[j].ID_DonViQuiDoi === item.ID_DonViQuiDoi) {
                                self.SLChuaKiems.remove((self.SLChuaKiems()[j]));
                            }
                        }
                    }
                }
            }
        }
        $('#tongitemkhop').text(self.SLKhops().length);
        $('#tongitemlech').text(self.SLLechs().length);
        $('#tongitem').text((self.SLLechs().length) + (self.SLKhops().length) + self.SLChuaKiems().length);
        $('#tongitemchuakiem').text((self.SLChuaKiems().length));
        self.TinhLaiLech();
        self.TinhLaiSLThuc();
        self.TinhLaiSLKhop();
    }

    self.xulySLKhop = function (item) {
        var slkhopedit = parseFloat(formatNumberToFloatKK($('#slthuckhop_' + item.ID_Random + item.ID_DonViQuiDoi).val()));
        if (isNaN(slkhopedit)) {
            slkhopedit = "";
        }
        if (self.SLKhops().length > 0) {
            for (var i = 0; i < self.SLKhops().length; i++) {
                if (self.SLKhops()[i].ID_DonViQuiDoi == item.ID_DonViQuiDoi) {
                    for (var k = 0; k < self.newKiemKho().BH_KiemKho_ChiTiet().length; k++) {
                        if (self.newKiemKho().BH_KiemKho_ChiTiet()[k].ID_DonViQuiDoi == item.ID_DonViQuiDoi) {
                            self.newKiemKho().BH_KiemKho_ChiTiet.remove(self.newKiemKho().BH_KiemKho_ChiTiet()[k]);
                            if (slkhopedit == "") {
                                self.SLKhops()[i].ThanhTien = null
                                self.SLKhops()[i].TienChietKhau = null;
                                self.SLKhops()[i].ThanhToan = null;
                            } else {
                                self.SLKhops()[i].ThanhTien = slkhopedit;
                                self.SLKhops()[i].TienChietKhau = slkhopedit - self.SLKhops()[i].SoLuong;
                                self.SLKhops()[i].ThanhToan = self.SLKhops()[i].GiaVon * (slkhopedit - self.SLKhops()[i].SoLuong);
                            }
                            self.KiemGanDays.unshift(self.SLKhops()[i]);
                            self.newKiemKho().BH_KiemKho_ChiTiet.splice(k, 0, self.SLKhops()[i]);

                            var store = db.transaction(table, "readwrite").objectStore(table);
                            var req = store.openCursor(key_Add);
                            req.onsuccess = function (event) {
                                var cursor = event.target.result;
                                if (cursor) {
                                    var updateData = cursor.value;
                                    updateData.Value = JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet());
                                    var request = cursor.update(updateData);
                                    request.onsuccess = function () {
                                    };
                                }
                                else {
                                    store.add({ Key: key_Add, Value: JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet()) })
                                }
                            };
                            //localStorage.setItem('lc_CTKiemKho', JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet()));
                        }
                    }
                    if (slkhopedit !== item.SoLuong && slkhopedit !== "") {
                        self.SLKhops()[i].TienChietKhau = slkhopedit - self.SLKhops()[i].SoLuong;;
                        self.SLKhops()[i].ThanhToan = self.SLKhops()[i].GiaVon * (slkhopedit - self.SLKhops()[i].SoLuong);
                        self.KiemGanDays.unshift(self.SLKhops()[i]);
                        self.SLLechs.push(self.SLKhops()[i]);
                        for (var j = 0; j < self.SLKhops().length; j++) {
                            if (self.SLKhops()[j].ID_DonViQuiDoi == item.ID_DonViQuiDoi) {
                                self.SLKhops.remove((self.SLKhops()[j]));
                            }
                        }
                    } else if (slkhopedit == "") {
                        self.SLKhops()[i].TienChietKhau = null;
                        self.SLKhops()[i].ThanhToan = null;
                        self.SLChuaKiems.push(self.SLKhops()[i]);
                        for (var j = 0; j < self.SLKhops().length; j++) {
                            if (self.SLKhops()[j].ID_DonViQuiDoi == item.ID_DonViQuiDoi) {
                                self.SLKhops.remove((self.SLKhops()[j]));
                            }
                        }
                    }
                }
            }
        }
        $('#tongitemkhop').text(self.SLKhops().length);
        $('#tongitemlech').text(self.SLLechs().length);
        $('#tongitem').text((self.SLLechs().length) + (self.SLKhops().length) + self.SLChuaKiems().length);
        $('#tongitemchuakiem').text((self.SLChuaKiems().length));
        self.TinhLaiLech();
        self.TinhLaiSLThuc();
        self.TinhLaiSLKhop();
    }

    self.xulySLLech = function (item) {
        var sllechedit = parseFloat(formatNumberToFloatKK($('#slthuclech_' + item.ID_Random + item.ID_DonViQuiDoi).val()));
        if (isNaN(sllechedit)) {
            sllechedit = "";
        }
        if (self.SLLechs().length > 0) {
            for (var i = 0; i < self.SLLechs().length; i++) {
                if (self.SLLechs()[i].ID_DonViQuiDoi == item.ID_DonViQuiDoi) {
                    for (var k = 0; k < self.newKiemKho().BH_KiemKho_ChiTiet().length; k++) {
                        if (self.newKiemKho().BH_KiemKho_ChiTiet()[k].ID_DonViQuiDoi == item.ID_DonViQuiDoi) {
                            self.newKiemKho().BH_KiemKho_ChiTiet.remove(self.newKiemKho().BH_KiemKho_ChiTiet()[k]);
                            if (sllechedit === "") {
                                self.SLLechs()[i].ThanhTien = null;
                                self.SLLechs()[i].TienChietKhau = null;
                                self.SLLechs()[i].ThanhToan = null;
                            } else {
                                self.SLLechs()[i].ThanhTien = sllechedit;
                                self.SLLechs()[i].TienChietKhau = parseFloat((sllechedit - self.SLLechs()[i].SoLuong).toFixed(3));
                                self.SLLechs()[i].ThanhToan = (self.SLLechs()[i].GiaVon * parseFloat((sllechedit - self.SLLechs()[i].SoLuong).toFixed(3))).toFixed();
                            }
                            self.KiemGanDays.unshift(self.SLLechs()[i]);
                            self.newKiemKho().BH_KiemKho_ChiTiet.splice(k, 0, self.SLLechs()[i]);
                            var store = db.transaction(table, "readwrite").objectStore(table);
                            var req = store.openCursor(key_Add);
                            req.onsuccess = function (event) {
                                var cursor = event.target.result;
                                if (cursor) {
                                    var updateData = cursor.value;
                                    updateData.Value = JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet());
                                    var request = cursor.update(updateData);
                                    request.onsuccess = function () {
                                    };
                                }
                                else {
                                    store.add({ Key: key_Add, Value: JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet()) })
                                }
                            };
                            //localStorage.setItem('lc_CTKiemKho', JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet()));
                        }
                    }
                    if (sllechedit === item.SoLuong) {
                        self.SLLechs()[i].TienChietKhau = 0;
                        self.SLLechs()[i].ThanhToan = 0;
                        self.KiemGanDays.unshift(self.SLLechs()[i]);
                        self.SLKhops.push(self.SLLechs()[i]);
                        for (var j = 0; j < self.SLLechs().length; j++) {
                            if (self.SLLechs()[j].ID_DonViQuiDoi === item.ID_DonViQuiDoi) {
                                self.SLLechs.remove((self.SLLechs()[j]));
                            }
                        }
                    } else if (sllechedit === "") {
                        self.SLLechs()[i].TienChietKhau = null;
                        self.SLLechs()[i].ThanhToan = null;
                        self.SLChuaKiems.push(self.SLLechs()[i]);
                        for (var j = 0; j < self.SLLechs().length; j++) {
                            if (self.SLLechs()[j].ID_DonViQuiDoi === item.ID_DonViQuiDoi) {
                                self.SLLechs.remove((self.SLLechs()[j]));
                            }
                        }
                        for (var j = 0; j < self.SLKhops().length; j++) {
                            if (self.SLKhops()[j].ID_DonViQuiDoi === item.ID_DonViQuiDoi) {
                                self.SLKhops.remove((self.SLKhops()[j]));
                            }
                        }
                    }
                }
            }
        }
        $('#tongitemkhop').text(self.SLKhops().length);
        $('#tongitemlech').text(self.SLLechs().length);
        $('#tongitem').text((self.SLLechs().length) + (self.SLKhops().length) + self.SLChuaKiems().length);
        $('#tongitemchuakiem').text((self.SLChuaKiems().length));
        self.TinhLaiLech();
        self.TinhLaiSLThuc();
        self.TinhLaiSLKhop();
    }

    self.xulySLKho = function (item) {
        var store = db.transaction(table, "readonly").objectStore(table);
        var req = store.openCursor(key_Add);
        req.onsuccess = function (event) {
            var cursor = event.target.result;
            var lc_CTKiemKho;
            if (cursor) {
                lc_CTKiemKho = JSON.parse(cursor.value.Value);
            }
            if (lc_CTKiemKho != null) {
                self.newKiemKho().BH_KiemKho_ChiTiet(lc_CTKiemKho);
                for (var i = 0; i < lc_CTKiemKho.length; i++) {

                    if (lc_CTKiemKho[i].ID_DonViQuiDoi === item.ID_DonViQuiDoi && lc_CTKiemKho[i].ID_Random === item.ID_Random) {
                        self.KiemGanDays.unshift(lc_CTKiemKho[i]);
                        if (lc_CTKiemKho[i].ThanhTien === item.SoLuong) {
                            self.SLKhops.push(lc_CTKiemKho[i]);
                            for (var j = 0; j < self.SLLechs().length; j++) {
                                if (self.SLLechs()[j].ID_DonViQuiDoi === lc_CTKiemKho[i].ID_DonViQuiDoi && self.SLLechs()[j].ID_Random === lc_CTKiemKho[i].ID_Random) {
                                    self.SLLechs.remove((self.SLLechs()[j]));
                                }
                            }
                            for (var j = 0; j < self.SLChuaKiems().length; j++) {
                                if (self.SLChuaKiems()[j].ID_DonViQuiDoi === lc_CTKiemKho[i].ID_DonViQuiDoi && self.SLChuaKiems()[j].ID_Random === lc_CTKiemKho[i].ID_Random) {
                                    self.SLChuaKiems.remove((self.SLChuaKiems()[j]));
                                }
                            }
                        }
                        else if (lc_CTKiemKho[i].ThanhTien == null) {
                            lc_CTKiemKho[i].TienChietKhau = null;
                            lc_CTKiemKho[i].ThanhToan = null;
                            self.SLChuaKiems.push(lc_CTKiemKho[i]);
                            for (var j = 0; j < self.SLLechs().length; j++) {
                                if (self.SLLechs()[j].ID_DonViQuiDoi === lc_CTKiemKho[i].ID_DonViQuiDoi && self.SLLechs()[j].ID_Random === lc_CTKiemKho[i].ID_Random) {
                                    self.SLLechs.remove((self.SLLechs()[j]));
                                }
                            }
                            for (var j = 0; j < self.SLKhops().length; j++) {
                                if (self.SLKhops()[j].ID_DonViQuiDoi === lc_CTKiemKho[i].ID_DonViQuiDoi && self.SLKhops()[j].ID_Random === lc_CTKiemKho[i].ID_Random) {
                                    self.SLKhops.remove((self.SLKhops()[j]));
                                }
                            }
                        }
                        else {
                            if (self.SLLechs().length === 0) {
                                self.SLLechs.push(lc_CTKiemKho[i]);
                            } else {
                                for (var k = 0; k < self.SLLechs().length; k++) {
                                    if (self.SLLechs()[k].ID_DonViQuiDoi === lc_CTKiemKho[i].ID_DonViQuiDoi && self.SLLechs()[k].ID_Random === lc_CTKiemKho[i].ID_Random) {
                                        self.SLLechs.remove(self.SLLechs()[k]);
                                        self.SLLechs.push(lc_CTKiemKho[i]);
                                    } else {
                                        self.SLLechs.push(lc_CTKiemKho[i]);
                                    }
                                }
                            }
                            for (var j = 0; j < self.SLKhops().length; j++) {
                                if (self.SLKhops()[j].ID_DonViQuiDoi === lc_CTKiemKho[i].ID_DonViQuiDoi && self.SLKhops()[j].ID_Random === lc_CTKiemKho[i].ID_Random) {
                                    self.SLKhops.remove((self.SLKhops()[j]));
                                }
                            }
                            for (var j = 0; j < self.SLChuaKiems().length; j++) {
                                if (self.SLChuaKiems()[j].ID_DonViQuiDoi === lc_CTKiemKho[i].ID_DonViQuiDoi && self.SLChuaKiems()[j].ID_Random === lc_CTKiemKho[i].ID_Random) {
                                    self.SLChuaKiems.remove((self.SLChuaKiems()[j]));
                                }
                            }
                        }
                    }
                }
            }
            $('#tongitemkhop').text(self.SLKhops().length);
            $('#tongitemlech').text(self.SLLechs().length);
            $('#tongitem').text((self.SLLechs().length) + (self.SLKhops().length) + self.SLChuaKiems().length);
            $('#tongitemchuakiem').text((self.SLChuaKiems().length));
            var sum = 0;
            var tonglechtang = 0;
            var tonglechgiam = 0;
            var tonglech = 0;
            var tonggiatritang = 0;
            var tonggiatrigiam = 0;
            var tonggiatri = 0;
            if (self.SLKhops().length > 0) {
                for (var i = 0; i < self.SLKhops().length; i++) {
                    sum += self.SLKhops()[i].ThanhTien;
                    $('#lbltongslthuckhop').html(sum);
                }
            } else {
                $('#lbltongslthuckhop').html(sum);
            }
            $(function () {
                if (self.SLLechs().length > 0) {
                    for (var i = 0; i < self.SLLechs().length; i++) {
                        if (self.SLLechs()[i].TienChietKhau > 0) {
                            tonglechtang = parseFloat(tonglechtang) + parseFloat(self.SLLechs()[i].TienChietKhau);
                        } else {
                            tonglechgiam = parseFloat(tonglechgiam) + parseFloat(self.SLLechs()[i].TienChietKhau);
                        }
                        if (self.SLLechs()[i].ThanhToan > 0) {
                            tonggiatritang = parseFloat(tonggiatritang) + parseFloat(self.SLLechs()[i].ThanhToan);
                        } else {
                            tonggiatrigiam = parseFloat(tonggiatrigiam) + parseFloat(self.SLLechs()[i].ThanhToan);
                        }
                    }
                    tonglech = parseFloat(tonglechtang + tonglechgiam);
                    tonglech = parseFloat(tonglechtang + tonglechgiam);
                    tonggiatri = parseFloat(tonggiatritang + tonggiatrigiam);
                    $('#sltang').html(formatNumber3Digit(parseFloat(tonglechtang.toFixed(3))));
                    $('#slgiam').html(formatNumber3Digit(parseFloat(tonglechgiam.toFixed(3))));
                    $('#tongchenhlech').html(formatNumber3Digit(parseFloat(tonglech.toFixed(3))));
                    $('#giatritang').html(formatNumber3Digit(parseFloat(tonggiatritang.toFixed())));
                    $('#giatrigiam').html(formatNumber3Digit(parseFloat(tonggiatrigiam.toFixed())));
                    $('#tonggiatrilech').html(formatNumber3Digit(parseFloat(tonggiatri.toFixed())));
                }
                else {
                    $('#sltang').html(formatNumber3Digit(parseFloat(tonglechtang.toFixed(3))));
                    $('#slgiam').html(formatNumber3Digit(parseFloat(tonglechgiam.toFixed(3))));
                    $('#tongchenhlech').html(formatNumber3Digit(parseFloat(tonglech.toFixed(3))));
                    $('#giatritang').html(formatNumber3Digit(parseFloat(tonggiatritang.toFixed())));
                    $('#giatrigiam').html(formatNumber3Digit(parseFloat(tonggiatrigiam.toFixed())));
                    $('#tonggiatrilech').html(formatNumber3Digit(parseFloat(tonggiatri.toFixed())));
                }
            })

        };


    }

    self.NgayHDXoa = ko.observable();
    self.ChoThanhToanKhiXoa = ko.observable();
    self.modalDeleteKK = function (item) {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('KiemKho_Xoa', lc_CTQuyen) > -1) {
            self.NgayHDXoa(item.NgayLapHoaDon);
            self.deleteMaHoaDon(item.MaHoaDon);
            self.deleteID(item.ID);
            self.ChoThanhToanKhiXoa(item.ChoThanhToan);
            $('#modalpopup_deleteKK').modal('show');
        }
    };
    self.xoaKK = function (KiemKhos) {
        $.ajax({
            type: "POST",
            url: "/api/DanhMuc/BH_HoaDonAPI/" + "UPdateKH_ChoThanhToan?id=" + KiemKhos.deleteID() + '&idnhanvien=' + _IDNhanVien + '&iddonvi=' + _IDchinhanh,
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                SearchKiemKho();
                $('#modalpopup_deleteKK').modal('hide');
                ShowMessage_Success("Cập nhật trạng thái thành công!");

            },
            error: function (error) {
                SearchKiemKho();
                $('#modalpopup_deleteKK').modal('hide');
                ShowMessage_Danger("Cập nhật trạng thái thất bại.");
            }
        })
    };

    self.capnhatKK = function (item) {
        var hdct = self.BH_HoaDonChiTietsThaoTac();
        for (var i = 0; i < hdct.length; i++) {
            hdct[i].ID_HoaDon = item.ID;
            hdct[i].SoLuong = hdct[i].TonKho;
            hdct[i].TienChietKhau = hdct[i].ThanhTien - hdct[i].TonKho;
            hdct[i].MaHoaDon = item.MaHoaDon;
            hdct[i].DienGiai = item.DienGiai;
            hdct[i].QuanLyTheoLoHang = hdct[i].QuanLyTheoLoHang;
            hdct[i].ThuocTinh_GiaTri = hdct[i].ThuocTinh_GiaTri;
            var idLoHang = hdct[i].ID_LoHang;
            var itemLot = [];
            if (idLoHang !== null) {
                itemLot = $.grep(self.ListLoHang(), function (x) {
                    return x.ID === idLoHang;
                });
            }
            var rd = Math.floor(Math.random() * 1000000 + 1);
            hdct[i].ID_Random = 'IDRandom' + rd + '_';
            hdct[i].ID_LoHang = idLoHang;
            hdct[i].MaLoHang = hdct[i].QuanLyTheoLoHang ? (itemLot.length > 0 ? itemLot[0].MaLoHang : null) : null;
            hdct[i].NgaySanXuat = hdct[i].QuanLyTheoLoHang ? (itemLot.length > 0 ? itemLot[0].NgaySanXuat : '') : '';
            hdct[i].NgayHetHan = hdct[i].QuanLyTheoLoHang ? (itemLot.length > 0 ? itemLot[0].NgayHetHan : '') : '';
        }
        localStorage.setItem('lc_UpdateKK', JSON.stringify(hdct));
        clickloadForm('StockTakes/inventory');
        localStorage.setItem('isUpdate', true);
        localStorage.setItem('isSaoChepKK', false);
    }

    self.SaoChepKK = function (item) {
        var hdct = self.BH_HoaDonChiTietsThaoTac();
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('KiemKho_SaoChep', lc_CTQuyen) > -1) {
            for (var i = 0; i < hdct.length; i++) {
                hdct[i].ID_HoaDon = item.ID;
                hdct[i].SoLuong = hdct[i].TonKho / hdct[i].TyLeChuyenDoi;
                hdct[i].TienChietKhau = hdct[i].ThanhTien - (hdct[i].TonKho / hdct[i].TyLeChuyenDoi);
                hdct[i].ThanhToan = parseFloat(hdct[i].TienChietKhau * hdct[i].GiaVon).toFixed();
                hdct[i].MaHoaDon = "Copy" + item.MaHoaDon;
                hdct[i].DienGiai = item.DienGiai;

                hdct[i].QuanLyTheoLoHang = hdct[i].QuanLyTheoLoHang;
                hdct[i].ThuocTinh_GiaTri = hdct[i].ThuocTinh_GiaTri;
                var idLoHang = hdct[i].ID_LoHang;
                var itemLot = [];
                if (idLoHang !== null) {
                    itemLot = $.grep(self.ListLoHang(), function (x) {
                        return x.ID === idLoHang;
                    });
                }
                var rd = Math.floor(Math.random() * 1000000 + 1);
                hdct[i].ID_Random = 'IDRandom' + rd + '_';
                hdct[i].ID_LoHang = idLoHang;
                hdct[i].MaLoHang = hdct[i].QuanLyTheoLoHang ? (itemLot.length > 0 ? itemLot[0].MaLoHang : null) : null;
                hdct[i].NgaySanXuat = hdct[i].QuanLyTheoLoHang ? (itemLot.length > 0 ? itemLot[0].NgaySanXuat : '') : '';
                hdct[i].NgayHetHan = hdct[i].QuanLyTheoLoHang ? (itemLot.length > 0 ? itemLot[0].NgayHetHan : '') : '';
            }
            localStorage.setItem('lc_CTSaoChepKK', JSON.stringify(hdct));
            clickloadForm('StockTakes/inventory');
            localStorage.setItem('isSaoChepKK', true);
            localStorage.setItem('isUpdate', false);
        }
        //else {
        //    ShowMessage_Danger( "Không có quyền sao chép phiếu kiểm kê!");
        //}
    }

    self.AddKiemKhoWhereLoHang = function (str) {
        var countMaLoErr = 0;
        var objectStore = db.transaction(table, "readwrite").objectStore(table);
        var req = objectStore.openCursor(key_Add);
        req.onsuccess = function (e) {
            if (req.result !== null && req.result.value !== null) {
                var lc_CTHoaDon = JSON.parse(req.result.value.Value);
                if (lc_CTHoaDon !== null) {
                    for (var i = 0; i < lc_CTHoaDon.length; i++) {
                        var lcCTHD = lc_CTHoaDon[i];
                        if (lcCTHD.QuanLyTheoLoHang === true) {
                            if (lc_CTHoaDon[i].MaLoHang === "" || lc_CTHoaDon[i].MaLoHang === null) {
                                countMaLoErr++;
                            }
                            var arrLot = $.grep(self.ListLoHang(), function (x) {
                                return x.MaLoHang === lc_CTHoaDon[i].MaLoHang;
                            });
                            if (arrLot === null) {
                                countMaLoErr++;
                            }
                        }
                    }
                }
                if (countMaLoErr === 0) {
                    self.addKiemKho(str);
                }
                else {
                    $('.bgwhite').hide();
                    ShowMessage_Danger("Mã lô hàng không được để trống hoặc không có trong lô hàng");
                    return false;
                }
            }
        }
    }

    self.AddKiemKhoTLWhereLoHang = function (str) {
        var countMaLoErr = 0;
        var objectStore = db.transaction(table, "readwrite").objectStore(table);
        var req = objectStore.openCursor(key_Add);
        req.onsuccess = function (e) {
            if (req.result !== null && req.result.value !== null) {
                var lc_CTHoaDon = JSON.parse(req.result.value.Value);
                if (lc_CTHoaDon !== null) {
                    for (var i = 0; i < lc_CTHoaDon.length; i++) {
                        var lcCTHD = lc_CTHoaDon[i];
                        if (lcCTHD.QuanLyTheoLoHang === true) {
                            if (lc_CTHoaDon[i].MaLoHang === "" || lc_CTHoaDon[i].MaLoHang === null) {
                                countMaLoErr++;
                            }
                            var arrLot = $.grep(self.ListLoHang(), function (x) {
                                return x.MaLoHang === lc_CTHoaDon[i].MaLoHang;
                            });
                            if (arrLot == null) {
                                countMaLoErr++;
                            }
                        }
                    }
                }
                if (countMaLoErr === 0) {
                    self.addKiemKhoTL(str);
                }
                else {
                    ShowMessage_Danger("Mã lô hàng không được để trống hoặc không có trong lô hàng");
                    return false;
                }

            }
        }
    }

    function EnableBtnLuu() {
        document.getElementById("btnHoanThanhKK").disabled = false;
        document.getElementById("btnHoanThanhKK").lastChild.data = "Lưu (F10)";
    }
    function EnableBtnTamLuu() {
        document.getElementById("btnTamLuuKK").disabled = false;
        document.getElementById("btnTamLuuKK").lastChild.data = "Lưu nháp";
    }

    self.addKiemKho = function (str) {
        document.getElementById("btnHoanThanhKK").disabled = true;
        document.getElementById("btnHoanThanhKK").lastChild.data = "Đang Lưu";
        self.numbersPrintHD($('#txtNumberOfPrint').val());
        var _maHoaDon = self.newKiemKho().MaHoaDon();
        var _id = self.newKiemKho().ID();
        var _idhoadon = self.newKiemKho().ID_HoaDon();
        var _idNhanVien = self.selectedNV();
        var specialChars = "<>!#$%^&*()+[]{}?:;|'\"\\,/~`=' '"
        var check = function (string) {
            for (i = 0; i < specialChars.length; i++) {
                if (string.indexOf(specialChars[i]) > -1) {
                    return true;
                }
            }
            return false;
        }

        if (check($('#makiemkho').val()) === false) {
        } else {
            $('.bgwhite').hide();
            EnableBtnLuu();
            ShowMessage_Danger("Mã phiếu kiểm không được chứa ký tự đặc biệt");
            $('#makiemkho').focus();
            return false;
        }

        var _ngaylaphd = $('#datetimepicker').val();
        if (_ngaylaphd === "") {
            _ngaylaphd = moment(new Date()).format('DD/MM/YYYY HH:mm');
        }
        var checkn = CheckNgayLapHD_format(_ngaylaphd);

        if (!checkn) {
            EnableBtnLuu();
            return false;
        }
        let idNVLap = self.newKiemKho().ID_NhanVien();
        if (commonStatisJs.CheckNull(idNVLap)) {
            idNVLap = _IDNhanVien;
        }
        var _ngayLapHoaDon = moment(_ngaylaphd, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
        var KiemKho = {
            ID: _id,
            MaHoaDon: _maHoaDon,
            ID_NhanVien: idNVLap,
            NgayLapHoaDon: _ngayLapHoaDon,
            LoaiHoaDon: loaiHoaDon,
            DienGiai: self.newKiemKho().DienGiai(),
            TongChiPhi: ($('#sltang').html()),
            TongTienHang: ($('#slgiam').html()),
            TongGiamGia: ($('#tongchenhlech').html()),
            PhaiThanhToan: $('#giatritang').html(),
            TongChietKhau: $('#giatrigiam').html(),
            TongTienThue: $('#tonggiatrilech').html(),
            ID_DonVi: _IDchinhanh,
            ChoThanhToan: false,
            NguoiTao: _txtTenTaiKhoan,
            TenNhanVien: $('#txtNhanVienKK option:selected').text(),
        };
        var myData = {};
        myData.id = _id;
        myData.idhoadon = _idhoadon;
        myData.objnewKho = KiemKho;
        myData.idnhanvien = _IDNhanVien;

        var store = db.transaction(table, "readwrite").objectStore(table);
        var req = store.openCursor(key_Add);
        req.onsuccess = function (event) {
            var cursor = event.target.result;
            if (cursor) {
                //myData.objChiTietKho = JSON.parse(cursor.value.Value);
                var objKK = [];
                var lc_CTKiemKho = JSON.parse(cursor.value.Value);
                for (var i = 0; i < lc_CTKiemKho.length; i++) {
                    if (lc_CTKiemKho[i].ThanhTien !== "" && lc_CTKiemKho[i].ThanhTien !== null) {
                        lc_CTKiemKho[i].TienThue = 0;
                        objKK.push(lc_CTKiemKho[i]);
                    }
                }
                myData.objChiTietKho = objKK;
                if (myData.objChiTietKho === null || self.newKiemKho().BH_KiemKho_ChiTiet() === null) {
                    $('.bgwhite').hide();
                    EnableBtnLuu();
                    ShowMessage_Danger("Chưa có hàng hóa trong danh sách kiểm hàng");
                    return false;
                }
            }
            if (self.SLChuaKiems().length === self.newKiemKho().BH_KiemKho_ChiTiet().length) {
                $('.bgwhite').hide();
                EnableBtnLuu();
                ShowMessage_Danger("Chi tiết phiếu kiểm kho trống");
                return false;
            }
            if (_id === undefined) {
                ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + "Check_MaHoaDonExist?maHoaDon=" + _maHoaDon, 'POST').done(function (data) {
                    if (data) {
                        ShowMessage_Danger("Mã hóa đơn đã tồn tại");
                        EnableBtnLuu();
                        $('#makiemkho').focus();
                        return false;
                    }
                    else {
                        ajaxHelper("/api/DanhMuc/BH_HoaDonAPI/" + "PostBH_HoaDonKiemKho", 'POST', myData).done(function (x) {
                            if (x.res) {
                                let item = x.dataSoure;
                                self.KiemKhos.push(item);

                                if (localStorage.getItem('InHoaDonKhiHT') === "true") {
                                    ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'GetChiTietHD_byIDHoaDon?idHoaDon=' + item.ID + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data1) {
                                        self.BH_HoaDonChiTietsThaoTac(data1);

                                        myData.objnewKho.MaHoaDon = item.MaHoaDon;
                                        myData.objnewKho.ID = item.ID;
                                        self.InHoaDon(myData.objnewKho);
                                    });
                                }
                                clickloadForm(str);

                                var objectStore = db.transaction(table, "readwrite").objectStore(table);
                                var req = objectStore.delete(key_Add);
                            }
                            else {
                                ShowMessage_Danger(x.mess);
                            }
                        }).always(function () {
                            EnableBtnLuu();
                        })
                    }
                });
            }
            else {
                ajaxHelper("/api/DanhMuc/BH_HoaDonAPI/" + "Put_KiemKho", 'POST', myData).done(function (x) {
                    if (x.res) {
                        clickloadForm(str);
                        var objectStore = db.transaction(table, "readwrite").objectStore(table);
                        var req = objectStore.delete(key_Add);
                    }
                    else {
                        ShowMessage_Danger(x.mess);
                    }
                }).always(function () {
                    EnableBtnLuu();
                })
            }
        };
    }

    self.addKiemKhoTL = function (str) {
        document.getElementById("btnTamLuuKK").disabled = true;
        document.getElementById("btnTamLuuKK").lastChild.data = "Đang Lưu";
        var _maHoaDon = self.newKiemKho().MaHoaDon();
        var _id = self.newKiemKho().ID();
        var _idhoadon = self.newKiemKho().ID_HoaDon();
        var _idNhanVien = self.newKiemKho().ID_NhanVien() !== null ? self.newKiemKho().ID_NhanVien() : null;
        var specialChars = "<>!#$%^&*()+[]{}?:;|'\"\\,/~`=' '"
        var check = function (string) {
            for (i = 0; i < specialChars.length; i++) {
                if (string.indexOf(specialChars[i]) > -1) {
                    return true;
                }
            }
            return false;
        }
        if (check($('#makiemkho').val()) == false) {
        } else {
            EnableBtnTamLuu();
            ShowMessage_Danger("Mã phiếu kiểm không được chứa ký tự đặc biệt");
            $('#makiemkho').focus();
            return false;
        }

        _ngaylaphd = $('#datetimepicker').val();
        if (_ngaylaphd === "") {
            _ngaylaphd = moment(new Date()).format('DD/MM/YYYY HH:mm');
        }
        var checkn = CheckNgayLapHD_format(_ngaylaphd);

        if (!checkn) {
            return false;
        }

        var _ngayLapHoaDon = moment(_ngaylaphd, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');

        var KiemKho = {
            ID: _id,
            MaHoaDon: _maHoaDon,
            ID_NhanVien: _idNhanVien,
            NgayLapHoaDon: _ngayLapHoaDon,
            LoaiHoaDon: loaiHoaDon,
            DienGiai: self.newKiemKho().DienGiai(),
            TongChiPhi: ($('#sltang').html()),
            TongTienHang: ($('#slgiam').html()),
            TongGiamGia: ($('#tongchenhlech').html()),
            PhaiThanhToan: $('#giatritang').html(),
            TongChietKhau: $('#giatrigiam').html(),
            TongTienThue: $('#tonggiatrilech').html(),
            ID_DonVi: _IDchinhanh,
            ChoThanhToan: true
        };
        var myData = {};
        myData.id = _id;
        myData.idhoadon = _idhoadon;
        myData.objnewKho = KiemKho;
        myData.idnhanvien = _IDNhanVien;
        var store = db.transaction(table, "readwrite").objectStore(table);
        var req = store.openCursor(key_Add);
        req.onsuccess = function (event) {
            var cursor = event.target.result;
            if (cursor) {
                let objKK = [];
                let lc_CTKiemKho = JSON.parse(cursor.value.Value);
                for (let i = 0; i < lc_CTKiemKho.length; i++) {
                    if (lc_CTKiemKho[i].ThanhTien !== "" && lc_CTKiemKho[i].ThanhTien !== null) {
                        lc_CTKiemKho[i].TienThue = 0;
                        objKK.push(lc_CTKiemKho[i]);
                    }
                }
                myData.objChiTietKho = objKK;

                if (myData.objChiTietKho == null || self.newKiemKho().BH_KiemKho_ChiTiet() == null) {
                    EnableBtnTamLuu();
                    ShowMessage_Danger("Chưa có hàng hóa trong danh sách kiểm hàng");
                    return false;
                }
            }

            if (self.SLChuaKiems().length == self.newKiemKho().BH_KiemKho_ChiTiet().length) {
                EnableBtnTamLuu();
                ShowMessage_Danger("Chi tiết phiếu kiểm kho trống");
                return false;
            }
            if (_id == undefined) {
                ajaxHelper("/api/DanhMuc/BH_HoaDonAPI/" + "PostBH_HoaDonKiemKho", 'POST', myData).done(function (x) {
                    if (x.res) {

                        self.KiemKhos.push(x.dataSoure);
                        clickloadForm(str);

                        var objectStore = db.transaction(table, "readwrite").objectStore(table);
                        var req = objectStore.delete(key_Add);
                    }
                    else {
                        ShowMessage_Danger(x.mess);
                    }
                }).always(function () {
                    EnableBtnTamLuu();
                })
            } else {
                ajaxHelper("/api/DanhMuc/BH_HoaDonAPI/" + "Put_KiemKho", 'POST', myData).done(function (x) {
                    if (x.res) {
                        clickloadForm(str);
                        var objectStore = db.transaction(table, "readwrite").objectStore(table);
                        var req = objectStore.delete(key_Add);
                    }
                    else {
                        ShowMessage_Danger(x.mess);
                    }
                }).always(function () {
                    EnableBtnTamLuu();
                })
            }
        };
    }

    function CheckNgayLapHD_format(valDate) {

        var dateNow = moment(new Date()).format('YYYY-MM-DD HH:mm');
        var ngayLapHD = moment(valDate, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');

        if (valDate === '') {
            ShowMessage_Danger("Vui lòng nhập ngày lập phiếu nhập");
            return false;
        }

        if (valDate.indexOf('_') > -1) {
            ShowMessage_Danger("Ngày lập phiếu nhập chưa đúng định dạng");
            return false;
        }

        if (ngayLapHD > dateNow) {
            ShowMessage_Danger("Ngày lập phiếu nhập vượt quá thời gian hiện tại");
            return false;
        }

        let chotSo = VHeader.CheckKhoaSo(moment(ngayLapHD, 'YYYY-MM-DD HH:mm').format('YYYY-MM-DD'));
        if (chotSo) {
            ShowMessage_Danger(VHeader.warning.ChotSo.Update);
            return false;
        }
        return true;
    }

    function GetDataChotSo() {
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/GetDataChotSo?idChiNhanh=' + _IDchinhanh, 'GET').done(function (data) {
            if (data !== null && data.length > 0) {
                self.ChotSo_ChiNhanh(data);
                getAllDMLoHang();
            }
            else {
                getAllDMLoHang();
            }
        })
    }

    GetDataChotSo();

    self.XoaLSHoaDonKK = function (mahanghoa) {
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Kiểm kho",
            NoiDung: "Xóa phiếu kiểm kho : " + mahanghoa,
            NoiDungChiTiet: "Xóa phiếu kiểm kho : <a onclick=\"FindKiemKho('" + mahanghoa + "')\">" + mahanghoa + " </a> ",
            LoaiNhatKy: 3// 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        Insert_NhatKyThaoTac_1Param(objDiary);
    }

    self.filterFind = function (item, inputString) {
        var itemSearch = locdau(item.TenHangHoa).toLowerCase();
        var itemSearch1 = locdau(item.MaHangHoa).toLowerCase();

        var locdauInput = locdau(inputString).toLowerCase();
        var arr = itemSearch.split(/\s+/);
        var arr1 = itemSearch1.split(/\s+/);

        var sThreechars = '';
        var sThreechars1 = '';
        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }
        for (var i = 0; i < arr1.length; i++) {
            sThreechars1 += arr1[i].toString().split('')[0];
        }
        return itemSearch.indexOf(locdauInput) > -1 ||
            itemSearch1.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1 ||
            sThreechars1.indexOf(locdauInput) > -1;
    }

    self.BH_HoaDonChiTiets = ko.observableArray();
    self.BH_HoaDonChiTietsThaoTac = ko.observableArray();
    self.IDHDSearch = ko.observable();

    self.Enable_NgayLapHD = ko.observable(true);

    var itemcheck = '';
    self.checkThietLapLoHang = ko.observable();
    self.LoadChiTietHD = function (item, e) {
        self.Enable_NgayLapHD(!VHeader.CheckKhoaSo(moment(item.NgayLapHoaDon).format('YYYY-MM-DD'), item.ID_DonVi));

        if (itemcheck !== item.ID) {
            itemcheck = item.ID;
            self.BH_HoaDonChiTiets([]);
            self.IDHDSearch(item.ID);

            $('.table-detal').gridLoader({
                style: "left: 460px;top: 200px;"
            });
            ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'GetChiTietHD_byIDHoaDonKiemKho?idHoaDon=' + item.ID + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
                var sum = 0;
                for (var i = 0; i < data.length; i++) {
                    sum = sum + data[i].SoLuong;
                    if (data[i].MaHangHoa.indexOf('{DEL}') > -1) {
                        data[i].MaHangHoa = data[i].MaHangHoa.substr(0, data[i].MaHangHoa.length - 5);
                        data[i].Del = '{Xóa}';
                    } else {
                        data[i].Del = "";
                    }
                }
                self.BH_HoaDonChiTietsThaoTac(data);
                $('#tongsltrahang1').html(formatNumber3Digit(sum));
                $('#tongslbanhang').html(formatNumber3Digit(sum));
                $('#lbltongsoluong').html(formatNumber3Digit(sum));
                $('#lbltongsoluongmh').html(data.length);
                $('.table-detal').gridLoader({ show: false });
                searchCTHN();
                SetHeightShowDetail($(e.currentTarget));
            });
            var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
            if ($.inArray('KiemKho_ThemMoi', lc_CTQuyen) > -1) {
                $('.clickkiemhang').css('display', 'block');
            }
            if ($.inArray('KiemKho_SaoChep', lc_CTQuyen) > -1) {
                $('.saochepKK').show();
            }
            if ($.inArray('KiemKho_Xoa', lc_CTQuyen) > -1) {
                $('.xoaKK').show();
            }
            else {
                $('.xoaKK').hide();
            }
            if ($.inArray('KiemKho_XuatFile', lc_CTQuyen) > -1) {
                $('.kiemkhoxuat').show();
            }
            var lc_CTThietLap = JSON.parse(localStorage.getItem('lc_CTThietLap'));
            self.checkThietLapLoHang(lc_CTThietLap.LoHang);
        }
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

    function getallKiemKho() {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('KiemKho_XemDS', lc_CTQuyen) > -1) {
            var FindHD = (localStorage.getItem('FindHD'));
            if (FindHD !== null) {
                self.filterKK(FindHD);
                SearchKiemKho();
            } else {
                if (localStorage.getItem('isUpdate') === 'true') {
                    var lc_UpdateKK = localStorage.getItem('lc_UpdateKK');
                    if (lc_UpdateKK !== null) {
                        lc_UpdateKK = JSON.parse(lc_UpdateKK);
                        self.newKiemKho().ID(lc_UpdateKK[0].ID_HoaDon);
                        self.newKiemKho().ID_HoaDon(lc_UpdateKK[0].ID);
                        self.newKiemKho().MaHoaDon(lc_UpdateKK[0].MaHoaDon);
                        self.newKiemKho().DienGiai(lc_UpdateKK[0].DienGiai);
                        self.newKiemKho().BH_KiemKho_ChiTiet(lc_UpdateKK);
                        //localStorage.setItem('lc_CTKiemKho', JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet()));
                        var store = db.transaction(table, "readwrite").objectStore(table);
                        var req = store.openCursor(key_Add);
                        req.onsuccess = function (event) {
                            var cursor = event.target.result;
                            if (cursor) {
                                var updateData = cursor.value;
                                updateData.Value = JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet());
                                var request = cursor.update(updateData);
                                request.onsuccess = function () {
                                };
                            }
                            else {
                                store.add({ Key: key_Add, Value: JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet()) })
                            }
                        };
                        self.loadData();

                    }
                }
                if (localStorage.getItem('isSaoChepKK') === 'true') {
                    var lc_CTSaoChepKK = localStorage.getItem('lc_CTSaoChepKK');
                    if (lc_CTSaoChepKK !== null) {
                        lc_CTSaoChepKK = JSON.parse(lc_CTSaoChepKK);
                        self.newKiemKho().MaHoaDon(lc_CTSaoChepKK[0].MaHoaDon);
                        self.newKiemKho().DienGiai(lc_CTSaoChepKK[0].DienGiai);
                        self.newKiemKho().BH_KiemKho_ChiTiet(lc_CTSaoChepKK);
                        //localStorage.setItem('lc_CTKiemKho', JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet()));
                        var store = db.transaction(table, "readwrite").objectStore(table);
                        var req = store.openCursor(key_Add);
                        req.onsuccess = function (event) {
                            var cursor = event.target.result;
                            if (cursor) {
                                var updateData = cursor.value;
                                updateData.Value = JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet());
                                var request = cursor.update(updateData);
                                request.onsuccess = function () {
                                };
                            }
                            else {
                                store.add({ Key: key_Add, Value: JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet()) })
                            }
                        };

                        self.loadData();
                    }
                }

                if (self.newKiemKho().BH_KiemKho_ChiTiet().length > 0) {
                    $('#importKiemKho').hide();
                }

            }
        }
        localStorage.removeItem('FindHD');
    }

    self.deleteHHCT = function (item) {
        self.newHangHoa().DinhLuongDichVu.remove(item);
        self.TinhLaiTienDV();
    };
    self.deleteHHCTHH = function (item) {
        self.newHangHoa().DinhLuongDichVu.remove(item);
        self.TinhLaiTien();
    }
    self.TinhLaiTien = function () {
        if (self.newHangHoa().DinhLuongDichVu() === "") {
            self.TPDL_SumTienVon(0);
        } else {
            var sum = 0;
            for (var i = 0; i < self.newHangHoa().DinhLuongDichVu().length; i++) {
                sum += self.newHangHoa().DinhLuongDichVu()[i].GiaVon * self.newHangHoa().DinhLuongDichVu()[i].SoLuong;
            }
            self.TPDL_SumTienVon(sum.toFixed());
        }
    }

    self.TinhLaiTienDV = function () {
        if (self.newHangHoa().DinhLuongDichVu().length === 0) {
            self.TPDL_SumTienVon(0);
        } else {
            var sum = 0;
            $('.clThanhTiendv').each(function () {
                var valThis = $(this).text();
                var ivalThis = formatNumberToFloat(valThis);
                sum += ivalThis;
                self.TPDL_SumTienVon(sum.toFixed());
            })
        }
    }

    self.xoaHH = function (HangHoas) {
        $.ajax({
            type: "DELETE",
            url: DMHangHoaUri + "DeleteDM_HangHoa?idquidoi=" + HangHoas.deleteID() + "&idcungloai=" + HangHoas.deleteID_CungLoai(),
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                SearchHangHoa();
                self.XoaHangHoaLS(HangHoas.deleteMaHangHoa());
                ShowMessage_Success("Cập nhật hàng hóa thành công!");
                ReloadSearchHangHoa();
            },
            error: function (error) {
                ShowMessage_Danger("Cập nhật hàng hóa thất bại. Vì mặt hàng này đã có trong kho!!!");
            }
        })
    };

    self.xoaNHH = function (NhomHangHoas) {
        $.ajax({
            type: "DELETE",
            url: NhomHHUri + "DeleteDM_NhomHangHoa1/" + NhomHangHoas.deleteID(),
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                self.XoaNhomHangHoaLS(NhomHangHoas.deleteTenNhomHang());
                $('#modalPopup_NhomHHDV').modal('hide');
                ShowMessage_Success("Cập nhật nhóm hàng hóa thành công!");
                ajaxHelper(NhomHHUri + 'UpdateHangHoaByIDNhom?id_nhom=' + NhomHangHoas.deleteID(), 'GET').done(function (data) {
                    SearchHangHoa();
                    GetAllNhomHH();
                })
            },
            error: function (error) {
                ShowMessage_Danger("Dữ liệu đang được sử dụng không thể xóa");
                $('#modalPopup_NhomHHDV').modal('hide');
                SearchHangHoa();
            }
        })
    };

    self.HHKiemKhos = ko.observableArray();
    self.valMaHangHoa = ko.observable();
    self.searchHH = function () {
        var txtautoHH = $('#txtHangHoaauto').val();
        self.valMaHangHoa(txtautoHH);
        txtautoHH = locdau(txtautoHH).toLowerCase();
        ajaxHelper(DMHangHoaUri + 'SearchHangHoaByText?id_donvi=' + _IDchinhanh + '&txtSearch=' + self.valMaHangHoa(), 'GET').done(function (data) {
            self.HHKiemKhos(data);
            $('#txtHangHoaauto').val(self.valMaHangHoa());
        });
    }

    function getAllDMHangHoas() {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('HangHoa_XemDS', lc_CTQuyen) > -1) {
            SearchHangHoa();
        }
    }
    self.getAllDMHangHoas = function () {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('HangHoa_XemDS', lc_CTQuyen) > -1) {
            //hidewait('table_h');
            $(".checkboxsetHH").prop("checked", false);
            //ajaxHelper(DMHangHoaUri + "GetListHangHoas?currentPage=" + self.currentPage() + '&pageSize=' + self.pageSize(), 'GET').done(function (data) {
            //    self.HangHoas(data);
            self.arrIDNhomHang([]);
            //$("div[id ^= 'wait']").text("");
            SearchHangHoa();

            $('.li-oo').removeClass("yellow");
            $('#tatcanhh a').css("display", "block");
            $('#tatcanhh').addClass("yellow");
        }
        //});
    }

    self.themmoicapnhatnhomhanghoa = function () {
        self.resetNhomHangHoa();
        self._ThemMoiNhomHH(true);
        $('#modalPopup_NhomHHDV').modal('show');

    }
    self.mahoadon = ko.observable();
    self.loaihoadon = ko.observable();
    self.loadYeuCau = ko.observable();
    self.showpopupTK = function (item, e) {
        self.IDHDSearch(item.ID_HoaDon);
        ajaxHelper("/api/DanhMuc/BH_HoaDonAPI/" + "GetChiTietHoaDon/" + item.ID_HoaDon, 'GET').done(function (data) {
            data[0].NgayLapHoaDon = moment(data[0].NgayLapHoaDon).format('DD/MM/YYYY HH:mm');
            data[0].NgaySua = data[0].NgaySua !== null ? moment(data[0].NgaySua).format('DD/MM/YYYY HH:mm') : null;
            data[0].TongTienHang = formatNumber3Digit(data[0].TongTienHang);
            data[0].TongGiamGia = formatNumber3Digit(data[0].TongGiamGia);
            data[0].PhaiThanhToan = formatNumber3Digit(data[0].PhaiThanhToan);
            data[0].KhachDaTra = formatNumber3Digit(data[0].KhachDaTra);
            data[0].TongTienHDTra = formatNumber3Digit(data[0].TongTienHDTra);

            console.log('data ', data)
            self.newHoaDon().SetData(data[0]);
            self.mahoadon(data[0].MaHoaDon);
            self.loaihoadon(data[0].LoaiHoaDon);
            self.loadYeuCau(data[0].LoadYeuCau);

            item.ID = item.ID_HoaDon;

            self.currentPageCTNH(0);
            self.LoadChiTietHD(item, e);
        });
        switch (item.LoaiHoaDon) {
            case 6:
                $('#modalpopup_PhieuTH').modal('show');
                break;
            case 7:
                $('#modalpopup_PhieuTHN').modal('show');
                break;
            case 8:
            case 35:// xuat TPDL
            case 37:
            case 38:
            case 39:
            case 40:
                $('#modalpopup_PhieuXH').modal('show');
                break;
            case 9:
                $('#modalpopup_KiemKho').modal('show');
                break;
            case 10:
                if (item.ID_CheckIn !== _IDchinhanh) {
                    $('#modalPopuplg_TheKho').modal('show');
                    $('#tenphieu').html("Chuyển hàng");
                }
                else {
                    $('#modalPopuplg_TheKho').modal('show');
                    $('#tenphieu').html("Nhận hàng");
                }
                break;
            case 18:
                $('#modalpopup_PhieuDieuChinh').modal('show');
                break;
            case 4:
            case 13:
            case 14:
                $('#modalPopuplg_NhapHang').modal('show');
                break;
            case 36:
                break;
        }
    }

    self.linkphieu = function () {
        self.mahoadon();
        localStorage.setItem('FindHD', self.mahoadon());
        self.loaihoadon();

        var url = '';
        switch (parseInt(self.loaihoadon())) {
            case 1:
                url = "/#/Invoices";
                break;
            case 2:
                url = "/#/HoaDonBaoHanh";
                break;
            case 4:
                url = "/#/PurchaseOrder";
                break;
            case 13:
                url = "/#/NhapNoiBo";
                break;
            case 6:
                url = "/#/Returns";
                break;
            case 7:
                url = "/#/PurchaseReturns";
                break;
            case 8:
            case 35:
            case 37:
            case 38:
            case 39:
            case 40:
                url = "/#/DamageItems";
                break;
            case 9:
                localStorage.setItem('FindDieuChinh', self.mahoadon());
                localStorage.removeItem('FindHD');
                url = "/#/StockTakes";
                break;
            case 10:
                url = "/#/Transfers";
                break;
            case 18:
                url = "/#/CouponAdjustment";
                break;
        }
        window.open(url);
    }

    self.getHangHoaByIDNhomHH = function () {
        var IDNhomHH = self.selectIDNhomHHKiemKho();
        if (IDNhomHH === undefined) {
            ShowMessage_Danger("Vui lòng chọn nhóm hàng hóa.");
            return false;
        }
        var arrIDChilds = [];
        var lcNhomHH = localStorage.getItem('lc_NhomHangHoas');
        if (lcNhomHH !== null) {
            lcNhomHH = JSON.parse(lcNhomHH);
            for (var i = 0; i < lcNhomHH.length; i++) {
                if (lcNhomHH[i].ID === IDNhomHH) {
                    for (var j = 0; j < lcNhomHH.length; j++) {
                        if (lcNhomHH[j].ID_Parent === IDNhomHH) {
                            // get ID_Child level 1
                            arrIDChilds.push(lcNhomHH[j].ID);
                            for (var k = 0; k < lcNhomHH.length; k++) {
                                if (lcNhomHH[k].ID_Parent === lcNhomHH[j].ID) {
                                    // get ID_Child level 2
                                    arrIDChilds.push(lcNhomHH[k].ID);
                                }
                            }
                        }
                    }
                    arrIDChilds.push(lcNhomHH[i].ID);
                    break;
                }
            }
        }
        self.arrIDNhomHang(arrIDChilds);
        if (self.arrIDNhomHang().length === 0) {
            self.arrIDNhomHang(null);
        }
        $('.hideloadhhbynhomhang').gridLoader({ style: "top:50px" });
        var _ngayKk = $('#datetimepicker').val();
        if (_ngayKk === "") {
            _ngayKk = moment(new Date()).format('DD/MM/YYYY HH:mm');
        }
        var check = CheckNgayLapHD_format(_ngayKk);

        if (!check) {
            $('.hideloadhhbynhomhang').gridLoader({ show: false });
            return false;
        }
        var _ngayKiemKe = moment(_ngayKk, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
        ajaxHelper(DMHangHoaUri + "GetHangHoa_ByIDNhomKiemKho?id=" + self.arrIDNhomHang() + '&iddonvi=' + _IDchinhanh + '&timeKK=' + _ngayKiemKe, 'GET').done(function (data) {
            $('.hideloadhhbynhomhang').gridLoader({ show: false });
            var arrID = [];
            var arrIDCK = [];
            for (var i = 0; i < self.newKiemKho().BH_KiemKho_ChiTiet().length; i++) {
                arrID.push(self.newKiemKho().BH_KiemKho_ChiTiet()[i].ID_DonViQuiDoi);
            }
            //for (var i = 0; i < self.SLChuaKiems().length; i++) {
            //    arrIDCK.push(self.SLChuaKiems()[i].ID_DonViQuiDoi);
            //}
            for (var i = 0; i < data.length; i++) {
                if ($.inArray(data[i].ID_DonViQuiDoi, arrID) === -1) {
                    self.newKiemKho().BH_KiemKho_ChiTiet.push(data[i]);
                }
                //if ($.inArray(data[i].ID_DonViQuiDoi, arrIDCK) === -1) {
                //    self.SLChuaKiems.push(data[i]);
                //}
            }
            var objectStore = db.transaction(table, "readwrite").objectStore(table);
            var req = objectStore.openCursor(key_Add);
            req.onsuccess = function (evt) {
                objectStore.delete(key_Add);
                objectStore.add({ Key: key_Add, Value: JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet()) });
                self.loadData();
            };
            if (self.newKiemKho().BH_KiemKho_ChiTiet().length > 0) {
                $('#importKiemKho').hide();
            }
            else {
                $('#importKiemKho').show();
                self.deleteFileSelect();
            }
            $('#modalPopuplg_TMTK').modal('hide');
            $('#tongitem').text(self.newKiemKho().BH_KiemKho_ChiTiet().length);
            $('#tongitemkhop').text(self.SLKhops().length);
            $('#tongitemlech').text(self.SLLechs().length);
            $('#tongitemchuakiem').text(self.SLChuaKiems().length);
            self.TinhLaiLech();
            self.TinhLaiSLKhop();
            self.TinhLaiSLThuc();
            UpdateAgain_ListDVT();
        });
    };

    self.arrIDNhomHang = ko.observableArray();
    self.changeddlNhomHangHoa = function (item) {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('HangHoa_XemDS', lc_CTQuyen) > -1) {
            self.currentPage(0);
            $('.ss-li .li-oo').removeClass("yellow");
            $('#tatcanhh').removeClass("yellow");
            $('.li-pp').removeClass("yellow");
            $('#tatcanhh a').css("display", "none");
            $('#' + item.ID).addClass("yellow");
            if (item.ID == undefined) {
                getAllDMHangHoas();
            } else {
                var arrIDChilds = [];
                var lcNhomHH = localStorage.getItem('lc_NhomHangHoas');
                if (lcNhomHH !== null) {
                    lcNhomHH = JSON.parse(lcNhomHH);
                    for (var i = 0; i < lcNhomHH.length; i++) {
                        if (lcNhomHH[i].ID === item.ID) {
                            for (var j = 0; j < lcNhomHH.length; j++) {
                                if (lcNhomHH[j].ID_Parent === item.ID) {
                                    // get ID_Child level 1
                                    arrIDChilds.push(lcNhomHH[j].ID);
                                    for (var k = 0; k < lcNhomHH.length; k++) {
                                        if (lcNhomHH[k].ID_Parent === lcNhomHH[j].ID) {
                                            // get ID_Child level 2
                                            arrIDChilds.push(lcNhomHH[k].ID);
                                        }
                                    }
                                }
                            }
                            arrIDChilds.push(lcNhomHH[i].ID);
                            break;
                        }
                    }
                }
                self.arrIDNhomHang(arrIDChilds);
                SearchHangHoa();
            }

            $(".checkboxsetHH").prop("checked", false);
        }
    }

    self.getChiTietHangHoaByID = function (item) {
        if (item.QuanLyTheoLoHang === false) {
            ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetHangHoa_ByIDQuyDoiDVT?id=" + item.ID_DonViQuiDoi + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
                for (var i = 0; i < self.HangHoas().length; i++) {
                    for (var j = 0; j < self.HangHoas()[i].DonViTinh.length; j++) {
                        if (data.ID_DonViQuiDoi === self.HangHoas()[i].DonViTinh[j].ID_DonViQuiDoi) {
                            data.CountCungLoai = 1;
                            self.HangHoas.replace(self.HangHoas()[i], data);
                        }
                        if ($.inArray(data.ID_DonViQuiDoi, arrIDHang) > -1) {
                            $('#' + data.ID_HangHoaCungLoai).find('#' + data.ID_DonViQuiDoi).prop('checked', true);
                        }
                    }
                }
                LoadHtmlGridHH();
                itemcheckhh = '';
            })
        }
        else {
            ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetHangHoa_ByIDQuyDoiDVTByLo?id=" + item.ID_DonViQuiDoi + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
                for (var i = 0; i < self.HangHoas().length; i++) {
                    for (var j = 0; j < self.HangHoas()[i].DonViTinh.length; j++) {
                        if (data.ID_DonViQuiDoi === self.HangHoas()[i].DonViTinh[j].ID_DonViQuiDoi) {
                            data.CountCungLoai = 1;
                            self.HangHoas.replace(self.HangHoas()[i], data);
                        }
                        if ($.inArray(data.ID_DonViQuiDoi, arrIDHang) > -1) {
                            $('#' + data.ID_HangHoaCungLoai).find('#' + data.ID_DonViQuiDoi).prop('checked', true);
                        }
                    }
                }
                LoadHtmlGridHH();
                itemcheckhh = '';
            })
        }
    }

    self.getChiTietHangHoaCungLoaiByID = function (item) {
        ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetHangHoa_ByIDQuyDoiDVT?id=" + item.ID_DonViQuiDoi + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
            for (let i = 0; i < self.HangHoaCungLoai().length; i++) {
                for (let j = 0; j < data.DonViTinh.length; j++) {
                    let dvt = data.DonViTinh[j];
                    if (self.HangHoaCungLoai()[i].ID_DonViQuiDoi == dvt.ID_DonViQuiDoi) {

                        break;
                    }
                }
            }
        })
    }

    self.getCTHHHH = ko.observable();

    self.tinhsoluong = function (item) {
        var thisObj = event.currentTarget;
        formatNumberObj(thisObj);
        var quycach = item.QuyCach;
        var soluongNhap = formatNumberToFloat($(thisObj).val());
        var soluongQuyDoi = soluongNhap / quycach;
        var thanhTien = (soluongQuyDoi * item.GiaVon).toFixed();
        for (var i = 0; i < self.newHangHoa().DinhLuongDichVu().length; i++) {
            if (self.newHangHoa().DinhLuongDichVu()[i].ID_DonViQuiDoi === item.ID_DonViQuiDoi) {
                self.newHangHoa().DinhLuongDichVu()[i].SoLuongQuyCach = soluongNhap;
                self.newHangHoa().DinhLuongDichVu()[i].SoLuong = soluongQuyDoi;
                self.newHangHoa().DinhLuongDichVu()[i].ThanhTien = (soluongQuyDoi * self.newHangHoa().DinhLuongDichVu()[i].GiaVon).toFixed();
                break;
            }
        }
        $(thisObj).parent().next().find('input').val(formatNumber3Digit(soluongQuyDoi, 3));
        $('#thanhtien_' + item.ID_DonViQuiDoi).html(formatNumber3Digit(thanhTien));
        $('#thanhtiendv_' + item.ID_DonViQuiDoi).html(formatNumber3Digit(thanhTien));
        self.TinhLaiTien();
        self.TinhLaiTienDV();
        var keycode = event.keyCode || event.which;
        if (keycode === 13) {
            let tr = $(thisObj).closest('tr').next();
            tr.children('td').eq(2).find('input').focus().select();
        }
    };

    self.tinhsoluongquicach = function (item) {
        var thisObj = event.currentTarget;
        formatNumberObj(thisObj);

        var quycach = item.QuyCach;
        var soluongNhap = formatNumberToFloat($(thisObj).val());
        var soluongQuyCach = soluongNhap * quycach;

        // update value in grid TPDL
        for (var i = 0; i < self.newHangHoa().DinhLuongDichVu().length; i++) {
            if (self.newHangHoa().DinhLuongDichVu()[i].ID_DonViQuiDoi === item.ID_DonViQuiDoi) {
                self.newHangHoa().DinhLuongDichVu()[i].SoLuongQuyCach = soluongQuyCach;
                self.newHangHoa().DinhLuongDichVu()[i].SoLuong = soluongNhap;
                break;
            }
        }

        $(thisObj).parent().prev().find('input').val(formatNumber3Digit(soluongQuyCach, 3));
        var keycode = event.keyCode || event.which;
        if (keycode === 13) {
            let tr = $(thisObj).closest('tr').next();
            tr.children('td').eq(3).find('input').focus().select();
        }
    };

    self.booleanAddViTri = ko.observable(true);
    self.AddNewViTri = function () {
        var _tenViTri = $('#txtTenViTriHH').val();
        var _id = self.IDViTriEdit();
        if (_tenViTri === null || _tenViTri === "" || _tenViTri === "undefined") {
            ShowMessage_Danger("Vui lòng nhập vị trí hàng hóa");
            $('#txtTenViTriHH').focus();
            return false;
        }

        var myData = {};
        var objViTri = {
            ID: _id,
            TenViTri: _tenViTri,
            NguoiTao: _txtTenTaiKhoan
        };
        myData.objDMHHViTri = objViTri;
        if (self.booleanAddViTri() === true) {
            $.ajax({
                data: myData,
                url: DMHangHoaUri + "POST_DM_HangHoa_ViTri",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (item) {
                    self.MangNhomViTriHH.push(item);
                    self.ArrViTriHH.push(item);
                    $('#choose_ViTri input').remove();
                    $('#selec-all-ViTri li').each(function () {
                        if ($(this).attr('id') === item.ID) {
                            $(this).find('.fa-check').remove();
                            $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>');
                        }
                    });
                    ShowMessage_Success("Thêm mới vị trí thành công!");
                },
                statusCode: {
                    404: function () {
                        self.error("page not found");
                    },
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    ShowMessage_Danger("Thêm mới vị trí thất bại!");
                },
                complete: function () {
                }
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                });
        }
        else {
            myData.objDMHHViTri.NguoiSua = _txtTenTaiKhoan;
            $.ajax({
                data: myData,
                url: DMHangHoaUri + "PUT_DM_HangHoa_ViTri",
                type: 'PUT',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (item) {
                    self.MangNhomViTriHH()[0].TenViTri = item.TenViTri;
                    self.ArrViTriHH().filter(p => p.ID === item.ID)[0].TenViTri = item.TenViTri;
                    self.MangNhomViTriHH.refresh();
                    self.ArrViTriHH.refresh();

                    $('#selec-all-ViTri li').each(function () {
                        if ($(this).attr('id') === item.ID) {
                            $(this).find('.fa-check').remove();
                            $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>');
                        }
                    });
                    ShowMessage_Success("Cập nhật vị trí thành công!");
                },
                statusCode: {
                    404: function () {
                        self.error("page not found");
                    },
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    ShowMessage_Danger("Cập nhật vị trí thất bại!");
                },
                complete: function () {
                }
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                });
        }
    }

    self.MangNhomViTriHH = ko.observableArray();
    self.selectedViTri = function (item) {
        var arrVT = [];
        for (var i = 0; i < self.MangNhomViTriHH().length; i++) {
            if ($.inArray(self.MangNhomViTriHH()[i], arrVT) === -1) {
                arrVT.push(self.MangNhomViTriHH()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrVT) === -1) {
            self.MangNhomViTriHH.push(item);
        }
        //$('#choose_DonVi input').remove();
        // thêm dấu check vào đối tượng được chọn
        $('#selec-all-ViTri li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
            }
        });
        $('#choose_ViTri input').remove();
    }

    self.CloseViTriHH = function (item) {
        self.MangNhomViTriHH.remove(item);
        if (self.MangNhomViTriHH().length === 0) {
            $('#choose_ViTri').append('<input type="text" id="dllViTriHH"  placeholder="Chọn vị trí hàng hóa">');
        }
        // remove check
        $('#selec-all-ViTri li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }
    self.MangNhomViTriHHComputed = ko.computed(function () {
        if (self.MangNhomViTriHH().length == 1) {
            $('.outselect-width').css('padding-right', '84px');
        }
        else {
            $('.outselect-width').css('padding-right', '34px');
        }
    });
    self.addNhomHangHoa = function (formElement) {
        document.getElementById("btnLuuNhomHangHoa").disabled = true;
        document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Đang lưu";
        var _idNhomHH = self.newNhomHangHoa().ID();
        var _tenNhomHH = self.newNhomHangHoa().TenNhomHangHoa();
        var _lanhomHangHoa = self.newNhomHangHoa().LaNhomHangHoa();

        var _idParent = null;
        if (self._ThemMoiNhomHH() === true) {
            _idParent = self.selectIDParent();
        }
        if (self._ThemMoiNhomHH() === false) {
            _idParent = self.selectIDParent();
        }
        if (_tenNhomHH === null || _tenNhomHH === "" || _tenNhomHH === "undefined" || _tenNhomHH === undefined) {
            ShowMessage_Danger("Không được để trống tên nhóm hàng!");
            $('#txtTenNhom1').focus();
            document.getElementById("btnLuuNhomHangHoa").disabled = false;
            document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
            return false;
        }
        else {
            _tenNhomHH = _tenNhomHH.replace(/\s{2,}/g, ' ');
        }
        var objNhomHH = {
            ID: _idNhomHH,
            ID_Parent: _idParent,
            TenNhomHangHoa: _tenNhomHH,
            NguoiTao: _txtTenTaiKhoan,
            NguoiSua: _txtTenTaiKhoan,
            LaNhomHangHoa: _lanhomHangHoa
        };
        if (self._ThemMoiNhomHH() === true) {
            ajaxHelper(NhomHHUri + "Check_TenNhomHangHoaExist?tenNhomHang=" + _tenNhomHH, 'POST').done(function (data) {
                if (data) {
                    ShowMessage_Danger("Tên nhóm hàng đã tồn tại!");
                    document.getElementById("btnLuuNhomHangHoa").disabled = false;
                    document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
                    return false;
                }
                else {
                    $.ajax({
                        url: NhomHHUri + "PostDM_NhomHangHoa",
                        type: 'POST',
                        dataType: 'json',
                        data: objNhomHH ? JSON.stringify(objNhomHH) : null,
                        contentType: 'application/json',
                        success: function (item) {
                            self.ThemMoiNhomHangHoaLS(item.TenNhomHangHoa);
                            item.Childs = [];
                            item.Childs.Child2s = [];
                            self.NhomHangHoas.push(item);
                            GetAllNhomHH();
                            self.NhomHangHoasFilter.push(item);
                            self.selectIDNhomHHAddHH(item.ID);
                            $('#choose_TenNHHAddHH').text(item.TenNhomHangHoa);
                            $(function () {
                                $('span[id=spanCheckNhomAddHH_' + item.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
                            });
                            $(function () {
                                $('span[id=spanCheckNhomAddDV_' + item.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
                            });
                            ShowMessage_Success("Thêm nhóm hàng hóa thành công!");
                            document.getElementById("btnLuuNhomHangHoa").disabled = false;
                            document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
                            if (self.booleanAdd() !== true) {
                                SearchHangHoa();
                            }
                            $(".op-js-modal").modal('hide');
                            $(".modal-ontop").hide();
                        },
                        statusCode: {
                            404: function () {
                                document.getElementById("btnLuuNhomHangHoa").disabled = false;
                                document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
                                self.error("page not found");
                            },
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            document.getElementById("btnLuuNhomHangHoa").disabled = false;
                            document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
                            self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                            $(".op-js-modal").moddal('hide');
                            $(".modal-ontop").hide();
                            ShowMessage_Danger("Thêm nhóm hàng hóa thất bại!");
                        },
                        complete: function () {
                            // window.location.href = '/Student/Index/';
                            //$("#modalPopup_AddNhomHHDV").modal("hide");
                            $(".op-js-modal").modal('hide');
                            $(".modal-ontop").hide();
                            self.resetNhomHangHoa();
                        }
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                            document.getElementById("btnLuuNhomHangHoa").disabled = false;
                            document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
                            self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                            $(".op-js-modal").modal('hide');
                            $(".modal-ontop").hide();
                        });
                }
            });
        }
        else {
            ajaxHelper(NhomHHUri + "Check_TenNhomHangHoaExistEdit?tenNhomHang=" + _tenNhomHH + '&idnhomhh=' + _idNhomHH, 'POST').done(function (data) {
                if (data) {
                    ShowMessage_Danger("Tên nhóm hàng đã tồn tại!");
                    document.getElementById("btnLuuNhomHangHoa").disabled = false;
                    document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
                    return false;
                }
                else {
                    var myData = {};
                    myData.id = _idNhomHH;
                    myData.dM_NhomHangHoa = objNhomHH;
                    $.ajax({
                        url: NhomHHUri + "PutDM_NhomHangHoa",
                        type: 'PUT',
                        dataType: 'json',
                        //contentType: 'application/json',
                        // data: book ? JSON.stringify(book) : null,
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                        data: myData,
                        success: function () {
                            self.SuaNhomHangHoaLS(self.newNhomHangHoa().TenNhomHangHoa());
                            GetAllNhomHH();
                            objNhomHH.Childs = [];
                            objNhomHH.Childs.Child2s = [];
                            //self.NhomHangHoas.push(objNhomHH);
                            self.NhomHangHoasFilter.push(objNhomHH);
                            self._ThemMoiNhomHH(true);
                            SearchHangHoa();
                            ShowMessage_Success("Cập nhật nhóm hàng hóa thành công!");
                        },
                        statusCode: {
                            404: function () {
                                document.getElementById("btnLuuNhomHangHoa").disabled = false;
                                document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
                                self.error("page not found");
                            },
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            document.getElementById("btnLuuNhomHangHoa").disabled = false;
                            document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
                            self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                            ShowMessage_Danger("Cập nhật nhóm hàng hóa thất bại");
                        },
                        complete: function () {
                            // window.location.href = '/Student/Index/';
                            $("#modalPopup_NhomHHDV").modal("hide");
                            self.resetNhomHangHoa();
                        }
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                            document.getElementById("btnLuuNhomHangHoa").disabled = false;
                            document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
                            self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                        });
                }
            });
        }
    }
    self.addNhomHangHoaByLaNHomHH = function (formElement) {
        document.getElementById("btnLuuNhomHangHoa").disabled = true;
        document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Đang lưu";
        var _idNhomHH = self.newNhomHangHoa().ID();
        var _tenNhomHH = self.newNhomHangHoa().TenNhomHangHoa();
        var _lanhomHangHoa = self.newNhomHangHoa().LaNhomHangHoa();

        var _idParent = null;
        if (self._ThemMoiNhomHH() == true) {
            _idParent = self.selectIDParent();
        }
        if (self._ThemMoiNhomHH() == false) {
            _idParent = self.selectIDParent();
        }
        if (_tenNhomHH === null || _tenNhomHH === "" || _tenNhomHH === "undefined" || _tenNhomHH === undefined) {
            ShowMessage_Danger("Không được để trống tên nhóm hàng!");
            $('#txtTenNhom2').focus();
            document.getElementById("btnLuuNhomHangHoa").disabled = false;
            document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
            return false;
        }
        else {
            _tenNhomHH = _tenNhomHH.replace(/\s{2,}/g, ' ');
        }
        var objNhomHH = {
            ID: _idNhomHH,
            ID_Parent: _idParent,
            TenNhomHangHoa: _tenNhomHH,
            NguoiTao: _txtTenTaiKhoan,
            NguoiSua: _txtTenTaiKhoan,
            LaNhomHangHoa: _lanhomHangHoa
        };
        if (self._ThemMoiNhomHH() === true) {
            ajaxHelper(NhomHHUri + "Check_TenNhomHangHoaExist?tenNhomHang=" + _tenNhomHH, 'POST').done(function (data) {
                if (data) {
                    ShowMessage_Danger("Tên nhóm hàng đã tồn tại!");
                    document.getElementById("btnLuuNhomHangHoa").disabled = false;
                    document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
                    return false;
                }
                else {
                    $.ajax({
                        url: NhomHHUri + "PostDM_NhomHangHoa",
                        type: 'POST',
                        dataType: 'json',
                        data: objNhomHH ? JSON.stringify(objNhomHH) : null,
                        contentType: 'application/json',
                        success: function (item) {
                            GetAllNhomHH();
                            self.ThemMoiNhomHangHoaLS(item.TenNhomHangHoa);
                            item.Childs = [];
                            item.Childs.Child2s = [];
                            GetAllNhomHHByLaNhomHH();
                            self.selectIDNhomHHAddHH(item.ID);
                            $('#choose_TenNHHAddHH').text(item.TenNhomHangHoa);
                            $('#choose_TenNHHAddDV').text(item.TenNhomHangHoa);
                            $(function () {
                                $('span[id=spanCheckNhomAddHH_' + item.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
                            });
                            $(function () {
                                $('span[id=spanCheckNhomAddDV_' + item.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
                            });
                            ShowMessage_Success("Thêm nhóm hàng hóa thành công!");
                            document.getElementById("btnLuuNhomHangHoa").disabled = false;
                            document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
                            if (self.booleanAdd() !== true) {
                                SearchHangHoa();
                            }
                            $(".op-js-modallnhomhh").modal('hide');
                            $(".modal-ontop").hide();
                        },
                        statusCode: {
                            404: function () {
                                document.getElementById("btnLuuNhomHangHoa").disabled = false;
                                document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
                                self.error("page not found");
                            },
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            document.getElementById("btnLuuNhomHangHoa").disabled = false;
                            document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
                            self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                            $(".op-js-modallnhomhh").modal('hide');
                            $(".modal-ontop").hide();
                            ShowMessage_Danger("Thêm nhóm hàng hóa thất bại!");
                        },
                        complete: function () {
                            // window.location.href = '/Student/Index/';
                            //$("#modalPopup_AddNhomHHDV").modal("hide");
                            $(".op-js-modallnhomhh").modal('hide');
                            $(".modal-ontop").hide();
                        }
                    })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                            document.getElementById("btnLuuNhomHangHoa").disabled = false;
                            document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
                            self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                            $(".op-js-modallnhomhh").modal('hide');
                            $(".modal-ontop").hide();
                        });
                }
            });
        }
        else {
            var myData = {};
            myData.id = _idNhomHH;
            myData.dM_NhomHangHoa = objNhomHH;
            $.ajax({
                url: NhomHHUri + "PutDM_NhomHangHoa",
                type: 'PUT',
                dataType: 'json',
                //contentType: 'application/json',
                // data: book ? JSON.stringify(book) : null,
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: myData,
                success: function () {
                    self.SuaNhomHangHoaLS(self.newNhomHangHoa().TenNhomHangHoa());
                    GetAllNhomHHByLaNhomHH();
                    objNhomHH.Childs = [];
                    objNhomHH.Childs.Child2s = [];
                    //self.NhomHangHoas.push(objNhomHH);
                    self.NhomHangHoasFilter.push(objNhomHH);
                    self._ThemMoiNhomHH(true);
                    SearchHangHoa();
                    ShowMessage_Success("Cập nhật nhóm hàng hóa thành công!");
                },
                statusCode: {
                    404: function () {
                        document.getElementById("btnLuuNhomHangHoa").disabled = false;
                        document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
                        self.error("page not found");
                    },
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    document.getElementById("btnLuuNhomHangHoa").disabled = false;
                    document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    ShowMessage_Danger("Cập nhật nhóm hàng hóa thất bại");
                },
                complete: function () {
                    // window.location.href = '/Student/Index/';
                    $("#modalPopup_NhomHHDV").modal("hide");
                    self.resetNhomHangHoa();
                }
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    document.getElementById("btnLuuNhomHangHoa").disabled = false;
                    document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                });
        }
    }

    self.NhomHH_NotChil4 = ko.observableArray();

    self.TenNhomHHLS = ko.observable();
    self.editNhomHangHoa = function (item) {
        self.CheckLocNhomHangCap3(true);
        if (!self.role_UpdateProduct()) {
            ShowMessage_Danger('Không có quyền cập nhật nhóm hàng');
            return;
        }
        vmThemNhomHang_NangCao.showModalUpdate(item.ID);
        return;
        ajaxHelper(NhomHHUri + "GetDM_NhomHangHoa/" + item.ID, 'GET').done(function (data) {
            if (data.LaNhomHangHoa === true) {
                $('#txtEditNhomHangHoa').html("Sửa nhóm hàng hóa");
            }
            else {
                $('#txtEditNhomHangHoa').html("Sửa nhóm dịch vụ");
            }
            self.newNhomHangHoa().LaNhomHangHoa(data.LaNhomHangHoa);
            GetAllNhomHHByLaNhomHH();
            var arrnhomHH = [];
            setTimeout(function () {
                self.NhomHH_NotChil4([]);
                for (var i = 0; i < self.NhomHHSuaByLoai().length; i++) {
                    if (self.NhomHHSuaByLoai()[i].ID !== data.ID) {
                        arrnhomHH.push(self.NhomHHSuaByLoai()[i]);
                    }
                }
                for (var j = 0; j < arrnhomHH.length; j++) {
                    if (arrnhomHH[j].ID_Parent == data.ID) {
                        arrnhomHH.splice(j, 1);
                    }
                }

                var arrPr_NotChild3 = [];
                for (var i = 0; i < arrnhomHH.length; i++) {
                    if ($.inArray(arrnhomHH[i].ID, arrChild3) === -1) {
                        arrPr_NotChild3.push(arrnhomHH[i]);
                    }
                }
                for (var i = 0; i < arrPr_NotChild3.length; i++) {
                    if (arrPr_NotChild3[i].ID_Parent == null) {
                        var objParent = {
                            ID: arrPr_NotChild3[i].ID,
                            TenNhomHangHoa: arrPr_NotChild3[i].TenNhomHangHoa,
                            Childs: [],
                        }

                        for (var j = 0; j < arrPr_NotChild3.length; j++) {
                            if (arrPr_NotChild3[j].ID !== arrPr_NotChild3[i].ID && arrPr_NotChild3[j].ID_Parent === arrPr_NotChild3[i].ID) {
                                var objChild =
                                {
                                    ID: arrPr_NotChild3[j].ID,
                                    TenNhomHangHoa: arrPr_NotChild3[j].TenNhomHangHoa,
                                    ID_Parent: arrPr_NotChild3[i].ID,
                                    Child2s: []
                                };
                                for (var k = 0; k < arrPr_NotChild3.length; k++) {
                                    if (arrPr_NotChild3[k].ID_Parent !== null && arrPr_NotChild3[k].ID_Parent === arrPr_NotChild3[j].ID) {
                                        var objChild2 =
                                        {
                                            ID: arrPr_NotChild3[k].ID,
                                            TenNhomHangHoa: arrPr_NotChild3[k].TenNhomHangHoa,
                                            ID_Parent: arrPr_NotChild3[j].ID,
                                        };
                                        objChild.Child2s.push(objChild2);
                                        arrChild3.push(arrPr_NotChild3[k].ID);
                                    }
                                }
                                objParent.Childs.push(objChild);
                            }
                        }
                        self.NhomHH_NotChil4.push(objParent);
                    }
                }
                $('#lstNhomHangSua span').each(function () {
                    $(this).empty();
                });
                $(function () {
                    $('span[id=spanCheckNhomSua_' + data.ID_Parent + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
                });

            }, 1000);
            $('#choose_TenNHHSua').text(data.TenNhomHangHoaCha);
            self.selectIDParent(data.ID_Parent);
            self._ThemMoiNhomHH(false);
            self.newNhomHangHoa().setdata(data);
            self.TenNhomHHLS(data.TenNhomHangHoa)
        });
        $('#modalPopup_NhomHHDV').modal('show');
        $('#modalPopup_NhomHHDV').on('shown.bs.modal', function () {
            $('#txtTenNhom').select();
        })
    }

    //self.format = function () {
    //    formatNumberObj($('#txtGiaBan'));
    //    formatNumberObj($('#txtGiaBanDV'));
    //}

    self.DM_HangHoa_Anh = ko.observableArray();
    self.IDCungLoai = ko.observable('00000000-0000-0000-0000-000000000000');
    self.ItemThuocTinh = ko.observableArray();
    self.LaQuanLyTheoLo = ko.observable(false);


    self.ThuocTinhCuaHHEdit = ko.observableArray();

    self.shouldShowTitleTTEdit = ko.computed(function () {
        if (self.ThuocTinhCuaHHEdit() !== null && self.ThuocTinhCuaHHEdit() !== undefined && self.ThuocTinhCuaHHEdit().length > 0)
            return true;
        else
            return false;
    }, this);
    self.CheckLaChaCungLoai = ko.observable();

    self.BackAdd = function () {
        $('#modalPopuplg_HHNew').hide();
        $('#modalPopuplg_DVNew').hide();
        $(".btn-tro-ve").hide();
        $('.addHHH').show();
        $("#table-reponsive").find('.op-js-tr-hide').each(function () {
            if ($(this).css('display') !== 'none') {
                $(this).prev('.prev-tr-hide').find('td').each(function () {
                    $(this).removeClass("bg-gray");
                    $(this).removeClass("bor-right");
                })

            }
        })
        $(".op-js-tr-hide").removeClass("ac");
        $('.line-right').height(0).css("margin-top", "0px");
    }
    function changebuttonaddnew() {

        $(".btn-tro-ve").show();
        $('.addHHH').hide();
        $("#table-reponsive").find('.op-js-tr-hide').each(function () {
            if ($(this).css('display') !== 'none') {
                $(this).prev('.prev-tr-hide').find('td').each(function () {
                    $(this).removeClass("bg-gray");
                    $(this).removeClass("bor-right");
                })

            }
        })
        $(".op-js-tr-hide").removeClass("ac");
        $('.line-right').height(0).css("margin-top", "0px");
        //SearchHangHoa();
    }

    self.ChangeCheckQuanLyBaoDuong = function () {
        self.BaoDuong_ApplyAllByGroup(false);
        if (!self.booleanAdd() && !self.QuanLyBaoDuong()) {
            dialogConfirm('Xác nhận', 'Bạn có muốn hủy <b>Quản lý bảo dưỡng </b> cho toàn bộ sản phẩm trong nhóm không?', function () {
                self.BaoDuong_ApplyAllByGroup(true);
            })
        }
    }

    function SetQuanLyBaoDuong(item) {
        let qlBduong = item.QuanLyBaoDuong;
        let loaiBduong = item.LoaiBaoDuong;
        let tinhHoaHongTruocCK = item.HoaHongTruocChietKhau;
        let thuocDM_Xe = !commonStatisJs.CheckNull(item.ID_Xe) && item.ID_Xe !== const_GuidEmpty;
        if (commonStatisJs.CheckNull(tinhHoaHongTruocCK)) {
            tinhHoaHongTruocCK = 0;
        }
        if (commonStatisJs.CheckNull(qlBduong)) {
            qlBduong = 0;
        }
        if (commonStatisJs.CheckNull(loaiBduong)) {
            loaiBduong = 2;
        }
        if (loaiBduong === 0) {
            loaiBduong = 2;
        }
        self.HangHoa_LaXe(thuocDM_Xe);
        self.TinhHoaHongTruocCK(tinhHoaHongTruocCK === 1);
        self.QuanLyBaoDuong(qlBduong === 1);
        self.BaoDuong_Type(loaiBduong);
        self.BaoDuong_ApplyAllByGroup(false);
        self.newHangHoa().ID(item.ID);
        $('jqauto-car ._jsInput').val(item.BienSo);

        self.Role_UpdateCar(self.Role_UpdateCar() && thuocDM_Xe);
        self.Role_AddCar(!self.Role_UpdateCar());
    }
    self.editHH = function (item) {
        $('.modal-backdrop in').css('display', 'none');
        SetQuanLyBaoDuong(item);
        self.tenNhomHangChosed(item.NhomHangHoa);
        self.productOld(item);

        self.DM_HangHoa_Anh([]);
        self.ThuocTinhCuaHHEdit([]);
        self.files([]);
        $('.errorAnh').text("");
        $('.errorAnhHH').text("");

        self.getCTHH(undefined);
        self.getCTHHHH(undefined);
        self.CheckThemHHCL(false);
        ajaxHelper(DMHangHoaUri + 'Check_HangHoaLaThanhPhanDichVu?idhanghoa=' + item.ID, 'POST').done(function (dataCheckTP) {
            self.LaQuanLyTheoLo(item.QuanLyTheoLoHang);
            if (item.LaHangHoa === false) {
                self.newNhomHangHoa().LaNhomHangHoa(false);
            }
            else {
                self.newNhomHangHoa().LaNhomHangHoa(true);
            }
            GetAllNhomHHByLaNhomHH();
            $('.themdonviclick1').show();
            $('.hidexoathuoctinh').show();
            $('#danhsachhanghoacungloai').hide();
            ajaxHelper(DMHangHoaUri + "GetDM_HangHoa?id=" + item.ID + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
                self.IDCungLoai(data.ID_HangHoaCungLoai);
                if (data.DM_HangHoa_Anh.length === 0) {
                    self.loadavt(false);
                } else {
                    self.loadavt(true);
                }
                self.DM_HangHoa_Anh(data.DM_HangHoa_Anh);
                self.ThuocTinhCuaHHEdit(data.HangHoa_ThuocTinh);

                switch (data.LoaiHangHoa) {
                    case 1:
                    case 2:
                        modelTypeSearchProduct.TypeSearch(1);// hh
                        break;
                    case 3:
                        modelTypeSearchProduct.TypeSearch(4);// hh  + dv
                        break;
                }

                if (data.LaHangHoa === true) {
                    if (data.DonViTinhChuan !== "") {
                        $('#hideDonViTinh').show();
                        $('#checkmanageDVT').prop('checked', true);
                    }
                    else {
                        $('#hideDonViTinh').hide();
                        $('#checkmanageDVT').prop('checked', false);
                    }

                    if (data.HangHoa_ThuocTinh.length > 0) {
                        $('#checkmanageThuocTinh').prop('checked', true);
                        $('#sanphamcothuoctinh').show();
                    }
                    else {
                        $('#checkmanageThuocTinh').prop('checked', false);
                        $('#sanphamcothuoctinh').hide();
                    }
                }
                else {
                    if (data.DonViTinhChuan !== "") {
                        $('#hideDonViTinhDV').show();
                        $('#checkmanageDVTDV').prop('checked', true);
                    }
                    else {
                        $('#hideDonViTinhDV').hide();
                        $('#checkmanageDVTDV').prop('checked', false);
                    }
                }

                data.GiaBan = formatNumber3Digit(data.GiaBan);
                data.TonKho = formatNumber3Digit(data.TonKho);
                data.SoPhutThucHien = data.SoPhutThucHien !== null ? formatNumber3Digit(data.SoPhutThucHien) : data.SoPhutThucHien;
                data.ChiPhiThucHien = formatNumber3Digit(data.ChiPhiThucHien);
                for (var j = 0; j < data.DonViTinh.length; j++) {
                    data.DonViTinh[j].GiaBan = formatNumber3Digit(data.DonViTinh[j].GiaBan);
                }
                data.HangHoaCungLoaiArr = [];
                self.newHangHoa().SetData(data);

                if (data.LaHangHoa === true) {
                    self.selectIDNhomHHAddHH(data.ID_NhomHangHoa);
                    if (data.ID_NhomHangHoa !== null) {
                        $('#choose_TenNHHAddHH').text(data.TenNhomHangHoa);
                        $(function () {
                            $('span[id=spanCheckNhomAddHH_' + data.ID_NhomHangHoa + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
                        });
                    }
                    else {
                        $('#choose_TenNHHAddHH').text("---Chọn nhóm---");
                    }
                    $('#lstNhomHangAddHH span').each(function () {
                        $(this).empty();
                    });
                }
                else {
                    $('#txtGhiChuDV').val(data.GhiChu);
                    self.selectIDNhomHHAddHH(data.ID_NhomHangHoa);
                    if (data.TenNhomHangHoa !== "") {
                        $('#choose_TenNHHAddDV').text(data.TenNhomHangHoa);
                    }
                    else {
                        $('#choose_TenNHHAddDV').text("---Chọn nhóm---");
                    }
                    $('#lstNhomHangAddDV span').each(function () {
                        $(this).empty();
                    });
                }
                $(function () {
                    $('span[id=spanCheckNhomAddHH_' + data.ID_NhomHangHoa + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
                });
                $(function () {
                    $('span[id=spanCheckNhomAddDV_' + data.ID_NhomHangHoa + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
                });
                if (data.ThoiGianBaoHanh === null) {
                    data.LoaiBaoHanh = 1;
                }
                self.selectedLoaiThoiGianBH(data.LoaiBaoHanh);
                self.MangNhomViTriHH(data.DM_HangHoa_ViTri);
                $('#choose_ViTri input').remove();
                if (self.MangNhomViTriHH().length === 0) {
                    $('#choose_ViTri').append('<input type="text" id="dllViTriHH" placeholder="Chọn vị trí hàng hóa">');
                    $('#selec-all-ViTri li').each(function () {
                        $(this).find('.fa-check').remove();
                    });
                }
                else {
                    for (var i = 0; i < self.MangNhomViTriHH().length; i++) {
                        $('#selec-all-ViTri li').each(function () {
                            if ($(this).attr('id') === self.MangNhomViTriHH()[i].ID) {
                                $(this).find('.fa-check').remove();
                                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                            }
                        });
                    }
                }

                let lstTP = self.ListThanhPhans();
                let sumGV = lstTP.reduce(function (x, tp) {
                    return x + tp.ThanhTien;
                }, 0);
                self.newHangHoa().DinhLuongDichVu(lstTP);
                self.TPDL_SumTienVon(sumGV);

                var idView = '';
                if (data.LaHangHoa === true) {
                    $('#modalPopuplg_HHNew').show();
                    idView = '#modalPopuplg_HHNew';
                    changebuttonaddnew();
                    $('#txtTenHangHoa1').select();
                    var lc_CTThietLap = JSON.parse(localStorage.getItem('lc_CTThietLap'));
                    if (lc_CTThietLap.LoHang === false || lc_CTThietLap.LoHang === null) {
                        $('#QuanLyTheoLoHangCheck').hide();
                    }
                    else {
                        if (dataCheckTP === false) {
                            $('#QuanLyTheoLoHangCheck').show();
                        }
                        else {
                            $('#QuanLyTheoLoHangCheck').hide();
                        }
                    }
                    $('.updateHHTab li').each(function () {
                        $(this).removeClass('active');
                    });
                } else {
                    idView = '#modalPopuplg_DVNew';
                    showdichvu();
                }
                SetDefaultActiveTab0($('' + idView));

                self.booleanAdd(false);
                if (item.QuanLyTheoLoHang === true) {
                    $('.dshh3').hide();
                    $('.checktonkholohanghoa').hide();
                }
                else {
                    $('.dshh3').show();
                    $('.checktonkholohanghoa').show();
                }
                self.CheckLaChaCungLoai(data.LaChaCungLoai);
                if (data.LaChaCungLoai === false) {
                    $('.ddlThuocTinh').attr('disabled', 'disabled');
                }

                if (self.ThuocTinhCuaHHEdit().length > 0) {
                    $('.btn-them-luu').show();
                } else {
                    $('.btn-them-luu').hide();
                }
            });
        });
    }

    // Reset
    self.resetNhomHangHoa = function () {
        self.newNhomHangHoa(new FormModel_NhomHHDV());
    }
    // Cancel
    self.cancel_editNhomHangHoa = function () {
    }

    // hangf hoas 
    self.loadavt = ko.observable(true);

    function SetDefaultBaoDuong() {
        self.BaoDuong_ApplyAllByGroup(false);
        self.BaoDuong_ListDetail([]);
        var obj = {
            ID: const_GuidEmpty,
            LanBaoDuong: 1,
            GiaTri: 0,
            LoaiThoiGian: 4,
            LapDinhKy: false,
        }
        self.BaoDuong_ListDetail.push(obj);
    }

    function SetDefaul_ofProduct() {
        self.CheckLocNhomHangCap3(false);
        self.QuanLyBaoDuong(false);
        self.LaHangHoaNha(true);
        self.TinhHoaHongTruocCK(false);
        self.HangHoa_LaXe(false);
        self.Role_AddCar(true);
        self.Role_UpdateCar(true);
        $('jqauto-car ._jsInput').val('');
    }

    self.themmoicapnhat = function () {
        self.tenNhomHangChosed('Nhóm hàng hóa mặc định');
        SetDefaultActiveTab0($('#modalPopuplg_HHNew'));
        self.Tab_Active(0);
        self.productOld({});
        SetDefaultBaoDuong();
        SetDefaul_ofProduct();

        modelTypeSearchProduct.TypeSearch(1);// hh

        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('HangHoa_ThemMoi', lc_CTQuyen) > -1) {
            ajaxHelper(DMHangHoaUri + 'GioiHanSoMatHang', 'GET').done(function (data) {
                if (data) {
                    ShowMessage_Danger("Cửa hàng đã đạt số mặt hàng quy định, không thể thêm mới");
                }
                else {
                    self.DM_HangHoa_Anh([]);
                    self.ThuocTinhCuaHH([]);
                    self.ThuocTinhCuaHHEdit([]);
                    self.files([]);
                    self.IDCungLoai(undefined);
                    checkThemCL = 2;
                    self.CheckThemHHCL(false);
                    self._ThemMoiNhomHH(true);
                    $('.btn-them-luu').hide();
                    $('.themhanghoa').text("Thêm hàng hóa");
                    $('.themdonviclick1').show();
                    $('.hidexoathuoctinh').show();
                    self.loadavt(false);
                    $('.errorAnhHH').text("");
                    self.booleanAdd(true);
                    $('.dshh3').show();
                    $('.checktonkholohanghoa').show();
                    //check đơn vị tính và quản lý thuộc tính hidden
                    $('#checkmanageDVT').prop('checked', false);
                    $('#checkmanageThuocTinh').prop('checked', false);
                    $('#hideDonViTinh').hide();
                    $('#sanphamcothuoctinh').hide();
                    $('#danhsachhanghoacungloai').hide();
                    //-----
                    self.newNhomHangHoa().LaNhomHangHoa(true);
                    GetAllNhomHHByLaNhomHH();
                    self.reset_HangHoaDichVu();
                    self.newHangHoa().LoaiHangHoa(1);
                    self.selectIDNhomHHAddHH(null);
                    self.selectedLoaiThoiGianBH("1");
                    self.MangNhomViTriHH([]);
                    $('#choose_ViTri input').remove();
                    $('#choose_ViTri').append('<input type="text" id="dllViTriHH" placeholder="Chọn vị trí hàng hóa">');

                    $('#selec-all-ViTri li').each(function () {
                        $(this).find('.fa-check').remove();
                    });

                    $('#lstNhomHangAddHH span').each(function () {
                        $(this).empty();
                    });

                    $('#choose_TenNHHAddHH').text("---Chọn nhóm---");
                    $('#lstNhomHangAddHH span').each(function () {
                        $(this).empty();
                    });
                    $('#lstNhomHangAddDV span').each(function () {
                        $(this).empty();
                    });
                    self.TPDL_SumTienVon(0);
                    $('#modalPopuplg_HHNew').show();
                    changebuttonaddnew();
                    $('.modal-backdrop in').hide();
                    self.getCTHHHH(undefined);
                    $('.updateHHTab li').each(function () {
                        $(this).removeClass('active');
                    });
                    $('#txtTenHangHoa1').focus();
                    var lc_CTThietLap = JSON.parse(localStorage.getItem('lc_CTThietLap'));
                    if (lc_CTThietLap.LoHang === false || lc_CTThietLap.LoHang === null) {
                        $('#QuanLyTheoLoHangCheck').hide();
                    }
                    else {
                        $('#QuanLyTheoLoHangCheck').show();
                    }
                }
            });
        }
    };

    self.backCurrentHH = function () {
        self.BackAdd();
    };

    self.backCurrentDV = function () {
        self.BackAdd();
    };

    self.showModalNhomHang = function () {
        vmThemNhomHang_NangCao.showModal();
    }

    $(".op-js-themmoinhomhang").click(function () {
        self.CheckLocNhomHangCap3(true);
        self.newNhomHangHoa().LaNhomHangHoa(true);
        self._ThemMoiNhomHH(true);
        self.selectIDParent(null);
        GetAllNhomHHByLaNhomHH();
        $(".modal-ontop").show();
        $(".op-js-modal").modal('show');
        $('#txtTenNhom1').val("");
        $('#txtTenNhom1').focus();
        $('#choose_TenNHH').text('---Chọn nhóm---');
        $('#lstNhomHang span').each(function () {
            $(this).empty();
        });
        document.getElementById("btnLuuNhomHangHoa").disabled = false;
        document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
    });

    $(".op-js-themmoi").click(function () {
        self.CheckLocNhomHangCap3(true);
        self.selectIDParent(null);
        self._ThemMoiNhomHH(true);
        GetAllNhomHHByLaNhomHH();
        $(".modal-ontop").show();
        $(".op-js-modallnhomhh").modal('show');
        $('#txtTenNhom2').val("");
        $('#txtTenNhom2').focus();
        $('#choose_TenNHHLHH').text('---Chọn nhóm---');
        $('#lstNhomHang span').each(function () {
            $(this).empty();
        });
        document.getElementById("btnLuuNhomHangHoa").disabled = false;
        document.getElementById("btnLuuNhomHangHoa").lastChild.data = " Lưu";
    });

    self.showPopupAddViTri = function () {
        $('#modalViTriKho').modal('show');
        $('#modalViTriKho').on('shown.bs.modal', function () {
            self.IDViTriEdit(undefined);
            $('#txtTenViTriHH').val("");
            $('#txtTenViTriHH').select();
        });
        self.booleanAddViTri(true);
    };

    self.editViTri = function () {
        $('#modalViTriKho').modal('show');
        self.booleanAddViTri(false);
        $('#modalViTriKho').on('shown.bs.modal', function () {
            $('#txtTenViTriHH').val(self.MangNhomViTriHH()[0].TenViTri);
            self.IDViTriEdit(self.MangNhomViTriHH()[0].ID);
            $('#txtTenViTriHH').select();
        });
    };
    function showdichvu() {
        $('#modalPopuplg_HHNew').hide();
        $('#modalPopuplg_DVNew').show();
        $(".btn-tro-ve").show();
        $('.addHHH').hide();
        $("#table-reponsive").find('.op-js-tr-hide').each(function () {
            if ($(this).css('display') !== 'none') {
                $(this).prev('.prev-tr-hide').find('td').each(function () {
                    $(this).removeClass("bg-gray");
                    $(this).removeClass("bor-right");
                })

            }
        })
        $(".op-js-tr-hide").removeClass("ac");
        $('.line-right').height(0).css("margin-top", "0px");
    }
    // type: 2.dichvu, 3.combo
    self.themmoicapnhatdichvu = function (type = 2) {
        switch (type) {
            case 2:
                modelTypeSearchProduct.TypeSearch(1);// hh 
                break;
            case 3:
                modelTypeSearchProduct.TypeSearch(4);// hh + dv (not combo)
                break;
        }

        self.Tab_Active(0);
        $('#txtGhiChuDV').val('');
        self.productOld({});
        SetDefaultBaoDuong();
        SetDefaul_ofProduct();
        SetDefaultActiveTab0($('#modalPopuplg_DVNew'));

        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('HangHoa_ThemMoi', lc_CTQuyen) > -1) {
            ajaxHelper(DMHangHoaUri + 'GioiHanSoMatHang', 'GET').done(function (data) {
                if (data) {
                    ShowMessage_Danger("Cửa hàng đã đạt số mặt hàng quy định, không thể thêm mới");
                }
                else {
                    self.DM_HangHoa_Anh([]);
                    self.files([]);
                    self.newNhomHangHoa().LaNhomHangHoa(false);
                    GetAllNhomHHByLaNhomHH();
                    self.loadavt(false);
                    self._ThemMoiNhomHH(true);
                    $('.errorAnh').text("");
                    self.reset_HangHoaDichVu();
                    self.newHangHoa().LoaiHangHoa(type);
                    self.selectIDNhomHHAddHH(null);
                    $('#choose_TenNHHAddDV').text("---Chọn nhóm---");
                    self.booleanAdd(true);
                    self.TPDL_SumTienVon(0);
                    self.getCTHH(undefined);
                    showdichvu();
                    self.selectedLoaiThoiGianBH("1");
                    $('#checkmanageDVTDV').prop('checked', false);
                    $('#hideDonViTinhDV').hide();
                    document.getElementById('txtTenHangHoa').focus();
                    $('.updateDVTab li').each(function () {
                        $(this).removeClass('active');
                    });
                    $('#txtHangHoaTP').val("");
                    $('#txtTenHangHoa').focus();
                }
            });
        }
    };

    self.arrPrintBarCode = ko.observableArray();
    self.ImageBarcode = ko.observableArray();

    self.maHangMaVach = ko.observable();
    self.tenHangMaVach = ko.observable();
    self.itemMaVach = ko.observableArray();
    self.IDitemMaVach = ko.observable();
    self.GiaBans = ko.observableArray();
    function getAllGiaBan() {
        ajaxHelper('/api/DanhMuc/DM_GiaBanAPI/' + "GetDM_GiaBanByIDDonVi?iddonvi=" + _IDchinhanh, 'GET').done(function (data) {
            self.GiaBans(data);
        });
    };
    getAllGiaBan();
    self.selectedGiaBan = ko.observable();
    self.selectedGiaBan.subscribe(function (newValue) {
    });
    self.modalInMaVach = function (item) {
        self.maHangMaVach(item.MaHangHoa);
        self.tenHangMaVach(item.TenHangHoa);
        self.itemMaVach(item);
        self.IDitemMaVach(item.ID_DonViQuiDoi);
        $('#myModalprint').modal('show');
        $('.khongingiacheck input').prop("checked", false);
        $('.khonginmahangcheck input').prop("checked", false);
        $('.khongintenhangcheck input').prop("checked", false);
        $('.khongintencuahang input').prop("checked", false);
        self.selectedGiaBan(undefined);
    };

    self.modalInTemThuocTinh = function (item) {
        ajaxHelper(DMHangHoaUri + "GetDM_HangHoa_ThuocTinh?id=" + item.ID, 'GET').done(function (data) {
            self.printThuocTinhCuaHH(data);
        });
        $('#printThuocTinh').modal('show');
    };
    self.checkboxClick = function (item) {
        var arrPrintThuocTinhCuaHH = self.printThuocTinhCuaHH();
        self.printThuocTinhCuaHH([]);
        var indexCheckbox = arrPrintThuocTinhCuaHH.findIndex(p => p.ID_ThuocTinh === item.ID_ThuocTinh);
        if (arrPrintThuocTinhCuaHH[indexCheckbox]) {
            if (item.checkboxChecked === false) {
                arrPrintThuocTinhCuaHH[indexCheckbox].checkboxChecked = true;
            }
            else {
                arrPrintThuocTinhCuaHH[indexCheckbox].checkboxChecked = false;
            }
        }
        self.printThuocTinhCuaHH(arrPrintThuocTinhCuaHH);
    };

    self.findbarceCode = ko.observableArray;
    self.CheckInBangGia = ko.observable(true);
    self.CheckInMaHang = ko.observable(true);
    self.CheckInTenHang = ko.observable(true);
    self.CheckInTenCuaHang = ko.observable(true);

    self.CheckInBangGia1 = ko.observable(true);
    self.CheckInMaHang1 = ko.observable(true);
    self.CheckInTenHang1 = ko.observable(true);
    self.CheckInTenCuaHang1 = ko.observable(true);

    function InMaVachItextSharp(tenhanghoa, mahanghoa, giaban, ingia, inmahh, intenhh, intench, sobanghi, somavach) {
        mahanghoa = commonStatisJs.URLEncoding(mahanghoa);
        ajaxHelper(DMHangHoaUri + 'InMaVachITextSharp?tenhanghoa=' + tenhanghoa + '&mahh=' + mahanghoa + '&giaban=' + giaban + '&ingia=' + ingia + '&inmahh=' + inmahh + '&intenhh=' + intenhh + '&intench=' + intench + '&sobanghi=' + sobanghi + '&somavach=' + somavach, 'GET').done(function (data) {
            printJS({ printable: data, type: 'pdf', showModal: false });
        });
    }

    function InMaVachItextSharp2Tem(tenhanghoa, mahanghoa, giaban, ingia, inmahh, intenhh, intench, sobanghi, somavach) {
        mahanghoa = commonStatisJs.URLEncoding(mahanghoa);
        ajaxHelper(DMHangHoaUri + 'InMaVachITextSharp2Tem?tenhanghoa=' + tenhanghoa + '&mahh=' + mahanghoa + '&giaban=' + giaban + '&ingia=' + ingia + '&inmahh=' + inmahh + '&intenhh=' + intenhh + '&intench=' + intench + '&sobanghi=' + sobanghi + '&somavach=' + somavach, 'GET').done(function (data) {
            printJS({ printable: data, type: 'pdf', showModal: false });
        });
    }

    function InMaVachItextSharp1(tenhanghoa, mahanghoa, giaban, ingia, inmahh, intenhh, intench, sobanghi, somavach) {
        mahanghoa = commonStatisJs.URLEncoding(mahanghoa);
        ajaxHelper(DMHangHoaUri + 'InMaVachITextSharp1?tenhanghoa=' + tenhanghoa + '&mahh=' + mahanghoa + '&giaban=' + giaban + '&ingia=' + ingia + '&inmahh=' + inmahh + '&intenhh=' + intenhh + '&intench=' + intench + '&sobanghi=' + sobanghi + '&somavach=' + somavach, 'GET').done(function (data) {
            printJS({ printable: data, type: 'pdf', showModal: false });
        });
    }

    self.PrintBarcode1 = function () {
        if ($('.khongingiacheck input').is(':checked')) {
            self.CheckInBangGia(false);
        }
        else {
            self.CheckInBangGia(true);
        }
        if ($('.khonginmahangcheck input').is(':checked')) {
            self.CheckInMaHang(false);
        }
        else {
            self.CheckInMaHang(true);
        }
        if ($('.khongintenhangcheck input').is(':checked')) {
            self.CheckInTenHang(false);
        }
        else {
            self.CheckInTenHang(true);
        }
        if ($('.khongintencuahang input').is(':checked')) {
            self.CheckInTenCuaHang(false);
        }
        else {
            self.CheckInTenCuaHang(true);
        }
        var tenhang = "";
        tenhang = self.itemMaVach().ThuocTinhGiaTri !== null ? self.itemMaVach().ThuocTinhGiaTri : "";
        tenhang = tenhang + (self.itemMaVach().TenDonViTinh !== null ? "(" + self.itemMaVach().TenDonViTinh + ")" : "");
        tenhang = self.itemMaVach().TenHangHoa + tenhang;
        if (self.selectedGiaBan() !== undefined) {
            ajaxHelper(DMHangHoaUri + 'GetHangHoaByID_BangGia?idgiaban=' + self.selectedGiaBan() + '&iddvqd=' + self.IDitemMaVach(), 'GET').done(function (data) {
                if (data !== null) {
                    InMaVachItextSharp(data.TenHangHoa, data.MaHangHoa, data.GiaBan, self.CheckInBangGia(), self.CheckInMaHang(), self.CheckInTenHang(), self.CheckInTenCuaHang(), 3, 3);
                } else {
                    InMaVachItextSharp(tenhang, self.itemMaVach().MaHangHoa, self.itemMaVach().GiaBan, self.CheckInBangGia(), self.CheckInMaHang(), self.CheckInTenHang(), self.CheckInTenCuaHang(), 3, 3);
                }
            });
        }
        else {
            InMaVachItextSharp(tenhang, self.itemMaVach().MaHangHoa, self.itemMaVach().GiaBan, self.CheckInBangGia(), self.CheckInMaHang(), self.CheckInTenHang(), self.CheckInTenCuaHang(), 3, 3);
        }
    };

    self.PrintBarcode2 = function () {
        self.arrPrintBarCode([]);
        if ($('.khongingiacheck input').is(':checked')) {
            self.CheckInBangGia(false);
        }
        else {
            self.CheckInBangGia(true);
        }
        if ($('.khonginmahangcheck input').is(':checked')) {
            self.CheckInMaHang(false);
        }
        else {
            self.CheckInMaHang(true);
        }
        if ($('.khongintenhangcheck input').is(':checked')) {
            self.CheckInTenHang(false);
        }
        else {
            self.CheckInTenHang(true);
        }
        if ($('.khongintencuahang input').is(':checked')) {
            self.CheckInTenCuaHang(false);
        }
        else {
            self.CheckInTenCuaHang(true);
        }
        var tenhang = "";
        tenhang = self.itemMaVach().ThuocTinhGiaTri;
        tenhang = tenhang + (self.itemMaVach().TenDonViTinh !== null ? "(" + self.itemMaVach().TenDonViTinh + ")" : "");
        tenhang = self.itemMaVach().TenHangHoa + tenhang;
        if (self.selectedGiaBan() !== undefined) {
            ajaxHelper(DMHangHoaUri + 'GetHangHoaByID_BangGia?idgiaban=' + self.selectedGiaBan() + '&iddvqd=' + self.IDitemMaVach(), 'GET').done(function (data) {
                if (data !== null) {
                    InMaVachItextSharp2Tem(data.TenHangHoa, data.MaHangHoa, data.GiaBan, self.CheckInBangGia(), self.CheckInMaHang(), self.CheckInTenHang(), self.CheckInTenCuaHang(), 2, 2);
                } else {
                    InMaVachItextSharp2Tem(tenhang, self.itemMaVach().MaHangHoa, self.itemMaVach().GiaBan, self.CheckInBangGia(), self.CheckInMaHang(), self.CheckInTenHang(), self.CheckInTenCuaHang(), 2, 2);
                }
            });
        }
        else {
            InMaVachItextSharp2Tem(tenhang, self.itemMaVach().MaHangHoa, self.itemMaVach().GiaBan, self.CheckInBangGia(), self.CheckInMaHang(), self.CheckInTenHang(), self.CheckInTenCuaHang(), 2, 2);
        }
    };
    self.PrintBarcode3 = function () {
        if ($('.khongingiacheck input').is(':checked')) {
            self.CheckInBangGia(false);
        }
        else {
            self.CheckInBangGia(true);
        }
        if ($('.khonginmahangcheck input').is(':checked')) {
            self.CheckInMaHang(false);
        }
        else {
            self.CheckInMaHang(true);
        }
        var tenhang = "";
        tenhang = self.itemMaVach().ThuocTinhGiaTri;
        tenhang = tenhang + (self.itemMaVach().TenDonViTinh !== null ? "(" + self.itemMaVach().TenDonViTinh + ")" : "");
        tenhang = self.itemMaVach().TenHangHoa + tenhang;
        if (self.selectedGiaBan() !== undefined) {
            ajaxHelper(DMHangHoaUri + 'GetHangHoaByID_BangGia?idgiaban=' + self.selectedGiaBan() + '&iddvqd=' + self.IDitemMaVach(), 'GET').done(function (data) {
                if (data !== null) {
                    InMaVachItextSharp(data.TenHangHoa, data.MaHangHoa, data.GiaBan, self.CheckInBangGia(), self.CheckInMaHang(), 6, 12);
                } else {
                    InMaVachItextSharp(tenhang, self.itemMaVach().MaHangHoa, self.itemMaVach().GiaBan, self.CheckInBangGia(), self.CheckInMaHang(), 6, 12);
                }
            });
        }
        else {
            InMaVachItextSharp(tenhang, self.itemMaVach().MaHangHoa, self.itemMaVach().GiaBan, self.CheckInBangGia(), self.CheckInMaHang(), 6, 12);
        }
    };
    self.PrintBarcode4 = function () {
        if ($('.khongingiacheck input').is(':checked')) {
            self.CheckInBangGia(false);
        }
        else {
            self.CheckInBangGia(true);
        }
        if ($('.khonginmahangcheck input').is(':checked')) {
            self.CheckInMaHang(false);
        }
        else {
            self.CheckInMaHang(true);
        }
        if ($('.khongintenhangcheck input').is(':checked')) {
            self.CheckInTenHang(false);
        }
        else {
            self.CheckInTenHang(true);
        }
        if ($('.khongintencuahang input').is(':checked')) {
            self.CheckInTenCuaHang(false);
        }
        else {
            self.CheckInTenCuaHang(true);
        }
        var tenhang = "";
        tenhang = self.itemMaVach().ThuocTinhGiaTri;
        tenhang = tenhang + (self.itemMaVach().TenDonViTinh !== null ? "(" + self.itemMaVach().TenDonViTinh + ")" : "");
        tenhang = self.itemMaVach().TenHangHoa + tenhang;
        if (self.selectedGiaBan() !== undefined) {
            ajaxHelper(DMHangHoaUri + 'GetHangHoaByID_BangGia?idgiaban=' + self.selectedGiaBan() + '&iddvqd=' + self.IDitemMaVach(), 'GET').done(function (data) {
                if (data !== null) {
                    InMaVachItextSharp1(data.TenHangHoa, data.MaHangHoa, data.GiaBan, self.CheckInBangGia(), self.CheckInMaHang(), self.CheckInTenHang(), self.CheckInTenCuaHang(), 5, 65);
                } else {
                    InMaVachItextSharp1(tenhang, self.itemMaVach().MaHangHoa, self.itemMaVach().GiaBan, self.CheckInBangGia(), self.CheckInMaHang(), 5, 65);
                }
            });
        }
        else {
            InMaVachItextSharp1(tenhang, self.itemMaVach().MaHangHoa, self.itemMaVach().GiaBan, self.CheckInBangGia(), self.CheckInMaHang(), self.CheckInTenHang(), self.CheckInTenCuaHang(), 5, 65);
        }
    };

    self.InTheoMauIn = function () {
        var id_mauIn = self.selectedMauInMaVach();
        var mahh = self.itemMaVach().MaHangHoa;
        var giaban = self.itemMaVach().GiaBan;
        var sobanghi = self.selectedLoaIn();
        var tenhang = "";
        tenhang = self.itemMaVach().ThuocTinhGiaTri;
        tenhang = tenhang + (self.itemMaVach().TenDonViTinh !== null ? "(" + self.itemMaVach().TenDonViTinh + ")" : "");
        tenhang = self.itemMaVach().TenHangHoa + tenhang;
        InMaVach(id_mauIn, tenhang, mahh, giaban, sobanghi, sobanghi, self.selectedGiaBan(), self.itemMaVach().ID);
    };

    self.InTheoMauInThaoTac = function () {
        var id_mauIn = self.selectedMauInMaVach();
        var sobanghi = self.selectedLoaIn();
        for (var i = 0; i < self.ListChooseHH().length; i++) {
            var aaa = $("#soluongIMV_" + self.ListChooseHH()[i].ID_DonViQuiDoi).val();
            if (aaa === "" || aaa <= 0) {
                aaa = 1;
            }

            self.ListChooseHH()[i].TonKho = aaa;
        }
        InListMaVach(self.ListChooseHH(), self.selectedGiaBan(), id_mauIn, sobanghi);
    };

    var arr = [];

    //$("#myModalprintChooseHH").on('inmavach', function () {
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
        for (var i = 0; i < self.ListChooseHH().length; i++) {
            var aaa = $("#soluongIMV_" + self.ListChooseHH()[i].ID_DonViQuiDoi).val();
            if (aaa === "" || aaa <= 0) {
                aaa = 1;
            }

            self.ListChooseHH()[i].TonKho = aaa;
        }
        var model = {
            listHH: self.ListChooseHH(),
            InGia: self.CheckInBangGia1(),
            InMaHH: self.CheckInMaHang1(),
            InTenHH: self.CheckInTenHang1(),
            InTenCH: self.CheckInTenCuaHang1(),
            ID_BangGia: self.selectedGiaBan(),
            SoBanGhi: sobanghi
        };
        ajaxHelper(DMHangHoaUri + 'PrintBarcodeThaoTac', 'POST', model).done(function (data) {
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
        for (var i = 0; i < self.ListChooseHH().length; i++) {
            var aaa = $("#soluongIMV_" + self.ListChooseHH()[i].ID_DonViQuiDoi).val();
            if (aaa === "" || aaa <= 0) {
                aaa = 1;
            }

            self.ListChooseHH()[i].TonKho = aaa;
        }
        var model = {
            listHH: self.ListChooseHH(),
            InGia: self.CheckInBangGia1(),
            InMaHH: self.CheckInMaHang1(),
            InTenHH: self.CheckInTenHang1(),
            InTenCH: self.CheckInTenCuaHang1(),
            ID_BangGia: self.selectedGiaBan(),
            SoBanGhi: sobanghi
        };
        ajaxHelper(DMHangHoaUri + 'PrintBarcodeThaoTac2Tem', 'POST', model).done(function (data) {
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
        for (var i = 0; i < self.ListChooseHH().length; i++) {
            var aaa = $("#soluongIMV_" + self.ListChooseHH()[i].ID_DonViQuiDoi).val();
            if (aaa === "" || aaa <= 0) {
                aaa = 1;
            }

            self.ListChooseHH()[i].TonKho = aaa;
        }
        var model = {
            listHH: self.ListChooseHH(),
            InGia: self.CheckInBangGia1(),
            InMaHH: self.CheckInMaHang1(),
            InTenHH: self.CheckInTenHang1(),
            InTenCH: self.CheckInTenCuaHang1(),
            ID_BangGia: self.selectedGiaBan(),
            SoBanGhi: sobanghi
        };
        ajaxHelper(DMHangHoaUri + 'PrintBarcodeThaoTac1', 'POST', model).done(function (data) {
            printJS({ printable: data, type: 'pdf', showModal: false });
        });
    }

    self.PrintBarcodeChoose1 = function () {
        inmavach(3);
    };

    self.PrintBarcodeChoose2 = function () {
        inmavach2tem(2);
    };
    self.PrintBarcodeChoose3 = function () {
        inmavach(6);
    };
    self.PrintBarcodeChoose4 = function () {
        inmavach1(5);
    };



    self.BarCodeChooseHH = function () {
        $('#myModalprintChooseHH').modal('show');
        self.CheckInBangGia1(true);
        self.CheckInMaHang1(true);
        $('#myModalprintChooseHH').on('shown.bs.modal', function () {
            $('.khongingiacheck1 input').prop('checked', false);
            $('.khonginmahangcheck1 input').prop('checked', false);
        });
        self.selectedGiaBan(undefined);
    };

    $(".close-print span").click(function () {
        $('.printpage').hide();
        $(".modal-ontop").hide();
        $('.printpageThaoTac').hide();
    });

    $(".close-print-thuoctinh span").click(function () {
        $('.printpage-a4').hide();
        $(".modal-ontop").hide();
        $('#viewPrintThuocTinh').hide();
    });

    self.newHangHoa().ID_NhomHangHoa.subscribe(function (newValue) {
    });

    self.arrChuoiThuocTinh = ko.observableArray();
    self.isSaving = ko.observable(false);
    self.isSavingThuocTinh = ko.observable(false);

    function Disable_btnSave() {
        self.isSaving(true);
    }

    function Enable_btnSave() {
        self.isSaving(false);
    }

    function Disable_btnSaveThuocTinh() {
        self.isSavingThuocTinh(true);
    }

    function Enable_btnSaveThuocTinh() {
        self.isSavingThuocTinh(false)
    }

    function GetProperties_ofHangHoa() {
        var _quanlytheolohang = self.newHangHoa().QuanLyTheoLoHang();
        _quanlytheolohang = _quanlytheolohang === null ? false : _quanlytheolohang;
        var _duoctichdiem = self.newHangHoa().DuocTichDiem();
        _duoctichdiem = _duoctichdiem === true ? 1 : _duoctichdiem === false ? 0 : _duoctichdiem;
        var _dvtheogio = self.newHangHoa().DichVuTheoGio();
        _dvtheogio = _dvtheogio === true ? 1 : _dvtheogio === false ? 0 : _dvtheogio;
        var loaiBduong = 0;
        if (self.QuanLyBaoDuong()) {
            loaiBduong = self.BaoDuong_Type();
        }
        var qlBaoDuong = self.QuanLyBaoDuong();
        qlBaoDuong = qlBaoDuong === true ? 1 : qlBaoDuong === false ? 0 : 0;

        var tinhHoaHongTruocCK = self.TinhHoaHongTruocCK();
        tinhHoaHongTruocCK = tinhHoaHongTruocCK === true ? 1 : tinhHoaHongTruocCK === false ? 0 : 0;

        var giavon = formatNumberToFloat(self.newHangHoa().GiaVon());
        if (self.newHangHoa().DinhLuongDichVu().length > 0) {
            giavon = self.TPDL_SumTienVon();
        }

        var loaiBaoHanh = self.selectedLoaiThoiGianBH();
        var gtriBaoHanh = self.newHangHoa().ThoiGianBaoHanh();
        if (commonStatisJs.CheckNull(gtriBaoHanh) || gtriBaoHanh === 0) {
            loaiBaoHanh = 0;
        }
        var mahanghoa = self.newHangHoa().MaHangHoa();
        var obj = {
            ID: self.newHangHoa().ID(),
            MaHangHoa: mahanghoa,
            ID_HangHoaCungLoai: self.IDCungLoai(),
            GiaBan: formatNumberToFloat(self.newHangHoa().GiaBan()),
            GiaVon: giavon,
            TonKho: self.newHangHoa().TonKho(),
            ID_NhomHang: self.selectIDNhomHHAddHH(),
            TenHangHoa: self.newHangHoa().TenHangHoa().trim(),
            GhiChu: self.newHangHoa().GhiChu(),
            QuyCach: self.newHangHoa().QuyCach(),
            DuocBanTrucTiep: self.newHangHoa().DuocBanTrucTiep(),
            QuanLyTheoLoHang: _quanlytheolohang,
            NguoiTao: _txtTenTaiKhoan,
            TonToiDa: self.newHangHoa().TonToiDa(),
            TonToiThieu: self.newHangHoa().TonToiThieu(),
            ThoiGianBaoHanh: formatNumberToFloat(gtriBaoHanh),
            LoaiBaoHanh: loaiBaoHanh,
            DonViTinhQuyCach: self.newHangHoa().DonViTinhQuyCach(),
            DichVuTheoGio: _dvtheogio,
            DuocTichDiem: _duoctichdiem,
            QuanLyBaoDuong: qlBaoDuong,
            LoaiBaoDuong: loaiBduong,
            HoaHongTruocChietKhau: tinhHoaHongTruocCK,
            LoaiHangHoa: self.newHangHoa().LoaiHangHoa(),
            SoKmBaoHanh: self.newHangHoa().SoKmBaoHanh(),
            ChiPhiTinhTheoPT: self.newHangHoa().ChiPhiTinhTheoPT(),
            ChiPhiThucHien: formatNumberToFloat(self.newHangHoa().ChiPhiThucHien()),
            ChietKhauMD_NV: formatNumberToFloat(self.newHangHoa().ChietKhauMD_NV()),
            ChietKhauMD_NVTheoPT: self.newHangHoa().ChietKhauMD_NVTheoPT(),
            NguoiTao: _txtTenTaiKhoan,
            ID_Xe: self.HangHoa_LaXe() ? self.newHangHoa().ID_Xe() : null,
        }
        console.log('obj ', obj);
        return obj;
    }

    function checkSpecialChars(string) {
        var specialChars = "<>!#$%^&*()+[]{}?:;|'\"\\,/~`=' '"
        for (i = 0; i < specialChars.length; i++) {
            if (string.indexOf(specialChars[i]) > -1) {
                return true;
            }
        }
        return false;
    };

    function CheckInput_Parent() {
        var tenhang = self.newHangHoa().TenHangHoa();
        var type = self.newHangHoa().LoaiHangHoa();
        var sloai = '';
        switch (type) {
            case 2:
            case 3:
                sloai = 'dịch vụ';
                break;
            case 1:
                sloai = 'hàng hóa';
                break;
        }

        if (commonStatisJs.CheckNull(tenhang)) {
            $('#txtTenHangHoa1').focus();
            ShowMessage_Danger('Vui lòng nhập tên ' + sloai);
            return true;
        }

        if (self.QuanLyBaoDuong()) {
            console.log(3, self.BaoDuong_ListDetail())
            let gtri0 = $.grep(self.BaoDuong_ListDetail(), function (x) {
                return formatNumberToFloat(x.GiaTri) === 0;
            })
            if (gtri0.length > 0) {
                ShowMessage_Danger('Vui lòng nhập giá trị cho các lần bảo dưỡng');
                return true;
            }
        }
        return false;
    }

    self.addHangHoa = function (formElement) {
        ajaxHelper(DMHangHoaUri + 'GioiHanSoMatHang', 'GET').done(function (data) {
            if (data) {
                ShowMessage_Danger('Cửa hàng đã đạt số mặt hàng quy định, không thể thêm mới');
                Enable_btnSave();
            }
            else {
                var chuoi = "";
                checkThemCL = 2;
                localStorage.removeItem('DVT');

                var check = CheckInput_Parent();
                if (check) {
                    return false;
                }

                var strMaHangHoa = self.newHangHoa().MaHangHoa();
                var _id = self.newHangHoa().ID();
                var _tonKho = self.newHangHoa().TonKho();
                if (_tonKho !== 0 && _tonKho.indexOf(',') > -1) {
                    _tonKho = formatNumberToInt(_tonKho);
                }
                var _donvitinhchuan = self.newHangHoa().DonViTinhChuan();

                var _quanlytheolohang = self.newHangHoa().QuanLyTheoLoHang();
                _quanlytheolohang = _quanlytheolohang === null ? false : _quanlytheolohang;

                var DM_HangHoa = GetProperties_ofHangHoa();
                var dvt = {
                    MaHangHoa: strMaHangHoa,
                    TenDonViTinh: self.newHangHoa().DonViTinhChuan(),
                    GiaVon: DM_HangHoa.GiaVon,
                    GiaBan: DM_HangHoa.GiaBan,
                    NguoiSua: _txtTenTaiKhoan,
                    NguoiTao: _txtTenTaiKhoan,
                };
                var dlDV = self.newHangHoa().DinhLuongDichVu();

                var dvtQuydois = [];

                var objDVT = self.newHangHoa().DonViTinh();
                dvtQuydois = objDVT;

                var arrThuocTinh = [];
                if (self.newHangHoa().HangHoaCungLoaiArr().length > 0) {
                    var objthuoctinh = self.ThuocTinhCuaHH();
                    if (self.booleanAdd() === true) {
                        var indexdemo = 0;
                        objthuoctinh = [];
                        for (var i = 0; i < self.ThuocTinhCuaHH().length; i++) {
                            for (var j = 0; j < self.ThuocTinhCuaHH()[i].GiaTri.length; j++) {
                                var objtt = {
                                    index: indexdemo,
                                    ID_ThuocTinh: self.ThuocTinhCuaHH()[i].GiaTri[j].ID_ThuocTinh,
                                    TenThuocTinh: "",
                                    GiaTri: self.ThuocTinhCuaHH()[i].GiaTri[j].TenGiaTri
                                };
                                objthuoctinh.push(objtt);
                                indexdemo++;
                            }
                        }
                    }
                    arrThuocTinh = objthuoctinh;
                }
                else {
                    var objThuocTinh = self.ThuocTinhCuaHHEdit();
                    arrThuocTinh = objThuocTinh;

                    var checkthuoctinh = 0;
                    for (var i = 0; i < arrThuocTinh.length; i++) {
                        for (var j = 0; j < arrThuocTinh.length; j++) {
                            if (arrThuocTinh[i].ID_ThuocTinh === arrThuocTinh[j].ID_ThuocTinh && i !== j) {
                                checkthuoctinh = checkthuoctinh + 1;
                            }
                        }
                    }
                    if (checkthuoctinh > 0) {
                        ShowMessage_Danger("Thuộc tính hàng hóa không được trùng nhau");
                        Enable_btnSave();
                        return false;
                    }
                }

                if (_donvitinhchuan !== undefined && _donvitinhchuan !== "" && _donvitinhchuan !== null) {
                    for (var i = 0; i < dvtQuydois.length; i++) {
                        if (dvtQuydois[i].TenDonViTinh !== undefined) {
                            if (dvtQuydois[i].TenDonViTinh === "") {
                                ShowMessage_Danger("Nhập tên đơn vị quy đổi trước khi lưu");
                                Enable_btnSave();
                                return false;
                            }
                            else {
                                if (dvtQuydois[i].TenDonViTinh.toLowerCase() === _donvitinhchuan.toLowerCase()) {
                                    ShowMessage_Danger("Tên đơn vị tính không được trùng nhau");
                                    Enable_btnSave();
                                    return false;
                                }
                                else {
                                    for (var j = 0; j < dvtQuydois.length; j++) {
                                        if (i !== j && (dvtQuydois[i].TenDonViTinh).toLowerCase() === (dvtQuydois[j].TenDonViTinh).toLowerCase()) {
                                            ShowMessage_Danger("Tên đơn vị tính không được trùng nhau");
                                            Enable_btnSave();
                                            return false;
                                        }
                                        if (i !== j && (dvtQuydois[i].MaHangHoa).toLowerCase() === (dvtQuydois[j].MaHangHoa).toLowerCase() && dvtQuydois[i].MaHangHoa !== "") {
                                            ShowMessage_Danger("Trùng lặp mã hàng trong các đơn vị tính, vui lòng kiểm tra lại");
                                            Enable_btnSave();
                                            return false;
                                        }
                                    }
                                }
                            }
                        }

                        if (dvtQuydois[i].TyLeChuyenDoi === "" || dvtQuydois[i].TyLeChuyenDoi === 0) {
                            ShowMessage_Danger("Vui lòng nhập tỷ lể chuyển đổi cho đơn vị tính: " + dvtQuydois[i].TenDonViTinh);
                            Enable_btnSave();
                            return false;
                        }
                    }
                }


                if (self.newHangHoa().HangHoaCungLoaiArr().length === 0) {
                    var checknull = 0;
                    for (var i = 0; i < arrThuocTinh.length; i++) {
                        if (arrThuocTinh[i].ID_ThuocTinh === undefined || arrThuocTinh[i].GiaTri === "") {
                            checknull = checknull + 1;
                        }
                    }
                    if (checknull > 0) {
                        ShowMessage_Danger("Vui lòng nhập đầy đủ thuộc tính của hàng hóa");
                        return false;
                    }
                    for (var i = 0; i < arrThuocTinh.length; i++) {
                        chuoi = chuoi + arrThuocTinh[i].GiaTri;
                    }
                }
                else {
                    var countCLHH = 0;
                    var countCLVoiNhau = 0;
                    var lstMaHHCLKhacEmpty = self.newHangHoa().HangHoaCungLoaiArr().filter(p => p.MaHangHoa !== "");
                    for (var i = 0; i < lstMaHHCLKhacEmpty.length; i++) {
                        if (lstMaHHCLKhacEmpty[i].MaHangHoa.trim().toLowerCase() === strMaHangHoa.trim().toLowerCase()) {
                            countCLHH++;
                        }
                        for (var j = 0; j < lstMaHHCLKhacEmpty.length; j++) {
                            if (i !== j && lstMaHHCLKhacEmpty[i].MaHangHoa.trim().toLowerCase() === lstMaHHCLKhacEmpty[j].MaHangHoa.trim().toLowerCase()) {
                                countCLVoiNhau++;
                            }
                        }
                    }
                    if (countCLHH > 0) {
                        ShowMessage_Danger("Mã hàng cùng loại không được trùng mã hàng hóa");
                        Enable_btnSave();
                        return false;
                    }

                    if (countCLVoiNhau > 0) {
                        ShowMessage_Danger("Trùng lặp mã hàng hóa trong danh sách hàng hóa cùng loại");
                        Enable_btnSave();
                        return false;
                    }
                }
                ajaxHelper(DMHangHoaUri + 'getThuocTinhGhepChuoi?id_hanghoa=' + self.IDCungLoai() + '&chuoi=' + chuoi, 'GET').done(function (data1) {
                    if (data1 === true && self.CheckThemHHCL() === true) {
                        ShowMessage_Danger("Thuộc tính đã tồn tại trong hàng hóa cùng loại");
                        Enable_btnSave();
                    }
                    else {
                        var myData = {};
                        myData.iddonvi = _IDchinhanh;
                        myData.idnhanvien = _IDNhanVien;
                        myData.objNewHH = DM_HangHoa;
                        myData.objNewDVT = dvt;
                        myData.listDVTQuydois = dvtQuydois;
                        myData.listDLDV = dlDV;
                        myData.listViTri = self.MangNhomViTriHH();
                        if (self.newHangHoa().HangHoaCungLoaiArr().length > 0) {
                            myData.objHangHoaCungLoai = self.newHangHoa().HangHoaCungLoaiArr();
                        }
                        else {
                            myData.ListThuocTinh = arrThuocTinh;
                        }
                        if (_tonKho !== null) {
                            var TonKhoKT = {
                                MaHangHoa: strMaHangHoa,
                                SoLuong: _tonKho,
                                DonGia: DM_HangHoa.GiaVon
                            };
                            myData.objNewTonKhoKT = TonKhoKT;
                        }

                        Disable_btnSave();

                        console.log('hh ', myData)
                        if (self.booleanAdd() === true) {
                            ajaxHelper(DMHangHoaUri + "Check_MaHangHoaExist?maHangHoa=" + commonStatisJs.URLEncoding(strMaHangHoa), 'POST').done(function (data) {
                                if (data) {
                                    ShowMessage_Danger("Mã hàng hóa đã tồn tại");
                                    $('#txtMaHangHoa1').focus();
                                    Enable_btnSave();
                                    return false;
                                } else {
                                    if (dvtQuydois.length === 0) {
                                        if (self.CheckThemHHCL() === true) {
                                            calljaxAddCungLoai(myData);
                                        }
                                        if (self.CheckThemHHCL() === false) {
                                            if (self.newHangHoa().HangHoaCungLoaiArr().length > 0) {
                                                calljaxAddMultilHHCungLoai(myData);
                                            } else {
                                                calljaxAdd(myData);
                                            }
                                        }
                                    } else {
                                        var _found = 0;
                                        var j = 0;
                                        var _foundDVT = 0;
                                        var _maHH = "";
                                        var _maHHDVT = "";
                                        var arrMaHHDVT = [];
                                        function checkmadvt() {
                                            if (j < dvtQuydois.length) {
                                                var madvt = dvtQuydois[j].MaHangHoa;
                                                j++;
                                                if (madvt === strMaHangHoa && madvt !== "") {
                                                    _found = _found + 1;
                                                    _maHH = _maHH + madvt + " ";
                                                    checkmadvt();
                                                }
                                                else if (madvt !== strMaHangHoa && madvt !== "") {
                                                    ajaxHelper(DMHangHoaUri + "Check_MaHangHoaExist?maHangHoa=" + commonStatisJs.URLEncoding(madvt), 'POST').done(function (dataDVT) {
                                                        if (dataDVT) {
                                                            _foundDVT = _foundDVT + 1;
                                                            _maHHDVT = _maHHDVT + madvt + " ";
                                                        }
                                                        checkmadvt();
                                                    });
                                                }
                                                else {
                                                    checkmadvt();
                                                }
                                            }
                                            else {
                                                addhh();
                                            }
                                        }
                                        checkmadvt();
                                        function addhh() {
                                            if (_found > 0) {
                                                ShowMessage_Danger("Mã đơn vị: " + _maHH + " " + "trùng mã hàng hóa");
                                                Enable_btnSave();
                                                return false;
                                            }
                                            else if (_foundDVT > 0) {
                                                ShowMessage_Danger("Mã đơn vị: " + _maHHDVT + " " + "đã tồn tại.");
                                                Enable_btnSave();
                                                return false;
                                            }
                                            else {
                                                if (self.CheckThemHHCL() == true) {
                                                    calljaxAddCungLoai(myData);
                                                }
                                                else {
                                                    if (self.newHangHoa().HangHoaCungLoaiArr().length > 0) {
                                                        calljaxAddMultilHHCungLoai(myData);
                                                    } else {
                                                        calljaxAdd(myData);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                        }
                        // edit
                        else {
                            myData.objNewHH.NguoiSua = _txtTenTaiKhoan;
                            if (_quanlytheolohang === true && self.LaQuanLyTheoLo() === false && (_tonKho > 0 || self.TheKhos().filter(p => p.LoaiHoaDon !== 18).length > 0)) {
                                ShowMessage_Danger("Sản phẩm đã có tồn kho hoặc đã phát sinh giao dịch, không thể chuyển sang quản lý theo lô");
                                Enable_btnSave();
                                return false;
                            }
                            else {
                                if (self.LaQuanLyTheoLo() === true && _quanlytheolohang === false) {
                                    ShowMessage_Danger("Sản phẩm đã được quản lý theo lô, không thể thay đổi ngược lại");
                                    Enable_btnSave();
                                    return false;
                                }
                                else {
                                    ajaxHelper(DMHangHoaUri + "Check_MaHangHoaExistDVT?maHangHoa=" + commonStatisJs.URLEncoding(strMaHangHoa) + '&idhangHoa=' + _id, 'POST').done(function (data) {
                                        if (data) {
                                            ShowMessage_Danger("Mã hàng hóa đã tồn tại");
                                            $('#txtMaHangHoa1').focus();
                                            Enable_btnSave();
                                            return false;
                                        } else {
                                            ajaxHelper(DMHangHoaUri + 'getThuocTinhGhepChuoiEdit?idcungloai=' + self.IDCungLoai() + '&chuoi=' + chuoi + '&idhanghoa=' + _id, 'GET').done(function (dataCheckTT) {
                                                if (dataCheckTT > 0) {
                                                    ShowMessage_Danger("Thuộc tính đã tồn tại trong hàng hóa cùng loại");
                                                    Enable_btnSave();
                                                    return false;
                                                }
                                                else {
                                                    if (dvtQuydois.length === 0) {
                                                        calljaxEditHH(myData);
                                                    }
                                                    else {
                                                        var _found = 0;
                                                        var j = 0;
                                                        var _foundDVT = 0;
                                                        var _maHH = "";
                                                        var _maHHDVT = "";
                                                        var arrMaHHDVT = [];
                                                        function checkmadvt() {
                                                            if (j < dvtQuydois.length) {
                                                                var madvt = dvtQuydois[j].MaHangHoa;
                                                                j++;
                                                                if (madvt === strMaHangHoa && madvt !== "") {
                                                                    _found = _found + 1;
                                                                    _maHH = _maHH + madvt + " ";
                                                                    checkmadvt();
                                                                }
                                                                else if (madvt !== strMaHangHoa && madvt !== "") {
                                                                    ajaxHelper(DMHangHoaUri + "Check_MaHangHoaExistDVT?maHangHoa=" + commonStatisJs.URLEncoding(madvt) + '&idhangHoa=' + _id, 'POST').done(function (dataDVT) {
                                                                        if (dataDVT) {
                                                                            _foundDVT = _foundDVT + 1;
                                                                            _maHHDVT = _maHHDVT + madvt + " ";
                                                                        }
                                                                        checkmadvt();
                                                                    })
                                                                }
                                                                else {
                                                                    checkmadvt();
                                                                }
                                                            }
                                                            else {
                                                                edithh();
                                                            }
                                                        }
                                                        checkmadvt();
                                                        function edithh() {
                                                            if (_found > 0) {
                                                                ShowMessage_Danger("Mã đơn vị: " + _maHH + " " + "trùng mã hàng hóa");
                                                                Enable_btnSave();
                                                                return false;
                                                            }
                                                            else if (_foundDVT > 0) {
                                                                ShowMessage_Danger("Mã đơn vị: " + _maHHDVT + " " + "đã tồn tại.");
                                                                Enable_btnSave();
                                                                return false;
                                                            }
                                                            else {
                                                                calljaxEditHH(myData);
                                                            }
                                                        }
                                                    }
                                                }
                                            });
                                        }
                                    });
                                }
                            }
                        }
                    }
                });
            }
        });
    };

    self.addHangHoaAndAddNew = function () {
        ajaxHelper(DMHangHoaUri + 'GioiHanSoMatHang', 'GET').done(function (data) {
            if (data) {
                ShowMessage_Danger("Cửa hàng đã đạt số mặt hàng quy định, không thể thêm mới");
            }
            else {
                var chuoi = "";
                checkThemCL = 2;
                localStorage.removeItem('DVT');

                var check = CheckInput_Parent();
                if (check) {
                    return false;
                }

                var strMaHangHoa = self.newHangHoa().MaHangHoa();
                var _id = self.newHangHoa().ID();
                var _tonKho = self.newHangHoa().TonKho();
                if (_tonKho !== 0 && _tonKho.indexOf(',') > -1) {
                    _tonKho = formatNumberToInt(_tonKho);
                }
                var _donvitinhchuan = self.newHangHoa().DonViTinhChuan();
                var _quanlytheolohang = self.newHangHoa().QuanLyTheoLoHang();
                _quanlytheolohang = _quanlytheolohang === null ? false : _quanlytheolohang;

                Disable_btnSave();

                var DM_HangHoa = GetProperties_ofHangHoa();
                var dvt = {
                    MaHangHoa: strMaHangHoa,
                    TenDonViTinh: _donvitinhchuan,
                    GiaVon: DM_HangHoa.GiaVon,
                    GiaBan: DM_HangHoa.GiaBan,
                };

                var dlDV = self.newHangHoa().DinhLuongDichVu();

                var dvtQuydois = [];

                var objDVT = self.newHangHoa().DonViTinh();
                dvtQuydois = objDVT;

                var arrThuocTinh = [];
                if (self.newHangHoa().HangHoaCungLoaiArr().length > 0) {
                    var objthuoctinh = self.ThuocTinhCuaHH();
                    if (self.booleanAdd() === true) {
                        var indexdemo = 0;
                        objthuoctinh = [];
                        for (var i = 0; i < self.ThuocTinhCuaHH().length; i++) {
                            for (var j = 0; j < self.ThuocTinhCuaHH()[i].GiaTri.length; j++) {
                                var objtt = {
                                    index: indexdemo,
                                    ID_ThuocTinh: self.ThuocTinhCuaHH()[i].GiaTri[j].ID_ThuocTinh,
                                    TenThuocTinh: "",
                                    GiaTri: self.ThuocTinhCuaHH()[i].GiaTri[j].TenGiaTri
                                };
                                objthuoctinh.push(objtt);
                                indexdemo++;
                            }
                        }
                    }
                    arrThuocTinh = objthuoctinh;
                }
                else {
                    var objThuocTinh = self.ThuocTinhCuaHHEdit();
                    arrThuocTinh = objThuocTinh;

                    var checkthuoctinh = 0;
                    for (var i = 0; i < arrThuocTinh.length; i++) {
                        for (var j = 0; j < arrThuocTinh.length; j++) {
                            if (arrThuocTinh[i].ID_ThuocTinh === arrThuocTinh[j].ID_ThuocTinh && i !== j) {
                                checkthuoctinh = checkthuoctinh + 1;
                            }
                        }
                    }
                    if (checkthuoctinh > 0) {
                        ShowMessage_Danger("Thuộc tính hàng hóa không được trùng nhau");
                        Enable_btnSave();
                        return false;
                    }
                }

                if (_donvitinhchuan !== undefined && _donvitinhchuan !== "" && _donvitinhchuan !== null) {
                    for (var i = 0; i < dvtQuydois.length; i++) {
                        if (dvtQuydois[i].TenDonViTinh !== undefined) {
                            if (dvtQuydois[i].TenDonViTinh === "") {
                                ShowMessage_Danger("Nhập tên đơn vị quy đổi trước khi lưu");
                                Enable_btnSave();
                                return false;
                            }
                            else {
                                if (dvtQuydois[i].TenDonViTinh.toLowerCase() === _donvitinhchuan.toLowerCase()) {
                                    ShowMessage_Danger("Tên đơn vị tính không được trùng nhau");
                                    Enable_btnSave();
                                    return false;
                                }
                                else {
                                    for (var j = 0; j < dvtQuydois.length; j++) {
                                        if (i !== j && (dvtQuydois[i].TenDonViTinh).toLowerCase() === (dvtQuydois[j].TenDonViTinh).toLowerCase()) {
                                            ShowMessage_Danger("Tên đơn vị tính không được trùng nhau");
                                            Enable_btnSave();
                                            return false;
                                        }
                                        if (i !== j && (dvtQuydois[i].MaHangHoa).toLowerCase() === (dvtQuydois[j].MaHangHoa).toLowerCase() && dvtQuydois[i].MaHangHoa !== "") {
                                            ShowMessage_Danger("Trùng lặp mã hàng trong các đơn vị tính, vui lòng kiểm tra lại");
                                            Enable_btnSave();
                                            return false;
                                        }
                                    }
                                }
                            }
                        }

                        if (dvtQuydois[i].TyLeChuyenDoi === "" || dvtQuydois[i].TyLeChuyenDoi === 0) {
                            ShowMessage_Danger("Vui lòng nhập tỷ lể chuyển đổi cho đơn vị tính: " + dvtQuydois[i].TenDonViTinh);
                            Enable_btnSave();
                            return false;
                        }
                    }
                }


                if (self.newHangHoa().HangHoaCungLoaiArr().length === 0) {
                    var checknull = 0;
                    for (var i = 0; i < arrThuocTinh.length; i++) {
                        if (arrThuocTinh[i].ID_ThuocTinh === undefined || arrThuocTinh[i].GiaTri === "") {
                            checknull = checknull + 1;
                        }
                    }
                    if (checknull > 0) {
                        ShowMessage_Danger("Vui lòng nhập đầy đủ thuộc tính của hàng hóa");
                        return false;
                    }
                    for (var i = 0; i < arrThuocTinh.length; i++) {
                        chuoi = chuoi + arrThuocTinh[i].GiaTri;
                    }
                }
                else {
                    var countCLHH = 0;
                    var countCLVoiNhau = 0;
                    var lstMaHHCLKhacEmpty = self.newHangHoa().HangHoaCungLoaiArr().filter(p => p.MaHangHoa !== "");
                    for (var i = 0; i < lstMaHHCLKhacEmpty.length; i++) {
                        if (lstMaHHCLKhacEmpty[i].MaHangHoa.trim().toLowerCase() === strMaHangHoa.trim().toLowerCase()) {
                            countCLHH++;
                        }
                        for (var j = 0; j < lstMaHHCLKhacEmpty.length; j++) {
                            if (i !== j && lstMaHHCLKhacEmpty[i].MaHangHoa.trim().toLowerCase() === lstMaHHCLKhacEmpty[j].MaHangHoa.trim().toLowerCase()) {
                                countCLVoiNhau++;
                            }
                        }
                    }
                    if (countCLHH > 0) {
                        ShowMessage_Danger("Mã hàng cùng loại không được trùng mã hàng hóa");
                        Enable_btnSave();
                        return false;
                    }

                    if (countCLVoiNhau > 0) {
                        ShowMessage_Danger("Trùng lặp mã hàng hóa trong danh sách hàng hóa cùng loại");
                        Enable_btnSave();
                        return false;
                    }
                }
                ajaxHelper(DMHangHoaUri + 'getThuocTinhGhepChuoi?id_hanghoa=' + self.IDCungLoai() + '&chuoi=' + chuoi, 'GET').done(function (data1) {
                    if (data1 === true && self.CheckThemHHCL() === true) {
                        ShowMessage_Danger("Thuộc tính đã tồn tại trong hàng hóa cùng loại");
                        Enable_btnSave();
                    }
                    else {
                        var myData = {};
                        myData.iddonvi = _IDchinhanh;
                        myData.idnhanvien = _IDNhanVien;
                        myData.objNewHH = DM_HangHoa;
                        myData.objNewDVT = dvt;
                        myData.listDVTQuydois = dvtQuydois;
                        myData.listDLDV = dlDV;
                        myData.listViTri = self.MangNhomViTriHH();

                        if (self.newHangHoa().HangHoaCungLoaiArr().length > 0) {
                            myData.objHangHoaCungLoai = self.newHangHoa().HangHoaCungLoaiArr();
                        }
                        else {
                            myData.ListThuocTinh = arrThuocTinh;
                        }
                        //return false;

                        if (_tonKho !== null) {
                            var TonKhoKT = {
                                MaHangHoa: strMaHangHoa,
                                SoLuong: _tonKho,
                                DonGia: DM_HangHoa.GiaVon
                            };
                            myData.objNewTonKhoKT = TonKhoKT;
                        }

                        if (self.booleanAdd() === true) {
                            ajaxHelper(DMHangHoaUri + "Check_MaHangHoaExist?maHangHoa=" + commonStatisJs.URLEncoding(strMaHangHoa), 'POST').done(function (data) {
                                if (data) {
                                    ShowMessage_Danger("Mã hàng hóa đã tồn tại");
                                    $('#txtMaHangHoa1').focus();
                                    Enable_btnSave();
                                    return false;
                                } else {
                                    if (dvtQuydois.length === 0) {
                                        if (self.CheckThemHHCL() === true) {
                                            calljaxAddCungLoai(myData);
                                        }
                                        if (self.CheckThemHHCL() === false) {
                                            if (self.newHangHoa().HangHoaCungLoaiArr().length > 0) {
                                                calljaxAddMultilHHCungLoai(myData);
                                            } else {
                                                calljaxAddAndAddNew(myData);
                                            }
                                        }
                                    } else {
                                        var _found = 0;
                                        var j = 0;
                                        var _foundDVT = 0;
                                        var _maHH = "";
                                        var _maHHDVT = "";
                                        var arrMaHHDVT = [];
                                        function checkmadvt() {
                                            if (j < dvtQuydois.length) {
                                                var madvt = dvtQuydois[j].MaHangHoa;
                                                j++;
                                                if (madvt === strMaHangHoa && madvt !== "") {
                                                    _found = _found + 1;
                                                    _maHH = _maHH + madvt + " ";
                                                    checkmadvt();
                                                }
                                                else if (madvt !== strMaHangHoa && madvt !== "") {
                                                    ajaxHelper(DMHangHoaUri + "Check_MaHangHoaExist?maHangHoa=" + commonStatisJs.URLEncoding(madvt), 'POST').done(function (dataDVT) {
                                                        if (dataDVT) {
                                                            _foundDVT = _foundDVT + 1;
                                                            _maHHDVT = _maHHDVT + madvt + " ";
                                                        }
                                                        checkmadvt();
                                                    });
                                                }
                                                else {
                                                    checkmadvt();
                                                }
                                            }
                                            else {
                                                addhh();
                                            }
                                        }
                                        checkmadvt();
                                        function addhh() {
                                            if (_found > 0) {
                                                Enable_btnSave();
                                                return false;
                                            }
                                            else if (_foundDVT > 0) {
                                                ShowMessage_Danger("Mã đơn vị: " + _maHHDVT + " " + "đã tồn tại.");
                                                Enable_btnSave();
                                                return false;
                                            }
                                            else {
                                                if (self.CheckThemHHCL() == true) {
                                                    calljaxAddCungLoai(myData);
                                                }
                                                else {
                                                    if (self.newHangHoa().HangHoaCungLoaiArr().length > 0) {
                                                        calljaxAddMultilHHCungLoai(myData);
                                                    } else {
                                                        calljaxAddAndAddNew(myData);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                        }
                        // edit
                        else {
                            myData.objNewHH.NguoiSua = _txtTenTaiKhoan;

                            if (_quanlytheolohang === true && self.LaQuanLyTheoLo() === false && (_tonKho > 0 || self.TheKhos().filter(p => p.LoaiHoaDon !== 18).length > 0)) {
                                ShowMessage_Danger("Sản phẩm đã có tồn kho hoặc đã phát sinh giao dịch, không thể chuyển sang quản lý theo lô");
                                Enable_btnSave();
                                return false;
                            }
                            else {
                                if (self.LaQuanLyTheoLo() === true && _quanlytheolohang === false) {
                                    ShowMessage_Danger("Sản phẩm đã được quản lý theo lô, không thể thay đổi ngược lại");
                                    Enable_btnSave();
                                    return false;
                                }
                                else {
                                    ajaxHelper(DMHangHoaUri + "Check_MaHangHoaExistDVT?maHangHoa=" + commonStatisJs.URLEncoding(strMaHangHoa) + '&idhangHoa=' + _id, 'POST').done(function (data) {
                                        if (data) {
                                            ShowMessage_Danger("Mã hàng hóa đã tồn tại");
                                            $('#txtMaHangHoa1').focus();
                                            Enable_btnSave();
                                            return false;
                                        } else {
                                            ajaxHelper(DMHangHoaUri + 'getThuocTinhGhepChuoiEdit?idcungloai=' + self.IDCungLoai() + '&chuoi=' + chuoi + '&idhanghoa=' + _id, 'GET').done(function (dataCheckTT) {
                                                if (dataCheckTT > 0) {
                                                    ShowMessage_Danger("Thuộc tính đã tồn tại trong hàng hóa cùng loại");
                                                    Enable_btnSave();
                                                    return false;
                                                }
                                                else {
                                                    if (dvtQuydois.length === 0) {
                                                        calljaxEditHHAndAddNew(myData);
                                                    }
                                                    else {
                                                        var _found = 0;
                                                        var j = 0;
                                                        var _foundDVT = 0;
                                                        var _maHH = "";
                                                        var _maHHDVT = "";
                                                        var arrMaHHDVT = [];
                                                        function checkmadvt() {
                                                            if (j < dvtQuydois.length) {
                                                                var madvt = dvtQuydois[j].MaHangHoa;
                                                                j++;
                                                                if (madvt === strMaHangHoa && madvt !== "") {
                                                                    _found = _found + 1;
                                                                    _maHH = _maHH + madvt + " ";
                                                                    checkmadvt();
                                                                }
                                                                else if (madvt !== strMaHangHoa && madvt !== "") {
                                                                    ajaxHelper(DMHangHoaUri + "Check_MaHangHoaExistDVT?maHangHoa=" + commonStatisJs.URLEncoding(madvt) + '&idhangHoa=' + _id, 'POST').done(function (dataDVT) {
                                                                        if (dataDVT) {
                                                                            _foundDVT = _foundDVT + 1;
                                                                            _maHHDVT = _maHHDVT + madvt + " ";
                                                                        }
                                                                        checkmadvt();
                                                                    })
                                                                }
                                                                else {
                                                                    checkmadvt();
                                                                }
                                                            }
                                                            else {
                                                                edithh();
                                                            }
                                                        }
                                                        checkmadvt();
                                                        function edithh() {
                                                            if (_found > 0) {
                                                                ShowMessage_Danger("Mã đơn vị: " + _maHH + " " + "trùng mã hàng hóa");
                                                                Enable_btnSave();
                                                                return false;
                                                            }
                                                            else if (_foundDVT > 0) {
                                                                ShowMessage_Danger("Mã đơn vị: " + _maHHDVT + " " + "đã tồn tại.");
                                                                Enable_btnSave();
                                                                return false;
                                                            }
                                                            else {
                                                                calljaxEditHHAndAddNew(myData);
                                                            }
                                                        }
                                                    }
                                                }
                                            });
                                        }
                                    });
                                }
                            }
                        }
                    }
                });
            }
        });
    };

    var checkThemCL = 0;
    self.addHangHoaCungLoai = function (formElement) {
        ajaxHelper(DMHangHoaUri + 'GioiHanSoMatHang', 'GET').done(function (data) {
            if (data) {
                ShowMessage_Danger("Cửa hàng đã đạt số mặt hàng quy định, không thể thêm mới");
            }
            else {
                chuoi = "";
                checkThemCL = 1;
                localStorage.removeItem('DVT');
                var strMaHangHoa = self.newHangHoa().MaHangHoa();
                var _id = self.newHangHoa().ID();

                var check = CheckInput_Parent();
                if (check) {
                    return false;
                }

                var _tonKho = self.newHangHoa().TonKho();
                if (_tonKho !== 0 && _tonKho.indexOf(',') > -1) {
                    _tonKho = formatNumberToInt(_tonKho);
                }
                var _donvitinhchuan = self.newHangHoa().DonViTinhChuan();
                var _quanlytheolohang = self.newHangHoa().QuanLyTheoLoHang();
                _quanlytheolohang = _quanlytheolohang === null ? false : _quanlytheolohang;

                var DM_HangHoa = GetProperties_ofHangHoa();
                var dvt = {
                    MaHangHoa: strMaHangHoa,
                    TenDonViTinh: _donvitinhchuan,
                    GiaVon: DM_HangHoa.GiaVon,
                    GiaBan: DM_HangHoa.GiaBan,
                };

                var dlDV = self.newHangHoa().DinhLuongDichVu();

                var dvtQuydois = [];

                var objDVT = self.newHangHoa().DonViTinh();
                dvtQuydois = objDVT;

                var arrThuocTinh = [];
                if (self.newHangHoa().HangHoaCungLoaiArr().length > 0) {
                    var objthuoctinh = self.ThuocTinhCuaHH();
                    if (self.booleanAdd() === true) {
                        var indexdemo = 0;
                        objthuoctinh = [];
                        for (var i = 0; i < self.ThuocTinhCuaHH().length; i++) {
                            for (var j = 0; j < self.ThuocTinhCuaHH()[i].GiaTri.length; j++) {
                                var objtt = {
                                    index: indexdemo,
                                    ID_ThuocTinh: self.ThuocTinhCuaHH()[i].GiaTri[j].ID_ThuocTinh,
                                    TenThuocTinh: "",
                                    GiaTri: self.ThuocTinhCuaHH()[i].GiaTri[j].TenGiaTri
                                };
                                objthuoctinh.push(objtt);
                                indexdemo++;
                            }
                        }
                    }
                    arrThuocTinh = objthuoctinh;
                }
                else {
                    var objThuocTinh = self.ThuocTinhCuaHHEdit();
                    arrThuocTinh = objThuocTinh;

                    var checkthuoctinh = 0;
                    for (var i = 0; i < arrThuocTinh.length; i++) {
                        for (var j = 0; j < arrThuocTinh.length; j++) {
                            if (arrThuocTinh[i].ID_ThuocTinh === arrThuocTinh[j].ID_ThuocTinh && i !== j) {
                                checkthuoctinh = checkthuoctinh + 1;
                            }
                        }
                    }
                    if (checkthuoctinh > 0) {
                        ShowMessage_Danger("Thuộc tính hàng hóa không được trùng nhau");
                        Enable_btnSave();
                        return false;
                    }
                }


                if (_donvitinhchuan !== undefined && _donvitinhchuan !== "" && _donvitinhchuan !== null) {
                    for (var i = 0; i < dvtQuydois.length; i++) {
                        if (dvtQuydois[i].TenDonViTinh !== undefined) {
                            if (dvtQuydois[i].TenDonViTinh == "") {
                                ShowMessage_Danger("Nhập tên đơn vị quy đổi trước khi lưu");
                                Enable_btnSave();
                                return false;
                            }
                            else {
                                if (dvtQuydois[i].TenDonViTinh.toLowerCase() === _donvitinhchuan.toLowerCase()) {
                                    ShowMessage_Danger("Tên đơn vị tính không được trùng nhau");
                                    Enable_btnSave();
                                    return false;
                                }
                                else {
                                    for (var j = 0; j < dvtQuydois.length; j++) {
                                        if (i !== j && (dvtQuydois[i].TenDonViTinh).toLowerCase() === (dvtQuydois[j].TenDonViTinh).toLowerCase()) {
                                            ShowMessage_Danger("Tên đơn vị tính không được trùng nhau");
                                            Enable_btnSave();
                                            return false;
                                        }
                                        if (i !== j && (dvtQuydois[i].MaHangHoa).toLowerCase() == (dvtQuydois[j].MaHangHoa).toLowerCase() && dvtQuydois[i].MaHangHoa !== "") {
                                            Enable_btnSave();
                                            return false;
                                        }
                                    }
                                }
                            }
                        }

                        if (dvtQuydois[i].TyLeChuyenDoi === "" || dvtQuydois[i].TyLeChuyenDoi === 0) {
                            ShowMessage_Danger("Vui lòng nhập tỷ lể chuyển đổi cho đơn vị tính: " + dvtQuydois[i].TenDonViTinh);
                            Enable_btnSave();
                            return false;
                        }
                    }
                }
                if (self.newHangHoa().HangHoaCungLoaiArr().length === 0) {
                    var checknull = 0;
                    for (var i = 0; i < arrThuocTinh.length; i++) {
                        if (arrThuocTinh[i].ID_ThuocTinh === undefined || arrThuocTinh[i].GiaTri === "") {
                            checknull = checknull + 1;
                        }
                    }
                    if (checknull > 0) {
                        ShowMessage_Danger("Vui lòng nhập đầy đủ thuộc tính của hàng hóa");
                        return false;
                    }
                    for (var i = 0; i < arrThuocTinh.length; i++) {
                        chuoi = chuoi + arrThuocTinh[i].GiaTri;
                    }
                }

                Disable_btnSave();
                ajaxHelper(DMHangHoaUri + 'getThuocTinhGhepChuoi?id_hanghoa=' + self.IDCungLoai() + '&chuoi=' + chuoi, 'GET').done(function (data1) {
                    if (data1 === true && self.CheckThemHHCL() === true) {
                        ShowMessage_Danger("Thuộc tính đã tồn tại trong hàng hóa cùng loại");
                        Enable_btnSave();
                    }
                    else {
                        var myData = {};
                        myData.iddonvi = _IDchinhanh;
                        myData.idnhanvien = _IDNhanVien;
                        myData.objNewHH = DM_HangHoa;
                        myData.objNewDVT = dvt;
                        myData.listDVTQuydois = dvtQuydois;
                        myData.listDLDV = dlDV;
                        myData.listViTri = self.MangNhomViTriHH();
                        if (self.newHangHoa().HangHoaCungLoaiArr().length > 0) {
                            myData.objHangHoaCungLoai = self.newHangHoa().HangHoaCungLoaiArr();
                        }
                        else {
                            myData.ListThuocTinh = arrThuocTinh;
                        }
                        if (_tonKho !== null) {
                            var TonKhoKT = {
                                MaHangHoa: strMaHangHoa,
                                SoLuong: _tonKho,
                                DonGia: DM_HangHoa.GiaVon
                            };
                            myData.objNewTonKhoKT = TonKhoKT;
                        }

                        if (self.booleanAdd() === true) {
                            ajaxHelper(DMHangHoaUri + "Check_MaHangHoaExist?maHangHoa=" + commonStatisJs.URLEncoding(strMaHangHoa), 'POST').done(function (data) {
                                if (data) {
                                    ShowMessage_Danger("Mã hàng hóa đã tồn tại");
                                    $('#txtMaHangHoa1').focus();
                                    Enable_btnSave();
                                    return false;
                                } else {
                                    if (dvtQuydois.length === 0) {
                                        if (self.newHangHoa().HangHoaCungLoaiArr().length > 0) {
                                            calljaxAddMultilHHCungLoai(myData);
                                        } else {
                                            calljaxAddCungLoai(myData);
                                        }
                                    } else {
                                        var _found = 0;
                                        var j = 0;
                                        var _foundDVT = 0;
                                        var _maHH = "";
                                        var _maHHDVT = "";
                                        var arrMaHHDVT = [];
                                        function checkmadvt() {
                                            if (j < dvtQuydois.length) {
                                                var madvt = dvtQuydois[j].MaHangHoa;
                                                j++;
                                                if (madvt === strMaHangHoa && madvt !== "") {
                                                    _found = _found + 1;
                                                    _maHH = _maHH + madvt + " ";
                                                    checkmadvt();
                                                }
                                                else if (madvt !== strMaHangHoa && madvt !== "") {
                                                    ajaxHelper(DMHangHoaUri + "Check_MaHangHoaExist?maHangHoa=" + commonStatisJs.URLEncoding(madvt), 'POST').done(function (dataDVT) {
                                                        if (dataDVT) {
                                                            _foundDVT = _foundDVT + 1;
                                                            _maHHDVT = _maHHDVT + madvt + " ";
                                                        }
                                                        checkmadvt();
                                                    });
                                                }
                                                else {
                                                    checkmadvt();
                                                }
                                            }
                                            else {
                                                addhh();
                                            }
                                        };
                                        checkmadvt();
                                        function addhh() {
                                            if (_found > 0) {
                                                ShowMessage_Danger("Mã hàng hóa đơn vị: " + _maHH + " " + "đã tồn tại");
                                                Enable_btnSave();
                                                return false;
                                            }
                                            else if (_foundDVT > 0) {
                                                ShowMessage_Danger("Mã đơn vị: " + _maHHDVT + " " + "đã tồn tại.");
                                                Enable_btnSave();
                                                return false;
                                            }
                                            else {
                                                if (self.newHangHoa().HangHoaCungLoaiArr().length > 0) {
                                                    calljaxAddMultilHHCungLoai(myData);
                                                } else {
                                                    calljaxAddCungLoai(myData);
                                                }
                                            }
                                        };
                                    }
                                }
                            });
                        }
                        // edit
                        else {
                            myData.objNewHH.NguoiSua = _txtTenTaiKhoan;
                            if (_quanlytheolohang === true && self.LaQuanLyTheoLo() === false && (_tonKho > 0 || self.TheKhos().filter(p => p.LoaiHoaDon !== 18).length > 0)) {
                                ShowMessage_Danger("Sản phẩm đã có tồn kho hoặc đã phát sinh giao dịch, không thể chuyển sang quản lý theo lô");
                                Enable_btnSave();
                                return false;
                            }
                            else {
                                if (self.LaQuanLyTheoLo() === true && _quanlytheolohang === false) {
                                    ShowMessage_Danger("Sản phẩm đã được quản lý theo lô, không thể thay đổi ngược lại");
                                    Enable_btnSave();
                                    return false;
                                }
                                else {
                                    ajaxHelper(DMHangHoaUri + "Check_MaHangHoaExistDVT?maHangHoa=" + commonStatisJs.URLEncoding(strMaHangHoa) + '&idhangHoa=' + _id, 'POST').done(function (data) {
                                        if (data) {
                                            ShowMessage_Danger("Mã hàng hóa đã tồn tại");
                                            $('#txtMaHangHoa1').focus();
                                            Enable_btnSave();
                                            return false;
                                        } else {
                                            ajaxHelper(DMHangHoaUri + 'getThuocTinhGhepChuoiEdit?idcungloai=' + self.IDCungLoai() + '&chuoi=' + chuoi + '&idhanghoa=' + _id, 'GET').done(function (dataCheckTT) {
                                                if (dataCheckTT > 0) {
                                                    ShowMessage_Danger("Thuộc tính đã tồn tại trong hàng hóa cùng loại");
                                                    Enable_btnSave();
                                                    return false;
                                                }
                                                else {
                                                    if (dvtQuydois.length === 0) {
                                                        calljaxEditHHCungLoai(myData);
                                                    }
                                                    else {
                                                        var _found = 0;
                                                        var j = 0;
                                                        var _foundDVT = 0;
                                                        var _maHH = "";
                                                        var _maHHDVT = "";
                                                        var arrMaHHDVT = [];
                                                        function checkmadvt() {
                                                            if (j < dvtQuydois.length) {
                                                                var madvt = dvtQuydois[j].MaHangHoa;
                                                                j++;
                                                                if (madvt === strMaHangHoa && madvt !== "") {
                                                                    _found = _found + 1;
                                                                    _maHH = _maHH + madvt + " ";
                                                                    checkmadvt();
                                                                }
                                                                else if (madvt !== strMaHangHoa && madvt !== "") {
                                                                    ajaxHelper(DMHangHoaUri + "Check_MaHangHoaExistDVT?maHangHoa=" + commonStatisJs.URLEncoding(madvt) + '&idhangHoa=' + _id, 'POST').done(function (dataDVT) {
                                                                        if (dataDVT) {
                                                                            _foundDVT = _foundDVT + 1;
                                                                            _maHHDVT = _maHHDVT + madvt + " ";
                                                                        }
                                                                        checkmadvt();
                                                                    })
                                                                }
                                                                else {
                                                                    checkmadvt();
                                                                }
                                                            }
                                                            else {
                                                                edithh();
                                                            }
                                                        }
                                                        checkmadvt();
                                                        function edithh() {
                                                            if (_found > 0) {
                                                                ShowMessage_Danger("Mã đơn vị: " + _maHH + " " + "trùng mã hàng hóa");
                                                                Enable_btnSave();
                                                                return false;
                                                            }
                                                            else if (_foundDVT > 0) {
                                                                ShowMessage_Danger("Mã đơn vị: " + _maHHDVT + " " + "đã tồn tại.");
                                                                Enable_btnSave();
                                                                return false;
                                                            }
                                                            else {
                                                                calljaxEditHHCungLoai(myData);
                                                            }
                                                        }
                                                    }
                                                }
                                            })
                                        }
                                    });
                                }
                            }
                        }
                    }
                });
            }
        });
    };

    self.ThemMoiHangHoaLS = function (item) {
        let sLoai = 'hàng hóa';
        switch (parseInt(self.newHangHoa().LoaiHangHoa())) {
            case 2:
                sLoai = 'dịch vụ';
                break;
            case 3:
                sLoai = 'combo';
                break;
        }
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Danh mục hàng hóa",
            NoiDung: "Thêm mới ".concat(sLoai, ': ', item.MaHangHoa + ", tên: " + item.TenHangHoa + ", giá bán: " + formatNumber3Digit(item.GiaBan) + ", giá vốn: " + formatNumber3Digit(item.GiaVon) + ", tồn kho: " + item.TonKho + ", được bán trực tiếp: " + (item.DuocBanTrucTiep === true ? "có" : "không") + ", quản lý theo lô: " + (item.QuanLyTheoLoHang === true ? "có" : "không")),
            NoiDungChiTiet: "Thêm mới " + sLoai + " <a onclick=\"FindMaHangHoa('" + item.MaHangHoa + "')\">" + item.MaHangHoa + " </a>"
                .concat("<br /> - Tên ", sLoai, ": ", item.TenHangHoa,
                    "<br /> - Giá bán: " + formatNumber3Digit(item.GiaBan),
                    "<br /> - Giá vốn: " + formatNumber3Digit(item.GiaVon),
                    "<br /> - Tồn kho: " + item.TonKho,
                    "<br /> - Được bán trực tiếp: ", self.newHangHoa().DuocBanTrucTiep() === true ? "có" : "không",
                    "<br /> - Quản lý theo lô: ", item.QuanLyTheoLoHang === true ? "có" : "không",
                    "<br /> - Tính hoa hồng trước chiết khấu: ", self.TinhHoaHongTruocCK() ? "có" : "không",
                    "<br /> - Hàng hóa thuộc danh mục xe: ", self.HangHoa_LaXe() ? 'có' : 'không',
                    "<br /> - Người tạo: ", VHeader.UserLogin,
                ),
            LoaiNhatKy: 1 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        Insert_NhatKyThaoTac_1Param(objDiary);
    }

    self.ThemMoiNhomHangHoaLS = function (tennhom) {
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Danh mục hàng hóa",
            NoiDung: "Thêm mới nhóm hàng hóa: " + tennhom,
            NoiDungChiTiet: "Thêm mới nhóm hàng hóa: " + tennhom,
            LoaiNhatKy: 1 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        Insert_NhatKyThaoTac_1Param(objDiary);
    }

    self.SuaNhomHangHoaLS = function (tennhom) {
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Danh mục hàng hóa",
            NoiDung: "Cập nhật nhóm hàng hóa: " + self.TenNhomHHLS() + " thành " + tennhom,
            NoiDungChiTiet: "Cập nhật nhóm hàng hóa: " + self.TenNhomHHLS() + " thành " + tennhom,
            LoaiNhatKy: 2 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        Insert_NhatKyThaoTac_1Param(objDiary);
    }

    self.SuaHangHoaLS = function () {
        var sLoai = 'hàng hóa';
        switch (parseInt(self.newHangHoa().LoaiHangHoa())) {
            case 1:
                sLoai = 'hàng hóa';
                break;
            case 2:
            case 3:
                sLoai = 'dịch vụ';
                break;
        }

        let objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Danh mục hàng hóa",
            NoiDung: "Cập nhật " + sLoai + ':' + self.newHangHoa().MaHangHoa() + ", tên: " + self.newHangHoa().TenHangHoa() + ", giá bán: " + formatNumber3Digit(self.newHangHoa().GiaBan()) + ", giá vốn: " + formatNumber3Digit(self.newHangHoa().GiaVon()) + ", tồn kho: " + self.newHangHoa().TonKho() + ", được bán trực tiếp: " + (self.newHangHoa().DuocBanTrucTiep() === true ? "có" : "không") + ", quản lý theo lô: " + (self.newHangHoa().QuanLyTheoLoHang() === true ? "có" : "không"),
            NoiDungChiTiet: "Cập nhật ".concat(sLoai, ':' + " <a onclick=\"FindMaHangHoa('" + self.newHangHoa().MaHangHoa() + "')\">"
                + self.newHangHoa().MaHangHoa(), ' </a>',
                "<br /> - Tên ", sLoai, ": ", self.newHangHoa().TenHangHoa(),
                "<br /> - Giá bán: " + formatNumber3Digit(self.newHangHoa().GiaBan()),
                "<br /> - Giá vốn: " + formatNumber3Digit(self.newHangHoa().GiaVon()),
                "<br /> - Tồn kho: " + self.newHangHoa().TonKho(),
                "<br /> - Được bán trực tiếp: ", self.newHangHoa().DuocBanTrucTiep() === true ? "có" : "không",
                "<br /> - Quản lý theo lô: ", self.newHangHoa().QuanLyTheoLoHang() === true ? "có" : "không",
                "<br /> - Tính hoa hồng trước chiết khấu: ", self.TinhHoaHongTruocCK() ? "có" : "không",
                "<br /> - Hàng hóa thuộc danh mục xe: ", self.HangHoa_LaXe() ? 'có' : 'không',
                "<br /> - Người sửa: ", VHeader.UserLogin,

                '<br /> <b> - Thông tin cũ: </b>',
                '<br />- Mã ', sLoai, ': ', self.productOld().MaHangHoa,
                '<br />- Tên ', sLoai, ': ', self.productOld().TenHangHoa,
                '<br />- Nhóm ', sLoai, ': ', self.productOld().NhomHangHoa,
                '<br />- Giá bán: ', formatNumber3Digit(self.productOld().GiaBan),
                '<br />- Giá vốn: ', formatNumber3Digit(self.productOld().GiaVon),
                '<br />- Tồn kho: ', formatNumber3Digit(self.productOld().TonKho),
                '<br />- Quản lý theo lô: ', self.productOld().QuanLyTheoLoHang,
                '<br />- Được tích điểm: ', self.productOld().DuocTichDiem,
                '<br />- Được bán trực tiếp: ', self.productOld().DuocBanTrucTiep,
                '<br />- Tính hoa hồng trước chiết khấu: ', self.productOld().HoaHongTruocChietKhau === 1 ? 'có' : 'không',
                "<br />- Hàng hóa thuộc danh mục xe: ",
                !commonStatisJs.CheckNull(self.productOld().ID_Xe) && self.productOld().ID_Xe !== const_GuidEmpty ? 'có' : 'không',
            ),
            LoaiNhatKy: 2 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        Insert_NhatKyThaoTac_1Param(objDiary);
    }

    self.XoaHangHoaLS = function (mahanghoa) {
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Danh mục hàng hóa",
            NoiDung: "Xóa hàng hóa : " + mahanghoa,
            NoiDungChiTiet: "Xóa hàng hóa có mã " + mahanghoa,
            LoaiNhatKy: 3 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        Insert_NhatKyThaoTac_1Param(objDiary);
    }

    self.XoaNhomHangHoaLS = function (mahanghoa) {
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Danh mục hàng hóa",
            NoiDung: "Xóa nhóm hàng hóa : " + mahanghoa,
            NoiDungChiTiet: "Xóa nhóm hàng hóa : " + mahanghoa,
            LoaiNhatKy: 3 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        Insert_NhatKyThaoTac_1Param(objDiary);
    };

    self.CheckThemHHCL = ko.observable(false);
    self.addHHCungLoai = function (item) {
        self.CheckThemHHCL(true);
        ajaxHelper(DMHangHoaUri + "GetDM_HangHoa?id=" + item.ID + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
            self.IDCungLoai(data.ID_HangHoaCungLoai);
            $('#modalPopuplg_HHNew').show();
            changebuttonaddnew();
            $('#txtTenHangHoa1').select();
            var lc_CTThietLap = JSON.parse(localStorage.getItem('lc_CTThietLap'));
            if (lc_CTThietLap.LoHang === false || lc_CTThietLap.LoHang === null) {
                $('#QuanLyTheoLoHangCheck').hide();
            }
            else {
                $('#QuanLyTheoLoHangCheck').show();
            }
            if (data.DM_HangHoa_Anh.length === 0) {
                self.loadavt(false);
            } else {
                self.loadavt(true);
            }
            self.DM_HangHoa_Anh(data.DM_HangHoa_Anh);
            self.ThuocTinhCuaHHEdit(data.HangHoa_ThuocTinh);
            GetAllNhomHHByLaNhomHH();
            //self.files(data.DM_HangHoa_Anh);
            data.GiaBan = formatNumber3Digit(data.GiaBan);
            data.GiaVon = formatNumber3Digit(data.GiaVon);
            data.TonKho = formatNumber3Digit(data.TonKho);
            //self.selectIDNHHAdd(data.ID_NhomHangHoa);
            self.selectIDNhomHHAddHH(data.ID_NhomHangHoa);
            for (var j = 0; j < data.DonViTinh.length; j++) {
                data.DonViTinh[j].GiaBan = formatNumber3Digit(data.DonViTinh[j].GiaBan);
            }
            data.MaHangHoa = "";
            data.TonKho = 0;
            self.DM_HangHoa_Anh([]);
            data.HangHoaCungLoaiArr = [];
            self.newHangHoa().SetData(data);

            $('#danhsachhanghoacungloai').hide();
            if (data.ID_NhomHangHoa !== null) {
                $('#choose_TenNHHAddHH').text(data.TenNhomHangHoa);
                $(function () {
                    $('span[id=spanCheckNhomAddHH_' + data.ID_NhomHangHoa + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
                });
            }
            else {
                $('#choose_TenNHHAddHH').text("---Chọn nhóm---");
            }
            $('#lstNhomHangAddHH span').each(function () {
                $(this).empty();
            });

            self.MangNhomViTriHH(data.DM_HangHoa_ViTri);
            $('#choose_ViTri input').remove();
            if (self.MangNhomViTriHH().length === 0) {
                $('#choose_ViTri').append('<input type="text" id="dllViTriHH" placeholder="Chọn vị trí hàng hóa">');
                $('#selec-all-ViTri li').each(function () {
                    $(this).find('.fa-check').remove();
                });
            }
            else {
                for (var i = 0; i < self.MangNhomViTriHH().length; i++) {
                    $('#selec-all-ViTri li').each(function () {
                        if ($(this).attr('id') === self.MangNhomViTriHH()[i].ID) {
                            $(this).find('.fa-check').remove();
                            $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                        }
                    });
                }
            }

            for (var i = 0; i < self.newHangHoa().DonViTinh().length; i++) {
                $('#txtmadv' + self.newHangHoa().DonViTinh()[i].ID).val("");
                $('.TyLeQuyDoi_DVT').removeAttr('disabled');
                self.newHangHoa().DonViTinh()[i].MaHangHoa = "";
                self.newHangHoa().DonViTinh()[i].ID = '00000000-0000-0000-0000-000000000000';
            }
            $('.themdonviclick1').hide();
            $('.hidexoathuoctinh').hide();
            if (!data.LaHangHoa) {
                showdichvu();
            }
            if (self.ThuocTinhCuaHHEdit().length > 0) {
                $('.btn-them-luu').show();
            } else {
                $('.btn-them-luu').hide();
            }
            self.booleanAdd(true);
            if (data.QuanLyTheoLoHang === true) {
                $('.dshh3').hide();
                $('.checktonkholohanghoa').hide();
            }
            else {
                $('.dshh3').show();
                $('.checktonkholohanghoa').show();
            }

            $('.ddlThuocTinh').attr('disabled', 'disabled');
        });
    };

    self.DichVu_ADD = function (formElement) {
        ajaxHelper(DMHangHoaUri + 'GioiHanSoMatHang', 'GET').done(function (data) {
            if (data) {
                ShowMessage_Danger("Cửa hàng đã đạt số mặt hàng quy định, không thể thêm mới");
            }
            else {
                var strMaHangHoa = self.newHangHoa().MaHangHoa();
                var _id = self.newHangHoa().ID();
                var _donvitinhchuan = self.newHangHoa().DonViTinhChuan();

                var check = CheckInput_Parent();
                if (check) {
                    Enable_btnSave();
                    return false;
                }

                var DM_HangHoa = GetProperties_ofHangHoa();
                var dvt = {
                    MaHangHoa: strMaHangHoa,
                    TenDonViTinh: _donvitinhchuan,
                    GiaBan: DM_HangHoa.GiaBan,
                    GiaVon: 0
                };

                var dlDV = self.newHangHoa().DinhLuongDichVu();

                var dvtQuydois = [];

                var objDVT = self.newHangHoa().DonViTinh();
                dvtQuydois = objDVT;

                if (_donvitinhchuan !== undefined && _donvitinhchuan !== "" && _donvitinhchuan !== null) {
                    for (var i = 0; i < dvtQuydois.length; i++) {
                        if (dvtQuydois[i].TenDonViTinh !== undefined) {
                            if (dvtQuydois[i].TenDonViTinh === "") {
                                ShowMessage_Danger("Nhập tên đơn vị quy đổi trước khi lưu");
                                Enable_btnSave();
                                return false;
                            }
                            else {
                                if (dvtQuydois[i].TenDonViTinh === _donvitinhchuan) {
                                    ShowMessage_Danger("Tên đơn vị tính không được trùng nhau");
                                    Enable_btnSave();
                                    return false;
                                }
                                else {
                                    for (var j = 0; j < dvtQuydois.length; j++) {
                                        if (i !== j && dvtQuydois[i].TenDonViTinh == dvtQuydois[j].TenDonViTinh) {
                                            ShowMessage_Danger("Tên đơn vị tính không được trùng nhau");
                                            Enable_btnSave();
                                            return false;
                                        }
                                    }
                                }
                            }
                        }

                        if (dvtQuydois[i].TyLeChuyenDoi == "" || dvtQuydois[i].TyLeChuyenDoi == 0) {
                            ShowMessage_Danger("Vui lòng nhập tỷ lể chuyển đổi cho đơn vị tính: " + dvtQuydois[i].TenDonViTinh);
                            Enable_btnSave();
                            return false;
                        }
                    }
                }

                var myData = {};
                myData.iddonvi = _IDchinhanh;
                myData.idnhanvien = _IDNhanVien;
                myData.objNewHH = DM_HangHoa;
                myData.objNewDVT = dvt;
                myData.listDVTQuydois = dvtQuydois;
                myData.listDLDV = dlDV;

                if (self.booleanAdd() === true) {
                    ajaxHelper(DMHangHoaUri + "Check_MaHangHoaExist?maHangHoa=" + commonStatisJs.URLEncoding(strMaHangHoa), 'POST').done(function (data) {
                        if (data) {
                            ShowMessage_Danger("Mã hàng hóa đã tồn tại");
                            $('#txtMaHangHoa1').focus();
                            Enable_btnSave();
                            return false;
                        } else {
                            if (dvtQuydois.length === 0) {
                                calljaxAddDV(myData);
                            } else {
                                var lcMaHH = JSON.parse(localStorage.getItem('lcarrMaHH'));
                                var _found = 0;
                                var _maHH = "";
                                for (var i = 0; i < dvtQuydois.length; i++) {
                                    if ($.inArray(dvtQuydois[i].MaHangHoa, lcMaHH) > -1 && dvtQuydois[i].ID === '00000000-0000-0000-0000-000000000000') {
                                        _found = _found + 1;
                                        if (dvtQuydois[i].MaHangHoa !== "") {
                                            _maHH = _maHH + dvtQuydois[i].MaHangHoa + " ";
                                        }
                                    }
                                }
                                if (_found > 0) {
                                    ShowMessage_Danger("Mã hàng hóa đơn vị: " + _maHH + " " + "đã tồn tại");
                                    Enable_btnSave();
                                    return false;
                                }
                                else {
                                    calljaxAddDV(myData);
                                }
                            }
                        }
                    });
                }
                // edit
                else {
                    myData.objNewHH.NguoiSua = _txtTenTaiKhoan;
                    ajaxHelper(DMHangHoaUri + "Check_MaHangHoaExistDVT?maHangHoa=" + commonStatisJs.URLEncoding(strMaHangHoa) + '&idhangHoa=' + _id, 'POST').done(function (data) {
                        if (data) {
                            ShowMessage_Danger("Mã hàng hóa đã tồn tại");
                            $('#txtMaHangHoa').focus();
                            Enable_btnSave();
                            return false;
                        } else {
                            if (dvtQuydois.length === 0) {
                                calljaxEditDV(myData);
                            } else {
                                var lcMaHH = JSON.parse(localStorage.getItem('lcarrMaHH'));
                                var _found = 0;
                                var _maHH = "";
                                for (var i = 0; i < dvtQuydois.length; i++) {
                                    if ($.inArray(dvtQuydois[i].MaHangHoa, lcMaHH) > -1 && dvtQuydois[i].ID === '00000000-0000-0000-0000-000000000000') {
                                        _found = _found + 1;
                                        if (dvtQuydois[i].MaHangHoa !== "") {
                                            _maHH = _maHH + dvtQuydois[i].MaHangHoa + " ";
                                        }
                                    }
                                }
                                if (_found > 0) {
                                    ShowMessage_Danger("Mã hàng hóa đơn vị: " + _maHH + " " + "đã tồn tại");
                                    Enable_btnSave();
                                    return false;
                                }
                                else {
                                    calljaxEditDV(myData);
                                }
                            }
                        }
                    });
                }
            }
        });
    };
    $("body").on('ChangeHangHoa', function () {
        SearchHangHoa();
    });

    self.checkVNDandPT = function () {
        var $this = $(event.currentTarget);
        if (self.newHangHoa().ChiPhiTinhTheoPT()) {
            if (formatNumberToFloat($this.val()) > 100) {
                $this.val(100);
                self.newHangHoa().ChiPhiThucHien(100);
            }
        }
        else {
            formatNumberObj($this);
        }
    };

    self.editChietKhauMacDinhNV = function () {
        var $this = $(event.currentTarget);
        if (self.newHangHoa().ChietKhauMD_NVTheoPT()) {
            if (formatNumberToFloat($this.val()) > 100) {
                $this.val(100);
                self.newHangHoa().ChietKhauMD_NV(100);
            }
        }
        else {
            formatNumberObj($this);
        }
    };

    self.ChietKhauMacDinhNV_ClickPtramVND = function () {
        self.newHangHoa().ChietKhauMD_NVTheoPT(!self.newHangHoa().ChietKhauMD_NVTheoPT());
        self.newHangHoa().ChietKhauMD_NV(0);
    }

    self.HoaHongDV_ClickPtramVND = function () {
        self.newHangHoa().ChiPhiTinhTheoPT(!self.newHangHoa().ChiPhiTinhTheoPT());
        self.newHangHoa().ChiPhiThucHien(0);
    }

    function PostChiTietBaoDuong(item) {
        if (self.QuanLyBaoDuong()
            || (!$.isEmptyObject(self.productOld()) && !commonStatisJs.CheckNull(self.productOld().QuanLyBaoDuong) && self.productOld().QuanLyBaoDuong === 1)) {
            var idHangHoa = item.ID;
            var lst = [], sDetails = '';
            for (let i = 0; i < self.BaoDuong_ListDetail().length; i++) {
                let itFor = self.BaoDuong_ListDetail()[i];
                itFor.LanBaoDuong = i + 1;
                itFor.ID_HangHoa = idHangHoa;
                itFor.BaoDuongLapDinhKy = 0;
                if (itFor.LapDinhKy && i === self.BaoDuong_ListDetail().length - 1) {
                    itFor.BaoDuongLapDinhKy = 1;
                }
                lst.push(itFor);

                let loai = self.BaoDuong_TypeDate().find(x => x.ID === parseInt(itFor.LoaiGiaTri)).Text;
                sDetails += ' <br /> - '.concat(' Lần ', itFor.LanBaoDuong,
                    ': Từ ', formatNumber3Digit(itFor.GiaTri), ' ', loai,
                    itFor.LapDinhKy ? ', Lặp lại cho các lần sau: có' : '');
            }

            var myData = {
                lstDetail: lst,
            }
            ajaxHelper(DMHangHoaUri + "PostChiTietBaoDuong", 'POST', myData).done(function (x) {
                if (x.res) {
                    let apply = 'không';
                    let tenNhom = '';

                    if (self.BaoDuong_ApplyAllByGroup()) {
                        apply = 'có';
                        if (commonStatisJs.CheckNull(item.ID_NhomHang) || item.ID_NhomHang.indexOf('000') > -1) {
                            if (item.LoaiHangHoa === 1) {
                                tenNhom = 'Nhóm hàng hóa mặc định';
                            }
                            else {
                                tenNhom = 'Nhóm dịch vụ mặc định';
                            }
                        }
                        tenNhom = ' <br />Tên nhóm hàng: ' + tenNhom;
                        ajaxHelper(DMHangHoaUri + "BaoDuong_InsertListDetail_ByNhomHang?idHangHoa=" + idHangHoa
                            , 'GET').done(function (x) {
                            })
                    }

                    var loai = 1, sLoai = 'Cài đặt';
                    if (!self.booleanAdd()) {
                        if (!self.QuanLyBaoDuong()) {
                            // xoa caidat baoduong
                            loai = 3;
                            sLoai = 'Xóa cài đặt';
                        }
                        else {
                            loai = 2;
                        }
                    }


                    let diary = {
                        ID_DonVi: _IDchinhanh,
                        ID_NhanVien: _IDNhanVien,
                        LoaiNhatKy: loai,
                        ChucNang: 'Quản lý bảo dưỡng',
                        NoiDung: sLoai.concat(' lịch bảo dưỡng cho hàng hóa ',
                            item.TenHangHoa, ' (', item.MaHangHoa, ')'),
                        NoiDungChiTiet: sLoai.concat(' lịch bảo dưỡng cho hàng hóa ',
                            item.TenHangHoa, ' (', item.MaHangHoa, ')', tenNhom,
                            ' <br /> <b> Nội dung chi tiết: </b>',
                            ' <br /> - Áp dụng tất cả hàng hóa thuộc nhóm: ', apply,
                            sDetails
                        )
                    }
                    Insert_NhatKyThaoTac_1Param(diary);
                }
            })
        }
    }

    function calljaxAddMultilHHCungLoai(myData) {
        var _founHHCL = 0;
        var _maHHCL = "";
        var j = 0;
        var listHHCungLoai = self.newHangHoa().HangHoaCungLoaiArr().filter(p => p.MaHangHoa !== "");
        function checkMaHHCL() {
            if (j < listHHCungLoai.length) {
                let mahanghoa = listHHCungLoai[j].MaHangHoa;
                ajaxHelper(DMHangHoaUri + "Check_MaHangHoaExist?maHangHoa=" + commonStatisJs.URLEncoding(mahanghoa), 'POST').done(function (dataDVT) {
                    if (dataDVT) {
                        _founHHCL = _founHHCL + 1;
                        _maHHCL = _maHHCL + mahanghoa + " ";
                    }
                    j++;
                    checkMaHHCL();
                });
            }
            else {
                addhhcl();
            }
        }
        checkMaHHCL();
        function addhhcl() {
            if (_founHHCL > 0) {
                ShowMessage_Danger("Mã hàng hóa cùng loại: " + _maHHCL + " " + "đã tồn tại.");
                Enable_btnSave();
                return false;
            }
            else {
                console.log('myData ', myData);
                ajaxHelper(DMHangHoaUri + "PostDM_HangHoaNhieuHangCL", 'POST', myData).done(function (x) {
                    console.log(x);
                    if (x.res) {
                        SearchHangHoa();
                        ShowMessage_Success("Thêm mới hàng hóa thành công!");
                        for (var i = 0; i < x.dataSoure.length; i++) {
                            self.InsertImage(x.dataSoure[i].ID);
                            self.ThemMoiHangHoaLS(x.dataSoure[i]);
                        }
                        PostChiTietBaoDuong(x.dataSoure[0]);
                        if (checkThemCL === 1) {
                            self.addHHCungLoai(x.dataSoure[0]);
                        }
                        else {
                            $('#modalPopuplg_HHNew').hide();
                            self.BackAdd();
                        }
                    }
                    else {
                        ShowMessage_Danger("Thêm hàng hóa thất bại");
                    }
                    Enable_btnSave();
                })
            }
        }
    }

    function calljaxAdd(myData) {
        ajaxHelper(DMHangHoaUri + "PostDM_HangHoa", 'POST', myData).done(function (x) {
            if (x.res) {
                var item = x.dataSoure;
                $("#modalPopuplg_HHNew").hide();
                PostChiTietBaoDuong(item);
                self.BackAdd();
                SearchHangHoa();
                ShowMessage_Success("Thêm mới hàng hóa thành công!");
                Enable_btnSave();
                self.InsertImage(item.ID);
                self.ThemMoiHangHoaLS(item);
                ReloadSearchHangHoa();
            }
            else {
                ShowMessage_Danger(x.mes);
            }
        }).fail(function () {
            ShowMessage_Danger("Thêm hàng hóa thất bại");
        }).always(function () {
            $("#modalPopuplg_HHNew").hide();
            self.BackAdd();
            Enable_btnSave();
        })
    }

    function calljaxAddAndAddNew(myData) {
        ajaxHelper(DMHangHoaUri + "PostDM_HangHoa", 'POST', myData).done(function (x) {
            if (x.res) {
                var item = x.dataSoure;
                PostChiTietBaoDuong(item);
                SearchHangHoa();
                self.InsertImage(item.ID);
                self.ThemMoiHangHoaLS(item);
                ReloadSearchHangHoa();
                self.themmoicapnhat();
                ShowMessage_Success("Thêm mới hàng hóa thành công");
            }
            else {
                ShowMessage_Danger(x.mess);
            }
        }).fail(function () {
            ShowMessage_Danger("Thêm hàng hóa thất bại");
        }).always(function () {
            Enable_btnSave();
        })
    }

    function calljaxAddCungLoai(myData) {
        ajaxHelper(DMHangHoaUri + "PostDM_HangHoaCungLoai", 'POST', myData).done(function (x) {
            if (x.res) {
                var item = x.dataSoure;
                PostChiTietBaoDuong(item);
                self.ThemMoiHangHoaLS(item);
                self.CheckThemHHCL(true);
                self.IDCungLoai(item.ID_HangHoaCungLoai);
                if (checkThemCL === 1) {
                    $('#modalPopuplg_HHNew').show();
                    changebuttonaddnew();
                }
                if (checkThemCL === 2) {
                    $('#modalPopuplg_HHNew').hide();
                    self.BackAdd();
                }
                self.BackAdd();
                self.booleanAdd(true);
                self.newHangHoa().MaHangHoa("");
                $('.ddlThuocTinh').attr('disabled', 'disabled');
                self.InsertImage(item.ID);
                ShowMessage_Success("Thêm mới hàng hóa thành công");
                SearchHangHoa();
            }
            else {
                ShowMessage_Danger(x.mess);
            }
            location.reload();
        }).fail(function () {
            ShowMessage_Danger("Thêm hàng hóa thất bại");
        }).always(function () {
            Enable_btnSave();
        })
    }

    function calljaxAddDV(myData) {
        ajaxHelper(DMHangHoaUri + "PostDM_DichVu", 'POST', myData).done(function (x) {
            if (x.res) {
                var item = x.dataSoure;
                PostChiTietBaoDuong(item);
                self.BackAdd();
                SearchHangHoa();
                ShowMessage_Success("Thêm dịch vụ thành công");
                self.InsertImage(item.ID);
                self.ThemMoiHangHoaLS(item);
            }
            else {
                ShowMessage_Danger(x.mess);
            }
        }).fail(function () {
            ShowMessage_Danger("Thêm dịch vụ thất bại");
            //location.reload();
        }).always(function () {
            Enable_btnSave();
        })
    }

    function calljaxEditDV(myData) {
        ajaxHelper(DMHangHoaUri + "PutDM_HangHoa", 'POST', myData).done(function (x) {
            if (x.res) {
                PostChiTietBaoDuong(myData.objNewHH);
                ShowMessage_Success("Cập nhật dịch vụ thành công");
                self.InsertImage(self.newHangHoa().ID());
                self.SuaHangHoaLS();
            }
            else {
                ShowMessage_Danger(x.mess);
            }
            SearchHangHoa();
            self.BackAdd();
        }).fail(function () {
            ShowMessage_Danger("Cập nhật hàng hóa thất bại");
        }).always(function () {
            Enable_btnSave();
        })
    }

    function calljaxEditHH(myData) {
        ajaxHelper(DMHangHoaUri + "PutDM_HangHoa", 'POST', myData).done(function (x) {
            if (x.res) {
                PostChiTietBaoDuong(myData.objNewHH);
                ShowMessage_Success("Cập nhật hàng hóa thành công");
                self.InsertImage(self.newHangHoa().ID());
                self.SuaHangHoaLS();
                itemcheckhh = '';
            }
            else {
                ShowMessage_Danger(x.mess);
            }
            SearchHangHoa();
            self.BackAdd();
        }).fail(function () {
            ShowMessage_Danger("Cập nhật hàng hóa thất bại");
            location.reload();
        }).always(function () {
            Enable_btnSave();
            self.BackAdd();
        })
    }

    function calljaxEditHHAndAddNew(myData) {
        ajaxHelper(DMHangHoaUri + "PutDM_HangHoa", 'POST', myData).done(function (x) {
            if (x.res) {
                PostChiTietBaoDuong(myData.objNewHH);
                ShowMessage_Success("Cập nhật hàng hóa thành công");
                self.InsertImage(self.newHangHoa().ID());
                self.SuaHangHoaLS();
                ReloadSearchHangHoa();
                itemcheckhh = '';
                self.themmoicapnhat();
            }
            else {
                ShowMessage_Danger(x.mess);
            }
            SearchHangHoa();
        }).fail(function () {
            ShowMessage_Danger("Cập nhật hàng hóa thất bại");
            location.reload();
        }).always(function () {
            Enable_btnSave();
        })
    }

    function calljaxEditHHCungLoai(myData) {
        ajaxHelper(DMHangHoaUri + "PutDM_HangHoa", 'POST', myData).done(function (x) {
            if (x.res) {
                PostChiTietBaoDuong(myData.objNewHH);
                self.CheckThemHHCL(true);
                ShowMessage_Success("Cập nhật hàng hóa thành công");
                self.InsertImage(self.newHangHoa().ID());
                self.SuaHangHoaLS();
                itemcheckhh = '';
            }
            else {
                ShowMessage_Danger(x.mess);
            }
            SearchHangHoa();
        }).fail(function () {
            ShowMessage_Danger("Cập nhật hàng hóa thất bại");
            location.reload();
        }).always(function () {
            Enable_btnSave();
            changebuttonaddnew();
            self.booleanAdd(true);
            self.newHangHoa().MaHangHoa("");
            self.newHangHoa().TonKho(0);
            self.DM_HangHoa_Anh([]);
            for (var i = 0; i < self.newHangHoa().DonViTinh().length; i++) {
                $('#txtmadv' + self.newHangHoa().DonViTinh()[i].ID).val("");
                $('.TyLeQuyDoi_DVT').removeAttr('disabled');
                self.newHangHoa().DonViTinh()[i].MaHangHoa = "";
                self.newHangHoa().DonViTinh()[i].ID = '00000000-0000-0000-0000-000000000000';
            }
            $('.ddlThuocTinh').attr('disabled', 'disabled');
            $('.themdonviclick1').hide();
            $('.hidexoathuoctinh').hide();
        })
    }

    self.CheckQuanLyLoHang = function () {
        self.newHangHoa().TonKho(0);
        self.newHangHoa().GiaVon(0);
        if (self.newHangHoa().QuanLyTheoLoHang() === true) {
            for (var i = 0; i < self.newHangHoa().HangHoaCungLoaiArr().length; i++) {
                self.newHangHoa().HangHoaCungLoaiArr()[i].TonKho = 0;
                self.newHangHoa().HangHoaCungLoaiArr()[i].GiaVon = 0;
            }
            self.newHangHoa().HangHoaCungLoaiArr.refresh();
            $('.dshh3').hide();
            $('.checktonkholohanghoa').hide();
        }
        else {
            $('.dshh3').show();
            $('.checktonkholohanghoa').show();
        }
        return true;
    };
    self.CheckQuanLyDVT = function () {
        if ($('#checkmanageDVT').is(':checked')) {
            $('#hideDonViTinh').show();
        }
        else {
            $('#checkmanageDVT').prop('checked', false);
            $('#hideDonViTinh').hide();
        }
        return true;
    };
    self.CheckQuanLyDVTDV = function () {
        if ($('#checkmanageDVTDV').is(':checked')) {
            $('#hideDonViTinhDV').show();
        }
        else {
            $('#checkmanageDVTDV').prop('checked', false);
            $('#hideDonViTinhDV').hide();
        }
        return true;
    };

    self.CheckQuanLyThuocTinh = function () {
        if ($('#checkmanageThuocTinh').is(':checked')) {
            $('#sanphamcothuoctinh').show();
            $('#danhsachhanghoacungloai').show();
        }
        else {
            $('#sanphamcothuoctinh').hide();
            $('#danhsachhanghoacungloai').hide();
        }
        return true;
    };

    self.addAndNewDichVu = function (formElement) {
        DichVu_ADD();
        self.reset_HangHoaDichVu();
        $('#txtTenDichVu').focus();
    }
    self.reset_HangHoaDichVu = function () {
        self.newHangHoa(new FormModel_HangHoaDichVu());
    }


    self.exportToExcelHangHoas = function () {
        //tableToExcel('tblDanhMucHangHoa', 'dmHangHoas.xls');
        var data = localStorage.getItem('lcExportHangHoa');
        if (data != null) {
            data = JSON.parse(data);
            alasql("SELECT [MaHangHoa] AS [Mã hàng hóa], " +
                " [TenHangHoa] AS [Tên hàng hóa], " +
                " [TenNhomHangHoa] AS [Nhóm hàng], " +
                " [LoaiHangHoa] AS [Loại hàng], " +
                " [GiaBan] AS [Giá bán], " +
                " [GiaVon] AS [Giá vốn], " +
                " [TonKho] AS [Tồn kho], " +
                " [TrangThai] AS [Trạng thái] " +
                " INTO XLSX('HangHoa.xlsx', { headers: true }) FROM ? ", [data]);
        }
        localStorage.removeItem('lcExportHangHoa');
    }

    // import from excel dmhanghoa
    self.importFromExcelHangHoas = function () {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('HangHoa_Import', lc_CTQuyen) > -1) {
            $("#fileLoader").click();
        } else {
            ShowMessage_Danger("Không có quyền Import hàng hóa dịch vụ");
            return false;
        }

    }


    //phan trang
    self.pageSizes = [10, 20, 30];
    self.pageSizesTK = ko.observableArray([20, 50, 100, 200, 500])
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.pageSizeTK = ko.observable(50);
    self.currentPage = ko.observable(0);
    self.currentPageTK = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();
    self.fromitemtk = ko.observable(1);
    self.toitemtk = ko.observable();
    self.arrPagging = ko.observableArray();

    self.PageCount = ko.observable();
    self.TotalRecord = ko.observable(0);
    self.PageCountKK = ko.observable();
    self.TotalRecordKK = ko.observable(0);

    var index = -1;
    self.selectIDHH = ko.observable();
    self.TonKhos = ko.observableArray();
    self.ListTonTheoLo = ko.observableArray();
    self.AnhDaiDien = ko.observableArray();
    self.HangHoaCungLoai = ko.observableArray();
    self.checkboxCungLoai = ko.observable(false);
    self.IDHangHoaCheckTonLo = ko.observable();

    function SetDefaultActiveTab0(elm) {
        var li = $(elm).find('.nav-tabs').children('li');
        li.each(function () {
            $(this).removeClass('active');
            let href = $(this).children('a').attr('href');
            $('' + href).removeClass('active');
        })
        li.eq(0).addClass('active');
        var tabcontent = li.eq(0).children('a').attr('href');
        $('' + tabcontent).addClass('active');
    }

    self.LoHangHoas = ko.observableArray();

    async function GetHangCungLoai_byID(idCungLoai) {
        if (commonStatisJs.CheckNull(idCungLoai)) {
            return [];
        }
        let xx = await $.getJSON(DMHangHoaUri + 'GetHangCungLoai_byID?idCungLoai=' + idCungLoai + '&idChiNhanh=' + VHeader.IdDonVi).done(function () { })
            .then(function (x) {
                if (x.res) {
                    return x.dataSoure;
                }
                return [];
            })
        return xx;
    }

    self.loadTheKho = async function (item, e) {
        self.Tab_Active(0);
        SetQuanLyBaoDuong(item);

        let lstCungLoai = await GetHangCungLoai_byID(item.ID_HangHoaCungLoai);
        self.HangHoaCungLoai(lstCungLoai);

        var $this = $(e.currentTarget);
        var tr = '';
        // co hang cungloai
        if (item.CountCungLoai > 1) {
            tr = $this.closest('tr').next().find('.same-product');
        }
        else {
            tr = $this.closest('tr').next().find('.js-thongtinkhicothanhphan');
        }
        SetDefaultActiveTab0(tr);

        if (itemcheckhh !== item.ID) {
            itemcheckhh = item.ID;
            self.IDHangHoaCheckTonLo(item.ID);
            $('#txtSearchLo' + item.ID).val("");
            self.CheckConHang("0");

            if (item.LaChaCungLoai !== false) {
                if (checkopenhide !== 1) {
                    $('.table-HH').gridLoader();
                }
            }
            else {
                sleep(200).then(() => { SetHeightShowDetail($(e.currentTarget), self.pageSize()); });
            }

            ajaxHelper(DMHangHoaUri + 'GetListTonKho?id=' + item.ID + '&idnhanvien=' + _IDNhanVien, 'GET').done(function (data) {
                self.TonKhos(data);
            });

            if (item.QuanLyTheoLoHang === true) {
                ajaxHelper(DMHangHoaUri + 'ListTonTheoLoHang?iddonvi=' + _IDchinhanh + '&idhanghoa=' + item.ID, 'GET').done(function (data) {
                    self.ListTonTheoLo(data);
                });
            }
            else {
                self.ListTonTheoLo([]);
            }
            ajaxHelper(DMHangHoaUri + 'GetListAnh/' + item.ID, 'GET').done(function (data) {
                self.DM_HangHoa_Anh([]);
                self.AnhDaiDien(undefined);
                self.DM_HangHoa_Anh(data);
                self.AnhDaiDien(data[0]);
            });
            if (self.DM_HangHoa_Anh().length > 5) {
                $('.imagenext').css('display', 'block');
            }
            $(document).ready(function () {
                $('.bxslider').bxSlider({
                    slideWidth: 60,
                    minSlides: 1,
                    maxSlides: 4,
                    slideMargin: 4
                });
            });

            var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
            if ($.inArray('HangHoa_Xoa', lc_CTQuyen) > -1) {
                $('.xoaHH').show();
            }
            if ($.inArray('HangHoa_XemDS', lc_CTQuyen) > -1) {
                $('.inmavach').show();
            }

            self.TheKhos([]);
            self.currentPageTK(0);
            self.selectIDHH(item.ID);
            if (item.QuanLyTheoLoHang === false) {
                searchTheKho();
            }
            GetChietKhauNV_byIDQuiDoi(item.ID_DonViQuiDoi);
            self.LoadThanhPhanHH(item);
        }
    };

    self.ListThanhPhans = ko.observableArray();
    self.LoadThanhPhanHH = function (item) {
        ajaxHelper(DMHangHoaUri + 'GetAllDinhLuongDichVu?iddvqd=' + item.ID_DonViQuiDoi + '&iddonvi=' + _IDchinhanh, 'GET').done(function (x) {
            if (x.res) {
                self.ListThanhPhans(x.dataSoure);
            }
            else {
                self.ListThanhPhans([]);
            }
        });
    };

    function checkhanghoacungloai(item) {
        if ($('#' + item.ID_HangHoaCungLoai).find('#' + item.ID_DonViQuiDoi).is(':checked')) {
            $('#cungl' + item.ID_HangHoaCungLoai).find('.prev-tr-hide1').find('.cmcheck input[type="checkbox"]').each(function () {
                $(this).prop('checked', true);
            });
        }
    }

    self.loadTheKhoCungLoai = function (item) {
        self.TheKhos([]);
        self.IDHangHoaCheckTonLo(item.ID);
        $('#txtSearchLo' + item.ID).val("");
        self.CheckConHang("0");

        var $this = $(event.currentTarget);
        var tr = $this.closest('tr').next();
        SetDefaultActiveTab0(tr);

        ajaxHelper(DMHangHoaUri + 'GetListTonKho?id=' + item.ID + '&idnhanvien=' + _IDNhanVien, 'GET').done(function (data) {
            self.TonKhos(data);
        });
        if (item.QuanLyTheoLoHang === true) {
            ajaxHelper(DMHangHoaUri + 'ListTonTheoLoHang?iddonvi=' + _IDchinhanh + '&idhanghoa=' + item.ID, 'GET').done(function (data) {
                self.ListTonTheoLo(data);
            });
        }
        else {
            self.ListTonTheoLo([]);
        }
        ajaxHelper(DMHangHoaUri + 'GetListAnh/' + item.ID, 'GET').done(function (data) {
            self.DM_HangHoa_Anh([]);
            self.AnhDaiDien(undefined);
            self.DM_HangHoa_Anh(data);
            self.AnhDaiDien(data[0]);
        });
        if (self.DM_HangHoa_Anh().length > 5) {
            $('.imagenext').css('display', 'block');
        }
        $(document).ready(function () {
            $('.bxslider').bxSlider({
                slideWidth: 60,
                minSlides: 1,
                maxSlides: 4,
                slideMargin: 4
            });
        });

        self.currentPageTK(0);
        self.selectIDHH(item.ID);
        if (item.QuanLyTheoLoHang === false) {
            searchTheKho();
        }
        if ($.inArray('HangHoa_Xoa', lc_CTQuyen) > -1) {
            $('.xoaHH').show();
        }
        self.LoadThanhPhanHH(item);
    };

    self.SearchLoHang = function (item, event) {
        var objCT = [];
        var txtSearch = $(event.target).val();
        ajaxHelper(DMHangHoaUri + 'ListTonTheoLoHang?iddonvi=' + _IDchinhanh + '&idhanghoa=' + self.IDHangHoaCheckTonLo(), 'GET').done(function (data) {
            self.ListTonTheoLo(data);
            if (!commonStatisJs.CheckNull(txtSearch)) {
                txtSearch = txtSearch.trim().toLowerCase();
            }
            switch (self.CheckConHang()) {
                case "0":
                    if (txtSearch !== "") {
                        for (var i = 0; i < data.length; i++) {
                            if (data[i].MaLoHang.toLowerCase().includes(txtSearch) === true) {
                                objCT.push(data[i]);
                            }
                        }
                        self.ListTonTheoLo(objCT);
                    }
                    break;
                case "1":
                    if (txtSearch !== "") {
                        for (var i = 0; i < data.length; i++) {
                            if (data[i].TonKho > 0 && data[i].MaLoHang.toLowerCase().includes(txtSearch) === true) {
                                objCT.push(data[i]);
                            }
                        }
                    }
                    else {
                        for (var i = 0; i < data.length; i++) {
                            if (data[i].TonKho > 0) {
                                objCT.push(data[i]);
                            }
                        }

                    }
                    self.ListTonTheoLo(objCT);
                    break;
                case "2":
                    if (txtSearch !== "") {
                        for (var i = 0; i < data.length; i++) {
                            if (data[i].TonKho <= 0 && data[i].MaLoHang.toLowerCase().includes(txtSearch) === true) {
                                objCT.push(data[i]);
                            }
                        }
                    }
                    else {
                        for (var i = 0; i < data.length; i++) {
                            if (data[i].TonKho <= 0) {
                                objCT.push(data[i]);
                            }
                        }

                    }
                    self.ListTonTheoLo(objCT);
                    break;
            }

        })
    }

    self.ListCheckLo = ko.observableArray([
        { name: "Tất cả", value: "0" },
        { name: "Còn hàng", value: "1" },
        { name: "Hết hàng", value: "2" },
    ]);
    self.CheckConHang = ko.observable();
    self.CheckConHang.subscribe(function (val) {
        var objCT = [];
        var txtSearch = $('#txtSearchLo' + self.IDHangHoaCheckTonLo()).val();
        if (self.IDHangHoaCheckTonLo() !== undefined) {
            ajaxHelper(DMHangHoaUri + 'ListTonTheoLoHang?iddonvi=' + _IDchinhanh + '&idhanghoa=' + self.IDHangHoaCheckTonLo(), 'GET').done(function (data) {
                self.ListTonTheoLo(data);
                switch (val) {
                    case "0":
                        if (txtSearch !== "") {
                            for (var i = 0; i < data.length; i++) {
                                if (data[i].MaLoHang.toLowerCase().includes(txtSearch.toLowerCase()) === true) {
                                    objCT.push(data[i]);
                                }
                            }
                            self.ListTonTheoLo(objCT);
                        }
                        break;
                    case "1":
                        if (txtSearch !== "") {
                            for (var i = 0; i < data.length; i++) {
                                if (data[i].TonKho > 0 && data[i].MaLoHang.toLowerCase().includes(txtSearch.toLowerCase()) === true) {
                                    objCT.push(data[i]);
                                }
                            }
                        }
                        else {
                            for (var i = 0; i < data.length; i++) {
                                if (data[i].TonKho > 0) {
                                    objCT.push(data[i]);
                                }
                            }

                        }
                        self.ListTonTheoLo(objCT);
                        break;
                    case "2":
                        if (txtSearch !== "") {
                            for (var i = 0; i < data.length; i++) {
                                if (data[i].TonKho <= 0 && data[i].MaLoHang.toLowerCase().includes(txtSearch.toLowerCase()) === true) {
                                    objCT.push(data[i]);
                                }
                            }
                        }
                        else {
                            for (var i = 0; i < data.length; i++) {
                                if (data[i].TonKho <= 0) {
                                    objCT.push(data[i]);
                                }
                            }

                        }
                        self.ListTonTheoLo(objCT);
                        break;
                }
            })
        }
    });

    self.PageCountTK = ko.observable();
    self.TotalRecordTK = ko.observable(0);
    var isGoToNext = false;
    function searchTheKho(isGoToNext) {
        $('.table-js-tk').gridLoader();
        ajaxHelper(DMHangHoaUri + 'GetListTheKho?currentPage=' + self.currentPageTK() + '&pageSize=' + self.pageSizeTK() + '&id=' + self.selectIDHH() + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
            self.TheKhos(data.lst);
            self.TotalRecordTK(data.Rowcount);
            self.PageCountTK(data.pageCount);
            if (data.RowErrKho !== null) {
                self.RowErrKho(data.RowErrKho);
            }
            else {
                self.RowErrKho();
            }
            $('.table-js-tk').gridLoader({ show: false });
        });
    };

    self.PageList_Display_TheKho = ko.computed(function () {
        var arrPage = [];

        var allPage = self.PageCountTK();
        var currentPage = self.currentPageTK();

        if (allPage > 4) {

            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPageTK()) + 1;
            }
            else {
                i = self.currentPageTK();
            }

            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumberTK: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    if (currentPage == 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumberTK: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj = {
                                pageNumberTK: j + 1,
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
                            pageNumberTK: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumberTK: i,
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
                        pageNumberTK: i + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }
        return arrPage;
    });

    self.PageResults = ko.computed(function () {
        if (self.HangHoas() !== null) {

            self.fromitem((self.currentPage() * self.pageSize()) + 1);

            if (((self.currentPage() + 1) * self.pageSize()) > self.HangHoas().length) {
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

    self.PageResultTKs = ko.computed(function () {
        if (self.TheKhos() !== null) {

            self.fromitemtk((self.currentPageTK() * self.pageSizeTK()) + 1);

            if (((self.currentPageTK() + 1) * self.pageSizeTK()) > self.TheKhos().length) {
                var fromItem = (self.currentPageTK() + 1) * self.pageSizeTK();
                if (fromItem < self.TotalRecordTK()) {
                    self.toitemtk((self.currentPageTK() + 1) * self.pageSizeTK());
                }
                else {
                    self.toitemtk(self.TotalRecordTK());
                }
            } else {
                self.toitemtk((self.currentPageTK() * self.pageSizeTK()) + self.pageSizeTK());
            }
        }
    });

    self.PageResultKKs = ko.computed(function () {
        if (self.KiemKhos() !== null) {

            self.fromitem((self.currentPage() * self.pageSize()) + 1);

            if (((self.currentPage() + 1) * self.pageSize()) > self.KiemKhos().length) {
                var fromItem = (self.currentPage() + 1) * self.pageSize();
                if (fromItem < self.TotalRecordKK()) {
                    self.toitem((self.currentPage() + 1) * self.pageSize());
                }
                else {
                    self.toitem(self.TotalRecordKK());
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
        if (loaiHoaDon === "9") {
            SearchKiemKho();
        }
        else {
            SearchHangHoa();
        }
    };

    self.ResetCurrentPageTheKho = function (item) {
        if (item.QuanLyTheoLoHang) {
            self.currentPageLH(0);
            SeartTheKhoByMaLoHang();
        }
        else {
            self.currentPageTK(0);
            searchTheKho();
        }
    };

    self.VisibleStartPageTheKho = ko.computed(function () {
        if (self.PageList_Display_TheKho().length > 0) {
            return self.PageList_Display_TheKho()[0].pageNumberTK !== 1;
        }
    });

    self.VisibleEndPageTheKho = ko.computed(function () {
        if (self.PageList_Display_TheKho().length > 0) {
            return self.PageList_Display_TheKho()[self.PageList_Display_TheKho().length - 1].pageNumberTK !== self.PageCountTK();
        }
    });

    self.GoToPage = function (page) {
        if (page.pageNumberTK !== '.') {
            self.currentPageTK(page.pageNumberTK - 1);
            searchTheKho();
        }
    };
    self.GetClass = function (page) {
        return ((page.pageNumberTK - 1) === self.currentPageTK()) ? "click" : "";
    };

    self.StartPageTheKho = function () {
        self.currentPageTK(0);
        searchTheKho();
    }
    self.BackPageTheKho = function () {
        if (self.currentPageTK() > 1) {
            self.currentPageTK(self.currentPageTK() - 1);
            searchTheKho();
        }
    }
    self.GoToNextPageTheKho = function () {
        if (self.currentPageTK() < self.PageCountTK() - 1) {
            self.currentPageTK(self.currentPageTK() + 1);
            searchTheKho();
        }
    }
    self.EndPageTheKho = function () {
        if (self.currentPageTK() < self.PageCountTK() - 1) {
            self.currentPageTK(self.PageCountTK() - 1);
            searchTheKho();
        }
    }

    self.pageSizesLH = ko.observableArray([20, 50, 100, 200, 500]);
    self.pageSizeLH = ko.observable(self.pageSizesLH[0]);
    self.currentPageLH = ko.observable(0);
    self.fromitemlh = ko.observable(1);
    self.toitemlh = ko.observable();
    self.TotalRecordLH = ko.observable();
    self.PageCountLH = ko.observable();
    //phân trang lô hàng hóa
    self.ListTheKhoByLoHang = ko.observableArray();
    self.selectIDLoHang = ko.observable();

    self.ClickTheKhoByLoHang = function (item) {
        self.selectIDLoHang(item.ID_LoHang);
        self.currentPageLH(0);
        SeartTheKhoByMaLoHang();
    }


    function SeartTheKhoByMaLoHang(isGoToNext) {
        ajaxHelper(DMHangHoaUri + 'GetListTheKhoByMaLoHang?currentPage=' + self.currentPageLH() + '&pageSize=' + self.pageSizeLH() + '&idlohang=' + self.selectIDLoHang() + '&iddonvi=' + _IDchinhanh + '&idhanghoa=' + self.IDHangHoaCheckTonLo(), 'GET').done(function (data) {
            self.ListTheKhoByLoHang(data.lst);
            self.TotalRecordLH(data.Rowcount);
            self.PageCountLH(data.pageCount);
        });
    };

    self.PageList_Display_TheKhoLoHang = ko.computed(function () {
        var arrPage = [];

        var allPage = self.PageCountLH();
        var currentPage = self.currentPageLH();

        if (allPage > 4) {

            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPageLH()) + 1;
            }
            else {
                i = self.currentPageLH();
            }

            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumberLH: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    if (currentPage == 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumberLH: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj = {
                                pageNumberLH: j + 1,
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
                            pageNumberLH: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumberLH: i,
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
                        pageNumberLH: i + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }
        return arrPage;
    });

    self.PageResultLHs = ko.computed(function () {
        if (self.ListTheKhoByLoHang() !== null) {

            self.fromitemlh((self.currentPageLH() * self.pageSizeLH()) + 1);

            if (((self.currentPageLH() + 1) * self.pageSizeLH()) > self.ListTheKhoByLoHang().length) {
                var fromItem = (self.currentPageLH() + 1) * self.pageSizeLH();
                if (fromItem < self.TotalRecordLH()) {
                    self.toitemlh((self.currentPageLH() + 1) * self.pageSizeLH());
                }
                else {
                    self.toitemlh(self.TotalRecordLH());
                }
            } else {
                self.toitemlh((self.currentPageLH() * self.pageSizeLH()) + self.pageSizeLH());
            }
        }
    });

    self.ResetCurrentPageLoHang = function () {
        self.currentPageLH(0);
        SeartTheKhoByMaLoHang();
    };

    self.VisibleStartPageLoHang = ko.computed(function () {
        if (self.PageList_Display_TheKhoLoHang().length > 0) {
            return self.PageList_Display_TheKhoLoHang()[0].pageNumberLH !== 1;
        }
    });

    self.VisibleEndPageLoHang = ko.computed(function () {
        if (self.PageList_Display_TheKhoLoHang().length > 0) {
            return self.PageList_Display_TheKhoLoHang()[self.PageList_Display_TheKhoLoHang().length - 1].pageNumberLH !== self.PageCountLH();
        }
    });

    self.GoToPageLoHang = function (page) {
        if (page.pageNumberLH !== '.') {
            self.currentPageLH(page.pageNumberLH - 1);
            SeartTheKhoByMaLoHang();
        }
    };
    self.GetClassLoHang = function (page) {
        return ((page.pageNumberLH - 1) === self.currentPageLH()) ? "click" : "";
    };

    self.StartPageLoHang = function () {
        self.currentPageLH(0);
        SeartTheKhoByMaLoHang();
    }
    self.BackPageLoHang = function () {
        if (self.currentPageLH() > 1) {
            self.currentPageLH(self.currentPageLH() - 1);
            SeartTheKhoByMaLoHang();
        }
    }
    self.GoToNextPageLoHang = function () {
        if (self.currentPageLH() < self.PageCountLH() - 1) {
            self.currentPageLH(self.currentPageLH() + 1);
            SeartTheKhoByMaLoHang();
        }
    }
    self.EndPageLoHang = function () {
        if (self.currentPageLH() < self.PageCountLH() - 1) {
            self.currentPageLH(self.PageCountLH() - 1);
            SeartTheKhoByMaLoHang();
        }
    }

    //phân trang hàng hóa kiểm kho tất cả
    self.pageSizesAll = [10, 20, 30, 40, 50, 100];
    self.pageSizeAll = ko.observable(100);
    self.currentPageAll = ko.observable(0);
    self.fromitemAll = ko.observable(1);
    self.toitemAll = ko.observable();
    self.arrPaggingAll = ko.observableArray();

    self.PageCountAll = ko.computed(function () {
        if (self.pageSizeAll()) {
            if (self.arrPaggingAll() !== null) {
                return Math.ceil(self.arrPaggingAll().length / self.pageSizeAll());
            }
            else {
                return 0;
            }
        }
        else {
            return 1;
        }
    });

    self.filteredHHAll = ko.computed(function () {
        var arrFilter = ko.utils.arrayFilter(self.newKiemKho().BH_KiemKho_ChiTiet(), function (prod) {
            var chon = true;
            return chon;
        });
        if (arrFilter !== null) {
            self.arrPaggingAll(arrFilter);

            self.fromitemAll((self.currentPageAll() * self.pageSizeAll()) + 1);

            if (((self.currentPageAll() + 1) * self.pageSizeAll()) > arrFilter.length) {
                self.toitemAll(arrFilter.length)
            } else {
                self.toitemAll((self.currentPageAll() * self.pageSizeAll()) + self.pageSizeAll());
            }

            if (self.PageCountAll() > 1) {
                if (self.currentPageAll() === self.PageCountAll()) {
                    self.currentPageAll(0);
                }

                return arrFilter.slice(self.currentPageAll() * self.pageSizeAll(), (self.currentPageAll() * self.pageSizeAll()) + self.pageSizeAll());
            }
            else {
                return arrFilter;
            }
        }
    });


    self.PageListAll = ko.computed(function () {
        var arrPage = [];

        var allPage = self.PageCountAll();
        var currentPage = self.currentPageAll();
        if (allPage > 4) {

            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPageAll()) + 1;
            }
            else {
                i = self.currentPageAll();
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
        return arrPage;
    });

    self.GoToPageAll = function (page) {
        if (page.pageNumber !== '.') {
            self.currentPageAll(page.pageNumber - 1);
        }
        self.loadData();
    };

    self.GetClassAll = function (page) {
        return ((page.pageNumber - 1) === self.currentPageAll()) ? "click" : "";
    };
    self.VisibleStartPageAll = ko.computed(function () {
        if (self.PageListAll().length > 0) {
            return self.PageListAll()[0].pageNumber !== 1;
        }
    });

    self.VisibleEndPageAll = ko.computed(function () {
        if (self.PageListAll().length > 0) {
            return self.PageListAll()[self.PageListAll().length - 1].pageNumber !== self.PageCountAll();
        }
    });
    self.StartPageAll = function () {
        self.currentPageAll(0);
        self.loadData();
    }

    self.BackPageAll = function () {
        if (self.currentPageAll() > 1) {
            self.currentPageAll(self.currentPageAll() - 1);
        }
        self.loadData();
    }

    self.GoToNextPageAll = function () {
        if (self.currentPageAll() < self.PageCountAll() - 1) {
            self.currentPageAll(self.currentPageAll() + 1);
        }
        self.loadData();
    }

    self.EndPageAll = function () {
        if (self.currentPageAll() < self.PageCountAll() - 1) {
            self.currentPageAll(self.PageCountAll() - 1);
        }
        self.loadData();
    }
    //phân trang hàng hóa Khớp kiểm kho
    self.pageSizesKhop = [10, 20, 30, 40, 50];
    self.pageSizeKhop = ko.observable(self.pageSizesKhop[0]);
    self.currentPageKhop = ko.observable(0);
    self.fromitemKhop = ko.observable(1);
    self.toitemKhop = ko.observable();
    self.arrPaggingKhop = ko.observableArray();

    self.PageCountKhop = ko.computed(function () {
        if (self.pageSizeKhop()) {
            if (self.arrPaggingKhop() !== null) {
                return Math.ceil(self.arrPaggingKhop().length / self.pageSizeKhop());
            }
            else {
                return 0;
            }
        }
        else {
            return 1;
        }
    });

    self.filteredHHKhop = ko.computed(function () {
        var arrFilter = ko.utils.arrayFilter(self.SLKhops(), function (prod) {
            var chon = true;
            return chon;
        });
        if (arrFilter !== null) {
            self.arrPaggingKhop(arrFilter);

            self.fromitemKhop((self.currentPageKhop() * self.pageSizeKhop()) + 1);

            if (((self.currentPageKhop() + 1) * self.pageSizeKhop()) > arrFilter.length) {
                self.toitemKhop(arrFilter.length)
            } else {
                self.toitemKhop((self.currentPageKhop() * self.pageSizeKhop()) + self.pageSizeKhop());
            }

            if (self.PageCountKhop() > 1) {
                if (self.currentPageKhop() === self.PageCountKhop()) {
                    self.currentPageKhop(0);
                }

                return arrFilter.slice(self.currentPageKhop() * self.pageSizeKhop(), (self.currentPageKhop() * self.pageSizeKhop()) + self.pageSizeKhop());
            }
            else {
                return arrFilter;
            }
        }
    });


    self.PageListKhop = ko.computed(function () {
        var arrPage = [];

        var allPage = self.PageCountKhop();
        var currentPage = self.currentPageKhop();

        if (allPage > 4) {

            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPageKhop()) + 1;
            }
            else {
                i = self.currentPageKhop();
            }

            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumberKhop: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    if (currentPage == 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumberKhop: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj = {
                                pageNumberKhop: j + 1,
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
                            pageNumberKhop: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumberKhop: i,
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
                        pageNumberKhop: i + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }
        return arrPage;
    });

    self.pageKhop = ko.observable(0);


    self.GoToPageKhop = function (page) {
        if (page.pageNumberKhop !== '.') {
            self.currentPageKhop(page.pageNumberKhop - 1);
            self.pageKhop(self.currentPageKhop());
        }
        self.loadData();
    };

    self.GetClassKhop = function (page) {
        return ((page.pageNumberKhop - 1) === self.currentPageKhop()) ? "click" : "";
    };
    self.VisibleStartPageKhop = ko.computed(function () {
        if (self.PageListKhop().length > 0) {
            return self.PageListKhop()[0].pageNumberKhop !== 1;
        }
    });

    self.VisibleEndPageKhop = ko.computed(function () {
        if (self.PageListKhop().length > 0) {
            return self.PageListKhop()[self.PageListKhop().length - 1].pageNumberKhop !== self.PageCountKhop();
        }
    });
    self.StartPageKhop = function () {
        self.currentPageKhop(0);
        self.pageKhop(self.currentPageKhop());
        self.loadData();
    }


    self.BackPageKhop = function () {
        self.pageKhop(self.currentPageKhop());
        if (self.pageKhop() > 1) {
            self.pageKhop(self.pageKhop() - 1);
            self.currentPageKhop(self.pageKhop());
        }
        self.loadData();
    }

    self.GoToNextPageKhop = function () {
        self.pageKhop(self.currentPageKhop());
        if (self.pageKhop() < self.PageCountKhop() - 1) {
            self.pageKhop(self.pageKhop() + 1);
            self.currentPageKhop(self.pageKhop());
        }
        self.loadData();
    }

    self.EndPageKhop = function () {
        if (self.currentPageKhop() < self.PageCountKhop() - 1) {
            self.currentPageKhop(self.PageCountKhop() - 1);
            self.pageKhop(self.currentPageKhop());
        }
        self.loadData();
    }

    //phân trang hàng hóa lệch kiểm kho
    self.pageSizesLech = [10, 20, 30, 40, 50];
    self.pageSizeLech = ko.observable(self.pageSizesLech[0]);
    self.currentPageLech = ko.observable(0);
    self.fromitemLech = ko.observable(1);
    self.toitemLech = ko.observable();
    self.arrPaggingLech = ko.observableArray();

    self.PageCountLech = ko.computed(function () {
        if (self.pageSizeLech()) {
            if (self.arrPaggingLech() !== null) {
                return Math.ceil(self.arrPaggingLech().length / self.pageSizeLech());
            }
            else {
                return 0;
            }
        }
        else {
            return 1;
        }
    });

    self.filteredHHLech = ko.computed(function () {
        var arrFilter = ko.utils.arrayFilter(self.SLLechs(), function (prod) {
            var chon = true;
            return chon;
        });
        if (arrFilter !== null) {
            self.arrPaggingLech(arrFilter);

            self.fromitemLech((self.currentPageLech() * self.pageSizeLech()) + 1);

            if (((self.currentPageLech() + 1) * self.pageSizeLech()) > arrFilter.length) {
                self.toitemLech(arrFilter.length)
            } else {
                self.toitemLech((self.currentPageLech() * self.pageSizeLech()) + self.pageSizeLech());
            }

            if (self.PageCountLech() > 1) {
                if (self.currentPageLech() === self.PageCountLech()) {
                    self.currentPageLech(0);
                }

                return arrFilter.slice(self.currentPageLech() * self.pageSizeLech(), (self.currentPageLech() * self.pageSizeLech()) + self.pageSizeLech());
            }
            else {
                return arrFilter;
            }
        }
    });


    self.PageListLech = ko.computed(function () {
        var arrPage = [];

        var allPage = self.PageCountLech();
        var currentPage = self.currentPageLech();

        if (allPage > 4) {

            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPageLech()) + 1;
            }
            else {
                i = self.currentPageLech();
            }

            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumberLech: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    if (currentPage == 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumberLech: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj = {
                                pageNumberLech: j + 1,
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
                            pageNumberLech: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumberLech: i,
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
                        pageNumberLech: i + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }
        return arrPage;
    });
    self.pagelech = ko.observable(0);
    self.GoToPageLech = function (page) {
        if (page.pageNumberLech !== '.') {
            self.currentPageLech(page.pageNumberLech - 1);
            self.pagelech(self.currentPageLech());
        }
        self.loadData();
    };

    self.GetClassLech = function (page) {
        return ((page.pageNumberLech - 1) === self.currentPageLech()) ? "click" : "";
    };
    self.VisibleStartPageLech = ko.computed(function () {
        if (self.PageListLech().length > 0) {
            return self.PageListLech()[0].pageNumberLech !== 1;
        }
    });

    self.VisibleEndPageLech = ko.computed(function () {
        if (self.PageListLech().length > 0) {
            return self.PageListLech()[self.PageListLech().length - 1].pageNumberLech !== self.PageCountLech();
        }
    });
    self.StartPageLech = function () {
        self.currentPageLech(0);
        self.pagelech(self.currentPageLech());
        self.loadData();
    }

    self.BackPageLech = function () {
        self.pagelech(self.currentPageLech());
        if (self.pagelech() > 1) {
            self.pagelech(self.pagelech() - 1);
            self.currentPageLech(self.pagelech());
        }
        self.loadData();
    }

    self.GoToNextPageLech = function () {
        self.pagelech(self.currentPageLech());
        if (self.pagelech() < self.PageCountLech() - 1) {
            self.pagelech(self.pagelech() + 1);
            self.currentPageLech(self.pagelech());
        }
        self.loadData();
    }

    self.EndPageLech = function () {
        if (self.currentPageLech() < self.PageCountLech() - 1) {
            self.currentPageLech(self.PageCountLech() - 1);
            self.pagelech(self.currentPageLech());
        }
        self.loadData();
    }


    //phân trang hàng hóa chưa kiểm kiểm kho
    self.pageSizesChuaK = [10, 20, 30, 40, 50];
    self.pageSizeChuaK = ko.observable(self.pageSizesChuaK[0]);
    self.currentPageChuaK = ko.observable(0);
    self.fromitemChuaK = ko.observable(1);
    self.toitemChuaK = ko.observable();
    self.arrPaggingChuaK = ko.observableArray();

    self.PageCountChuaK = ko.computed(function () {
        if (self.pageSizeChuaK()) {
            if (self.arrPaggingChuaK() !== null) {
                return Math.ceil(self.arrPaggingChuaK().length / self.pageSizeChuaK());
            }
            else {
                return 0;
            }
        }
        else {
            return 1;
        }
    });

    self.filteredHHChuaK = ko.computed(function () {
        var arrFilter = ko.utils.arrayFilter(self.SLChuaKiems(), function (prod) {
            var chon = true;
            return chon;
        });
        if (arrFilter !== null) {
            self.arrPaggingChuaK(arrFilter);

            self.fromitemChuaK((self.currentPageChuaK() * self.pageSizeChuaK()) + 1);

            if (((self.currentPageChuaK() + 1) * self.pageSizeChuaK()) > arrFilter.length) {
                self.toitemChuaK(arrFilter.length)
            } else {
                self.toitemChuaK((self.currentPageChuaK() * self.pageSizeChuaK()) + self.pageSizeChuaK());
            }

            if (self.PageCountChuaK() > 1) {
                if (self.currentPageChuaK() === self.PageCountChuaK()) {
                    self.currentPageChuaK(0);
                }

                return arrFilter.slice(self.currentPageChuaK() * self.pageSizeChuaK(), (self.currentPageChuaK() * self.pageSizeChuaK()) + self.pageSizeChuaK());
            }
            else {
                return arrFilter;
            }
        }
    });


    self.PageListChuaK = ko.computed(function () {
        var arrPage = [];

        var allPage = self.PageCountChuaK();
        var currentPage = self.currentPageChuaK();

        if (allPage > 4) {

            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPageChuaK()) + 1;
            }
            else {
                i = self.currentPageChuaK();
            }

            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumberChuaK: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    if (currentPage == 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumberChuaK: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj = {
                                pageNumberChuaK: j + 1,
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
                            pageNumberChuaK: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumberChuaK: i,
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
                        pageNumberChuaK: i + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }
        return arrPage;
    });

    self.pagechuaK = ko.observable(0);


    self.GoToPageChuaK = function (page) {
        if (page.pageNumberChuaK !== '.') {
            self.currentPageChuaK(page.pageNumberChuaK - 1);
            self.pagechuaK(self.currentPageChuaK());
        }
        self.loadData();
    };

    self.GetClassChuaK = function (page) {
        return ((page.pageNumberChuaK - 1) === self.currentPageChuaK()) ? "click" : "";
    };
    self.VisibleStartPageChuaK = ko.computed(function () {
        if (self.PageListChuaK().length > 0) {
            return self.PageListChuaK()[0].pageNumberChuaK !== 1;
        }
    });

    self.VisibleEndPageChuaK = ko.computed(function () {
        if (self.PageListChuaK().length > 0) {
            return self.PageListChuaK()[self.PageListChuaK().length - 1].pageNumberChuaK !== self.PageCountChuaK();
        }
    });
    self.StartPageChuaK = function () {
        self.currentPageChuaK(0);
        self.pagechuaK(self.currentPageChuaK());
        self.loadData();
    }

    self.BackPageChuaK = function () {
        self.pagechuaK(self.currentPageChuaK());
        if (self.pagechuaK() > 1) {
            self.pagechuaK(self.pagechuaK() - 1);
            self.currentPageChuaK(self.pagechuaK());
        }
        self.loadData();
    }

    self.GoToNextPageChuaK = function () {
        self.pagechuaK(self.currentPageChuaK());
        if (self.pagechuaK() < self.PageCountChuaK() - 1) {
            self.pagechuaK(self.pagechuaK() + 1);
            self.currentPageChuaK(self.pagechuaK());
        }
        self.loadData();
    }

    self.EndPageChuaK = function () {
        if (self.currentPageChuaK() < self.PageCountChuaK() - 1) {
            self.currentPageChuaK(self.PageCountChuaK() - 1);
            self.pagechuaK(self.currentPageChuaK());
        }
        self.loadData();
    }

    //phân trang chi tiết hàng hóa nhập hàng
    self.pageSizesCTNH = [10, 20, 30, 40, 50];
    self.pageSizeCTNH = ko.observable(self.pageSizesCTNH[0]);
    self.currentPageCTNH = ko.observable(0);
    self.fromitemCTNH = ko.observable(1);
    self.toitemCTNH = ko.observable();
    self.arrPaggingCTNH = ko.observableArray();

    self.PageCountCTNH = ko.observable();
    self.TotalRecordCTNH = ko.observable();
    var isGoToNext = false;
    function searchCTHN(isGoToNext) {
        var pagecount = Math.ceil(self.BH_HoaDonChiTietsThaoTac().length / self.pageSizeCTNH());
        self.PageCountCTNH(pagecount);
        self.TotalRecordCTNH(self.BH_HoaDonChiTietsThaoTac().length);
        self.BH_HoaDonChiTiets(self.BH_HoaDonChiTietsThaoTac().slice(self.currentPageCTNH() * self.pageSizeCTNH(), self.currentPageCTNH() * self.pageSizeCTNH() + self.pageSizeCTNH()));
    };

    function searchCTTheKho(isGoToNext) {
        $('.table-detal').gridLoader({
            style: "left: 460px;top: 200px;"
        });
        ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'GetChiTietHD_byIDHoaDonLoadChiTiet?idHoaDon=' + self.IDHDSearch() + '&currentpage=' + self.currentPageCTNH() + '&pageSize=' + self.pageSizeCTNH() + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
            for (var i = 0; i < data.length; i++) {
                if (data[i].MaHangHoa.indexOf('{DEL}') > -1) {
                    data[i].MaHangHoa = data[i].MaHangHoa.substr(0, data[i].MaHangHoa.length - 5);
                    data[i].Del = '{Xóa}';
                } else {
                    data[i].Del = "";
                }
            }
            $('.table-detal').gridLoader({ show: false });
            self.BH_HoaDonChiTiets(data);
        });
        if (!isGoToNext) {
            // page count
            ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'GetPageCountCTGiaoDich_Where?pageSize=' + self.pageSizeCTNH() + '&idHoaDon=' + self.IDHDSearch() + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
                self.PageCountCTNH(data.PageCount);
                self.TotalRecordCTNH(data.TotalRecord);
            });
        };
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
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumberCTN: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    if (currentPage == 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumberCTN: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj = {
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
                        var obj = {
                            pageNumberCTN: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj = {
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
                for (var i = 0; i < allPage; i++) {
                    var obj = {
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


    self.chitietdonvitinh = function (data) {
        ajaxHelper(DMHangHoaUri + "ChiTietDonViTinh/" + this.MaHangHoa, 'GET').done(function (data) {
        });
    };

    // xóa đơn vị tính
    self.removeDonViQuiDoi = function (item) {
        if (item.ID === '00000000-0000-0000-0000-000000000000') {
            self.newHangHoa().DonViTinh.remove(item);
            AddDonViTinhChoTungDong()
        }
        else {
            $.ajax({
                type: "DELETE",
                url: DonViQuiDoiUri + "DeleteDonViQuiDoi/" + item.ID,
                dataType: 'json',
                contentType: 'application/json',
                success: function (result) {
                    bottomrightnotify("Xóa đơn vị tính thành công!");
                    //$('#' + item.ID).remove();
                    self.newHangHoa().DonViTinh.remove(item);
                    AddDonViTinhChoTungDong();
                },
                error: function (error) {
                    bottomrightnotify(error.responseJSON);
                }
            });
        }
    };

    self.removeThuocTinh = function (item) {
        if (self.booleanAdd() === true) {
            self.ThuocTinhCuaHH.remove(item);
            ListHangHoaCungLoaiAfterRemoveTT();
        } else {
            self.ThuocTinhCuaHH.remove(item);
        }

        if (self.ThuocTinhCuaHH().length > 0) {
            $('.btn-them-luu').show();
        } else {
            $('.btn-them-luu').hide();
        }
    };

    self.removeThuocTinhOld = function (item) {
        self.ThuocTinhCuaHHEdit.remove(item);

        if (self.ThuocTinhCuaHHEdit().length > 0) {
            $('.btn-them-luu').show();
        } else {
            $('.btn-them-luu').hide();
        }
    };

    //Tính tỉ lệ chuyển đổi
    self.CalculateConvertRate = function () {
    };
    self.addDVQD = function () {
        var giaban = formatNumberToFloat(self.newHangHoa().GiaBan());
        //if (self.LaHangHoaNha() === true) {
        //    var giaban = $('#txtGiaBan').val();
        //}
        //if (self.LaHangHoaNha() === false) {
        //    var giaban = $('#txtGiaBanDV').val();
        //}
        var giavon = $('#txtGiaVon').val();
        var donvitinhchuan = $('#txtDonViTinhChuan').val();
        if (donvitinhchuan == null || donvitinhchuan === "" || donvitinhchuan === "undefined") {
            $('#txtDonViTinhChuan').focus();
            ShowMessage_Danger("Vui lòng nhập đơn vị tính!");
        } else {
            var newEmpty = {
                ID: '00000000-0000-0000-0000-000000000000',
                ID_HangHoa: '00000000-0000-0000-0000-000000000000',
                TenDonViTinh: '',
                GiaBan: giaban,
                GiaVon: giavon,
                TyLeChuyenDoi: 1,
                MaHangHoa: ''
            };
            self.newHangHoa().DonViTinh.push(newEmpty);

            $('input[type=text]').click(function () {
                $(this).select();
            });
        }
    }


    self.ArrManghangHoaCungLoai = ko.observableArray();
    self.pushGiaTriNew = function (e) {
        $('#txtAddGiaTriNew' + e.index).keypress(function (event) {
            var code = event.which;
            if (code === 13) {
                var giatrinew = $('#txtAddGiaTriNew' + e.index).val();
                var objgiatri = {
                    TenGiaTri: giatrinew,
                    ID_ThuocTinh: e.ID_ThuocTinh
                };
                if (self.ThuocTinhCuaHH().filter(p => p.index === e.index)[0].GiaTri.filter(p => p.TenGiaTri === giatrinew).length === 0 && giatrinew !== "") {
                    self.ThuocTinhCuaHH().filter(p => p.index === e.index)[0].GiaTri.push(objgiatri);
                    self.ThuocTinhCuaHH.refresh();
                    ListDanhSachHangCungLoai();
                    AddDonViTinhChoTungDong();
                    self.clicktien();
                }
                else {
                    $('#txtAddGiaTriNew' + e.index).val("");
                }
                $('#txtAddGiaTriNew' + e.index).focus();
            }
        });
    };

    function allPossibleCases(arr) {
        var result = [];
        if (arr.length === 1) {
            for (var j = 0; j < arr[0].length; j++) {
                var objct11 = {
                    ID_ThuocTinh: arr[0][j]["ID_ThuocTinh"] + ',' + arr[0][j]["TenGiaTri"] + '_',
                    TenGiaTri: arr[0][j]["TenGiaTri"],
                    TenDonViTinh: '',
                    LaDonViChuan: true,
                    TyLeChuyenDoi: 1
                };
                result.push(objct11);
            }

        } else {
            var allCasesOfRest = allPossibleCases(arr.slice(1));  // recur with the rest of array
            for (var i = 0; i < allCasesOfRest.length; i++) {
                for (var j = 0; j < arr[0].length; j++) {
                    var objct = {
                        ID_ThuocTinh: arr[0][j]["ID_ThuocTinh"] + ',' + arr[0][j]["TenGiaTri"] + '_' + allCasesOfRest[i]["ID_ThuocTinh"],
                        TenGiaTri: arr[0][j]["TenGiaTri"] + '-' + allCasesOfRest[i]["TenGiaTri"],
                        TenDonViTinh: allCasesOfRest[i]["TenDonViTinh"],
                        LaDonViChuan: allCasesOfRest[i]["LaDonViChuan"],
                        TyLeChuyenDoi: allCasesOfRest[i]["TyLeChuyenDoi"]
                    };
                    result.push(objct);
                }
            }

        }
        return result;

    }

    self.closeGiaTri = function (item) {
        removeGiaTriThuocTinh(item.ID_ThuocTinh, item.TenGiaTri);
    };

    function removeGiaTriThuocTinh(ID_ThuocTinh, TenGiaTri) {
        for (var i = 0; i < self.ThuocTinhCuaHH().filter(p => p.ID_ThuocTinh === ID_ThuocTinh)[0].GiaTri.length; i++) {
            var lcCT = self.ThuocTinhCuaHH().filter(p => p.ID_ThuocTinh === ID_ThuocTinh)[0].GiaTri[i];
            if (lcCT.TenGiaTri === TenGiaTri) {
                self.ThuocTinhCuaHH().filter(p => p.ID_ThuocTinh === ID_ThuocTinh)[0].GiaTri.splice(i, 1);
            }
        }
        self.ThuocTinhCuaHH.refresh();
        var objTrungGianHHCL = [];
        var ArrHangHoaCLOld = self.newHangHoa().HangHoaCungLoaiArr().filter(p => p.LaDonViChuan === true && p.TrangThai === true);
        self.newHangHoa().HangHoaCungLoaiArr([]);
        ListDanhSachHangCungLoai();
        for (var i = 0; i < self.newHangHoa().HangHoaCungLoaiArr().length; i++) {
            for (var j = 0; j < ArrHangHoaCLOld.length; j++) {
                if (ArrHangHoaCLOld[j]["ID_ThuocTinh"].includes(self.newHangHoa().HangHoaCungLoaiArr()[i]["ID_ThuocTinh"])) {
                    var objHH = {
                        ID_ThuocTinh: self.newHangHoa().HangHoaCungLoaiArr()[i].ID_ThuocTinh,
                        TenHangHoa: self.newHangHoa().HangHoaCungLoaiArr()[i].TenHangHoa,
                        MaHangHoa: ArrHangHoaCLOld[j].MaHangHoa,
                        GiaVon: ArrHangHoaCLOld[j].GiaVon,
                        GiaBan: ArrHangHoaCLOld[j].GiaBan,
                        TonKho: ArrHangHoaCLOld[j].TonKho,
                        TrangThai: true,
                        TenDonViTinh: self.newHangHoa().HangHoaCungLoaiArr()[i].TenDonViTinh,
                        LaDonViChuan: self.newHangHoa().HangHoaCungLoaiArr()[i].LaDonViChuan
                    };
                    objTrungGianHHCL.push(objHH);
                }
            }
        }
        self.newHangHoa().HangHoaCungLoaiArr(objTrungGianHHCL);
        AddDonViTinhChoTungDong();
        self.clicktien();
        self.newHangHoa().HangHoaCungLoaiArr.refresh();
    }

    function ListHangHoaCungLoaiAfterRemoveTT() {
        var objTrungGianHHCL = [];
        var ArrHangHoaCLOld = self.newHangHoa().HangHoaCungLoaiArr().filter(p => p.TrangThai === true);
        self.newHangHoa().HangHoaCungLoaiArr([]);
        ListDanhSachHangCungLoai();
        for (var i = 0; i < self.newHangHoa().HangHoaCungLoaiArr().length; i++) {
            //for (var j = 0; j < ArrHangHoaCLOld.length; j++) {
            //    if (ArrHangHoaCLOld[j]["ID_ThuocTinh"].includes(self.newHangHoa().HangHoaCungLoaiArr()[i]["ID_ThuocTinh"])) {
            //        var objHH = {
            //            ID_ThuocTinh: self.newHangHoa().HangHoaCungLoaiArr()[i].ID_ThuocTinh,
            //            TenHangHoa: self.newHangHoa().HangHoaCungLoaiArr()[i].TenHangHoa,
            //            MaHangHoa: ArrHangHoaCLOld[j].MaHangHoa,
            //            GiaVon: ArrHangHoaCLOld[j].GiaVon,
            //            GiaBan: ArrHangHoaCLOld[j].GiaBan,
            //            TonKho: ArrHangHoaCLOld[j].TonKho,
            //            TrangThai: true,
            //            TenDonViTinh: ArrHangHoaCLOld[j].TenDonViTinh,
            //            LaDonViChuan: ArrHangHoaCLOld[j].LaDonViChuan
            //        };
            //        objTrungGianHHCL.push(objHH);
            //    }
            //}

            var objHH = {
                ID_ThuocTinh: self.newHangHoa().HangHoaCungLoaiArr()[i].ID_ThuocTinh,
                TenHangHoa: self.newHangHoa().HangHoaCungLoaiArr()[i].TenHangHoa,
                MaHangHoa: ArrHangHoaCLOld.filter(p => p.ID_ThuocTinh.includes(self.newHangHoa().HangHoaCungLoaiArr()[i]["ID_ThuocTinh"]))[0].MaHangHoa,
                GiaVon: ArrHangHoaCLOld.filter(p => p.ID_ThuocTinh.includes(self.newHangHoa().HangHoaCungLoaiArr()[i]["ID_ThuocTinh"]))[0].GiaVon,
                GiaBan: ArrHangHoaCLOld.filter(p => p.ID_ThuocTinh.includes(self.newHangHoa().HangHoaCungLoaiArr()[i]["ID_ThuocTinh"]))[0].GiaBan,
                TonKho: ArrHangHoaCLOld.filter(p => p.ID_ThuocTinh.includes(self.newHangHoa().HangHoaCungLoaiArr()[i]["ID_ThuocTinh"]))[0].TonKho,
                TrangThai: true,
                TenDonViTinh: ArrHangHoaCLOld.filter(p => p.ID_ThuocTinh.includes(self.newHangHoa().HangHoaCungLoaiArr()[i]["ID_ThuocTinh"]))[0].TenDonViTinh,
                LaDonViChuan: ArrHangHoaCLOld.filter(p => p.ID_ThuocTinh.includes(self.newHangHoa().HangHoaCungLoaiArr()[i]["ID_ThuocTinh"]))[0].LaDonViChuan
            };
            objTrungGianHHCL.push(objHH);
        }
        self.newHangHoa().HangHoaCungLoaiArr(objTrungGianHHCL);
        AddDonViTinhChoTungDong();
        self.newHangHoa().HangHoaCungLoaiArr.refresh();
    }

    function ListDanhSachHangCungLoai() {
        var MangGiaTri = [];
        for (var i = 0; i < self.ThuocTinhCuaHH().length; i++) {
            var objGiaTri = [];
            for (var j = 0; j < self.ThuocTinhCuaHH()[i].GiaTri.length; j++) {
                objGiaTri.push(self.ThuocTinhCuaHH()[i].GiaTri[j]);
            }
            if (objGiaTri.length > 0) {
                MangGiaTri.push(objGiaTri);
            }
        }
        if (MangGiaTri.length > 0) {
            var result = allPossibleCases(MangGiaTri);
            for (var i = 0; i < result.length; i++) {
                var objHHCL = {
                    ID_ThuocTinh: result[i]["ID_ThuocTinh"],
                    TenHangHoa: result[i]["TenGiaTri"],
                    MaHangHoa: '',
                    GiaVon: self.newHangHoa().GiaVon(),
                    GiaBan: self.newHangHoa().GiaBan(),
                    TonKho: self.newHangHoa().TonKho(),
                    TrangThai: true,
                    TenDonViTinh: result[i]["TenDonViTinh"],
                    LaDonViChuan: result[i]["LaDonViChuan"],
                    TyLeChuyenDoi: result[i]["TyLeChuyenDoi"]
                };

                var datontai = false;
                for (var j = 0; j < self.newHangHoa().HangHoaCungLoaiArr().length; j++) {
                    if (result[i]["ID_ThuocTinh"].includes(self.newHangHoa().HangHoaCungLoaiArr()[j]["ID_ThuocTinh"])) {
                        self.newHangHoa().HangHoaCungLoaiArr()[j].TenHangHoa = result[i]["TenGiaTri"];
                        self.newHangHoa().HangHoaCungLoaiArr()[j].ID_ThuocTinh = result[i]["ID_ThuocTinh"];
                        datontai = true;
                    }
                }
                if (datontai === false) {
                    self.newHangHoa().HangHoaCungLoaiArr.push(objHHCL);
                }
            }
            self.newHangHoa().HangHoaCungLoaiArr.refresh();
        }
        else {
            self.newHangHoa().HangHoaCungLoaiArr([]);
            self.newHangHoa().HangHoaCungLoaiArr.refresh();
        }
    };

    function AddDonViTinhChoTungDong() {
        if (self.newHangHoa().DonViTinhChuan() !== "" && self.newHangHoa().DonViTinhChuan() !== undefined && self.newHangHoa().DonViTinhChuan() !== null) {
            for (var i = 0; i < self.newHangHoa().HangHoaCungLoaiArr().length; i++) {
                self.newHangHoa().HangHoaCungLoaiArr()[i].TenDonViTinh = self.newHangHoa().DonViTinhChuan();
            }
        }
        else {
            $.map(self.newHangHoa().HangHoaCungLoaiArr(), function (item, i) {
                self.newHangHoa().HangHoaCungLoaiArr()[i].TenDonViTinh = "";
            });
        }
        var objTG = [];
        var hanghoacungloaichuan = self.newHangHoa().HangHoaCungLoaiArr().filter(p => p.LaDonViChuan === true && p.TrangThai === true);
        if (self.newHangHoa().DonViTinh().length > 0) {
            for (var i = 0; i < self.newHangHoa().DonViTinh().length; i++) {
                if (self.newHangHoa().DonViTinh()[i].TenDonViTinh !== "" && self.newHangHoa().DonViTinh()[i].TenDonViTinh !== undefined && self.newHangHoa().DonViTinh()[i].TenDonViTinh !== null) {
                    for (var j = 0; j < hanghoacungloaichuan.length; j++) {
                        var objCT = {
                            ID_ThuocTinh: hanghoacungloaichuan[j].ID_ThuocTinh,
                            TenHangHoa: hanghoacungloaichuan[j].TenHangHoa,
                            MaHangHoa: '',
                            GiaVon: self.newHangHoa().GiaVon() * self.newHangHoa().DonViTinh()[i].TyLeChuyenDoi,
                            GiaBan: self.newHangHoa().GiaBan() * self.newHangHoa().DonViTinh()[i].TyLeChuyenDoi,
                            TonKho: self.newHangHoa().TonKho() / self.newHangHoa().DonViTinh()[i].TyLeChuyenDoi,
                            TrangThai: true,
                            TenDonViTinh: self.newHangHoa().DonViTinh()[i].TenDonViTinh,
                            LaDonViChuan: false,
                            TyLeChuyenDoi: self.newHangHoa().DonViTinh()[i].TyLeChuyenDoi
                        };
                        objTG.push(objCT);
                    }
                }
            }
        }
        if (objTG.length > 0) {
            $.map(objTG, function (item, i) {
                hanghoacungloaichuan.push(item);
            });
            hanghoacungloaichuan.sort(function (a, b) {
                var x = a["TenHangHoa"]; var y = b["TenHangHoa"];
                return ((x < y) ? -1 : ((x > y) ? 1 : 0));
            });
            self.newHangHoa().HangHoaCungLoaiArr(hanghoacungloaichuan);
        }
        else {
            self.newHangHoa().HangHoaCungLoaiArr(hanghoacungloaichuan);
        }
        self.newHangHoa().HangHoaCungLoaiArr.refresh();
    };

    self.XoaHangHoaCungLoai = function (item) {
        $.map(self.newHangHoa().HangHoaCungLoaiArr(), function (item1, i) {
            if (item1.ID_ThuocTinh === item.ID_ThuocTinh && item1.TenDonViTinh.trim().toLowerCase() === item.TenDonViTinh.trim().toLowerCase()) {
                self.newHangHoa().HangHoaCungLoaiArr()[i].TrangThai = false;
            }
        });
        var idthuoctinh = item.ID_ThuocTinh;
        var chuoi = idthuoctinh.split('_');
        var manghanghoa = self.newHangHoa().HangHoaCungLoaiArr().filter(p => p.TrangThai === true);
        var lenMangHangHoa = manghanghoa.length;
        for (var i = 0; i < chuoi.length; i++) {
            var xoa = true;
            if (lenMangHangHoa > 0) {
                for (var j = 0; j < lenMangHangHoa; j++) {
                    if (manghanghoa[j]["ID_ThuocTinh"].includes(chuoi[i])) {
                        xoa = false;
                    }
                    if (j === lenMangHangHoa - 1 && xoa === true) {
                        var mangXoa = chuoi[i].split(',');
                        removeGiaTriThuocTinh(mangXoa[0], mangXoa[1]);
                    }
                }
            }
            else {
                var mangXoa1 = chuoi[i].split(',');
                removeGiaTriThuocTinh(mangXoa1[0], mangXoa1[1]);
            }
        }
        self.newHangHoa().HangHoaCungLoaiArr.refresh();
    };

    self.DVTChuanHangHoaCungLoai = function () {
        if (self.newHangHoa().DonViTinhChuan() === "" || self.newHangHoa().DonViTinhChuan() === undefined && self.newHangHoa().DonViTinhChuan() === null) {
            if (self.newHangHoa().DonViTinh().length > 0) {
                ShowMessage_Danger("Vui lòng nhập đơn vị cơ bản");
                $('#txtDonViTinhChuan').focus();
                return false;
            }
        }
        AddDonViTinhChoTungDong();
    };

    //Thuộc tính
    self.arrThuocTinhSave = ko.observableArray();
    self.addThuocTinhCon = function () {
        var newEmpty = {
            index: self.ThuocTinhCuaHH().length,
            ID_ThuocTinh: '00000000-0000-0000-0000-000000000000',
            TenThuocTinh: "",
            GiaTri: []
        };
        getallThuocTinh();
        //self.selectedThuocTinh(undefined);
        self.ThuocTinhCuaHH.push(newEmpty);

        if (self.ThuocTinhCuaHH().length > 0) {
            $('.btn-them-luu').show();
        } else {
            $('.btn-them-luu').hide();
        }
        $('input[type=text]').click(function () {
            $(this).select();
        });
        $('.ddlThuocTinh').focus();

    };

    self.addThuocTinhConOld = function () {
        var newEmpty = {
            index: self.ThuocTinhCuaHHEdit().length,
            ID_ThuocTinh: '00000000-0000-0000-0000-000000000000',
            TenThuocTinh: "",
            GiaTri: ""
        };
        getallThuocTinh();
        //self.selectedThuocTinh(undefined);
        self.ThuocTinhCuaHHEdit.push(newEmpty);
        if (self.ThuocTinhCuaHHEdit().length > 0) {
            $('.btn-them-luu').show();
        } else {
            $('.btn-them-luu').hide();
        }
        $('input[type=text]').click(function () {
            $(this).select();
        });
        $('.ddlThuocTinh').focus();
    }

    self.showChuyenNhomhang = function () {
        $('#mymodalshipping').modal('show');
        self.selectIDNhomHHChuyenNhom(null);
        $('#lstNhomHangChuyenNhom span').each(function () {
            $(this).empty();
        });
        $('#choose_TenNHHChuyenNhom').text('---Chọn nhóm---');
    };

    self.AllNhomHoTro = ko.observableArray();

    function GetListNhomHang_SetupHoTro() {
        let param = {
            IDChiNhanhs: [VHeader.IdDonVi]
        }
        ajaxHelper(NhomHHUri + 'GetListNhomHang_SetupHoTro', 'POST', param).done(function (x) {
            if (x.res) {
                let xx = x.dataSoure;
                let arrIDNhom = [], arrNhom = [];
                for (let i = 0; i < xx.length; i++) {
                    let forIn = xx[i];
                    if ($.inArray(forIn.Id_NhomHang, arrIDNhom) === -1) {
                        arrIDNhom.push(forIn.Id_NhomHang);
                        let obj = {
                            Id_NhomHang: forIn.Id_NhomHang,
                            TenNhomHangHoa: forIn.TenNhomHangHoa,
                            SoNgayThuoc: '',
                            IsXuatNgayThuoc: false,
                            IsApply: false,
                            GhiChu: '',
                        }
                        arrNhom.push(obj);
                    }
                }
                arrNhom.map(function (x) {
                    x['id'] = x.Id_NhomHang
                    x['text'] = x.TenNhomHangHoa
                    x['children'] = [
                        //{id: 1, text:'Con11'},
                        //{id: 2, text:'Con12'},
                    ]
                })
                self.AllNhomHoTro(arrNhom);
                filterNhomHoTro.listNhomHang = arrNhom;
            }
        })
    }

    self.chuyenNhomHoTro = async function () {
        vmChuyenNhomHang.showModal(2, self.AllNhomHoTro());
    }

    $('#vmThemNhomHang_NangCao').on('hidden.bs.modal', function () {
        if (vmThemNhomHang_NangCao.saveOK) {
            GetListNhomHang_SetupHoTro();
            GetAllNhomHH();

            let objNhom = vmThemNhomHang_NangCao.newNhomHang;
            $('#choose_TenNHHAddHH').text(objNhom.TenNhomHangHoa);
            $('#choose_TenNHHAddDV').text(objNhom.TenNhomHangHoa);
            modelHangHoa.selectIDNhomHHAddHH(objNhom.ID);
        }
    })

    $('#vmChuyenNhomHang').on('hidden.bs.modal', function () {
        if (vmChuyenNhomHang.saveOK) {
            if (vmChuyenNhomHang.formType === 2) {
                if (vmChuyenNhomHang.arrIDChosed.length > 1) {
                    ShowMessage_Danger('Chỉ được chọn 1 nhóm hỗ trợ');
                    return;
                }

                let myData = {
                    idNhomHoTro: vmChuyenNhomHang.arrIDChosed[0],
                    idChiNhanh: VHeader.IdDonVi,
                    arrIDQuiDoi: arrIDHang
                }
                ajaxHelper(NhomHHUri + 'MoveHangHoa_toNhomHoTro', 'POST', myData).done(function (x) {
                    if (x.res) {
                        ShowMessage_Success('Chuyển nhóm thành công');
                        let diary = {
                            ID_DonVi: VHeader.IdDonVi,
                            ID_NhanVien: VHeader.IdNhanVien,
                            LoaiNhatKy: 2,
                            ChucNang: 'Chuyển nhóm hỗ trợ',
                            NoiDung: 'Chuyển '.concat(arrIDHang.length, ' hàng hóa đến nhóm hỗ trợ ', vmChuyenNhomHang.TenNhomChoseds),
                            NoiDungChiTiet: 'Chuyển '.concat(arrIDHang.length, ' hàng hóa đến nhóm hỗ trợ ', vmChuyenNhomHang.TenNhomChoseds,
                                '<br /> Người chuyển: ', VHeader.UserLogin),
                        }
                        Insert_NhatKyThaoTac_1Param(diary);
                        arrIDHang = [];
                        SearchHangHoa();
                        $('.choose-commodity').hide();
                    }
                })
            }
        }
    })

    self.XoaHHTT = function () {
        if (arrIDHang === "") {
            self.ListChooseHH([]);
        } else {
            self.ListChooseHH([]);
            var i = 0;
            function callback() {
                if (i < arrIDHang.length) {
                    ajaxHelper(DMHangHoaUri + "GetChoose_HangHoa?id=" + arrIDHang[i] + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
                        i++;
                        self.ListChooseHH(data);
                        self.XoaHangHoaLS(self.ListChooseHH().MaHangHoa);
                        var IDHH = self.ListChooseHH().ID_DonViQuiDoi;
                        $.ajax({
                            type: "DELETE",
                            url: DMHangHoaUri + "DeleteDM_HangHoa?idquidoi=" + IDHH + "&idcungloai=" + self.ListChooseHH().ID_HangHoaCungLoai,
                            dataType: 'json',
                            contentType: 'application/json',
                            success: function (result) {
                                $('modalpopup_deleteTTHH').hide();
                                callback();
                            },
                            error: function (error) {
                                $('modalpopup_deleteTTHH').hide();
                                ShowMessage_Danger("Cập nhật hàng hóa thất bại. Vì mặt hàng này đã có trong kho!!!");
                            }
                        })
                    });
                }
                else {
                    arrIDHang = [];
                    SearchHangHoa();
                }
            }
            callback();
            $('.operation').hide();
            $('.choose-commodity').hide();
            ShowMessage_Success("Cập nhật hàng hóa thành công!");
        }
    }

    self.NgungKDChoose = function () {
        var i = 0;
        function callNKD() {
            if (i < arrIDHang.length) {
                var IDHH = arrIDHang[i];
                ajaxHelper(DMHangHoaUri + "NgungKinhDoanh_HH?id=" + IDHH, 'DELETE').done(function (data) {
                    i++;
                    if (data == "") {
                        $('#modalpopup_NgungKinhDoanh').hide();
                        //location.reload();
                    } else {
                        ShowMessage_Danger("Không thể ngừng kinh doanh hàng hóa đã được chọn!");
                        $('#modalpopup_NgungKinhDoanh').hide();
                        callNKD();
                    }
                    callNKD();
                });
            }
            else {
                arrIDHang = [];
                SearchHangHoa();
            }
        }
        callNKD();
        $('.operation').hide();
        $('.choose-commodity').hide();

        ShowMessage_Success("Cập nhật dữ liệu thành công!");
    }

    self.ChoKDChoose = function () {
        var i = 0;
        function callCKD() {
            if (i < arrIDHang.length) {
                var IDHH = arrIDHang[i];
                ajaxHelper(DMHangHoaUri + "ChoKinhDoanh_HH?id=" + IDHH, 'DELETE').done(function (data) {
                    i++;
                    if (data == "") {
                        $('#modalpopup_NgungKinhDoanh').hide();
                        SearchHangHoa();
                    } else {
                        ShowMessage_Danger("Không thể ngừng kinh doanh hàng hóa đã được chọn!");
                        $('#modalpopup_NgungKinhDoanh').hide();
                        callCKD();
                    }
                    callCKD();
                });
            }
            else {
                arrIDHang = [];
                SearchHangHoa();
            }
        }
        callCKD();

        //$('#count').html(arrIDHang.length);
        $('.operation').hide();
        $('.choose-commodity').hide();
        ShowMessage_Success("Cập nhật dữ liệu thành công!");
    }

    self.ChuyenNHH = function () {
        var IDNhomHH = self.selectIDNhomHHChuyenNhom();
        var i = 0;
        $('.table-reponsive').gridLoader();
        function chuyennhh() {
            if (i < arrIDHang.length) {
                ajaxHelper(DMHangHoaUri + "MoveNhomHH?idNhom=" + IDNhomHH + '&id_dvqd=' + arrIDHang[i], 'POST').done(function (data) {
                    $('#mymodalshipping').modal('hide');
                    i++;
                    chuyennhh();
                });
            }
            else {
                $('.checkboxsetHH').prop("checked", false);
                arrIDHang = [];
                $('#count').html(arrIDHang.length);
                $('.operation').hide();
                $('.choose-commodity').hide();
                SearchHangHoa();
                ShowMessage_Success("Cập nhật dữ liệu thành công!");
            }
        }
        chuyennhh();

    }
    //var list = [];
    self.getListChooseHH = function () {
        self.ListChooseHH([]);
        if (arrIDHang == "") {
            self.ListChooseHH([]);
        } else {
            self.ListChooseHH([]);
            for (var i = 0; i < arrIDHang.length; i++) {
                ajaxHelper(DMHangHoaUri + "GetChoose_HangHoa?id=" + arrIDHang[i] + '&iddonvi=' + _IDchinhanh, 'GET').done(function (data) {
                    self.ListChooseHH.push(data);
                });
            }
        }
    }

    self.modalInThuocTinh = function (item) {
        self.maHangMaVach(item.MaHangHoa);
        self.tenHangMaVach(item.TenHangHoa);
        self.itemMaVach(item);
        self.IDitemMaVach(item.ID_DonViQuiDoi);
        $('#printThuocTinh').modal('show');
    }
    self.SoLuongBanIn = ko.observable(4);
    self.btnInTemThuocTinh = function () {
        var valoptOrientation = $('input[name=optOrientation]:checked').val();
        if (valoptOrientation === '1') {
            var printElement = document.getElementsByClassName("landscape");
            if (printElement.length !== 0) {
                printElement[0].classList.add("portrait");
                printElement[0].classList.remove("landscape");
            }
        }
        else {
            var printElement = document.getElementsByClassName("portrait");
            if (printElement.length !== 0) {
                printElement[0].classList.add("landscape");
                printElement[0].classList.remove("portrait");
            }
        }

        var valOptKhoGiay = $('input[name=optKhoGiay]:checked').val();
        if (valOptKhoGiay === '1') {
            self.SoLuongBanIn(4);
            var printElement = document.getElementsByClassName("a48tem");
            if (printElement.length !== 0) {
                printElement[0].classList.add("a44tem");
                printElement[0].classList.remove("a48tem");
            }
        }
        else {
            self.SoLuongBanIn(8);
            var printElement = document.getElementsByClassName("a44tem");
            if (printElement.length !== 0) {
                printElement[0].classList.add("a48tem");
                printElement[0].classList.remove("a44tem");
            }
        }

        $('#printThuocTinh').modal('hide');
        $('#viewPrintThuocTinh').show();
        $(".modal-ontop").show();
    }

    self.xoaChooseHH = function (item) {
        self.ListChooseHH.remove(item);
        for (var i = 0; i < arrIDHang.length; i++) {
            if (arrIDHang[i] == item.ID_DonViQuiDoi) {
                arrIDHang.splice(i, 1);
            }
        }

        $('.prev-tr-hide .check-group input').each(function () {
            $(this).prop('checked', false);
        })

        $('.prev-tr-hide1 .check-group input').each(function () {
            $(this).prop('checked', false);
        })

        for (var i = 0; i < arrIDHang.length; i++) {
            $('.prev-tr-hide .check-group input').each(function () {
                if ($(this).attr('id') === arrIDHang[i]) {
                    $(this).prop('checked', true);
                }
            })
            $('.prev-tr-hide1 .check-group input').each(function () {
                if ($(this).attr('id') === arrIDHang[i]) {
                    $(this).prop('checked', true);
                }
            })
        }
        $('#count').html(arrIDHang.length);
        if (arrIDHang.length === 0) {
            $('.operation').hide();
            $('.choose-commodity').hide();
        }
    }

    self.xoaAllChooseHH = function () {
        //for (var i = 0; i < arrIDHang.length; i++) {
        self.ListChooseHH([]);
        arrIDHang = [];
        $('.prev-tr-hide .check-group input').each(function () {
            $(this).prop('checked', false);
        })
        $('#count').html(arrIDHang.length);
        $('.checkboxsetHH').prop("checked", false);
        $('.operation').hide();
        $('.choose-commodity').hide();
    }

    self.clicktien = function () {
        var giaBan = formatNumberToFloat(self.newHangHoa().GiaBan());
        var giaVon = formatNumberToInt($('#txtGiaVon').val());
        var tonKho = formatNumberToInt($('#txtTonKho').val());

        var arr = self.newHangHoa().DonViTinh();
        for (var i = 0; i < arr.length; i++) {
            if (arr[i].ID === '00000000-0000-0000-0000-000000000000' && arr[i].TyLeChuyenDoi !== null) {
                arr[i].GiaBan = formatNumber3Digit(arr[i].TyLeChuyenDoi * giaBan);
                arr[i].GiaVon = formatNumber3Digit(arr[i].TyLeChuyenDoi * giaVon);
            }
        }
        localStorage.setItem('DVT', JSON.stringify(arr));

        var lcDVT = localStorage.getItem('DVT');
        if (lcDVT !== null) {
            lcDVT = JSON.parse(lcDVT);
            self.newHangHoa().DonViTinh(lcDVT);
        }
        var arrHHCL = self.newHangHoa().HangHoaCungLoaiArr();
        for (var j = 0; j < arrHHCL.length; j++) {
            if (arrHHCL[j].TenDonViTinh === self.newHangHoa().DonViTinhChuan()) {
                arrHHCL[j].GiaBan = formatNumber3Digit(giaBan);
                arrHHCL[j].TonKho = formatNumber3Digit(tonKho);
                arrHHCL[j].GiaVon = formatNumber3Digit(giaVon);
            }
            else {
                if (self.newHangHoa().DonViTinh().length > 0) {
                    arrHHCL[j].GiaBan = self.newHangHoa().DonViTinh().filter(p => p.TenDonViTinh === arrHHCL[j].TenDonViTinh)[0].GiaBan;
                    arrHHCL[j].TonKho = formatNumber3Digit(parseFloat((tonKho / self.newHangHoa().DonViTinh().filter(p => p.TenDonViTinh === arrHHCL[j].TenDonViTinh)[0].TyLeChuyenDoi).toFixed(3)));
                    arrHHCL[j].GiaVon = formatNumber3Digit(giaVon * self.newHangHoa().DonViTinh().filter(p => p.TenDonViTinh === arrHHCL[j].TenDonViTinh)[0].TyLeChuyenDoi);
                    arrHHCL[j].TyLeChuyenDoi = self.newHangHoa().DonViTinh().filter(p => p.TenDonViTinh === arrHHCL[j].TenDonViTinh)[0].TyLeChuyenDoi;
                }
            };
        };
        self.newHangHoa().HangHoaCungLoaiArr.refresh();
    };

    self.clicktienHHCL = function (item) {
        var _giaVon = item.GiaVon;
        var _giaban = item.GiaBan;
        $.map(self.newHangHoa().HangHoaCungLoaiArr(), function (item1, i) {
            if (item1.ID_ThuocTinh === item.ID_ThuocTinh && item1.LaDonViChuan === false) {
                self.newHangHoa().HangHoaCungLoaiArr()[i].GiaVon = _giaVon.toString().includes(',') ? formatNumber3Digit(formatNumberToInt(_giaVon) * self.newHangHoa().HangHoaCungLoaiArr()[i].TyLeChuyenDoi) : formatNumber3Digit(_giaVon * self.newHangHoa().HangHoaCungLoaiArr()[i].TyLeChuyenDoi);
                self.newHangHoa().HangHoaCungLoaiArr()[i].GiaBan = _giaban.toString().includes(',') ? formatNumber3Digit(formatNumberToInt(_giaban) * self.newHangHoa().HangHoaCungLoaiArr()[i].TyLeChuyenDoi) : formatNumber3Digit(_giaban * self.newHangHoa().HangHoaCungLoaiArr()[i].TyLeChuyenDoi);
            }
        });
        self.newHangHoa().HangHoaCungLoaiArr.refresh();
    };

    self.clickhide = function () {
        $(this).next("ul").toggleClass("open");
    };

    var time = null
    self.NoteNhomHang = function () {
        //clearTimeout(time);
        //time = setTimeout(
        //    function () {
        tk = $('#SeachNhomHang').val();
        if (tk.trim() != '') {
            ajaxHelper('/api/DanhMuc/DM_NhomHangHoaAPI/' + "SeachDM_NhomHangHoa?TenNhom=" + tk, 'GET').done(function (data) {
                self.NhomHangHoas([]);
                for (var i = 0; i < data.length; i++) {
                    if (data[i].ID_Parent == null) {
                        var objParent = {
                            ID: data[i].ID,
                            TenNhomHangHoa: data[i].TenNhomHangHoa,
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
                    else {
                        var objParent = {
                            ID: data[i].ID,
                            TenNhomHangHoa: data[i].TenNhomHangHoa,
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
                if (self.NhomHangHoas().length > 10) {
                    $('.close-goods').css('display', 'block');
                }
            })
        }
        else {
            GetAllNhomHH();
        }
        //}, 300);
    };

    self.NhomHH_NotChil3 = ko.observableArray();

    var arrChild3 = [];
    function GetAllNhomHH() {
        ajaxHelper(NhomHHUri + 'GetDM_NhomHangHoa', 'GET').done(function (data) {
            self.NhomHangHoas([]);
            self.NhomHH_NotChil3([]);
            localStorage.setItem('lc_NhomHangHoas', JSON.stringify(data));
            for (var i = 0; i < data.length; i++) {
                if (data[i].ID_Parent == null) {
                    var objParent = {
                        ID: data[i].ID,
                        TenNhomHangHoa: data[i].TenNhomHangHoa,
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
                                    arrChild3.push(data[k].ID);
                                }
                            }
                            objParent.Childs.push(objChild);
                        }
                    }
                    self.NhomHangHoas.push(objParent);
                    self.NhomHangHoasFilter.push(objParent);
                }
            }
            if (self.NhomHangHoas().length > 10) {
                $('.close-goods').css('display', 'block');
            }
            var arrPr_NotChild3 = [];
            for (var i = 0; i < data.length; i++) {
                if ($.inArray(data[i].ID, arrChild3) === -1) {
                    arrPr_NotChild3.push(data[i]);
                }
            }
            for (var i = 0; i < arrPr_NotChild3.length; i++) {
                if (arrPr_NotChild3[i].ID_Parent == null) {
                    var objParent = {
                        ID: arrPr_NotChild3[i].ID,
                        TenNhomHangHoa: arrPr_NotChild3[i].TenNhomHangHoa,
                        Childs: [],
                    }

                    for (var j = 0; j < arrPr_NotChild3.length; j++) {
                        if (arrPr_NotChild3[j].ID !== arrPr_NotChild3[i].ID && arrPr_NotChild3[j].ID_Parent === arrPr_NotChild3[i].ID) {
                            var objChild =
                            {
                                ID: arrPr_NotChild3[j].ID,
                                TenNhomHangHoa: arrPr_NotChild3[j].TenNhomHangHoa,
                                ID_Parent: arrPr_NotChild3[i].ID,
                                Child2s: []
                            };
                            for (var k = 0; k < arrPr_NotChild3.length; k++) {
                                if (arrPr_NotChild3[k].ID_Parent !== null && arrPr_NotChild3[k].ID_Parent === arrPr_NotChild3[j].ID) {
                                    var objChild2 =
                                    {
                                        ID: arrPr_NotChild3[k].ID,
                                        TenNhomHangHoa: arrPr_NotChild3[k].TenNhomHangHoa,
                                        ID_Parent: arrPr_NotChild3[j].ID,
                                    };
                                    objChild.Child2s.push(objChild2);
                                    arrChild3.push(arrPr_NotChild3[k].ID);
                                }
                            }
                            objParent.Childs.push(objChild);
                        }
                    }
                    self.NhomHH_NotChil3.push(objParent);
                }
            }
            //self.NhomHH_NotChil3(arrPr_NotChild3);
        });
    };
    GetAllNhomHH();

    self.NhomHH_Add = ko.observableArray();
    self.NhomHHSuaByLoai = ko.observableArray();
    function GetAllNhomHHByLaNhomHH() {
        var laNhomHH = self.newNhomHangHoa().LaNhomHangHoa();
        ajaxHelper(NhomHHUri + 'GetDM_NhomHangHoaByLaNhomHH?lanhomhh=' + laNhomHH, 'GET').done(function (data) {
            self.NhomHHSuaByLoai(data);
            self.NhomHH_Add([]);
            var arrPr_NotChild3 = [];
            for (var i = 0; i < data.length; i++) {
                if (self.CheckLocNhomHangCap3() === true) {
                    if ($.inArray(data[i].ID, arrChild3) === -1) {
                        arrPr_NotChild3.push(data[i]);
                    }
                }
                else {
                    arrPr_NotChild3.push(data[i]);
                }
            }
            for (var i = 0; i < arrPr_NotChild3.length; i++) {
                if (arrPr_NotChild3[i].ID_Parent === null) {
                    var objParent = {
                        ID: arrPr_NotChild3[i].ID,
                        TenNhomHangHoa: arrPr_NotChild3[i].TenNhomHangHoa,
                        Childs: []
                    }

                    for (var j = 0; j < arrPr_NotChild3.length; j++) {
                        if (arrPr_NotChild3[j].ID !== arrPr_NotChild3[i].ID && arrPr_NotChild3[j].ID_Parent === arrPr_NotChild3[i].ID) {
                            var objChild =
                            {
                                ID: arrPr_NotChild3[j].ID,
                                TenNhomHangHoa: arrPr_NotChild3[j].TenNhomHangHoa,
                                ID_Parent: arrPr_NotChild3[i].ID,
                                Child2s: []
                            };
                            for (var k = 0; k < arrPr_NotChild3.length; k++) {
                                if (arrPr_NotChild3[k].ID_Parent !== null && arrPr_NotChild3[k].ID_Parent === arrPr_NotChild3[j].ID) {
                                    var objChild2 =
                                    {
                                        ID: arrPr_NotChild3[k].ID,
                                        TenNhomHangHoa: arrPr_NotChild3[k].TenNhomHangHoa,
                                        ID_Parent: arrPr_NotChild3[j].ID
                                    };
                                    objChild.Child2s.push(objChild2);
                                    arrChild3.push(arrPr_NotChild3[k].ID);
                                }
                            }
                            objParent.Childs.push(objChild);
                        }
                    }
                    self.NhomHH_Add.push(objParent);
                }
            }

            //self.NhomHH_NotChil3(arrPr_NotChild3);
        });
    };

    self.loadLaiNhomCha = function () {
        GetAllNhomHHByLaNhomHH();
    }
    self.filterNHH = ko.observable();
    //Tìm kiếm ở nhóm hàng hóa LaHangHoa = true
    self.SearchNhomHangHoaTrue = function () {
        tk = $('#txtSearchNhomHangHoaTrue').val();
        if (tk.trim() != '') {
            ajaxHelper('/api/DanhMuc/DM_NhomHangHoaAPI/' + "SeachDM_NhomHangHoa?TenNhom=" + tk, 'GET').done(function (data) {
                self.NhomHH_Add([]);
                data = data.filter(p => p.LaNhomHangHoa == true);
                for (var i = 0; i < data.length; i++) {
                    if (data[i].ID_Parent == null) {
                        var objParent = {
                            ID: data[i].ID,
                            TenNhomHangHoa: data[i].TenNhomHangHoa,
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
                        self.NhomHH_Add.push(objParent);
                    }
                    else {
                        var objParent = {
                            ID: data[i].ID,
                            TenNhomHangHoa: data[i].TenNhomHangHoa,
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
                        self.NhomHH_Add.push(objParent);
                    }
                }
            })
        }
        else {
            GetAllNhomHHByLaNhomHH();
        }
    };

    //Tìm kiếm ở nhóm hàng hóa LaHangHoa = false
    self.SearchNhomHangHoaFalse = function () {
        tk = $('#txtSearchNhomHangHoaFalse').val();
        if (tk.trim() != '') {
            ajaxHelper('/api/DanhMuc/DM_NhomHangHoaAPI/' + "SeachDM_NhomHangHoa?TenNhom=" + tk, 'GET').done(function (data) {
                self.NhomHH_Add([])
                data = data.filter(p => p.LaNhomHangHoa == false);
                for (var i = 0; i < data.length; i++) {
                    if (data[i].ID_Parent == null) {
                        var objParent = {
                            ID: data[i].ID,
                            TenNhomHangHoa: data[i].TenNhomHangHoa,
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
                        self.NhomHH_Add.push(objParent);
                    }
                    else {
                        var objParent = {
                            ID: data[i].ID,
                            TenNhomHangHoa: data[i].TenNhomHangHoa,
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
                        self.NhomHH_Add.push(objParent);
                    }
                }
            })
        }
        else {
            GetAllNhomHHByLaNhomHH();
        }
    };

    //Tìm kiếm khi thêm nhóm hàng hóa
    self.SearchNhomHangHoaWhenAddNew = function () {
        tk = $('#txtSearchNHHAddNew').val();
        if (tk.trim() != '') {
            ajaxHelper('/api/DanhMuc/DM_NhomHangHoaAPI/' + "SeachDM_NhomHangHoa?TenNhom=" + tk, 'GET').done(function (data) {
                self.NhomHH_Add([])
                data = data.filter(p => p.LaNhomHangHoa == self.newNhomHangHoa().LaNhomHangHoa());
                var arrPr_NotChild3 = [];
                for (var i = 0; i < data.length; i++) {
                    if ($.inArray(data[i].ID, arrChild3) === -1) {
                        arrPr_NotChild3.push(data[i]);
                    }
                }
                for (var i = 0; i < arrPr_NotChild3.length; i++) {
                    if (arrPr_NotChild3[i].ID_Parent == null) {
                        var objParent = {
                            ID: arrPr_NotChild3[i].ID,
                            TenNhomHangHoa: arrPr_NotChild3[i].TenNhomHangHoa,
                            Childs: [],
                        }
                        for (var j = 0; j < arrPr_NotChild3.length; j++) {
                            if (arrPr_NotChild3[j].ID !== arrPr_NotChild3[i].ID && arrPr_NotChild3[j].ID_Parent === arrPr_NotChild3[i].ID) {
                                var objChild =
                                {
                                    ID: arrPr_NotChild3[j].ID,
                                    TenNhomHangHoa: arrPr_NotChild3[j].TenNhomHangHoa,
                                    ID_Parent: arrPr_NotChild3[i].ID,
                                    Child2s: []
                                };
                                for (var k = 0; k < arrPr_NotChild3.length; k++) {
                                    if (arrPr_NotChild3[k].ID_Parent !== null && arrPr_NotChild3[k].ID_Parent === arrPr_NotChild3[j].ID) {
                                        var objChild2 =
                                        {
                                            ID: arrPr_NotChild3[k].ID,
                                            TenNhomHangHoa: arrPr_NotChild3[k].TenNhomHangHoa,
                                            ID_Parent: arrPr_NotChild3[j].ID,
                                        };
                                        objChild.Child2s.push(objChild2);
                                    }
                                }
                                objParent.Childs.push(objChild);
                            }
                        }
                        self.NhomHH_Add.push(objParent);
                    }
                    else {
                        var objParent = {
                            ID: arrPr_NotChild3[i].ID,
                            TenNhomHangHoa: arrPr_NotChild3[i].TenNhomHangHoa,
                            Childs: [],
                        }
                        for (var j = 0; j < arrPr_NotChild3.length; j++) {
                            if (arrPr_NotChild3[j].ID !== arrPr_NotChild3[i].ID && arrPr_NotChild3[j].ID_Parent === arrPr_NotChild3[i].ID) {
                                var objChild =
                                {
                                    ID: arrPr_NotChild3[j].ID,
                                    TenNhomHangHoa: arrPr_NotChild3[j].TenNhomHangHoa,
                                    ID_Parent: arrPr_NotChild3[i].ID,
                                    Child2s: []
                                };
                                for (var k = 0; k < arrPr_NotChild3.length; k++) {
                                    if (arrPr_NotChild3[k].ID_Parent !== null && arrPr_NotChild3[k].ID_Parent === arrPr_NotChild3[j].ID) {
                                        var objChild2 =
                                        {
                                            ID: arrPr_NotChild3[k].ID,
                                            TenNhomHangHoa: arrPr_NotChild3[k].TenNhomHangHoa,
                                            ID_Parent: arrPr_NotChild3[j].ID,
                                        };
                                        objChild.Child2s.push(objChild2);
                                    }
                                }
                                objParent.Childs.push(objChild);
                            }
                        }
                        self.NhomHH_Add.push(objParent);
                    }
                }
            })
        }
        else {
            GetAllNhomHHByLaNhomHH();
        }
    };

    //Tìm kiếm khi thêm nhóm hàng hóa ở view hàng hóa
    self.SearchNhomHangHoaAdd = function () {
        tk = $('#txtSearchNHH').val();
        if (tk.trim() != '') {
            ajaxHelper('/api/DanhMuc/DM_NhomHangHoaAPI/' + "SeachDM_NhomHangHoa?TenNhom=" + tk, 'GET').done(function (data) {
                self.NhomHH_Add([])
                data = data.filter(p => p.LaNhomHangHoa == true);
                var arrPr_NotChild3 = [];
                for (var i = 0; i < data.length; i++) {
                    if ($.inArray(data[i].ID, arrChild3) === -1) {
                        arrPr_NotChild3.push(data[i]);
                    }
                }
                for (var i = 0; i < arrPr_NotChild3.length; i++) {
                    if (arrPr_NotChild3[i].ID_Parent == null) {
                        var objParent = {
                            ID: arrPr_NotChild3[i].ID,
                            TenNhomHangHoa: arrPr_NotChild3[i].TenNhomHangHoa,
                            Childs: [],
                        }
                        for (var j = 0; j < arrPr_NotChild3.length; j++) {
                            if (arrPr_NotChild3[j].ID !== arrPr_NotChild3[i].ID && arrPr_NotChild3[j].ID_Parent === arrPr_NotChild3[i].ID) {
                                var objChild =
                                {
                                    ID: arrPr_NotChild3[j].ID,
                                    TenNhomHangHoa: arrPr_NotChild3[j].TenNhomHangHoa,
                                    ID_Parent: arrPr_NotChild3[i].ID,
                                    Child2s: []
                                };
                                for (var k = 0; k < arrPr_NotChild3.length; k++) {
                                    if (arrPr_NotChild3[k].ID_Parent !== null && arrPr_NotChild3[k].ID_Parent === arrPr_NotChild3[j].ID) {
                                        var objChild2 =
                                        {
                                            ID: arrPr_NotChild3[k].ID,
                                            TenNhomHangHoa: arrPr_NotChild3[k].TenNhomHangHoa,
                                            ID_Parent: arrPr_NotChild3[j].ID,
                                        };
                                        objChild.Child2s.push(objChild2);
                                    }
                                }
                                objParent.Childs.push(objChild);
                            }
                        }
                        self.NhomHH_Add.push(objParent);
                    }
                    else {
                        var objParent = {
                            ID: arrPr_NotChild3[i].ID,
                            TenNhomHangHoa: arrPr_NotChild3[i].TenNhomHangHoa,
                            Childs: [],
                        }
                        for (var j = 0; j < arrPr_NotChild3.length; j++) {
                            if (arrPr_NotChild3[j].ID !== arrPr_NotChild3[i].ID && arrPr_NotChild3[j].ID_Parent === arrPr_NotChild3[i].ID) {
                                var objChild =
                                {
                                    ID: arrPr_NotChild3[j].ID,
                                    TenNhomHangHoa: arrPr_NotChild3[j].TenNhomHangHoa,
                                    ID_Parent: arrPr_NotChild3[i].ID,
                                    Child2s: []
                                };
                                for (var k = 0; k < arrPr_NotChild3.length; k++) {
                                    if (arrPr_NotChild3[k].ID_Parent !== null && arrPr_NotChild3[k].ID_Parent === arrPr_NotChild3[j].ID) {
                                        var objChild2 =
                                        {
                                            ID: arrPr_NotChild3[k].ID,
                                            TenNhomHangHoa: arrPr_NotChild3[k].TenNhomHangHoa,
                                            ID_Parent: arrPr_NotChild3[j].ID,
                                        };
                                        objChild.Child2s.push(objChild2);
                                    }
                                }
                                objParent.Childs.push(objChild);
                            }
                        }
                        self.NhomHH_Add.push(objParent);
                    }
                }
            })
        }
        else {
            GetAllNhomHHByLaNhomHH();
        }
    };

    self.arrFilterNhomHH_ChuyenNhomHH = ko.computed(function () {
        var _filter = self.filterNHH();
        return arrFilter = ko.utils.arrayFilter(self.NhomHangHoasFilter(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenNhomHangHoa).toLowerCase().split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = (locdau(prod.TenNhomHangHoa).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    sSearch.indexOf(locdau(_filter).toLowerCase()) >= 0
                );
            }
            return chon;
        });
    });

    self.arrFilterNhomHHSua = ko.computed(function () {
        var _filter = self.filterNHH();

        return arrFilter = ko.utils.arrayFilter(self.NhomHH_NotChil4(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenNhomHangHoa).toLowerCase().split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = (locdau(prod.TenNhomHangHoa).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    sSearch.indexOf(locdau(_filter).toLowerCase()) >= 0
                );
            }
            return chon;
        });
    });
    //========Paging KiemKho====
    self.GetClassKK = function (page) {
        return ((page.pageNumber - 1) === self.currentPage()) ? "click" : "";
    };


    $('#txtSeachKK').keypress(function (e) {
        if (e.keyCode === 13) {
            self.currentPage(0);
            SearchKiemKho();
        }
    });

    //lọc theo đơn vị
    self.ChiNhanhs = ko.observableArray();
    self.MangNhomDV = ko.observableArray();
    self.MangIDDV = ko.observableArray();

    function getAllChiNhanh() {
        ajaxHelper('/api/DanhMuc/DM_DonViAPI/' + "GetListDonViByIDNguoiDung?idnhanvien=" + _IDNhanVien, 'GET').done(function (data) {
            data = data.sort((a, b) => a.TenDonVi.localeCompare(b.TenDonVi, undefined, { caseFirst: "upper" }));
            self.ChiNhanhs(data);
            var obj = {
                ID: _IDchinhanh,
                TenDonVi: $('#_txtTenDonVi').html()
            }
            self.selectedCN(obj);
            //vmNKyHoatDongNhomHang.listData.ChiNhanhs = data;
        });
    }
    if (loaiHoaDon == 9) {
        getAllChiNhanh();
    }

    self.selectedCN = function (item) {
        var arrDV = [];
        for (var i = 0; i < self.MangNhomDV().length; i++) {
            if ($.inArray(self.MangNhomDV()[i], arrDV) === -1) {
                arrDV.push(self.MangNhomDV()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrDV) === -1) {
            self.MangNhomDV.push(item);
        }
        var url;
        var Urlhash = window.location.hash.split("?");
        if (Urlhash[0].length > 3) {
            url = Urlhash[0].substring(2, Urlhash[0].length);

        }
        else {
            url = Urlhash[0];
        }
        if (url === 'StockTakes') {
            SearchKiemKho();
        }
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
        SearchKiemKho();
        // remove check
        $('#selec-all-DV li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }

    $('.choseNgayTao li').on('click', function () {
        $('#txtNgayTao').val($(this).text());
        self.filterNgayLapHD_Quy($(this).val());
        self.currentPage(0);
        SearchKiemKho();
    });


    $('#txtNgayTaoInput').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        SearchKiemKho();
    });
    var txtMaHDonKK_Excel = "";
    var txtTrangThaiKK_Excel = 6;
    var dayStartKK_Excel = "";
    var dayEndKK_Excel = "";
    var loaihoadonKK_Excel;

    function SearchKiemKho(isGoToNext) {
        //$('.line-right').height(0).css("margin-top", "0px");
        var FindKiemKho = localStorage.getItem('FindKiemKho');
        if (FindKiemKho !== null) {
            self.filterKK(FindKiemKho);
        }
        var txtMaHDon = self.filterKK();


        if (txtMaHDon === undefined) {
            txtMaHDon = "";
        }
        txtMaHDonKK_Excel = txtMaHDon;
        // trang thai: DH (1), HT (3),Pt(2) HT + Huy + PT(6), HT + DH(5), PT +DH = 4, HT +PT =0

        var arrDV = [];
        self.TenChiNhanh([]);
        for (var i = 0; i < self.MangNhomDV().length; i++) {
            self.TenChiNhanh.push(self.MangNhomDV()[i].TenDonVi);
            if ($.inArray(self.MangNhomDV()[i], arrDV) === -1) {
                arrDV.push(self.MangNhomDV()[i].ID);
            }
        }
        self.MangIDDV(arrDV);
        var statusInvoice = 1;
        if (self.Loc_TrangThaiDH()) {
            if (self.Loc_TrangThaiHT()) {
                if (self.Loc_TrangThaiPT()) {
                    statusInvoice = 6;
                } else {
                    statusInvoice = 5;
                }
            }
            else {
                if (self.Loc_TrangThaiPT()) {
                    statusInvoice = 4;
                } else {
                    statusInvoice = 1; // HT
                }
            }
        }
        else {
            if (self.Loc_TrangThaiHT()) {
                if (self.Loc_TrangThaiPT()) {
                    statusInvoice = 0;
                } else {
                    statusInvoice = 3;
                }
            } else {
                if (self.Loc_TrangThaiPT()) {
                    statusInvoice = 2;
                } else {
                    statusInvoice = 7;
                }
            }
        }
        txtTrangThaiKK_Excel = statusInvoice;
        // NgayLapHoaDon
        var _now = new Date();  //current date of week
        var currentWeekDay = _now.getDay();
        var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
        var dayStart = '';
        var dayEnd = '';
        var dateChose = '';

        if (self.filterNgayLapHD() === '0') {

            switch (self.filterNgayLapHD_Quy()) {
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
                    dayEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
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
                    var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
                    dayStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                    dayEnd = moment().quarter(prevQuarter).endOf('quarter').add('days', 1).format('YYYY-MM-DD');
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
            dayEnd = moment(arrDate[1], 'DD/MM/YYYY').format('YYYY-MM-DD');
            self.TodayBC('Từ ' + moment(arrDate[0], 'DD/MM/YYYY').format('DD/MM/YYYY') + ' đến ' + moment(arrDate[1], 'DD/MM/YYYY').format('DD/MM/YYYY'));
        }
        dayStartKK_Excel = dayStart;
        dayEndKK_Excel = dayEnd;
        $('.table-hover').gridLoader();
        ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'GetListHoaDonsKiemKho_where?currentPage=' + self.currentPage() + '&pageSize=' + self.pageSize() + '&loaiHoaDon=' + loaiHoaDon +
            '&maHoaDon=' + txtMaHDon + '&trangThai=' + statusInvoice + '&dayStart=' + dayStart + '&dayEnd=' + dayEnd + '&iddonvi=' + _IDchinhanh + '&arrChiNhanh=' + self.MangIDDV() + '&columsort=' + self.columsort() + '&sort=' + self.sort(),
            'GET').done(function (data) {
                $('.table-hover').gridLoader({ show: false });
                self.KiemKhos(data);
                LoadHtmlGridKK();

            });
        if (!isGoToNext) {
            // page count
            ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'GetPageCountHoaDonKK_Where?currentPage=' + self.currentPage() + '&pageSize=' + self.pageSize() + '&loaiHoaDon=' + loaiHoaDon +
                '&maHoaDon=' + txtMaHDon + '&trangThai=' + statusInvoice + '&dayStart=' + dayStart + '&dayEnd=' + dayEnd + '&iddonvi=' + _IDchinhanh + '&arrChiNhanh=' + self.MangIDDV(),
                'GET').done(function (data) {
                    $('.table-hover').gridLoader({ show: false });
                    self.TotalRecordKK(data.TotalRecord);
                    self.PageCountKK(data.PageCount);
                });
        }
        localStorage.removeItem('FindKiemKho');
    }

    self.clickiconSearch = function () {
        SearchKiemKho();
    }

    self.PageList_DisplayKK = ko.computed(function () {
        var arrPage = [];

        var allPage = self.PageCountKK();
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
        return arrPage;
    });

    self.Loc_TrangThaiDH.subscribe(function (newVal) {
        self.currentPage(0);
        SearchKiemKho();
    });

    self.Loc_TrangThaiHT.subscribe(function (newVal) {
        self.currentPage(0);
        SearchKiemKho();
    });

    self.Loc_TrangThaiPT.subscribe(function (newVal) {
        self.currentPage(0);
        SearchKiemKho();
    });

    self.filterNgayLapHD.subscribe(function (newVal) {
        self.currentPage(0);
        SearchKiemKho();
    });

    self.GoToPageKK = function (page) {
        if (page.pageNumber !== '.') {
            self.currentPage(page.pageNumber - 1);
            SearchKiemKho(true);
        }
    };

    self.StartPageKK = function () {
        self.currentPage(0);
        SearchKiemKho(true);
    }

    self.BackPageKK = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
            SearchKiemKho(true);
        }
    }

    self.GoToNextPageKK = function () {
        if (self.currentPage() < self.PageCountKK() - 1) {
            self.currentPage(self.currentPage() + 1);
            SearchKiemKho(true);
        }
    }

    self.EndPageKK = function () {
        if (self.currentPage() < self.PageCountKK() - 1) {
            self.currentPage(self.PageCountKK() - 1);
            SearchKiemKho(true);
        }
    }

    self.VisibleStartPageKK = ko.computed(function () {
        if (self.PageList_DisplayKK().length > 0) {
            return self.PageList_DisplayKK()[0].pageNumber !== 1;
        }
    });

    self.VisibleEndPageKK = ko.computed(function () {
        if (self.PageList_DisplayKK().length > 0) {
            return self.PageList_DisplayKK()[self.PageList_DisplayKK().length - 1].pageNumber !== self.PageCountKK();
        }
    });

    //=====Paging HangHoa ======
    self.GetClassHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage()) ? "click" : "";
    };

    $('#txtSearchHH').keypress(function (e) {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        if (e.keyCode === 13) {
            var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
            if ($.inArray('HangHoa_XemDS', lc_CTQuyen) > -1) {
                self.currentPage(0);
                SearchHangHoa();
            }
        }
    })
    $('.number-search').keyup(function () {
        var val = this.value.replace(/\D/g, '');
        this.value = formatNumber3Digit(val);
    });
    $(".number-search").keypress(function (e) {
        if (e.keyCode === 13) {
            self.currentPage(0);
            if (self.ListFilterColumn().some(o => o.Key === $(this).data('id'))) {
                self.ListFilterColumn().filter(o => o.Key === $(this).data('id'))[0].Value = $(this).val();
            }
            SearchColumnHangHoa(false);
        }
    });
    $(".number-search").focusout(function () {
        self.currentPage(0);
        var val = ($(this).val() !== null ? $(this).val() : '');
        if (self.ListFilterColumn().some(o => o.Key === $(this).data('id')
            && ((o.Value === null && val !== '')
                || (o.Value !== null && o.Value !== val))
        )) {
            self.ListFilterColumn().filter(o => o.Key === $(this).data('id'))[0].Value = val;
            SearchColumnHangHoa(false);
        }

    });
    $(".search-grid").keypress(function (e) {
        if (e.keyCode === 13) {
            self.currentPage(0);
            if (self.ListFilterColumn().some(o => o.Key === $(this).data('id'))) {
                self.ListFilterColumn().filter(o => o.Key === $(this).data('id'))[0].Value = $(this).val();
            }
            SearchColumnHangHoa(false);
        }
    });
    $(".search-grid").focusout(function () {
        self.currentPage(0);
        var val = ($(this).val() !== null ? $(this).val() : '');
        if (self.ListFilterColumn().some(o => o.Key === $(this).data('id')
            && ((o.Value === null && val !== '')
                || (o.Value !== null && o.Value !== val))
        )) {
            self.ListFilterColumn().filter(o => o.Key === $(this).data('id'))[0].Value = val;
            SearchColumnHangHoa(false);
        }

    });
    $('.list-kv ul').on('click', 'li', function () {
        $(".list-kv").each(function () {
            $(this).hide();
        });
        $(this).closest('.list-kv').parent().find('.kv1').find('span').text($(this).text().split(':')[0]);
        var value = $(this).closest('.list-kv').parent().next('.col-md-9').find('input').val();
        if (value !== null && value !== '' && value !== undefined) {
            SearchColumnHangHoa(false);
        }
    });
    $('#exampleFormControlSelect1').on('change', function () {
        if (self.ListFilterColumn().some(o => o.Key === $(this).data('id'))) {
            self.ListFilterColumn().filter(o => o.Key === $(this).data('id'))[0].Value = $(this).val();
        }
        SearchColumnHangHoa(false);
    });
    $('#exampleFormControlSelect2').on('change', function () {
        if (self.ListFilterColumn().some(o => o.Key === $(this).data('id'))) {
            self.ListFilterColumn().filter(o => o.Key === $(this).data('id'))[0].Value = $(this).val();
        }
        SearchColumnHangHoa(false);
    });
    var txtmMaHDon_Excel;
    var txtTonKho_Excel;
    var txtTrangThai_Excel;
    var txtKinhDoanh_Excel;
    var txtIDNhomHang_Excel;

    self.TongTon = ko.observable();
    self.checkLoadDemCL = ko.observable(true);
    self.ListFilterColumn = ko.observableArray();
    self.ComparisonFields = ko.observableArray();
    self.selecttedKeyCompare = function (key, type) {
        if (self.ListFilterColumn().some(o => o.Key === parseInt(key))) {
            var data = self.ListFilterColumn().filter(o => o.Key === parseInt(key))[0];
            data.type = parseInt(type);
            SearchColumnHangHoa(false);
        }
    }
    function loadkeysearchcolumn() {
        ajaxHelper(DMHangHoaUri + 'GetListKeyColumHanghoa', 'GET').done(function (data1) {
            self.ListFilterColumn(data1.keycolumn);
            self.ComparisonFields(data1.compareFile);
        });
    }
    loadkeysearchcolumn();

    function GetParamSearch() {
        var MaHangLoad = localStorage.getItem('loadMaHang');
        if (MaHangLoad !== null) {
            self.filter(MaHangLoad);
        }
        var txtMaHDon = self.filter();

        if (txtMaHDon === undefined) {
            txtMaHDon = "";
        }
        txtmMaHDon_Excel = txtMaHDon;
        // trang thai: H.Thanh (1), Huy (2), HT + Huy (3), ChuaHT + Chua Huy (0)
        var arrLoaiHang = [];
        var loaiHangs = '';
        if (self.LoaiSP_HH()) {
            arrLoaiHang.push(1);
        }
        if (self.LoaiSP_DV()) {
            arrLoaiHang.push(2);
        }
        if (self.LoaiSP_CB()) {
            arrLoaiHang.push(3);
        }
        if (arrLoaiHang.length === 0) {
            arrLoaiHang = [1, 2, 3];
        }
        loaiHangs = arrLoaiHang.toString();
        var tonkho = 0;
        switch (parseInt(self.Loc_TonKho())) {
            case 1:// tonkho > 0
                tonkho = 1;
                break;
            case 2:// tonkho <= 0
                tonkho = 2;
                break;
            case 3:// duoi dinhmuc
                tonkho = 3;
                break;
            case 4:// vuot dinhmuc
                tonkho = 4;
                break;
            case 5:// tonkho < 0
                tonkho = 5;
                break;
            case 6: // tonkho = 0
                tonkho = 6;
                break;
        }

        var kinhdoanh = 1;
        if (self.Loc_TinhTrangKD() === '0') {
            kinhdoanh = 3;
        }
        if (self.Loc_TinhTrangKD() === '2') {
            kinhdoanh = 2;
        }
        if (self.Loc_TinhTrangKD() === '1') {
            kinhdoanh = 1;
        }
        txtTonKho_Excel = tonkho;
        txtKinhDoanh_Excel = kinhdoanh;
        txtTrangThai_Excel = loaiHangs;
        if (txtMaHDon === "" && tonkho === 3 && kinhdoanh === 1 && self.arrIDNhomHang().length === 0 && self.ListThuocTinh().length === 0) {
            self.checkLoadDemCL(true);
        } else {
            self.checkLoadDemCL(false);
        }

        // trangthaixoa (key = 8)     
        self.ListFilterColumn(self.ListFilterColumn().filter(x => x.Key !== 8));
        switch (parseInt(self.Loc_TrangThaiXoa())) {
            case 0:
                self.ListFilterColumn.push({ Key: 8, Value: 'false', type: 0 })
                break;
            case 1:
                self.ListFilterColumn.push({ Key: 8, Value: 'true', type: 0 })
                break;
        }


        return {
            currentPage: self.currentPage(),
            pageSize: self.pageSize(),
            idnhomhang: self.arrIDNhomHang(),
            maHoaDon: txtMaHDon,
            loaihangs: loaiHangs,
            tonkho: tonkho,
            kinhdoanh: kinhdoanh,
            iddonvi: _IDchinhanh,
            listthuoctinh: self.ListThuocTinh(),
            columsort: self.columsort(),
            sort: self.sort(),
            listSearchColumn: self.ListFilterColumn(),
        };
    }

    function GetParamSearch1() {
        var MaHangLoad = localStorage.getItem('loadMaHang');
        if (MaHangLoad !== null) {
            self.filter(MaHangLoad);
        }
        var txtMaHDon = self.filter();
        if (!commonStatisJs.CheckNull(txtMaHDon)) {
            txtMaHDon = txtMaHDon.trim();
        }

        // trang thai: H.Thanh (1), Huy (2), HT + Huy (3), ChuaHT + Chua Huy (0)
        var arrLoaiHang = [];
        if (self.LoaiSP_HH()) {
            arrLoaiHang.push(1);
        }
        if (self.LoaiSP_DV()) {
            arrLoaiHang.push(2);
        }
        if (self.LoaiSP_CB()) {
            arrLoaiHang.push(3);
        }
        if (arrLoaiHang.length === 0) {
            arrLoaiHang = [1, 2, 3];
        }

        // loaihanghoa (key = 3)  
        self.ListFilterColumn(self.ListFilterColumn().filter(x => x.Key !== 3));

        let filterHeader_LoaiHang = $('#exampleFormControlSelect2').val();
        if (formatNumberToFloat(filterHeader_LoaiHang) !== 0) {
            self.ListFilterColumn.push({ Key: 3, Value: filterHeader_LoaiHang, type: 0 })
        }
        else {
            self.ListFilterColumn.push({ Key: 3, Value: arrLoaiHang.toString(), type: 0 })
        }

        var tonkho = 0;
        switch (parseInt(self.Loc_TonKho())) {
            case 1:// tonkho > 0
                tonkho = 1;
                break;
            case 2:// tonkho <= 0
                tonkho = 2;
                break;
            case 3:// duoi dinhmuc
                tonkho = 3;
                break;
            case 4:// vuot dinhmuc
                tonkho = 4;
                break;
            case 5:// tonkho < 0
                tonkho = 5;
                break;
            case 6: // tonkho = 0
                tonkho = 6;
                break;
        }

        // nhomhanghoa (key = 2)     
        self.ListFilterColumn(self.ListFilterColumn().filter(x => x.Key !== 2));
        if (!commonStatisJs.CheckNull(self.header_filterNhomHang())) {
            self.ListFilterColumn.push({ Key: 2, Value: self.header_filterNhomHang(), type: 0 })
        }
        else {
            if (self.arrIDNhomHang().length > 0) {
                self.ListFilterColumn.push({ Key: 2, Value: self.arrIDNhomHang().toString(), type: 0 })
            }
        }

        // nhomHoTro (key = 9)
        self.ListFilterColumn(self.ListFilterColumn().filter(x => x.Key !== 9));
        if (!commonStatisJs.CheckNull(filterNhomHoTro.arrIDChosed)) {
            self.ListFilterColumn.push({ Key: 9, Value: filterNhomHoTro.arrIDChosed.toString(), type: 0 })
        }
        // thuoctinhhang
        let arrThuocTinh = self.arrListThuocTinh().map(function (x) { return x.ID });

        // trangthaikinhdoanh (key = 7)   
        let filterKD = $.grep(self.ListFilterColumn(), function (x) {
            return x.Key === 7 && $.inArray(x.Value, ['true', 'false']) > -1;
        });
        self.ListFilterColumn(self.ListFilterColumn().filter(x => x.Key !== 7));

        if (filterKD.length === 0) {
            switch (parseInt(self.Loc_TinhTrangKD())) {
                case 1:
                    self.ListFilterColumn.push({ Key: 7, Value: 1, type: 0 })
                    break;
                case 2:
                    self.ListFilterColumn.push({ Key: 7, Value: 2, type: 0 })
                    break;
            }
        }
        else {
            for (let i = 0; i < filterKD.length; i++) {
                if (filterKD[i].Value === 'true') {
                    self.ListFilterColumn.push({ Key: 7, Value: 1, type: 0 })
                }
                if (filterKD[i].Value === 'false') {
                    self.ListFilterColumn.push({ Key: 7, Value: 2, type: 0 })
                }
            }
        }


        // trangthaixoa (key = 8)     
        self.ListFilterColumn(self.ListFilterColumn().filter(x => x.Key !== 8));
        switch (parseInt(self.Loc_TrangThaiXoa())) {
            case 0:
                self.ListFilterColumn.push({ Key: 8, Value: '0', type: 0 })
                break;
            case 1:
                self.ListFilterColumn.push({ Key: 8, Value: '1', type: 0 })
                break;
        }

        let arrFilter = self.ListFilterColumn().filter(x => x.Value != null);

        let colSort = self.columsort();
        if (commonStatisJs.CheckNull(colSort)) {
            colSort = 'NgayTao'
        }
        let sort = self.sort();
        if (commonStatisJs.CheckNull(sort)) {
            sort = 1;
        }
        return {
            IDChiNhanhs: [VHeader.IdDonVi],
            TextSearch: txtMaHDon,
            TrangThaiKho: tonkho,
            CurrentPage: self.currentPage(),
            PageSize: self.pageSize(),
            ColumnSort: colSort,
            SortBy: sort == 1 ? 'DESC' : 'ASC',
            ListThuocTinh: arrThuocTinh,
            ListSearchColumn: arrFilter,
        };
    }

    function SearchColumnHangHoa(isGoToNext) {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('HangHoa_XemDS', lc_CTQuyen) > -1) {
            $('.prev-tr-hide .check-group input').each(function () {
                $(this).prop('checked', false);
            })

            $('.table-reponsive').gridLoader();

            var model = GetParamSearch1();

            ajaxHelper(DMHangHoaUri + 'LoadDanhMuchangHoa',
                'POST', model).done(function (x) {
                    console.log('LoadDanhMuchangHoa', x)
                    if (x.res && x.dataSoure.length > 0) {
                        let first = x.dataSoure[0];
                        self.TotalRecord(first.TotalRow);
                        self.PageCount(first.TotalPage);
                        self.TongTon(first.SumTonKho);
                        self.HangHoas(x.dataSoure);
                    }
                    else {
                        self.TotalRecord(0);
                        self.PageCount(0);
                        self.TongTon(0);
                        self.HangHoas([]);
                    }

                    let arr = [], count = 0;
                    for (let i = 0; i < self.HangHoas().length; i++) {
                        arr.push(self.HangHoas()[i].ID);
                    }
                    for (let i = 0; i < arr.length; i++) {
                        if ($.inArray(arr[i], arrIDHang) > -1) {
                            count = count + 1;
                        }
                    }
                    if (count == 10) {
                        $(".checkboxsetHH").prop("checked", true);
                    } else {
                        $(".checkboxsetHH").prop("checked", false);
                    }
                    $('.hideCheck input').each(function () {
                        if ($.inArray($(this).attr('id'), arrIDHang) > -1) {
                            $(this).prop("checked", true);
                        }
                    })
                    itemcheckhh = '';
                }).always(function () {
                    LoadHtmlGridHH();
                    $('.table-reponsive').gridLoader({ show: false });
                });
        }
        localStorage.removeItem('loadMaHang');
    }
    if (LocalCaches.loadFIlterAdvanced(LocalCaches.keyHangHoa)) {
        $('.tr-show').toggle();
    }
    $('.showfiltercolumn').on('click', function () {
        LocalCaches.setitemFilterAdvanced(LocalCaches.keyHangHoa);
        if ($('.tr-show').css('display') !== 'none') {
            ResetSearchColumn();
            SearchHangHoa();
        }
        $('.tr-show').toggle();
    });

    self.header_filterNhomHang.subscribe(function () {
        SearchColumnHangHoa();
    })
    function ResetSearchColumn() {
        self.ListFilterColumn([]);
        $('.search-grid').each(function () {
            $(this).val('');
        });
        $('#exampleFormControlSelect1').val('');
        $('#exampleFormControlSelect2').val('');
    }
    function SearchHangHoa(isGoToNext) {
        SearchColumnHangHoa();
    }

    self.clickiconSearchHH = function () {
        SearchHangHoa();
    }

    self.LoaiSP_HH.subscribe(function (newVal) {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        self.currentPage(0);
        SearchHangHoa();
    });

    self.LoaiSP_DV.subscribe(function (newVal) {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        self.currentPage(0);
        SearchHangHoa();
    });

    self.LoaiSP_CB.subscribe(function (newVal) {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        self.currentPage(0);
        SearchHangHoa();
    });

    self.Loc_TinhTrangKD.subscribe(function (newVal) {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        self.currentPage(0);
        SearchHangHoa();
    });
    self.Loc_TonKho.subscribe(function (newVal) {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        self.currentPage(0);
        SearchHangHoa();
    });

    self.Loc_TrangThaiXoa.subscribe(function () {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        self.currentPage(0);
        SearchHangHoa();
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
            SearchColumnHangHoa(true);
            //$(".checkboxsetHH").prop("checked", false);
        }
    };

    self.StartPage = function () {
        self.currentPage(0);
        SearchColumnHangHoa(true);
    }

    self.BackPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
            SearchColumnHangHoa(true);
        }
    }

    self.GoToNextPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.currentPage() + 1);
            SearchColumnHangHoa(true);
        }
    }

    self.EndPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
            SearchColumnHangHoa(true);
        }
    }

    //sort by cột trong bảng DM_HanGHoa
    $('#myTable thead tr').on('click', 'th', function () {
        var id = $(this).attr('id');
        if (id === "txtMaHang") {
            self.columsort("MaHangHoa");
            SortGrid(id);
        }
        if (id === "txttenhang") {
            self.columsort("TenHangHoa");
            SortGrid(id);
        }
        if (id === "txtnhomhang") {
            self.columsort("TenNhomHang");
            SortGrid(id);
        }
        if (id === "txtgiaban") {
            self.columsort("GiaBan");
            SortGrid(id);
        }
        if (id === "txtgiavon") {
            self.columsort("GiaVon");
            SortGrid(id);
        }
        if (id === "txttonkho") {
            self.columsort("TonKho");
            SortGrid(id);
        }
    });
    //sort kiểm kho
    $('#tableKK thead tr').on('click', 'th', function () {
        var id = $(this).attr('id');
        if (id === "txtMaHoaDon") {
            self.columsort("MaHoaDon");
            SortGridKK(id);
        }
        if (id === "txtThoiGian") {
            self.columsort("ThoiGian");
            SortGridKK(id);
        }
        if (id === "txtTenDonVi") {
            self.columsort("DonVi");
            SortGridKK(id);
        }
        if (id === "txtGhiChu") {
            self.columsort("GhiChu");
            SortGridKK(id);
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
        SearchColumnHangHoa();
    };

    function SortGridKK(item) {
        $("#iconSort").remove();
        if (self.sort() === 0) {
            self.sort(1);
            $('#' + item).append(' ' + "<i id='iconSort' class='fa fa-caret-down' aria-hidden='true'></i>");
        }
        else {
            self.sort(0);
            $('#' + item).append(' ' + "<i id='iconSort' class='fa fa-caret-up' aria-hidden='true'></i>");
        }
        SearchKiemKho();
    };

    self.DownloadFile_byURL = function (pathFile) {
        var url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + pathFile;
        window.location.href = url;
    }

    self.SetUp_byGroup = function (type) {
        let valUpdate = 0;
        let typeProp = parseInt(type);
        switch (typeProp) {
            case 1:
                valUpdate = self.TinhHoaHongTruocCK() === true ? 1 : 0;
                break;
            case 2:
                var tichdiem = self.newHangHoa().DuocTichDiem();
                valUpdate = tichdiem === true ? 1 : tichdiem === false ? 0 : tichdiem;
                break;
        }
        vmApplyGroupProduct.showModal(typeProp, valUpdate, self.productOld().ID_NhomHangHoa);
    }

    // xuất danh mục hàng hóa
    self.ExportDMHHtoExcel = function () {
        let param = GetParamSearch1();
        let columnHide = [];
        for (let i = 0; i < self.ColumnsExcel().length; i++) {
            columnHide.push(self.ColumnsExcel()[i]);
        }
        param.ColumnHide = columnHide;
        param.PageSize = self.TotalRecord();

        ajaxHelper(DMHangHoaUri + 'ExportExcel_DanhMucHangHoa', 'POST', param).done(function (x) {
            $('.content-table').gridLoader({ show: false });
            if (x.res) {
                self.DownloadFile_byURL(x.dataSoure);

                let objDiary = {
                    ID_NhanVien: _IDNhanVien,
                    ID_DonVi: _IDchinhanh,
                    ChucNang: "Danh mục hàng hóa",
                    NoiDung: "Xuất file danh mục hàng hóa",
                    NoiDungChiTiet: "Xuất file danh mục hàng hóa".concat(', Người xuất: ', VHeader.UserLogin),
                    LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                };
                Insert_NhatKyThaoTac_1Param(objDiary);
            }
        })
    }

    // xuất thẻ kho danh mục hàng hóa
    self.ExportTheKhoHHtoExcel = function () {
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Danh Mục Hàng hóa",
            NoiDung: "Xuất danh sách thẻ kho của hàng hóa",
            NoiDungChiTiet: "Xuất danh sách thẻ kho của hàng hóa",
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        Insert_NhatKyThaoTac_1Param(objDiary);

        var columnHide = null;
        for (var i = 0; i < self.ColumnsExcel().length; i++) {
            if (i == 0) {
                columnHide = self.ColumnsExcel()[i];
            }
            else {
                columnHide = self.ColumnsExcel()[i] + "_" + columnHide;
            }
        }
        var url = DMHangHoaUri + 'ExportExel_TheKhoHH?id=' + self.selectIDHH() + '&iddonvi=' + _IDchinhanh + '&columnsHide=' + columnHide;
        window.location.href = url;
    }

    self.ColumnsExcel = ko.observableArray();
    var arrColumnsExcel = [];
    self.addColum = function (item) {
        if (self.ColumnsExcel().length < 1) {
            self.ColumnsExcel.push(item);
        }
        else {
            for (var i = 0; i < self.ColumnsExcel().length; i++) {
                if (self.ColumnsExcel()[i] === item) {
                    self.ColumnsExcel.splice(i, 1);
                    break;
                }
                if (i == self.ColumnsExcel().length - 1) {
                    self.ColumnsExcel.push(item);
                    break;
                }
            }
        }
        self.ColumnsExcel.sort();
    }
    //===============================
    // Load lai form lưu cache bộ lọc 
    // trên grid hàng hóa
    //===============================
    var loadColumExeHH = true;
    function LoadHtmlGridHH() {

        if (window.localStorage) {
            var current = localStorage.getItem('DanhSachHangHoa');
            if (!current) {
                current = [];
                loadColumExelKK = false;
                localStorage.setItem('DanhSachHangHoa', JSON.stringify(current));
            } else {
                current = JSON.parse(current);
                for (var i = 0; i < current.length; i++) {
                    $(current[i].NameClass).addClass("operation");
                    document.getElementById(current[i].NameId).checked = false;
                    if (loadColumExeHH) {
                        self.addColum(current[i].Value);
                    }
                }
                loadColumExeHH = false;
            }
            $('#myTable tbody .op-js-tr-hide').each(function () {
                $(this).find('.td-colspan').attr("colspan", 11 - current.length);
            });
        }

    }
    //===============================
    // Load lai form lưu cache bộ lọc 
    // trên grid Kiểm kho
    //===============================
    var loadColumExelKK = true;

    function LoadHtmlGridKK() {
        if (window.localStorage) {
            var current = localStorage.getItem('KiemKho');
            if (!current) {
                current = [];
                loadColumExelKK = false;
                localStorage.setItem('KiemKho', JSON.stringify(current));
            } else {
                current = JSON.parse(current);
                for (var i = 0; i < current.length; i++) {
                    $(current[i].NameClass).addClass("operation");
                    $(current[i].NameClass)[0].style.display = 'none';
                    document.getElementById(current[i].NameId).checked = false;
                    if (loadColumExelKK) {
                        self.addColumKK(current[i].Value);
                    }
                }
                loadColumExelKK = false;
            }

        }
    }
    //===============================
    // Add Các tham số cần lưu lại đẻ 
    // cache khi load lại form hàng hóa
    //===============================
    function setColspanForTable() {

    }
    function addClassHH(name, id, value) {

        var current = localStorage.getItem('DanhSachHangHoa');
        if (!current) {
            current = [];
        } else {
            current = JSON.parse(current);
        }
        if (current.length > 0) {
            for (var i = 0; i < current.length; i++) {
                if (current[i].NameId === id.toString()) {
                    current.splice(i, 1);
                    break;
                }
                if (i == current.length - 1) {
                    current.push({
                        NameClass: name,
                        NameId: id,
                        Value: value
                    });
                    break;
                }
            }
        }
        else {
            current.push({
                NameClass: name,
                NameId: id,
                Value: value
            });
        }
        $('#myTable tbody .op-js-tr-hide').each(function () {
            $(this).find('td.td-colspan').attr("colspan", 9 - current.length);
        });
        localStorage.setItem('DanhSachHangHoa', JSON.stringify(current));
    }
    //===============================
    // Add Các tham số cần lưu lại đẻ 
    // cache khi load lại form kiểm kho
    //===============================
    function addClassKK(name, id, value) {

        var current = localStorage.getItem('KiemKho');
        if (!current) {
            current = [];
        } else {
            current = JSON.parse(current);
        }
        if (current.length > 0) {
            for (var i = 0; i < current.length; i++) {
                if (current[i].NameId === id.toString()) {
                    current.splice(i, 1);
                    break;
                }
                if (i == current.length - 1) {
                    current.push({
                        NameClass: name,
                        NameId: id,
                        Value: value
                    });
                    break;
                }
            }
        }
        else {
            current.push({
                NameClass: name,
                NameId: id,
                Value: value
            });
        }
        localStorage.setItem('KiemKho', JSON.stringify(current));
    }

    $("#cbmahang").click(function () {
        //$(".mahang").toggle();
        addClassHH(".mahang", "cbmahang", $(this).val());
        self.addColum($(this).val())
    });
    $("#cbtenhang").click(function () {
        //$(".tenhang").toggle();
        addClassHH(".tenhang", "cbtenhang", $(this).val());
        self.addColum($(this).val())
    });
    $("#cbdvt").click(function () {
        //$(".tendvt").toggle();
        addClassHH(".tendvt", "cbdvt", $(this).val());
        self.addColum($(this).val())
    });
    $("#cbnhomhang").click(function () {
        //$(".nhomhang").toggle();
        addClassHH(".nhomhang", "cbnhomhang", $(this).val());
        self.addColum($(this).val())
    });
    $("#cbloaihang").click(function () {
        //$(".loaihang").toggle();
        addClassHH(".loaihang", "cbloaihang", $(this).val());
        self.addColum($(this).val())
    });
    $("#cbgiaban").click(function () {
        //$(".giaban").toggle();
        addClassHH(".giaban", "cbgiaban", $(this).val());
        self.addColum($(this).val())
    });
    $("#cbgiavon").click(function () {
        //$(".giavon").toggle();
        addClassHH(".giavon", "cbgiavon", $(this).val());
        self.addColum($(this).val())
    });
    $("#cbtonkho").click(function () {
        //$(".tonkho").toggle();
        addClassHH(".tonkho", "cbtonkho", $(this).val());
        self.addColum($(this).val())
    });
    $("#cbghichu").click(function () {
        //$(".ghichu").toggle();
        addClassHH(".ghichu", "cbghichu", $(this).val());
        self.addColum($(this).val())
    });
    $("#cbtrangthai").click(function () {
        //$(".trangthai").toggle();
        addClassHH(".trangthai", "cbtrangthai", $(this).val());
        self.addColum($(this).val())
    });

    $("#cbmahangkk").click(function () {
        $(".mahang").toggle();
        addClassKK(".mahang", "cbmahangkk", $(this).val());
        self.addColumKK($(this).val())
    });

    $("#cbthoigiankk").click(function () {
        $(".thoigian").toggle();
        addClassKK(".thoigian", "cbthoigiankk", $(this).val());
        self.addColumKK($(this).val())
    });

    $("#cbtongkk").click(function () {
        $(".tongchenhlech").toggle();
        addClassKK(".tongchenhlech", "cbtongkk", $(this).val());
        self.addColumKK($(this).val())
    });
    $("#cblechtangkk").click(function () {
        $(".sllechtang").toggle();
        addClassKK(".sllechtang", "cblechtangkk", $(this).val());
        self.addColumKK($(this).val())
    });
    $("#cblechgiamkk").click(function () {
        $(".sllechgiam").toggle();
        addClassKK(".sllechgiam", "cblechgiamkk", $(this).val());
        self.addColumKK($(this).val())
    });
    $("#cbghichukk").click(function () {
        $(".ghichu").toggle();
        addClassKK(".ghichu", "cbghichukk", $(this).val());
        self.addColumKK($(this).val())
    });
    $("#cbtrangthaikk").click(function () {
        $(".trangthai").toggle();
        addClassKK(".trangthai", "cbtrangthaikk", $(this).val());
        self.addColumKK($(this).val())
    });
    $("#cbtendonvikk").click(function () {
        $(".tendonvi").toggle();
        addClassKK(".tendonvi", "cbtendonvikk", $(this).val());
        self.addColumKK($(this).val())
    });

    self.ColumnsExcelKK = ko.observableArray();
    // xuất kiểm kho hàng hóa
    self.ExportExcel_KiemKho = function () {
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Kiểm kho hàng hóa",
            NoiDung: "Xuất báo cáo kiểm kho hàng hóa",
            NoiDungChiTiet: "Xuất báo cáo kiểm kho hàng hóa",
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        var arrDV = [];
        for (var i = 0; i < self.MangNhomDV().length; i++) {
            if ($.inArray(self.MangNhomDV()[i], arrDV) === -1) {
                arrDV.push(self.MangNhomDV()[i].ID);
            }
        }
        self.MangIDDV(arrDV);
        Insert_NhatKyThaoTac_1Param(objDiary);

        var columnHide = null;
        for (var i = 0; i < self.ColumnsExcelKK().length; i++) {
            if (i == 0) {
                columnHide = self.ColumnsExcelKK()[i];
            }
            else {
                columnHide = self.ColumnsExcelKK()[i] + "_" + columnHide;
            }
        }
        var url = '/api/DanhMuc/BH_HoaDonAPI/' + 'ExportExcel_KiemKho?loaiHoaDon=' + loaiHoaDon +
            '&maHoaDon=' + txtMaHDonKK_Excel + '&trangThai=' + txtTrangThaiKK_Excel + '&dayStart=' + dayStartKK_Excel + '&dayEnd=' + dayEndKK_Excel + "&columnsHide=" + columnHide + '&iddonvi=' + _IDchinhanh + '&arrChiNhanh=' + self.MangIDDV() + '&time=' + self.TodayBC() + '&TenChiNhanh=' + self.TenChiNhanh();
        window.location.href = url;
    }

    self.addColumKK = function (item) {
        if (self.ColumnsExcelKK().length < 1) {
            self.ColumnsExcelKK.push(item);
        }
        else {
            for (var i = 0; i < self.ColumnsExcelKK().length; i++) {
                if (self.ColumnsExcelKK()[i] === item) {
                    self.ColumnsExcelKK.splice(i, 1);
                    break;
                }
                if (i == self.ColumnsExcelKK().length - 1) {
                    self.ColumnsExcelKK.push(item);
                    break;
                }
            }
        }
        self.ColumnsExcelKK.sort();
    }

    // xuất kiểm kho chi tiết
    self.ExportExcel_KiemKhoChiTiet = function (item) {
        var columnHide = null;
        var url = '/api/DanhMuc/BH_HoaDonAPI/' + 'ExportExcel_KiemKhoChiTiet?ID_HoaDon=' + item.ID + '&columnsHide=' + columnHide;
        window.location.href = url;
        var objDiary = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Kiểm kho hàng hóa",
            NoiDung: "Xuất báo cáo kiểm kho chi tiết hàng hóa theo mã: " + item.MaHoaDon,
            NoiDungChiTiet: "Xuất báo cáo kiểm kho chi tiết hàng hóa theo mã: <a onclick=\"FindKiemKho('" + item.MaHoaDon + "')\">" + item.MaHoaDon + " </a>",
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        Insert_NhatKyThaoTac_1Param(objDiary);
    }
    //Download file teamplate excel format (*.xls)
    self.DownloadFileTeamplateXLS = function () {
        var url;
        var CauHinhHeThong = JSON.parse(localStorage.getItem("lc_CTThietLap"));
        if (CauHinhHeThong.LoHang) {
            url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "QuanLyHangHoaTheoLo/FileImport_DanhMucHangHoa.xls";
            window.location.href = url;
        }
        else {
            url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_DanhMucHangHoa.xls";
            window.location.href = url;
        }
    }
    //Download file teamplate excel format (*.xlsx)
    self.DownloadFileTeamplateXLSX = function () {
        var url;
        var CauHinhHeThong = JSON.parse(localStorage.getItem("lc_CTThietLap"));
        if (CauHinhHeThong.LoHang) {
            url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "QuanLyHangHoaTheoLo/FileImport_DanhMucHangHoa.xlsx";
            window.location.href = url;
        }
        else {
            url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_DanhMucHangHoa.xlsx";
            window.location.href = url;
        }
    }

    //Import file excel to DM_HangHoa
    self.onFileSelectedEvent = function (vm, evt) {
        /* set up XMLHttpRequest */
        if (evt.target.files.length > 0) {
            var url = URL.createObjectURL(evt.target.files[0]);
            var oReq = new XMLHttpRequest();
            oReq.open("GET", url, true);
            oReq.responseType = "arraybuffer";

            oReq.onload = function (e) {
                var arraybuffer = oReq.response;
                /* convert data to binary string */
                var data = new Uint8Array(arraybuffer);
                var arr = new Array();
                for (var i = 0; i !== data.length; ++i) arr[i] = String.fromCharCode(data[i]);
                var bstr = arr.join("");
                /* Call XLSX */
                var workbook = XLSX.read(bstr, { type: "binary" });
                /* DO SOMETHING WITH workbook HERE */
                var first_sheet_name = workbook.SheetNames[0];
                /* Get worksheet */
                var worksheet = workbook.Sheets[first_sheet_name];
                var dataObjects = XLSX.utils.sheet_to_json(worksheet, { raw: true });
                if (dataObjects.length > 0) {
                    for (var k = 0; k != dataObjects.length; ++k) {
                        var objdataInsert = dataObjects[k];
                        var _maHangHoa = objdataInsert.MaHangHoa;
                        var _tenHangHoa = objdataInsert.TenHangHoa;
                        //var _tenNhomHangHoa = objdataInsert.TenNhomHangHoa;
                        var _loaiHang = objdataInsert.LoaiHangHoa;
                        var _giaBan = objdataInsert.GiaBan;
                        var _giaVon = objdataInsert.GiaVon;
                        var _tonKho = objdataInsert.TonKho;
                        var _trangThai = objdataInsert.TrangThai;
                        //var _idNhomHH="";
                        //var strTenHangHoa="";
                        //var _ghiChu="";
                        var DM_HangHoa = {
                            MaHangHoa: _maHangHoa,
                            TenHangHoa: _tenHangHoa,
                            //TenNhomHangHoa: _tenNhomHangHoa,
                            LoaiHang: _loaiHang,
                            GiaBan: _giaBan,
                            GiaVon: _giaVon,
                            TonKho: _tonKho,
                            TrangThai: _trangThai,
                        };
                        var myData = {};
                        myData.objNewHH = DM_HangHoa;
                        //
                        $.ajax({
                            url: DMHangHoaUri + "PostDM_HangHoa",
                            type: 'POST',
                            async: true,
                            dataType: 'json',
                            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                            data: myData,
                            success: function (item) {
                                self.HangHoas.push(item);
                            },
                            statusCode: {
                                404: function () {
                                    self.error("page not found");
                                },
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                            },
                            complete: function () {
                            }
                        })
                            .fail(function (jqXHR, textStatus, errorThrown) {
                                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                            });
                    }
                }
            }
            oReq.send();
        }
    }

    self.check_kieuImport = ko.observable('1');
    self.checkPhuongThuc = function () {
        self.check_kieuImport('2');
        $('#modalpopup_selectImport').modal('hide');
    }
    self.ingoPhuongThuc = function () {
        self.check_kieuImport('1');
        $('#modalpopup_selectImport').modal('hide');
    }
    $('.chose_kieuImport li label input').on('click', function () {
        if ($(this).val() == 2 & self.check_kieuImport() == '1') {
            $('#modalpopup_selectImport').modal('show');
            self.check_kieuImport('2');
        }
    });
    self.fileNameExcel = ko.observable("Lựa chọn file Excel...");
    self.refreshFileSelectHH = function () {
        self.fileNameExcel("Lựa chọn file Excel...");
        $(".filterFileSelect").hide();
        $(".btnImportExcel").hide();
        document.getElementById('imageUploadForm').value = "";
    }
    $(".filterFileSelect").hide();
    $(".btnImportExcel").hide();
    self.SelectedFileImport = function (vm, evt) {
        self.fileNameExcel(evt.target.files[0].name)
        $(".filterFileSelect").show();
        $(".btnImportExcel").show();
        $(".NoteImport").show();
        $(".BangBaoLoi").hide();
    }
    self.ShowandHide = function () {
        self.insertArticleNews();
    }
    self.Upload = function () {
        //var url = DMHangHoaUri + "Upload_DMHH?upload=" + item;
        //window.open(url);
        ajaxHelper(DMHangHoaUri + "Upload_DMHH?upload=" + self.fileNameExcel(), "GET").done(function () { });
    }
    self.loiExcel = ko.observableArray();
    $(".BangBaoLoi").hide();
    self.updateExcel = ko.observableArray();
    self.insertArticleNews = function () {
        var CauHinhHeThong = JSON.parse(localStorage.getItem("lc_CTThietLap"));
        if (CauHinhHeThong.LoHang)//import hàng hóa có lô
        {
            self.loiExcel([]);
            self.updateExcel([]);
            $('.btnImportExcel ').gridLoader({
                style: "left: 30px;top: 1px;"
            });
            var formData = new FormData();
            var totalFiles = document.getElementById("imageUploadForm").files.length;
            for (var i = 0; i < totalFiles; i++) {
                var file = document.getElementById("imageUploadForm").files[i];
                formData.append("imageUploadForm", file);
            }
            $.ajax({
                type: "POST",
                url: DMHangHoaUri + "ImfortExcelToDanhMucHH_LoHang?ID_DonVi=" + _IDchinhanh + "&ID_NhanVien=" + _IDNhanVien + "&LoaiUpdate=" + self.check_kieuImport(),
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (xx) {
                    if (!xx.res) {
                        let item = xx.dataSoure;
                        for (var i = 0; i < item.length; i++) {
                            if (item[i].loaiError == 1)
                                self.loiExcel.push(item[i]);
                            else
                                self.updateExcel.push(item[i]);
                        }
                        if (self.loiExcel().length > 0) {
                            self.sumError(self.loiExcel().length);
                            $(".BangBaoLoi").show();
                            $(".NoteImport").hide();
                            $(".filterFileSelect").hide();
                            $(".btnImportExcel").hide();
                        }
                        else {
                            if (self.updateExcel().length > 0) {
                                $('#myModalinport').modal("hide");
                                $('#myModalinportUpdate').modal("show");
                            }
                        }
                        $('.btnImportExcel ').gridLoader({ show: false });
                    }
                    else {
                        self.sumError(0);
                        ShowMessage_Success("Import hàng hóa thành công");
                        var objDiary = {
                            ID_NhanVien: _IDNhanVien,
                            ID_DonVi: _IDchinhanh,
                            ChucNang: "Danh mục hàng hóa",
                            NoiDung: "Import danh sách hàng hóa",
                            NoiDungChiTiet: "Import danh sách hàng hóa",
                            LoaiNhatKy: 5 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                        };

                        Insert_NhatKyThaoTac_1Param(objDiary);

                        document.getElementById('imageUploadForm').value = "";
                        $(".NoteImport").show();
                        $(".filterFileSelect").hide();
                        $(".btnImportExcel").hide();
                        $(".BangBaoLoi").hide();
                        $("#myModalinport").modal("hide");
                        $('.btnImportExcel ').gridLoader({ show: false });
                        SearchHangHoa();
                        self.NoteNhomHang();
                        ReloadSearchHangHoa();
                    }
                },
            }).always(function () {
                $('.btnImportExcel ').gridLoader({ show: false });
            });
        }
        else {
            self.loiExcel([]);
            self.updateExcel([]);
            $('.btnImportExcel ').gridLoader({
                style: "left: 50px;top: 15px;"
            });
            var formData = new FormData();
            var totalFiles = document.getElementById("imageUploadForm").files.length;
            for (var i = 0; i < totalFiles; i++) {
                var file = document.getElementById("imageUploadForm").files[i];
                formData.append("imageUploadForm", file);
            }
            $.ajax({
                type: "POST",
                url: DMHangHoaUri + "ImfortExcelToDanhMucHH?ID_DonVi=" + _IDchinhanh + "&ID_NhanVien=" + _IDNhanVien + "&LoaiUpdate=" + self.check_kieuImport(),
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (xx) {
                    if (!xx.res) {
                        let item = xx.dataSoure;
                        for (let i = 0; i < item.length; i++) {
                            if (item[i].loaiError == 1)
                                self.loiExcel.push(item[i]);
                            else
                                self.updateExcel.push(item[i]);
                        }

                        if (self.loiExcel().length > 0) {
                            self.sumError(self.loiExcel().length);
                            $(".BangBaoLoi").show();
                            $(".NoteImport").hide();
                            $(".filterFileSelect").hide();
                            $(".btnImportExcel").hide();
                        }
                        else {
                            if (self.updateExcel().length > 0) {
                                $('#myModalinport').modal("hide");
                                $('#myModalinportUpdate').modal("show");
                            }
                        }
                        $('.btnImportExcel ').gridLoader({ show: false });
                    }
                    else {
                        self.sumError(0);
                        ShowMessage_Success("Import hàng hóa thành công");
                        let objDiary = {
                            ID_NhanVien: _IDNhanVien,
                            ID_DonVi: _IDchinhanh,
                            ChucNang: "Danh mục hàng hóa",
                            NoiDung: "Import danh sách hàng hóa",
                            NoiDungChiTiet: "Import danh sách hàng hóa",
                            LoaiNhatKy: 5 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                        };
                        Insert_NhatKyThaoTac_1Param(objDiary);

                        document.getElementById('imageUploadForm').value = "";
                        $(".NoteImport").show();
                        $(".filterFileSelect").hide();
                        $(".btnImportExcel").hide();
                        $(".BangBaoLoi").hide();
                        $("#myModalinport").modal("hide");
                        $('.btnImportExcel ').gridLoader({ show: false });
                        SearchHangHoa();
                        self.NoteNhomHang();
                        ReloadSearchHangHoa();
                    }
                }
            }).always(function () {
                $('.btnImportExcel ').gridLoader({ show: false });
            })
        }
    }

    self.addRownError = ko.observableArray();
    self.DoneWithError = function () {
        var rownError = '';
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
        self.addRownError = self.addRownError.sort(function (a, b) {
            var x = a, y = b;
            return x > y ? 1 : x < y ? -1 : 0;
        })
        for (var i = 0; i < self.addRownError().length; i++) {
            if (i == 0)
                rownError = self.addRownError()[i];
            else
                rownError = rownError + "_" + self.addRownError()[i];
        }
        var CauHinhHeThong = JSON.parse(localStorage.getItem("lc_CTThietLap"));
        if (CauHinhHeThong.LoHang)//import hàng hóa có lô
        {
            $('.table_h10').gridLoader();
            var formData = new FormData();
            var totalFiles = document.getElementById("imageUploadForm").files.length;
            for (var i = 0; i < totalFiles; i++) {
                var file = document.getElementById("imageUploadForm").files[i];
                formData.append("imageUploadForm", file);
            }
            $.ajax({
                type: "POST",
                url: DMHangHoaUri + "ImfortHangHoa_LoHang_WithError?ID_DonVi=" + _IDchinhanh + "&ID_NhanVien=" + _IDNhanVien + "&RownError=" + rownError + "&LoaiUpdate=" + self.check_kieuImport(),
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (xx) {
                    if (xx.res) {
                        ShowMessage_Success("Import hàng hóa thành công!");
                        let objDiary = {
                            ID_NhanVien: _IDNhanVien,
                            ID_DonVi: _IDchinhanh,
                            ChucNang: "Danh mục hàng hóa",
                            NoiDung: "Import danh sách hàng hóa",
                            NoiDungChiTiet: "Import danh sách hàng hóa",
                            LoaiNhatKy: 5 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                        };
                        Insert_NhatKyThaoTac_1Param(objDiary);

                        document.getElementById('imageUploadForm').value = "";
                        $(".NoteImport").show();
                        $(".filterFileSelect").hide();
                        $(".btnImportExcel").hide();
                        $(".BangBaoLoi").hide();
                        $("#myModalinport").modal("hide");
                        $('#myModalinportUpdate').modal("hide");
                        SearchHangHoa();
                    }
                    else {
                        console.log(xx.dataSoure)
                        ShowMessage_Danger("Import hàng hóa thất bại!");
                    }
                }
            }).always(function (x) {
                $('.table_h10').gridLoader({ show: false });
            })
        }
        else {
            $('.table_h10').gridLoader();

            var formData = new FormData();
            var totalFiles = document.getElementById("imageUploadForm").files.length;
            for (var i = 0; i < totalFiles; i++) {
                var file = document.getElementById("imageUploadForm").files[i];
                formData.append("imageUploadForm", file);
            }
            $.ajax({
                type: "POST",
                url: DMHangHoaUri + "ImfortHangHoa_WithError?ID_DonVi=" + _IDchinhanh + "&ID_NhanVien=" + _IDNhanVien + "&RownError=" + rownError + "&LoaiUpdate=" + self.check_kieuImport(),
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (xx) {
                    if (xx.res) {
                        ShowMessage_Success("Import hàng hóa thành công!");
                        var objDiary = {
                            ID_NhanVien: _IDNhanVien,
                            ID_DonVi: _IDchinhanh,
                            ChucNang: "Danh mục hàng hóa",
                            NoiDung: "Import danh sách hàng hóa",
                            NoiDungChiTiet: "Import danh sách hàng hóa",
                            LoaiNhatKy: 5 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                        };
                        Insert_NhatKyThaoTac_1Param(objDiary);

                        document.getElementById('imageUploadForm').value = "";
                        $(".NoteImport").show();
                        $(".filterFileSelect").hide();
                        $(".btnImportExcel").hide();
                        $(".BangBaoLoi").hide();
                        $("#myModalinport").modal("hide");
                        $('#myModalinportUpdate').modal("hide");
                        SearchHangHoa();
                    }
                    else {
                        console.log(xx.dataSoure)
                        ShowMessage_Danger("Import hàng hóa thất bại");
                    }
                }
            }).always(function (x) {
                $('.table_h10').gridLoader({ show: false });
            })
        }
    }
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

    //Insert Image in DM_HangHoa
    self.arrImage = ko.observableArray();
    self.InsertImage = function (id) {
        //var i = 0;
        //function anhhang() {
        //    if (i < self.files().length) {
        //        var formData = new FormData();
        //        formData.append("imageUpHHForm", self.files()[i].file);
        //        $.ajax({
        //            type: "POST",
        //            url: '/api/DanhMuc/DM_HangHoaAPI/' + "ImageUpload/" + id,
        //            data: formData,
        //            dataType: 'json',
        //            contentType: false,
        //            processData: false,
        //            success: function (response) {
        //                i++;
        //                anhhang();
        //            },
        //            error: function (jqXHR, textStatus, errorThrown) {
        //            }
        //        });
        //    }
        //}
        //anhhang();
        let myData = {};
        let formData = new FormData();
        for (let i = 0; i < self.files().length; i++) {
            formData.append("files", self.files()[i].file);
        }
        myData.Subdomain = VHeader.SubDomain;
        myData.Function = "2";
        myData.Id = id;
        myData.files = formData;
        var result = Open24FileManager.UploadImage(myData);
        if (result.length > 0) {
            $.ajax({
                url: '/api/DanhMuc/DM_HangHoaAPI/' + "UpdateAnhHangHoa?id=" + id,
                type: "POST",
                data: JSON.stringify(result),
                contentType: "application/json",
                dataType: "JSON",
                success: function (data, textStatus, jqXHR) {
                },
                error: function (jqXHR, textStatus, errorThrown) {

                }
            });
        }

    }

    self.files = ko.observableArray([]);
    self.fileSelect = function (elemet, event) {
        var files = event.target.files;// FileList object
        // Loop through the FileList and render image files as thumbnails.
        for (var i = 0; i < files.length; i++) {
            var f = files[i];
            // Only process image files.
            if (!f.type.match('image.*')) {
                continue;
            }
            var size = parseFloat(f.size / 1024).toFixed(2);
            $('.errorAnh').text("");
            $('.errorAnhHH').text("");
            if (size > 2048) {
                $('.errorAnh').text('Dung lượng file không được lớn quá 2Mb');
                $('.errorAnhHH').text('Dung lượng file không được lớn quá 2Mb');
            }
            if (size <= 2048) {
                var reader = new FileReader();
                // Closure to capture the file information.
                reader.onload = (function (theFile) {
                    return function (e) {
                        self.files.push(new FileModel(theFile, e.target.result));
                    };
                })(f);
                // Read in the image file as a data URL.
                reader.readAsDataURL(f);
            }
        }
    };

    self.arrAnhAvt = ko.observableArray();
    self.loadAnh = function (item) {
        var caption = modelHangHoa.HangHoas().find(p => p.ID === itemcheckhh).TenHangHoa;
        VImageView.ListImg = self.DM_HangHoa_Anh().map(p => ({
            src: Open24FileManager.hostUrl + p.URLAnh,
            caption: caption,
            active: false
        }));
        VImageView.openModal();
    }

    self.ThayAnhNho = function (item) {
        var i = self.DM_HangHoa_Anh().indexOf(item);
        if (i >= 1) {
            var array = self.DM_HangHoa_Anh();
            var tempimg = self.DM_HangHoa_Anh()[0];
            array[0] = array[i];
            array[i] = tempimg;
        }
        self.DM_HangHoa_Anh(array);
        self.DM_HangHoa_Anh.refresh();
        self.AnhDaiDien(item);
    };

    self.closeAnh = function () {
        $(".model-images").hide();
        $(".modal-ontop").hide();
    }
    self.deleteURLAnh = ko.observable();
    self.modalDeleteAnh = function (item) {
        if (item.ID !== undefined) {
            $('#modalpopup_XoaAnh').modal('show');
            self.deleteID(item.ID);
            self.deleteURLAnh(item.URLAnh);
        } else {
            xoaAnh(item);
        }
    }

    function xoaAnh(item) {
        self.files.remove(item);
    }

    self.deleteAnh = function (item) {
        if (self.deleteID() !== undefined) {
            ajaxHelper(DMHangHoaUri + 'DeleteDM_HangHoa_Anh/' + self.deleteID(), 'DELETE').done(function (data) {
                if (data == "") {
                    for (var i = 0; i < self.DM_HangHoa_Anh().length; i++) {
                        if (self.deleteID() == self.DM_HangHoa_Anh()[i].ID) {
                            self.DM_HangHoa_Anh.splice(i, 1);
                        }
                    };
                    Open24FileManager.RemoveFiles([self.deleteURLAnh()]);
                    ShowMessage_Success("Xóa ảnh thành công!");
                    $('#modalpopup_XoaAnh').modal('hide');
                } else {
                    ShowMessage_Danger("Không thể xóa!");
                    $('#modalpopup_XoaAnh').modal('hide');
                }
            })
        }
    }

    //trinhpv import HangHoakiemKho
    self.ExportHangHoa_CapNhat = function () {
        $('.table_h10').gridLoader();
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
        self.addRownError = self.addRownError.sort(function (a, b) {
            var x = a, y = b;
            return x > y ? 1 : x < y ? -1 : 0;
        })
        for (var i = 0; i < self.addRownError().length; i++) {
            if (i == 0)
                rownError = self.addRownError()[i];
            else
                rownError = rownError + "_" + self.addRownError()[i];
        }

        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadForm").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadForm").files[i];
            formData.append("imageUploadForm", file);
        }
        $.ajax({
            type: "POST",
            url: DMHangHoaUri + "ExportHangHoa_CapNhat?ID_DonVi=" + _IDchinhanh + "&RownError=" + rownError + "&LoaiUpdate=" + self.check_kieuImport(),
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (item) {
                $("div[id ^= 'wait']").text("");
            },
            statusCode: {
            },
            error: function () {
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "DanhSachHangHoaCapNhat.xlsx";
                window.location.href = url;
                //ShowMessage_Danger( "Xuất danh sách hàng hóa cập nhật thất bại.");
                $('.table_h10').gridLoader({ show: false });
            },
        });
    }
    self.updateHangHoa = function () {
        self.DoneWithError();
    }
    self.ignoreError = function () {
        if (self.updateExcel().length > 0) {
            $('#myModalinport').modal("hide");
            $('#myModalinportUpdate').modal("show");
        }
        else
            self.DoneWithError();
    }
    //Download file teamplate excel format (*.xls)
    self.DownloadFileTeamplateXLS_KiemKho = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_DanhSachHangKiemKho.xls";
        window.location.href = url;
    }
    //Download file teamplate excel format (*.xlsx)
    self.DownloadFileTeamplateXLSX_KiemKho = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_DanhSachHangKiemKho.xlsx";
        window.location.href = url;
    }
    self.fileNameExcel = ko.observable("Lựa chọn file Excel...");
    $(".btnImportExcel").hide();
    $(".filterFileSelect").hide();
    self.deleteFileSelect = function () {
        self.fileNameExcel("Lựa chọn file Excel...");
        $(".filterFileSelect").hide();
        $(".btnImportExcel").hide();
        self.visibleImport(true);
        self.loiExcel([]);
        $(".BangBaoLoi").hide();
        $(".NoteImport").show();
        document.getElementById('imageUploadForm').value = "";
    }
    self.refreshFileSelect = function () {
        self.importKiemKho();
    }

    self.clickImportHangHoas = function () {
        self.visibleImport(true);
        self.deleteFileSelect();
    }
    self.visibleImport = ko.observable(true);
    self.notvisibleImport = ko.computed(function () {
        return !self.visibleImport();

    });
    self.ChoseFileAgain = function () {

    }
    self.SelectedFileImport = function (vm, evt) {
        self.fileNameExcel(evt.target.files[0].name);
        self.visibleImport(false);
        $(".btnImportExcel").show();
        $(".filterFileSelect").show();
        self.loiExcel([]);
        self.sumError(0);
    }
    self.loiExcel = ko.observableArray();

    async function GetTonKho_byIDQuyDoi(param) {
        let xx = await ajaxHelper(DMHangHoaUri + 'GetTonKho_byIDQuyDois', 'POST', param).done(function (x) { }).then(function (x) {
            if (x.res) return x.data;
            return [];
        }).fail(function () {
            return [];
        });
        return xx;
    }

    self.importKiemKho = async function () {
        $('.choose-file').gridLoader({
            style: "left: 200px;top: 77px;"
        });
        var _ngayKk = $('#datetimepicker').val();
        if (_ngayKk === "") {
            _ngayKk = moment(new Date()).format('DD/MM/YYYY HH:mm');
        }
        var check = CheckNgayLapHD_format(_ngayKk);
        if (!check) {
            $('.choose-file').gridLoader({ show: false });
            return false;
        }
        var _ngayKiemKe = moment(_ngayKk, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadForm").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadForm").files[i];
            formData.append("imageUploadForm", file);
        }

        const dataErr = await $.ajax({
            type: "POST",
            url: DMHangHoaUri + "ImfortExcelKiemKho",
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false
        }).done()
            .then(function (data) {
                return data
            })

        if (dataErr.length > 0) {
            self.loiExcel(dataErr);
            $(".BangBaoLoi").show();
            $(".btnImportExcel").hide();
            $(".refreshFile").show();
            $(".deleteFile").hide();
            $('.choose-file').gridLoader({ show: false });
        }
        else {
            const hdct = await $.ajax({
                type: "POST",
                url: DMHangHoaUri + "getList_DanhSachHangKiemKho?iddonvi=" + _IDchinhanh + '&timeKK=' + _ngayKiemKe,
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false
            }).done()
                .then(function (data) {
                    return data;
                });

            let arrIDQuyDoi = $.unique(hdct.map(function (x) {
                return x.ID_DonViQuiDoi;
            }));
            let arrIDLoHang = hdct.map(function (x) {
                return x.ID_LoHang;
            }).filter(x => x !== null);

            const paramCheckTon = {
                ID_ChiNhanh: _IDchinhanh,
                ToDate: _ngayKiemKe,
                ListIDQuyDoi: arrIDQuyDoi,
                ListIDLoHang: arrIDLoHang,
            };

            const dataTonkho = await GetTonKho_byIDQuyDoi(paramCheckTon);

            for (let i = 0; i < hdct.length; i++) {
                let itFor = hdct[i];
                let dataDB = $.grep(dataTonkho, function (o) {
                    return o.ID_DonViQuiDoi === itFor.ID_DonViQuiDoi
                        && (!itFor.QuanLyTheoLoHang || (o.ID_LoHang === itFor.ID_LoHang))
                });

                hdct[i].ID_HangHoa = hdct[i].ID;
                hdct[i].SoLuong = 0;
                if (dataDB.length > 0) {
                    hdct[i].SoLuong = dataDB[0].TonKho;// soluongDB
                }
                hdct[i].TienChietKhau = hdct[i].ThanhTien - hdct[i].SoLuong;// soluong thucte - soluongDB
                hdct[i].ThanhToan = hdct[i].TienChietKhau * hdct[i].GiaVon;//gtri lech
                //hdct[i].QuanLyTheoLoHang = hdct[i].QuanLyTheoLoHang;
                //hdct[i].ThuocTinh_GiaTri = hdct[i].ThuocTinh_GiaTri;
                //hdct[i].DonViTinh = hdct[i].DonViTinh;
                var idLoHang = hdct[i].ID_LoHang;
                var itemLot = [];
                if (idLoHang !== null) {
                    itemLot = $.grep(self.ListLoHang(), function (x) {
                        return x.ID === idLoHang;
                    });
                }
                var rd = Math.floor(Math.random() * 1000000 + 1);
                hdct[i].ID_Random = 'IDRandom' + rd + '_';
                hdct[i].ID_LoHang = idLoHang;
                hdct[i].MaLoHang = hdct[i].MaLoHang === '' ? null : hdct[i].MaLoHang;
                hdct[i].NgaySanXuat = hdct[i].QuanLyTheoLoHang ? (itemLot.length > 0 ? itemLot[0].NgaySanXuat : '') : '';
                hdct[i].NgayHetHan = hdct[i].QuanLyTheoLoHang ? (itemLot.length > 0 ? itemLot[0].NgayHetHan : '') : '';
            }

            self.newKiemKho().BH_KiemKho_ChiTiet(hdct);
            for (var i = 0; i < self.newKiemKho().BH_KiemKho_ChiTiet().length; i++) {
                if (parseInt(self.newKiemKho().BH_KiemKho_ChiTiet()[i].ThanhTien) === self.newKiemKho().BH_KiemKho_ChiTiet()[i].SoLuong) {
                    self.SLKhops.push(self.newKiemKho().BH_KiemKho_ChiTiet()[i]);
                }
                if (self.newKiemKho().BH_KiemKho_ChiTiet()[i].ThanhTien === null) {
                    self.SLChuaKiems.push(self.newKiemKho().BH_KiemKho_ChiTiet()[i]);
                }
                if (self.newKiemKho().BH_KiemKho_ChiTiet()[i].ThanhTien !== null && parseInt(self.newKiemKho().BH_KiemKho_ChiTiet()[i].ThanhTien) !== self.newKiemKho().BH_KiemKho_ChiTiet()[i].SoLuong) {
                    self.SLLechs.push(self.newKiemKho().BH_KiemKho_ChiTiet()[i]);
                }
            }
            var objectStore = db.transaction(table, "readwrite").objectStore(table);
            var req = objectStore.openCursor(key_Add);
            req.onsuccess = function (evt) {
                objectStore.delete(key_Add);
                objectStore.add({ Key: key_Add, Value: JSON.stringify(self.newKiemKho().BH_KiemKho_ChiTiet()) });

            };
            if (self.newKiemKho().BH_KiemKho_ChiTiet().length > 0) {
                $('#importKiemKho').hide();
            }
            else {
                $('#importKiemKho').show();
                self.deleteFileSelect();
            }
            $('#tongitem').text(self.newKiemKho().BH_KiemKho_ChiTiet().length);
            $('#tongitemkhop').text(self.SLKhops().length);
            $('#tongitemlech').text(self.SLLechs().length);
            $('#tongitemchuakiem').text(self.SLChuaKiems().length);
            self.TinhLaiLech();
            self.TinhLaiSLKhop();
            self.TinhLaiSLThuc();
            UpdateAgain_ListDVT();
            //$("#wait").remove();
            $('.choose-file').gridLoader({ show: false });
            ShowMessage_Success("Import file thành công");
        }
    }
    //keycode
    var ttsl = 0;
    var thisSL;

    self.selectedSL = function (item) {
        thisSL = "#slthuc_" + item.ID_Random + item.ID_DonViQuiDoi;
        $(thisSL).focus().select();
        ttsl = $(thisSL).closest('tr').index() + self.currentPageAll() * 10;
    }
    $(document).keyup(function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (self.newKiemKho().BH_KiemKho_ChiTiet().length > 0) {

            if (code === 36) {
                if (self.currentPageAll() === 0) {
                    ttsl = 0;
                    thisSL = "#slthuc_" + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_Random + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_DonViQuiDoi;
                    $(thisSL).focus().select();
                }
                else {
                    ttsl = 10 + self.currentPageAll() * 10 - 10;
                    thisSL = "#slthuc_" + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_Random + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_DonViQuiDoi;
                    $(thisSL).focus().select();
                }
            }
            if (code === 35) {
                if (self.currentPageAll() === 0) {
                    ttsl = self.newKiemKho().BH_KiemKho_ChiTiet().length >= 10 ? 9 : self.newKiemKho().BH_KiemKho_ChiTiet().length - 1;
                    thisSL = "#slthuc_" + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_Random + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_DonViQuiDoi;
                    $(thisSL).focus().select();
                }
                else {
                    ttsl = self.toitemAll() - 1;
                    thisSL = "#slthuc_" + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_Random + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_DonViQuiDoi;
                    $(thisSL).focus().select();
                }
            }
            if (code === 13 && ttsl >= 0 && $(thisSL).is(":focus")) {
                ttsl = ttsl + 1 - self.currentPageAll() * 10;
                if (self.newKiemKho().BH_KiemKho_ChiTiet().length >= 10) {
                    if (self.currentPageAll() === 0) {
                        if (ttsl < 10) {
                            thisSL = "#slthuc_" + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_Random + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_DonViQuiDoi;
                            $(thisSL).focus().select();
                        }
                        else {
                            ttsl = 0;
                            thisSL = "#slthuc_" + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_Random + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_DonViQuiDoi;
                            $(thisSL).focus().select();
                        }
                    }
                    else {
                        ttsl = self.currentPageAll() * 10 + ttsl;
                        if (ttsl < self.toitemAll()) {
                            thisSL = "#slthuc_" + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_Random + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_DonViQuiDoi;
                            $(thisSL).focus().select();
                        }
                        else {
                            ttsl = self.currentPageAll() * 10;
                            thisSL = "#slthuc_" + self.newKiemKho().BH_KiemKho_ChiTiet()[self.currentPageAll() * 10].ID_Random + self.newKiemKho().BH_KiemKho_ChiTiet()[self.currentPageAll() * 10].ID_DonViQuiDoi;
                            $(thisSL).focus().select();
                        }
                    }
                }
                if (self.newKiemKho().BH_KiemKho_ChiTiet().length < 10) {
                    if (ttsl < self.newKiemKho().BH_KiemKho_ChiTiet().length) {
                        thisSL = "#slthuc_" + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_Random + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_DonViQuiDoi;
                        $(thisSL).focus().select();
                    }
                    else {
                        ttsl = 0;
                        thisSL = "#slthuc_" + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_Random + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_DonViQuiDoi;
                        $(thisSL).focus().select();
                    }
                }
            }
            if (code === 16 && ttsl >= 0 && $(thisSL).is(":focus")) {
                ttsl = ttsl - 1 - self.currentPageAll() * 10;
                if (self.currentPageAll() === 0) {
                    if (ttsl >= 0) {
                        thisSL = "#slthuc_" + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_Random + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_DonViQuiDoi;
                        $(thisSL).focus().select();
                    }
                    else {
                        ttsl = self.newKiemKho().BH_KiemKho_ChiTiet().length >= 10 ? 9 : self.newKiemKho().BH_KiemKho_ChiTiet().length - 1;
                        thisSL = "#slthuc_" + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_Random + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_DonViQuiDoi;
                        $(thisSL).focus().select();
                    }
                }
                else {
                    ttsl = self.currentPageAll() * 10 + ttsl;
                    if (ttsl >= self.currentPageAll() * 10) {
                        thisSL = "#slthuc_" + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_Random + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_DonViQuiDoi;
                        $(thisSL).focus().select();
                    }
                    else {
                        ttsl = self.toitemAll() - 1;
                        thisSL = "#slthuc_" + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_Random + self.newKiemKho().BH_KiemKho_ChiTiet()[ttsl].ID_DonViQuiDoi;
                        $(thisSL).focus().select();
                    }
                }
            }
        }
    })
    //keycode sl khớp
    var ttslKhop = 0;
    var thisSLKhop;

    self.selectedSLKhop = function (item) {
        thisSLKhop = "#slthuckhop_" + item.ID_Random + item.ID_DonViQuiDoi;
        $(thisSLKhop).focus().select();
        ttslKhop = $(thisSLKhop).closest('tr').index() + self.currentPageKhop() * 10;
    }
    $(document).keyup(function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (self.SLKhops().length > 0) {
            if (code === 36) {
                if (self.currentPageKhop() === 0) {
                    ttslKhop = 0;
                    thisSLKhop = "#slthuckhop_" + self.SLKhops()[ttslKhop].ID_Random + self.SLKhops()[ttslKhop].ID_DonViQuiDoi;
                    $(thisSLKhop).focus().select();
                }
                else {
                    ttslKhop = 10 + self.currentPageKhop() * 10 - 10;
                    thisSLKhop = "#slthuckhop_" + self.SLKhops()[ttslKhop].ID_Random + self.SLKhops()[ttslKhop].ID_DonViQuiDoi;
                    $(thisSLKhop).focus().select();
                }
            }
            if (code === 35) {
                if (self.currentPageKhop() === 0) {
                    ttslKhop = self.SLKhops().length >= 10 ? 9 : self.SLKhops().length - 1;
                    thisSLKhop = "#slthuckhop_" + self.SLKhops()[ttslKhop].ID_Random + self.SLKhops()[ttslKhop].ID_DonViQuiDoi;
                    $(thisSLKhop).focus().select();
                }
                else {
                    ttslKhop = self.toitemKhop() - 1;
                    thisSLKhop = "#slthuckhop_" + self.SLKhops()[ttslKhop].ID_Random + self.SLKhops()[ttslKhop].ID_DonViQuiDoi;
                    $(thisSLKhop).focus().select();
                }
            }
            if (code === 13 && ttslKhop >= 0 && $(thisSLKhop).is(":focus")) {
                ttslKhop = ttslKhop + 1 - self.currentPageKhop() * 10;
                if (self.SLKhops().length >= 10) {
                    if (self.currentPageKhop() === 0) {
                        if (ttslKhop < 10) {
                            thisSLKhop = "#slthuckhop_" + self.SLKhops()[ttslKhop].ID_Random + self.SLKhops()[ttslKhop].ID_DonViQuiDoi;
                            $(thisSLKhop).focus().select();
                        }
                        else {
                            ttslKhop = 0;
                            thisSLKhop = "#slthuckhop_" + self.SLKhops()[ttslKhop].ID_Random + self.SLKhops()[ttslKhop].ID_DonViQuiDoi;
                            $(thisSLKhop).focus().select();
                        }
                    }
                    else {
                        ttslKhop = self.currentPageKhop() * 10 + ttslKhop;
                        if (ttslKhop < self.toitemKhop()) {
                            thisSLKhop = "#slthuckhop_ + self.SLKhops()[ttslKhop].ID_Random" + self.SLKhops()[ttslKhop].ID_DonViQuiDoi;
                            $(thisSLKhop).focus().select();
                        }
                        else {
                            ttslKhop = self.currentPageKhop() * 10;
                            thisSLKhop = "#slthuckhop_" + self.SLKhops()[self.currentPageKhop() * 10].ID_Random + self.SLKhops()[self.currentPageKhop() * 10].ID_DonViQuiDoi;
                            $(thisSLKhop).focus().select();
                        }
                    }
                }
                if (self.SLKhops().length < 10) {
                    if (ttslKhop < self.SLKhops().length) {
                        thisSLKhop = "#slthuckhop_" + self.SLKhops()[ttslKhop].ID_Random + self.SLKhops()[ttslKhop].ID_DonViQuiDoi;
                        $(thisSLKhop).focus().select();
                    }
                    else {
                        ttslKhop = 0;
                        thisSLKhop = "#slthuckhop_" + self.SLKhops()[ttslKhop].ID_Random + self.SLKhops()[ttslKhop].ID_DonViQuiDoi;
                        $(thisSLKhop).focus().select();
                    }
                }
            }
            if (code === 16 && ttslKhop >= 0 && $(thisSLKhop).is(":focus")) {
                ttslKhop = ttslKhop - 1 - self.currentPageKhop() * 10;
                if (self.currentPageKhop() === 0) {
                    if (ttslKhop >= 0) {
                        thisSLKhop = "#slthuckhop_" + self.SLKhops()[ttslKhop].ID_Random + self.SLKhops()[ttslKhop].ID_DonViQuiDoi;
                        $(thisSLKhop).focus().select();
                    }
                    else {
                        ttslKhop = self.SLKhops().length >= 10 ? 9 : self.SLKhops().length - 1;
                        thisSLKhop = "#slthuckhop_" + self.SLKhops()[ttslKhop].ID_Random + self.SLKhops()[ttslKhop].ID_DonViQuiDoi;
                        $(thisSLKhop).focus().select();
                    }
                }
                else {
                    ttslKhop = self.currentPageKhop() * 10 + ttslKhop;
                    if (ttslKhop >= self.currentPageKhop() * 10) {
                        thisSLKhop = "#slthuckhop_" + self.SLKhops()[ttslKhop].ID_Random + self.SLKhops()[ttslKhop].ID_DonViQuiDoi;
                        $(thisSLKhop).focus().select();
                    }
                    else {
                        ttslKhop = self.toitemKhop() - 1;
                        thisSLKhop = "#slthuckhop_" + self.SLKhops()[ttslKhop].ID_Random + self.SLKhops()[ttslKhop].ID_DonViQuiDoi;
                        $(thisSLKhop).focus().select();
                    }
                }
            }
        }
    })

    //key code sl lệch

    var ttslLech = 0;
    var thisSLLech;

    self.selectedSLLech = function (item) {
        thisSLLech = "#slthuclech_" + item.ID_Random + item.ID_DonViQuiDoi;
        $(thisSLLech).focus().select();
        ttslLech = $(thisSLLech).closest('tr').index() + self.currentPageLech() * 10;
    }
    $(document).keyup(function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (self.SLLechs().length > 0) {
            if (code === 36) {
                if (self.currentPageLech() === 0) {
                    ttslLech = 0;
                    thisSLLech = "#slthuclech_" + self.SLLechs()[ttslLech].ID_Random + self.SLLechs()[ttslLech].ID_DonViQuiDoi;
                    $(thisSLLech).focus().select();
                }
                else {
                    ttslLech = 10 + self.currentPageLech() * 10 - 10;
                    thisSLLech = "#slthuclech_" + self.SLLechs()[ttslLech].ID_Random + self.SLLechs()[ttslLech].ID_DonViQuiDoi;
                    $(thisSLLech).focus().select();
                }
            }
            if (code === 35) {
                if (self.currentPageLech() === 0) {
                    ttslLech = self.SLLechs().length >= 10 ? 9 : self.SLLechs().length - 1;
                    thisSLLech = "#slthuclech_" + self.SLLechs()[ttslLech].ID_Random + self.SLLechs()[ttslLech].ID_DonViQuiDoi;
                    $(thisSLLech).focus().select();
                }
                else {
                    ttslLech = self.toitemLech() - 1;
                    thisSLLech = "#slthuclech_" + self.SLLechs()[ttslLech].ID_Random + self.SLLechs()[ttslLech].ID_DonViQuiDoi;
                    $(thisSLLech).focus().select();
                }
            }
            if (code === 13 && ttslLech >= 0 && $(thisSLLech).is(":focus")) {
                ttslLech = ttslLech + 1 - self.currentPageLech() * 10;
                if (self.SLLechs().length >= 10) {
                    if (self.currentPageLech() === 0) {
                        if (ttslLech < 10) {
                            thisSLLech = "#slthuclech_" + self.SLLechs()[ttslLech].ID_Random + self.SLLechs()[ttslLech].ID_DonViQuiDoi;
                            $(thisSLLech).focus().select();
                        }
                        else {
                            ttslLech = 0;
                            thisSLLech = "#slthuclech_" + self.SLLechs()[ttslLech].ID_Random + self.SLLechs()[ttslLech].ID_DonViQuiDoi;
                            $(thisSLLech).focus().select();
                        }
                    }
                    else {
                        ttslLech = self.currentPageLech() * 10 + ttslLech;
                        if (ttslLech < self.toitemLech()) {
                            thisSLLech = "#slthuclech_" + self.SLLechs()[ttslLech].ID_Random + self.SLLechs()[ttslLech].ID_DonViQuiDoi;
                            $(thisSLLech).focus().select();
                        }
                        else {
                            ttslLech = self.currentPageLech() * 10;
                            thisSLLech = "#slthuclech_" + self.SLLechs()[self.currentPageLech() * 10].ID_Random + self.SLLechs()[self.currentPageLech() * 10].ID_DonViQuiDoi;
                            $(thisSLLech).focus().select();
                        }
                    }
                }
                if (self.SLLechs().length < 10) {
                    if (ttslLech < self.SLLechs().length) {
                        thisSLLech = "#slthuclech_" + self.SLLechs()[ttslLech].ID_Random + self.SLLechs()[ttslLech].ID_DonViQuiDoi;
                        $(thisSLLech).focus().select();
                    }
                    else {
                        ttslLech = 0;
                        thisSLLech = "#slthuclech_" + self.SLLechs()[ttslLech].ID_Random + self.SLLechs()[ttslLech].ID_DonViQuiDoi;
                        $(thisSLLech).focus().select();
                    }
                }
            }
            if (code === 16 && ttslLech >= 0 && $(thisSLLech).is(":focus")) {
                ttslLech = ttslLech - 1 - self.currentPageLech() * 10;
                if (self.currentPageLech() === 0) {
                    if (ttslLech >= 0) {
                        thisSLLech = "#slthuclech_" + self.SLLechs()[ttslLech].ID_Random + self.SLLechs()[ttslLech].ID_DonViQuiDoi;
                        $(thisSLLech).focus().select();
                    }
                    else {
                        ttslLech = self.SLLechs().length >= 10 ? 9 : self.SLLechs().length - 1;
                        thisSLLech = "#slthuclech_" + self.SLLechs()[ttslLech].ID_Random + self.SLLechs()[ttslLech].ID_DonViQuiDoi;
                        $(thisSLLech).focus().select();
                    }
                }
                else {
                    ttslLech = self.currentPageLech() * 10 + ttslLech;
                    if (ttslLech >= self.currentPageLech() * 10) {
                        thisSLLech = "#slthuclech_" + self.SLLechs()[ttslLech].ID_Random + self.SLLechs()[ttslLech].ID_DonViQuiDoi;
                        $(thisSLLech).focus().select();
                    }
                    else {
                        ttslLech = self.toitemLech() - 1;
                        thisSLLech = "#slthuclech_" + self.SLLechs()[ttslLech].ID_Random + self.SLLechs()[ttslLech].ID_DonViQuiDoi;
                        $(thisSLLech).focus().select();
                    }
                }
            }
        }
    })

    //key code slchuakiem
    var ttslChuaK = 0;
    var thisSLChuaK;

    self.selectedSLChuaK = function (item) {
        thisSLChuaK = "#slthucchuakiem_" + item.ID_Random + item.ID_DonViQuiDoi;
        $(thisSLChuaK).focus().select();
        ttslChuaK = $(thisSLChuaK).closest('tr').index() + self.currentPageChuaK() * 10;
    }
    $(document).keyup(function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (self.SLChuaKiems().length > 0) {
            if (code === 36) {
                if (self.currentPageChuaK() === 0) {
                    ttslChuaK = 0;
                    thisSLChuaK = "#slthucchuakiem_" + self.SLChuaKiems()[ttslChuaK].ID_Random + self.SLChuaKiems()[ttslChuaK].ID_DonViQuiDoi;
                    $(thisSLChuaK).focus().select();
                }
                else {
                    ttslChuaK = 10 + self.currentPageChuaK() * 10 - 10;
                    thisSLChuaK = "#slthucchuakiem_" + self.SLChuaKiems()[ttslChuaK].ID_Random + self.SLChuaKiems()[ttslChuaK].ID_DonViQuiDoi;
                    $(thisSLChuaK).focus().select();
                }
            }
            if (code === 35) {
                if (self.currentPageChuaK() === 0) {
                    ttslChuaK = self.SLChuaKiems().length >= 10 ? 9 : self.SLChuaKiems().length - 1;
                    thisSLChuaK = "#slthucchuakiem_" + self.SLChuaKiems()[ttslChuaK].ID_Random + self.SLChuaKiems()[ttslChuaK].ID_DonViQuiDoi;
                    $(thisSLChuaK).focus().select();
                }
                else {
                    ttslChuaK = self.toitemChuaK() - 1;
                    thisSLChuaK = "#slthucchuakiem_" + self.SLChuaKiems()[ttslChuaK].ID_Random + self.SLChuaKiems()[ttslChuaK].ID_DonViQuiDoi;
                    $(thisSLChuaK).focus().select();
                }
            }
            if (code === 13 && ttslChuaK >= 0 && $(thisSLChuaK).is(":focus")) {
                ttslChuaK = ttslChuaK + 1 - self.currentPageChuaK() * 10;
                if (self.SLChuaKiems().length >= 10) {
                    if (self.currentPageChuaK() === 0) {
                        if (ttslChuaK < 10) {
                            thisSLChuaK = "#slthucchuakiem_" + self.SLChuaKiems()[ttslChuaK].ID_Random + self.SLChuaKiems()[ttslChuaK].ID_DonViQuiDoi;
                            $(thisSLChuaK).focus().select();
                        }
                        else {
                            ttslChuaK = 0;
                            thisSLChuaK = "#slthucchuakiem_" + self.SLChuaKiems()[ttslChuaK].ID_Random + self.SLChuaKiems()[ttslChuaK].ID_DonViQuiDoi;
                            $(thisSLChuaK).focus().select();
                        }
                    }
                    else {
                        ttslChuaK = self.currentPageChuaK() * 10 + ttslChuaK;
                        if (ttslChuaK < self.toitemChuaK()) {
                            thisSLChuaK = "#slthucchuakiem_" + self.SLChuaKiems()[ttslChuaK].ID_Random + self.SLChuaKiems()[ttslChuaK].ID_DonViQuiDoi;
                            $(thisSLChuaK).focus().select();
                        }
                        else {
                            ttslChuaK = self.currentPageChuaK() * 10;
                            thisSLChuaK = "#slthucchuakiem_" + self.SLChuaKiems()[self.currentPageChuaK() * 10].ID_Random + self.SLChuaKiems()[self.currentPageChuaK() * 10].ID_DonViQuiDoi;
                            $(thisSLChuaK).focus().select();
                        }
                    }
                }
                if (self.SLChuaKiems().length < 10) {
                    if (ttslChuaK < self.SLChuaKiems().length) {
                        thisSLChuaK = "#slthucchuakiem_" + self.SLChuaKiems()[ttslChuaK].ID_Random + self.SLChuaKiems()[ttslChuaK].ID_DonViQuiDoi;
                        $(thisSLChuaK).focus().select();
                    }
                    else {
                        ttslChuaK = 0;
                        thisSLChuaK = "#slthucchuakiem_" + self.SLChuaKiems()[ttslChuaK].ID_Random + self.SLChuaKiems()[ttslChuaK].ID_DonViQuiDoi;
                        $(thisSLChuaK).focus().select();
                    }
                }
            }
            if (code === 16 && ttslChuaK >= 0 && $(thisSLChuaK).is(":focus")) {
                ttslChuaK = ttslChuaK - 1 - self.currentPageChuaK() * 10;
                if (self.currentPageChuaK() === 0) {
                    if (ttslChuaK >= 0) {
                        thisSLChuaK = "#slthucchuakiem_" + self.SLChuaKiems()[ttslChuaK].ID_Random + self.SLChuaKiems()[ttslChuaK].ID_DonViQuiDoi;
                        $(thisSLChuaK).focus().select();
                    }
                    else {
                        ttslChuaK = self.SLChuaKiems().length >= 10 ? 9 : self.SLChuaKiems().length - 1;
                        thisSLChuaK = "#slthucchuakiem_" + self.SLChuaKiems()[ttslChuaK].ID_Random + self.SLChuaKiems()[ttslChuaK].ID_DonViQuiDoi;
                        $(thisSLChuaK).focus().select();
                    }
                }
                else {
                    ttslChuaK = self.currentPageChuaK() * 10 + ttslChuaK;
                    if (ttslChuaK >= self.currentPageChuaK() * 10) {
                        thisSLChuaK = "#slthucchuakiem_" + self.SLChuaKiems()[ttslChuaK].ID_Random + self.SLChuaKiems()[ttslChuaK].ID_DonViQuiDoi;
                        $(thisSLChuaK).focus().select();
                    }
                    else {
                        ttslChuaK = self.toitemChuaK() - 1;
                        thisSLChuaK = "#slthucchuakiem_" + self.SLChuaKiems()[ttslChuaK].ID_Random + self.SLChuaKiems()[ttslChuaK].ID_DonViQuiDoi;
                        $(thisSLChuaK).focus().select();
                    }
                }
            }
        }
    })

    self.numbersPrintHD = ko.observable(1);
    self.TienKhachDaTra = ko.observable();
    self.downUpvaluesPrintPG = function () {
        var objsoluong = formatNumberObj($("#txtNumberOfPrint"))
        var soluong = formatNumberToFloat(objsoluong); // lấy ra số lượng hàng xuất hủy
        var keyCode = event.keyCode || event.which;
        // press up
        if (keyCode === 38 || soluong == 0) {
            soluong = soluong + 1;
            $("#txtNumberOfPrint").val(formatNumber3Digit(soluong));
        }
        // press down
        if (keyCode === 40) {
            if (soluong > 1) {
                soluong = soluong - 1;
                $("#txtNumberOfPrint").val(formatNumber3Digit(soluong));
            }
        }
    }

    self.showPrint = function () {
        $(".install-notifi").toggle();
        $(".install-notifi").mouseup(function () {
            return false
        });
        $(".import-fast").mouseup(function () {
            return false;
        });
        $(document).mouseup(function () {
            $(".install-notifi").hide();
        });
        var CheckInHD = localStorage.getItem('InHoaDonKhiHT');
        if (CheckInHD === "true") {
            $('#divSetPrintPay .main-show').addClass("main-hide");
        }
        else {
            $('#divSetPrintPay .main-show').removeClass("main-hide");
        }
    }

    self.CongTy = ko.observableArray();
    function GetInforCongTy() {
        ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CongTy', 'GET').done(function (data) {
            if (data != null) {
                self.CongTy(data);
            }
        });
    }

    GetInforCongTy();

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

    function GetCTHDPrint_Format(arrCTHD) {
        for (var i = 0; i < arrCTHD.length; i++) {
            arrCTHD[i].MaHangHoa = arrCTHD[i].MaHangHoa;
            arrCTHD[i].TenHangHoa = arrCTHD[i].TenHangHoa.split('(')[0] + (arrCTHD[i].TenDonViTinh !== "" && arrCTHD[i].TenDonViTinh !== null ? "(" + arrCTHD[i].TenDonViTinh + ")" : "") + (arrCTHD[i].ThuocTinh_GiaTri !== null ? arrCTHD[i].ThuocTinh_GiaTri : "") + (arrCTHD[i].MaLoHang !== "" && arrCTHD[i].MaLoHang !== null ? "(Lô: " + arrCTHD[i].MaLoHang + ")" : "");
            arrCTHD[i].TonKho = formatNumber3Digit(arrCTHD[i].TienChietKhau);
            arrCTHD[i].KThucTe = formatNumber3Digit(arrCTHD[i].ThanhTien);
            arrCTHD[i].SLLech = formatNumber3Digit(arrCTHD[i].SoLuong);
            arrCTHD[i].GiaTriLech = formatNumber3Digit(arrCTHD[i].ThanhToan, 2);
        }
        return arrCTHD;
    }

    function GetInforHDPrint(objHD) {
        console.log('obj ', objHD)
        var hd = $.extend({}, objHD);
        hd.NguoiTaoHD = hd.NguoiTao;
        hd.NguoiCanBang = hd.TenNhanVien;
        hd.NgayLapHoaDon = moment(hd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss');;
        hd.NgayTao = hd.NgayLapHoaDon;
        hd.NgayCanBang = hd.NgayLapHoaDon;
        hd.TongLechTang = formatNumber3Digit(hd.TongChiPhi, 2);
        hd.TongLechGiam = formatNumber3Digit(hd.TongTienHang, 2);
        hd.TongChenhLech = formatNumber3Digit(hd.TongGiamGia, 2);
        hd.GhiChu = hd.DienGiai;
        hd.TrangThaiKK = "Đã cân bằng";

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
    //in hóa đơn kiểm kho
    self.CTHoaDonPrint = ko.observableArray();
    self.InforHDprintf = ko.observableArray();
    self.dataIframe = ko.observable();
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
                    + "; var item4=[], item5= []; var item2=" + JSON.stringify(self.CTHoaDonPrint())
                    + "; var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(result, data, self.numbersPrintHD());
            }
        });
    }

    // ========== Chiet Khau Nhan Vien =======

    self.NhanViens_ChiNhanh = ko.observableArray();
    self.searchNVien_ChietKhau = ko.observable(); // search Nvien at popup
    self.searchNVien_ChietKhau_HangHoa = ko.observable(); // search Nvien at list detail HangHoa
    self.IDQuiDoi_Chosing = ko.observable();
    self.ItemHangHoa_Chosing = ko.observableArray();
    self.ListChietKhauNV_byIDQuiDoi = ko.observableArray();
    self.ListNVien_notHaveCKhau = ko.observableArray();
    self.CKYeuCau_ApplyGiaBan = ko.observableArray();

    function GetChietKhauNV_byIDQuiDoi(idQuiDoi) {
        ajaxHelper(NSNhanVienUri + 'GetChietKhauNV_byIDQuiDoi?idQuiDoi=' + idQuiDoi + '&idChiNhanh=' + _IDchinhanh, 'GET').done(function (data) {
            if (data !== null) {
                self.ListChietKhauNV_byIDQuiDoi(data);
            }
        })
    }

    self.ClickTab_HoaHongNVien = function (item) {
        GetChietKhauNV_byIDQuiDoi(item.ID_DonViQuiDoi);
    }

    self.ShowPop_ChoseNhanVien = function (item) {
        self.IDQuiDoi_Chosing(item.ID_DonViQuiDoi);
        self.ItemHangHoa_Chosing(item);

        // get list ID_NhanVien have ChietKhau
        var arrIDNhanVien = [];
        for (var i = 0; i < self.ListChietKhauNV_byIDQuiDoi().length; i++) {
            arrIDNhanVien.push(self.ListChietKhauNV_byIDQuiDoi()[i].ID_NhanVien);
        }
        // return list NVien not have ChietKhau
        var lstNV_notHaveCK = $.grep(self.NhanViens_ChiNhanh(), function (x) {
            return $.inArray(x.ID, arrIDNhanVien) === -1;
        });
        self.ListNVien_notHaveCKhau(lstNV_notHaveCK);

        $('#txtSearchNV_CK').val('');
        // reset checked = false for all check
        $('#divNhanVien_CK input[type=checkbox]').prop('checked', false);
        $('#modalChonNhanVien').modal("show");
    }

    self.ListNhanVienCK_Filter = ko.computed(function (item) {

        var filter = self.searchNVien_ChietKhau();

        var arrFilter = ko.utils.arrayFilter(self.ListNVien_notHaveCKhau(), function (item) {

            var chon = true;
            var locdauInput = locdau(filter);
            var sCode = item.MaNhanVien.toLowerCase();
            var sName = locdau(item.TenNhanVien);
            var startChar = GetChartStart(sName);
            var phone = item.SoDienThoai === null ? '' : item.SoDienThoai;

            if (chon && filter) {
                chon = sCode.indexOf(locdauInput) > -1 || sName.indexOf(locdauInput) > -1
                    || phone.indexOf(locdauInput) > -1 || startChar.indexOf(locdauInput) > -1;
            }
            return chon;
        });
        return arrFilter;
    })

    self.totalNhanVienDiv2 = ko.computed(function () {
        return Math.ceil(self.ListNhanVienCK_Filter().length / 2);
    })

    self.AddNhanVien_HangHoa = function () {
        var arrIDNhanVien = [];
        $('#divNhanVien_CK input').each(function () {
            if ($(this).is(':checked')) {
                arrIDNhanVien.push($(this).attr('id'));
            }
        })

        var lstChietKhau_NhanVien = [];
        for (var i = 0; i < arrIDNhanVien.length; i++) {
            var objChietKhau = {
                ID_NhanVien: arrIDNhanVien[i],
                ID_DonVi: _IDchinhanh,
                ID_DonViQuiDoi: self.IDQuiDoi_Chosing(),
                ChietKhau: 0,
                LaPhanTram: true,
                ChietKhau_YeuCau: 0,
                LaPhanTram_YeuCau: false,
                ChietKhau_TuVan: 0,
                LaPhanTram_TuVan: false,
                ChietKhau_BanGoi: 0,
                LaPhanTram_BanGoi: false,
                TheoPhanTram_ThucHien: 0,
                NgayNhap: new Date(),
            }
            lstChietKhau_NhanVien.push(objChietKhau);
        }

        var myData = {
            lstChietKhau: lstChietKhau_NhanVien,
        }

        ajaxHelper(NSNhanVienUri + "Post_ChietKhauNhanVien", 'POST', myData).done(function (data) {
            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>'
                + 'Thêm mới chiết khấu nhân viên thành công', 'success');

            GetChietKhauNV_byIDQuiDoi(self.IDQuiDoi_Chosing());
        })

        // get lst name, code NhanVien
        var lstNhanVien = '';
        for (var i = 0; i < arrIDNhanVien.length; i++) {
            var itemNV = $.grep(self.NhanViens_ChiNhanh(), function (x) {
                return x.ID === arrIDNhanVien[i];
            });

            if (itemNV.length > 0) {
                lstNhanVien += itemNV[0].TenNhanVien + ' (' + itemNV[0].MaNhanVien + '), ';
            }
        }

        // Insert NhatKyThaoTac
        var style1 = '<a style= \"cursor: pointer\" onclick = \"';
        var style2 = "('";
        var style3 = "')\" >";
        var style4 = '</a>';

        var maHangHoa = self.ItemHangHoa_Chosing().MaHangHoa;
        var noiDung = 'Hàng hóa '.concat(maHangHoa, ': Thêm mới chiết khấu mặc định cho nhân viên ');
        var noiDungChiTiet = 'Hàng hóa '.concat(style1, 'FindMaHangHoa', style2, maHangHoa, style3, maHangHoa, style4);
        noiDungChiTiet = noiDungChiTiet.concat(': Thêm mới chiết khấu mặc định cho nhân viên. <br /> Gồm các nhân viên: ', Remove_LastComma(lstNhanVien), '<br /> Giá trị chiết khấu: 0');
        noiDungChiTiet = noiDungChiTiet.concat('<br /> Nhân viên thực hiện: ', _txtTenTaiKhoan);

        var objNhatKy = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Hàng hóa - Chiết khấu nhân viên",
            LoaiNhatKy: 1,
            NoiDung: noiDung,
            NoiDungChiTiet: noiDungChiTiet,
        };

        Insert_NhatKyThaoTac_1Param(objNhatKy);

        $('#modalChonNhanVien').modal("hide");
    }

    self.ListNhanVienCK_Filter_atHangHoa = ko.computed(function (item) {

        var filter = self.searchNVien_ChietKhau_HangHoa();

        var arrFilter = ko.utils.arrayFilter(self.ListChietKhauNV_byIDQuiDoi(), function (item) {

            var chon = true;
            var locdauInput = locdau(filter);
            var sCode = item.MaNhanVien.toLowerCase();
            var sName = locdau(item.TenNhanVien);
            var startChar = GetChartStart(sName);
            var phone = item.DienThoaiDiDong === null ? '' : item.DienThoaiDiDong;

            if (chon && filter) {
                chon = sCode.indexOf(locdauInput) > -1 || sName.indexOf(locdauInput) > -1
                    || phone.indexOf(locdauInput) > -1 || startChar.indexOf(locdauInput) > -1;
            }
            return chon;
        });

        var lenData = arrFilter.length;
        self.TotalRecordListCK(lenData);
        self.PageCountListCK(Math.ceil(lenData / self.pageSizeListCK()));

        arrFilter = arrFilter.sort(function (a, b) {
            var x = a.MaNhanVien, y = b.MaNhanVien;
            return x < y ? -1 : x > y ? 1 : 0;
        })
        return arrFilter;
    })

    self.CountNVien_ChietKhau = ko.computed(function () {
        return self.ListNhanVienCK_Filter_atHangHoa().length;
    })

    self.ShowChietKhau_NhanVien = function (item, typeCK) {
        var thisObj = event.currentTarget;
        var inputCK = $(thisObj).next().find('input[type="text"]');

        switch (typeCK) {
            case 1:// thuchien
                // %
                if (item.LaPhanTram) {
                    $(inputCK).val(item.ChietKhau);
                }
                else {
                    // vnd
                    $(inputCK).val(formatNumber3Digit(item.ChietKhau));
                }
                break;
            case 2:// tu van
                if (item.LaPhanTram_TuVan) {
                    $(inputCK).val(item.ChietKhau_TuVan);
                }
                else {
                    $(inputCK).val(formatNumber3Digit(item.ChietKhau_TuVan));
                }
                break;
            case 3: // ban goi
                if (item.LaPhanTram_BanGoi) {
                    $(inputCK).val(item.ChietKhau_BanGoi);
                }
                else {
                    $(inputCK).val(formatNumber3Digit(item.ChietKhau_BanGoi));
                }
                break;
            case 4: // theoyeucau
                var theoGB = item.TheoChietKhau_ThucHien == 1 ? false : true;// if AppyThucHien = 1 --> Apply GiaBan = false
                if (item.LaPhanTram_YeuCau) {
                    $(inputCK).val(item.ChietKhau_YeuCau);
                }
                else {
                    $(inputCK).val(formatNumber3Digit(item.ChietKhau_YeuCau));
                }
                self.CKYeuCau_ApplyGiaBan(theoGB);
                var $this = event.currentTarget;
                var lblYC = $($this).closest('td').find('.lblYeuCau');
                if (theoGB) {
                    $(lblYC).text(' Hoa hồng thực hiện theo yêu cầu = ');
                }
                else {
                    $(lblYC).text(' Hoa hồng thực hiện theo yêu cầu =  Hoa hồng thực hiện +');
                }
                break;
        }

        $(function () {
            $(inputCK).select();
        })
    }

    self.EditChietKhau_NhanVien = function (item) {
        var thisObj = event.currentTarget;
        var thisNext = $(thisObj).next();

        if ($(thisNext).hasClass('gb')) {
            // vnd
            formatNumberObj(thisObj);
        }
        else {
            // %
            if ($(thisObj).val() > 100) {
                $(thisObj).val(100);
            }
        }
    }

    self.DeleteNhanVien_fromListCK = function (item, itemHH) {

        var idDelete = item.ID;
        var idQuiDoi = item.ID_DonViQuiDoi;

        dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn chiết khấu mặc định của nhân viên <b> ' + item.MaNhanVien + '</b> không?', function () {
            $.ajax({
                type: "GET",
                url: NSNhanVienUri + "deleteChiTiet/" + idDelete,
                success: function (result) {
                    if (result === true) {
                        bottomrightnotify("Xóa chiết khấu mặc định của nhân viên thành công");

                        var lstAfterDelete = $.grep(self.ListChietKhauNV_byIDQuiDoi(), function (x) {
                            return x.ID !== idDelete;
                        });

                        self.ListChietKhauNV_byIDQuiDoi(lstAfterDelete);

                        var strNoiDung = 'Hàng hóa ' + itemHH.MaHangHoa + ': Xóa chiết khấu mặc định của nhân viên ';
                        var objNhatKy = {
                            ID_NhanVien: _IDNhanVien,
                            ID_DonVi: _IDchinhanh,
                            ChucNang: "Hàng hóa - Chiết khấu nhân viên",
                            LoaiNhatKy: 3,
                            NoiDung: strNoiDung,
                            NoiDungChiTiet: strNoiDung + item.TenNhanVien + ' (' + item.MaNhanVien + ')',
                        };

                        Insert_NhatKyThaoTac_1Param(objNhatKy);
                    }
                    else {
                        bottomrightnotify("Xóa chiết khấu mặc định của nhân viên thất bại!");
                    }
                }
            })
        })
    }

    self.ClickChietKhau_VND = function (item, dataHH) {
        var thisObj = event.currentTarget;
        $(thisObj).next().removeClass('gb');
        $(thisObj).addClass('gb');

        var objCK = $(thisObj).prev('input');
        var valInput = parseFloat($(objCK).val());

        var tienGiam = valInput * dataHH.GiaBan / 100;
        objCK.val(formatNumber3Digit(tienGiam));
    }

    self.ClickChietKhau_Ptram = function (item, dataHH) {
        var thisObj = event.currentTarget;
        $(thisObj).prev().removeClass('gb');
        $(thisObj).addClass('gb');

        var objCK = $(thisObj).parent().children('input');
        var valInput = formatNumberToInt($(objCK).val());

        var ptGiam = (valInput / dataHH.GiaBan) * 100;
        objCK.val(ptGiam);
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

    self.AgreeApply_ChietKhau = function (dataHH, item, typeCK) {
        var idUpdate = item.ID;
        var idQuiDoi = dataHH.ID_DonViQuiDoi;

        var thisObj = event.currentTarget;
        var objInput = $(thisObj).parent().find('._js input');
        var valCK = 0;

        // find checkbox apply
        var elmCheckBox = $(thisObj).prev().find('input');
        // find div contain % OR vnd
        var thisParent = $(thisObj).parent().find('._js');
        var isPtram = $(thisParent).find('div:last-child').hasClass('gb');

        var txtGtriCK = '<br /> Giá trị chiết khấu: ';// use diary
        var lstNhanVien = '';
        var txtTH_TV = '';
        var valPTram = '0';
        // get value ChietKhau from input
        if (isPtram) {
            valCK = parseFloat(objInput.val());
            valPTram = '1';
            txtGtriCK = txtGtriCK + valCK + ' %';
        }
        else {
            valCK = formatNumberToInt(objInput.val())
            txtGtriCK = txtGtriCK + formatNumber3Digit(valCK) + ' đ';
        }

        var sWhere = '';
        let applyGiaBan = 1;
        if (self.CKYeuCau_ApplyGiaBan() === true) applyGiaBan = 0;

        if (elmCheckBox.is(':checked')) {
            switch (typeCK) {
                case 1:
                    txtTH_TV = 'thực hiện';
                    sWhere = " ChietKhau = '" + valCK + "' , LaPhanTram = " + valPTram
                        + " WHERE ID_DonViQuiDoi= '" + idQuiDoi + "'";
                    break;
                case 2:
                    txtTH_TV = 'tư vấn';
                    sWhere = " ChietKhau_TuVan = '" + valCK + "' , LaPhanTram_TuVan = " + valPTram
                        + " WHERE ID_DonViQuiDoi= '" + idQuiDoi + "'";
                    break;
                case 3:
                    txtTH_TV = 'bán gói';
                    sWhere = " ChietKhau_BanGoi = '" + valCK + "' , LaPhanTram_BanGoi = " + valPTram
                        + " WHERE ID_DonViQuiDoi= '" + idQuiDoi + "'";
                    break;
                case 4:
                    txtTH_TV = 'thực hiện theo yêu cầu';
                    sWhere = " ChietKhau_YeuCau = '" + valCK + "' , LaPhanTram_YeuCau = " + valPTram + ' , TheoChietKhau_ThucHien= ' + applyGiaBan
                        + " WHERE ID_DonViQuiDoi= '" + idQuiDoi + "'";
                    break;
            }

            for (var i = 0; i < self.PageResult_ChietKhauNV().length; i++) {
                lstNhanVien += self.PageResult_ChietKhauNV()[i].TenNhanVien + ' (' + self.PageResult_ChietKhauNV()[i].MaNhanVien + '), ';
            }
        }
        else {
            // apply 1 nvien
            switch (typeCK) {
                case 1:
                    txtTH_TV = 'thực hiện';
                    sWhere = " ChietKhau = '" + valCK + "' , LaPhanTram = " + valPTram
                        + " WHERE ID= '" + idUpdate + "'";
                    break;
                case 2:
                    txtTH_TV = 'tư vấn';
                    sWhere = " ChietKhau_TuVan = '" + valCK + "' , LaPhanTram_TuVan = " + valPTram
                        + " WHERE ID= '" + idUpdate + "'";
                    break;
                case 3:
                    txtTH_TV = 'bán gói';
                    sWhere = " ChietKhau_BanGoi = '" + valCK + "' , LaPhanTram_BanGoi = " + valPTram
                        + " WHERE ID= '" + idUpdate + "'";
                    break;
                case 4:
                    txtTH_TV = 'thực hiện theo yêu cầu';
                    sWhere = " ChietKhau_YeuCau = '" + valCK + "' , LaPhanTram_YeuCau = " + valPTram + ' , TheoChietKhau_ThucHien= ' + applyGiaBan
                        + " WHERE ID= '" + idUpdate + "'";
                    break;
            }
            lstNhanVien = item.TenNhanVien + ' (' + item.MaNhanVien + ')';
        }

        ajaxHelper(NSNhanVienUri + "Update_ChietKhauNhanVien?stringSQL=" + sWhere, 'POST').done(function (data) {
            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>'
                + 'Cập nhật chiết khấu nhân viên thành công', 'success');

            GetChietKhauNV_byIDQuiDoi(idQuiDoi);
        })

        // Insert NhatKyThaoTac
        var style1 = '<a style= \"cursor: pointer\" onclick = \"';
        var style2 = "('";
        var style3 = "')\" >";
        var style4 = '</a>';

        var noiDung = 'Hàng hóa '.concat(dataHH.MaHangHoa, ': Cập nhật chiết khấu mặc định cho nhân viên ', txtTH_TV);
        var noiDungChiTiet = 'Hàng hóa '.concat(style1, 'FindMaHangHoa', style2, dataHH.MaHangHoa, style3, dataHH.MaHangHoa, style4);
        noiDungChiTiet = noiDungChiTiet.concat(': Cập nhật chiết khấu mặc định cho nhân viên ', txtTH_TV);
        noiDungChiTiet = noiDungChiTiet.concat('<br /> Gồm các nhân viên: ', Remove_LastComma(lstNhanVien), txtGtriCK);
        noiDungChiTiet = noiDungChiTiet.concat('<br /> Nhân viên thực hiện: ', _txtTenTaiKhoan);

        var objNhatKy = {
            ID_NhanVien: _IDNhanVien,
            ID_DonVi: _IDchinhanh,
            ChucNang: "Hàng hóa - Chiết khấu nhân viên",
            LoaiNhatKy: 2,
            NoiDung: noiDung,
            NoiDungChiTiet: noiDungChiTiet,
        };

        Insert_NhatKyThaoTac_1Param(objNhatKy)

        $(thisObj).closest('.callprice').hide();
    }

    // paging list CK NhanVien
    self.PageResult_ChietKhauNV = ko.computed(function (x) {
        var first = self.currentPageListCK() * self.pageSizeListCK();
        if (self.ListNhanVienCK_Filter_atHangHoa() !== null) {
            return self.ListNhanVienCK_Filter_atHangHoa().slice(first, first + self.pageSizeListCK());
        }
        return null;
    })

    self.PageListChietKhauNV = ko.computed(function () {
        var arrPage = [];

        var allPage = self.PageCountListCK();
        var currentPageListCK = self.currentPageListCK();

        if (allPage > 4) {

            var i = 0;
            if (currentPageListCK === 0) {
                i = parseInt(self.currentPageListCK()) + 1;
            }
            else {
                i = self.currentPageListCK();
            }

            if (allPage >= 5 && currentPageListCK > allPage - 5) {
                if (currentPageListCK >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumber: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    if (currentPageListCK == 1) {
                        for (var j = currentPageListCK - 1; (j <= currentPageListCK + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                    else {
                        // get currentPageListCK - 2 , currentPageListCK, currentPageListCK + 2
                        for (var j = currentPageListCK - 2; (j <= currentPageListCK + 2) && j < allPage; j++) {
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

        if (self.PageResult_ChietKhauNV() !== null) {

            self.fromitemListCK((self.currentPageListCK() * self.pageSizeListCK()) + 1);

            if (((self.currentPageListCK() + 1) * self.pageSizeListCK()) > self.PageResult_ChietKhauNV().length) {
                var fromItem = (self.currentPageListCK() + 1) * self.pageSizeListCK();
                if (fromItem < self.TotalRecordListCK()) {
                    self.toitemListCK((self.currentPageListCK() + 1) * self.pageSizeListCK());
                }
                else {
                    self.toitemListCK(self.TotalRecordListCK());
                }
            } else {
                self.toitemListCK((self.currentPageListCK() * self.pageSizeListCK()) + self.pageSizeListCK());
            }
        }
        return arrPage;
    });

    self.VisibleStartPageListCK = ko.computed(function () {
        if (self.PageListChietKhauNV().length > 0) {
            return self.PageListChietKhauNV()[0].pageNumber !== 1;
        }
    });

    self.VisibleEndPageListCK = ko.computed(function () {
        if (self.PageListChietKhauNV().length > 0) {
            return self.PageListChietKhauNV()[self.PageListChietKhauNV().length - 1].pageNumber !== self.PageCountListCK();
        }
    })

    self.GoToPageListCK = function (page) {
        self.currentPageListCK(page.pageNumber - 1);
    };

    self.GetClassListCK = function (page) {
        return ((page.pageNumber - 1) === self.currentPageListCK()) ? "click" : "";
    };

    self.StartPageListCK = function () {
        self.currentPageListCK(0);
    }

    self.BackPageListCK = function () {
        if (self.currentPageListCK() > 1) {
            self.currentPageListCK(self.currentPageListCK() - 1);
        }
    }

    self.GoToNextPageListCK = function () {
        if (self.currentPageListCK() < self.PageCountListCK() - 1) {
            self.currentPageListCK(self.currentPageListCK() + 1);
        }
    }

    self.EndPageListCK = function () {
        if (self.currentPageListCK() < self.PageCountListCK() - 1) {
            self.currentPageListCK(self.PageCountListCK() - 1);
        }
    }

    function PageLoad() {
        getallThietLap();
        getallThuocTinh();
        getallViTri();
        getQuyen_NguoiDung();
        loadQuyenIndex();
        loadMauIn();
        getListNhanVien();
        GetListNhomHang_SetupHoTro();
    }
    PageLoad();

    // caidat hoahong hanghoa
    self.TinhHoaHongTruocCK = ko.observable(false);

    // baoduong
    self.TPDL_SumTienVon = ko.observable(0);
    self.Tab_Active = ko.observable(0);
    self.QuanLyBaoDuong = ko.observable(false);
    self.BaoDuong_Type = ko.observable(2);
    self.BaoDuong_Repeater = ko.observable(false);
    self.BaoDuong_FromDate = ko.observable(false);
    self.BaoDuong_ApplyAllByGroup = ko.observable(false);
    self.BaoDuong_ListDetail = ko.observableArray();
    self.BaoDuong_ListType = ko.observableArray([
        { ID: 1, Text: 'Thời gian' },
        { ID: 2, Text: 'Km' },
    ]);
    self.BaoDuong_TypeDate = ko.observableArray([
        { ID: 1, Text: 'Ngày' },
        { ID: 2, Text: 'Tháng' },
        { ID: 3, Text: 'Năm' },
        { ID: 5, Text: 'Giờ' },
        { ID: 4, Text: 'Km' },
    ]);

    self.BaoDuong_TypeDate_Filter = ko.computed(function () {
        if (parseInt(self.BaoDuong_Type()) === 2) {
            return self.BaoDuong_TypeDate().filter(x => x.ID === 4);
        }
        return self.BaoDuong_TypeDate().filter(x => x.ID < 4 || x.ID === 5);
    })

    self.BaoDuong_VisibleColumnRepeater = ko.observable(false);

    function GetChiTietBaoDuong_TheoHangHoa() {
        var idHangHoa = self.newHangHoa().ID();
        if (commonStatisJs.CheckNull(idHangHoa)) {
            return;
        }
        var obj = {
            ID: const_GuidEmpty,
            LanBaoDuong: 1,
            GiaTri: 0,
            LoaiThoiGian: 4,
            LapDinhKy: false,
        }
        ajaxHelper(DMHangHoaUri + 'GetChiTietBaoDuong_TheoHangHoa?idHangHoa=' + idHangHoa, 'GET')
            .done(function (x) {
                if (x.dataSoure.length === 0) {
                    self.BaoDuong_ListDetail([]);
                    self.BaoDuong_ListDetail.push(obj);
                }
                else {
                    self.BaoDuong_ListDetail(x.dataSoure);
                }
                var repeater = self.BaoDuong_ListDetail().filter(x => x.LapDinhKy).length > 0;
                self.BaoDuong_VisibleColumnRepeater(repeater);
            })
    }

    self.ChangeTab_HangHoa = function (val) {
        self.Tab_Active(parseInt(val));
        if (self.Tab_Active() === 5) {
            self.LoadTabBaoDuong();
        }

        var $this = $(event.currentTarget);
        var hrefThis = $this.find('a').attr('href');
        $this.closest('ul').children('li').each(function () {
            let href = $(this).children('a').attr('href');
            if (hrefThis !== href) {
                $('' + href).removeClass('active');
                $(this).removeClass('active');
            }
        });
    }

    self.LoadTabBaoDuong = function () {
        GetChiTietBaoDuong_TheoHangHoa();
    }

    self.BaoDuong_TypeText = ko.computed(function () {
        var loai = parseInt(self.BaoDuong_Type());
        if (loai === 0) {
            return '';
        }
        return self.BaoDuong_ListType().find(x => x.ID == loai).Text;
    });

    self.BaoDuong_ChangeType = function (item) {
        self.BaoDuong_Type(item.ID)
    }
    self.BaoDuong_ChangeTypeDate = function (item) {
        if (parseInt(item.LoaiThoiGian) < 4) {
            self.BaoDuong_Type(1);
        }
    }

    self.BaoDuong_EditGiaTri = function (index) {
        var $this = $(event.currentTarget);
        formatNumberObj($this);

        for (let i = 0; i < self.BaoDuong_ListDetail().length; i++) {
            let itFor = self.BaoDuong_ListDetail()[i];
            if (i === index) {
                itFor.GiaTri = formatNumberToFloat($this.val());
                break;
            }
        }
    }

    self.ChangeCheckRepeater = function (index) {
        for (let i = 0; i < self.BaoDuong_ListDetail().length; i++) {
            let itFor = self.BaoDuong_ListDetail()[i];
            if (i === index) {
                itFor.LapDinhKy = item.LapDinhKy;
                break;
            }
        }
        var arr = self.BaoDuong_ListDetail();
        self.BaoDuong_ListDetail($.extend([], true, arr));
    }

    self.addLanBaoDuong = function () {
        if (self.BaoDuong_Repeater()) {
            return;
        }
        var obj = {
            ID: const_GuidEmpty,
            LanBaoDuong: self.BaoDuong_ListDetail().length + 1,
            GiaTri: 0,
            LoaiGiaTri: 4,
            LapDinhKy: false,
        }
        self.BaoDuong_ListDetail.push(obj);
    }

    self.removeLanBaoDuong = function (index) {
        if (self.BaoDuong_ListDetail().length === 1) {
            return;
        }
        if (self.BaoDuong_Repeater()) {
            return;
        }
        self.BaoDuong_ListDetail.splice(index, 1);
    }

    self.JqAutoSelectItem = function (item) {

        console.log('item ', item)
        item.SoLuong = 1;
        item.SoLuongQuyCach = item.QuyCach;
        item.ThanhTien = item.GiaVon;
        item.DonGia = item.GiaBan;

        var arr = self.newHangHoa().DinhLuongDichVu();
        var countdem = 0;
        if (arr.length > 0) {
            for (var i = 0; i < arr.length; i++) {
                if (item.MaHangHoa === arr[i].MaHangHoa || item.MaHangHoa === self.newHangHoa().MaHangHoa()) {
                    countdem++;
                }
            }
            if (countdem === 0) {
                self.newHangHoa().DinhLuongDichVu.push(item);
            }
            else {
                ShowMessage_Danger("Thành phần định lượng đã được chọn");
                return false;
            }
        } else {
            if (item.MaHangHoa !== self.newHangHoa().MaHangHoa()) {
                self.newHangHoa().DinhLuongDichVu.push(item);
            } else {
                ShowMessage_Danger("Thành phần định lượng đã được chọn");
                return false;
            }
        }
        self.TinhLaiTienDV();
    }

    self.TPDLuong_GiaTriSo = ko.observable();
    self.TPDLuong_GiaTriPtram = ko.observable(true);

    self.TPDLuong_showDivGiaBan = function () {
        var $this = $(event.currentTarget);
        $this.next().show();
        $this.next().find('input').focus().select();
        self.TPDLuong_GiaTriSo('');
        self.TPDLuong_GiaTriPtram(true);
    }
    self.TPDLuong_clickPtramVND = function (val) {
        self.TPDLuong_GiaTriPtram(val);
        $('.callprice').mouseup(function () {
            return false;
        });
    }
    self.TPDLuong_editGiaTriSo = function () {
        var $this = $(event.currentTarget);
        if (!self.TPDLuong_GiaTriPtram()) {
            formatNumberObj($this);
        }
        else {
            let gtri = formatNumberToFloat($this.val());
            if (gtri > 100) {
                $this.val(100);
            }
        }
    }

    self.TPDLuong_editGiaBan = function (index) {
        var $this = $(event.currentTarget);
        formatNumberObj($this);
        var val = formatNumberToFloat($this.val());
        for (let i = 0; i < self.newHangHoa().DinhLuongDichVu().length; i++) {
            let itFor = self.newHangHoa().DinhLuongDichVu()[i];
            if (index === i) {
                itFor.DonGia = val;
                break;
            }
        }
        var keycode = event.keyCode || event.which;
        if (keycode === 13) {
            let tr = $($this).closest('tr').next();
            tr.children('td').eq(6).find('input').focus().select();
        }
    }
    console.log(2);
    self.TPDLuong_applyGiaBan = function () {
        let gtri = self.TPDLuong_GiaTriSo();
        if (commonStatisJs.CheckNull(gtri)) {
            ShowMessage_Danger('Vui lòng nhập giá trị');
            return;
        }
        gtri = formatNumberToFloat(gtri);
        for (let i = 0; i < self.newHangHoa().DinhLuongDichVu().length; i++) {
            let itFor = self.newHangHoa().DinhLuongDichVu()[i];
            itFor.DonGia = itFor.GiaBan * gtri / 100;
        }
        self.newHangHoa().DinhLuongDichVu.refresh();
        $('.callprice').hide();
    }

    self.showModalImport = function (type) {
        switch (type) {
            case 1:
                vmImportDinhLuong.showModal();
                break;
        }
    }

    $('#vmImportDinhLuong').on('hidden.bs.modal', function () {
        vmImportDinhLuong.isChosingFile = false;
        vmImportDinhLuong.ListErr = [];
    })

    function vmXe_ResetRole() {
        vmThemMoiXe.role.Xoa = false;
        vmThemMoiXe.role.KhachHang.ThemMoi = false;
        vmThemMoiXe.role.KhachHang.CapNhat = false;
    }

    // hanghoa = xe
    self.HangHoa_LaXe = ko.observable(false);
    self.ChoseCar = function (item) {
        self.newHangHoa().ID_Xe(item.ID);
        if (self.booleanAdd()) {
            self.newHangHoa().MaHangHoa(item.BienSo);
            self.newHangHoa().TenHangHoa(item.BienSo);
        }
    }
    self.ShowForm_NewCar = function () {
        vmXe_ResetRole();
        vmThemMoiXe.ShowModalNewCar();
    }

    self.ShowForm_UpdateCar = function () {
        vmXe_ResetRole();
        vmThemMoiXe.GetInforCar_byID(self.newHangHoa().ID_Xe(), 2);
    }

    $('#ThemMoiXemModal').on('hidden.bs.modal', function () {
        if (vmThemMoiXe.saveOK) {
            $('jqauto-car ._jsInput').val(vmThemMoiXe.newCar.BienSo);
            self.newHangHoa().ID_Xe(vmThemMoiXe.newCar.ID);
            self.newHangHoa().MaHangHoa(vmThemMoiXe.newCar.BienSo);
            self.newHangHoa().TenHangHoa(vmThemMoiXe.newCar.BienSo);
        }
    })

    self.gotoPage = function (type = 0, text) {
        let url = '';
        switch (type) {
            case 0:
                localStorage.setItem('loadMaHang', text);
                url = '/#/Product';
                var modalName = '';
                switch (self.newHoaDon().LoaiHoaDon()) {
                    case 1:
                    case 19:
                        modalName = '#modalpopup_PhieuBH';
                        break;
                    case 4:
                        modalName = '#modalPopuplg_NhapHang';
                        break;
                    case 6:
                        modalName = '#modalpopup_PhieuTH';
                        break;
                    case 7:
                        modalName = '#modalpopup_PhieuTHN';
                        break;
                    case 8:
                        modalName = '#modalpopup_PhieuXH';
                        break;
                    case 9:
                        modalName = '#modalpopup_KiemKho';
                        break;
                    case 10:
                        modalName = '#modalPopuplg_TheKho';
                        break;
                    case 18:
                        modalName = '#modalpopup_PhieuDieuChinh';
                        break;
                    default:
                        modalName = '#modalPopuplg_NhapHang';
                        break;
                }
                $(modalName).modal('hide');
                break;
            case 1:
                localStorage.setItem('FindKhachHang', text);
                url = '/#/Customers';
                break;
            case 2:
                localStorage.setItem('FindKhachHang', text);
                url = '/#/Suppliers';
                break;
        }
        window.open(url, '_blank');
    }


    self.textSearch = ko.observable();
    self.indexFocus = ko.observable(0);
    self.ListNVienSearch = ko.observableArray();

    self.SearchNhanVien = function () {
        var self = this;
        var txt = locdau(self.textSearch());
        var keyCode = event.keyCode;

        if ($.inArray(keyCode, [13, 38, 40]) === -1) {
            let arr = [];
            if (txt === '') {
                arr = self.NhanViens().slice(0, 20);
                self.ListNVienSearch(arr);
                return;
            }
            arr = $.grep(self.NhanViens(), function (x) {
                x.NameFull = x.MaNhanVien.concat(' ', x.TenNhanVien, ' ', x.SoDienThoai);
                return locdau(x.NameFull).indexOf(txt) > -1;
            })
            self.ListNVienSearch(arr);
        }
        else {
            switch (keyCode) {
                case 13:
                    var chosing = $.grep(self.ListNVienSearch(), function (item, index) {
                        return index === self.indexFocus;
                    });
                    if (chosing.length > 0) {
                        self.ChoseNhanVien(chosing[0]);
                    }
                    break;
                case 38:
                    if (self.indexFocus() < 0) {
                        self.indexFocus(0);
                    }
                    else {
                        self.indexFocus(self.indexFocus() - 1);
                    }
                    break;
                case 40:
                    if (self.indexFocus() > self.ListNVien_BanGoi().length) {
                        self.indexFocus(0);
                    }
                    else {
                        self.indexFocus(self.indexFocus() + 1);
                    }
                    break;
            }
        }
    }

    self.ChoseNhanVien = function (item) {
        self.textSearch(item.TenNhanVien);
        self.selectedNV(item.ID);
        self.newKiemKho().ID_NhanVien(item.ID);
    }

    self.CapNhatTonKho = function (item) {
        let itemTK = self.RowErrKho();
        if (!$.isEmptyObject(itemTK) && itemTK !== undefined) {
            //// check if PhieuKiemKe: update PhieuKiemKe, xong roi moi chay TonLuyKe
            //if (itemTK.LoaiHoaDon === 9) {
            //    ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/UpdateChiTietKiemKe_WhenEditCTHD?idHoaDonUpdate=' + itemTK.ID_HoaDon
            //        + "&idChiNhanh=" + itemTK.ID_DonVi + "&ngayLapHDMin=" + itemTK.NgayLapHoaDon).done(function (x) {
            //            console.log('UpdateChiTietKiemKe_WhenEditCTHD ', x)
            //        })
            //}

            let diary = {
                ID_DonVi: itemTK.ID_DonVi,
                ID_NhanVien: VHeader.IdNhanVien,
                LoaiNhatKy: 2,
                ChucNang: 'Cập nhật tồn lũy kế',
                NoiDung: 'Cập nhật tồn lũy kế cho hàng hóa '.concat(item.TenHangHoa, ' (', item.MaHangHoa, ')'),
                NoiDungChiTiet: 'Cập nhật tồn lũy kế '.concat(item.TenHangHoa, ' (', item.MaHangHoa,
                    ') <br /> Mã phiếu: ', itemTK.MaHoaDon,
                    ') <br /> Ngày lập phiếu: ', itemTK.NgayLapHoaDon,
                    ' <br /> Người cập nhật: ', VHeader.UserLogin
                ),
                ID_HoaDon: itemTK.ID_HoaDon,
                LoaiHoaDon: itemTK.LoaiHoaDon,
                ThoiGianUpdateGV: itemTK.NgayLapHoaDon,
            }
            Post_NhatKySuDung_UpdateGiaVon(diary);
            ShowMessage_Success('Cập nhật thành công');
            SearchHangHoa();
        }
    }
};
var FileModel = function (filef, srcf) {
    var self = this;
    this.file = filef;
    this.URLAnh = srcf;
};
var modelHangHoa = new ViewModel();
ko.applyBindings(modelHangHoa);

function jqAutoSelectItem(item) {
    modelHangHoa.JqAutoSelectItem(item);
}

function changePropertiesKH(array, compareVal, tonkho, slthuc, sllech, giatrilech, idrandom) {
    for (var i = 0; i < array.length; i++) {
        if (array[i].ID_DonViQuiDoi === compareVal && array[i].ID_Random === idrandom) {
            array[i].SoLuong = tonkho;
            array[i].ThanhTien = slthuc;
            array[i].TienChietKhau = sllech;
            array[i].ThanhToan = giatrilech;
            break;
        }
    }
    return array;
}
//checkbox
var arrIDHang = [];

function SetCheckAllHH(obj) {
    var isChecked = $(obj).is(":checked");
    $('.chacungloai input[type=checkbox]').each(function () {
        $(this).prop('checked', isChecked);
    });
    if (isChecked) {
        if (modelHangHoa.checkLoadDemCL() === true) {
            $('.check-group input[type=checkbox]').each(function () {
                var thisID = $(this).attr('id');
                if (thisID !== undefined) {
                    ajaxHelper('/api/DanhMuc/DM_HangHoaAPI/' + 'GetlistCheckBoxHH?id_dvqd=' + thisID, 'GET').done(function (data) {
                        for (var i = 0; i < data.length; i++) {
                            if (!(jQuery.inArray(data[i].ID_DonViQuiDoi, arrIDHang) > -1)) {
                                arrIDHang.push(data[i].ID_DonViQuiDoi);
                            }
                        }
                        if (arrIDHang.length === 0) {
                            $('.operation').hide();
                            $('.choose-commodity').hide();
                        }
                        $('#count').html(arrIDHang.length);
                    });
                }
                $('.operation').show();
                $('.choose-commodity').show();
            });
        }
        else {
            $('.chacungloai input[type=checkbox]').each(function () {
                var thisID = $(this).attr('id');
                if (thisID !== undefined) {
                    ajaxHelper('/api/DanhMuc/DM_HangHoaAPI/' + 'GetlistCheckBoxHHNotDemCL?id_dvqd=' + thisID, 'GET').done(function (data) {
                        for (var i = 0; i < data.length; i++) {
                            if (!(jQuery.inArray(data[i].ID_DonViQuiDoi, arrIDHang) > -1)) {
                                arrIDHang.push(data[i].ID_DonViQuiDoi);
                            }
                        }
                        if (arrIDHang.length === 0) {
                            $('.operation').hide();
                            $('.choose-commodity').hide();
                        }
                        $('#count').html(arrIDHang.length);
                    });
                }
                $('.operation').show();
                $('.choose-commodity').show();
            });
        }
    }
    else {
        $('.chacungloai input[type=checkbox]').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined) {
                ajaxHelper('/api/DanhMuc/DM_HangHoaAPI/' + 'GetlistCheckBoxHH?id_dvqd=' + thisID, 'GET').done(function (data) {
                    for (var j = 0; j < arrIDHang.length; j++) {
                        for (var i = 0; i < data.length; i++) {
                            if (arrIDHang[j] === data[i].ID_DonViQuiDoi) {
                                arrIDHang.splice(j, 1);
                            }
                        }
                    }
                    if (arrIDHang.length === 0) {
                        $('.operation').hide();
                        $('.choose-commodity').hide();
                    }
                    $('#count').html(arrIDHang.length);
                });
            }
            //if (thisID !== undefined && (jQuery.inArray(thisID, arrIDHang) > -1)) {
            //    $.map(arrIDHang, function (item, i) {
            //        if (item === thisID) {
            //            arrIDHang.splice(i, 1);
            //        }
            //    })
            //}
        })
        //arrIDHang = [];
    }
    //if (arrIDHang.length == 0) {
    //    $('.operation').hide();
    //    $('.choose-commodity').hide();
    //}
    //$('#count').html(arrIDHang.length);
}

function RemoveCheckHH() {
    $('.check-group input[type=checkbox]').each(function () {
        $(this).prop('checked', false);
    })
    arrIDHang = [];
    $('.operation').hide();
    $('.choose-commodity').hide();
    $('#count').html(arrIDHang.length);
}

function getIDHH(obj) {
    var thisID = $(obj).attr('id');
    if ($(obj).is(':checked')) {
        ajaxHelper('/api/DanhMuc/DM_HangHoaAPI/' + 'GetlistCheckBoxHH?id_dvqd=' + thisID, 'GET').done(function (data) {
            for (var i = 0; i < data.length; i++) {
                if (!(jQuery.inArray(data[i].ID_DonViQuiDoi, arrIDHang) > -1)) {
                    arrIDHang.push(data[i].ID_DonViQuiDoi);
                    if ($('#' + data[i].ID_HangHoaCungLoai).find('#' + thisID).is(':checked')) {
                        $('#cungl' + data[i].ID_HangHoaCungLoai).find('.prev-tr-hide1').find('.cmcheck input[type="checkbox"]').each(function () {
                            $(this).prop('checked', true);
                        });
                    }
                }
            }
            $('.operation').show();
            $('.choose-commodity').show();
            if (arrIDHang.length === 0) {
                $('.operation').hide();
                $('.choose-commodity').hide();
            }
            $('#count').html(arrIDHang.length);
        });
    }
    else {
        //remove item in arrID
        var arrID = [];
        ajaxHelper('/api/DanhMuc/DM_HangHoaAPI/' + 'GetlistCheckBoxHH?id_dvqd=' + thisID, 'GET').done(function (data) {
            for (i = 0; i < arrIDHang.length; i++) {
                for (var j = 0; j < data.length; j++) {
                    if (arrIDHang[i] === data[j].ID_DonViQuiDoi) {
                        arrIDHang.splice(i, 1);
                        $('#cungl' + data[j].ID_HangHoaCungLoai).find('.prev-tr-hide1').find('.cmcheck input[type="checkbox"]').each(function () {
                            $(this).prop('checked', false);
                        });
                    }
                }
            }
            if (arrIDHang.length === 0) {
                $('.operation').hide();
                $('.choose-commodity').hide();
            }
            $('#count').html(arrIDHang.length);
        });
    }
}


function getIDHHConCungLoai(obj) {
    var thisID = $(obj).attr('id');
    if ($(obj).is(':checked')) {
        ajaxHelper('/api/DanhMuc/DM_HangHoaAPI/' + 'GetlistCheckBoxKhongCoCL?id_dvqd=' + thisID, 'GET').done(function (data) {
            for (var i = 0; i < data.length; i++) {
                if (!(jQuery.inArray(data[i].ID_DonViQuiDoi, arrIDHang) > -1)) {
                    arrIDHang.push(data[i].ID_DonViQuiDoi);
                }
            }
            $('#count').html(arrIDHang.length);
            $('.operation').show();
            $('.choose-commodity').show();
        });
    }
    else {
        //remove item in arrID
        ajaxHelper('/api/DanhMuc/DM_HangHoaAPI/' + 'GetlistCheckBoxKhongCoCL?id_dvqd=' + thisID, 'GET').done(function (data) {
            for (i = 0; i < arrIDHang.length; i++) {
                for (var j = 0; j < data.length; j++) {
                    if (arrIDHang[i] === data[j].ID_DonViQuiDoi) {
                        arrIDHang.splice(i, 1);
                    }
                }
            }
            if (arrIDHang == "") {
                $('.operation').hide();
                $('.choose-commodity').hide();
            }
            $('#count').html(arrIDHang.length);
        });
        //$.map(arrIDHang, function (item, i) {
        //    if (item === thisID) {
        //        arrIDHang.splice(i, 1);
        //    }
        //})
    }
}
//format number
function formatNumberObj(obj) {
    if (obj !== null) {
        var objVal = $(obj).val();
        $(obj).val(objVal.toString().replace(/,/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        return objVal;
    }
}
function formatNumberToInt(objVal) {
    return parseInt(objVal.replace(/,/g, ''));
}

function hidewait(o) {
    $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /><div class="happy-wait">' +
        ' </div>' +
        '</div>')
}
$('input[type=text]').click(function () {
    $(this).select();
});
$('#modalPopuplg_HH, #modalPopuplg_DichVu, .model - group - goods').on('shown.bs.modal', function () {
    $('input[type=text]').click(function () {
        $(this).select();
    });
})
function keypressEnterSelected(e) {
    if (e.keyCode === 13) {
        modelHangHoa.selectSearchHH();
    }
}
function itemSelectedHHKK(item) {
    modelHangHoa.selectHHDM_LoHang(item);
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

// ==== chiet khau nhan vien =====

function CheckAll_NhanVien(obj) {
    var isCheck = $(obj).is(':checked');
    $('#divNhanVien_CK input[type="checkbox"]').prop('checked', isCheck);
}

function RemoveAllCheck_NhanVien() {
    $('#modalChonNhanVien input[type="checkbox"]').prop('checked', false);
}

function containsAll(needles, haystack) {
    for (var i = 0, len = needles.length; i < len; i++) {
        if (needles[i] === '') continue;
        if (locdau(haystack).search(new RegExp(locdau(needles[i]), "i")) < 0) return false;
    }
    return true;
}

function getNumber(e, obj) {
    var elementAfer = $(obj).next();
    if (elementAfer.hasClass('gb')) {
        // chi cho phep nhap so
        return keypressNumber(e);
    }
    else {
        var keyCode = window.event.keyCode || e.which;
        if (keyCode < 48 || keyCode > 57) {
            // cho phep nhap dau .
            if (keyCode === 8 || keyCode === 127 || keyCode === 46) {
                return;
            }
            return false;
        }
    }
}
ko.observableArray.fn.refresh = function () {
    var data = this().slice(0);
    this([]);
    this(data);
};