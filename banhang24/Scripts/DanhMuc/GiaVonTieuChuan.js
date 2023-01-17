var vmGiaVonTieuChuan = new Vue({
    el: '#vmGiaVonTieuChuan',
    components: {
        'my-date-time': cpmDatetime,
    },
    created: function () {
        let self = this;
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
                let notEx = self.Header_CheckKeyNotExist(localHeader, initHeader);
                if (Object.keys(initHeader[0]).length !== Object.keys(localHeader[0]).length || notEx) {
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
                let notEx = self.Header_CheckKeyNotExist(localHeader, initHeader);
                if (Object.keys(initHeader[0]).length !== Object.keys(localHeader[0]).length || notEx) {
                    self.BaoCaoChiTietHeader = initHeader;
                }
                else {
                    self.BaoCaoChiTietHeader = JSON.parse(localStorage.getItem('DMGiaVon_ChiTiet'));
                }
            }
        }
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
        NgayLapHoaDon_Change: null,
        InvoiceChosing: {},

        filter: {
            TextSearch: '',
            TT_TamLuu: false,
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
            data: [],
            ListPage: [1],
            PageView: "",
            NumberOfPage: 1,
            currentPage: 1,
            PageSize: 10,
        },
        BaoCaoChiTietHeader: [],
        BaoCaoChiTiet: {
            data: [],
            ListPage: [1],
            PageView: "",
            NumberOfPage: 1,
            currentPage: 1,
            PageSize: 10,
        },
        BH_HoaDon_ChiTiet: {
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
        Header_CheckKeyNotExist: function (localHeader = [], initHeader = []) {
            let local_arrCol = $.map(localHeader, function (x) {
                return x.colName;
            });
            let init_arrCol = $.map(initHeader, function (x) {
                return x.colName;
            });
            let arrNotEx = $.grep(local_arrCol, function (x) {
                return $.inArray(x, init_arrCol) === - 1;
            });
            return arrNotEx.length > 0;
        },
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
            { colName: 'colTenNhomHang', colText: 'Tên nhóm hàng', colShow: false, index: 2 },
            { colName: 'colMaHangHoa', colText: 'Mã hàng hóa', colShow: true, index: 3 },
            { colName: 'colTenHangHoa', colText: 'Tên hàng hóa', colShow: true, index: 4 },
            { colName: 'colTenDonViTinh', colText: 'Đơn vị tính', colShow: false, index: 5 },
            { colName: 'colGiaBan', colText: 'Giá bán', colShow: true, index: 6 },
            { colName: 'colGiaVonMoi', colText: 'Giá vốn tiêu chuẩn', colShow: true, index: 7 },
            { colName: 'colGiaVonTB', colText: 'Giá vốn trung bình', colShow: true, index: 8 },
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
            let sChiNhanh = self.listData.ChiNhanhs.filter(p => p.CNChecked === true).map(p => p.TenDonVi).toString();
            let param = self.GetParamSearch();
            switch (parseInt(self.LoaiBaoCao)) {
                case 1:
                    param.PageSize = self.BaoCaoTongHop.TotalRow;
                    param.ColumnHide = self.BaoCaoTongHopHeader.filter(p => p.colShow === false).map(p => p.index);
                    param.ReportTime = self.BaoCaoThoiGianText;
                    param.ReportBranch = sChiNhanh;
                    urlExport = 'ExportExcel_';
                    break;
                case 2:
                    param.PageSize = self.BaoCaoChiTiet.TotalRow;
                    param.ColumnHide = self.BaoCaoChiTietHeader.filter(p => p.colShow === false).map(p => p.index);
                    param.ReportTime = self.BaoCaoThoiGianText;
                    param.ReportBranch = sChiNhanh;
                    urlExport = 'Export_GiaVonTieuChuan_ChiTiet';
                    break;
            }
            ajaxHelper(self.urlApi.BHDieuChinh + urlExport, 'POST', param).done(function (pathFile) {
                if (pathFile!=='') {
                    let url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + pathFile;
                    window.location.href = url;
                    commonStatisJs.ShowMessageSuccess("Xuất file thành công");

                    let diary = {
                        ID_DonVi: VHeader.IdDonVi,
                        ID_NhanVien: VHeader.IdNhanVien,
                        LoaiNhatKy: 6,
                        ChucNang: 'Danh mục giá vốn tiêu chuẩn',
                        NoiDung: 'Xuất file danh mục giá vốn tiêu chuẩn',
                        NoiDungChiTiet: 'Xuất file danh mục giá vốn tiêu chuẩn '.
                            concat(parseInt(self.LoaiBaoCao) == 1 ? ' - tổng hợp' : ' - chi tiết',
                                '<br /> Người xuất: ', VHeader.UserLogin),
                    }
                    Insert_NhatKyThaoTac_1Param(diary);
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
        BaoCaoChiTietPageChange: function (value) {
            let self = this;
            if (self.BaoCaoChiTiet.currentPage !== value.currentPage) {
                self.BaoCaoChiTiet.currentPage = value.currentPage;
                self.LoadBaoCaoChiTiet();
            } else if (self.BaoCaoChiTiet.PageSize !== value.pageSize) {
                self.BaoCaoChiTiet.currentPage = 1;
                self.LoadBaoCaoChiTiet();
            }
        },
        BeforeLoadData: function () {
            let self = this;
            console.log('arr ', self.listData.ChiNhanhs, VHeader.ListChiNhanh);
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
                arrTT.push(1);
            }
            if (self.filter.TT_HoanThanh) {
                arrTT.push(0);
            }
            if (self.filter.TT_Huy) {
                arrTT.push(2);
            }

            let from = self.filter.DateFrom;
            let to = self.filter.DateTo;
            if (commonStatisJs.CheckNull(from)) {
                from = '2016-01-01';
            }
            if (commonStatisJs.CheckNull(to)) {
                to = moment(new Date()).add('days', 1).format('YYYY-DD-DD');
            }

            return {
                IDChiNhanhs: self.listData.ChiNhanhs.filter(p => p.CNChecked === true).map(p => p.ID),
                DateFrom: from,
                DateTo: to,
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

            $('#vmGiaVonTieuChuan').gridLoader({ show: true });
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
                $('#vmGiaVonTieuChuan').gridLoader({ show: false });
            })
        },
        LoadBaoCaoChiTiet: function () {
            let self = this;
            self.isLoading = true;

            let param = self.GetParamSearch();
            param.CurrentPage = self.BaoCaoChiTiet.currentPage - 1;
            param.PageSize = self.BaoCaoChiTiet.PageSize;
            $('#vmGiaVonTieuChuan').gridLoader({ show: true });

            ajaxHelper(self.urlApi.BHDieuChinh + "GetListGiaVonTieuChuan_ChiTiet", 'POST', param).done(function (data) {
                console.log(data)
                if (data.res === true) {
                    let obj = data.dataSoure;
                    obj.data.map(function (x) {
                        x["GiaVonMoi"] = formatNumber3Digit(x.ThanhTien) // used to editGiaVon
                    })
                    self.BaoCaoChiTiet.data = obj.data;
                    self.BaoCaoChiTiet.ListPage = obj.listpage;
                    self.BaoCaoChiTiet.PageView = obj.pageview;
                    self.BaoCaoChiTiet.TotalRow = obj.totalRow;
                }
            }).always(function () {
                self.isLoading = false;
                self.onRefresh = false;
                $('#vmGiaVonTieuChuan').gridLoader({ show: false });
            })
        },
        LoadChiTiet_PhieuDieuChinh: function (item) {
            let self = this;
            self.InvoiceChosing = item;
            self.NgayLapHoaDon_Change = item.NgayLapHoaDon;

            let from = self.filter.DateFrom;
            let to = self.filter.DateTo;
            if (commonStatisJs.CheckNull(from)) {
                from = '2016-01-01';
            }
            if (commonStatisJs.CheckNull(to)) {
                to = moment(new Date()).add('days', 1).format('YYYY-DD-DD');
            }
            let param = {
                IDChiNhanhs: self.listData.ChiNhanhs.filter(p => p.CNChecked === true).map(p => p.ID),
                DateFrom: from,
                DateTo: to,
                TrangThais: [0, 1, 2],
                TextSearch: item.MaHoaDon,
                CurrentPage: 0,
                PageSize: 10000,
            }
            ajaxHelper(self.urlApi.BHDieuChinh + "GetListGiaVonTieuChuan_ChiTiet", 'POST', param).done(function (data) {
                if (data.res === true) {
                    let obj = data.dataSoure;
                    self.BH_HoaDon_ChiTiet.data = obj.data;
                    self.BH_HoaDon_ChiTiet.ListPage = obj.listpage;
                    self.BH_HoaDon_ChiTiet.PageView = obj.pageview;
                    self.BH_HoaDon_ChiTiet.TotalRow = obj.totalRow;
                }
            }).always(function () {
            })
        },
        ChangeStatus: function () {
            let self = this;
            self.BaoCaoTongHop.currentPage = 1;
            self.BaoCaoChiTiet.currentPage = 1;
            self.LoadData();
        },

        onCallThoiGian: function (value) {
            let self = this;
            if (self.filter.DateFrom !== value.fromdate || self.filter.DateTo !== value.todate) {
                if (value.fromdate !== '2016-01-01') {
                    self.filter.DateFrom = value.fromdate;
                    self.filter.DateTo = value.todate;
                    self.BaoCaoThoiGianText = 'Từ ngày ' + moment(self.filter.DateFrom).format('DD/MM/YYYY') + ' đến ngày ' + moment(self.filter.DateTo).add(-1, 'days').format('DD/MM/YYYY');
                }
                else {
                    self.filter.DateFrom = '';
                    self.filter.DateTo = '';
                    self.BaoCaoThoiGianText = 'Toàn thời gian';
                }
                if (self.onRefresh === false) {
                    self.LoadData();
                }
            }
            self.filter.TypeTime = value.radioselect;
            self.LoadData();
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
        },
        ThemPhieuDieuChinh: function () {
            let self = this;
            window.open('/#/ThemPhieuDieuChinh');
        },
        editGiaVon: function (item) {
            let self = this;
            let $this = $(event.currentTarget);
            formatNumberObj($this);
            let gtri = formatNumberToFloat($this.val());
            $.getJSON(self.urlApi.BHDieuChinh + 'UpdateGiaVonTieuChuan_ofCTHD?id=' + item.ID_ChiTiet + '&giavonMoi=' + gtri).done(function (x) {
                if (x.res) {
                    ShowMessage_Success('Cập nhật thành công');
                    let diary = {
                        ID_DonVi: VHeader.IdDonVi,
                        ID_NhanVien: VHeader.IdNhanVien,
                        LoaiNhatKy: 2,
                        ChucNang: 'Giá vốn tiêu chuẩn',
                        NoiDung: 'Cập nhật giá vốn tiêu chuẩn của hàng ' + item.MaHangHoa + ' thuộc phiếu ' + item.MaHoaDon,
                        NoiDungChiTiet: 'Cập nhật giá vốn tiêu chuẩn của hàng '.concat(item.MaHangHoa,
                            '<br /> <b> Thông tin chi tiết: </b>',
                            '<br /> Tên hàng: ', item.TenHangHoa, ' (', item.MaHangHoa, ')',
                            '<br /> Giá vốn cũ: ', formatNumber3Digit(item.ThanhTien),
                            '<br /> Giá vốn mới: ', formatNumber3Digit(gtri),
                            '<br /> Mã phiếu: ', item.MaHoaDon,
                            '<br /> Thời gian: ', moment(item.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'),
                            '<br /> Người sửa: ', VHeader.UserLogin,
                            '<br /> Ngày sửa: ', moment(new Date()).format('DD/MM/YYYY HH:mm')
                        ),
                    }
                    Insert_NhatKyThaoTac_1Param(diary);
                }
                else {
                    ShowMessage_Danger('Xóa thất bại');
                }
            })
        },
        DeleteRow: function (item) {
            let self = this;
            dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn xóa giá vốn tiêu chuẩn của hàng <b>' + item.MaHangHoa
                + ' </b> đã điều chỉnh vào ngày <b> ' + moment(item.NgayLapHoaDon).format('DD/MM/YYYY HH:mm') + ' </b> không?', function () {
                    $.getJSON(self.urlApi.BHDieuChinh + 'DeleteCTHD_byID?id=' + item.ID_ChiTiet).done(function (x) {
                        if (x.res) {
                            ShowMessage_Success('Xóa thành công');
                            let diary = {
                                ID_DonVi: VHeader.IdDonVi,
                                ID_NhanVien: VHeader.IdNhanVien,
                                LoaiNhatKy: 3,
                                ChucNang: 'Giá vốn tiêu chuẩn',
                                NoiDung: 'Xóa giá vốn tiêu chuẩn của hàng ' + item.MaHangHoa + ' thuộc phiếu ' + item.MaHoaDon,
                                NoiDungChiTiet: 'Xóa giá vốn tiêu chuẩn của hàng '.concat(item.MaHangHoa,
                                    '<br /> <b> Thông tin xóa: </b>',
                                    '<br /> Tên hàng: ', item.TenHangHoa, ' (', item.MaHangHoa, ')',
                                    '<br /> Giá vốn tiêu chuẩn: ', formatNumber3Digit(item.ThanhTien),
                                    '<br /> Giá vốn trung bình: ', formatNumber3Digit(item.GiaVon),
                                    '<br /> Mã phiếu: ', item.MaHoaDon,
                                    '<br /> Thời gian: ', moment(item.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'),
                                    '<br /> Người xóa: ', VHeader.UserLogin
                                ),
                            }
                            Insert_NhatKyThaoTac_1Param(diary);
                            self.LoadBaoCaoChiTiet();
                        }
                        else {
                            ShowMessage_Danger('Xóa thất bại');
                        }
                    })
                })
        },
        ChangeNgayLapHoaDon: function (e) {
            let self = this;
            let dt = moment(e).format('YYYY-MM-DD HH:mm');
            self.NgayLapHoaDon_Change = dt;
        },
        editDienGiai: function () {
            let self = this;
            //self.InvoiceChosing.DienGiai = dt;
        },
        Update_HoaDon: function (item) {
            let self = this;
            $.getJSON('/api/DanhMuc/BH_HoaDonAPI/' + 'UpdateHoaDon_Common?id=' + item.ID
                + '&ngaylap=' + self.NgayLapHoaDon_Change
                + '&diengiai=' + item.DienGiai
                + '&nguoisua=' + VHeader.UserLogin
            ).done(function (x) {
                if (x.res) {
                    ShowMessage_Success('Cập nhật thành công');
                    let diary = {
                        ID_DonVi: VHeader.IdDonVi,
                        ID_NhanVien: VHeader.IdNhanVien,
                        LoaiNhatKy: 2,
                        ChucNang: 'Giá vốn tiêu chuẩn',
                        NoiDung: 'Cập nhật phiếu điều chỉnh ' + item.MaHoaDon,
                        NoiDungChiTiet: 'Cập nhật phiếu điều chỉnh '.concat(item.MaHoaDon,
                            '<br /> <b> Thông tin phiếu: </b>',
                            '<br /> Mã phiếu: ', item.MaHoaDon,
                            '<br /> Thời gian: ', self.NgayLapHoaDon_Change,
                            '<br /> Người sửa: ', VHeader.UserLogin,
                            '<br /> <b> Thông tin cũ: </b>',
                            '<br /> Thời gian: ', moment(self.InvoiceChosing.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'),
                            '<br /> Ghi chú: ', self.InvoiceChosing.DienGiai
                        ),
                    }
                    Insert_NhatKyThaoTac_1Param(diary);
                }
                else {
                    ShowMessage_Danger('Cập nhật phiếu thất bại');
                }
            })
        },
        Huy_HoaDon: function (item) {
            let self = this;
            dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn hủy phiếu điều chỉnh <b>' + item.MaHoaDon + ' </b> không?', function () {
                $.getJSON(self.urlApi.BHDieuChinh + 'Huy_PhieuDieuChinh?id=' + item.ID).done(function (x) {
                    if (x.res) {
                        ShowMessage_Success('Hủy phiếu thành công');
                        let diary = {
                            ID_DonVi: VHeader.IdDonVi,
                            ID_NhanVien: VHeader.IdNhanVien,
                            LoaiNhatKy: 3,
                            ChucNang: 'Giá vốn tiêu chuẩn',
                            NoiDung: 'Hủy phiếu điều chỉnh ' + item.MaHoaDon,
                            NoiDungChiTiet: 'Hủy phiếu điều chỉnh '.concat(item.MaHoaDon,
                                '<br /> <b> Thông tin phiếu: </b>',
                                '<br /> Mã phiếu: ', item.MaHoaDon,
                                '<br /> Thời gian: ', moment(item.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'),
                                '<br /> Người hủy: ', VHeader.UserLogin
                            ),
                        }
                        Insert_NhatKyThaoTac_1Param(diary);
                        self.LoadBaoCaoTongHop();
                    }
                    else {
                        ShowMessage_Danger('Hủy phiếu thất bại');
                    }
                })
            })
        },
        SaoChepUpdate_HoaDon: function (item, type = 0) {
            let self = this;
            let idHoaDon = item.ID;
            let cthdLoHang = [];

            for (let i = 0; i < self.BH_HoaDon_ChiTiet.data.length; i++) {
                var ctNew = $.extend({}, self.BH_HoaDon_ChiTiet.data[i]);
                ctNew.IDRandom = CreateIDRandom('CTHD_');
                ctNew.ThuocTinh_GiaTri = ctNew.ThuocTinhGiaTri;
                ctNew.SoThuTu = cthdLoHang.length + 1;
                ctNew.LotParent = false;
                ctNew.LaConCungLoai = false;
                ctNew.HangCungLoais = [];
                ctNew.DM_LoHang = [];
                ctNew.DonViTinh = [];

                let ngaysx = ctNew.NgaySanXuat !== null ? moment(ctNew.NgaySanXuat).format('DD/MM/YYYY') : '';
                let hethan = ctNew.NgayHetHan !== null ? moment(ctNew.NgayHetHan).format('DD/MM/YYYY') : '';

                if (ngaysx === 'Invalid date') {
                    ngaysx = '';
                }
                if (hethan === 'Invalid date') {
                    hethan = '';
                }
                ctNew.NgaySanXuat = ngaysx;
                ctNew.NgayHetHan = hethan;

                cthdLoHang.push(ctNew);
            }

            if (cthdLoHang.length > 0) {
                let idNhanVien = null, idDonVi = null, mahoadon = '', ngaylapHD = null;
                switch (type) {
                    case 0://copy
                        idNhanVien = VHeader.IdNhanVien;
                        idDonVi = VHeader.IdDonvi;
                        mahoadon = 'Copy' + item.MaHoaDon;
                        idHoaDon = const_GuidEmpty;
                        break;
                    case 1://update hd tamluu
                        idNhanVien = item.ID_NhanVien;
                        idDonVi = item.ID_DonVi;
                        mahoadon = item.MaHoaDon;
                        ngaylapHD = item.NgayLapHoaDon;
                        break;
                }
                cthdLoHang[0].ID_HoaDon = idHoaDon;
                cthdLoHang[0].ID_DonVi = idDonVi;
                cthdLoHang[0].ID_NhanVien = idNhanVien;
                cthdLoHang[0].MaHoaDon = mahoadon;
                cthdLoHang[0].NgayLapHoaDon = ngaylapHD;
                cthdLoHang[0].DienGiai = item.DienGiai;
            }
            localStorage.setItem('danhmuc_ctDieuChinh', JSON.stringify(cthdLoHang));
            localStorage.setItem('dieuchinh_typeCache', type);
            window.open('/#/ThemPhieuDieuChinh', '_blank');
        },
    },
    computed: {
        sPlaceholder: function () {
            let self = this;
            let sText = '';
            switch (parseInt(self.LoaiBaoCao)) {
                case 1:
                    sText = 'Mã phiếu, người tạo';
                    break;
                case 2:
                    sText = 'Mã phiếu, tên hàng hóa, mã hàng hóa';
                    break;
            }
            return sText;
        }
    }
})