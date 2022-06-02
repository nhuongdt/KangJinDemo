

var ClientCustomer = function () {
    var self = this;
    self.leftCustomer = ko.observableArray();
    self.rightCustomer = ko.observableArray();
    self.ListBusiness = ko.observableArray();
    self.ListAdress = ko.observableArray();
    self.ShowAdress = ko.observable("Khu vực");
    self.ValueAdress = ko.observable();
    self.Showbusiness = ko.observable("Chọn loại hình kinh doanh");
    self.Valuebusiness = ko.observable();
    self.pageCount = ko.observable(0);
    self.page = ko.observable(0);
    self.search = ko.observable("");
    //===============================
    // Select combobox
    //===============================
    self.selectAdress = function (value) {
        self.ShowAdress(value.TEN);
        self.ValueAdress(value.ID);
        self.page(0);
        SearFilter();
        $('.list-kv').css('display', 'none');
    }

    self.selectbusiness = function (value) {
        self.Showbusiness(value.TEN);
        self.Valuebusiness(value.ID);
        self.page(0);
        SearFilter();
        $('.list-kv').css('display', 'none');
    }
    //===============================
    // tìm kiếm 
    //===============================
    self.SearchCustomer = function (d, e) {
        if (e.keyCode === 13) {
            self.page(0);
            SearFilter();
        }
    }
    //===============================
    // Tải thêm bài viết
    //===============================
    self.nextPage = function () {
        self.page(self.page() + 1);
        SearFilter(true);

    }
    //===============================
    // Call search server
    //===============================
    function SearFilter(isnext) {
        if (self.page() <= self.pageCount()) {
            var url = '/Open24Api/ApiCustomer/SearchFilter?Search=' + self.search() + '&Adress=' + self.ValueAdress() + '&business=' + self.Valuebusiness() + '&page=' + self.page();
            $.ajax({
                url: url,
                type: 'GET',
                async: true,
                cache: false,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {
                    if (result.res === true) {
                        if (!result.DataSoure.IsShow) {
                            $('#Addnew').hide();
                            self.leftCustomer(result.DataSoure.left);
                            self.rightCustomer(result.DataSoure.right);
                            self.pageCount(result.DataSoure.pageCount);
                        }
                        else {
                            if (isnext) {
                                var itemleft = self.leftCustomer();
                                itemleft.push.apply(itemleft, result.DataSoure.left);
                                var itemright = self.rightCustomer();
                                itemright.push.apply(itemright, result.DataSoure.right)
                                self.leftCustomer(itemleft);
                                self.rightCustomer(itemright);
                                self.pageCount(result.DataSoure.pageCount);
                            }
                            else {
                                self.leftCustomer(result.DataSoure.left);
                                self.rightCustomer(result.DataSoure.right);
                                self.pageCount(result.DataSoure.pageCount);
                            }
                            if (self.pageCount() - self.page() === 1) {
                                $('#Addnew').hide();
                            }
                            else {
                                $('#Addnew').show();
                            }
                        }
                    }
                    else {
                        console.log(result.mess);
                    }
                },
                error: function (result) {
                    console.log(result);
                }
            });
        }
    }

    //===============================
    // Load dữ liệu lúc vào form
    //===============================
    function GetValueCustomer() {
        $.ajax({
            url: '/Open24Api/ApiCustomer/GetClient',
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                if (result.res === true) {
                    self.leftCustomer(result.DataSoure.left);
                    self.rightCustomer(result.DataSoure.right);
                    self.pageCount(result.DataSoure.pageCount);
                    if (self.pageCount() <= 1) {
                        $('#Addnew').hide();
                    }
                }
                else {
                    console.log(result.mess);
                }
            },
            error: function (result) {
                console.log(result);
            }
        });
    }
    GetValueCustomer();
    function GetValueCombobox() {
        $.ajax({
            url: '/Open24Api/ApiCustomer/GetCombobxforSearch',
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                if (result.res === true) {
                    self.ListBusiness(result.DataSoure.DataNN);
                    self.ListAdress(result.DataSoure.DataTT);
                }
                else {
                    console.log(result.mess);
                }
            },
            error: function (result) {
                console.log(result);
            }
        });
    }

    GetValueCombobox();
}
ko.applyBindings(new ClientCustomer());


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