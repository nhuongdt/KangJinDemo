var vmThanhToanNCC = new Vue({
    el: '#vmThanhToanNCC',
    components: {
        'account-bank': cmpChoseAccountBank,
        'my-date-time': cpmDatetime,
        'nhanviens': ComponentChoseStaff,
        'khoan-thu-chi': cmpChoseKhoanThu,
    },
    created: function () {
        let self = this;
        self.GuidEmpty = '00000000-0000-0000-0000-000000000000';
        self.inforLogin = {
            ID_NhanVien: VHeader.IdNhanVien,
            ID_User: VHeader.IdNguoiDung,
            UserLogin: VHeader.UserLogin,
            ID_DonVi: VHeader.IdDonVi,
            TenNhanVien: VHeader.TenNhanVien,
        };
        console.log('thutien nhap', self.inforLogin)
        self.isKhoaSo = false;

        self.role.SoQuy.Insert = VHeader.Quyen.indexOf('SoQuy_ThemMoi') > -1;
        self.role.SoQuy.Update = VHeader.Quyen.indexOf('SoQuy_CapNhat') > -1;
        self.role.SoQuy.Delete = VHeader.Quyen.indexOf('SoQuy_Xoa') > -1;
        self.role.SoQuy.ChangeNgayLap = VHeader.Quyen.indexOf('SoQuy_ThayDoiThoiGian') > -1;
    },
    computed: {
        sLoaiThuChi: function () {
            return this.newPhieuThu.LoaiHoaDon == 11 ? 'thu' : 'chi';
        },
    },
    data: {
        saveOK: false,
        isNew: true,
        isCheckTraLaiCoc: false,
        isKhoaSo: false,

        SoDuDatCoc: 0,
        formType: 0, //0.DS hoadon (nhap/tra), 1.DS NCC
        LoaiHoaDon: 4,
        HinhThucTT: { ID: 0, Text: 'Tất cả' },
        ddl_textVal: {
            staffName: '',
            accountPOSName: '',
            accountCKName: '',
            cusName: '',
            cusPhone: '',
            khoanthu: '',
            TenNganHangPos: '',
            TenNganHangCK: '',
        },
        role: {
            SoQuy: {},
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
        newPhieuThu: {
            MaHoaDon: '',
            NgayLapHoaDon: moment(new Date()).format('YYYY-MM-DD HH:mm'),
            TienDatCoc: 0,
            TienMat: 0,
            TienPOS: 0,
            TienCK: 0,
            TienTheGiaTri: 0,
            TTBangDiem: 0,
            ID_TaiKhoanPos: null,
            ID_TaiKhoanChuyenKhoan: null,
            ID_NhanVien: null,
            ID_DonVi: null,
            ID_KhoanThuChi: null,
            ID_DoiTuong: null,// nguoinop
            NoHienTai: 0, // nohientai of ncc
            TongNoHD: 0,
            DaThanhToan: 0,
            TienThua: 0,
            ThucThu: 0,
            NoiDungThu: '',
        },
        listData: {
            AccountBanks: [],
            NhanViens: [],
            KhoanThuChis: [],
            NguoiNops: [],// ncc/ kh
            HoaDons: [],
            arrIDDonVi: [],
            ChiNhanhs: [],// used to get infor chinhanh when print
        },
        QuyHD_Share: [],
        HoaDonChosing: {},
        phieuThuOld: {},
    },
    methods: {
        GetListHD_isDebitOfKH: function (idDoiTuong) {
            var self = this;
            let arrIDDV = self.listData.arrIDDonVi;
            ajaxHelper('/api/DanhMuc/DM_DoiTuongAPI/' + 'GetListHD_isDebit?idDoiTuong=' + idDoiTuong
                + '&arrDonVi=' + arrIDDV + '&loaiDoiTuong=' + 2,
                'POST', arrIDDV).done(function (x) {
                    if (x.res) {
                        var data = x.dataSoure;
                        // chi thanh toan PhiDV ngoai (NCC)
                        if (self.formType === 2) {
                            data = data.filter(x => x.LoaiHoaDon === 25);
                        }
                        var tongnoHD = 0;
                        for (let i = 0; i < data.length; i++) {
                            let itFor = data[i];
                            data[i].TienThu = 0;
                            data[i].BH_NhanVienThucHiens = [];
                            data[i].CanThu = itFor.TienMat;
                            data[i].PhaiThu = itFor.PhaiThanhToan;
                            data[i].DaThuTruoc = itFor.KhachDaTra;
                            tongnoHD += itFor.TienMat;
                        }
                        self.listData.HoaDons = data;

                        self.newPhieuThu.NoHienTai = tongnoHD;
                        self.newPhieuThu.TongNoHD = tongnoHD;
                        self.HoaDonChosing.PhaiThanhToan = tongnoHD;
                        self.GetSoDuDatCoc(idDoiTuong);

                        let loaiHD = 4;
                        if (data.length.length > 0) {
                            loaiHD = data[0].LoaiHoaDon;
                        }
                        self.LoaiHoaDon = loaiHD;
                    }
                });
        },

        GetSoDuDatCoc: function (idDoiTuong) {
            var self = this;
            var soduDatCoc = 0;
            var nohientai = self.newPhieuThu.NoHienTai;
            ajaxHelper('/api/DanhMuc/DM_DoiTuongAPI/' + "GetTienDatCoc_ofDoiTuong?idDoiTuong=" + idDoiTuong
                + '&idDonVi=' + self.inforLogin.ID_DonVi, 'GET').done(function (x) {
                    if (x.res && x.dataSoure.length > 0) {
                        soduDatCoc = x.dataSoure[0].SoDuTheGiaTri;
                    }
                    soduDatCoc = soduDatCoc < 0 ? 0 : soduDatCoc;
                    self.SoDuDatCoc = soduDatCoc;
                    if (!self.isNew) {// capnhat phieuchi
                        self.SoDuDatCoc += formatNumberToFloat(self.newPhieuThu.TienDatCoc);
                    }
                    else {
                        let tiendatcoc = 0, tienmat = 0;
                        if (soduDatCoc > 0 && self.HoaDonChosing.LoaiHoaDon === 4) {
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

                        self.newPhieuThu.TienDatCoc = formatNumber(tiendatcoc);
                        self.newPhieuThu.TienMat = formatNumber3Digit(tienmat);
                        self.newPhieuThu.DaThanhToan = self.newPhieuThu.NoHienTai;
                        self.newPhieuThu.ThucThu = tienmat;
                        if (self.formType !== 0) {
                            self.AssignMoney_InHoaDonDebit();
                        }
                    }
                });
        },

        GetKhoanThuChi_byLoaiChungTu: function (lakhoanthu = false) {
            let self = this;
            let loaiHD = self.LoaiHoaDon;
            if (commonStatisJs.CheckNull(loaiHD)) {
                loaiHD = 4;
            }
            let ktc = $.grep(self.listData.KhoanThuChis, function (x) {
                return x.LoaiChungTu.indexOf(loaiHD) > -1 && x.LaKhoanThu === lakhoanthu;
            });
            return ktc;
        },

        showListNguoiNop: function () {
            $(event.currentTarget).next().show();
        },
        showModalUpdate: function (id, nohientai = 0, formType = 0) {
            let self = this;
            self.formType = formType;
            self.isNew = false;
            self.isKhoaSo = false;
            self.GetSoQuy_andHoaDonLienQuan(id, nohientai);
        },
        GetSoQuy_andHoaDonLienQuan: function (idQuyHD, nohientai = 0) {
            let self = this;
            ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/' + 'GetQuyChiTiet_byIDQuy/' + idQuyHD, 'GET').done(function (x) {
                if (x.res) {
                    let sumMat = 0, sumCK = 0, sumPOS = 0, sumCoc = 0, sumTGT = 0, sumDiem = 0, sumNoHD = 0;
                    let idChuyenKhoan = null, idPos = null, tenTKPos = '', tenTKCK = '';

                    self.listData.HoaDons = [];
                    let arrIDHD = [];
                    if (x.dataSoure.length > 0) {
                        let firstRow = x.dataSoure[0];
                        if (commonStatisJs.CheckNull(firstRow.ID_HoaDonLienQuan)) {
                            vmThemPhieuThuChi.showModalUpdate(firstRow, x.dataSoure);
                        }
                        else {
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

                                    let hd = {
                                        ID: itFor.ID_HoaDonLienQuan,
                                        MaHoaDon: itFor.MaHoaDonHD,
                                        NgayLapHoaDon: itFor.NgayLapHoaDon,
                                        ID: itFor.ID_HoaDonLienQuan,
                                        LoaiHoaDon: itFor.LoaiHoaDonHD,
                                        PhaiThu: itFor.TongThanhToanHD,
                                        TongTienThue: itFor.TongTienThue,
                                        PhaiThanhToan: itFor.TongThanhToanHD,
                                        KhachDaTra: itFor.DaThuTruoc,
                                        CanThu: Math.abs(itFor.TongThanhToanHD - itFor.DaThuTruoc),// neu khach thanh toan khi dathang > gtri hoadon
                                        TienThu: formatNumber3Digit(sumCT),
                                        BH_NhanVienThucHiens: [],
                                    }
                                    sumNoHD += hd.CanThu;
                                    self.listData.HoaDons.push(hd);
                                }

                                switch (itFor.HinhThucThanhToan) {
                                    case 1:
                                        sumMat += itFor.TienThu;
                                        break;
                                    case 2:
                                        sumPOS += itFor.TienThu;
                                        if (commonStatisJs.CheckNull(idPos)) {
                                            idPos = itFor.ID_TaiKhoanNganHang;
                                            tenTKPos = itFor.TenTaiKhoanPOS;
                                        }
                                        break;
                                    case 3:
                                        sumCK += itFor.TienThu;
                                        if (commonStatisJs.CheckNull(idChuyenKhoan)) {
                                            idChuyenKhoan = itFor.ID_TaiKhoanNganHang;
                                            tenTKCK = itFor.TenTaiKhoanNOTPOS;
                                        }
                                        break;
                                    case 4:
                                        sumTGT += itFor.TienThu;
                                        break;
                                    case 5:
                                        sumDiem += itFor.TienThu;
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
                                LoaiHoaDon: firstRow.LoaiHoaDon,
                                MaHoaDon: firstRow.MaHoaDon,
                                NgayLapHoaDon: firstRow.NgayLapPhieuThu,
                                TienDatCoc: formatNumber3Digit(sumCoc),
                                TienMat: formatNumber3Digit(sumMat),
                                TienPOS: formatNumber3Digit(sumPOS),
                                TienCK: formatNumber3Digit(sumCK),
                                TienTheGiaTri: formatNumber3Digit(sumTGT),
                                TTBangDiem: formatNumber3Digit(sumDiem),
                                TongTienThu: firstRow.TongTienThu,
                                DaThanhToan: firstRow.TongTienThu,
                                ID_TaiKhoanPos: idPos,
                                ID_TaiKhoanChuyenKhoan: idChuyenKhoan,
                                ID_KhoanThuChi: firstRow.ID_KhoanThuChi,
                                ID_DoiTuong: firstRow.ID_DoiTuong,
                                ID_NhanVien: firstRow.ID_NhanVien,
                                ID_DonVi: firstRow.ID_DonVi,
                                TongNoHD: sumNoHD,
                                NoHienTai: noHD,
                                TienThua: 0,
                                ThucThu: 0,
                                NoiDungThu: firstRow.NoiDungThu,
                                HachToanKinhDoanh: true,
                                LoaiDoiTuong: firstRow.LoaiDoiTuong,
                                TrangThai: firstRow.TrangThai
                            };

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

                            self.ddl_textVal.cusName = firstRow.NguoiNopTien;
                            self.ddl_textVal.cusPhone = firstRow.SoDienThoai;
                            self.ddl_textVal.staffName = firstRow.TenNhanVien;
                            self.ddl_textVal.khoanthu = firstRow.NoiDungThuChi;
                            self.ddl_textVal.accountPOSName = tenTKPos;
                            self.ddl_textVal.accountCKName = tenTKCK;
                            self.HoaDonChosing.DienThoaiKhachHang = firstRow.SoDienThoai;

                            self.isKhoaSo = VHeader.CheckKhoaSo(moment(self.phieuThuOld.NgayLapHoaDon).format('YYYY-MM-DD'), self.newPhieuThu.ID_DonVi);

                            self.GetSoDuDatCoc(firstRow.ID_DoiTuong);
                            $('#vmThanhToanNCC').modal('show');
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
        },
        showModalThanhToan: function (item, formType = 0) {
            var self = this;
            if (commonStatisJs.CheckNull(item.KhachDaTra)) {
                item.KhachDaTra = 0;
            }
            self.HoaDonChosing = item;
            self.LoaiHoaDon = item.LoaiHoaDon;
            self.formType = formType;
            self.isNew = true;
            self.isKhoaSo = false;
            self.ResetHinhThucTT();
            self.listData.HoaDons = [];

            // reset infor phieuthu
            self.ddl_textVal = {
                staffName: self.inforLogin.TenNhanVien,
                accountPOSName: '',
                accountCKName: '',
                khoanthu: '',
                cusName: '',
                cusPhone: '',
            };

            self.newPhieuThu = {
                MaHoaDon: '',
                NgayLapHoaDon: moment(new Date()).format('YYYY-MM-DD HH:mm'),
                TienDatCoc: 0,
                TienMat: 0,
                TienPOS: 0,
                TienCK: 0,
                TienTheGiaTri: 0,
                TTBangDiem: 0,
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
                LoaiHoaDon: self.LoaiHoaDon === 7 ? 11 : 12,// trahangnhap: phieuthu
                LoaiDoiTuong: 2,
            };

            let ktc = self.GetKhoanThuChi_byLoaiChungTu(self.LoaiHoaDon === 7);
            if (ktc.length > 0) {
                self.newPhieuThu.ID_KhoanThuChi = ktc[0].ID;
                self.ddl_textVal.khoanthu = ktc[0].NoiDungThuChi;
            }

            switch (formType) {
                case 0:// at DS nhaphang/trahangnhap
                    var nguoinop = [];
                    var invoice = [];
                    var khachCanTra = item.PhaiThanhToan - item.KhachDaTra;

                    if (khachCanTra > 0) {
                        // get thong tin khachhang
                        invoice = [{
                            ID: item.ID,
                            LoaiHoaDon: item.LoaiHoaDon,
                            MaHoaDon: item.MaHoaDon,
                            NgayLapHoaDon: item.NgayLapHoaDon,
                            PhaiThanhToan: item.PhaiThanhToan,
                            KhachDaTra: item.KhachDaTra,
                            CanThu: khachCanTra,
                            TienThu: formatNumber(khachCanTra),
                        }];

                        if (item.ID_DoiTuong !== null) {
                            let cus = {
                                ID: item.ID_DoiTuong,
                                MaNguoiNop: item.MaDoiTuong,
                                TenNguoiNop: item.TenDoiTuong,
                                DienThoaiKhachHang: item.DienThoai
                            };
                            nguoinop.push(cus);
                        }

                        self.newPhieuThu.NoHienTai = khachCanTra;
                        self.newPhieuThu.TongNoHD = khachCanTra;
                        self.ddl_textVal.cusName = item.TenDoiTuong;
                        self.ddl_textVal.cusPhone = item.DienThoai;
                        self.newPhieuThu.ID_DoiTuong = item.ID_DoiTuong;
                    }

                    self.listData.NguoiNops = nguoinop;
                    self.listData.HoaDons = invoice;
                    self.GetSoDuDatCoc(nguoinop[0].ID);
                    break;
                case 1: // at DS NCC
                case 2:// DS NCC: chi lay chiphi DV
                    var idCus = item.ID;
                    let cus = {
                        ID: idCus,
                        MaNguoiNop: item.MaDoiTuong,
                        TenNguoiNop: item.TenDoiTuong,
                        DienThoaiKhachHang: item.DienThoai,
                    };
                    self.listData.NguoiNops = [cus];
                    self.ddl_textVal.cusName = item.TenDoiTuong;
                    self.ddl_textVal.cusPhone = item.DienThoai;
                    self.newPhieuThu.ID_DoiTuong = idCus;
                    self.HoaDonChosing.DienThoaiKhachHang = item.DienThoai;

                    self.GetListHD_isDebitOfKH(idCus);
                    break;
                default:
                    break;
            }
            $('#vmThanhToanNCC').modal('show');
        },
        ChangeCreator: function (item) {
            var self = this;
            self.ddl_textVal.staffName = item.TenNhanVien;
            self.newPhieuThu.ID_NhanVien = item.ID;
        },
        CaculatorDaThanhToan: function (isChangeTienThu = false) {
            var self = this;
            self.newPhieuThu.DaThanhToan = formatNumberToFloat(self.newPhieuThu.TienMat)
                + formatNumberToFloat(self.newPhieuThu.TienPOS)
                + formatNumberToFloat(self.newPhieuThu.TienCK)
                + formatNumberToFloat(self.newPhieuThu.TienTheGiaTri)
                + formatNumberToFloat(self.newPhieuThu.TienDatCoc)
                + formatNumberToFloat(self.newPhieuThu.TTBangDiem);
            self.newPhieuThu.ThucThu = self.newPhieuThu.DaThanhToan
                - formatNumberToFloat(self.newPhieuThu.TTBangDiem)
                - formatNumberToFloat(self.newPhieuThu.TienDatCoc)
                - formatNumberToFloat(self.newPhieuThu.TienTheGiaTri);
            self.newPhieuThu.TienThua = self.newPhieuThu.DaThanhToan - self.newPhieuThu.TongNoHD;
            self.AssignMoney_InHoaDonDebit(isChangeTienThu);
        },

        Only_ResetAccountPOS: function () {
            var self = this;
            self.ddl_textVal.accountPOSName = '';
            self.ddl_textVal.TenNganHangPos = '';
            self.newPhieuThu.ID_TaiKhoanPos = null;
            self.newPhieuThu.TienPOS = 0;
        },
        ResetAccountPOS: function () {
            var self = this;
            self.Only_ResetAccountPOS();
            self.CaculatorDaThanhToan();
        },
        ChangeAccountPOS: function (item) {
            var self = this;
            self.ddl_textVal.accountPOSName = item.TenChuThe;
            self.ddl_textVal.TenNganHangPos = item.TenNganHang;
            self.newPhieuThu.ID_TaiKhoanPos = item.ID;
        },
        ChangeAccountCK: function (item) {
            var self = this;
            self.ddl_textVal.accountCKName = item.TenChuThe;
            self.ddl_textVal.TenNganHangCK = item.TenNganHang;
            self.newPhieuThu.ID_TaiKhoanChuyenKhoan = item.ID;
        },
        Only_ResetAccountCK: function () {
            var self = this;
            self.ddl_textVal.accountCKName = '';
            self.ddl_textVal.TenNganHangCK = '';
            self.newPhieuThu.ID_TaiKhoanChuyenKhoan = null;
            self.newPhieuThu.TienCK = 0;
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
            $(event.currentTarget).closest('div').hide();
        },
        ChangeNgayLapPhieu: function (e) {
            let self = this;
            let dt = moment(e).format('YYYY-MM-DD HH:mm');
            let khoaSo = VHeader.CheckKhoaSo(moment(e).format('YYYY-MM-DD'), self.newPhieuThu.ID_DonVi);
            if (khoaSo) {
                ShowMessage_Danger(VHeader.warning.ChotSo.Update);
            }
            self.newPhieuThu.NgayLapHoaDon = dt;
        },
        AssignMoney_InHoaDonDebit: function (isChangeTienThu = false) {
            var self = this;
            if (!isChangeTienThu) {
                var tongthu = self.newPhieuThu.DaThanhToan;
                for (let i = 0; i < self.listData.HoaDons.length; i++) {
                    if (tongthu > self.listData.HoaDons[i].CanThu) {
                        self.listData.HoaDons[i].TienThu = formatNumber(self.listData.HoaDons[i].CanThu);
                        tongthu = tongthu - self.listData.HoaDons[i].CanThu;
                    }
                    else {
                        self.listData.HoaDons[i].TienThu = formatNumber(tongthu);
                        tongthu = 0;
                    }
                }
            }
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
                    self.listData.HoaDons[i].TienThu = formatNumber(tienThu);
                    break;
                }
            }
            var hinhthuc = self.HinhThucTT.ID;
            if (hinhthuc === 0) {
                hinhthuc = 2;
            }
            self.ChoseHinhThucTT(hinhthuc);
        },
        ChangeTienThu_Enter: function () {
            var $this = $(event.currentTarget);
            $this.closest('tr').next().find('input').select();
        },
        EditTienCoc: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var tienmat = 0;
            var khachcantra = formatNumberToFloat(self.newPhieuThu.TongNoHD);
            var soduDatCoc = self.SoDuDatCoc;
            var datcoc = formatNumberToFloat($this.val());
            if (datcoc >= khachcantra) {
                if (datcoc >= soduDatCoc) {
                    if (soduDatCoc > khachcantra) {
                        datcoc = khachcantra;
                        tienmat = 0;
                    }
                    else {
                        datcoc = soduDatCoc;
                        tienmat = khachcantra - datcoc;
                    }
                }
                else {
                    datcoc = khachcantra;
                    tienmat = 0;
                }
            }
            else {
                if (datcoc >= soduDatCoc) {
                    datcoc = soduDatCoc;
                }
                tienmat = khachcantra - datcoc;
            }
            self.newPhieuThu.TienDatCoc = formatNumber(datcoc);
            self.newPhieuThu.TienMat = formatNumber(tienmat);
            self.newPhieuThu.PhaiThanhToan = tienmat;

            self.newPhieuThu.TienPOS = 0;
            self.newPhieuThu.TienCK = 0;
            self.newPhieuThu.TienTheGiaTri = 0;
            self.CaculatorDaThanhToan();

            var key = event.keyCode || event.which;
            if (key === 13) {
                $this.closest('.container-fluid').next().find('input').select();
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
            self.newPhieuThu.TienTheGiaTri = formatNumber(tienthe);
        },

        EditTienMat: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var tiencoc = formatNumberToFloat(self.newPhieuThu.TienDatCoc);
            var tienmat = formatNumberToFloat($this.val());
            self.newPhieuThu.TienMat = formatNumber(tienmat);

            var tienpos = 0, tienck = 0;
            var cantt = self.newPhieuThu.TongNoHD - tienmat - tiencoc;
            cantt = cantt < 0 ? 0 : cantt;

            if (self.newPhieuThu.ID_TaiKhoanPos !== null) {
                tienpos = cantt;
                tienck = 0;
            }
            else {
                if (self.newPhieuThu.ID_TaiKhoanChuyenKhoan !== null) {
                    tienck = cantt;
                }
                else {
                    self.EditMoney_AssignTienThe(cantt);
                }
            }
            self.newPhieuThu.TienPOS = formatNumber(tienpos);
            self.newPhieuThu.TienCK = formatNumber(tienck);
            self.CaculatorDaThanhToan();
            self.ResetHinhThucTT();

            var key = event.keyCode || event.which;
            if (key === 13) {
                if (self.newPhieuThu.ID_TaiKhoanPos !== null) {
                    $this.closest('.container-fluid').next().find('.js-input').select();
                }
                else {
                    if (self.newPhieuThu.ID_TaiKhoanChuyenKhoan !== null) {
                        $this.closest('.container-fluid').next().next().find('.js-input').select();
                    }
                    else {

                    }
                }
            }
        },
        EditTienPos: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var tienpos = formatNumberToFloat($this.val());
            self.newPhieuThu.TienPOS = $this.val();

            var tienck = 0;
            var tiencoc = formatNumberToFloat(self.newPhieuThu.TienDatCoc);
            var tienmat = formatNumberToFloat(self.newPhieuThu.TienMat);
            var cantt = self.newPhieuThu.TongNoHD - tienmat - tiencoc - tienpos;
            cantt = cantt < 0 ? 0 : cantt;
            if (self.newPhieuThu.ID_TaiKhoanChuyenKhoan !== null) {
                tienck = cantt;
            }
            else {
                self.EditMoney_AssignTienThe(cantt);
            }
            self.newPhieuThu.TienCK = formatNumber(tienck);
            self.CaculatorDaThanhToan();
            self.ResetHinhThucTT();

            var key = event.keyCode || event.which;
            if (key === 13) {
                if (self.newPhieuThu.ID_TaiKhoanChuyenKhoan !== null) {
                    $this.closest('.container-fluid').next().find('.js-input').select();
                }
                else {
                    if (self.HoaDonChosing.SoDuTheGiaTri > 0) {
                        $this.closest('.container-fluid').parent().next().next().find('.js-input').select();
                    }
                }
            }
        },
        EditTienCK: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            self.newPhieuThu.TienCK = $this.val();
            self.CaculatorDaThanhToan();
            self.ResetHinhThucTT();

            var key = event.keyCode || event.which;
            if (key === 13) {
                if (self.HoaDonChosing.SoDuTheGiaTri > 0) {
                    $this.closest('.container-fluid').next().find('input').select();
                }
            }
        },
        EditTienThe: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            self.newPhieuThu.TienTheGiaTri = $this.val();
            self.CaculatorDaThanhToan();
            self.ResetHinhThucTT();
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
                        commonStatisJs.ShowMessageDanger('Tổng tiền chi vượt quá số tiền đã đặt cọc');
                        return;
                    }
                    self.HinhThucTT = { ID: val, Text: 'Thu từ cọc' };
                    self.newPhieuThu.TienDatCoc = formatNumber(sum);
                    self.newPhieuThu.TienMat = 0;
                    self.newPhieuThu.TienPOS = 0;
                    self.newPhieuThu.TienCK = 0;
                    self.newPhieuThu.TienTheGiaTri = 0;
                    self.Only_ResetAccountCK();
                    self.Only_ResetAccountPOS();
                    break;
                case 2:
                    self.HinhThucTT = { ID: val, Text: 'Tiền mặt' };
                    self.newPhieuThu.TienMat = formatNumber(sum);
                    self.newPhieuThu.TienPOS = 0;
                    self.newPhieuThu.TienCK = 0;
                    self.newPhieuThu.TienTheGiaTri = 0;
                    self.newPhieuThu.TienDatCoc = 0;
                    self.Only_ResetAccountCK();
                    self.Only_ResetAccountPOS();
                    break;
                case 3:
                    self.HinhThucTT = { ID: val, Text: 'POS' };
                    self.newPhieuThu.TienMat = 0;
                    self.newPhieuThu.TienPOS = formatNumber(sum);
                    self.newPhieuThu.TienCK = 0;
                    self.newPhieuThu.TienTheGiaTri = 0;
                    self.newPhieuThu.TienDatCoc = 0;
                    self.Only_ResetAccountCK();
                    break;
                case 4:
                    self.HinhThucTT = { ID: val, Text: 'Chuyển khoản' };
                    self.newPhieuThu.TienMat = 0;
                    self.newPhieuThu.TienPOS = 0;
                    self.newPhieuThu.TienCK = formatNumber(sum);
                    self.newPhieuThu.TienTheGiaTri = 0;
                    self.newPhieuThu.TienDatCoc = 0;
                    self.Only_ResetAccountPOS();
                    break;
                case 5:
                    if (self.HoaDonChosing.SoDuTheGiaTri < sum) {
                        commonStatisJs.ShowMessageDanger('Tổng tiền chi vượt quá số dư thẻ giá trị');
                        return;
                    }
                    self.HinhThucTT = { ID: val, Text: 'Thẻ giá trị' };
                    self.newPhieuThu.TienTheGiaTri = formatNumber(sum);
                    self.newPhieuThu.TienMat = 0;
                    self.newPhieuThu.TienPOS = 0;
                    self.newPhieuThu.TienCK = 0;
                    self.newPhieuThu.TienDatCoc = 0;
                    self.Only_ResetAccountCK();
                    self.Only_ResetAccountPOS();
                    break;
            }
            self.CaculatorDaThanhToan(true);
        },
        shareMoney_QuyHD: function (phaiTT, tienDiem, tienmat, tienPOS, chuyenkhoan, thegiatri, tiencoc) {
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
                    }
                }
                else {
                    phaiTT = phaiTT - tienDiem;
                    if (thegiatri >= phaiTT) {
                        return {
                            TienCoc: tiencoc,
                            TTBangDiem: tienDiem,
                            TienMat: 0,
                            TienPOS: 0,
                            TienChuyenKhoan: 0,
                            TienTheGiaTri: Math.abs(phaiTT),
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
                }
            }
        },

        SavePhieuChi: function (print) {
            var self = this;
            var ptKhach = self.newPhieuThu;

            let khoaSo = VHeader.CheckKhoaSo(moment(ptKhach.NgayLapHoaDon, 'YYYY-MM-DD HH:mm').format('YYYY-MM-DD'), ptKhach.ID_DonVi);
            if (khoaSo) {
                ShowMessage_Danger(VHeader.warning.ChotSo.Update);
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

            let sNguoiNop = '';
            if (self.listData.NguoiNops.length > 0) {
                sNguoiNop = self.listData.NguoiNops[0].TenNguoiNop.concat(' (', self.listData.NguoiNops[0].MaNguoiNop, ')');
                if (!commonStatisJs.CheckNull(ptKhach.NoiDungThu)) {
                    sNguoiNop = ' /' + sNguoiNop;
                }
            }
            let ghichu = ptKhach.NoiDungThu + sNguoiNop;
            let idDoiTuong = ptKhach.ID_DoiTuong;
            let idKhoanThuChi = ptKhach.ID_KhoanThuChi;
            let tenDoiTuong = self.ddl_textVal.cusName;
            let sMaHoaDon = '', sTaiKhoan = '';
            let lstQuyCT = [], arrPhuongThuc = [];
            let loaiThuChi = self.LoaiHoaDon === 7 ? 11 : 12;
            let phuongthucTT = '';
            self.QuyHD_Share = [];
            let tongthu = 0, tienmat = 0, tienpos = 0, tienck = 0, tienthe = 0, tiendiem = 0, tiendatcoc = 0;

            // phan bo tien neu nhap thua
            let dataReturn = self.shareMoney_QuyHD(ptKhach.TongNoHD, formatNumberToFloat(ptKhach.TTBangDiem),
                formatNumberToFloat(ptKhach.TienMat), formatNumberToFloat(ptKhach.TienPOS),
                formatNumberToFloat(ptKhach.TienCK), formatNumberToFloat(ptKhach.TienTheGiaTri),
                formatNumberToFloat(ptKhach.TienDatCoc));

            tiendatcoc = dataReturn.TienCoc;
            tienmat = dataReturn.TienMat;
            tienpos = dataReturn.TienPOS;
            tienck = dataReturn.TienChuyenKhoan;
            tienthe = dataReturn.TienTheGiaTri;
            tiendiem = dataReturn.TTBangDiem;
            tongthu = tiendatcoc + tienmat + tienpos + tienck + tienthe + tiendiem;

            // phanbo tien cho hoadon debit
            for (let i = 0; i < self.listData.HoaDons.length; i++) {
                let itFor = self.listData.HoaDons[i];
                let tienThucTeThu = formatNumberToFloat(itFor.TienThu);
                if (tienThucTeThu > 0) {
                    let obj = self.shareMoney_QuyHD(tienThucTeThu, tiendiem, tienmat, tienpos, tienck, tienthe, tiendatcoc);
                    obj.ID_HoaDonLienQuan = itFor.ID;
                    obj.MaHoaDon = itFor.MaHoaDon;
                    self.QuyHD_Share.push(obj);

                    // tinh lai cho tung hoadon
                    tiendatcoc = tiendatcoc - obj.TienCoc;
                    tienmat = tienmat - obj.TienMat;
                    tienpos = tienpos - obj.TienPOS;
                    tienck = tienck - obj.TienChuyenKhoan;
                    tienthe = tienthe - obj.TienTheGiaTri;
                    tiendiem = tiendiem - obj.TTBangDiem;
                }
            }

            // insert quy_ct
            for (let i = 0; i < self.QuyHD_Share.length; i++) {
                let itemFor = self.QuyHD_Share[i];
                let idHoaDon = itemFor.ID_HoaDonLienQuan;

                if (itemFor.TienCoc > 0 && loaiThuChi === 12) {
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
                if (itemFor.TienPOS > 0) {
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: idHoaDon,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ''.concat(self.ddl_textVal.accountPOSName, ' - ', self.ddl_textVal.TenNganHangPos),
                        TienThu: itemFor.TienPOS,
                        TienPOS: itemFor.TienPOS,
                        HinhThucThanhToan: 2,
                        ID_TaiKhoanNganHang: ptKhach.ID_TaiKhoanPos,
                    });
                    lstQuyCT.push(qct);
                    sTaiKhoan += '/ ' + qct.GhiChu;

                    if ($.inArray(qct.HinhThucThanhToan, arrPhuongThuc) === -1) {
                        arrPhuongThuc.push(qct.HinhThucThanhToan);
                    }
                }
                if (itemFor.TienChuyenKhoan > 0) {
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: idHoaDon,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ''.concat(self.ddl_textVal.accountCKName, ' - ', self.ddl_textVal.TenNganHangCK),
                        TienThu: itemFor.TienChuyenKhoan,
                        TienChuyenKhoan: itemFor.TienChuyenKhoan,
                        HinhThucThanhToan: 3,
                        ID_TaiKhoanNganHang: ptKhach.ID_TaiKhoanChuyenKhoan,
                    });
                    lstQuyCT.push(qct);
                    sTaiKhoan += '/ ' + qct.GhiChu;

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

                    if ($.inArray(qct.HinhThucThanhToan, arrPhuongThuc) === -1) {
                        arrPhuongThuc.push(qct.HinhThucThanhToan);
                    }
                }
                sMaHoaDon += itemFor.MaHoaDon + ', ';
            }
            sMaHoaDon = Remove_LastComma(sMaHoaDon);
            ghichu += '/ ' + sMaHoaDon + sTaiKhoan;

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

            let quyhd = {
                LoaiHoaDon: loaiThuChi,
                TongTienThu: tongthu,
                MaHoaDon: ptKhach.MaHoaDon,
                NgayLapHoaDon: ptKhach.NgayLapHoaDon,
                NguoiNopTien: tenDoiTuong,
                NguoiTao: self.inforLogin.UserLogin,
                NoiDungThu: ghichu,
                ID_NhanVien: ptKhach.ID_NhanVien,
                ID_DonVi: ptKhach.ID_DonVi,
                ID_DoiTuong: ptKhach.ID_DoiTuong,// used to get when saveDB
                HoaDonLienQuan: sMaHoaDon,
            }
            phuongthucTT = Remove_LastComma(phuongthucTT);
            quyhd.PhuongThucTT = phuongthucTT;

            // use when print phieuthu
            quyhd.TienMat = dataReturn.TienMat;
            quyhd.TienPOS = dataReturn.TienPOS;
            quyhd.TienChuyenKhoan = dataReturn.TienChuyenKhoan;
            quyhd.TienTheGiaTri = dataReturn.TienTheGiaTri;
            quyhd.TienCoc = dataReturn.TienCoc;
            quyhd.TTBangDiem = dataReturn.TTBangDiem;

            var myData = {
                objQuyHoaDon: quyhd,
                lstCTQuyHoaDon: lstQuyCT
            };

            if (lstQuyCT.length > 0) {
                console.log('quyhd ', myData);
                if (self.isNew) {
                    ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/PostQuy_HoaDon_DefaultIDDoiTuong', 'POST', myData).done(function (x) {
                        console.log(x)
                        if (x.res === true) {
                            self.saveOK = true;

                            quyhd.MaHoaDon = x.data.MaHoaDon;
                            let diary = {
                                LoaiNhatKy: 1,
                                ID_DonVi: self.inforLogin.ID_DonVi,
                                ID_NhanVien: self.inforLogin.ID_NhanVien,
                                ChucNang: 'Phiếu '.concat(self.sLoaiThuChi),
                                NoiDung: 'Tạo phiếu '.concat(self.sLoaiThuChi, ' ', quyhd.MaHoaDon, ' cho hóa đơn ', sMaHoaDon,
                                    ', Nhà cung cấp: ', quyhd.NguoiNopTien, ', với giá trị ', formatNumber(quyhd.TongTienThu),
                                    ', Phương thức thanh toán:', phuongthucTT,
                                    ', Thời gian: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')),
                                NoiDungChiTiet: 'Tạo phiếu '.concat(self.sLoaiThuChi, ' <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD(', quyhd.MaHoaDon, ')" >', quyhd.MaHoaDon, '</a> ',
                                    ' cho hóa đơn: <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD(', sMaHoaDon, ')" >', sMaHoaDon, '</a> ',
                                    '<br /> Nhà cung cấp: <a style="cursor: pointer" onclick = "LoadKhachHang_byMaKH(', quyhd.NguoiNopTien, ')" >', quyhd.NguoiNopTien, '</a> ',
                                    '<br /> Giá trị: ', formatNumber(quyhd.TongTienThu),
                                    '<br/ > Phương thức thanh toán: ', phuongthucTT,
                                    '<br/ > Thời gian: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'),
                                    '<br/ > Khoản ', self.sLoaiThuChi, ': ', self.ddl_textVal.khoanthu,
                                    '<br/ > NV lập phiếu: ', self.ddl_textVal.staffName,
                                    '<br/ > Người tạo: ', self.inforLogin.UserLogin,
                                )
                            }
                            Insert_NhatKyThaoTac_1Param(diary);

                            if (print) {
                                self.InPhieuThu(quyhd);
                            }
                            commonStatisJs.ShowMessageSuccess('Thanh toán thành công');
                        }
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
                        console.log(x.mess)
                        if (x.res) {
                            commonStatisJs.ShowMessageSuccess('Cập nhật phiếu ' + self.sLoaiThuChi + ' thành công');

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
                                case 2:
                                    sLoaiDT += 'nhà cung cấp: ';
                                    break;
                                case 3:
                                    sLoaiDT += 'Cty bảo hiểm: ';
                                    break;
                            }

                            let diary = {
                                LoaiNhatKy: 2,
                                ID_DonVi: self.inforLogin.ID_DonVi,
                                ID_NhanVien: self.inforLogin.ID_NhanVien,
                                ChucNang: 'Cập nhật phiếu ' + self.sLoaiThuChi,
                                NoiDung: 'Cập nhật phiếu '.concat(self.sLoaiThuChi, ' ', quyhd.MaHoaDon, ' cho hóa đơn ', sMaHoaDon),
                                NoiDungChiTiet: 'Cập nhật phiếu '.concat(self.sLoaiThuChi, ' ', quyhd.MaHoaDon,
                                    '<br/> <b> Thông tin mới: </b>',
                                    '<br/> - Mã hóa đơn: ', quyhd.MaHoaDon,
                                    '<br/> - Ngày lập hóa đơn: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'),
                                    '<br/> - Tổng tiền ', self.sLoaiThuChi, ': ', formatNumber3Digit(quyhd.TongTienThu),
                                    '<br/> - ', sLoaiDT, quyhd.NguoiNopTien,
                                    '<br/> - Khoản ', self.sLoaiThuChi, ': ', self.ddl_textVal.khoanthu,
                                    '<br/> - Phương thức thanh toán: ', quyhd.PhuongThucTT,
                                    '<br/> - Tiền mặt: ', formatNumber3Digit(quyhd.TienMat),
                                    '<br/> - Tiền POS: ', formatNumber3Digit(quyhd.TienPOS),
                                    '<br/> - Tiền chuyển khoản: ', formatNumber3Digit(quyhd.TienChuyenKhoan),
                                    '<br/> - Thu từ cọc: ', formatNumber3Digit(quyhd.TienCoc),
                                    '<br/> - Tài khoản POS: ', self.ddl_textVal.accountPOSName,
                                    '<br/> - Tài khoản chuyển khoản: ', self.ddl_textVal.accountCKName,
                                    '<br/> - NV lập phiếu: ', self.ddl_textVal.staffName,
                                    '<br/> - Người sửa: ', self.inforLogin.UserLogin,

                                    '<br/> <b> Thông tin cũ: </b>',
                                    '<br/> - Mã hóa đơn: ', self.phieuThuOld.MaHoaDon,
                                    '<br/> - Ngày lập hóa đơn: ', moment(self.phieuThuOld.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'),
                                    '<br/> - Tổng tiền ', self.sLoaiThuChi, ': ', formatNumber3Digit(self.phieuThuOld.TongTienThu),
                                    '<br/> - ', sLoaiDT, self.phieuThuOld.NguoiNopTien, ' (', self.phieuThuOld.MaNguoiNop, ')',
                                    '<br/> - Khoản ', self.sLoaiThuChi, ': ', self.phieuThuOld.TenKhoanThuChi,
                                    '<br/> - Phương thức thanh toán: ', self.phieuThuOld.PhuongThucTT,
                                    '<br/> - Tiền mặt: ', formatNumber3Digit(self.phieuThuOld.TienMat),
                                    '<br/> - Tiền POS: ', formatNumber3Digit(self.phieuThuOld.TienPOS),
                                    '<br/> - Tiền chuyển khoản: ', formatNumber3Digit(self.phieuThuOld.TienCK),
                                    '<br/> - Thu từ cọc: ', formatNumber3Digit(self.phieuThuOld.TienDatCoc),
                                    '<br/> - Tài khoản POS: ', self.phieuThuOld.TenTaiKhoanPos,
                                    '<br/> - Tài khoản chuyển khoản: ', self.phieuThuOld.TenTaiKhoanCK,
                                    '<br/> - NV lập phiếu: ', self.phieuThuOld.TenNhanVien,
                                ),
                            }
                            Insert_NhatKyThaoTac_1Param(diary);

                            if (print) {
                                self.InPhieuThu(quyhd);
                            }
                        }
                        $('#vmThanhToanNCC').modal('hide');
                    })
                }

            }
            $('#vmThanhToanNCC').modal('hide');
        },

        HuyPhieu: function () {
            let self = this;
            let khoaSo = VHeader.CheckKhoaSo(moment(self.phieuThuOld.NgayLapHoaDon).format('YYYY-MM-DD'), self.newPhieuThu.ID_DonVi);
            if (khoaSo) {
                ShowMessage_Danger(VHeader.warning.ChotSo.Delete);
                return;
            }
            dialogConfirm('Xác nhận xóa', 'Bạn có chắc chắn muốn hủy phiếu ' + self.sLoaiThuChi + ' <b> ' + self.newPhieuThu.MaHoaDon + ' </b> không?', function () {

                ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/' + "DeleteQuy_HoaDon/" + self.newPhieuThu.ID, 'DELETE').done(function (x) {
                    if (x === "") {
                        ShowMessage_Success("Xóa sổ quỹ thành công");
                        $('#vmThanhToanNCC').modal('hide');

                        let diary = {
                            ID_NhanVien: self.inforLogin.ID_NhanVien,
                            ID_DonVi: self.inforLogin.ID_DonVi,
                            ChucNang: 'Phiếu ' + self.sLoaiThuChi,
                            NoiDung: 'Hủy phiếu '.concat(self.sLoaiThuChi, ' ', self.newPhieuThu.MaHoaDon, ', nhân viên hủy: ', self.inforLogin.UserLogin),
                            NoiDungChiTiet: ''.concat('Hủy phiếu ', self.sLoaiThuChi, ' ', self.newPhieuThu.MaHoaDon, ', nhân viên hủy: ', self.inforLogin.UserLogin,
                                '<br /><b> Thông tin cũ: </b>',
                                '<br /> - Giá trị: ', formatNumber3Digit(self.phieuThuOld.TongTienThu),
                                '<br /> - Phương thức thanh toán: ', self.phieuThuOld.PhuongThucTT,
                                '<br /> - Nhà cung cấp: ', self.phieuThuOld.NguoiNopTien, ' (', self.phieuThuOld.MaNguoiNop, ')'
                            ),
                            LoaiNhatKy: 3
                        }
                        Insert_NhatKyThaoTac_1Param(diary);
                    }
                    else {
                        ShowMessage_Danger("Giá trị thẻ nạp đã được sử dụng không thể xóa phiếu này");
                    }
                });
            });
        },
        KhoiPhuc: function () {
            let self = this;

            dialogConfirm('Khôi phục', 'Bạn có chắc chắn muốn khôi phục phiếu ' + self.sLoaiThuChi + ' <b> ' + self.newPhieuThu.MaHoaDon + ' </b> không?', function () {

                ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/' + "KhoiPhucQuy_HoaDon/" + self.newPhieuThu.ID, 'GET').done(function (x) {
                    if (x.res) {
                        ShowMessage_Success("Khôi phục sổ quỹ thành công");
                        $('#vmThanhToanNCC').modal('hide');

                        let diary = {
                            ID_NhanVien: self.inforLogin.ID_NhanVien,
                            ID_DonVi: self.inforLogin.ID_DonVi,
                            ChucNang: 'Khôi phục phiếu ' + self.sLoaiThuChi,
                            NoiDung: 'Khôi phục phiếu '.concat(self.sLoaiThuChi, ' ', self.newPhieuThu.MaHoaDon, ', Người khôi phục: ', self.inforLogin.UserLogin),
                            NoiDungChiTiet: ''.concat('Khôi phục phiếu ', self.sLoaiThuChi, ' ', self.newPhieuThu.MaHoaDon, ', Người khôi phục: ', self.inforLogin.UserLogin),
                            LoaiNhatKy: 2
                        }
                        Insert_NhatKyThaoTac_1Param(diary);
                    }
                    else {
                        ShowMessage_Danger(x.mess);
                    }
                });
            });
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
            let quyhd = $.extend({}, obj);
            let self = this;
            let loaiCT = 'SQPT'
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
            let tongThu = formatNumberToInt(quyhd.TongTienThu);
            quyhd.TienBangChu = DocSo(tongThu);
            quyhd.GiaTriPhieu = formatNumber(tongThu);

            quyhd.TienMat = formatNumber(obj.TienMat);
            quyhd.TienATM = formatNumber(obj.TienPOS);
            quyhd.ChuyenKhoan = formatNumber(obj.TienChuyenKhoan);
            quyhd.TienTheGiaTri = formatNumber(obj.TienTheGiaTri);
            quyhd.TTBangTienCoc = formatNumber(obj.TienCoc);
            quyhd.TienDoiDiem = formatNumber(obj.TTBangDiem);
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
    },
})
