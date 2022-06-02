var dtNow = new Date();
var _idNhanVien = $('.idnhanvien').text();
var _tenNhanVien = $('#txtTenNhanVien').val();

var FormModel_NewLoaiCongViec = function () {
    var self = this;
    self.ID = ko.observable();
    self.TenLoaiTuVanLichHen = ko.observable();
    self.TrangThai = ko.observable('2'); // 0.xoa, 1. chua xoa
    self.NguoiSua = ko.observable();
    self.NgaySua = ko.observable();

    self.setdata = function (item) {
        self.ID(item.ID);
        self.TenLoaiTuVanLichHen(item.TenLoaiTuVanLichHen);
        self.TrangThai(item.TrangThai);
        self.NguoiSua(item.NguoiSua);
        self.NgaySua(item.NgaySua);
    }
}

var FormModel_NewCongViec = function () {
    var self = this;
    self.ID = ko.observable();
    self.ID_LoaiTuVan = ko.observable();
    self.ID_KhachHang = ko.observable();
    self.ID_LienHe = ko.observable();
    self.ID_DonVi = ko.observable();
    self.ThoiGianTu = ko.observable(moment(dtNow).format('DD/MM/YYYY HH:mm'));
    self.ThoiGianDen = ko.observable(moment(dtNow).format('DD/MM/YYYY HH:mm'));
    self.NhacTruoc = ko.observable();
    self.ID_NhanVien = ko.observable(_idNhanVien);
    self.NoiDung = ko.observable();
    self.KetQua = ko.observable();
    self.CaNgay = ko.observable();
    self.NhacTruocLienHeLai = ko.observable();
    self.TrangThai = ko.observable('1');
    self.ID_TrangThai = ko.observable();
    self.Ma_TieuDe = ko.observable();
    self.MucDoUuTien = ko.observable(2);
    self.FileDinhKem = ko.observable('');

    // reset when show modal Add work
    $('#txtLoaiCongViec').text('--- Chọn loại công việc ---');
    $('#txtNhanVien_CV').text('--- Chọn nhân viên ---');
    $('#txtLienHe_CV').text('--- Chọn liên hệ---');
    $('#ddlRemindBefore').val('0');
    $('#ddlRemindAfter').val('0');
    $('#typeTrangThaiCV').val('1');
    $('#lstNhanVien li span i').remove();// delete check-after
    $('#txtStaffIncharge').text(_tenNhanVien);
    $('span[id=spanCheckNhanVien_' + _idNhanVien + ']').append(element_appendCheck);

    self.SetData = function (item) {
        self.ID(item.ID);
        self.ID_LoaiTuVan(item.ID_LoaiTuVan);
        self.ID_KhachHang(item.ID_KhachHang);
        self.ID_LienHe(item.ID_LienHe);
        self.ID_DonVi(item.ID_DonVi);
        self.NhacTruoc(item.NhacTruoc);

        self.NoiDung(item.NoiDung);
        self.KetQua(item.KetQua);
        self.CaNgay(item.CaNgay);
        self.TrangThai(item.TrangThai);
        self.Ma_TieuDe(item.Ma_TieuDe);
        self.MucDoUuTien(item.MucDoUuTien);
        if (item.ID_NhanVien !== null) {
            self.ID_NhanVien(item.ID_NhanVien);
        }
        else {
            self.ID_NhanVien(_idNhanVien);
        }
        $('#ddlRemindBefore').val(item.NhacTruoc);
        $('#ddlRemindAfter').val(item.NhacTruocLienHeLai);
        $('#typeTrangThaiCV').val(item.TrangThai);

        if (item.TenLoaiTuVanLichHen !== '') {
            $('#txtLoaiCongViec').text(item.TenLoaiTuVanLichHen);
            $('span[id=spanCheckLoaiCongViec_' + item.ID_LoaiTuVan + ']')
                .append(element_appendCheck);
        }

        if (item.TenDoiTuong !== '') {
            $('#txtKhachHang').val(item.TenDoiTuong);
        }

        if (item.TenLienHe !== '') {
            $('#txtLienHe_CV').text(item.TenLienHe);
            $('span[id=spanCheckLienHe_' + item.ID_LienHe + ']').append(element_appendCheck);
        }

        $('#lstNhanVien li span i').remove();// delete check-after
        if (item.TenNhanVien !== '') {
            $('#txtStaffIncharge').text(item.TenNhanVien);
            $('span[id=spanCheckNhanVien_' + item.ID_NhanVien + ']').append(element_appendCheck);
        }
        else {
            $('#txtStaffIncharge').text(_tenNhanVien);
            $('span[id=spanCheckNhanVien_' + _idNhanVien + ']').append(element_appendCheck);
        }

        self.ThoiGianTu(moment(item.ThoiGianTu).format('DD/MM/YYYY HH:mm'));
        var date = moment(item.ThoiGianTu).format('DD/MM/YYYY');
        var time = moment(item.ThoiGianTu).format('HH:mm');
        $('#txtNgayTu').val(date);
        $('#txtGioTu').val(time);

        if (item.ThoiGianDen != null) {
            self.ThoiGianDen(moment(item.ThoiGianDen).format('DD/MM/YYYY HH:mm'));

            date = moment(item.ThoiGianDen).format('DD/MM/YYYY');
            time = moment(item.ThoiGianDen).format('HH:mm');
            $('#txtNgayDen').val(date);
            $('#txtGioDen').val(time);
        }
        else {
            self.ThoiGianDen('');
            $('#txtNgayDen').val('');
            $('#txtGioDen').val('');
        }
        self.FileDinhKem(item.FileDinhKem);
        if (item.FileDinhKem !== null) {
            $('#workfile').show();
        }
        else {
            $('#workfile').hide();
        }
    };
};

var PartialView_CongViec = function () {
    var self = this;
    var ChamSocKhachHangUri = '/api/DanhMuc/ChamSocKhachHangAPI/';
    var DM_LoaiTVLHUri = '/api/DanhMuc/DM_LoaiTuVanLichHenAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    var userLogin = $('#txtTenTaiKhoan').val().trim();
    const txtLoaiCongViec = '#txtLoaiCongViec';
    const txtLienHe = '#txtLienHe_CV';
    const txtStaffIncharge = '#txtStaffIncharge';
    const txtStaffShare = '#txtStaffShare';
    const _idDonVi = $('#hd_IDdDonVi').val();
    var _idNhanVien = $('.idnhanvien').text();

    self.newCongViec = ko.observable(new FormModel_NewCongViec());
    self.newLoaiCongViec = ko.observable(new FormModel_NewLoaiCongViec());

    self.IsUpdate = ko.observable(false);
    self.LoaiCongViecs = ko.observableArray();
    self.ListAllDoiTuong = ko.observableArray();
    self.NhanViens = ko.observableArray();
    self.LoaiKhachHang = ko.observableArray();// tiem nang, uu tien,..
    self.listUserContact = ko.observableArray();
    self.IsAddTypeWork = ko.observable(true);
    self.booleanAddCV = ko.observable(true);
    self.IsCustomer = ko.observable(true);
    self.IsUpdateCustomerType = ko.observable(false);
    self.ListStaffShare = ko.observableArray([]);// nhan vien phoi hop thuc hien
    self.FileSelect = ko.observableArray();
    self.JqAutoSelectKH = ko.observable();

    function getallLoaiCongViec() {
        ajaxHelper(DM_LoaiTVLHUri + 'GetDM_LoaiCongViec', 'GET').done(function (x) {
            if (x.res === true) {
                self.LoaiCongViecs(x.data);
            }
        });
    }
    getallLoaiCongViec();

    self.showPopupAddNguoiLienHe = function () {
        $('#lblTitle').text('Thêm mới liên hệ');
        $('#modalPopuplg_Contact').modal('show');
        self.booleanAdd(true);
        var tenkhachhang = $('#txtKhachHang').val();
        self.newUserContact(new FormModel_NewUserContact());
        $('#modalPopuplg_Contact').on('shown.bs.modal', function () {
            $('#txtCustomer_modal').val(tenkhachhang);
        })
    }

    function Enable_btnSaveCongViec() {
        document.getElementById("btnLuuCongViec").disabled = false;
        document.getElementById("btnLuuCongViec").lastChild.data = "Lưu";
    }

    self.addCongViec = function () {
        var id = self.newCongViec().ID();
        var idLoaiCongViec = self.newCongViec().ID_LoaiTuVan();
        var idKhachHang = self.newCongViec().ID_KhachHang();
        var idLienHe = self.newCongViec().ID_LienHe();
        var idNhanVienPhuTrach = self.newCongViec().ID_NhanVien();
        var trangThai = self.newCongViec().TrangThai();
        var ketquaCongViec = self.newCongViec().KetQua();
        var noiDung = self.newCongViec().NoiDung();
        var workName = self.newCongViec().Ma_TieuDe();
        var dateFrom = $('#txtNgayTu').val();
        var dateTo = $('#txtNgayDen').val();
        var timeFrom = $('#txtGioTu').val();
        var timeTo = $('#txtGioDen').val();
        var thoiGianTu = dateFrom + " " + timeFrom;
        var thoiGianDen = dateTo + " " + timeTo;
        var priority = self.newCongViec().MucDoUuTien();
        var allDay = self.newCongViec().CaNgay();

        var datetimeFinish = moment($('.newDateTimesingle').val(), 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
        if (trangThai === "1") {
            datetimeFinish = null;
            ketquaCongViec = "";
        }
        if (allDay === true) {
            thoiGianTu = dateFrom;
            thoiGianDen = dateTo + " 23:59";
        }

        var itemEx = $.grep(self.LoaiCongViecs(), function (item) {
            return item.ID === idLoaiCongViec;
        });
        if (itemEx.length === 0) {
            ShowMessage_Danger('Vui lòng chọn loại công việc');
            Enable_btnSaveCongViec();
            $(txtLoaiCongViec).focus();
            return false;
        }

        if (workName === "" || workName === undefined || workName === null) {
            ShowMessage_Danger("Vui lòng nhập tên công việc");
            $('#txtTenCongViec').select();
            Enable_btnSaveCongViec();
            return false;
        }

        if (moment(thoiGianTu, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') === "Invalid date") {
            ShowMessage_Danger('Vui lòng chọn thời gian bắt đầu công việc');
            Enable_btnSaveCongViec();
            $('#txtThoiGianTu').select();
            return false;
        }

        if (thoiGianDen !== ' ') {
            if ((moment(thoiGianTu, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') > moment(thoiGianDen, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm'))
                && moment(thoiGianDen, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') !== "Invalid date") {
                ShowMessage_Danger('Thời gian kết thúc phải lớn hơn thời gian bắt đầu');
                Enable_btnSaveCongViec();
                return false;
            }
        }
        else {
            thoiGianDen = null;
        }

        if (trangThai !== "1" && datetimeFinish === "Invalid date") {
            ShowMessage_Danger("Vui lòng chọn thời gian hoàn thành hoặc hủy");
            $('.newDateTimesingle').select();
            Enable_btnSaveCongViec();
            return false;
        }

        if (datetimeFinish < moment(thoiGianTu, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm')
            && trangThai !== "1" && datetimeFinish !== "Invalid date") {
            ShowMessage_Danger("Thời gian hoàn thành phải lớn hơn thời gian bắt đầu");
            Enable_btnSaveCongViec();
            return false;
        }

        if (trangThai !== "1" && (ketquaCongViec === "" || ketquaCongViec === null || ketquaCongViec === undefined)) {
            ShowMessage_Danger("Vui lòng nhập kết quả công việc");
            $('#txtKetQuaCongViec').select();
            Enable_btnSaveCongViec();
            return false;
        }

        if (idNhanVienPhuTrach === const_GuidEmpty) {
            idNhanVienPhuTrach = null;
        }
        var CongViec = {
            ID: id,
            ID_LoaiTuVan: idLoaiCongViec,
            ID_KhachHang: idKhachHang,
            ID_LienHe: idLienHe,
            ID_DonVi: _idDonVi,
            ID_NhanVienQuanLy: _idNhanVien,// nhan vien dang nhap ht
            ID_NhanVien: idNhanVienPhuTrach,
            NoiDung: noiDung,
            NguoiTao: userLogin,
            TrangThai: trangThai,
            KetQua: ketquaCongViec,
            Ma_TieuDe: workName,
            MucDoUuTien: priority,
            GhiChu: '',
            NgayHoanThanh: datetimeFinish,
            CaNgay: allDay,
            NgayGio: moment(thoiGianTu, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm'),
            NgayGioKetThuc: thoiGianDen !== null ? moment(thoiGianDen, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm') : null,
        }
        var myData = {};
        myData.objCongViec = CongViec;
        myData.LstStaffShare = self.ListStaffShare();

        console.log(CongViec)

        // used to save diary
        var loaiCV = $('#txtLoaiCongViec').text();
        var tenDoiTuong = $('#txtKhachHang').text();
        var nvThucHien = '';
        var itemNV = $.grep(self.NhanViens(), function (x) {
            return x.ID === _idNhanVien;
        });
        if (itemNV.length > 0) {
            nvThucHien = itemNV[0].TenNhanVien;
        }

        if (self.booleanAddCV() === true) {
            $.ajax({
                url: ChamSocKhachHangUri + "Post_ChamSocKhachHang",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: myData,
                success: function (obj) {
                    if (obj.res === true) {
                        ShowMessage_Success('Thêm mới công việc thành công');

                        self.UpLoadFile(obj.data.ID)

                        var noiDungDiary = 'Thêm mới công việc "' + loaiCV + '" với khách hàng ' + tenDoiTuong;
                        var noiDungChiTiet = noiDungDiary.concat('<br /> - Thời gian: ', thoiGianTu, ' - ', thoiGianDen,
                            '<br/>- Nội dung: ', noiDung, '<br />- Nhân viên thực hiện: ', nvThucHien);
                        var objDiary = {
                            ID_NhanVien: _idNhanVien,
                            ID_DonVi: _idDonVi,
                            ChucNang: 'Khách hàng - Công việc',
                            LoaiNhatKy: 1,
                            NoiDung: noiDungDiary,
                            NoiDungChiTiet: noiDungChiTiet,
                        }
                        Insert_NhatKyThaoTac_1Param(objDiary);
                    }
                },
                error: function (jqXHR, textStatus, errorThrow) {
                    ShowMessage_Danger('Thêm mới công việc thất bại');
                },
                complete: function () {
                    Enable_btnSaveCongViec();
                    $('#modalPopuplg_Work').modal('hide');
                }
            })
        }
        else {

            $.ajax({
                url: ChamSocKhachHangUri + "Put_ChamSocKhachHang",
                type: 'PUT',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: myData,
                success: function (obj) {
                    if (obj.res === true) {
                        ShowMessage_Success('Cập nhật công việc thành công');

                        self.UpLoadFile(id);

                        var noiDungDiary = 'Cập nhật công việc "' + loaiCV + '" với khách hàng ' + tenDoiTuong;
                        var noiDungChiTiet = noiDungDiary.concat('<br /> - Thời gian: ', thoiGianTu, ' - ', thoiGianDen,
                            '<br/>- Nội dung: ', noiDung, '<br />- Nhân viên thực hiện: ', nvThucHien)

                        if (trangThai == 2) {
                            noiDungChiTiet = noiDungChiTiet.concat('<br /> - Kết quả công việc: ', ketquaCongViec);
                        }

                        var objDiary = {
                            ID_NhanVien: _idNhanVien,
                            ID_DonVi: _idDonVi,
                            ChucNang: 'Khách hàng - Công việc',
                            LoaiNhatKy: 2,
                            NoiDung: noiDungDiary,
                            NoiDungChiTiet: noiDungChiTiet,
                        }
                        Insert_NhatKyThaoTac_1Param(objDiary);
                    }
                },
                error: function (jqXHR, textStatus, errorThrow) {
                    ShowMessage_Danger('Cập nhật công việc thất bại');
                },
                complete: function () {
                    Enable_btnSaveCongViec();
                    $('#modalPopuplg_Work').modal('hide');
                }
            })
        }

        // update ID_TrangThai in DM_DoiTuong
        var idCusType = $('#ddlTrangThai').val();
        if (idCusType === undefined || idCusType == '') {
            idCusType = const_GuidEmpty;
        }

        console.log('idKhachHang ', idKhachHang, 'idCusType ', idCusType)
        ajaxHelper(ChamSocKhachHangUri + 'Update_LoaiKhachHang_DMDoituong?idDoituong=' + idKhachHang + '&cusType=' + idCusType, 'POST').done(function (x) {
            if (x == true) {
                self.IsUpdateCustomerType(true);
            }
        })
    }

    self.DeleteLoaiCongViec = function (item) {
        var idDelete = self.newCongViec().ID_LoaiTuVan();

        var loaiCV = self.newLoaiCongViec().TenLoaiTuVanLichHen();

        dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn xóa loại công việc "' + loaiCV + '"</b> không?', function () {
            $.ajax({
                type: "DELETE",
                url: ChamSocKhachHangUri + "Delete_LoaiCongViec/" + idDelete,
                dataType: 'json',
                contentType: 'application/json',
                success: function (result) {
                    if (result === '') {
                        for (var i = 0; i < self.LoaiCongViecs().length; i++) {
                            if (self.LoaiCongViecs()[i].ID === idDelete) {
                                self.LoaiCongViecs.remove(self.LoaiCongViecs()[i]);
                                break;
                            }
                        }
                        ShowMessage_Success('Xóa loại công việc thành công');
                    }
                    else {
                        ShowMessage_Danger(result);
                    }
                },
                error: function (error) {
                    $('#modalPopuplgDelete').modal('hide');
                    ShowMessage_Danger('Xóa loại công việc thất bại');
                }
            });
        })
    }

    self.addLoaiCongViec = function () {
        var _id = self.newLoaiCongViec().ID();
        var _loaiCongViec = self.newLoaiCongViec().TenLoaiTuVanLichHen();
        if (_loaiCongViec === "" || _loaiCongViec === null || _loaiCongViec === undefined) {
            ShowMessage_Danger('Vui lòng nhập tên công việc');
            return false;
        }
        _loaiCongViec.trim();

        var myData = {};
        var objLoaiCV = {
            ID: _id,
            TenLoaiTuVanLichHen: _loaiCongViec,
            TuVan_LichHen: 4,
            NguoiTao: userLogin,
            NgayTao: moment(new Date()).format('YYYY-MM-DD HH:mm:ss')
        }
        myData.objLoaiTVLH = objLoaiCV;

        if (self.IsAddTypeWork()) {
            _id = const_GuidEmpty; // assign to to check exist
        }

        ajaxHelper(ChamSocKhachHangUri + "Check_LoaiCongViec_Exist?loaicongviec=" + _loaiCongViec + '&idloaicv=' + _id + '&loaiTuVan=4', 'GET').done(function (boolReturn) {
            if (boolReturn) {
                ShowMessage_Danger('Loại công việc đã tồn tại');
            }
            else {
                if (self.IsAddTypeWork()) {
                    $.ajax({
                        url: DM_LoaiTVLHUri + "Post_LoaiTuVanLichHen",
                        type: 'POST',
                        async: true,
                        dataType: 'json',
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                        data: myData,
                        success: function (obj) {
                            if (obj.res === true) {
                                var item = obj.data;
                                self.LoaiCongViecs.unshift(item);
                                self.newCongViec().ID_LoaiTuVan(item.ID);
                                self.IsAddTypeWork(false);
                                $('span[id=spanCheckLoaiCongViec_' + item.ID + ']').append(element_appendCheck);

                                $(txtLoaiCongViec).text(item.TenLoaiTuVanLichHen);

                                ShowMessage_Success('Thêm mới loại công việc thành công');

                                var noiDung = 'Thêm mới loại công việc "' + _loaiCongViec;
                                var noiDungChiTiet = noiDung.concat('<br />- Nhân viên tạo: ', userLogin);
                                var objDiary = {
                                    ID_NhanVien: _idNhanVien,
                                    ID_DonVi: _idDonVi,
                                    ChucNang: 'Khách hàng - Công việc',
                                    LoaiNhatKy: 1,
                                    NoiDung: noiDung,
                                    NoiDungChiTiet: noiDungChiTiet,
                                }

                                Insert_NhatKyThaoTac_1Param(objDiary);
                            }
                        },
                        error: function (jqXHR, textStatus, errorThrow) {
                            ShowMessage_Danger('Thêm mới loại công việc thất bại');
                        },
                        complete: function () {
                            $('#modalTypeWork').modal('hide');
                        }
                    })
                }
                else {
                    myData.objLoaiTVLH.NguoiSua = userLogin;

                    $.ajax({
                        url: DM_LoaiTVLHUri + "Put_LoaiTuVanLichHen",
                        type: 'POST',
                        async: true,
                        dataType: 'json',
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                        data: myData,
                        success: function (obj) {
                            if (obj.res === true) {
                                for (var i = 0; i < self.LoaiCongViecs().length; i++) {
                                    if (self.LoaiCongViecs()[i].ID === _id) {
                                        self.LoaiCongViecs.remove(self.LoaiCongViecs()[i]);
                                        break;
                                    }
                                }
                                self.LoaiCongViecs.push(objLoaiCV);

                                $(txtLoaiCongViec).text(objLoaiCV.TenLoaiTuVanLichHen);
                                ShowMessage_Success('Cập nhật loại công việc thành công');
                            }
                        },
                        error: function (jqXHR, textStatus, errorThrow) {
                            ShowMessage_Danger('Cập nhật loại công việc thất bại');
                        },
                        complete: function () {
                            $('#modalTypeWork').modal('hide');
                        }
                    })
                }
            }
        })
    }

    self.showModalAddLoaiCV = function () {
        $('#modalTypeWork').modal('show');
        if (self.IsAddTypeWork()) {
            $('#titleTypeWork').text('Thêm mới loại công việc');
            self.newLoaiCongViec(new FormModel_NewLoaiCongViec());
        }
        else {
            $('#titleTypeWork').text('Cập nhật loại công việc');
            //self.newCongViec().ID_LoaiCongViec(item.ID);

            var itemEx = $.grep(self.LoaiCongViecs(), function (x) {
                return x.ID === self.newCongViec().ID_LoaiTuVan(); /*self.selectedLoaiCV();*/
            });
            if (itemEx.length > 0) {
                self.newLoaiCongViec().setdata(itemEx[0]);
            }
        }
    }

    self.changeTypeRemind_Before = function (item, isRemindBefore) {
        var thisObj = event.currentTarget;
        self.newCongViec().NhacTruoc($(thisObj).val());
    }

    self.changeTypeRemind_After = function () {
        var thisObj = event.currentTarget;
        self.newCongViec().NhacTruocLienHeLai($(thisObj).val());
    }

    self.filterLoaiCongViec = ko.observable();
    self.arrFilterLoaiCongViec = ko.computed(function () {
        var _filter = self.filterLoaiCongViec();

        if (_filter) {
            _filter = locdau(_filter);
        }

        return arrFilter = ko.utils.arrayFilter(self.LoaiCongViecs(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenLoaiTuVanLichHen).split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = (locdau(prod.TenLoaiTuVanLichHen).indexOf(_filter) >= 0 ||
                    sSearch.indexOf(_filter) >= 0
                );
            }
            return chon;
        });
    });

    //liên hệ
    self.filterLienHe = ko.observable();
    self.arrFilterLienHe = ko.computed(function () {
        var _filter = self.filterLienHe();
        if (_filter) {
            _filter = locdau(_filter);
        }

        return arrFilter = ko.utils.arrayFilter(self.listUserContact(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenLienHe).split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            var arr1 = locdau(prod.MaLienHe).split(/\s+/);
            var sSearch1 = '';

            for (var i = 0; i < arr1.length; i++) {
                sSearch1 += arr1[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = (locdau(prod.TenLienHe).indexOf(_filter) >= 0 ||
                    sSearch.indexOf(_filter) >= 0 || locdau(prod.MaLienHe).indexOf(_filter) >= 0 ||
                    sSearch1.indexOf(_filter) >= 0
                );
            }
            return chon;
        });
    });

    self.filterNhanVien = ko.observable();
    self.arrFilterNhanVien = ko.computed(function () {
        var _filter = self.filterNhanVien();
        if (_filter) {
            _filter = locdau(_filter);
        }

        return arrFilter = ko.utils.arrayFilter(self.NhanViens(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenNhanVien).split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            var arr1 = locdau(prod.MaNhanVien).split(/\s+/);
            var sSearch1 = '';

            for (var i = 0; i < arr1.length; i++) {
                sSearch1 += arr1[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = (locdau(prod.TenNhanVien).indexOf(_filter) >= 0 ||
                    sSearch.indexOf(_filter) >= 0 || locdau(prod.MaNhanVien).indexOf(_filter) >= 0 ||
                    sSearch1.indexOf(_filter) >= 0
                );
            }
            return chon;
        });
    });

    self.ChoseTypeWork = function (item) {
        self.filterLoaiCongViec('');
        $('#lstLoaiCongViec span').each(function () {
            $(this).empty();
        });

        if (item.ID == undefined) {
            $(txtLoaiCongViec).text('--- Chọn loại công việc ---');
            $('#btnDeleteTyepWork').hide();
            self.IsAddTypeWork(true);
            self.newCongViec().ID_LoaiTuVan(undefined);
        }
        else {
            $(txtLoaiCongViec).text(item.TenLoaiTuVanLichHen);
            self.IsAddTypeWork(false);
            self.newCongViec().ID_LoaiTuVan(item.ID);

            $('span[id=spanCheckLoaiCongViec_' + item.ID + ']').append(element_appendCheck);
        }
    }

    self.ChoseUserContact = function (item) {
        self.filterLienHe('');
        $('#lstLienHe span').each(function () {
            $(this).empty();
        });

        if (item.ID == undefined) {
            $(txtLienHe).text('--- Chọn liên hệ ---');
            self.newCongViec().ID_LienHe(undefined);
        }
        else {
            $(txtLienHe).text(item.TenLienHe);
            self.newCongViec().ID_LienHe(item.ID);

            $('span[id=spanCheckLienHe_' + item.ID + ']').append(element_appendCheck);
        }
    }

    self.ChoseStaffInCharge = function (item) {
        self.filterNhanVien('');

        $('#lstNhanVien span').each(function () {
            $(this).empty();
        });

        if (item.ID == undefined) {
            $(txtStaffIncharge).text('--- Chọn nhân viên ---');
            self.newCongViec().ID_NhanVien(undefined);
        }
        else {
            $(txtStaffIncharge).text(item.TenNhanVien);
            self.newCongViec().ID_NhanVien(item.ID);

            $('span[id=spanCheckNhanVien_' + item.ID + ']').append(element_appendCheck);
            $('span[id=spanCheckNhanVien_' + item.ID + ']').parent().parent().siblings().find("i.fa").removeClass('fa-check')
        }
    }

    self.formatDateTime_CV = function () {
        var thisObj = event.currentTarget;
        $(thisObj).datetimepicker(
            {
                format: "d/m/Y H:i",
                mask: true,
                timepicker: true,
            });
    }

    self.Change_StatusWork = function () {
        var thisObj = event.currentTarget;
        var status = $(thisObj).val();
        self.newCongViec().TrangThai(status);
    }

    // trang thai KH (loai KH) --- not use
    self.filterCusType = ko.observable();
    self.ChoseCustomeType = function (item) {
        self.filterCusType('');
        $('#lstCusType span').each(function () {
            $(this).empty();
        });

        if (item.ID == undefined) {
            if (self.IsCustomer()) {
                $('#txtStatusKH').text('--- Chọn trạng thái khách----');
            }
            else {
                $('#txtStatusKH').text('--- Chọn trạng thái NCC----');
            }
            self.newCongViec().ID_TrangThai(undefined);
        }
        else {
            $('#txtStatusKH').text(item.Name);
            self.newCongViec().ID_TrangThai(item.ID);

            $('span[id=spanCheckCusType_' + item.ID + ']').append(element_appendCheck);
        }
    }

    self.arrFilterCusType = ko.computed(function () {
        var _filter = self.filterCusType();

        if (_filter) {
            _filter = locdau(_filter);
        }

        return arrFilter = ko.utils.arrayFilter(self.LoaiKhachHang(), function (prod) {
            var chon = true;
            var arr = locdau(prod.Name).split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = (locdau(prod.Name).indexOf(_filter) >= 0 ||
                    sSearch.indexOf(_filter) >= 0
                );
            }
            return chon;
        });
    });

    self.CloseNV = function (item) {
        self.ListStaffShare.remove(item);
        if (self.ListStaffShare().length === 0) {
            $('#choose_NhanVien').append('<input type="text" style="background:#f0f1f1" id="dllNhanVien" readonly="readonly" class="dropdown" placeholder="Chọn nhân viên">');
        }
        $('#selec-all-NhanVien li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }

    self.ChoseStaffShare = function (item) {
        if (self.newCongViec().ID_NhanVien() !== item.ID) {
            var arrLCV = [];
            for (var i = 0; i < self.ListStaffShare().length; i++) {
                if ($.inArray(self.ListStaffShare()[i], arrLCV) === -1) {
                    arrLCV.push(self.ListStaffShare()[i].ID);
                }
            }
            if ($.inArray(item.ID, arrLCV) === -1) {
                self.ListStaffShare.push(item);
            }
            $('#selec-all-NhanVien li').each(function () {
                if ($(this).attr('id') === item.ID) {
                    $(this).find('.fa-check').remove();
                    $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                }
            });
            $('#choose_NhanVien input').remove();
        }
        else {
            ShowMessage_Danger("Nhân viên phối hợp không được trùng nhân viên phụ trách.");
            return false;
        }
    }

    self.ChoseFileUpload = function (elemet, event) {

        var files = event.target.files;// FileList object
        // Loop through the FileList and render image files as thumbnails.
        for (var i = 0; i < files.length; i++) {
            var f = files[i];
            // Only process image files.
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
                self.FileSelect([]);
                reader.onload = (function (theFile) {
                    return function (e) {
                        self.FileSelect.push(new FileModel(theFile, e.target.result));
                        //$('#txtTenFile').html(theFile.name);
                        self.newCongViec().FileDinhKem(theFile.name);
                        $('#clickXoaFile').show();
                        $('#workfile').show();
                    };
                })(f);
                // Read in the image file as a data URL.
                reader.readAsDataURL(f);
            }
        }
    };

    self.DeleteFile = function () {
        self.FileSelect([]);
        self.newCongViec().FileDinhKem('');
        $('#clickXoaFile').hide();
        $('#workfile').hide();
    };

    self.UpLoadFile = function (id) {
        var i = 0;
        if (i < self.FileSelect().length) {
            var formData = new FormData();
            formData.append("fileWork", self.FileSelect()[i].file);
            $.ajax({
                type: "POST",
                url: '/api/DanhMuc/DM_HangHoaAPI/' + "UpLoadFileCongViec/" + id,
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (response) {
                },
                error: function (jqXHR, textStatus, errorThrown) {
                }
            });
        }
        else {
            ajaxHelper(ChamSocKhachHangUri + 'UpdateCongViecXoaFile?idcv=' + id, 'GET').done(function (data) {
            });
        }
    }

    $('.check-all-day').on('change', function () {
        if ($(this).is(':checked')) {
            $('.newTimesingle').prop('disabled', true);
            self.newCongViec().CaNgay(true);
        }
        else {
            $('.newTimesingle').prop('disabled', false);
            self.newCongViec().CaNgay(false);
        }
    });

    $('.work-ttt-group-btn').on('click', 'a', function () {
        if ($(this).data('id') === 1) {
            $('#workdoitac').toggle();
        }
        else if ($(this).data('id') === 2) {
            $('#worknguoiphutrach').toggle();
        }
        else {
            $('#fileWork').click();
        }
    });

}