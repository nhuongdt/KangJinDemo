// validate ky tu dac biet
function isValid(str) {
    return !/[~`!@#$%\^&*()+=\-\[\]\\';,/{}|\\":<>\?]/g.test(str);
};
// khong cho phep nhap chu
function keypressNumber(e) {

    var keyCode = window.event.keyCode || e.which;
    if (keyCode < 48 || keyCode > 57) {
        // 8: tab; 127: delete
        if (keyCode == 8 || keyCode == 127) {
            return;
        }
        return false;
    }
}
// validate nhap email
function isValidEmail(email) {
    var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
    return pattern.test(email);
};

var FormModel_NewNhanVien = function () {
    var self = this;
    self.ID = ko.observable();
    self.MaNhanVien = ko.observable();
    self.TenNhanVien = ko.observable();
    self.NgaySinh = ko.observable();
    self.NguyenQuan = ko.observable();
    this.Email = ko.observable();
    this.GioiTinh = ko.observable(true);
    self.ThuongTru = ko.observable();
    self.DienThoaiDiDong = ko.observable();
    self.SoCMND = ko.observable();
    self.SoBHXH = ko.observable();
    self.GhiChu = ko.observable();
    this.DaNghiViec = ko.observable(true);
    self.Image = ko.observable('');
    self.ImageRemove = ko.observable('');
    //self.DaNghiViec = ko.observableArray();


    self.SetData = function (item) {
        self.ID(item.ID);
        self.MaNhanVien(item.MaNhanVien);
        self.TenNhanVien(item.TenNhanVien);
        self.NgaySinh(moment(item.NgaySinh, "YYYY-MM-DD hh:mm:ss").format("DD/MM/YYYY"));
        self.NguyenQuan(item.NguyenQuan);
        self.Email(item.Email);
        this.GioiTinh(item.GioiTinh);
        this.ThuongTru(item.ThuongTru);
        this.DienThoaiDiDong(item.DienThoaiDiDong);
        self.SoCMND(item.SoCMND);
        self.SoBHXH(item.SoBHXH);
        self.GhiChu(item.GhiChu);
        if (item.DaNghiViec == false)
            this.DaNghiViec(true);
        else
            this.DaNghiViec(false);
        if (item.Image !== "") {
            self.Image(Open24FileManager.hostUrl + item.Image);
            self.ImageRemove(item.Image);
        }
        else {
            self.Image('');
            self.ImageRemove('');
        }
        //self.DaNghiViec(item.DaNghiViec);
    };
};
var ViewModel = function () {
    var NhanVienUri = '/api/DanhMuc/NS_NhanVienAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';
    var _id_NhanVien_LS = $('.idnhanvien').text();
    var _id_DonVi = $('#hd_IDdDonVi').val();
    var _IDNguoiDung = $('.idnguoidung').text();
    var self = this;
    self.deleteMaNhanVien = ko.observable();
    self.deleteID = ko.observable();
    self.error = ko.observable();
    self.filter = ko.observable();
    self.NhanViens = ko.observableArray();
    self.booleanAdd = ko.observable(true);
    self.newNhanVien = ko.observable(new FormModel_NewNhanVien());
    self.loc_DaNghiViec = ko.observable("2");
    self.TieuDeThemNhanVien = ko.observable('Thêm mới nhân viên');
    self.Quyen_NguoiDung = ko.observableArray();
    self.rolePhongBan_Insert = ko.observable(CheckQuyenExist('NS_PhongBan_ThemMoi'));
    self.rolePhongBan_Update = ko.observable(CheckQuyenExist('NS_PhongBan_CapNhat'));
    self.rolePhongBan_Delete = ko.observable(CheckQuyenExist('NS_PhongBan_Xoa'));

    //Start HeaderInit
    self.ListHeader = ko.observableArray();
    var _id_ChiNhanh_Defeaul = {
        ID: $('#hd_IDdDonVi').val()
    }
    self.ListHeaderSelected = ko.observableArray([]);
    self.ListHeader = [{ colName: 'colMaNhanVien', colText: 'Mã nhân viên', colshow: true, index: 0, isHRM: false, GhiChu: '' },
    { colName: 'colTenNhanVien', colText: 'Tên nhân viên', colshow: true, index: 1, isHRM: false, GhiChu: '' },
    { colName: 'colNgaySinh', colText: 'Ngày sinh', colshow: true, index: 2, isHRM: true, GhiChu: '' },
    { colName: 'colGioiTinh', colText: 'Giới tính', colshow: true, index: 3, isHRM: true, GhiChu: '' },
    { colName: 'colNoiSinh', colText: 'Nơi sinh', colshow: true, index: 4, isHRM: true, GhiChu: '' },
    { colName: 'colDienThoai', colText: 'Điện thoại', colshow: true, index: 5, isHRM: false, GhiChu: '' },
    { colName: 'colEmail', colText: 'Email', colshow: true, index: 6, isHRM: false, GhiChu: '' },
    { colName: 'colCMND', colText: 'CMND/CCCD', colshow: true, index: 7, isHRM: true, GhiChu: '' },
    { colName: 'colNgayCap', colText: 'Ngày cấp', colshow: true, index: 8, isHRM: true, GhiChu: '' },
    { colName: 'colNoiCap', colText: 'Nơi cấp', colshow: true, index: 9, isHRM: true, GhiChu: '' },
    { colName: 'colDanToc', colText: 'Dân tộc', colshow: true, index: 10, isHRM: true, GhiChu: '' },
    { colName: 'colTonGiao', colText: 'Tôn giáo', colshow: true, index: 11, isHRM: true, GhiChu: '' },
    { colName: 'colHonNhan', colText: 'Hôn nhân', colshow: true, index: 12, isHRM: true, GhiChu: '' },
    { colName: 'colQuocTich', colText: 'Quốc tịch', colshow: true, index: 13, isHRM: true, GhiChu: '' },
    { colName: 'colHKDiaChi', colText: 'Địa chỉ', colshow: true, index: 14, isHRM: true, GhiChu: ' (Hộ khẩu)' },
    { colName: 'colHKXaPhuong', colText: 'Xã phường', colshow: true, index: 15, isHRM: true, GhiChu: ' (Hộ khẩu)' },
    { colName: 'colHKQuanHuyen', colText: 'Quận huyện', colshow: true, index: 16, isHRM: true, GhiChu: ' (Hộ khẩu)' },
    { colName: 'colHKTinhThanh', colText: 'Tỉnh thành', colshow: true, index: 17, isHRM: true, GhiChu: ' (Hộ khẩu)' },
    { colName: 'colTTDiaChi', colText: 'Địa chỉ', colshow: true, index: 18, isHRM: true, GhiChu: ' (Tạm trú)' },
    { colName: 'colTTXaPhuong', colText: 'Xã phường', colshow: true, index: 19, isHRM: true, GhiChu: ' (Tạm trú)' },
    { colName: 'colTTQuanHuyen', colText: 'Quận huyện', colshow: true, index: 20, isHRM: true, GhiChu: ' (Tạm trú)' },
    { colName: 'colTTTinhThanh', colText: 'Tỉnh thành', colshow: true, index: 21, isHRM: true, GhiChu: ' (Tạm trú)' },
    { colName: 'colPhongBan', colText: 'Phòng ban', colshow: true, index: 22, isHRM: true, GhiChu: '' },
    { colName: 'colNgayVaoLam', colText: 'Ngày vào làm', colshow: true, index: 23, isHRM: true, GhiChu: '' },
    { colName: 'colTrangThai', colText: 'Trạng thái', colshow: true, index: 24, isHRM: false, GhiChu: '' }];


    $('.dropdown-menu-header').on('click', function () {
        // your code goes here when dropdown closed
        localStorage.setItem("HeaderNSNhanVien", JSON.stringify(self.ListHeaderSelected()));
    });

    function LoadHeaderFromLocalStorage() {
        var cacheHeaderNhanVien = localStorage.getItem('HeaderNSNhanVien');
        if (cacheHeaderNhanVien === null || cacheHeaderNhanVien === 'undefined') {
            self.ListHeaderSelected = ko.observableArray([0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24]);
            localStorage.setItem("HeaderNSNhanVien", JSON.stringify(self.ListHeaderSelected()));
        }
        else {
            self.ListHeaderSelected = ko.observableArray(JSON.parse(cacheHeaderNhanVien));
        }
    }
    LoadHeaderFromLocalStorage();


    //End HeaderInit

    self.ChiNhanh_Defeaul = ko.observableArray();
    self.ChiNhanh_Defeaul.push(_id_ChiNhanh_Defeaul);
    self.ContinueImport = ko.observable(false);

    self.role_ThietLapLuongInsert = ko.observable(false);
    self.role_ThietLapLuongCopy = ko.observable(false);
    self.role_ThietLapLuongUpdate = ko.observable(false);
    self.role_ThietLapLuongXemDS = ko.observable(false);

    //self.DaNghiViec = ko.observableArray([
    //    { name: "Đang làm việc", value: "true" },
    //    { name: "Đã nghỉ việc", value: "false" }
    //]);
    //self.selectedDaNghiViec = ko.observable();
    var nextPageAll_NhanVien = 1;

    function GetQuyenNguoiDung() {
        var arrQuyen = [];
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetHT_NhomNguoiDung?idnguoidung=" + _IDNguoiDung + '&iddonvi=' + _id_DonVi, 'GET').done(function (data) {
            if (data.ID !== null) {
                self.Quyen_NguoiDung(data.HT_Quyen_NhomDTO);

                self.role_ThietLapLuongInsert(CheckQuyen_byMa('ThietLapLuong_ThemMoi'));
                self.role_ThietLapLuongCopy(CheckQuyen_byMa('ThietLapLuong_SaoChep'));
                self.role_ThietLapLuongUpdate(CheckQuyen_byMa('ThietLapLuong_CapNhat'));
                self.role_ThietLapLuongXemDS(CheckQuyen_byMa('ThietLapLuong_XemDS'));
            }
            else {
                commonStatisJs.ShowMessageDanger('Không có quyền xem danh sách ' + sLoai);
            }
        });
    }
    GetQuyenNguoiDung();

    function CheckQuyen_byMa(maquyen) {
        var role = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen === maquyen;
        });
        return role.length > 0;
    }

    function CheckQuyenExist(maquyen) {
        return VHeader.Quyen.indexOf(maquyen) > -1;
    }

    function GetListPhongBan_byChiNhanh(idChiNhanh) {
        $.getJSON(NhanVienUri + "GetTreePhongBan?chinhanhId=" + idChiNhanh, function (data) {
            self.listAddChiNhanh()[0].listPhongBan = data;
            self.listAddChiNhanh.refresh();
        });
    }

    self.themmoicapnhatnv = function () {
        //$("#Staff").show();
        //$("#staffnew").hide();
        if (CheckQuyen_byMa('NhanVien_ThemMoi')) {
            refreshChiNhanh();
            self.TieuDeThemNhanVien('Thêm mới nhân viên');
            self.MangChiNhanhNhanVien([]);
            if (self.ListAddNewStaffUnit().some(o => o === $('#hd_IDdDonVi').val())) {
                OnloadForm();
                changeButtonPopup();
            }
            else {
                self.resetTextBox();
                self.booleanAdd(true);
                $('#modalPopuplg_NV').modal('show');
            }
            GetListPhongBan_byChiNhanh(_id_DonVi);
        }
    };
    self.formatDatetime = function () {
        $('#datetimepicker_mask').datetimepicker({
            timepicker: false,
            mask: true, // '9999/19/39 29:59' - digit is the maximum possible for a cell
            format: 'd/m/Y',
        });
    }

    self.editNV = function (item) {
        ajaxHelper(NhanVienUri + "getlistQTCongTac?ID_NhanVien=" + item.ID, "GET").done(function (data) {
            self.MangChiNhanhNhanVien(data);
            self.listAddChiNhanh(data);
            if (data.length == 0)
                refreshChiNhanh();
            if (CheckQuyen_byMa('NhanVien_CapNhat')) {
                self.TieuDeThemNhanVien('Cập nhật nhân viên');
                if (self.ListAddNewStaffUnit().some(o => o === $('#hd_IDdDonVi').val())) {
                    ajaxHelper(NhanVienUri + "GetDetailStaff/" + item.ID, 'GET').done(function (data) {
                        if (data.res === true) {
                            setUpdateStaff(data.dataSoure.model);
                            EditStaffphongbanchinhanh(data.dataSoure.listChiNhanhPhongBan);
                            changeButtonPopup();
                        }
                        else {
                            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + data.mess, "danger");
                        }
                    });

                }
                else {
                    ajaxHelper(NhanVienUri + "GetNS_NhanVien/" + item.ID, 'GET').done(function (data) {
                        self.newNhanVien().SetData(data);
                        self.booleanAdd(false);

                        // asssign chihanh, phongban for nhanvien
                        for (let i = 0; i < self.MangChiNhanhNhanVien().length; i++) {
                            let itFor = self.MangChiNhanhNhanVien()[i];
                            LoadPhongBanChiNhanh(itFor.ID_ChiNhanh)
                        }
                        $('#modalPopuplg_NV').modal('show');
                    });
                }
            }
            else {
                commonStatisJs.ShowMessageDanger('Không có quyền cập nhật');
            }
        });
    }

    self.resetTextBox = function () {
        self.newNhanVien(new FormModel_NewNhanVien());
    }

    function ajaxHelper(uri, method, data) {
        self.error(''); // Clear error message
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null,
            statusCode: {
                404: function () {
                    self.error("Page not found");
                }
            }
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
            });
    }

    self.modalDelete = function (item) {
        if (CheckQuyen_byMa('NhanVien_Xoa')) {
            self.deleteMaNhanVien(item.TenNhanVien + ' (' + item.MaNhanVien + ')');
            self.deleteID(item.ID);
            $('#modalpopup_deleteNV').modal('show');
        }
        else {
            commonStatisJs.ShowMessageDanger('Không có quyền xóa');
        }
    };

    //Xóa
    self.xoaNV = function (NhanViens) {
        var objDiary = {
            ID_NhanVien: _id_NhanVien_LS,
            ID_DonVi: _id_DonVi,
            ChucNang: "Nhân viên",
            NoiDung: "Xóa nhân viên " + self.deleteMaNhanVien(),
            NoiDungChiTiet: "Xóa nhân viên ".concat(self.deleteMaNhanVien(), ', Người xóa: ', VHeader.UserLogin),
            LoaiNhatKy: 3 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
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

                $.ajax({
                    type: "DELETE",
                    url: NhanVienUri + "DeleteNS_NhanVien/" + NhanViens.deleteID(),
                    dataType: 'json',
                    contentType: 'application/json',
                    success: function (result) {
                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Xóa nhân viên thành công !", "success");
                        getAllNhanViens();
                    },
                    error: function (error) {
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Xóa thất bại. Vì nhân viên này đã thực hiện hóa đơn !", "danger");
                    }
                })
            },
            statusCode: {
                404: function () {
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
            },
            complete: function () {

            }
        })

    };

    //THem/Sua
    self.addNhanVien = function () {
        var button = (event.target);
        $(button).addClass("cursor-waiting");
        var _manhanvien = self.newNhanVien().MaNhanVien();
        _manhanvien = _manhanvien == "" ? undefined : (_manhanvien == null ? undefined : _manhanvien);
        var _id = self.newNhanVien().ID();
        var _tennhanvien = self.newNhanVien().TenNhanVien();
        var _ngaysinh = self.newNhanVien().NgaySinh();
        var _quequan = self.newNhanVien().NguyenQuan();
        var _email = this.newNhanVien().Email();
        _email = _email == "" ? null : _email;
        var _dienthoai = this.newNhanVien().DienThoaiDiDong();
        var _gioitinh = this.newNhanVien().GioiTinh();
        var _socmt = this.newNhanVien().SoCMND();
        var _sobh = this.newNhanVien().SoBHXH();
        var _ghichu = this.newNhanVien().GhiChu();
        var _diachi = this.newNhanVien().ThuongTru();
        var _danghiviec = false;
        if (this.newNhanVien().DaNghiViec() == false)
            _danghiviec = true;
        else
            _danghiviec = false;
        //var _danghiviec = this.newNhanVien().DaNghiViec();

        if (!isValid(_manhanvien)) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Mã nhân viên không được nhập ký tự đặc biệt!", "danger");
            $('#txtMaNhanVien').focus();

            $(button).removeClass("cursor-waiting");
            return false;
        }
        if (_tennhanvien == null || _tennhanvien == "" || _tennhanvien == "undefined") {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Vui lòng nhập tên nhân viên!", "danger");

            $(button).removeClass("cursor-waiting");
            $('#txtTenNhanVien1').focus();
            return false;
        }
        var dtNow = new Date(Date.now());
        var date1 = '';
        if (_ngaysinh === undefined || _ngaysinh === 'Invalid date' || _ngaysinh === "") {
            _ngaysinh = moment(dtNow).format('YYYY-MM-DD HH:mm:ss')
            date1 = moment(dtNow).format('DD/MM/YYYY');
        }
        else {
            _ngaysinh = moment(_ngaysinh, "DD/MM/YYYY").format("YYYY/MM/DD hh:mm:ss");
            date1 = self.newNhanVien().NgaySinh();
        }

        // check ngaysinh > Date now
        var date2 = moment(dtNow).format('DD/MM/YYYY');
        if ($.datepicker.parseDate('dd/mm/yy', date1) > $.datepicker.parseDate('dd/mm/yy', date2)) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ngày sinh không được lớn hơn ngày hiện tại", "danger");

            $(button).removeClass("cursor-waiting");
            return false;
        }
        if (_email != null) {
            if (!isValidEmail(_email)) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Email không đúng định dạng", "danger");

                $(button).removeClass("cursor-waiting");
                return false;
            }
        }

        if (_danghiviec && _id === self.ID_NhanVienAdmin()) {
            ShowMessage_Danger("Nhân viên là 'Admin'. Vui lòng không cho nghỉ việc");
            return;
        }

        var Nhan_Vien = {
            ID: _id,
            MaNhanVien: _manhanvien,
            TenNhanVien: _tennhanvien,
            NgaySinh: _ngaysinh,
            NguyenQuan: _quequan,
            ThuongTru: _diachi,
            Email: _email,
            DienThoaiDiDong: _dienthoai,
            GioiTinh: _gioitinh,
            SoCMND: _socmt,
            SoBHXH: _sobh,
            GhiChu: _ghichu,
            DaNghiViec: _danghiviec
        };


        if (self.booleanAdd() === true) {
            var myData = {};
            myData.objNVien = Nhan_Vien;
            if (self.listAddChiNhanh()[0].ID_ChiNhanh == null) {
                myData.objQuaTrinhCongTac = [{
                    ID: 1,
                    ID_ChiNhanh: $('#hd_IDdDonVi').val(),
                    ID_PhongBan: null,
                    LaMacDinh: true,
                    Text_ChiNhanh: $("#_txtTenDonVi").text(),
                    Text_PhongBan: "Phòng ban mặc định"
                }];

            }
            else {
                myData.objQuaTrinhCongTac = self.listAddChiNhanh();

            }
            $.ajax({
                data: Nhan_Vien,
                url: NhanVienUri + "Check_Exist",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (item) {

                    $(button).removeClass("cursor-waiting");
                    // khong dong popup neu item !== ''
                    if (item === '') {
                        callAjaxAdd(myData);
                    }
                    else {
                        bottomrightnotify(item, "danger");
                    }


                },
                statusCode: {
                    404: function () {
                        self.error("page not found");

                        $(button).removeClass("cursor-waiting");
                    },
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                },
            })
        }
        //Sua
        else {
            var myData = {};
            myData.id = _id;
            myData.objNVien = Nhan_Vien;
            myData.objQuaTrinhCongTac = self.listAddChiNhanh();
            var dem = 0;
            var arrChiNhanhNV = [];
            ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + 'GetListVaiTro?idnhanvien=' + _id, 'GET').done(function (data) {
                for (var i = 0; i < myData.objQuaTrinhCongTac.length; i++) {
                    arrChiNhanhNV.push(myData.objQuaTrinhCongTac[i].ID);
                }
                for (var i = 0; i < data.length; i++) {
                    if ($.inArray(data[i].ID_DonVi, arrChiNhanhNV) > -1) {
                        dem = dem + 1;
                    }
                }
                //if (dem === data.length) {
                $.ajax({
                    data: Nhan_Vien,
                    url: NhanVienUri + "Check_Exist",
                    type: 'POST',
                    async: true,
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",

                    success: function (item) {
                        // khong dong popup neu item !== ''
                        if (item === '') {
                            callAjaxUpdate(myData);
                        }
                        else {
                            bottomrightnotify(item, "danger");
                        }
                        $(button).removeClass("cursor-waiting");

                    },
                    statusCode: {
                        404: function () {
                            self.error("page not found");
                        },
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                    },
                })

            })


        }
    };

    function callAjaxAdd(myData) {
        var formDataTR = new FormData();
        var totalFilesTR = document.getElementById("imageUploadFormTR").files.length;
        for (var i = 0; i < totalFilesTR; i++) {
            var file = document.getElementById("imageUploadFormTR").files[i];
            formDataTR.append("files", file);
        }
        $.ajax({
            data: myData,
            url: NhanVienUri + "PostNS_NhanVien?ID_DonVi=" + _id_DonVi + "&ID_NhanVien=" + _id_NhanVien_LS,
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                if (totalFilesTR > 0) {
                    //$.ajax({
                    //    type: "POST",
                    //    url: NhanVienUri + "UploadImageStaff?nhanvienId=" + item.ID,
                    //    data: formDataTR,
                    //    dataType: 'json',
                    //    contentType: false,
                    //    processData: false,
                    //    success: function (response) {
                    //        bottomrightnotify("Thêm mới nhân viên thành công", "success");
                    //        getAllNhanViens();
                    //    },
                    //});
                    let myData = {};
                    myData.Subdomain = VHeader.SubDomain;
                    myData.Function = "8"; //8. Nhân viên
                    myData.Id = item.ID;
                    myData.files = formDataTR;
                    var result = Open24FileManager.UploadImage(myData);
                    if (result.length > 0) {
                        $.ajax({
                            url: NhanVienUri + "UpdateAnhNhanVien?id=" + item.ID,
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
                    bottomrightnotify("Thêm mới nhân viên thành công", "success");
                    getAllNhanViens();
                }
                else {
                    bottomrightnotify("Thêm mới nhân viên thành công", "success");
                    getAllNhanViens();
                }
            },
            statusCode: {
                404: function () {
                    self.error("page not found");
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                self.error(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
                bottomrightnotify("Thêm mới nhân viên thất bại", "danger");
            },
            complete: function () {
                $("#modalPopuplg_NV").modal("hide");
            }
        })
    }

    function callAjaxUpdate(myData) {
        var formDataTR = new FormData();
        var totalFilesTR = document.getElementById("imageUploadFormTR").files.length;
        for (var i = 0; i < totalFilesTR; i++) {
            var file = document.getElementById("imageUploadFormTR").files[i];
            formDataTR.append("files", file);
        }
        $.ajax({
            url: NhanVienUri + "PutNS_NhanVien?ID_DonVi=" + _id_DonVi + "&ID_NhanVien=" + _id_NhanVien_LS,
            type: 'PUT',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myData,
            success: function () {
                if (totalFilesTR > 0) {
                    //$.ajax({
                    //    type: "POST",
                    //    url: NhanVienUri + "UploadImageStaff?nhanvienId=" + self.newNhanVien().ID(),
                    //    data: formDataTR,
                    //    dataType: 'json',
                    //    contentType: false,
                    //    processData: false,
                    //    success: function (response) {
                    //        bottomrightnotify("Cập nhật nhân viên thành công", "success");
                    //        getAllNhanViens();
                    //    },
                    //});
                    let myData = {};
                    myData.Subdomain = VHeader.SubDomain;
                    myData.Function = "8"; //8. Nhân viên
                    myData.Id = self.newNhanVien().ID();
                    myData.files = formDataTR;
                    var result = Open24FileManager.UploadImage(myData);
                    if (result.length > 0) {
                        $.ajax({
                            url: NhanVienUri + "UpdateAnhNhanVien?id=" + self.newNhanVien().ID(),
                            type: "POST",
                            data: JSON.stringify(result),
                            contentType: "application/json",
                            dataType: "JSON",
                            success: function (data, textStatus, jqXHR) {
                                if (self.newNhanVien().ImageRemove() !== "")
                                    Open24FileManager.RemoveFiles([self.newNhanVien().ImageRemove()]);
                            },
                            error: function (jqXHR, textStatus, errorThrown) {

                            }
                        });
                    }
                    bottomrightnotify("Cập nhật nhân viên thành công", "success");
                    getAllNhanViens();
                }
                else {
                    bottomrightnotify("Cập nhật nhân viên thành công", "success");
                    getAllNhanViens();
                }
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
                $("#modalPopuplg_NV").modal("hide");
            }
        })
    }
    self.showModelPhongBan = function (item) {
        vmNsPhongBan.Insert(item.ID_ChiNhanh);
        //self.ShowPopupPhongban();
    }
    self.ShowEditPhongBan = function (item) {
        vmNsPhongBan.edit(item, $('#RolePhongBan_Update').val(), $('#RolePhongBan_Delete').val());
    }
    var _maNhanVien_seach = null;
    var _trangthai_seach = 0;
    self.PagesAll_NhanVien = ko.observableArray();
    self.RowsAll_NhanVien = ko.observable();
    var pageAllSize = 10;
    var pageAllNum = 1;
    function load_cache() {
        var lc_NhanVien = localStorage.getItem("Ls_NhanVien");
        if (lc_NhanVien != null) {
            _maNhanVien_seach = lc_NhanVien;
            $('#txtMaNhanVien1').val(lc_NhanVien);
            localStorage.removeItem("Ls_NhanVien");
        }
    }
    load_cache();
    self.selectedPhongBan = ko.observable(null);
    self.selectAllPhongBan = function () {
        self.selectedPhongBan(null);
        getAllNhanViens();
    }

    // phân trang nhân viên
    //self.PagesAll_NhanVien = ko.observableArray([{ SoTrang: 1 }]);
    self.RowsStartAll_NhanVien = ko.observable();
    self.RowsEndAll_NhanVien = ko.observable();
    self.currentPageAll_NhanVien = ko.observable(1);
    self.GetClassAll_NhanVien = function (page) {
        return (page.SoTrang === self.currentPageAll_NhanVien()) ? "click" : "";
    };
    self.selecPageAll_NhanVien = function () {
        self.PagesAll_NhanVien([]);
        if (AllPageAll_NhanVien > 4) {
            self.PagesAll_NhanVien.push({ SoTrang: 1 });
            self.PagesAll_NhanVien.push({ SoTrang: 2 });
            self.PagesAll_NhanVien.push({ SoTrang: 3 });
            self.PagesAll_NhanVien.push({ SoTrang: 4 });
            self.PagesAll_NhanVien.push({ SoTrang: 5 });
        }
        else {
            for (var j = 0; j < AllPageAll_NhanVien; j++) {
                self.PagesAll_NhanVien.push({ SoTrang: j + 1 });
            }
        }
    }
    self.ReserPageAll_NhanVien = function (item) {
        if (nextPageAll_NhanVien > 1 && AllPageAll_NhanVien > 5) {
            if (nextPageAll_NhanVien > 3 && nextPageAll_NhanVien < AllPageAll_NhanVien - 1) {
                for (var i = 0; i < 5; i++) {
                    self.PagesAll_NhanVien.replace(self.PagesAll_NhanVien()[i], { SoTrang: parseInt(nextPageAll_NhanVien) + i - 2 });
                }
            }
            else if (parseInt(nextPageAll_NhanVien) === parseInt(AllPageAll_NhanVien) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.PagesAll_NhanVien.replace(self.PagesAll_NhanVien()[i], { SoTrang: parseInt(nextPageAll_NhanVien) + i - 3 });
                }
            }
            else if (nextPageAll_NhanVien == AllPageAll_NhanVien) {
                for (var i = 0; i < 5; i++) {
                    self.PagesAll_NhanVien.replace(self.PagesAll_NhanVien()[i], { SoTrang: parseInt(nextPageAll_NhanVien) + i - 4 });
                }
            }
        }
        else if (nextPageAll_NhanVien == 1) {
            for (var i = 0; i < 5; i++) {
                self.PagesAll_NhanVien.replace(self.PagesAll_NhanVien()[i], { SoTrang: parseInt(nextPageAll_NhanVien) + i });
            }
        }
        self.currentPageAll_NhanVien(parseInt(nextPageAll_NhanVien));
    }
    self.ReRenderPage = function () {
        setTimeout(function () {
            $('.collumn-filter input[type="checkbox"]').each(function () {
                let target = ($(this).val());
                if ($(this).is(':checked') == true) {
                    $('.' + target).show();
                } else {
                    $('.' + target).hide();
                }
            });
        }, 1000)

    };

    self.NextPageAll_NhanVien = function (item) {
        if (nextPageAll_NhanVien < AllPageAll_NhanVien) {
            nextPageAll_NhanVien = nextPageAll_NhanVien + 1;
            pageNum = nextPageAll_NhanVien;
            pageAllNum = nextPageAll_NhanVien;
            self.ReserPageAll_NhanVien();
            self.gotoNextPageAll_NhanVien();
        }
    };
    self.BackPageAll_NhanVien = function (item) {
        if (nextPageAll_NhanVien > 1) {
            nextPageAll_NhanVien = nextPageAll_NhanVien - 1;
            pageNum = nextPageAll_NhanVien;
            pageAllNum = nextPageAll_NhanVien;
            self.ReserPageAll_NhanVien();
            self.gotoNextPageAll_NhanVien();
        }
    };
    self.EndPageAll_NhanVien = function (item) {
        //debugger;
        nextPageAll_NhanVien = AllPageAll_NhanVien;
        pageNum = nextPageAll_NhanVien;
        pageAllNum = nextPageAll_NhanVien;
        self.ReserPageAll_NhanVien();
        self.gotoNextPageAll_NhanVien();
    };
    self.StartPageAll_NhanVien = function (item) {
        nextPageAll_NhanVien = 1;
        pageNum = nextPageAll_NhanVien;
        pageAllNum = nextPageAll_NhanVien;
        self.gotoNextPageAll_NhanVien();
        self.ReserPageAll_NhanVien();
    };
    self.gotoNextPageAll_NhanVien = function (item) {

        LoadingForm(true);
        try {
            var numbepage = item.SoTrang;
            pageAllNum = numbepage;
            nextPageAll_NhanVien = numbepage;
        }
        catch (e) {

        }
        if ($('#IsOpenHRM').val() === 'True') {
            getAllNhanViens(false, true);
        }
        else {
            ajaxHelper(NhanVienUri + "getListNhanVien?phongbanId=" + self.selectedPhongBan() + "&maNhanVien=" + _maNhanVien_seach + "&trangthai=" + _trangthai_seach + "&pageSize=" + pageAllSize + "&pageNum=" + pageAllNum, 'GET').done(function (data) {
                self.NhanViens(data.LstData);
                LoadHtmlGrid();
                if (self.NhanViens() != null) {
                    self.RowsStartAll_NhanVien((pageAllNum - 1) * pageAllSize + 1);
                    self.RowsEndAll_NhanVien((pageAllNum - 1) * pageAllSize + self.NhanViens().length)
                }
                else {
                    self.RowsStartAll_NhanVien('0');
                    self.RowsEndAll_NhanVien('0');
                }
                self.ReserPageAll_NhanVien();

                LoadingForm(false);
            });
        };
        self.ReRenderPage();

    }
    //lấy mã nhân viên
    self.selectMaNhanVien = function () {
        _maNhanVien_seach = $('#txtMaNhanVien1').val();
    }
    $('#txtMaNhanVien1').keypress(function (e) {
        if (e.keyCode == 13) {
            pageAllNum = 1;
            self.currentPageAll_NhanVien(pageAllNum);
            getAllNhanViens(true);
        }
    });
    $('.choseTrangThai input').on('click', function () {
        _trangthai_seach = $(this).val();
        pageAllNum = 1;
        self.currentPageAll_NhanVien(pageAllNum);
        getAllNhanViens(true);
    });
    self.HD_NhanViens = ko.observableArray();
    self.SQ_NhanViens = ko.observableArray();
    self.PagesHD_NhanVien = ko.observableArray();
    self.RowsHD_NhanVien = ko.observable();
    self.PagesSQ_NhanVien = ko.observableArray();
    self.RowsSQ_NhanVien = ko.observable();
    var pageSize = 10;
    var pageNum = 1;
    self.currentPageHD_NhanVien = ko.observable(1);
    self.currentPageSQ_NhanVien = ko.observable(1);
    var nextPageHD_NhanVien = 1;
    var nextPageSQ_NhanVien = 1;
    self.RowsStartHD_NhanVien = ko.observable('0');
    self.RowsEndHD_NhanVien = ko.observable('10');
    self.RowsStartSQ_NhanVien = ko.observable('0');
    self.RowsEndSQ_NhanVien = ko.observable('10');
    var AllPageHD_NhanVien;
    var AllPageSQ_NhanVien;
    var _id_Nhanvien;
    //Phân trang hóa đơn - nhân viên
    self.GetClassHD_NhanVien = function (page) {
        return (page.SoTrang === self.currentPageHD_NhanVien()) ? "click" : "";
    };
    self.selecPageHD_NhanVien = function () {
        self.PagesHD_NhanVien([]);
        if (AllPageHD_NhanVien > 4) {
            self.PagesHD_NhanVien.push({ SoTrang: 1 });
            self.PagesHD_NhanVien.push({ SoTrang: 2 });
            self.PagesHD_NhanVien.push({ SoTrang: 3 });
            self.PagesHD_NhanVien.push({ SoTrang: 4 });
            self.PagesHD_NhanVien.push({ SoTrang: 5 });
        }
        else {
            for (var j = 0; j < AllPageHD_NhanVien; j++) {
                self.PagesHD_NhanVien.push({ SoTrang: j + 1 });
            }
        }
    }
    self.ReserPageHD_NhanVien = function (item) {
        if (nextPageHD_NhanVien > 1 && AllPageHD_NhanVien > 5) {
            if (nextPageHD_NhanVien > 3 && nextPageHD_NhanVien < AllPageHD_NhanVien - 1) {
                for (var i = 0; i < 5; i++) {
                    self.PagesHD_NhanVien.replace(self.PagesHD_NhanVien()[i], { SoTrang: parseInt(nextPageHD_NhanVien) + i - 2 });
                }
            }
            else if (parseInt(nextPageHD_NhanVien) === parseInt(AllPageHD_NhanVien) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.PagesHD_NhanVien.replace(self.PagesHD_NhanVien()[i], { SoTrang: parseInt(nextPageHD_NhanVien) + i - 3 });
                }
            }
            else if (nextPageHD_NhanVien == AllPageHD_NhanVien) {
                for (var i = 0; i < 5; i++) {
                    self.PagesHD_NhanVien.replace(self.PagesHD_NhanVien()[i], { SoTrang: parseInt(nextPageHD_NhanVien) + i - 4 });
                }
            }
        }
        else if (nextPageHD_NhanVien == 1) {
            for (var i = 0; i < 5; i++) {
                self.PagesHD_NhanVien.replace(self.PagesHD_NhanVien()[i], { SoTrang: parseInt(nextPageHD_NhanVien) + i });
            }
        }
        self.currentPageHD_NhanVien(parseInt(nextPageHD_NhanVien));
    }
    self.NextPageHD_NhanVien = function (item) {
        if (nextPageHD_NhanVien < AllPageHD_NhanVien) {
            nextPageHD_NhanVien = nextPageHD_NhanVien + 1;
            pageNum = nextPageHD_NhanVien;
            self.ReserPageHD_NhanVien();
            self.gotoNextPageHD_NhanVien();
        }
    };
    self.BackPageHD_NhanVien = function (item) {
        if (nextPageHD_NhanVien > 1) {
            nextPageHD_NhanVien = nextPageHD_NhanVien - 1;
            pageNum = nextPageHD_NhanVien;
            self.ReserPageHD_NhanVien();
            self.gotoNextPageHD_NhanVien();
            self.ReRenderPage();
        }
    };
    self.EndPageHD_NhanVien = function (item) {
        nextPageHD_NhanVien = AllPageHD_NhanVien;
        pageNum = nextPageHD_NhanVien;
        self.ReserPageHD_NhanVien();
        self.gotoNextPageHD_NhanVien();
    };
    self.StartPageHD_NhanVien = function (item) {
        nextPageHD_NhanVien = 1;
        pageNum = nextPageHD_NhanVien;
        self.gotoNextPageHD_NhanVien();
        self.ReserPageHD_NhanVien();
        self.ReRenderPage();
    };
    self.gotoNextPageHD_NhanVien = function (item) {
        try {
            var numbepage = item.SoTrang;
            pageNum = numbepage;
            nextPageHD_NhanVien = numbepage;
        }
        catch (e) {

        }
        ajaxHelper(NhanVienUri + "getListHD_NhanVien?ID_NhanVien=" + _id_Nhanvien + "&pageSize=" + pageSize + "&pageNum=" + pageNum, "GET").done(function (data) {
            self.HD_NhanViens(data);

            if (self.HD_NhanViens() != null) {
                self.RowsStartHD_NhanVien((pageNum - 1) * pageSize + 1);
                self.RowsEndHD_NhanVien((pageNum - 1) * pageSize + self.HD_NhanViens().length)
            }
            else {
                self.RowsStartHD_NhanVien('0');
                self.RowsEndHD_NhanVien('0');
            }
            self.ReserPageHD_NhanVien();
        });
        self.ReRenderPage()
    }
    var _ma_Nhanvien;
    self.loadQTCongTac = function (item, e) {
        _id_Nhanvien = item.ID;
        _ma_Nhanvien = item.MaNhanVien + ": " + item.TenNhanVien;
        //ajaxHelper(NhanVienUri + "getlistQTCongTac?ID_NhanVien=" + item.ID, "GET").done(function (data) {
        //    self.MangChiNhanhNhanVien(data);
        //    for (var i = 0; i < self.MangChiNhanhNhanVien().length; i++) {
        //        $('#selec-all-DonVi li').each(function () {
        //            if ($(this).attr('id') === self.MangChiNhanhNhanVien()[i].ID) {
        //                $(this).find('.fa-check').remove();
        //                $(this).append('<i class="fa fa-check check-after-li"></i>')
        //            }
        //        });
        //    }
        //});
        self.loadHD_NhanVien(item);
        self.loadSQ_NhanVien(item);
    }

    self.loadHD_NhanVien = function (item) {
        ajaxHelper(NhanVienUri + "getListHD_NhanVien?ID_NhanVien=" + item.ID + "&pageSize=" + pageSize + "&pageNum=" + pageNum, "GET").done(function (data) {

            self.HD_NhanViens(data);
            //self.ReserPageHD_NhanVien();
            if (self.HD_NhanViens() != null) {
                self.RowsStartHD_NhanVien((pageNum - 1) * pageSize + 1);
                self.RowsEndHD_NhanVien((pageNum - 1) * pageSize + self.HD_NhanViens().length)
            }
            else {
                self.RowsStartHD_NhanVien('0');
                self.RowsEndHD_NhanVien('0');
            }

        });
        ajaxHelper(NhanVienUri + "getRowsHD_NhanVien?ID_NhanVien=" + item.ID, "GET").done(function (data) {
            self.RowsHD_NhanVien(data);
        });
        ajaxHelper(NhanVienUri + "getPageHD_NhanVien?ID_NhanVien=" + item.ID + "&pageSize=" + pageSize, "GET").done(function (data) {
            self.PagesHD_NhanVien(data);
            AllPageHD_NhanVien = self.PagesHD_NhanVien().length;
            self.selecPageHD_NhanVien();
        });
    }
    // Phân trang sổ quỹ - nhân viên
    self.GetClassSQ_NhanVien = function (page) {
        return (page.SoTrang === self.currentPageSQ_NhanVien()) ? "click" : "";
    };
    self.selecPageSQ_NhanVien = function () {
        if (AllPageSQ_NhanVien > 4) {
            for (var i = 3; i < AllPageSQ_NhanVien; i++) {
                self.PagesSQ_NhanVien.pop(i + 1);
            }
            self.PagesSQ_NhanVien.push({ SoTrang: 4 });
            self.PagesSQ_NhanVien.push({ SoTrang: 5 });
        }
        else {
            for (var i = 0; i < 6; i++) {
                self.PagesSQ_NhanVien.pop(i);
            }
            for (var j = 0; j < AllPageSQ_NhanVien; j++) {
                self.PagesSQ_NhanVien.push({ SoTrang: j + 1 });
            }
        }
    }
    self.ReserPageSQ_NhanVien = function (item) {
        if (nextPageSQ_NhanVien > 1 && AllPageSQ_NhanVien > 5) {
            if (nextPageSQ_NhanVien > 3 && nextPageSQ_NhanVien < AllPageSQ_NhanVien - 1) {
                for (var i = 0; i < 5; i++) {
                    self.PagesSQ_NhanVien.replace(self.PagesSQ_NhanVien()[i], { SoTrang: parseInt(nextPageSQ_NhanVien) + i - 2 });
                }
            }
            else if (parseInt(nextPageSQ_NhanVien) === parseInt(AllPageSQ_NhanVien) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.PagesSQ_NhanVien.replace(self.PagesSQ_NhanVien()[i], { SoTrang: parseInt(nextPageSQ_NhanVien) + i - 3 });
                }
            }
            else if (nextPageSQ_NhanVien == AllPageSQ_NhanVien) {
                for (var i = 0; i < 5; i++) {
                    self.PagesSQ_NhanVien.replace(self.PagesSQ_NhanVien()[i], { SoTrang: parseInt(nextPageSQ_NhanVien) + i - 4 });
                }
            }
        }
        else if (nextPageSQ_NhanVien == 1) {
            for (var i = 0; i < 5; i++) {
                self.PagesSQ_NhanVien.replace(self.PagesSQ_NhanVien()[i], { SoTrang: parseInt(nextPageSQ_NhanVien) + i });
            }
        }
        self.currentPageSQ_NhanVien(parseInt(nextPageSQ_NhanVien));
    }
    self.NextPageSQ_NhanVien = function (item) {
        if (nextPageSQ_NhanVien < AllPageSQ_NhanVien) {
            nextPageSQ_NhanVien = nextPageSQ_NhanVien + 1;
            pageNum = nextPageSQ_NhanVien;
            self.ReserPageSQ_NhanVien();
            self.gotoNextPageSQ_NhanVien();
        }
    };
    self.BackPageSQ_NhanVien = function (item) {
        if (nextPageSQ_NhanVien > 1) {
            nextPageSQ_NhanVien = nextPageSQ_NhanVien - 1;
            pageNum = nextPageSQ_NhanVien;
            self.ReserPageSQ_NhanVien();
            self.gotoNextPageSQ_NhanVien();
        }
    };
    self.EndPageSQ_NhanVien = function (item) {
        nextPageSQ_NhanVien = AllPageSQ_NhanVien;
        pageNum = nextPageSQ_NhanVien;
        self.ReserPageSQ_NhanVien();
        self.gotoNextPageSQ_NhanVien();
    };
    self.StartPageSQ_NhanVien = function (item) {
        nextPageSQ_NhanVien = 1;
        pageNum = nextPageSQ_NhanVien;
        self.gotoNextPageSQ_NhanVien();
        self.ReserPageSQ_NhanVien();
    };
    self.gotoNextPageSQ_NhanVien = function (item) {
        try {
            var numbepage = item.SoTrang;
            pageNum = numbepage;
            nextPageSQ_NhanVien = numbepage;
        }
        catch (e) {

        }
        ajaxHelper(NhanVienUri + "GetListQuyHoaDons?ID_NhanVien=" + _id_Nhanvien + "&pageSize=" + pageSize + "&pageNum=" + pageNum, "GET").done(function (data) {
            self.SQ_NhanViens(data.LstData);
            self.RowsStartSQ_NhanVien((pageNum - 1) * pageSize + 1);
            self.RowsEndSQ_NhanVien((pageNum - 1) * pageSize + self.SQ_NhanViens().length)
            self.ReserPageSQ_NhanVien();
        });
    }
    self.loadSQ_NhanVien = function (item) {
        ajaxHelper(NhanVienUri + "GetListQuyHoaDons?ID_NhanVien=" + item.ID + "&pageSize=" + pageSize + "&pageNum=" + pageNum, "GET").done(function (data) {
            self.SQ_NhanViens(data.LstData);
            self.RowsSQ_NhanVien(data.Rowcount);

            if (self.SQ_NhanViens() != null) {
                self.RowsStartSQ_NhanVien((pageNum - 1) * pageSize + 1);
                self.RowsEndSQ_NhanVien((pageNum - 1) * pageSize + self.SQ_NhanViens().length)
            }
            else {
                self.RowsStartSQ_NhanVien('0');
                self.RowsEndSQ_NhanVien('0');
            }
            self.PagesSQ_NhanVien(data.LstPageNumber);
            AllPageSQ_NhanVien = self.PagesSQ_NhanVien().length;
            self.selecPageSQ_NhanVien();
        });
    }
    self.getAllNhanViens = function () {
        ajaxHelper(NhanVienUri + "GetListNhanViens", 'GET').done(function (data) {
            self.NhanViens(data);
            $('.collumn-filter input[type="checkbox"]').each(function () {
                let target = ($(this).val());
                if ($(this).is(':checked') == true) {
                    $('.' + target).show();
                } else {
                    $('.' + target).hide();
                }
            });
            LoadHtmlGrid();
            $('.ss-li').css("background", "white");
            $('#tatcanhh a').css("display", "block");
            $('#tatcanhh').css("background", "#e6f5d6");
        });
    }

    function locdau(obj) {
        if (!obj)
            return "";
        var str = obj;
        str = str.toString().toLowerCase();
        str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
        str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
        str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
        str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
        str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
        str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
        str = str.replace(/đ/g, "d");
        str = str.replace(/^\-+|\-+$/g, "");
        return str;
    }

    //phân trang
    self.pageSizes = [10, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();
    self.arrPagging = ko.observableArray();

    self.PageCount = ko.computed(function () {
        if (self.pageSize()) {
            if (self.arrPagging() !== null) {
                return Math.ceil(self.arrPagging().length / self.pageSize());
            }
            else {
                return 0
            }
        }
        else {
            return 1;
        }
    });

    self.filteredNhanVien = ko.computed(function () {
        var first = self.currentPage() * self.pageSize();
        var _filter = self.filter();
        var arrFilter = '';
        var _daNghiViec = self.loc_DaNghiViec();
        arrFilter = ko.utils.arrayFilter(self.NhanViens(), function (prod) {
            var chon = true;
            if (_daNghiViec === "1") {
                chon = prod.DaNghiViec === true;
            }
            else if (_daNghiViec === "0") {

                chon = prod.DaNghiViec === false;
            }
            var arr = locdau(prod.TenNhanVien).toLowerCase().split(/\s+/);
            var sSearch = '';
            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }
            if (chon && _filter) {
                chon = (locdau(prod.MaNhanVien).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    locdau(prod.TenNhanVien).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    locdau(prod.DienThoaiDiDong).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    locdau(prod.Email).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    locdau(prod.DaNghiViec).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    sSearch.indexOf(locdau(_filter).toLowerCase()) >= 0
                );
            }
            return chon;
        });
        if (arrFilter !== null) {
            self.arrPagging(arrFilter);

            self.fromitem((self.currentPage() * self.pageSize()) + 1);

            if (((self.currentPage() + 1) * self.pageSize()) > arrFilter.length) {
                self.toitem(arrFilter.length)
            } else {
                self.toitem(first + self.pageSize());
            }

            if (self.PageCount() > 1) {
                if (self.currentPage() === self.PageCount()) {
                    self.currentPage(0);
                }
                return arrFilter.slice(first, first + self.pageSize());
            }
            else {
                return arrFilter;
            }
        }
    });

    self.PageResults = ko.computed(function () {
        if (self.NhanViens() !== null) {
            self.fromitem((self.currentPage() * self.pageSize()) + 1);
            if (((self.currentPage() + 1) * self.pageSize()) > self.NhanViens().length) {
                self.toitem(self.NhanViens().length)
            } else {
                self.toitem((self.currentPage() * self.pageSize()) + self.pageSize());
            }

            if (self.PageCount() > 1) {
                return self.NhanViens().slice(self.currentPage() * self.pageSize(), (self.currentPage() * self.pageSize()) + self.pageSize())
            }
            else {
                return self.NhanViens();
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

        pageAllSize = self.pageSize();
        pageAllNum = 1;
        self.currentPageAll_NhanVien(pageAllNum);
        nextPageAll_NhanVien = 1;
        getAllNhanViens();
        //self.currentPage(1);
    };

    self.GoToPage = function (page) {
        self.currentPage(page);
    };

    self.GetClass = function (page) {
        return (page === self.currentPage()) ? "click" : "";
    };

    self.headers = [
        { title: 'Mã nhân viên', link: '#', name: 'm0', nameid: 'txtmanhanvien', sortPropertyName: 'MaNhanVien', asc: true, arrowDown: false, arrowUp: false },
        { title: 'Tên nhân viên', link: '#', name: 'm1', nameid: 'txttennhanvien', sortPropertyName: 'TenNhanVien', asc: true, arrowDown: false, arrowUp: false },
        { title: 'Điện thoại', link: '#', name: 'm2', nameid: 'txtdienthoai', sortPropertyName: 'DienThoaiDiDong', asc: true, arrowDown: true, arrowUp: false },
        { title: 'Email', link: '#', name: 'm3', nameid: 'txtemail', sortPropertyName: 'Email', asc: true, arrowDown: true, arrowUp: false },
        { title: 'Trạng thái', link: '#', name: 'm4', nameid: 'txtdanghiviec', sortPropertyName: 'DaNghiViec', asc: true, arrowDown: true, arrowUp: false }

    ];
    self.activeSort = self.headers[0];

    self.sort = function (header, event) {
        if (self.activeSort === header) {
            header.asc = !header.asc;
            header.arrowDown = !header.arrowDown;
            header.arrowUp = !header.arrowUp;
        } else {
            self.activeSort.arrowDown = false;
            self.activeSort.arrowUp = false;
            self.activeSort = header;
            header.arrowDown = true;
        }
        if (header.arrowDown == true) {
            $('th a[id^=txt]').each(function () {
                $(this).html($(this).text())
            });
            $('#' + header.nameid).html(header.title);
            $('#' + header.nameid).append(' ' + '<i class="fa fa-caret-down" aria-hidden="true"></i>');
        } else {
            $('th[id^=txt]').each(function () {
                $(this).html($(this).text())
            });
            $('#' + header.nameid).html(header.title);
            $('#' + header.nameid).append(' ' + '<i class="fa fa-caret-up" aria-hidden="true"></i>');
        }
        var prop = header.sortPropertyName;
        var ascSort = function (a, b) {
            if (typeof a[prop] === "number" || typeof a[prop] === "boolean") {
                return a[prop] < b[prop] ? -1 : a[prop] > b[prop] ? 1 : a[prop] == b[prop] ? 0 : 0;
            }
            else {
                if (a[prop] === null || a[prop] === undefined || b[prop] === null || b[prop] === undefined) {
                    if (a[prop] === null || b[prop] === null) {
                        // compare(null, string)= -1, compare(string, null)= 1
                        return (a[prop] === null && b[prop] !== null) ? -1 : (a[prop] !== null && b[prop] === null) ? 1 : 0;
                    }
                    else {
                        return a[prop] < b[prop] ? -1 : a[prop] > b[prop] ? 1 : a[prop] == b[prop] ? 0 : 0;
                    }
                }
                else {
                    return locdau(a[prop]).toLowerCase() < locdau(b[prop]).toLowerCase() ? -1 : locdau(a[prop]).toLowerCase() > locdau(b[prop]).toLowerCase() ? 1 : locdau(a[prop]).toLowerCase() == locdau(b[prop]).toLowerCase() ? 0 : 0;
                }
            }
        };
        var descSort = function (a, b) {
            if (typeof a[prop] === "number" || typeof a[prop] === "boolean") {
                return a[prop] > b[prop] ? -1 : a[prop] < b[prop] ? 1 : a[prop] == b[prop] ? 0 : 0;
            }
            else {
                if (a[prop] !== null && a[prop] !== undefined && b[prop] !== null && b[prop] !== undefined) {
                    return locdau(a[prop]).toLowerCase() > locdau(b[prop]).toLowerCase() ? -1 : locdau(a[prop]).toLowerCase() < locdau(b[prop]).toLowerCase() ? 1 : locdau(a[prop]).toLowerCase() == locdau(b[prop]).toLowerCase() ? 0 : 0;
                }
                else {
                    if (a[prop] === null || b[prop] === null) {
                        // compare(null, string)= 1, compare(string, null)= -1
                        return (a[prop] === null && b[prop] !== null) ? 1 : (a[prop] !== null && b[prop] === null) ? -1 : 0;
                    }
                    else {
                        return a[prop] > b[prop] ? -1 : a[prop] < b[prop] ? 1 : a[prop] == b[prop] ? 0 : 0;
                    }
                }
            }
        };
        var sortFunc = header.asc ? ascSort : descSort;
        self.NhanViens.sort(sortFunc);
    };
    // thêm chi nhánh
    self.DonVis = ko.observableArray();
    self.MangChiNhanhNhanVien = ko.observableArray();
    self.SelectedAllDonVi = function () {
        //$('#choose_DonVi input').remove();
        self.MangChiNhanhNhanVien(self.DonVis());
        for (var i = 0; i < self.MangChiNhanhNhanVien().length; i++) {
            $('#selec-all-DonVi li').each(function () {
                if ($(this).attr('id') === self.MangChiNhanhNhanVien()[i].ID) {
                    $(this).find('.fa-check').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>')
                }
            });
        }
    }
    self.SelectedDonVi = function (item) {
        var arrIDDonVi = [];
        for (var i = 0; i < self.MangChiNhanhNhanVien().length; i++) {
            if ($.inArray(self.MangChiNhanhNhanVien()[i], arrIDDonVi) === -1) {
                arrIDDonVi.push(self.MangChiNhanhNhanVien()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDDonVi) === -1) {
            self.MangChiNhanhNhanVien.push(item);
        }
        $('#selec-all-DonVi li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });
    }
    self.CloseDonVi = function (item) {
        self.MangChiNhanhNhanVien.remove(item);
        if (self.MangChiNhanhNhanVien().length === 0) {
            //$('#choose_DonVi').append('<input type="text" class="dropdown" placeholder="Chọn chi nhánh...">');
        }
        // remove check
        $('#selec-all-DonVi li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }
    // lấy về danh sách đơn vị
    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/';
    function getDonVi() {
        ajaxHelper(BH_DonViUri + "GetListDonVi1", "GET").done(function (data) {
            self.DonVis(data);
            vmNsPhongBan.listChiNhanh = self.DonVis();
        });
    }
    getDonVi();
    //===============================
    // Load lai form lưu cache bộ lọc 
    // trên grid 
    //===============================
    function LoadHtmlGrid() {
        if (window.localStorage) {
            var current = localStorage.getItem('NhanvienList');
            if (!current) {
                current = [];
                localStorage.setItem('NhanvienList', JSON.stringify(current));
            } else {
                current = JSON.parse(current);
                for (var i = 0; i < current.length; i++) {
                    updateColspan(current[i].index, -1);
                    $('.' + current[i].NameClass).hide();
                    document.querySelector(`.dropdown-list input[value=${current[i].NameClass}]`).checked = false;
                }
            }
        }
    }
    //===============================
    // Add Các tham số cần lưu lại để 
    // cache khi load lại form
    //===============================
    function updateColspan(colIndex, number) {
        var colGroup = '';
        if (colIndex < 14) {
            colGroup = 'thongtinchung';
        } else if (colIndex < 18) {
            colGroup = 'thong-tin-thuong-tru';
        } else if (colIndex < 22) {
            colGroup = 'thong-tin-tam-tru';
        } else {
            colGroup = 'thong-tin-cong-viec';
        }
        var oldColspan = $('.' + colGroup).attr('colspan') * 1;
        var newColspan = oldColspan + number;
        $('.' + colGroup).attr('colspan', `${newColspan}`);
        if (newColspan === 0) {
            $('.' + colGroup).hide();
        } else {
            $('.' + colGroup).show();
        }
    }

    function addClass(inputValue, colIndex) {
        var current = localStorage.getItem('NhanvienList');
        if (!current) {
            current = [];
        } else {
            current = JSON.parse(current);
        }
        if (current.length > 0) {
            for (var i = 0; i < current.length; i++) {
                if (current[i].NameClass === inputValue) {
                    updateColspan(colIndex, 1);
                    current.splice(i, 1);
                    break;
                }
                if (i == current.length - 1) {
                    updateColspan(colIndex, -1);
                    current.push({
                        NameClass: inputValue,
                        index: colIndex,
                    });
                    break;
                }
            }
        }
        else {
            updateColspan(colIndex, -1);
            current.push({
                NameClass: inputValue,
                index: colIndex,
            });
        }
        localStorage.setItem('NhanvienList', JSON.stringify(current));
    }

    $('.collumn-filter input[type="checkbox"]').click(function () {
        let inputValue = ($(this).val());
        const inputIndex = $('.collumn-filter input[type="checkbox"]').index($(this));
        const colIndex = inputIndex + 2;//cộng với cột mã nhân viên, tên nhân viên
        $('.' + inputValue).toggle();
        addClass(inputValue, colIndex);
    });


    //============================
    //  Filter
    //============================
    self.ListBaoHiem_filter = ko.observableArray();
    self.ListChinhTri_filter = ko.observableArray();
    self.ListLoaiHopDong_filter = ko.observableArray();
    self.ListDanToc_filter = ko.observableArray();
    self.startDate = ko.observable();
    self.endDate = ko.observable();
    self.TextBirthDate = ko.observable("Toàn thời gian");
    self.TypeBirthDate = ko.observable(0);
    self.IsGioiTinh = ko.observable();
    self.ListBaoHiem_filter_computed = ko.computed(function () {
        return self.ListBaoHiem_filter().filter(o => o.IsSelected === true);
    });
    self.ListChinhTri_filter_computed = ko.computed(function () {
        return self.ListChinhTri_filter().filter(o => o.IsSelected === true);
    });
    self.ListLoaiHopDong_filter_computed = ko.computed(function () {
        return self.ListLoaiHopDong_filter().filter(o => o.IsSelected === true);
    });
    self.ListDanToc_filter_computed = ko.computed(function () {
        return self.ListDanToc_filter().filter(o => o.IsSelected === true);
    });
    function loadFilterOpenHRM() {
        $.getJSON(NhanVienUri + "GetBaoHienFilter", function (data) {
            self.ListBaoHiem_filter(data);
        });
        $.getJSON(NhanVienUri + "GetChinhTriFilter", function (data) {
            self.ListChinhTri_filter(data);
        });
        $.getJSON(NhanVienUri + "GetLOaiHopDongFilter", function (data) {
            self.ListLoaiHopDong_filter(data);
        });
        $.getJSON(NhanVienUri + "GetDanTocFilter", function (data) {
            self.ListDanToc_filter(data);
        });
    }
    if ($('#IsOpenHRM').val() === 'True') {
        loadFilterOpenHRM();
    }
    $('.newDateTime').on('apply.daterangepicker', function (ev, picker) {
        self.startDate(picker.startDate.format('MM/DD/YYYY'));
        self.endDate(picker.endDate.format('MM/DD/YYYY'));
        getAllNhanViens(true);
    });
    $('#SinhNhatSL ').on('click', '.radio-menu input[type="radio"]', function () {
        var dataid = $(this).data('id');
        $('#SinhNhatSL .form-group').each(function () {
            $(this).find('.conten-choose').find('input').removeAttr('disabled');
            if (dataid !== $(this).find('.radio-menu').find('input').data('id')) {
                $(this).find('.conten-choose').find('input').attr('disabled', 'disabled');
            }
        });
        if ($(this).data('id') !== 1) {
            self.TypeBirthDate(null);
        }
        else if (self.TypeBirthDate() === null || self.TypeBirthDate() === undefined) {
            self.TypeBirthDate(0);
        }
        getAllNhanViens(true);
    });
    $('#SelectNgaySinh').on('click', 'ul li', function () {
        self.TextBirthDate($(this).find('a').text());
        self.TypeBirthDate($(this).val());
        getAllNhanViens(true);
    })
    $('#GioiTinhSL').on('click', 'ul li', function () {
        if (self.IsGioiTinh() != $(this).find('a').data('id') || self.IsGioiTinh() === undefined || $(this).find('a').data('id') == undefined) {
            self.IsGioiTinh($(this).find('a').data('id'));
            getAllNhanViens(true);
        }
    });
    self.SelectedLoaiBaoHiem = function (model) {
        var result = self.ListBaoHiem_filter().filter(o => o.ID === model.ID);
        if (result != null && result.length > 0) {
            result[0].IsSelected = true;
        }
        var data = self.ListBaoHiem_filter().filter(function (item) {
            return item.ID !== model.ID;
        });
        data.push(result[0])
        self.ListBaoHiem_filter(data);
        getAllNhanViens(true);
    }
    self.RemoveLOaiBaoHien_FIlter = function (model) {
        var result = self.ListBaoHiem_filter().filter(o => o.ID === model.ID);
        if (result != null && result.length > 0) {
            result[0].IsSelected = false;
        }
        var data = self.ListBaoHiem_filter().filter(function (item) {
            return item.ID !== model.ID;
        });
        data.push(result[0])
        self.ListBaoHiem_filter(data);
        getAllNhanViens(true);
    }

    self.RemoveChinhTri_FIlter = function (model) {
        var result = self.ListChinhTri_filter().filter(o => o.ID === model.ID);
        if (result != null && result.length > 0) {
            result[0].IsSelected = false;
        }
        var data = self.ListChinhTri_filter().filter(function (item) {
            return item.ID !== model.ID;
        });
        data.push(result[0])
        self.ListChinhTri_filter(data);
        getAllNhanViens(true);
    }
    self.SelectedChinhTri = function (model) {
        var result = self.ListChinhTri_filter().filter(o => o.ID === model.ID);
        if (result != null && result.length > 0) {
            result[0].IsSelected = true;
        }
        var data = self.ListChinhTri_filter().filter(function (item) {
            return item.ID !== model.ID;
        });
        data.push(result[0])
        self.ListChinhTri_filter(data);
        getAllNhanViens(true);
    }

    self.RemoveLoaiHopDong_FIlter = function (model) {
        var result = self.ListLoaiHopDong_filter().filter(o => o.ID === model.ID);
        if (result != null && result.length > 0) {
            result[0].IsSelected = false;
        }
        var data = self.ListLoaiHopDong_filter().filter(function (item) {
            return item.ID !== model.ID;
        });
        data.push(result[0])
        self.ListLoaiHopDong_filter(data);
        getAllNhanViens(true);
    }
    self.SelectedLoaiHopDong = function (model) {
        var result = self.ListLoaiHopDong_filter().filter(o => o.ID === model.ID);
        if (result != null && result.length > 0) {
            result[0].IsSelected = true;
        }
        var data = self.ListLoaiHopDong_filter().filter(function (item) {
            return item.ID !== model.ID;
        });
        data.push(result[0])
        self.ListLoaiHopDong_filter(data);
        getAllNhanViens(true);
    }

    self.RemoveDanToc_FIlter = function (model) {
        var result = self.ListDanToc_filter().filter(o => o.ID === model.ID);
        if (result != null && result.length > 0) {
            result[0].IsSelected = false;
        }
        var data = self.ListDanToc_filter().filter(function (item) {
            return item.ID !== model.ID;
        });
        data.push(result[0])
        self.ListDanToc_filter(data);
        getAllNhanViens(true);
    }
    self.SelectedLoaiDanToc = function (model) {
        var result = self.ListDanToc_filter().filter(o => o.ID === model.ID);
        if (result != null && result.length > 0) {
            result[0].IsSelected = true;
        }
        var data = self.ListDanToc_filter().filter(function (item) {
            return item.ID !== model.ID;
        });
        data.push(result[0])
        self.ListDanToc_filter(data);
        getAllNhanViens(true);
    }

    self.HK_TinhThanhFilter = ko.observableArray();
    self.HK_QuanHuyenFilter = ko.observableArray();
    self.HK_XaPhuongFilter = ko.observableArray();
    self.TT_TinhThanhFilter = ko.observableArray();
    self.TT_QuanHuyenFilter = ko.observableArray();
    self.TT_XaPhuongFilter = ko.observableArray();
    $('#hk_tinhthanh_filter').on('change', function () {
        getAllNhanViens(true);
        $.getJSON(NhanVienUri + "GetQuanHuyen?tinhthanhID=" + $(this).val(), function (data) {
            self.HK_QuanHuyenFilter(data);
        });
    });
    $('#hk_quanhuyen_filter').on('change', function () {
        getAllNhanViens(true);
        $.getJSON(NhanVienUri + "GetXaPhuong?quanhuyenID=" + $(this).val(), function (data) {
            self.HK_XaPhuongFilter(data);
        });
    });

    $('#tt_tinhthanh_filter').on('change', function () {
        getAllNhanViens(true);
        $.getJSON(NhanVienUri + "GetQuanHuyen?tinhthanhID=" + $(this).val(), function (data) {
            self.TT_QuanHuyenFilter(data);
        });
    });
    $('#tt_quanhuyen_filter').on('change', function () {
        getAllNhanViens(true);
        $.getJSON(NhanVienUri + "GetXaPhuong?quanhuyenID=" + $(this).val(), function (data) {
            self.TT_XaPhuongFilter(data);
        });
    });
    // -- end filter



    //============================
    //  Add new -TuanDl
    //============================
    self.news_Idnhanvien = ko.observable();
    self.news_MaNhanVien = ko.observable();
    self.news_HoTen = ko.observable();
    self.news_GioiTinh = ko.observable(true);
    self.news_DiDong = ko.observable();
    self.news_SDT = ko.observable();
    self.news_Email = ko.observable();
    self.news_NoiSinh = ko.observable();
    self.news_SoCMTND = ko.observable();
    self.news_CMNDNoiCap = ko.observable();
    self.news_DanToc = ko.observable();
    self.news_TonGiao = ko.observable();
    self.news_GhiChu = ko.observable();
    self.news_HKTinhThanh = ko.observable();
    self.news_HKQuanHuyen = ko.observable();
    self.news_HKXaPhuong = ko.observable();
    self.news_HKDiaChi = ko.observable();
    self.news_NOTinhThanh = ko.observable();
    self.news_NOQuanHuyen = ko.observable();
    self.news_NOXaPhuong = ko.observable();
    self.news_NODiaChi = ko.observable();
    self.news_TrangThai = ko.observable(false);
    self.ListTinhThanh = ko.observableArray();
    self.ListTinhThanhHK = ko.observableArray();
    self.ListPhongBan = ko.observableArray();
    self.ListQuocTich = ko.observableArray();
    self.HKListQuanHuyen = ko.observableArray();
    self.HKListXaPhuong = ko.observableArray();
    self.NOListQuanHuyen = ko.observableArray();
    self.NOListXaPhuong = ko.observableArray();
    self.News_IdNhanVienOld = ko.observable(null);
    self.tenPBSelected = ko.observable();
    self.SelectedPBId = ko.observable(null);
    self.visibleCMND = ko.computed(function () {
        var result = !commonStatisJs.CheckNull(self.news_SoCMTND()) && (self.news_SoCMTND().length >= 8);
        if (!commonStatisJs.CheckNull(self.news_SoCMTND()) && !result) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Số CMND không đúng", "danger");
        }
        return result;
    });
    self.backtoform = function () {
        OnloadForm();
        changeButtonPopup();
    }
    function setUpdateStaff(model) {
        self.News_IdNhanVienOld(null);
        self.news_Idnhanvien(model.ID);
        self.news_MaNhanVien(model.MaNhanVien);
        self.news_HoTen(model.TenNhanVien);
        self.news_GioiTinh(model.GioiTinh);
        self.news_DiDong(model.DienThoaiDiDong);
        self.news_SDT(model.DienThoaiNhaRieng);
        self.news_Email(model.Email);
        self.news_NoiSinh(model.NoiSinh);
        self.news_SoCMTND(model.SoCMND);
        self.news_CMNDNoiCap(model.NoiCap);
        self.news_TonGiao(model.TonGiao);
        self.news_GhiChu(model.GhiChu);
        $('#hk_tinhthanh').val(model.ID_TinhThanhHKTT);
        $('#no_tinhthanh').val(model.ID_TinhThanhTT);
        $('#hk_quanhuyen').val(model.ID_QuanHuyenHKTT);
        $('#hk_xaphuong').val(model.ID_XaPhuongHKTT);
        $('#no_quanhuyen').val(model.ID_QuanHuyenTT);
        $('#no_xaphuong').val(model.ID_XaPhuongTT);
        $('#News_ngayvaolamviec').val(convertDateTime(model.NgayVaoLamViec));
        $('#News_ngaysinh').val(convertDateTime(model.NgaySinh));
        $('#News_ngaycap').val(convertDateTime(model.NgayCap));
        self.news_HKDiaChi(model.NguyenQuan);
        self.news_NODiaChi(model.ThuongTru);
        self.news_TrangThai(model.DaNghiViec);
        self.SelectedPBId(model.ID_NSPhongBan);
        self.tenPBSelected(model.TenPhongBan);
        //$('#IsFamily').val(model.TinhTrangHonNhan);
        if (model.Image !== "") {
            $('#blah').attr('src', Open24FileManager.hostUrl + model.Image);
        }
        $('#ns_dantoc').val(model.DanTocTonGiao);
        $('#IsFamily label input[type="radio"]').each(function () {
            this.checked = false;
            if ($(this).closest('label').data('id') === model.TinhTrangHonNhan) {
                this.checked = true;
            }

        });
    }
    $('.selectPhongBan').on('click', 'li div', function () {
        self.SelectedPBId($(this).closest('li').attr('id'));
        self.tenPBSelected($(this).text());
    });
    function OnloadForm() {
        self.news_Idnhanvien(null);
        self.news_MaNhanVien("");
        self.news_HoTen("");
        self.news_DiDong("");
        self.news_SDT("");
        self.news_Email("");
        self.news_NoiSinh("");
        self.news_SoCMTND("");
        self.news_DanToc("");
        self.news_TonGiao("");
        self.news_GhiChu("");
        $('#hk_tinhthanh').val('');
        $('#no_tinhthanh').val('');
        $('#hk_quanhuyen').val('');
        $('#hk_xaphuong').val('');
        $('#no_quanhuyen').val('');
        $('#no_xaphuong').val('');
        self.SelectedPBId(null);
        self.tenPBSelected("");
        $('#IsFamily').val(null);
        $('#News_ngayvaolamviec').val(null);
        $('#News_ngaysinh').val(null);
        $('#News_ngaycap').val(null);
        $('#ns_dantoc').val('');
        self.news_HKDiaChi("");
        self.news_NODiaChi("");
        self.MangChiNhanhNhanVien([]);
        $('#blah').attr('src', "/Content/images/photo.png");
        $('#selec-all-DonVi li').each(function () {
            $(this).find('.fa-check').remove();

        });
        self.News_IdNhanVienOld(null);
    }
    OnloadForm();

    function SaveAgain() {
        self.TieuDeThemNhanVien('Thêm mới nhân viên');
        self.news_Idnhanvien(null);
        self.news_MaNhanVien("");
        self.news_HoTen("");
    }
    self.ListAddNewStaffUnit = ko.observableArray();

    $.getJSON(NhanVienUri + "GetListAddNewStaffUnit", function (data) {
        self.ListAddNewStaffUnit(data);
    });
    $.getJSON(NhanVienUri + "GetAllTinhThanh", function (data) {
        self.ListTinhThanh(data);
        self.HK_TinhThanhFilter(data);
        self.TT_TinhThanhFilter(data);
    });
    $.getJSON(NhanVienUri + "GetAllPhongBan", function (data) {
        self.ListPhongBan(data);
    });
    $.getJSON(NhanVienUri + "GetQuocGia", function (data) {
        self.ListQuocTich(data);
    });
    $('#selectQuocTich').on('change', function () {
        $.getJSON(NhanVienUri + "GetTinhThanhforKey?quocgiaId=" + $(this).val(), function (data) {
            self.ListTinhThanhHK(data);
        });
    });
    $('#hk_tinhthanh').on('change', function () {
        self.news_HKTinhThanh($(this).val());
        $.getJSON(NhanVienUri + "GetQuanHuyen?tinhthanhID=" + $(this).val(), function (data) {
            self.HKListQuanHuyen(data);
        });
    });
    $('#hk_quanhuyen').on('change', function () {
        self.news_HKQuanHuyen($(this).val());
        $.getJSON(NhanVienUri + "GetXaPhuong?quanhuyenID=" + $(this).val(), function (data) {
            self.HKListXaPhuong(data);
        });
    });
    $('#hk_xaphuong').on('change', function () {
        self.news_HKXaPhuong($(this).val());

    });
    $('#no_tinhthanh').on('change', function () {
        self.news_NOTinhThanh($(this).val());
        $.getJSON(NhanVienUri + "GetQuanHuyen?tinhthanhID=" + $(this).val(), function (data) {
            self.NOListQuanHuyen(data);
        });
    });
    $('#no_quanhuyen').on('change', function () {
        self.news_NOQuanHuyen($(this).val());
        $.getJSON(NhanVienUri + "GetXaPhuong?quanhuyenID=" + $(this).val(), function (data) {
            self.NOListXaPhuong(data);
        });
    });
    $('#no_xaphuong').on('change', function () {
        self.news_NOXaPhuong($(this).val());

    });
    $('#dm_manhanvien').focusout(function () {
        self.news_MaNhanVien(commonStatisJs.convertVarchar($(this).val()));
    });
    $('#dm_manhanvien').on('change', function () {
        self.news_MaNhanVien(commonStatisJs.convertVarchar($(this).val()));
    });

    self.SaveStaff = function () {
        createStaff(false);
    }

    self.SaveAndNewStaff = function () {
        createStaff(true);
    }

    function createStaff(IsNew) {
        self.news_MaNhanVien(commonStatisJs.convertVarchar(self.news_MaNhanVien()));
        if (commonStatisJs.CheckNull(self.news_HoTen())) {
            ShowMessage_Danger("Vui lòng nhập họ tên nhân viên");
            return;
        }
        if (!commonStatisJs.CheckNull(self.news_Email()) && !commonStatisJs.CheckEmail(self.news_Email())) {
            ShowMessage_Danger("Email không hợp lệ vui lòng kiểm tra lại");
            return;
        }
        if (!commonStatisJs.CheckNull(self.news_DiDong()) && !commonStatisJs.CheckPhoneNumber(self.news_DiDong())) {
            ShowMessage_Danger("Di động không hợp lệ");
            return;
        }
        if (!commonStatisJs.CheckNull(self.news_SDT()) && !commonStatisJs.CheckPhoneNumber(self.news_SDT())) {
            ShowMessage_Danger("Số điện thoại không hợp lệ");
            return;
        }
        if (!commonStatisJs.CheckNull(self.news_SoCMTND()) && self.news_SoCMTND().length < 8) {
            ShowMessage_Danger("Số CMND không hợp lệ");
            return;
        }
        if (self.listAddChiNhanh().some(o => o.ID_ChiNhanh === null)) {
            ShowMessage_Danger("Vui lòng chọn chi nhánh, phòng ban");
            return;
        }

        if (self.news_TrangThai() && self.news_Idnhanvien() === self.ID_NhanVienAdmin()) {
            ShowMessage_Danger("Nhân viên là 'Admin'. Vui lòng không cho nghỉ việc");
            return;
        }

        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadForm").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadForm").files[i];
            formData.append("files", file);
        }
        var keyFamily;
        $('#IsFamily label input[type="radio"]').each(function () {
            if (this.checked) {
                keyFamily = $(this).closest('label').data('id');
            }

        });
        var Nhan_Vien = {
            ID: self.news_Idnhanvien(),
            MaNhanVien: commonStatisJs.convertStrFormC(self.news_MaNhanVien()),
            TenNhanVien: commonStatisJs.convertStrFormC(self.news_HoTen()),
            NgaySinh: commonStatisJs.convertDateToServer($('#News_ngaysinh').val()),
            GioiTinh: self.news_GioiTinh(),
            DienThoaiDiDong: commonStatisJs.convertStrFormC(self.news_DiDong()),
            DienThoaiNhaRieng: commonStatisJs.convertStrFormC(self.news_SDT()),
            Email: commonStatisJs.convertStrFormC(self.news_Email()),
            SoCMND: commonStatisJs.convertStrFormC(self.news_SoCMTND()),
            NgayCap: commonStatisJs.convertDateToServer($('#News_ngaycap').val()),
            TonGiao: commonStatisJs.convertStrFormC(self.news_TonGiao()),
            GhiChu: commonStatisJs.convertStrFormC(self.news_GhiChu()),
            ID_TinhThanhHKTT: self.news_HKTinhThanh(),
            ID_QuanHuyenHKTT: self.news_HKQuanHuyen(),
            ID_XaPhuongHKTT: self.news_HKXaPhuong(),
            ID_TinhThanhTT: self.news_NOTinhThanh(),
            ID_QuanHuyenTT: self.news_NOQuanHuyen(),
            ID_XaPhuongTT: self.news_NOXaPhuong(),
            ThuongTru: self.news_NODiaChi(),
            NguyenQuan: self.news_HKDiaChi(),
            NguoiTao: $('#txtTenTaiKhoan').text(),
            NgayVaoLamViec: commonStatisJs.convertDateToServer($('#News_ngayvaolamviec').val()),
            ID_NSPhongBan: self.SelectedPBId(),
            DaNghiViec: self.news_TrangThai(),
            NoiCap: self.news_CMNDNoiCap(),
            DanTocTonGiao: $('#ns_dantoc').val(),
            ID_QuocGia: $('#selectQuocTich').val(),
            TinhTrangHonNhan: keyFamily,
            NoiSinh: commonStatisJs.convertStrFormC(self.news_NoiSinh()),
            DiaChiHKTT: self.news_HKDiaChi(),
            DiaChiTT: self.news_NODiaChi()
        };
        var quatrinhct = [];
        for (var i = 0; i < self.listAddChiNhanh().length; i++) {

            if (!quatrinhct.some(o => o.ID_ChiNhanh === self.listAddChiNhanh()[i].ID_ChiNhanh
                && o.ID_PhongBan === self.listAddChiNhanh()[i].ID_PhongBan) || self.listAddChiNhanh()[i].LaMacDinh === true) {
                quatrinhct.push({
                    ID_ChiNhanh: self.listAddChiNhanh()[i].ID_ChiNhanh,
                    ID_PhongBan: self.listAddChiNhanh()[i].ID_PhongBan,
                    LaMacDinh: self.listAddChiNhanh()[i].LaMacDinh,
                });
            }

        }
        var model = {
            nhanvien: Nhan_Vien,
            QuaTrinhCongTac: quatrinhct
        }
        $.ajax({
            data: model,
            url: NhanVienUri + "EditStaff?ID_DonVi=" + _id_DonVi + "&ID_NhanVien=" + _id_NhanVien_LS + "&IdSaveAgain=" + self.News_IdNhanVienOld(),
            type: 'POST',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                if (item.res === true) {
                    self.News_IdNhanVienOld(item.dataSoure);
                    if (totalFiles > 0) {
                        let myData = {};
                        myData.Subdomain = VHeader.SubDomain;
                        myData.Function = "8"; //8. Nhân viên
                        myData.Id = item.dataSoure;
                        myData.files = formData;
                        var result = Open24FileManager.UploadImage(myData);
                        if (result.length > 0) {
                            $.ajax({
                                url: NhanVienUri + "UpdateAnhNhanVien?id=" + item.dataSoure,
                                type: "POST",
                                data: JSON.stringify(result),
                                contentType: "application/json",
                                dataType: "JSON",
                                success: function (data, textStatus, jqXHR) {
                                    if (self.newNhanVien().ImageRemove() !== "")
                                        Open24FileManager.RemoveFiles([self.newNhanVien().ImageRemove()]);
                                    if (self.news_Idnhanvien() == null)
                                        bottomrightnotify("Thêm mới nhân viên thành công!", "success");
                                    else
                                        bottomrightnotify("Cập nhật nhân viên thành công!", "success");

                                    if (IsNew !== true) {
                                        getAllNhanViens(true);
                                        changeButtonPopup();
                                    }
                                    else {
                                        SaveAgain();
                                    }
                                },
                                error: function (jqXHR, textStatus, errorThrown) {

                                }
                            });
                        }
                    }
                    else {
                        if (self.news_Idnhanvien() == null)
                            bottomrightnotify("Thêm mới nhân viên thành công!", "success");
                        else
                            bottomrightnotify("Cập nhật nhân viên thành công!", "success");

                        if (IsNew !== true) {
                            getAllNhanViens(true);
                            changeButtonPopup();
                        }
                        else {
                            SaveAgain();
                        }
                    }
                }
                else {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + item.mess, "danger");
                }
            }
        });
    }

    function changeButtonPopup() {
        if ($('#StaffNew').css('display') !== 'none') {
            $('#BtnAddStaff').html('<i class="fa fa-plus"></i><font>Thêm mới</font>');
            $('#StaffNew').hide();
            $("#Staff").show();

        }
        else {
            $('#BtnAddStaff').html('<i class="fa fa-arrow-left" ></i><font>Quay về</font>');
            $('#StaffNew').show();
            $("#Staff").hide();

        }

    }

    self.selectedNv = ko.observable();
    //===========================================
    //  Tab thông tin chính trị
    //===========================================
    self.news_NgayNhapNgu = ko.observable();
    self.news_NgayVaoDoan = ko.observable();
    self.news_NgayXuatNgu = ko.observable();
    self.news_NgayVaoDang = ko.observable();
    self.news_NgayRaKhoiDang = ko.observable();
    self.news_NgayVaoDangChinhThuc = ko.observable();
    self.SavettChinhTri = function (model) {
        model.NgayXuatNgu = convertDateToServer(self.news_NgayXuatNgu());
        model.NgayNhapNgu = convertDateToServer(self.news_NgayNhapNgu());
        model.NgayVaoDoan = convertDateToServer(self.news_NgayVaoDoan());
        model.NgayVaoDangChinhThuc = convertDateToServer(self.news_NgayVaoDangChinhThuc());
        model.NgayVaoDang = convertDateToServer(self.news_NgayVaoDang());
        model.NgayRoiDang = convertDateToServer(self.news_NgayRaKhoiDang());
        $.ajax({
            data: model,
            url: NhanVienUri + "SaveThongTinChinhtri",
            type: 'POST',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                if (item.res === true) {
                    bottomrightnotify(item.mess, "success");
                }
                else {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + item.mess, "danger");
                }
            }
        });

    }

    self.ShowThongTinCT = function (model) {
        $('.News_ngayxuatngu').datetimepicker({
            timepicker: false,
            mask: true,
            format: 'd/m/Y',
            onChangeDateTime: function (dp, $input) {
                self.news_NgayXuatNgu($input.val());
            }
        });
        $('.News_ngaynhapngu').datetimepicker({
            timepicker: false,
            mask: true,
            format: 'd/m/Y',
            onChangeDateTime: function (dp, $input) {
                self.news_NgayNhapNgu($input.val());
            }
        });
        $('.News_ngayvaodoan').datetimepicker({
            timepicker: false,
            mask: true,
            format: 'd/m/Y',
            onChangeDateTime: function (dp, $input) {
                self.news_NgayVaoDoan($input.val());
            }
        });
        $('.News_ngayvaodang').datetimepicker({
            timepicker: false,
            mask: true,
            format: 'd/m/Y',
            onChangeDateTime: function (dp, $input) {
                self.news_NgayVaoDang($input.val());
            }
        });
        $('.News_ngayvaochinhthuc').datetimepicker({
            timepicker: false,
            mask: true,
            format: 'd/m/Y',
            onChangeDateTime: function (dp, $input) {
                self.news_NgayVaoDangChinhThuc($input.val());
            }
        });
        $('.News_ngayradang').datetimepicker({
            timepicker: false,
            mask: true,
            format: 'd/m/Y',
            onChangeDateTime: function (dp, $input) {
                self.news_NgayRaKhoiDang($input.val());
            }
        });
        self.news_NgayXuatNgu(convertDateTime(model.NgayXuatNgu));
        self.news_NgayNhapNgu(convertDateTime(model.NgayNhapNgu));
        self.news_NgayVaoDoan(convertDateTime(model.NgayVaoDoan));
        self.news_NgayVaoDang(convertDateTime(model.NgayVaoDang));
        self.news_NgayRaKhoiDang(convertDateTime(model.NgayRoiDang));
        self.news_NgayVaoDangChinhThuc(convertDateTime(model.NgayVaoDangChinhThuc));
    }
    //-- end tab thông tin chính trị --//

    //===========================================
    //  Delete
    //===========================================
    self.IndexTabs = ko.observable();
    self.removeNvTTSucKhoe = function (item) {
        vmModalRemove.show(NhanVienUri + "deleteTTSucKhoeNv?id=" + item.ID, "Xác nhận",
            "Bạn có chắc chắn muốn xóa thông tin sức khỏe ngày: " + convertDateTime(item.NgayKham) + " không");
        self.selectedNv(item.ID_NhanVien);
        self.IndexTabs(10);
    }
    self.removeNvGiaDinh = function (item) {
        vmModalRemove.show(NhanVienUri + "deleteGiaDinhNv?id=" + item.ID, "Xác nhận",
            "Bạn có chắc chắn muốn xóa thông tin " + item.HoTen + "( " + item.QuanHe + ") không");
        self.selectedNv(item.ID_NhanVien);
        self.IndexTabs(8);
    }
    self.removeNvQTCongTac = function (item) {
        vmModalRemove.show(NhanVienUri + "deleteQTCongTacNv?id=" + item.ID, "Xác nhận",
            "Bạn có chắc chắn muốn xóa quá trình công tác từ ngày " + convertDateTime(item.TuNgay) + " đến ngày " + convertDateTime(item.DenNgay) + " không");
        self.selectedNv(item.ID_NhanVien);
        self.IndexTabs(7);
    }
    self.removeNvQTDaoTao = function (item) {
        vmModalRemove.show(NhanVienUri + "deleteQTDaoTaoNv?id=" + item.ID, "Xác nhận",
            "Bạn có chắc chắn muốn xóa quy trình đào tạo từ ngày " + convertDateTime(item.TuNgay) + " đến ngày " + convertDateTime(item.DenNgay) + " không");
        self.selectedNv(item.ID_NhanVien);
        self.IndexTabs(6);
    }
    self.removeNvTTHopDong = function (item) {
        vmModalRemove.show(NhanVienUri + "deleteTTHopDongNv?id=" + item.ID, "Xác nhận",
            "Bạn có chắc chắn muốn xóa hợp đồng số: " + item.SoHopDong + " không");
        self.selectedNv(item.ID_NhanVien);
        self.IndexTabs(1);
    }
    self.removeNvBaoHiem = function (item) {
        vmModalRemove.show(NhanVienUri + "deleteBaoHiemNv?id=" + item.ID, "Xác nhận",
            "Bạn có chắc chắn muốn xóa sổ bảo hiểm:  " + item.SoBaoHiem + " không");
        self.selectedNv(item.ID_NhanVien);
        self.IndexTabs(2);
    }
    self.removeNvKhenthuong = function (item) {
        vmModalRemove.show(NhanVienUri + "deleteKhenThuongNv?id=" + item.ID, "Xác nhận",
            "Bạn có chắc chắn muốn xóa quá khen thưởng theo quyết định: " + item.SoQuyetDinh + " không");
        self.selectedNv(item.ID_NhanVien);
        self.IndexTabs(3);
    }
    self.removeNvLuongPhuCap = function (item) {
        vmModalRemove.show(NhanVienUri + "deleteLuongPhuCapNv?id=" + item.ID, "Xác nhận",
            "Bạn có chắc chắn muốn xóa lương phụ cấp có tên loại là :" + item.TenLoaiLuong + "  ngày áp dụng " + convertDateTime(item.NgayApDung) + " đến ngày áp dụng" + convertDateTime(item.NgayKetThuc) + " không");
        self.selectedNv(item.ID_NhanVien);
        self.IndexTabs(4);
    }
    self.removeNvMienGiam = function (item) {
        vmModalRemove.show(NhanVienUri + "deleteMienGiamthueNv?id=" + item.ID, "Xác nhận",
            "Bạn có chắc chắn muốn xóa khoản miễn giảm: " + item.KhoanMienGiam + " không");
        self.selectedNv(item.ID_NhanVien);
        self.IndexTabs(5);
    }
    $('body').on('DeleteSuccess', function () {

        switch (self.IndexTabs()) {
            case 1:
                getAllHopDongNv();
                break;
            case 2:
                getAllBaoHiemNv();
                break;
            case 3:
                getAllKhenThuongNv();
                break;
            case 4:
                getAllLuongPhuCapNv();
                break
            case 5:
                getAllMienGiamNv();
                break
            case 6:
                getAllQTDaoTaoNv();
                break;
            case 7:
                getAllQTCongTacNv();
                break;
            case 8:
                getAllTTGiaDinhNv();
                break;
            case 9:

                break;
            case 10:
                getAllTTSucKhoeNv();
                break;
            default:
                break;
        }
        if (self.IndexTabs() === 6) {

        }
    });
    //-- end delete --//

    //===========================================
    //  Tab thông tin gia đình
    //===========================================
    self.listTTGiaDinh = ko.observableArray();

    self.ShowThongTinGD = function (model) {
        self.selectedNv(model.ID);
        getAllTTGiaDinhNv();
    }
    function getAllTTGiaDinhNv() {
        $.getJSON(NhanVienUri + "GetTTGiaDinhNv?nhanvienId=" + self.selectedNv(), function (data) {
            self.listTTGiaDinh(data);
        });
    }
    self.editTTGD = function (item) {
        vm.edit(item);
        self.selectedNv(item.ID_NhanVien);
    }
    self.showPopupGiaDinh = function (item) {
        vm.Insert(item.ID);
        self.selectedNv(item.ID);
    };
    $('body').on('InsertNvGiaDinhSuccess', function () {
        getAllTTGiaDinhNv();
    });

    //-- end tab thông tin gia đình --//

    //===========================================
    //  Tab Quy trình đào tạo
    //===========================================
    self.listNvQtDaoTao = ko.observableArray();
    self.ShowQuyTrinhDaoTao = function (model) {
        self.selectedNv(model.ID);
        getAllQTDaoTaoNv();
    }
    function getAllQTDaoTaoNv() {
        $.getJSON(NhanVienUri + "GetQTDaotaoNv?nhanvienId=" + self.selectedNv(), function (data) {
            self.listNvQtDaoTao(data);
        });
    }
    self.ShowPopupNvDaoTao = function (item) {
        vmQuyTrinhDaoTao.Insert(item.ID);
        self.selectedNv(item.ID);

    }
    self.editNvQtDaoTao = function (item) {
        vmQuyTrinhDaoTao.edit(item);
        self.selectedNv(item.ID_NhanVien);
    }
    $('body').on('InsertNvQtDaoTaoSuccess', function () {
        getAllQTDaoTaoNv();
    });
    //-- end tab Quy trình đào tạo --//

    //===========================================
    //  Tab Qúa trình công tác
    //===========================================
    self.listNvQtCongTac = ko.observableArray();
    self.ShowQuaTrinhCongTac = function (model) {
        self.selectedNv(model.ID);
        getAllQTCongTacNv();

    }
    function getAllQTCongTacNv() {
        $.getJSON(NhanVienUri + "GetQTCongTacNv?nhanvienId=" + self.selectedNv(), function (data) {
            self.listNvQtCongTac(data);
        });
    }
    self.ShowPopupNvCongTac = function (item) {
        vmQuaTrinhCongTac.Insert(item.ID);
        self.selectedNv(item.ID);

    }
    self.editNvQtCongTac = function (item) {
        vmQuaTrinhCongTac.edit(item);
        self.selectedNv(item.ID_NhanVien);
    }
    $('body').on('InsertNvQtCongTacSuccess', function () {
        getAllQTCongTacNv();
    });
    //-- end tab Qúa trình công tác --//


    //===========================================
    //  Tab Lưởng phụ cấp
    //===========================================
    self.listNvLuongPhuCap = ko.observableArray();
    self.ShowLuongPhuCap = function (model) {
        self.selectedNv(model.ID);
        getAllLuongPhuCapNv();
    }
    function getAllLuongPhuCapNv() {
        $.getJSON(NhanVienUri + "GetThietLapLuong_ofNhanVien?idNhanVien=" + self.selectedNv() + '&idChiNhanh=' + _id_DonVi +
            '&currentPage=0&pageSize=100', function (data) {
                self.listNvLuongPhuCap(data);
                vmNvLuong.listData.NS_LuongPhuCap = data;
            });
    }
    self.ShowPopupNvLuongPhuCap = function (item) {
        vmNvLuong.CheckRole($('#RoleLoaiLuong_Insert').val(), $('#RoleLoaiLuong_Update').val(), $('#RoleLoaiLuong_Delete').val());
        vmNvLuong.Insert(item.ID);
        self.selectedNv(item.ID);

    }
    self.editNvLUongPhuCap = function (item) {
        vmNvLuong.CheckRole($('#RoleLoaiLuong_Insert').val(), $('#RoleLoaiLuong_Update').val(), $('#RoleLoaiLuong_Delete').val());
        $.getJSON(NhanVienUri + 'GetThietLapLuongChiTiet?idLuongPC=' + item.ID, function (x) {
            if (x.res === true) {
                for (let i = 0; i < x.dataSoure.length; i++) {
                    let itFor = x.dataSoure[i];
                    x.dataSoure[i].LuongNgayThuong = commonStatisJs.FormatNumber3Digit(itFor.LuongNgayThuong);
                    x.dataSoure[i].Thu7_GiaTri = commonStatisJs.FormatNumber3Digit(itFor.Thu7_GiaTri);
                    x.dataSoure[i].ThCN_GiaTri = commonStatisJs.FormatNumber3Digit(itFor.ThCN_GiaTri);
                    x.dataSoure[i].NgayNghi_GiaTri = commonStatisJs.FormatNumber3Digit(itFor.NgayNghi_GiaTri);
                    x.dataSoure[i].NgayLe_GiaTri = commonStatisJs.FormatNumber3Digit(itFor.NgayLe_GiaTri);
                }
                item.NS_ThietLapLuongChiTiet = x.dataSoure;
                vmNvLuong.edit(item);
                self.selectedNv(item.ID_NhanVien);
            }
        }).fail(function (x) {
        });
    }

    self.SaoChepThietLapLuongTuNvien = function (item) {
        item.Active = false;
        var lstLoai = [];
        var luong = $.grep(self.listNvLuongPhuCap(), function (x) {
            return $.inArray(x.LoaiLuong, [1, 2, 3, 4]) > -1;
        });
        if (luong.length > 0) {
            lstLoai = [1];
        }
        luong = $.grep(self.listNvLuongPhuCap(), function (x) {
            return $.inArray(x.LoaiLuong, [51, 52, 53]) > -1;
        });
        if (luong.length > 0) {
            lstLoai.push(5);
        }
        luong = $.grep(self.listNvLuongPhuCap(), function (x) {
            return $.inArray(x.LoaiLuong, [61, 62, 63]) > -1;
        });
        if (luong.length > 0) {
            lstLoai.push(6);
        }
        vmCopySetupSalary.listdata.PhongBan = self.listphongbanold();
        vmCopySetupSalary.saoChepFrom(item, lstLoai);
    }

    self.SaoChepThietLapLuong = function () {
        vmCopySetupSalary.listdata.PhongBan = commonStatisJs.CopyArray(self.listphongbanold());
        vmCopySetupSalary.showModal();
    }

    $('body').on('InsertNvluongphucapSuccess', function () {
        getAllLuongPhuCapNv();
    });
    //-- end tab  Lưởng phụ cấp --//

    //===========================================
    //  Tab sức khỏe
    //===========================================
    self.listNvTTSucKhoe = ko.observableArray();
    self.ShowTTSucKhoe = function (model) {
        self.selectedNv(model.ID);
        getAllTTSucKhoeNv();
    }
    function getAllTTSucKhoeNv() {
        $.getJSON(NhanVienUri + "GetTTSucKhoeNv?nhanvienId=" + self.selectedNv(), function (data) {
            self.listNvTTSucKhoe(data);
        });
    }
    self.ShowPopupNvTTSucKhoe = function (item) {
        vmTTSucKhoe.Insert(item.ID);
        self.selectedNv(item.ID);

    }
    self.editNvTTSucKhoe = function (item) {
        vmTTSucKhoe.edit(item);
        self.selectedNv(item.ID_NhanVien);
    }

    $('body').on('InsertNvTTSucKhoeSuccess', function () {
        getAllTTSucKhoeNv();
    });
    //-- end tab sức khỏe --//

    //===========================================
    //  Tab hợp dồng
    //===========================================
    self.listNvHopDong = ko.observableArray();
    self.ShowHopDong = function (model) {
        self.selectedNv(model.ID);
        getAllHopDongNv();
    }
    function getAllHopDongNv() {
        $.getJSON(NhanVienUri + "GetHopDongNv?nhanvienId=" + self.selectedNv(), function (data) {
            self.listNvHopDong(data);
        });
    }
    self.ShowPopupNvHopDong = function (item) {
        vmNvHopDong.Insert(item.ID);
        self.selectedNv(item.ID);

    }
    self.editNvHopDong = function (item) {
        vmNvHopDong.edit(item);
        self.selectedNv(item.ID_NhanVien);
    }

    $('body').on('InsertNvHopDongSuccess', function () {
        getAllHopDongNv();
    });
    //-- end tab hợp đồng --//

    //===========================================
    //  Tab hbảo hiểm
    //===========================================
    self.listNvBaoHiem = ko.observableArray();
    self.ShowBaoHiem = function (model) {
        self.selectedNv(model.ID);
        getAllBaoHiemNv();
    }
    function getAllBaoHiemNv() {
        $.getJSON(NhanVienUri + "GetBaoHiemNv?nhanvienId=" + self.selectedNv(), function (data) {
            self.listNvBaoHiem(data);
        });
    }
    self.ShowPopupNvBaoHiem = function (item) {
        vmNvBaoHiem.Insert(item.ID);
        self.selectedNv(item.ID);

    }
    self.editNvHBaoHiem = function (item) {
        vmNvBaoHiem.edit(item);
        self.selectedNv(item.ID_NhanVien);
    }

    $('body').on('InsertNvBaoHiemSuccess', function () {
        getAllBaoHiemNv();
    });
    //-- end tab bảo hiểm --//

    //===========================================
    //  Tab Khen thưởng
    //===========================================
    self.listNvKhenThuong = ko.observableArray();
    self.ShowKhenThuong = function (model) {
        self.selectedNv(model.ID);
        getAllKhenThuongNv();
    }
    function getAllKhenThuongNv() {
        $.getJSON(NhanVienUri + "GetKhenThuongNv?nhanvienId=" + self.selectedNv(), function (data) {
            self.listNvKhenThuong(data);
        });
    }
    self.ShowPopupNvKhenThuong = function (item) {
        vmNvKhenThuongKyLuat.Insert(item.ID);
        self.selectedNv(item.ID);

    }
    self.editNvKhenThuong = function (item) {
        vmNvKhenThuongKyLuat.edit(item);
        self.selectedNv(item.ID_NhanVien);
    }

    $('body').on('InsertNvKhenThuongSuccess', function () {
        getAllKhenThuongNv();
    });
    //-- end tab khen thưởng --//

    //===========================================
    //  Tab miễn giảm
    //===========================================
    self.listNvMienGiam = ko.observableArray();
    self.ShowMienGiam = function (model) {
        self.selectedNv(model.ID);
        getAllMienGiamNv();
    }
    function getAllMienGiamNv() {
        $.getJSON(NhanVienUri + "GetMienGiamNv?nhanvienId=" + self.selectedNv(), function (data) {
            self.listNvMienGiam(data);
        });
    }
    self.ShowPopupNvMienGiam = function (item) {
        vmMienGiamThue.Insert(item.ID);
        self.selectedNv(item.ID);

    }
    self.editNvMienGiam = function (item) {
        vmMienGiamThue.edit(item);
        self.selectedNv(item.ID_NhanVien);
    }

    $('body').on('InsertNvMienGiamThueSuccess', function () {
        getAllMienGiamNv();
    });
    //-- end tab miễn giảm --//

    //===========================================
    //  Phòng ban
    //===========================================
    self.searchPB = ko.observable();
    self.listphongbanold = ko.observableArray();
    var tree = '';
    function loadPhongBan() {
        $.getJSON(NhanVienUri + "GetTreePhongBan?chinhanhId=" + $('#hd_IDdDonVi').val(), function (data) {
            self.listphongbanold(data);
            vmNsPhongBan.listPhongBan = self.listphongban();

            tree = $('#treePhongBan1').tree({
                primaryKey: 'id',
                uiLibrary: 'bootstrap',
                imageHtmlField: 'icon',
                dataSource: data,
            }).on('select', function (e, node, id) {
                model_nv.PhongBan_getAllChild(id);
            })
        });
    }

    $('#modalNsPhongBan').on('hidden.bs.modal', function () {
        if (vmNsPhongBan.saveOK) {
            tree.destroy();
            loadPhongBan();
        }
    })

    $(document).on('click', '#treePhongBan1 .list-group-item .fa-edit', function () {
        var $li = $(this).closest('.list-group-item').attr('data-id');
        let nodeItem = tree.getDataById($li);
        if (!model_nv.rolePhongBan_Update()) {
            ShowMessage_Danger('Bạn không có quyền cập nhật phòng ban');
            return;
        }
        if (nodeItem.ID_DonVi !== null) {// phongban macdinh
            vmNsPhongBan.edit(nodeItem);
        }
    });
   
    function GetChildren_Phong(arrParent, arrJson, txtSearch, arr, isRoot) {
        if (txtSearch === '') {
            return self.listphongbanold();
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
            var arr = GetChildren_Phong([], self.listphongbanold(), filter, [], true);
            tree.destroy();
            tree = $('#treePhongBan1').tree({
                primaryKey: 'id',
                uiLibrary: 'bootstrap',
                dataSource: arr,
                checkboxes: false,
            }).on('select', function (e, node, id) {
                model_nv.PhongBan_getAllChild(id);
            });
        }
    });

    self.PhongBan_getAllChild = function (idNhom) {
        self.selectedPhongBan(idNhom);
        getAllNhanViens();
    }

    loadPhongBan();
    self.listphongban = ko.computed(function () {
        if (commonStatisJs.CheckNull(self.searchPB())) {
            return self.listphongbanold();
        }
        else {
            var data = $.grep(self.listphongbanold(), function (e) {
                var result = (commonStatisJs.convertVieToEng(e.text).indexOf(commonStatisJs.convertVieToEng(self.searchPB())) >= 0
                    || (e.children.length > 0 && (e.children.some(evensearch1) || e.children.some(evensearch2))));
                return result;
            });
            return data;
        }
    });
    var evensearch1 = function (o) {
        return commonStatisJs.convertVieToEng(o.text).indexOf(commonStatisJs.convertVieToEng(self.searchPB())) >= 0;
    };
    var evensearch2 = function (o) {
        return o.children.length > 0 && o.children.some(evensearch1);
    };
    self.ShowPopupPhongban = function () {
        vmNsPhongBan.Insert();
    }

    self.editPhongBan = function (item) {
        vmNsPhongBan.edit(item, $('#RolePhongBan_Update').val(), $('#RolePhongBan_Delete').val());
    }
    self.editPhongBan1 = function (item) {
        vmNsPhongBan.edit(item, $('#RolePhongBan_Update').val(), $('#RolePhongBan_Delete').val());
    }
    self.editPhongBan2 = function (item) {
        vmNsPhongBan.edit(item, $('#RolePhongBan_Update').val(), $('#RolePhongBan_Delete').val());
    }
    $('body').on('InsertNsPhongBanSuccess', function () {
        loadPhongBan();
        LoadPhongBanChiNhanh(vmNsPhongBan.ChinhanhId);
    });

    $('.tree-phong-ban').on('click', '.treename', function () {
        $('.tree-phong-ban .treename').each(function () {

            $(this).removeClass('yellow');
        });
        $(this).addClass('yellow');
    });

    //-- end Phòng ban --//

    //===========================================
    //  Ẩn hiện cột grid
    //===========================================
    var Key_Form = 'Key_DsNhanVien';

    self.listAddChiNhanh = ko.observableArray();
    var ID_news = 1;
    refreshChiNhanh();
    function refreshChiNhanh() {
        self.listAddChiNhanh([]);
        ID_news = 1;

        self.listAddChiNhanh.push({
            ID: ID_news,
            ID_ChiNhanh: _id_DonVi,
            Text_ChiNhanh: $("#_txtTenDonVi").text(),
            ID_PhongBan: null,
            Text_PhongBan: 'Phòng ban mặc định',
            listPhongBan: [],
            LaMacDinh: true
        });
    }
    function LoadPhongBanChiNhanh(chinhanhId) {
        var result = self.listAddChiNhanh().filter(o => o.ID_ChiNhanh === chinhanhId);
        for (var i = 0; i < result.length; i++) {
            CallAjaxPhongBanChang(result[i]);
        }
    }

    function CallAjaxPhongBanChang(root) {
        self.listAddChiNhanh.refresh();
        $.getJSON(NhanVienUri + "GetTreePhongBan?chinhanhId=" + root.ID_ChiNhanh, function (data) {
            root.listPhongBan = data;
            self.listAddChiNhanh.refresh();
        });
    }
    function EditStaffphongbanchinhanh(item) {
        refreshChiNhanh();
        if (item != null && item.length > 0) {
            var t = 1;
            for (var i = 0; i < item.length; i++) {
                item[i].ID = t;
                t++;
            }
            self.listAddChiNhanh(item);
            self.listAddChiNhanh.refresh;
        }
    }

    self.AddChiNhanh = function () {
        if (self.listAddChiNhanh().some(o => o.ID_ChiNhanh === null)) {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> Vui lòng chọn chi nhánh trước khi thêm ', "danger");

        }
        else {
            ID_news += 1;
            self.listAddChiNhanh.push({
                ID: ID_news,
                ID_ChiNhanh: null,
                Text_ChiNhanh: "",
                ID_PhongBan: null,
                Text_PhongBan: "",
                listPhongBan: [],
                LaMacDinh: false
            });
        }
    }
    self.RemoveChiNhanh = function (item) {
        self.listAddChiNhanh(self.listAddChiNhanh().filter(o => o.ID !== item.ID));
        self.listAddChiNhanh.refresh();
    };
    self.SeletecChiNhanh = function (root, item) {
        root.ID_ChiNhanh = item.ID;
        root.Text_ChiNhanh = item.TenDonVi;
        root.ID_PhongBan = null;
        root.listPhongBan = [];
        root.Text_PhongBan = null;
        self.listAddChiNhanh.refresh();
        $.getJSON(NhanVienUri + "GetTreePhongBan?chinhanhId=" + item.ID, function (data) {
            root.listPhongBan = data;
            self.listAddChiNhanh.refresh();
        });
    }
    self.clickInputPhongBan = function (root) {
        $.getJSON(NhanVienUri + "GetTreePhongBan?chinhanhId=" + root.ID_ChiNhanh, function (data) {
            root.listPhongBan = data;
            self.listAddChiNhanh.refresh();
        });
    }
    self.Seletecphongban = function (root, item) {
        if (root.ID_ChiNhanh !== null) {
            if (self.listAddChiNhanh().some(o => o.ID_ChiNhanh === root.ID_ChiNhanh && o.ID_PhongBan === item.id)) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> Phòng ban chi nhánh này đã được chọn', "danger");
            }
            else {
                root.ID_PhongBan = item.id;
                root.Text_PhongBan = item.text;
                self.listAddChiNhanh()[0].ID_PhongBan = item.id;
                self.listAddChiNhanh()[0].Text_PhongBan = item.text;
                self.listAddChiNhanh.refresh();
            }
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> Vui lòng chọn chi nhánh trước khi chọn phòng ban', "danger");
        }

    }
    self.ID_NhanVienAdmin = ko.observable();
    function getID_NhanVienAdmin() {
        ajaxHelper(NhanVienUri + "getID_NhanVienAdmin", 'GET').done(function (data) {
            self.ID_NhanVienAdmin(data);
        });
    }
    getID_NhanVienAdmin();

    self.ChosePhongBan = function (item) {
        self.selectedPhongBan(item.id);
        getAllNhanViens();
    }
    //-- end ẩn hiện cột grid --//
    function getAllNhanViens(IsFilter = false, IsLoad = false) {
        var NhanVienLoad = localStorage.getItem('FindNhanVien');
        if (NhanVienLoad !== null) {
            $('#txtMaNhanVien1').val(NhanVienLoad);
            _maNhanVien_seach = NhanVienLoad;
        }
        $('#table-reponsive').gridLoader();
        if ($('#IsOpenHRM').val() === 'True') {
            if (IsFilter) {
                pageAllNum = 1;
                nextPageAll_NhanVien = 1;
            }
            var model = {
                DonViId: $('#hd_IDdDonVi').val(),
                PhongBanId: self.selectedPhongBan(),
                Text: _maNhanVien_seach,
                TrangThai: _trangthai_seach,
                pageSize: pageAllSize,
                pageNum: pageAllNum,
                GioiTinh: self.IsGioiTinh(),
                BaoHiem: self.ListBaoHiem_filter_computed().map(function (i, e) {
                    return i.ID;
                }),
                DanToc: self.ListDanToc_filter_computed().map(function (i, e) {
                    return i.Name;
                }),
                ChinhTri: self.ListChinhTri_filter_computed().map(function (i, e) {
                    return i.ID;
                }),
                LoaiHopDong: self.ListLoaiHopDong_filter_computed().map(function (i, e) {
                    return i.ID;
                }),
                HK_TT: $('#hk_tinhthanh_filter').val(),
                HK_QH: $('#hk_quanhuyen_filter').val(),
                HK_XP: $('#hk_xaphuong_filter').val(),
                TT_TT: $('#tt_tinhthanh_filter').val(),
                TT_QH: $('#tt_quanhuyen_filter').val(),
                TT_XP: $('#tt_xaphuong_filter').val(),
                TypeTime: self.TypeBirthDate(),
                TuNgay: self.startDate(),
                DenNgay: self.endDate(),

            }
            $.ajax({
                data: model,
                url: NhanVienUri + "getListNhanVienHRM",
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    $('#table-reponsive').gridLoader({ show: false });
                    if (data.res === true) {
                        self.NhanViens(data.dataSoure.LstData);
                        LoadHtmlGrid();
                        if (self.NhanViens() != null) {
                            self.RowsStartAll_NhanVien((pageAllNum - 1) * pageAllSize + 1);
                            self.RowsEndAll_NhanVien((pageAllNum - 1) * pageAllSize + self.NhanViens().length)
                        }
                        else {
                            self.RowsStartAll_NhanVien('0');
                            self.RowsEndAll_NhanVien('0');
                        }
                        self.RowsAll_NhanVien(data.dataSoure.Rowcount);
                        //self.PagesAll_NhanVien(data.dataSoure.LstPageNumber);
                        //self.PagesAll_NhanVien(data.dataSoure.numberPage);
                        AllPageAll_NhanVien = data.dataSoure.numberPage;
                        self.selecPageAll_NhanVien();
                        if (IsLoad || IsFilter) {
                            self.ReserPageAll_NhanVien();
                        }
                    }
                    else {
                        alert(data.mess);
                    }
                }
            });
        }
        else {
            ajaxHelper(NhanVienUri + "getListNhanVien?phongbanId=" + self.selectedPhongBan() + "&maNhanVien=" + _maNhanVien_seach + "&trangthai=" + _trangthai_seach + "&pageSize=" + pageAllSize + "&pageNum=" + pageAllNum, 'GET').done(function (data) {
                $('#table-reponsive').gridLoader({ show: false });
                self.NhanViens(data.LstData);
                LoadHtmlGrid();
                if (self.NhanViens() != null) {
                    self.RowsStartAll_NhanVien((pageAllNum - 1) * pageAllSize + 1);
                    self.RowsEndAll_NhanVien((pageAllNum - 1) * pageAllSize + self.NhanViens().length)
                }
                else {
                    self.RowsStartAll_NhanVien('0');
                    self.RowsEndAll_NhanVien('0');
                }
                self.RowsAll_NhanVien(data.Rowcount);
                // self.PagesAll_NhanVien(data.LstPageNumber);
                //self.PagesAll_NhanVien(data.numberPage);
                AllPageAll_NhanVien = data.numberPage;
                self.selecPageAll_NhanVien();
            });
        }
        $('.line-right').height(0).css("margin-top", "0px");
        localStorage.removeItem('FindNhanVien');
    }

    getAllNhanViens();

    self.clickSearchNhanVien = function () {
        pageAllNum = 1;
        self.currentPageAll_NhanVien(pageAllNum);
        getAllNhanViens(true);
    }

    self.ExportNhanVienExcel = function () {
        $('#table-reponsive').gridLoader();
        var model = {
            DonViId: $('#hd_IDdDonVi').val(),
            PhongBanId: self.selectedPhongBan(),
            Text: _maNhanVien_seach,
            TrangThai: _trangthai_seach,
            pageSize: pageAllSize,
            pageNum: pageAllNum,
            GioiTinh: self.IsGioiTinh(),
            BaoHiem: self.ListBaoHiem_filter_computed().map(function (i, e) {
                return i.ID;
            }),
            DanToc: self.ListDanToc_filter_computed().map(function (i, e) {
                return i.Name;
            }),
            ChinhTri: self.ListChinhTri_filter_computed().map(function (i, e) {
                return i.ID;
            }),
            LoaiHopDong: self.ListLoaiHopDong_filter_computed().map(function (i, e) {
                return i.ID;
            }),
            HK_TT: $('#hk_tinhthanh_filter').val(),
            HK_QH: $('#hk_quanhuyen_filter').val(),
            HK_XP: $('#hk_xaphuong_filter').val(),
            TT_TT: $('#tt_tinhthanh_filter').val(),
            TT_QH: $('#tt_quanhuyen_filter').val(),
            TT_XP: $('#tt_xaphuong_filter').val(),
            TypeTime: self.TypeBirthDate(),
            TuNgay: self.startDate(),
            DenNgay: self.endDate(),
            ColumnHiden: LocalCaches.LoadColumnGrid(Key_Form).map(x => x.Value)

        }
        $.ajax({
            data: model,
            url: NhanVienUri + "ExportExcelNhanVien",
            type: 'POST',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (data) {
                $('#table-reponsive').gridLoader({ show: false });
                if (data.res === true) {
                    window.location.href = "/api/DanhMuc/NS_NhanVienAPI/DownloadExecl?fileSave=" + data.dataSoure;
                }
                else {
                    alert(data.mess);
                }
            }
        });
    }
    self.PrintHsnv = function (item) {
        vmPrintHsnv.print(item.ID, $('.table-reponsive'));
    }
    self.ExportWord = function (item) {
        vmPrintHsnv.exportWord(item.ID, $('.table-reponsive'));
    }
    //Trinhpv import NhanVien
    var loai_Import = 1;
    self.TieuDe_ImportNhanVien = ko.observable("Nhập thông tin nhân viên từ File dữ liệu");
    self.SelectModal_import = function (item) {
        var thisObj = event.currentTarget;
        loai_Import = $(thisObj).attr('values');
        if (loai_Import === '1')
            self.TieuDe_ImportNhanVien("Nhập thông tin nhân viên từ File dữ liệu");
        else if (loai_Import === '2')
            self.TieuDe_ImportNhanVien("Nhập thông tin hợp đồng từ File dữ liệu");
        else if (loai_Import === '3')
            self.TieuDe_ImportNhanVien("Nhập thông tin bảo hiểm từ File dữ liệu");
        else if (loai_Import === '4')
            self.TieuDe_ImportNhanVien("Nhập thông tin khen thưởng, phụ cấp từ File dữ liệu");
        else if (loai_Import === '5')
            self.TieuDe_ImportNhanVien("Nhập thông tin khoản lương, phụ cấp từ File dữ liệu");
        else if (loai_Import === '6')
            self.TieuDe_ImportNhanVien("Nhập thông tin miễn giảm thuế từ File dữ liệu");
        else if (loai_Import === '7')
            self.TieuDe_ImportNhanVien("Nhập thông tin quy trình đào tạo từ File dữ liệu");
        else if (loai_Import === '8')
            self.TieuDe_ImportNhanVien("Nhập thông tin quá trình công tác từ File dữ liệu");
        else if (loai_Import === '9')
            self.TieuDe_ImportNhanVien("Nhập thông tin gia đình từ File dữ liệu");
        else if (loai_Import === '10')
            self.TieuDe_ImportNhanVien("Nhập thông tin chính trị từ File dữ liệu");
        else if (loai_Import === '11')
            self.TieuDe_ImportNhanVien("Nhập thông tin sức khỏe từ File dữ liệu");
        $("#ModalImport").modal("show");
    }
    self.SelectModal_importNhanVienCoBan = function (item) {
        loai_Import = '12';
        self.TieuDe_ImportNhanVien("Nhập Danh sách nhân viên từ File dữ liệu");
        $("#ModalImport").modal("show");
    }
    function LoadingForm(IsShow) {
        $('.table-reponsive').gridLoader({ show: IsShow });
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
    self.DownloadFileTeamplateXLS = function () {
        switch (loai_Import) {
            case '1':
                //var url = NhanVienUri + "Download_ThongTinNhanVien?filePath=" + "FileImport_ThongTinNhanVien.xls" + "&fileSave=" + "FileImport-ThongTinNhanVien.xls";
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport_ThongTinNhanVien.xls";
                window.open(url)
                break;
            case '2':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinHopDong.xls";
                window.open(url)
                break;
            case '3':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinBaoHiem.xls";
                window.open(url)
                break;
            case '4':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinKhenThuongPhuCap.xls";
                window.open(url)
                break;
            case '5':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinKhoanLuongPhuCap.xls";
                window.open(url)
                break;
            case '6':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinMienGiamThue.xls";
                window.open(url)
                break;
            case '7':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinQuyTrinhDaoTao.xls";
                window.open(url)
                break;
            case '8':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinQuaTrinhCongTac.xls";
                window.open(url)
                break;
            case '9':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinGiaDinh.xls";
                window.open(url)
                break;
            case '10':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinChinhTri.xls";
                window.open(url)
                break;
            case '11':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinSucKhoeNhanVien.xls";
                window.open(url)
                break;
            case '12':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-DanhSachNhanVien.xls";
                window.open(url)
                break;
        }
    }
    //Download file teamplate excel format (*.xlsx)
    self.DownloadFileTeamplateXLSX = function () {
        switch (loai_Import) {
            case '1':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport_ThongTinNhanVien.xlsx";
                window.open(url)
                break;
            case '2':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinHopDong.xlsx";
                window.open(url)
                break;
            case '3':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinBaoHiem.xlsx";
                window.open(url)
                break;
            case '4':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinKhenThuongPhuCap.xlsx";
                window.open(url)
                break;
            case '5':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinKhoanLuongPhuCap.xlsx";
                window.open(url)
                break;
            case '6':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinMienGiamThue.xlsx";
                window.open(url)
                break;
            case '7':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinQuyTrinhDaoTao.xlsx";
                window.open(url)
                break;
            case '8':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinQuaTrinhCongTac.xlsx";
                window.open(url)
                break;
            case '9':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinGiaDinh.xlsx";
                window.open(url)
                break;
            case '10':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinChinhTri.xlsx";
                window.open(url)
                break;
            case '11':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-ThongTinSucKhoeNhanVien.xlsx";
                window.open(url)
                break;
            case '12':
                var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "NhanVien/FileImport-DanhSachNhanVien.xlsx";
                window.open(url)
                break;
        }
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
    self.ShowandHide = function () {
        self.insertArticleNews();
    }
    self.loiExcel = ko.observableArray();
    $(".BangBaoLoi").hide();
    self.insertArticleNews = function () {
        LoadingForm(true);
        var url_import;
        var thongbao;
        switch (loai_Import) {
            case '1':
                url_import = NhanVienUri + "ImportExcel_ThongTinNhanVien?ID_ChiNhanh=" + $('#hd_IDdDonVi').val();
                thongbao = 'nhân viên '
                break;
            case '2':
                url_import = NhanVienUri + "ImportExcel_ThongTinHopDong";
                thongbao = 'hợp đồng '
                break;
            case '3':
                url_import = NhanVienUri + "ImportExcel_ThongTinBaoHiem";
                thongbao = 'bảo hiểm '
                break;
            case '4':
                url_import = NhanVienUri + "ImportExcel_ThongTinKhenThuong";
                thongbao = 'khen thưởng, phụ cấp '
                break;
            case '5':
                url_import = NhanVienUri + "ImportExcel_ThongTinKhoanLuong";
                thongbao = 'khoản lương, phụ cấp '
                break;
            case '6':
                url_import = NhanVienUri + "ImportExcel_ThongTinMienGiamThue";
                thongbao = 'miễn giảm thuế '
                break;
            case '7':
                url_import = NhanVienUri + "ImportExcel_ThongTinQuyTrinhDaoTao";
                thongbao = 'quy trình đào tạo '
                break;
            case '8':
                url_import = NhanVienUri + "ImportExcel_ThongTinQuaTrinhCongTac";
                thongbao = 'quá trình công tác '
                break;
            case '9':
                url_import = NhanVienUri + "ImportExcel_ThongTinGiaDinh";
                thongbao = 'gia đình '
                break;
            case '10':
                url_import = NhanVienUri + "ImportExcel_ThongTinChinhTri";
                thongbao = 'chính trị '
                break;
            case '11':
                url_import = NhanVienUri + "ImportExcel_ThongTinSucKhoe";
                thongbao = 'sức khỏe '
                break;
            case '12':// if don't setup detail nhanvien
                url_import = NhanVienUri + "ImportExcel_DanhSachNhanVien?idDonVi=" + _id_DonVi;
                thongbao = 'nhân viên '
                break;
        }
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadFormKH").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadFormKH").files[i];
            formData.append("imageUploadFormKH", file);
        }
        $.ajax({
            type: "POST",
            url: url_import,
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
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Import thông tin " + thongbao + "thành công", "success");
                    Insert_NhatKyThaoTac(null, 1, 5);
                    document.getElementById('imageUploadFormKH').value = "";
                    self.visibleImport(true);
                    $(".NoteImport").show();
                    $(".filterFileSelect").hide();
                    $(".btnImportExcel").hide();
                    $(".BangBaoLoi").hide();
                    $("#ModalImport").modal("hide");
                    getAllNhanViens();
                }
                LoadingForm(false);
            },
            statusCode: {
                404: function () {
                    LoadingForm(false);
                },
                406: function (item) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + item.responseJSON.Message, "danger")
                    LoadingForm(false);
                },
                500: function (item) {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Import thông tin " + thongbao + "thất bại: " + item.responseJSON.Message, "danger");
                    LoadingForm(false);
                }

            },
            error: function (jqXHR, textStatus, errorThrown) {
            },
        });

    }

    self.addRownError = ko.observableArray();
    self.DoneWithError = function () {
        var url_import;
        var thongbao;
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
        for (var i = 0; i < self.addRownError().length; i++) {
            if (i == 0)
                rownError = self.addRownError()[i];
            else
                rownError = rownError + "_" + self.addRownError()[i];
        }
        LoadingForm(true);
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadFormKH").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadFormKH").files[i];
            formData.append("imageUploadFormKH", file);
        }
        switch (loai_Import) {
            case '1':
                url_import = NhanVienUri + "ImportThongTinNhanVien_WithError?RownError=" + rownError + "&ID_ChiNhanh=" + $('#hd_IDdDonVi').val();
                thongbao = 'nhân viên '
                break;
            case '2':
                url_import = NhanVienUri + "ImportThongTinHopDong_WithError?RownError=" + rownError;
                thongbao = 'hợp đồng '
                break;
            case '3':
                url_import = NhanVienUri + "ImportThongTinBaoHiem_WithError?RownError=" + rownError;
                thongbao = 'bảo hiểm '
                break;
            case '4':
                url_import = NhanVienUri + "ImportThongTinKhenThuong_WithError?RownError=" + rownError;
                thongbao = 'khen thưởng, phụ cấp '
                break;
            case '5':
                url_import = NhanVienUri + "ImportThongTinKhoanLuong_WithError?RownError=" + rownError;
                thongbao = 'khoản lương, phụ cấp '
                break;
            case '6':
                url_import = NhanVienUri + "ImportThongTinMienGiamThue_WithError?RownError=" + rownError;
                thongbao = 'miễn giảm thuế '
                break;
            case '7':
                url_import = NhanVienUri + "ImportThongTinQuyTrinhDaoTao_WithError?RownError=" + rownError;
                thongbao = 'quy trình đào tạo '
                break;
            case '8':
                url_import = NhanVienUri + "ImportThongTinQuaTrinhCongTac_WithError?RownError=" + rownError;
                thongbao = 'quá trình công tác '
                break;
            case '9':
                url_import = NhanVienUri + "ImportThongTinGiaDinh_WithError?RownError=" + rownError;
                thongbao = 'gia đình '
                break;
            case '10':
                url_import = NhanVienUri + "ImportThongTinChinhTri_WithError?RownError=" + rownError;
                thongbao = 'chính trị '
                break;
            case '11':
                url_import = NhanVienUri + "ImportThongTinSucKhoe_WithError?RownError=" + rownError;
                thongbao = 'sức khỏe '
                break;
            case '12':
                url_import = NhanVienUri + "ImportDanhSachNhanVien_WithError?RownError=" + rownError + '&idDonVi=' + _id_DonVi;
                thongbao = 'nhân viên '
                break;
        }
        $.ajax({
            type: "POST",
            url: url_import,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (item) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Import thông tin " + thongbao + "thành công", "success");
                Insert_NhatKyThaoTac(null, 1, 5);
                document.getElementById('imageUploadFormKH').value = "";
                $(".NoteImport").show();
                $(".filterFileSelect").hide();
                $(".btnImportExcel").hide();
                $(".BangBaoLoi").hide();
                $("#ModalImport").modal("hide");
                getAllNhanViens();
                LoadingForm(false);
            },
            statusCode: {
                404: function () {
                    LoadingForm(false);
                },
                500: function () {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Import thông tin " + thongbao + "thất bại", "danger");
                    LoadingForm(false);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                LoadingForm(false);
            },
        });

    }
    self.Export_NhanVienToExcel = function () {
        LoadingForm(true);
        var columnHide = null;
        var objDiary = {
            ID_NhanVien: $('.idnhanvien').text(),
            ID_DonVi: $('#hd_IDdDonVi').val(),
            ChucNang: "Quản lý nhân viên",
            NoiDung: "Xuất danh sách nhân viên",
            NoiDungChiTiet: "Xuất thông tin cơ bản nhân viên",
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
                if (self.NhanViens().length != 0) {
                    var url = NhanVienUri + "Export_NhanVienToExcel?phongbanId=" + self.selectedPhongBan() + "&maNhanVien=" + _maNhanVien_seach + "&trangthai=" + _trangthai_seach + "&columnsHide=" + columnHide;
                    window.location.href = url;
                    LoadingForm(false);
                }
                else {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Báo cáo không có dữ liệu", "danger");
                    LoadingForm(false);
                }
            },
            statusCode: {
                404: function () {
                    LoadingForm(false);
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
                LoadingForm(false);
            },
            complete: function () {
                LoadingForm(false);
            }
        })
    }
    function Insert_NhatKyThaoTac(objUsing, chucNang = 1, loaiNhatKy = 1) {
        // chuc nang (1.DoiTuong, 2.NhomDoiTuong, 3.PhieuThu, 4.Export, 5.Import)
        var tenChucNang = 'Quản lý nhân viên';
        var noiDung = '';
        var noiDungChiTiet = '';
        var txtFirst = 'Import';
        var tenChucNangLowercase = 'nhân viên';
        noiDung = txtFirst.concat('danh sách ', tenChucNangLowercase);
        noiDungChiTiet = noiDung.concat('<br /> Nhân viên thực hiện: ', $('#txtTenTaiKhoan').val().trim())
        // insert HT_NhatKySuDung
        var objNhatKy = {
            ID_NhanVien: $('.idnhanvien').text(),
            ID_DonVi: $('#hd_IDdDonVi').val(),
            ChucNang: tenChucNang,
            LoaiNhatKy: loaiNhatKy,
            NoiDung: noiDung,
            NoiDungChiTiet: noiDungChiTiet,
        };

        var myDataNK = {};
        myDataNK.objDiary = objNhatKy;
        $.ajax({
            url: DiaryUri + "post_NhatKySuDung",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myDataNK,
            success: function (x) {

            },
        });
    }

    self.listAddChiNhanhOld = ko.observableArray([]);
    var intkey = 1;
    function refreshChiNhanhold() {
        self.listAddChiNhanhOld([]);
        intkey = 1;
        self.listAddChiNhanhOld.push({
            Key: intkey
        });
    }
    refreshChiNhanhold();
    self.AddChiNhanhOld = function () {
        intkey = intkey + 1;
        self.listAddChiNhanhOld.push({
            Key: intkey
        })
    }
    self.RemoveChiNhanhold = function (item) {
        self.listAddChiNhanhOld(self.listAddChiNhanhOld().filter(o => o.Key !== item.Key));
        self.listAddChiNhanhOld.refresh();
    }
    return self;
};
var model_nv = new ViewModel();
ko.applyBindings(model_nv);

function convertDateTime(input) {
    if (input !== null && input !== undefined && input !== '') {
        return moment(input).format('DD/MM/YYYY');
    }
    return "";
}

function convertDateToServer(input) {
    if (input !== null && input !== undefined && input !== '') {
        var result = input.split('/');
        if (result.length >= 3) { return result[1] + "/" + result[0] + "/" + result[2]; }
    }
    return "";
}
ko.observableArray.fn.refresh = function () {
    var data = this().slice(0);
    this([]);
    this(data);
};

function formatNumberObj(obj) {
    var objVal = $(obj).val();
    $(obj).val(objVal.toString().replace(/,/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    return objVal;
}

$(window.document).on('shown.bs.modal', '.modal', function () {
    window.setTimeout(function () {
        $('[autofocus]', this).focus();
        $('[autofocus]').select();
    }.bind(this), 100);
});