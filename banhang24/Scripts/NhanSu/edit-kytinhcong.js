var vmEditKyTinhCong = new Vue({
    el: '#modalEditKyTinhCong',
    data: {
        isNew: true,
        loadding: false,
        data: {}
        
    },
    methods: {
        AddNew: function () {
            this.data = {
                ID: null,
                Ky: null,
                TuNgay: null,
                DenNgay: null,
                TrangThai: '1',
            };
            this.isNew = true;
            $('#modalthemmoikytinhcong').modal('show')
        },

        Edit: function (item) {
            var self = this;
            self.isNew = false;
            self.data = item;
            self.data.TuNgay = new Date(self.data.TuNgay);
            self.data.DenNgay = new Date(self.data.DenNgay);
            $('#modalthemmoikytinhcong').modal('show')
        },
        Delete: function (item) {
            var self = this;
            self.data = item;
            $('#modaldeletekytinhcong').modal('show')
        },
        ChangDatetime: function (date, type) {
            var sel = this;
            if (type === 1) {
                sel.data.TuNgay = date;
            }
            else {
                sel.data.DenNgay = date;
            }
        },
        SaveKyTinhCong: function () {
            var self = this;
           
            if (self.data.TuNgay === null || self.data.TuNgay === undefined) {
                commonStatisJs.ShowMessageDanger("Vui lòng nhập từ ngày");
                return;
            }
            if (self.data.DenNgay === null || self.data.DenNgay === undefined) {
                commonStatisJs.ShowMessageDanger("Vui lòng nhập đến ngày");
                return;
            }
            if (self.data.TuNgay.getMonth() !== self.data.DenNgay.getMonth() || self.data.TuNgay.getFullYear() !== self.data.DenNgay.getFullYear()) {
                commonStatisJs.ShowMessageDanger("Từ ngày đến ngày phải nằm trong 1 tháng");
                return;
            }
            else {
                self.loadding = true;
                var model = {
                    ID: self.data.ID,
                    Ky: self.data.Ky,
                    TuNgay: commonStatisJs.convertDateToDateServer(self.data.TuNgay),
                    DenNgay: commonStatisJs.convertDateToDateServer(self.data.DenNgay),
                    TrangThai: self.data.TrangThai,
                }
                var url = "/api/DanhMuc/NS_NhanSuAPI/InsertKyTinhCong";
                if (!self.isNew) {
                    url = "/api/DanhMuc/NS_NhanSuAPI/UpdateKyTinhCong";
                }

                $.ajax({
                    data: model,
                    url: url + "?" + "ID_DonVi=" + $('#hd_IDdDonVi').val() + "&ID_NhanVien=" + $('.idnhanvien').text(),
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (data) {
                        if (data.res === true) {
                            commonStatisJs.ShowMessageSuccess(data.mess);
                            $('#modalthemmoikytinhcong').modal('hide');
                            $('body').trigger('AddKyTinhCongSucces');
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

        DeleteKyTinhCong: function () {
            var self = this;
                self.loadding = true;
                var model = {
                    ID: self.data.ID,
                    Ky: self.data.Ky,
                    TuNgay: commonStatisJs.convertDateToDateServer(self.data.TuNgay),
                    DenNgay: commonStatisJs.convertDateToDateServer(self.data.DenNgay),
                    TrangThai: self.data.TrangThai,
                }
                var url = "/api/DanhMuc/NS_NhanSuAPI/DeleteKyTinhCong";
                $.ajax({
                    data: model,
                    url: url + "?" + "ID_DonVi=" + $('#hd_IDdDonVi').val() + "&ID_NhanVien=" + $('.idnhanvien').text(),
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (data) {
                        if (data.res === true) {
                            commonStatisJs.ShowMessageSuccess(data.mess);
                            $('#modaldeletekytinhcong').modal('hide');
                            $('body').trigger('AddKyTinhCongSucces');
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
        },
        LatchWorkCount: function (item) {

        },

        
    },
    computed: {
        TitlePopup: function () {
            if (this.isNew) {
                return "Thêm mới kỳ tính công";
            }
            else {
                return "Cập nhật kỳ tính công";
            }
        },
    },
});
$('.datetime').on('change', function () {
    var dateParts = $(this).val().split("/");
    if (dateParts.length >= 3) {
        var dateObject = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
        vmEditKyTinhCong.ChangDatetime(dateObject, $(this).data('id'));
    }
})