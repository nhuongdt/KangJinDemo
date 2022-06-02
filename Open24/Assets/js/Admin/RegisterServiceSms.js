function Customer() {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.koSearch = ko.observable();
    self.ListResult = ko.observableArray();
    self.PageItem = ko.observable();
    self.pageCount = ko.observable();
    self.page = ko.observable(1);
    self.sort = ko.observable(1);
    self.Colum = ko.observable();
    self.priceactived = ko.observable();

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
            self.page(self.page() - 1)
            FilterGrid();
        }
    }

    self.ClickNext = function () {
        if (self.page() < self.pageCount()) {
            self.page(self.page() + 1)
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
        self.page(1);
    });
    

    //===============================
    // Tìm kiếm gridview chung
    //===============================
    function FilterGrid() {
        var model = {
            Search: self.koSearch(),
            Page: self.page(),
            Limit: $('#SelectedLimit').val(),
        };

        $.ajax({
            url: '/Open24Api/ApiHome/SearchServiceSms',
            type: 'POST',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: model,
            success: function (result) {
                if (result.res === true) {
                    self.ListResult(result.DataSoure.Data);
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
    self.suppliersms = ko.observableArray();
    function loadsuppliersms() {
        $.ajax({
            url: '/Open24Api/ApiHome/GetSupplierSms',
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                if (result.res === true) {
                    self.suppliersms(result.DataSoure);

                }
                else {
                    alert(result.mess);
                }
            }
        });
    }
    loadsuppliersms();
    self.ID_ServiceSms = ko.observable();
    self.changeStatus = function (item) {
        self.ID_ServiceSms(item.ID);
        $('#selected-Status').val(item.Status);
        $('#selected-suppliersms').val(item.ID_SupplierSms);
        self.priceactived(item.Price);

        $('#myModal').modal('show');
        if (item.Status !== '2' && item.Status !== 2) {
            $('.pricea-ctived').show();
        }
        else {

            $('.pricea-ctived').hide();
        }
    };
    self.saveStatus = function () {
        var model = {
            ID: self.ID_ServiceSms(),
            ID_SupplierSms: $('#selected-suppliersms').val(),
            Status: $('#selected-Status').val(),
            Price: self.priceactived()
        };
        $.ajax({
            url: '/Open24Api/ApiHome/UpdateServiceSms',
            type: 'POST',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: model,
            success: function (result) {
                if (result.res === true) {
                    AlertNotice(result.mess);
                    FilterGrid(); $('#myModal').modal('hide');
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
    FilterGrid();

};
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
        var a = moment(config).format('DD/MM/YYYY');
        return a;
    }
}
//===============================
// Hiện thị trạng thái
//===============================
function ConvertTrangthai(value) {
    if (value === 0) {
        return "Hủy"
    }
    else if (value === 1) {
        return "Kích hoạt"
    }
    else {
        return "Chờ kích hoạt";
    }
};
$('#selected-Status').on('change', function () {

    if ($(this).val() !== '2' && $(this).val() !== 2) {
        $('.pricea-ctived').show();
    }
    else {

        $('.pricea-ctived').hide();
    }
});