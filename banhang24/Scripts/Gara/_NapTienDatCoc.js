var vmNapTienDatCoc = new Vue({
    el: '#NapTienDatCoc',
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
        self.GuidEmpty = '00000000-0000-0000-0000-000000000000';
        self.inforLogin = {
            ID_NhanVien: VHeader.IdNhanVien,
            ID_User: VHeader.IdNhanVien,
            UserLogin: VHeader.UserLogin,
            ID_DonVi: VHeader.IdDonVi,
            TenNhanVien: VHeader.TenNhanVien,
        };

        self.role.SoQuy.Insert = VHeader.Quyen.indexOf('SoQuy_ThemMoi') > -1;
        self.role.SoQuy.Update = VHeader.Quyen.indexOf('SoQuy_CapNhat') > -1;
        self.role.SoQuy.Delete = VHeader.Quyen.indexOf('SoQuy_Xoa') > -1;
        self.role.SoQuy.ChangeNgayLap = VHeader.Quyen.indexOf('SoQuy_ThayDoiThoiGian') > -1;
        self.role.SoQuy.ChangeNVLap = VHeader.Quyen.indexOf('SoQuy_ThayDoiThoiNVLapPhieu') > -1;
        self.role.SoQuy.XoaNeuKhacNgay = VHeader.Quyen.indexOf('SoQuy_Xoa_NeuKhacNgay') > -1;
        self.role.SoQuy.ThayDoiKhoanThuChi = VHeader.Quyen.indexOf('SoQuy_ThayDoiKhoanThuChi') > -1;

        self.role.Customer.Insert = VHeader.Quyen.indexOf('KhachHang_ThemMoi') > -1;
        self.role.Vendor.Insert = VHeader.Quyen.indexOf('NhaCungCap_ThemMoi') > -1;
        console.log('vmnapcoc')
    },
    data: {
        saveOK: false,
        isNew: true,
        isNapTien: false,
        isLoading: false,
        isKhoaSo: false,
        formType: 0, // 1. tra lai tien TGT, 0.con lai
        loaiMenu: 0,// 0.thu, 1.chi, 2.all (0.1 used to ds soquy, 2.ds hoadon + khachhang + ncc)
        role: {
            SoQuy: { ChangeNVLap: true, ThayDoiKhoanThuChi: false },
            Customer: {},
            Vendor: {},
        },
        inforOld: {},
        ddl_textVal: {
            staffName: '',
            accountCKName: '',
            bankName: '',
            khoanthu: '',
        },

        inforNguoiNop: {
            ID: null,
            MaDoiTuong: '',
            TenDoiTuong: '',
            SoDienThoai: '',
            SoDuDatCoc: 0,
            CongNoThe: 0,
        },
        inforCongTy: {
            TenCongTy: '',
            DiaChiCuaHang: '',
            LogoCuaHang: ''
        },
        newHoaDonTGT: {
            ID: '00000000-0000-0000-0000-000000000000',
            MaHoaDon: '',
            LoaiHoaDon: 32,
            ID_DoiTuong: null,
            NgayLapHoaDon: moment(new Date()).format('YYYY-MM-DD HH:mm'),
            TongTienHang: 0,
            TongChietKhau: 0,
            TongGiamGia: 0,
            PhaiThanhToan: 0,
            TongThanhToan: 0,
            PhiHoanThe_LaPtram: true,
            ID_DonVi: null,
            ID_NhanVien: null,
            NguoiTao: '',
            ChoThanhToan: false,
            TienThua: 0,
        },

        newPhieuThu: {
            ID: '00000000-0000-0000-0000-000000000000',
            LoaiHoaDon: 11,
            LoaiDoiTuong: 1,// 1.kh, 2.ncc
            MaHoaDon: '',
            NgayLapHoaDon: moment(new Date()).format('YYYY-MM-DD HH:mm'),
            TienMat: 0,
            TienCK: 0,
            ID_TaiKhoanChuyenKhoan: null,
            ID_NganHang: null,
            ID_NhanVien: null,
            ID_DoiTuong: null,
            NoiDungThu: '',
            NguoiNopTien: '',
            TongTienThu: 0,
            NguoiTao: '',
            ID_DonVi: null,
            HachToanKinhDoanh: true,
            PhieuDieuChinhCongNo: 2,
            TrangThai: true,
            TenNhanVien: '',
        },
        listData: {
            AccountBanks: [],
            NhanViens: [],
            AllKhoanThuChis: [],
        },
    },
    methods: {
        ChangeLoaiDoiTuongNop: function () {
            let self = this;
            if (self.newPhieuThu.LoaiDoiTuong === 1) {
                self.loaiMenu = 1;
                self.newPhieuThu.LoaiDoiTuong = 2;
                self.newPhieuThu.LoaiHoaDon = 12;// chicoc nhacungcap
            }
            else {
                self.loaiMenu = 0;
                self.newPhieuThu.LoaiDoiTuong = 1;
                self.newPhieuThu.LoaiHoaDon = 11;// thucoc khachhang
            }
        },
        ChangeTab: function (loaiDT) {
            var self = this;
            var loaiHD = 11;
            var laKhoanThu = true;
            if (self.isNapTien) {
                if (loaiDT === 2) {
                    loaiHD = 12;
                    laKhoanThu = false;
                }
            }
            else {
                if (loaiDT !== 2) {
                    loaiHD = 12;
                    laKhoanThu = false;
                }
            }
            self.newPhieuThu.LoaiDoiTuong = loaiDT;
            self.newPhieuThu.LoaiHoaDon = loaiHD;
            self.newPhieuThu.ID_DoiTuong = null;
            self.newPhieuThu.NguoiNopTien = '';
            self.inforNguoiNop = {
                MaDoiTuong: '',
                TenDoiTuong: '',
                SoDienThoai: '',
                DiaChi: '',
                SoDuDatCoc: 0,
            };
        },
        GetSoDuDatCoc: async function (idDoiTuong) {
            let self = this;
            if (self.formType === 1) {
                // get sodu TGT
                let obj = await vmThanhToan.Async_GetInforTheGiaTri(idDoiTuong);
                self.inforNguoiNop.SoDuDatCoc = obj.SoDuTheGiaTri;
                self.inforNguoiNop.CongNoThe = obj.CongNoThe;
            }
            else {
                ajaxHelper('/api/DanhMuc/DM_DoiTuongAPI/' + "GetTienDatCoc_ofDoiTuong?idDoiTuong=" + idDoiTuong
                    + '&idDonVi=' + self.inforLogin.ID_DonVi, 'GET').done(function (x) {
                        let soduDatCoc = 0;
                        if (x.res && x.dataSoure.length > 0) {
                            soduDatCoc = x.dataSoure[0].SoDuTheGiaTri;
                        }
                        self.inforNguoiNop.SoDuDatCoc = soduDatCoc;
                    });
            }
        },
        ChoseNguoiNopTien: async function (item) {
            var self = this;
            self.newPhieuThu.ID_DoiTuong = item.ID;
            self.newPhieuThu.NguoiNopTien = item.TenDoiTuong;
            self.newHoaDonTGT.ID_DoiTuong = item.ID;
            self.inforNguoiNop = {
                MaDoiTuong: item.MaDoiTuong,
                TenDoiTuong: item.TenDoiTuong,
                SoDienThoai: item.DienThoai,
                DiaChi: item.DiaChi,
                SoDuDatCoc: 0,
            };
            self.GetSoDuDatCoc(item.ID);
        },
        showListNguoiNop: function () {
            $(event.currentTarget).next().show();
        },
        TGT_Caculator: function () {
            let self = this;
            self.newHoaDonTGT.PhaiThanhToan = formatNumberToFloat(self.newHoaDonTGT.TongTienHang)
                - formatNumberToFloat(self.newHoaDonTGT.TongGiamGia);
            self.newHoaDonTGT.TongThanhToan = self.newHoaDonTGT.PhaiThanhToan;

            // set deafult tienmat for phieuthu
            self.newPhieuThu.TienMat = formatNumber3Digit(self.newHoaDonTGT.PhaiThanhToan);
            self.newPhieuThu.TongTienThu = self.newHoaDonTGT.PhaiThanhToan;
            self.newPhieuThu.TienCK = 0;
            self.newHoaDonTGT.TienThua = 0;
        },
        PhiHoanTra_ClickPtramVND: function () {
            let self = this;
            self.newHoaDonTGT.PhiHoanTra_LaPtram = !self.newHoaDonTGT.PhiHoanTra_LaPtram;
            if (!self.newHoaDonTGT.PhiHoanTra_LaPtram) {
                self.newHoaDonTGT.TongChietKhau = 0;
            }
            else {
                self.newHoaDonTGT.TongGiamGia = self.newHoaDonTGT.TongChietKhau * formatNumberToFloat(self.newHoaDonTGT.TongTienHang) / 100;
            }
        },
        EditPhiHoanTra: function () {
            let self = this;
            let $this = $(event.currentTarget);
            let gtri = formatNumberToFloat($this.val());
            if (self.newHoaDonTGT.PhiHoanTra_LaPtram) {
                if (gtri > 100) {
                    $this.val(100);
                }
                self.newHoaDonTGT.TongChietKhau = formatNumberToFloat($this.val());
                self.newHoaDonTGT.TongGiamGia = self.newHoaDonTGT.TongChietKhau * formatNumberToFloat(self.newHoaDonTGT.TongTienHang) / 100;
            }
            else {
                formatNumberObj($this);
                if (gtri > formatNumberToFloat(self.newHoaDonTGT.TongTienHang)) {
                    self.newHoaDonTGT.TongGiamGia = self.newHoaDonTGT.TongTienHang;
                }
                else {
                    self.newHoaDonTGT.TongGiamGia = $this.val();
                }
            }
            self.TGT_Caculator();
        },
        EditTongTienHang: function () {
            let self = this;
            let $this = $(event.currentTarget);
            formatNumberObj($this);

            let gtri = formatNumberToFloat($this.val());
            if (gtri > self.inforNguoiNop.SoDuDatCoc) {
                ShowMessage_Danger('Vui lòng không nhập quá số dư hiện tại');
                self.newHoaDonTGT.TongTienHang = formatNumber3Digit(self.inforNguoiNop.SoDuDatCoc);
                return;
            }
            self.newHoaDonTGT.TongTienHang = formatNumber3Digit(gtri);
            self.TGT_Caculator();
        },
        showModalAddNew: function (isNapTien, formType = 0) { // 1. tra lai tien TGT, 0.con lai
            var self = this;
            self.isNapTien = isNapTien;
            self.isNew = true;
            self.isLoading = false;
            self.isKhoaSo = false;
            self.formType = formType;

            let ktc = [];
            let idKhoanThuChi = null, tenKhoanThu = '';
            if (formType === 1) {
                ktc = $.grep(self.listData.AllKhoanThuChis, function (x) {
                    return x.LaKhoanThu === false && x.LoaiChungTu === '22';
                });
                if (ktc.length > 0) {
                    idKhoanThuChi = ktc[0].ID;
                    tenKhoanThu = ktc[0].NoiDungThuChi;
                }
            }
            self.ddl_textVal.khoanthu = tenKhoanThu;

            let loaiDoiTuong = 1;
            let loaiHoaDon = 11;
            switch (self.loaiMenu) {
                case 0:// thu
                    if (self.isNapTien) {//napcoc
                        loaiDoiTuong = 1;
                    }
                    else {
                        loaiDoiTuong = 2;// tracoc
                    }
                    break;
                case 1://chi
                    loaiHoaDon = 12;
                    if (self.isNapTien) {
                        loaiDoiTuong = 2;
                    }
                    else {
                        loaiDoiTuong = 1;
                    }
                    break;
            }

            self.newPhieuThu = {
                ID: '00000000-0000-0000-0000-000000000000',
                LoaiHoaDon: loaiHoaDon,
                LoaiDoiTuong: loaiDoiTuong,
                MaHoaDon: '',
                NgayLapHoaDon: moment(new Date()).format('YYYY-MM-DD HH:mm'),
                TienMat: 0,
                TienCK: 0,
                ID_TaiKhoanChuyenKhoan: null,
                ID_KhoanThuChi: idKhoanThuChi,
                ID_NganHang: null,
                ID_DoiTuong: null,
                NoiDungThu: '',
                NguoiNopTien: '',
                TongTienThu: 0,
                HachToanKinhDoanh: true,
                PhieuDieuChinhCongNo: 2,
                TrangThai: true,
                ID_DonVi: self.inforLogin.ID_DonVi,
                NguoiTao: self.inforLogin.UserLogin,
                ID_NhanVien: self.inforLogin.ID_NhanVien,
                TenNhanVien: self.inforLogin.TenNhanVien,
            };
            self.inforNguoiNop = {
                ID: null,
                MaDoiTuong: '',
                TenDoiTuong: '',
                SoDienThoai: '',
                DiaChi: '',
                SoDuDatCoc: 0,
            };

            self.newHoaDonTGT = {
                ID: '00000000-0000-0000-0000-000000000000',
                MaHoaDon: '',
                LoaiHoaDon: 32,// hoan tra TGT
                ID_DoiTuong: null,
                NgayLapHoaDon: moment(new Date()).format('YYYY-MM-DD HH:mm'),
                TongTienHang: 0,
                TongChietKhau: 0,
                TongGiamGia: 0,
                PhaiThanhToan: 0,
                TongThanhToan: 0,
                PhiHoanTra_LaPtram: true,
                ID_DonVi: self.inforLogin.ID_DonVi,
                ID_NhanVien: self.inforLogin.ID_NhanVien,
                NguoiTao: self.inforLogin.UserLogin,
                ChoThanhToan: false,
                TienThua: 0,
            };
            self.ddl_textVal.accountCKName = '';
            self.ddl_textVal.bankName = '';
            $('#NapTienDatCoc').modal('show');
        },
        showModalUpdate: async function (quyHD, qct) {
            var self = this;
            self.inforOld = $.extend({}, quyHD);
            self.isNew = false;
            self.isLoading = false;
            let ngayLap = moment(quyHD.NgayLapHoaDon).format('YYYY-MM-DD');
            let dtNow = moment(new Date()).format('YYYY-MM-DD')
            let role = VHeader.Quyen.indexOf('SoQuy_Xoa_NeuKhacNgay') > -1;
            if (dtNow === ngayLap) {
                role = true;
            }
            self.role.SoQuy.XoaNeuKhacNgay = role;
            self.isKhoaSo = VHeader.CheckKhoaSo(ngayLap, quyHD.ID_DonVi);
            var isNapTien = true;
            var loaiHD = quyHD.LoaiHoaDon;
            switch (quyHD.LoaiDoiTuong) {
                case 1:
                    if (loaiHD === 12) {
                        isNapTien = false;
                    }
                    break;
                case 3:
                    if (loaiHD === 12) {
                        isNapTien = false;
                    }
                    break;
                case 2:
                    if (loaiHD === 11) {
                        isNapTien = false;
                    }
                    break;
            }
            self.isNapTien = isNapTien;

            var tienmat = qct.reduce(function (_this, item) {
                return _this + item.TienMat;
            }, 0);
            var ck = qct.reduce(function (_this, item) {
                return _this + item.TienGui;
            }, 0)
            var qct0 = qct[0];
            self.newPhieuThu = {
                ID: quyHD.ID,
                LoaiHoaDon: quyHD.LoaiHoaDon,
                LoaiDoiTuong: quyHD.LoaiDoiTuong,// 1.kh, 2.ncc
                MaHoaDon: quyHD.MaHoaDon,
                NgayLapHoaDon: moment(quyHD.NgayLapHoaDon).format('YYYY-MM-DD HH:mm'),
                TienMat: formatNumber(tienmat),
                TienCK: formatNumber(ck),
                ID_TaiKhoanChuyenKhoan: quyHD.ID_TaiKhoanNganHang,
                ID_KhoanThuChi: qct0.ID_KhoanThuChi,
                ID_NganHang: null,
                ID_DoiTuong: qct0.ID_DoiTuong,
                NoiDungThu: quyHD.NoiDungThu,
                NguoiNopTien: quyHD.NguoiNopTien,
                TongTienThu: ck + tienmat,
                HachToanKinhDoanh: quyHD.HachToanKinhDoanh,
                PhieuDieuChinhCongNo: 2,
                TrangThai: quyHD.TrangThai,
                ID_DonVi: quyHD.ID_DonVi,
                NguoiTao: '',
                ID_NhanVien: quyHD.ID_NhanVien,
                TenNhanVien: quyHD.TenNhanVien,
            };
            self.inforNguoiNop = {
                ID: quyHD.ID_DoiTuong,
                MaDoiTuong: quyHD.MaDoiTuong,
                TenDoiTuong: quyHD.NguoiNopTien,
                SoDienThoai: quyHD.SoDienThoai,
                DiaChi: '',
                SoDuDatCoc: 0,
            };
            self.ddl_textVal.accountCKName = quyHD.TenTaiKhoanNOTPOS;
            self.ddl_textVal.bankName = '';// todo
            self.ddl_textVal.khoanthu = quyHD.NoiDungThuChi;
            self.GetSoDuDatCoc(qct0.ID_DoiTuong);
            $('#NapTienDatCoc').modal('show');
        },
        getQuyHoaDon_byID: function (idQuyHD, isShowModal = false, formType = 0) {
            let self = this;
            self.formType = formType;
            ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/' + 'GetQuyChiTiet_byIDQuy/' + idQuyHD, 'GET').done(function (x) {
                if (x.res && x.dataSoure.length > 0) {
                    let quyHD = x.dataSoure[0];
                    let qct = x.dataSoure;
                    let arrTK = $.grep(qct, function (x) {
                        return x.ID_TaiKhoanNganHang !== null;
                    });
                    let idNganHang = null, tenTK = '', pthuc = '';
                    if (arrTK.length > 0) {
                        idNganHang = arrTK[0].ID_TaiKhoanNganHang;
                        tenTK = arrTK[0].TenTaiKhoanNOTPOS;
                    }

                    let tienmat = qct.reduce(function (_this, item) {
                        return _this + item.TienMat;
                    }, 0);
                    let ck = qct.reduce(function (_this, item) {
                        return _this + item.TienGui;
                    }, 0);

                    if (tienmat > 0) {
                        pthuc = 'Tiền mặt,';
                    }
                    if (ck > 0) {
                        pthuc += 'Chuyển khoản,';
                    }
                    quyHD.ID_TaiKhoanNganHang = idNganHang;
                    quyHD.TenTaiKhoanNOTPOS = tenTK;
                    quyHD.PhuongThuc = pthuc;

                    if (isShowModal) {
                        self.loaiMenu = quyHD.LoaiHoaDon == 11 ? 0 : 1;
                        self.showModalUpdate(quyHD, qct);
                    }
                }
            })
        },
        ChangeCreator: function (item) {
            var self = this;
            self.newPhieuThu.TenNhanVien = item.TenNhanVien;
            self.newPhieuThu.ID_NhanVien = item.ID;
            self.newHoaDonTGT.ID_NhanVien = item.ID;
        },

        CaculatorDaThanhToan: function () {
            let self = this;
            let tienmat = formatNumberToFloat(self.newPhieuThu.TienMat);
            let tienck = formatNumberToFloat(self.newPhieuThu.TienCK);
            self.newPhieuThu.TongTienThu = tienmat + tienck;
            self.newHoaDonTGT.TienThua = tienmat + tienck - self.newHoaDonTGT.PhaiThanhToan;
        },
        ChangeAccountCK: function (item) {
            var self = this;
            self.ddl_textVal.accountCKName = item.TenChuThe;
            self.ddl_textVal.bankName = item.TenNganHang;
            self.newPhieuThu.ID_TaiKhoanChuyenKhoan = item.ID;
            self.newPhieuThu.ID_NganHang = item.ID_NganHang;
        },
        ResetAccountCK: function () {
            var self = this;
            self.ddl_textVal.accountCKName = '';
            self.ddl_textVal.bankName = '';
            self.newPhieuThu.TienCK = 0;
            self.newPhieuThu.ID_TaiKhoanChuyenKhoan = null;
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
        ChangeNgayLapPhieu: function (e) {
            let self = this;
            let dt = moment(e).format('YYYY-MM-DD HH:mm');
            let khoaSo = VHeader.CheckKhoaSo(moment(e).format('YYYY-MM-DD'), self.newPhieuThu.ID_DonVi);
            if (khoaSo) {
                ShowMessage_Danger(VHeader.warning.ChotSo.Update);
            }
            self.newPhieuThu.NgayLapHoaDon = dt;
            self.newHoaDonTGT.NgayLapHoaDon = dt;
        },
        EditTienMat: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var tienmat = formatNumberToFloat($this.val());
            self.newPhieuThu.TienMat = formatNumber(tienmat);
            self.CaculatorDaThanhToan();

            var key = event.keyCode || event.which;
            if (key === 13) {
                if (self.newPhieuThu.ID_TaiKhoanChuyenKhoan !== null) {
                    $this.closest('.form-group').next().find('input').select();
                }
            }
        },
        EditTienCK: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            self.newPhieuThu.TienCK = $this.val();
            self.CaculatorDaThanhToan();
        },

        CheckSave: function () {
            let self = this;
            let khoaSo = VHeader.CheckKhoaSo(moment(self.newPhieuThu.NgayLapHoaDon, 'YYYY-MM-DD HH:mm').format('YYYY-MM-DD'), self.newPhieuThu.ID_DonVi);
            if (khoaSo) {
                ShowMessage_Danger(VHeader.warning.ChotSo.Update);
                return false;
            }
            if (commonStatisJs.CheckNull(self.newPhieuThu.ID_NhanVien)) {
                commonStatisJs.ShowMessageDanger('Vui lòng chọn nhân viên lập phiếu');
                return;
            }
            if (commonStatisJs.CheckNull(self.newPhieuThu.ID_DoiTuong)) {
                commonStatisJs.ShowMessageDanger('Vui lòng chọn người nộp tiền');
                return false;
            }

            if (self.newPhieuThu.TongTienThu === 0) {
                commonStatisJs.ShowMessageDanger('Vui lòng nhập số tiền cần thanh toán');
                return false;
            }
            return true;
        },

        SaveHoaDonHoanTraTGT: function (print) {
            let self = this;
            let ptKhach = self.newPhieuThu;
            if (formatNumberToFloat(self.newHoaDonTGT.TongTienHang) === 0) {
                commonStatisJs.ShowMessageDanger('Vui lòng nhập số tiền hoàn trả');
                return;
            }
            if (self.newHoaDonTGT.TienThua < 0) {
                commonStatisJs.ShowMessageDanger('Vui lòng nhập đủ số tiền cần thanh toán');
                return;
            }
            self.isLoading = true;

            let myData = {
                objHoaDon: self.newHoaDonTGT,
            }
            ajaxHelper('/api/DanhMuc/Bh_HoaDonAPI/PostBH_HoaDonNapThe', 'POST', myData).done(function (x) {
                if (x.res === true) {
                    self.saveOK = true;
                    self.newHoaDonTGT.ID = x.dataSoure.ID;
                    self.newHoaDonTGT.MaHoaDon = x.dataSoure.MaHoaDon;

                    self.SavePhieuThu(print);
                }
                console.log(x);
            }).always(function () {
                self.isLoading = false;
                $('#vmThemMoiTheNap').modal('hide');
            })
        },

        SavePhieuThu: function (print) {
            let self = this;
            let checkSave = self.CheckSave();
            if (!checkSave) {
                return;
            }
            self.isLoading = true;

            let hdTGT = self.newHoaDonTGT;
            let idHoaDonLienQuan = hdTGT.ID;
            let ptKhach = self.newPhieuThu;
            let ghichu = ptKhach.NoiDungThu;
            let idDoiTuong = ptKhach.ID_DoiTuong;
            let idKhoanThuChi = ptKhach.ID_KhoanThuChi;
            let loaiThuChi = ptKhach.LoaiHoaDon;
            let tienmat = formatNumberToFloat(ptKhach.TienMat);
            let tienck = formatNumberToFloat(ptKhach.TienCK);
            let phuongthucTT = '';
            let mahoadon = ptKhach.MaHoaDon ? ptKhach.MaHoaDon : '';
            let lstQuyCT = [];
            let loaiThanhToan = 1;
            let inforTGT = '';
            if (self.formType === 1) {// tra lai tien khach da nap TheGiaTri
                loaiThanhToan = 0;
                ptKhach.PhieuDieuChinhCongNo = 0;
            }

            if (commonStatisJs.CheckNull(idHoaDonLienQuan) || idHoaDonLienQuan === '00000000-0000-0000-0000-000000000000') {
                idHoaDonLienQuan = null;
            }
            else {
                // caculator if mat + ck phaitt
                if (tienmat + tienck > hdTGT.PhaiThanhToan) {
                    if (tienck >= hdTGT.PhaiThanhToan) {
                        tienck = hdTGT.PhaiThanhToan;
                        tienck = tienmat;
                    }
                    else {
                        tienmat = hdTGT.PhaiThanhToan - tienck;
                    }
                    ptKhach.TongTienThu = tienmat + tienck;
                }
                inforTGT = ''.concat(' <br /> - Giá trị trả: ', hdTGT.TongTienHang,
                    '; Phí hoàn thẻ: ', hdTGT.TongGiamGia, ' (', hdTGT.TongChietKhau,
                    ' %) --> Tổng cần trả: ', formatNumber3Digit(hdTGT.PhaiThanhToan))
            }
            if (commonStatisJs.CheckNull(ghichu)) {
                ghichu = self.inforNguoiNop.TenDoiTuong.concat(' (', self.inforNguoiNop.MaDoiTuong, ')');
            }
            else {
                ghichu = ghichu.concat(' /', self.inforNguoiNop.TenDoiTuong, ' (', self.inforNguoiNop.MaDoiTuong, ')');
            }

            if (tienmat > 0) {
                let qct = newQuyChiTiet({
                    ID_KhoanThuChi: idKhoanThuChi,
                    ID_DoiTuong: idDoiTuong,
                    ID_HoaDonLienQuan: idHoaDonLienQuan,
                    GhiChu: ghichu,
                    TienThu: tienmat,
                    TienMat: tienmat,
                    HinhThucThanhToan: 1,
                    LoaiThanhToan: loaiThanhToan,
                });
                lstQuyCT.push(qct);
                phuongthucTT = 'Tiền mặt, ';
            }
            if (tienck > 0) {
                let qct = newQuyChiTiet({
                    ID_KhoanThuChi: idKhoanThuChi,
                    ID_DoiTuong: idDoiTuong,
                    ID_HoaDonLienQuan: idHoaDonLienQuan,
                    GhiChu: self.ddl_textVal.accountCKName.concat(' - ', self.ddl_textVal.bankName),
                    TienThu: tienck,
                    TienChuyenKhoan: tienck,
                    HinhThucThanhToan: 3,
                    LoaiThanhToan: loaiThanhToan,
                    ID_NganHang: ptKhach.ID_NganHang,
                    ID_TaiKhoanNganHang: ptKhach.ID_TaiKhoanChuyenKhoan,
                });
                lstQuyCT.push(qct);
                phuongthucTT += 'Chuyển khoản, ';
                ghichu += '/ ' + qct.GhiChu;
            }
            ptKhach.PhuongThucTT = Remove_LastComma(phuongthucTT);
            ptKhach.NoiDungThu = ghichu;

            var textFirst = "Tạo phiếu ";
            if (!self.isNew) {
                ptKhach.NguoiSua = self.inforLogin.UserLogin;
                textFirst = "Cập nhật phiếu ";
            }

            var myData = {
                objQuyHoaDon: ptKhach,
                objQuyHoaDonCT: lstQuyCT,
            }
            console.log('myData ', myData)
            $.ajax({
                data: myData,
                type: 'POST',
                url: '/api/DanhMuc/Quy_HoaDonAPI/Post_HDDatCoc',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            }).done(function (x) {
                self.isLoading = false;
                if (x.res) {
                    self.saveOK = true;
                    mahoadon = x.dataSoure;
                    ptKhach.MaHoaDon = mahoadon;

                    let sLoai = loaiThuChi == 11 ? 'thu' : 'chi';
                    let diary = {
                        LoaiNhatKy: 1,
                        ID_DonVi: ptKhach.ID_DonVi,
                        ID_NhanVien: ptKhach.ID_NhanVien,
                        ChucNang: 'Phiếu ' + sLoai,
                        NoiDung: textFirst.concat(sLoai, ' đặt cọc ', mahoadon, ', ',
                            self.sLoaiDoiTuong, ': ', ptKhach.NguoiNopTien,
                            ', với giá trị ', formatNumber(ptKhach.TongTienThu),
                            ', Phương thức thanh toán:', ptKhach.PhuongThucTT,
                            ', Thời gian: ', moment(ptKhach.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')),
                        NoiDungChiTiet: textFirst + sLoai + ' đặt cọc ' + ' <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD('.concat(mahoadon, ')" >', mahoadon, '</a> ',
                            '<br />- ', self.labelNopTra, ' (', self.sLoaiDoiTuong, '): ',
                            '<a style="cursor: pointer" onclick="LoadKhachHang_byMaKH(', ptKhach.NguoiNopTien, ')" >', ptKhach.NguoiNopTien, '</a> ',
                            inforTGT,
                            '<br />- Giá trị: ', formatNumber(ptKhach.TongTienThu),
                            '<br/ >- Phương thức thanh toán: ', ptKhach.PhuongThucTT,
                            '<br/ >- Thời gian: ', moment(ptKhach.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'),
                            '<br/ >- Khoản mục ', sLoai, ': ', self.ddl_textVal.khoanthu
                        )
                    }

                    if (self.isNew) {
                        commonStatisJs.ShowMessageSuccess('Tạo phiếu ' + sLoai + ' thành công');
                        diary.LoaiNhatKy = 1;
                    }
                    else {
                        commonStatisJs.ShowMessageSuccess('Cập nhật phiếu ' + sLoai + ' thành công');

                        let nguoiNopOld = '';
                        switch (self.newPhieuThu.LoaiDoiTuong) {
                            case 1:
                                nguoiNopOld = 'khách hàng';
                                break;
                            case 2:
                                nguoiNopOld = 'nhà cung cấp';
                                break;
                            case 3:
                                nguoiNopOld = 'Cty bảo hiểm';
                                break;
                        }
                        diary.LoaiNhatKy = 2;
                        diary.NoiDungChiTiet = diary.NoiDungChiTiet.concat(' <br /> <b> Thông tin cũ: </b> ',
                            ' <br />- Mã phiếu ', sLoai, ': ', self.inforOld.MaHoaDon,
                            ' <br />- Ngày lập phiếu: ', moment(self.inforOld.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss'),
                            ' <br />- Giá trị: ', formatNumber(self.inforOld.TongTienThu),
                            ' <br />- ', self.labelNopTra, ' (', nguoiNopOld, '): ', self.inforOld.NguoiNopTien,
                            ' <br />- Nhân viên lập: ', self.inforOld.TenNhanVien,
                            ' <br />- Phương thức thanh toán: ', self.inforOld.PhuongThuc,
                            ' <br/ >- Khoản mục ', sLoai, ': ', self.inforOld.NoiDungThuChi
                        );
                    }
                    Insert_NhatKyThaoTac_1Param(diary);

                    if (print) {
                        self.InPhieuThu(ptKhach);
                    }
                }
                else {
                    commonStatisJs.ShowMessageDanger(x.mess);
                }
                $('#NapTienDatCoc').modal('hide');
            })
        },

        HuyTienCoc_CheckVuotHanMuc: async function () {
            let self = this;
            let xx = await ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/' + 'HuyTienCoc_CheckVuotHanMuc/' + self.newPhieuThu.ID).done(function (x) {
            }).then(function (x) {
                return x;
            })
            return xx;
        },

        HuyPhieu: async function () {
            let self = this;
            let title = 'thu';
            if (self.newPhieuThu.LoaiHoaDon === 12) {
                title = 'chi';
            }

            let khoaSo = VHeader.CheckKhoaSo(moment(self.inforOld.NgayLapHoaDon).format('YYYY-MM-DD'), self.inforOld.ID_DonVi);
            if (khoaSo) {
                ShowMessage_Danger(VHeader.warning.ChotSo.Delete);
                return;
            }

            let overBudget = await self.HuyTienCoc_CheckVuotHanMuc();
            if (overBudget) {
                ShowMessage_Danger("Phiếu nạp cọc đã được sử dụng. Không thể hủy");
                return;
            }

            dialogConfirm('Xác nhận xóa', 'Bạn có chắc chắn muốn hủy phiếu ' + title + ' <b> ' + self.newPhieuThu.MaHoaDon + ' </b> không?', function () {
                ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/' + "DeleteQuy_HoaDon/" + self.newPhieuThu.ID, 'DELETE').done(function (x) {
                    if (x === "") {
                        ShowMessage_Success("Xóa sổ quỹ thành công");
                        $('#NapTienDatCoc').modal('hide');

                        let diary = {
                            ID_NhanVien: self.inforLogin.ID_NhanVien,
                            ID_DonVi: self.inforLogin.ID_DonVi,
                            ChucNang: 'Phiếu ' + title,
                            NoiDung: 'Hủy phiếu '.concat(title, ' tiền cọc ', self.newPhieuThu.MaHoaDon, ', nhân viên hủy: ', self.inforLogin.UserLogin),
                            NoiDungChiTiet: ''.concat('Hủy phiếu ', title, ' tiền cọc ', self.newPhieuThu.MaHoaDon, ', nhân viên hủy: ', self.inforLogin.UserLogin,
                                '<br /><b> Thông tin cũ: </b>',
                                '<br /> - Giá trị: ', formatNumber3Digit(self.inforOld.TongTienThu),
                                '<br /> - Phương thức thanh toán: ', self.inforOld.PhuongThuc,
                                '<br /> - Nhà cung cấp: ', self.inforOld.NguoiNopTien),
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
            let title = 'thu';
            if (self.newPhieuThu.LoaiHoaDon === 12) {
                title = 'chi';
            }
            dialogConfirm('Khôi phục', 'Bạn có chắc chắn muốn khôi phục phiếu ' + title + ' <b> ' + self.newPhieuThu.MaHoaDon + ' </b> không?', function () {

                ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/' + "KhoiPhucQuy_HoaDon/" + self.newPhieuThu.ID, 'GET').done(function (x) {
                    if (x.res) {
                        ShowMessage_Success("Khôi phục sổ quỹ thành công");
                        $('#ThuTienHoaDonModal').modal('hide');

                        let diary = {
                            ID_NhanVien: self.inforLogin.ID_NhanVien,
                            ID_DonVi: self.inforLogin.ID_DonVi,
                            ChucNang: 'Khôi phục phiếu ' + title,
                            NoiDung: 'Khôi phục phiếu '.concat(title, ' ', self.newPhieuThu.MaHoaDon, ', Người khôi phục: ', self.inforLogin.UserLogin),
                            NoiDungChiTiet: ''.concat(self.sForm, ': Khôi phục phiếu ', title, ' ', self.newPhieuThu.MaHoaDon, ', Người khôi phục: ', self.inforLogin.UserLogin),
                            LoaiNhatKy: 2
                        }
                        Insert_NhatKyThaoTac_1Param(diary);

                        if (self.listData.HoaDons.length > 0) {
                            let lstIDDoituong = $.unique(self.listData.HoaDons.map(function (x) {
                                return x.ID_DoiTuong;
                            }));
                            console.log('lstIDDoituong ', lstIDDoituong);// todo
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

        InPhieuThu: function (quyhd) {
            var self = this;
            var loaiCT = 'SQPT'
            if (quyhd.LoaiHoaDon === 12) {
                loaiCT = 'SQPC';
            }

            quyhd.TenCuaHang = self.inforCongTy.TenCongTy;
            quyhd.DiaChiCuaHang = self.inforCongTy.DiaChiCuaHang;
            quyhd.LogoCuaHang = self.inforCongTy.LogoCuaHang;

            let cn = VHeader.GetInforChiNhanh(quyhd.ID_DonVi);
            quyhd.TenChiNhanh = cn.TenChiNhanh;
            quyhd.DiaChiChiNhanh = cn.DiaChiChiNhanh;
            quyhd.DienThoaiChiNhanh = cn.DienThoaiChiNhanh;
            quyhd.ChiNhanhBanHang = cn.TenChiNhanh;

            let ngaylap = quyhd.NgayLapHoaDon;
            quyhd.MaPhieu = quyhd.MaHoaDon;
            quyhd.NgayLapHoaDon = moment(ngaylap).format('DD/MM/YYYY HH:mm:ss');
            quyhd.Ngay = moment(ngaylap).format('DD');
            quyhd.Thang = moment(ngaylap).format('MM');
            quyhd.Nam = moment(ngaylap).format('YYYY');
            quyhd.NguoiNopTien = quyhd.NguoiNopTien;
            quyhd.NguoiNhanTien = quyhd.NguoiNopTien;
            quyhd.DiaChiKhachHang = self.inforNguoiNop.DiaChi;
            quyhd.DienThoaiKhachHang = self.inforNguoiNop.SoDienThoai;
            quyhd.NoiDungThu = quyhd.NoiDungThu;
            quyhd.TienBangChu = DocSo(quyhd.TongTienThu);
            quyhd.ChuyenKhoan = formatNumber(quyhd.TienCK);
            quyhd.TienMat = formatNumber(quyhd.TienMat);
            quyhd.GiaTriPhieu = formatNumber(quyhd.TongTienThu);
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
    computed: {
        sLoaiThuChi: function () {
            let self = this;
            let sLoai = '';
            switch (self.newPhieuThu.LoaiHoaDon) {
                case 11:
                    sLoai = 'thu';
                    break;
                case 12:
                    sLoai = 'chi';
                    break;
            }
            return sLoai;
        },
        sLoaiDoiTuong: function () {
            let self = this;
            let sLoai = '';
            switch (self.newPhieuThu.LoaiDoiTuong) {
                case 1:
                    sLoai = 'khách hàng';
                    break;
                case 2:
                    sLoai = 'nhà cung cấp';
                    break;
                case 3:
                    sLoai = 'Cty bảo hiểm';
                    break;
            }
            return sLoai;
        },
        labelNopTra: function () {
            let self = this;
            if (self.isNapTien) {
                if (self.newPhieuThu.LoaiHoaDon === 12) {
                    return "Nộp cho";
                }
                return "Người nộp";
            }
            else {
                return "Trả lại cho";
            }
        },
        roleInsert_CusVen: function () {
            let self = this;
            let role = false;
            switch (self.newPhieuThu.LoaiDoiTuong) {
                case 1:
                    role = self.role.Customer.Insert;
                    break;
                case 2:
                    role = self.role.Vendor.Insert;
                    break;
                case 3:
                    break;
            }
            return role;
        },
    }
})
