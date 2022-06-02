function Order() {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.koSearch = ko.observable(null);
    self.ListOrder = ko.observableArray();
    self.PageItem = ko.observable();
    self.pageCount = ko.observable();
    self.OrderDetail = ko.observableArray();
    self.OrderID = ko.observable();
    self.page = ko.observable(1);
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
                || self.page() < 1
                || !$.isNumeric(self.page())) {
                self.page(1);
            }
            FilterGrid();
        }
    }

    $('#SelectedLimit').on('change', function () {
        self.page(1);
        FilterGrid();
    });

    function FilterGrid() {
        var model ={
            Search: self.koSearch(),
            Page: self.page(),
            Limit: $('#SelectedLimit').val(),
            Columname: null,
            Sort: null
        };
        $.ajax({
            url: '/Open24Api/ApiProduct/OrderSearchGrid',
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: model,
            success: function (result) {
                if (result.res === true) {
                    self.ListOrder(result.DataSoure.Data);
                    self.PageItem(result.DataSoure.PageItem);
                    self.pageCount(result.DataSoure.PageCount);
                    self.page(result.DataSoure.Page);
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
    self.loadOrderDetail = function (id)
    {
        $.ajax({
            url: '/Open24Api/ApiProduct/GetDetailOrder?Id='+id,
            type: 'GET',
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result.res === true) {
                    self.OrderDetail(result.DataSoure);
                }
                else {
                    AlertError(result.mess);
                }
            }
        });
    }

    FilterGrid();
 
    self.UpdateOrder = function (item)
    {
        self.OrderID(item.ID);
        $('#selected-Status').val(item.status);
        $('#myModal').modal('show');
    }
    self.DeleteOrder = function (item) {

        if (confirm('Bạn có chắc chắn muốn xóa đơn hàng này không?')) {

            var model = {
                ID: item.ID
            }
            $.ajax({
                data: model,
                url: '/Open24Api/ApiProduct/DeleteOrder',
                type: 'POST',
                dataType: "json",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {
                    if (result.res === true) {
                        AlertNotice(result.mess);
                        FilterGrid();
                    }
                    else {
                        AlertError(result.mess);
                    }
                }
            });
        }
    }

    self.saveStatusOrder = function () {
        var model = {
            ID: self.OrderID,
            Status: parseInt($('#selected-Status').val())
        }
        $.ajax({
            data: model,
            url: '/Open24Api/ApiProduct/ChangeStatusOrder',
            type: 'POST',
            dataType: "json",
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                if (result.res === true) {
                    AlertNotice(result.mess);
                    FilterGrid();
                    $('#myModal').modal('hide');
                }
                else {
                    AlertError(result.mess);
                }
            }
        });
    }
    return self;
};
var Orders = new Order();
$(document).on('click', '.classSalesDevice', function () {
    var t = $(this).closest('tr').next(".tr-hide").css("display");
    if (t == "none") {
        Orders.loadOrderDetail($(this).closest('tr').attr('id'));
        $(".tr-hide").removeClass("block");
        $(this).closest('tr').next(".tr-hide").addClass("block");
    }
    else if (t == "block") {
        $(this).closest('tr').next(".tr-hide").removeClass("block");
    }
    else {
        $(".tr-hide").removeClass("block");
    }
});
ko.applyBindings(Orders);
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