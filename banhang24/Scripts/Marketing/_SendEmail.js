var vmEmail = new Vue({
    el: '#vmSendEmail',
    components: {
        'dropdown': cmpDropdown1Item,
        'dropdown-multiple': cmpDropdownMultipleItem,
        'date-range': cmpDateRange,
    },
    created: function () {
        let self = this;
        self.PageLoad();
    },
    data: {
        urlAPI: {
            ThietLap: '/api/DanhMuc/ThietLapApi/',
            DoiTuong: '/api/DanhMuc/DM_DoiTuongAPI/',
        },
        inforLogin: {
            ID_DonVi: VHeader.IdDonVi,
            UserLogin: VHeader.UserLogin,
            ID_User: VHeader.IdNguoiDung,
            ID_NhanVien: VHeader.IdNhanVien,
        },
        saveOK: false,
        listIDCustomer: [],// get at parent
        filter: {
            FromDate: moment(new Date()).format('YYYY-MM-DD'),
            ToDate: moment(new Date()).format('YYYY-MM-DD'),
            txtDate: '',
            txtSearchCus: '',
            txtSearchDll: '',
        },
        listData: {
            Customer: [],
            listCusChosed: [],
        },
        newEmail: {
            ID_NguoiGui: null,
            ID_KhachHang: null,
            ID_DonVi: VHeader.IdDonVi,
            ID_LoaiTin: null,
            TieuDe: null,
            NoiDung: '',

            txtLoaiTin: '',
        },

    },
    methods: {
        PageLoad: function () {
            let self = this;
            self.GetListLoaiTinSMS();
        },
        GetListLoaiTinSMS: function () {
            let self = this;
            ajaxHelper(self.urlAPI.ThietLap + 'GetListLoaiTinSMS', 'GET').done(function (data) {
                self.listData.LoaiTin = data.filter(x => x.ID !== 3).map(function (x) {
                    return {
                        ID: x.ID,
                        Text1: x.Name,
                        Text2: x.Name
                    }
                })
            })
        },
        // type (LoaiTin) 1.giaodich, 2.sinhnhat, 3.tinthuong, 4.congviec, 5.baoduong
        showModal: function (type = 1, lstIDCus = []) {
            let self = this;
            self.saveOK = false;
            self.listIDCustomer = $.unique(lstIDCus.filter(x => x !== null));
            self.listData.listCusChosed = [];

            self.newEmail.ID_LoaiTin = type;
            self.newEmail.NoiDung = '';

            let from = moment(self.filter.FromDate, 'YYYY-MM-DD').format('DD/MM/YYYY');
            let to = moment(self.filter.ToDate, 'YYYY-MM-DD').format('DD/MM/YYYY');
            self.filter.txtDate = from + ' - ' + to;

            self.ChoseLoaiTin(self.listData.LoaiTin.find(x => x.ID === self.newEmail.ID_LoaiTin));
            self.GetKhachHang_enoughCondition();
            $('#vmSendEmail').modal('show');
        },
        ChoseLoaiTin: function (item) {
            let self = this;
            self.newEmail.ID_LoaiTin = item.ID;
            self.newEmail.txtLoaiTin = item.Text2;
            self.GetKhachHang_enoughCondition();
        },
        ChoseKhachHang: function (lst) {
            let self = this;
            self.listData.listCusChosed = lst;
        },
        ChangeDateRange: function (picker) {
            let self = this;
            var from = moment(picker.startDate).format('YYYY-MM-DD');
            var to = moment(picker.endDate).format('YYYY-MM-DD');
            self.filter.FromDate = from;
            self.filter.ToDate = to;
            self.GetKhachHang_enoughCondition();
        },
        GetKhachHang_enoughCondition: function () {
            let self = this;
            let param = {};
            switch (parseInt(self.newEmail.ID_LoaiTin)) {
                case 1:// giaodich
                    param = {
                        IDChiNhanhs: [self.inforLogin.ID_DonVi],
                        DateFrom: self.filter.FromDate,
                        DateTo: self.filter.ToDate,
                    }
                    ajaxHelper(self.urlAPI.DoiTuong + 'GetCustomer_haveTransaction', 'POST', param).done(function (x) {
                        if (x.res) {
                            let arr = x.dataSoure.filter(o => o.SoDienThoai !== null && o.SoDienThoai !== '');
                            arr = arr.map(function (o) {
                                return {
                                    ID: o.ID,
                                    Text1: o.Email,
                                    Text2: o.NguoiNopTien,
                                    Text3: o.MaNguoiNop,
                                }
                            })
                            self.listData.Customer = arr;
                            if (self.listIDCustomer.length > 0) {
                                self.listData.listCusChosed = $.grep(arr, function (x) {
                                    return $.inArray(x.ID, self.listIDCustomer) > -1;
                                });
                            }
                        }
                    })
                    break;
                case 2:// sinhnhat
                    param = {
                        IDChiNhanhs: [self.inforLogin.ID_DonVi],
                        DateFrom: self.filter.FromDate,
                        DateTo: self.filter.ToDate,
                    }
                    ajaxHelper(self.urlAPI.DoiTuong + 'GetCustomer_haveBirthday', 'POST', param).done(function (x) {
                        if (x.res) {
                            let arr = x.dataSoure.filter(o => o.SoDienThoai !== null && o.SoDienThoai !== '');
                            arr = arr.map(function (o) {
                                return {
                                    ID: o.ID,
                                    Text1: o.Email,
                                    Text2: o.NguoiNopTien,
                                    Text3: o.MaNguoiNop,
                                }
                            })

                            self.listData.Customer = arr;
                            if (self.listIDCustomer.length > 0) {
                                self.listData.listCusChosed = $.grep(arr, function (x) {
                                    return $.inArray(x.ID, self.listIDCustomer) > -1;
                                });
                            }
                        }
                    })
                    break;
                case 3:// tin thuong
                    break;
                case 4://lichhen todo - nên viết lại store
                    url = 'SMS_LichHen?status=1';
                    break;
                case 5:// baoduong
                    urlAPI = '';
                    param = {
                        IDChiNhanhs: [self.inforLogin.ID_DonVi],
                        NgayBaoDuongFrom: self.filter.FromDate,
                        NgayBaoDuongTo: self.filter.ToDate,
                    }
                    ajaxHelper('/api/DanhMuc/GaraAPI/GetKhachCoLichBaoDuong', 'POST', param).done(function (x) {
                        if (x.res) {
                            let arr = x.dataSoure.filter(o => o.ID_DoiTuong !== null);
                            arr = arr.map(function (o) {
                                return {
                                    ID: o.ID_DoiTuong,
                                    Text1: o.Email,
                                    Text2: o.TenDoiTuong,
                                    Text3: o.MaDoiTuong,
                                }
                            })
                            arr= arr.filter(o => o.Text1 !== '');

                            self.listData.Customer = arr;
                            if (self.listIDCustomer.length > 0) {
                                self.listData.listCusChosed = $.grep(arr, function (x) {
                                    return $.inArray(x.ID, self.listIDCustomer) > -1;
                                });
                            }
                        }
                    })
                    break;
            }
        },
        GuiEmail: function () {
            let self = this;
            if (commonStatisJs.CheckNull(self.newEmail.TieuDe)) {
                commonStatisJs.ShowMessageDanger('Vui lòng nhập tiêu đề email');
                return;
            }
            var lstCus = self.listData.listCusChosed;
            if (lstCus.length === 0) {
                commonStatisJs.ShowMessageDanger("Vui lòng chọn khách hàng cần gửi");
                return;
            }
            if (lstCus[0].ID === null) {
                lstCus = self.listData.Customer;
            }
            if (commonStatisJs.CheckNull(self.newEmail.NoiDung)) {
                commonStatisJs.ShowMessageDanger("Vui lòng nhập nội dung email");
                return;
            }
            $('#vmSendEmail').gridLoader();
            var arr = [];
            for (let j = 0; j < lstCus.length; j++) {
                let itFor = lstCus[j];
                let obj = {
                    ID_NguoiGui: self.inforLogin.ID_User,
                    ID_DonVi: self.inforLogin.ID_DonVi,
                    ID_KhachHang: itFor.ID,
                    LoaiTinNhan: self.newEmail.ID_LoaiTin,
                    TieuDe: self.newEmail.TieuDe,
                    NoiDung: self.newEmail.NoiDung,
                    Email: "inftech.ts91@gmail.com",
                    Password: 'nhuong_inftech',
                    TenDoiTuong: itFor.Text2,
                };
                arr.push(obj);
            }
            var myData = {
                lst: arr,
            }
            console.log('myData', myData)
            //ajaxHelper('/api/DanhMuc/ChamSocKhachHangAPI/' + 'SendEmail', 'POST', myData).done(function (x) {
                //if (x.res) {

                //}
                console.log(x)
                self.saveOK = true;
                $('#vmSendEmail').gridLoader({ show: false });
                commonStatisJs.ShowMessageSuccess('Gửi email thành công');
                $('#vmSendEmail').modal('hide');


                let diary = {
                    ID_DonVi: self.inforLogin.ID_DonVi,
                    ID_NhanVien: self.inforLogin.ID_NhanVien,
                    LoaiNhatKy: 1,
                    ChucNang: 'Gửi email',
                    NoiDung: 'Gửi email tới ' + lstCus.length + ' khách hàng',
                    NoiDungChiTiet: 'Gửi tin email tới các khách hàng gồm: '.concat(lstCus.map(function (k) { return k.Text2 }).toString(),
                        '<br /> - Nội dung tin: ', self.newEmail.NoiDung,
                        '<br /> - Người gửi: ', self.inforLogin.UserLogin,
                    )
                }
                Insert_NhatKyThaoTac_1Param(diary);
            //});
        },
    },
    computed: {
        txtSendTo: function () {
            let self = this;
            if (commonStatisJs.CheckNull(self.newEmail.ID_LoaiTin)) {
                return '';
            }
            return self.listData.LoaiTin.find(x => x.ID === self.newEmail.ID_LoaiTin).Text2;
        }
    }
})