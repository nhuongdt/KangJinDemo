var vmDanhMucCa = new Vue({
    el: '#CaLamViec',
    data: {
        listdata: {
            CaLamViec: [],
            ChiNhanh: [],
        },
        Role: {
            Insert: false,
            Update: false,
            Delete: false,
            NhanSu: false,
        },
    },
    methods: {
        ModalShow: function () {
            this.GetRole();
            $('#modalDMCaLamViec').modal('show');
        },
        GetRole: function () {
            var self = this;
            $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetRoleCaLamViec", function (data) {
                if (data.res) {
                    self.Role = data.dataSoure;
                    vmChamCong.Role.CaLamViec = data.dataSoure;
                }
                else {
                    commonStatisJs.ShowMessageDanger(data.mess);
                }
            });
        },
        AddNew: function () {
            vmEditCaLamViec.AddNew();
        },
        Edit: function (item) {
            var self = this;
            vmEditCaLamViec.listchinhanh = commonStatisJs.CopyArray(self.listdata.ChiNhanh);
            item.MaCa = item.Ma;
            item.TenCa = item.Ten;
            vmEditCaLamViec.Edit(item);
        },
        Delete: function (item) {
            var self = this;
            commonStatisJs.ConfirmDialog_OKCancel('Xóa ca làm việc',
                'Bạn có chắc chắn muốn xóa ca làm việc <b> ' + item.Ma +' </b > không ? ',
                function () {
                    var model = item;
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

                }, function () {
                    $('#modalPopuplgDelete').modal('hide');
                    return false;
                });
        },
    }
});
vmDanhMucCa.GetRole();