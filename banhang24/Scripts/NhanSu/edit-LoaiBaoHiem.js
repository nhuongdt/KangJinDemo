
var vmEditLoaiBaoHiem = new Vue({
    el: '#modalEditLoaiBaoHiem',
    data: {
        isNew: true,
        loadding: false,
        data: {}
    },
    methods: {
        AddNew: function () {
            this.isNew = true;
            this.data = {
                ID:null,
                TenBaoHiem: '',
                NgayApDung:null,
                TrangThai: "1",
                TyLeCongTy: 0,
                TyLeNhanVien: 0,
                GhiChu: null,
            }
            $('#modalThemMoiLoaiBaoHiem').modal('show')
        },

        Edit: function (item) {
            this.isNew = false;
            this.data = {
                ID: item.ID,
                TenBaoHiem: item.TenBaoHiem,
                NgayApDung: item.NgayApDung,
                TrangThai: item.TrangThai.toString(),
                TyLeCongTy: item.TyLeCongTy,
                TyLeNhanVien: item.TyLeNhanVien,
                GhiChu: item.GhiChu,
            }
            $('#modalThemMoiLoaiBaoHiem').modal('show')
        },
        Delete: function (item) {
            this.data = item;
            $('#modalPopupDeleteLoaiBaoHiem').modal('show')
        },

        SaveLoaiBaoHiem: function (evt) {
            var self = this;
            self.loadding = true;
            if (commonStatisJs.CheckNull(self.data.TenBaoHiem)) {
                commonStatisJs.ShowMessageDanger("Vui lòng nhập tên loại bảo hiểm");
                self.loadding = false;
                return;
            }
            else if (self.data.NgayApDung === null || self.data.NgayApDung === "" || self.data.NgayApDung === undefined) {
                commonStatisJs.ShowMessageDanger("Vui lòng chọn ngày áp dụng");
                self.loadding = false;
                return;
            }
            else if (commonStatisJs.CheckNull(self.data.TrangThai)) {
                commonStatisJs.ShowMessageDanger("Vui lòng chọn trạng thái");
                self.loadding = false;
                return;
            }
            else {
                var model = commonStatisJs.CopyObject(self.data);
                model.NgayApDung = commonStatisJs.convertDateToDateServer(self.data.NgayApDung);
                var url = "/api/DanhMuc/NS_NhanSuAPI/InsertLoaiBaoHiem";
                if (!self.isNew) {
                    url = "/api/DanhMuc/NS_NhanSuAPI/UpdateLoaiBaoHiem";
                }
                $.ajax({
                    data: model,
                    url: url ,
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (data) {
                        if (data.res === true) {
                            commonStatisJs.ShowMessageSuccess(data.mess);
                            $('#modalThemMoiLoaiBaoHiem').modal('hide');
                            $('body').trigger('AddLoaiBaoHiemSucces');
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

        DeleteLoaiBaoHiem: function () {
            var self = this;
            $.ajax({
                data: self.data,
                url: "/api/DanhMuc/NS_NhanSuAPI/DeleteLoaiBaoHiem",
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (data) {
                    $('#modalPopupDeleteLoaiBaoHiem').modal('hide');
                    if (data.res === true) {
                        commonStatisJs.ShowMessageSuccess(data.mess);
                        $('body').trigger('AddLoaiBaoHiemSucces');
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
                return "Thêm mới loại bảo hiểm";
            }
            else {
                return "Cập nhật loại bảo hiểm";
            }
        },
    },
});
$('.datetime').on('change', function () {
    var dateParts = $(this).val().split("/");
    if (dateParts.length >= 3) {
        var dateObject = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
        vmEditLoaiBaoHiem.data.NgayApDung = dateObject;
    }
})