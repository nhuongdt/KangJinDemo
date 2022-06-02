function Advertising() {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.koSearch = ko.observable(null);
    self.ListAdvertising = ko.observableArray();
    self.PageItem = ko.observable();
    self.pageCount = ko.observable();
    self.page = ko.observable(1);
    self.sort = ko.observable(1);
    self.Colum = ko.observable(null);
    self.TitlePopup = ko.observable();
    self.Title = ko.observable();
    self.pathImage = ko.observable();
    self.Status = ko.observable(false);
    self.ID = ko.observable();
    self.Link = ko.observable();
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
        self.page(1);
        FilterGrid();
    });

    //===============================
    // Tìm kiếm gridview chung
    //===============================
    function FilterGrid() {
        var model = {
            Search: self.koSearch(),
            Page: self.page(),
            Limit: $('#SelectedLimit').val(),
            Columname: null,
            Sort: null
        };

        $.ajax({
            url: '/Open24Api/ApiHome/SearchAdvertising',
            type: 'POST',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: model,
            success: function (result) {
                if (result.res === true) {
                    self.ListAdvertising(result.DataSoure.Data);
                    self.PageItem(result.DataSoure.PageItem);
                    self.pageCount(result.DataSoure.PageCount);
                    self.page(result.DataSoure.Page);
                }
                else {
                    alert(result.mess);
                }
            },
            error: function (evt) {
                AlertError("Đã xảy ra lỗi.");
            }
        });
    }
    self.Adnew = function () {
        self.ID(null);
        self.Link(null);
        self.TitlePopup("Thêm mới quảng cáo");
        self.Title('');
        self.pathImage('/Content/images/imgdemo.png');
        self.Status(false);
        $("#datetimepicker1").find("input").val('');
        $("#datetimepicker2").find("input").val('');
        $('#myModal').modal('show');

    }
    self.Edit = function (item) {
        self.TitlePopup("Cập nhật quảng cáo");
        self.ID(item.ID);
        self.Title(item.Title);
        self.Link(item.Link);
        self.pathImage(item.UrlImage);
        self.Status(item.Status);
        if (item.FromDate !== null && item.FromDate !== undefined)
            $("#datetimepicker1").find("input").val(moment(item.FromDate).format('MM/DD/YYYY'));
        else
            $("#datetimepicker1").find("input").val('');
        if (item.ToDate !== null && item.ToDate !== undefined)
            $("#datetimepicker2").find("input").val(moment(item.ToDate).format('MM/DD/YYYY'));
        else
            $("#datetimepicker2").find("input").val('');
        $('#myModal').modal('show');
    }
    self.Delete = function (model) {

        if (confirm('Bạn có chắc chắn muốn xóa quảng cáo này không?')) {

            $.ajax({
                url: '/Open24Api/ApiHome/DeleteAdvertising',
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: model,
                success: function (result) {
                    if (result.res === true) {
                        FilterGrid();
                        AlertNotice(result.mess);
                    }
                    else {
                        AlertError(result.mess);
                    }
                }
            });

        }
    }

    self.Save = function () {
        if (localValidate.CheckNull(self.Title())) {
            AlertError("Vui lòng nhập tiêu đề");
        }
        else if (localValidate.CheckNull(self.pathImage())) {
            AlertError("Vui lòng chọn ảnh quảng cáo");

        }
        else {
            var url = '';
            if (self.ID() === undefined || self.ID() === null) {
                url = '/Open24Api/ApiHome/InsertAdvertising';
            }
            else {
                url = '/Open24Api/ApiHome/UpdateAdvertising';

            }
            var model = {
                ID: self.ID(),
                Title: self.Title(),
                UrlImage: self.pathImage(),
                FromDate: $("#datetimepicker1").find("input").val(),
                ToDate: $("#datetimepicker2").find("input").val(),
                Status: self.Status(),
                Link: self.Link()
            };

            $.ajax({
                url: url,
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
                error: function (evt) {
                    AlertError("Đã xảy ra lỗi.");
                }
            });
        }
    }
    FilterGrid();
    return self;
};
var Advertisings = new Advertising();
ko.applyBindings( Advertisings);

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
        return "ẩn"
    }
    else {
        return "";
    }
};
$("#datetimepicker1").datetimepicker({
    format: "MM/DD/YYYY",
    minDate: new Date()
});
$("#datetimepicker2").datetimepicker({
    format: "MM/DD/YYYY",
    minDate: new Date()
});
$(document).ready(function () {
    $('#imageUploadForm').on('click', function (e) {
        e.preventDefault();
        var finder = new CKFinder();
        finder.selectActionFunction = function (url) {
            Advertisings.pathImage(url);
        };
        finder.popup();
    });
});