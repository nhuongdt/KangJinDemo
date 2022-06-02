function Customer() {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.koSearch = ko.observable();
    self.ListCustomer = ko.observableArray();
    self.PageItem = ko.observable();
    self.pageCount = ko.observable();
    self.page = ko.observable();
    self.sort = ko.observable(0);
    self.Colum = ko.observable();

    //===============================
    // Cập nhật đối tác
    //===============================
    self.UpdateCustomer = function (model)
    {
        window.location.href = "/AdminPage/Customer/Create/" + model.ID;


    }
     //===============================
    // Xóa đối tác
    //===============================
    self.deleteCustomer = function (model)
    {
        if (confirm('Bạn có chắc chắn muốn xóa đối tác này không?')) {
            $.ajax({
                data: model,
                url: '/Open24Api/ApiCustomer/DeleteCustomer',
                type: 'POST',
                async: false,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {
                    if (result.res === true) {
                        AlertNotice(result.mess);
                        GetValueCustomer();
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

    //===============================
    // Click thêm mới đối tác
    //===============================
    self.AddCustomer = function ()
    {
        location.href = "/AdminPage/Customer/Create";
    }
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
    self.ClickPrevious = function ()
    {
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

    self.ClickNext = function ()
    {
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

    $('#sortName').click(function () {
        self.Colum(0);
        SortGrid(this);
    });
    $('#sortCity').click(function () {
        self.Colum(1);
        SortGrid(this);
    });

    $('#sortBussines').click(function () {
        self.Colum(2);
        SortGrid(this);
    });
    $('#sortPhone').click(function () {
        self.Colum(3);
        SortGrid(this);
    });

    $('#sortEmail').click(function () {
        self.Colum(4);
        SortGrid(this);
    });
    $('#sortCreatedate').click(function () {
        self.Colum(5);
        SortGrid(this);
    });
    $('#sortPrioritize').click(function () {
        self.Colum(6);
        SortGrid(this);
    });
    $('#sortStatus').click(function () {
        self.Colum(7);
        SortGrid(this);
    });

    function SortGrid($item)
    {
        $("#iconSort").remove();
        if (self.sort() === 0)
        {
            self.sort(1);
            $item.innerHTML += " <i id='iconSort' class='fa fa-caret-down pull-right' aria-hidden='true'></i>";
        }
        else
        {
            self.sort(0);
            $item.innerHTML += " <i id='iconSort' class='fa fa-caret-up pull-right' aria-hidden='true'></i>";
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

    //===============================
    // Tìm kiếm gridview chung
    //===============================
    function FilterGrid(model) {
        $.ajax({
            url: '/Open24Api/ApiCustomer/SearchGrid',
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: model,
            success: function (result) {
                if (result.res === true) {
                    self.ListCustomer(result.DataSoure.Data);
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
    function GetValueCustomer()
    {
        $.ajax({
            url: '/Open24Api/ApiCustomer/GetAll',
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                if (result.res === true)
                {
                    self.ListCustomer(result.DataSoure.Data);
                    self.PageItem(result.DataSoure.PageItem);
                    self.pageCount(result.DataSoure.PageCount);
                    self.page(result.DataSoure.Page);
                }
                else
                {
                    AlertError(result.mess);
                }
            },
            error: function (result) {
                exception(result);
            }
        });
    }
    GetValueCustomer();

   
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