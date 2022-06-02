var dtNow = new Date();

var FormModel_NewLoaiCongViec = function () {
    var self = this;
    self.ID = ko.observable();
    self.LoaiCongViec = ko.observable();
    self.TrangThai = ko.observable('2'); // 2.Dang xu ly
    self.LoaiCongViec = ko.observable();
    self.NguoiTao = ko.observable();
    self.NgayTao = ko.observable();
    self.NguoiSua = ko.observable();
    self.NgaySua = ko.observable();

    self.setdata = function (item) {
        self.ID(item.ID);
        self.LoaiCongViec(item.LoaiCongViec);
        self.TrangThai(item.TrangThai);
        self.LoaiCongViec(item.LoaiCongViec);
        self.NguoiTao(item.NguoiTao);
        self.NgayTao(item.NgayTao);
        self.NguoiSua(item.NguoiSua);
        self.NgaySua(item.NgaySua);
    }
}

var FormModel_NewCongViec = function () {
    var self = this;
    self.ID = ko.observable();
    self.ID_LoaiCongViec = ko.observable();
    self.ID_KhachHang = ko.observable();
    self.ID_LienHe = ko.observable();
    self.ID_DonVi = ko.observable();
    self.ThoiGianTu = ko.observable(moment(dtNow).format('DD/MM/YYYY HH:mm'));
    self.ThoiGianDen = ko.observable(moment(dtNow).format('DD/MM/YYYY HH:mm'));
    self.NhacTruoc = ko.observable();
    self.ID_NhanVienChiaSe = ko.observable();
    self.NoiDung = ko.observable();
    self.KetQuaCongViec = ko.observable();
    self.LyDoHenLai = ko.observable();
    self.ThoiGianLienHeLai = ko.observable();
    self.NhacTruocLienHeLai = ko.observable();
    self.TrangThai = ko.observable('1');
    self.ID_CustomerType = ko.observable();

    // reset when show modal Add work
    $('#txtLoaiCongViec').text('--- Chọn loại công việc ---');
    $('#txtNhanVien_CV').text('--- Chọn nhân viên ---');
    $('#txtLienHe_CV').text('--- Chọn liên hệ---');
    $('#ddlRemindBefore').val('0');
    $('#ddlRemindAfter').val('0');
    $('#typeTrangThaiCV').val('1');

    self.SetData = function (item) {
        self.ID(item.ID);
        self.ID_LoaiCongViec(item.ID_LoaiCongViec);
        self.ID_KhachHang(item.ID_KhachHang);
        self.ID_LienHe(item.ID_LienHe);
        self.ID_DonVi(item.ID_DonVi);
        self.NhacTruoc(item.NhacTruoc);
        self.ID_NhanVienChiaSe(item.ID_NhanVienChiaSe);
        self.NoiDung(item.NoiDung);
        self.KetQuaCongViec(item.KetQuaCongViec);
        self.LyDoHenLai(item.LyDoHenLai);
        self.ThoiGianLienHeLai(item.ThoiGianLienHeLai);
        self.NhacTruocLienHeLai(item.NhacTruocLienHeLai);
        self.TrangThai(item.TrangThai);
        self.ID_CustomerType(item.ID_CustomerType);

        $('#ddlRemindBefore').val(item.NhacTruoc);
        $('#ddlRemindAfter').val(item.NhacTruocLienHeLai);
        $('#typeTrangThaiCV').val(item.TrangThai);

        if (item.LoaiCongViec !== '') {
            $('#txtLoaiCongViec').text(item.LoaiCongViec);
            $('span[id=spanCheckLoaiCongViec_' + item.ID_LoaiCongViec + ']')
                .append(element_appendCheck);
            $('span[id=spanCheckLoaiCongViec_' + item.ID_LoaiCongViec + ']').parent().parent().siblings().find("i.fa").removeClass('fa-check')
        }

        if (item.ID_CustomerType !== const_GuidEmpty) {
            $('span[id=spanCheckCusType_' + item.ID_CustomerType + ']')
                .append(element_appendCheck);

            // assign text fro input
            var nameCusType = $('span[id=spanCheckCusType_' + item.ID_CustomerType + ']').prev().text();
            $('#txtStatusKH').text(nameCusType);
        }

        if (item.TenDoiTuong !== '') {
            $('#txtKhachHang').val(item.TenDoiTuong);
        }

        if (item.TenLienHe !== '') {
            $('#txtLienHe_CV').text(item.TenLienHe);
            $('span[id=spanCheckLienHe_' + item.ID_LienHe + ']').append(element_appendCheck);
        }

        if (item.TenNVChiaSe !== '') {
            $('#txtNhanVien_CV').text(item.TenNVChiaSe);
            $('span[id=spanCheckNhanVien_' + item.ID_NhanVienChiaSe + ']').append(element_appendCheck);
        }

        if (item.ThoiGianTu !== null) {
            self.ThoiGianTu(moment(item.ThoiGianTu).format('DD/MM/YYYY HH:mm'));
        }
        else {
            self.ThoiGianTu('');
        }

        if (item.ThoiGianDen !== null) {
            self.ThoiGianDen(moment(item.ThoiGianDen).format('DD/MM/YYYY HH:mm'));
        }
        else {
            self.ThoiGianDen('');
        }

        if (item.ThoiGianLienHeLai !== null) {
            self.ThoiGianLienHeLai(moment(item.ThoiGianLienHeLai).format('DD/MM/YYYY HH:mm'));
        }
        else {
            self.ThoiGianLienHeLai('');
        }
    };
};

var PartialView_CongViec = function () {
    var self = this;
    var ChamSocKhachHangUri = '/api/DanhMuc/ChamSocKhachHangAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    var userLogin = $('#txtTenTaiKhoan').val().trim();
    const txtLoaiCongViec = '#txtLoaiCongViec';
    const txtLienHe = '#txtLienHe_CV';
    const txtNhanVien = '#txtNhanVien_CV';
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
    self.IsContactAgain = ko.observable(false);
    self.IsUpdateCustomerType = ko.observable(false);;

    function getallLoaiCongViec() {
        ajaxHelper(ChamSocKhachHangUri + 'GetAllLoaiCongViec', 'GET').done(function (data) {
            self.LoaiCongViecs(data);
        });
    }
    getallLoaiCongViec();

    self.showPopupAddKH = function () {

        // khong can check quyen vi da an button nay roi
        self.resetTextBox();
        self.booleanAdd(true);
        $('#modalPopuplg_KH').modal('show');
        $('#lblTitleKH').text('Thêm khách hàng');
        self.newDoiTuong().LaCaNhan(true);

        self.HaveImage_Select(false);
        self.FilesSelect([]);
        $('#file').val('');
        Reset_NhomDoiTuong();
        Reset_NguonKhach();

        self.IsOpenModalCus(true);
    }

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
        var idLoaiCongViec = self.newCongViec().ID_LoaiCongViec();
        var idKhachHang = self.newCongViec().ID_KhachHang();
        var idLienHe = self.newCongViec().ID_LienHe();
        var idNhanVienChiaSe = self.newCongViec().ID_NhanVienChiaSe();
        var trangThai = self.newCongViec().TrangThai();
        var ketquaCongViec = self.newCongViec().KetQuaCongViec();
        var lydoLienHeLai = self.newCongViec().LyDoHenLai();
        var noiDung = self.newCongViec().NoiDung();
        var nhacTruoc = self.newCongViec().NhacTruoc();
        var nhacTruocLienHeLai = self.newCongViec().NhacTruocLienHeLai();
        var thoiGianTu = $('#txtThoiGianTu').val();
        var thoiGianDen = $('#txtThoiGianDen').val();
        var thoiGianLienHeLai = $('#txtThoiGianLienHeLai').val();

        var itemEx = $.grep(self.LoaiCongViecs(), function (item) {
            return item.ID === idLoaiCongViec;
        });
        if (itemEx.length === 0) {
            ShowMessage_Danger('Vui lòng chọn loại công việc');
            Enable_btnSaveCongViec();
            $(txtLoaiCongViec).focus();
            return false;
        }

        if (moment(thoiGianTu, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') === "Invalid date") {
            ShowMessage_Danger('Vui lòng chọn thời gian bắt đầu công việc');
            Enable_btnSaveCongViec();
            $('#txtThoiGianTu').select();
            return false;
        }
        if ((moment(thoiGianTu, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') > moment(thoiGianDen, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm')) && moment(thoiGianDen, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') !== "Invalid date") {
            ShowMessage_Danger('Thời gian kết thúc phải lớn hơn thời gian bắt đầu');
            Enable_btnSaveCongViec();
            return false;
        }

        if (idNhanVienChiaSe === const_GuidEmpty) {
            idNhanVienChiaSe = null;
        }
        var CongViec = {
            ID: id,
            ID_LoaiCongViec: idLoaiCongViec,
            ID_KhachHang: idKhachHang,
            ID_LienHe: idLienHe,
            ID_DonVi: _idDonVi,
            ID_NhanVienQuanLy: _idNhanVien,
            ID_NhanVienChiaSe: idNhanVienChiaSe,
            ThoiGianTu: moment(thoiGianTu, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm'),
            ThoiGianDen: moment(thoiGianDen, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') === "Invalid date" ? null : moment(thoiGianDen, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm'),
            NhacTruoc: nhacTruoc === undefined ? 0 : nhacTruoc,
            NoiDung: noiDung,
            ThoiGianLienHeLai: moment(thoiGianLienHeLai, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') === "Invalid date" ? null : moment(thoiGianLienHeLai, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm'),
            NhacTruocLienHeLai: nhacTruocLienHeLai === undefined ? 0 : nhacTruocLienHeLai,
            NguoiTao: userLogin,
            TrangThai: trangThai,
            KetQuaCongViec: ketquaCongViec,
            LyDoHenLai: lydoLienHeLai,
        }
        var myData = {};
        myData.objCongViec = CongViec;

        console.log(CongViec)

        // used to save diary
        var loaiCV = $('#txtLoaiCongViec').text();
        var tenDoiTuong = $('#txtKhachHang').val();
        var nvThucHien = '';
        var itemNV = $.grep(self.NhanViens(), function (x) {
            return x.ID === _idNhanVien;
        });
        if (itemNV.length > 0) {
            nvThucHien = itemNV[0].TenNhanVien;
        }

        if (self.booleanAddCV() === true) {
            $.ajax({
                url: ChamSocKhachHangUri + "PostNS_CongViec",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: myData,
                success: function (item) {
                    ShowMessage_Success('Thêm mới công việc thành công');

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
            if (self.IsContactAgain() && thoiGianLienHeLai === '') {
                ShowMessage_Danger('Vui lòng chọn thời gian liên hệ lại');
                Enable_btnSaveCongViec();
                return false;
            }

            // Neu truoc do co lien he lai, sau do cap nhat khong lien he: reset thoiGianLienHeLai, NhacLienHeLai
            if (!self.IsContactAgain()) {
                myData.objCongViec.ThoiGianLienHeLai = null;
                myData.objCongViec.NhacTruocLienHeLai = 0;
                myData.objCongViec.LyDoHenLai = '';
                myData.objCongViec.NguoiSua = userLogin;
            }

            $.ajax({
                url: ChamSocKhachHangUri + "PutNS_CongViec",
                type: 'PUT',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: myData,
                success: function (item) {
                    ShowMessage_Success('Cập nhật công việc thành công');

                    var noiDungDiary = 'Cập nhật công việc "' + loaiCV + '" với khách hàng ' + tenDoiTuong;
                    var noiDungChiTiet = noiDungDiary.concat('<br /> - Thời gian: ', thoiGianTu, ' - ', thoiGianDen,
                        '<br/>- Nội dung: ', noiDung, '<br />- Nhân viên thực hiện: ', nvThucHien)

                    if (self.IsContactAgain()) {
                        noiDungChiTiet = noiDungChiTiet.concat('<br /> - Thời gian hẹn lại: ', thoiGianLienHeLai);
                    }

                    if (trangThai === 2) {
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
        var idCusType = self.newCongViec().ID_CustomerType();
        if (idCusType === undefined) {
            idCusType = const_GuidEmpty;
        }

        console.log('idKhachHang ', idKhachHang, 'idCusType ', idCusType)
        ajaxHelper(ChamSocKhachHangUri + 'Update_LoaiKhachHang_DMDoituong?idDoituong=' + idKhachHang + '&cusType=' + idCusType, 'POST').done(function (x) {
            if (x === true) {
                self.IsUpdateCustomerType(true);
            }
        })
    }

    self.DeleteLoaiCongViec = function (item) {
        var idDelete = self.newCongViec().ID_LoaiCongViec();

        var loaiCV = self.newLoaiCongViec().LoaiCongViec();

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
        var _loaiCongViec = self.newLoaiCongViec().LoaiCongViec();
        if (_loaiCongViec === "" || _loaiCongViec === null || _loaiCongViec === undefined) {
            ShowMessage_Danger('Vui lòng nhập tên công việc');
            return false;
        }
        _loaiCongViec.trim();

        var myData = {};
        var objLoaiCV = {
            ID: _id,
            LoaiCongViec: _loaiCongViec,
            NguoiTao: userLogin
        }
        myData.objLoaiCongViec = objLoaiCV;

        if (self.IsAddTypeWork()) {
            _id = 'null'; // assign to to check exist
        }

        ajaxHelper(ChamSocKhachHangUri + "Check_LoaiCongViec_Exist?loaicongviec=" + _loaiCongViec + '&idloaicv=' + _id, 'POST').done(function (boolReturn) {
            if (boolReturn) {
                ShowMessage_Danger('Loại công việc đã tồn tại');
            }
            else {
                if (self.IsAddTypeWork()) {
                    $.ajax({
                        url: ChamSocKhachHangUri + "PostLoaiCongViec",
                        type: 'POST',
                        async: true,
                        dataType: 'json',
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                        data: myData,
                        success: function (item) {
                            self.LoaiCongViecs.unshift(item);
                            self.newCongViec().ID_LoaiCongViec(item.ID);
                            self.IsAddTypeWork(false);
                            $('span[id=spanCheckLoaiCongViec_' + item.ID + ']').append(element_appendCheck);
                            $('span[id=spanCheckLoaiCongViec_' + item.ID_LoaiCongViec + ']').parent().parent().siblings().find("i.fa").removeClass('fa-check')
                            $(txtLoaiCongViec).text(item.LoaiCongViec);

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
                    myData.objLoaiCongViec.NguoiSua = userLogin;

                    $.ajax({
                        url: ChamSocKhachHangUri + "PutLoaiCongViec",
                        type: 'POST',
                        async: true,
                        dataType: 'json',
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                        data: myData,
                        success: function (item) {
                            for (var i = 0; i < self.LoaiCongViecs().length; i++) {
                                if (self.LoaiCongViecs()[i].ID === _id) {
                                    self.LoaiCongViecs.remove(self.LoaiCongViecs()[i]);
                                    break;
                                }
                            }
                            self.LoaiCongViecs.push(item);

                            $(txtLoaiCongViec).text(item.LoaiCongViec);
                            ShowMessage_Success('Cập nhật loại công việc thành công');
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
                return x.ID === self.newCongViec().ID_LoaiCongViec(); /*self.selectedLoaiCV();*/
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
            var arr = locdau(prod.LoaiCongViec).split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = (locdau(prod.LoaiCongViec).indexOf(_filter) >= 0 ||
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

            for (let i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            var arr1 = locdau(prod.MaLienHe).split(/\s+/);
            var sSearch1 = '';

            for (let i = 0; i < arr1.length; i++) {
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

            for (let i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            var arr1 = locdau(prod.MaNhanVien).split(/\s+/);
            var sSearch1 = '';

            for (let i = 0; i < arr1.length; i++) {
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

        if (item.ID === undefined) {
            $(txtLoaiCongViec).text('--- Chọn loại công việc ---');
            $('#btnDeleteTyepWork').hide();
            self.IsAddTypeWork(true);
            self.newCongViec().ID_LoaiCongViec(undefined);
        }
        else {
            $(txtLoaiCongViec).text(item.LoaiCongViec);
            self.IsAddTypeWork(false);
            self.newCongViec().ID_LoaiCongViec(item.ID);

            $('span[id=spanCheckLoaiCongViec_' + item.ID + ']').append(element_appendCheck);
            $('span[id=spanCheckLoaiCongViec_' + item.ID_LoaiCongViec + ']').parent().parent().siblings().find("i.fa").removeClass('fa-check')
        }
    }

    self.ChoseUserContact = function (item) {
        self.filterLienHe('');
        $('#lstLienHe span').each(function () {
            $(this).empty();
        });

        if (item.ID === undefined) {
            $(txtLienHe).text('--- Chọn liên hệ ---');
            self.newCongViec().ID_LienHe(undefined);
        }
        else {
            $(txtLienHe).text(item.TenLienHe);
            self.newCongViec().ID_LienHe(item.ID);

            $('span[id=spanCheckLienHe_' + item.ID + ']').append(element_appendCheck);
        }
    }

    self.ChoseStaffShare = function (item) {
        self.filterNhanVien('');

        $('#lstNhanVien span').each(function () {
            $(this).empty();
        });

        if (item.ID === undefined) {
            $(txtNhanVien).text('--- Chọn nhân viên ---');
            self.newCongViec().ID_NhanVienChiaSe(undefined);
        }
        else {
            $(txtNhanVien).text(item.TenNhanVien);
            self.newCongViec().ID_NhanVienChiaSe(item.ID);

            $('span[id=spanCheckNhanVien_' + item.ID + ']').append(element_appendCheck);
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

    // trang thai KH (loai KH)
    self.filterCusType = ko.observable();
    self.ChoseCustomeType = function (item) {
        self.filterCusType('');
        $('#lstCusType span').each(function () {
            $(this).empty();
        });

        if (item.ID === undefined) {
            if (self.IsCustomer()) {
                $('#txtStatusKH').text('--- Chọn trạng thái khách----');
            }
            else {
                $('#txtStatusKH').text('--- Chọn trạng thái NCC----');
            }
            self.newCongViec().ID_CustomerType(undefined);
        }
        else {
            $('#txtStatusKH').text(item.Name);
            self.newCongViec().ID_CustomerType(item.ID);

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
}