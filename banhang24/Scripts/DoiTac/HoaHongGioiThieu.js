var vmDanhSachHoaHongGioiThieu = new Vue({
    el: '#vmDanhSachHoaHongGioiThieu',
    components: {
        'date-time': cpmDatetime,
    },
    created: function () {
        var self = this;
        self.UrlAPI = {
            NhomHang: '/api/DanhMuc/DM_NhomHangHoaAPI/',
            HoaDon: '/api/DanhMuc/BH_HoaDonAPI/',
            QuyHoaDon: '/api/DanhMuc/Quy_HoaDonAPI/',
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
        console.log('vmDSHoaHongGT')

        self.PageLoad();
    },
    computed: {
        txtLeftPlaceholder: function () {
            let self = this;
            let txt = '';
            switch (parseInt(self.LoaiBaoCao)) {
                case 1:
                    txt = 'Mã hóa đơn, người giới thiệu';
                    break;
                case 2:
                    txt = 'Mã hóa đơn trích, mã khách hàng';
                    break;
                case 3:
                    txt = 'Mã, tên, SĐT người giới thiệu';
                    break;
            }
            return txt;
        },
        TenBaoCao: function () {
            let self = this;
            let txt = '';
            switch (parseInt(self.LoaiBaoCao)) {
                case 1:
                    txt = 'Danh sách phiếu trích hoa hồng';
                    break;
                case 2:
                    txt = 'Chi tiết phiếu trích hoa hồng';
                    break;
                case 3:
                    txt = 'Danh sách người giới thiệu';
                    break;
            }
            return txt;
        }
    },
    data: {
        saveOK: false,
        isLoading: false,
        typeUpdate: 1,
        isKhoaSo: false,
        role: {
            HoaHongGioiThieu: { ThemMoi: true, SuaDoi: true, Xoa: true, Xoa_NeuKhacNgay: false },
        },
        LoaiBaoCao: '1',
        inforOld: {},
        ngayLapHoaDon_update: null,

        ListHeader: [],

        Paging: {
            CurrentPage: 1,
            PageSize: 10,
            ListPage: [],
            PageView: '',
            NumberOfPage: 10,
        },

        LichSuThanhToan: [],

        TongHop: {
            data: [],
            SumTongTienHang: 0,
            SumDaTrich: 0,
            SumTienChietKhau: 0,
            SumKhachDaTra: 0,
            SumConNo: 0,
        },

        ChiTiet: {
            data: [],
            SumTongTienHang: 0,
            SumDaTrich: 0,
            SumTienChietKhau: 0,
        },

        filter: {
            TextSearch: '',
            TypeTime: 0,
            DateFrom: null,
            DateTo: null,

            NgayTao_TypeTime: 0,
            NgayTaoFrom: null,
            NgayTaoTo: null,

            LaHoaDonBoSung: 0,// 1. 
        },
        listData: {
            ChiNhanh: [],
            LoaiHoaDon: [
                { Text: 'Hóa đơn bổ sung', Value: 1, Checked: true },
                { Text: 'Hóa đơn thường', Value: 0, Checked: true }],
            TrangThai: [
                { Text: 'Hoàn thành', Value: 0, Checked: true },
                { Text: 'Đã hủy', Value: 2, Checked: false }],
            TrangThaiHoatDong: [
                { Text: 'Đang theo dõi', Value: 1, Checked: true },
                { Text: 'Ngừng theo dõi', Value: 0, Checked: false }],
            LoaiDoiTuong: [
                { Text: 'Khách hàng', Value: 1, Checked: true },
                { Text: 'Nhân viên', Value: 5, Checked: true },
                { Text: 'Nhà cung cấp', Value: 2, Checked: true },
                { Text: 'Khác', Value: 4, Checked: true }]
        },
    },
    methods: {
        PageLoad: function () {
            let self = this;
            self.CheckRole();
            self.InitHeader();
            self.GetAllQuy_KhoanThuChi();
            self.getListNhanVien();
            self.GetDM_TaiKhoanNganHang();
        },
        CheckRole: function () {
            let self = this;
            let arrQuyen = VHeader.Quyen;
            self.roleChangeNgayLapHD = arrQuyen.indexOf('HoaDon_ThayDoiThoiGian') > -1;
            self.role.HoaHongGioiThieu.ThemMoi = arrQuyen.indexOf('HoaHongKhachGioiThieu_ThemMoi') > -1;
            self.role.HoaHongGioiThieu.SuaDoi = arrQuyen.indexOf('HoaHongKhachGioiThieu_SuaDoi') > -1;
            self.role.HoaHongGioiThieu.Xoa = arrQuyen.indexOf('HoaHongKhachGioiThieu_Xoa') > -1;
        },
        GetAllQuy_KhoanThuChi: function () {
            let self = this;
            ajaxHelper(self.UrlAPI.QuyHoaDon + 'GetQuy_KhoanThuChi', 'GET').done(function (x) {
                if (x.res === true) {
                    vmThanhToanNCC.listData.KhoanThuChis = x.data;
                    vmThanhToan.listData.KhoanThuChis = x.data;// used at vmHoaHong (savePhieuThu)
                }
            });
        },
        getListNhanVien: function () {
            let self = this;
            ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetNS_NhanVien_InforBasic?idDonVi=" + self.inforLogin.ID_DonVi, 'GET').done(function (x) {
                vmThanhToanNCC.listData.NhanViens = x.data;
            });
        },
        GetDM_TaiKhoanNganHang: function () {
            let self = this;
            ajaxHelper(self.UrlAPI.QuyHoaDon + 'GetAllTaiKhoanNganHang_ByDonVi?idDonVi=' + self.inforLogin.ID_DonVi, 'GET').done(function (x) {
                if (x.res === true) {
                    vmThanhToanNCC.listData.AccountBanks = x.data;
                    vmThanhToan.listData.AccountBanks = x.data;
                }
            })
        },
        InitHeader: function () {
            let self = this;
            switch (parseInt(self.LoaiBaoCao)) {
                case 1:
                    self.ListHeader = [{ colName: 'colMaHoaDon', colText: 'Mã phiếu', colShow: true, index: 0 },
                    { colName: 'colNgayLapHoaDon', colText: 'Ngày lập phiếu', colShow: true, index: 1 },
                    { colName: 'colLoaiDoiTuong', colText: 'Loại đối tượng', colShow: true, index: 2 },
                    { colName: 'colMaDoiTuong', colText: 'Mã người giới thiệu', colShow: true, index: 3 },
                    { colName: 'colTenDoiTuong', colText: 'Tên người giới thiệu', colShow: true, index: 4 },
                    { colName: 'colTongGiaTri', colText: 'Tổng giá trị', colShow: true, index: 5 },
                    { colName: 'colKhachDaTra', colText: 'Đã thanh toán', colShow: true, index: 6 },
                    { colName: 'colConNo', colText: 'Còn nợ', colShow: true, index: 7 },
                    { colName: 'colDienGiai', colText: 'Ghi chú', colShow: true, index: 8 },
                    { colName: 'colTrangThai', colText: 'Trạng thái', colShow: false, index: 9 },
                    { colName: 'colNguoiTao', colText: 'User lập phiếu', colShow: false, index: 10 },
                    { colName: 'colTenChiNhanh', colText: 'Tên chi nhánh', colShow: false, index: 11 },
                    ];
                    break;
                case 2:
                    self.ListHeader = [{ colName: 'colMaHoaDon', colText: 'Mã phiếu', colShow: true, index: 0 },
                    { colName: 'colNgayLapPhieu', colText: 'Ngày lập phiếu', colShow: true, index: 1 },
                    { colName: 'colLoaiDoiTuong', colText: 'Loại đối tượng', colShow: true, index: 2 },
                    { colName: 'colMaNguoiGT', colText: 'Mã người giới thiệu', colShow: false, index: 3 },
                    { colName: 'colTenNguoiGT', colText: 'Tên người giới thiệu', colShow: true, index: 4 },
                    { colName: 'colMaHoaDonTrich', colText: 'Mã hóa đơn trích', colShow: true, index: 5 },
                    { colName: 'colNgayLapHoaDon', colText: 'Ngày lập HĐ', colShow: true, index: 6 },
                    { colName: 'colMaKhachHang', colText: 'Mã khách hàng', colShow: false, index: 7 },
                    { colName: 'colTenKhachHang', colText: 'Tên khách hàng', colShow: true, index: 8 },
                    { colName: 'colTongThanhToan', colText: 'Giá trị HĐ', colShow: true, index: 9 },
                    { colName: 'colPTChietKhau', colText: '% trích', colShow: false, index: 10 },
                    { colName: 'colTienChietKhau', colText: 'Tiền hoa hồng', colShow: true, index: 11 },
                    ]
                    break;
                case 3:
                    self.ListHeader = [
                        { colName: 'colLoaiDoiTuong', colText: 'Loại đối tượng', colShow: false, index: 1 },
                        { colName: 'colMaNguoiGT', colText: 'Mã người giới thiệu', colShow: true, index: 2 },
                        { colName: 'colTenNguoiGT', colText: 'Tên người giới thiệu', colShow: true, index: 3 },
                        { colName: 'colSDTNguoiGT', colText: 'Số điện thoại', colShow: true, index: 4 },
                        { colName: 'colTienHoaHong', colText: 'Tiền hoa hồng', colShow: true, index: 5 },
                        { colName: 'colNguoiTao', colText: 'Người tạo', colShow: false, index: 6 },
                        { colName: 'colNgayTao', colText: 'Ngày tạo', colShow: false, index: 7 },
                        { colName: 'colTrangThai', colText: 'Trạng thái', colShow: false, index: 8 },
                    ]
                    break;
            }
        },
        CheckColShow: function (colName) {
            let self = this;
            let data = self.ListHeader.find(x => x.colName === colName);
            if (data != undefined && !$.isEmptyObject(data)) {
                return data.colShow;
            }
            return true;
        },
        GetParam: function () {
            let self = this;
            let loaiDT = [], laHDBoSung = 2;

            let arrCN = self.listData.ChiNhanh.filter(p => p.CNChecked);
            if (self.listData.LoaiDoiTuong.filter(p => p.Checked === true).length > 0) {
                loaiDT = self.listData.LoaiDoiTuong.filter(p => p.Checked === true).map(p => p.Value);
            }

            let arrLoaiHD = self.listData.LoaiHoaDon.filter(p => p.Checked === true);
            if (arrLoaiHD.length == 1) {
                laHDBoSung = arrLoaiHD[0].Value;
            }

            let arrTrangThai = [];
            switch (parseInt(self.LoaiBaoCao)) {
                case 3:
                    arrTrangThai = self.listData.TrangThaiHoatDong.filter(p => p.Checked === true);
                    break;
                default:
                    arrTrangThai = self.listData.TrangThai.filter(p => p.Checked === true);
                    break;
            }

            let txt = self.filter.TextSearch;
            if (!commonStatisJs.CheckNull(txt)) {
                txt = txt.trim();
            }
            let txt2 = self.filter.TextSearch2;
            if (!commonStatisJs.CheckNull(txt2)) {
                txt2 = txt2.trim();
            }

            return {
                IDChiNhanhs: arrCN.map(p => p.ID),
                TrangThais: arrTrangThai.map(p => p.Value),
                LoaiDoiTuongs: loaiDT,// 1.kh, 2.ncc,4, nguoi gt #, 5.nhanvien
                DateFrom: self.filter.DateFrom,
                DateTo: self.filter.DateTo,
                TextSearch: txt,
                TextSearch2: txt2,
                CurrentPage: self.Paging.CurrentPage - 1,
                PageSize: self.Paging.PageSize,
                LaHoaDonBoSung: laHDBoSung,

                NgayTaoFrom: self.filter.NgayTaoFrom,
                NgayTaoTo: self.filter.NgayTaoTo,

                ColumnHide: self.ListHeader.filter(p => p.colShow === false).map(p => p.index),
                ReportBranch: arrCN.map(p => p.TenDonVi).toString(),
                ReportTime: moment(self.filter.DateFrom, 'YYYY-MM-DD').format('DD/MM/YYYY').concat(' - ',
                    moment(self.filter.DateTo, 'YYYY-MM-DD').add('days', -1).format('DD/MM/YYYY'))
            }
        },
        LoadData: function () {
            let self = this;
            switch (parseInt(self.LoaiBaoCao)) {
                case 1:
                    self.GetList_PhieuTrichHoaHong();
                    break;
                case 2:
                    self.GetAll_ChiTietPhieuTrich();
                    break;
                case 3:
                    self.GetList_NguoiGioiThieu();
                    break;
            }
        },
        GetList_PhieuTrichHoaHong: function () {
            let self = this;
            let param = self.GetParam();
            $('#tb').gridLoader({ show: true });
            ajaxHelper(self.UrlAPI.HoaDon + 'GetList_PhieuTrichHoaHong', 'POST', param).done(function (x) {
                if (x.res && x.dataSoure.data.length > 0) {
                    self.TongHop.data = x.dataSoure.data;
                    let itFirst = x.dataSoure.data[0];
                    self.TongHop.SumTongTienHang = itFirst.SumTongTienHang;
                    self.TongHop.SumKhachDaTra = itFirst.SumKhachDaTra;
                    self.TongHop.SumConNo = itFirst.SumConNo;

                    self.Paging.TotalRow = x.dataSoure.TotalRow;
                    self.Paging.PageView = x.dataSoure.PageView;
                    self.Paging.NumberOfPage = x.dataSoure.NumOfPage;
                    self.Paging.ListPage = x.dataSoure.ListPage;
                }
                else {
                    self.TongHop.data = [];
                    self.TongHop.SumTongTienHang = 0;
                    self.TongHop.SumKhachDaTra = 0;
                    self.TongHop.SumConNo = 0;

                    self.Paging.TotalRow = 0;
                    self.Paging.PageView = '';
                    self.Paging.NumberOfPage = 0;
                    self.Paging.ListPage = 0;
                }
            }).always(function () {
                $('#tb').gridLoader({ show: false })
            })
        },
        GetAll_ChiTietPhieuTrich: function () {
            let self = this;
            let param = self.GetParam();

            $('#tb').gridLoader({ show: true });
            ajaxHelper(self.UrlAPI.HoaDon + 'GetAll_ChiTietPhieuTrich', 'POST', param).done(function (x) {
                if (x.res && x.dataSoure.data.length > 0) {
                    self.TongHop.data = x.dataSoure.data;
                    let itFirst = x.dataSoure.data[0];
                    self.TongHop.SumTongTienHang = itFirst.SumTongTienHang;
                    self.TongHop.SumDaTrich = itFirst.SumDaTrich;
                    self.TongHop.SumTienChietKhau = itFirst.SumTienChietKhau;

                    self.Paging.TotalRow = x.dataSoure.TotalRow;
                    self.Paging.PageView = x.dataSoure.PageView;
                    self.Paging.NumberOfPage = x.dataSoure.NumOfPage;
                    self.Paging.ListPage = x.dataSoure.ListPage;
                }
                else {
                    self.TongHop.data = [];
                    self.TongHop.SumTongTienHang = 0;
                    self.TongHop.SumDaTrich = 0;
                    self.TongHop.SumTienChietKhau = 0;

                    self.Paging.TotalRow = 0;
                    self.Paging.PageView = '';
                    self.Paging.NumberOfPage = 0;
                    self.Paging.ListPage = [];
                }
            }).always(function () {
                $('#tb').gridLoader({ show: false })
            })
        },
        GetList_NguoiGioiThieu: function () {
            let self = this;
            let param = self.GetParam();

            $('#tb').gridLoader({ show: true });
            ajaxHelper(self.UrlAPI.HoaDon + 'GetList_NguoiGioiThieu', 'POST', param).done(function (x) {
                if (x.res && x.dataSoure.data.length > 0) {
                    self.TongHop.data = x.dataSoure.data;

                    let itFirst = x.dataSoure.data[0];
                    self.TongHop.SumTongTienHang = itFirst.SumTongTienHang;

                    self.Paging.TotalRow = x.dataSoure.TotalRow;
                    self.Paging.PageView = x.dataSoure.PageView;
                    self.Paging.NumberOfPage = x.dataSoure.NumOfPage;
                    self.Paging.ListPage = x.dataSoure.ListPage;
                }
                else {
                    self.TongHop.data = [];
                    self.TongHop.SumTongTienHang = 0;

                    self.Paging.TotalRow = 0;
                    self.Paging.PageView = '';
                    self.Paging.NumberOfPage = 0;
                    self.Paging.ListPage = [];
                }
            }).always(function () {
                $('#tb').gridLoader({ show: false })
            })
        },
        GetPhieuTrichHoaHong_byNguoiGioiThieu: function (idNguoiGT) {
            let self = this;
            let param = self.GetParam();
            param.IDCustomers = [idNguoiGT];
            param.CurrentPage = 0;
            param.PageSize = 50;

            $('.table-detail').gridLoader({ show: true });
            ajaxHelper(self.UrlAPI.HoaDon + 'GetPhieuTrichHoaHong_byNguoiGioiThieu', 'POST', param).done(function (x) {
                console.log(x)
                if (x.res && x.dataSoure.data.length > 0) {
                    self.ChiTiet.data = x.dataSoure.data;

                    self.Paging.TotalRow = x.dataSoure.TotalRow;
                    self.Paging.PageView = x.dataSoure.PageView;
                    self.Paging.NumberOfPage = x.dataSoure.NumOfPage;
                    self.Paging.ListPage = x.dataSoure.ListPage;
                }
                else {
                    self.ChiTiet.data = [];

                    self.Paging.TotalRow = 0;
                    self.Paging.PageView = '';
                    self.Paging.NumberOfPage = 0;
                    self.Paging.ListPage = [];
                }
            }).always(function () {
                $('.table-detail').gridLoader({ show: false })
            })
        },
        showModalThemMoi: function () {
            vmHoaHongKhachGioiThieu.showModal();
        },
        showModalUpdate: async function (item) {
            vmHoaHongKhachGioiThieu.showModalUpdate(item.ID);
        },
        RowSelected: async function (item) {
            let self = this;
            switch (parseInt(self.LoaiBaoCao)) {
                case 1:
                    if (self.inforOld.ID === item.ID) {
                        // don't expand class .op-js-tr-hide
                        self.inforOld = { ID: null, ID_CheckIn: null };
                    }
                    else {
                        let ct = await vmHoaHongKhachGioiThieu.LoadChiTiet(item.ID);
                        self.ChiTiet.data = ct;
                        self.inforOld = $.extend({}, true, item);
                        // hide/show btnXoa
                        let dtNow = moment(new Date()).format('YYYY-MM-DD')
                        let ngayLapPhieu = moment(item.NgayLapHoaDon).format('YYYY-MM-DD');
                        let role = VHeader.Quyen.indexOf('HoaHongKhachGioiThieu_Xoa_NeuKhacNgay') > -1;
                        if (dtNow === ngayLapPhieu) {
                            role = self.role.HoaHongGioiThieu.Xoa;
                        }
                        self.role.HoaHongGioiThieu.Xoa_NeuKhacNgay = role;
                    }
                    break;
                case 3:
                    if (self.inforOld.ID_CheckIn === item.ID_CheckIn) {
                        self.inforOld = { ID: null, ID_CheckIn: null };
                    }
                    else {
                        self.GetPhieuTrichHoaHong_byNguoiGioiThieu(item.ID_CheckIn);
                        self.inforOld = $.extend({}, true, item);
                    }
                    break;
            }
        },
        ChangeTab: async function (type, item) {
            let self = this;
            switch (type) {
                case 0:
                    break;
                case 1:
                    let his = await self.GetLichSuThanhToan(item.ID);
                    self.LichSuThanhToan = his;
                    break;
            }
        },
        LoadAgain_LichSuThanhToan: async function () {
            let self = this;
            let his = await self.GetLichSuThanhToan(self.inforOld.ID);
            self.LichSuThanhToan = his;
        },
        ChangeLoaiBaoCao: function () {
            let self = this;
            self.inforOld = { ID: null, ID_CheckIn: null };
            self.Paging.CurrentPage = 1;
            self.LoadData();
            self.InitHeader();
        },
        GetLichSuThanhToan: async function (idHoaDon) {
            let xx = await ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/' + 'GetQuyHoaDon_byIDHoaDon?idHoaDon=' + idHoaDon, 'GET').done(function () {
            }).then(function (data) {
                return data;
            });
            return xx;
        },
        Update_ChangeDate: function (e) {
            let self = this;
            self.ngayLapHoaDon_update = moment(e).format('YYYY-MM-DD HH:mm');
        },
        ResetCurrentPage_andLoadData: function () {
            let self = this;
            self.Paging.CurrentPage = 1;
            self.LoadData();
        },
        onCallThoiGian: function (value) {
            let self = this;
            self.filter.TypeTime = value.radioselect;
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
        },
        changeNgayTao: function (value) {
            let self = this;
            self.filter.NgayTao_TypeTime = value.radioselect;
            if (self.filter.NgayTaoFrom !== value.fromdate || self.filter.NgayTaoTo !== value.todate) {
                if (value.fromdate !== '2001-01-01') {
                    self.filter.NgayTaoFrom = value.fromdate;
                    self.filter.NgayTaoTo = value.todate;
                }
                else {
                    self.filter.NgayTaoFrom = '';
                    self.filter.NgayTaoTo = '';
                }
                self.ResetCurrentPage_andLoadData();
            }
        },
        PageChange: function (value) {
            let self = this;
            if (self.Paging.CurrentPage !== value.currentPage) {
                self.Paging.CurrentPage = value.currentPage;
            } else if (self.Paging.PageSize !== value.pageSize) {
                self.Paging.CurrentPage = 1;
                self.Paging.PageSize = value.pageSize;
            }
            self.LoadData();
        },

        showModalThanhToan: function (item) {
            let obj = {
                ID: item.ID,
                ID_DoiTuong: item.ID_CheckIn,
                ID_CheckIn: item.ID_CheckIn,
                MaHoaDon: item.MaHoaDon,
                LoaiHoaDon: 41,
                NgayLapHoaDon: item.NgayLapHoaDon,
                LoaiDoiTuong: parseInt(item.TongChietKhau),
                PhaiThanhToan: item.TongTienHang,
                KhachDaTra: item.KhachDaTra,
                MaDoiTuong: item.MaNguoiGioiThieu,
                TenDoiTuong: item.TenNguoiGioiThieu,
                DienThoai: '',
            }
            vmThanhToanNCC.showModalThanhToan(obj);
        },
        updatePhieuChi: function (item) {
            vmThanhToanNCC.showModalUpdate(item.ID);
        },
        HuyPhieuTrich: function (item) {
            let self = this;
            dialogConfirm('Thông báo xóa', 'Có muốn hủy hóa đơn <b>' + item.MaHoaDon + '</b> cùng những phiếu liên quan không?', function () {
                ajaxHelper(self.UrlAPI.HoaDon + 'Huy_HoaDon?id=' + item.ID + ' &nguoiSua=' + self.inforLogin.UserLogin
                    + ' &iddonvi=' + self.inforLogin.ID_DonVi).done(function () {
                        ShowMessage_Success("Hủy phiếu trích hoa hồng thành công");
                        let diary = {
                            ID_NhanVien: self.inforLogin.ID_NhanVien,
                            ID_DonVi: self.inforLogin.ID_DonVi,
                            ChucNang: 'Phiếu trích hoa hồng',
                            NoiDung: "Xóa phiếu trích hoa hồng ".concat(item.MaHoaDon),
                            NoiDungChiTiet: "Xóa phiếu trích hoa hồng".concat(": ", item.MaHoaDon, ', Người xóa: ', self.inforLogin.UserLogin),
                            LoaiNhatKy: 3
                        };
                        Insert_NhatKyThaoTac_1Param(diary);
                        self.ResetCurrentPage_andLoadData();
                    })
            })
        },
        Export_PhieuTrichHoaHong_byID: async function (item) {
            let self = this;
            let ngay = item.NgayLapHoaDon;
            let obj = {
                id: item.ID,
                lstCell: [
                    { RowIndex: 1, ColumnIndex: 0,CellValue: 'Mã phiếu: '.concat(item.MaHoaDon, ' - Ngày lập:', moment(ngay).format('DD/MM/YYYY HH:mm')) },
                    { RowIndex: 2, ColumnIndex: 1,CellValue: item.MaNguoiGioiThieu },
                    { RowIndex: 3, ColumnIndex: 1,CellValue: item.TenNguoiGioiThieu },
                    { RowIndex: 4, ColumnIndex: 1,CellValue: item.DienGiai },

                    { RowIndex: 2, ColumnIndex: 7,CellValue: item.TongTienHang },
                    { RowIndex: 3, ColumnIndex: 7,CellValue: item.KhachDaTra },
                    { RowIndex: 4, ColumnIndex: 7,CellValue: item.ConNo },
                ],
            }
            const exportOK = await commonStatisJs.NPOI_ExportExcel(self.UrlAPI.HoaDon + 'Export_PhieuTrichHoaHong_byID', 'POST', obj, "PhieuTrichHoaHong_byID.xlsx");

            if (exportOK) {
                let diary = {
                    ID_NhanVien: VHeader.IdNhanVien,
                    ID_DonVi: VHeader.IdDonVi,
                    ChucNang: "Phiếu trích hoa hồng",
                    NoiDung: "Xuất excel phiếu trích hoa hồng ".concat(item.MaHoaDon),
                    NoiDungChiTiet: "Xuất excel phiếu trích hoa hồng ".concat(item.MaHoaDon, '<br />- Người xuất: ', VHeader.UserLogin),
                    LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                };
                Insert_NhatKyThaoTac_1Param(diary);
            }
            
        },
        ExportExcel: async function () {
            let self = this;
            let param = self.GetParam();
            param.PageSize = self.Paging.TotalRow;

            let url = '', txt = '', fileNameExport = '';
            switch (parseInt(self.LoaiBaoCao)) {
                case 1:
                    url = 'Export_PhieuTrichHoaHong';
                    txt = 'danh sách';
                    fileNameExport = 'DanhSachPhieuTrichHoaHong.xlsx';
                    break;
                case 2:
                    url = 'Export_ChiTietPhieuTrichHoaHong';
                    txt = 'chi tiết';
                    break;
            }

            const exportOK = await commonStatisJs.NPOI_ExportExcel(self.UrlAPI.HoaDon + url, 'POST', param, fileNameExport);

            if (exportOK) {
                commonStatisJs.ShowMessageSuccess("Xuất file thành công");
                let diary = {
                    ID_DonVi: VHeader.IdDonVi,
                    ID_NhanVien: VHeader.IdNhanVien,
                    LoaiNhatKy: 6,
                    ChucNang: 'Phiếu trích hoa hồng',
                    NoiDung: 'Xuất file ' + txt + ' trích hoa hồng',
                    NoiDungChiTiet: 'Xuất file ' + txt + ' phiếu trích hoa hồng'.
                        concat('<br /> Người xuất: ', VHeader.UserLogin),
                }
                Insert_NhatKyThaoTac_1Param(diary);
            } 

        },
        updateNguoiGioiThieu: async function (idCus) {
            let cus = await vmThemMoiKhach.GetInforKhachHangFromDB_ByID(idCus);
            if (cus !== null && cus.length > 0) {
                vmThemMoiKhach.showModalUpdate(cus[0]);
            }
        },
        gotoPageOther: function (item, type) {
            let self = this;
            let url = '';
            switch (type) {
                case 0:// khachhang
                    switch (item.TongChietKhau) {
                        case 1:
                            url = '/#/Customers';
                            localStorage.setItem('FindKhachHang', item.MaNguoiGioiThieu);
                            break;
                        case 2:
                            url = '/#/Suppliers';
                            localStorage.setItem('FindKhachHang', item.MaNguoiGioiThieu);
                            break;
                        case 3:
                            url = '/#/BaoHiem';
                            localStorage.setItem('FindKhachHang', item.MaNguoiGioiThieu);
                            break;
                        case 5:
                            url = '/#/User';
                            break;
                    }
                    break;
                case 1:// hoadon
                    localStorage.setItem('FindHD', item.MaHoaDon);
                    switch (item.LoaiHoaDon) {
                        case 1:
                            url = '/#/Invoices';
                            break;
                        case 19:
                            url = '/#/ServicePackage';
                            break;
                        case 22:
                            url = '/#/RechargeValueCard';
                            break;
                    }
                    break;
            }
            window.open(url, '_blank');
        },
    }
})

$(function () {
    $('#vmHoaHongKhachGioiThieu').on('hidden.bs.modal', function () {
        if (vmHoaHongKhachGioiThieu.saveOK) {
            vmDanhSachHoaHongGioiThieu.ResetCurrentPage_andLoadData();
        }
    })

    $('#vmThanhToanNCC').on('hidden.bs.modal', function () {
        if (vmThanhToanNCC.saveOK) {
            vmDanhSachHoaHongGioiThieu.LoadAgain_LichSuThanhToan();
        }
    })
})