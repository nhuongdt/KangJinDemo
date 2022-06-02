var cauhoithuonggap = function () {

    self.pageCount = ko.observable();
    self.PageIten = ko.observable();
    self.page = ko.observable(1);
    self.koSearch = ko.observable();
    self.sort = ko.observable();
    self.colum = ko.observable();
    self.TitlePopup = ko.observable("Thêm mới câu hỏi");
    self.ListResult = ko.observableArray();
    self.TrangThai = ko.observable(true);
    self.CauHoi = ko.observable();
    self.ViTri = ko.observable();
    self.ID = ko.observable();
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
            url: '/Open24Api/ApiHoTro/SearchGridHoTro',
            type: 'POST',
            async: true,
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
            error: function () {
                alert("Đã xảy ra lỗi.");
            }
        });
    }
    FilterGrid();
    console.log(1);
    self.btnInsert = function () {
        self.ID(null);
        self.CauHoi('');
        self.TrangThai(true);
        CKEDITOR.instances['CauTraLoi'].setData('');
        self.TitlePopup("Thêm mới câu hỏi thường gặp");
        self.ViTri(0);
        $('#modalhoidap').modal('show');
    }
    self.btnUpdate = function (item) {
        self.ID(item.ID);
        self.CauHoi(item.CauHoi);
        self.TrangThai(item.TrangThai);
        CKEDITOR.instances['CauTraLoi'].setData(item.CauTraLoi);
        self.TitlePopup("Cập nhật câu hỏi thường gặp");
        self.ViTri(item.ViTri);
        $('#modalhoidap').modal('show');
    }
    self.btndelete = function(model){
        if (confirm('Bạn có chắc chắn muốn xóa câu hỏi này không?')) {

            $.ajax({
                url: '/Open24Api/ApiHoTro/DeleteHoiDap',
                data: model,
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {
                    if (result.res === true) {
                        FilterGrid();
                        $('#modalhoidap').modal('hide');
                        AlertNotice(result.mess);
                    }
                    else {
                        AlertError(result.mess);
                    }
                }
            });

        }
    }
    self.save = function () {
        var cautraloi = CKEDITOR.instances['CauTraLoi'].getData();
        if (self.CauHoi() === undefined
            || self.CauHoi() === null
            || self.CauHoi().replace(/\s+/g, '') === "") {

            AlertError("Vui lòng nhập câu hỏi.");
        }
        else if (cautraloi === undefined
            || cautraloi === null
            || cautraloi === "") {
            AlertError("Vui lòng nhập câu trả lời.");
        }
        else {
            var model = {
                ID: self.ID(),
                CauHoi: self.CauHoi(),
                CauTraLoi: cautraloi,
                TrangThai: self.TrangThai(),
                ViTri: self.ViTri()
            };
            $.ajax({
                url: '/Open24Api/ApiHoTro/EditHoiDap',
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: model,
                success: function (result) {
                    if (result.res === true) {
                        FilterGrid();
                        $('#modalhoidap').modal('hide');
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

    }
    ////===============================
    //// Load dữ liệu tree role
    ////===============================
    //var tree = $('#tree').tree({
    //    primaryKey: 'id',
    //    uiLibrary: 'bootstrap',
    //    dataSource: '/AdminPage/HoTro/GetAllTinhNang',
    //    checkboxes: true,
    //});
}
ko.applyBindings(new cauhoithuonggap());


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