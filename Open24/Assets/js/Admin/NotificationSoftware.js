function View() {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.koSearch = ko.observable();
    self.ListNotification = ko.observableArray();
    self.PageItem = ko.observable();
    self.pageCount = ko.observable();
    self.page = ko.observable(1);

    //===============================
    // Cập nhật đối tác
    //===============================
    self.Update = function (model) {
            window.location.href = "/AdminPage/Home/editNotificationSoftware/" + model.ID;
        
    }
    //===============================
    // Xóa đối tác
    //===============================
    self.delete = function (model) {
        if (confirm('Bạn có chắc chắn muốn xóa thông báo này không?')) {
            $.ajax({
                data: model,
                url: '/Open24Api/ApiHome/DeleteNotificationSoftware',
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {
                    if (result.res === true) {
                        AlertNotice(result.mess);
                        self.page(1);
                        FilterGrid();
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
    self.addnew = function () {
        window.location.href = "/AdminPage/Home/editNotificationSoftware";
    }
    //===============================
    // Click tìm kiếm
    //===============================

    self.SearchGrid = function (d, e) {
        if (e.keyCode === 13) {
            FilterGrid();
        }
    }
    //===============================
    // Phân trang 
    //===============================
    self.ClickPrevious = function () {
        if (self.page() > 1) {
            self.page(self.page() - 1);
            FilterGrid();
        }
    }

    self.ClickNext = function () {
        if (self.page() < self.pageCount()) {
            self.page(self.page() + 1);
           
            FilterGrid();
        }
    }

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

    $('#SelectedLimit').on('change', function () {
        FilterGrid();
    });
    FilterGrid();
    //===============================
    // Tìm kiếm gridview chung
    //===============================
    function FilterGrid() {
        var model = {
            Search: self.koSearch(),
            Page: self.page(),
            Limit: $('#SelectedLimit').val(),
        }
        $.ajax({
            url: '/Open24Api/ApiHome/SearchNotificationSoftware',
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: model,
            success: function (result) {
                if (result.res === true) {
                    self.ListNotification(result.DataSoure.Data);
                    self.PageItem(result.DataSoure.PageItem);
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
   


};
ko.applyBindings(new View());

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
//===============================
// Hiện thị trạng thái
//===============================
function ConvertTrangthai(value) {
    if (value === true) {
        return "kích hoạt"
    }
    else if (value === false) {
        return "Đóng"
    }
    else {
        return "";
    }
};