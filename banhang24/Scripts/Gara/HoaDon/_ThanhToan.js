var vmThanhToan = new Vue({
    el: '#ThuTienHoaDonModal',
    components: {
        'account-bank': cmpChoseAccountBank,
        'my-date-time': cpmDatetime,
        'nhanviens': ComponentChoseStaff,
        'khoan-thu-chi': cmpChoseKhoanThu,
        'customers': cmpChoseCustomer,
    },
    created: function () {
        let self = this;
        self.isKhoaSo = false;
        let idDonVi = $('#txtDonVi').val();
        if (commonStatisJs.CheckNull(idDonVi)) {
            self.role.PhieuThu.Insert = VHeader.Quyen.indexOf('SoQuy_ThemMoi') > -1;
            self.role.PhieuThu.Update = VHeader.Quyen.indexOf('SoQuy_CapNhat') > -1;
            self.role.PhieuThu.Delete = VHeader.Quyen.indexOf('SoQuy_Xoa') > -1;
            self.role.PhieuThu.ChangeNgayLap = VHeader.Quyen.indexOf('SoQuy_ThayDoiThoiGian') > -1;

            self.inforLogin = {
                ID_NhanVien: VHeader.IdNhanVien,
                ID_User: VHeader.IdNguoiDung,
                UserLogin: VHeader.UserLogin,
                ID_DonVi: VHeader.IdDonVi,
                TenNhanVien: VHeader.TenNhanVien,
            };

            self.ThietLapCuaHang = VHeader.ThietLapCuaHang;
            self.ThietLapChotSo = VHeader.ThietLapChotSo;
        }
        else {
            self.inforLogin.ID_DonVi = idDonVi;
        }

        self.ThietLap_TichDiem = {
            DuocThietLap: false,
            DiemThanhToan: 0,
            TienThanhToan: 0,
            TyLeDoiDiem: 0,
            TichDiemGiamGia: false,
            TichDiemHoaDonGiamGia: false,
        }
        self.inforCus = {
            TongTichDiem: 0,
            DienThoai: 0,
            DiaChi: 0,
        };

        self.GetHT_TichDiem();
        console.log('thu tien HD')
    },
    computed: {
        sLoai: function () {
            return this.newPhieuThu.LoaiHoaDon === 11 ? 'thu' : 'chi';
        },
        sForm: function () {
            let self = this;
            let sText = '';
            switch (self.formType) {
                case 0:
                    sText = 'Danh mục hóa đơn';
                    break;
                case 1:
                    sText = 'Danh sách khách hàng';
                    break;
                case 3:
                    sText = 'Danh sách bảo hiểm';
                    break;
            }
            return sText;
        }
    },
    data: {
        saveOK: false,
        isLoading: false,
        typeUpdate: 0,//0.insert, 1.update
        formType: 0,// 0.DS hoadon, 1.DS KhachHang, 2.modal LapPhieuThu (at banhang), 3. DS baohiem
        SoDuDatCoc: 0,
        KhongBuTruCongNo: false,
        isThuTienThua: false,
        showCheckHachToan: false,
        isKhoaSo: false,
        NgayChotSo: null,

        ThietLapCuaHang: [],
        ThietLapChotSo: [],

        ThietLap_TichDiem: {
            DuocThietLap: false,
            DiemThanhToan: 0,
            TienThanhToan: 0,
            TyLeDoiDiem: 0,
            TichDiemGiamGia: false,
            TichDiemHoaDonGiamGia: false,
        },

        inforCus: {
            TongTichDiem: 0,
            DienThoai: 0,
            DiaChi: 0,
        },

        ddl_textVal: {
            staffName: '',
            accountPOSName: '',
            accountCKName: '',
            cusName: '',
            cusCode: '',
            cusPhone: '',
            khoanthu: '',
        },
        role: {
            PhieuThu: { Insert: true, Update: true, Delete: false, ChangeNgayLap: false }
        },
        inforLogin: {
            ID_NhanVien: null,
            ID_User: null,
            UserLogin: null,
            ID_DonVi: null,
            TenNhanVien: '',
        },
        inforCongTy: {
            TenCongTy: '',
            DiaChiCuaHang: '',
            LogoCuaHang: ''
        },
        phieuThuOld: {},
        newPhieuThu: {
            MaHoaDon: '',
            LoaiHoaDon: 11,
            NgayLapHoaDon: moment(new Date()).format('YYYY-MM-DD HH:mm'),
            TienDatCoc: 0,
            TienMat: 0,
            TienPOS: 0,
            TienCK: 0,
            TienTheGiaTri: 0,
            TTBangDiem: 0,
            DiemQuyDoi: 0,
            ID_TaiKhoanPos: null,
            ID_TaiKhoanChuyenKhoan: null,
            ID_NhanVien: null,
            ID_KhoanThuChi: null,
            ID_DoiTuong: null,// nguoinop
            NoHienTai: 0,// nohientai of khachhang (used to show at form thanhtoan, not use tinhtoan)
            TongNoHD: 0,// tongnoHD
            DaThanhToan: 0,
            TienThua: 0,
            ThucThu: 0,
            NoiDungThu: '',
            TrangThai: true,

            HachToanKinhDoanh: true,
            PhieuDieuChinhCongNo: 0,

            TongPhiThanhToan: 0,
            PhiThanhToan_PTGiaTri: 0,
            PhiThanhToan_LaPhanTram: true,
            PhiThanhToan_PTGiaTriTheoHoaDon: 0,// nếu chi phí Thanh toán = VND -> tính và chia % theo hóa đơn (TongChiPhi/TongPOS)

            ListTKPos: [],
            ListTKChuyenKhoan: [],
        },
        listData: {
            AccountBanks: [],
            NhanViens: [],
            KhoanThuChis: [],
            NguoiNops: [],// khachhang/baohiem
            HoaDons: [],
            ChiNhanhs: [],// used to get infor chinhanh when  print
            arrIDDonVi: [], // used at form KhachHang: get hdDebit by list chinhanh
        },
        QuyHD_Share: [],
        HoaDonChosing: {},
        theGiaTriCus: {
            TongNapThe: 0,
            SuDungThe: 0,
            HoanTraTheGiaTri: 0,
            SoDuTheGiaTri: 0,
            CongNoThe: 0,
        },
        HinhThucTT: { ID: 0, Text: 'Tất cả' },
        Pos_indexChosing: 0,
        CK_indexChosing: 0,
        LoaiHoaDon: 0,
    },
    methods: {
        Async_GetInforTheGiaTri: async function (idDoiTuong) {
            let obj = {
                SoDuTheGiaTri: 0,
                CongNoThe: 0,
                TongNapThe: 0,
                SuDungThe: 0,
            }
            if (!commonStatisJs.CheckNull(idDoiTuong) && idDoiTuong !== const_GuidEmpty) {
                let date = moment(new Date()).format('YYYY-MM-DD HH:mm:ss');
                let objXX = await ajaxHelper('/api/DanhMuc/DM_DoiTuongAPI/' + 'Get_SoDuTheGiaTri_ofKhachHang?idDoiTuong=' + idDoiTuong
                    + '&datetime=' + date, 'GET').done(function () {
                    }).then(function (data) {
                        if (data != null && data.length > 0) {
                            let soduX = data[0].SoDuTheGiaTri;
                            soduX = soduX > 0 ? soduX : 0;
                            obj = {
                                SoDuTheGiaTri: soduX,
                                CongNoThe: data[0].CongNoThe,
                                TongNapThe: data[0].TongThuTheGiaTri,
                                SuDungThe: data[0].SuDungThe,
                            }
                        }
                        return obj;
                    });
                return objXX;
            }
            return obj;
        },

        newPhuongThucTT: function (tkPos = true) {
            if (tkPos) {
                return {
                    TienPOS: 0,
                    ID_TaiKhoanPos: null,
                    TenTaiKhoanPos: '',
                    SoTaiKhoanPos: '',
                    TenNganHangPos: '',
                    ChiPhiThanhToan: 0,
                    TheoPhanTram: false,
                }
            }
            else {
                return {
                    TienCK: 0,
                    ID_TaiKhoanChuyenKhoan: null,
                    TenTaiKhoanCK: '',
                    SoTaiKhoanCK: '',
                    TenNganHangCK: '',
                    ChiPhiThanhToan: 0,
                    TheoPhanTram: false,
                }
            }
        },

        GetInforBasic_Cus: function () {
            let self = this;
            if (!commonStatisJs.CheckNull(self.newPhieuThu.ID_DoiTuong) && self.newPhieuThu.ID_DoiTuong !== const_GuidEmpty) {
                let obj = {
                    ID_NhanVienQuanLys: [self.newPhieuThu.ID_DoiTuong],
                    CurrentPage: 0,
                    PageSize: 0,
                    TrangThai: 0,
                }
                ajaxHelper('/api/DanhMuc/DM_DoiTuongAPI/' + 'GetListCustomer_byIDs', 'POST', obj).done(function (x) {
                    if (x.res && x.data.length > 0) {
                        self.inforCus = x.data[0];
                    }
                    else {
                        self.inforCus = {
                            TongTichDiem: 0,
                            DienThoai: 0,
                            DiaChi: 0,
                        };
                    }
                })
            }
            else {
                self.inforCus = {
                    TongTichDiem: 0,
                    DienThoai: 0,
                    DiaChi: 0,
                };
            }
        },
        GetHT_TichDiem: function () {
            let self = this;
            if (self.formType !== 2) {// if not gara/banle
                ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CauHinh_TichDiemChiTiet?idDonVi=' + self.inforLogin.ID_DonVi, 'GET').done(function (obj) {
                    if (obj.res === true) {
                        let data = obj.data;
                        if (data.length > 0) {
                            self.ThietLap_TichDiem = {
                                DuocThietLap: true,
                                DiemThanhToan: data[0].DiemThanhToan,
                                TienThanhToan: data[0].TienThanhToan,
                                TyLeDoiDiem: data[0].TyLeDoiDiem,
                                TichDiemGiamGia: data[0].TichDiemGiamGia,
                                TichDiemHoaDonGiamGia: data[0].TichDiemHoaDonGiamGia,
                            }
                        }
                    }
                });
            }
        },
        GetSoDuTheGiaTri: function (idDoiTuong) {
            var self = this;
            let datetime = moment(new Date()).add('days', 1).format('YYYY-MM-DD');
            $.getJSON("/api/DanhMuc/DM_DoiTuongAPI/Get_SoDuTheGiaTri_ofKhachHang?idDoiTuong=" + idDoiTuong + '&datetime=' + datetime, function (data) {
                if (data !== null && data.length > 0) {
                    let sodu = data[0].SoDuTheGiaTri;
                    sodu = sodu < 0 ? 0 : sodu;
                    self.HoaDonChosing.SoDuTheGiaTri = sodu;

                    self.theGiaTriCus.SoDuTheGiaTri = sodu;
                    self.theGiaTriCus.CongNoThe = data[0].CongNoThe;
                    self.theGiaTriCus.TongNapThe = data[0].TongThuTheGiaTri;
                    self.theGiaTriCus.SuDungThe = data[0].SuDungThe;
                }
                else {
                    self.HoaDonChosing.SoDuTheGiaTri = 0;
                    self.theGiaTriCus.SoDuTheGiaTri = 0;
                    self.theGiaTriCus.CongNoThe = 0;
                    self.theGiaTriCus.TongNapThe = 0;
                    self.theGiaTriCus.SuDungThe = 0;
                }
            });
        },
        GetDebitCustomer_allBrands: function (idDoiTuong) {
            var self = this;
            if (navigator.onLine) {
                ajaxHelper('/api/DanhMuc/DM_DoiTuongAPI/' + 'GetDebitCustomer_allBrands?idDoiTuong='
                    + idDoiTuong, 'GET').done(function (nohientai) {
                        self.newPhieuThu.NoHienTai = nohientai;
                        self.HoaDonChosing.PhaiThanhToan = nohientai;
                        self.HoaDonChosing.DaThanhToan = nohientai;

                        self.GetListHD_isDebitOfKH(idDoiTuong);
                    });
            }
        },
        GetListHD_isDebitOfKH: function (idDoiTuong) {
            var self = this;
            var arrIDDV = self.listData.arrIDDonVi;
            var loaiDoiTuong = self.formType;
            if (loaiDoiTuong === 2) {
                loaiDoiTuong = 1;
            }
            $.ajax({
                type: 'POST',
                url: '/api/DanhMuc/DM_DoiTuongAPI/' + 'GetListHD_isDebit?idDoiTuong=' + idDoiTuong
                    + '&arrDonVi=' + arrIDDV + '&loaiDoiTuong=' + loaiDoiTuong,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(arrIDDV),
            }).done(function (x) {
                if (x.res) {
                    self.showCheckHachToan = x.dataSoure.length === 0;

                    let tongnoHD = 0;
                    let data = x.dataSoure;
                    let arrIDHoaDon = [];
                    for (let i = 0; i < data.length; i++) {
                        let itFor = data[i];
                        if (itFor.MaHoaDon.indexOf('GDV') > -1) {
                            data[i].LoaiHoaDon = 19;
                        }
                        if (itFor.MaHoaDon.indexOf('HD') > -1) {
                            data[i].LoaiHoaDon = 1;
                        }
                        if (itFor.MaHoaDon.indexOf('TGT') > -1) {
                            data[i].LoaiHoaDon = 22;
                        }
                        data[i].TienThu = 0;
                        data[i].BH_NhanVienThucHiens = [];
                        data[i].CanThu = itFor.TienMat;
                        data[i].PhaiThu = itFor.PhaiThanhToan;
                        data[i].DaThuTruoc = itFor.KhachDaTra;
                        arrIDHoaDon.push(itFor.ID);
                        tongnoHD += itFor.TienMat;
                    }
                    self.listData.HoaDons = data;

                    // chỉ lấy tổng nợ hóa đơn (không tính nợ tiền cọc)
                    self.newPhieuThu.TongNoHD = tongnoHD;
                    self.HoaDonChosing.PhaiThanhToan = tongnoHD;
                    self.GetSoDuDatCoc(idDoiTuong);

                    let obj = {
                        IDHoaDons: arrIDHoaDon,
                    }
                    self.GetListNVChietKhau_ManyHoaDon(obj);
                }
                else {
                    self.showCheckHachToan = false;
                }
            });
        },
        GetListNVChietKhau_ManyHoaDon: function (data) {
            var self = this;
            if (data.IDHoaDons.length > 0) {
                ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'GetListNVChietKhau_ManyHoaDon', 'POST', data).done(function (obj) {
                    if (obj.res === true && obj.data.length > 0) {
                        var arrCK = obj.data;
                        // assign lst NVThucHien into lstHD
                        for (let i = 0; i < self.listData.HoaDons.length; i++) {
                            let itFor = self.listData.HoaDons[i];
                            let arrNVienCK = $.grep(arrCK, function (x) {
                                return x.ID_HoaDon === itFor.ID;
                            });
                            // format ChietKhauMacDinh --> show at modal
                            for (let k = 0; k < arrNVienCK.length; k++) {
                                arrNVienCK[k].ChietKhauMacDinh = formatNumber3Digit(arrNVienCK[k].ChietKhauMacDinh);
                            }
                            self.listData.HoaDons[i].BH_NhanVienThucHiens = arrNVienCK;
                        }
                    }
                    if (self.formType !== 1) {
                        self.AssignMoney_InHoaDonDebit();
                    }
                })
            }
        },
        GetSoDuDatCoc: function (idDoiTuong, isGetData = false) {// neu mucdich: chi lay sodu
            let self = this;
            let soduDatCoc = 0;
            ajaxHelper('/api/DanhMuc/DM_DoiTuongAPI/' + "GetTienDatCoc_ofDoiTuong?idDoiTuong=" + idDoiTuong
                + '&idDonVi=' + self.inforLogin.ID_DonVi, 'GET').done(function (x) {
                    if (x.res && x.dataSoure.length > 0) {
                        soduDatCoc = x.dataSoure[0].SoDuTheGiaTri;
                    }
                    self.SoDuDatCoc = soduDatCoc;

                    if (!isGetData) {
                        let nohientai = self.newPhieuThu.TongNoHD;
                        if (self.typeUpdate === 1) {
                            self.SoDuDatCoc += formatNumberToFloat(self.newPhieuThu.TienDatCoc);
                        }
                        else {
                            let tiendatcoc = 0, tienmat = 0;
                            if (soduDatCoc > 0) {
                                if (soduDatCoc > nohientai) {
                                    tiendatcoc = nohientai;
                                }
                                else {
                                    tiendatcoc = soduDatCoc;
                                    tienmat = nohientai - tiendatcoc;
                                }
                            }
                            else {
                                tienmat = nohientai;
                            }
                            self.newPhieuThu.TienDatCoc = formatNumber3Digit(tiendatcoc);
                            self.newPhieuThu.TienMat = formatNumber3Digit(tienmat);
                            self.newPhieuThu.DaThanhToan = self.newPhieuThu.TongNoHD;
                            self.newPhieuThu.ThucThu = tienmat;
                            if (self.formType !== 0) {
                                self.AssignMoney_InHoaDonDebit();
                            }
                        }
                    }
                });
        },
        showListNguoiNop: function () {
            $(event.currentTarget).next().show();
        },
        GetKhoanThuChi_byLoaiChungTu: function (lakhoanthu = false) {
            let self = this;
            let loaiHD = self.LoaiHoaDon;
            if (commonStatisJs.CheckNull(loaiHD)) {
                loaiHD = 1;
            }
            let ktc = $.grep(self.listData.KhoanThuChis, function (x) {
                return x.LoaiChungTu === loaiHD.toString() && x.LaKhoanThu === lakhoanthu;
            });
            return ktc;
        },
        showModalThanhToan: function (item, formType = 0) {
            var self = this;
            self.ResetHinhThucTT();
            self.formType = formType;
            self.LoaiHoaDon = item.LoaiHoaDon;
            self.typeUpdate = 0;
            self.isThuTienThua = false;
            self.isKhoaSo = false;
            self.saveOK = false;
            self.KhongBuTruCongNo = false;
            self.listData.HoaDons = [];
            self.ddl_textVal = {
                staffName: self.inforLogin.TenNhanVien,
                accountPOSName: '',
                accountCKName: '',
                khoanthu: '',
                cusCode: '',
                cusName: '',
                cusPhone: '',
            };
            self.newPhieuThu = {
                MaHoaDon: '',
                LoaiHoaDon: 11,
                NgayLapHoaDon: moment(new Date()).format('YYYY-MM-DD HH:mm'),
                TienDatCoc: 0,
                TienMat: 0,
                TienPOS: 0,
                TienCK: 0,
                TienTheGiaTri: 0,
                TTBangDiem: 0,
                DiemQuyDoi: 0,
                ID_TaiKhoanPos: null,
                ID_TaiKhoanChuyenKhoan: null,
                ID_KhoanThuChi: null,
                ID_DoiTuong: null,// nguoinop
                ID_NhanVien: self.inforLogin.ID_NhanVien,
                ID_DonVi: self.inforLogin.ID_DonVi,
                NoHienTai: 0,
                TongNoHD: 0,
                TienThua: 0,
                ThucThu: 0,
                NoiDungThu: '',
                LoaiDoiTuong: formType,
                TrangThai: true,

                HachToanKinhDoanh: true,
                PhieuDieuChinhCongNo: 0,

                TongPhiThanhToan: 0,
                PhiThanhToan_PTGiaTri: 0,
                PhiThanhToan_LaPhanTram: true,
                PhiThanhToan_PTGiaTriTheoHoaDon: 0,

                ListTKPos: [self.newPhuongThucTT(true)],
                ListTKChuyenKhoan: [self.newPhuongThucTT(false)],
            };

            let ktc = self.GetKhoanThuChi_byLoaiChungTu(self.LoaiHoaDon !== 6);
            if (ktc.length > 0) {
                self.newPhieuThu.ID_KhoanThuChi = ktc[0].ID;
                self.ddl_textVal.khoanthu = ktc[0].NoiDungThuChi;
            }

            switch (formType) {
                case 0:// DS hoadon + banlamviec
                    self.showCheckHachToan = false;
                    self.HoaDonChosing = item;

                    self.newPhieuThu.LoaiHoaDon = item.LoaiHoaDon === 6 ? 12 : 11;

                    var nguoinop = [];
                    var invoice = [];
                    var khachCanTra = item.PhaiThanhToan - item.KhachDaTra;
                    var baohiemCanTra = item.PhaiThanhToanBaoHiem - item.BaoHiemDaTra;

                    if (khachCanTra > 0) {
                        // get thong tin khachhang
                        invoice = [{
                            ID: item.ID,
                            MaHoaDon: item.MaHoaDon,
                            LoaiHoaDon: item.LoaiHoaDon,
                            NgayLapHoaDon: item.NgayLapHoaDon,
                            TongThanhToan: item.TongThanhToan,
                            PhaiThu: item.PhaiThanhToan,
                            TongTienThue: item.TongTienThue,
                            DaThuTruoc: item.KhachDaTra,
                            CanThu: khachCanTra,
                            TienThu: formatNumber3Digit(khachCanTra),
                            BH_NhanVienThucHiens: [],
                        }];

                        if (item.ID_DoiTuong !== null) {
                            let cus = {
                                ID: item.ID_DoiTuong,
                                MaNguoiNop: item.MaDoiTuong,
                                TenNguoiNop: item.TenDoiTuong,
                                DienThoaiKhachHang: item.DienThoai,
                                LoaiDoiTuong: 2
                            };
                            nguoinop.push(cus);
                        }

                        self.newPhieuThu.NoHienTai = khachCanTra;
                        self.newPhieuThu.LoaiDoiTuong = 1;
                        self.newPhieuThu.TongNoHD = khachCanTra;
                        self.ddl_textVal.cusCode = item.MaDoiTuong;
                        self.ddl_textVal.cusName = item.TenDoiTuong;
                        self.ddl_textVal.cusPhone = item.DienThoai;
                        self.newPhieuThu.ID_DoiTuong = item.ID_DoiTuong;
                    }
                    else {
                        if (baohiemCanTra > 0) {
                            invoice = [{
                                ID: item.ID,
                                MaHoaDon: item.MaHoaDon,
                                LoaiHoaDon: item.LoaiHoaDon,
                                NgayLapHoaDon: item.NgayLapHoaDon,
                                TongThanhToan: item.TongThanhToan,
                                PhaiThu: item.PhaiThanhToanBaoHiem,
                                TongTienThue: item.TongTienThue,
                                DaThuTruoc: item.BaoHiemDaTra,
                                CanThu: baohiemCanTra,
                                TienThu: formatNumber3Digit(baohiemCanTra),
                                BH_NhanVienThucHiens: [],
                            }];
                            self.newPhieuThu.NoHienTai = baohiemCanTra;
                            self.newPhieuThu.TongNoHD = baohiemCanTra;
                            self.newPhieuThu.LoaiDoiTuong = 3;

                            if (khachCanTra <= 0) {
                                self.ddl_textVal.cusCode = item.MaBaoHiem;
                                self.ddl_textVal.cusName = item.TenBaoHiem;
                                self.ddl_textVal.cusPhone = item.DienThoaiBaoHiem;
                                self.newPhieuThu.ID_DoiTuong = item.ID_BaoHiem;
                            }
                        }
                    }
                    if (baohiemCanTra > 0) {
                        if (item.ID_BaoHiem !== null) {
                            let baohiem = {
                                ID: item.ID_BaoHiem,
                                MaNguoiNop: item.MaBaoHiem,
                                TenNguoiNop: item.TenBaoHiem,
                                DienThoaiKhachHang: item.DienThoaiBaoHiem,
                                LoaiDoiTuong: 3
                            };
                            nguoinop.push(baohiem);
                        }
                    }
                    self.listData.NguoiNops = nguoinop;
                    self.listData.HoaDons = invoice;

                    if (nguoinop.length > 0) {
                        self.GetSoDuDatCoc(nguoinop[0].ID);
                        self.GetSoDuTheGiaTri(nguoinop[0].ID);
                    }

                    // get chietkhaunv theo hoadon
                    ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'GetChietKhauNV_HoaDon?idHoaDon=' + item.ID, 'GET').done(function (x) {
                        if (x.res === true) {
                            self.listData.HoaDons[0].BH_NhanVienThucHiens = x.data;
                        }
                    });
                    break;
                case 1:// DS khachhang
                case 3:// DS baohiem
                    var idCus = item.ID;
                    let cus = {
                        ID: idCus,
                        MaNguoiNop: item.MaDoiTuong,
                        TenNguoiNop: item.TenDoiTuong,
                        DienThoaiKhachHang: item.DienThoai,
                    };
                    self.listData.NguoiNops = [cus];
                    self.ddl_textVal.cusCode = item.MaDoiTuong;
                    self.ddl_textVal.cusName = item.TenDoiTuong;
                    self.ddl_textVal.cusPhone = item.DienThoai;
                    self.newPhieuThu.ID_DoiTuong = idCus;
                    self.HoaDonChosing.DienThoaiKhachHang = item.DienThoai;

                    self.newPhieuThu.NoHienTai = item.NoHienTai;
                    self.GetListHD_isDebitOfKH(idCus);
                    self.GetSoDuTheGiaTri(idCus);
                    break;
                default:// form lapphieuthu at banhang
                    break;
            }
            self.GetInforBasic_Cus();
            $('#ThuTienHoaDonModal').modal('show');
        },
        ChangeCustomer: function (item) {
            var self = this;
            var idCus = item.ID;
            self.newPhieuThu.ID_DoiTuong = idCus;
            self.ddl_textVal.cusCode = item.MaDoiTuong;
            self.ddl_textVal.cusName = item.TenDoiTuong;
            self.ddl_textVal.cusPhone = item.DienThoai;
            self.HoaDonChosing.DienThoaiKhachHang = item.DienThoai;
            self.GetDebitCustomer_allBrands(idCus);
            self.GetSoDuTheGiaTri(idCus);
            self.GetInforBasic_Cus();
        },
        ChangeCreator: function (item) {
            var self = this;
            self.ddl_textVal.staffName = item.TenNhanVien;
            self.newPhieuThu.ID_NhanVien = item.ID;
        },
        AccountPos_AddRow: function () {
            let self = this;
            self.newPhieuThu.ListTKPos.push(self.newPhuongThucTT(true));
        },
        AccountCK_AddRow: function () {
            let self = this;
            self.newPhieuThu.ListTKChuyenKhoan.push(self.newPhuongThucTT(false));
        },
        RemoveAccount: function (type, index) {
            let self = this;
            switch (type) {
                case 0:// pos
                    self.newPhieuThu.ListTKPos.splice(index, 1);
                    break;
                case 1:// ck
                    self.newPhieuThu.ListTKChuyenKhoan.splice(index, 1);
                    break;
            }
        },
        KH_GetTongPos: function () {
            var self = this;
            var tongPos = 0;
            for (let i = 0; i < self.newPhieuThu.ListTKPos.length; i++) {
                let itFor = self.newPhieuThu.ListTKPos[i];
                tongPos += formatNumberToFloat(itFor.TienPOS);
            }
            return tongPos;
        },
        KH_GetTongCK: function () {
            let self = this;
            let tongCK = 0;
            for (let i = 0; i < self.newPhieuThu.ListTKChuyenKhoan.length; i++) {
                let itFor = self.newPhieuThu.ListTKChuyenKhoan[i];
                tongCK += formatNumberToFloat(itFor.TienCK);
            }
            return tongCK;
        },
        GetChiPhi_Visa: function () {
            let self = this;
            let tongChiPhi = 0, gtriPTram = 0, tongPOS = 0;
            let laPhanTram = true;
            for (let i = 0; i < self.newPhieuThu.ListTKPos.length; i++) {
                let itFor = self.newPhieuThu.ListTKPos[i];
                if (itFor.ChiPhiThanhToan > 0) {
                    laPhanTram = itFor.TheoPhanTram;
                    gtriPTram = itFor.ChiPhiThanhToan;
                    if (itFor.TheoPhanTram) {
                        tongChiPhi += formatNumberToFloat(itFor.TienPOS) * itFor.ChiPhiThanhToan / 100;
                        tongPOS += formatNumberToFloat(itFor.TienPOS);
                    }
                    else {
                        tongChiPhi += itFor.ChiPhiThanhToan;
                    }
                }
            }
            self.newPhieuThu.TongPhiThanhToan = tongChiPhi;
            self.newPhieuThu.PhiThanhToan_PTGiaTri = gtriPTram;
            self.newPhieuThu.PhiThanhToan_LaPhanTram = laPhanTram;
            if (laPhanTram) {
                self.newPhieuThu.PhiThanhToan_PTGiaTriTheoHoaDon = gtriPTram;
            }
            else {
                self.newPhieuThu.PhiThanhToan_PTGiaTriTheoHoaDon = tongChiPhi / tongPOS * 100;
            }
            return tongChiPhi;
        },
        CaculatorDaThanhToan: function (isChangeTienThu = false) {
            var self = this;
            let tongPos = self.KH_GetTongPos();
            let tongCK = self.KH_GetTongCK();
            self.newPhieuThu.TienPOS = tongPos;
            self.newPhieuThu.TienCK = tongCK;
            self.newPhieuThu.DaThanhToan = formatNumberToFloat(self.newPhieuThu.TienMat)
                + formatNumberToFloat(self.newPhieuThu.TienPOS)
                + formatNumberToFloat(self.newPhieuThu.TienCK)
                + formatNumberToFloat(self.newPhieuThu.TienTheGiaTri)
                + formatNumberToFloat(self.newPhieuThu.TienDatCoc)
                + formatNumberToFloat(self.newPhieuThu.TTBangDiem);
            self.newPhieuThu.ThucThu = self.newPhieuThu.DaThanhToan
                - formatNumberToFloat(self.newPhieuThu.TTBangDiem)
                - formatNumberToFloat(self.newPhieuThu.TienTheGiaTri);
            self.newPhieuThu.TienThua = self.newPhieuThu.DaThanhToan - self.newPhieuThu.TongNoHD;
            self.GetChiPhi_Visa();
            self.AssignMoney_InHoaDonDebit(isChangeTienThu);
        },
        Only_ResetAccountPOS: function () {
            let self = this;
            for (let i = 0; i < self.newPhieuThu.ListTKPos.length; i++) {
                if (i === self.Pos_indexChosing) {
                    self.newPhieuThu.ListTKPos[i].ID_TaiKhoanPos = null;
                    self.newPhieuThu.ListTKPos[i].TenTaiKhoanPos = '';
                    self.newPhieuThu.ListTKPos[i].SoTaiKhoanPos = '';
                    self.newPhieuThu.ListTKPos[i].TenNganHangCK = '';
                    self.newPhieuThu.ListTKPos[i].TienPOS = 0;
                    self.newPhieuThu.ListTKPos[i].ChiPhiThanhToan = 0;
                    self.newPhieuThu.ListTKPos[i].TheoPhanTram = true;
                    self.newPhieuThu.ListTKPos[i].ThuPhiThanhToan = false;
                    self.newPhieuThu.ListTKPos[i].MacDinh = false;
                    break;
                }
            }
        },
        GetIndexChosing: function (type, index) {
            let self = this;
            switch (type) {
                case 0:// pos
                    self.Pos_indexChosing = index;
                    break;
                case 1:// ck
                    self.CK_indexChosing = index;
                    break;
            }
        },
        ResetAccountPOS: function () {
            var self = this;
            self.Only_ResetAccountPOS();
            self.CaculatorDaThanhToan();
        },
        ChangeAccountPOS: function (item) {
            var self = this;
            for (let i = 0; i < self.newPhieuThu.ListTKPos.length; i++) {
                if (i === self.Pos_indexChosing) {
                    self.newPhieuThu.ListTKPos[i].ID_TaiKhoanPos = item.ID.toUpperCase();
                    self.newPhieuThu.ListTKPos[i].TenTaiKhoanPos = item.TenChuThe;
                    self.newPhieuThu.ListTKPos[i].SoTaiKhoanPos = item.SoTaiKhoan;
                    self.newPhieuThu.ListTKPos[i].TenNganHangPos = item.TenNganHang;
                    self.newPhieuThu.ListTKPos[i].ChiPhiThanhToan = item.ChiPhiThanhToan;
                    self.newPhieuThu.ListTKPos[i].TheoPhanTram = item.TheoPhanTram;
                    self.newPhieuThu.ListTKPos[i].ThuPhiThanhToan = item.ThuPhiThanhToan;
                    self.newPhieuThu.ListTKPos[i].MacDinh = item.MacDinh;
                    break;
                }
            }
        },
        ChangeAccountCK: function (item) {
            let self = this;
            // chuyenkhoan: khong tinh phi nganhang
            for (let i = 0; i < self.newPhieuThu.ListTKChuyenKhoan.length; i++) {
                if (i === self.CK_indexChosing) {
                    self.newPhieuThu.ListTKChuyenKhoan[i].ID_TaiKhoanChuyenKhoan = item.ID.toUpperCase();
                    self.newPhieuThu.ListTKChuyenKhoan[i].TenTaiKhoanCK = item.TenChuThe;
                    self.newPhieuThu.ListTKChuyenKhoan[i].SoTaiKhoanCK = item.SoTaiKhoan;
                    self.newPhieuThu.ListTKChuyenKhoan[i].TenNganHangCK = item.TenNganHang;
                    self.newPhieuThu.ListTKChuyenKhoan[i].ChiPhiThanhToan = 0;
                    self.newPhieuThu.ListTKChuyenKhoan[i].TheoPhanTram = true;
                    self.newPhieuThu.ListTKChuyenKhoan[i].ThuPhiThanhToan = item.ThuPhiThanhToan;
                    self.newPhieuThu.ListTKChuyenKhoan[i].MacDinh = item.MacDinh;
                    break;
                }
            }
        },
        Only_ResetAccountCK: function () {
            let self = this;
            for (let i = 0; i < self.newPhieuThu.ListTKChuyenKhoan.length; i++) {
                if (i === self.CK_indexChosing) {
                    self.newPhieuThu.ListTKChuyenKhoan[i].ID_TaiKhoanChuyenKhoan = null;
                    self.newPhieuThu.ListTKChuyenKhoan[i].TenTaiKhoanCK = '';
                    self.newPhieuThu.ListTKChuyenKhoan[i].SoTaiKhoanCK = '';
                    self.newPhieuThu.ListTKChuyenKhoan[i].TenNganHangCK = '';
                    self.newPhieuThu.ListTKChuyenKhoan[i].TienCK = 0;
                    self.newPhieuThu.ListTKChuyenKhoan[i].ChiPhiThanhToan = 0;
                    self.newPhieuThu.ListTKChuyenKhoan[i].TheoPhanTram = true;
                    self.newPhieuThu.ListTKChuyenKhoan[i].ThuPhiThanhToan = false;
                    self.newPhieuThu.ListTKChuyenKhoan[i].MacDinh = false;
                    break;
                }
            }
        },
        ResetAccountCK: function () {
            var self = this;
            self.Only_ResetAccountCK();
            self.CaculatorDaThanhToan();
        },
        ChangeKhoanThu: function (item) {
            var self = this;
            self.ddl_textVal.khoanthu = item.NoiDungThuChi;
            self.newPhieuThu.ID_KhoanThuChi = item.ID;
        },
        ResetKhoanThu: function () {
            var self = this;
            self.ddl_textVal.khoanthu = '';
            self.newPhieuThu.ID_KhoanThuChi = null;
        },

        ChoseNguoiNopTien: function (item) {
            var self = this;
            self.newPhieuThu.ID_DoiTuong = item.ID;
            $(event.currentTarget).closest('div').hide();

            if (self.formType === 0) {
                // khachhang
                var khachCanTra = 0;
                var invoice = [];
                if (item.ID === self.HoaDonChosing.ID_DoiTuong) {
                    khachCanTra = self.HoaDonChosing.PhaiThanhToan - self.HoaDonChosing.KhachDaTra;

                    invoice = [{
                        ID: self.HoaDonChosing.ID,
                        MaHoaDon: self.HoaDonChosing.MaHoaDon,
                        LoaiHoaDon: self.HoaDonChosing.LoaiHoaDon,
                        NgayLapHoaDon: self.HoaDonChosing.NgayLapHoaDon,
                        TongThanhToan: self.HoaDonChosing.TongThanhToan,
                        PhaiThu: self.HoaDonChosing.PhaiThanhToan,
                        DaThuTruoc: self.HoaDonChosing.KhachDaTra,
                        TongTienThue: self.HoaDonChosing.TongTienThue,
                        CanThu: khachCanTra,
                        TienThu: formatNumber3Digit(khachCanTra),
                        BH_NhanVienThucHiens: [],
                    }];
                    self.newPhieuThu.LoaiDoiTuong = 1;
                }
                else {
                    khachCanTra = self.HoaDonChosing.PhaiThanhToanBaoHiem - self.HoaDonChosing.BaoHiemDaTra;
                    invoice = [{
                        ID: self.HoaDonChosing.ID,
                        MaHoaDon: self.HoaDonChosing.MaHoaDon,
                        LoaiHoaDon: self.HoaDonChosing.LoaiHoaDon,
                        NgayLapHoaDon: self.HoaDonChosing.NgayLapHoaDon,
                        TongThanhToan: self.HoaDonChosing.TongThanhToan,
                        PhaiThu: self.HoaDonChosing.PhaiThanhToanBaoHiem,
                        DaThuTruoc: self.HoaDonChosing.BaoHiemDaTra,
                        TongTienThue: self.HoaDonChosing.TongTienThue,
                        CanThu: khachCanTra,
                        TienThu: formatNumber3Digit(khachCanTra),
                        BH_NhanVienThucHiens: [],
                    }];
                    self.newPhieuThu.LoaiDoiTuong = 3;
                }
                self.newPhieuThu.ID_DoiTuong = item.ID;
                self.ddl_textVal.cusCode = item.MaNguoiNop;
                self.ddl_textVal.cusName = item.TenNguoiNop;
                self.ddl_textVal.cusPhone = item.DienThoaiKhachHang;
                self.listData.HoaDons = invoice;
                self.newPhieuThu.NoHienTai = khachCanTra;
                self.newPhieuThu.TongNoHD = khachCanTra;
                self.newPhieuThu.TienMat = formatNumber3Digit(khachCanTra);
                self.newPhieuThu.TienATM = 0;
                self.newPhieuThu.TienCK = 0;
                self.newPhieuThu.TienTheGiaTri = 0;
                self.newPhieuThu.DaThanhToan = khachCanTra;
                self.newPhieuThu.ThucThu = khachCanTra;

                self.GetSoDuDatCoc(self.newPhieuThu.ID_DoiTuong);
                self.GetSoDuTheGiaTri(self.newPhieuThu.ID_DoiTuong);
                self.GetInforBasic_Cus();
            }
            else {

            }
        },
        CheckKhoaSo: function (dtChose, idChiNhanh = null) {
            let self = this;
            if (idChiNhanh === null) {
                idChiNhanh = VHeader.IdDonVi;
            }
            let tLap = $.grep(self.ThietLapCuaHang, function (x) {
                return x.ID_DonVi === idChiNhanh;
            });
            if (tLap.length > 0) {
                if (tLap[0].KhoaSo) {
                    let chotSo = $.grep(self.ThietLapChotSo, function (x) {
                        return x.ID_DonVi === idChiNhanh;
                    });
                    if (chotSo.length > 0) {
                        let ngayChot = moment(chotSo[0].NgayChotSo).format('YYYY-MM-DD');
                        if (dtChose <= ngayChot) {
                            self.NgayChotSo = chotSo[0].NgayChotSo;
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                    else {
                        return false;
                    }
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }
        },
        ChangeNgayLapPhieu: function (e) {
            var self = this;
            var dt = moment(e).format('YYYY-MM-DD HH:mm');
            let khoaSo = self.CheckKhoaSo(moment(e).format('YYYY-MM-DD'), self.newPhieuThu.ID_DonVi);
            if (khoaSo) {
                ShowMessage_Danger('Ngày lập phải sau thời gian chốt sổ là ' + moment(self.NgayChotSo).format('DD/MM/YYYY'));
            }
            self.newPhieuThu.NgayLapHoaDon = dt;
        },
        AssignMoney_InHoaDonDebit: function (isChangeTienThu = false) {
            var self = this;
            if (!isChangeTienThu) {
                var tongthu = self.newPhieuThu.DaThanhToan;
                for (let i = 0; i < self.listData.HoaDons.length; i++) {
                    if (tongthu > self.listData.HoaDons[i].CanThu) {
                        self.listData.HoaDons[i].TienThu = formatNumber3Digit(self.listData.HoaDons[i].CanThu);
                        tongthu = tongthu - self.listData.HoaDons[i].CanThu;
                    }
                    else {
                        self.listData.HoaDons[i].TienThu = formatNumber3Digit(tongthu);
                        tongthu = 0;
                    }
                }
            }
        },
        UpdateChietKhauNV_ifChangeThucThu: function (idHoaDon, thucthu) {
            var self = this;
            for (let j = 0; j < self.listData.HoaDons.length; j++) {
                let hd = self.listData.HoaDons[j];
                if (hd.ID === idHoaDon) {
                    for (let i = 0; i < hd.BH_NhanVienThucHiens.length; i++) {
                        let nv = hd.BH_NhanVienThucHiens[i];
                        if (parseInt(nv.TinhChietKhauTheo) === 1) {
                            let chiphiNganHang = formatNumberToFloat(hd.TongPhiNganHang);
                            self.listData.HoaDons[j].BH_NhanVienThucHiens[i].TienChietKhau = formatNumber3Digit((thucthu - chiphiNganHang) * (nv.PT_ChietKhau / 100) * nv.HeSo);
                        }
                    }
                    break;
                }
            }
        },
        EditTienCoc: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var tienmat = 0;
            var tiendiem = formatNumberToFloat(self.newPhieuThu.TTBangDiem);
            var khachcantra = formatNumberToFloat(self.newPhieuThu.TongNoHD);
            var soduDatCoc = self.SoDuDatCoc;
            var datcoc = formatNumberToFloat($this.val());
            if (datcoc >= khachcantra) {
                if (datcoc >= soduDatCoc) {
                    if (soduDatCoc > khachcantra) {
                        datcoc = khachcantra;
                        tienmat = soduDatCoc - datcoc;
                    }
                    else {
                        datcoc = soduDatCoc;
                        tienmat = khachcantra - datcoc - tiendiem;
                    }
                }
                else {
                    datcoc = khachcantra;
                    tienmat = soduDatCoc - datcoc - tiendiem;
                }
            }
            else {
                if (datcoc >= soduDatCoc) {
                    datcoc = soduDatCoc;
                }
                tienmat = khachcantra - datcoc - tiendiem;
            }
            tienmat = tienmat < 0 ? 0 : tienmat;

            self.newPhieuThu.TienDatCoc = formatNumber3Digit(datcoc);
            self.newPhieuThu.TienMat = formatNumber3Digit(tienmat);
            self.newPhieuThu.TienPOS = 0;
            self.newPhieuThu.TienCK = 0;
            self.newPhieuThu.TienTheGiaTri = 0;

            self.Reset_AllAccountPOS();
            self.Reset_AllAccountCK();
            self.CaculatorDaThanhToan();

            var key = event.keyCode || event.which;
            if (key === 13) {
                $this.parent().next().find('input').focus();
            }
        },

        Reset_AllAccountPOS: function () {
            let self = this;
            for (let i = 0; i < self.newPhieuThu.ListTKPos.length; i++) {
                self.newPhieuThu.ListTKPos[i].ID_TaiKhoanPos = null;
                self.newPhieuThu.ListTKPos[i].TenTaiKhoanPos = '';
                self.newPhieuThu.ListTKPos[i].SoTaiKhoanPos = '';
                self.newPhieuThu.ListTKPos[i].TenNganHangCK = '';
                self.newPhieuThu.ListTKPos[i].TienPOS = 0;
                self.newPhieuThu.ListTKPos[i].ChiPhiThanhToan = 0;
                self.newPhieuThu.ListTKPos[i].TheoPhanTram = true;
                self.newPhieuThu.ListTKPos[i].ThuPhiThanhToan = false;
                self.newPhieuThu.ListTKPos[i].MacDinh = false;
            }
        },
        Reset_AllAccountCK: function () {
            let self = this;
            for (let i = 0; i < self.newPhieuThu.ListTKPos.length; i++) {
                self.newPhieuThu.ListTKPos[i].ID_TaiKhoanPos = null;
                self.newPhieuThu.ListTKPos[i].TenTaiKhoanPos = '';
                self.newPhieuThu.ListTKPos[i].SoTaiKhoanPos = '';
                self.newPhieuThu.ListTKPos[i].TenNganHangCK = '';
                self.newPhieuThu.ListTKPos[i].TienPOS = 0;
                self.newPhieuThu.ListTKPos[i].ChiPhiThanhToan = 0;
                self.newPhieuThu.ListTKPos[i].TheoPhanTram = true;
                self.newPhieuThu.ListTKPos[i].ThuPhiThanhToan = false;
                self.newPhieuThu.ListTKPos[i].MacDinh = false;
            }
        },

        EditMoney_AssignTienThe: function (cantt) {
            var self = this;
            var soduThe = self.HoaDonChosing.SoDuTheGiaTri;
            var tienthe = 0;
            if (soduThe > 0) {
                if (soduThe >= cantt) {
                    tienthe = cantt;
                }
                else {
                    tienthe = soduThe;
                }
            }
            self.newPhieuThu.TienTheGiaTri = formatNumber3Digit(tienthe);
        },

        QuyDoiDiem_fromGtriTien: function (gtri = 0) {
            let self = this;
            gtri = formatNumberToFloat(gtri);
            return Math.ceil(gtri * self.ThietLap_TichDiem.DiemThanhToan / self.ThietLap_TichDiem.TienThanhToan);
        },

        UpdateDiemKH_toDB: function (diemNow) {
            let self = this;
            let obj = {
                ID: self.newPhieuThu.ID_DoiTuong,
                TongTichDiem: diemNow,
            }
            let myData = {
                objDoiTuong: obj,
            };
            ajaxHelper('/api/DanhMuc/DM_DoiTuongAPI/' + 'UpdateDiem_DMDoiTuong', 'POST', myData).done(function (err) {
                if (err !== '') {
                }
            });
        },

        Edit_TTBangDiem: function () {
            let self = this;
            let $this = $(event.currentTarget);
            formatNumberObj($this);

            let diemQD = formatNumberToFloat($this.val());
            if (!self.ThietLap_TichDiem.DuocThietLap) {
                return;
            }
            if (self.typeUpdate === 1) {
                if (self.inforCus.TongTichDiem + formatNumberToFloat(self.phieuThuOld.DiemQuyDoi) < diemQD) {
                    ShowMessage_Danger('Vượt quá số điểm hiện tại ');
                    return;
                }
            }
            else {
                if (self.inforCus.TongTichDiem < diemQD) {
                    ShowMessage_Danger('Vượt quá số điểm hiện tại ');
                    return;
                }
            }

            let diemTT = self.ThietLap_TichDiem.DiemThanhToan;
            diemTT = diemTT === 0 ? 1 : diemTT;
            let tienQuyDoi = Math.floor(diemQD * self.ThietLap_TichDiem.TienThanhToan / self.ThietLap_TichDiem.DiemThanhToan);
            self.newPhieuThu.DiemQuyDoi = $this.val();
            self.newPhieuThu.TTBangDiem = tienQuyDoi;

            // caculator again tienmat
            let tienMat = self.newPhieuThu.TongNoHD - tienQuyDoi
                - formatNumberToFloat(self.newPhieuThu.TienTheGiaTri) - formatNumberToFloat(self.newPhieuThu.TienDatCoc);
            tienMat = tienMat < 0 ? 0 : tienMat;
            self.newPhieuThu.TienMat = formatNumber3Digit(tienMat, 2);
            self.CaculatorDaThanhToan();
        },

        EditTienMat: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var tiencoc = formatNumberToFloat(self.newPhieuThu.TienDatCoc);
            var tiendiem = formatNumberToFloat(self.newPhieuThu.TTBangDiem);

            var tienmat = formatNumberToFloat($this.val());
            self.newPhieuThu.TienMat = $this.val();

            var tienthe = formatNumberToFloat(self.newPhieuThu.TienTheGiaTri);
            var tienpos = 0, tienck = 0;
            var cantt = self.newPhieuThu.TongNoHD - tienmat - tiencoc - tienthe - tiendiem;
            cantt = cantt < 0 ? 0 : cantt;

            let ttPos = $.grep(self.newPhieuThu.ListTKPos, function (x) {
                return x.ID_TaiKhoanPos !== null;
            });
            if (ttPos.length > 0) {
                tienpos = cantt;
                tienpos = tienpos > 0 ? tienpos : 0;
                tienck = 0;

                for (let i = 0; i < self.newPhieuThu.ListTKPos.length; i++) {
                    if (self.newPhieuThu.ListTKPos[i].ID_TaiKhoanPos !== null) {
                        self.newPhieuThu.ListTKPos[i].TienPOS = formatNumber3Digit(tienpos);
                        break;
                    }
                }

                // reset tienCK
                for (let i = 0; i < self.newPhieuThu.ListTKChuyenKhoan.length; i++) {
                    self.newPhieuThu.ListTKChuyenKhoan[i].TienCK = 0;
                }
            }
            else {
                let ttCK = $.grep(self.newPhieuThu.ListTKChuyenKhoan, function (x) {
                    return x.ID_TaiKhoanChuyenKhoan !== null;
                });
                if (ttCK.length > 0) {
                    tienck = cantt;
                    tienck = tienck > 0 ? tienck : 0;

                    for (let i = 0; i < self.newPhieuThu.ListTKChuyenKhoan.length; i++) {
                        if (self.newPhieuThu.ListTKChuyenKhoan[i].ID_TaiKhoanChuyenKhoan !== null) {
                            self.newPhieuThu.ListTKChuyenKhoan[i].TienCK = formatNumber3Digit(tienck);
                            break;
                        }
                    }
                }
                else {
                    // reset tienCK
                    for (let i = 0; i < self.newPhieuThu.ListTKChuyenKhoan.length; i++) {
                        self.newPhieuThu.ListTKChuyenKhoan[i].TienCK = 0;
                    }
                }
            }
            self.CaculatorDaThanhToan();

            var key = event.keyCode || event.which;
            if (key === 13) {
                self.Focus_NextElem();
            }
            self.ResetHinhThucTT();
        },
        EditTienPos: function (index) {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            for (let i = 0; i < self.newPhieuThu.ListTKPos.length; i++) {
                if (i === index) {
                    self.newPhieuThu.ListTKPos[i].TienPOS = $this.val();
                    break;
                }
            }

            var tienck = 0;
            let tienpos = self.KH_GetTongPos();
            var tiencoc = formatNumberToFloat(self.newPhieuThu.TienDatCoc);
            var tienmat = formatNumberToFloat(self.newPhieuThu.TienMat);
            var tienthe = formatNumberToFloat(self.newPhieuThu.TienTheGiaTri);

            for (let i = 0; i < self.newPhieuThu.ListTKChuyenKhoan.length; i++) {
                if (self.newPhieuThu.ListTKChuyenKhoan[i].ID_TaiKhoanChuyenKhoan !== null) {
                    tienck = self.newPhieuThu.PhaiThanhToan - tienmat - tienpos - self.newPhieuThu.TTBangDiem - tienthe - tiencoc;
                    tienck = tienck < 0 ? 0 : tienck;
                    self.newPhieuThu.ListTKChuyenKhoan[i].TienCK = formatNumber3Digit(tienck);
                    break;
                }
            }

            self.CaculatorDaThanhToan();

            var key = event.keyCode || event.which;
            if (key === 13) {
                self.Focus_NextElem(index);
            }
            self.ResetHinhThucTT();
        },
        Focus_NextElem: function (index = null) {
            if (index === null) {
                // enter tienmat
                let elms = $('#ThuTienHoaDonModal ._jsCheck .form-control:not(:disabled)');
                if (elms.length > 0) {
                    $(elms).eq(0).select();
                }
            }
            else {
                let elms = $('#ThuTienHoaDonModal ._jsCheck:gt(' + index + ')  .form-control:not(:disabled)');
                if (elms.length > 0) {
                    $(elms).eq(0).select();
                }
            }
        },
        EditTienCK: function (index) {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            for (let i = 0; i < self.newPhieuThu.ListTKChuyenKhoan.length; i++) {
                if (i === index) {
                    self.newPhieuThu.ListTKChuyenKhoan[i].TienCK = $this.val();
                    break;
                }
            }
            self.CaculatorDaThanhToan();

            var key = event.keyCode || event.which;
            if (key === 13) {
                index = self.newPhieuThu.ListTKPos.length + index;
                self.Focus_NextElem(index);
            }
            self.ResetHinhThucTT();
        },
        EditTienThe: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var tiencoc = formatNumberToFloat(self.newPhieuThu.TienDatCoc);
            if (formatNumberToFloat($this.val()) > self.HoaDonChosing.SoDuTheGiaTri) {
                ShowMessage_Danger('Nhập quá số dư thẻ');
                $this.val(formatNumber3Digit(self.HoaDonChosing.SoDuTheGiaTri));
                self.newPhieuThu.TienTheGiaTri = formatNumber3Digit(self.HoaDonChosing.SoDuTheGiaTri);
                self.CaculatorDaThanhToan();
                return;
            }
            self.newPhieuThu.TienTheGiaTri = $this.val();

            var tienmat = self.newPhieuThu.TongNoHD - formatNumberToFloat($this.val())
                - formatNumberToFloat(self.newPhieuThu.TTBangDiem) - tiencoc;
            tienmat = tienmat < 0 ? 0 : tienmat;
            self.newPhieuThu.TienMat = formatNumber3Digit(tienmat);

            self.CaculatorDaThanhToan();
            self.ResetHinhThucTT();

            var key = event.keyCode || event.which;
            if (key === 13) {
                $this.closest('.container-fluid').next().find('input').select();
            }
        },

        ResetHinhThucTT: function () {
            var self = this;
            self.HinhThucTT = { ID: 0, Text: 'Tất cả' };
        },

        ChoseHinhThucTT: function (val) {
            var self = this;
            // sum TienThu at list HoaDon
            var sum = 0;
            for (let i = 0; i < self.listData.HoaDons.length; i++) {
                let itFor = self.listData.HoaDons[i];
                sum += formatNumberToFloat(itFor.TienThu);
            }
            switch (val) {
                case 1:
                    if (self.SoDuDatCoc < sum) {
                        commonStatisJs.ShowMessageDanger('Tổng tiền thu vượt quá số tiền đã đặt cọc');
                        return;
                    }
                    self.HinhThucTT = { ID: val, Text: 'Thu từ cọc' };
                    self.newPhieuThu.TienDatCoc = formatNumber3Digit(sum);
                    self.newPhieuThu.TienMat = 0;
                    self.newPhieuThu.TienPOS = 0;
                    self.newPhieuThu.TienCK = 0;
                    self.newPhieuThu.TienTheGiaTri = 0;
                    self.Reset_AllAccountPOS();
                    self.Reset_AllAccountCK();
                    break;
                case 2:
                    self.HinhThucTT = { ID: val, Text: 'Tiền mặt' };
                    self.newPhieuThu.TienMat = formatNumber3Digit(sum);
                    self.newPhieuThu.TienPOS = 0;
                    self.newPhieuThu.TienCK = 0;
                    self.newPhieuThu.TienTheGiaTri = 0;
                    self.newPhieuThu.TienDatCoc = 0;
                    self.Reset_AllAccountPOS();
                    self.Reset_AllAccountCK();
                    break;
                case 3:
                    self.HinhThucTT = { ID: val, Text: 'POS' };
                    self.newPhieuThu.TienMat = 0;
                    self.newPhieuThu.TienPOS = formatNumber3Digit(sum);
                    self.newPhieuThu.TienCK = 0;
                    self.newPhieuThu.TienTheGiaTri = 0;
                    self.newPhieuThu.TienDatCoc = 0;
                    self.Reset_AllAccountCK();
                    break;
                case 4:
                    self.HinhThucTT = { ID: val, Text: 'Chuyển khoản' };
                    self.newPhieuThu.TienMat = 0;
                    self.newPhieuThu.TienPOS = 0;
                    self.newPhieuThu.TienCK = formatNumber3Digit(sum);
                    self.newPhieuThu.TienTheGiaTri = 0;
                    self.newPhieuThu.TienDatCoc = 0;
                    self.Reset_AllAccountPOS();
                    break;
                case 5:
                    if (self.HoaDonChosing.SoDuTheGiaTri < sum) {
                        commonStatisJs.ShowMessageDanger('Tổng tiền thu vượt quá số dư thẻ giá trị');
                        return;
                    }
                    self.HinhThucTT = { ID: val, Text: 'Thẻ giá trị' };
                    self.newPhieuThu.TienTheGiaTri = formatNumber3Digit(sum);
                    self.newPhieuThu.TienMat = 0;
                    self.newPhieuThu.TienPOS = 0;
                    self.newPhieuThu.TienCK = 0;
                    self.newPhieuThu.TienDatCoc = 0;
                    self.Reset_AllAccountPOS();
                    self.Reset_AllAccountCK();
                    break;
            }
            self.CaculatorDaThanhToan(true);
        },

        ChangeTienThu: function (item, index) {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var tienThu = formatNumberToFloat($this.val());
            if (isNaN(tienThu)) {
                tienThu = 0;
            }

            for (let i = 0; i < self.listData.HoaDons.length; i++) {
                if (index === i) {
                    if (tienThu > self.listData.HoaDons[i].TienMat) {
                        tienThu = self.listData.HoaDons[i].TienMat;
                    }
                    self.listData.HoaDons[i].TienThu = tienThu;
                    break;
                }
            }
            var hinhthuc = self.HinhThucTT.ID;
            if (hinhthuc === 0) {
                hinhthuc = 2;
            }
            self.ChoseHinhThucTT(hinhthuc);
        },
        ChangeTienThu_Enter: function (index) {
            var $this = $(event.currentTarget);
            $($this).closest('tbody').find('tr').eq(index + 1).find('input').focus().select();
        },
        shareMoney_QuyHD: function (phaiTT, tienDiem, tienmat, tienPOS, chuyenkhoan, thegiatri, tiencoc) {
            let self = this;
            if (tiencoc < phaiTT) {
                phaiTT = phaiTT - tiencoc;
                if (tienDiem >= phaiTT) {
                    return {
                        TienCoc: tiencoc,
                        TTBangDiem: phaiTT,
                        TienMat: 0,
                        TienPOS: 0,
                        TienChuyenKhoan: 0,
                        TienTheGiaTri: 0,
                        DiemThanhToan: self.QuyDoiDiem_fromGtriTien(phaiTT),
                    }
                }
                else {
                    phaiTT = phaiTT - tienDiem;
                    let diemQD = self.QuyDoiDiem_fromGtriTien(tienDiem);
                    if (thegiatri >= phaiTT) {
                        return {
                            TienCoc: tiencoc,
                            TTBangDiem: tienDiem,
                            TienMat: 0,
                            TienPOS: 0,
                            TienChuyenKhoan: 0,
                            TienTheGiaTri: Math.abs(phaiTT),
                            DiemThanhToan: diemQD,
                        }
                    }
                    else {
                        phaiTT = phaiTT - thegiatri;
                        if (tienPOS >= phaiTT) {
                            return {
                                TienCoc: tiencoc,
                                TTBangDiem: tienDiem,
                                TienMat: 0,
                                TienPOS: Math.abs(phaiTT),
                                TienChuyenKhoan: 0,
                                TienTheGiaTri: thegiatri,
                                DiemThanhToan: diemQD,
                            }
                        }
                        else {
                            phaiTT = phaiTT - tienPOS;
                            if (chuyenkhoan >= phaiTT) {
                                return {
                                    TienCoc: tiencoc,
                                    TTBangDiem: tienDiem,
                                    TienMat: 0,
                                    TienPOS: tienPOS,
                                    TienChuyenKhoan: Math.abs(phaiTT),
                                    TienTheGiaTri: thegiatri,
                                    DiemThanhToan: diemQD,
                                }
                            }
                            else {
                                phaiTT = phaiTT - chuyenkhoan;
                                if (tienmat >= phaiTT) {
                                    return {
                                        TienCoc: tiencoc,
                                        TTBangDiem: tienDiem,
                                        TienMat: Math.abs(phaiTT),
                                        TienPOS: tienPOS,
                                        TienChuyenKhoan: chuyenkhoan,
                                        TienTheGiaTri: thegiatri,
                                        DiemThanhToan: diemQD,
                                    }
                                }
                                else {
                                    phaiTT = phaiTT - tienmat;
                                    return {
                                        TienCoc: tiencoc,
                                        TTBangDiem: tienDiem,
                                        TienMat: tienmat,
                                        TienPOS: tienPOS,
                                        TienChuyenKhoan: chuyenkhoan,
                                        TienTheGiaTri: thegiatri,
                                        DiemThanhToan: diemQD,
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else {
                return {
                    TienCoc: phaiTT,
                    TTBangDiem: 0,
                    TienMat: 0,
                    TienPOS: 0,
                    TienChuyenKhoan: 0,
                    TienTheGiaTri: 0,
                    DiemThanhToan: 0,
                }
            }
        },

        SavePhieuThu: function (print) {
            var self = this;
            var ptKhach = self.newPhieuThu;

            let khoaSo = self.CheckKhoaSo(moment(ptKhach.NgayLapHoaDon, 'YYYY-MM-DD HH:mm').format('YYYY-MM-DD'), ptKhach.ID_DonVi);
            if (khoaSo) {
                ShowMessage_Danger('Ngày lập phải sau thời gian chốt sổ là ' + moment(self.NgayChotSo).format('DD/MM/YYYY'));
                return;
            }

            if (self.listData.HoaDons.length > 0) {
                let ngaylapHD = moment(self.listData.HoaDons[0].NgayLapHoaDon).format('YYYY-MM-DD HH:mm');
                let ngaylapPT = ptKhach.NgayLapHoaDon;

                if (ngaylapPT < ngaylapHD) {
                    ShowMessage_Danger('Ngày lập phiếu thu không được nhỏ hơn ngày lập hóa đơn');
                    return;
                }
            }

            if (ptKhach.DaThanhToan === 0) {
                commonStatisJs.ShowMessageDanger('Vui lòng nhập số tiền cần thanh toán');
                return;
            }
            if (ptKhach.ID_DoiTuong === null) {
                commonStatisJs.ShowMessageDanger('Vui lòng chọn người nộp tiền');
                return;
            }
            if (self.HinhThucTT.ID === 3) {
                if (ptKhach.ID_TaiKhoanPos === null || ptKhach.ID_TaiKhoanPos === undefined) {
                    commonStatisJs.ShowMessageDanger('Vui lòng chọn tài khoản POS');
                    return;
                }
            }
            if (self.HinhThucTT.ID === 4) {
                if (ptKhach.ID_TaiKhoanChuyenKhoan === null || ptKhach.ID_TaiKhoanChuyenKhoan === undefined) {
                    commonStatisJs.ShowMessageDanger('Vui lòng chọn tài khoản chuyển khoản');
                    return;
                }
            }

            self.isLoading = true;

            let ghichu = ptKhach.NoiDungThu;
            let idDoiTuong = ptKhach.ID_DoiTuong;
            let idKhoanThuChi = ptKhach.ID_KhoanThuChi;
            let tenDoiTuong = self.ddl_textVal.cusName;
            let sMaHoaDon = '';
            let lstQuyCT = [];
            let nvThucHien = [];
            let loaiThuChi = ptKhach.LoaiHoaDon;
            let phuongthucTT = '';
            var arrPhuongThuc = [];
            self.QuyHD_Share = [];

            if (commonStatisJs.CheckNull(ghichu)) {
                ghichu = ghichu.concat(tenDoiTuong, ' (', self.ddl_textVal.cusCode, ')');
            }
            else {
                ghichu = ghichu.concat(' / ', tenDoiTuong, ' (', self.ddl_textVal.cusCode, ')');
            }
            var objShare = self.UpdateThucThu_EachHoaDonDebit();

            let tongthu = objShare.TienCoc + objShare.TienMat + objShare.TienPOS + objShare.TienChuyenKhoan
                + objShare.TienTheGiaTri + objShare.TTBangDiem;

            // set default {ConLai} for listpos, listCK
            if (!commonStatisJs.CheckNull(ptKhach.ListTKPos)) {
                for (let k = 0; k < ptKhach.ListTKPos.length; k++) {
                    ptKhach.ListTKPos[k].ConLai = formatNumberToFloat(ptKhach.ListTKPos[k].TienPOS);
                }
            }
            if (!commonStatisJs.CheckNull(ptKhach.ListTKChuyenKhoan)) {
                for (let k = 0; k < ptKhach.ListTKChuyenKhoan.length; k++) {
                    ptKhach.ListTKChuyenKhoan[k].ConLai = formatNumberToFloat(ptKhach.ListTKChuyenKhoan[k].TienCK);
                }
            }

            let sumMat = 0, sumPOS = 0, sumCK = 0, sumDiemQD = 0, sumCoc = 0;
            // insert quy_ct
            for (let i = 0; i < self.QuyHD_Share.length; i++) {
                let itemFor = self.QuyHD_Share[i];
                let idHoaDon = itemFor.ID_HoaDonLienQuan;
                let conlai = itemFor.TienThu;// số tiền phải thanh toán còn lại (sau khi trừ dần các loại tiền # nhau)

                // used to congtienthua
                sumMat += formatNumberToFloat(itemFor.TienMat);
                sumPOS += formatNumberToFloat(itemFor.TienPOS);
                sumCK += formatNumberToFloat(itemFor.TienChuyenKhoan);
                sumCoc += formatNumberToFloat(itemFor.TienCoc);

                if (itemFor.TienCoc > 0 && loaiThuChi === 11) {
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: idHoaDon,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ghichu,
                        TienThu: itemFor.TienCoc,
                        TienCoc: itemFor.TienCoc,
                        HinhThucThanhToan: 6,
                    });
                    lstQuyCT.push(qct);
                    conlai -= qct.TienThu;

                    if ($.inArray(qct.HinhThucThanhToan, arrPhuongThuc) === -1) {
                        arrPhuongThuc.push(qct.HinhThucThanhToan);
                    }
                }

                if (itemFor.TTBangDiem > 0) {
                    sumDiemQD += itemFor.DiemThanhToan;// used to caculator again DiemKhachHang
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: idHoaDon,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ghichu,
                        TienThu: itemFor.TTBangDiem,
                        TTBangDiem: itemFor.TTBangDiem,
                        HinhThucThanhToan: 5,
                        DiemThanhToan: itemFor.DiemThanhToan,
                    });
                    lstQuyCT.push(qct);
                    conlai -= qct.TienThu;

                    if ($.inArray(qct.HinhThucThanhToan, arrPhuongThuc) === -1) {
                        arrPhuongThuc.push(qct.HinhThucThanhToan);
                    }
                }

                if (itemFor.TienTheGiaTri > 0) {
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: idHoaDon,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ghichu,
                        TienThu: itemFor.TienTheGiaTri,
                        TienTheGiaTri: itemFor.TienTheGiaTri,
                        HinhThucThanhToan: 4,
                    });
                    lstQuyCT.push(qct);
                    conlai -= qct.TienThu;
                    if ($.inArray(qct.HinhThucThanhToan, arrPhuongThuc) === -1) {
                        arrPhuongThuc.push(qct.HinhThucThanhToan);
                    }
                }

                let hd_POS = itemFor.TienPOS;// tienPOS tối đa của hóa đơn
                if (conlai > 0 && hd_POS > 0) {
                    let qct = {
                        HinhThucThanhToan: 2,
                    }

                    if (!commonStatisJs.CheckNull(ptKhach.ListTKPos)) {
                        for (let j = 0; j < ptKhach.ListTKPos.length; j++) {
                            let forAccount = ptKhach.ListTKPos[j];
                            let item_TienPos = formatNumberToFloat(forAccount.TienPOS);
                            if (conlai > 0 && forAccount.ConLai > 0) {// phân bổ dần tiền POS theo hóa đơn
                                if (hd_POS > forAccount.ConLai) {
                                    hd_POS = hd_POS - forAccount.ConLai;

                                    qct = newQuyChiTiet({
                                        ID_HoaDonLienQuan: idHoaDon,
                                        ID_KhoanThuChi: idKhoanThuChi,
                                        ID_DoiTuong: idDoiTuong,
                                        GhiChu: forAccount.TenTaiKhoanPos.concat(' - ', forAccount.TenNganHangPos),
                                        TienThu: forAccount.ConLai,
                                        TienPOS: forAccount.ConLai,
                                        HinhThucThanhToan: 2,
                                        ID_TaiKhoanNganHang: forAccount.ID_TaiKhoanPos,
                                    });
                                    qct.ChiPhiNganHang = forAccount.ChiPhiThanhToan;
                                    qct.LaPTChiPhiNganHang = forAccount.TheoPhanTram;
                                    lstQuyCT.push(qct);
                                    ptKhach.ListTKPos[j].ConLai = 0;
                                    conlai -= qct.TienThu;
                                }
                                else {
                                    qct = newQuyChiTiet({
                                        ID_HoaDonLienQuan: idHoaDon,
                                        ID_KhoanThuChi: idKhoanThuChi,
                                        ID_DoiTuong: idDoiTuong,
                                        GhiChu: forAccount.TenTaiKhoanPos.concat(' - ', forAccount.TenNganHangPos),
                                        TienThu: hd_POS,
                                        TienPOS: hd_POS,
                                        HinhThucThanhToan: 2,
                                        ID_TaiKhoanNganHang: forAccount.ID_TaiKhoanPos,
                                    });
                                    qct.ChiPhiNganHang = forAccount.ChiPhiThanhToan;
                                    qct.LaPTChiPhiNganHang = forAccount.TheoPhanTram;
                                    lstQuyCT.push(qct);
                                    ptKhach.ListTKPos[j].ConLai = item_TienPos - hd_POS;// phân bổ dần tiền POS theo hóa đơn
                                    hd_POS = 0;
                                    conlai -= qct.TienThu;
                                }
                            }
                        }
                    }
                    else {
                        qct = newQuyChiTiet({
                            ID_HoaDonLienQuan: idHoaDon,
                            ID_KhoanThuChi: idKhoanThuChi,
                            ID_DoiTuong: idDoiTuong,
                            GhiChu: ghichu,
                            TienThu: hd_POS,
                            TienPOS: hd_POS,
                            HinhThucThanhToan: 2,
                            ID_TaiKhoanNganHang: ptKhach.ID_TaiKhoanPos,
                        });
                        lstQuyCT.push(qct);
                    }

                    if ($.inArray(qct.HinhThucThanhToan, arrPhuongThuc) === -1) {
                        arrPhuongThuc.push(qct.HinhThucThanhToan);
                    }
                }

                let hd_CK = itemFor.TienChuyenKhoan;// tienCK tối đa của hóa đơn
                if (conlai > 0 && hd_CK > 0) {
                    let qct = {
                        HinhThucThanhToan: 3,
                    }
                    if (!commonStatisJs.CheckNull(ptKhach.ListTKChuyenKhoan)) {
                        for (let j = 0; j < ptKhach.ListTKChuyenKhoan.length; j++) {
                            let forAccount = ptKhach.ListTKChuyenKhoan[j];
                            let item_TienCK = formatNumberToFloat(forAccount.TienCK);
                            if (conlai > 0 && forAccount.ConLai > 0) {// phân bổ dần tiền POS theo hóa đơn
                                if (hd_CK > forAccount.ConLai) {
                                    hd_CK = hd_CK - forAccount.ConLai;

                                    qct = newQuyChiTiet({
                                        ID_HoaDonLienQuan: idHoaDon,
                                        ID_KhoanThuChi: idKhoanThuChi,
                                        ID_DoiTuong: idDoiTuong,
                                        GhiChu: forAccount.TenTaiKhoanCK.concat(' - ', forAccount.TenNganHangCK),
                                        TienThu: forAccount.ConLai,
                                        TienChuyenKhoan: forAccount.ConLai,
                                        HinhThucThanhToan: 3,
                                        ID_TaiKhoanNganHang: forAccount.ID_TaiKhoanChuyenKhoan,
                                    });
                                    qct.ChiPhiNganHang = forAccount.ChiPhiThanhToan;
                                    qct.LaPTChiPhiNganHang = forAccount.TheoPhanTram;
                                    lstQuyCT.push(qct);
                                    ptKhach.ListTKChuyenKhoan[j].ConLai = 0;
                                    conlai -= qct.TienThu;
                                }
                                else {
                                    qct = newQuyChiTiet({
                                        ID_HoaDonLienQuan: idHoaDon,
                                        ID_KhoanThuChi: idKhoanThuChi,
                                        ID_DoiTuong: idDoiTuong,
                                        GhiChu: forAccount.TenTaiKhoanCK.concat(' - ', forAccount.TenNganHangCK),
                                        TienThu: hd_CK,
                                        TienChuyenKhoan: hd_CK,
                                        HinhThucThanhToan: 3,
                                        ID_TaiKhoanNganHang: forAccount.ID_TaiKhoanChuyenKhoan,
                                    });
                                    qct.ChiPhiNganHang = forAccount.ChiPhiThanhToan;
                                    qct.LaPTChiPhiNganHang = forAccount.TheoPhanTram;
                                    lstQuyCT.push(qct);
                                    ptKhach.ListTKChuyenKhoan[j].ConLai = item_TienCK - hd_CK;// phân bổ dần tiền POS theo hóa đơn
                                    hd_CK = 0;
                                    conlai -= qct.TienThu;
                                }
                            }
                        }
                    }
                    else {
                        qct = newQuyChiTiet({
                            ID_HoaDonLienQuan: idHoaDon,
                            ID_KhoanThuChi: idKhoanThuChi,
                            ID_DoiTuong: idDoiTuong,
                            GhiChu: ghichu,
                            TienThu: hd_CK,
                            TienChuyenKhoan: hd_CK,
                            HinhThucThanhToan: 3,
                            ID_TaiKhoanNganHang: ptKhach.ID_TaiKhoanChuyenKhoan,
                        });
                        lstQuyCT.push(qct);
                    }

                    if ($.inArray(qct.HinhThucThanhToan, arrPhuongThuc) === -1) {
                        arrPhuongThuc.push(qct.HinhThucThanhToan);
                    }
                }

                if (itemFor.TienMat > 0) {
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: idHoaDon,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ghichu,
                        TienThu: itemFor.TienMat,
                        TienMat: itemFor.TienMat,
                        HinhThucThanhToan: 1,
                    });
                    lstQuyCT.push(qct);

                    if ($.inArray(qct.HinhThucThanhToan, arrPhuongThuc) === -1) {
                        arrPhuongThuc.push(qct.HinhThucThanhToan);
                    }
                }

                sMaHoaDon += itemFor.MaHoaDon + ', ';
            }
            sMaHoaDon = Remove_LastComma(sMaHoaDon);

            let sThuThem = '';
            let tongtienNhap = formatNumberToFloat(ptKhach.TienMat) + formatNumberToFloat(ptKhach.TienPOS) +
                formatNumberToFloat(ptKhach.TienCK) + formatNumberToFloat(ptKhach.TienTheGiaTri);

            if (self.isThuTienThua || (self.showCheckHachToan && tongtienNhap > 0)) {
                let loaiTT = self.KhongBuTruCongNo ? 3 : 0;
                if (self.KhongBuTruCongNo) {
                    sThuThem = '<br/> Không tính vào công nợ: Check';
                }
                else {
                    sThuThem = '<br/> Cộng tiền thừa vào tài khoản khách: Check';
                }

                if (objShare.TienMat > sumMat) {
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: null,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ghichu,
                        TienThu: objShare.TienMat - sumMat,
                        TienMat: objShare.TienMat - sumMat,
                        HinhThucThanhToan: 1,
                        LoaiThanhToan: loaiTT,
                    });
                    lstQuyCT.push(qct);

                    if ($.inArray(qct.HinhThucThanhToan, arrPhuongThuc) === -1) {
                        arrPhuongThuc.push(qct.HinhThucThanhToan);
                    }
                }
                if (objShare.TienPOS > sumPOS) {
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: null,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_TaiKhoanNganHang: ptKhach.ID_TaiKhoanPos,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ghichu,
                        TienThu: objShare.TienPOS - sumPOS,
                        TienPOS: objShare.TienPOS - sumPOS,
                        HinhThucThanhToan: 2,
                        LoaiThanhToan: loaiTT,
                    });
                    lstQuyCT.push(qct);

                    if ($.inArray(qct.HinhThucThanhToan, arrPhuongThuc) === -1) {
                        arrPhuongThuc.push(qct.HinhThucThanhToan);
                    }
                }
                if (objShare.TienChuyenKhoan > sumCK) {
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: null,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_TaiKhoanNganHang: ptKhach.ID_TaiKhoanChuyenKhoan,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ghichu,
                        TienThu: objShare.TienChuyenKhoan - sumCK,
                        TienChuyenKhoan: objShare.TienChuyenKhoan - sumCK,
                        HinhThucThanhToan: 3,
                        LoaiThanhToan: loaiTT,
                    });
                    lstQuyCT.push(qct);

                    if ($.inArray(qct.HinhThucThanhToan, arrPhuongThuc) === -1) {
                        arrPhuongThuc.push(qct.HinhThucThanhToan);
                    }
                }

                if (objShare.TienTheGiaTri > 0) {
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: null,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ghichu,
                        TienThu: objShare.TienTheGiaTri,
                        TienTheGiaTri: objShare.TienTheGiaTri,
                        HinhThucThanhToan: 4,
                        LoaiThanhToan: loaiTT,
                    });
                    lstQuyCT.push(qct);

                    if ($.inArray(qct.HinhThucThanhToan, arrPhuongThuc) === -1) {
                        arrPhuongThuc.push(qct.HinhThucThanhToan);
                    }
                }
            }

            for (let i = 0; i < arrPhuongThuc.length; i++) {
                switch (arrPhuongThuc[i]) {
                    case 1:
                        phuongthucTT += 'Tiền mặt, ';
                        break;
                    case 2:
                        phuongthucTT += 'POS, ';
                        break;
                    case 3:
                        phuongthucTT += 'Chuyển khoản, ';
                        break;
                    case 4:
                        phuongthucTT += 'Thẻ giá trị, ';
                        break;
                    case 5:
                        phuongthucTT += 'Điểm, ';
                        break;
                    case 6:
                        phuongthucTT = 'Tiền cọc, ';
                        break;
                }
            }

            let arrAcc = [], sAccount = '';
            for (let i = 0; i < lstQuyCT.length; i++) {
                let ctQuy = lstQuyCT[i];
                if (ctQuy.HinhThucThanhToan === 2 || ctQuy.HinhThucThanhToan === 3) {
                    if ($.inArray(ctQuy.ID_TaiKhoanNganHang, arrAcc) === -1) {
                        arrAcc.push(ctQuy.ID_TaiKhoanNganHang);
                        sAccount += ctQuy.GhiChu + ' ,';
                    }
                }
            }
            sAccount = Remove_LastComma(sAccount);
            sAccount = sAccount !== '' ? '/ ' + sAccount : '';
            ghichu += ' / '.concat(sMaHoaDon, sAccount);

            let quyhd = {
                LoaiHoaDon: loaiThuChi,
                TongTienThu: tongthu,
                MaHoaDon: ptKhach.MaHoaDon,
                NgayLapHoaDon: ptKhach.NgayLapHoaDon,
                NguoiNopTien: tenDoiTuong,
                NguoiTao: self.inforLogin.UserLogin,
                NoiDungThu: ghichu,
                ID_NhanVien: ptKhach.ID_NhanVien,
                ID_DonVi: self.inforLogin.ID_DonVi,
                ID_DoiTuong: ptKhach.ID_DoiTuong,// used to get when saveDB
                HoaDonLienQuan: sMaHoaDon,// print
                HachToanKinhDoanh: ptKhach.HachToanKinhDoanh,
                PhieuDieuChinhCongNo: self.KhongBuTruCongNo ? 3 : 0,
                LoaiDoiTuong: ptKhach.LoaiDoiTuong
            }
            phuongthucTT = Remove_LastComma(phuongthucTT);
            quyhd.PhuongThucTT = phuongthucTT;

            // use when print phieuthu
            quyhd.TienMat = objShare.TienMat;
            quyhd.TienPOS = objShare.TienPOS;
            quyhd.TienChuyenKhoan = objShare.TienChuyenKhoan;
            quyhd.TienCK = objShare.TienChuyenKhoan;
            quyhd.TienTheGiaTri = objShare.TienTheGiaTri;
            quyhd.TienCoc = objShare.TienCoc;
            quyhd.TienDatCoc = objShare.TienCoc;
            quyhd.TTBangDiem = objShare.TTBangDiem;
            quyhd.DiemQuyDoi = sumDiemQD;

            var myData = {
                objQuyHoaDon: quyhd,
                lstCTQuyHoaDon: lstQuyCT
            };

            if (lstQuyCT.length > 0) {
                if (self.typeUpdate === 0) {
                    ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/PostQuy_HoaDon_DefaultIDDoiTuong', 'POST', myData).done(function (x) {
                        self.isLoading = false;
                        if (x.res === true) {
                            self.saveOK = true;
                            var idQuyHD = x.data.ID;
                            quyhd.MaHoaDon = x.data.MaHoaDon;
                            let diary = {
                                LoaiNhatKy: 1,
                                ID_DonVi: quyhd.ID_DonVi,
                                ID_NhanVien: self.inforLogin.ID_NhanVien,
                                ChucNang: 'Phiếu ' + self.sLoai,
                                NoiDung: 'Tạo phiếu '.concat(self.sLoai, ' ', quyhd.MaHoaDon, ' cho hóa đơn ', sMaHoaDon,
                                    ', Khách hàng: ', quyhd.NguoiNopTien, ', với giá trị ', formatNumber3Digit(quyhd.TongTienThu),
                                    ', Phương thức thanh toán:', phuongthucTT,
                                    ', Thời gian: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')),
                                NoiDungChiTiet: 'Tạo phiếu ' + self.sLoai + ' <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD('.concat(quyhd.MaHoaDon, ')" >', quyhd.MaHoaDon, '</a> ',
                                    ' cho hóa đơn: <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD(', sMaHoaDon, ')" >', sMaHoaDon, '</a> ',
                                    '<br /> Khách hàng: <a style="cursor: pointer" onclick = "LoadKhachHang_byMaKH(', quyhd.NguoiNopTien, ')" >', quyhd.NguoiNopTien, '</a> ',
                                    '<br /> Giá trị: ', formatNumber3Digit(quyhd.TongTienThu),
                                    '<br/ > Phương thức thanh toán: ', phuongthucTT,
                                    '<br/ > Khoản ', self.sLoai, ': ', self.ddl_textVal.khoanthu,
                                    '<br/ > Thời gian: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'),
                                    '<br/ > Hạch toán kinh doanh: ', quyhd.HachToanKinhDoanh,
                                    sThuThem,
                                    '<br/ > Người tạo: ', self.inforLogin.UserLogin
                                )
                            }
                            Insert_NhatKyThaoTac_1Param(diary);

                            // save hoahong nvien
                            for (let i = 0; i < self.listData.HoaDons.length; i++) {
                                let itFor = self.listData.HoaDons[i];
                                if (itFor.BH_NhanVienThucHiens.length > 0) {
                                    for (let j = 0; j < itFor.BH_NhanVienThucHiens.length; j++) {
                                        nvThucHien.push(itFor.BH_NhanVienThucHiens[j]);
                                    }
                                }
                            }

                            if (nvThucHien.length > 0) {
                                for (let i = 0; i < nvThucHien.length; i++) {
                                    nvThucHien[i].ID_QuyHoaDon = idQuyHD;
                                }
                                var data1 = {
                                    lstObj: nvThucHien
                                }
                                // only add nvien (don't remove)
                                ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'Post_BHNhanVienThucHien', 'POST', data1).done(function (data) {
                                    if (data.res == false) {
                                    }
                                })
                            }

                            if (sumDiemQD > 0) {
                                let diem = self.inforCus.TongTichDiem - sumDiemQD;
                                diem = diem < 0 ? 0 : diem;
                                self.UpdateDiemKH_toDB(diem);
                            }

                            if (print) {
                                self.InPhieuThu(quyhd);
                            }

                            self.NangNhomKhachHang(idDoiTuong);
                            commonStatisJs.ShowMessageSuccess('Thanh toán thành công');
                        }
                        $('#ThuTienHoaDonModal').modal('hide');
                    })
                }
                else {
                    myData.objQuyHoaDon.ID = self.phieuThuOld.ID;
                    myData.objQuyHoaDon.LoaiHoaDon = self.phieuThuOld.LoaiHoaDon;
                    myData.objQuyHoaDon.NguoiSua = self.inforLogin.UserLogin;

                    for (let i = 0; i < myData.lstCTQuyHoaDon.length; i++) {
                        myData.lstCTQuyHoaDon[i].ID_HoaDon = self.phieuThuOld.ID;
                    }
                    myData.objCTQuyHoaDon = myData.lstCTQuyHoaDon;

                    ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/PutQuy_HoaDon', 'POST', myData).done(function (x) {
                        self.isLoading = false;
                        if (x.res) {
                            commonStatisJs.ShowMessageSuccess('Cập nhật phiếu thu thành công');

                            let sLoaiDT = '';
                            if (quyhd.LoaiHoaDon === 11) {
                                sLoaiDT = 'Thu của '
                            }
                            else {
                                sLoaiDT = 'Chi cho '
                            }
                            switch (quyhd.LoaiDoiTuong) {
                                case 1:
                                    sLoaiDT += 'khách hàng: ';
                                    break;
                                case 3:
                                    sLoaiDT += 'Cty bảo hiểm: ';
                                    break;
                            }

                            let diary = {
                                LoaiNhatKy: 2,
                                ID_DonVi: quyhd.ID_DonVi,
                                ID_NhanVien: self.inforLogin.ID_NhanVien,
                                ChucNang: 'Cập nhật phiếu ' + self.sLoai,
                                NoiDung: 'Cập nhật phiếu '.concat(self.sLoai, ' ', quyhd.MaHoaDon, ' cho hóa đơn ', sMaHoaDon),
                                NoiDungChiTiet: 'Cập nhật phiếu '.concat(self.sLoai, ' ', quyhd.MaHoaDon,
                                    '<br/> <b> Thông tin mới: </b>',
                                    '<br/> - Mã phiếu ', self.sLoai, ': ', quyhd.MaHoaDon,
                                    '<br/> - Ngày lập phiếu: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'),
                                    '<br/> - Tổng tiền ', self.sLoai, ': ', formatNumber3Digit(quyhd.TongTienThu),
                                    '<br/> - ', sLoaiDT, quyhd.NguoiNopTien,
                                    '<br/> - Khoản ', self.sLoai, ': ', self.ddl_textVal.khoanthu,
                                    '<br/> - Phương thức thanh toán: ', quyhd.PhuongThucTT,
                                    '<br/> - Tiền mặt: ', formatNumber3Digit(quyhd.TienMat),
                                    '<br/> - Tiền POS: ', formatNumber3Digit(quyhd.TienPOS),
                                    '<br/> - Tiền chuyển khoản: ', formatNumber3Digit(quyhd.TienChuyenKhoan),
                                    '<br/> - Thu từ thẻ: ', formatNumber3Digit(quyhd.TienTheGiaTri),
                                    '<br/> - Thu từ cọc: ', quyhd.TienCoc,
                                    '<br/> - Sử dụng điểm: ', formatNumber3Digit(quyhd.TTBangDiem), ' (', quyhd.DiemQuyDoi, ' điểm)',
                                    '<br/> - Tài khoản POS: ', self.ddl_textVal.accountPOSName,
                                    '<br/> - Tài khoản chuyển khoản: ', self.ddl_textVal.accountCKName,
                                    '<br/> - NV lập phiếu: ', self.ddl_textVal.staffName,
                                    '<br/> - Người sửa: ', self.inforLogin.UserLogin,

                                    '<br/> <b> Thông tin cũ: </b>',
                                    '<br/> - Mã phiếu ', self.sLoai, ': ', self.phieuThuOld.MaHoaDon,
                                    '<br/> - Ngày lập phiếu: ', moment(self.phieuThuOld.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'),
                                    '<br/> - Tổng tiền ', self.sLoai, ': ', formatNumber3Digit(self.phieuThuOld.TongTienThu),
                                    '<br/> - ', sLoaiDT, self.phieuThuOld.NguoiNopTien, ' (', self.phieuThuOld.MaNguoiNop, ')',
                                    '<br/> - Khoản ', self.sLoai, ': ', self.phieuThuOld.TenKhoanThuChi,
                                    '<br/> - Phương thức thanh toán: ', self.phieuThuOld.PhuongThucTT,
                                    '<br/> - Tiền mặt: ', formatNumber3Digit(self.phieuThuOld.TienMat),
                                    '<br/> - Tiền POS: ', formatNumber3Digit(self.phieuThuOld.TienPOS),
                                    '<br/> - Tiền chuyển khoản: ', formatNumber3Digit(self.phieuThuOld.TienCK),
                                    '<br/> - Thu từ thẻ: ', formatNumber3Digit(self.phieuThuOld.TienTheGiaTri),
                                    '<br/> - Thu từ cọc: ', formatNumber3Digit(self.phieuThuOld.TienDatCoc),
                                    '<br/> - Sử dụng điểm: ', self.phieuThuOld.TTBangDiem, ' (', self.phieuThuOld.DiemQuyDoi, ' điểm)',
                                    '<br/> - Tài khoản POS: ', self.phieuThuOld.TenTaiKhoanPos,
                                    '<br/> - Tài khoản chuyển khoản: ', self.phieuThuOld.TenTaiKhoanCK,
                                    '<br/> - NV lập phiếu: ', self.phieuThuOld.TenNhanVien,
                                ),
                            }
                            Insert_NhatKyThaoTac_1Param(diary);

                            if (self.phieuThuOld.DiemQuyDoi > 0 || sumDiemQD > 0) {
                                let diem = self.inforCus.TongTichDiem + formatNumberToFloat(self.phieuThuOld.DiemQuyDoi) - sumDiemQD;
                                diem = diem < 0 ? 0 : diem;
                                self.UpdateDiemKH_toDB(diem);
                            }

                            if (print) {
                                self.InPhieuThu(quyhd);
                            }
                        }
                        $('#ThuTienHoaDonModal').modal('hide');
                    })
                    let gtriTinhCkOld = formatNumberToFloat(self.phieuThuOld.TienMat) + formatNumberToFloat(self.phieuThuOld.TienPOS) +
                        formatNumberToFloat(self.phieuThuOld.TienCK) + formatNumberToFloat(self.phieuThuOld.TienDatCoc);
                    let gtriTinhCkNew = formatNumberToFloat(quyhd.TienMat) + formatNumberToFloat(quyhd.TienPOS) +
                        formatNumberToFloat(quyhd.TienCK) + formatNumberToFloat(quyhd.TienDatCoc);

                    if (gtriTinhCkNew !== gtriTinhCkOld) {
                        let arrNV = self.listData.HoaDons.map(function (x) {
                            return x.BH_NhanVienThucHiens;
                        });

                        // if not agree at modal cknhanvien
                        if (!vmHoaHongHoaDon.saveOK && arrNV.length > 0) {
                            //dialogConfirm('Xác nhận cập nhật', 'Tổng tiền ' + self.sLoai + ' của phiếu <b> '
                            //    + self.phieuThuOld.MaHoaDon + ' </b> đã bị thay đổi. Bạn có muốn cập nhật lại hoa hồng nhân viên không?', function () {
                            for (let i = 0; i < self.listData.HoaDons.length; i++) {
                                let itFor = self.listData.HoaDons[i];
                                let myData = {
                                    objCT: itFor.BH_NhanVienThucHiens,
                                    idthegiatri: itFor.ID,
                                    idquyhoadon: self.phieuThuOld.ID
                                };

                                let nviens = itFor.BH_NhanVienThucHiens.map(function (xx) {
                                    return xx.TenNhanVien
                                }).toString();

                                ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/PostNhanVien_ThucHien', 'POST', myData).done(function (x) {
                                    if (x.res) {
                                        let diary = {
                                            ID_DonVi: self.inforLogin.ID_DonVi,
                                            ID_NhanVien: self.inforLogin.ID_NhanVien,
                                            LoaiNhatKy: 2,
                                            ChucNang: 'Cập nhật phiếu thu - Cập nhật chiết khấu nhân viên',
                                            NoiDung: 'Cập nhật chiết khấu nhân viên cho hóa đơn '.concat(itFor.MaHoaDon),
                                            NoiDungChiTiet: 'Cập nhật chiết khấu nhân viên cho hóa đơn '.concat(itFor.MaHoaDon,
                                                ' gồm: ', nviens, ' <br /> Mã phiếu thu ', self.phieuThuOld.MaHoaDon),
                                        }
                                        Insert_NhatKyThaoTac_1Param(diary);
                                    }
                                    else {
                                        ShowMessage_Danger(x.mes);
                                    }
                                });
                            }
                            //});
                        }
                    }
                }
            }
            else {
                self.isLoading = false;
                ShowMessage_Danger('Không có chi tiết sổ quỹ')
            }
        },

        NangNhomKhachHang: function (id) {
            var self = this;
            if (id !== null && id !== const_GuidEmpty) {
                ajaxHelper("/api/DanhMuc/DM_DoiTuongAPI/" + 'NangNhomKhachhang_byIDDoituong?idDoituong=' + id
                    + '&idChiNhanh=' + self.inforLogin.ID_DonVi, 'GET').done(function (x) {
                        if (x.res) {
                            // todo get rows success in sql
                            //ShowMessage_Success("Đã tự động cập nhật nhóm cho khách hàng " + madoituong);
                        }
                    })
            }
        },

        GetInforChiNhanh: function () {
            let self = this;
            let tenchinhanh = '', diachi = '', dienthoai = '';
            let chinhanh = $.grep(self.listData.ChiNhanhs, function (x) {
                return x.ID === self.newPhieuThu.ID_DonVi;
            });
            if (chinhanh.length > 0) {
                tenchinhanh = chinhanh[0].TenDonVi;
                diachi = chinhanh[0].DiaChi;
                dienthoai = chinhanh[0].SoDienThoai;
            }
            return {
                TenChiNhanh: tenchinhanh,
                DiaChiChiNhanh: diachi,
                DienThoaiChiNhanh: dienthoai,
            }
        },

        InPhieuThu: function (obj) {
            var quyhd = $.extend({}, obj);
            var self = this;
            var loaiCT = 'SQPT'
            if (quyhd.LoaiHoaDon === 12) {
                loaiCT = 'SQPC';
            }

            quyhd.TenCuaHang = self.inforCongTy.TenCongTy;
            quyhd.DiaChiCuaHang = self.inforCongTy.DiaChiCuaHang;
            quyhd.LogoCuaHang = self.inforCongTy.LogoCuaHang;

            let objCN = self.GetInforChiNhanh();
            quyhd.TenChiNhanh = objCN.TenChiNhanh;
            quyhd.ChiNhanhBanHang = objCN.TenChiNhanh;
            quyhd.DienThoaiChiNhanh = objCN.DienThoaiChiNhanh;
            quyhd.DiaChiChiNhanh = objCN.DiaChiChiNhanh;

            let ngaylap = quyhd.NgayLapHoaDon;
            quyhd.MaPhieu = quyhd.MaHoaDon;
            quyhd.NgayLapHoaDon = moment(ngaylap).format('DD/MM/YYYY HH:mm:ss');
            quyhd.Ngay = moment(ngaylap).format('DD');
            quyhd.Thang = moment(ngaylap).format('MM');
            quyhd.Nam = moment(ngaylap).format('YYYY');
            quyhd.NguoiNopTien = quyhd.NguoiNopTien;
            quyhd.NguoiNhanTien = quyhd.NguoiNopTien;
            quyhd.DiaChiKhachHang = '';
            quyhd.DienThoaiKhachHang = self.ddl_textVal.cusPhone;
            quyhd.NoiDungThu = quyhd.NoiDungThu;
            var tongThu = formatNumberToInt(quyhd.TongTienThu);
            quyhd.TienBangChu = DocSo(tongThu);
            quyhd.GiaTriPhieu = formatNumber3Digit(tongThu);

            quyhd.TienMat = formatNumber3Digit(obj.TienMat);
            quyhd.TienATM = formatNumber3Digit(obj.TienPOS);
            quyhd.ChuyenKhoan = formatNumber3Digit(obj.TienChuyenKhoan);
            quyhd.TienTheGiaTri = formatNumber3Digit(obj.TienTheGiaTri);
            quyhd.TTBangTienCoc = formatNumber3Digit(obj.TienCoc);
            quyhd.TienDoiDiem = formatNumber3Digit(obj.TTBangDiem);
            quyhd.KhoanMucThuChi = self.ddl_textVal.khoanthu;

            ajaxHelper('/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=' + loaiCT + '&idDonVi='
                + self.inforLogin.ID_DonVi, 'GET').done(function (result) {
                    let data = result;
                    data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                    data = data.concat("<script > var item1=[], item4=[], item5=[] ; var item2=[] ;var item3=" + JSON.stringify(quyhd) + "; </script>");
                    data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                    PrintExtraReport(data);
                })
        },

        UpdateThucThu_EachHoaDonDebit: function () {
            var self = this;

            // get thucthu each invoice
            var ptKhach = self.newPhieuThu;
            var tongno = ptKhach.TongNoHD;
            let tongtienNhap = formatNumberToFloat(ptKhach.TienMat) + formatNumberToFloat(ptKhach.TienPOS) +
                formatNumberToFloat(ptKhach.TienCK) + formatNumberToFloat(ptKhach.TienTheGiaTri);
            if (self.isThuTienThua || (self.showCheckHachToan && tongtienNhap > 0)) {
                tongno = tongtienNhap;
            }
            let money = self.shareMoney_QuyHD(tongno, formatNumberToFloat(ptKhach.TTBangDiem),
                formatNumberToFloat(ptKhach.TienMat), formatNumberToFloat(ptKhach.TienPOS),
                formatNumberToFloat(ptKhach.TienCK), formatNumberToFloat(ptKhach.TienTheGiaTri),
                formatNumberToFloat(ptKhach.TienDatCoc));

            var tiendatcoc = money.TienCoc;
            var tienmat = money.TienMat;
            var tienpos = money.TienPOS;
            var tienck = money.TienChuyenKhoan;
            var tienthe = money.TienTheGiaTri;
            var tiendiem = money.TTBangDiem;
            let ptPhiNganHang = self.newPhieuThu.PhiThanhToan_PTGiaTriTheoHoaDon;
            if (commonStatisJs.CheckNull(ptPhiNganHang)) {
                ptPhiNganHang = 0;
            }
            for (let i = 0; i < self.listData.HoaDons.length; i++) {
                let itFor = self.listData.HoaDons[i];
                let tienThucTeThu = formatNumberToFloat(itFor.TienThu);

                self.listData.HoaDons[i].PhiThanhToan_LaPhanTram = self.newPhieuThu.PhiThanhToan_LaPhanTram;
                if (tienThucTeThu > 0) {
                    let obj = self.shareMoney_QuyHD(tienThucTeThu, tiendiem, tienmat, tienpos, tienck, tienthe, tiendatcoc);
                    obj.ID_HoaDonLienQuan = itFor.ID;
                    obj.MaHoaDon = itFor.MaHoaDon;
                    obj.TienThu = tienThucTeThu;
                    self.QuyHD_Share.push(obj);
                    self.listData.HoaDons[i].TongPhiNganHang = obj.TienPOS * ptPhiNganHang / 100;
                    self.listData.HoaDons[i].ThucThu = obj.TienMat + obj.TienPOS + obj.TienChuyenKhoan + obj.TienCoc;

                    // tinh lai cho tung hoadon
                    tiendatcoc = tiendatcoc - obj.TienCoc;
                    tienmat = tienmat - obj.TienMat;
                    tienpos = tienpos - obj.TienPOS;
                    tienck = tienck - obj.TienChuyenKhoan;
                    tienthe = tienthe - obj.TienTheGiaTri;
                    tiendiem = tiendiem - obj.TTBangDiem;
                }
                else {
                    self.listData.HoaDons[i].TongPhiNganHang = 0;
                    self.listData.HoaDons[i].ThucThu = 0;
                }
                // tinh ThucThu tung HoaDon (khong tinh thu tu TheGiaTri)
                self.UpdateChietKhauNV_ifChangeThucThu(itFor.ID, self.listData.HoaDons[i].ThucThu);
            }
            return {
                TienCoc: money.TienCoc,
                TTBangDiem: money.TTBangDiem,
                TienMat: money.TienMat,
                TienPOS: money.TienPOS,
                TienChuyenKhoan: money.TienChuyenKhoan,
                TienTheGiaTri: money.TienTheGiaTri,
            }
        },

        // hoahong nv hoadon
        showModalDiscount: function (item) {
            var self = this;
            self.GetChiPhi_Visa();// used to get %ChiPhi of phieuthu
            self.UpdateThucThu_EachHoaDonDebit();

            if (self.typeUpdate === 1) {
                let tongCP = 0;
                for (let i = 0; i < self.listData.HoaDons.length; i++) {
                    let itFor = self.listData.HoaDons[i];
                    if (itFor.ID === item.ID) {
                        tongCP = itFor.TongPhiNganHang;
                        break;
                    }
                }
                item.PhiThanhToan_LaPhanTram = self.newPhieuThu.PhiThanhToan_LaPhanTram;
                item.TongPhiNganHang = tongCP;
                vmHoaHongHoaDon.GetChietKhauHoaDon_byID(item, self.newPhieuThu);
            }
            else {
                for (let i = 0; i < self.listData.HoaDons.length; i++) {
                    let itFor = self.listData.HoaDons[i];
                    if (itFor.ID === item.ID) {
                        let obj = {
                            ID: itFor.ID,
                            MaHoaDon: itFor.MaHoaDon,
                            LoaiHoaDon: itFor.LoaiHoaDon,
                            TongThanhToan: itFor.TongThanhToan,
                            TongTienThue: itFor.TongTienThue,
                            ThucThu: itFor.ThucThu,
                            TongPhiNganHang: itFor.TongPhiNganHang,
                            PhiThanhToan_LaPhanTram: itFor.PhiThanhToan_LaPhanTram,
                            DaThuTruoc: itFor.DaThuTruoc,
                            ConNo: itFor.PhaiThu - itFor.DaThuTruoc - formatNumberToFloat(itFor.TienThu),
                        }
                        vmHoaHongHoaDon.GridNVienBanGoi_Chosed = itFor.BH_NhanVienThucHiens;
                        vmHoaHongHoaDon.showModal(obj);
                        break;
                    }
                }
            }
        },

        HuyPhieu: function () {
            let self = this;
            let khoaSo = self.CheckKhoaSo(moment(self.phieuThuOld.NgayLapHoaDon).format('YYYY-MM-DD'), self.phieuThuOld.ID_DonVi);
            if (khoaSo) {
                ShowMessage_Danger('Chi nhánh đã được chốt sổ ngày ' + moment(self.NgayChotSo).format('DD/MM/YYYY') + '. Không thể hủy');
                return;
            }

            dialogConfirm('Xác nhận xóa', 'Bạn có chắc chắn muốn hủy phiếu ' + self.sLoai + ' <b> ' + self.newPhieuThu.MaHoaDon + ' </b> không?', function () {

                ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/' + "DeleteQuy_HoaDon/" + self.newPhieuThu.ID, 'DELETE').done(function (x) {
                    if (x === "") {
                        ShowMessage_Success("Xóa sổ quỹ thành công");
                        $('#ThuTienHoaDonModal').modal('hide');

                        let diary = {
                            ID_NhanVien: self.inforLogin.ID_NhanVien,
                            ID_DonVi: self.inforLogin.ID_DonVi,
                            ChucNang: 'Phiếu ' + self.sLoai,
                            NoiDung: 'Hủy phiếu '.concat(self.sLoai, ' ', self.newPhieuThu.MaHoaDon, ', nhân viên hủy: ', self.inforLogin.UserLogin),
                            NoiDungChiTiet: ''.concat(self.sForm, ': Hủy phiếu ', self.sLoai, ' ', self.newPhieuThu.MaHoaDon, ', nhân viên hủy: ', self.inforLogin.UserLogin,
                                '<br /><b> Thông tin cũ: </b>',
                                '<br /> - Giá trị: ', formatNumber3Digit(self.phieuThuOld.TongTienThu),
                                '<br /> - Phương thức thanh toán: ', self.phieuThuOld.PhuongThucTT,
                                '<br /> - Khoản ', self.sLoai, ': ', self.ddl_textVal.khoanthu,
                                '<br /> - Người nộp: ', self.phieuThuOld.NguoiNopTien, ' (', self.phieuThuOld.MaNguoiNop, ')'),
                            LoaiNhatKy: 3
                        }
                        Insert_NhatKyThaoTac_1Param(diary);
                        // todo nangnhom khach
                    }
                    else {
                        ShowMessage_Danger("Giá trị thẻ nạp đã được sử dụng không thể xóa phiếu này");
                    }
                });
            });
        },
        KhoiPhuc: function () {
            let self = this;

            dialogConfirm('Khôi phục', 'Bạn có chắc chắn muốn khôi phục phiếu ' + self.sLoai + ' <b> ' + self.newPhieuThu.MaHoaDon + ' </b> không?', function () {

                ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/' + "KhoiPhucQuy_HoaDon/" + self.newPhieuThu.ID, 'GET').done(function (x) {
                    if (x.res) {
                        ShowMessage_Success("Khôi phục sổ quỹ thành công");
                        $('#ThuTienHoaDonModal').modal('hide');

                        let diary = {
                            ID_NhanVien: self.inforLogin.ID_NhanVien,
                            ID_DonVi: self.inforLogin.ID_DonVi,
                            ChucNang: 'Khôi phục phiếu ' + self.sLoai,
                            NoiDung: 'Khôi phục phiếu '.concat(self.sLoai, ' ', self.newPhieuThu.MaHoaDon, ', Người khôi phục: ', self.inforLogin.UserLogin),
                            NoiDungChiTiet: ''.concat(self.sForm, ': Khôi phục phiếu ', self.sLoai, ' ', self.newPhieuThu.MaHoaDon, ', Người khôi phục: ', self.inforLogin.UserLogin),
                            LoaiNhatKy: 2
                        }
                        Insert_NhatKyThaoTac_1Param(diary);

                        if (self.listData.HoaDons.length > 0) {
                            let lstIDDoituong = $.unique(self.listData.HoaDons.map(function (x) {
                                return x.ID_DoiTuong;
                            }));
                            for (let i = 0; i < lstIDDoituong.length; i++) {
                                self.NangNhomKhachHang(lstIDDoituong[i]);
                            }
                        }
                    }
                    else {
                        ShowMessage_Danger(x.mess);
                    }
                });
            });
        },
        showModalUpdate: function (id, nohientai = 0, formType = 0) {
            let self = this;
            self.typeUpdate = 1;
            self.LoaiHoaDon = 0;// reset LoaiHoaDon vì chưa biết loại gì
            self.saveOK = false;
            self.formType = formType;
            self.isThuTienThua = false;
            self.KhongBuTruCongNo = false;
            self.showCheckHachToan = false;
            self.GetSoQuy_andHoaDonLienQuan(id, nohientai);
        },
        GetSoQuy_andHoaDonLienQuan: function (idQuyHD, nohientai = 0) {
            let self = this;
            ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/' + 'GetQuyChiTiet_byIDQuy/' + idQuyHD, 'GET').done(function (x) {
                if (x.res) {

                    let sumMat = 0, sumCK = 0, sumPOS = 0, sumCoc = 0, sumTGT = 0, sumDiem = 0, quyDoiDiem = 0, sumNoHD = 0;
                    let idChuyenKhoan = null, idPos = null, tenTKPos = '', tenTKCK = '';
                    let tenNganHangPOS = '', tenNganHangCK = '';

                    self.listData.HoaDons = [];
                    let arrIDHD = [];
                    if (x.dataSoure.length > 0) {
                        let firstRow = x.dataSoure[0];
                        let thuKhongHD = x.dataSoure.filter(x => x.ID_HoaDonLienQuan !== null).length === 0;

                        if (thuKhongHD) {
                            if (self.formType === 1) {// chi show at DS khachhang (because thu khong lienquan hoadon)
                                vmThemPhieuThuChi.showModalUpdate(firstRow, x.dataSoure);
                            }
                        }
                        else {
                            let arrPOS = [], arrCK = [];
                            let arrIDPOS = [], arrIDCK = [];
                            for (let i = 0; i < x.dataSoure.length; i++) {
                                let itFor = x.dataSoure[i];
                                if ($.inArray(itFor.ID_HoaDonLienQuan, arrIDHD) === -1) {
                                    arrIDHD.push(itFor.ID_HoaDonLienQuan);

                                    let arrCT = $.grep(x.dataSoure, function (x) {
                                        return x.ID_HoaDonLienQuan === itFor.ID_HoaDonLienQuan;
                                    });

                                    let sumCT = arrCT.reduce(function (_this, xx) {
                                        return _this + xx.TienThu;
                                    }, 0);
                                    let sumPOS = arrCT.filter(x => x.HinhThucThanhToan === 2).reduce(function (_this, xx) {
                                        return _this + xx.TienThu;
                                    }, 0);

                                    let hd = {
                                        ID: itFor.ID_HoaDonLienQuan,
                                        MaHoaDon: itFor.MaHoaDonHD,
                                        NgayLapHoaDon: itFor.NgayLapHoaDon,
                                        ID: itFor.ID_HoaDonLienQuan,
                                        LoaiHoaDon: itFor.LoaiHoaDonHD,
                                        PhaiThu: itFor.TongThanhToanHD,
                                        TongTienThue: itFor.TongTienThue,
                                        TongThanhToan: itFor.TongThanhToanHD,
                                        DaThuTruoc: itFor.DaThuTruoc,
                                        CanThu: Math.abs(itFor.TongThanhToanHD - itFor.DaThuTruoc),// neu khach thanh toan khi dathang > gtri hoadon
                                        TienThu: formatNumber3Digit(sumCT),
                                        TienPOS: sumPOS,
                                        BH_NhanVienThucHiens: [],
                                    }
                                    sumNoHD += hd.CanThu;
                                    self.listData.HoaDons.push(hd);

                                    self.LoaiHoaDon = itFor.LoaiHoaDonHD;
                                }

                                switch (itFor.HinhThucThanhToan) {
                                    case 1:
                                        sumMat += itFor.TienThu;
                                        break;
                                    case 2:
                                        sumPOS += itFor.TienThu;
                                        idPos = itFor.ID_TaiKhoanNganHang;

                                        if (!commonStatisJs.CheckNull(idPos)) {
                                            if ($.inArray(idPos, arrIDPOS) === -1) {
                                                arrIDPOS.push(idPos);
                                                tenTKPos = itFor.TenTaiKhoanPOS;
                                                tenNganHangPOS = itFor.TenNganHang;

                                                arrPOS.push({
                                                    TienPOS: formatNumber3Digit(itFor.TienThu),
                                                    ID_TaiKhoanPos: itFor.ID_TaiKhoanNganHang,
                                                    TenTaiKhoanPos: itFor.TenTaiKhoanPOS,
                                                    SoTaiKhoanPos: '',
                                                    TenNganHangPos: itFor.TenNganHang,
                                                    ChiPhiThanhToan: itFor.ChiPhiThanhToan,
                                                    TheoPhanTram: itFor.TheoPhanTram,
                                                    ThuPhiThanhToan: itFor.ThuPhiThanhToan,
                                                    MacDinh: itFor.MacDinh,
                                                });
                                            }
                                            else {
                                                // sum TienThu if exists
                                                for (let k = 0; k < arrPOS.length; k++) {
                                                    if (arrPOS[k].ID_TaiKhoanPos === idPos) {
                                                        arrPOS[k].TienPOS = formatNumber3Digit(formatNumberToFloat(arrPOS[k].TienPOS) + itFor.TienThu);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 3:
                                        sumCK += itFor.TienThu;
                                        idChuyenKhoan = itFor.ID_TaiKhoanNganHang;

                                        if (!commonStatisJs.CheckNull(idChuyenKhoan)) {
                                            if ($.inArray(idChuyenKhoan, arrIDCK) === -1) {
                                                arrIDCK.push(idChuyenKhoan);
                                                tenTKCK = itFor.TenTaiKhoanNOTPOS;
                                                tenNganHangCK = itFor.TenNganHang;

                                                arrCK.push({
                                                    TienCK: formatNumber3Digit(itFor.TienThu),
                                                    ID_TaiKhoanChuyenKhoan: itFor.ID_TaiKhoanNganHang,
                                                    TenTaiKhoanCK: itFor.TenTaiKhoanNOTPOS,
                                                    SoTaiKhoanCK: '',
                                                    TenNganHangCK: itFor.TenNganHang,
                                                    ChiPhiThanhToan: itFor.ChiPhiThanhToan,
                                                    TheoPhanTram: itFor.TheoPhanTram,
                                                    ThuPhiThanhToan: itFor.ThuPhiThanhToan,
                                                    MacDinh: itFor.MacDinh,
                                                });
                                            }
                                            else {
                                                // sum TienThu if exists
                                                for (let k = 0; k < arrIDCK.length; k++) {
                                                    if (arrIDCK[k].ID_TaiKhoanChuyenKhoan === idChuyenKhoan) {
                                                        arrIDCK[k].TienCK = formatNumber3Digit(formatNumberToFloat(arrIDCK[k].TienCK) + itFor.TienThu);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 4:
                                        sumTGT += itFor.TienThu;
                                        break;
                                    case 5:
                                        sumDiem += itFor.DiemThanhToan;
                                        quyDoiDiem += itFor.TienThu;
                                        break;
                                    case 6:
                                        sumCoc += itFor.TienThu;
                                        break;
                                }
                            }

                            let noHD = nohientai;
                            if (self.formType !== 1) {// if not at DC NCC
                                noHD = sumNoHD;
                            }

                            self.newPhieuThu = {
                                ID: firstRow.ID,
                                ID_DonVi: firstRow.ID_DonVi,
                                LoaiHoaDon: firstRow.LoaiHoaDon,
                                MaHoaDon: firstRow.MaHoaDon,
                                NgayLapHoaDon: firstRow.NgayLapPhieuThu,
                                TienDatCoc: formatNumber3Digit(sumCoc),
                                TienMat: formatNumber3Digit(sumMat),
                                TienPOS: formatNumber3Digit(sumPOS),
                                TienCK: formatNumber3Digit(sumCK),
                                TienTheGiaTri: formatNumber3Digit(sumTGT),
                                TTBangDiem: formatNumber3Digit(quyDoiDiem),
                                DiemQuyDoi: formatNumber3Digit(sumDiem),
                                TongTienThu: firstRow.TongTienThu,
                                DaThanhToan: firstRow.TongTienThu,
                                ID_TaiKhoanPos: idPos,
                                ID_TaiKhoanChuyenKhoan: idChuyenKhoan,
                                ID_KhoanThuChi: firstRow.ID_KhoanThuChi,
                                ID_DoiTuong: firstRow.ID_DoiTuong,
                                ID_NhanVien: firstRow.ID_NhanVien,
                                TongNoHD: sumNoHD,
                                NoHienTai: noHD,
                                TienThua: 0,
                                ThucThu: 0,
                                NoiDungThu: firstRow.NoiDungThu,
                                HachToanKinhDoanh: true,
                                LoaiDoiTuong: firstRow.LoaiDoiTuong,
                                TrangThai: firstRow.TrangThai,

                                TongPhiThanhToan: 0,
                                PhiThanhToan_PTGiaTri: 0,
                                PhiThanhToan_LaPhanTram: true,

                                ListTKPos: [],
                                ListTKChuyenKhoan: [],
                            };

                            if (arrPOS.length > 0) {
                                self.newPhieuThu.ListTKPos = arrPOS;
                            }
                            else {
                                self.newPhieuThu.ListTKPos.push(self.newPhuongThucTT(true));
                            }
                            if (arrCK.length > 0) {
                                self.newPhieuThu.ListTKChuyenKhoan = arrCK;
                            }
                            else {
                                self.newPhieuThu.ListTKChuyenKhoan.push(self.newPhuongThucTT(false));
                            }

                            let pthuc = '';
                            if (sumMat > 0) {
                                pthuc = 'Tiền mặt,';
                            }
                            if (sumPOS > 0) {
                                pthuc += 'POS,';
                            }
                            if (sumCK > 0) {
                                pthuc += 'Chuyển khoản,';
                            }
                            if (sumCoc > 0) {
                                pthuc += 'Thu từ cọc,';
                            }
                            if (sumTGT > 0) {
                                pthuc += 'Thẻ giá trị,';
                            }
                            if (sumDiem > 0) {
                                pthuc += 'Sử dụng điểm,';
                            }
                            pthuc = Remove_LastComma(pthuc);

                            self.phieuThuOld = $.extend({}, self.newPhieuThu);
                            self.phieuThuOld.TenTaiKhoanPos = tenTKPos;
                            self.phieuThuOld.TenTaiKhoanCK = tenTKCK;
                            self.phieuThuOld.LoaiDoiTuong = firstRow.LoaiDoiTuong;
                            self.phieuThuOld.NguoiNopTien = firstRow.NguoiNopTien;
                            self.phieuThuOld.MaNguoiNop = firstRow.MaDoiTuong;
                            self.phieuThuOld.TenKhoanThuChi = firstRow.NoiDungThuChi;
                            self.phieuThuOld.TenNhanVien = firstRow.TenNhanVien;
                            self.phieuThuOld.PhuongThucTT = pthuc;
                            self.phieuThuOld.TenNganHangPOS = tenNganHangPOS;
                            self.phieuThuOld.TenNganHangCK = tenNganHangCK;

                            self.ddl_textVal.cusName = firstRow.NguoiNopTien;
                            self.ddl_textVal.cusPhone = firstRow.SoDienThoai;
                            self.ddl_textVal.staffName = firstRow.TenNhanVien;
                            self.ddl_textVal.khoanthu = firstRow.NoiDungThuChi;
                            self.ddl_textVal.accountPOSName = tenTKPos;
                            self.ddl_textVal.accountCKName = tenTKCK;
                            self.HoaDonChosing.DienThoaiKhachHang = firstRow.SoDienThoai;
                            self.HoaDonChosing.SoDuTheGiaTri += sumTGT;

                            self.isKhoaSo = self.CheckKhoaSo(moment(self.phieuThuOld.NgayLapHoaDon).format('YYYY-MM-DD'), self.phieuThuOld.ID_DonVi);

                            self.GetSoDuTheGiaTri(firstRow.ID_DoiTuong);
                            self.GetSoDuDatCoc(firstRow.ID_DoiTuong);
                            self.GetInforBasic_Cus();

                            // get lst NV_ThucHienHoaDon of all hoadon
                            let arrIDHoaDon = self.listData.HoaDons.map(function (x) {
                                return x.ID;
                            });
                            let param = {
                                IDHoaDons: arrIDHoaDon,
                                idQuyHD: self.phieuThuOld.ID,
                            }
                            self.GetListNVChietKhau_ManyHoaDon(param);

                            $('#ThuTienHoaDonModal').modal('show');
                        }
                    }
                    else {
                        self.ResetAccountCK();
                        self.ResetAccountPOS();
                        self.ResetKhoanThu();
                    }
                    self.HinhThucTT = { ID: 0, Text: 'Tất cả' };
                }
            })
        }
    },
})

$(function () {
    $('#HoaHongNhanVienHD').on('hidden.bs.modal', function () {
        if (vmHoaHongHoaDon.saveOK) {
            if (vmHoaHongHoaDon.isNew) {
                for (let i = 0; i < vmThanhToan.listData.HoaDons.length; i++) {
                    let hd = vmThanhToan.listData.HoaDons[i];
                    if (hd.ID === vmHoaHongHoaDon.inforHoaDon.ID) {
                        vmThanhToan.listData.HoaDons[i].BH_NhanVienThucHiens = vmHoaHongHoaDon.GridNVienBanGoi_Chosed;
                        break;
                    }
                }
            }
        }
    })
})

