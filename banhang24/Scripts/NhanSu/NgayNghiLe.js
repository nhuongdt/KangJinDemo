var vmNgayNghiLe = new Vue({
    el: '#NgayNghiLe',
    data: {
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
        Role: {
            Insert: false,
            Update: false,
            Delete: false,
            NhanSu: false,
        }
    },
    methods: {
        ModalShow: function () {
            //this.GetForSearchNgayNghiLe();
            this.GetRole();
            $('#modalNgayNghiLe').modal('show');
        },
        GetRole: function () {
            var self = this;
            self.Role = {
                Insert: false,
                Update: false,
                Delete: false,
                NhanSu: false,
            };
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetRoleNgayNghiLe", function (data) {
                if (data.res) {
                    self.Role = data.dataSoure;
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        },
        GetForSearchNgayNghiLe: function () {
            var self = this;
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetForSearchNgayNghiLe", function (data) {
                if (data.res) {
                    self.databind.data = data.dataSoure;
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        },
        ChoseDateOfWeek: function (key, text) {
            var self = this;
            self.data.Thu = key;
            self.data.ThuText = text;
        },
        ChoseLoaiNgay: function () {
            var self = this;
            var loai = parseInt(self.data.LoaiNgay);
            if (loai === 0) {
                if (self.databind.data.filter(x => x.Thu != -1).length === 7) {
                    self.data.LoaiNgay = 0;
                    return;
                }
            }
            else {
                if (self.isNew) {
                    self.data.Thu = -1;
                }
            }
            self.data.LoaiNgay = loai;
        },
        AddNew: function () {
            var self = this;
            var loaingay = 2;
            if (self.databind.data.filter(x => x.Thu != -1).length < 7) {
                loaingay = 0;
            }
            this.data = {
                ID: null,
                Ngay: null,
                MoTa: null,
                Thu: -1,
                CongQuyDoi: 1.0,
                LoaiNgay: loaingay,
                HeSoLuong: 1.0,
                HeSoLuongOT: 1.0,
                ThuText: 'Chọn thứ'
            };
            this.isNew = true;
            $('#modalthemmoingaynghile').modal('show');
        },
        Edit: function (item) {
            this.data = commonStatisJs.CopyObject(item);
            this.isNew = false;
            $('#modalthemmoingaynghile').modal('show');
        },
        Delete: function (item) {
            this.data = item;
            this.isNew = false;
            $('#modaldeletengaynghile').modal('show');
        },
        Save: function () {
            var self = this;

            if (self.data.Thu < 0 && (self.data.Ngay === null || self.data.Ngay === undefined)) {
                commonStatisJs.ShowMessageDanger("Vui lòng nhập ngày");
                return;
            }
            else {
                var ngayLe = null;
                if (self.data.LoaiNgay !== 0) {
                    ngayLe = commonStatisJs.convertDateToDateServer(self.data.Ngay);
                }
                self.loadding = true;
                var model = {
                    ID: self.data.ID,
                    Thu: self.data.Thu,
                    Ngay: ngayLe,
                    MoTa: self.data.MoTa,
                    LoaiNgay: self.data.LoaiNgay,
                }
                console.log('ngayle ', model);
                var url = "/api/DanhMuc/NS_NhanSuAPI/InsertNgayNghiLe";
                if (!self.isNew) {
                    url = "/api/DanhMuc/NS_NhanSuAPI/UpdateNgayNghiLe";
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
                            $('#modalthemmoingaynghile').modal('hide');
                            self.GetForSearchNgayNghiLe();
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
        DeleteNgayNghiLe: function () {
            var self = this;
            self.loadding = true;
            var url = "/api/DanhMuc/NS_NhanSuAPI/DeleteNgayNghiLe";
            $.ajax({
                data: self.data,
                url: url + "?" + "ID_DonVi=" + $('#hd_IDdDonVi').val() + "&ID_NhanVien=" + $('.idnhanvien').text(),
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    if (data.res === true) {
                        commonStatisJs.ShowMessageSuccess(data.mess);
                        $('#modaldeletengaynghile').modal('hide');
                        self.GetForSearchNgayNghiLe();
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

    },
    computed: {
        TitlePopup: function () {
            if (this.isNew) {
                return "Thêm mới ngày nghỉ lễ";
            }
            else {
                return "Cập nhật ngày nghỉ lễ";
            }
        },
        TitleForm: function () {
            if (this.Role.NhanSu) {
                return "Danh mục ngày nghỉ lễ";
            }
            else {
                return "Danh mục ngày nghỉ lễ";
            }
        },
    },
});
vmNgayNghiLe.GetRole();
vmNgayNghiLe.GetForSearchNgayNghiLe();

$('#_NgayNghiLeDateTime').on('change', function () {
    var dateParts = $(this).val().split("/");
    if (dateParts.length >= 3) {
        vmNgayNghiLe.data.Ngay = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
    }
})

$('#ddlLoaiNgay').on('change', function () {
    vmNgayNghiLe.ChoseLoaiNgay();
});