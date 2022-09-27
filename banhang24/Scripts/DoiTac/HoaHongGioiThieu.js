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
        self.roleChangeNgayLapHD = VHeader.Quyen.indexOf('HoaDon_ThayDoiThoiGian') > -1;
        console.log('vmDSHoaHongGT')

        self.PageLoad();
    },
    watch: {
        LoaiBaoCao: {
            handler: function () {
                let self = this;
                self.Paging.CurrentPage = 1;
                self.LoadData();
                self.InitHeader();
            },
            deep: true
        },
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
            }
            return txt;
        }
    },
    data: {
        saveOK: false,
        isLoading: false,
        typeUpdate: 1,
        isKhoaSo: false,
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

        HoaDonChiTiet: [],
        LichSuThanhToan: [],

        BaoCaoChiTiet: {
            data: [],
            SumTongTienHang: 0,
            SumDaTrich: 0,
            SumTienChietKhau: 0,
        },

        BaoCaoTongHop: {
            data: [],
            SumDaTrich: 0,
        },

        HoaDon: {
            data: [],
            SumTongTienHang: 0,
            SumKhachDaTra: 0,
            SumConNo: 0,
        },

        filter: {
            TextSearch: '',
            TypeTime: 0,
            DateFrom: null,
            DateTo: null,

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
            self.InitHeader();
            self.GetAllQuy_KhoanThuChi();
            self.getListNhanVien();
            self.GetDM_TaiKhoanNganHang();
        },
        GetAllQuy_KhoanThuChi: function () {
            let self = this;
            ajaxHelper(self.UrlAPI.QuyHoaDon + 'GetQuy_KhoanThuChi', 'GET').done(function (x) {
                if (x.res === true) {
                    vmThanhToanNCC.listData.KhoanThuChis = x.data;
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
                    { colName: 'colGiaTriTinh', colText: 'Giá trị tính', colShow: true, index: 10 },
                    { colName: 'colPTChietKhau', colText: '% trích', colShow: false, index: 11 },
                    { colName: 'colTienChietKhau', colText: 'Tiền hoa hồng', colShow: true, index: 12 },
                    ]
                    break;
                case 3:
                    self.ListHeader = [{ colName: 'colMaHoaDon', colText: 'Mã phiếu', colShow: true, index: 0 },
                    { colName: 'colNgayLapPhieu', colText: 'Ngày lập phiếu', colShow: true, index: 1 },
                    { colName: 'colLoaiDoiTuong', colText: 'Loại đối tượng', colShow: true, index: 2 },
                    { colName: 'colMaNguoiGT', colText: 'Mã người giới thiệu', colShow: true, index: 3 },
                    { colName: 'colTenNguoiGT', colText: 'Tên người giới thiệu', colShow: true, index: 4 },
                    { colName: 'colMaHoaDon', colText: 'Mã hóa đơn trích', colShow: true, index: 5 },
                    { colName: 'colNgayLapHoaDon', colText: 'Ngày lập HĐ', colShow: true, index: 6 },
                    { colName: 'colMaKhachHang', colText: 'Mã khách hàng', colShow: true, index: 7 },
                    { colName: 'colTenKhachHang', colText: 'Tên khách hàng', colShow: true, index: 8 },
                    { colName: 'colTongThanhToan', colText: 'Giá trị HĐ', colShow: false, index: 9 },
                    { colName: 'colGiaTriTinh', colText: 'Giá trị tính', colShow: false, index: 10 },
                    { colName: 'colPTTrich', colText: '% trích', colShow: false, index: 11 },
                    { colName: 'colTienHoaHong', colText: 'Tiền hoa hồng', colShow: false, index: 12 },
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
            let loaiDT = '', laHDBoSung = 2;
            if (self.listData.LoaiDoiTuong.filter(p => p.Checked === true).length > 0) {
                loaiDT = self.listData.LoaiDoiTuong.filter(p => p.Checked === true).map(p => p.Value).toString();
            }
            if (self.listData.LoaiHoaDon.filter(p => p.Checked === true).length > 0) {
                laHDBoSung = self.listData.LoaiHoaDon.filter(p => p.Checked === true).map(p => p.Value).toString();
            }
            return {
                IDChiNhanhs: self.listData.ChiNhanh.filter(p => p.CNChecked).map(p => p.ID),
                TrangThais: self.listData.TrangThai.filter(p => p.Checked === true).map(p => p.Value),
                LoaiHoaDons: loaiDT,// muon tam truong (LoaiDoiTuong.1.kh, 2.ncc)
                DateFrom: self.filter.DateFrom,
                DateTo: self.filter.DateTo,
                TextSearch: self.filter.TextSearch,
                IDCustomers: [],// muontamtruong (list IDNguoiGioiThieu)
                CurrentPage: self.Paging.CurrentPage - 1,
                PageSize: self.Paging.PageSize,
                IDCars: [laHDBoSung]
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
            }
        },
        GetList_PhieuTrichHoaHong: function () {
            let self = this;
            let param = self.GetParam();
            $('#tb').gridLoader({ show: true });
            ajaxHelper(self.UrlAPI.HoaDon + 'GetList_PhieuTrichHoaHong', 'POST', param).done(function (x) {
                if (x.res && x.dataSoure.data.length > 0) {
                    self.HoaDon.data = x.dataSoure.data;

                    let itFirst = x.dataSoure.data[0];
                    self.HoaDon.SumTongTienHang = itFirst.SumTongTienHang;
                    self.HoaDon.SumKhachDaTra = itFirst.SumKhachDaTra;
                    self.HoaDon.SumConNo = itFirst.SumConNo;

                    self.Paging.TotalRow = x.dataSoure.TotalRow;
                    self.Paging.PageView = x.dataSoure.PageView;
                    self.Paging.NumberOfPage = x.dataSoure.NumOfPage;
                    self.Paging.ListPage = x.dataSoure.ListPage;
                }
                else {
                    self.HoaDon.data = [];
                    self.HoaDon.SumTongTienHang = 0;
                    self.HoaDon.SumKhachDaTra = 0;
                    self.HoaDon.SumConNo = 0;

                    self.Paging.TotalRow = 0;
                    self.Paging.PageView = 0;
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

            $('#tblDetail').gridLoader({ show: true });
            ajaxHelper(self.UrlAPI.HoaDon + 'GetAll_ChiTietPhieuTrich', 'POST', param).done(function (x) {
                console.log('GetAll_ChiTietPhieuTrich', param, x)
                if (x.res && x.dataSoure.data.length > 0) {
                    self.BaoCaoChiTiet.data = x.dataSoure.data;

                    let itFirst = x.dataSoure.data[0];
                    self.BaoCaoChiTiet.SumTongTienHang = itFirst.SumTongTienHang;
                    self.BaoCaoChiTiet.SumDaTrich = itFirst.SumDaTrich;

                    self.Paging.TotalRow = x.dataSoure.TotalRow;
                    self.Paging.PageView = x.dataSoure.PageView;
                    self.Paging.NumberOfPage = x.dataSoure.NumOfPage;
                    self.Paging.ListPage = x.dataSoure.ListPage;
                }
                else {
                    self.BaoCaoChiTiet.data = [];
                    self.BaoCaoChiTiet.SumTongTienHang = 0;
                    self.BaoCaoChiTiet.SumDaTrich = 0;

                    self.Paging.TotalRow = x.dataSoure.TotalRow;
                    self.Paging.PageView = x.dataSoure.PageView;
                    self.Paging.NumberOfPage = x.dataSoure.NumOfPage;
                    self.Paging.ListPage = x.dataSoure.ListPage;
                }
            }).always(function () {
                $('#tblDetail').gridLoader({ show: false })
            })
        },
        showModalThemMoi: function () {
            let self = this;
            vmHoaHongKhachGioiThieu.showModal();
        },
        showModalUpdate: async function (item) {
            vmHoaHongKhachGioiThieu.showModalUpdate(item.ID);
        },
        RowSelected: async function (item) {
            let self = this;
            var $this = $(event.currentTarget).closest('tr');
            if (!$this.hasClass('active')) {
                $('tr').removeClass('active');
                $this.addClass('active');
            }
            else {
                $this.removeClass('active');
            }
            var $trdetail = $this.next();
            $('.op-js-tr-hide').css('display', 'none');
            $('.op-js-tr-hide').not($trdetail).removeClass('active');

            if (!$trdetail.hasClass('active')) {
                $trdetail.css('display', 'table-row');
                $trdetail.addClass('active');
            }
            else {
                $trdetail.css('display', 'none');
                $trdetail.removeClass('active');
            }
            $('div[id^=nky_]').removeClass('active');
            $('#infor_' + item.ID).addClass('active');
            if (self.inforOld.ID !== item.ID) {
                let ct = await vmHoaHongKhachGioiThieu.LoadChiTiet(item.ID);
                self.HoaDonChiTiet = ct;
            }
            self.inforOld = $.extend({}, true, item);
        },
        ChangeTab: async function (type, item) {
            let self = this;
            switch (type) {
                case 0:
                    $('div[id^=nky_]').removeClass('active');
                    $('#infor_' + item.ID).addClass('active');
                    break;
                case 1:
                    $('div[id^=infor]').removeClass('active');
                    $('#nky_' + item.ID).addClass('active');

                    let his = await self.GetLichSuThanhToan(item.ID);
                    self.LichSuThanhToan = his;
                    break;
            }
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
        PageChange: function (value) {
            let self = this;
            if (self.Paging.CurrentPage !== value.currentPage) {
                self.Paging.CurrentPage = value.currentPage;
            } else if (self.Paging.PageSize !== value.pageSize) {
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
        ExportExcel: function () {
            let self = this;
            let param = self.GetParam();
            param.PageSize = self.Paging.TotalRow;
            param.ReportBranch = self.listData.ChiNhanh.filter(p => p.CNChecked).map(p => p.TenDonVi).toString();
            param.ReportTime = moment(param.DateFrom, 'YYYY-MM-DD').format('DD/MM/YYYY').concat(' - ',
                moment(param.DateTo, 'YYYY-MM-DD').add('days', -1).format('DD/MM/YYYY'));

            ajaxHelper(self.UrlAPI.HoaDon + 'Export_PhieuTrichHoaHong', 'POST', param).done(function (pathFile) {
                if (pathFile !== '') {
                    let url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + pathFile;
                    window.location.href = url;
                    commonStatisJs.ShowMessageSuccess("Xuất file thành công");

                    let diary = {
                        ID_DonVi: VHeader.IdDonVi,
                        ID_NhanVien: VHeader.IdNhanVien,
                        LoaiNhatKy: 6,
                        ChucNang: 'Phiếu trích hoa hồng',
                        NoiDung: 'Xuất file phiếu trích hoa hồng',
                        NoiDungChiTiet: 'Xuất file phiếu trích hoa hồng'.
                            concat('<br /> Người xuất: ', VHeader.UserLogin),
                    }
                    Insert_NhatKyThaoTac_1Param(diary);
                }
            })
        }
    }
})

$('#vmHoaHongKhachGioiThieu').on('hidden.bs.modal', function () {
    debugger
    if (vmHoaHongKhachGioiThieu.saveOK) {
        vmDanhSachHoaHongGioiThieu.ResetCurrentPage_andLoadData();
    }
})