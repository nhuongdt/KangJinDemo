var vmKyHieuCong = new Vue({
    el: '#KyHieuCong',
    data: {
        ID_DonVi: $('#hd_IDdDonVi').val(),
        databind: {
            pageview: '',
            pagenow: 1,
            data: [],
            listpage: [],
            isprev: false,
            isnext: false,
            countpage: 0,
        },
        isNew: true,
        loadding: false,
        data: {},
        Role: {}
    },
    methods: {
        ModalShow: function () {
            this.GetForSearchKyHieuCong();
            this.GetRole();
            $('#modalKyHieuCong').modal('show');
        },
        GetRole: function () {
            var self = this;
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetRoleKyHieuCong", function (data) {
                if (data.res) {
                    self.Role = data.dataSoure;
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        },
        GetForSearchKyHieuCong: function () {
            var self = this;
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetForSearchKyHieuCong?idDonVi=" + self.ID_DonVi, function (data) {
                if (data.res) {
                    self.databind.data = data.dataSoure;
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        },
        CompareTime(time1, time2) {
            if (!commonStatisJs.CheckNull(time1) && !commonStatisJs.CheckNull(time2)) {
                return (time1 < time2);
            }
            return true;
        },
        ChangeTime: function (time, type) {
            var self = this;
            var timenew = new Date(2020, 1, 1, 7, 0, 0);
            if (commonStatisJs.CheckNull(time)) {
                timenew = null;
            }
            else if (time.split(':').length === 2) {
                timenew = new Date(2020, 1, 1, time.split(':')[0], time.split(':')[1]);
            }
            switch (type) {
                case 1:
                    self.data.GioVao = timenew;
                    if (!self.CompareTime(self.data.GioVao, self.data.GioRa)) {
                        self.data.GioVao = self.data.GioRa;
                    }
                    break;
                case 2:
                    self.data.GioRa = timenew;
                    if (!self.CompareTime(self.data.GioVao, self.data.GioRa)) {
                        self.data.GioRa = self.data.GioVao;
                    }
                    break;

                default:
                // code block
            }

        },
        AddNew: function () {
            this.isNew = true;
            this.data = {
                ID: null,
                KyHieu: null,
                MoTa: null,
                CongQuyDoi: 1.0,
                GioVao: null,
                GioRa: null,
                LayGioMacDinh: true
            }
            $('#modalthemmoikyhieucong').modal('show');
        },
        Edit: function (item) {
            this.isNew = false;
            this.data = {
                ID: item.ID,
                KyHieu: item.KyHieu,
                MoTa: item.MoTa,
                CongQuyDoi: item.CongQuyDoi,
                GioVao: item.GioVao,
                GioRa: item.GioRa,
                LayGioMacDinh: item.LayGioMacDinh
            }
            $('#modalthemmoikyhieucong').modal('show');
        },
        Delete: function (item) {
            this.data = item;
            $('#modaldeletekyhieucong').modal('show');
        },
        SaveKyHieuCong: function () {
            var self = this;
            if (commonStatisJs.CheckNull(self.data.KyHieu)) {
                commonStatisJs.ShowMessageDanger("Vui lòng nhập ký hiệu");
                return;
            }
            if (commonStatisJs.CheckNull(self.data.MoTa)) {
                commonStatisJs.ShowMessageDanger("Vui lòng nhập mô tả");
                return;
            }
            if (self.data.CongQuyDoi === null || self.data.CongQuyDoi === undefined) {
                commonStatisJs.ShowMessageDanger("Vui lòng nhập công đơn vị quy đổi");
                return;
            }
            if (!self.data.LayGioMacDinh && (self.data.GioVao === null || self.data.GioVao === undefined)) {
                commonStatisJs.ShowMessageDanger("Vui lòng nhập giờ vào");
                return;
            }
            if (!self.data.LayGioMacDinh && (self.data.GioRa === null || self.data.GioRa === undefined)) {
                commonStatisJs.ShowMessageDanger("Vui lòng nhập giờ ra");
                return;
            }
            else {
                console.log('lstKyHieuCong ', self.databind.data)
                // check if LayGioMacDinh = true & same CongQuyDoi same chinhanh
                // có thể tạo ký hiệu cùng cong quy đổi: VD: nghỉ phép có lương (công = 1)
                //if (self.data.LayGioMacDinh) {
                //    let itemQD = $.grep(self.databind.data, function (x) {
                //        return x.LayGioMacDinh && x.CongQuyDoi === self.data.CongQuyDoi && x.ID !== self.data.ID;
                //    });
                //    if (itemQD.length > 0) {
                //        commonStatisJs.ShowMessageDanger("Trùng lặp công quy đổi");
                //        return;
                //    }
                //}
                self.loadding = true;
                var model = {
                    ID: self.data.ID,
                    ID_DonVi: self.ID_DonVi,
                    KyHieu: self.data.KyHieu,
                    GioVao: commonStatisJs.convertDateToDateServer(self.data.GioVao),
                    GioRa: commonStatisJs.convertDateToDateServer(self.data.GioRa),
                    MoTa: self.data.MoTa,
                    CongQuyDoi: self.data.CongQuyDoi,
                    LayGioMacDinh: self.data.LayGioMacDinh,
                }
                var url = "/api/DanhMuc/NS_NhanSuAPI/InsertKyHieuCong";
                if (!self.isNew) {
                    var url = "/api/DanhMuc/NS_NhanSuAPI/UpdateKyHieuCong";
                }
                $.ajax({
                    data: model,
                    url: url + "?" + "ID_DonVi=" + self.ID_DonVi + "&ID_NhanVien=" + $('.idnhanvien').text(),
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (data) {
                        console.log(data);
                        if (data.res === true) {
                            commonStatisJs.ShowMessageSuccess(data.mess);
                            $('#modalthemmoikyhieucong').modal('hide');
                            $('body').trigger('SaveKyHieuCongSuccess');
                            self.GetForSearchKyHieuCong();
                        }
                        else {
                            commonStatisJs.ShowMessageDanger(data.mess);
                        }
                        self.loadding = false;
                    },
                    error: function (result) {
                        self.loadding = false;
                    }
                });
            }
        },

        DeleteKyHieuCong: function () {
            var self = this;
            var url = "/api/DanhMuc/NS_NhanSuAPI/DeleteKyHieuCong";
            $.ajax({
                data: self.data,
                url: url + "?" + "ID_DonVi=" + self.ID_DonVi + "&ID_NhanVien=" + $('.idnhanvien').text(),
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    if (data.res === true) {
                        commonStatisJs.ShowMessageSuccess(data.mess);
                        $('#modaldeletekyhieucong').modal('hide');
                        self.GetForSearchKyHieuCong();
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
    computed: {
        TitlePopup: function () {
            if (this.isNew) {
                return "Thêm mới ký hiệu công";
            }
            else {
                return "Cập nhật ký hiệu công";
            }
        },
    },
});
vmKyHieuCong.GetForSearchKyHieuCong();
$('._settimeKyHieu').timepicker().on('change', function () {
    var time = $(this).val();
    vmKyHieuCong.ChangeTime(time, $(this).data('id'))
})