var VueReport = new Vue({
    el: '#vmNhatKyHoatDongXe',
    data: {
        urlApi: {
            ChiNhanhApi: '/api/DanhMuc/DM_DonViAPI/',
            DoiTuongApi: '/api/DanhMuc/DM_DoiTuongAPI/',
            GaraApi: '/api/DanhMuc/GaraAPI/',
            ReportApi: '/api/DanhMuc/ReportAPI/',
            NhomHangHoaApi: '/api/DanhMuc/DM_NhomHangHoaAPI/'
        },
        LoaiBaoCao: '1',
        BaoCaoTieuDe: 'Báo cáo tổng hợp nhật ký hoạt động của xe',
        BaoCaoThoiGianText: '',
        listChiNhanh: [],
        ThoiGianTypeTime: 0,
        ThoiGianFrom: '',
        ThoiGianTo: '',
        SoGioHoatDongFrom: '',
        SoGioHoatDongTo: '',
        TextSearch: '',
        BaoCaoTongHopHeader: [],
        BaoCaoTongHop: {
            dataAll: [],
            data: [],
            ListPage: [1],
            PageView: "",
            NumberOfPage: 1,
            currentPage: 1,
            PageSize: 10,
            reLoad: true
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
            reLoad: true
        },

        isLoading: true,
        onRefresh: true,
        NhomHangHoa: [],
        NhomHangHoaSearch: '',
        IdNhomHangHoaSelected: ''
    },
    methods: {
        LoadChiNhanh: function () {
            let self = this;
            VHeader.ListChiNhanh.map(function (item) {
                if (item['ID'] === VHeader.IdDonVi) {
                    item['CNChecked'] = true;
                }
                else {
                    item['CNChecked'] = false;
                }
            });
            self.listChiNhanh = VHeader.ListChiNhanh;
            //self.LoadData();
        },
        LoadNhomHangHoa: function () {
            let self = this;
            $.ajax({
                url: self.urlApi.NhomHangHoaApi + "GetAllDMNhomHangHoa",
                type: 'GET',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    if (data !== null) {
                        self.NhomHangHoa = data.dataSoure.data;
                    }
                    else {
                        commonStatisJs.ShowMessageDanger("Có lỗi xảy ra trong quá trình tải dữ liệu. Vui lòng kiểm tra lại.");
                    }
                }
            });
        },
        TongHop_CheckColShow: function (colName) {
            let self = this;
            return self.BaoCaoTongHopHeader.find(x => x.colName === colName).colShow;
        },
        ChiTiet_CheckColShow: function (colName) {
            let self = this;
            return self.BaoCaoChiTietHeader.find(x => x.colName === colName).colShow;
        },
        SelectNhomHangHoa: function (value) {
            let self = this;
            self.IdNhomHangHoaSelected = value;
            self.BaoCaoChiTiet.reLoad = true;
            self.LoadData();
        },
        TreeFilter: function (data, text) {
            let self = this;
            var r = data.filter(function (o) {
                if (o.children) o.children = self.TreeFilter(o.children, text);
                return commonStatisJs.convertVieToEng(o.Item.Text).match(text);
            })
            return r;
        },
        BeforeLoadData: function () {
            this.BaoCaoTongHop.reLoad = true;
            this.BaoCaoChiTiet.reLoad = true;
            this.LoadData();
        },
        LoadData: function () {
            let self = this;
            self.isLoading = true;
            switch (parseInt(self.LoaiBaoCao)) {
                case 1:
                    self.BaoCaoTieuDe = 'Báo cáo tổng hợp nhật ký hoạt động của xe';
                    self.LoadBaoCaoTongHop();
                    break;
                case 2:
                    self.BaoCaoTieuDe = 'Báo cáo chi tiết nhật ký hoạt động của xe';
                    self.LoadBaoCaoTongHop();
                    break;
            }
            self.onRefresh = false;
        },
        LoadBaoCaoTongHop: function () {
            let self = this;
            if (self.BaoCaoTongHop.reLoad) {
                let myData = {};
                myData.IdChiNhanhs = self.listChiNhanh.filter(p => p.CNChecked === true).map(p => p.ID);;
                myData.ThoiGianFrom = self.ThoiGianFrom;
                myData.ThoiGianTo = self.ThoiGianTo;
                myData.TextSearch = self.TextSearch;
                myData.SoGioFrom = 0;
                myData.SoGioTo = 0;
                $.ajax({
                    url: self.urlApi.ReportApi + "GetBaoCaoDoanhThuSuaChuaTongHop",
                    type: 'POST',
                    dataType: 'json',
                    data: myData,
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (data) {
                        if (data.res === true) {
                            self.BaoCaoTongHop.dataAll = data.dataSoure.data;
                            self.BaoCaoTongHop.currentPage = 1;
                            self.BaoCaoTongHop.data = self.BaoCaoTongHop.dataAll.slice((self.BaoCaoTongHop.currentPage - 1) * self.BaoCaoTongHop.PageSize, self.BaoCaoTongHop.PageSize);
                            self.GetPageValue();
                        }
                        else {
                            commonStatisJs.ShowMessageDanger("Có lỗi xảy ra trong quá trình tải dữ liệu. Vui lòng kiểm tra lại.");
                        }
                        self.isLoading = false;
                    }
                });
                self.BaoCaoTongHop.reLoad = false;
            }
            else {
                self.isLoading = false;
            }
        },
        LoadBaoCaoChiTiet: function () {
            let self = this;
            if (self.BaoCaoChiTiet.reLoad) {
                let myData = {};
                myData.IdChiNhanhs = self.listChiNhanh.filter(p => p.CNChecked === true).map(p => p.ID);;
                myData.ThoiGianFrom = self.ThoiGianFrom;
                myData.ThoiGianTo = self.ThoiGianTo;
                myData.TextSearch = self.TextSearch;
                myData.IdNhomHangHoa = self.IdNhomHangHoaSelected;
                myData.SoGioFrom = 0;
                myData.SoGioTo = 0;

                $.ajax({
                    url: self.urlApi.ReportApi + "GetBaoCaoDoanhThuSuaChuaChiTiet",
                    type: 'POST',
                    dataType: 'json',
                    data: myData,
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (data) {
                        if (data.res === true) {
                            self.BaoCaoChiTiet.dataAll = data.dataSoure.data;
                            self.BaoCaoChiTiet.currentPage = 1;
                            self.BaoCaoChiTiet.data = self.BaoCaoChiTiet.dataAll.slice((self.BaoCaoChiTiet.currentPage - 1) * self.BaoCaoChiTiet.PageSize, self.BaoCaoChiTiet.PageSize);
                            self.GetPageValue();
                        }
                        else {
                            commonStatisJs.ShowMessageDanger("Có lỗi xảy ra trong quá trình tải dữ liệu. Vui lòng kiểm tra lại.");
                        }
                        self.isLoading = false;
                    }
                });
                self.BaoCaoChiTiet.reLoad = false;
            }
            else {
                self.isLoading = false;
            }
        },
        BaoCaoTongHopPageChange: function (value) {
            let self = this;
            if (self.BaoCaoTongHop.currentPage !== value.currentPage) {
                self.BaoCaoTongHop.currentPage = value.currentPage;
                self.BaoCaoTongHop.data = self.BaoCaoTongHop.dataAll.slice((self.BaoCaoTongHop.currentPage - 1) * self.BaoCaoTongHop.PageSize, self.BaoCaoTongHop.PageSize * self.BaoCaoTongHop.currentPage);
                self.GetPageValue();
            } else if (self.BaoCaoTongHop.PageSize !== value.pageSize) {
                self.BaoCaoTongHop.currentPage = 1;
                self.BaoCaoTongHop.PageSize = value.pageSize;
                self.BaoCaoTongHop.data = self.BaoCaoTongHop.dataAll.slice((self.BaoCaoTongHop.currentPage - 1) * self.BaoCaoTongHop.PageSize, self.BaoCaoTongHop.PageSize * self.BaoCaoTongHop.currentPage);
                self.GetPageValue();
            }
        },
        BaoCaoChiTietPageChange: function (value) {
            let self = this;
            if (self.BaoCaoChiTiet.currentPage !== value.currentPage) {
                self.BaoCaoChiTiet.currentPage = value.currentPage;
                self.BaoCaoChiTiet.data = self.BaoCaoChiTiet.dataAll.slice((self.BaoCaoChiTiet.currentPage - 1) * self.BaoCaoChiTiet.PageSize, self.BaoCaoChiTiet.PageSize * self.BaoCaoChiTiet.currentPage);
                self.GetPageValue();
            } else if (self.BaoCaoChiTiet.PageSize !== value.pageSize) {
                self.BaoCaoChiTiet.currentPage = 1;
                self.BaoCaoChiTiet.PageSize = value.pageSize;
                self.BaoCaoChiTiet.data = self.BaoCaoChiTiet.dataAll.slice((self.BaoCaoChiTiet.currentPage - 1) * self.BaoCaoChiTiet.PageSize, self.BaoCaoChiTiet.PageSize * self.BaoCaoChiTiet.currentPage);
                self.GetPageValue();
            }
        },

        onCallThoiGian: function (value) {
            let self = this;
            if (self.ThoiGianFrom !== value.fromdate || self.ThoiGianTo !== value.todate) {
                this.BaoCaoTongHop.reLoad = true;
                this.BaoCaoChiTiet.reLoad = true;
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
                if (self.onRefresh === false)
                    self.LoadData();
            }
            self.ThoiGianTypeTime = value.radioselect;
        },
        InitBaoCaoTongHopHeader: function () {
            return [{ colName: 'colTenChiNhanh', colText: 'Tên chi nhánh', colShow: true, index: 0 },
            { colName: 'colBienSo', colText: 'Biển số xe', colShow: true, index: 1 },
            { colName: 'colTongSoGioThucHien', colText: 'Tổng số giờ thực hiện', colShow: true, index: 2 },
            { colName: 'colTenNhomHang', colText: 'Tên nhóm hàng', colShow: true, index: 3 },
            { colName: 'colMaHangHoa', colText: 'Mã hàng hóa', colShow: true, index: 4 },
            { colName: 'colTenHangHoa', colText: 'Tên hàng hóa', colShow: true, index: 5 },
            { colName: 'colDonViTinh', colText: 'ĐVT', colShow: true, index: 6 },
            { colName: 'colMocBaoHanh', colText: 'Mốc bảo hành', colShow: true, index: 7 },
            { colName: 'colBHConLai', colText: 'Còn lại', colShow: true, index: 8 },
            { colName: 'colBHTrangThai', colText: 'Trạng thái BH', colShow: true, index: 9 }];
        },
        InitBaoCaoChiTietHeader: function () {
            return [{ colName: 'colTenChiNhanh', colText: 'Tên chi nhánh', colShow: true, index: 0 },
            { colName: 'colMaTiepNhan', colText: 'Mã tiếp nhận', colShow: true, index: 1 },
            { colName: 'colNgayTiepNhan', colText: 'Ngày tiếp nhận', colShow: true, index: 2 },
            { colName: 'colBienSo', colText: 'Biển số xe', colShow: true, index: 3 },
            { colName: 'colSoGioHoatDong', colText: 'Số giờ hoạt động', colShow: true, index: 4 },
            { colName: 'colNVThucHien', colText: 'Nhân viên thực hiện', colShow: true, index: 5 },
            { colName: 'GhiChu', colText: 'Ghi chú', colShow: true, index: 6 },
            { colName: 'colTenNhomHang', colText: 'Tên nhóm hàng', colShow: true, index: 7 },
            { colName: 'colMaHangHoa', colText: 'Mã hàng hóa', colShow: true, index: 8 },
            { colName: 'colTenHangHoa', colText: 'Tên hàng hóa', colShow: true, index: 9 },
            { colName: 'colDonViTinh', colText: 'ĐVT', colShow: true, index: 10 },];
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
        GetPageValue: function () {
            let self = this;
            switch (parseInt(self.LoaiBaoCao)) {
                case 1:
                    var hienthiFrom = ((self.BaoCaoTongHop.currentPage - 1) * self.BaoCaoTongHop.PageSize) + 1;
                    var hienthiTo = '';
                    var sobanghi = self.BaoCaoTongHop.dataAll.length;
                    if (self.BaoCaoTongHop.currentPage * self.BaoCaoTongHop.PageSize < sobanghi) {
                        hienthiTo = self.BaoCaoTongHop.currentPage * self.BaoCaoTongHop.PageSize;
                    }
                    else {
                        hienthiTo = sobanghi;
                    }
                    self.BaoCaoTongHop.PageView = 'Hiển thị ' + hienthiFrom + ' - ' + hienthiTo + ' trên tổng số ' + sobanghi + ' bản ghi';
                    self.BaoCaoTongHop.NumberOfPage = Math.ceil(sobanghi / self.BaoCaoTongHop.PageSize);
                    self.BaoCaoTongHop.ListPage = self.GetPageList(self.BaoCaoTongHop.NumberOfPage, self.BaoCaoTongHop.currentPage);
                    break;
                case 2:
                    var hienthiFrom = ((self.BaoCaoChiTiet.currentPage - 1) * self.BaoCaoChiTiet.PageSize) + 1;
                    var hienthiTo = '';
                    var sobanghi = self.BaoCaoChiTiet.dataAll.length;
                    if (self.BaoCaoChiTiet.currentPage * self.BaoCaoChiTiet.PageSize < sobanghi) {
                        hienthiTo = self.BaoCaoChiTiet.currentPage * self.BaoCaoChiTiet.PageSize;
                    }
                    else {
                        hienthiTo = sobanghi;
                    }
                    self.BaoCaoChiTiet.PageView = 'Hiển thị ' + hienthiFrom + ' - ' + hienthiTo + ' trên tổng số ' + sobanghi + ' bản ghi';
                    self.BaoCaoChiTiet.NumberOfPage = Math.ceil(sobanghi / self.BaoCaoChiTiet.PageSize);
                    self.BaoCaoChiTiet.ListPage = self.GetPageList(self.BaoCaoChiTiet.NumberOfPage, self.BaoCaoChiTiet.currentPage);
                    break;
            }
        },
        GetPageList: function (NumberOfPage, currentPage) {
            let page = NumberOfPage;
            let pagenow = currentPage;
            let listpage = [];
            if (page > 5) {
                if (pagenow > 2 && pagenow < (page - 2)) {
                    listpage = Array.from({ length: 5 }, (x, i) => i + pagenow - 2);
                }
                else if (pagenow >= (page - 2)) {
                    if (pagenow !== page) {
                        listpage = Array.from({ length: 5 }, (x, i) => i + pagenow - 3);
                    }
                    else {
                        listpage = Array.from({ length: 5 }, (x, i) => i + pagenow - 4);
                    }
                }
                else {
                    listpage = [1, 2, 3, 4, 5];
                }
            }
            else {
                if (page != 0) {
                    listpage = Array.from({ length: page }, (x, i) => i + 1);
                }
            }
            return listpage;
        },
        EnterKeyup: function (e) {
            if (e.keyCode === 13) {
                this.BaoCaoTongHop.reLoad = true;
                this.BaoCaoChiTiet.reLoad = true;
                this.LoadData();
            }
        },
        ExportExcel: function () {
            let self = this;
            let urlExport = '';
            let myData = {};
            myData.BaoCaoThoiGian = self.BaoCaoThoiGianText;
            myData.BaoCaoChiNhanh = 'Chi nhánh: ' + self.listChiNhanh.filter(p => p.CNChecked === true).map(p => p.TenDonVi).toString();
            switch (parseInt(self.LoaiBaoCao)) {
                case 1:
                    urlExport = 'ExportExcel_GetBaoCaoDoanhThuSuaChuaTongHop';
                    myData.IdChiNhanhs = self.listChiNhanh.filter(p => p.CNChecked === true).map(p => p.ID);
                    myData.ThoiGianFrom = self.ThoiGianFrom;
                    myData.ThoiGianTo = self.ThoiGianTo;
                    myData.TextSearch = self.TextSearch;
                    myData.SoGioFrom = 0;
                    myData.SoGioTo = 0;
                    myData.ColHide = self.BaoCaoTongHopHeader.filter(p => p.colShow === false).map(p => p.index);
                    break;
                case 2:
                    urlExport = 'ExportExcel_GetBaoCaoDoanhThuSuaChuaChiTiet';
                    myData.IdChiNhanhs = self.listChiNhanh.filter(p => p.CNChecked === true).map(p => p.ID);
                    myData.ThoiGianFrom = self.ThoiGianFrom;
                    myData.ThoiGianTo = self.ThoiGianTo;
                    myData.DoanhThuFrom = self.DoanhThuFrom.replace(/,/gi, '');
                    myData.DoanhThuTo = self.DoanhThuTo.replace(/,/gi, '');
                    myData.LoiNhuanFrom = self.LoiNhuanFrom.replace(/,/gi, '');
                    myData.LoiNhuanTo = self.LoiNhuanTo.replace(/,/gi, '');
                    myData.TextSearch = self.TextSearch;
                    myData.ColHide = self.BaoCaoChiTietHeader.filter(p => p.colShow === false).map(p => p.index);
                    break;
            }

            $.ajax({
                url: self.urlApi.ReportApi + urlExport,
                type: 'POST',
                dataType: 'json',
                data: myData,
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    if (data.res === true) {
                        var url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + data.mess;
                        window.location.href = url;
                        commonStatisJs.ShowMessageSuccess("Xuất file thành công.");
                    }
                    else {
                        commonStatisJs.ShowMessageDanger("Có lỗi xảy ra trong quá trình tải dữ liệu. Vui lòng kiểm tra lại.");
                    }
                }
            });
        },
    },
    created: function () {
        var self = this;
        self.LoadChiNhanh();
        self.LoadNhomHangHoa();
        if (localStorage.getItem('RpNKyXe_TongHop') === null) {
            self.BaoCaoTongHopHeader = self.InitBaoCaoTongHopHeader();
        }
        else {
            let localHeader = JSON.parse(localStorage.getItem('RpNKyXe_TongHop'));
            let initHeader = self.InitBaoCaoTongHopHeader();
            if (initHeader.length !== localHeader.length) {
                self.BaoCaoTongHopHeader = initHeader;
            }
            else {
                if (Object.keys(initHeader[0]).length !== Object.keys(localHeader[0]).length) {
                    self.BaoCaoTongHopHeader = initHeader;
                }
                else {
                    self.BaoCaoTongHopHeader = JSON.parse(localStorage.getItem('RpNKyXe_TongHop'));
                }
            }
        }
        if (localStorage.getItem('RpNKyXe_ChiTiet') === null) {
            self.BaoCaoChiTietHeader = self.InitBaoCaoChiTietHeader();
        }
        else {
            let localHeader = JSON.parse(localStorage.getItem('RpNKyXe_ChiTiet'));
            let initHeader = self.InitBaoCaoChiTietHeader();
            if (initHeader.length !== localHeader.length) {
                self.BaoCaoChiTietHeader = initHeader;
            }
            else {
                if (Object.keys(initHeader[0]).length !== Object.keys(localHeader[0]).length) {
                    self.BaoCaoChiTietHeader = initHeader;
                }
                else {
                    self.BaoCaoChiTietHeader = JSON.parse(localStorage.getItem('RpNKyXe_ChiTiet'));
                }
            }
        }
    },
    watch: {
        LoaiBaoCao: {
            handler: function () {
                this.LoadData();
            },
            deep: true
        },
        BaoCaoTongHopHeader: {
            handler: function () {
                localStorage.setItem('RpNKyXe_TongHop', JSON.stringify(this.BaoCaoTongHopHeader));
            },
            deep: true
        },
        BaoCaoChiTietHeader: {
            handler: function () {
                localStorage.setItem('RpNKyXe_ChiTiet', JSON.stringify(this.BaoCaoChiTietHeader));
            },
            deep: true
        },
    },
});