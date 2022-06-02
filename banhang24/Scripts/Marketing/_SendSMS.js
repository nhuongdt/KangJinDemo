var vmSMS = new Vue({
    el: '#vmSendSMS',
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
            BrandName: [],
            LoaiTin: [],
            MauTin: [],
            AllMauTin: [],
            LoaiTin: [],
            Customer: [],
            listCusChosed: [],
        },
        newSMS: {
            ID_NguoiGui: null,
            ID_KhachHang: null,
            ID_DonVi: VHeader.IdDonVi,
            SoDienThoai: null,
            SoTinGui: 0,
            NoiDung: '',
            ID_HoaDon: null,
            SoDuTaiKhoan: 0,
            GiaTienMotTrangTin: 0,
            MaxChar: 0,
            CountChar: 0,

            ID_BrandName: null,
            ID_LoaiTin: null,
            ID_MauTin: null,
            txtBrandName: '',
            txtLoaiTin: '',
            txtMauTin: '',
        },

    },
    methods: {
        PageLoad: function () {
            let self = this;
            self.GetListBrandName();
            self.GetListMauTin();
            self.GetListLoaiTinSMS();
            self.GetSoDuTaiKhoan();
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
        GetListBrandName: function () {
            let self = this;
            ajaxHelper(self.urlAPI.ThietLap + 'GetallBrandName', 'GET').done(function (data) {
                data = data.filter(p => p.Status === 1);
                var arr = [];
                if (data.length > 0) {
                    arr = data.map(function (x) {
                        return {
                            ID: x.ID,
                            Text1: '',
                            Text2: x.BrandName,
                        }
                    })
                }
                self.listData.BrandName = arr;

                if (arr.length === 1) {
                    self.newSMS.ID_BrandName = arr[0].ID;
                    self.newSMS.txtBrandName = arr[0].Text2;
                    self.GetGiaTienTinNhan(arr[0]);
                }
            });
        },
        GetListMauTin: function () {
            let self = this;
            ajaxHelper(self.urlAPI.ThietLap + 'GetAllMauTin', 'GET').done(function (data) {
                self.listData.AllMauTin = data;
            });
        },
        GetGiaTienTinNhan: function (item) {
            let self = this;
            ajaxHelper(self.urlAPI.ThietLap + 'GetGiaTienTrenTinNhan?id_brand=' + item.ID, "GET").done(function (data) {
                self.newSMS.GiaTienMotTrangTin = data;
            })
        },

        GetSoDuTaiKhoan: function () {
            let self = this;
            ajaxHelper(self.urlAPI.ThietLap + 'GetSoDuCuaTaiKhoan?idnd=' + self.inforLogin.ID_User, "GET").done(function (data) {
                self.newSMS.SoDuTaiKhoan = data;
            })
        },
        // type (LoaiTin) 1.giaodich, 2.sinhnhat, 3.tinthuong, 4.congviec, 5.baoduong
        showModal: function (type = 1, lstIDCus = []) {
            let self = this;
            self.saveOK = false;
            self.listIDCustomer = $.unique(lstIDCus.filter(x => x !== null));
            self.listData.listCusChosed = [];

            self.newSMS.ID_LoaiTin = type;
            self.newSMS.NoiDung = '';
            self.newSMS.ID_MauTin = null;
            self.newSMS.txtMauTin = '';

            let from = moment(self.filter.FromDate, 'YYYY-MM-DD').format('DD/MM/YYYY');
            let to = moment(self.filter.ToDate, 'YYYY-MM-DD').format('DD/MM/YYYY');
            self.filter.txtDate = from + ' - ' + to;

            self.ChoseLoaiTin(self.listData.LoaiTin.find(x => x.ID === self.newSMS.ID_LoaiTin));
            self.GetKhachHang_enoughCondition();
            $('#vmSendSMS').modal('show');
        },
        ChoseBrandName: function (item) {
            let self = this;
            self.newSMS.ID_BrandName = item.ID;
            self.newSMS.txtBrandName = item.Text2;
        },
        ChoseLoaiTin: function (item) {
            let self = this;
            self.newSMS.ID_LoaiTin = item.ID;
            self.newSMS.txtLoaiTin = item.Text2;
            self.ChoseMauTin(null);

            let arr = self.listData.AllMauTin.filter(x => x.LoaiTin === item.ID);
            arr = arr.map(function (x) {
                return {
                    ID: x.ID,
                    Text1: '',
                    Text2: x.NoiDungTin,
                }
            })
            self.listData.MauTin = arr;
            self.GetKhachHang_enoughCondition();
        },
        ChoseMauTin: function (item) {
            let self = this;
            if (item === null) {
                self.newSMS.ID_MauTin = null;
                self.newSMS.txtMauTin = '';
                self.newSMS.NoiDung = '';
            }
            else {
                self.newSMS.ID_MauTin = item.ID;
                self.newSMS.txtMauTin = item.Text2;
                self.newSMS.NoiDung = item.Text2;
            }
            self.countTinNhan();
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
            switch (parseInt(self.newSMS.ID_LoaiTin)) {
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
                                    Text1: o.SoDienThoai,
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
                                    Text1: o.SoDienThoai,
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
                                    Text1: o.DienThoai,
                                    Text2: o.TenDoiTuong,
                                    Text3: o.MaDoiTuong,
                                }
                            })
                            arr = arr.filter(o => o.Text1 !== '');
                            console.log('data ', arr)

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
        countTinNhan: function () {
            let self = this;
            let len = self.newSMS.NoiDung.length;
            self.newSMS.SoTinGui = Math.ceil(len / 140);
            self.newSMS.MaxChar = self.newSMS.SoTinGui * 140;
            self.newSMS.CountChar = len;
        },
        GuiTinNhan: function () {
            let self = this;
            //if (commonStatisJs.CheckNull(self.newSMS.ID_BrandName)) {
            //    commonStatisJs.ShowMessageDanger('Vui lòng chọn brand name');
            //    return;
            //}
            var lstCus = self.listData.listCusChosed;
            if (lstCus.length === 0) {
                commonStatisJs.ShowMessageDanger("Vui lòng chọn khách hàng cần gửi tin");
                return;
            }
            if (lstCus[0].ID === null) {
                lstCus = self.listData.Customer;
            }
            if (commonStatisJs.CheckNull(self.newSMS.NoiDung)) {
                commonStatisJs.ShowMessageDanger("Vui lòng nhập nội dung tin nhắn");
                return;
            }
            //if (self.newSMS.GiaTienMotTrangTin * self.newSMS.SoTinNhan * lstCus.length > self.newSMS.SoDuTaiKhoan) {
            //    commonStatisJs.ShowMessageDanger("Số dư không đủ để gửi tin. Vui lòng nạp thêm tiền");
            //    return;
            //}
            $('#vmSendSMS').gridLoader();
            for (let j = 0; j < lstCus.length; j++) {
                let itFor = lstCus[j];
                let myData = {};
                let objTin = {
                    SoDienThoai: itFor.Text1,
                    NoiDung: self.newSMS.NoiDung,
                    SoTinGui: self.newSMS.SoTinGui,
                    LoaiTinNhan: self.newSMS.ID_LoaiTin,
                    ID_NguoiGui: self.inforLogin.ID_User,
                    ID_DonVi: self.inforLogin.ID_DonVi,
                    ID_KhachHang: itFor.ID
                };
                myData.objTinNhan = objTin;
                //myData.ID_BrandName = _idbrand;
                console.log('myData ', j, myData)
                //ajaxHelper(self.urlAPI.ThietLap + 'PostTinNhan', 'POST', myData).done(function () {
                //  
                //}).fail(function (err) {

                //});
                ajaxHelper(self.urlAPI.ThietLap + 'GetAllMauTin', 'GET').done(function (data) {
                    if (j === lstCus.length - 1) {
                        self.saveOK = true;
                        $('#vmSendSMS').gridLoader({ show: false });
                        commonStatisJs.ShowMessageSuccess('Gửi tin nhắn thành công');
                        $('#vmSendSMS').modal('hide');
                        let diary = {
                            ID_DonVi: self.inforLogin.ID_DonVi,
                            ID_NhanVien: self.inforLogin.ID_NhanVien,
                            LoaiNhatKy: 1,
                            ChucNang: 'Gửi SMS',
                            NoiDung: 'Gửi tin nhắn cho ' + lstCus.length + ' khách hàng',
                            NoiDungChiTiet: 'Gửi tin nhắn cho các khách hàng gồm: '.concat(lstCus.map(function (k) { return k.Text2 }).toString(),
                                '<br /> - Nội dung tin: ', self.newEmail.NoiDung,
                                '<br /> - Người gửi: ', self.inforLogin.UserLogin,
                            )
                        }
                        Insert_NhatKyThaoTac_1Param(diary);
                    }
                });
            }
        },
    },
    computed: {
        txtSendTo: function () {
            let self = this;
            if (commonStatisJs.CheckNull(self.newSMS.ID_LoaiTin)) {
                return '';
            }
            return self.listData.LoaiTin.find(x => x.ID === self.newSMS.ID_LoaiTin).Text2;
        }
    }
})