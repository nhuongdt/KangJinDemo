
var vmEditCaLamViec = new Vue({
    el: '#modalEditCaLamViec',
    data: {
        isNew: true,
        loadding: false,
        data: {},
        listchinhanh: [],
    },
    methods: {
        AddNew: function () {
            this.isNew = true;
            for (var i = 0; i < this.listchinhanh.length; i++) {
                this.listchinhanh[i].Checked = false;
                if (this.listchinhanh[i].Id === $('#hd_IDdDonVi').val()) {
                    this.listchinhanh[i].Checked = true;
                }
            }

            this.data = {
                MaCa: '',
                TenCa: '',
                TrangThai: 1,
                NguoiTao: '',
                NgayTao: new Date(2020, 0, 1, 7, 0, 0),
                GioVao: null,
                GioRa: null,
                NghiGiuaCaTu: null,
                NghiGiuaCaDen: null,
                GioOTBanNgayTu: null,
                GioOTBanNgayDen: null,
                GioOTBanDemTu: null,
                GioOTBanDemDen: null,
                CachLayGioCong: 1,
                LaCaDem: false,
                TongGioCong: 0,
                GhiChuCaLamViec: '',
                SoPhutDiMuon: 0,
                SoPhutVeSom: 0,
                GioVaoTu: null,
                GioVaoDen: null,
                GioRaTu: null,
                GioRaDen: null,
                TinhOTBanNgayTu: null,
                TinhOTBanNgayDen: null,
                TinhOTBanDemTu: null,
                TinhOTBanDemDen: null,
                SoGioOTToiThieu: 0,
                GhiChuTinhGio: '',
            }
            $('#modalthemmoicalamviec').modal('show')
        },

        Edit: function (item) {
            var self = this;
            self.isNew = false;
            self.data = item;
            self.data.MaCa = item.MaCa;
            self.data.TenCa = item.TenCa;
            self.data.GioVao = new Date(self.data.GioVao);
            self.data.GioRa = new Date(self.data.GioRa);
            if (!commonStatisJs.CheckNull(self.data.NghiGiuaCaTu)) {
                self.data.NghiGiuaCaTu = new Date(self.data.NghiGiuaCaTu);
            }
            if (!commonStatisJs.CheckNull(self.data.NghiGiuaCaDen)) {
                self.data.NghiGiuaCaDen = new Date(self.data.NghiGiuaCaDen);
            }
            if (!commonStatisJs.CheckNull(self.data.GioOTBanNgayTu)) {
                self.data.GioOTBanNgayTu = new Date(self.data.GioOTBanNgayTu);
            }
            if (!commonStatisJs.CheckNull(self.data.GioOTBanNgayDen)) {
                self.data.GioOTBanNgayDen = new Date(self.data.GioOTBanNgayDen);
            }
            if (!commonStatisJs.CheckNull(self.data.GioOTBanDemTu)) {
                self.data.GioOTBanDemTu = new Date(self.data.GioOTBanDemTu);
            }
            if (!commonStatisJs.CheckNull(self.data.GioOTBanDemDen)) {
                self.data.GioOTBanDemDen = new Date(self.data.GioOTBanDemDen);
            }
            if (!commonStatisJs.CheckNull(self.data.GioVaoTu)) {
                self.data.GioVaoTu = new Date(self.data.GioVaoTu);
            }
            if (!commonStatisJs.CheckNull(self.data.GioVaoDen)) {
                self.data.GioVaoDen = new Date(self.data.GioVaoDen);
            }
            if (!commonStatisJs.CheckNull(self.data.GioRaTu)) {
                self.data.GioRaTu = new Date(self.data.GioRaTu);
            }
            if (!commonStatisJs.CheckNull(self.data.GioRaDen)) {
                self.data.GioRaDen = new Date(self.data.GioRaDen);
            }
            if (!commonStatisJs.CheckNull(self.data.TinhOTBanNgayTu)) {
                self.data.TinhOTBanNgayTu = new Date(self.data.TinhOTBanNgayTu);
            }
            if (!commonStatisJs.CheckNull(self.data.TinhOTBanNgayDen)) {
                self.data.TinhOTBanNgayDen = new Date(self.data.TinhOTBanNgayDen);
            }
            if (!commonStatisJs.CheckNull(self.data.TinhOTBanDemTu)) {
                self.data.TinhOTBanDemTu = new Date(self.data.TinhOTBanDemTu);
            }
            if (!commonStatisJs.CheckNull(self.data.TinhOTBanDemDen)) {
                self.data.TinhOTBanDemDen = new Date(self.data.TinhOTBanDemDen);
            }
            if (item.ListIdDonVI !== null && item.ListIdDonVI.length > 0) {
                // reset chinhanh is checked
                self.listchinhanh.map(x => x.Checked = false);
                var result = self.listchinhanh.filter(o => item.ListIdDonVI.includes(o.Id));
                result.forEach(function (element) {
                    element.Checked = true;
                });
            }
            $('#modalthemmoicalamviec').modal('show')
        },

        Copy: function (item) {
            var self = this;
            self.isNew = true;
            self.data = item;
            self.data.ID = null;
            self.data.MaCa = null;
            self.data.GioVao = new Date(self.data.GioVao);
            self.data.GioRa = new Date(self.data.GioRa);
            if (!commonStatisJs.CheckNull(self.data.NghiGiuaCaTu)) {
                self.data.NghiGiuaCaTu = new Date(self.data.NghiGiuaCaTu);
            }
            if (!commonStatisJs.CheckNull(self.data.NghiGiuaCaDen)) {
                self.data.NghiGiuaCaDen = new Date(self.data.NghiGiuaCaDen);
            }
            if (!commonStatisJs.CheckNull(self.data.GioOTBanNgayTu)) {
                self.data.GioOTBanNgayTu = new Date(self.data.GioOTBanNgayTu);
            }
            if (!commonStatisJs.CheckNull(self.data.GioOTBanNgayDen)) {
                self.data.GioOTBanNgayDen = new Date(self.data.GioOTBanNgayDen);
            }
            if (!commonStatisJs.CheckNull(self.data.GioOTBanDemTu)) {
                self.data.GioOTBanDemTu = new Date(self.data.GioOTBanDemTu);
            }
            if (!commonStatisJs.CheckNull(self.data.GioOTBanDemDen)) {
                self.data.GioOTBanDemDen = new Date(self.data.GioOTBanDemDen);
            }
            if (!commonStatisJs.CheckNull(self.data.GioVaoTu)) {
                self.data.GioVaoTu = new Date(self.data.GioVaoTu);
            }
            if (!commonStatisJs.CheckNull(self.data.GioVaoDen)) {
                self.data.GioVaoDen = new Date(self.data.GioVaoDen);
            }
            if (!commonStatisJs.CheckNull(self.data.GioRaTu)) {
                self.data.GioRaTu = new Date(self.data.GioRaTu);
            }
            if (!commonStatisJs.CheckNull(self.data.GioRaDen)) {
                self.data.GioRaDen = new Date(self.data.GioRaDen);
            }
            if (!commonStatisJs.CheckNull(self.data.TinhOTBanNgayTu)) {
                self.data.TinhOTBanNgayTu = new Date(self.data.TinhOTBanNgayTu);
            }
            if (!commonStatisJs.CheckNull(self.data.TinhOTBanNgayDen)) {
                self.data.TinhOTBanNgayDen = new Date(self.data.TinhOTBanNgayDen);
            }
            if (!commonStatisJs.CheckNull(self.data.TinhOTBanDemTu)) {
                self.data.TinhOTBanDemTu = new Date(self.data.TinhOTBanDemTu);
            }
            if (!commonStatisJs.CheckNull(self.data.TinhOTBanDemDen)) {
                self.data.TinhOTBanDemDen = new Date(self.data.TinhOTBanDemDen);
            }
            if (item.ListIdDonVI !== null && item.ListIdDonVI.length > 0) {
                var result = self.listchinhanh.filter(o => item.ListIdDonVI.includes(o.Id));
                result.forEach(function (element) {
                    element.Checked = true;
                });
            }
            $('#modalthemmoicalamviec').modal('show')
        },
        Delete: function (item) {
            var self = this;
            this.data = item;
            commonStatisJs.ConfirmDialog_OKCancel('Xóa ca làm việc', 'Bạn có chắc chắn muốn xóa ca <b> ' + item.MaCa + '</b> không?',
                function () {
                    self.DeleteCalamViec();
                })
        },

        CompareTime(time1, time2) {
            if (!commonStatisJs.CheckNull(time1) && !commonStatisJs.CheckNull(time2)) {
                return (time1 < time2);
            }
            return true;
        },
        ChangeTime: function (time, type) {
            var self = this;
            var timenew = new Date(2020, 0, 1, 7, 0, 0);
            if (commonStatisJs.CheckNull(time)) {
                timenew = null;
            }
            else if (time.split(':').length === 2) {
                timenew = new Date(2020, 0, 1, time.split(':')[0], time.split(':')[1]);
            }
            switch (type) {
                case 1:
                    self.data.GioVao = timenew;
                    if (!self.CompareTime(self.data.GioVao, self.data.GioRa)) {
                        self.data.GioVao = self.data.GioRa;
                    }
                    self.SumTongGioCong();
                    break;
                case 2:
                    self.data.GioRa = timenew;
                    if (!self.CompareTime(self.data.GioVao, self.data.GioRa)) {
                        self.data.GioRa = self.data.GioVao;
                    }
                    self.SumTongGioCong();
                    break;
                case 3:
                    self.data.NghiGiuaCaTu = timenew;
                    if (!self.CompareTime(self.data.NghiGiuaCaTu, self.data.NghiGiuaCaDen)) {
                        self.data.NghiGiuaCaTu = self.data.NghiGiuaCaDen;
                    }
                    self.SumTongGioCong();
                    break;
                case 4:
                    self.data.NghiGiuaCaDen = timenew;
                    if (!self.CompareTime(self.data.NghiGiuaCaTu, self.data.NghiGiuaCaDen)) {
                        self.data.NghiGiuaCaDen = self.data.NghiGiuaCaTu;
                    }
                    self.SumTongGioCong();
                    break;
                case 5:
                    self.data.GioOTBanNgayTu = timenew;
                    if (!self.CompareTime(self.data.GioOTBanNgayTu, self.data.GioOTBanNgayDen)) {
                        self.data.GioOTBanNgayTu = self.data.GioOTBanNgayDen;
                    }
                    break;
                case 6:
                    self.data.GioOTBanNgayDen = timenew;
                    if (!self.CompareTime(self.data.GioOTBanNgayTu, self.data.GioOTBanNgayDen)) {
                        self.data.GioOTBanNgayDen = self.data.GioOTBanNgayTu;
                    }
                    break;
                case 7:
                    self.data.GioOTBanDemTu = timenew;
                    if (!self.CompareTime(self.data.GioOTBanDemTu, self.data.GioOTBanDemDen)) {
                        self.data.GioOTBanDemTu = self.data.GioOTBanDemDen;
                    }
                    break;
                case 8:
                    self.data.GioOTBanDemDen = timenew;
                    if (!self.CompareTime(self.data.GioOTBanDemTu, self.data.GioOTBanDemDen)) {
                        self.data.GioOTBanDemDen = self.data.GioOTBanDemTu;
                    }
                    break;
                case 9:
                    self.data.GioVaoTu = timenew;
                    if (!self.CompareTime(self.data.GioVaoTu, self.data.GioVaoDen)) {
                        self.data.GioVaoTu = self.data.GioVaoDen;
                    }
                    break;
                case 10:
                    self.data.GioVaoDen = timenew;
                    if (!self.CompareTime(self.data.GioVaoTu, self.data.GioVaoDen)) {
                        self.data.GioVaoDen = self.data.GioVaoTu;
                    }
                    break;
                case 11:
                    self.data.GioRaTu = timenew;
                    if (!self.CompareTime(self.data.GioRaTu, self.data.GioRaDen)) {
                        self.data.GioRaTu = self.data.GioRaDen;
                    }
                    break;
                case 12:
                    self.data.GioRaDen = timenew;
                    if (!self.CompareTime(self.data.GioRaTu, self.data.GioRaDen)) {
                        self.data.GioRaDen = self.data.GioRaTu;
                    }
                    break;
                case 13:
                    self.data.TinhOTBanNgayTu = timenew;
                    if (!self.CompareTime(self.data.TinhOTBanNgayTu, self.data.TinhOTBanNgayDen)) {
                        self.data.TinhOTBanNgayTu = self.data.TinhOTBanNgayDen;
                    }
                    break;
                case 14:
                    self.data.TinhOTBanNgayDen = timenew;
                    if (!self.CompareTime(self.data.TinhOTBanNgayTu, self.data.TinhOTBanNgayDen)) {
                        self.data.TinhOTBanNgayDen = self.data.TinhOTBanNgayTu;
                    }
                    break;
                case 15:
                    self.data.TinhOTBanDemTu = timenew;
                    if (!self.CompareTime(self.data.TinhOTBanDemTu, self.data.TinhOTBanDemDen)) {
                        self.data.TinhOTBanDemTu = self.data.TinhOTBanDemDen;
                    }
                    break;
                case 16:
                    self.data.TinhOTBanDemDen = timenew;
                    if (!self.CompareTime(self.data.TinhOTBanDemTu, self.data.TinhOTBanDemDen)) {
                        self.data.TinhOTBanDemDen = self.data.TinhOTBanDemTu;
                    }
                    break;
                default:
                // code block
            }

        },

        SumTongGioCong: function () {
            var self = this;
            if (!commonStatisJs.CheckNull(self.data.GioRa) && !commonStatisJs.CheckNull(self.data.GioVao)) {
                var tong = (self.data.GioRa.getHours() * 60 + self.data.GioRa.getMinutes()) - (self.data.GioVao.getHours() * 60 + self.data.GioVao.getMinutes());
                if (!commonStatisJs.CheckNull(self.data.NghiGiuaCaTu) && !commonStatisJs.CheckNull(self.data.NghiGiuaCaDen)) {
                    tong = tong - ((self.data.NghiGiuaCaDen.getHours() * 60 + self.data.NghiGiuaCaDen.getMinutes()) - (self.data.NghiGiuaCaTu.getHours() * 60 + self.data.NghiGiuaCaTu.getMinutes()));
                }
                self.data.TongGioCong = (tong / 60).toFixed(1);
            }
            else {
                self.data.TongGioCong = 0;
            }
        },

        ChangeMaCa: function () {
            this.data.MaCa = commonStatisJs.convertVieToEng(this.data.MaCa).toUpperCase();
        },

        SaveCaLamViec: function (evt) {
            var self = this;
            if (commonStatisJs.CheckNull(self.data.TenCa)) {
                commonStatisJs.ShowMessageDanger("Vui lòng nhập tên ca");
                return;
            }
            else if (!self.listchinhanh.some(o => o.Checked)) {
                commonStatisJs.ShowMessageDanger("Vui lòng chọn chi nhánh");
                return;
            }
            else if (commonStatisJs.CheckNull(self.data.GioVao)) {
                commonStatisJs.ShowMessageDanger("Vui lòng chọn giờ vào");
                return;
            }
            else if (commonStatisJs.CheckNull(self.data.GioRa)) {
                commonStatisJs.ShowMessageDanger("Vui lòng chọn giờ ra");
                return;
            }
            else {
                self.loadding = true;
                var model = commonStatisJs.CopyObject(self.data);
                model.MaCa = (model.MaCa !== null && model.MaCa !== undefined) ? model.MaCa.trim() : '';
                model.GioVao = commonStatisJs.convertDateToDateServer(self.data.GioVao);
                model.GioRa = commonStatisJs.convertDateToDateServer(self.data.GioRa);
                model.NghiGiuaCaTu = commonStatisJs.convertDateToDateServer(self.data.NghiGiuaCaTu);
                model.NghiGiuaCaDen = commonStatisJs.convertDateToDateServer(self.data.NghiGiuaCaDen);
                model.GioOTBanNgayTu = commonStatisJs.convertDateToDateServer(self.data.GioOTBanNgayTu);
                model.GioOTBanNgayDen = commonStatisJs.convertDateToDateServer(self.data.GioOTBanNgayDen);
                model.GioOTBanDemTu = commonStatisJs.convertDateToDateServer(self.data.GioOTBanDemTu);
                model.GioOTBanDemDen = commonStatisJs.convertDateToDateServer(self.data.GioOTBanDemDen);
                model.GioVaoTu = commonStatisJs.convertDateToDateServer(self.data.GioVaoTu);
                model.GioVaoDen = commonStatisJs.convertDateToDateServer(self.data.GioVaoDen);
                model.GioRaTu = commonStatisJs.convertDateToDateServer(self.data.GioRaTu);
                model.GioRaDen = commonStatisJs.convertDateToDateServer(self.data.GioRaDen);
                model.TinhOTBanNgayTu = commonStatisJs.convertDateToDateServer(self.data.TinhOTBanNgayTu);
                model.TinhOTBanNgayDen = commonStatisJs.convertDateToDateServer(self.data.TinhOTBanNgayDen);
                model.TinhOTBanDemTu = commonStatisJs.convertDateToDateServer(self.data.TinhOTBanDemTu);
                model.TinhOTBanDemDen = commonStatisJs.convertDateToDateServer(self.data.TinhOTBanDemDen);
                model.LaCaDem = self.data.LaCaDem === true ? 1 : false;
                var url = "/api/DanhMuc/NS_NhanSuAPI/InsertCaLamViec";
                if (!self.isNew) {
                    url = "/api/DanhMuc/NS_NhanSuAPI/UpdateCaLamViec";
                }
                var data = {
                    Model: model,
                    ListDonVi: self.listchinhanh.filter(x => x.Checked).map(x => x.Id)
                };
                $.ajax({
                    data: data,
                    url: url + "?" + "ID_DonVi=" + $('#hd_IDdDonVi').val() + "&ID_NhanVien=" + $('.idnhanvien').text(),
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (data) {
                        if (data.res === true) {
                            commonStatisJs.ShowMessageSuccess(data.mess);
                            $('#modalthemmoicalamviec').modal('hide');
                            $('body').trigger('AddCaLamViecSucces');
                        }
                        else {
                            commonStatisJs.ShowMessageDanger(data.mess);
                        }
                        self.loadding = false;
                    },
                    error: function (result) {
                        console.log(result);
                        self.loadding = false;
                    }
                });
            }
        },

        DeleteCalamViec: function () {
            var self = this;
            var model = self.data;
            $.ajax({
                data: model,
                url: "/api/DanhMuc/NS_NhanSuAPI/DeleteCaLamViec?ID_DonVi=" + $('#hd_IDdDonVi').val() + "&ID_NhanVien=" + $('.idnhanvien').text(),
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    $('#modalpopup_delete').modal('hide');
                    if (data.res === true) {
                        commonStatisJs.ShowMessageSuccess(data.mess);
                        $('body').trigger('AddCaLamViecSucces');
                    }
                    else {
                        commonStatisJs.ShowMessageDanger(data.mess);
                    }
                },
                error: function (result) {
                    $('#modalpopup_delete').modal('hide');
                    console.log(result);
                }
            });
        },
    },
    computed: {
        TitlePopup: function () {
            if (this.isNew) {
                return "Thêm mới ca làm việc";
            }
            else {
                return "Cập nhật ca làm việc";
            }
        },
    },
});

$('._settimeca').on('change', function () {
    var time = $(this).val();
    vmEditCaLamViec.ChangeTime(time, $(this).data('id'))
});
$('._settimeca').datetimepicker(
    {
        format: "H:i",
        step: 5,
        datepicker: false,
    });