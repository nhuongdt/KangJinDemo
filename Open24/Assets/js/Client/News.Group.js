
var groupNews = function () {
    var self = this;
    self.pagenow = ko.observable(1);
    //===============================
    // Khai báo chung
    //===============================
    self.LisstgroupNews = ko.observableArray();
    self.listNews = ko.observableArray();
    self.categoryName = ko.observable();
    self.pageNow = ko.observable(1);
    self.pageCount = ko.observable();


    //===============================
    // Phân trang
    //===============================
    self.nextPage = function () {
        self.pageNow(self.pageNow() + 1);
        LoadPage(self.pageNow());
    }
    //===============================
    // Load dữ liệu khi vào form
    //===============================
    function Loadform() {
        $.ajax({
            url: "/Open24Api/ApiPost/GetNewDate",
            type: 'GET',
            async: true,
            contentType: 'application/json',
            success: function (result) {
                if (result.res === true) {
                    self.listNews(result.DataSoure);
                } else {
                    console.log(result.mess);
                }
            },
            error: function (result) {
                console.log(result);
            }
        });
    };

    Loadform();

    LoadPage(1);
    function LoadPage(page) {
        $.ajax({
            url: "/Open24Api/ApiPost/GetCategory?CategoryId=" + $('#CategoryId').val() + "&page=" + page,
            type: 'GET',
            async: true,
            contentType: 'application/json',
            success: function (result) {
                if (result.res === true) {
                    if (page === 1) {
                        self.categoryName(result.DataSoure.category);
                        self.LisstgroupNews(result.DataSoure.Data);
                    }
                    else {
                        var itemleft = self.LisstgroupNews();
                        itemleft.push.apply(itemleft, result.DataSoure.Data);
                        self.LisstgroupNews(itemleft);
                    }
                    self.pageCount(result.DataSoure.countPage);
                    if (self.pageCount() < 2 || self.pageNow() >= self.pageCount()) {
                        $('#Addnew').hide();
                    }
                } else {
                    console.log(result.mess);
                }
            },
            error: function (result) {
                console.log(result);
            }
        });
    }
}
ko.applyBindings(new groupNews());


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