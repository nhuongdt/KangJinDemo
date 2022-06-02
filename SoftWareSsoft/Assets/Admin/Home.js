var vmMain= new Vue({
    el: '#mainHome',
    data: {
        contact: {
        },
        recruitment: {
        },
          modelSearch: {
            pageItem: "",
            pageCount: 1,
            page: 1,
            text: "",
            limit: 20,
            groupId: null,
            TrangThais:[1,0]
        },
          modelGridSearch: {
              pageItem: "",
              pageCount: 1,
              page: 1,
              text: "",
              limit: null,
              groupId: null,
              data: []
          },
          dataResult: [],
          url:'/SsoftApi/ApiHome/SearchGridContact',
    },
    methods: {
        reloadUrl: function (key=1) {
            if (key === 1) {
                this.url = "/SsoftApi/ApiHome/SearchGridContact";
            }
            else if (key === 2)
            {
                this.url = "/SsoftApi/ApiRecruitment/SearchGridHoSoUngTuyen";
                this.modelSearch.TrangThais = [1, 2];
            }
        },
        GetData: function () {
            var self = this;
            self.contact = {};
            self.recruitment = {};
            $.ajax({
                data: self.modelSearch,
                url: self.url,
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (item) {
                    if (item.res === true) {
                        self.modelSearch = item.dataSoure;
                        self.dataResult = item.dataSoure.data;
                        $('.mail-content a').each(function () {
                            $(this).removeClass('contact-active');
                        });
                    }
                    else {
                        AlertError(item.mess);
                    }
                }
            });
        },
        keySearch: function (event) {
            if (event.key === "Enter") {
                this.GetData();
            }
        },
        ClickPrevious: function () {
            if (this.modelSearch.page > 1) {
                this.modelSearch.page = this.modelSearch.page - 1;
                this.GetData();
            }
        },
        ClickNext: function () {
            if (this.modelSearch.page < this.modelSearch.pageCount) {
                this.modelSearch.page = this.modelSearch.page + 1;
                this.GetData();
            }
        },
        checkedSetup: function (id) {
            if (this.modelSearch.TrangThais === null)
            {
                this.modelSearch.TrangThais = [];
            }
            if (this.modelSearch.TrangThais.some(o => o == id)) {
                this.modelSearch.TrangThais = this.modelSearch.TrangThais.filter(o => o != id);
            }
            else {
                this.modelSearch.TrangThais.push(id);
            }
            this.GetData();
        },
        UpdateRead: function (item, e) {
            if (item.TrangThai !== false) {
                $.getJSON("/SsoftApi/ApiHome/UpdateContactRead?id=" + item.ID, function (data) {
                    if (data.res === false) {
                        AlertError(item.mess);

                    }

                });
            }
            item.TrangThai = false;
            $('.mail-content a').each(function () {
                $(this).removeClass('contact-active');
            });
                $(e.currentTarget).addClass('contact-active');
           
            this.contact = item;
            

        },
        UpdateHoSoUngTuyen:function(item, e) {
            if (item.TrangThai !== false) {
                $.getJSON("/SsoftApi/ApiRecruitment/UpdateHoSoUngTuyen?id=" + item.ID, function (data) {
                    if (data.res === false) {
                        AlertError(item.mess);

                    }

                });
            }
            item.TrangThai = false;
            $('.mail-content a').each(function () {
                $(this).removeClass('contact-active');
            });
            $(e.currentTarget).addClass('contact-active');
            this.recruitment = item;
        },
        DeleteHoSoUngTuyen: function () {
            var self = this;
            if (self.recruitment === null || self.recruitment.ID === null || self.recruitment.ID === undefined)
            {
                AlertError("Chọn hồ sơ cần xóa");
            }
            else {
                if (confirm('Bạn có chắc chắn muốn xóa hồ sơ  này không?')) {
                    $.getJSON("/SsoftApi/ApiRecruitment/RemoveHoSoUngTuyen?id=" + self.recruitment.ID, function (data) {
                        if (data.res === false) {
                            AlertError(data.mess);
                        }
                        else {
                            AlertSuccess(data.mess);
                            self.GetData();
                        }

                    });
                }
            }
        },
        // ----- Menu ----------//
        keySearchMenu: function () {
            if (event.key === "Enter") {
                this.GetSearchMenu();
            }
        },
        ClickPreviousMenu: function () {
            if (this.modelGridSearch.page > 1) {
                this.modelGridSearch.page = this.modelGridSearch.page - 1;
                this.GetSearchMenu();
            }
        },
        ClickNextMenu: function () {
            if (this.modelGridSearch.page < this.modelGridSearch.pageCount) {
                this.modelGridSearch.page = this.modelGridSearch.page + 1;
                this.GetSearchMenu();
            }
        },
        GetSearchMenu: function () {
            var self = this;
            $.ajax({
                data: self.modelGridSearch,
                url: "/SsoftApi/ApiHome/GetListMenuMeta",
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (item) {
                    if (item.res === true) {
                        self.modelGridSearch = item.dataSoure;
                        self.dataResult = item.dataSoure.data;
                    }
                    else {
                        AlertError(item.mess);
                    }
                }
            });
        },
        InsertMenu: function () {
            vmPopupmenu.Insert();
        },
        UpdateMenu: function (item) {
            vmPopupmenu.Update(item);
        },
        RemoveMenu: function (item) {
            var self = this;
            if (confirm('Bạn có chắc chắn muốn xóa menu tags này không?')) {
                $.ajax({
                    data: item,
                    url: '/SsoftApi/ApiHome/RemoveMenu',
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (result) {
                        if (result.res === true) {
                            AlertSuccess(result.mess);
                            self.modelGridSearch.page = 1;
                            self.GetSearchMenu();

                        }
                        else {
                            AlertError(result.mess);
                        }
                    }
                });
            }
        }
    }
})
$('#SelectedLimit').on('change', function () {
    vmMain.modelGridSearch.limit = $(this).val();
    vmMain.GetSearchMenu();
});
var vmPopupmenu = new Vue({
    el: '#exampleModalMenu',
    data: {
        titile: "Thêm mới",
        menu: {
            ID: null,
            Title: '',
            DuongDan: '',
            Link: '',
            Description: '',
            KeyWord: '',
            ID_Loaimenu:null
        }
    },
    methods: {
        Insert: function () {
            this.titile = "Thêm mới menu tags";
            this.menu = {
                ID: null,
                Title: '',
                DuongDan: '',
                Link: '',
                Description: '',
                KeyWord: '',
                ID_Loaimenu: null,
                TrangThai:true,
            };
            $('#exampleModalMenu').modal('show');

        },
        Update: function (item) {
            this.titile = "Cập nhật menu tags";
            this.menu = {
                ID: item.ID,
                Title: item.Title,
                DuongDan: item.DuongDan,
                Link: item.Link,
                Description: item.Description,
                KeyWord: item.KeyWord,
                ID_Loaimenu: item.ID_Loaimenu,
                TrangThai: item.TrangThai
            };
            $('#exampleModalMenu').modal('show');
        },
        SaveMenu: function () {
            var self= this;
            if (localValidate.CheckNull(self.menu.DuongDan)) {
                AlertError("Vui lòng nhập tên menu");
            }
            else if (localValidate.CheckNull(self.menu.Link)) {
                AlertError("Vui lòng nhập nội link menu");
            }
            else {
                $.ajax({
                    data: self.menu,
                    url: '/SsoftApi/ApiHome/Savemenu',
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (result) {
                        if (result.res === true) {
                            $('#exampleModalMenu').modal('hide');
                            vmMain.modelGridSearch.page = 1;
                            vmMain.GetSearchMenu();

                        }
                        else {
                            AlertError(result.mess);
                        }
                    }
                });
            }
        }
    }
})
