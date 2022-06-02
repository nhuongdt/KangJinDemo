var vmGroupNewspaper = new Vue({
    el: '#modalNhomTinTuc',
    data: {
        model: {
            ID: null,
            Ten: "",
            GhiChu: "",
            NhomID: null,
            Loai: null,
            IsNews: true
        },
        titileNewsGroup: "Thêm mới",

    },
    methods: {
        Insert: function (type) {
            this.titileNewsGroup = "Thêm mới thể loại";
            this.model = {
                ID: null,
                Ten: "",
                GhiChu: "",
                NhomID: null,
                Loai: type,
                IsNews: true
            };
            
            $('#modalNhomTinTuc').modal('show');
        },
        Update: function (item) {
            this.titileNewsGroup = "Thêm mới thể loại";
            this.model = {
                ID: item.ID,
                Ten: item.TenNhomBaiViet,
                GhiChu: item.GhiChu,
                NhomID: item.ID_NhomCha,
                Loai: item.LoaiNhomBaiViet,
                IsNews: false,
            };
            $('#modalNhomTinTuc').modal('show');
        },
        Save: function () {
            var self = this;
            if (localValidate.CheckNull(self.model.Ten)) {
                AlertError("Vui lòng nhập tên thể loại");
            }
            else {
                var model = self.model;
                $.ajax({
                    data: model,
                    url: '/SsoftApi/News/EditGroupNews',
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (result) {
                        if (result.res === true) {
                            $('body').trigger("NhomTinTucSuccess");
                            $('#modalNhomTinTuc').modal('hide');
                            AlertSuccess(result.mess);

                        }
                        else {
                            AlertError(result.mess);
                        }
                    }
                });
            }
        },
        remove: function (model) {
            if (confirm('Bạn có chắc chắn muốn xóa thể loại này không?')) {
                $.ajax({
                    data: model,
                    url: '/SsoftApi/News/RemoveGroupNews',
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (result) {
                        if (result.res === true) {
                            $('body').trigger("NhomTinTucSuccess");
                            AlertSuccess(result.mess);
                        }
                        else {
                            AlertError(result.mess);
                        }
                    }
                });
            }
        }
        
    }
});