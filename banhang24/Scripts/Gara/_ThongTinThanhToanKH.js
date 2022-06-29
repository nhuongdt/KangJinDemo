var vmThanhToanGara = new Vue({
    el: '#ThongTinThanhToanKHNCC',
    components: {
        'account-bank': cmpChoseAccountBank,
        'khoan-thu-chi': cmpChoseKhoanThu,
        'nvien-hoadon-search': cmpSearchNVDisscount,
    },
    created: function () {
        this.GuidEmpty = '00000000-0000-0000-0000-000000000000';
    },
    data: {
        saveOK: false,
        isNew: true,
        formType: 1,
        IsShareDiscount: '2',
        LoaiChietKhauHD_NV: '1',
        RoleChange_ChietKhauNV: true,
        isCheckTraLaiCoc: false,
        ThietLap_TichDiem: {
            DuocThietLap: false,
            DiemThanhToan: 1,
            TienThanhToan: 1,
            TyLeDoiDiem: 1,
            TichDiemGiamGia: false,
            TichDiemHoaDonGiamGia: true,
        },
        itemChosing: {},

        theGiaTriCus: {
            TongNapThe: 0,
            SuDungThe: 0,
            HoanTraTheGiaTri: 0,
            SoDuTheGiaTri: 0,
            CongNoThe: 0,
        },

        inforHoaDon: {
            ID: null,
            LoaiDoiTuong: 1,// 1.kh, 2.ncc, 3.bh
            LoaiHoaDon: 1,
            ID_DoiTuong: null,
            ID_BaoHiem: null,
            SoDuDatCoc: 0,
            SoDuTheGiaTri: 0,
            HoanTraTamUng: 0,
            PhaiThanhToan: 0,
            PhaiThanhToanBaoHiem: 0,
            TongThanhToan: 0,
            TongTienThue: 0,
            TongTichDiem: 0,
            ThucThu: 0,
            ConNo: 0,
            DienGiai: '',
            MaDoiTuong: '',
            TenDoiTuong: '',
            TenBaoHiem: '',
            MaHoaDon: '',
            ID_DonVi: null,
            ID_NhanVien: null,
            NgayLapHoaDon: null,
            ID_PhieuTiepNhan: null,
        },
        GridNVienBanGoi_Chosed: [],
        PhieuThuKhach: {
            DiemQuyDoi: 0,// phai gan de lay gtri khi update diem khachhang
            TongPhiThanhToan: 0,
            PhiThanhToan_PTGiaTri: 0,
            PhiThanhToan_LaPhanTram: true,
        },
        PhieuThuBaoHiem: {},
        listData: {
            AccountBanks: [],
            KhoanThuChis: [],
            NhanViens: [],
            ChietKhauHoaDons: [],
        },
        PhieuThuKhachPrint: {},
        PhieuThuBaoHiemPrint: {},

        Pos_indexChosing: 0,
        CK_indexChosing: 0
    },
    methods: {
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
        newPhieuThu: function (loaiDoiTuong) {
            var self = this;
            return {
                LoaiDoiTuong: loaiDoiTuong,
                MaHoaDon: '',
                NgayLapHoaDon: moment(new Date()).format('YYYY-MM-DD HH:mm'),
                SoDuTheGiaTri: 0,
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

                TongPhiThanhToan: 0,
                PhiThanhToan_PTGiaTri: 0,
                PhiThanhToan_LaPhanTram: true,

                ListTKPos: [self.newPhuongThucTT(true)],
                ListTKChuyenKhoan: [self.newPhuongThucTT(false)],
            };
        },
        newNhanVien_ChietKhauHoaDon: function (itemCK, itemNV, exitChietKhau) {
            let self = this;
            let chiPhiNganHang = self.GetChiPhi_Visa();
            let doanhThu = self.inforHoaDon.TongThanhToan - self.inforHoaDon.TongTienThue;
            let thucThu = self.PhieuThuKhach.ThucThu + self.PhieuThuBaoHiem.ThucThu - chiPhiNganHang;
            if (exitChietKhau) {
                let tinhCKTheo = parseInt(itemCK.TinhChietKhauTheo);
                let valChietKhau = itemCK.GiaTriChietKhau;
                let tienCK_NV = 0; // used to assign in Grid
                let ptramCK = 0;
                switch (tinhCKTheo) {
                    case 1:
                        ptramCK = valChietKhau;
                        tienCK_NV = Math.round((valChietKhau / 100) * thucThu);
                        break;
                    case 2:
                        ptramCK = valChietKhau;
                        tienCK_NV = Math.round((valChietKhau / 100) * doanhThu);
                        break;
                    case 3:
                        tienCK_NV = valChietKhau;
                        break;
                }
                return {
                    ID_NhanVien: itemNV.ID,
                    MaNhanVien: itemNV.MaNhanVien,
                    TenNhanVien: itemNV.TenNhanVien,
                    ThucHien_TuVan: false,
                    TheoYeuCau: false,
                    HeSo: 1,
                    TinhChietKhauTheo: tinhCKTheo.toString(),
                    TienChietKhau: formatNumber3Digit(tienCK_NV),
                    PT_ChietKhau: ptramCK,
                    ChietKhauMacDinh: valChietKhau,
                }
            }
            else {
                return {
                    ID_NhanVien: itemNV.ID,
                    MaNhanVien: itemNV.MaNhanVien,
                    TenNhanVien: itemNV.TenNhanVien,
                    ThucHien_TuVan: false,
                    TheoYeuCau: false,
                    HeSo: 1,
                    TinhChietKhauTheo: '1',
                    TienChietKhau: 0,
                    PT_ChietKhau: 0,
                    ChietKhauMacDinh: 0,
                }
            }
        },

        GetSoDuTheGiaTri: function (idDoiTuong) {
            var self = this;
            let datetime = moment(new Date()).add('days', 1).format('YYYY-MM-DD');
            $.getJSON("/api/DanhMuc/DM_DoiTuongAPI/Get_SoDuTheGiaTri_ofKhachHang?idDoiTuong=" + idDoiTuong + '&datetime=' + datetime, function (data) {
                if (data !== null && data.length > 0) {
                    let item0 = data[0];
                    let sodu = item0.SoDuTheGiaTri;
                    sodu = sodu < 0 ? 0 : sodu;
                    self.theGiaTriCus.SoDuTheGiaTri = sodu;
                    self.theGiaTriCus.CongNoThe = item0.CongNoThe;
                    self.theGiaTriCus.TongNapThe = item0.TongThuTheGiaTri;
                    self.theGiaTriCus.SuDungThe = item0.SuDungThe;
                }
            });
        },

        SetDefault_TienTheGiaTri_ifHaveSoSu: function (phaiTT) {
            var self = this;
            var soduThe = self.theGiaTriCus.SoDuTheGiaTri - self.theGiaTriCus.CongNoThe; // chi dc su dung the voi so tien da nop
            if (soduThe > 0) {
                if (soduThe > phaiTT) {
                    return {
                        TienMat: 0, TienTheGiaTri: phaiTT
                    };
                }
                else {
                    return {
                        TienMat: phaiTT - soduThe, TienTheGiaTri: soduThe
                    };
                }
            }
            else {
                return {
                    TienMat: phaiTT, TienTheGiaTri: 0
                };
            }
        },
        showModalUpdate: function (hd, formType = 2) {// use at banle
            var self = this;
            self.isCheckTraLaiCoc = false;
            self.saveOK = false;
            self.formType = formType;
            self.GridNVienBanGoi_Chosed = [];
            self.PhieuThuKhach = self.newPhieuThu(1);
            self.PhieuThuBaoHiem = self.newPhieuThu(3);
            self.PhieuThuKhachPrint = self.newPhieuThu(1);
            self.PhieuThuBaoHiemPrint = self.newPhieuThu(3);
            var tiendatcoc = 0, datt = 0, cantt = 0, tienmat = 0, tienpos = 0, tienck = 0, tiendiem = 0, diemquydoi = 0, tienthe = 0;
            var idPOS = null, idCK = null;
            var lstPos = null, lstCK = null;
            var hoantra = hd.HoanTraTamUng;
            var soduDatCoc = hd.SoDuDatCoc;

            var lstHD = localStorage.getItem('lstHDLe');
            if (lstHD !== null) {
                lstHD = JSON.parse(lstHD);
            }
            else {
                lstHD = [];
            }
            console.log('hd ', hd)

            for (let i = 0; i < lstHD.length; i++) {
                let itFor = lstHD[i];
                if (itFor.IDRandom === hd.IDRandom) {
                    tienmat = itFor.TienMat;
                    tienpos = itFor.TienATM;
                    tienck = itFor.TienGui;
                    diemquydoi = itFor.DiemQuyDoi;
                    tiendiem = itFor.TTBangDiem;
                    tienthe = itFor.TienTheGiaTri;
                    idPOS = itFor.ID_TaiKhoanPos;
                    idCK = itFor.ID_TaiKhoanChuyenKhoan;
                    lstPos = itFor.ListTKPos;
                    lstCK = itFor.ListTKChuyenKhoan;

                    self.GridNVienBanGoi_Chosed = itFor.BH_NhanVienThucHiens;
                    break;
                }
            }

            if (hoantra > 0) {
                cantt = hoantra;
            }
            else {
                tiendatcoc = soduDatCoc;
                cantt = hd.PhaiThanhToan - tiendatcoc - hd.KhachDaTra;

                let isAgree = tienthe > 0 || tienpos > 0 || tienck > 0;
                if (self.theGiaTriCus.SoDuTheGiaTri > 0 && isAgree === false) {
                    let obj = self.SetDefault_TienTheGiaTri_ifHaveSoSu(hd.PhaiThanhToan);
                    tienthe = obj.TienTheGiaTri;
                    tienmat = obj.TienMat;
                }
            }
            datt = tienmat + tienpos + tienck + tiendiem + tienthe;

            self.inforHoaDon = hd;
            self.inforHoaDon.DaThanhToan = datt;
            self.inforHoaDon.ThucThu = datt - tiendiem - tienthe;

            self.PhieuThuKhach.TienDatCoc = formatNumber3Digit(tiendatcoc, 2);
            self.PhieuThuKhach.TienMat = formatNumber3Digit(tienmat, 2);
            self.PhieuThuKhach.TienPOS = formatNumber3Digit(tienpos, 2);
            self.PhieuThuKhach.TienCK = formatNumber3Digit(tienck, 2);
            self.PhieuThuKhach.TienTheGiaTri = formatNumber3Digit(tienthe, 2);
            self.PhieuThuKhach.TTBangDiem = formatNumber3Digit(tiendiem, 2);
            self.PhieuThuKhach.DiemQuyDoi = formatNumber3Digit(diemquydoi);

            self.PhieuThuKhach.PhaiThanhToan = cantt;
            self.PhieuThuKhach.HoanTraTamUng = hoantra;
            self.PhieuThuKhach.ThucThu = self.inforHoaDon.ThucThu;
            self.PhieuThuKhach.ID_TaiKhoanPos = idPOS;
            self.PhieuThuKhach.ID_TaiKhoanChuyenKhoan = idCK;

            if (!commonStatisJs.CheckNull(lstPos)) {
                //for (let i = 0; i < lstPos.length; i++) {
                //    let tkPos = $.grep(self.listData.AccountBanks, function (x) {
                //        return x.ID === lstPos[i].ID_TaiKhoanPos;
                //    });
                //    if (tkPos.length > 0) {
                //        self.ChangeAccountPOS(tkPos[0]);
                //    }
                //    else {
                //        self.Pos_indexChosing = i;
                //        self.ResetAccountPOS();
                //    }
                //}
                self.PhieuThuKhach.ListTKPos = lstPos;
            }
            if (!commonStatisJs.CheckNull(lstCK)) {
                //for (let i = 0; i < lstCK.length; i++) {
                //    let tkCK = $.grep(self.listData.AccountBanks, function (x) {
                //        return x.ID === lstPos[i].ID_TaiKhoanChuyenKhoan;
                //    });
                //    if (tkCK.length > 0) {
                //        self.ChangeAccountCK(tkCK[0]);
                //    }
                //    else {
                //        self.CK_indexChosing = i;
                //        self.ResetAccountCK();
                //    }
                //}
                self.PhieuThuKhach.ListTKChuyenKhoan = lstCK;
            }
            $('#ThongTinThanhToanKHNCC').modal('show');
        },
        showModalThanhToan: function (hd, formType = 1) { // 1.gara, 2.lapphieuthu banle, 3.thegiatri
            var self = this;
            self.isCheckTraLaiCoc = false;
            self.saveOK = false;
            self.formType = formType;
            self.inforHoaDon = hd;
            self.GridNVienBanGoi_Chosed = [];
            self.PhieuThuKhach = self.newPhieuThu(1);
            self.PhieuThuBaoHiem = self.newPhieuThu(3);
            self.PhieuThuKhachPrint = self.newPhieuThu(1);
            self.PhieuThuBaoHiemPrint = self.newPhieuThu(3);
            console.log('gara-tt')

            var lstHD = localStorage.getItem('gara_lstHDLe');
            if (lstHD !== null) {
                lstHD = JSON.parse(lstHD);
            }
            else {
                lstHD = [];
            }

            for (let i = 0; i < lstHD.length; i++) {
                let itFor = lstHD[i];
                if (itFor.IDRandom === hd.IDRandom) {
                    self.GridNVienBanGoi_Chosed = itFor.BH_NhanVienThucHiens;
                    break;
                }
            }

            // set default tienmat phaitt for khachhang
            var tiendatcoc = 0, datt = 0, cantt = 0, tienmat = 0;
            var hoantra = hd.HoanTraTamUng;
            var soduDatCoc = hd.SoDuDatCoc;
            if (hoantra > 0) {
                datt = cantt = hoantra;

                // neu update hdold
                if (soduDatCoc > 0) {
                    self.isCheckTraLaiCoc = true;// set default tralaicoc: if sodu > 0

                    var hdUpdate_CanTT = hd.PhaiThanhToan - hd.KhachDaTra;
                    if (hd.KhachDaTra > 0) {
                        if (hdUpdate_CanTT > 0) {
                            if (hdUpdate_CanTT > soduDatCoc) {
                                tiendatcoc = soduDatCoc;
                            }
                            else {
                                tiendatcoc = hdUpdate_CanTT;
                            }
                        }
                    }
                    else {
                        tiendatcoc = hd.PhaiThanhToan;
                    }
                }
                tienmat = hoantra;
            }
            else {
                tiendatcoc = soduDatCoc;
                cantt = tienmat = hd.PhaiThanhToan - tiendatcoc - hd.KhachDaTra;
                datt = tienmat + tiendatcoc;
            }
            self.PhieuThuKhach.TienDatCoc = formatNumber3Digit(tiendatcoc, 2);
            self.PhieuThuKhach.TienMat = formatNumber3Digit(tienmat, 2);
            self.PhieuThuKhach.PhaiThanhToan = cantt;
            self.PhieuThuKhach.DaThanhToan = datt; // sum (mat + coc)
            self.PhieuThuKhach.HoanTraTamUng = hoantra; // used when edit kh_tiencoc
            self.PhieuThuKhach.ThucThu = self.inforHoaDon.ThucThu;// first show: set default ThucThu

            $('#ThongTinThanhToanKHNCC').modal('show');
        },
        changeTab: function (loaiDT) {
            var self = this;
            self.inforHoaDon.LoaiDoiTuong = loaiDT;
            self.UpdateChietKhauNV_ifChangeThucThu();
        },
        AccountPos_AddRow: function () {
            let self = this;
            self.PhieuThuKhach.ListTKPos.push(self.newPhuongThucTT(true));
        },
        AccountCK_AddRow: function () {
            let self = this;
            self.PhieuThuKhach.ListTKChuyenKhoan.push(self.newPhuongThucTT(false));
        },
        RemoveAccount: function (type, index) {
            let self = this;
            switch (type) {
                case 0:// pos
                    self.PhieuThuKhach.ListTKPos.splice(index, 1);
                    break;
                case 1:// ck
                    self.PhieuThuKhach.ListTKChuyenKhoan.splice(index, 1);
                    break;
            }
        },
        KH_GetTongPos: function () {
            var self = this;
            var tongPos = 0;
            for (let i = 0; i < self.PhieuThuKhach.ListTKPos.length; i++) {
                let itFor = self.PhieuThuKhach.ListTKPos[i];
                tongPos += formatNumberToFloat(itFor.TienPOS);
            }
            return tongPos;
        },
        KH_GetTongCK: function () {
            let self = this;
            let tongCK = 0;
            for (let i = 0; i < self.PhieuThuKhach.ListTKChuyenKhoan.length; i++) {
                let itFor = self.PhieuThuKhach.ListTKChuyenKhoan[i];
                tongCK += formatNumberToFloat(itFor.TienCK);
            }
            return tongCK;
        },
        GetChiPhi_Visa: function () {
            let self = this;
            let tongChiPhi = 0, gtriPTram = 0;
            let laPhanTram = true;
            for (let i = 0; i < self.PhieuThuKhach.ListTKPos.length; i++) {
                let itFor = self.PhieuThuKhach.ListTKPos[i];
                if (itFor.ChiPhiThanhToan > 0) {
                    laPhanTram = itFor.TheoPhanTram;
                    gtriPTram = itFor.ChiPhiThanhToan;
                    if (itFor.TheoPhanTram) {
                        tongChiPhi += formatNumberToFloat(itFor.TienPOS) * itFor.ChiPhiThanhToan / 100;
                    }
                    else {
                        tongChiPhi += itFor.ChiPhiThanhToan;
                    }
                }
            }
            self.PhieuThuKhach.TongPhiThanhToan = tongChiPhi;
            self.PhieuThuKhach.PhiThanhToan_PTGiaTri = gtriPTram;
            self.PhieuThuKhach.PhiThanhToan_LaPhanTram = laPhanTram;
            return tongChiPhi;
        },
        CaculatorDaThanhToan: function () {
            let self = this;
            let tongPos = self.KH_GetTongPos();
            let tongCK = self.KH_GetTongCK();
            self.PhieuThuKhach.TienPOS = tongPos;
            self.PhieuThuKhach.TienCK = tongCK;
            let tiencoc = formatNumberToFloat(self.PhieuThuKhach.TienDatCoc);
            let tienmat = formatNumberToFloat(self.PhieuThuKhach.TienMat);
            let tienTGT = formatNumberToFloat(self.PhieuThuKhach.TienTheGiaTri);
            let tienDiem = formatNumberToFloat(self.PhieuThuKhach.TTBangDiem);
            if (self.PhieuThuKhach.HoanTraTamUng > 0 && self.inforHoaDon.SoDuDatCoc > 0) {
                // neu chi tra tien: khong tinh tiencoc
                tiencoc = 0;
            }
            self.PhieuThuKhach.DaThanhToan = tongPos + tongCK + tienmat + tiencoc + tienTGT + tienDiem;

            let thucthu = 0;
            if (self.PhieuThuKhach.HoanTraTamUng > 0) {
                thucthu = formatNumberToFloat(self.PhieuThuKhach.TienDatCoc);
                self.PhieuThuKhach.TienThua = self.PhieuThuKhach.DaThanhToan - self.PhieuThuKhach.PhaiThanhToan;
            }
            else {
                thucthu = tongPos + tongCK + tienmat + tiencoc;
                self.PhieuThuKhach.TienThua = self.PhieuThuKhach.DaThanhToan + self.inforHoaDon.KhachDaTra - self.inforHoaDon.PhaiThanhToan;
                if (self.PhieuThuKhach.TienThua > 0) {
                    thucthu = self.inforHoaDon.PhaiThanhToan
                        - self.inforHoaDon.KhachDaTra
                        - formatNumberToFloat(self.PhieuThuKhach.TienTheGiaTri)
                        - formatNumberToFloat(self.PhieuThuKhach.TTBangDiem);
                }
            }
            self.PhieuThuKhach.ThucThu = thucthu;
            self.Caculator_ThucThu();
            self.GetChiPhi_Visa();
            self.UpdateChietKhauNV_ifChangeThucThu();
        },
        BH_CaculatorDaThanhToan: function () {
            var self = this;
            self.PhieuThuBaoHiem.DaThanhToan = formatNumberToFloat(self.PhieuThuBaoHiem.TienMat)
                + formatNumberToFloat(self.PhieuThuBaoHiem.TienPOS)
                + formatNumberToFloat(self.PhieuThuBaoHiem.TienCK)
                + formatNumberToFloat(self.PhieuThuBaoHiem.TienTheGiaTri)
            self.PhieuThuBaoHiem.ThucThu = formatNumberToFloat(self.PhieuThuBaoHiem.TienMat)
                + formatNumberToFloat(self.PhieuThuBaoHiem.TienPOS)
                + formatNumberToFloat(self.PhieuThuBaoHiem.TienCK);
            self.PhieuThuBaoHiem.TienThua = self.PhieuThuBaoHiem.DaThanhToan - self.inforHoaDon.PhaiThanhToanBaoHiem;
            self.Caculator_ThucThu();
            self.UpdateChietKhauNV_ifChangeThucThu();
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
            let self = this;
            for (let i = 0; i < self.PhieuThuKhach.ListTKPos.length; i++) {
                if (i === self.Pos_indexChosing) {
                    self.PhieuThuKhach.ListTKPos[i].ID_TaiKhoanPos = null;
                    self.PhieuThuKhach.ListTKPos[i].TenTaiKhoanPos = '';
                    self.PhieuThuKhach.ListTKPos[i].SoTaiKhoanPos = '';
                    self.PhieuThuKhach.ListTKPos[i].TenNganHangCK = '';
                    self.PhieuThuKhach.ListTKPos[i].TienPOS = 0;
                    self.PhieuThuKhach.ListTKPos[i].ChiPhiThanhToan = 0;
                    self.PhieuThuKhach.ListTKPos[i].TheoPhanTram = true;
                    self.PhieuThuKhach.ListTKPos[i].ThuPhiThanhToan = false;
                    self.PhieuThuKhach.ListTKPos[i].MacDinh = false;
                    break;
                }
            }
            self.CaculatorDaThanhToan();
        },
        ChangeAccountPOS: function (item) {
            let self = this;
            for (let i = 0; i < self.PhieuThuKhach.ListTKPos.length; i++) {
                if (i === self.Pos_indexChosing) {
                    self.PhieuThuKhach.ListTKPos[i].ID_TaiKhoanPos = item.ID;
                    self.PhieuThuKhach.ListTKPos[i].TenTaiKhoanPos = item.TenChuThe;
                    self.PhieuThuKhach.ListTKPos[i].SoTaiKhoanPos = item.SoTaiKhoan;
                    self.PhieuThuKhach.ListTKPos[i].TenNganHangPos = item.TenNganHang;
                    self.PhieuThuKhach.ListTKPos[i].ChiPhiThanhToan = item.ChiPhiThanhToan;
                    self.PhieuThuKhach.ListTKPos[i].TheoPhanTram = item.TheoPhanTram;
                    self.PhieuThuKhach.ListTKPos[i].ThuPhiThanhToan = item.ThuPhiThanhToan;
                    self.PhieuThuKhach.ListTKPos[i].MacDinh = item.MacDinh;
                    break;
                }
            }
            self.GetChiPhi_Visa();
        },
        ChangeAccountCK: function (item) {
            let self = this;
            for (let i = 0; i < self.PhieuThuKhach.ListTKChuyenKhoan.length; i++) {
                if (i === self.CK_indexChosing) {
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].ID_TaiKhoanChuyenKhoan = item.ID;
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].TenTaiKhoanCK = item.TenChuThe;
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].SoTaiKhoanCK = item.SoTaiKhoan;
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].TenNganHangCK = item.TenNganHang;
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].ChiPhiThanhToan = item.ChiPhiThanhToan;
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].TheoPhanTram = item.TheoPhanTram;
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].ThuPhiThanhToan = item.ThuPhiThanhToan;
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].MacDinh = item.MacDinh;
                    break;
                }
            }
            self.GetChiPhi_Visa();
        },
        ResetAccountCK: function () {
            let self = this;
            for (let i = 0; i < self.PhieuThuKhach.ListTKChuyenKhoan.length; i++) {
                if (i === self.CK_indexChosing) {
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].ID_TaiKhoanChuyenKhoan = null;
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].TenTaiKhoanCK = '';
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].SoTaiKhoanCK = '';
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].TenNganHangCK = '';
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].TienCK = 0;
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].ChiPhiThanhToan = 0;
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].TheoPhanTram = true;
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].ThuPhiThanhToan = false;
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].MacDinh = false;
                    break;
                }
            }
            self.CaculatorDaThanhToan();
        },

        BH_ResetAccountPOS: function () {
            var self = this;
            self.PhieuThuBaoHiem.TenTaiKhoanPos = '';
            self.PhieuThuBaoHiem.ID_TaiKhoanPos = null;
            self.PhieuThuBaoHiem.TenNganHangPos = '';
            self.PhieuThuBaoHiem.SoTaiKhoanPos = null;
            self.PhieuThuBaoHiem.TienPOS = 0;
            self.CaculatorDaThanhToan();
        },
        BH_ChangeAccountPOS: function (item) {
            var self = this;
            self.PhieuThuBaoHiem.TenTaiKhoanPos = item.TenChuThe;
            self.PhieuThuBaoHiem.ID_TaiKhoanPos = item.ID;
            self.PhieuThuBaoHiem.SoTaiKhoanPos = item.SoTaiKhoan;
            self.PhieuThuBaoHiem.TenNganHangPos = item.TenNganHang;
        },
        BH_ChangeAccountCK: function (item) {
            var self = this;
            self.PhieuThuBaoHiem.TenTaiKhoanCK = item.TenChuThe;
            self.PhieuThuBaoHiem.ID_TaiKhoanChuyenKhoan = item.ID;
            self.PhieuThuBaoHiem.SoTaiKhoanCK = item.SoTaiKhoan;
            self.PhieuThuBaoHiem.TenNganHangCK = item.TenNganHang;
        },
        BH_ResetAccountCK: function () {
            var self = this;
            self.PhieuThuBaoHiem.TenTaiKhoanCK = '';
            self.PhieuThuBaoHiem.ID_TaiKhoanPos = null;
            self.PhieuThuBaoHiem.SoTaiKhoanCK = '';
            self.PhieuThuBaoHiem.TenNganHangCK = '';
            self.PhieuThuBaoHiem.TienCK = 0;
            self.BH_CaculatorDaThanhToan();
        },
        Change_IsShareDiscount: function (x) {
            var self = this;
            self.HoaHongHD_UpdateHeSo_AndBind();
        },
        HoaHongHD_RemoveNhanVien: function (index) {
            var self = this;
            for (let i = 0; i < self.GridNVienBanGoi_Chosed.length; i++) {
                if (i === index) {
                    self.GridNVienBanGoi_Chosed.splice(i, 1);
                    break;
                }
            }
            self.HoaHongHD_UpdateHeSo_AndBind();
        },
        HoaHongHD_KeyEnter: function (columnEdit) {
            let thisObj = $(event.currentTarget);
            let trClosest = $(thisObj).closest('tr');
            let tdNext = trClosest.next().find('td');
            $(tdNext).eq(columnEdit).find('input').focus().select();
        },
        HoaHongHD_EditChietKhau: function (item, index) {
            var self = this;
            item.Index = index;
            self.itemChosing = item;
            var thisObj = event.currentTarget;
            var gtriNhap = formatNumberToFloat($(thisObj).val());
            var tinhCKTheo = parseInt(self.LoaiChietKhauHD_NV);
            var ptramCK = 0;
            if (tinhCKTheo === 3) {
                formatNumberObj($(thisObj))
            }
            else {
                if (gtriNhap > 100) {
                    $(thisObj).val(100);
                }
            }
            // get gtri after check % or VND
            var gtriCK_After = formatNumberToFloat($(thisObj).val());
            if (tinhCKTheo !== 3) {
                ptramCK = gtriCK_After;
            }
            var tienCK = self.CaculatorAgain_TienDuocNhan(gtriCK_After, item.HeSo, tinhCKTheo);
            for (let i = 0; i < self.GridNVienBanGoi_Chosed.length; i++) {
                if (i === item.Index) {
                    self.GridNVienBanGoi_Chosed[i].PT_ChietKhau = ptramCK;
                    self.GridNVienBanGoi_Chosed[i].TienChietKhau = formatNumber3Digit(tienCK, 2);
                    self.GridNVienBanGoi_Chosed[i].ChietKhauMacDinh = formatNumber3Digit(gtriCK_After);
                    break;
                }
            }
        },
        HoaHongHD_EditThanhTien: function (item, index) {
            let self = this;
            item.Index = index;
            self.itemChosing = item;

            let thisObj = $(event.currentTarget);
            let gtriNhap = formatNumberToFloat(thisObj.val());
            formatNumberObj(thisObj)

            let chiphiNganHang = self.GetChiPhi_Visa();
            let thucthu = self.PhieuThuKhach.ThucThu - self.PhieuThuBaoHiem.ThucThu - chiphiNganHang;
            let ptramCK = gtriNhap / thucthu * 100;

            for (let i = 0; i < self.GridNVienBanGoi_Chosed.length; i++) {
                if (i === item.Index) {
                    self.GridNVienBanGoi_Chosed[i].PT_ChietKhau = ptramCK;
                    self.GridNVienBanGoi_Chosed[i].TienChietKhau = thisObj.val();
                    self.GridNVienBanGoi_Chosed[i].ChietKhauMacDinh = formatNumber3Digit(ptramCK);
                    break;
                }
            }
        },
        AddNhanVien_BanGoi: function (item) {
            var self = this;
            var idNhanVien = item.ID;
            // check IDNhanVien exist in grid with same TacVu
            var itemEx = $.grep(self.GridNVienBanGoi_Chosed, function (x) {
                return x.ID_NhanVien === idNhanVien;
            });
            if (itemEx.length > 0) {
                ShowMessage_Danger('Nhân viên ' + itemEx[0].TenNhanVien + ' đã được chọn');
                return;
            }
            var loaiHD = self.inforHoaDon.LoaiHoaDon.toString();
            // get all ChietKhau HoaDon with LoaiHD
            var lstCK = $.grep(self.listData.ChietKhauHoaDons, function (x) {
                return x.ChungTuApDung.indexOf(loaiHD) > -1;
            })
            // remove ChungTu not apply LoaiHoaDon (ex: loaiHD= 1, but ChungTu contain 19
            var arrAfter = [];
            for (let i = 0; i < lstCK.length; i++) {
                var arrChungTu = lstCK[i].ChungTuApDung.split(',');
                if ($.inArray(loaiHD, arrChungTu) > -1) {
                    arrAfter.push(lstCK[i]);
                }
            }
            var exist = false;
            for (let i = 0; i < arrAfter.length; i++) {
                let itemOut = arrAfter[i];
                for (let j = 0; j < itemOut.NhanViens.length; j++) {
                    if (itemOut.NhanViens[j].ID === idNhanVien) {
                        let newObject1 = self.newNhanVien_ChietKhauHoaDon(itemOut, item, true);
                        self.GridNVienBanGoi_Chosed.unshift(newObject1);
                        exist = true;
                        break;
                    }
                }
            }
            if (exist === false) {
                let newObject2 = self.newNhanVien_ChietKhauHoaDon(null, item, false);
                self.GridNVienBanGoi_Chosed.push(newObject2);
            }
            self.HoaHongHD_UpdateHeSo_AndBind();
        },
        UpdateChietKhauNV_ifChangeThucThu: function () {
            let self = this;
            let chiphiNganHang = self.GetChiPhi_Visa();
            let thucthu = self.PhieuThuKhach.ThucThu + self.PhieuThuBaoHiem.ThucThu - chiphiNganHang;
            for (let i = 0; i < self.GridNVienBanGoi_Chosed.length; i++) {
                let itemFor = self.GridNVienBanGoi_Chosed[i];
                if (parseInt(itemFor.TinhChietKhauTheo) === 1) {
                    self.GridNVienBanGoi_Chosed[i].TienChietKhau = formatNumber3Digit(thucthu * formatNumberToFloat(itemFor.PT_ChietKhau) / 100 * itemFor.HeSo);
                }
            }
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

        KH_EditTienCoc: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var tienmat = 0;
            // !!important: get self.inforHoaDon.PhaiThanhToan
            var khachcantra = formatNumberToFloat(self.inforHoaDon.PhaiThanhToan) - self.inforHoaDon.KhachDaTra;
            var soduDatCoc = self.inforHoaDon.SoDuDatCoc;
            var datcoc = formatNumberToFloat($this.val());
            if (datcoc >= khachcantra) {
                if (datcoc >= soduDatCoc) {
                    if (soduDatCoc > khachcantra) {
                        datcoc = khachcantra;
                        tienmat = soduDatCoc - datcoc;
                    }
                    else {
                        datcoc = soduDatCoc;
                        tienmat = khachcantra - datcoc;
                    }
                }
                else {
                    datcoc = khachcantra;
                    tienmat = soduDatCoc - datcoc;
                }
            }
            else {
                if (datcoc >= soduDatCoc) {
                    datcoc = soduDatCoc;
                }
                tienmat = khachcantra - datcoc;
            }
            self.PhieuThuKhach.TienDatCoc = formatNumber3Digit(datcoc, 2);

            var hoantraOld = self.inforHoaDon.HoanTraTamUng;
            if (hoantraOld > 0 && soduDatCoc > 0) {
                if (datcoc < khachcantra && datcoc < soduDatCoc) {
                    self.PhieuThuKhach.HoanTraTamUng = 0;
                    self.isCheckTraLaiCoc = false;
                }
                else {
                    self.PhieuThuKhach.HoanTraTamUng = datcoc;
                }
            }

            self.PhieuThuKhach.TienMat = formatNumber3Digit(tienmat, 2);
            self.PhieuThuKhach.PhaiThanhToan = tienmat;

            self.PhieuThuKhach.ListTKPos[0].TienPOS = 0;
            self.PhieuThuKhach.ListTKChuyenKhoan[0].TienCK = 0;
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
            self.PhieuThuKhach.TienMat = $this.val();

            var tienthe = formatNumberToFloat(self.PhieuThuKhach.TienTheGiaTri);
            var tienpos = 0, tienck = 0;
            var cantt = self.PhieuThuKhach.PhaiThanhToan - tienmat - self.PhieuThuKhach.TTBangDiem - tienthe;

            let ttPos = $.grep(self.PhieuThuKhach.ListTKPos, function (x) {
                return x.ID_TaiKhoanPos !== null;
            });
            if (ttPos.length > 0) {
                tienpos = cantt;
                tienpos = tienpos > 0 ? tienpos : 0;
                tienck = 0;

                for (let i = 0; i < self.PhieuThuKhach.ListTKPos.length; i++) {
                    if (self.PhieuThuKhach.ListTKPos[i].ID_TaiKhoanPos !== null) {
                        self.PhieuThuKhach.ListTKPos[i].TienPOS = formatNumber3Digit(tienpos);
                        break;
                    }
                }

                // reset tienCK
                for (let i = 0; i < self.PhieuThuKhach.ListTKChuyenKhoan.length; i++) {
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].TienCK = 0;
                }
            }
            else {
                let ttCK = $.grep(self.PhieuThuKhach.ListTKChuyenKhoan, function (x) {
                    return x.ID_TaiKhoanChuyenKhoan !== null;
                });
                if (ttCK.length > 0) {
                    tienck = cantt;
                    tienck = tienck > 0 ? tienck : 0;

                    for (let i = 0; i < self.PhieuThuKhach.ListTKChuyenKhoan.length; i++) {
                        if (self.PhieuThuKhach.ListTKChuyenKhoan[i].ID_TaiKhoanChuyenKhoan !== null) {
                            self.PhieuThuKhach.ListTKChuyenKhoan[i].TienCK = formatNumber3Digit(tienck);
                            break;
                        }
                    }
                }
                else {
                    // reset tienCK
                    for (let i = 0; i < self.PhieuThuKhach.ListTKChuyenKhoan.length; i++) {
                        self.PhieuThuKhach.ListTKChuyenKhoan[i].TienCK = 0;
                    }
                }
            }
            self.CaculatorDaThanhToan();

            var key = event.keyCode || event.which;
            if (key === 13) {
                self.Focus_NextElem();
            }
        },
        KH_EditTienPos: function (index) {
            let self = this;
            let $this = $(event.currentTarget);
            formatNumberObj($this);

            for (let i = 0; i < self.PhieuThuKhach.ListTKPos.length; i++) {
                if (i === index) {
                    self.PhieuThuKhach.ListTKPos[i].TienPOS = $this.val();
                    break;
                }
            }

            let tienck = 0;
            let tienpos = self.KH_GetTongPos();
            let tienmat = formatNumberToFloat(self.PhieuThuKhach.TienMat);
            let tienthe = formatNumberToFloat(self.PhieuThuKhach.TienTheGiaTri);

            for (let i = 0; i < self.PhieuThuKhach.ListTKChuyenKhoan.length; i++) {
                if (self.PhieuThuKhach.ListTKChuyenKhoan[i].ID_TaiKhoanChuyenKhoan !== null) {
                    tienck = self.PhieuThuKhach.PhaiThanhToan - tienmat - tienpos - self.PhieuThuKhach.TTBangDiem - tienthe;
                    tienck = tienck < 0 ? 0 : tienck;
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].TienCK = formatNumber3Digit(tienck);
                    break;
                }
            }
            self.CaculatorDaThanhToan();

            let key = event.keyCode || event.which;
            if (key === 13) {
                self.Focus_NextElem(index);
            }
        },
        Focus_NextElem: function (index = null) {
            if (index === null) {
                // enter tienmat
                let elms = $('#ThongTinThanhToanKHNCC ._jsCheck .form-control:not(:disabled)');
                if (elms.length > 0) {
                    $(elms).eq(0).select();
                }
            }
            else {
                let elms = $('#ThongTinThanhToanKHNCC ._jsCheck:gt(' + index + ')  .form-control:not(:disabled)');
                if (elms.length > 0) {
                    $(elms).eq(0).select();
                }
            }
        },
        KH_EditTienCK: function (index) {
            let self = this;
            let $this = $(event.currentTarget);
            formatNumberObj($this);

            for (let i = 0; i < self.PhieuThuKhach.ListTKChuyenKhoan.length; i++) {
                if (i === index) {
                    self.PhieuThuKhach.ListTKChuyenKhoan[i].TienCK = $this.val();
                    break;
                }
            }
            self.CaculatorDaThanhToan();

            let key = event.keyCode || event.which;
            if (key === 13) {
                index = self.PhieuThuKhach.ListTKPos.length + index;
                self.Focus_NextElem(index);
            }
        },
        KH_EditTienThe: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);
            if (formatNumberToFloat($this.val()) > self.theGiaTriCus.SoDuTheGiaTri) {
                ShowMessage_Danger('Nhập quá số dư thẻ');
                $this.val(formatNumber3Digit(self.theGiaTriCus.SoDuTheGiaTri));
                self.PhieuThuKhach.TienTheGiaTri = formatNumber3Digit(self.theGiaTriCus.SoDuTheGiaTri, 2);
                self.CaculatorDaThanhToan();
                return;
            }
            self.PhieuThuKhach.TienTheGiaTri = $this.val();

            var tienmat = self.PhieuThuKhach.PhaiThanhToan - formatNumberToFloat($this.val()) - self.PhieuThuKhach.TTBangDiem;
            tienmat = tienmat < 0 ? 0 : tienmat;
            self.PhieuThuKhach.TienMat = formatNumber3Digit(tienmat, 2);
            self.CaculatorDaThanhToan();

            var key = event.keyCode || event.which;
            if (key === 13) {
                $this.closest('.container-fluid').next().find('input').select();
            }
        },

        KH_HoanTraTienThe: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            self.PhieuThuKhach.TienTheGiaTri = $this.val();
            self.CaculatorDaThanhToan();
        },

        KH_TTBangDiem: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var diemQD = formatNumberToFloat($this.val());
            if (!self.ThietLap_TichDiem.DuocThietLap) {
                return;
            }
            if (self.inforHoaDon.TongTichDiem < diemQD) {
                ShowMessage_Danger('Vượt quá số điểm hiện tại ');
                return;
            }
            var diemTT = self.ThietLap_TichDiem.DiemThanhToan;
            diemTT = diemTT === 0 ? 1 : diemTT;
            var tienQuyDoi = Math.floor(diemQD * self.ThietLap_TichDiem.TienThanhToan / self.ThietLap_TichDiem.DiemThanhToan);
            self.PhieuThuKhach.DiemQuyDoi = $this.val();
            self.PhieuThuKhach.TTBangDiem = tienQuyDoi;

            // caculator again tienmat
            var tienMat = self.PhieuThuKhach.PhaiThanhToan - tienQuyDoi - formatNumberToFloat(self.PhieuThuKhach.TienTheGiaTri);
            tienMat = tienMat < 0 ? 0 : tienMat;
            self.PhieuThuKhach.TienMat = formatNumber3Digit(tienMat, 2);
            self.CaculatorDaThanhToan();
        },

        BH_EditTienMat: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var tienmat = formatNumberToFloat($this.val());
            self.PhieuThuBaoHiem.TienMat = formatNumber3Digit(tienmat, 2);

            var tienpos = 0, tienck = 0;
            if (self.PhieuThuBaoHiem.ID_TaiKhoanPos !== null) {
                tienpos = self.inforHoaDon.PhaiThanhToanBaoHiem - tienmat;
                tienck = 0;
            }
            else {
                if (self.PhieuThuBaoHiem.ID_TaiKhoanChuyenKhoan !== null) {
                    tienck = self.inforHoaDon.PhaiThanhToanBaoHiem - tienmat;
                }
                else {

                }
            }
            self.PhieuThuBaoHiem.TienPOS = formatNumber3Digit(tienpos, 2);
            self.PhieuThuBaoHiem.TienCK = formatNumber3Digit(tienck, 2);
            self.BH_CaculatorDaThanhToan();

            var key = event.keyCode || event.which;
            if (key === 13) {
                if (self.PhieuThuBaoHiem.ID_TaiKhoanPos !== null) {
                    $this.parent().next().find('input').focus();
                }
                else {
                    if (self.PhieuThuBaoHiem.ID_TaiKhoanChuyenKhoan !== null) {
                        $this.parent().next().find('input').focus();
                    }
                    else {

                    }
                }
            }
        },
        BH_EditTienPos: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var tienpos = formatNumberToFloat($this.val());
            self.PhieuThuBaoHiem.TienPOS = formatNumber3Digit(tienpos, 2);

            var tienck = 0;
            var tienmat = formatNumberToFloat(self.PhieuThuBaoHiem.TienMat);
            if (self.PhieuThuBaoHiem.ID_TaiKhoanChuyenKhoan !== null) {
                tienck = self.inforHoaDon.PhaiThanhToanBaoHiem - tienmat - tienpos;
            }
            self.PhieuThuBaoHiem.TienCK = formatNumber3Digit(tienck, 2);
            self.BH_CaculatorDaThanhToan();

            var key = event.keyCode || event.which;
            if (key === 13) {
                if (self.PhieuThuBaoHiem.ID_TaiKhoanChuyenKhoan !== null) {
                    $this.parent().next().find('input').focus();
                }
                else {
                    if (self.PhieuThuBaoHiem.SoDuTheGiaTri > 0) {
                        $this.parent().next().next().find('input').focus();
                    }
                }
            }
        },
        BH_EditTienCK: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            self.PhieuThuBaoHiem.TienCK = $this.val();
            self.BH_CaculatorDaThanhToan();

            var key = event.keyCode || event.which;
            if (key === 13) {
                if (self.PhieuThuBaoHiem.SoDuTheGiaTri > 0) {
                    $this.parent().next().find('input').focus();
                }
            }
        },
        BH_EditTienThe: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            self.PhieuThuBaoHiem.TienTheGiaTri = formatNumberToFloat($this.val());
            self.BH_CaculatorDaThanhToan();

            var key = event.keyCode || event.which;
            if (key === 13) {
                if (self.PhieuThuKhach.ID_TaiKhoanPos !== null) {
                    $this.parent().next().find('input').focus();
                }
                else {
                    if (self.PhieuThuKhach.ID_TaiKhoanChuyenKhoan !== null) {
                        $this.parent().next().find('input').focus();
                    }
                    else {

                    }
                }
            }
        },
        Caculator_ThucThu: function () {
            var self = this;
            var kh_tiencoc = formatNumberToFloat(self.PhieuThuKhach.TienDatCoc);
            var kh_tienmat = formatNumberToFloat(self.PhieuThuKhach.TienMat);
            var kh_tienpos = self.KH_GetTongPos();
            var kh_tienck = self.KH_GetTongCK();
            var kh_tienthe = formatNumberToFloat(self.PhieuThuKhach.TienTheGiaTri);
            var kh_tiendiem = formatNumberToFloat(self.PhieuThuKhach.TTBangDiem);
            var kh_tienthua = self.PhieuThuKhach.TienThua;
            kh_tienthua = kh_tienthua > 0 ? kh_tienthua : 0;

            var bh_tienmat = formatNumberToFloat(self.PhieuThuBaoHiem.TienMat);
            var bh_tienpos = formatNumberToFloat(self.PhieuThuBaoHiem.TienPOS);
            var bh_tienck = formatNumberToFloat(self.PhieuThuBaoHiem.TienCK);
            var bh_tienthe = formatNumberToFloat(self.PhieuThuBaoHiem.TienTheGiaTri);
            var bh_tienthua = self.PhieuThuBaoHiem.TienThua;
            bh_tienthua = bh_tienthua > 0 ? bh_tienthua : 0;

            var khachtt = kh_tiencoc + kh_tienmat + kh_tienpos + kh_tienck + kh_tienthe + kh_tiendiem;
            var baohiemtt = bh_tienmat + bh_tienpos + bh_tienck + bh_tienthe;
            var thucthuHD = 0;
            if (self.PhieuThuKhach.HoanTraTamUng > 0) {
                thucthuHD = kh_tiencoc + baohiemtt - bh_tienthua;
            }
            else {
                thucthuHD = khachtt + baohiemtt - kh_tienthua - bh_tienthua;
            }
            self.inforHoaDon.ThucThu = thucthuHD;
            if (self.inforHoaDon.LoaiHoaDon === 6) {
                self.inforHoaDon.ConNo = self.inforHoaDon.PhaiThanhToan - thucthuHD;
            }
            else {
                self.inforHoaDon.ConNo = self.inforHoaDon.TongThanhToan - thucthuHD - self.inforHoaDon.KhachDaTra;
            }
            self.UpdateChietKhauNV_ifChangeThucThu();
        },

        HoaHongHD_UpdateHeSo_AndBind: function () {
            let self = this;
            let heso = 1;
            let lenGrid = self.GridNVienBanGoi_Chosed.length;

            let chiphiNganHang = self.GetChiPhi_Visa();
            let thucthu = formatNumberToFloat(self.PhieuThuKhach.ThucThu) + formatNumberToFloat(self.PhieuThuBaoHiem.ThucThu)
                - chiphiNganHang;
            let doanhthu = formatNumberToFloat(self.inforHoaDon.TongThanhToan) - self.inforHoaDon.TongTienThue;

            if (self.IsShareDiscount === '1') {
                // same pt ckThucThu all nhan vien
                let ptCK_Share = 100 / lenGrid;
                let tienCK_Share = thucthu / lenGrid;

                for (let i = 0; i < self.GridNVienBanGoi_Chosed.length; i++) {
                    let itemFor = self.GridNVienBanGoi_Chosed[i];
                    let tinhCKTheo = parseInt(itemFor.TinhChietKhauTheo);
                    let ptCK = formatNumberToFloat(itemFor.PT_ChietKhau);
                    let tienCK = formatNumberToFloat(itemFor.TienChietKhau);
                    switch (tinhCKTheo) {
                        case 1:
                            tienCK = Math.round(thucthu * ptCK_Share / 100 * heso);
                            break;
                        case 2:
                            tienCK = Math.round(doanhthu * ptCK_Share / 100 * heso);
                            break;
                        case 3:// vnd, keep heso =1
                            if (heso !== 1) {
                                tienCK = itemFor.ChietKhauMacDinh * heso;
                            }
                            else {
                                tienCK = itemFor.ChietKhauMacDinh / heso;
                            }
                            break;
                    }
                    self.GridNVienBanGoi_Chosed[i].HeSo = heso;
                    self.GridNVienBanGoi_Chosed[i].PT_ChietKhau = ptCK_Share;
                    self.GridNVienBanGoi_Chosed[i].ChietKhauMacDinh = formatNumber3Digit(ptCK_Share);
                    self.GridNVienBanGoi_Chosed[i].TienChietKhau = formatNumber3Digit(tienCK);
                }
            }
            else {
                for (let i = 0; i < self.GridNVienBanGoi_Chosed.length; i++) {
                    let itemFor = self.GridNVienBanGoi_Chosed[i];
                    let tinhCKTheo = parseInt(itemFor.TinhChietKhauTheo);
                    let ptCK = formatNumberToFloat(itemFor.PT_ChietKhau);
                    let tienCK = formatNumberToFloat(itemFor.TienChietKhau);
                    switch (tinhCKTheo) {
                        case 1:
                            tienCK = Math.round(thucthu * ptCK / 100 * heso);
                            break;
                        case 2:
                            tienCK = Math.round(doanhthu * ptCK / 100 * heso);
                            break;
                        case 3:// vnd, keep heso =1
                            if (heso !== 1) {
                                tienCK = itemFor.ChietKhauMacDinh * heso;
                            }
                            else {
                                tienCK = itemFor.ChietKhauMacDinh / heso;
                            }
                            break;
                    }
                    self.GridNVienBanGoi_Chosed[i].HeSo = heso;
                    self.GridNVienBanGoi_Chosed[i].TienChietKhau = formatNumber3Digit(tienCK);
                }
            }
        },

        HoaHongHD_ShowDivChietKhau: function (item, index) {
            var self = this;
            var thisObj = $(event.currentTarget);
            if (self.RoleChange_ChietKhauNV === false) {
                ShowMessage_Danger('Không có quyền thay đổi chiết khấu nhân viên');
                return false;
            }
            var pos = thisObj.closest('td').position();
            $('#jsDiscountKH').show();
            $('#jsDiscountKH').css({
                left: (pos.left - 120) + "px",
                top: (pos.top + 71) + "px"
            });

            item.Index = index;
            self.itemChosing = item;
            var tinhCKTheo = parseInt(item.TinhChietKhauTheo);
            var gtriCK = 0;
            switch (tinhCKTheo) {
                case 1:
                    gtriCK = item.ChietKhauMacDinh;
                    break;
                case 2:
                    gtriCK = item.ChietKhauMacDinh;
                    break;
                case 3:
                    gtriCK = formatNumber3Digit(item.ChietKhauMacDinh, 2);
                    break;
            }
            self.LoaiChietKhauHD_NV = tinhCKTheo.toString();
            $(function () {
                let inputNext = $('#jsDiscountKH').find("input[type!='radio']").eq(0);
                $(inputNext).val(gtriCK);
                $(inputNext).focus().select();
            });
        },
        CaculatorAgain_TienDuocNhan: function (gtriCK, heso, tinhCKTheo) {
            let self = this;
            let chiphiNganHang = self.GetChiPhi_Visa();
            let doanhthu = self.inforHoaDon.TongThanhToan - self.inforHoaDon.TongTienThue;
            let thucthu = self.PhieuThuKhach.ThucThu + self.PhieuThuBaoHiem.ThucThu - chiphiNganHang;
            let tienCK = 0;
            switch (parseInt(tinhCKTheo)) {
                case 1:
                    tienCK = Math.round(thucthu * gtriCK / 100 * heso);
                    break;
                case 2:
                    tienCK = Math.round(doanhthu * gtriCK / 100 * heso);
                    break;
                case 3:
                    if (heso !== 1) {
                        tienCK = gtriCK * heso;
                    }
                    else {
                        tienCK = gtriCK / heso;
                    }
                    break;
            }
            return tienCK;
        },

        HoaHongHD_ChangeLoaiChietKhau: function (loaiCK) {
            let self = this;
            let item = self.itemChosing;
            let gtriCK = item.ChietKhauMacDinh;
            let ptramCK = gtriCK;
            let chietKhauMacDinh = 0;
            let chiphiNganHang = self.GetChiPhi_Visa();
            let doanhthu = formatNumberToFloat(self.inforHoaDon.TongThanhToan) - self.inforHoaDon.TongTienThue;
            let thucthu = self.PhieuThuKhach.ThucThu + self.PhieuThuBaoHiem.ThucThu - chiphiNganHang;
            let loaiCK_Old = parseInt(self.LoaiChietKhauHD_NV);
            if (loaiCK_Old === 3) {
                switch (loaiCK) {
                    case 1:// thuc thu
                        ptramCK = gtriCK = RoundDecimal(gtriCK / thucthu * 100);
                        chietKhauMacDinh = ptramCK;
                        break;
                    case 2:// doanh thu
                        ptramCK = gtriCK = RoundDecimal(gtriCK / doanhthu * 100);
                        chietKhauMacDinh = ptramCK;
                        break;
                    case 3:
                        ptramCK = 0;
                        break;
                }
            }
            else {
                switch (loaiCK) {
                    case 3:
                        if (loaiCK_Old === 1) {
                            gtriCK = Math.round(ptramCK * thucthu) / 100;
                        }
                        if (loaiCK_Old === 2) {
                            gtriCK = Math.round(ptramCK * doanhthu) / 100;
                        }
                        ptramCK = 0;
                        chietKhauMacDinh = gtriCK;
                        break;
                }
            }
            let tienCK = self.CaculatorAgain_TienDuocNhan(gtriCK, item.HeSo, loaiCK);
            for (let i = 0; i < self.GridNVienBanGoi_Chosed.length; i++) {
                if (i === item.Index) {
                    self.GridNVienBanGoi_Chosed[i].TinhChietKhauTheo = loaiCK.toString();
                    self.GridNVienBanGoi_Chosed[i].PT_ChietKhau = ptramCK;
                    self.GridNVienBanGoi_Chosed[i].TienChietKhau = formatNumber3Digit(tienCK);
                    if (chietKhauMacDinh !== 0 || (chietKhauMacDinh === 0 && tienCK === 0)) {
                        self.GridNVienBanGoi_Chosed[i].ChietKhauMacDinh = formatNumber3Digit(chietKhauMacDinh);
                    }
                    break;
                }
            }
            self.HoaHongHD_UpdateHeSo_AndBind();
            $(event.currentTarget).closest('div').prev().find('input').select().focus();
        },
        HoaHongHD_EditHeSo: function (item, index) {
            let self = this;
            let thisObj = event.currentTarget;
            let gtriCK = item.ChietKhauMacDinh;
            let heso = formatNumberToFloat($(thisObj).val());
            let tienCK = self.CaculatorAgain_TienDuocNhan(gtriCK, heso, item.TinhChietKhauTheo);
            for (let i = 0; i < self.GridNVienBanGoi_Chosed.length; i++) {
                if (index === i) {
                    self.GridNVienBanGoi_Chosed[i].TienChietKhau = tienCK;
                    break;
                }
            }
        },

        shareMoney_QuyHD: function (phaiTT, tienDiem, tienmat, tienPOS, chuyenkhoan, thegiatri, tiencoc) {
            // thutu uutien: 1.coc, 2.diem, 3.thegiatri, 4.pos, 5.chuyenkhoan, mat
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
        UpdateIDQuyHoaDon_toBHThucHien: function (idHoaDon, idQuyHD) {
            var self = this;
            if (self.GridNVienBanGoi_Chosed.length > 0 && idHoaDon !== null) {
                for (let i = 0; i < self.GridNVienBanGoi_Chosed.length; i++) {
                    self.GridNVienBanGoi_Chosed[i].ID_QuyHoaDon = idQuyHD;
                    self.GridNVienBanGoi_Chosed[i].ID_HoaDon = idHoaDon;
                }

                let myData = {
                    lstObj: self.GridNVienBanGoi_Chosed
                }
                ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'Post_BHNhanVienThucHien', 'POST', myData).done(function (data) {
                    if (data.res == false) {
                        self.GridNVienBanGoi_Chosed = [];
                    }
                })
            }
            else {
                if (self.inforHoaDon.LoaiHoaDon === 6) {
                    ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/InsertChietKhauTraHang_TheoThucThu?idHoaDonTra=' + idHoaDon
                        + '&idPhieuChi=' + idQuyHD, 'GET').done(function (x) {
                            console.log('x ', x)
                            self.GridNVienBanGoi_Chosed = [];
                        });
                }
            }
        },

        AgreeThanhToan: function () {
            var self = this;
            let sumCK = 0;
            for (let i = 0; i < self.GridNVienBanGoi_Chosed.length; i++) {
                let itFor = self.GridNVienBanGoi_Chosed[i];
                sumCK += formatNumberToFloat(itFor.TienChietKhau);
            }

            let thucTinh = self.PhieuThuKhach.ThucThu - self.PhieuThuKhach.TongPhiThanhToan;
            if (sumCK > thucTinh) {
                commonStatisJs.ShowMessageDanger("Tổng phân bổ không được vượt quá " + formatNumber3Digit(thucTinh));
                return;
            }

            self.saveOK = true;
            switch (self.formType) {
                case 1:// gara
                    newModelBanLe.saveHoaDonTraHang();
                    break;
                case 2:// banle
                    self.saveOK = true;
                    break;
                case 3://thegiatri
                    self.saveOK = true;
                    break;
            }
            $('#ThongTinThanhToanKHNCC').modal('hide');
        },

        SavePhieuThu_HDDoiTra: function () {
            var self = this;
            var ptKhach = self.PhieuThuKhach;

            if (ptKhach.DaThanhToan === 0 || ptKhach.DaThanhToan === undefined) {
                // if click btnTrahang - dont' save phieuthu
                return;
            }

            var hd = self.inforHoaDon;
            var ghichu = hd.DienGiai;
            var idHoaDon = hd.ID;
            var idDoiTuong = hd.ID_DoiTuong;
            var tenDoiTuong = hd.TenDoiTuong;
            var idKhoanThuChi = ptKhach.ID_KhoanThuChi;
            var sLoai = 'thu ';
            var maPhieuThuChi = 'TT' + hd.MaHoaDon;
            var chitracoc = self.isCheckTraLaiCoc;
            var tiendatcoc = 0, tienmat = 0, tienck = 0, tienthe = 0, tiendiem = 0, tongthu = 0;
            var lstQuyCT = [], lstQuyCT2 = [];
            var myData = {}, myData2 = {};
            var phuongthucTT = '', phuongthucTT2 = '';

            var chenhLechTraMua = hd.ChenhLechTraMua;
            if (chenhLechTraMua > 0) {//tra > mua
                //Cần tạo 2 phiếu chi: 
                //      +) phiếu chi 1: (ID_LienQuan là hóa đơn trả, HinhThucThanhToan = 1/hoac 3, TongThu = chenhlech tramua
                //      +) phiếu chi 2: chi trả cọc 1 triệu(ID_LienQuan = null, LoaiThanhToan = 1), TongChi = soducoc hientai

                sLoai = 'chi ';
                if (ptKhach.DaThanhToan > 0) {
                    let dataReturn = self.shareMoney_QuyHD(ptKhach.PhaiThanhToan, ptKhach.TTBangDiem,
                        formatNumberToFloat(ptKhach.TienMat), formatNumberToFloat(ptKhach.TienPOS),
                        formatNumberToFloat(ptKhach.TienCK), formatNumberToFloat(ptKhach.TienTheGiaTri),
                        0);

                    tiendatcoc = dataReturn.TienCoc;
                    tienmat = dataReturn.TienMat;
                    tienpos = dataReturn.TienPOS;
                    tienck = dataReturn.TienChuyenKhoan;
                    tienthe = dataReturn.TienTheGiaTri;
                    tiendiem = dataReturn.TTBangDiem;
                    tongthu = tienmat + tienpos + tienck + tienthe + tiendiem + tiendatcoc;

                    let conlai = 0;

                    if (tienck > 0) {
                        if (tienck > chenhLechTraMua) {
                            conlai = tienck - chenhLechTraMua;

                            // phieuchi1: ck all
                            let qct = newQuyChiTiet({
                                ID_HoaDonLienQuan: idHoaDon,
                                ID_KhoanThuChi: idKhoanThuChi,
                                ID_DoiTuong: idDoiTuong,
                                GhiChu: ghichu,
                                TienThu: chenhLechTraMua,
                                TienChuyenKhoan: chenhLechTraMua,
                                HinhThucThanhToan: 3,
                                ID_TaiKhoanNganHang: ptKhach.ID_TaiKhoanChuyenKhoan,
                                LoaiThanhToan: 0,
                            });
                            phuongthucTT += 'Chuyển khoản, ';

                            let nowSeconds = (new Date()).getSeconds() + 1;
                            let quyhd = {
                                LoaiHoaDon: 12,
                                TongTienThu: chenhLechTraMua,
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

                            myData = {
                                objQuyHoaDon: quyhd,
                                lstCTQuyHoaDon: [qct]
                            }

                            // phieuchi2: ck 1phan
                            let qct2 = newQuyChiTiet({
                                ID_HoaDonLienQuan: null,
                                ID_KhoanThuChi: idKhoanThuChi,
                                ID_DoiTuong: idDoiTuong,
                                GhiChu: ghichu,
                                TienThu: conlai,
                                TienChuyenKhoan: conlai,
                                HinhThucThanhToan: 3,
                                ID_TaiKhoanNganHang: ptKhach.ID_TaiKhoanChuyenKhoan,
                                LoaiThanhToan: chitracoc ? 1 : 0,
                            });
                            lstQuyCT2.push(qct2);
                            phuongthucTT2 += 'Chuyển khoản, ';
                        }
                        else {
                            // phieuchi1: ck 1phan
                            let qct = newQuyChiTiet({
                                ID_HoaDonLienQuan: idHoaDon,
                                ID_KhoanThuChi: idKhoanThuChi,
                                ID_DoiTuong: idDoiTuong,
                                GhiChu: ghichu,
                                TienThu: tienck,
                                TienChuyenKhoan: tienck,
                                HinhThucThanhToan: 3,
                                ID_TaiKhoanNganHang: ptKhach.ID_TaiKhoanChuyenKhoan,
                                LoaiThanhToan: 0,
                            });
                            lstQuyCT.push(qct);
                            phuongthucTT += 'Chuyển khoản, ';

                        }
                    }

                    if (tienmat > 0) {
                        if (tienmat > chenhLechTraMua) {
                            let tienmatDaDung = 0;
                            if (lstQuyCT.length > 0) {// da tt 1 phan ck
                                let thuMatCL = chenhLechTraMua - lstQuyCT[0].TienThu;
                                let qct = newQuyChiTiet({
                                    ID_HoaDonLienQuan: idHoaDon,
                                    ID_KhoanThuChi: idKhoanThuChi,
                                    ID_DoiTuong: idDoiTuong,
                                    GhiChu: ghichu,
                                    TienThu: thuMatCL,
                                    TienMat: thuMatCL,
                                    HinhThucThanhToan: 1,
                                    LoaiThanhToan: 0,
                                });
                                lstQuyCT.push(qct);
                                phuongthucTT += 'Tiền mặt, ';
                                tienmatDaDung = thuMatCL;
                            }
                            else {
                                tienmatDaDung = chenhLechTraMua;
                                // phieuchi1: mat all
                                let qct = newQuyChiTiet({
                                    ID_HoaDonLienQuan: idHoaDon,
                                    ID_KhoanThuChi: idKhoanThuChi,
                                    ID_DoiTuong: idDoiTuong,
                                    GhiChu: ghichu,
                                    TienThu: chenhLechTraMua,
                                    TienMat: chenhLechTraMua,
                                    HinhThucThanhToan: 1,
                                    LoaiThanhToan: 0,
                                });
                                phuongthucTT += 'Tiền mặt, ';

                                let nowSeconds = (new Date()).getSeconds() + 1;
                                let quyhd = {
                                    LoaiHoaDon: 12,
                                    TongTienThu: chenhLechTraMua,
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

                                myData = {
                                    objQuyHoaDon: quyhd,
                                    lstCTQuyHoaDon: [qct]
                                }
                            }

                            // phieuchi2: mat 1phan
                            conlai = tienmat - tienmatDaDung;
                            let qct2 = newQuyChiTiet({
                                ID_HoaDonLienQuan: null,
                                ID_KhoanThuChi: idKhoanThuChi,
                                ID_DoiTuong: idDoiTuong,
                                GhiChu: ghichu,
                                TienThu: conlai,
                                TienMat: conlai,
                                HinhThucThanhToan: 1,
                                LoaiThanhToan: chitracoc ? 1 : 0,
                            });
                            lstQuyCT2.push(qct2);
                            phuongthucTT2 += 'Tiền mặt, ';
                        }
                        else {
                            // phieuchi1: mat 1phan
                            let qct = newQuyChiTiet({
                                ID_HoaDonLienQuan: idHoaDon,
                                ID_KhoanThuChi: idKhoanThuChi,
                                ID_DoiTuong: idDoiTuong,
                                GhiChu: ghichu,
                                TienThu: tienmat,
                                TienMat: tienmat,
                                HinhThucThanhToan: 1,
                                LoaiThanhToan: 0,
                            });
                            lstQuyCT.push(qct);
                            phuongthucTT += 'Tiền mặt, ';
                        }
                    }

                    if (lstQuyCT.length > 0) {

                        let tongThu1 = 0;
                        for (let i = 0; i < lstQuyCT.length; i++) {
                            tongThu1 += lstQuyCT[i].TienThu;
                        }
                        let nowSeconds = (new Date()).getSeconds() + 1;
                        let quyhd = {
                            LoaiHoaDon: 12,
                            TongTienThu: tongThu1,
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
                        myData = {
                            objQuyHoaDon: quyhd,
                            lstCTQuyHoaDon: lstQuyCT
                        }
                    }

                    if (lstQuyCT2.length > 0) {

                        let tongThu2 = 0;
                        for (let i = 0; i < lstQuyCT2.length; i++) {
                            tongThu2 += lstQuyCT2[i].TienThu;
                        }
                        let nowSeconds = (new Date()).getSeconds() + 1;
                        let quyhd = {
                            LoaiHoaDon: 12,
                            TongTienThu: tongThu2,
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
                        myData2 = {
                            objQuyHoaDon: quyhd,
                            lstCTQuyHoaDon: lstQuyCT2
                        }
                    }
                }
            }
            else {
                chenhLechTraMua = Math.abs(chenhLechTraMua);
                // mua > tra
                //Cần tạo 1 phieuthu + 1 phieuchi
                //      +) phiếu thu: (ID_LienQuan là hóa đơn mua, HinhThucThanhToan 6)
                //      +) phiếu chi: ID_LienQuan = null, LoaiThanhToan = 1

                // always taophieuthu (thutucoc)
                let qct = newQuyChiTiet({
                    ID_HoaDonLienQuan: idHoaDon,
                    ID_KhoanThuChi: idKhoanThuChi,
                    ID_DoiTuong: idDoiTuong,
                    GhiChu: ghichu,
                    TienThu: chenhLechTraMua,
                    TienCoc: chenhLechTraMua,
                    HinhThucThanhToan: 6,
                });
                phuongthucTT = 'Thu từ cọc';
                let nowSeconds = (new Date()).getSeconds() + 1;
                let quyhd = {
                    LoaiHoaDon: 11,
                    TongTienThu: chenhLechTraMua,
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
                myData = {
                    objQuyHoaDon: quyhd,
                    lstCTQuyHoaDon: [qct]
                }

                // phieuchi tralaitien
                let dataReturn = self.shareMoney_QuyHD(ptKhach.PhaiThanhToan, ptKhach.TTBangDiem,
                    formatNumberToFloat(ptKhach.TienMat), formatNumberToFloat(ptKhach.TienPOS),
                    formatNumberToFloat(ptKhach.TienCK), formatNumberToFloat(ptKhach.TienTheGiaTri),
                    0);

                tiendatcoc = dataReturn.TienCoc;
                tienmat = dataReturn.TienMat;
                tienpos = dataReturn.TienPOS;
                tienck = dataReturn.TienChuyenKhoan;
                tienthe = dataReturn.TienTheGiaTri;
                tiendiem = dataReturn.TTBangDiem;
                tongthu = tienmat + tienpos + tienck + tienthe + tiendiem + tiendatcoc;

                if (tienck > 0) {
                    // phieuchi1: ck all
                    let qct2 = newQuyChiTiet({
                        ID_HoaDonLienQuan: null,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ghichu,
                        TienThu: tienck,
                        TienChuyenKhoan: tienck,
                        HinhThucThanhToan: 3,
                        ID_TaiKhoanNganHang: ptKhach.ID_TaiKhoanChuyenKhoan,
                        LoaiThanhToan: chitracoc ? 1 : 0,
                    });
                    lstQuyCT2.push(qct2);
                    phuongthucTT2 += 'Chuyển khoản, ';
                }

                if (tienmat > 0) {
                    let qct2 = newQuyChiTiet({
                        ID_HoaDonLienQuan: null,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ghichu,
                        TienThu: tienmat,
                        TienMat: tienmat,
                        HinhThucThanhToan: 1,
                        LoaiThanhToan: chitracoc ? 1 : 0,
                    });
                    lstQuyCT2.push(qct2)
                    phuongthucTT2 += 'Tiền mặt, ';
                }
                //let nowSeconds = (new Date()).getSeconds() + 1;
                let quyhd2 = {
                    LoaiHoaDon: 11,
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
                myData2 = {
                    objQuyHoaDon: quyhd2,
                    lstCTQuyHoaDon: lstQuyCT2
                }
            }
            phuongthucTT = Remove_LastComma(phuongthucTT);
            phuongthucTT2 = Remove_LastComma(phuongthucTT2);

            console.log(1, myData, 2, myData2)
            // phieuchi (neu tra > mua), thu (neu mua > tra)
            ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/PostQuy_HoaDon_DefaultIDDoiTuong', 'POST', myData).done(function (x) {
                if (x.res === true) {
                    let maPhieuChi = x.data.MaHoaDon;
                    let diary = {
                        LoaiNhatKy: 1,
                        ID_DonVi: myData.objQuyHoaDon.ID_DonVi,
                        ID_NhanVien: myData.objQuyHoaDon.ID_NhanVien,
                        ChucNang: 'Phiếu '.concat(sLoai),
                        NoiDung: 'Tạo phiếu '.concat(sLoai, maPhieuChi, ' cho hóa đơn ', hd.MaHoaDon,
                            ', Khách hàng: ', myData.objQuyHoaDon.NguoiNopTien, ', với giá trị ', formatNumber3Digit(myData.objQuyHoaDon.TongTienThu, 2),
                            ', Phương thức thanh toán:', phuongthucTT,
                            ', Thời gian: ', moment(myData.objQuyHoaDon.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')),
                        NoiDungChiTiet: 'Tạo phiếu ' + sLoai + ' <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD('.concat(maPhieuChi, ')" >', maPhieuChi, '</a> ',
                            ' cho hóa đơn: <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD(', hd.MaHoaDon, ')" >', hd.MaHoaDon, '</a> ',
                            ', Khách hàng: <a style="cursor: pointer" onclick = "LoadKhachHang_byMaKH(', myData.objQuyHoaDon.NguoiNopTien, ')" >', myData.objQuyHoaDon.NguoiNopTien, '</a> ',
                            '<br /> Giá trị: ', formatNumber3Digit(myData.objQuyHoaDon.TongTienThu, 2),
                            '<br/ > Phương thức thanh toán: ', phuongthucTT,
                            '<br/ > Thời gian: ', moment(myData.objQuyHoaDon.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')
                        )
                    }
                    Insert_NhatKyThaoTac_1Param(diary);

                    vmThemMoiKhach.NangNhomKhachHang(idDoiTuong);
                }
            })

            // phieuchi2
            ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/PostQuy_HoaDon_DefaultIDDoiTuong', 'POST', myData2).done(function (x) {
                if (x.res === true) {
                    let maPhieuChi = x.data.MaHoaDon;
                    let diary = {
                        LoaiNhatKy: 1,
                        ID_DonVi: myData2.objQuyHoaDon.ID_DonVi,
                        ID_NhanVien: myData2.objQuyHoaDon.ID_NhanVien,
                        ChucNang: 'Phiếu chi',
                        NoiDung: 'Tạo phiếu chi '.concat(maPhieuChi, ' cho hóa đơn ', hd.MaHoaDon,
                            ', Khách hàng: ', myData2.objQuyHoaDon.NguoiNopTien, ', với giá trị ', formatNumber3Digit(myData2.objQuyHoaDon.TongTienThu, 2),
                            ', Phương thức thanh toán:', phuongthucTT2, chitracoc ? ' (Trả lại tiền cọc)' : '',
                            ', Thời gian: ', moment(myData2.objQuyHoaDon.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')),
                        NoiDungChiTiet: 'Tạo phiếu chi <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD('.concat(maPhieuChi, ')" >', maPhieuChi, '</a> ',
                            ' cho hóa đơn: <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD(', hd.MaHoaDon, ')" >', hd.MaHoaDon, '</a> ',
                            ', Khách hàng: <a style="cursor: pointer" onclick = "LoadKhachHang_byMaKH(', myData2.objQuyHoaDon.NguoiNopTien, ')" >', myData2.objQuyHoaDon.NguoiNopTien, '</a> ',
                            '<br /> Giá trị: ', formatNumber3Digit(myData2.objQuyHoaDon.TongTienThu, 2),
                            '<br/ > Phương thức thanh toán: ', phuongthucTT2, chitracoc ? ' (Trả lại tiền cọc)' : '',
                            '<br/ > Thời gian: ', moment(myData2.objQuyHoaDon.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')
                        )
                    }
                    Insert_NhatKyThaoTac_1Param(diary);

                    vmThemMoiKhach.NangNhomKhachHang(idDoiTuong);
                }
            })
        },

        SavePhieuThu_Default: function (hd) {
            var loaiThuChi = 11;
            var sLoai = 'thu';
            let lstQuyCT = [];
            let daTT = formatNumberToFloat(hd.DaThanhToan);
            var soduDatCoc = hd.SoDuDatCoc;
            let idDoiTuong = hd.ID_DoiTuong;
            let idHoaDon = hd.ID;
            let tenDoiTuong = hd.TenDoiTuong;
            var ghichu = hd.DienGiai;
            var tiendatcoc = 0;
            var idKhoanThuChi = null;
            if (hd.HoanTraTamUng > 0) {
                loaiThuChi = 12;
                sLoai = 'chi';
            }
            if (daTT > 0 || (soduDatCoc > 0 && loaiThuChi === 11)) {
                let phuongthucTT = '';

                if (soduDatCoc > 0 && loaiThuChi === 11) {
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
                            HinhThucThanhToan: 1,
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
                let nowSeconds = (new Date()).getSeconds() + 1;
                let quyhd = {
                    LoaiHoaDon: loaiThuChi,
                    TongTienThu: daTT + (loaiThuChi == 11 ? soduDatCoc : 0),// chi cong tien coc neu la phieuthu
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
                    if (x.res === true) {
                        quyhd.MaHoaDon = x.data.MaHoaDon;
                        let diary = {
                            LoaiNhatKy: 1,
                            ID_DonVi: quyhd.ID_DonVi,
                            ID_NhanVien: quyhd.ID_NhanVien,
                            ChucNang: 'Phiếu ' + sLoai,
                            NoiDung: 'Tạo phiếu '.concat(sLoai, ' ', quyhd.MaHoaDon, ' cho hóa đơn ', hd.MaHoaDon,
                                ', Khách hàng: ', quyhd.NguoiNopTien, ', với giá trị ', formatNumber3Digit(quyhd.TongTienThu, 2),
                                ', Phương thức thanh toán:', phuongthucTT,
                                ', Thời gian: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')),
                            NoiDungChiTiet: 'Tạo phiếu ' + sLoai + ' <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD('.concat(quyhd.MaHoaDon, ')" >', quyhd.MaHoaDon, '</a> ',
                                ' cho hóa đơn: <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD(', hd.MaHoaDon, ')" >', hd.MaHoaDon, '</a> ',
                                ', Khách hàng: <a style="cursor: pointer" onclick = "LoadKhachHang_byMaKH(', quyhd.NguoiNopTien, ')" >', quyhd.NguoiNopTien, '</a> ',
                                '<br /> Giá trị: ', formatNumber3Digit(quyhd.TongTienThu, 2),
                                '<br/ > Phương thức thanh toán: ', phuongthucTT,
                                '<br/ > Thời gian: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')
                            )
                        }
                        Insert_NhatKyThaoTac_1Param(diary);
                        vmThemMoiKhach.NangNhomKhachHang(idDoiTuong);
                    }
                })
            }
        },

        GetKhoanThuChi_byLoaiChungTu: function (lakhoanthu = false) {
            let self = this;
            let ktc = $.grep(self.listData.KhoanThuChis, function (x) {
                return x.LoaiChungTu === self.inforHoaDon.LoaiHoaDon.toString() && x.LaKhoanThu === lakhoanthu;
            });
            return ktc;
        },

        SavePhieuThu_BuTruDoiTra: function (nvthHoaDon = []) {

        },

        SavePhieuThu: function (nvthHoaDon = []) {
            var self = this;
            var hd = self.inforHoaDon;
            vmThemMoiKhach.inforLogin.ID_DonVi = hd.ID_DonVi;// used to check nangnhomkh

            let idHoaDon = hd.ID;
            let idDoiTuong = hd.ID_DoiTuong;
            let tenDoiTuong = hd.TenDoiTuong;
            let maDoiTuong = hd.MaDoiTuong;
            let ptKhach = self.PhieuThuKhach;
            let idKhoanThuChi = ptKhach.ID_KhoanThuChi;
            let sKhoanThuChi = '';
            let arrPhuongThuc = [];

            let ghichu = hd.DienGiai;
            if (commonStatisJs.CheckNull(ghichu)) {
                ghichu = ghichu.concat(tenDoiTuong, ' (', maDoiTuong, ') /', hd.MaHoaDon);
            }
            else {
                ghichu = ghichu.concat(' / ', tenDoiTuong, ' (', maDoiTuong, ') /', hd.MaHoaDon);
            }

            let loaiThuChi = 11;
            let sLoai = 'thu';
            let tiendatcoc = formatNumberToFloat(ptKhach.TienDatCoc), soduDatCoc = formatNumberToFloat(hd.SoDuDatCoc);
            let maPhieuThuChi = 'TT' + hd.MaHoaDon;
            let chitracoc = self.isCheckTraLaiCoc;
            let tienmat = 0, tienpos = 0, tienck = 0, tienthe = 0, tiendiem = 0, tongthu = 0;
            let ktc = [];

            if (soduDatCoc > 0 && hd.ChenhLechTraMua != 0) {
                self.SavePhieuThu_HDDoiTra();// chưa dùng đến
            }
            else {
                if ((hd.HoanTraTamUng > 0 && soduDatCoc <= 0) || ptKhach.HoanTraTamUng > 0) {
                    loaiThuChi = 12;
                    sLoai = 'chi';
                }
                var khach_PhieuThuTT = hd.PhaiThanhToan;
                if (ptKhach.HoanTraTamUng > 0) {
                    // chitracoc
                    khach_PhieuThuTT = hd.HoanTraTamUng;
                    if (tiendatcoc > 0) {
                        idHoaDon = null; // neu tiencoc > 0 & hoantra tiencho khach --> khong gan idHoaDon (vi no bi bu tru congno)
                    }
                }

                if (commonStatisJs.CheckNull(idKhoanThuChi)) {
                    ktc = self.GetKhoanThuChi_byLoaiChungTu(loaiThuChi === 11);
                }
                if (ktc.length > 0) {
                    idKhoanThuChi = ktc[0].ID;
                    sKhoanThuChi = ktc[0].NoiDungThuChi;
                }

                //  used to get save diary
                if (formatNumberToFloat(ptKhach.DaThanhToan) > 0) {
                    let lstQuyCT = [];
                    let phuongthucTT = '';
                    let dataReturn = self.shareMoney_QuyHD(khach_PhieuThuTT, ptKhach.TTBangDiem,
                        formatNumberToFloat(ptKhach.TienMat), formatNumberToFloat(ptKhach.TienPOS),
                        formatNumberToFloat(ptKhach.TienCK), formatNumberToFloat(ptKhach.TienTheGiaTri),
                        ptKhach.HoanTraTamUng > 0 ? 0 : tiendatcoc);// nếu hoàn trả tiền: không gán tiền cọc ở đây

                    tiendatcoc = dataReturn.TienCoc;
                    tienmat = dataReturn.TienMat;
                    tienpos = dataReturn.TienPOS;
                    tienck = dataReturn.TienChuyenKhoan;
                    tienthe = dataReturn.TienTheGiaTri;
                    tiendiem = dataReturn.TTBangDiem;
                    tongthu = tienmat + tienpos + tienck + tienthe + tiendiem + tiendatcoc;

                    //todo: gán lai tiền khách đã trả/ tiền trả khách --> use when print
                    self.PhieuThuKhachPrint.TienDatCoc = tiendatcoc;
                    self.PhieuThuKhachPrint.TienMat = tienmat;
                    self.PhieuThuKhachPrint.TienCK = tienck;
                    self.PhieuThuKhachPrint.TienPOS = tienpos;
                    self.PhieuThuKhachPrint.TienTheGiaTri = tienthe;
                    self.PhieuThuKhachPrint.TTBangDiem = tiendiem;
                    self.PhieuThuKhachPrint.DaThanhToan = tongthu;

                    // thu tiền cọc
                    if (tiendatcoc > 0 && loaiThuChi === 11) {
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

                        if ($.inArray(qct.HinhThucThanhToan, arrPhuongThuc) === -1) {
                            arrPhuongThuc.push(qct.HinhThucThanhToan);
                        }
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

                        if ($.inArray(qct.HinhThucThanhToan, arrPhuongThuc) === -1) {
                            arrPhuongThuc.push(qct.HinhThucThanhToan);
                        }
                    }
                    
                    if (tienpos > 0) {
                        let qct = {
                            HinhThucThanhToan: 2,
                        }
                        if (!commonStatisJs.CheckNull(ptKhach.ListTKPos)) {
                            for (let i = 0; i < ptKhach.ListTKPos.length; i++) {
                                let itFor = ptKhach.ListTKPos[i];
                                let item_TienPos = formatNumberToFloat(itFor.TienPOS);
                                if (tienpos > item_TienPos) {
                                    tienpos = tienpos - item_TienPos;

                                    qct = newQuyChiTiet({
                                        ID_HoaDonLienQuan: idHoaDon,
                                        ID_KhoanThuChi: idKhoanThuChi,
                                        ID_DoiTuong: idDoiTuong,
                                        GhiChu: itFor.TenTaiKhoanPos.concat(' - ', itFor.TenNganHangPos),
                                        TienThu: item_TienPos,
                                        TienPOS: item_TienPos,
                                        HinhThucThanhToan: 2,
                                        ID_TaiKhoanNganHang: itFor.ID_TaiKhoanPos,
                                        LoaiThanhToan: chitracoc ? 1 : 0,
                                    });
                                    qct.ChiPhiNganHang = itFor.ChiPhiThanhToan;
                                    qct.LaPTChiPhiNganHang = itFor.TheoPhanTram;
                                    lstQuyCT.push(qct);
                                    ghichu += ' / ' + qct.GhiChu;
                                }
                                else {
                                    qct = newQuyChiTiet({
                                        ID_HoaDonLienQuan: idHoaDon,
                                        ID_KhoanThuChi: idKhoanThuChi,
                                        ID_DoiTuong: idDoiTuong,
                                        GhiChu: itFor.TenTaiKhoanPos.concat(' - ', itFor.TenNganHangPos),
                                        TienThu: tienpos,
                                        TienPOS: tienpos,
                                        HinhThucThanhToan: 2,
                                        ID_TaiKhoanNganHang: itFor.ID_TaiKhoanPos,
                                        LoaiThanhToan: chitracoc ? 1 : 0,
                                    });
                                    qct.ChiPhiNganHang = itFor.ChiPhiThanhToan;
                                    qct.LaPTChiPhiNganHang = itFor.TheoPhanTram;
                                    lstQuyCT.push(qct);
                                    tienpos = 0;
                                    ghichu += ' / ' + qct.GhiChu;
                                }
                            }
                        }
                        else {
                            qct = newQuyChiTiet({
                                ID_HoaDonLienQuan: idHoaDon,
                                ID_KhoanThuChi: idKhoanThuChi,
                                ID_DoiTuong: idDoiTuong,
                                GhiChu: ghichu,
                                TienThu: tienpos,
                                TienPOS: tienpos,
                                HinhThucThanhToan: 2,
                                ID_TaiKhoanNganHang: ptKhach.ID_TaiKhoanPos,
                                LoaiThanhToan: chitracoc ? 1 : 0,
                            });
                            lstQuyCT.push(qct);
                        }

                        if ($.inArray(qct.HinhThucThanhToan, arrPhuongThuc) === -1) {
                            arrPhuongThuc.push(qct.HinhThucThanhToan);
                        }
                    }

                    if (tienck > 0) {
                        let qct = {
                            HinhThucThanhToan: 3,
                        }
                        if (!commonStatisJs.CheckNull(ptKhach.ListTKChuyenKhoan)) {
                            for (let i = 0; i < ptKhach.ListTKChuyenKhoan.length; i++) {
                                let itFor = ptKhach.ListTKChuyenKhoan[i];
                                let item_TienCK = formatNumberToFloat(itFor.TienCK);
                                if (tienck > item_TienCK) {
                                    tienck = tienck - item_TienCK;

                                    qct = newQuyChiTiet({
                                        ID_HoaDonLienQuan: idHoaDon,
                                        ID_KhoanThuChi: idKhoanThuChi,
                                        ID_DoiTuong: idDoiTuong,
                                        GhiChu: itFor.TenTaiKhoanCK.concat(' - ', itFor.TenNganHangCK),
                                        TienThu: item_TienCK,
                                        HinhThucThanhToan: 3,
                                        ID_TaiKhoanNganHang: itFor.ID_TaiKhoanChuyenKhoan,
                                    });
                                    qct.ChiPhiNganHang = itFor.ChiPhiThanhToan;
                                    qct.LaPTChiPhiNganHang = itFor.TheoPhanTram;
                                    lstQuyCT.push(qct);
                                    ghichu += ' / ' + qct.GhiChu;
                                    khach_idCK = qct.ID_TaiKhoanNganHang;
                                }
                                else {
                                    qct = newQuyChiTiet({
                                        ID_HoaDonLienQuan: idHoaDon,
                                        ID_KhoanThuChi: idKhoanThuChi,
                                        ID_DoiTuong: idDoiTuong,
                                        GhiChu: itFor.TenTaiKhoanCK.concat(' - ', itFor.TenNganHangCK),
                                        TienThu: tienck,
                                        HinhThucThanhToan: 3,
                                        ID_TaiKhoanNganHang: itFor.ID_TaiKhoanChuyenKhoan,
                                        LoaiThanhToan: chitracoc ? 1 : 0,
                                    });
                                    qct.ChiPhiNganHang = itFor.ChiPhiThanhToan;
                                    qct.LaPTChiPhiNganHang = itFor.TheoPhanTram;
                                    lstQuyCT.push(qct);
                                    tienGui = 0;
                                    ghichu += ' / ' + qct.GhiChu;
                                }
                            }
                        }
                        else {
                            khach_idCK = ptKhach.ID_TaiKhoanChuyenKhoan;
                            qct = newQuyChiTiet({
                                ID_HoaDonLienQuan: idHoaDon,
                                ID_KhoanThuChi: idKhoanThuChi,
                                ID_DoiTuong: idDoiTuong,
                                GhiChu: ghichu,
                                TienThu: tienck,
                                TienChuyenKhoan: tienck,
                                HinhThucThanhToan: 3,
                                ID_TaiKhoanNganHang: ptKhach.ID_TaiKhoanChuyenKhoan,
                                LoaiThanhToan: chitracoc ? 1 : 0,
                            });
                            lstQuyCT.push(qct);
                        }
                        if ($.inArray(qct.HinhThucThanhToan, arrPhuongThuc) === -1) {
                            arrPhuongThuc.push(qct.HinhThucThanhToan);
                        }
                    }
                    if (tienthe > 0) {
                        let qct = newQuyChiTiet({
                            ID_HoaDonLienQuan: idHoaDon,
                            ID_KhoanThuChi: idKhoanThuChi,
                            ID_DoiTuong: idDoiTuong,
                            GhiChu: ghichu,
                            TienThu: tienthe,
                            TienTheGiaTri: tienthe,
                            HinhThucThanhToan: 4,
                        });
                        lstQuyCT.push(qct);

                        if ($.inArray(qct.HinhThucThanhToan, arrPhuongThuc) === -1) {
                            arrPhuongThuc.push(qct.HinhThucThanhToan);
                        }
                    }
                    if (tiendiem > 0) {
                        let qct = newQuyChiTiet({
                            ID_HoaDonLienQuan: idHoaDon,
                            ID_KhoanThuChi: idKhoanThuChi,
                            ID_DoiTuong: idDoiTuong,
                            GhiChu: ghichu,
                            TienThu: tiendiem,
                            TTBangDiem: tiendiem,
                            HinhThucThanhToan: 5,
                            DiemThanhToan: ptKhach.DiemQuyDoi,
                        });
                        lstQuyCT.push(qct);
                        if ($.inArray(qct.HinhThucThanhToan, arrPhuongThuc) === -1) {
                            arrPhuongThuc.push(qct.HinhThucThanhToan);
                        }
                    }

                    arrPhuongThuc = $.unique(arrPhuongThuc);
                    for (let i = 0; i < arrPhuongThuc.length; i++) {
                        switch (arrPhuongThuc[i]) {
                            case 1:
                                phuongthucTT += 'Tiền mặt, ';
                                break;
                            case 2:
                                phuongthucTT += 'POS, ';
                                break;
                            case 3:
                                phuongthucTT += 'Chuyển khoản,  ';
                                break;
                            case 4:
                                phuongthucTT += 'Thẻ giá trị, ';
                                break;
                            case 5:
                                phuongthucTT += 'Điểm, ';
                                break;
                            case 6:
                                phuongthucTT += 'Thu từ cọc, ';
                                break;
                        }
                    }

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
                    quyhd.NoiDungThu = ghichu;

                    self.PhieuThuKhachPrint.PhuongThucTT = phuongthucTT;

                    let myData = {
                        objQuyHoaDon: quyhd,
                        lstCTQuyHoaDon: lstQuyCT
                    };

                    if (lstQuyCT.length > 0) {
                        ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/PostQuy_HoaDon_DefaultIDDoiTuong', 'POST', myData).done(function (x) {
                            if (x.res === true) {

                                quyhd.MaHoaDon = x.data.MaHoaDon;
                                self.UpdateIDQuyHoaDon_toBHThucHien(idHoaDon, x.data.ID);

                                let diary = {
                                    LoaiNhatKy: 1,
                                    ID_DonVi: quyhd.ID_DonVi,
                                    ID_NhanVien: quyhd.ID_NhanVien,
                                    ChucNang: 'Phiếu ' + sLoai,
                                    NoiDung: 'Tạo phiếu '.concat(sLoai, ' ', quyhd.MaHoaDon, ' cho hóa đơn ', hd.MaHoaDon,
                                        ', Khách hàng: ', quyhd.NguoiNopTien, ', với giá trị ', formatNumber3Digit(quyhd.TongTienThu, 2),
                                        ', Phương thức thanh toán:', phuongthucTT, chitracoc ? ' (Trả lại tiền cọc)' : '',
                                        ', Thời gian: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')),
                                    NoiDungChiTiet: 'Tạo phiếu ' + sLoai + ' <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD('.concat(quyhd.MaHoaDon, ')" >', quyhd.MaHoaDon, '</a> ',
                                        ' cho hóa đơn: <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD(', hd.MaHoaDon, ')" >', hd.MaHoaDon, '</a> ',
                                        ', Khách hàng: <a style="cursor: pointer" onclick = "LoadKhachHang_byMaKH(', quyhd.NguoiNopTien, ')" >', quyhd.NguoiNopTien, '</a> ',
                                        '<br /> Giá trị: ', formatNumber3Digit(quyhd.TongTienThu, 2),
                                        '<br/ > Phương thức thanh toán: ', phuongthucTT, chitracoc ? ' (Trả lại tiền cọc)' : '',
                                        '<br /> Khoản ', sLoai, ': ', sKhoanThuChi,
                                        '<br/ > Thời gian: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')
                                    )
                                }
                                Insert_NhatKyThaoTac_1Param(diary);
                                vmThemMoiKhach.NangNhomKhachHang(idDoiTuong);
                            }
                        })
                    }
                    else {
                        self.isLoading = false;
                    }
                }

                // thu tu coc
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
                    let phuongthucTT2 = 'Thu từ cọc';
                    let sLoai2 = 'thu';
                    tongthu = tienCocX;

                    self.PhieuThuKhachPrint.TienDatCoc = tienCocX;
                    self.PhieuThuKhachPrint.DaThanhToan = tienCocX;

                    let nowSeconds = (new Date()).getSeconds() + 1;
                    let quyhd = {
                        LoaiHoaDon: 11,
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
                    quyhd.PhuongThucTT = phuongthucTT2;

                    let myData = {
                        objQuyHoaDon: quyhd,
                        lstCTQuyHoaDon: lstQuyCT
                    };

                    if (lstQuyCT.length > 0) {
                        console.log('myData_quyKH ', myData);
                        ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/PostQuy_HoaDon_DefaultIDDoiTuong', 'POST', myData).done(function (x) {
                            if (x.res === true) {
                                self.UpdateIDQuyHoaDon_toBHThucHien(lstQuyCT[0].ID_HoaDonLienQuan, x.data.ID);

                                quyhd.MaHoaDon = x.data.MaHoaDon;
                                let diary = {
                                    LoaiNhatKy: 1,
                                    ID_DonVi: quyhd.ID_DonVi,
                                    ID_NhanVien: quyhd.ID_NhanVien,
                                    ChucNang: 'Phiếu ' + sLoai2,
                                    NoiDung: 'Tạo phiếu '.concat(sLoai2, ' ', quyhd.MaHoaDon, ' cho hóa đơn ', hd.MaHoaDon,
                                        ', Khách hàng: ', quyhd.NguoiNopTien, ', với giá trị ', formatNumber3Digit(quyhd.TongTienThu, 2),
                                        ', Phương thức thanh toán:', phuongthucTT2,
                                        ', Thời gian: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')),
                                    NoiDungChiTiet: 'Tạo phiếu ' + sLoai2 + ' <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD('.concat(quyhd.MaHoaDon, ')" >', quyhd.MaHoaDon, '</a> ',
                                        ' cho hóa đơn: <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD(', hd.MaHoaDon, ')" >', hd.MaHoaDon, '</a> ',
                                        ', Khách hàng: <a style="cursor: pointer" onclick = "LoadKhachHang_byMaKH(', quyhd.NguoiNopTien, ')" >', quyhd.NguoiNopTien, '</a> ',
                                        '<br /> Giá trị: ', formatNumber3Digit(quyhd.TongTienThu, 2),
                                        '<br/ > Phương thức thanh toán: ', phuongthucTT2,
                                        '<br/ > Thời gian: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')
                                    )
                                }
                                Insert_NhatKyThaoTac_1Param(diary);
                                vmThemMoiKhach.NangNhomKhachHang(idDoiTuong);
                            }
                        });
                    }
                }

                let ptBaoHiem = self.PhieuThuBaoHiem;
                if (ptBaoHiem.DaThanhToan > 0) {
                    let lstQuyCT = [];
                    let phuongthucTT = '';
                    let idBaoHiem = hd.ID_BaoHiem;
                    idBaoHiem = idBaoHiem === null ? const_GuidEmpty : idBaoHiem;

                    let dataReturn = self.shareMoney_QuyHD(hd.PhaiThanhToanBaoHiem, formatNumberToFloat(ptBaoHiem.TTBangDiem),
                        formatNumberToFloat(ptBaoHiem.TienMat), formatNumberToFloat(ptBaoHiem.TienPOS),
                        formatNumberToFloat(ptBaoHiem.TienCK), formatNumberToFloat(ptBaoHiem.TienTheGiaTri),
                        formatNumberToFloat(ptBaoHiem.TienDatCoc));
                    tiendatcoc = dataReturn.TienCoc;
                    tienmat = dataReturn.TienMat;
                    tienpos = dataReturn.TienPOS;
                    tienck = dataReturn.TienChuyenKhoan;
                    tienthe = dataReturn.TienTheGiaTri;
                    tiendiem = dataReturn.TTBangDiem;
                    tongthu = tienmat + tienpos + tienck + tienthe + tiendiem;

                    self.PhieuThuBaoHiemPrint.TienDatCoc = tiendatcoc;
                    self.PhieuThuBaoHiemPrint.TienMat = tienmat;
                    self.PhieuThuBaoHiemPrint.TienCK = tienck;
                    self.PhieuThuBaoHiemPrint.TienPOS = tienpos;
                    self.PhieuThuBaoHiemPrint.TienTheGiaTri = tienthe;
                    self.PhieuThuBaoHiemPrint.TTBangDiem = tiendiem;
                    self.PhieuThuBaoHiemPrint.DaThanhToan = tongthu;

                    if (tienmat > 0) {
                        let qct = newQuyChiTiet({
                            ID_HoaDonLienQuan: hd.ID,
                            ID_KhoanThuChi: idKhoanThuChi,
                            ID_DoiTuong: idBaoHiem,
                            GhiChu: ghichu,
                            TienThu: tienmat,
                            TienMat: tienmat,
                            HinhThucThanhToan: 1,
                        });
                        lstQuyCT.push(qct);
                        phuongthucTT += 'Tiền mặt, ';
                    }
                    if (tienpos > 0) {
                        let qct = newQuyChiTiet({
                            ID_HoaDonLienQuan: hd.ID,
                            ID_KhoanThuChi: idKhoanThuChi,
                            ID_DoiTuong: idBaoHiem,
                            GhiChu: ghichu,
                            TienThu: tienpos,
                            TienPOS: tienpos,
                            HinhThucThanhToan: 2,
                            ID_TaiKhoanNganHang: ptBaoHiem.ID_TaiKhoanPos,
                        });
                        lstQuyCT.push(qct);
                        phuongthucTT += 'POS, ';
                    }
                    if (tienck > 0) {
                        let qct = newQuyChiTiet({
                            ID_HoaDonLienQuan: hd.ID,
                            ID_KhoanThuChi: idKhoanThuChi,
                            ID_DoiTuong: idBaoHiem,
                            GhiChu: ghichu,
                            TienThu: tienck,
                            TienChuyenKhoan: tienck,
                            HinhThucThanhToan: 3,
                            ID_TaiKhoanNganHang: ptBaoHiem.ID_TaiKhoanChuyenKhoan,
                        });
                        lstQuyCT.push(qct);
                        phuongthucTT += 'Chuyển khoản, ';
                    }
                    if (tienthe > 0) {
                        let qct = newQuyChiTiet({
                            ID_HoaDonLienQuan: hd.ID,
                            ID_KhoanThuChi: idKhoanThuChi,
                            ID_DoiTuong: idBaoHiem,
                            GhiChu: ghichu,
                            TienThu: tienthe,
                            TienTheGiaTri: tienthe,
                            HinhThucThanhToan: 4,
                        });
                        lstQuyCT.push(qct);
                        phuongthucTT += 'Thẻ giá trị, ';
                    }
                    if (tiendiem > 0) {
                        let qct = newQuyChiTiet({
                            ID_HoaDonLienQuan: hd.ID,
                            ID_KhoanThuChi: idKhoanThuChi,
                            ID_DoiTuong: idBaoHiem,
                            GhiChu: ghichu,
                            TienThu: tiendiem,
                            TTBangDiem: tiendiem,
                            HinhThucThanhToan: 5,
                            DiemThanhToan: ptBaoHiem.DiemQuyDoi,
                        });
                        lstQuyCT.push(qct);
                        phuongthucTT += 'Điểm, ';
                    }

                    if (ptKhach.DaThanhToan > 0) {
                        maPhieuThuChi = 'TT' + hd.MaHoaDon + '_2';
                    }

                    let nowSeconds = (new Date()).getSeconds() + 1;
                    let quyhd = {
                        LoaiHoaDon: 11,
                        TongTienThu: tongthu,
                        MaHoaDon: maPhieuThuChi,
                        NgayLapHoaDon: moment(hd.NgayLapHoaDon).add(nowSeconds, 'seconds').format('YYYY-MM-DD HH:mm:ss'),
                        NguoiNopTien: hd.TenBaoHiem,
                        NguoiTao: hd.NguoiTao,
                        NoiDungThu: ghichu,
                        ID_NhanVien: hd.ID_NhanVien,
                        ID_DonVi: hd.ID_DonVi,
                        ID_DoiTuong: idBaoHiem,
                    };
                    phuongthucTT = Remove_LastComma(phuongthucTT);
                    quyhd.PhuongThucTT = phuongthucTT;
                    self.PhieuThuBaoHiemPrint.PhuongThucTT = phuongthucTT;

                    var myData = {
                        objQuyHoaDon: quyhd,
                        lstCTQuyHoaDon: lstQuyCT
                    };
                    if (lstQuyCT.length > 0) {
                        console.log('myData_quyBH ', myData);
                        ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/PostQuy_HoaDon_DefaultIDDoiTuong', 'POST', myData).done(function (x) {
                            if (x.res === true) {
                                quyhd.MaHoaDon = x.data.MaHoaDon;
                                let diary = {
                                    LoaiNhatKy: 1,
                                    ID_DonVi: quyhd.ID_DonVi,
                                    ID_NhanVien: quyhd.ID_NhanVien,
                                    ChucNang: 'Phiếu thu',
                                    NoiDung: 'Tạo phiếu thu '.concat(quyhd.MaHoaDon, ' cho hóa đơn ', hd.MaHoaDon,
                                        ', Khách hàng: ', quyhd.NguoiNopTien, ', với giá trị ', formatNumber3Digit(quyhd.TongTienThu, 2),
                                        ', Phương thức thanh toán:', phuongthucTT,
                                        ', Thời gian: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')),
                                    NoiDungChiTiet: 'Tạo phiếu thu <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD('.concat(quyhd.MaHoaDon, ')" >', quyhd.MaHoaDon, '</a> ',
                                        ' cho hóa đơn: <a style="cursor: pointer" onclick = "LoadHoaDon_byMaHD(', hd.MaHoaDon, ')" >', hd.MaHoaDon, '</a> ',
                                        ', Khách hàng: <a style="cursor: pointer" onclick = "LoadKhachHang_byMaKH(', quyhd.NguoiNopTien, ')" >', quyhd.NguoiNopTien, '</a> ',
                                        '<br /> Giá trị:', formatNumber3Digit(quyhd.TongTienThu, 2),
                                        '<br/ > Phương thức thanh toán:', phuongthucTT,
                                        '<br/ > Thời gian: ', moment(quyhd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')
                                    )
                                }
                                Insert_NhatKyThaoTac_1Param(diary);
                            }
                        })
                    }
                }

                if ((ptKhach.DaThanhToan === undefined || ptKhach.DaThanhToan === 0)
                    && (ptBaoHiem.DaThanhToan === undefined || ptBaoHiem.DaThanhToan === 0) && hd.ID_PhieuTiepNhan === null) {
                    //self.SavePhieuThu_Default(hd);
                }

                if (nvthHoaDon.length > 0 && idHoaDon !== null &&
                    (commonStatisJs.CheckNull(ptKhach.DaThanhToan) || ptKhach.DaThanhToan === 0)
                    && (commonStatisJs.CheckNull(ptBaoHiem.DaThanhToan) || ptBaoHiem.DaThanhToan === 0)
                ) {
                    // only get cktheo doanhthu
                    self.GridNVienBanGoi_Chosed = nvthHoaDon.filter(x => x.TinhChietKhauTheo !== 1);
                    self.UpdateIDQuyHoaDon_toBHThucHien(idHoaDon);
                }
            }

            $('#ThongTinThanhToanKHNCC').modal('hide');
        },
    },
})
