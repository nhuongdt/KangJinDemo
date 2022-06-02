var MenuTags = function () {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.ListMenu = ko.observableArray();
    self.koName = ko.observable();
    self.koDescription = ko.observable();
    self.koTags = ko.observable();
    self.koId = ko.observable();
    self.koLink = ko.observable();
    self.koStatus = ko.observable();
    self.koTitle = ko.observable();

    self.koSearch = ko.observable();
    self.PageItem = ko.observable();
    self.pageCount = ko.observable();
    self.page = ko.observable(1);

    self.AddMenu = function () {
        self.koId(0);
        self.koName('');
        self.koDescription('');
        self.koTags('');
        self.koTitle('');
        self.koLink('');
        self.koStatus(true);
        $('#myModal').modal('show');
    }
    self.btnUpdate = function (value)
    {
        self.koId(value.ID);
        self.koName(value.Text);
        self.koDescription(value.Description);
        self.koTags(value.Tags);
        self.koTitle(value.Title);
        self.koLink(value.Link);
        self.koStatus(value.Status);
        $('#myModal').modal('show');
    }
    self.btndelete = function (model) {
        if (confirm('Bạn có chắc chắn muốn xóa bản ghi này không?')) {
            $.ajax({
                data: model,
                url: '/Open24Api/ApiHome/DeleteTagsMenu',
                type: 'POST',
                async: false,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {
                    if (result.res === true) {
                        AlertNotice(result.mess);
                        load();
                    }
                    else {
                        AlertError(result.mess);
                    }
                }
            });
        }
    }

    self.SaveUpdate = function ()
    {
       var model  = {
            ID: self.koId(),
            Tags: self.koTags(),
            Description: self.koDescription(),
            Status: self.koStatus(),
            Link: self.koLink(),
            Title: self.koTitle(),
            Text: self.koName()
        }
       $.ajax({
           data: model,
            url: '/Open24Api/ApiHome/UpdateTagsMenu',
            type: 'POST',
            async: false,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                if (result.res === true) { 
                    AlertNotice(result.mess);
                    $('#myModal').modal('hide');
                    load();
                }
                else {
                    AlertError(result.mess);
                }
            }
        });
    }
    function load()
    {
        var object = {
            Search: self.koSearch(),
            Page: self.page(),
            Limit: $('#SelectedLimit').val(),
        };
        FilterGrid(object);
    }
    load();

    //===============================
    // Click tìm kiếm
    //===============================

    self.SearchGrid = function (d, e) {
        if (e.keyCode === 13) {
            var object = {
                Search: self.koSearch(),
                Page: self.page(),
                Limit: $('#SelectedLimit').val(),
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
        };

        FilterGrid(object);
    });

    //===============================
    // Tìm kiếm gridview chung
    //===============================
    function FilterGrid(model) {
        $.ajax({
            url: '/Open24Api/ApiHome/SearchGridMenu',
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: model,
            success: function (result) {
                if (result.res === true) {
                    self.ListMenu(result.DataSoure.Data);
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
    return self;
};
var seltknockout = new MenuTags();
ko.applyBindings(seltknockout);