var vmNewspaper = new Vue({
    el: '#Newspaper',
    data: {
        modelSearch: {
            pageItem: "",
            pageCount: 1,
            page: 1,
            text: "",
            limit: null,
            groupId: null,
            data: []
        },
        news: {
            ID:0,
            TenBaiViet: "",
            Anh: "",
            NoiDung: "",
            Mota: "",
            TrangThai: true,
            MetaTitle: "",
            MetaDescriptions: "",
            ID_NhomBaiViet: null,
            Tags: "",
            NgayDangBai: null,
            IsLichHen: false,
            Link: "/bai-viet-moi",
            IsNews: true,
            TenNhom:""
        },
        recruitment: {
            ID: 0,
            TieuDe: "",
            Mota: "",
            TrangThai: 1,
            MetaTitle: "",
            MetaDescriptions: "",
            ID_NhomBaiViet: null,
            Tags: "",
            NgayDangBai: null,
            IsLichHen: false,
            Link: "/bai-viet-moi",
            IsNews: true,
            TenNhom: "",
            DiaChi: "",
            Soluong: "",
            MucLuong: "",
            Tungay: "",
            Denngay: "",
            MaTinhThanh:""
        },
        dataResults: [],
        LitsTinhThanh: [],
        dataNewsGroup: [],
        titileNewsGroup: "Thêm mới",
        UrlSearchPage: "/SsoftApi/News/SearchGrid",
        IsLoadCombobox: 0,
        IsPageNews: true,
        IsChangeMetaTitle: false,
        Namemeta:"Chỉnh sửa"
        
    },
    methods: {
        //----- Tìm kiếm  phân trang----------//
        GetGroupCombobox: function () {
            var self = this;
            var url = "/SsoftApi/News/GetRecruitmentGroup";
            if (self.IsPageNews)
            {
                url = "/SsoftApi/News/GetNewsGroup";
            }
            $.getJSON(url, function (data) {
                if (data.res === true) {
                    self.dataNewsGroup = data.dataSoure;
                    
                } else {
                    console.log(data.mess);
                }
            });
        },
        GetData: function () {
            var self = this;
            self.modelSearch.data = [];
            var url = "/SsoftApi/ApiRecruitment/SearchGrid";
            if (self.IsPageNews) {
                url = "/SsoftApi/News/SearchGrid";
            }
            var model = self.modelSearch;
            $.ajax({
                data: model,
                url: url,
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (item) {
                    if (item.res === true) {
                        self.modelSearch = item.dataSoure;
                        self.dataResults = item.dataSoure.data;
                    }
                    else {
                        self.messageError(item.mess);
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
            if (this.modelSearch.page < this.modelSearch.pageCount ) {
                this.modelSearch.page = this.modelSearch.page + 1;
                this.GetData();
            }
        },
        //----- Nhóm bài viết----------//
        EditGroupNew: function (item) {
            vmGroupNewspaper.Update(item);
        },
        RemoveGroupNew: function (model) {
            vmGroupNewspaper.remove(model);
        },
        AddGroupNew: function () {
            vmGroupNewspaper.Insert($('#IDTypeNews').val());
        },
        //------ Tin tức----------//
        ClickEdit: function (ID) {
            window.location.href = "/Admin/News/EditNews?id=" + ID;
        },
        GetResultEdit: function (id) {
            var self = this;
            self.news.IsNews = false;
            self.titileNewsGroup = "Cập nhật";
            $.getJSON("/SsoftApi/News/GetEditNews?id=" + id, function (data) {
                if (data.res === true) {
                    self.news = data.dataSoure;
                    if (self.news.IsLichHen && self.news.NgayDangBai != null && self.news.NgayDangBai !== undefined) {
                        $('#DatLichhen').toggle();
                        $('.hen-thoi-gian').toggle();
                        var date = moment(self.news.NgayDangBai).format('DD/MM/YYYY hh:mm');
                        $('#myDatepicker2').find("input").val();
                    }
                    if (!localValidate.CheckNull(self.news.Anh)) {
                        $('.content-upload-img').hide();
                        $('.btn-upload-img-news').show();
                        $('#blah').attr('src', self.news.Anh)
                            .width("100%")
                            .height("100%").show();
                    }
                    localValidate.sleep(1000).then(() => {

                        CKEDITOR.instances['txtContentNews'].setData(self.news.NoiDung);
                    });
                } else {
                    AlertError(data.mess);
                }
            });
        },
        ClickRemove: function (id) {
            var self = this;
            if (confirm('Bạn có chắc chắn muốn xóa bài viết này không?')) {
                $.getJSON("/SsoftApi/News/RemoveNews?id=" + id, function (result) {
                         if (result.res === true) {
                             AlertSuccess(result.mess);
                             self.GetData();
                            }
                            else {
                                AlertError(result.mess);
                            }
                    
                });
            }
        },
        SaveNews: function () {
            var self = this;
                self.news.NoiDung = CKEDITOR.instances['txtContentNews'].getData();

            var fileUpload = $("#imageUploadForm").get(0);
            var files = fileUpload.files;
            var fileData = new FormData();
            if (files.length <= 0 && self.news.IsNews === true) {
                AlertError("Vui lòng chọn ảnh bài viết");
                return;
            }
            for (var i = 0; i < files.length; i++) {
                fileData.append(files[i].name, files[i]);
            }
            if (localValidate.CheckNull(self.news.TenBaiViet)) {
                AlertError("Vui lòng nhập tiêu đề bài viết");
            }
            else if (localValidate.CheckNull(self.news.Mota)) {
                AlertError("Vui lòng nhập mô tả bài viết");
            }
            else if (localValidate.CheckNull(self.news.NoiDung)) {
                AlertError("Vui lòng nhập nội dung bài viết");
            }
            else if (self.news.ID_NhomBaiViet === null) {
                AlertError("Vui lòng chọn nhóm bài viêt");
            }
            else {
                self.news.Tags = "";
                $('#tags_1_tagsinput .tag span').each(function () {
                    self.news.Tags += $(this).text().trim() + ",";
                });
                $.ajax({
                    data: fileData,
                    url: '/SsoftApi/News/UploadImages',
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    success: function (result) {
                        if (result.res === true) {
                            self.news.Anh = result.dataSoure;
                            var model = self.news;
                            self.EditNews(model);

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
        },
        EditNews: function (model) {
            $.ajax({
                data: model,
                url: '/SsoftApi/News/EditNews',
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {

                    if (result.res === true) {
                        AlertSuccess(result.mess);
                        location.href = "/Admin/News/News";
                    }
                    else {
                        AlertError(result.mess);
                    }
                }
            });
        },
        ChangeNewsTitle: function () {
            
            if (!this.IsChangeMetaTitle) {
                this.news.MetaTitle = this.news.TenBaiViet;
                this.news.MetaDescriptions = this.news.Mota;
            }
        },
        changeEditMeta: function () {
            if (!this.IsChangeMetaTitle) {
                this.Namemeta="Hủy"
            }
            else {
                this.Namemeta = "Chỉnh sửa";
                this.news.MetaTitle = this.news.TenBaiViet;
                this.news.MetaDescriptions = this.news.Mota;
                this.recruitment.MetaTitle = this.recruitment.TieuDe;
                this.recruitment.MetaDescriptions = this.recruitment.TieuDe;
            }

            this.IsChangeMetaTitle = !this.IsChangeMetaTitle;
        },
        // ----- Tuyển dụng ----------//
        GetTinhThanhCombobox: function () {
            var self = this;
            $.getJSON("/SsoftApi/News/GetAllTinhThanh", function (data) {
                self.LitsTinhThanh = data;
                self.loadseletectcbb();
            });
        },
        SelectAdress: function (item) {
            item.IsSelect = !item.IsSelect;
        },
        SaveRecruitment: function () {
            var self = this;
            self.recruitment.MaTinhThanh = self.LitsTinhThanh.filter(o => o.IsSelect).map(function (i, e) {
                return i.Key;
            }).toString();
            self.recruitment.Mota = CKEDITOR.instances['txtContentNews'].getData();
            self.recruitment.MucLuong = $('#range').val();
            if (localValidate.CheckNull(self.recruitment.TieuDe)) {
                AlertError("Vui lòng nhập tiêu đề tuyển dụng");
            }
            else if (localValidate.CheckNull(self.recruitment.Mota)) {
                AlertError("Vui lòng nhập mô tả tuyển dụng");
            }
            else if (self.recruitment.ID_NhomBaiViet === null) {
                AlertError("Vui lòng chọn nhóm tuyển dụng");
            }
            else if (self.recruitment.Soluong === null || self.recruitment.Soluong=="") {
                AlertError("Vui lòng nhập số lượng tuyển dụng");
            }
            else if (localValidate.CheckNull(self.recruitment.MucLuong)) {
                AlertError("Vui lòng nhập số lượng tuyển dụng");
            }
            else if (localValidate.CheckNull(self.recruitment.MaTinhThanh)) {
                AlertError("Vui lòng chọn tỉnh thành tuyển dụng");
            }
            else if (localValidate.CheckNull(self.recruitment.Tungay) || localValidate.CheckNull(self.recruitment.Denngay)) {
                AlertError("Vui lòng nhập thời gian tuyển dụng");
            }
            else {
                self.recruitment.Tags = "";
                $('#tags_1_tagsinput .tag span').each(function () {
                    self.recruitment.Tags += $(this).text().trim() + ",";
                });
                var model = self.recruitment;
                $.ajax({
                    data: model,
                    url: '/SsoftApi/ApiRecruitment/EditApiRecruitment',
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    success: function (result) {

                        if (result.res === true) {
                            AlertSuccess(result.mess);
                            location.href = "/Admin/News/Recruitment";
                        }
                        else {
                            AlertError(result.mess);
                        }
                    }
                });
            }
        },
        ClickRemoveRecruitment: function (id) {
            var self = this;
            if (confirm('Bạn có chắc chắn muốn xóa bài tuyển dụng này không?')) {
                $.getJSON("/SsoftApi/ApiRecruitment/RemoveRecruitment?id=" + id, function (result) {
                    if (result.res === true) {
                        AlertSuccess(result.mess);
                        self.GetData();
                    }
                    else {
                        AlertError(result.mess);
                    }

                });
            }
        },
        ClickEditRecruitment: function(id) {
            window.location.href = "/Admin/News/EditRecruitment?id=" + id;
        },
        GetResultEditRecruitment: function (id) {
            var self = this;
            self.news.IsNews = false;
            self.titileNewsGroup = "Cập nhật";
            $.getJSON("/SsoftApi/ApiRecruitment/GetEditRecruitment?id=" + id, function (data) {
                if (data.res === true) {
                    self.recruitment = data.dataSoure;
                    self.recruitment.Tungay = localValidate.convertDateServer(self.recruitment.Tungay);
                    self.recruitment.Denngay = localValidate.convertDateServer(self.recruitment.Denngay);
                    $("#range").data("ionRangeSlider").update({
                        from: self.recruitment.MucLuong.split(';')[0],
                        to: self.recruitment.MucLuong.split(';')[1]
                    });
                    localValidate.sleep(500).then(() => {
                        CKEDITOR.instances['txtContentNews'].setData(self.recruitment.Mota);
                    });
                    $('#reservation').val(self.recruitment.Tungay + ' - ' + self.recruitment.Denngay);
                    self.loadseletectcbb();
                } else {
                    AlertError(data.mess);
                }
            });
        },
        loadseletectcbb: function () {
            var self = this;
            self.IsLoadCombobox += 1;
            if (self.IsLoadCombobox === 2) {
                self.IsLoadCombobox = 0;
                if (!localValidate.CheckNull(self.recruitment.MaTinhThanh)) {
                    var listtt = self.recruitment.MaTinhThanh.split(',');
                    self.LitsTinhThanh.filter(o => jQuery.inArray(o.Key, listtt) >= 0).forEach(function (element) {
                        element.IsSelect = true;
                    });
                }
            }
        },
        ChangeRecruitmentTitle: function () {

            if (!this.IsChangeMetaTitle) {
                this.recruitment.MetaTitle = this.recruitment.TieuDe;
                this.recruitment.MetaDescriptions = this.recruitment.TieuDe;
            }
        },
    }
});
$('#heard').on('change', function () {
    vmNewspaper.modelSearch.groupId = $(this).val();
    vmNewspaper.GetData();
});
$('#SelectedLimit').on('change', function () {
    vmNewspaper.modelSearch.limit = $(this).val();
    vmNewspaper.modelSearch.page =1;
    vmNewspaper.GetData();
});
$('body').on('NhomTinTucSuccess', function () {
    vmNewspaper.GetGroupCombobox();
    vmNewspaper.news.TenNhom = "";
    vmNewspaper.news.ID_NhomBaiViet = null;
    vmNewspaper.recruitment.TenNhom = "";
    vmNewspaper.recruitment.ID_NhomBaiViet = null;
});