function Customer() {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.koSearch = ko.observable();
    self.ListContact = ko.observableArray();
    self.PageItem = ko.observable();
    self.pageCount = ko.observable();
    self.page = ko.observable(1);
    self.sort = ko.observable(1);
    self.Colum = ko.observable();


    //===============================
    // Click tìm kiếm
    //===============================

    self.SearchGrid = function (d, e) {
        if (e.keyCode === 13) {
            FilterGrid();
        }
    };
    //===============================
    // Phân trang 
    //===============================
    self.ClickPrevious = function () {
        if (self.page() > 1) {
            self.page(self.page() - 1);
            FilterGrid();
        }
    };

    self.ClickNext = function () {
        if (self.page() < self.pageCount()) {
            self.page(self.page() + 1);
            FilterGrid();
        }
    };

    self.netPageKeyup = function (d, e) {
        if (e.keyCode === 13) {
            if (self.page() > self.pageCount()
                || self.pageCount() === 1
                || !$.isNumeric(self.page())) {
                self.page(1);
            }
            FilterGrid();

        }
    };

    $('#SelectedLimit').on('change', function () {
        self.page(1);
        FilterGrid();
    });

    $('#selectedStatus').on('change', function () {
        self.page(1);
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
            Columname: self.Colum(),
            Sort: self.sort(),
            TypeHsd: $('#selectedStatus').val()
        };
        $.ajax({
            url: '/Open24Api/ApiHome/SearchCustomerContactSales',
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: model,
            success: function (result) {
                if (result.res === true) {
                    self.ListContact(result.DataSoure.Data);
                    self.PageItem(result.DataSoure.PageItem);
                    self.pageCount(result.DataSoure.PageCount);
                    self.page(result.DataSoure.Page);
                }
                else {
                    AlertError(result.mess);
                }
            },
            error: function () {
                AlertError("Đã xảy ra lỗi.");
            }
        });
    }
    self.Id = ko.observable();
    self.changeStatus = function (item) {
        self.Id(item.ID);
        $('#selected-Status').val(item.Status);
        $('#myModal').modal('show');
    };
    self.saveStatus = function () {
        var model = {
            ID: self.Id(),
            Status: $('#selected-Status').val()
        };
        $.ajax({
            url: '/Open24Api/ApiHome/SaveStatusContact',
            type: 'POST',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: model,
            success: function (result) {
                AlertNotice(result.mess);
                if (result.res === true) {
                    $('#myModal').modal('hide');
                    self.page(1);
                    FilterGrid();
                }

            },
            error: function () {
                AlertError("Đã xảy ra lỗi.");
            }
        });
    };

}
ko.applyBindings(new Customer());

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
        var a = moment(config).format('DD/MM/YYYY hh:mm');
        return a;
    }
}
//===============================
// Hiện thị trạng thái
//===============================
function ConvertTrangthai(value) {
    if (value === true) {
        return "Hiện thị";
    }
    else if (value === false) {
        return "Ẩn";
    }
    else {
        return "";
    }
}
function ConvertType(value) {
    if (value === 1) {
        return "Đặt mua";
    }
    else if (value === 2) {
        return "Dùng thử";
    }
    else {
        return "";
    }
}
$(document).on('click', '.classSalesDevice', function () {
    var t = $(this).closest('tr').next(".tr-hide").css("display");
    if (t === "none") {
        $(".tr-hide").removeClass("block");
        $(this).closest('tr').next(".tr-hide").addClass("block");
    }
    else if (t === "block") {
        $(this).closest('tr').next(".tr-hide").removeClass("block");
    }
    else {
        $(".tr-hide").removeClass("block");
    }
});