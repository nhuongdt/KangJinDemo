var vmThongTinThanhToanBaoHiem = new Vue({
    el: '#ThongTinThanhToanBaoHiem',
    components: {

    },
    created: function () {
        this.GuidEmpty = '00000000-0000-0000-0000-000000000000';
        console.log('ttBoaHiem')
    },
    data: {
        saveOK: false,
        isNew: true,
        inforLogin: {
            ID_DonVi: $('#txtDonVi').val(),
            ID_NhanVien: $('#txtIDNhanVien').val(),
            UserLogin: $('#txtUserLogin').val(),
        },

        invoiceChosing: {},
        invoiceOld: {},
        TongTienBHDuyetOld: 0,

        IDRandomHD: '',
        TongTienBHDuyet: 0,
        PTThueBaoHiem: 0,
        TongTienThueBaoHiem: 0,
        SoVuBaoHiem: 0,
        KhauTruTheoVu: 0,
        PTGiamTruBoiThuong: 0,
        GiamTruBoiThuong: 0,
        BHThanhToanTruocThue: 0,
        TongGiaTriSauThue: 0,
        PhaiThanhToanBaoHiem: 0,
        GiamTruThanhToanBaoHiem: 0,
        CongThucChosed: 13, // default

        RdoKhauTru: 1, // 1.truoc VAT, 2.sau VAT
        RdoCheTai: 3,

        isPtramGiamTru: true,
        isPtramThue: true,

        KhauTrus: [
            { ID: 1, Text: 'Chưa bao gồm thuế' },
            { ID: 2, Text: 'Đã bao gồm thuế' },
        ],

        CheTais: [
            { ID: 3, Text: 'Không trừ khấu trừ, trước thuế' },
            { ID: 4, Text: 'Không trừ khấu trừ, sau thuế' },
            { ID: 5, Text: 'Trừ khấu trừ, trước thuế' },
            { ID: 6, Text: 'Trừ khấu trừ, sau thuế' },
        ],
        CheTaiFilter: [],

        // 13. KhauTru truoc VAT, CheTai truoc KhauTru, CheTai truoc VAT
        // 14. KhauTru truoc VAT, CheTai truoc KhauTru, CheTai sau VAT 
        // 15. KhauTru truoc VAT, CheTai sau KhauTru, CheTai truoc VAT
        // 16. KhauTru truoc VAT, CheTai sau KhauTru, CheTai sau VAT

        // 23. KhauTru sau VAT, CheTai truoc KhauTru, CheTai truoc VAT
        // 24. KhauTru sau VAT, CheTai truoc KhauTru, CheTai sau VAT
        // 25. KhauTru sau VAT, CheTai sau KhauTru, CheTai truoc VAT
        // 26. KhauTru sau VAT, CheTai sau KhauTru, CheTai sau VAT
    },
    methods: {
        showModalInsert: function (hd) {// at banhang
            var self = this;
            self.saveOK = false;
            self.isNew = true;
            self.SetInforBaoHiem_fromHoaDon(hd);
            $('#ThongTinThanhToanBaoHiem').modal('show');
        },

        SetInforBaoHiem_fromHoaDon: function (hd) {
            var self = this;
            var congthuc = hd.CongThucBaoHiem();
            if (commonStatisJs.CheckNull(congthuc) || parseInt(congthuc) === 0) {
                congthuc = '13';
            }
            congthuc = congthuc.toString();
            self.RdoKhauTru = congthuc[0];
            self.RdoCheTai = congthuc[1];
            self.CongThucChosed = congthuc;
            self.IDRandomHD = hd.IDRandom();
            self.TongTienBHDuyetOld = formatNumber3Digit(hd.TongTienBHDuyet());
            self.TongTienBHDuyet = formatNumber3Digit(hd.TongTienBHDuyet());
            self.PTThueBaoHiem = hd.PTThueBaoHiem();
            self.TongTienThueBaoHiem = formatNumber3Digit(hd.TongTienThueBaoHiem());
            self.SoVuBaoHiem = hd.SoVuBaoHiem();
            self.KhauTruTheoVu = formatNumber3Digit(hd.KhauTruTheoVu());
            self.PTGiamTruBoiThuong = hd.PTGiamTruBoiThuong();
            self.GiamTruBoiThuong = formatNumber3Digit(hd.GiamTruBoiThuong());
            self.BHThanhToanTruocThue = hd.BHThanhToanTruocThue();
            self.GiamTruThanhToanBaoHiem = hd.GiamTruThanhToanBaoHiem();
            self.PhaiThanhToanBaoHiem = hd.PhaiThanhToanBaoHiem();
            self.TongGiaTriSauThue = hd.BHThanhToanTruocThue() + hd.TongTienThueBaoHiem();

            self.CheckPtram();
        },

        showModaUpdate: function (hd) {
            var self = this;
            self.saveOK = false;
            self.isNew = false;
            self.invoiceChosing = hd;
            self.invoiceOld = $.extend({}, true, hd);

            console.log('update ', hd)
            var congthuc = hd.CongThucBaoHiem;
            if (commonStatisJs.CheckNull(congthuc) || parseInt(congthuc) === 0) {
                congthuc = '13';
            }
            congthuc = congthuc.toString();
            self.RdoKhauTru = congthuc[0];
            self.RdoCheTai = congthuc[1];
            self.CongThucChosed = congthuc;

            self.TongTienBHDuyetOld = formatNumber3Digit(hd.TongTienBHDuyet);
            self.TongTienBHDuyet = formatNumber3Digit(hd.TongTienBHDuyet);
            self.PTThueBaoHiem = hd.PTThueBaoHiem;
            self.TongTienThueBaoHiem = formatNumber3Digit(hd.TongTienThueBaoHiem);
            self.SoVuBaoHiem = hd.SoVuBaoHiem;
            self.KhauTruTheoVu = formatNumber3Digit(hd.KhauTruTheoVu);
            self.PTGiamTruBoiThuong = hd.PTGiamTruBoiThuong;
            self.GiamTruBoiThuong = formatNumber3Digit(hd.GiamTruBoiThuong);
            self.GiamTruThanhToanBaoHiem = formatNumber3Digit(hd.GiamTruThanhToanBaoHiem);
            self.BHThanhToanTruocThue = hd.BHThanhToanTruocThue;
            self.PhaiThanhToanBaoHiem = hd.PhaiThanhToanBaoHiem;
            self.TongGiaTriSauThue = hd.BHThanhToanTruocThue + hd.TongTienThueBaoHiem;

            self.CheckPtram();
            $('#ThongTinThanhToanBaoHiem').modal('show');
        },

        CheckPtram: function () {
            var self = this;
            if (formatNumberToFloat(self.PTThueBaoHiem) > 0 || formatNumberToFloat(self.TongTienThueBaoHiem) === 0) {
                self.isPtramThue = true;
            }
            else {
                self.isPtramThue = false;
            }
            if (formatNumberToFloat(self.PTGiamTruBoiThuong) > 0 || formatNumberToFloat(self.GiamTruBoiThuong) === 0) {
                self.isPtramGiamTru = true;
            }
            else {
                self.isPtramGiamTru = false;
            }
        },

        ResetCongThuc: function () {
            var self = this;
            self.RdoKhauTru = 1;
            self.RdoCheTai = 3;

            self.PTThueBaoHiem = 0;
            self.TongTienThueBaoHiem = 0;
            self.SoVuBaoHiem = 0;
            self.KhauTruTheoVu = 0;
            self.PTGiamTruBoiThuong = 0;
            self.GiamTruBoiThuong = 0;
            self.GiamTruThanhToanBaoHiem = 0;
            self.BHThanhToanTruocThue = formatNumberToFloat(self.TongTienBHDuyet);
            self.TongGiaTriSauThue = 0;
            self.PhaiThanhToanBaoHiem = self.BHThanhToanTruocThue;
            self.CongThucChosed = 13;
        },

        LoadCheTai_byKhauTru: function () {
            var self = this;
            switch (parseInt(self.RdoKhauTru)) {
                case 1:
                    self.CheTaiFilter = self.CheTais.filter(x => x.ID !== 4);
                    break;
                case 2:
                    self.CheTaiFilter = self.CheTais.filter(x => x.ID !== 5);
                    break;
            }
        },

        ChonKhauTru: function (item) {
            var self = this;
            self.RdoKhauTru = item.ID;
            self.Caculator();
        },
        ChonCheTai: function (item) {
            var self = this;
            self.RdoCheTai = item.ID;
            self.Caculator();
        },

        Caculator: function () {
            var self = this;
            var tongBHduyet = formatNumberToFloat(self.TongTienBHDuyet);
            var chetai = formatNumberToFloat(self.GiamTruBoiThuong);
            var khautru = formatNumberToFloat(self.KhauTruTheoVu);
            var thueBH = formatNumberToFloat(self.TongTienThueBaoHiem);

            switch (parseInt(self.RdoKhauTru)) {
                case 1:
                    switch (parseInt(self.RdoCheTai)) {
                        case 3:// KhauTru truoc VAT, CheTai truoc KhauTru, CheTai truoc VAT
                            if (self.isPtramGiamTru) {
                                chetai = tongBHduyet * self.PTGiamTruBoiThuong / 100;
                                self.GiamTruBoiThuong = formatNumber3Digit(chetai);
                            }
                            self.BHThanhToanTruocThue = tongBHduyet - chetai - khautru;
                            if (self.PTThueBaoHiem > 0) {
                                thueBH = self.PTThueBaoHiem * self.BHThanhToanTruocThue / 100;
                                self.TongTienThueBaoHiem = formatNumber3Digit(thueBH);
                            }
                            self.PhaiThanhToanBaoHiem = self.BHThanhToanTruocThue + thueBH;
                            self.TongGiaTriSauThue = self.PhaiThanhToanBaoHiem;
                            break;
                        case 4: // 14. KhauTru truoc VAT, CheTai truoc KhauTru, CheTai sau VAT 
                            break;
                        case 5: // KhauTru truoc VAT, CheTai truoc VAT, CheTai sau KhauTru (da tru khautru),
                            var gtritinhKhauTru = tongBHduyet - khautru;
                            if (self.PTGiamTruBoiThuong > 0) {
                                chetai = self.PTGiamTruBoiThuong * gtritinhKhauTru / 100;
                                self.GiamTruBoiThuong = formatNumber3Digit(chetai);
                            }
                            self.BHThanhToanTruocThue = gtritinhKhauTru - chetai;
                            if (self.PTThueBaoHiem > 0) {
                                thueBH = self.PTThueBaoHiem * self.BHThanhToanTruocThue / 100;
                                self.TongTienThueBaoHiem = formatNumber3Digit(thueBH);
                            }
                            self.PhaiThanhToanBaoHiem = self.BHThanhToanTruocThue + thueBH;
                            self.TongGiaTriSauThue = self.PhaiThanhToanBaoHiem;
                            break;
                        case 6: // KhauTru truoc VAT, CheTai sau VAT, CheTai sau KhauTru,
                            self.BHThanhToanTruocThue = tongBHduyet - khautru;
                            if (self.PTThueBaoHiem > 0) {
                                thueBH = self.PTThueBaoHiem * self.BHThanhToanTruocThue / 100;
                                self.TongTienThueBaoHiem = formatNumber3Digit(thueBH);
                            }
                            var gtritinhKhauTru = self.BHThanhToanTruocThue + thueBH;
                            self.TongGiaTriSauThue = gtritinhKhauTru;
                            if (self.PTGiamTruBoiThuong > 0) {
                                chetai = self.PTGiamTruBoiThuong * gtritinhKhauTru / 100;
                                self.GiamTruBoiThuong = formatNumber3Digit(chetai);
                            }
                            self.PhaiThanhToanBaoHiem = gtritinhKhauTru - chetai;
                            break;
                    }
                    break;
                case 2:
                    switch (parseInt(self.RdoCheTai)) {
                        case 3: // KhauTru sau VAT,  CheTai truoc VAT, CheTai truoc KhauTru (chua tru khautru)
                            if (self.PTGiamTruBoiThuong > 0) {
                                chetai = tongBHduyet * self.PTGiamTruBoiThuong / 100;
                                self.GiamTruBoiThuong = formatNumber3Digit(chetai);
                            }
                            self.BHThanhToanTruocThue = tongBHduyet - chetai;
                            if (self.PTThueBaoHiem > 0) {
                                thueBH = self.PTThueBaoHiem * self.BHThanhToanTruocThue / 100;
                                self.TongTienThueBaoHiem = formatNumber3Digit(thueBH);
                            }
                            self.TongGiaTriSauThue = self.BHThanhToanTruocThue + thueBH;
                            self.PhaiThanhToanBaoHiem = self.TongGiaTriSauThue - khautru;
                            break;
                        case 4:// KhauTru sau VAT, CheTai sau VAT, CheTai truoc KhauTru,
                            self.BHThanhToanTruocThue = tongBHduyet;
                            if (self.PTThueBaoHiem > 0) {
                                thueBH = self.PTThueBaoHiem * tongBHduyet / 100;
                                self.TongTienThueBaoHiem = formatNumber3Digit(thueBH);
                            }
                            var tongsauVAT = tongBHduyet + thueBH;
                            self.TongGiaTriSauThue = tongsauVAT;
                            if (self.PTGiamTruBoiThuong > 0) {
                                chetai = tongsauVAT * self.PTGiamTruBoiThuong / 100;
                                self.GiamTruBoiThuong = formatNumber3Digit(chetai);
                            }
                            self.PhaiThanhToanBaoHiem = tongsauVAT - chetai - khautru;
                            break;
                        case 5:// khong xayra
                            break;
                        case 6:// KhauTru sau VAT, CheTai sau VAT, CheTai sau KhauTru
                            self.BHThanhToanTruocThue = tongBHduyet;
                            if (self.PTThueBaoHiem > 0) {
                                thueBH = self.PTThueBaoHiem * tongBHduyet / 100;
                                self.TongTienThueBaoHiem = formatNumber3Digit(thueBH);
                            }
                            var tongsauVAT = tongBHduyet + thueBH;
                            self.TongGiaTriSauThue = tongsauVAT;
                            var gtritinhKhauTru = tongsauVAT - khautru;
                            if (self.PTGiamTruBoiThuong > 0) {
                                chetai = self.PTGiamTruBoiThuong * gtritinhKhauTru / 100;
                                self.GiamTruBoiThuong = formatNumber3Digit(chetai);
                            }
                            self.PhaiThanhToanBaoHiem = gtritinhKhauTru - chetai;
                            break;
                    }
                    break;
            }
        },

        editTongTienBHDuyet: function () {
            var self = this;
            var $this = $(event.currentTarget);
            self.TongTienBHDuyet = formatNumber3Digit($this.val());
            self.Caculator();
        },

        GiamTruBoiThuong_ClickVND: function () {
            var self = this;
            self.isPtramGiamTru = false;
            self.PTGiamTruBoiThuong = 0;
        },

        GiamTruBoiThuong_ClickPtram: function () {
            var self = this;
            self.isPtramGiamTru = true;
            var $this = $(event.currentTarget);
            let ptCK = RoundDecimal(formatNumberToFloat(self.GiamTruBoiThuong) / formatNumberToFloat(self.TongTienBHDuyet) * 100);
            self.PTGiamTruBoiThuong = ptCK;
            $(function () {
                $this.closest('div').find('input').select().focus();
            })
        },

        editGiamTruBoiThuong: function () {
            var self = this;
            var $this = $(event.currentTarget);
            if ($this.val() === '') {
                self.PTGiamTruBoiThuong = 0;
                self.GiamTruBoiThuong = 0;
            }
            else {
                if (self.isPtramGiamTru) {
                    if ($this.val() > 100) {
                        self.PTGiamTruBoiThuong = 100;
                    }
                    else {
                        if (formatNumberToFloat($this.val()) === 0) {
                            self.GiamTruBoiThuong = 0;
                        }
                    }
                }
                else {
                    self.GiamTruBoiThuong = formatNumber3Digit($this.val());
                }
            }
            self.Caculator();
        },

        editKhauTruTheoVu: function () {
            var self = this;
            var $this = $(event.currentTarget);
            formatNumberObj($this);
            self.KhauTruTheoVu = formatNumber3Digit($this.val());
            self.Caculator();
        },

        BaoHiem_editTax: function () {
            var self = this;
            var $this = $(event.currentTarget);
            var ptThue = self.PTThueBaoHiem;
            var tienThue = self.TongTienThueBaoHiem;
            var tiensauCK = self.BHThanhToanTruocThue;

            if ($this.val() === '') {
                ptThue = tienThue = 0;
            }
            else {
                if (self.isPtramThue) {
                    if (formatNumberToFloat($this.val()) > 100) {
                        ptThue = 100;
                        $this.val(100);
                    }
                    else {
                        ptThue = formatNumberToFloat($this.val());
                    }
                    tienThue = ptThue * tiensauCK / 100;
                }
                else {
                    formatNumberObj($this);
                    tienThue = formatNumberToFloat($this.val());
                }
            }
            self.TongTienThueBaoHiem = formatNumber3Digit(tienThue);
            self.PTThueBaoHiem = ptThue;
            self.Caculator();
        },

        BaoHiem_ClickTaxVND: function () {
            var self = this;
            var $this = $(event.currentTarget);
            self.PTThueBaoHiem = 0;
            self.isPtramThue = false;
            $(function () {
                $this.closest('div').find('input').select().focus();
            })
        },

        BaoHiem_ClickTaxPtram: function () {
            var self = this;
            var $this = $(event.currentTarget);
            self.isPtramThue = true;
            let ptThue = RoundDecimal(formatNumberToFloat(self.TongTienThueBaoHiem) / formatNumberToFloat(self.BHThanhToanTruocThue) * 100);
            self.PTThueBaoHiem = ptThue;
            $(function () {
                $this.closest('div').find('input').select().focus();
            })
        },

        Agree: function () {
            var self = this;
            self.saveOK = true;
            self.CongThucChosed = self.RdoKhauTru.toString() + self.RdoCheTai.toString();

            if (self.isNew) {
                $('#ThongTinThanhToanBaoHiem').modal('hide');
            }
            else {
                let title = '';
                if (self.invoiceChosing.KhachDaTra > 0 || self.invoiceChosing.BaoHiemDaTra > 0) {
                    title = 'Hóa đơn đã có phiếu thu. Bạn có chắc chắn muốn thay đổi không?'
                }
                else {
                    title = 'Bạn có chắc chắn muốn cập nhật thông tin bảo hiểm không?'
                }
                dialogConfirm('Xác nhận thay đổi', title, function () {
                    // caculator again infor khachhang
                    let ptThueHD = self.invoiceChosing.PTThueHoaDon;
                    let tongthueKH = self.invoiceChosing.TongThueKhachHang;
                    let tongthueHD = self.invoiceChosing.TongTienThue;
                    let phaiTT = self.invoiceChosing.PhaiThanhToan;
                    let tongTT = self.invoiceChosing.TongThanhToan;
                    let thueBH = formatNumberToFloat(self.TongTienThueBaoHiem);

                    let thueKH = 0;
                    if (tongthueKH === 0) {
                        // neu truocdo khach khong thue
                        thueKH = 0;
                    }
                    else {
                        thueKH = self.invoiceChosing.TongTienThue - thueBH;
                        if (thueKH <= 0) {
                            thueKH = 0;
                            ptThueHD = 0;
                        }
                    }
                    tongthueKH = thueKH;
                    tongthueHD = thueKH + thueBH;

                    let giamgiaHD = self.invoiceChosing.TongGiamGia;
                    phaiTT = self.invoiceChosing.TongTienHang
                        - formatNumberToFloat(self.TongTienBHDuyet)
                        + formatNumberToFloat(self.GiamTruBoiThuong)
                        + formatNumberToFloat(self.KhauTruTheoVu)
                        + tongthueKH;
                    if (self.invoiceChosing.TongChietKhau > 0) {
                        giamgiaHD = RoundDecimal(self.invoiceChosing.TongChietKhau * phaiTT / 100, 3);
                    }
                    phaiTT = phaiTT - giamgiaHD;
                    phaiTT = phaiTT < 0 ? 0 : phaiTT;
                    tongTT = phaiTT + self.PhaiThanhToanBaoHiem;

                    // danh sach hd: update to DB
                    let obj = {
                        ID: self.invoiceChosing.ID,
                        TongTienBHDuyet: self.TongTienBHDuyet,
                        PTThueBaoHiem: self.PTThueBaoHiem,
                        TongTienThueBaoHiem: self.TongTienThueBaoHiem,
                        SoVuBaoHiem: self.SoVuBaoHiem,
                        KhauTruTheoVu: self.KhauTruTheoVu,
                        PTGiamTruBoiThuong: self.PTGiamTruBoiThuong,
                        GiamTruBoiThuong: self.GiamTruBoiThuong,
                        BHThanhToanTruocThue: self.BHThanhToanTruocThue,
                        TongGiaTriSauThue: self.TongGiaTriSauThue,
                        PhaiThanhToanBaoHiem: self.PhaiThanhToanBaoHiem,
                        CongThucBaoHiem: self.CongThucChosed,
                        GiamTruThanhToanBaoHiem: self.GiamTruThanhToanBaoHiem,

                        TongThueKhachHang: tongthueKH,
                        PTThueHoaDon: ptThueHD,
                        TongTienThue: tongthueHD,
                        TongGiamGia: giamgiaHD,
                        PhaiThanhToan: phaiTT,
                        TongThanhToan: tongTT,
                        NguoiSua: self.inforLogin.UserLogin,
                    }
                    var myData = {
                        objHoaDon: obj,
                    }
                    console.log('myData ', myData);
                    ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'Update_ThongTinBaoHiem', 'POST', myData).done(function (x) {

                        if (x.res) {

                            let congthucOld = self.invoiceOld.CongThucBaoHiem;
                            if (commonStatisJs.CheckNull(congthucOld) || parseInt(congthucOld) === 0) {
                                congthucOld = '13';
                            }
                            congthucOld = congthucOld.toString();
                            let khautru = parseInt(congthucOld[0]);
                            let chetai = parseInt(congthucOld[1]);
                            let txtKhauTru = self.KhauTrus.find(x => x.ID === khautru).Text;
                            let txtCheTai = self.CheTais.find(x => x.ID === chetai).Text;

                            if (self.invoiceOld.TongTienBHDuyet !== formatNumberToFloat(self.TongTienBHDuyet)) {
                                // reset dongiaBH = 0
                                ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'ResetDonGiaBaoHiem?idHoaDon=' + self.invoiceChosing.ID, 'GET').done(function (x) {

                                })
                            }

                            if (tongthueKH === 0) {
                                // reset thueCT = 0
                                ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'ResetThueChiTiet?idHoaDon=' + self.invoiceChosing.ID, 'GET').done(function (x) {

                                })
                            }

                            var diary = {
                                ID_DonVi: self.inforLogin.ID_DonVi,
                                ID_NhanVien: self.inforLogin.ID_NhanVien,
                                LoaiNhatKy: 2,
                                ChucNang: 'Hóa đơn - Sửa đổi thông tin bảo hiểm',
                                NoiDung: 'Cập nhật thông tin thanh toán bảo hiểm cho hóa đơn '.concat(self.invoiceChosing.MaHoaDon,
                                    ', Người sửa: ', self.inforLogin.UserLogin),
                                NoiDungChiTiet: 'Cập nhật thông tin thanh toán bảo hiểm cho hóa đơn '.concat(self.invoiceChosing.MaHoaDon,
                                    ', Người sửa ', self.inforLogin.UserLogin,
                                    '<br /> <b> Thông tin mới </b>:',
                                    '<br /> - Tổng tiền duyệt ', self.TongTienBHDuyet,
                                    '<br /> - Công thức áp dụng (', self.CongThucChosed, '):Khấu trừ - ', self.txtKhauTru, ', Chế tài - ', self.txtCheTai,
                                    '<br /> - Khấu trừ (', self.SoVuBaoHiem, ' vụ) ', self.KhauTruTheoVu,
                                    '<br /> - Chế tài (', self.PTGiamTruBoiThuong, '%) ', self.GiamTruBoiThuong,
                                    '<br /> - Tổng thuế bảo hiểm (', self.PTThueBaoHiem, '%) ', self.TongTienThueBaoHiem,
                                    '<br /> <b> Thông tin cũ </b>:',
                                    '<br /> - Tổng tiền duyệt ', formatNumber(self.invoiceOld.TongTienBHDuyet),
                                    '<br /> - Công thức áp dụng (', self.invoiceOld.CongThucBaoHiem, '): Khấu trừ - ', txtKhauTru, ', Chế tài - ', txtCheTai,
                                    '<br /> - Khấu trừ (', self.invoiceOld.SoVuBaoHiem, ' vụ): ', formatNumber(self.invoiceOld.KhauTruTheoVu),
                                    '<br /> - Chế tài (', self.invoiceOld.PTGiamTruBoiThuong, '%): ', formatNumber(self.invoiceOld.GiamTruBoiThuong),
                                    '<br /> - Tổng thuế bảo hiểm (', self.invoiceOld.PTThueBaoHiem, '%): ', formatNumber(self.invoiceOld.TongTienThueBaoHiem),
                                ),
                            }
                            Insert_NhatKyThaoTac_1Param(diary);

                            ShowMessage_Success('Cập nhật thành công');
                        }
                    })

                    $('#ThongTinThanhToanBaoHiem').modal('hide');
                })
            }
        }
    },
    computed: {
        clGiamTru: function () {
            var self = this;
            return self.isPtramGiamTru ? 'op-js-toggle-2' : '';
        },
        clThue: function () {
            var self = this;
            return self.isPtramThue ? 'op-js-toggle-2' : '';
        },
        txtKhauTru: function () {
            var self = this;
            switch (parseInt(self.RdoKhauTru)) {
                case 1:
                    if (parseInt(self.RdoCheTai) === 4) {
                        self.RdoCheTai = 3;
                    }
                    break;
                case 2:
                    if (parseInt(self.RdoCheTai) === 5) {
                        self.RdoCheTai = 3;
                    }
                    break;
            }
            var txt = self.KhauTrus.find(x => x.ID === parseInt(self.RdoKhauTru)).Text;
            return txt;
        },
        txtCheTai: function () {
            var self = this;
            var txt = self.CheTais.find(x => x.ID === parseInt(self.RdoCheTai)).Text;
            return txt;
        },
    },
})


$(function () {
    $(window.document).on('shown.bs.modal', '.modal', function () {
        window.setTimeout(function () {
            $('[autofocus]', this).focus();
            $('[autofocus]').select();
        }.bind(this), 100);
    });
})