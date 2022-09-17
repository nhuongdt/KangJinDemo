var vmDanhSachHoaHongGioiThieu = new Vue({
    el: '#vmDanhSachHoaHongGioiThieu',
    components: {
        'my-date-time': cpmDatetime,
        'customers': cmpChoseCustomer,
        'dropdown': cmpDropdown1Item,
    },
    created: function () {
        var self = this;
        self.UrlAPI = {
            NhomHang: '/api/DanhMuc/DM_NhomHangHoaAPI/',
            HoaDon: '/api/DanhMuc/BH_HoaDonAPI/',
        }

        self.inforLogin = {
            ID_DonVi: VHeader.IdDonVi,
            ID_NhanVien: VHeader.IdNhanVien,
            UserLogin: VHeader.UserLogin,
        }

        let arr = $.extend(true, [], VHeader.ListChiNhanh);
        arr.map(function (item) {
            if (item['ID'] === VHeader.IdDonVi) {
                item['CNChecked'] = true;
            }
            else {
                item['CNChecked'] = false;
            }
        });
        self.listData.ChiNhanhs = arr;
        console.log(VHeader.ListChiNhanh)

        self.roleChangeNgayLapHD = VHeader.Quyen.indexOf('HoaDon_ThayDoiThoiGian') > -1;

        self.InitHeader();
    },
    watch: {
        //ListHeader: {
        //    handler: function () {

        //    },
        //    deep: true
        //},
    },
    data: {

        saveOK: false,
        isLoading: false,
        typeUpdate: 1,
        isKhoaSo: false,
        ListHeader: [],
        PageList: {
            currentPage: 1,
            PageSize: 10,
            ListPage: [],
            PageView: '',
            NumberOfPage: 10,
        },
        CTPageList: {
            currentPage: 1,
            PageSize: 10,
            ListPage: [],
            PageView: '',
            NumberOfPage: 10,
        },
        filter: {
            TypeTime: 0,
        },
        listData: {
            ChiNhanhs: [],
            TrangThai: [
                { Text: 'Hoàn thành', Value: 1, Checked: true },
                { Text: 'Đã hủy', Value: 2, Checked: false }],
            LoaiDoiTuong: [
                { Text: 'Khách hàng', Value: 1, Checked: true },
                { Text: 'Nhân viên', Value: 2, Checked: true },
                { Text: 'Nhà cung cấp', Value: 3, Checked: false },
                { Text: 'Khác', Value: 4, Checked: false }]
        },

    },
    methods: {
        InitHeader: function () {
            let self = this;
            self.ListHeader = [{ colName: 'colMaHoaDon', colText: 'Mã phiếu', colShow: true, index: 0 },
            { colName: 'colNgayLapHoaDon', colText: 'Ngày lập phiếu', colShow: true, index: 1 },
            { colName: 'colLoaiDoiTuong', colText: 'Loại đối tượng', colShow: true, index: 2 },
            { colName: 'colMaDoiTuong', colText: 'Mã người giới thiệu', colShow: true, index: 3 },
            { colName: 'colTenDoiTuong', colText: 'Tên người giới thiệu', colShow: true, index: 4 },
            { colName: 'colTongGiaTri', colText: 'Tổng giá trị', colShow: true, index: 5 },
            { colName: 'colDienGiai', colText: 'Ghi chú', colShow: true, index: 6 },
            { colName: 'colTrangThai', colText: 'Trạng thái', colShow: true, index: 7 },
            { colName: 'colNguoiTao', colText: 'User lập phiếu', colShow: true, index: 8 },
            { colName: 'colTenChiNhanh', colText: 'Tên chi nhánh', colShow: false, index: 9 },
            ]
        },
        CheckColShow: function (colName) {
            let self = this;
            let data = self.ListHeader.find(x => x.colName === colName);
            if (data != undefined) {
                return data.colShow;
            }
            return true;
        },

        showModalThemMoi: function () {
            let self = this;
            vmHoaHongKhachGioiThieu.showModal();
        },
        showModalUpdate: function (idHoaDon, cthd = []) {
            let self = this;
            self.saveOK = false;
            self.isLoading = false;
            self.typeUpdate = 2;
        },

        BeforeLoadData: function () {

        },
        onCallThoiGian: function () {

        },
        changeTrangThai: function () {

        },
        PageChange: function () {

        },
        Enter_FocusNext: function (index) {
            let $this = $(event.currentTarget);
            let $tr = $this.closest('tr').next();
            $tr.find('td').eq(index).find('input').focus().select();
        },

        CheckSave: function () {
            let self = this;

            if (commonStatisJs.CheckNull(self.newHoaDon.ID_DoiTuong)) {
                ShowMessage_Danger('Vui lòng chọn khách hàng');
                return;
            }

            return true;
        },
        Agree: function () {
            let self = this;
            let check = self.CheckSave();
            if (!check) {
                return;
            }

            self.saveOK = true;


        },
        ChangeNgayLapHoaDon: function (e) {
            let self = this;
            let dt = moment(e).format('YYYY-MM-DD HH:mm');
            self.newHoaDon.NgayLapHoaDon = dt;
        },
        ChangeCustomer: function (item) {
            let self = this;
            self.newHoaDon.ID_DoiTuong = item.ID;
            self.newHoaDon.TenDoiTuong = item.TenDoiTuong;
            self.newHoaDon.MaDoiTuong = item.MaDoiTuong;

            let $this = $(event.currentTarget);
            $($this).closest('div').hide();
            $($this).closest('div').prev('focus');
        },
        ResetCustomer: function () {
            let self = this;
            self.newHoaDon.ID_DoiTuong = null;
            self.newHoaDon.TenDoiTuong = '';
            self.newHoaDon.MaDoiTuong = '';
        },
        saveHoaDon: function (idRandomHD, ngaylapHD) {// used to lui ngaylapHD
            let self = this;

            if (self.typeUpdate === 1) {

            }
            else {

            }


            let myData = {
                objHoaDon: hd,
                objCTHoaDon: cthd,
            }
            if (!$.isEmptyObject(hd)) {
                self.isLoading = true;
                ajaxHelper(self.UrlAPI.HoaDon + url, 'POST', myData).done(function (x) {
                    if (x.res) {
                        ShowMessage_Success(' hóa đơn hỗ trợ thành công');

                        let data = x.data;
                        let diary = {
                            ID_DonVi: self.inforLogin.ID_DonVi,
                            ID_NhanVien: self.inforLogin.ID_NhanVien,
                            LoaiNhatKy: self.typeUpdate,
                            ChucNang: 'Áp dụng hỗ trợ ',
                            NoiDung: sType.concat(' hóa đơn hỗ trợ ', data.MaHoaDon,
                                ', Khách hàng ', hd.TenDoiTuong, '(', hd.MaDoiTuong, ')'),
                            NoiDungChiTiet: 'Nội dung chi tiết '.concat(' <br /> Tên nhóm hàng: ', hd.TenNhomHangHoa,
                                ' <br /> Xuất ngày thuốc: ', hd > 0 ? hd.SoNgayThuoc : 'không',
                                ' <br /> Chuyển phát nhanh: ', hd.An_Hien ? 'có' : 'không',
                                ' <br /> Ngày lập phiếu: ', moment(data.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'),
                                sListSP, user, sOld),
                            LoaiHoaDon: 36,
                            ID_HoaDon: data.ID,
                            ThoiGianUpdateGV: data.NgayLapHoaDon
                        }
                        Post_NhatKySuDung_UpdateGiaVon(diary);

                    }
                    else {
                        ShowMessage_Danger(sType + ' hóa đơn hỗ trợ thất bại');
                    }
                }).always(function () {
                    self.isLoading = false;
                    $('#vmDanhSachHoaHongGioiThieu').modal('hide');
                })
            }
        },
    }
})