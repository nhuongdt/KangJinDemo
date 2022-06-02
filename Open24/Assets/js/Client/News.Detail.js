$(document).ready(function () {
    var viewDetail = function () {
        var self = this;
        self.LisstNewsDate = ko.observableArray();
        function load() {
            //danh sách bài viết trang tin tức
            $.getJSON("/Open24Api/ApiPost/GetNewDate", function (result) {
                if (result.res === true) {
                    self.LisstNewsDate(result.DataSoure);
                } else {
                    console.log(result.mess);
                }
            });
        }
        load();
    }
    ko.applyBindings(new viewDetail());
});
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