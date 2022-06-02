var vmNews = new Vue({
    el: '#renderNews',
    data: {
        curentpage: {
            text: '',
            adress: '',
            business: '',
            data: [],
            page: 0,
            ishow: true
        },
        comboboxcut: {
            adresstext: 'Khu vực',
            businesstext: 'Chọn loại hình kinh doanh',
            adress: [],
            business: []
        },
        listcurentolbar: [],
        data: {
            name: '',
            note: '',
            discription: '',
            createdate: '',
            createby: ''
        },
        groupnewsid: null
    },
    methods: {

        GetCustomer: function (filter = true) {
            var self = this;
            var pageold = self.curentpage.page;
            if (filter) {
                self.curentpage.page = 0;
            }
            var url = '/Open24Api/ApiCustomer/SearchformFilter?Search=' + self.curentpage.text + '&Adress=' + self.curentpage.adress + '&business=' + self.curentpage.business + '&page=' + self.curentpage.page;
            $.ajax({
                url: url,
                type: 'GET',
                async: true,
                cache: false,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {
                    if (result.res === true) {
                        result.DataSoure.data.map(function (item) {
                            item["CreatDate"] = localValidate.converToTimeview(item["CreatDate"]);
                        });
                        if (filter) {
                            self.curentpage.data = [];
                        }
                        var itemlist = self.curentpage.data;
                        itemlist.push.apply(itemlist, result.DataSoure.data);
                        self.curentpage.data = itemlist;
                        self.curentpage.ishow = result.DataSoure.isshow;
                    }
                    else {
                        console.log(result.mess);
                        self.curentpage.page = pageold;
                    }
                },
                error: function (result) {
                    self.curentpage.page = pageold;
                    console.log(result);
                }
            });
        },
        NextPageCustomer: function () {
            var self = this;
            self.curentpage.page += 1;
            self.GetCustomer(false);
        },
        KeymonitorCustomer: function (event) {
            if (event.key == "Enter") {
                this.GetCustomer();
            }
        },
        GetValueCombobox: function () {
            var self = this;
            $.ajax({
                url: '/Open24Api/ApiCustomer/GetCombobxforSearch',
                type: 'GET',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {
                    if (result.res === true) {

                        self.comboboxcut.adress = result.DataSoure.DataTT;
                        self.comboboxcut.business = result.DataSoure.DataNN;
                    }
                    else {
                        console.log(result.mess);
                    }
                },
                error: function (result) {
                    console.log(result);
                }
            });
        },
        SelectAdress: function (item) {
            this.curentpage.adress = item.ID;
            this.comboboxcut.adresstext = item.TEN;
            this.GetCustomer();
        },
        SelectBusiness: function (item) {
            this.curentpage.business = item.ID;
            this.comboboxcut.businesstext = item.TEN;
            this.GetCustomer();
        },
        getDetailCustomer: function (id) {
            var self = this;
            $.ajax({
                url: "/Open24Api/ApiCustomer/GetCustomerNewDate",
                type: 'GET',
                async: true,
                dataType: 'json',
                contentType: 'application/json',
                success: function (result) {
                    if (result.res === true) {
                        result.DataSoure.map(function (item) {
                            item["CreatedDate"] = localValidate.converToTimeview(item["CreatedDate"]);
                        });
                        self.listcurentolbar = result.DataSoure;
                    }
                    else {
                        console.log(result.mess);
                    }
                },
                error: function (result) {
                    console.log(result);
                }
            });
            $.ajax({
                url: "/Open24Api/ApiCustomer/GetViewDetail/" + id,
                type: 'GET',
                async: true,
                dataType: 'json',
                contentType: 'application/json',
                success: function (result) {
                    if (result.res === true) {
                        self.data = {
                            name: result.DataSoure.Name,
                            note: result.DataSoure.Note,
                            discription: result.DataSoure.Description,
                            createby: result.DataSoure.CreatedBy,
                            createdate: localValidate.converToTimeview(result.DataSoure.CreatedDate)
                        };
                    }
                    else {
                        console.log(result.mess);
                    }
                },
                error: function (result) {
                    console.log(result);
                }
            });
        },
        pageprev: function () {
            if (this.curentpage.page > 0) {
                this.curentpage.page -= 1;
            }
        },

        GetToolbar: function () {
            var self = this;
            $.getJSON("/Open24Api/ApiPost/GetNewsView", function (result) {
                if (result.res === true) {
                     result.DataSoure.map(function (item) {
                         item["CreatDate"] = localValidate.converToTimeview(item["CreatDate"]);
                     });
                    self.listcurentolbar = result.DataSoure;
                    
                } else {
                    console.log(result.mess);
                }
            });
        },
        Getnews: function () {
            var self = this;
            $.getJSON('/Open24Api/ApiPost/Getpage?page=' + self.curentpage.page, function (result) {
                if (result.res === true) {
                    result.DataSoure.Data.map(function (item) {
                        item["CreatDate"] = localValidate.converToTimeview(item["CreatDate"]);
                    });
                    var itemleft = self.curentpage.data;
                    itemleft.push.apply(itemleft, result.DataSoure.Data);
                    self.curentpage.data = itemleft;
                    self.curentpage.ishow = result.DataSoure.iShow;
                } else {
                    self.pageprev();
                    console.log(result.mess);
                }
            });
        },
        NextPageNews: function () {
            this.curentpage.page += 1;
            this.Getnews();
        },
        SetGroupNewsId: function (id) {
            this.groupnewsid = id;
            this.GetGroupNews();
            this.GetNewsDate();
        },
        NextPageGroupNews: function () {
            this.curentpage.page += 1;
            this.GetGroupNews();
        },
        GetGroupNews: function () {
            var self = this;
            $.ajax({
                url: "/Open24Api/ApiPost/GetCategory?CategoryId=" + self.groupnewsid + "&page=" + self.curentpage.page,
                type: 'GET',
                async: true,
                contentType: 'application/json',
                success: function (result) {
                    if (result.res === true) {
                        result.DataSoure.Data.map(function (item) {
                            item["CreatDate"] = localValidate.converToTimeview(item["CreatDate"]);
                        });
                        var itemleft = self.curentpage.data;
                        itemleft.push.apply(itemleft, result.DataSoure.Data);
                        self.curentpage.data = itemleft;
                        self.curentpage.ishow = result.DataSoure.iShow;
                        self.curentpage.text = result.DataSoure.category;
                    } else {
                        self.pageprev();
                        console.log(result.mess);
                    }
                },
                error: function (result) {
                    self.pageprev();
                    console.log(result);
                }
            });
        },
        GetNewsDate: function () {
            var self = this;
            $.ajax({
                url: "/Open24Api/ApiPost/GetNewDate",
                type: 'GET',
                async: true,
                contentType: 'application/json',
                success: function (result) {
                    if (result.res === true) {
                        result.DataSoure.Data.map(function (item) {
                            item["CreatedDate"] = localValidate.converToTimeview(item["CreatedDate"]);
                        });
                        self.listcurentolbar = result.DataSoure;
                    } else {
                        console.log(result.mess);
                    }
                },
                error: function (result) {
                    console.log(result);
                }
            });
        },
        SetTagNewsId: function (id) {
            this.groupnewsid = id;
            this.GetTagNews();
            this.GetNewsDate();
        },
        GetTagNews: function () {
            var self = this;
            $.getJSON('/Open24Api/ApiPost/GetArticleByTag?tagId=' + self.groupnewsid + '&page=' + self.curentpage.page, function (result) {
                if (result.res === true) {
                    result.DataSoure.Data.map(function (item) {
                        item["CreatDate"] = localValidate.converToTimeview(item["CreatDate"]);
                    });
                    var itemleft = self.curentpage.data;
                    itemleft.push.apply(itemleft, result.DataSoure.Data);
                    self.curentpage.data = itemleft;
                    self.curentpage.ishow = result.DataSoure.iShow;

                } else {
                    self.pageprev();
                    console.log(result.mess);
                }
            });
        }
    },
    created: function () {
        let self = this;
        var idCustomer = $("#customerID").val();
        if ($("#op_type").val() == "Tintuc") {
            self.GetToolbar();
            self.Getnews();
        }
        if ($("#op_type").val() == "Khachhang") {
            self.GetCustomer();
            self.GetValueCombobox();
        }

        if ($("#op_type").val() == "KhachhangChitiet") {
            self.GetCustomer();
            self.GetValueCombobox();
            self.getDetailCustomer(idCustomer);
        }

    }
})