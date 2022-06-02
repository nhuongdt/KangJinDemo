var vmCustomers = new Vue({
    el: '#Customers',
    data: {
        modelSearch: {
            pageItem: "",
            pageCount: 1,
            page: 1,
            text: "",
            limit: null,
            groupId: null,
            data: []
        },
        Customer:
        {
            ID: 0,
            TenKhach: "",
            Anh: "",
            NoiDung: "",
            Mota: "",
            TrangThai: 1,
            MetaTitle: "",
            MetaDescriptions: "",
            MaTinhThanh: "",
            Tags: "",
            NgayDangBai: null,
            IsLichHen: false,
            Link: "/bai-viet-moi",
            IsNews: true,
            Email: "",
            SoDienThoai: "",
            MaSanPham: "",
            DiaChi:""
        }
        ,
        dataResults: [],
        Listproduct: [],
        LitsTinhThanh:[],
        titileNewsGroup: "Thêm mới",
        IsLoadCombobox: 0,
        IsChangeMetaTitle: false,
        Namemeta: "Chỉnh sửa"

    },
    methods: {
        GetData: function () {
            var self = this;
            self.modelSearch.data = [];
            $.ajax({
                data: self.modelSearch,
                url: '/SsoftApi/ApiCustomer/SearchGrid',
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (item) {
                    if (item.res === true) {
                        self.modelSearch = item.dataSoure;
                        self.dataResults = item.dataSoure.data;
                    }
                    else {
                        self.messageError(item.mess);
                    }
                }
            });
        },
        keySearch: function (event) {
            if (event.key === "Enter") {
                this.GetData();
            }
        },
        ClickPrevious: function () {
            if (this.modelSearch.page > 1) {
                this.modelSearch.page = this.modelSearch.page - 1;
                this.GetData();
            }
        },
        ClickNext: function () {
            if (this.modelSearch.page < this.modelSearch.pageCount) {
                this.modelSearch.page = this.modelSearch.page + 1;
                this.GetData();
            }
        },
        loadseletectcbb: function () {
            var self = this;
            self.IsLoadCombobox += 1;
            if (self.IsLoadCombobox === 2) {
                self.IsLoadCombobox = 0;
            var listtt = self.Customer.MaTinhThanh.split(',');
            var listsp = self.Customer.MaSanPham.split(',');
            self.LitsTinhThanh.filter(o => jQuery.inArray(o.Key, listtt) >= 0).forEach(function (element) {
                element.IsSelect = !element.IsSelect;
            });
            self.Listproduct.filter(o => jQuery.inArray(o.Key, listsp) >= 0).forEach(function (element) {
                element.IsSelect = !element.IsSelect;
            });

            }
        },
        GetCombobox: function () {
            var self = this;
            $.getJSON("/SsoftApi/ApiCustomer/GetProductAndTinhThanh", function (data) {
                if (data.res === true) {
                    self.Listproduct = data.dataSoure.sp;
                    self.LitsTinhThanh = data.dataSoure.tt;
                    self.loadseletectcbb();
                 
                } else {
                    AlertError(data.mess);
                }
            });
        },
        SelectProduct: function (item) {
            item.IsSelect = !item.IsSelect;

        },
        SelectAdress: function (item) {
            item.IsSelect = !item.IsSelect;
        },
        ClickEdit: function (ID) {
            window.location.href = "/Admin/Customer/EditCustomer?id=" + ID;
        },
        GetResultEdit: function (id) {
            var self = this;
            self.Customer.IsNews = false;
            self.titileNewsGroup = "Cập nhật";
            $.getJSON("/SsoftApi/ApiCustomer/GetEditNews?id=" + id, function (data) {
                if (data.res === true) {
                    self.Customer = data.dataSoure;
                    if (!localValidate.CheckNull(self.Customer.Anh)) {
                        $('.content-upload-img').hide();
                        $('.btn-upload-img-news').show();
                        $('#blah').attr('src', self.Customer.Anh)
                            .width("100%")
                            .height("100%").show();
                    }
                    localValidate.sleep(1000).then(() => {

                        CKEDITOR.instances['txtContentNews'].setData(self.Customer.NoiDung);
                    });
                    self.loadseletectcbb();
                   
                } else {
                    AlertError(data.mess);
                }
            });
        },
        ClickRemove: function (id) {
            var self = this;
            if (confirm('Bạn có chắc chắn muốn xóa khách hàng này không?')) {
                $.getJSON("/SsoftApi/ApiCustomer/RemoveCustomer?id=" + id, function (result) {
                    if (result.res === true) {
                        AlertSuccess(result.mess);
                        self.GetData();
                    }
                    else {
                        AlertError(result.mess);
                    }

                });
            }
        },
        SaveNews: function () {
            var self = this;
            self.Customer.NoiDung = CKEDITOR.instances['txtContentNews'].getData();

            var fileUpload = $("#imageUploadForm").get(0);
            var files = fileUpload.files;
            var fileData = new FormData();
            if (files.length <= 0 && self.Customer.IsNews === true) {
                AlertError("Vui lòng chọn ảnh đối tác");
                return;
            }
            for (var i = 0; i < files.length; i++) {
                fileData.append(files[i].name, files[i]);
            }
            self.Customer.MaSanPham = self.Listproduct.filter(o => o.IsSelect).map(function (i, e) {
                return i.Key;
            }).toString();
            self.Customer.MaTinhThanh = self.LitsTinhThanh.filter(o => o.IsSelect).map(function (i, e) {
                return i.Key;
            }).toString();
            if (localValidate.CheckNull(self.Customer.TenKhach)) {
                AlertError("Vui lòng nhập tên đối tác");
            }
            else if (localValidate.CheckNull(self.Customer.Mota)) {
                AlertError("Vui lòng nhập mô tả đối tác");
            }
            else if (localValidate.CheckNull(self.Customer.NoiDung)) {
                AlertError("Vui lòng nhập nội dung bài viết đối tác");
            }
            else if (!localValidate.CheckNull(self.Customer.Email) && !localValidate.CheckEmail(self.Customer.Email)) {
                AlertError("Email không hợp lệ");
            }
            else if (!localValidate.CheckNull(self.Customer.SoDienThoai) && !localValidate.CheckPhoneNumber(self.Customer.SoDienThoai)) {
                AlertError("Số điện thoại không hợp lệ");
            }
            else if (localValidate.CheckNull(self.Customer.MaSanPham)) {
                AlertError("Vui lòng nhập sản phẩm");
            }
            else if (localValidate.CheckNull(self.Customer.MaTinhThanh)) {
                AlertError("Vui lòng nhập tỉnh thành");
            }
            else {
            
                self.Customer.Tags = "";
                $('#tags_1_tagsinput .tag span').each(function () {
                    self.Customer.Tags += $(this).text().trim() + ",";
                });
                $.ajax({
                    data: fileData,
                    url: '/SsoftApi/ApiCustomer/UploadImages',
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    success: function (result) {
                        if (result.res === true) {
                            self.Customer.Anh = result.dataSoure;
                          
                            var model = self.Customer;
                            self.EditCustomer(model);

                        }
                        else {
                            AlertError(result.mess);
                        }
                    },
                    error: function (result) {
                        exception(result);
                    }
                });
            }
        },
        EditCustomer: function (model) {
            $.ajax({
                data: model,
                url: '/SsoftApi/ApiCustomer/EditCustomer',
                type: 'POST',
                async: false,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {

                    if (result.res === true) {
                        AlertSuccess(result.mess);
                        location.href = "/Admin/Customer/Customer";
                    }
                    else {
                        AlertError(result.mess);
                    }
                }
            });
        },
         changeEditMeta: function () {
            if (!this.IsChangeMetaTitle) {
                this.Namemeta = "Hủy"
            }
            else {
                this.Namemeta = "Chỉnh sửa";
                this.Customer.MetaTitle = this.Customer.TenKhach;
                this.Customer.MetaDescriptions = this.Customer.Mota;
            }

            this.IsChangeMetaTitle = !this.IsChangeMetaTitle;
        },
        ChangeNewsTitle: function () {
            if (!this.IsChangeMetaTitle) {
                this.Customer.MetaTitle = this.Customer.TenKhach;
                this.Customer.MetaDescriptions = this.Customer.Mota;
            }
        },
    }
});