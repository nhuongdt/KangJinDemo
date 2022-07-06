var vmThanhToan = new Vue({
    el: '#ThongTinThanhToanNCC',
    components: {
        'account-bank': cmpChoseAccountBank,
        'khoan-thu-chi': cmpChoseKhoanThu,
        'nvien-hoadon-search': cmpSearchNVDisscount,
    },
    created: function () {
        this.GuidEmpty = '00000000-0000-0000-0000-000000000000';
        this.khoanthuchi = {
            ID: null,
            NoiDungThuChi: '',
        };
    },
    computed: {
        LaKhoanThu: function () {
            let self = this;
            let loaiHD = self.inforHoaDon.LoaiHoaDon;
            if (commonStatisJs.CheckNull(loaiHD)) {
                loaiHD = 4;
            }
            loaiHD = parseInt(loaiHD);
            return loaiHD === 7 || (loaiHD === 4 && self.inforHoaDon.HoanTra > 0);
        }
    },
    data: {
        saveOK: false,
        isNew: true,
        isGara: false,
        isCheckTraLaiCoc: false,
        itemChosing: {},
        khoanthuchi: {
            ID: null,
            NoiDungThuChi: '',
        },

        inforHoaDon: {
            ID: null,
            LoaiDoiTuong: 1,// 1.kh, 2.ncc, 3.bh
            LoaiHoaDon: 1,
            ID_DoiTuong: null,
            ID_BaoHiem: null,
            SoDuDatCoc: 0,
            HoanTra: 0,
            PhaiThanhToan: 0,
            TongThanhToan: 0,
            TongTichDiem: 0,
            ThucThu: 0,
            ConNo: 0,
            DienGiai: '',
            MaDoiTuong: '',
            TenDoiTuong: '',
            MaHoaDon: '',
            ID_DonVi: null,
            ID_NhanVien: null,
            NgayLapHoaDon: null,
            ID_PhieuTiepNhan: null,
        },
        PhieuThuKhach: {},
        listData: {
            AccountBanks: [],
            KhoanThuChis: [],
            NhanViens: [],
        },
        PhieuThuKhachPrint: {},
    },
    methods: {
        newPhieuThu: function (loaiDoiTuong) {
            return {
                LoaiDoiTuong: loaiDoiTuong,
                MaHoaDon: '',
                NgayLapHoaDon: moment(new Date()).format('YYYY-MM-DD HH:mm'),
                TienDatCoc: 0,
                TienMat: 0,
                TienPOS: 0,
                TienCK: 0,
                TienTheGiaTri: 0,
                DiemQuyDoi: 0,
                TTBangDiem: 0,
                ID_TaiKhoanPos: null,
                ID_TaiKhoanChuyenKhoan: null,
                ID_NhanVien: null,
                ID_KhoanThuChi: null,
                NoHienTai: 0,
                PhaiThanhToan: 0,// sotien phaitt sau khi nhap tiencoc
                DaThanhToan: 0,
                TienThua: 0,
                ThucThu: 0,
                TenTaiKhoanPos: '',//= ten chu the
                TenTaiKhoanCK: '',
                SoTaiKhoanPos: '',
                SoTaiKhoanCK: '',
                TenNganHangPos: '',
                TenNganHangCK: '',
                HoanTraTamUng: 0,
            };
        },
        GetKhoanThuChi_byLoaiChungTu: function (lakhoanthu = true) {
            let self = this;
            let ktc = $.grep(self.listData.KhoanThuChis, function (x) {
                return x.LoaiChungTu.indexOf(self.inforHoaDon.LoaiHoaDon) > -1 && x.LaKhoanThu === lakhoanthu;
            });
            return ktc;
        },
        showModalThanhToan: function (hd) {
            console.log('hd ', hd)
            var self = this;
            self.saveOK = false;
            self.isCheckTraLaiCoc = false;
            self.inforHoaDon = hd;
            self.PhieuThuKhach = self.newPhieuThu(2);

            // set default tienmat phaitt for khachhang
            var tienmat = 0, tienPOS = 0, tienCK = 0;
            var tiendatcoc = 0, datt = 0, cantt = 0;
            let khachDaTra = formatNumberToFloat(hd.KhachDaTra); // used to update again PhieuNhapHang
            var hoantra = hd.HoanTra;
            var soduDatCoc = hd.SoDuDatCoc;
            var phaiTT = self.inforHoaDon.PhaiThanhToan - khachDaTra;

            let cacheName = 'lcHDNhapHang'
            if (hd.LoaiHoaDon === 7) {
                cacheName = 'lcHDTraHangNhap';
            }
            var lstHD = localStorage.getItem(cacheName);
            if (lstHD !== null) {
                lstHD = JSON.parse(lstHD);
            }
            else {
                lstHD = [];
            }

            let ktc = [];
            if (lstHD.length > 0) {
                let itHD = lstHD[0];
                if (!commonStatisJs.CheckNull(hd.IDRandom)) {
                    for (let i = 0; i < lstHD.length; i++) {
                        if (lstHD[i].IDRandom === hd.IDRandom) {
                            itHD = lstHD[i];
                            break;
                        }
                    }
                }

                if (!commonStatisJs.CheckNull(itHD.TienMat)) {
                    tienmat = formatNumberToFloat(itHD.TienMat);
                }
                if (!commonStatisJs.CheckNull(itHD.TienPOS)) {
                    tienPOS = formatNumberToFloat(itHD.TienPOS);
                }
                if (!commonStatisJs.CheckNull(itHD.TienChuyenKhoan)) {
                    tienCK = formatNumberToFloat(itHD.TienChuyenKhoan);
                }
                if (!commonStatisJs.CheckNull(itHD.TienDatCoc)) {
                    tiendatcoc = formatNumberToFloat(itHD.TienDatCoc);
                }

                if (!commonStatisJs.CheckNull(itHD.ID_TaiKhoanPos)) {
                    self.PhieuThuKhach.ID_TaiKhoanPos = itHD.ID_TaiKhoanPos;
                }
                if (!commonStatisJs.CheckNull(itHD.TenTaiKhoanPos)) {
                    self.PhieuThuKhach.TenTaiKhoanPos = itHD.TenTaiKhoanPos;
                }
                if (!commonStatisJs.CheckNull(itHD.ID_TaiKhoanChuyenKhoan)) {
                    self.PhieuThuKhach.ID_TaiKhoanChuyenKhoan = itHD.ID_TaiKhoanChuyenKhoan;
                }
                if (!commonStatisJs.CheckNull(itHD.TenTaiKhoanCK)) {
                    self.PhieuThuKhach.TenTaiKhoanCK = itHD.TenTaiKhoanCK;
                }

                if (!commonStatisJs.CheckNull(itHD.ID_KhoanThuChi)) {
                    self.PhieuThuKhach.ID_KhoanThuChi = itHD.ID_KhoanThuChi;
                    ktc = $.grep(self.listData.KhoanThuChis, function (x) {
                        return x.ID === itHD.ID_KhoanThuChi;
                    });
                }
            }

            if (ktc.length > 0) {
                self.PhieuThuKhach.ID_KhoanThuChi = ktc[0].ID;
                self.khoanthuchi.ID = ktc[0].ID;
                self.khoanthuchi.NoiDungThuChi = ktc[0].NoiDungThuChi;
            }
            else {
                ktc = self.GetKhoanThuChi_byLoaiChungTu(self.LaKhoanThu);
                if (ktc.length > 0) {
                    // set default khoanthuchi by loaichungtu
                    self.PhieuThuKhach.ID_KhoanThuChi = ktc[0].ID;
                    self.khoanthuchi.ID = ktc[0].ID;
                    self.khoanthuchi.NoiDungThuChi = ktc[0].NoiDungThuChi;
                }
                else {
                    self.PhieuThuKhach.ID_KhoanThuChi = null;
                    self.khoanthuchi.ID = null;
                    self.khoanthuchi.NoiDungThuChi = '';
                }
            }
            datt = tienmat + tienPOS + tienCK + tiendatcoc;

            // check and assign HoanTraTamUng, PhaiThanhToan of PhieuThu
            if (datt === 0) {
                if (hoantra <= 0) {
                    if (soduDatCoc > phaiTT) {
                        tiendatcoc = phaiTT;
                        tienmat = soduDatCoc - tiendatcoc;// tra lai coc
                        self.PhieuThuKhach.HoanTraTamUng = tienmat;
                    }
                    else {
                        tiendatcoc = soduDatCoc;
                        tienmat = phaiTT - tiendatcoc;
                    }
                    self.PhieuThuKhach.PhaiThanhToan = phaiTT;
                }
                else {
                    tienmat = hoantra;
                    self.PhieuThuKhach.PhaiThanhToan = hoantra;
                    self.PhieuThuKhach.HoanTraTamUng = hoantra;
                }
            }
            else {
                if (commonStatisJs.CheckNull(hd.isCheckTraLaiCoc)) {
                    self.isCheckTraLaiCoc = false;
                }
                else {
                    self.isCheckTraLaiCoc = hd.isCheckTraLaiCoc;
                }

                if (soduDatCoc > phaiTT) {
                    if (tiendatcoc >= phaiTT) {
                        self.PhieuThuKhach.HoanTraTamUng = soduDatCoc - tiendatcoc;
                        self.PhieuThuKhach.PhaiThanhToan = self.PhieuThuKhach.HoanTraTamUng;
                    }
                    else {
                        self.PhieuThuKhach.PhaiThanhToan = phaiTT - tiendatcoc;
                    }
                }
                else {
                    self.PhieuThuKhach.PhaiThanhToan = phaiTT - tiendatcoc;
                }
            }
            datt = tienmat + tienPOS + tienCK + tiendatcoc;

            self.PhieuThuKhach.TienDatCoc = formatNumber3Digit(tiendatcoc, 2);
            self.PhieuThuKhach.TienMat = formatNumber3Digit(tienmat, 2);
            self.PhieuThuKhach.TienPOS = formatNumber3Digit(tienPOS, 2);
            self.PhieuThuKhach.TienCK = formatNumber3Digit(tienCK, 2);
            self.PhieuThuKhach.DaThanhToan = datt;
            self.PhieuThuKhach.TienThua = datt - phaiTT;

            $('#ThongTinThanhToanNCC').modal('show');
        },
        ChangeCheckTraLaiCoc: function () {
            let self = this;
            if (self.isCheckTraLaiCoc) {
                self.PhieuThuKhach.TienMat = formatNumber3Digit(self.PhieuThuKhach.HoanTraTamUng);
            }
            else {
                self.PhieuThuKhach.TienMat = 0;
            }
            self.PhieuThuKhach.TienPOS = 0;
            self.PhieuThuKhach.TienCK = 0;
            self.CaculatorDaThanhToan();
        },
        CaculatorDaThanhToan: function () {
            var self = this;
            var tiencoc = formatNumberToFloat(self.PhieuThuKhach.TienDatCoc);
            if (self.PhieuThuKhach.HoanTraTamUng > 0 && self.inforHoaDon.SoDuDatCoc > 0) {
                // neu chi tra tien: khong tinh tiencoc
                tiencoc = 0;
            }
            self.PhieuThuKhach.DaThanhToan = formatNumberToFloat(self.PhieuThuKhach.TienMat)
                + tiencoc
                + formatNumberToFloat(self.PhieuThuKhach.TienPOS)
                + formatNumberToFloat(self.PhieuThuKhach.TienCK)
                + formatNumberToFloat(self.PhieuThuKhach.TienTheGiaTri)
                + formatNumberToFloat(self.PhieuThuKhach.TTBangDiem);
            self.PhieuThuKhach.ThucThu = formatNumberToFloat(self.PhieuThuKhach.TienMat)
                + formatNumberToFloat(self.PhieuThuKhach.TienPOS)
                + formatNumberToFloat(self.PhieuThuKhach.TienCK);

            if (self.PhieuThuKhach.HoanTraTamUng > 0) {
                self.PhieuThuKhach.TienThua = self.PhieuThuKhach.DaThanhToan - self.PhieuThuKhach.PhaiThanhToan;
            }
            else {
                self.PhieuThuKhach.TienThua = self.PhieuThuKhach.DaThanhToan + formatNumberToFloat(self.inforHoaDon.KhachDaTra) - self.inforHoaDon.PhaiThanhToan;
            }
        },
        ResetAccountPOS: function () {
            var self = this;
            self.PhieuThuKhach.TenTaiKhoanPos = '';
            self.PhieuThuKhach.SoTaiKhoanPos = '';
            self.PhieuThuKhach.TenNganHangPos = '';
            self.PhieuThuKhach.ID_TaiKhoanPos = null;
            self.PhieuThuKhach.TienPOS = 0;
            self.CaculatorDaThanhToan();
        },
        ChangeAccountPOS: function (item) {
            var self = this;
            self.PhieuThuKhach.TenTaiKhoanPos = item.TenChuThe;
            self.PhieuThuKhach.SoTaiKhoanPos = item.SoTaiKhoan;
            self.PhieuThuKhach.TenNganHangPos = item.TenNganHang;
            self.PhieuThuKhach.ID_TaiKhoanPos = item.ID;
        },
        ChangeAccountCK: function (item) {
            var self = this;
            self.PhieuThuKhach.TenTaiKhoanCK = item.TenChuThe;
            self.PhieuThuKhach.SoTaiKhoanCK = item.SoTaiKhoan;
            self.PhieuThuKhach.TenNganHangCK = item.TenNganHang;
            self.PhieuThuKhach.ID_TaiKhoanChuyenKhoan = item.ID;
        },
        ResetAccountCK: function () {
            var self = this;
            self.PhieuThuKhach.TenTaiKhoanCK = '';
            self.PhieuThuKhach.ID_TaiKhoanChuyenKhoan = null;
            self.PhieuThuKhach.SoTaiKhoanCK = '';
            self.PhieuThuKhach.TenNganHangCK = '';
            self.PhieuThuKhach.TienCK = 0;
            self.CaculatorDaThanhToan();
        },
        ChangeKhoanThu: function (item) {
            var self = this;
            self.PhieuThuKhach.ID_KhoanThuChi = item.ID;
            self.khoanthuchi.ID = item.ID;
            self.khoanthuchi.NoiDungThuChi = item.NoiDungThuChi;
        },
        ResetKhoanThu: function () {
            var self = this;
            self.PhieuThuKhach.ID_KhoanThuChi = null;
            self.khoanthuchi.ID = null;
            self.khoanthuchi.NoiDungThuChi = '';
        },

        KH_EditTienCoc: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var tienmat = 0;
            var khachdatra = formatNumberToFloat(self.inforHoaDon.KhachDaTra);
            var khachcantra = formatNumberToFloat(self.inforHoaDon.PhaiThanhToan) - khachdatra;
            var soduDatCoc = self.inforHoaDon.SoDuDatCoc;
            var datcoc = formatNumberToFloat($this.val());

            if (datcoc <= soduDatCoc) {
                if (datcoc <= khachcantra) {
                    tienmat = khachcantra - datcoc;
                }
                else {
                    datcoc = khachcantra;
                    tienmat = soduDatCoc - datcoc;// tra lai
                }
            }
            else {
                if (soduDatCoc <= khachcantra) {
                    datcoc = soduDatCoc;
                    tienmat = khachcantra - datcoc;
                }
                else {
                    datcoc = khachcantra;
                    tienmat = soduDatCoc - datcoc; // tralai
                }
            }
            self.PhieuThuKhach.TienDatCoc = formatNumber3Digit(datcoc, 2);

            if (datcoc === khachcantra && tienmat > 0) {
                // tra lai coc
                self.PhieuThuKhach.HoanTraTamUng = tienmat;
            }
            else {
                self.PhieuThuKhach.HoanTraTamUng = 0;
                self.isCheckTraLaiCoc = false;
            }

            self.PhieuThuKhach.TienDatCoc = formatNumber3Digit(datcoc, 2);
            self.PhieuThuKhach.TienMat = formatNumber3Digit(tienmat, 2);
            self.PhieuThuKhach.PhaiThanhToan = tienmat;

            self.PhieuThuKhach.TienPOS = 0;
            self.PhieuThuKhach.TienCK = 0;
            self.PhieuThuKhach.TienTheGiaTri = 0;
            self.CaculatorDaThanhToan();

            var key = event.keyCode || event.which;
            if (key === 13) {
                $this.parent().next().find('input').focus();
            }
        },

        KH_EditTienMat: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var tienmat = formatNumberToFloat($this.val());
            self.PhieuThuKhach.TienMat = formatNumber3Digit(tienmat, 2);

            var tienpos = 0, tienck = 0;
            if (self.PhieuThuKhach.ID_TaiKhoanPos !== null) {
                tienpos = self.PhieuThuKhach.PhaiThanhToan - tienmat;
                tienpos = tienpos > 0 ? tienpos : 0;
                tienck = 0;
            }
            else {
                if (self.PhieuThuKhach.ID_TaiKhoanChuyenKhoan !== null) {
                    tienck = self.PhieuThuKhach.PhaiThanhToan - tienmat;
                    tienck = tienck > 0 ? tienck : 0;
                }
                else {

                }
            }
            self.PhieuThuKhach.TienPOS = formatNumber3Digit(tienpos, 2);
            self.PhieuThuKhach.TienCK = formatNumber3Digit(tienck, 2);
            self.CaculatorDaThanhToan();

            var key = event.keyCode || event.which;
            if (key === 13) {
                if (self.PhieuThuKhach.ID_TaiKhoanPos !== null) {
                    $this.closest('.container-fluid').next().find('input').select();
                }
                else {
                    if (self.PhieuThuKhach.ID_TaiKhoanChuyenKhoan !== null) {
                        $this.closest('.container-fluid').next().next().find('input').select();
                    }
                    else {

                    }
                }
            }
        },
        KH_EditTienPos: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var tienpos = formatNumberToFloat($this.val());
            self.PhieuThuKhach.TienPOS = $this.val();

            var tienck = 0;
            var tienmat = formatNumberToFloat(self.PhieuThuKhach.TienMat);
            if (self.PhieuThuKhach.ID_TaiKhoanChuyenKhoan !== null) {
                tienck = self.PhieuThuKhach.PhaiThanhToan - tienmat - tienpos;
                tienck = tienck < 0 ? 0 : tienck;
            }
            self.PhieuThuKhach.TienCK = formatNumber3Digit(tienck, 2);
            self.CaculatorDaThanhToan();

            var key = event.keyCode || event.which;
            if (key === 13) {
                if (self.PhieuThuKhach.ID_TaiKhoanChuyenKhoan !== null) {
                    $this.closest('.container-fluid').next().find('input').select();
                }
                else {
                }
            }
        },
        KH_EditTienCK: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            self.PhieuThuKhach.TienCK = $this.val();
            self.CaculatorDaThanhToan();

            var key = event.keyCode || event.which;
            if (key === 13) {
            }
        },

        shareMoney_QuyHD: function (phaiTT, tienDiem, tienmat, tienPOS, chuyenkhoan, thegiatri, tiencoc) {
            if (tiencoc >= phaiTT) {
                return {
                    TienCoc: phaiTT,
                    TTBangDiem: 0,
                    TienMat: 0,
                    TienPOS: 0,
                    TienChuyenKhoan: 0,
                    TienTheGiaTri: 0,
                }
            }
            else {
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
        },
        // use gara
        AgreeThanhToan: function () {
            modelGiaoDich.SaveInvoice(0);
            $('#ThongTinThanhToanNCC').modal('hide');
        },

        SavePhieuThu_Default: function (hd) {
            var loaiThuChi = hd.LoaiHoaDon === 7 ? 11 : 12;
            var sLoai = 'thu';
            let lstQuyCT = [];
            let daTT = formatNumberToFloat(hd.DaThanhToan);
            var soduDatCoc = hd.SoDuDatCoc;
            let idDoiTuong = hd.ID_DoiTuong;
            idDoiTuong = idDoiTuong ? idDoiTuong : '00000000-0000-0000-0000-000000000002';
            let idHoaDon = hd.ID;
            let tenDoiTuong = hd.TenDoiTuong;
            var ghichu = hd.DienGiai;
            var idKhoanThuChi = null;
            var tiendatcoc = 0;
            if (hd.HoanTra > 0) {
                loaiThuChi = 11;
                sLoai = 'chi';
            }
            if (daTT > 0 || (soduDatCoc > 0 && loaiThuChi === 12)) {
                let phuongthucTT = '';

                if (soduDatCoc > 0 && loaiThuChi === 12) {
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: idHoaDon,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ghichu,
                        TienThu: soduDatCoc,
                        TienCoc: soduDatCoc,
                        HinhThucThanhToan: 6,
                    });
                    lstQuyCT.push(qct);
                    phuongthucTT += 'Tiền cọc, ';
                }
                if (daTT > 0) {
                    if (tiendatcoc > 0) {
                        let qct = newQuyChiTiet({
                            ID_HoaDonLienQuan: idHoaDon,
                            ID_KhoanThuChi: idKhoanThuChi,
                            ID_DoiTuong: idDoiTuong,
                            GhiChu: ghichu,
                            TienThu: tiendatcoc,
                            TienCoc: tiendatcoc,
                            HinhThucThanhToan: 6,
                        });
                        lstQuyCT.push(qct);
                        phuongthucTT = 'Trả lại tiền đặt cọc';
                    }
                    else {
                        let qct = newQuyChiTiet({
                            ID_HoaDonLienQuan: idHoaDon,
                            ID_KhoanThuChi: idKhoanThuChi,
                            ID_DoiTuong: idDoiTuong,
                            GhiChu: ghichu,
                            TienThu: daTT,
                            TienMat: daTT,
                            HinhThucThanhToan: 1,
                        });
                        lstQuyCT.push(qct);
                        phuongthucTT += 'Tiền mặt, ';
                    }
                }

                // tao phieuthu tu HD datcoc
                var nowSeconds = (new Date()).getSeconds() + 1;
                let quyhd = {
                    LoaiHoaDon: loaiThuChi,
                    TongTienThu: daTT + (loaiThuChi == 12 ? soduDatCoc : 0),// chi cong tien coc neu la phieuchi
                    MaHoaDon: 'TT' + hd.MaHoaDon,
                    NgayLapHoaDon: moment(hd.NgayLapHoaDon).add(nowSeconds, 'seconds').format('YYYY-MM-DD HH:mm:ss'),
                    NguoiNopTien: tenDoiTuong,
                    NguoiTao: hd.NguoiTao,
                    NoiDungThu: ghichu,
                    ID_NhanVien: hd.ID_NhanVien,
                    ID_DonVi: hd.ID_DonVi,
                    ID_DoiTuong: idDoiTuong,
                }

                var myData = {
                    objQuyHoaDon: quyhd,
                    lstCTQuyHoaDon: lstQuyCT
                };

                ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/PostQuy_HoaDon_DefaultIDDoiTuong', 'POST', myData).done(function (x) {
                    console.log(x)
                    if (x.res === true) {
                        quyhd.MaHoaDon = x.data.MaHoaDon;
                        let diary = {
                            LoaiNhatKy: 1,
                            ID_DonVi: quyhd.ID_DonVi,
                            ID_NhanVien: quyhd.ID_NhanVien,
                            ChucNang: 'Phiếu ' + sLoai,
                            NoiDung: 'Tạo phiếu '.concat(sLoai, ' ', quyhd.MaHoaDon, ' cho hóa đơn ', hd.MaHoaDon,
                                ', Nhà cung cấp: ', quyhd.NguoiNopTien, ', với giá trị ', formatNumber(quyhd.TongTienThu),
                                ', Phương thức thanh toán:', phuongthucTT,
                                ', Thời gian: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')),
                            NoiDungChiTiet: 'Tạo phiếu ' + sLoai + ' <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD('.concat(quyhd.MaHoaDon, ')" >', quyhd.MaHoaDon, '</a> ',
                                ' cho hóa đơn: <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD(', hd.MaHoaDon, ')" >', hd.MaHoaDon, '</a> ',
                                ', Nhà cung cấp: <a style="cursor: pointer" onclick = "LoadKhachHang_byMaKH(', quyhd.NguoiNopTien, ')" >', quyhd.NguoiNopTien, '</a> ',
                                '<br /> Giá trị: ', formatNumber(quyhd.TongTienThu),
                                '<br/ > Phương thức thanh toán: ', phuongthucTT,
                                '<br/ > Thời gian: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')
                            )
                        }
                        Insert_NhatKyThaoTac_1Param(diary);
                    }
                })
            }
        },


        SavePhieuThu: function () {
            var self = this;
            var hd = self.inforHoaDon;
            let ghichu = ''.concat(hd.TenDoiTuong, ' (', hd.MaDoiTuong, ') / ', hd.MaHoaDon);
            var idHoaDon = hd.ID;
            let idDoiTuong = hd.ID_DoiTuong;
            idDoiTuong = idDoiTuong ? idDoiTuong : '00000000-0000-0000-0000-000000000002';
            let tenDoiTuong = hd.TenDoiTuong;
            var ptKhach = self.PhieuThuKhach;
            let idKhoanThuChi = ptKhach.ID_KhoanThuChi;
            console.log('save pt')
            var loaiThuChi = hd.LoaiHoaDon === 7 ? 11 : 12;
            var sLoai = hd.LoaiHoaDon === 7 ? 'thu' : 'chi';
            var tiendatcoc = formatNumberToFloat(ptKhach.TienDatCoc), soduDatCoc = hd.SoDuDatCoc;
            var maPhieuThuChi = 'TT' + hd.MaHoaDon;
            var chitracoc = self.isCheckTraLaiCoc;
            var khach_PhieuThuTT = hd.PhaiThanhToan - formatNumberToFloat(hd.KhachDaTra);
            if ((hd.HoanTra > 0 && soduDatCoc <= 0) || ptKhach.HoanTraTamUng > 0) {
                sLoai = 'thu';
                loaiThuChi = 11;
            }
            if (ptKhach.HoanTraTamUng > 0) {
                khach_PhieuThuTT = ptKhach.PhaiThanhToan;
                if (tiendatcoc > 0) {
                    idHoaDon = null; // neu tiencoc > 0 & hoantra tiencho khach --> khong gan idHoaDon (vi no bi bu tru congno)
                }
            }

            //  used to get save diary
            if (ptKhach.DaThanhToan > 0) {
                let lstQuyCT = [];
                let phuongthucTT = '';
                let dataReturn = self.shareMoney_QuyHD(khach_PhieuThuTT, ptKhach.TTBangDiem,
                    formatNumberToFloat(ptKhach.TienMat), formatNumberToFloat(ptKhach.TienPOS),
                    formatNumberToFloat(ptKhach.TienCK), 0,
                    ptKhach.HoanTraTamUng > 0 ? 0 : tiendatcoc);// nếu hoàn trả tiền: không gán tiền cọc ở đây

                tiendatcoc = dataReturn.TienCoc;
                tienmat = dataReturn.TienMat;
                tienpos = dataReturn.TienPOS;
                tienck = dataReturn.TienChuyenKhoan;
                tongthu = tienmat + tienpos + tienck + tiendatcoc;

                //todo: gán lai tiền khách đã trả/ tiền trả khách --> use when print
                self.PhieuThuKhachPrint.TienDatCoc = tiendatcoc;
                self.PhieuThuKhachPrint.TienMat = tienmat;
                self.PhieuThuKhachPrint.TienCK = tienck;
                self.PhieuThuKhachPrint.TienPOS = tienpos;
                self.PhieuThuKhachPrint.DaThanhToan = tongthu;

                if (commonStatisJs.CheckNull(idKhoanThuChi)) {
                    let ktc = self.GetKhoanThuChi_byLoaiChungTu(loaiThuChi === 11);
                    if (ktc.length > 0) {
                        idKhoanThuChi = ktc[0].ID;
                        self.khoanthuchi.NoiDungThuChi = ktc[0].NoiDungThuChi;
                    }
                }

                // thu tiền cọc
                if (tiendatcoc > 0 && loaiThuChi === 12) {
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: idHoaDon,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ghichu,
                        TienThu: tiendatcoc,
                        TienCoc: tiendatcoc,
                        HinhThucThanhToan: 6,
                    });
                    lstQuyCT.push(qct);
                    phuongthucTT = 'Tiền cọc, ';
                }

                if (tienmat > 0) {
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: idHoaDon,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ghichu,
                        TienThu: tienmat,
                        TienMat: tienmat,
                        HinhThucThanhToan: 1,
                        LoaiThanhToan: chitracoc ? 1 : 0,
                    });
                    lstQuyCT.push(qct);
                    phuongthucTT += 'Tiền mặt, ';
                }

                if (tienpos > 0) {
                    khach_idPos = ptKhach.ID_TaiKhoanPos;
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: idHoaDon,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ''.concat(ptKhach.TenTaiKhoanPos, ' - ', ptKhach.TenNganHangPos),
                        TienThu: tienpos,
                        TienPOS: tienpos,
                        HinhThucThanhToan: 2,
                        ID_TaiKhoanNganHang: ptKhach.ID_TaiKhoanPos,
                        LoaiThanhToan: chitracoc ? 1 : 0,
                    });
                    lstQuyCT.push(qct);
                    phuongthucTT += 'POS, ';
                    ghichu += '/ '.concat(qct.GhiChu, ', ');
                }
                if (tienck > 0) {
                    khach_idCK = ptKhach.ID_TaiKhoanChuyenKhoan;
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: idHoaDon,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ' /'.concat(ptKhach.TenTaiKhoanCK, ' - ', ptKhach.TenNganHangCK),
                        TienThu: tienck,
                        TienChuyenKhoan: tienck,
                        HinhThucThanhToan: 3,
                        ID_TaiKhoanNganHang: ptKhach.ID_TaiKhoanChuyenKhoan,
                        LoaiThanhToan: chitracoc ? 1 : 0,
                    });
                    lstQuyCT.push(qct);
                    phuongthucTT += 'Chuyển khoản, ';
                    ghichu += '/ ' + qct.GhiChu;
                }

                //if (commonStatisJs.CheckNull(idDoiTuong) && tongthu < ptKhach.DaThanhToan) {
                //    commonStatisJs.ShowMessageDanger("Là khách lẻ, không cho phép nợ");
                //    return;
                //}
                var nowSeconds = (new Date()).getSeconds() + 1;
                let quyhd = {
                    LoaiHoaDon: loaiThuChi,
                    TongTienThu: tongthu,
                    MaHoaDon: maPhieuThuChi,
                    NgayLapHoaDon: moment(hd.NgayLapHoaDon).add(nowSeconds, 'seconds').format('YYYY-MM-DD HH:mm:ss'),
                    NguoiNopTien: tenDoiTuong,
                    NguoiTao: hd.NguoiTao,
                    NoiDungThu: ghichu,
                    ID_NhanVien: hd.ID_NhanVien,
                    ID_DonVi: hd.ID_DonVi,
                    // used to get save diary 
                    MaHoaDonTraHang: hd.MaHoaDon,
                    ID_DoiTuong: idDoiTuong,
                }
                phuongthucTT = Remove_LastComma(phuongthucTT);
                quyhd.PhuongThucTT = phuongthucTT;

                let myData = {
                    objQuyHoaDon: quyhd,
                    lstCTQuyHoaDon: lstQuyCT
                };

                if (lstQuyCT.length > 0) {
                    console.log('myData_quyKH ', myData);
                    ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/PostQuy_HoaDon_DefaultIDDoiTuong', 'POST', myData).done(function (x) {
                        console.log(x)
                        if (x.res === true) {
                            // update ID_QuyHoaDon in BH_NhanVienThucHien by ID_HoaDon
                            quyhd.MaHoaDon = x.data.MaHoaDon;
                            let diary = {
                                LoaiNhatKy: 1,
                                ID_DonVi: quyhd.ID_DonVi,
                                ID_NhanVien: quyhd.ID_NhanVien,
                                ChucNang: 'Phiếu ' + sLoai,
                                NoiDung: 'Tạo phiếu '.concat(sLoai, ' ', quyhd.MaHoaDon, ' cho hóa đơn ', hd.MaHoaDon,
                                    ', Nhà cung cấp: ', quyhd.NguoiNopTien, ', với giá trị ', formatNumber3Digit(quyhd.TongTienThu),
                                    ', Phương thức thanh toán: ', phuongthucTT, chitracoc ? ' (NCC trả lại tiền cọc)' : '',
                                    ', Thời gian: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')),
                                NoiDungChiTiet: 'Tạo phiếu ' + sLoai + ' <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD('.concat(quyhd.MaHoaDon, ')" >', quyhd.MaHoaDon, '</a> ',
                                    ' cho hóa đơn: <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD(', hd.MaHoaDon, ')" >', hd.MaHoaDon, '</a> ',
                                    '<br /> Nhà cung cấp: <a style="cursor: pointer" onclick = "LoadKhachHang_byMaKH(', quyhd.NguoiNopTien, ')" >', quyhd.NguoiNopTien, '</a> ',
                                    '<br /> Giá trị: ', formatNumber3Digit(quyhd.TongTienThu),
                                    '<br /> Khoản ', sLoai, ': ', self.khoanthuchi.NoiDungThuChi,
                                    '<br/ > Phương thức thanh toán: ', phuongthucTT, chitracoc ? ' (NCC trả lại tiền cọc)' : '',
                                    '<br/ > Thời gian: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')
                                )
                            }
                            Insert_NhatKyThaoTac_1Param(diary);
                        }
                    })
                }
            }

            //  hoantra 
            var tienCocX = formatNumberToFloat(ptKhach.TienDatCoc);
            if (ptKhach.HoanTraTamUng > 0 && tienCocX > 0) {
                let qct = newQuyChiTiet({
                    ID_HoaDonLienQuan: hd.ID,
                    ID_KhoanThuChi: idKhoanThuChi,
                    ID_DoiTuong: idDoiTuong,
                    GhiChu: ghichu,
                    TienThu: tienCocX,
                    TienCoc: tienCocX,
                    HinhThucThanhToan: 6,
                });
                lstQuyCT = [qct];
                let phuongthucTT2 = 'Chi từ cọc';
                let sLoai2 = 'chi';
                tongthu = tienCocX;

                self.PhieuThuKhachPrint.TienDatCoc = tienCocX;
                self.PhieuThuKhachPrint.DaThanhToan = tienCocX;

                let quyhd = {
                    LoaiHoaDon: 12,
                    TongTienThu: tongthu,
                    MaHoaDon: maPhieuThuChi + '_2',
                    NgayLapHoaDon: moment(hd.NgayLapHoaDon).add(nowSeconds, 'seconds').format('YYYY-MM-DD HH:mm:ss'),
                    NguoiNopTien: tenDoiTuong,
                    NguoiTao: hd.NguoiTao,
                    NoiDungThu: ghichu,
                    ID_NhanVien: hd.ID_NhanVien,
                    ID_DonVi: hd.ID_DonVi,
                    // used to get save diary 
                    MaHoaDonTraHang: hd.MaHoaDon,
                    ID_DoiTuong: idDoiTuong,
                }
                quyhd.PhuongThucTT = phuongthucTT2;

                let myData = {
                    objQuyHoaDon: quyhd,
                    lstCTQuyHoaDon: lstQuyCT
                };

                if (lstQuyCT.length > 0) {
                    console.log('myData_quyKH ', myData);
                    ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/PostQuy_HoaDon_DefaultIDDoiTuong', 'POST', myData).done(function (x) {
                        if (x.res === true) {
                            // update ID_QuyHoaDon in BH_NhanVienThucHien by ID_HoaDon
                            quyhd.MaHoaDon = x.data.MaHoaDon;
                            let diary = {
                                LoaiNhatKy: 1,
                                ID_DonVi: quyhd.ID_DonVi,
                                ID_NhanVien: quyhd.ID_NhanVien,
                                ChucNang: 'Phiếu ' + sLoai2,
                                NoiDung: 'Tạo phiếu '.concat(sLoai2, ' ', quyhd.MaHoaDon, ' cho hóa đơn ', hd.MaHoaDon,
                                    ', Nhà cung cấp: ', quyhd.NguoiNopTien, ', với giá trị ', formatNumber3Digit(quyhd.TongTienThu),
                                    ', Phương thức thanh toán: ', phuongthucTT2,
                                    ', Thời gian: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')),
                                NoiDungChiTiet: 'Tạo phiếu ' + sLoai2 + ' <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD('.concat(quyhd.MaHoaDon, ')" >', quyhd.MaHoaDon, '</a> ',
                                    ' cho hóa đơn: <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD(', hd.MaHoaDon, ')" >', hd.MaHoaDon, '</a> ',
                                    ', Nhà cung cấp: <a style="cursor: pointer" onclick = "LoadKhachHang_byMaKH(', quyhd.NguoiNopTien, ')" >', quyhd.NguoiNopTien, '</a> ',
                                    '<br /> Giá trị: ', formatNumber3Digit(quyhd.TongTienThu),
                                    '<br /> Khoản ', sLoai2, ': ', self.khoanthuchi.NoiDungThuChi,
                                    '<br/ > Phương thức thanh toán: ', phuongthucTT2,
                                    '<br/ > Thời gian: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')
                                )
                            }
                            Insert_NhatKyThaoTac_1Param(diary);
                        }
                    });
                }
            }

            //if (ptKhach.DaThanhToan === 0) {
            //    self.SavePhieuThu_Default(hd);
            //}

            $('#ThongTinThanhToanNCC').modal('hide');
        },


        AgreePay: function () {// chỉ đồng ý các hình thức thah toán và đóng modal(chưa lưu)
            var self = this;
            self.saveOK = true;
            $('#ThongTinThanhToanNCC').modal('hide');
        }
    },
})
