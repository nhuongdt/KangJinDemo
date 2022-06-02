var nhomvaitro = function () {

    var self = this;
    self.pageCount = ko.observable();
    self.PageIten = ko.observable();
    self.page = ko.observable();
    self.koSearch = ko.observable();
    self.sort = ko.observable();
    self.colum = ko.observable();
    self.TitlePopup = ko.observable("Thêm mới nhóm ngành");
    self.ListResult = ko.observableArray();
    self.ID = ko.observable();
    self.TenNganh = ko.observable();
    self.GhiChu = ko.observable();
    self.ViTri = ko.observable();
    self.TrangThai = ko.observable();
    //===============================
    // select số bản ghi trang
    //===============================
    $('#SelectedLimit').on('change', function () {
        self.page(1);
        FilterGrid();


    });

    //===============================
    // Nhập trang
    //===============================
    self.netPageKeyup = function (d, e) {
        if (e.keyCode === 13) {
            if (self.page() > self.pageCount()
                || self.pageCount() === 1
                || !$.isNumeric(self.page())) {
                self.page(1);
            }
            FilterGrid();

        }
    }

    //===============================
    // Next trang
    //===============================
    self.ClickNext = function () {
        if (self.page() < self.pageCount()) {
            self.page(self.page() + 1);
            FilterGrid();
        }

    }
    //===============================
    // click Tìm kiếm gridview
    //===============================
    self.SearchGrid = function (d, e) {
        if (e.keyCode === 13) {

            FilterGrid();
        }
    }
    //===============================
    // back trang
    //===============================
    self.ClickPrevious = function () {
        if (self.page() > 1) {
            self.page(self.page() - 1);

            FilterGrid();
        }
    }
    //===============================
    // Tìm kiếm gridview chung
    //===============================
    function FilterGrid() {
        var model = {
            Search: self.koSearch(),
            Page: self.page(),
            Limit: $('#SelectedLimit').val(),
            Columname: self.colum(),
            Sort: self.sort()
        };
        $.ajax({
            url: '/Open24Api/ApiHoTro/SearchGridNhomVaiTro',
            type: 'POST',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: model,
            success: function (result) {
                if (result.res === true) {
                    self.ListResult(result.DataSoure.Data);
                    self.PageIten(result.DataSoure.PageItem);
                    self.pageCount(result.DataSoure.PageCount);
                    self.page(result.DataSoure.Page);
                }
                else {
                    alert(result.mess);
                }
            },
            error: function (e ,ex) {
                alert("Đã xảy ra lỗi.");
            }
        });
    }
    FilterGrid();
    self.btnInsert = function () {
        self.ID(null);
        self.TenNganh('');
        self.GhiChu('');
        self.TrangThai(true);
        self.TitlePopup("Thêm mới nhóm ngành");
        tree.uncheckAll();
        $('.anh-icon').find('i').show();
        $('#blah').width("100%")
            .height("100%").hide();
        self.ViTri(0);
        $('#myModal').modal('show');
    };
    self.btnUpdate = function (item) {
        self.ID(item.ID);
        tree.uncheckAll();
        self.TenNganh(item.Ten);
        self.GhiChu(item.GhiChu);
        self.TrangThai(item.TrangThai);
        self.ViTri(item.ViTri);
        $.each(item.Listcheck, function (i, ite) {
            var node = tree.getNodeById(ite);
            if (node !== null && node !== undefined) {
                tree.check(node);
            }
        });
        if (item.Icon !== '' && item.Icon !== null) {
            $('.anh-icon').find('i').hide();
            $('#blah')
                .attr('src', item.Icon)
                .width("100%")
                .height("100%").show();
        }
        self.TitlePopup("Cập nhật nhóm ngành");
        $('#myModal').modal('show');
    };
    self.btndelete = function (model) {
        if (confirm('Bạn có chắc chắn muốn xóa nhóm ngành không?')) {
            $.ajax({
                url: '/Open24Api/ApiHoTro/DeleteNhomNganh?id=' + model.ID,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {
                    if (result.res === true) {
                        FilterGrid();
                        AlertNotice(result.mess);
                    }
                    else {

                        AlertError(result.mess);
                    }
                },
                error: function () {
                    alert("Đã xảy ra lỗi.");
                }
            });
        } else {
            return;
        }
    };
    console.log(1);
    self.save = function () {
        var fileUpload = $("#imageUploadForm").get(0);
        var files = fileUpload.files;
        var fileData = new FormData();
        if (files.length <= 0 && (self.ID() === 0 || self.ID() === null)) {
            AlertError("Vui lòng chọn icon");
            return;
        }
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }
        if (self.TenNganh() === undefined
            || self.TenNganh() === null
            || self.TenNganh() === "") {
            AlertError("Vui lòng nhập tên nhóm ngành.");
        }
        
        else {
          
            $.ajax({
                data: fileData,
                url: '/Open24Api/ApiHoTro/UploadImages',
                type: 'POST',
                processData: false,
                contentType: false,
                success: function (result) {
                    if (result.res === true) {
                        var model = {
                            ID: self.ID(),
                            Ten: self.TenNganh(),
                            TrangThai: self.TrangThai(),
                            Icon: result.mess,
                            GhiChu: self.GhiChu(),
                            ViTri: self.ViTri(),
                            ListTinhNang: tree.getCheckedNodes()
                        }
                        EditNhomNganh(model);
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
    }

    function EditNhomNganh(model) {
        $.ajax({
            url: '/Open24Api/ApiHoTro/EditNhomNganh',
            type: 'POST',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: model,
            success: function (result) {
                if (result.res === true) {
                    FilterGrid();
                    $('#myModal').modal('hide');
                    AlertNotice(result.mess);
                }
                else {
                    AlertError(result.mess);
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi.");
            }
        });
    }
    //===============================
    // Load dữ liệu tree role
    //===============================
    var tree = $('#tree').tree({
        primaryKey: 'id',
        uiLibrary: 'bootstrap',
        dataSource: '/AdminPage/HoTro/GetAllTinhNangCha',
        checkboxes: true,
    });
}
ko.applyBindings(new nhomvaitro());


//===============================
// Hiện thị Datetime
//===============================
function ConvertDate(config) {
    if (config === undefined
        || config === null
        || config.replace(/\s+/g, '') === "") {
        return "";
    }
    else {
        var a = moment(config).format('DD/MM/YYYY');
        return a;
    }
}