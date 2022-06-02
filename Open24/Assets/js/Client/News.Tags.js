
    var NewsTags = function () {
        var self = this;
        //===============================
        // Khai báo chung
        //===============================
        self.LisstNews = ko.observableArray();
        self.LisstNewsDate = ko.observableArray();
        self.pageNow = ko.observable(1);
        self.pageCount = ko.observable(0);
        self.nextPage = function ()
        {
           
            self.pageNow(self.pageNow() + 1);
            LoadPage(self.pageNow());
        }
        //===============================
        // Load dữ liệu người dùng
        //===============================
        function Loadform() {
            $.getJSON("/Open24Api/ApiPost/GetNewDate", function (result) {
                    if (result.res === true) {
                        self.LisstNewsDate(result.DataSoure);
                    } else {
                        console.log(result.mess);
                    }
            });
            LoadPage(1);
        };

        Loadform();
        function LoadPage(page)
        {
            $.getJSON('/Open24Api/ApiPost/GetArticleByTag?tagId=' + window.sessionStorage.getItem("tagId")+'&page=' + page, function (result) {
                if (result.res === true) {
                    if (page === 1) {
                        self.LisstNews(result.DataSoure.Data);
                    }
                    else {
                        var itemleft = self.LisstNews();
                        itemleft.push.apply(itemleft, result.DataSoure.Data);
                        self.LisstNews(itemleft);
                    }
                    self.pageCount(result.DataSoure.countPage);
                    if (self.pageCount() < 2 || self.pageNow() >= self.pageCount()) {
                        $('#Addnew').hide();
                    }
                } else {
                    console.log(result.mess);
                }
            });
        }
    
    }
ko.applyBindings(new NewsTags());
//===============================
// Hiện thị Date
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
// Hiện thị Datetime
//===============================
function ConvertDatetime(config) {
    if (config === undefined
        || config === null
        || config.replace(/\s+/g, '') === "") {
        return "";
    }
    else {
        var a = moment(config).format('YYYY-MM-DD HH:mm:ss');
        return a;
    }
}