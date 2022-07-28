var vmGiaVonTieuChuan = new Vue({
    el: '#vmGiaVonTieuChuan',
    components: {
        'my-date': cpmDatetime,
    },
    created: function () {
        let self = this;

        let arr = VHeader.ListChiNhanh;
        arr.map(function (item) {
            if (item['ID'] === VHeader.IdDonVi) {
                item['CNChecked'] = true;
            }
            else {
                item['CNChecked'] = false;
            }
        });
        console.log('arr ', arr);
        self.listData.ChiNhanhs = arr;

        debugger
        if (localStorage.getItem('DMGiaVon_TongHop') === null) {
            self.BaoCaoTongHopHeader = self.InitBaoCaoTongHopHeader();
        }
        else {
            let localHeader = JSON.parse(localStorage.getItem('DMGiaVon_TongHop'));
            let initHeader = self.InitBaoCaoTongHopHeader();
            if (initHeader.length !== localHeader.length) {
                self.BaoCaoTongHopHeader = initHeader;
            }
            else {
                if (Object.keys(initHeader[0]).length !== Object.keys(localHeader[0]).length) {
                    self.BaoCaoTongHopHeader = initHeader;
                }
                else {
                    self.BaoCaoTongHopHeader = JSON.parse(localStorage.getItem('DMGiaVon_TongHop'));
                }
            }
        }
        if (localStorage.getItem('DMGiaVon_ChiTiet') === null) {
            self.BaoCaoChiTietHeader = self.InitBaoCaoChiTietHeader();
        }
        else {
            let localHeader = JSON.parse(localStorage.getItem('DMGiaVon_ChiTiet'));
            let initHeader = self.InitBaoCaoChiTietHeader();
            if (initHeader.length !== localHeader.length) {
                self.BaoCaoChiTietHeader = initHeader;
            }
            else {
                if (Object.keys(initHeader[0]).length !== Object.keys(localHeader[0]).length) {
                    self.BaoCaoChiTietHeader = initHeader;
                }
                else {
                    self.BaoCaoChiTietHeader = JSON.parse(localStorage.getItem('DMGiaVon_ChiTiet'));
                }
            }
        }
        self.LoadData();
    },
    data: {
        isLoading: false,
        LoaiBaoCao: '2',// 2.detai, 1.tonghop

        role: {
            View: true, Export: true, Insert: true, Update: false, Delete: false, Copy: true
        },
        urlApi: {
            BHDieuChinh: '/api/DanhMuc/BH_DieuChinh/',
        },

        BaoCaoTieuDe: 'Danh mục giá vốn tiêu chuẩn',
        BaoCaoThoiGianText: '',

        filter: {
            TextSearch: '',
            TT_TamLuu: true,
            TT_HoanThanh: true,
            TT_Huy: false,

            TypeTime: 0,
            DateFrom: null,
            DateTo: null,
        },

        listData: {
            ChiNhanhs: [],
        },

        BaoCaoTongHopHeader: [],
        BaoCaoTongHop: {
            dataAll: [],
            data: [],
            ListPage: [1],
            PageView: "",
            NumberOfPage: 1,
            currentPage: 1,
            PageSize: 10,
        },
        BaoCaoChiTietHeader: [],
        BaoCaoChiTiet: {
            dataAll: [],
            data: [],
            ListPage: [1],
            PageView: "",
            NumberOfPage: 1,
            currentPage: 1,
            PageSize: 10,
        },
    },
    watch: {
        LoaiBaoCao: {
            handler: function () {
                this.LoadData();
            },
            deep: true
        },
        filterTrangThai: {
            handler: function () {
                this.LoadData();
            },
            deep: true
        },
        BaoCaoTongHopHeader: {
            handler: function () {
                localStorage.setItem('DMGiaVon_TongHop', JSON.stringify(this.BaoCaoTongHopHeader));
            },
            deep: true
        },
        BaoCaoChiTietHeader: {
            handler: function () {
                localStorage.setItem('DMGiaVon_ChiTiet', JSON.stringify(this.BaoCaoChiTietHeader));
            },
            deep: true
        },
    },
    methods: {
        InitBaoCaoTongHopHeader: function () {
            return [{ colName: 'colMaHoaDon', colText: 'Mã phiếu', colShow: true, index: 0 },
            { colName: 'colNgayLapHoaDon', colText: 'Thời gian', colShow: true, index: 1 },
            { colName: 'colNguoiTao', colText: 'User lập phiếu', colShow: true, index: 2 },
            { colName: 'colTenChiNhanh', colText: 'Tên chi nhánh', colShow: false, index: 3 },
            { colName: 'colTongMatHang', colText: 'Tổng mặt hàng', colShow: true, index: 4 },
            { colName: 'colDienGiai', colText: 'Ghi chú', colShow: true, index: 5 },
            { colName: 'colTrangThai', colText: 'Trạng thái', colShow: true, index: 6 },
            ]
        },
        InitBaoCaoChiTietHeader: function () {
            return [{ colName: 'colMaHoaDon', colText: 'Mã phiếu', colShow: true, index: 0 },
            { colName: 'colNgayLapHoaDon', colText: 'Thời gian', colShow: true, index: 1 },
            { colName: 'colTenNhomHang', colText: 'Tên nhóm hàng', colShow: true, index: 2 },
            { colName: 'colMaHangHoa', colText: 'Mã hàng hóa', colShow: true, index: 3 },
            { colName: 'colTenHangHoa', colText: 'Tên hàng hóa', colShow: true, index: 4 },
            { colName: 'colGiaVonCu', colText: 'Giá vốn cũ', colShow: true, index: 5 },
            { colName: 'colGiaVonMoi', colText: 'Giá vốn mới', colShow: true, index: 6 },
            { colName: 'colGiaVonTB', colText: 'Giá vốn trung bình', colShow: true, index: 7 },
            { colName: 'colGhiChu', colText: 'Ghi chú', colShow: true, index: 8 },
            ];
        },

        GetListHeader: function () {
            let self = this;
            switch (parseInt(self.LoaiBaoCao)) {
                case 1:
                    return self.BaoCaoTongHopHeader;
                    break;
                case 2:
                    return self.BaoCaoChiTietHeader;
                    break;
            }
        },
        TongHop_CheckColShow: function (colName) {
            let self = this;
            return self.BaoCaoTongHopHeader.find(x => x.colName === colName).colShow;
        },
        ChiTiet_CheckColShow: function (colName) {
            let self = this;
            let data = self.BaoCaoChiTietHeader.find(x => x.colName === colName);
            if (data != undefined) {
                return data.colShow;
            }
            return true;
        },
        ExportExcel: function () {
            let self = this;
            switch (parseInt(self.LoaiBaoCao)) {
                case 1:
                    myData.DateTo = self.BCTongHop_ToDate;
                    myData.PageSize = self.BaoCaoTongHop.TotalRow;
                    myData.ReportText.ColumnHide = self.BaoCaoTongHopHeader.filter(p => p.colShow === false).map(p => p.index);
                    myData.ReportText.ReportTime = self.BaoCaoTongHop_ThoiGianText;
                    urlExport = 'ExportExcel_';
                    break;
                case 2:
                    myData.PageSize = self.BaoCaoChiTiet.TotalRow;
                    myData.ReportText.ColumnHide = self.BaoCaoChiTietHeader.filter(p => p.colShow === false).map(p => p.index);
                    myData.ReportText.ReportTime = self.BaoCaoThoiGianText;
                    urlExport = 'ExportExcel_';
                    break;
            }
            ajaxHelper(self.urlApi.BHDieuChinh + urlExport, 'POST', myData).done(function (x) {
                if (x.res === true) {
                    let url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + x.mess;
                    window.location.href = url;
                    commonStatisJs.ShowMessageSuccess("Xuất file thành công");

                    let diary = {
                        ID_DonVi: VHeader.IdDonVi,
                        ID_NhanVien: VHeader.IdNhanVien,
                        LoaiNhatKy: 6,
                        ChucNang: 'Danh mục giá vốn tiểu chuẩn',
                        NoiDung: 'Xuất file danh mục giá vốn tiểu chuẩn',
                        NoiDungChiTiet: 'Xuất file danh mục giá vốn tiểu chuẩn '.
                            concat(parseInt(self.LoaiBaoCao) == 1 ? ' - tổng hợp' : ' - chi tiết',
                                '<br /> Người xuất: ', VHeader.UserLogin),
                    }
                    Insert_NhatKyThaoTac_1Param(diary);
                }
                else {
                    commonStatisJs.ShowMessageDanger("Xuất file thất bại");
                }
            })
        },

        BaoCaoTongHopPageChange: function (value) {
            let self = this;
            if (self.BaoCaoTongHop.currentPage !== value.currentPage) {
                self.BaoCaoTongHop.currentPage = value.currentPage;
                self.LoadBaoCaoTongHop();
            } else if (self.BaoCaoTongHop.PageSize !== value.pageSize) {
                self.BaoCaoTongHop.currentPage = 1;
                self.LoadBaoCaoTongHop();
            }
        },
        BeforeLoadData: function () {
            let self = this;
            self.LoadData();
        },
        LoadData: function () {
            let self = this;
            self.isLoading = false;
            switch (parseInt(self.LoaiBaoCao)) {
                case 1:
                    self.BaoCaoTieuDe = 'Danh mục giá vốn tiêu chuẩn - chi tiết';
                    self.LoadBaoCaoTongHop();
                    break;
                case 2:
                    self.BaoCaoTieuDe = 'Danh mục giá vốn tiêu chuẩn - tổng hợp';
                    self.LoadBaoCaoChiTiet();
                    break;
            }
        },
        GetParamSearch: function () {
            let self = this;
            let arrTT = [];
            if (self.filter.TT_TamLuu) {
                arrTT.push(0);
            }
            if (self.filter.TT_HoanThanh) {
                arrTT.push(1);
            }
            if (self.filter.TT_Huy) {
                arrTT.push(2);
            }

            return {
                IDChiNhanhs: self.listData.ChiNhanhs.filter(p => p.CNChecked === true).map(p => p.ID),
                DateFrom: self.filter.DateFrom,
                DateTo: self.filter.DateTo,
                TrangThais: arrTT,
                TextSearch: self.filter.TextSearch,
            }
        },
        LoadBaoCaoTongHop: function () {
            let self = this;
            self.isLoading = true;
            let param = self.GetParamSearch();
            param.CurrentPage = self.BaoCaoTongHop.currentPage - 1;
            param.PageSize = self.BaoCaoTongHop.PageSize;
            
            ajaxHelper(self.urlApi.BHDieuChinh + "GetListGiaVonTieuChuan_TongHop", 'POST', param).done(function (data) {
                console.log(data)
                if (data.res === true) {
                    let obj = data.dataSoure;
                    self.BaoCaoTongHop.data = obj.data;
                    self.BaoCaoTongHop.ListPage = obj.listpage;
                    self.BaoCaoTongHop.PageView = obj.pageview;
                    self.BaoCaoTongHop.TotalRow = obj.totalRow;
                }
            }).always(function () {
                self.isLoading = false;
                self.onRefresh = false;
            })
        },
        LoadBaoCaoChiTiet: function () {
            let self = this;
            self.isLoading = true;

            let param = self.GetParamSearch();
            param.CurrentPage = self.BaoCaoChiTiet.currentPage - 1;
            param.PageSize = self.BaoCaoChiTiet.PageSize;

            ajaxHelper(self.urlApi.BHDieuChinh + "GetListGiaVonTieuChuan_ChiTiet", 'POST', param).done(function (data) {
                console.log(data)
                if (data.res === true) {
                    let obj = data.dataSoure;
                    self.BaoCaoChiTiet.data = obj.data;
                    self.BaoCaoChiTiet.ListPage = obj.listpage;
                    self.BaoCaoChiTiet.PageView = obj.pageview;
                    self.BaoCaoChiTiet.TotalRow = obj.totalRow;
                }
            }).always(function () {
                self.isLoading = false;
                self.onRefresh = false;
            })
        },
        ChangeDate: function (e) {
            var self = this;
            var dt = moment(e).format('YYYY-MM-DD');
            self.BCTongHop_ToDate = dt;
            self.BaoCaoTongHop.currentPage = 1;
            self.BaoCaoTongHop_ThoiGianText = 'Đến ngày: ' + moment(e).format('DD/MM/YYYY');
            self.LoadData();
        },

        onCallThoiGian: function (value) {
            let self = this;
            if (self.ThoiGianFrom !== value.fromdate || self.ThoiGianTo !== value.todate) {
                if (value.fromdate !== '2016-01-01') {
                    self.ThoiGianFrom = value.fromdate;
                    self.ThoiGianTo = value.todate;
                    self.BaoCaoThoiGianText = 'Từ ngày ' + moment(self.ThoiGianFrom).format('DD/MM/YYYY') + ' đến ngày ' + moment(self.ThoiGianTo).add(-1, 'days').format('DD/MM/YYYY');
                }
                else {
                    self.ThoiGianFrom = '';
                    self.ThoiGianTo = '';
                    self.BaoCaoThoiGianText = 'Toàn thời gian';
                }
                if (self.onRefresh === false) {
                    self.LoadData();
                }
            }
            self.ThoiGianTypeTime = value.radioselect;
            self.BaoCaoTongHop_ThoiGianText = 'Đến ngày: ' + moment(self.ThoiGianTo, 'YYYY-MM-DD').format('DD/MM/YYYY');
        },
        EnterKeyup: function (e) {
            if (e.keyCode === 13) {
                this.LoadData();
            }
        },
        gotoPage: function (item, typePage = 0) {
            let url = '';
            switch (typePage) {
                case 1:
                    localStorage.setItem('loadMaHang', item.MaHangHoa);
                    url = "/#/Product";
                    break;
            }
            window.open(url);
        }
    },
    computed: {

    }
})