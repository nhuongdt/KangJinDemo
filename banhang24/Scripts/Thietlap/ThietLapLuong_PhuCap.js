
var vmNvLuong = new Vue({
    el: '#modalNvluongphucap',
    data: {
        Isnew: true,
        title: "",
        error: '',
        thietLapCoBan: {
            ID: '00000000-0000-0000-0000-000000000000',
            ID_NhanVien: null,
            ID_LoaiLuong: null,
            TenLoaiLuong: 'Lương cố định',
            NgayApDung: '',
            NgayKetThuc: '',
            SoTien: "",
            HeSo: 1,
            Bac: '',
            NoiDung: "",
            LoaiLuong: 1, // 1.loailuong, 5.phucap, 6.giamtru
            LoaiPhuCap_GiamTru: 1, // 52.phucap codinh vnd, 53.phucap codinh %luong,
            LaPhuCap_TheoNgay: false,
            LaGiamTru_TheoLan: false,
        },
        thietLapNangCao: {
            isShow: false,
            isShowOT: false,
        },
        thietLapOT: {
            isShow: false,
            ID_LuongPhuCap: '00000000-0000-0000-0000-000000000000',
            NgayThuong_LaPhanTramLuong: 1, LuongNgayThuong: 150,
            Thu7_LaPhanTramLuong: 1, Thu7_GiaTri: 150,
            CN_LaPhanTramLuong: 1, ThCN_GiaTri: 150,
            NgayNghi_LaPhanTramLuong: 1, NgayNghi_GiaTri: 200,
            NgayLe_LaPhanTramLuong: 1, NgayLe_GiaTri: 200,
            LaOT: 1,
        },
        role: {
            Insert: true,
            Update: true,
            Delete: true,
        },
        listData: {
            ListLoaiLuong: [],
            NS_ThietLapLuongChiTiet: [],
            NS_LuongPhuCap: [],
            ListCa: [],
            ListCa_ofNhanVien: [],
        },
    },
    created: function () {
        var self = this;
        self.UrlNhanVienAPI = '/api/DanhMuc/NS_NhanVienAPI/';
        self.UrlNhanSuAPI = '/api/DanhMuc/NS_NhanSuAPI/';
        self.ID_DonVi = $('#hd_IDdDonVi').val();
        self.Guid_Empty = '00000000-0000-0000-0000-000000000000';
    },
    methods: {
        GetListCaLamViec_ofDonVi: function () {
            var self = this;
            $.getJSON(self.UrlNhanVienAPI + 'GetListCaLamViec_ofDonVi?idDonVi=' + self.ID_DonVi, function (x) {
                if (x.res === true) {
                    self.listData.ListCa = x.dataSoure;
                }
             
            });
        },
        CreateObject_ThietLapNangCao: function (laOT) {
            var self = this;
            return {
                isShow: false,
                isShowOT: false,
                ID_CaLamViec: null,
                ID_LuongPhuCap: self.Guid_Empty,
                TenCa: 'Chọn ca',
                NgayThuong_LaPhanTramLuong: laOT, LuongNgayThuong: 100,
                Thu7_LaPhanTramLuong: 1, Thu7_GiaTri: 100,
                CN_LaPhanTramLuong: 1, ThCN_GiaTri: 100,
                NgayNghi_LaPhanTramLuong: 1, NgayNghi_GiaTri: 100,
                NgayLe_LaPhanTramLuong: 1, NgayLe_GiaTri: 100,
                LaOT: laOT,
                KieuTinhLuong: 'ca', // ca/giờ
            }
        },
        ChangeLoaiLuong: function () {
            console.log(11)
            var self = this;
            self.thietLapCoBan.LoaiLuong = parseInt(self.thietLapCoBan.LoaiLuong);
            switch (self.thietLapCoBan.LoaiLuong) {
                case 1:
                    self.thietLapCoBan.TenLoaiLuong = 'Lương cố định';
                    self.thietLapCoBan.LoaiPhuCap_GiamTru = 1;
                    self.thietLapNangCao.isShow = false;
                    break;
                case 5:// phucap
                    self.thietLapCoBan.TenLoaiLuong = 'Chọn loại phụ cấp';
                    self.thietLapCoBan.LoaiPhuCap_GiamTru = 52;
                    self.thietLapNangCao.isShow = false;
                    self.thietLapNangCao.isShowOT = false;
                    vmNvLuong.getLoaiLuong();
                    vmNvloailuong.IsPhuCap = true;
                    break;
                case 6: // giamtru
                    self.thietLapCoBan.TenLoaiLuong = 'Chọn loại giảm trừ';
                    self.thietLapCoBan.LoaiPhuCap_GiamTru = 62;
                    self.thietLapNangCao.isShow = false;
                    self.thietLapNangCao.isShowOT = false;
                    vmNvLuong.getLoaiLuong();
                    vmNvloailuong.IsPhuCap = false;
                    break;
            }
        },
        PhuCap_ClickVND: function () {
            var self = this;
            if (self.thietLapCoBan.LoaiLuong === 5) {
                self.thietLapCoBan.LoaiPhuCap_GiamTru = 52;
            }
            else {
                self.thietLapCoBan.LoaiPhuCap_GiamTru = 62;
            }
            self.SetCheck_PhuCapTheoNgay();
        },
        PhuCap_ClickPtram: function () {
            var self = this;
            if (self.thietLapCoBan.LoaiLuong === 5) {
                self.thietLapCoBan.LoaiPhuCap_GiamTru = 53;
            }
            else {
                self.thietLapCoBan.LoaiPhuCap_GiamTru = 63;
            }
            self.SetCheck_PhuCapTheoNgay();
        },
        ChoseLoaiPhuCap: function (item) {
            var self = this;
            self.thietLapCoBan.ID_LoaiLuong = item.ID;
            self.thietLapCoBan.TenLoaiLuong = item.TenLoaiLuong;
            self.thietLapCoBan.LaGiamTru_TheoLan = false;
            self.SetCheck_PhuCapTheoNgay();
        },

        ChoseKieuLuong: function (item) {
            var self = this;
            var valKey = parseInt(item.Key);
            self.thietLapCoBan.TenLoaiLuong = item.Value;
            self.thietLapCoBan.LoaiPhuCap_GiamTru = item.Key;
            self.thietLapCoBan.LoaiLuong = 1;
            self.thietLapCoBan.ID_LoaiLuong = null;

            switch (valKey) {
                case 1:// luongcodinh
                    self.thietLapNangCao.isShow = false;
                    self.thietLapNangCao.isShowOT = false;
                    break;
                case 2:// theongay
                    self.thietLapNangCao.isShow = false;
                    self.thietLapNangCao.isShowOT = true;
                    break;
                case 3:// theoca
                    self.thietLapNangCao.isShow = (self.showSoTien === false);
                    self.thietLapNangCao.isShowOT = true;
                    break;
                case 4:// theogio
                    self.thietLapNangCao.isShow = (self.showSoTien === false);
                    self.thietLapNangCao.isShowOT = false;
                    break;
            }
            vmNvLuong.SetCheck_PhuCapTheoNgay();
        },
        HideShow_ThietLapNangCao: function () {
            var self = this;
            // used with luong theoca/gio
            if (self.thietLapCoBan.LoaiLuong === 1 && parseInt(self.thietLapCoBan.LoaiPhuCap_GiamTru) > 2) {
                self.thietLapNangCao.isShow = !self.thietLapNangCao.isShow;
                if (self.thietLapCoBan.SoTien !== '') {
                    self.listData.NS_ThietLapLuongChiTiet[0].LuongNgayThuong = self.thietLapCoBan.SoTien;
                }
            }
        },
        Add_ThietLapChitiet: function () {
            var self = this;
            var obj = self.CreateObject_ThietLapNangCao(0);
            self.listData.NS_ThietLapLuongChiTiet.push(obj);
        },
        formatNumberSoTien: function (e) {
            var $this = $(e.currentTarget);
            var val = $this.val();
            if (!commonStatisJs.CheckNull(val) && val.indexOf('.') === -1) {
                formatNumberObj($this);
                this.thietLapCoBan.SoTien = formatNumber3Digit(this.thietLapCoBan.SoTien)
            }
        },

        ShowPop_AddPhuCap: function () {
            var self = this;
            if (parseInt(self.thietLapCoBan.LoaiLuong) === 5) {
                vmNvloailuong.IsPhuCap = true;
            }
            else {
                vmNvloailuong.IsPhuCap = false;
            }
            vmNvloailuong.refresh();
            $('#modalNvloailuong').modal('show');
        },

        EditPhuCap_GiamTru: function () {
            var self = this;
            var itemUp = {
                ID: self.thietLapCoBan.ID_LoaiLuong,
                TenLoaiLuong: self.thietLapCoBan.TenLoaiLuong,
                LoaiLuong: self.thietLapCoBan.LoaiLuong,
            }
            var itemEx = $.grep(self.listData.ListLoaiLuong, function (x) {
                return x.ID === itemUp.ID;
            });
            if (itemEx.length > 0) {
                itemUp.GhiChu = itemEx[0].GhiChu;
            }
            vmNvloailuong.edit(itemUp);
        },

        messageError: function (input) {
            this.error = input;
        },

        CheckRole: function (insert, update, dele) {
            this.role.Insert = insert === '1';
            this.role.Update = update === '1';
            this.role.Delete = dele === '1';
        },
        Insert: function (id) {
            var self = this;
            $('#modalNvluongphucap').modal("show");
            self.thietLapCoBan.ID_NhanVien = id;
            self.listData.ListCa_ofNhanVien = $.grep(self.listData.ListCa, function (x) {
                return x.ID_NhanVien === id;
            });
            self.refresh();
        },
        edit: function (model) {
            var self = this;
            console.log(3);
            if (model !== null && !commonStatisJs.CheckNull(model.ID)) {
                self.thietLapCoBan.ID = model.ID;
                self.thietLapCoBan.ID_NhanVien = model.ID_NhanVien;
                self.thietLapCoBan.Bac = model.Bac;
                self.thietLapCoBan.HeSo = model.HeSo;
                self.thietLapCoBan.SoTien = self.convertMoney(model.SoTien);
                self.thietLapCoBan.NoiDung = model.NoiDung;
                self.thietLapCoBan.LoaiPhuCap_GiamTru = model.LoaiLuong;

                if (model.NS_ThietLapLuongChiTiet.length > 0) {
                    self.listData.NS_ThietLapLuongChiTiet = $.grep(model.NS_ThietLapLuongChiTiet, function (x) {
                        return x.LaOT === 0;
                    });
                    let ctOT = $.grep(model.NS_ThietLapLuongChiTiet, function (x) {
                        return x.LaOT === 1;
                    });
                    if (ctOT.length > 0) {
                        self.thietLapOT = {
                            isShow: true,
                            ID_LuongPhuCap: ctOT[0].ID_LuongPhuCap,
                            LuongNgayThuong: ctOT[0].LuongNgayThuong,
                            NgayThuong_LaPhanTramLuong: ctOT[0].NgayThuong_LaPhanTramLuong,
                            Thu7_LaPhanTramLuong: ctOT[0].Thu7_LaPhanTramLuong,
                            Thu7_GiaTri: ctOT[0].Thu7_GiaTri,
                            CN_LaPhanTramLuong: ctOT[0].CN_LaPhanTramLuong,
                            ThCN_GiaTri: ctOT[0].ThCN_GiaTri,
                            NgayNghi_LaPhanTramLuong: ctOT[0].NgayNghi_LaPhanTramLuong,
                            NgayNghi_GiaTri: ctOT[0].NgayNghi_GiaTri,
                            NgayLe_LaPhanTramLuong: ctOT[0].NgayLe_LaPhanTramLuong,
                            NgayLe_GiaTri: ctOT[0].NgayLe_GiaTri,
                            LaOT: 1,
                        };
                    }
                    else {
                        self.thietLapOT.isShow = false;
                    }
                    self.listData.ListCa_ofNhanVien = $.grep(self.listData.ListCa, function (x) {
                        return x.ID_NhanVien === model.ID_NhanVien;
                    });
                }
                else {
                    self.thietLapOT.isShow = false;
                }

                if (model.LoaiLuong > 4) {
                    // neu phucap/giamtru
                    var loailuong_loaipc = model.LoaiLuong.toString().split('');// 51: loailuong =5
                    self.thietLapCoBan.LoaiLuong = parseFloat(loailuong_loaipc[0]);
                    vmNvLuong.getLoaiLuong();
                    vmNvLuong.ChoseLoaiPhuCap({ ID: model.ID_LoaiLuong, TenLoaiLuong: model.TenLoaiLuong, LoaiLuong: self.thietLapCoBan.LoaiLuong, LoaiPhuCap_GiamTru: model.LoaiLuong });
                    self.thietLapNangCao.isShow = false;
                    self.thietLapNangCao.isShowOT = false;

                    if (model.LoaiLuong.toString().indexOf('5') > -1) {
                        self.title = "Cập nhật khoản phụ cấp";
                    }
                    else {
                        self.title = "Cập nhật khoản giảm trừ";
                    }
                }
                else {
                    self.thietLapCoBan.LoaiLuong = 1;
                    vmNvLuong.ChoseKieuLuong({ Key: model.LoaiLuong, Value: model.TenLoaiLuong });
                    self.thietLapNangCao.isShow = (self.showSoTien === false);
                    self.title = "Cập nhật lương";
                }

                self.Isnew = false;
                self.error = "";

                $('#dateTNLuong').val(self.convertDate(model.NgayApDung));
                $('#dateDNLuong').val(self.convertDate(model.NgayKetThuc));
                $('#modalNvluongphucap').modal("show");
            }
            else {
                bottomrightnotify("Không lấy được thông tin cần cập nhật", "danger");
            }
        },
        refresh: function () {
            console.log(1)
            var self = this;
            var obj = self.CreateObject_ThietLapNangCao(0);
            obj.TenCa = 'Mặc định';
            self.listData.NS_ThietLapLuongChiTiet = [obj];

            self.Isnew = true;
            self.error = '';
            self.title = "Thêm mới khoản lương, phụ cấp";
            self.thietLapCoBan.ID = self.Guid_Empty;
            self.thietLapCoBan.Bac = "";
            self.thietLapCoBan.HeSo = 1;
            self.thietLapCoBan.SoTien = '';
            self.thietLapCoBan.NoiDung = '';
            self.thietLapCoBan.ID_LoaiLuong = null;
            self.thietLapCoBan.LoaiPhuCap_GiamTru = 1;
            self.thietLapCoBan.LoaiLuong = 1;
            self.thietLapCoBan.TenLoaiLuong = 'Lương cố định';

            self.thietLapNangCao.isShowOT = false;
            self.thietLapNangCao.isShow = false;

            self.thietLapOT = self.CreateObject_ThietLapNangCao(1);
            self.thietLapOT.LuongNgayThuong = 150;


            $('#dateTNLuong').val(null);
            $('#dateDNLuong').val(null);
        },
        convertDate: function (input) {
            if (input !== null && input !== undefined && input !== '') {
                return moment(input).format('DD/MM/YYYY');
            }
            return "";
        },
        getLoaiLuong: function () {
            let loaipc = parseInt(this.thietLapCoBan.LoaiLuong);
            $.getJSON("/api/DanhMuc/NS_NhanVienAPI/GetAllLoaiLuong", function (data) {
                let lstType = [];
                if (data.length > 0) {
                    lstType = $.grep(data, function (x) {
                        return x.LoaiLuong === loaipc;
                    });
                }
                var obj = {
                    ID: null,
                    TenLoaiLuong: 'Chọn loại '.concat(loaipc == 5 ? 'phụ cấp' : 'giảm trừ'),
                    LoaiLuong: loaipc,
                }
                lstType.unshift(obj);
                vmNvLuong.listData.ListLoaiLuong = lstType;
            });
        },
        convertMoney: function (input) {
            if (commonStatisJs.CheckNull(input))
                return "";
            return input.toString().replace(/,/g, '').replace(/(\d)(?=(\d{3})+(?:\.\d+)?$)/g, "$1,")
        },
        convertToNumber: function (input) {
            if (commonStatisJs.CheckNull(input))
                return "";
            return parseFloat(input.toString().replace(/,/g, ''));
        },

        thietLapOT_OnOff: function () {
            var self = this;
            self.thietLapOT.isShow = !self.thietLapOT.isShow;
        },
        thietLapOT_EditGiaTri: function (loaingay) {
            var self = this;
            var gtriNew = $(event.currentTarget).val();
            switch (loaingay) {
                case 0:
                    gtriNew = self.convertMoney(gtriNew);
                    self.thietLapOT.LuongNgayThuong = gtriNew;
                    break;
                case 1:
                    if (self.thietLapOT.Thu7_LaPhanTramLuong === 1) {
                        if (gtriNew > 999) {
                            gtriNew = 999;
                        }
                    } else {
                        gtriNew = self.convertMoney(gtriNew);
                    }
                    self.thietLapOT.Thu7_GiaTri = gtriNew;
                    break;
                case 2:
                    if (self.thietLapOT.CN_LaPhanTramLuong === 1) {
                        if (gtriNew > 999) {
                            gtriNew = 999;
                        }
                    } else {
                        gtriNew = self.convertMoney(gtriNew);
                    }
                    self.thietLapOT.ThCN_GiaTri = gtriNew;
                    break;
                case 3:
                    if (self.thietLapOT.NgayNghi_LaPhanTramLuong === 1) {
                        if (gtriNew > 999) {
                            gtriNew = 999;
                        }
                    }
                    else {
                        gtriNew = self.convertMoney(gtriNew);
                    }
                    self.thietLapOT.NgayNghi_GiaTri = gtriNew;
                    break;
                case 4:
                    if (self.thietLapOT.NgayLe_LaPhanTramLuong === 1) {
                        if (gtriNew > 999) {
                            gtriNew = 999;
                        }
                    } else {
                        gtriNew = self.convertMoney(gtriNew);
                    }
                    self.thietLapOT.NgayLe_GiaTri = gtriNew;
                    break;
            }

            var keycode = event.keycode || event.which;
            if (keycode === 13) {
                $('#tblOT tbody').children('tr').eq(0).find('td').eq(loaingay + 2).find('input').focus().select();
            }
        },
        thietLapOT_ClickVND: function (loaingay) {
            var self = this;
            var luongCB = self.convertToNumber(self.thietLapCoBan.SoTien);
            var loailuong = parseInt(self.thietLapCoBan.LoaiPhuCap_GiamTru);
            if (loailuong === 2) {
                luongCB = luongCB / 26 / 8;// 1gio
            }
            switch (loaingay) {
                case 0:
                    self.thietLapOT.LuongNgayThuong = commonStatisJs.FormatNumber3Digit(self.convertToNumber(self.thietLapOT.LuongNgayThuong) * luongCB / 100);
                    self.thietLapOT.NgayThuong_LaPhanTramLuong = 0;
                    break;
                case 1:
                    self.thietLapOT.Thu7_GiaTri = commonStatisJs.FormatNumber3Digit(self.convertToNumber(self.thietLapOT.Thu7_GiaTri) * luongCB / 100);
                    self.thietLapOT.Thu7_LaPhanTramLuong = 0;
                    break;
                case 2:
                    self.thietLapOT.ThCN_GiaTri = commonStatisJs.FormatNumber3Digit(self.convertToNumber(self.thietLapOT.ThCN_GiaTri) * luongCB / 100);
                    self.thietLapOT.CN_LaPhanTramLuong = 0;
                    break;
                case 3:
                    self.thietLapOT.NgayNghi_GiaTri = commonStatisJs.FormatNumber3Digit(self.convertToNumber(self.thietLapOT.NgayNghi_GiaTri) * luongCB / 100);
                    self.thietLapOT.NgayNghi_LaPhanTramLuong = 0;
                    break;
                case 4:
                    self.thietLapOT.NgayLe_GiaTri = commonStatisJs.FormatNumber3Digit(self.convertToNumber(self.thietLapOT.NgayLe_GiaTri) * luongCB / 100);
                    self.thietLapOT.NgayLe_LaPhanTramLuong = 0;
                    break;
            }
        },
        thietLapOT_ClickPtram: function (loaingay) {
            var self = this;
            var luongCB = self.convertToNumber(self.thietLapCoBan.SoTien);
            var loailuong = parseInt(self.thietLapCoBan.LoaiPhuCap_GiamTru);
            if (loailuong === 2) {
                luongCB = luongCB / 26 / 8;// ot theongay
            }
            switch (loaingay) {
                case 0:
                    self.thietLapOT.LuongNgayThuong = commonStatisJs.roundDecimal(self.convertToNumber(self.thietLapOT.LuongNgayThuong) * 100 / luongCB);
                    self.thietLapOT.NgayThuong_LaPhanTramLuong = 1;
                    break;
                case 1:
                    self.thietLapOT.Thu7_GiaTri = commonStatisJs.roundDecimal(self.convertToNumber(self.thietLapOT.Thu7_GiaTri) * 100 / luongCB);
                    self.thietLapOT.Thu7_LaPhanTramLuong = 1;
                    break;
                case 2:
                    self.thietLapOT.ThCN_GiaTri = commonStatisJs.roundDecimal(self.convertToNumber(self.thietLapOT.ThCN_GiaTri) * 100 / luongCB);
                    self.thietLapOT.CN_LaPhanTramLuong = 1;
                    break;
                case 3:
                    self.thietLapOT.NgayNghi_GiaTri = commonStatisJs.roundDecimal(self.convertToNumber(self.thietLapOT.NgayNghi_GiaTri) * 100 / luongCB);
                    self.thietLapOT.NgayNghi_LaPhanTramLuong = 1;
                    break;
                case 4:
                    self.thietLapOT.NgayLe_GiaTri = commonStatisJs.roundDecimal(self.convertToNumber(self.thietLapOT.NgayLe_GiaTri) * 100 / luongCB);
                    self.thietLapOT.NgayLe_LaPhanTramLuong = 1;
                    break;
            }
        },

        ctluong_DeleteRow: function (index) {
            var self = this;
            for (let i = 0; i < self.listData.NS_ThietLapLuongChiTiet.length; i++) {
                if (i === index) {
                    self.listData.NS_ThietLapLuongChiTiet.splice(i, 1);
                    break;
                }
            }
        },

        ctluong_ChoseCaLamViec: function (index, ca) {
            var self = this;
            for (let i = 0; i < self.listData.NS_ThietLapLuongChiTiet.length; i++) {
                if (i === index) {
                    self.listData.NS_ThietLapLuongChiTiet[i].ID_CaLamViec = ca.ID_CaLamViec;
                    self.listData.NS_ThietLapLuongChiTiet[i].TenCa = ca.TenCa;
                    break;
                }
            }
        },
        ctluong_EditGiaTri: function (ctluong, index, loaingay) {
            var self = this;
            var gtriNew = $(event.currentTarget).val();
            switch (loaingay) {
                case 0:
                    gtriNew = self.convertMoney(gtriNew);
                    if (index === 0) {
                        self.thietLapCoBan.SoTien = gtriNew;
                    }
                    break;
                case 1:
                    if (ctluong.Thu7_LaPhanTramLuong === 1) {
                        if (gtriNew > 999) {
                            gtriNew = 999;
                        }
                    } else {
                        gtriNew = self.convertMoney(gtriNew);
                    }
                    break;
                case 2:
                    if (ctluong.CN_LaPhanTramLuong === 1) {
                        if (gtriNew > 999) {
                            gtriNew = 999;
                        }
                    } else {
                        gtriNew = self.convertMoney(gtriNew);
                    }
                    break;
                case 3:
                    if (ctluong.NgayNghi_LaPhanTramLuong === 1) {
                        if (gtriNew > 999) {
                            gtriNew = 999;
                        }
                    }
                    else {
                        gtriNew = self.convertMoney(gtriNew);
                    }
                    break;
                case 4:
                    if (ctluong.NgayLe_LaPhanTramLuong === 1) {
                        if (gtriNew > 999) {
                            gtriNew = 999;
                        }
                    } else {
                        gtriNew = self.convertMoney(gtriNew);
                    }
                    break;
            }
            for (let i = 0; i < self.listData.NS_ThietLapLuongChiTiet.length; i++) {
                if (i === index) {
                    switch (loaingay) {
                        case 0:
                            self.listData.NS_ThietLapLuongChiTiet[i].LuongNgayThuong = gtriNew;
                            break;
                        case 1:
                            self.listData.NS_ThietLapLuongChiTiet[i].Thu7_GiaTri = gtriNew;
                            break;
                        case 2:
                            self.listData.NS_ThietLapLuongChiTiet[i].ThCN_GiaTri = gtriNew;
                            break;
                        case 3:
                            self.listData.NS_ThietLapLuongChiTiet[i].NgayNghi_GiaTri = gtriNew;
                            break;
                        case 4:
                            self.listData.NS_ThietLapLuongChiTiet[i].NgayLe_GiaTri = gtriNew;
                            break;
                    }
                    break;
                }
            }

            var keycode = event.keycode || event.which;
            if (keycode === 13) {
                if (loaingay === 4 && index === self.listData.NS_ThietLapLuongChiTiet.length - 1) {
                    $('#tblOT tbody').children('tr').eq(0).find('td').eq(1).find('input').focus().select();
                }
                else {
                    switch (loaingay) {
                        case 4:
                            loaingay = -1;
                            index = index + 1;
                            break;
                    }
                    $('#tblNangCao tbody').children('tr').eq(index).find('td').eq(loaingay + 2).find('input').focus().select();
                }
            }
        },
        ctluong_ClickVND: function (index, ctluong, loaingay) {
            var self = this;
            for (let i = 0; i < self.listData.NS_ThietLapLuongChiTiet.length; i++) {
                let itFor = self.listData.NS_ThietLapLuongChiTiet[i];
                if (i === index) {
                    let luongCB = self.convertToNumber(itFor.LuongNgayThuong);
                    switch (loaingay) {
                        case 1:
                            self.listData.NS_ThietLapLuongChiTiet[i].Thu7_GiaTri = commonStatisJs.FormatNumber3Digit(itFor.Thu7_GiaTri * luongCB / 100);
                            self.listData.NS_ThietLapLuongChiTiet[i].Thu7_LaPhanTramLuong = 0;
                            break;
                        case 2:
                            self.listData.NS_ThietLapLuongChiTiet[i].ThCN_GiaTri = commonStatisJs.FormatNumber3Digit(itFor.ThCN_GiaTri * luongCB / 100);
                            self.listData.NS_ThietLapLuongChiTiet[i].CN_LaPhanTramLuong = 0;
                            break;
                        case 3:
                            self.listData.NS_ThietLapLuongChiTiet[i].NgayNghi_GiaTri = commonStatisJs.FormatNumber3Digit(itFor.NgayNghi_GiaTri * luongCB / 100);
                            self.listData.NS_ThietLapLuongChiTiet[i].NgayNghi_LaPhanTramLuong = 0;
                            break;
                        case 4:
                            self.listData.NS_ThietLapLuongChiTiet[i].NgayLe_GiaTri = commonStatisJs.FormatNumber3Digit(itFor.NgayLe_GiaTri * luongCB / 100);
                            self.listData.NS_ThietLapLuongChiTiet[i].NgayLe_LaPhanTramLuong = 0;
                            break;
                    }
                    break;
                }
            }
        },
        ctluong_ClickPtram: function (index, ctluong, loaingay) {
            var self = this;
            for (let i = 0; i < self.listData.NS_ThietLapLuongChiTiet.length; i++) {
                let itFor = self.listData.NS_ThietLapLuongChiTiet[i];
                if (i === index) {
                    let luongCB = self.convertToNumber(itFor.LuongNgayThuong);
                    switch (loaingay) {
                        case 1:
                            self.listData.NS_ThietLapLuongChiTiet[i].Thu7_GiaTri = commonStatisJs.roundDecimal(self.convertToNumber(itFor.Thu7_GiaTri) * 100 / luongCB);
                            self.listData.NS_ThietLapLuongChiTiet[i].Thu7_LaPhanTramLuong = 1;
                            break;
                        case 2:
                            self.listData.NS_ThietLapLuongChiTiet[i].ThCN_GiaTri = commonStatisJs.roundDecimal(self.convertToNumber(itFor.ThCN_GiaTri) * 100 / luongCB);
                            self.listData.NS_ThietLapLuongChiTiet[i].CN_LaPhanTramLuong = 1;
                            break;
                        case 3:
                            self.listData.NS_ThietLapLuongChiTiet[i].NgayNghi_GiaTri = commonStatisJs.roundDecimal(self.convertToNumber(itFor.NgayNghi_GiaTri) * 100 / luongCB);
                            self.listData.NS_ThietLapLuongChiTiet[i].NgayNghi_LaPhanTramLuong = 1;
                            break;
                        case 4:
                            self.listData.NS_ThietLapLuongChiTiet[i].NgayLe_GiaTri = commonStatisJs.roundDecimal(self.convertToNumber(itFor.NgayLe_GiaTri) * 100 / luongCB);
                            self.listData.NS_ThietLapLuongChiTiet[i].NgayLe_LaPhanTramLuong = 1;
                            break;
                    }
                    break;
                }
            }
        },
        SaveThietLapLuongToDB: function (ngayketthuc) {
            var self = this;
            var loaipc = parseInt(self.thietLapCoBan.LoaiPhuCap_GiamTru);
            var ngayapdung = $('#dateTNLuong').val();
            ngayapdung = moment(ngayapdung, 'DD/MM/YYYY').format('YYYY-MM-DD');

            var model = {
                ID: self.thietLapCoBan.ID,
                ID_NhanVien: self.thietLapCoBan.ID_NhanVien,
                NgayApDung: ngayapdung,
                NgayKetThuc: ngayketthuc,
                SoTien: self.thietLapCoBan.SoTien,
                HeSo: self.thietLapCoBan.HeSo,
                Bac: self.thietLapCoBan.bac,
                NoiDung: self.thietLapCoBan.NoiDung,
                ID_LoaiLuong: self.thietLapCoBan.ID_LoaiLuong,
                LoaiLuong: loaipc,
                ID_DonVi: self.ID_DonVi,
            }
            var lstDetail = self.listData.NS_ThietLapLuongChiTiet;
            if (loaipc < 5) {
                lstDetail[0].LuongNgayThuong = model.SoTien;

                if (self.thietLapOT.isShow) {
                    lstDetail.push(self.thietLapOT);
                }
            }
            else {
                // phucap/giamtru
                lstDetail = [];
            }

            var myData = {
                luongPhuCap: model,
                thietLapChiTiet: lstDetail,
                IsNew: self.Isnew,
            };
            console.log(myData);
            $.ajax({
                data: myData,
                url: self.UrlNhanVienAPI + "SaveLuongPhuCap",
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (item) {
                    if (item.res === true) {
                        $('#modalNvluongphucap').modal("hide");
                        bottomrightnotify(item.mess, "success");
                        $('body').trigger("InsertNvluongphucapSuccess");
                    }
                    else {
                        self.messageError(item.mess);
                    }
                    console.log(item)
                }
            });
        },
        SaveTax: function (event) {
            var self = this;
            if (commonStatisJs.CheckNull(self.thietLapCoBan.HeSo)) {
                self.thietLapCoBan.heso = 1;
            }
            // check khoangthoigian tao
            var ngayapdung = $('#dateTNLuong').val();
            var ngayketthuc = $('#dateDNLuong').val();
            ngayapdung = moment(ngayapdung, 'DD/MM/YYYY').format('YYYY-MM-DD');

            var stext = 'lương';
            var loailuong = parseInt(self.thietLapCoBan.LoaiLuong);
            var loaipc = parseInt(self.thietLapCoBan.LoaiPhuCap_GiamTru);
            switch (loailuong) {
                case 5:
                    stext = 'phụ cấp';
                    break;
                case 6:
                    stext = 'giảm trừ';
            }

            if (loailuong !== 1) {
                if (self.thietLapCoBan.ID_LoaiLuong === null) {
                    self.messageError("Vui lòng chọn " + stext);
                    return false;
                }
            }

            var ex = [];
            if (loailuong < 5) {
                if (ngayketthuc === null || ngayketthuc === '') {
                    ngayketthuc = null;
                    ex = $.grep(self.listData.NS_LuongPhuCap, function (x) {
                        return x.ID !== self.thietLapCoBan.ID && x.LoaiLuong < 5 && (x.NgayKetThuc === null
                            || (x.NgayKetThuc !== null && ngayapdung <= moment(x.NgayKetThuc).format('YYYY-MM-DD')));
                    });
                }
                else {
                    ngayketthuc = moment(ngayketthuc, 'DD/MM/YYYY').format('YYYY-MM-DD');

                    ex = $.grep(self.listData.NS_LuongPhuCap, function (x) {
                        return x.ID !== self.thietLapCoBan.ID && x.LoaiLuong < 5 && ((x.NgayKetThuc === null && ngayketthuc >= moment(x.NgayApDung).format('YYYY-MM-DD'))
                            || (x.NgayKetThuc !== null && ((ngayapdung >= moment(x.NgayApDung).format('YYYY-MM-DD') && ngayapdung <= moment(x.NgayKetThuc).format('YYYY-MM-DD'))
                                || ((ngayketthuc <= moment(x.NgayKetThuc).format('YYYY-MM-DD')) && ngayketthuc >= moment(x.NgayApDung).format('YYYY-MM-DD')))));
                    });
                }
                if (ex.length > 0) {
                    self.messageError("Bạn đã thiết lập loại " + stext + " trong khoảng thời gian này");
                    return false;
                }
            }
            else {
                // check exist loaiphucap/giamtru
                if (ngayketthuc === null || ngayketthuc === '') {
                    ngayketthuc = null;
                    ex = $.grep(self.listData.NS_LuongPhuCap, function (x) {
                        return x.LoaiLuong > 5 && x.ID !== self.thietLapCoBan.ID && x.ID_LoaiLuong === self.thietLapCoBan.ID_LoaiLuong && (x.NgayKetThuc === null
                            || (x.NgayKetThuc !== null && ngayapdung <= moment(x.NgayKetThuc).format('YYYY-MM-DD')));
                    });
                }
                else {
                    ngayketthuc = moment(ngayketthuc, 'DD/MM/YYYY').format('YYYY-MM-DD');
                    ex = $.grep(self.listData.NS_LuongPhuCap, function (x) {
                        return x.LoaiLuong > 5 && x.ID !== self.thietLapCoBan.ID && x.ID_LoaiLuong === self.thietLapCoBan.ID_LoaiLuong && ((x.NgayKetThuc === null && ngayketthuc >= moment(x.NgayApDung).format('YYYY-MM-DD'))
                            || (x.NgayKetThuc !== null && ((ngayapdung >= moment(x.NgayApDung).format('YYYY-MM-DD') && ngayapdung <= moment(x.NgayKetThuc).format('YYYY-MM-DD'))
                                || ((ngayketthuc <= moment(x.NgayKetThuc).format('YYYY-MM-DD')) && ngayketthuc >= moment(x.NgayApDung).format('YYYY-MM-DD')))));
                    });
                }
                if (ex.length > 0) {
                    self.messageError("Bạn đã thiết lập loại " + stext + " trong khoảng thời gian này");
                    return false;
                }
            }

            if (commonStatisJs.CheckNull($('#dateTNLuong').val())) {
                self.messageError("Vui lòng nhập ngày áp dụng");
                return false;
            }
            if (commonStatisJs.CheckNull(self.thietLapCoBan.SoTien)) {
                if (loaipc !== 3 && loaipc !== 4) {
                    self.messageError("Vui lòng nhập số tiền");
                    return false;
                }
            }

            $.getJSON(self.UrlNhanSuAPI + 'CheckNhanVienExist_ChamCong?idNhanVien=' + self.thietLapCoBan.ID_NhanVien + '&idDonVi=' + self.ID_DonVi, function (x) {
             
                if (x.res === true) {
                    if (x.dataSoure !== '') {
                        commonStatisJs.ConfirmDialog_OKCancel('Thiết lập lương', x.dataSoure,
                            function () {
                                // cập nhật + update TrangThai BangLuong
                                self.SaveThietLapLuongToDB(ngayketthuc);
                                $('#modalPopuplgDelete').modal('hide');
                            }, function () {
                                $('#modalPopuplgDelete').modal('hide');
                                // không làm gì cả
                                return false;
                            });
                    }
                    else {
                        self.SaveThietLapLuongToDB(ngayketthuc);
                    }
                }
            });
        },
        InsertLoaiLuong: function (event) {
            vmNvloailuong.Insert();
        },

        UpdateLoaiLuong: function (item, event) {
            vmNvloailuong.edit(item);
        },

        DeleteLoaiLuong: function (item, event) {
            vmModalRemove.show("/api/DanhMuc/NS_NhanVienAPI/deleteloailuongNv?id=" + item.ID, "Xác nhận",
                "Bạn có chắc chắn muốn xóa loại lương:  " + item.TenLoaiLuong + " không", "deleteloailuongNvSuccess");
        },
        ClickPhuCapTheoNgay: function () {
            var self = this;
            if (self.thietLapCoBan.LaPhuCap_TheoNgay) {
                self.thietLapCoBan.LoaiPhuCap_GiamTru = 51;
            }
            else {
                self.thietLapCoBan.LoaiPhuCap_GiamTru = 52;
            }
        },
        ClickGiamTruTheoNgay: function () {
            var self = this;
            if (self.thietLapCoBan.LaGiamTru_TheoLan) {
                self.thietLapCoBan.LoaiPhuCap_GiamTru = 61;
            }
            else {
                self.thietLapCoBan.LoaiPhuCap_GiamTru = 62;
            }
        },
        SetCheck_PhuCapTheoNgay: function () {
            var self = this;
            if (self.thietLapCoBan.LoaiPhuCap_GiamTru === 51) {
                self.thietLapCoBan.LaPhuCap_TheoNgay = true;
            }
            else {
                if (self.thietLapCoBan.LoaiPhuCap_GiamTru === 61) {
                    self.thietLapCoBan.LaGiamTru_TheoLan = true;
                }
                else {
                    self.thietLapCoBan.LaGiamTru_TheoLan = false;
                }
                self.thietLapCoBan.LaPhuCap_TheoNgay = false;
            }
        },
    },
    computed: {
        lbl_SalaryType: function () {
            var self = this;
            var txt = '';
            switch (parseInt(self.thietLapCoBan.LoaiLuong)) {
                case 1:
                case 2:
                case 3:
                case 4:
                    txt = 'Loại lương';
                    break;
                case 5:
                    txt = 'Phụ cấp';
                    break;
                case 6:
                    txt = 'Giảm trừ';
                    break;
            }
            return txt;
        },
        showSoTien: function () {
            var self = this;
            var len = self.listData.NS_ThietLapLuongChiTiet.length;
            var itDefault = $.grep(self.listData.NS_ThietLapLuongChiTiet, function (x) {
                return parseFloat(x.Thu7_GiaTri) === 100 && parseFloat(x.ThCN_GiaTri) === 100
                    && parseFloat(x.NgayNghi_GiaTri) === 100 && parseFloat(x.NgayLe_GiaTri) === 100;
            });
            return self.thietLapCoBan.LoaiLuong > 4 // phucap,giamtru
                || $.inArray(self.thietLapCoBan.LoaiPhuCap_GiamTru, [1, 2, '1', '2']) > -1 // kieuluong: ngay, codinh
                || self.thietLapNangCao.isShow === false;
        },
    }
});
$('body').on('deleteloailuongNvSuccess', function () {
    vmNvLuong.getLoaiLuong();
});
vmNvLuong.getLoaiLuong();

$('#dateTNLuong').datetimepicker({
    timepicker: false,
    mask: true,
    format: 'd/m/Y'
});
$('#dateDNLuong').datetimepicker({
    timepicker: false,
    mask: true,
    format: 'd/m/Y'
});

var vmNvloailuong = new Vue({
    el: '#modalNvloailuong',
    data: {
        title: "",
        error: '',
        IsPhuCap: true,
        Isnew: true,
        ID: null,
        TenLoaiLuong: "",
        GhiChu: '',
        LoaiLuong: 5, // 5.phucap, 6.giamtru
    },
    methods: {
        messageError: function (input) {
            this.error = input;
        },
        Insert: function () {
            $('#modalNvloailuong').modal("show");
            this.refresh();
        },
        edit: function (model) {
            if (model !== null && !commonStatisJs.CheckNull(model.ID)) {
                this.error = "";
                this.Isnew = false;
                this.ID = model.ID;
                this.TenLoaiLuong = model.TenLoaiLuong;
                this.GhiChu = model.GhiChu;
                this.LoaiLuong = model.LoaiLuong;

                if (model.LoaiLuong === 6) {
                    this.title = "khoản giảm trừ";
                    this.IsPhuCap = false;
                }
                else {
                    this.title = "khoản phụ cấp";
                    this.IsPhuCap = true;
                }

                $('#modalNvloailuong').modal("show");
            }
            else {
                bottomrightnotify("Không lấy được thông tin cần cập nhật", "danger");
            }
        },
        refresh: function () {
            this.ID = null;
            this.Isnew = true;
            this.TenLoaiLuong = "";
            this.error = "";
            this.GhiChu = "";
            if (this.IsPhuCap) {
                this.title = "khoản phụ cấp";
            }
            else {
                this.title = "khoản giảm trừ";
            }
        },
        Save: function (event) {
            var self = this;
            if (commonStatisJs.CheckNull(self.TenLoaiLuong)) {
                self.messageError("Vui lòng tên " + self.title);
            }
            else {
                let loaipc = 52;
                if (vmNvLuong.thietLapCoBan.LoaiLuong === 6) {
                    loaipc = 62;
                }

                var model = {
                    ID: self.ID,
                    TenLoaiLuong: self.TenLoaiLuong,
                    GhiChu: self.GhiChu,
                    LoaiLuong: vmNvLuong.thietLapCoBan.LoaiLuong,
                };

                $.ajax({
                    data: model,
                    url: "/api/DanhMuc/NS_NhanVienAPI/SaveLoaiLuong?Isnew=" + self.Isnew,
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (item) {
                        if (item.res === true) {
                            vmNvLuong.getLoaiLuong();
                            $('#modalNvloailuong').modal("hide");
                            bottomrightnotify('Thêm mới thành công', "success");
                            item.LoaiPhuCap_GiamTru = loaipc;
                            vmNvLuong.ChoseLoaiPhuCap(item.dataSoure);
                        }
                        else {
                            self.messageError(item.mess);
                        }
                    }
                });
            }
        }
    },
    computed: {

    }
});

function ChoseKieuTinhLuong(key, value) {
    let obj = {
        Key: key,
        Value: value,
    }
    vmNvLuong.ChoseKieuLuong(obj);
}
vmNvLuong.GetListCaLamViec_ofDonVi();


