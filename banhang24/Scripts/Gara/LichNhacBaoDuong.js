var vmLichBaoDuong = new Vue({
    el: '#vmLichBaoDuong',
    components: {
        'date-time': cpmDatetime
    },
    created: function () {
        var self = this;
        let localtionTemp = window.location.href.substring(window.location.href.lastIndexOf("/") + 1, window.location.href.length);
        if (localtionTemp.indexOf('?s=') > 0) {
            let searchTemp = localtionTemp.substring(localtionTemp.indexOf('?s=') + 3, localtionTemp.length);
            let indexOfT = searchTemp.indexOf('&t=');
            if (indexOfT === -1) {
                indexOfT = 0
            }
            if (indexOfT === 0) {
                self.filter.txtSearch = searchTemp.substring(0, searchTemp.length);
            }
            else {
                self.filter.txtSearch = searchTemp.substring(0, indexOfT);
                console.log(searchTemp.substring(indexOfT + 3, searchTemp.length));
                if (searchTemp.substring(indexOfT + 3, searchTemp.length) === '1') {
                    self.filter.NgayNhac_DefaultTypeTime = 1;
                }
            }
        }
        self.urlAPI = {
            Gara: '/api/DanhMuc/GaraAPI/'
        }
        self.inforLogin = {
            ID_DonVi: VHeader.IdDonVi,
            ID_User: VHeader.IdNguoiDung,
            UserLogin: VHeader.UserLogin,
            ID_NhanVien: VHeader.IdNhanVien,
        };
        self.onRefresh = true;
        self.PageLoad();
    },
    data: {
        onRefresh: true,
        role: {},
        inforLogin: {
            ID_DonVi: null,
            UserLogin: '',
            ID_User: '',
        },
        lichOld: {},
        lichUpdate: {
            LanNhac: 0,
            ngayNhacBaoDuong_update: null,
        },

        ListHeader: [],
        ListHeaderAll: [{ colName: 'BienSo', colText: 'Biển số xe', colShow: true, index: 1, Css: '' },
        { colName: 'MaDoiTuong', colText: 'Mã chủ xe', colShow: true, index: 2, Css: '' },
        { colName: 'TenDoiTuong', colText: 'Tên chủ xe', colShow: true, index: 3, Css: '' },
        { colName: 'DienThoai', colText: 'Điện thoại', colShow: true, index: 4, Css: '' },
        { colName: 'Email', colText: 'Email', colShow: false, index: 5, Css: '' },
        { colName: 'TenNhomHangHoa', colText: 'Tên nhóm hàng hóa', colShow: false, index: 6, Css: '' },
        { colName: 'MaHangHoa', colText: 'Mã hàng hóa', colShow: true, index: 7, Css: '' },
        { colName: 'TenHangHoa', colText: 'Tên hàng hóa', colShow: true, index: 8, Css: '' },
        { colName: 'LanBaoDuong', colText: 'Lần bảo dưỡng', colShow: true, index: 9, Css: '' },
        { colName: 'SoKmBaoDuong', colText: 'Số Km bảo dưỡng', colShow: true, index: 10, Css: '' },
        { colName: 'NgayBaoDuongDuKien', colText: 'Ngày bảo dưỡng dự kiến', colShow: true, index: 11, Css: '' },
        { colName: 'NgayNhacFrom', colText: 'Ngày nhắc bắt đầu', colShow: false, index: 12, Css: '' },
        { colName: 'NgayNhacTo', colText: 'Ngày nhắc kết thúc', colShow: false, index: 13, Css: '' },
        { colName: 'LanNhac', colText: 'Lần nhắc', colShow: false, index: 14, Css: '' },
        { colName: 'GhiChu', colText: 'Ghi chú', colShow: true, index: 15, Css: '' },
        { colName: 'TrangThai', colText: 'Trạng thái', colShow: true, index: 16, Css: '' },
        ],
        LanNhacs: [],
        TrangThais: [
            { Text: 'Chưa xử lý', Value: 1, Checked: true },
            { Text: 'Đã xử lý', Value: 2, Checked: false },
            { Text: 'Đã nhắc', Value: 3, Checked: true },
            { Text: 'Quá hạn', Value: 5, Checked: false },
            { Text: 'Đã hủy', Value: 0, Checked: false } // 0, 4
            ,
        ],
        listData: {
            HT_ThietLap: [],
            ChiNhanh: [],
            NhomHang: [],
            NhomHangFilter: [],
            NhanVien: [],

            LichBaoDuong: [],
            NhatKyBaoDuong: [],
            NhatKySuaChua: [],
        },
        filter: {
            txtSearch: '',
            ChiNhanhChosed: [],

            NgayBaoDuongTypeTime: 0,
            NgayBaoDuong_DefaultTypeTime: 5,
            NgayBaoDuongFrom: null,
            NgayBaoDuongTo: null,

            NgayNhacTypeTime: 0,
            NgayNhac_DefaultTypeTime: 5,
            NgayNhacFrom: null,
            NgayNhacTo: null,

            txtSeachNhomHang: '',
            ID_NhomHang: null,
            TrangThai: 1,

            CurrentPage: 0,
            PageSize: 10,
            ListPage: [],
            PageView: '',
            TotalPage: 1,
        },
        dataSMS: {
            arrIDChosed: [],
            lstCustomer: [],
            isCheckAll: false,
        }
    },
    methods: {
        PageLoad: function () {
            let self = this;
            self.InitListHeader();
            self.GetListDonVi_User();
            self.GetListNhomHang();
            self.NhacBaoDuong_GetHTCaiDat();
        },
        InitListHeader: function () {
            let self = this;
            let exist = false;
            let header = localStorage.getItem('lstHeader');
            if (header !== null) {
                header = JSON.parse(header);
                for (let i = 0; i < header.length; i++) {
                    if (header[i].Key === 'LichBaoDuong') {
                        self.ListHeader = header[i].List;
                        exist = true;
                        break;
                    }
                }
            }
            else {
                header = [];
            }
            if (!exist) {
                self.ListHeader = self.ListHeaderAll;

                let obj = {
                    Key: 'LichBaoDuong',
                    List: self.ListHeaderAll,
                };
                header.push(obj);
                localStorage.setItem('lstHeader', JSON.stringify(header));
            }
            else {
                if (self.ListHeader.length !== self.ListHeaderAll.length) {
                    // id exist cache && column is add new --> assign again cache
                    for (let i = 0; i < header.length; i++) {
                        if (header[i].Key === 'LichBaoDuong') {
                            self.ListHeader = self.ListHeaderAll;
                            header[i].List = self.ListHeaderAll;
                            break;
                        }
                    }
                    localStorage.setItem('lstHeader', JSON.stringify(header));
                }
            }
        },
        CheckColShow: function (colName) {
            let self = this;
            return self.ListHeader.find(x => x.colName === colName).colShow;
        },
        Header_ChangeCheck: function (index, item) {
            let self = this;
            let header = localStorage.getItem('lstHeader');
            if (header !== null) {
                header = JSON.parse(header);

                let arr = [];
                for (let i = 0; i < header.length; i++) {
                    if (header[i].Key === 'LichBaoDuong') {
                        for (let j = 0; i < header[i].List.length; j++) {
                            if (j === index) {
                                header[i].List[j].colShow = !item.colShow;
                                arr = header[i].List;
                                break;
                            }
                        }
                        break;
                    }
                }
                localStorage.setItem('lstHeader', JSON.stringify(header));
                self.ListHeader = $.extend([], true, arr);
            }
        },
        NhacBaoDuong_GetHTCaiDat: function () {
            let self = this;
            $.getJSON('/api/DanhMuc/HT_ThietLapAPI/' + 'GetCaiDatLichNhacBaoDuong').done(function (x) {
                if (x.res) {
                    let data = x.dataSoure;
                    for (let i = 0; i <= data.SoLanLapLai; i++) {
                        let obj = {}
                        if (i === 0) {
                            obj = {
                                Value: i,
                                Text: 'Chưa nhắc',
                            }
                        }
                        else {
                            obj = {
                                Value: i,
                                Text: 'Lần ' + i,
                            }
                        }
                        self.LanNhacs.push(obj);
                    }
                }
            });
        },
        GetListNhomHang: function () {
            let self = this;
            ajaxHelper('/api/DanhMuc/DM_NhomHangHoaAPI/' + 'GetAllDMNhomHangHoa', 'GET').done(function (data) {
                if (data !== null) {
                    self.listData.NhomHang = data.dataSoure.data;
                    self.listData.NhomHangFilter = data.dataSoure.data;
                }
            });
        },
        GetListDonVi_User: function () {
            var self = this;
            ajaxHelper('/api/DanhMuc/DM_DonViAPI/' + 'GetListDonVi_User', 'GET').done(function (data) {
                if (data !== null) {
                    data.map(function (item) {
                        if (item['ID'] === self.inforLogin.ID_DonVi) {
                            item['CNChecked'] = true;
                        }
                        else {
                            item['CNChecked'] = false;
                        }
                    });
                    self.listData.ChiNhanh = data;
                    self.GetLichBaoDuong();
                }
                else {
                    commonStatisJs.ShowMessageDanger("Có lỗi xảy ra trong quá trình tải dữ liệu. Vui lòng kiểm tra lại.");
                }
            })
        },
        GetParamSeach: function () {
            let self = this;

            let arrCN = self.listData.ChiNhanh.filter(x => x.CNChecked).map(function (x) {
                return x.ID
            });
            let arrTrangThai = self.TrangThais.filter(x => x.Checked).map(function (x) {
                return x.Value
            });
            var status = '';
            if (arrTrangThai.length > 0) {
                if (arrTrangThai.filter(x => x === 0).length > 0) {
                    arrTrangThai.push(4);
                }
                status = arrTrangThai.toString();
            }
            let cacheTongQuan = localStorage.getItem('tongquan_LichBaoDuong');
            if (cacheTongQuan !== null) {
                cacheTongQuan = JSON.parse(cacheTongQuan);
                self.filter.NgayBaoDuongFrom = moment(cacheTongQuan.NgayBaoDuongFrom, "DD/MM/YYYY").format("YYYY-MM-DD");
                self.filter.NgayBaoDuongTo = moment(cacheTongQuan.NgayBaoDuongTo, "DD/MM/YYYY").format("YYYY-MM-DD");
            }
            let txt = self.filter.txtSearch;
            if (!commonStatisJs.CheckNull(txt)) {
                txt = locdau(txt);
            }
            return {
                IDChiNhanhs: arrCN,
                TextSeach: txt,
                TrangThais: status,
                NgayBaoDuongFrom: self.filter.NgayBaoDuongFrom,
                NgayBaoDuongTo: self.filter.NgayBaoDuongTo,
                NgayNhacFrom: self.filter.NgayNhacFrom,
                NgayNhacTo: self.filter.NgayNhacTo,
                IDNhomHangs: self.filter.ID_NhomHang,
                CurrentPage: self.filter.CurrentPage,
                PageSize: self.filter.PageSize,
            };
        },
        GetLichBaoDuong: function () {
            let self = this;
            $('#tblLichBaoDuong').gridLoader();
            let param = self.GetParamSeach();

            ajaxHelper(self.urlAPI.Gara + 'GetLichBaoDuong', 'POST', param).done(function (x) {
                console.log('GetLichBaoDuong ', param, x)
                $('#tblLichBaoDuong').gridLoader({ show: false });
                if (x.res) {
                    self.listData.LichBaoDuong = x.dataSoure.data;
                    self.filter.PageView = x.dataSoure.PageView;
                    self.filter.ListPage = x.dataSoure.ListPage;
                    self.filter.TotalPage = x.dataSoure.TotalPage;

                    self.Paging_SetCheckBox();
                }
            });
            self.onRefresh = false;
        },
        Paging_SetCheckBox: function () {
            let self = this;
            let count = 0;
            for (let i = 0; i < self.listData.LichBaoDuong.length; i++) {
                let itFor = self.listData.LichBaoDuong[i];
                itFor.isCheck = $.inArray(itFor.ID, self.dataSMS.arrIDChosed) > -1;
                if (itFor.isCheck) {
                    count += 1;
                }
            }
            self.dataSMS.isCheckAll = count === self.listData.LichBaoDuong.length;
        },
        LichNhac_ResetSearch: function () {
            let self = this;
            self.filter.CurrentPage = 0;
            self.GetLichBaoDuong();
        },
        NgayBaoDuong_Change: function (value) {
            let self = this;
            if (value.fromdate !== '2016-01-01') {
                self.filter.NgayBaoDuongFrom = value.fromdate;
                self.filter.NgayBaoDuongTo = value.todate;
            }
            else {
                self.filter.NgayBaoDuongFrom = '';
                self.filter.NgayBaoDuongTo = '';
            }
            if (!self.onRefresh) {
                self.LichNhac_ResetSearch();
            }
            self.filter.NgayBaoDuongTypeTime = value.radioselect;
        },
        NgayNhac_Change: function (value) {
            let self = this;
            if (value.fromdate !== '2016-01-01') {
                self.filter.NgayNhacFrom = value.fromdate;
                self.filter.NgayNhacTo = value.todate;
            }
            else {
                self.filter.NgayNhacFrom = '';
                self.filter.NgayNhacTo = '';
            }
            if (!self.onRefresh) {
                self.LichNhac_ResetSearch();
            }
            self.filter.NgayNhacTypeTime = value.radioselect;
        },
        TrangThai_Change: function () {
            let self = this;
            self.LichNhac_ResetSearch();
        },
        ChoseNhomHang: function (value) {
            let self = this;
            self.filter.ID_NhomHang = value;
            self.LichNhac_ResetSearch();
        },
        TreeFilter: function (data, text) {
            let self = this;
            self.listData.NhomHangFilter = data.filter(function (o) {
                if (o.children) o.children = self.TreeFilter(o.children, text);
                return commonStatisJs.convertVieToEng(o.Item.Text).match(text);
            })
        },
        ChangePage: function (value) {
            let self = this;
            $('tr').removeClass('active');
            $('.op-js-tr-hide').css('display', 'none');
            let currentPage = value.currentPage - 1;
            if (self.filter.CurrentPage !== currentPage) {
                self.filter.CurrentPage = currentPage;
                self.GetLichBaoDuong();
            } else if (self.filter.PageSize !== value.pageSize) {
                self.filter.PageSize = value.pageSize;
                self.GetLichBaoDuong();
            }
        },
        RowSelected: function (item) {
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

            self.lichOld = $.extend({}, true, item);
        },
        Update_ChangeDate: function (e) {
            let self = this;
            self.lichUpdate.ngayNhacBaoDuong_update = moment(e).format('YYYY-MM-DD HH:mm');
        },
        UpdateTrangThai: function (tt, item) {
            let self = this;
            let status = parseInt(tt.Value);
            if (status === 2) {
                commonStatisJs.ShowMessageDanger('Trạng thái này chỉ được cập nhật tự động khi tạo hóa đơn');
                return;
            }
            if (status === 5 || status === 3) {
                self.InsertLichNhac(item, 1);
            }
            var param = {
                arrIDLich: [item.ID],
                status: tt.Value
            }

            ajaxHelper(self.urlAPI.Gara + 'LichNhacBaoDuong_updateStatus', 'POST', param).done(function (x) {
                if (x.res) {
                    let diary = {
                        ID_DonVi: self.inforLogin.ID_DonVi,
                        ID_NhanVien: self.inforLogin.ID_NhanVien,
                        LoaiNhatKy: 2,
                        ChucNang: 'Lịch nhắc bảo dưỡng',
                        NoiDung: 'Cập nhật trạng thái lịch nhắc bảo dưỡng, Biển số xe: ' + item.BienSo,
                        NoiDungChiTiet: 'Cập nhật trạng thái lịch nhắc bảo dưỡng, Biển số xe: '.concat(item.BienSo,
                            ', Tên hàng hóa: ', item.TenHangHoa, ' (', item.MaHangHoa, ')',
                            '<br /> - Trạng thái cũ: ', item.sTrangThai,
                            '<br /> - Trạng thái mới: ', tt.Text,
                            '<br /> - Người sửa: ', self.inforLogin.UserLogin,
                        )
                    }
                    item.TrangThai = tt.Value;
                    Insert_NhatKyThaoTac_1Param(diary);
                }
            })
        },
        ChangeSoLanNhac: function (item, itemLanNhac) {
            let self = this;
            self.lichOld = $.extend({}, true, item);
            self.lichUpdate.LanNhac = itemLanNhac.Value;
            self.UpdateLichNhac(item, 2);
        },
        InsertLichNhac: function (item, type = 0) {
            let self = this;
            let idLich = item.ID;
            // always insert lịch nhắc nếu: cập nhật lần nhắc + trạng thái (đã nhắc/quá hạn)
            $.getJSON(self.urlAPI.Gara + 'InsertLichNhacBaoDuong_whenQuaHan_orEnoughLanNhac?idLichNhac=' + idLich
                + '&typeUpdate=' + type).done(function (x) {
                    console.log('insert ', x)
                    if (x.res) {

                    }
                })
        },
        UpdateLichNhac: function (item, type = 1) {// type: 1.(ngaybaoduong + ghichu), 2.(solannhac)
            let self = this;
            let ngayBD = item.NgayBaoDuongDuKien;
            switch (type) {
                case 1:
                    if (!commonStatisJs.CheckNull(self.lichUpdate.ngayNhacBaoDuong_update)) {
                        item.NgayBaoDuongDuKien = self.lichUpdate.ngayNhacBaoDuong_update;
                    }
                    break;
                case 2:
                    item.LanNhac = self.lichUpdate.LanNhac;
                    break;
            }

            ajaxHelper(self.urlAPI.Gara + 'LichNhacBaoDuong_Update', 'POST', item).done(function (x) {
                if (x.res) {
                    let diary = {
                        ID_DonVi: self.inforLogin.ID_DonVi,
                        ID_NhanVien: self.inforLogin.ID_NhanVien,
                        LoaiNhatKy: 2,
                        ChucNang: 'Lịch nhắc bảo dưỡng',
                        NoiDung: 'Cập nhật lịch nhắc bảo dưỡng, Biển số xe:' + item.BienSo,
                        NoiDungChiTiet: 'Cập nhật lịch nhắc bảo dưỡng, Biển số xe: '.concat(item.BienSo,
                            ', Tên hàng hóa: ', item.TenHangHoa, ' (', item.MaHangHoa, ')',
                            '<br /> - Ngày nhắc dự kiến (cũ): ', moment(ngayBD).format('DD/MM/YYYY'),
                            '<br /> - Ngày nhắc dự kiến (mới): ', moment(item.NgayBaoDuongDuKien).format('DD/MM/YYYY'),
                            '<br /> - Lần nhắc (cũ): ', self.lichOld.LanNhac,
                            '<br /> - Lần nhắc (mới): ', item.LanNhac,
                            '<br /> - Người sửa: ', self.inforLogin.UserLogin,
                            '<br /> - Ghi chú: ', item.GhiChu,
                        )
                    }
                    Insert_NhatKyThaoTac_1Param(diary);
                    commonStatisJs.ShowMessageSuccess('Cập nhật lịch nhắc thành công');

                    if (type === 2) {
                        self.InsertLichNhac(item, 1);
                    }
                }
                else {
                    commonStatisJs.ShowMessageDanger('Cập nhật lịch nhắc thất bại');
                }
            })
        },
        DeleteLichNhac: function (type, item = null) {
            let self = this;
            let param = {
                arrIDLich: [],
                status: 0
            }
            let detail = '';
            if (type === 0) {
                param.arrIDLich = self.dataSMS.arrIDChosed;
                detail = '(xóa '.concat(param.arrIDLich.length, ' lịch nhắc)')
            }
            else {
                param.arrIDLich = [item.ID];
                detail = 'Biển số xe: '.concat(item.BienSo, ', Tên hàng hóa: ', item.TenHangHoa, ' (', item.MaHangHoa, ')');
                item.TrangThai = 0;
            }
            commonStatisJs.ConfirmDialog_OKCancel('Xác nhận xóa', 'Bạn có chắc chắn muốn xóa lịch nhắc không?',
                function () {
                    ajaxHelper(self.urlAPI.Gara + 'LichNhacBaoDuong_updateStatus', 'POST', param).done(function (x) {
                        if (x.res) {
                            let diary = {
                                ID_DonVi: self.inforLogin.ID_DonVi,
                                ID_NhanVien: self.inforLogin.ID_NhanVien,
                                LoaiNhatKy: 3,
                                ChucNang: 'Lịch nhắc bảo dưỡng',
                                NoiDung: 'Xóa lịch nhắc bảo dưỡng ' + detail,
                                NoiDungChiTiet: 'Xóa lịch nhắc bảo dưỡng '.concat(detail,
                                    '<br /> - Người xóa: ', self.inforLogin.UserLogin,
                                )
                            }
                            Insert_NhatKyThaoTac_1Param(diary);
                            commonStatisJs.ShowMessageSuccess('Xóa lịch nhắc thành công');

                            self.dataSMS.arrIDChosed = [];
                            self.dataSMS.lstCustomer = [];
                        }
                        else {
                            commonStatisJs.ShowMessageDanger('Xóa lịch nhắc thất bại');
                        }
                    })
                })

        },
        GetNhatKyBaoDuong_byCar: function (idCar) {
            var self = this;
            if (!commonStatisJs.CheckNull(idCar) && idCar !== const_GuidEmpty) {
                ajaxHelper(self.urlAPI.Gara + 'GetNhatKyBaoDuong_byCar?idCar=' + idCar
                    , 'GET').done(function (x) {
                        if (x.res) {
                            self.listData.NhatKyBaoDuong = x.dataSoure;
                        }
                    });
            }
        },
        ClickTab: function (item) {
            let self = this;
            var $this = $(event.currentTarget);
            $this.parent().children('a').removeClass('active');
            $this.addClass('active');

            self.GetNhatKyBaoDuong_byCar(item.ID_Xe);
        },
        ExportExcel: function () {
            let self = this;
            if (self.listData.LichBaoDuong.length > 0) {
                var colHide = [];
                for (let i = 0; i < self.ListHeader.length; i++) {
                    let itFor = self.ListHeader[i];
                    if (!itFor.colShow) {
                        colHide.push(i);
                    }
                }
                $('.content-table').gridLoader();
                let param = self.GetParamSeach();
                param.ColumnsHide = colHide;
                param.CurrentPage = 0;
                param.PageSize = self.listData.LichBaoDuong[0].TotalRow;

                ajaxHelper(self.urlAPI.Gara + 'ExportExcel_LichBaoDuong', 'POST', param).done(function (x) {
                    $('.content-table').gridLoader({ show: false });
                    if (x.res) {
                        self.DownloadFileTeamplateXLSX(x.mess);
                    }
                })
            }
        },
        DownloadFileTeamplateXLSX: function (pathFile) {
            var url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + pathFile;
            window.location.href = url;
        },
        RemoveCheck: function () {
            let self = this;
            self.dataSMS.isCheckAll = false;
            self.dataSMS.arrIDChosed = [];
            self.dataSMS.lstCustomer = [];
            self.listData.LichBaoDuong.map(function (x) {
                x['isCheck'] = false;
            })
        },
        CheckBox_Chose: function (type, item) {
            let self = this;
            let $this = $(event.currentTarget);
            let isCheck = $this.is(':checked');

            var arrChosed = [];
            switch (type) {
                case 0:// check all
                    arrChosed = self.listData.LichBaoDuong.map(function (x) {
                        return { ID: x.ID, ID_DoiTuong: x.ID_DoiTuong }
                    });
                    self.listData.LichBaoDuong.map(function (x) {
                        x['isCheck'] = isCheck;
                    })
                    break;
                case 1:// check one
                    arrChosed = [item];
                    if (!isCheck) {
                        self.dataSMS.isCheckAll = false;
                    }
                    break;
            }
            if (isCheck) {
                for (let i = 0; i < arrChosed.length; i++) {
                    let itFor = arrChosed[i];
                    if ($.inArray(itFor.ID, self.dataSMS.arrIDChosed) === -1) {
                        self.dataSMS.arrIDChosed.push(itFor.ID);
                        self.dataSMS.lstCustomer.push(itFor.ID_DoiTuong);
                    }
                }
            }
            else {
                let arrID = arrChosed.map(function (x) { return x.ID });
                let arrIDCus = arrChosed.map(function (x) { return x.ID_DoiTuong });
                let arr = $.grep(self.dataSMS.arrIDChosed, function (x) {
                    return $.inArray(x, arrID) === -1;
                });
                let arrCus = $.grep(self.dataSMS.lstCustomer, function (x) {
                    return $.inArray(x, arrIDCus) === -1;
                });
                self.dataSMS.arrIDChosed = arr;
                self.dataSMS.lstCustomer = arrCus;
            }
        },
        showModalSMS: function () {
            let self = this;
            let from = self.filter.NgayBaoDuongFrom;
            let to = self.filter.NgayBaoDuongTo;
            vmSMS.filter.FromDate = from;
            vmSMS.filter.ToDate = to;
            vmSMS.showModal(5, self.dataSMS.lstCustomer);
        },
        hideModalSMS: function (isSendEmail = false) {
            let self = this;
            let loaitin = 1;
            if (isSendEmail) {
                loaitin = vmEmail.ID_LoaiTin;
            }
            else {
                loaitin = vmSMS.newSMS.ID_LoaiTin;
            }
            if (loaitin === 5) {
                let param = {
                    arrIDLich: self.dataSMS.arrIDChosed,
                }
                console.log('param ', param)
                // update solannhac theo id_lich
                ajaxHelper(self.urlAPI.Gara + 'LichNhacBaoDuong_updateSoLanNhac', 'POST', param).done(function (x) {
                })
            }
            self.dataSMS.lstCustomer = [];
            self.dataSMS.arrIDChosed = [];
            self.dataSMS.isCheckAll = false;
            self.listData.LichBaoDuong.map(function (x) {
                x['isCheck'] = false;
            })
        },
        showModalEmail: function () {
            let self = this;
            let from = self.filter.NgayBaoDuongFrom;
            let to = self.filter.NgayBaoDuongTo;
            vmEmail.filter.FromDate = from;
            vmEmail.filter.ToDate = to;
            vmEmail.showModal(5, self.dataSMS.lstCustomer);
        },
    }
})

$('#vmSendSMS').on('hidden.bs.modal', function () {
    if (vmSMS.saveOK) {
        vmLichBaoDuong.hideModalSMS(false);
    }
})
$('#vmSendEmail').on('hidden.bs.modal', function () {
    if (vmEmail.saveOK) {
        vmLichBaoDuong.hideModalSMS(true);
    }
})