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
        self.listData.ChiNhanh = arr;
        self.roleChangeNgayLapHD = VHeader.Quyen.indexOf('HoaDon_ThayDoiThoiGian') > -1;
        console.log('vmDSHoaHongGT')

        self.InitHeader();
        self.GetList_PhieuTrichHoaHong();
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
        HoaDon: {
            data: [],
            SumTongTienHang:0,
            CurrentPage: 1,
            PageSize: 10,
            ListPage: [],
            PageView: '',
            NumberOfPage: 10,
        },
        HoaDonChiTiet: {
            data:[],
            CurrentPage: 1,
            PageSize: 10,
            ListPage: [],
            PageView: '',
            NumberOfPage: 10,
        },
        filter: {
            TextSearch:'',
            TypeTime: 0,
            DateFrom: null,
            DateTo: null,
        },
        listData: {
            ChiNhanh: [],
            TrangThai: [
                { Text: 'Hoàn thành', Value: 0, Checked: true },
                { Text: 'Đã hủy', Value: 2, Checked: false }],
            LoaiDoiTuong: [
                { Text: 'Khách hàng', Value: 1, Checked: true },
                { Text: 'Nhân viên', Value: 4, Checked: true },
                { Text: 'Nhà cung cấp', Value: 2, Checked: false },
                { Text: 'Khác', Value: 0, Checked: false }]
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
        GetParam: function () {
            let self = this;
            let loaiDT = '';
            if (self.listData.LoaiDoiTuong.filter(p => p.Checked === true).length > 0) {
                loaiDT = self.listData.LoaiDoiTuong.filter(p => p.Checked === true).map(p => p.Value).toString();
            }
            return {
                IDChiNhanhs: self.listData.ChiNhanh.filter(p => p.CNChecked).map(p => p.ID),
                TrangThais: self.listData.TrangThai.filter(p => p.Checked === true).map(p => p.Value),
                LoaiHoaDons: loaiDT,// muon tam truong (LoaiDoiTuong.1.kh, 2.ncc)
                DateFrom: self.filter.DateFrom,
                DateTo: self.filter.DateTo,
                TextSearch: self.filter.TextSearch,
                IDCustomers:[],// muontamtruong (list IDNguoiGioiThieu)
                CurrentPage: self.HoaDon.CurrentPage - 1,
                PageSize: self.HoaDon.PageSize,
            }
        },
        GetList_PhieuTrichHoaHong: function () {
            let self = this;
            let param = self.GetParam();
            $('#tb').gridLoader({ show: true });
            console.log('param', param)
            ajaxHelper(self.UrlAPI.HoaDon + 'GetList_PhieuTrichHoaHong', 'POST', param).done(function (x) {
                console.log(x)
                if (x.res && x.dataSoure.data.length > 0) {
                    self.HoaDon.data = x.dataSoure.data;
                   
                    let itFirst = x.dataSoure.data[0];
                    self.HoaDon.SumTongTienHang = itFirst.SumTongTienHang;
                    self.HoaDon.TotalRow = x.dataSoure.TotalRow;
                    self.HoaDon.PageView = x.dataSoure.PageView;
                    self.HoaDon.NumberOfPage = x.dataSoure.NumOfPage;
                    self.HoaDon.ListPage = x.dataSoure.ListPage;
                }
                else {
                    self.HoaDon.data = [];
                    self.HoaDon.SumTongTienHang = 0;
                    self.HoaDon.TotalRow = 0;
                    self.HoaDon.PageView = 0;
                    self.HoaDon.NumberOfPage = 0;
                    self.HoaDon.ListPage =0;
                }
            }).always(function () {
                $('#tb').gridLoader({ show: false })
            })
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
        ResetCurrentPage_andLoadData: function () {
            let self = this;
            self.HoaDon.CurrentPage = 1;
            self.GetList_PhieuTrichHoaHong();
        },
        onCallThoiGian: function (value) {
            let self = this;
            if (self.filter.DateFrom !== value.fromdate || self.filter.DateTo !== value.todate) {
                if (value.fromdate !== '2001-01-01') {
                    self.filter.DateFrom = value.fromdate;
                    self.filter.DateTo = value.todate;
                }
                else {
                    self.filter.DateFrom = '';
                    self.filter.DateTo = '';
                }
                self.ResetCurrentPage_andLoadData();
            }
            self.filter.TypeTime = value.radioselect;
        },
        PageChange: function () {
            let self = this;
            self.GetList_PhieuTrichHoaHong();
        },
    }
})