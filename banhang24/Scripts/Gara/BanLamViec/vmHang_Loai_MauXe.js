var vmHangXe = new Vue({
    el: '#ThemMoiHangXe',
    created: function () {
        var self = this;
        self.GuidEmpty = '00000000-0000-0000-0000-000000000000';
    },
    data: {
        urlApi: {
            GaraApi: '/api/DanhMuc/GaraAPI/'
        },
        isNew: true,
        saveOK: false,
        error: '',
        ID: '',
        MaHangXe: '',
        TenHangXe: '',
        TrangThai: '1',
        Logo: '/Content/images/anhhh/logo24.png',
        LogoOld: '',
        inforOld: {},
        FilesSelect: [],
        role: {},
    },
    methods: {
        ShowModalAddNew: function () {
            var self = this;
            self.isNew = true;
            self.saveOK = false;
            self.ID = '';
            self.MaHangXe = '';
            self.TenHangXe = '';
            self.TrangThai = '1';
            self.Logo = '/Content/images/anhhh/logo24.png';
            self.LogoOld = '';
            $('#ThemMoiHangXe').modal('show');
        },
        ShowModalUpdate: function (item) {
            var self = this;
            self.isNew = false;
            self.saveOK = false;
            self.ID = item.ID;
            self.MaHangXe = item.MaHangXe;
            self.TenHangXe = item.TenHangXe;
            self.TrangThai = item.TrangThai;
            self.Logo = item.Logo;
            self.LogoOld = item.Logo;
            self.inforOld = item;
            $('#ThemMoiHangXe').modal('show');
        },
        ChoseLogo: function () {
            var self = this;
            var files = event.target.files;

            var checkSize = commonStatisJs.checkSizeImage(event);
            if (!checkSize) {
                return;
            }

            for (let i = 0; i < files.length; i++) {
                let f = files[i];

                // Only process image files.
                if (!f.type.match('image.*')) {
                    continue;
                }

                let reader = new FileReader();
                // Closure to capture the file information.
                reader.onload = (function (theFile) {
                    return function (e) {
                        self.Logo = e.target.result;
                        self.FilesSelect =
                            [{
                                file: theFile,
                                URLAnh: e.target.result,
                            }]
                    };
                })(f);
                // Read in the image file as a data URL.
                reader.readAsDataURL(f);
            }
        },
        Delete_LogoOld: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.LogoOld)) {
                return;
            }
            var myData = {
                lstFile: [self.LogoOld],
            }
            ajaxHelper('/api/DanhMuc/DM_DoiTuongAPI/' + 'DeleteFile_inFolder', 'POST', myData).done(function (x) {
            });
        },
        InsertImage: function () {
            var self = this;
            let formData = new FormData();
            for (let i = 0; i < self.FilesSelect.length; i++) {
                formData.append("file", self.FilesSelect[i].file);
            }
            $.ajax({
                type: "POST",
                url: '/api/DanhMuc/DM_DoiTuongAPI/' + "ImageUpload?rootFolder=LogoHangXe&subFolder=" + self.MaHangXe,
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                async: false,
            }).done(function (x) {
            });
        },
        AddNew_HangXe: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.TenHangXe)) {
                commonStatisJs.ShowMessageDanger('Vui lòng nhập tên hãng');
                return;
            }
            if (commonStatisJs.CheckNull(self.MaHangXe)) {
                self.MaHangXe = locdau(self.TenHangXe).toUpperCase();
            }
            var logo = '';
            if (self.FilesSelect.length > 0) {
                // ImageUpload/SubDomain/LogoHangXe/a.png
                logo = '/ImageUpload/'.concat(VHeader.SubDomain, '/LogoHangXe/', self.MaHangXe, '/', self.FilesSelect[0].file.name)
            }
            var myData = {
                MaHangXe: self.MaHangXe,
                TenHangXe: self.TenHangXe,
                Logo: logo,
                TrangThai: self.TrangThai,
            }

            var diary = {
                ID_DonVi: vmThemMoiXe.inforLogin.ID_DonVi,
                ID_NhanVien: vmThemMoiXe.inforLogin.ID_NhanVien,
                ChucNang: 'Hãng xe',
            }
            self.CheckSave().then(function (data) {
                if (data.res === true) {
                    if (data.dataSoure.TenHangXe) {
                        commonStatisJs.ShowMessageDanger("Hãng xe đã tồn tại.");
                    }
                    else {
                        var sTrangThai = ', Trạng thái: '.concat(parseInt(self.TrangThai) == 1 ? 'Hoạt động' : 'Ngừng hoạt động');
                        if (self.isNew) {
                            myData.ID = self.GuidEmpty;
                            myData.NguoiTao = vmThemMoiXe.inforLogin.UserLogin;

                            ajaxHelper(self.urlApi.GaraApi + 'Post_HangXe', 'POST', myData).done(function (x) {
                                if (x.res === true) {
                                    self.saveOK = true;
                                    self.ID = x.dataSoure.ID;
                                    commonStatisJs.ShowMessageSuccess('Thêm mới hãng xe thành công');

                                    diary.NoiDung = "Thêm mới hãng xe " + self.TenHangXe;
                                    diary.NoiDungChiTiet = "Thêm mới hãng xe ".concat(self.TenHangXe, ', Người tạo: ', vmThemMoiXe.inforLogin.UserLogin, sTrangThai);
                                    diary.LoaiNhatKy = 1;
                                    Insert_NhatKyThaoTac_1Param(diary);

                                    myData.ID = self.ID;
                                    myData.NguoiSua = vmThemMoiXe.inforLogin.UserLogin;

                                    self.Delete_LogoOld();
                                    self.InsertImage(myData);
                                }
                                else {
                                    self.saveOK = false;
                                    commonStatisJs.ShowMessageDanger(x.mess);
                                }
                            });
                        }
                        else {
                            myData.ID = self.ID;
                            myData.NguoiSua = vmThemMoiXe.inforLogin.UserLogin;

                            ajaxHelper(self.urlApi.GaraApi + 'Put_HangXe', 'POST', myData).done(function (x) {
                                if (x.res === true) {
                                    self.saveOK = true;
                                    diary.NoiDung = "Cập nhật hãng xe " + self.TenHangXe;
                                    diary.NoiDungChiTiet = "Cập nhật hãng xe ".concat(self.TenHangXe, ', Người sửa: ', vmThemMoiXe.inforLogin.UserLogin, sTrangThai,
                                        ' <br/ > <b> Thông tin cũ: </b> Mã hãng xe: ', self.inforOld.MaHangXe, ', Tên hãng: ', self.inforOld.TenHangXe);
                                    diary.LoaiNhatKy = 2;
                                    Insert_NhatKyThaoTac_1Param(diary);

                                    self.Delete_LogoOld();
                                    self.InsertImage(myData);
                                    commonStatisJs.ShowMessageSuccess('Cập nhật hãng xe thành công');
                                }
                                else {
                                    self.saveOK = false;
                                }
                            })
                        }
                        $('#ThemMoiHangXe').modal('hide');
                    }
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        },
        CheckSave: function () {
            let self = this;
            let datacheck = {};
            datacheck.TenHangXe = self.TenHangXe;
            datacheck.New = self.isNew;
            datacheck.ID = self.ID;
            return $.ajax({
                url: self.urlApi.GaraApi + "CheckSaveHangXe",
                type: 'POST',
                dataType: 'json',
                data: datacheck,
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            });
        },
        Delete_HangXe: function () {
            let self = this;
            commonStatisJs.ConfirmDialog_OKCancel('Xác nhận xóa', 'Bạn có chắc chắn muốn xóa hãng xe <b> ' + self.TenHangXe + ' </b> không ? ', function () {
                ajaxHelper(self.urlApi.GaraApi + 'Delete_HangXe/' + self.ID, 'DELETE').done(function (x) {
                    if (x.res === true) {
                        commonStatisJs.ShowMessageSuccess('Xóa hãng xe thành công');

                        var diary = {
                            ID_DonVi: vmThemMoiXe.inforLogin.ID_DonVi,
                            ID_NhanVien: vmThemMoiXe.inforLogin.ID_NhanVien,
                            ChucNang: 'Hãng xe',
                            LoaiNhatKy: 3,
                            NoiDung: 'Xóa hãng xe ' + self.TenHangXe,
                            NoiDungChiTiet: 'Xóa hãng xe '.concat(self.TenHangXe, ', Người xóa: ', vmThemMoiXe.inforLogin.UserLogin),
                        }
                        Insert_NhatKyThaoTac_1Param(diary);
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(x.mes);
                    }
                    $('#ThemMoiHangXe').modal('hide');
                });
            })
        },
    }
})

var vmLoaiXe = new Vue({
    el: '#ThemMoiLoaiXe',
    created: function () {
        var self = this;
        self.GuidEmpty = '00000000-0000-0000-0000-000000000000';
    },
    data: {
        urlApi: {
            GaraApi: '/api/DanhMuc/GaraAPI/'
        },
        isNew: true,
        saveOK: false,
        ID: '',
        MaLoaiXe: '',
        TenLoaiXe: '',
        TrangThai: '1',
        inforOld: {},
        role: {}
    },
    methods: {
        ShowModalAddNew: function () {
            var self = this;
            self.isNew = true;
            self.saveOK = false;
            self.ID = '';
            self.MaLoaiXe = '';
            self.TenLoaiXe = '';
            self.TrangThai = '1';
            $('#ThemMoiLoaiXe').modal('show');
        },
        ShowModalUpdate: function (item) {
            var self = this;
            self.isNew = false;
            self.saveOK = false;
            self.ID = item.ID;
            self.MaLoaiXe = item.MaLoaiXe;
            self.TenLoaiXe = item.TenLoaiXe;
            self.TrangThai = item.TrangThai.toString();
            self.inforOld = item;
            $('#ThemMoiLoaiXe').modal('show');
        },
        AddNew_LoaiXe: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.TenLoaiXe)) {
                commonStatisJs.ShowMessageDanger('Vui lòng nhập tên loại xe');
                return;
            }
            if (commonStatisJs.CheckNull(self.MaLoaiXe)) {
                self.MaLoaiXe = locdau(self.TenLoaiXe).toUpperCase();
            }
            var myData = {
                MaLoaiXe: self.MaLoaiXe,
                TenLoaiXe: self.TenLoaiXe,
                TrangThai: self.TrangThai,
            }
            var diary = {
                ID_DonVi: vmThemMoiXe.inforLogin.ID_DonVi,
                ID_NhanVien: vmThemMoiXe.inforLogin.ID_NhanVien,
                ChucNang: 'Loại xe',
            }
            var sTrangThai = ', Trạng thái: '.concat(parseInt(self.TrangThai) == 1 ? 'Hoạt động' : 'Ngừng hoạt động');

            self.CheckSave().then(function (data) {
                if (data.res === true) {
                    if (data.dataSoure.TenLoaiXe) {
                        commonStatisJs.ShowMessageDanger("Loại xe đã tồn tại.");
                    }
                    else {
                        if (self.isNew) {
                            myData.ID = self.GuidEmpty;
                            myData.NguoiTao = vmThemMoiXe.inforLogin.UserLogin;

                            ajaxHelper(self.urlApi.GaraApi + 'Post_LoaiXe', 'POST', myData).done(function (x) {
                                if (x.res === true) {
                                    self.saveOK = true;
                                    self.ID = x.dataSoure.ID;
                                    commonStatisJs.ShowMessageSuccess('Thêm mới loại xe thành công');

                                    diary.NoiDung = "Thêm mới loại xe " + self.TenLoaiXe;
                                    diary.NoiDungChiTiet = "Thêm mới loại xe ".concat(self.TenLoaiXe,
                                        ', Người tạo: ', vmThemMoiXe.inforLogin.UserLogin, sTrangThai);
                                    diary.LoaiNhatKy = 1;
                                    Insert_NhatKyThaoTac_1Param(diary);
                                }
                                else {
                                    self.saveOK = false;
                                    commonStatisJs.ShowMessageDanger(x.mes);
                                }
                                $('#ThemMoiLoaiXe').modal('hide');
                            });
                        }
                        else {
                            myData.ID = self.ID;
                            myData.NguoiSua = vmThemMoiXe.inforLogin.UserLogin;

                            ajaxHelper(self.urlApi.GaraApi + 'Put_LoaiXe', 'POST', myData).done(function (x) {
                                if (x.res === true) {
                                    self.saveOK = true;
                                    commonStatisJs.ShowMessageSuccess('Cập nhật loại xe thành công');

                                    diary.NoiDung = "Cập nhật loại xe " + self.TenHangXe;
                                    diary.NoiDungChiTiet = "Cập nhật loại xe ".concat(self.TenLoaiXe, ', Người sửa: ',
                                        vmThemMoiXe.inforLogin.UserLogin, sTrangThai,
                                        ' <br /> <b> Thông tin cũ: </b> Mã: ', self.inforOld.MaLoaiXe, ', Tên: ', self.inforOld.TenLoaiXe);
                                    diary.LoaiNhatKy = 2;
                                    Insert_NhatKyThaoTac_1Param(diary);
                                }
                                else {
                                    self.saveOK = false;
                                    commonStatisJs.ShowMessageDanger(x.mes);
                                }
                                $('#ThemMoiLoaiXe').modal('hide');
                            });
                        }
                    }
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        },
        CheckSave: function () {
            let self = this;
            let datacheck = {};
            datacheck.TenLoaiXe = self.TenLoaiXe;
            datacheck.New = self.isNew;
            datacheck.ID = self.ID;
            return $.ajax({
                url: self.urlApi.GaraApi + "CheckSaveLoaiXe",
                type: 'POST',
                dataType: 'json',
                data: datacheck,
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            });
        },
        Delete_LoaiXe: function () {
            let self = this;
            commonStatisJs.ConfirmDialog_OKCancel('Xác nhận xóa', 'Bạn có chắc chắn muốn xóa loại xe <b> ' + self.TenLoaiXe + ' </b> không ? ', function () {
                ajaxHelper(self.urlApi.GaraApi + 'Delete_LoaiXe/' + self.ID, 'DELETE').done(function (x) {
                    if (x.res === true) {
                        commonStatisJs.ShowMessageSuccess('Xóa loại xe thành công');

                        var diary = {
                            ID_DonVi: vmThemMoiXe.inforLogin.ID_DonVi,
                            ID_NhanVien: vmThemMoiXe.inforLogin.ID_NhanVien,
                            ChucNang: 'Loại xe',
                            LoaiNhatKy: 3,
                            NoiDung: 'Xóa loại xe ' + self.TenLoaiXe,
                            NoiDungChiTiet: 'Xóa loại xe '.concat(self.TenLoaiXe, ', Người xóa: ', vmThemMoiXe.inforLogin.UserLogin),
                        }
                        Insert_NhatKyThaoTac_1Param(diary);
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(x.mes);
                    }
                    $('#ThemMoiLoaiXe').modal('hide');
                });
            })
        }
    }
})

var vmMauXe = new Vue({
    el: '#ThemMoiMauXe',
    created: function () {
        var self = this;
        self.GuidEmpty = '00000000-0000-0000-0000-000000000000';
    },
    components: {
        'car-manufacturer': ComponentChoseManufacturer,
        'car-type': ComponentChoseTypeCar
    },
    data: {
        urlApi: {
            GaraApi: '/api/DanhMuc/GaraAPI/'
        },
        isNew: true,
        saveOK: false,
        ID: '',
        ID_HangXe: null,
        ID_LoaiXe: null,
        TenMauXe: '',
        TrangThai: '1',
        GhiChu: '',
        inforOld: {},
        role: {},
        hangxe: {
            ID: null,
            TenHangXe: '',
        },
        loaixe: {
            ID: null,
            TenLoaiXe: '',
        },
    },
    methods: {
        ShowModalAddNew: function () {
            var self = this;
            self.isNew = true;
            self.saveOK = false;
            self.ID = '';
            self.ID_HangXe = self.GuidEmpty;
            self.ID_LoaiXe = self.GuidEmpty;
            self.TrangThai = '1';
            self.TenMauXe = '';
            self.hangxe.TenHangXe = '';
            self.loaixe.TenLoaiXe = '';
            $('#ThemMoiMauXe').modal('show');
        },
        changeManufacturerParent: function (item) {
            var self = this;
            self.hangxe = item;
            self.ID_HangXe = item.ID;
        },
        changeTypeCarParent: function (item) {
            var self = this;
            self.loaixe = item;
            self.ID_LoaiXe = item.ID;
            $(event.currentTarget).closest('div').hide();
        },
        ShowModalUpdate: function (item) {
            var self = this;
            self.isNew = false;
            self.saveOK = false;
            self.ID = item.ID;
            self.TenMauXe = item.TenMauXe;
            self.ID_HangXe = item.ID_HangXe;
            self.ID_LoaiXe = item.ID_LoaiXe;
            self.TrangThai = item.TrangThai.toString();
            self.GhiChu = item.GhiChu;

            self.hangxe.TenHangXe = item.TenHangXe;
            self.loaixe.TenLoaiXe = item.TenLoaiXe;
            self.inforOld = item;
            $('#ThemMoiMauXe').modal('show');
        },
        Delete_MauXe: function () {
            let self = this;
            commonStatisJs.ConfirmDialog_OKCancel('Xác nhận xóa', 'Bạn có chắc chắn muốn xóa mẫu xe <b> ' + self.TenMauXe + ' </b> không ? ', function () {
                ajaxHelper(self.urlApi.GaraApi + 'Delete_MauXe/' + self.ID, 'DELETE').done(function (x) {
                    if (x.res === true) {
                        commonStatisJs.ShowMessageSuccess('Xóa mẫu xe thành công');

                        var diary = {
                            ID_DonVi: vmThemMoiXe.inforLogin.ID_DonVi,
                            ID_NhanVien: vmThemMoiXe.inforLogin.ID_NhanVien,
                            ChucNang: 'Mẫu xe',
                            LoaiNhatKy: 3,
                            NoiDung: 'Xóa mẫu xe ' + self.TenLoaiXe,
                            NoiDungChiTiet: 'Xóa mẫu xe '.concat(self.TenLoaiXe, ', Người xóa: ', vmThemMoiXe.inforLogin.UserLogin),
                        }
                        Insert_NhatKyThaoTac_1Param(diary);
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(x.mes);
                    }
                    $('#ThemMoiMauXe').modal('hide');
                });
            })
        },

        AddNew_MauXe: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.TenMauXe)) {
                commonStatisJs.ShowMessageDanger('Vui lòng nhập tên mẫu xe');
                return;
            }

            if (commonStatisJs.CheckNull(self.ID_HangXe)) {
                self.ID_HangXe == self.GuidEmpty;
            }
            if (commonStatisJs.CheckNull(self.ID_LoaiXe)) {
                self.ID_LoaiXe = self.GuidEmpty;
            }
            var myData = {
                ID_HangXe: self.ID_HangXe,
                ID_LoaiXe: self.ID_LoaiXe,
                TenMauXe: self.TenMauXe.trim(),
                GhiChu: self.GhiChu,
                TrangThai: self.TrangThai,
            };

            var diary = {
                ID_DonVi: vmThemMoiXe.inforLogin.ID_DonVi,
                ID_NhanVien: vmThemMoiXe.inforLogin.ID_NhanVien,
                ChucNang: 'Mẫu xe',
            };
            var sTrangThai = ', Trạng thái: '.concat(parseInt(self.TrangThai) == 1 ? 'Hoạt động' : 'Ngừng hoạt động');

            self.CheckSave().then(function (data) {
                if (data.res === true) {
                    if (data.dataSoure.TenMauXe) {
                        commonStatisJs.ShowMessageDanger("Mẫu xe đã tồn tại.");
                    }
                    else {
                        if (self.isNew) {
                            myData.ID = self.GuidEmpty;
                            myData.NguoiTao = vmThemMoiXe.inforLogin.UserLogin;

                            ajaxHelper(self.urlApi.GaraApi + 'Post_MauXe', 'POST', myData).done(function (x) {
                                if (x.res === true) {
                                    self.saveOK = true;
                                    self.ID = x.dataSoure.ID;
                                    commonStatisJs.ShowMessageSuccess('Thêm mới mẫu xe thành công');

                                    diary.NoiDung = "Thêm mới mẫu xe " + self.TenMauXe;
                                    diary.NoiDungChiTiet = "Thêm mới mẫu xe ".concat(self.TenMauXe,
                                        ', Loại: ', vmMauXe.loaixe.TenLoaiXe,
                                        ', Hãng: ', vmMauXe.hangxe.TenHangXe, ', Người tạo: ', vmThemMoiXe.inforLogin.UserLogin, sTrangThai);
                                    diary.LoaiNhatKy = 1;
                                    Insert_NhatKyThaoTac_1Param(diary);
                                }
                                else {
                                    self.saveOK = false;
                                    commonStatisJs.ShowMessageDanger(x.mes);
                                }
                                $('#ThemMoiMauXe').modal('hide');
                            });
                        }
                        else {
                            myData.ID = self.ID;
                            myData.NguoiSua = vmThemMoiXe.inforLogin.UserLogin;

                            ajaxHelper(self.urlApi.GaraApi + 'Put_MauXe', 'POST', myData).done(function (x) {
                                if (x.res) {
                                    self.saveOK = true;
                                    diary.NoiDung = "Cập nhật mẫu xe " + self.TenMauXe;
                                    diary.NoiDungChiTiet = "Cập nhật mẫu xe ".concat(self.TenMauXe,
                                        ', Loại: ', vmMauXe.loaixe.TenLoaiXe,
                                        ', Hãng: ', vmMauXe.hangxe.TenHangXe, ', Người sửa: ', vmThemMoiXe.inforLogin.UserLogin, sTrangThai,
                                        ' <br /> <b> Thông tin cũ </b>: Tên mẫu: ', self.inforOld.TenMauXe,
                                        ', Hãng: ', self.inforOld.TenHangXe,
                                        ', Loại: ', self.inforOld.TenLoaiXe);

                                    Insert_NhatKyThaoTac_1Param(diary);
                                    commonStatisJs.ShowMessageSuccess('Cập nhật mẫu xe thành công');


                                }
                                else {
                                    self.saveOK = false;
                                    commonStatisJs.ShowMessageDanger(x.mes);
                                }
                                $('#ThemMoiMauXe').modal('hide');
                            })
                        }
                    }
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }

            });
        },
        CheckSave: function () {
            let self = this;
            let datacheck = {};
            datacheck.TenMauXe = self.TenMauXe;
            datacheck.New = self.isNew;
            datacheck.ID = self.ID;

            datacheck.ID_LoaiXe = self.ID_LoaiXe;
            datacheck.ID_HangXe = self.ID_HangXe;
            console.log(1, datacheck)

            return $.ajax({
                url: self.urlApi.GaraApi + "CheckSaveMauXe",
                type: 'POST',
                dataType: 'json',
                data: datacheck,
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            });
        },
        showModalHangXe: function () {
            vmHangXe.inforLogin = vmThemMoiXe.inforLogin;
            vmHangXe.ShowModalAddNew();
        },
        showModalLoaiXe: function () {
            vmLoaiXe.inforLogin = vmThemMoiXe.inforLogin;
            vmLoaiXe.ShowModalAddNew();
        },
        resetHangXe: function () {
            let self = this;
            self.ID_HangXe = self.GuidEmpty;
            self.hangxe = {
                ID: self.GuidEmpty,
                TenHangXe: '',
            };
        },
        resetLoaiXe: function () {
            let self = this;
            self.ID_LoaiXe = self.GuidEmpty;
            self.loaixe = {
                ID: self.GuidEmpty,
                TenLoaiXe: '',
            };
        }
    }
})

$(function () {
    $('#ThemMoiHangXe').on('hidden.bs.modal', function () {
        if (vmHangXe.saveOK) {
            var newObj = {
                ID: vmHangXe.ID,
                MaHangXe: vmHangXe.MaHangXe,
                TenHangXe: vmHangXe.TenHangXe,
            }
            vmMauXe.hangxe = newObj;
            vmMauXe.ID_HangXe = vmHangXe.ID;
        }
    });
    $('#ThemMoiLoaiXe').on('hidden.bs.modal', function () {
        if (vmLoaiXe.saveOK) {
            var newObj = {
                ID: vmLoaiXe.ID,
                MaLoaiXe: vmLoaiXe.MaLoaiXe,
                TenLoaiXe: vmLoaiXe.TenLoaiXe,
            }
            vmMauXe.loaixe = newObj;
            vmMauXe.ID_LoaiXe = vmLoaiXe.ID;
        }
    });
})
