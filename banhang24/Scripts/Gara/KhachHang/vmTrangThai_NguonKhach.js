var vmTrangThaiKhach = new Vue({
    el: '#modalStatus_Customer',
    created: function () {
        this.UrlDoiTuongAPI = '/api/DanhMuc/DM_DoiTuongAPI/';
    },
    data: {
        saveOK: false,
        isNew: true,
        typeUpdate: 1,//1.themmoi, 2.update, 0.delete
        isLoading: false,
        error: '',
        itemOld: {},
        inforLogin: {},
        role: {
            Update: false,
            Delete: true,
        },
        newTrangThai: {
            ID: '00000000-0000-0000-0000-000000000000',
            TenTrangThai: '',
            NguoiTao: ''
        }
    },
    methods: {
        showModalAdd: function () {
            var self = this;
            self.isNew = true;
            self.saveOK = false;
            self.typeUpdate = 1;
            self.isLoading = false;

            self.newTrangThai = {
                ID: '00000000-0000-0000-0000-000000000000',
                TenTrangThai: '',
                NguoiTao: '',
            };
            $('#modalStatus_Customer').modal('show');
        },
        showModalUpdate: function (item) {
            console.log('vmTT ', item)
            var self = this;
            self.isNew = false;
            self.saveOK = false;
            self.typeUpdate = 2;
            self.newTrangThai = item;
            self.itemOld = $.extend({}, item);
            $('#modalStatus_Customer').modal('show');
        },
        SaveTrangThai: function () {
            var self = this;
            var myData = {
                ID: self.newTrangThai.ID,
                TenTrangThai: self.newTrangThai.TenTrangThai,
                NguoiTao: self.inforLogin.UserLogin,
                NgayTao: moment(new Date()).format('YYYY-MM-DD'),
            }

            var noidung = '<br /> - Tên trạng thái: '.concat(self.newTrangThai.TenTrangThai);
            var diary = {
                ID_NhanVien: self.inforLogin.ID_NhanVien,
                ID_DonVi: self.inforLogin.ID_DonVi,
                ChucNang: 'Trạng thái khách',
            }
            console.log('trangthai ', myData)
            if (self.isNew) {
                diary.LoaiNhatKy = 1;
                diary.NoiDung = 'Thêm mới trạng thái khách ';
                diary.NoiDungChiTiet = 'Thêm mới trạng thái khách ' + noidung;

                ajaxHelper(self.UrlDoiTuongAPI + 'PostDM_DoiTuong_TrangThai', 'POST', myData).done(function (item) {
                    self.saveOK = true;
                    self.newTrangThai.ID = item.ID;
                    Insert_NhatKyThaoTac_1Param(diary);

                    ShowMessage_Success('Thêm mới trạng thái khách hàng thành công');

                }).always(function () {
                    $('#modalStatus_Customer').modal('hide');
                });
            }
            else {
                myData.NguoiSua = self.inforLogin.UserLogin;

                ajaxHelper(self.UrlDoiTuongAPI + 'PutDM_DoiTuong_TrangThai', 'POST', myData).done(function (result) {
                    if (result === '') {
                        ShowMessage_Success('Cập nhật trạng thái khách hàng thành công');
                        self.saveOK = true;
                        diary.LoaiNhatKy = 2;
                        diary.NoiDung = 'Cập nhật trạng thái khách ';
                        diary.NoiDungChiTiet = 'Cập nhật trạng thái khách '.concat(noidung, '<br /> <b>Thông tin cũ: </b>',
                            '<br /> - Tên trạng thái: ', self.itemOld.TenTrangThai);

                        Insert_NhatKyThaoTac_1Param(diary);
                    }
                    else {
                        ShowMessage_Danger(result);
                    }

                }).always(function () {
                    $('#modalStatus_Customer').modal('hide');
                });
            }
        },
        DeleteTrangThai: function () {
            var self = this;
            dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn xóa trạng thái <b> ' + self.newTrangThai.TenTrangThai + ' </b> không?', function () {
                ajaxHelper(self.UrlDoiTuongAPI + 'Delete_DoiTuong_TrangThai?idTrangThai=' + self.newTrangThai.ID, 'PUT').done(function (result) {
                    if (result === '') {
                        ShowMessage_Success('Xóa loại trạng thái khách hàng thành công');
                        self.saveOK = true;
                        self.typeUpdate = 0;
                        var diary = {
                            ID_NhanVien: self.inforLogin.ID_NhanVien,
                            ID_DonVi: self.inforLogin.ID_DonVi,
                            LoaiNhatKy: 3,
                            ChucNang: 'Trạng thái khách',
                            NoiDung: 'Xóa trạng thái khách <b>'.concat(self.newTrangThai.TenTrangThai, '</b />'),
                            NoiDungChiTiet: 'Xóa trạng thái khách <b>'.concat(self.newTrangThai.TenTrangThai, '</b />',
                                ', Người xóa ', self.inforLogin.UserLogin),
                        }
                        Insert_NhatKyThaoTac_1Param(diary);
                    }
                    else {
                        ShowMessage_Danger(result);
                    }
                }).always(function () {
                    $('#modalStatus_Customer').modal('hide');
                });
            })
        },
    }
})

var vmNguonKhach = new Vue({
    el: '#modalNguonKhach',
    created: function () {
        this.NguonKhachAPI = '/api/DanhMuc/DM_NguonKhachAPI/';
        const header = $('#op24-nav');
        if (header.length > 0) {
            this.role.Delete = VHeader.Quyen.indexOf('NguonKhach_Xoa') > -1;
        }
    },
    data: {
        saveOK: false,
        isNew: true,
        isLoading: false,
        typeUpdate: 1,
        error: '',
        itemOld: {},
        inforLogin: {},
        role: {
            Update: false,
            Delete: true,
        },
        newNguonKhach: {
            ID: '00000000-0000-0000-0000-000000000000',
            TenNguonKhach: '',
            NguoiTao: ''
        }
    },
    methods: {
        showModalAdd: function () {
            var self = this;
            self.isNew = true;
            self.saveOK = false;
            self.typeUpdate = 1;
            self.isLoading = false;
            self.newNguonKhach = {
                ID: '00000000-0000-0000-0000-000000000000',
                TenNguonKhach: '',
                NguoiTao: '',
            };
            $('#modalNguonKhach').modal('show');
        },
        showModalUpdate: function (item) {
            console.log('vmTT ', item)
            var self = this;
            self.isNew = false;
            self.saveOK = false;
            self.typeUpdate = 2;
            self.newNguonKhach = item;
            self.itemOld = $.extend({}, item);
            $('#modalNguonKhach').modal('show');
        },
        SaveNguonKhach: function () {
            var self = this;
            var myData = {
                ID: self.newNguonKhach.ID,
                TenNguonKhach: self.newNguonKhach.TenNguonKhach,
                NguoiTao: self.inforLogin.UserLogin,
                NgayTao: moment(new Date()).format('YYYY-MM-DD'),
            }

            var noidung = '<br /> - Tên nguồn khách: '.concat(self.newNguonKhach.TenNguonKhach);
            var diary = {
                ID_NhanVien: self.inforLogin.ID_NhanVien,
                ID_DonVi: self.inforLogin.ID_DonVi,
                ChucNang: 'Nguồn khách',
            }
            console.log('nguon ', myData)
            if (self.isNew) {
                diary.LoaiNhatKy = 1;
                diary.NoiDung = 'Thêm mới nguồn khách ';
                diary.NoiDungChiTiet = 'Thêm mới nguồn khách ' + noidung;

                ajaxHelper(self.NguonKhachAPI + 'PostDM_NguonKhachHang', 'POST', myData).done(function (item) {
                    self.saveOK = true;
                    self.newNguonKhach.ID = item.ID;
                    Insert_NhatKyThaoTac_1Param(diary);

                    ShowMessage_Success('Thêm mới nguồn khách hàng thành công');

                }).always(function () {
                    $('#modalNguonKhach').modal('hide');
                });
            }
            else {
                myData.NguoiSua = self.inforLogin.UserLogin;

                ajaxHelper(self.NguonKhachAPI + 'PutDM_NguonKhachHang', 'PUT', myData).done(function (result) {
                    ShowMessage_Success('Cập nhật nguồn khách hàng thành công');
                    self.saveOK = true;
                    diary.LoaiNhatKy = 2;
                    diary.NoiDung = 'Cập nhật nguồn khách ';
                    diary.NoiDungChiTiet = 'Cập nhật nguồn khách '.concat(noidung, '<br /> <b>Thông tin cũ: </b>',
                        '<br /> - Tên nguồn khách: ', self.itemOld.TenNguonKhach);

                    Insert_NhatKyThaoTac_1Param(diary);

                }).always(function () {
                    $('#modalNguonKhach').modal('hide');
                });
            }
        },
        DeleteNguonKhach: function () {
            var self = this;
            dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn xóa nguồn khách <b> ' + self.newNguonKhach.TenNguonKhach + ' </b> không?', function () {
                ajaxHelper(self.NguonKhachAPI + 'DeleteDM_NguonKhach?id=' + self.newNguonKhach.ID, 'delete').done(function (result) {

                    ShowMessage_Success('Xóa nguồn khách hàng thành công');
                    self.saveOK = true;
                    self.typeUpdate = 0;
                    var diary = {
                        ID_NhanVien: self.inforLogin.ID_NhanVien,
                        ID_DonVi: self.inforLogin.ID_DonVi,
                        LoaiNhatKy: 3,
                        ChucNang: 'Nguồn khách',
                        NoiDung: 'Xóa nguồn khách <b>'.concat(self.newNguonKhach.TenNguonKhach, '</b />'),
                        NoiDungChiTiet: 'Xóa nguồn khách <b>'.concat(self.newNguonKhach.TenNguonKhach, '</b />',
                            ', Người xóa ', self.inforLogin.UserLogin),
                    }
                    Insert_NhatKyThaoTac_1Param(diary);

                }).always(function () {
                    $('#modalNguonKhach').modal('hide');
                });
            })
        },
    }
})

