function Customer() {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.koSearch = ko.observable();
    self.ListContact = ko.observableArray();
    self.PageItem = ko.observable();
    self.pageCount = ko.observable();
    self.page = ko.observable();
    self.sort = ko.observable(1);
    self.Colum = ko.observable();


    //===============================
    // Click tìm kiếm
    //===============================

    self.SearchGrid = function (d, e) {
        if (e.keyCode === 13) {
            var object = {
                Search: self.koSearch(),
                Page: self.page(),
                Limit: $('#SelectedLimit').val(),
                Columname: self.Colum(),
                Sort: self.sort()
            };

            FilterGrid(object);
        }
    }
    //===============================
    // Phân trang 
    //===============================
    self.ClickPrevious = function () {
        if (self.page() > 1) {
            var object = {
                Search: self.koSearch(),
                Page: self.page() - 1,
                Limit: $('#SelectedLimit').val(),
                Columname: self.Colum(),
                Sort: self.sort()
            };

            FilterGrid(object);
        }
    }

    self.ClickNext = function () {
        if (self.page() < self.pageCount()) {
            var object = {
                Search: self.koSearch(),
                Page: self.page() + 1,
                Limit: $('#SelectedLimit').val(),
                Columname: self.Colum(),
                Sort: self.sort()
            };
            FilterGrid(object);
        }
    }

    self.netPageKeyup = function (d, e) {
        if (e.keyCode === 13) {
            if (self.page() > self.pageCount()
                || self.pageCount() === 1
                || !$.isNumeric(self.page())) {
                self.page(1);
            }
            var object = {
                Search: self.koSearch(),
                Page: self.page(),
                Limit: $('#SelectedLimit').val(),
                Columname: self.Colum(),
                Sort: self.sort()
            };
            FilterGrid(object);

        }
    }

    $('#SelectedLimit').on('change', function () {
        self.page(1);
        var object = {
            Search: self.koSearch(),
            Page: self.page(),
            Limit: $('#SelectedLimit').val(),
            Columname: self.Colum(),
            Sort: self.sort()
        };

        FilterGrid(object);
    });


    $('#example thead tr').on('click', 'th', function ()
    {

        $("#iconSort").remove();
        if (self.sort() === 0) {
            self.sort(1);
            this.innerHTML += " <i id='iconSort' class='fa fa-caret-down pull-right' aria-hidden='true'></i>";
        }
        else {
            self.sort(0);
            this.innerHTML += " <i id='iconSort' class='fa fa-caret-up pull-right' aria-hidden='true'></i>";
        }
        console.log($(this).data("id"));
        self.Colum($(this).data("id"));
        var object = {
            Search: self.koSearch(),
            Page: self.page(),
            Limit: $('#SelectedLimit').val(),
            Columname: self.Colum(),
            Sort: self.sort()
        };

        FilterGrid(object);

    });

    //===============================
    // Tìm kiếm gridview chung
    //===============================
    function FilterGrid(model) {
        $.ajax({
            url: '/Open24Api/ApiHome/SearchContact',
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
                    alert(result.mess);
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi.");
            }
        });
    }
    //===============================
    // Load dữ liệu lúc vào form
    //===============================
    function GetValueContact() {
        $.ajax({
            url: '/Open24Api/ApiHome/GetContact',
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
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
            error: function (result) {
                exception(result);
            }
        });
    }
    GetValueContact();


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
    if (value === true) {
        return "Hiện thị"
    }
    else if (value === false) {
        return "Ẩn"
    }
    else {
        return "";
    }
};