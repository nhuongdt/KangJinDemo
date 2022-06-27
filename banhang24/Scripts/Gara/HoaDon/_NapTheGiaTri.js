var vmThemMoiTheNap = new Vue({
    el: '#vmThemMoiTheNap',
    components: {
        'account-bank': cmpChoseAccountBank,
        'my-date-time': cpmDatetime,
        'nhanviens': ComponentChoseStaff,
        'khoan-thu-chi': cmpChoseKhoanThu,
        'customers': cmpChoseCustomer,
        'date-range-picker': cmpDateRange,
    },
    created: function () {
        var self = this;
        self.role.ChangeNgayNapThe = self.CheckRole('TheGiaTri_ThayDoiThoiGian');
        console.log('napTGT');
    },
    data: {
        saveOK: false,
        isLoading: false,
        typeUpdate: 1,
        formType: 0,// 0.from DSTheGT, 1. from DS KhachHang
        isShowList: false,
        tgtOld: {},
        role: {
            ChangeNgayNapThe: true,
        },
        cusChosing: {
            MaDoiTuong: '',
            TenDoiTuong: '',
            DienThoai: '',
            DiaChi: '',
        },

        inforCongTy: {
            TenCongTy: '',
            DiaChiCuaHang: '',
            LogoCuaHang: ''
        },
        LichSuNapTien: {
            Data: [],
            TotalRow: 0,
            TotalPage: 0,
            CurrentPage: 0,
            PageSize: 10,
            FromItem: 0,
            ToItem: 0,
            FromDate: moment().startOf('month').format('YYYY-MM-DD'),
            ToDate: moment().endOf('month').format('YYYY-MM-DD'),
            DateRange: '',
        },
        inforTheGiaTri: {// used to print
            SoDuTheGiaTri: 0,
        },

        newHoaDon: {
        },
        listData: {
            KhachHangs: [],
            NhanViens: [],
            ListLichSuNap: [],
        },
    },
    methods: {
        CheckRole: function (maquyen) {
            var xx = VHeader.Quyen.indexOf(maquyen) > -1;
            return xx;
        },

        GetSoDuTheGiaTri: function (idDoiTuong) {
            var self = this;
            let sodu = 0;
            var date = moment(new Date()).format('YYYY-MM-DD HH:mm:ss');
            ajaxHelper('/api/DanhMuc/DM_DoiTuongAPI/' + 'Get_SoDuTheGiaTri_ofKhachHang?idDoiTuong=' + idDoiTuong
                + '&datetime=' + date, 'GET').done(function (data) {
                    if (data != null && data.length > 0) {
                        sodu = data[0].SoDuTheGiaTri;
                        sodu = sodu > 0 ? sodu : 0;
                    }
                    if (self.formType === 2) {
                        sodu = sodu - self.newHoaDon.TongTienHang;
                    }
                    self.inforTheGiaTri.SoDuTheGiaTri = sodu;
                    self.inforTheGiaTri.TongTaiKhoanThe = data[0].TongThuTheGiaTri;
                    self.inforTheGiaTri.SoDuTheSauNap = sodu;
                });
        },

        showLSNapTien: function () {
            var self = this;
            self.isShowList = !self.isShowList;
        },

        ChangeDateRange: function (picker) {
            var self = this;
            self.LichSuNapTien.FromDate = picker.startDate.format('YYYY-MM-DD');
            self.LichSuNapTien.ToDate = picker.endDate.format('YYYY-MM-DD');
            self.LichSuNapTien.DateRange = picker.startDate.format('DD/MM/YYYY') + " - " + picker.endDate.format('DD/MM/YYYY');
        },

        GetLichSuNapTien: function () {
            var self = this;
            var dtNow = new Date();
            //var arrDate = $('#datetimeTimKiemLichSu').val().split('-');
            var dayStart = '2016-01-01';
            var dayEnd = moment(dtNow).add(1, 'days').format('YYYY-MM-DD');

            var iddoituong = self.newHoaDon.ID_DoiTuong;
            ajaxHelper('/api/DanhMuc/DM_DoiTuongAPI/' + 'GetLichSuNapTienByIDDoiTuong?iddt=' + iddoituong
                + '&currentpage=' + self.LichSuNapTien.CurrentPage + '&pagesize=' + self.LichSuNapTien.PageSize
                + '&dayStart=' + dayStart + '&dayEnd=' + dayEnd, 'GET').done(function (data) {
                    console.log(data)
                    self.LichSuNapTien.Data = data.lst;
                    self.LichSuNapTien.TotalRow = data.Rowcount;
                    self.LichSuNapTien.TotalPage = data.pageCount;
                });
        },

        GoToPage: function (page) {
            var self = this;
            if (page.pageNumber !== '.') {
                self.LichSuNapTien.CurrentPage = page.pageNumber - 1;
                self.GetLichSuNapTien();
            }
        },

        StartPage: function () {
            var self = this;
            self.LichSuNapTien.CurrentPage = 0;
            self.GetLichSuNapTien();
        },

        BackPage: function () {
            var self = this;
            if (self.LichSuNapTien.CurrentPage > 1) {
                self.LichSuNapTien.CurrentPage = self.LichSuNapTien.CurrentPage - 1;
                self.GetLichSuNapTien();
            }
        },

        GoToNextPage: function () {
            var self = this;
            if (self.LichSuNapTien.CurrentPage < self.LichSuNapTien.PageSize - 1) {
                self.LichSuNapTien.CurrentPage = self.LichSuNapTien.CurrentPage + 1;
                self.GetLichSuNapTien();
            }
        },

        EndPage: function () {
            var self = this;
            if (self.LichSuNapTien.CurrentPage < self.LichSuNapTien.PageSize - 1) {
                self.LichSuNapTien.CurrentPage = self.LichSuNapTien.PageSize - 1;
                self.GetLichSuNapTien();
            }
        },

        GetClass: function (page) {
            var self = this;
            return ((page.pageNumber - 1) === self.LichSuNapTien.CurrentPage) ? "click" : "";
        },

        showModalUpdate: function (idThe, formType = 0) {
            let self = this;
            self.saveOk = false;
            self.isLoading = false;
            self.typeUpdate = 2;
            self.formType = formType;

            ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/GetInforTheGiaTri_byID?id=' + idThe).done(function (x) {
                if (x.res && x.dataSoure.length > 0) {
                    let data = x.dataSoure[0];
                    self.tgtOld = $.extend({}, true, data);

                    self.newHoaDon = {
                        ID: data.ID,
                        MaHoaDon: data.MaHoaDon,
                        LoaiHoaDon: data.LoaiHoaDon,
                        ChoThanhToan: data.ChoThanhToan,
                        NgayLapHoaDon: moment(data.NgayLapHoaDon).format('YYYY-MM-DD HH:mm'),
                        ID_DonVi: data.ID_DonVi,
                        ID_NhanVien: data.ID_NhanVien,
                        NguoiTao: data.NguoiTao,
                        ID_DoiTuong: data.ID_DoiTuong,
                        TongChiPhi: formatNumber3Digit(data.MucNap),
                        TongChietKhau: data.KhuyenMaiVND,
                        TongTienHang: data.TongTienNap,
                        TongGiamGia: data.TongGiamGia,
                        TongTienThue: data.SoDuSauNap,
                        PhaiThanhToan: data.PhaiThanhToan,
                        TongThanhToan: data.PhaiThanhToan,
                        KhachDaTra: data.KhachDaTra,
                        DienGiai: data.GhiChu,

                        PTKhuyenMai: data.PTKhuyenMai,
                        PTChietKhau: data.PTChietKhau,

                        DaThanhToan: 0,
                        ThucThu: 0,
                        TienThua: 0,
                        NoHienTai: 0,
                        TenLoaiTien: '(Tiền mặt)',
                        BH_NhanVienThucHiens: [],
                        TongCong: data.PhaiThanhToan - data.KhachDaTra
                    };

                    self.GetSoDuTheGiaTri(data.ID_DoiTuong);

                    self.cusChosing = {
                        MaDoiTuong: data.MaKhachHang,
                        TenDoiTuong: data.TenKhachHang,
                        DienThoai: data.SoDienThoai,
                        DiaChi: data.DiaChiKhachHang,
                    }

                    $('#vmThemMoiTheNap').modal('show');
                }
            })

        },
        showModalAddNew: function (formType = 0) {
            var self = this;
            self.saveOk = false;
            self.isLoading = false;
            self.typeUpdate = 1;
            self.formType = formType;

            self.newHoaDon = {
                ID: '00000000-0000-0000-0000-000000000000',
                MaHoaDon: '',
                LoaiHoaDon: 22,
                ChoThanhToan: false,
                NgayLapHoaDon: moment(new Date()).format('YYYY-MM-DD HH:mm'),
                ID_DonVi: VHeader.IdDonVi,
                ID_NhanVien: VHeader.IdNhanVien,
                NguoiTao: VHeader.UserLogin,
                ID_DoiTuong: null,// nguoinop
                TongChiPhi: 0, // mucnap
                TongChietKhau: 0,// khuyenmai
                TongTienHang: 0, // tongnap =mucnap + khuyenmai 
                TongGiamGia: 0, // giamgia
                TongTienThue: 0, // sodusaunap
                PhaiThanhToan: 0, // mucnap - giamgia
                TongThanhToan: 0, // = phaitt
                DienGiai: '',

                PTKhuyenMai: 0,
                PTChietKhau: 0,

                KhachDaTra: 0,
                DaThanhToan: 0,
                ThucThu: 0,
                TienThua: 0,
                NoHienTai: 0,
                TenLoaiTien: '(Tiền mặt)',
                BH_NhanVienThucHiens: [],
            };

            self.inforTheGiaTri = {
                TongTaiKhoanThe: 0,
                TongSuDungThe: 0,
                SoDuTheGiaTri: 0,
                SoDuTheSauNap: 0,
            };

            self.cusChosing = {
                MaDoiTuong: '',
                TenDoiTuong: '',
                DienThoai: '',
                DiaChi: '',
            }
            console.log('napthe')
            $('#vmThemMoiTheNap').modal('show');
        },
        ChangeCustomer: function (item) {
            var self = this;
            var idCus = item.ID;
            self.newHoaDon.ID_DoiTuong = idCus;
            self.cusChosing.MaDoiTuong = item.MaDoiTuong;
            self.cusChosing.TenDoiTuong = item.TenDoiTuong;
            self.cusChosing.DienThoai = item.DienThoai;
            self.GetSoDuTheGiaTri(idCus);
            self.GetLichSuNapTien();
        },

        ChangeNgayLapPhieu: function (e) {
            let self = this;
            let dt = moment(e).format('YYYY-MM-DD HH:mm');
            let khoaSo = VHeader.CheckKhoaSo(moment(e).format('YYYY-MM-DD'));
            if (khoaSo) {
                ShowMessage_Danger(VHeader.warning.ChotSo.Update);
            }
            self.newHoaDon.NgayLapHoaDon = dt;
        },

        CaculatorAgain_InforHoaDon: function (isEditVND = false) {
            var self = this;
            var hd = self.newHoaDon;
            var ptKM = formatNumberToFloat(hd.PTKhuyenMai);
            var gtriKM = formatNumberToFloat(hd.TongChietKhau);
            var ptCK = formatNumberToFloat(hd.PTChietKhau);
            var gtriCK = formatNumberToFloat(hd.TongGiamGia);

            var mucnap = formatNumberToFloat(hd.TongChiPhi);
            if (ptKM > 0 && !isEditVND) {
                gtriKM = mucnap * ptKM / 100;
            }
            if (ptCK > 0 && !isEditVND) {
                gtriCK = mucnap * ptCK / 100;
            }
            self.newHoaDon.TongChietKhau = formatNumber(gtriKM);
            self.newHoaDon.TongTienHang = mucnap + gtriKM;
            self.newHoaDon.TongGiamGia = formatNumber(gtriCK);
            self.newHoaDon.PhaiThanhToan = mucnap - gtriCK;
            self.newHoaDon.ThucThu = mucnap - gtriCK;
            self.newHoaDon.TienThua = 0;
            let khachcantra = mucnap - gtriCK - self.newHoaDon.KhachDaTra;
            if (khachcantra < 0) {
                self.newHoaDon.HoanTraTamUng = Math.abs(khachcantra);
                self.newHoaDon.TongCong = 0;
                self.newHoaDon.DaThanhToan = formatNumber3Digit(self.newHoaDon.HoanTraTamUng);
            }
            else {
                self.newHoaDon.HoanTraTamUng = 0;
                self.newHoaDon.TongCong = khachcantra;
                self.newHoaDon.DaThanhToan = formatNumber3Digit(khachcantra);
            }
            self.inforTheGiaTri.SoDuTheSauNap = self.inforTheGiaTri.SoDuTheGiaTri + self.newHoaDon.TongTienHang;
            self.Assign_ValueKhachThanhToan(self.newHoaDon.DaThanhToan);
        },

        EditMucNap: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            self.newHoaDon.TongChiPhi = formatNumber($this.val());
            self.CaculatorAgain_InforHoaDon();
        },
        EditKhuyenMai_Ptram: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var ptKM = formatNumberToFloat($this.val());
            if (ptKM === 0) {
                self.newHoaDon.TongChietKhau = 0;
            }
            //không gán lại self.newHoaDon.PTKhuyenMai, vì bị format gtri có dấu thập phân
            self.CaculatorAgain_InforHoaDon();
        },
        EditKhuyenMai_VND: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var gtKM = formatNumberToFloat($this.val());
            var ptKM = gtKM / formatNumberToFloat(self.newHoaDon.TongChiPhi) * 100;
            self.newHoaDon.PTKhuyenMai = ptKM;
            self.newHoaDon.TongChietKhau = formatNumber($this.val());
            self.CaculatorAgain_InforHoaDon(true);
        },
        EditGiamGia_Ptram: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var ptCK = formatNumberToFloat($this.val());
            if (ptCK === 0) {
                self.newHoaDon.TongGiamGia = 0;
            }
            self.CaculatorAgain_InforHoaDon();
        },

        EditGiamGia_VND: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);

            var gtCK = formatNumberToFloat($this.val());
            var ptCK = gtCK / formatNumberToFloat(self.newHoaDon.TongChiPhi) * 100;
            self.newHoaDon.PTChietKhau = ptCK;
            self.newHoaDon.TongGiamGia = formatNumber($this.val());
            self.CaculatorAgain_InforHoaDon(true);
        },

        EditTienKhachDua: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);
            var daTT = formatNumberToFloat(self.newHoaDon.DaThanhToan);
            self.newHoaDon.DaThanhToan = formatNumber($this.val());
            self.newHoaDon.TienThua = daTT - self.newHoaDon.PhaiThanhToan;
            self.Assign_ValueKhachThanhToan(daTT);
        },
        Assign_ValueKhachThanhToan: function (daTT) {
            let self = this;
            vmThanhToanGara.PhieuThuKhach.TienMat = daTT;
            vmThanhToanGara.PhieuThuKhach.TienPOS = 0;
            vmThanhToanGara.PhieuThuKhach.TienCK = 0;
            vmThanhToanGara.PhieuThuKhach.TienTheGiaTri = 0;
            vmThanhToanGara.PhieuThuKhach.DiemQuyDoi = 0;
            vmThanhToanGara.PhieuThuKhach.TTBangDiem = 0;
            vmThanhToanGara.PhieuThuKhach.DaThanhToan = daTT;
            if (self.newHoaDon.HoanTraTamUng > 0) {
                vmThanhToanGara.PhieuThuKhach.HoanTraTamUng = self.newHoaDon.HoanTraTamUng;
            }
            else {
                vmThanhToanGara.PhieuThuKhach.HoanTraTamUng = 0;
            }
        },

        SaveTheNap: function (print) {
            var self = this;
            var hd = self.newHoaDon;
            if (hd.ID_DoiTuong === null) {
                commonStatisJs.ShowMessageDanger('Vui lòng chọn người nộp tiền');
                return;
            }

            let khoaSo = VHeader.CheckKhoaSo(moment(hd.NgayLapHoaDon).format('YYYY-MM-DD'));
            if (khoaSo) {
                ShowMessage_Danger(VHeader.warning.ChotSo.Update);
                return;
            }
            self.isLoading = true;

            var nowSeconds = (new Date()).getSeconds();
            hd.NgayLapHoaDon = moment(hd.NgayLapHoaDon).add(nowSeconds, 'seconds').format('YYYY-MM-DD HH:mm:ss');
            hd.TongThanhToan = hd.PhaiThanhToan;
            hd.TongTienThue = self.inforTheGiaTri.SoDuTheSauNap;

            if (self.typeUpdate === 2) {
                hd.NguoiSua = VHeader.UserLogin;
            }

            var myData = {
                objHoaDon: hd
            }

            let sDiary = '<br /> <b>Thông tin chi tiết </b>'.concat(
                '<br /> - Mã hóa đơn: ', hd.MaHoaDon,
                '<br /> - Khách hàng: ', self.cusChosing.TenDoiTuong, '(', self.cusChosing.MaDoiTuong, ')',
                '<br /> - Mức nạp: ', hd.TongChiPhi,
                '<br /> - Khuyến mãi: ', hd.TongChietKhau, ' (', hd.PTKhuyenMai, ' %)',
                '<br /> - Chiết khấu: ', hd.TongGiamGia, ' (', hd.PTChietKhau, ' %)',
                '<br /> - Phải thanh toán: ', formatNumber(hd.PhaiThanhToan),
                '<br /> - Ngày nạp: ', hd.NgayLapHoaDon
            )
            ajaxHelper('/api/DanhMuc/Bh_HoaDonAPI/PostBH_HoaDonNapThe', 'POST', myData).done(function (x) {
                console.log(x)
                if (x.res === true) {
                    self.saveOK = true;
                    self.newHoaDon.ID = x.dataSoure.ID;
                    self.newHoaDon.MaHoaDon = x.dataSoure.MaHoaDon;
                    commonStatisJs.ShowMessageSuccess('Nạp thẻ thành công');

                    let diary = {
                        LoaiNhatKy: 1,
                        ID_DonVi: VHeader.IdDonVi,
                        ID_NhanVien: VHeader.IdNhanVien,
                        ChucNang: 'Nạp thẻ',
                        NoiDung: (self.typeUpdate === 1 ? 'Tạo mới thẻ nạp ' : 'Cập nhật thẻ nạp ').concat(self.newHoaDon.MaHoaDon, ' cho khách hàng ', self.cusChosing.TenDoiTuong),
                        NoiDungChiTiet: sDiary
                    }
                    console.log('VHeader.IdNhanVien', VHeader.IdNhanVien)

                    if (self.typeUpdate === 2) {
                        diary.LoaiNhatKy = 2;
                        diary.NoiDungChiTiet = sDiary.concat('<br /> <b>Thông tin cũ: </b>',
                            '<br /> - Mã hóa đơn: ', self.tgtOld.MaHoaDon,
                            '<br /> - Khách hàng: ', self.tgtOld.TenKhachHang, '(', self.tgtOld.MaKhachHang, ')',
                            '<br /> - Mức nạp: ', self.tgtOld.TongChiPhi,
                            '<br /> - Khuyến mãi: ', self.tgtOld.KhuyenMaiVND, ' (', self.tgtOld.PTKhuyenMai, ' %)',
                            '<br /> - Chiết khấu: ', self.tgtOld.TongGiamGia, ' (', self.tgtOld.PTChietKhau, ' %)',
                            '<br /> - Phải thanh toán: ', formatNumber(self.tgtOld.PhaiThanhToan),
                            '<br /> - Ngày nạp: ', moment(self.tgtOld.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss'));
                    }
                    Insert_NhatKyThaoTac_1Param(diary);

                    vmThanhToanGara.inforHoaDon = self.newHoaDon;
                    vmThanhToanGara.inforHoaDon.TenDoiTuong = self.cusChosing.TenDoiTuong;
                    vmThanhToanGara.inforHoaDon.MaDoiTuong = self.cusChosing.MaDoiTuong;
                    vmThanhToanGara.SavePhieuThu();

                    if (print) {
                        self.InPhieuThu();
                    }
                }
            }).always(function () {
                self.isLoading = false;
                $('#vmThemMoiTheNap').modal('hide');
            })
        },

        InPhieuThu: function () {
            var self = this;
            var hd = $.extend({}, self.newHoaDon);
            hd.NgayLapHoaDon = moment(hd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss');;
            hd.MucNap = hd.TongChiPhi;
            hd.KhuyenMai = hd.TongChietKhau;
            hd.NoSau = formatNumber(Math.abs(hd.TienThua));
            hd.PhaiThanhToan = formatNumber(hd.PhaiThanhToan);
            hd.DaThanhToan = hd.DaThanhToan;
            hd.TienBangChu = DocSo(formatNumberToFloat(hd.DaThanhToan));
            hd.NhanVienBanHang = VHeader.TenNhanVien;

            hd.TienMat = vmThanhToanGara.PhieuThuKhach.TienMat;
            hd.TienATM = vmThanhToanGara.PhieuThuKhach.TienPOS;
            hd.ChuyenKhoan = vmThanhToanGara.PhieuThuKhach.TienCK;

            hd.MaKhachHang = self.cusChosing.MaDoiTuong;
            hd.TenKhachHang = self.cusChosing.TenDoiTuong;
            hd.DiaChiKhachHang = self.cusChosing.DiaChi;
            hd.DienThoaiKhachHang = self.cusChosing.DienThoai;

            hd.TenCuaHang = self.inforCongTy.TenCongTy;
            hd.DiaChiCuaHang = self.inforCongTy.DiaChiCuaHang;
            hd.LogoCuaHang = self.inforCongTy.LogoCuaHang;
            hd.TenChiNhanh = VHeader.TenDonVi;
            hd.DienThoaiChiNhanh = '';
            hd.DiaChiChiNhanh = '';

            hd.TongTaiKhoanThe = formatNumber(self.inforTheGiaTri.SoDuTheSauNap);
            hd.TongSuDungThe = formatNumber(self.inforTheGiaTri.TongSuDungThe);
            hd.SoDuConLai = formatNumber(self.inforTheGiaTri.SoDuTheSauNap - self.inforTheGiaTri.TongSuDungThe);

            let pthuc = '';
            if (formatNumberToFloat(vmThanhToanGara.PhieuThuKhach.TienMat) > 0) {
                pthuc = 'Tiền mặt, ';
            }
            if (formatNumberToFloat(vmThanhToanGara.PhieuThuKhach.TienPOS) > 0) {
                pthuc += 'POS, ';
            }
            if (formatNumberToFloat(vmThanhToanGara.PhieuThuKhach.TienCK) > 0) {
                pthuc += 'Chuyển khoản, ';
            }
            hd.PhuongThucTT = Remove_LastComma(pthuc);

            ajaxHelper('/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=TGT&idDonVi='
                + VHeader.IdDonVi, 'GET').done(function (result) {
                    let data = result;
                    data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                    data = data.concat("<script > var item1=[]"
                        + "; var item2= [], item4= [], item5 =[]"
                        + "; var item3=" + JSON.stringify(hd) + "; </script>");
                    data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                    PrintExtraReport(data);
                })
        },

        showModalThanhToan: function (item) {
            var self = this;
            vmThanhToanGara.GridNVienBanGoi_Chosed = [];
            var obj = {
                MaHoaDon: '',
                LoaiDoiTuong: 1,// 1.kh, 2.ncc, 3.bh
                LoaiHoaDon: 22,
                SoDuDatCoc: 0,
                SoDuTheGiaTri: 0,
                HoanTraTamUng: self.newHoaDon.HoanTraTamUng,
                PhaiThanhToan: self.newHoaDon.PhaiThanhToan,
                PhaiThanhToanBaoHiem: 0,
                TongThanhToan: self.newHoaDon.PhaiThanhToan,
                TongTienThue: 0,
                TongTichDiem: 0,
                ThucThu: self.newHoaDon.ThucThu,
                ConNo: self.newHoaDon.TienThua,
                DienGiai: self.newHoaDon.DienGiai,
                ID_DoiTuong: self.newHoaDon.ID_DoiTuong,
                MaDoiTuong: self.cusChosing.MaDoiTuong,
                TenDoiTuong: self.cusChosing.TenDoiTuong,
                TenBaoHiem: '',
                NgayLapHoaDon: self.newHoaDon.NgayLapHoaDon,
                ID_NhanVien: self.newHoaDon.ID_NhanVien,
                ID_DonVi: self.newHoaDon.ID_DonVi,
                NguoiTao: self.newHoaDon.NguoiTao,
                ID_PhieuTiepNhan: null,
                DaThanhToan: formatNumber(self.newHoaDon.DaThanhToan),
                KhachDaTra: 0, // used when update hd
            }
            vmThanhToanGara.showModalThanhToan(obj, 3);
        },

        GetInforPhieuThu_whenHideModal: function () {
            var self = this;
            var ptKhach = vmThanhToanGara.PhieuThuKhach;
            var tienmat = formatNumberToFloat(ptKhach.TienMat);
            var pos = formatNumberToFloat(ptKhach.TienPOS);
            var ck = formatNumberToFloat(ptKhach.TienCK);
            var sLoai = '';
            if (tienmat > 0) {
                sLoai = 'Tiền mặt,';
            }
            if (pos > 0) {
                sLoai += 'POS,';
            }
            if (ck > 0) {
                sLoai += 'Chuyển khoản,';
            }
            sLoai = '(' + Remove_LastComma(sLoai) + ')';
            self.newHoaDon.TenLoaiTien = sLoai;
            self.newHoaDon.DaThanhToan = formatNumber(ptKhach.DaThanhToan);
            self.newHoaDon.TienThua = ptKhach.DaThanhToan - self.newHoaDon.PhaiThanhToan;
            self.newHoaDon.BH_NhanVienThucHiens = vmThanhToanGara.GridNVienBanGoi_Chosed;

            /// save to cache if was agree and chose again  (todo)
        }
        ,
        showModalCustomer: function () {
            var self = this;
            vmThemMoiKhach.showModalAdd();
        },
        hideModalAddCus_afterSave: function () {
            var self = this;
            self.cusChosing = {
                MaDoiTuong: vmThemMoiKhach.newCustomer.MaDoiTuong,
                TenDoiTuong: vmThemMoiKhach.newCustomer.TenDoiTuong,
                DienThoai: vmThemMoiKhach.newCustomer.DienThoai,
                DiaChi: vmThemMoiKhach.newCustomer.DiaChi,
            };
            self.newHoaDon.ID_DoiTuong = vmThemMoiKhach.newCustomer.ID;
            self.inforTheGiaTri.SoDuTheGiaTri = 0;
            self.inforTheGiaTri.TongTaiKhoanThe = 0;
            self.inforTheGiaTri.SoDuTheSauNap = 0;
            self.LichSuNapTien = [];
        },
    },
    computed: {
        PageList_Display: function () {

        }
    }
})

$(function () {
    $('#ThongTinThanhToanKHNCC').on('hidden.bs.modal', function () {
        if (vmThanhToanGara.saveOK) {
            vmThemMoiTheNap.GetInforPhieuThu_whenHideModal();
        }
    });

    $('#ThemMoiKhachHang').on('hidden.bs.modal', function () {
        if (vmThemMoiKhach.saveOK) {
            vmThemMoiTheNap.hideModalAddCus_afterSave();
        }
    })

    $('#datetimeTimKiemLichSu').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
    });
})

