var FormModel_NewTrangThaiKH = function () {
    var self = this;
    self.ID = ko.observable();
    self.TenTrangThai = ko.observable();

    self.SetData = function (item) {
        self.ID(item.ID);
        self.TenTrangThai(item.Name);
    }
}

var PartialView_TrangThaiKH = function () {
    var self = this;
    var _userLogin = $('#txtTenTaiKhoan').text();
    const _idDonVi = $('#hd_IDdDonVi').val();
    var _idNhanVien = $('.idnhanvien').text();

    self.newObjTrangThaiKH = ko.observable(new FormModel_NewTrangThaiKH());
    self.booleanAddStatus = ko.observable(0);
    self.DoAction = ko.observable(false);// assign = true when delete/insert/update finished
    self.ObjTrangThaiChosing = ko.observableArray();

    function Enable_btnSaveTrangThai() {
        document.getElementById("btnLuuCongViec").disabled = false;
        document.getElementById("btnLuuCongViec").lastChild.data = "Lưu";
    }

    self.SaveTrangThai = function () {
        var apiDoiTuong = '/api/DanhMuc/DM_DoiTuongAPI/';
        var id = self.newObjTrangThaiKH().ID();
        var tenTrangThai = self.newObjTrangThaiKH().TenTrangThai();

        var noiDungDiary = '';

        var TrangThaiKH = {
            ID: id,
            TenTrangThai: tenTrangThai,
            Name: tenTrangThai,
            NguoiTao: _userLogin,
            NgayTao: moment(new Date()).format('YYYY-MM-DD HH:mm:ss'),
        }

        switch (self.booleanAddStatus()) {
            case 1:// insert
                noiDungDiary = 'Thêm mới';
                noiDungChiTiet = noiDungDiary.concat('<br /> - Ngày tạo: ', new Date(),
                    '<br />- Người tạo: ', _userLogin);
                $.ajax({
                    url: apiDoiTuong + "PostDM_DoiTuong_TrangThai",
                    type: 'POST',
                    async: true,
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    data: TrangThaiKH,
                    success: function (item) {
                        TrangThaiKH.ID = item.ID;
                        self.ObjTrangThaiChosing(TrangThaiKH);
                        self.DoAction(true);

                        ShowMessage_Success('Thêm mới trạng thái khách hàng thành công');
                    },
                    error: function (jqXHR, textStatus, errorThrow) {
                        ShowMessage_Danger('Thêm mới trạng thái khách hàng thất bại');
                    },
                    complete: function () {
                        Enable_btnSaveTrangThai();
                        $('#modalStatus_Customer').modal('hide');
                    }
                })
                break;
            case 2:// update
                noiDungDiary = 'Cập nhật';
                noiDungChiTiet = noiDungDiary.concat('<br /> - Ngày sửa: ', new Date(),
                    '<br />- Người sửa: ', _userLogin);

                TrangThaiKH.NguoiSua = _userLogin;

                $.ajax({
                    url: apiDoiTuong + "PutDM_DoiTuong_TrangThai",
                    type: 'PUT',
                    async: true,
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    data: TrangThaiKH,
                    success: function (result) {
                        self.DoAction(true);

                        if (result === '') {
                            ShowMessage_Success('Cập nhật trạng thái khách hàng thành công');
                            self.ObjTrangThaiChosing(TrangThaiKH);
                        }
                        else {
                            ShowMessage_Danger(result);
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrow) {
                        ShowMessage_Danger('Cập nhật trạng thái khách hàng thất bại');
                    },
                    complete: function () {
                        Enable_btnSaveTrangThai();
                        $('#modalStatus_Customer').modal('hide');
                    }
                })
                break;
            case 3:// delete
                noiDungDiary = 'Xóa';
                noiDungChiTiet = noiDungDiary.concat('<br /> - Ngày xóa: ', new Date(),
                    '<br />- Người xóa: ', _userLogin);

                self.ObjTrangThaiChosing({ ID: id });

                dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn xóa trạng thái "' + tenTrangThai + '"</b> không?', function () {
                    $.ajax({
                        type: "PUT",
                        url: apiDoiTuong + "Delete_DoiTuong_TrangThai?idTrangThai=" + id,
                        dataType: 'json',
                        contentType: 'application/json',
                        success: function (result) {
                            self.DoAction(true);

                            if (result === '') {
                                ShowMessage_Success('Xóa loại trạng thái khách hàng thành công');
                            }
                            else {
                                ShowMessage_Danger(result);
                            }
                            $('#modalStatus_Customer').modal('hide');
                        },
                        error: function (error) {
                            $('#modalPopuplgDelete').modal('hide');
                            ShowMessage_Danger('Xóa trạng thái khách hàng thất bại');
                        }
                    });
                })
                break;
        }

        noiDungDiary = noiDungDiary + ' trạng thái khách hàng';
        var objDiary = {
            ID_NhanVien: _idNhanVien,
            ID_DonVi: _idDonVi,
            ChucNang: 'Khách hàng - trạng thái',
            LoaiNhatKy: self.booleanAddStatus(),
            NoiDung: noiDungDiary,
            NoiDungChiTiet: noiDungChiTiet,
        }
        Insert_NhatKyThaoTac_1Param(objDiary);
    }

    self.DeleteTrangThai = function () {
        self.DoAction(false);
        self.booleanAddStatus(3);
        self.SaveTrangThai();
    }
}
