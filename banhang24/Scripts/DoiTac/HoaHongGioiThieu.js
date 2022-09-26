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
        inforOld: {},
        ListHeader: [],
        ngayLapHoaDon_update: null,

        HoaDonChiTiet: [],
        LichSuThanhToan: [],

        HoaDon: {
            data: [],
            SumTongTienHang: 0,
            SumKhachDaTra: 0,
            SumConNo: 0,
            CurrentPage: 1,
            PageSize: 10,
            ListPage: [],
            PageView: '',
            NumberOfPage: 10,
        },

        filter: {
            TextSearch: '',
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
                { Text: 'Nhân viên', Value: 5, Checked: true },
                { Text: 'Nhà cung cấp', Value: 2, Checked: true },
                { Text: 'Khác', Value: 4, Checked: true }]
        },
    },
    methods: {
        PageLoad: function () {
            let self = this;
            self.InitHeader();
            self.GetList_PhieuTrichHoaHong();
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
                IDCustomers: [],// muontamtruong (list IDNguoiGioiThieu)
                CurrentPage: self.HoaDon.CurrentPage - 1,
                PageSize: self.HoaDon.PageSize,
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
                    self.HoaDon.TotalRow = x.dataSoure.TotalRow;
                    self.HoaDon.PageView = x.dataSoure.PageView;
                    self.HoaDon.NumberOfPage = x.dataSoure.NumOfPage;
                    self.HoaDon.ListPage = x.dataSoure.ListPage;
                }
                else {
                    self.HoaDon.data = [];
                    self.HoaDon.SumTongTienHang = 0;
                    self.HoaDon.SumKhachDaTra = 0;
                    self.HoaDon.SumConNo = 0;
                    self.HoaDon.TotalRow = 0;
                    self.HoaDon.PageView = 0;
                    self.HoaDon.NumberOfPage = 0;
                    self.HoaDon.ListPage = 0;
                }
            }).always(function () {
                $('#tb').gridLoader({ show: false })
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
            param.PageSize = self.HoaDon.TotalRow;
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