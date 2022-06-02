var View = function () {
    var self = this;
    //Binding Article News
    self.dbkoTitleNews = ko.observable();
    self.dbkoImageNews = ko.observable();
    self.dbkoTagNews = ko.observable();
    self.dbkoSummaryNews = ko.observable();
    self.dbkoStatusNews = ko.observable(true);
    self.fileInput = ko.observable();
    self.GroupCateNews = ko.observableArray();
    //Binding Article Recuit
    self.dbkoTitleRecruit = ko.observable();
    self.dbkoImageRecruit = ko.observable();
    self.dbkoSalaryRecruit = ko.observable();
    self.dbkoAddressRecruit = ko.observable();
    self.dbkoExperienceRecruit = ko.observable();
    self.dbkoPositionRecruit = ko.observable();
    self.dbkoDegreeRecruit = ko.observable();
    self.dbkoTradesRecruit = ko.observable();
    self.dbkoExpirationDateRecruit = ko.observable();
    self.dbkoNumberOfRecruitsRecruit = ko.observable();
    self.dbkoSummaryRecruit = ko.observable();
    self.dbkoGenderRecruit = ko.observable(true);
    self.dbkoStatusRecruit = ko.observable(true);
    self.GroupCateRecruit = ko.observableArray();
    //Binding Article News Update
    self.valIDNews = ko.observable();
    self.valTitleNews = ko.observable();
    //self.valImageNews = ko.observable();
    self.valTagNews = ko.observable();
    self.valSummaryNews = ko.observable();
    self.chkStatusNews = ko.observable(true);
    self.GroupCateNewsUd = ko.observableArray();
    //Binding Article Recruit Update
    self.valIDRecruit = ko.observable();
    self.valTitleRecruit = ko.observable();
    self.valImageRecruit = ko.observable(); 
    self.valSalaryRecruit = ko.observable();
    self.valAddressRecruit = ko.observable();
    self.valExperienceRecruit = ko.observable();
    self.valPositionRecruit = ko.observable();
    self.valDegreeRecruit = ko.observable();
    self.valTradesRecruit = ko.observable();
    self.valExpirationDateRecruit = ko.observable();
    self.valNumberOfRecruitsRecruit = ko.observable();
    self.valSummaryRecruit = ko.observable();
    self.optGenderRecruitUp = ko.observable(true);
    self.chkStatusRecruit = ko.observable(true);
    self.GroupCateRecruitUp = ko.observableArray();
    //Load list articles
    self.ID = ko.observable();
    self.listArticle = ko.observableArray();
    //Load CategoryType
    self.GroupCateTypeNews = ko.observableArray();
    self.GroupCateTypeRecruit = ko.observableArray();
    //Insert CategoryType
    self.valCategoryNameNews = ko.observable();
    self.valCategoryTypeNameRecruit = ko.observable();
    self.fileInput = ko.observable();
    //phân trang
    self.pageSizes = [5, 25, 40];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();
    self.arrPagging = ko.observableArray();
    self.PageCount = ko.observable();
    self.TotalRecord = ko.observable(0);
    //nhóm bài viết
    self.groupArticle = ko.observableArray();
    //cập nhật ảnh form update article
    self.pathImage = ko.observable();
    self.koUrl = ko.observable();

    self.koPageItem = ko.observable();
    self.kopage = ko.observable();
    self.kopageCount = ko.observable();
    self.koSearch = ko.observable();
    self.checkDatetime = ko.observable(true);
    self.Url = ko.observable();
    self.nameNewsChanged = function () {

        self.Url(localValidate.ConvertUrl(self.dbkoTitleNews()));
    }
    self.NewsChanged = function () {

        self.Url(localValidate.ConvertUrl(self.valTitleNews()));
    }
    self.UrlChanged = function () {

        self.Url(localValidate.ConvertUrl(self.Url()));
    }
    //===============================
    // click Tìm kiếm gridview
    //===============================
    self.SearchGrid = function (d, e) {
        if (e.keyCode === 13) {
            FilterGrid();
        }
    }

    //===============================
    // Nhập trang
    //===============================
    self.nextPageKeyup = function (d, e) {
        if (e.keyCode === 13) {
            if (self.kopage() > self.kopageCount()
                || self.kopageCount() === 1
                || !$.isNumeric(self.kopage())) {
                self.kopage(1);
            }
            FilterGrid();

        }
    }
    //===============================
    // back trang
    //===============================
    self.koClickPrevious = function () {
        if (self.kopage() > 1) {
            self.kopage(self.kopage() - 1);

            FilterGrid();
        }
    }
    //===============================
    // Next trang
    //===============================
    self.koClickNext = function () {
        if (self.kopage() < self.kopageCount()) {
            self.kopage(self.kopage() + 1);
            FilterGrid();
        }
    }
    //===============================
    // select số bản ghi trang
    //===============================
    $('#SelectedLimit').on('change', function () {
        self.kopage(1);
        FilterGrid();
    });
    //===============================
    // Tìm kiếm gridview chung
    //===============================
    function FilterGrid() {
        var model = {
            Search: self.koSearch(),
            Page: self.kopage(),
            Limit: $('#SelectedLimit').val(),
            Columname: colum,
            Sort: sort
        };
        $.ajax({
            url: '/Open24Api/PostAPI/SearchCustomGrid',
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: model,
            success: function (result) {
                self.listArticle(result.Data);
                self.koPageItem(result.PageItem);
                self.kopage(result.Page);
                self.kopageCount(result.PageCount);
            },
            error: function () {
                alert("Đã xảy ra lỗi.");
            }
        });
    }
    //===============================
    // Sort grid
    //===============================
    var sort = 1;
    var colum = 2;
    $('#eventTitle').click(function () {
        colum = 1;
        OnloadSort(this);
    });
    $('#eventCreatdate').click(function () {
        colum = 2;
        OnloadSort(this);
    });
    $('#eventCreatby').click(function () {
        colum = 3;
        OnloadSort(this);
    });

    $('#eventCategory').click(function () {
        colum = 4;
        OnloadSort(this);
    });
    $('#eventView').click(function () {
        colum = 5;
        OnloadSort(this);
    });
    function OnloadSort($this) {
        $("#iconSort").remove();
        if (sort === 0) {
            sort = 1;
            $this.innerHTML += " <i id='iconSort' class='fa fa-caret-down pull-right' aria-hidden='true'></i>";
        }
        else {
            sort = 0;
            $this.innerHTML += " <i id='iconSort' class='fa fa-caret-up pull-right' aria-hidden='true'></i>";
        }
        FilterGrid();
    }
    //---------------------------------------------------------------------------------------------------

    function getAllArticle() {
        ajaxHelper('/Open24Api/PostAPI/' + "GetAllArticle", 'GET').done(function (data) {
            //hiển thị danh sách bài viết
            self.listArticle(data.Data);
            self.koPageItem(data.PageItem);
            self.kopage(data.Page);
            self.kopageCount(data.PageCount);
        });
    }
    getAllArticle();

    self.insertArticleNews = function () {
        $("#validateTitleNews").text("");
        $("#validateTagNews").text("");
        $("#validateSummaryNews").text("");
        $("#checkeditorValidate").text("");
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadForm").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadForm").files[i];
            formData.append("imageUploadForm", file);
        }

        var title = self.dbkoTitleNews();
        var image = self.dbkoImageNews();
        var tag = self.dbkoTagNews();
        var summary = self.dbkoSummaryNews();
        var status = self.dbkoStatusNews();
        var content = CKEDITOR.instances['txtContentNews'].getData();
        var category = $("#selCategoryNews").val();

        var vtitle = $("#valTitleNews").val();
        var vtag = $("#valTagNews").val();
        var vsummary = $("#valSummaryNews").val();
        if (totalFiles <= 0) {
            AlertError("Vui lòng chọn ảnh đại diện.");
        }
        else if (vtitle === "" || vtitle.length > 200) {
            AlertError("Tiêu đề bài viết không được để trống và có độ dài dưới 200 kí tự");
        }
        else if (self.Url() === undefined
            || self.Url() === null
            || self.Url().replace(/\s+/g, '') === "") {
            AlertError("Vui lòng nhập url bài viết.");
            return;
        }
        else if (validatespecialcharacters(vtitle)) {
            AlertError("Tiêu đề bài viết không được chứa ký tự đặc biệt.");
        }
        else if (vtag.length > 255) {
            AlertError("Độ dài quá giới hạn cho phép");
        }
        else if (content === '' || content.replace(/\s+/g, '') === '') {
            AlertError("Nội dung không được để trống.");

        }
        else if (vtitle !== "" || vtitle.length < 201 && vtag.length < 256 ) {
            $("#validateTitleNews").text("");
            $("#validateTagNews").text("");
            $("#validateSummaryNews").text("");
            $("#checkeditorValidate").text("");
            $.ajax({
                type: "POST",
                url: '/Open24Api/PostAPI/' + "ImageUploadFolder",
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (response) {
                    var model = {
                        Title: title,
                        Summary: summary,
                        UrlImage: response,
                        Content: content,
                        Tag: tag,
                        CategoryID: category,
                        CreateDate: getDate(),
                        UpdateDate: getDate(),
                        Url: self.Url(),
                        Status: status,
                        Gender: $('.checkedGhim input').is(':checked'),
                        DatePost: $('#datetimepicker').find("input").val()
                    };
                    $.ajax({
                        data: model,
                        url: '/Open24Api/PostAPI/' + "AddArticle",
                        type: 'POST',
                        async: true,
                        dataType: 'json',
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                        success: function (item) {
                            if (item.res) {
                                if (item.DataSoure.timeout) {
                                    AlertError("Phiên làm việc đã hết vui lòng tải lại trang");
                                }
                                else {
                                    AlertNotice('Thêm mới thành công.');
                                }
                                resetInsertArticleNews();
                                window.location.href = "/AdminPage/Post/";
                            }
                            else {
                                AlertError(item.mess);
                            }
                           
                        }
                    });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log('err');
                }
            });
        }
    }

    function resetInsertArticleNews() {
        $("#valTitleNews").val("");
        $("#imageUploadForm").val("");
        document.getElementById("blah").src = "";
        $("#valTagNews").val("");
        $("#valSummaryNews").val("");
        $("#checkeditorValidate").text("");
        CKEDITOR.instances['txtContentNews'].setData("");
    }

    function resetUpdateArticleNews() {
        $("#idTitleNews").val("");
        $("#imageUploadForm").val("");
        document.getElementById("blah").src = "";
        $("#idTagNews").val("");
        $("#checkeditorValidate").text("");
        $("#idSummaryNews").val("");
        CKEDITOR.instances['idContentNews'].setData("");
    }

    self.insertArticleRecruit = function () {
        var title = self.dbkoTitleRecruit();
        var image = self.dbkoImageRecruit();
        var salary = self.dbkoSalaryRecruit();
        var address = self.dbkoAddressRecruit();
        var experience = self.dbkoExperienceRecruit();
        var position = self.dbkoPositionRecruit();
        var degree = self.dbkoDegreeRecruit();
        var trades = self.dbkoTradesRecruit();
        var expirationDate = self.dbkoExpirationDateRecruit();
        var numberOfRecruits = self.dbkoNumberOfRecruitsRecruit();
        var summary = self.dbkoSummaryRecruit();
        var gender = self.dbkoGenderRecruit();
        var status = self.dbkoStatusRecruit();
        var content = CKEDITOR.instances['txtContentRecruit'].getData();
        var category = $("#selCategoryRecruit").val();

        var model = {
            Title: title,
            Summary: summary,
            UrlImage: image,
            Content: content,
            CategoryID: category,
            //CreatedBy: 1,
            //UpdatedBy: 1,
            CreateDate: getDate(),
            UpdateDate: getDate(),
            Status: status,
            Salary: salary,
            Address: address,
            Experience: experience,
            Position: position,
            Degree: degree,
            Trades: trades,
            NumberOfRecruits: numberOfRecruits,
            Gender: gender,
            ExpirationDate: expirationDate
        };
        $.ajax({
            data: model,
            url: '/Open24Api/PostAPI/' + "AddArticle",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                alert(item);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log('err');
            }
        })
    }
    self.btnUpdatePin = function (model) {
        $.ajax({
            url: '/Open24Api/PostAPI/UpdatePin',
            type: 'POST',
            data: model,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
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

    self.btnUpdateArticle = function () {
        //if ($("input[name='optradio']:checked").val()) {
        //var id = $("input[name='optradio']:checked").val();
        //var cateID = "";
        var a = "";
        ajaxHelper('/Open24Api/PostAPI/' + "GetCateIDArticleforUpdate?id=" + this.ID, 'GET').done(function (data) {
            a = JSON.stringify(data);
            b = JSON.parse(a);

            for (var key in b) {
                if (b.hasOwnProperty(key)) {
                    a = b[key];
                    //cateID = b[key].CategoryTypeID;
                }
            }
        });
        $('#myModal2').modal('show');
        var mydate = new Date(a.DatePost);
        console.log(mydate.toDateString());
        var $datepicker = $('#datetimepicker');
        if (a.Status == false) {
            if (a.DatePost !== undefined && a.DatePost !== null && a.DatePost.replace(/\s+/g, '') !== "" && mydate > new Date()) {
                $datepicker.datetimepicker();
                $datepicker.datetimepicker({ dateFormat: 'MM/DD/YYYY HH:mm' });
                var date = moment(a.DatePost).format('MM/DD/YYYY HH:mm');
                $datepicker.find("input").val(date);
                self.checkDatetime(true);
            }
            else {
                self.checkDatetime(false);
            }
        }
        else {
            self.checkDatetime(false);
        }
        $("#idIDNews").val(a.ID);
        $("#idTitleNews").val(a.Title);
        self.koUrl(a.Url.split("/")[3]);
        let aray = a.Url.split('.')[0].split('/')[2].split("-");
            aray.pop();
        self.Url(aray.join('-'));
        self.pathImage(a.UrlImage);
        $("#idCateTypeNews").val(a.CategoryID);
        $("#idTagNews").val(a.Tag);
        $("#idSummaryNews").val(a.Summary);
        CKEDITOR.instances['idContentNews'].setData(a.Content);
        self.chkStatusNews(a.Status);//$("input[name='optStatusNews']:checked").val(a.Status);//$("#txtExpirationDate").val(obj.expirationDate.split('T')[0]);
    }

    self.updateArticleNews = function () {
        $("#validateTitleNews").text("");
        $("#validateTagNews").text("");
        $("#validateSummaryNews").text("");
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadForm").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadForm").files[i];
            formData.append("imageUploadForm", file);
        }
        var vtitle = $("#idTitleNews").val();
        var vtag = $("#idTagNews").val();
        var vsummary = $("#idSummaryNews").val();

        if (totalFiles <= 0
            && (self.pathImage() === ''
                || self.pathImage() === null)) {
            AlertError("Vui lòng chọn ảnh đại diện.");
        }
        else if (vtitle === "" || vtitle.length > 200) {
            AlertError("Tiêu đề bài viết không được để trống và có độ dài dưới 200 kí tự");
        }
        else if (self.Url() === undefined
            || self.Url() === null
            || self.Url().replace(/\s+/g, '') === "") {
            AlertError("Vui lòng nhập url bài viết.");
            return;
        }
        else if (vtag.length > 255) {
            AlertError("Độ dài quá giới hạn cho phép");
        }
        else if (vsummary === "" || vsummary.length > 400) {
            AlertError("Tóm tắt bài viết không được để trống và có độ dài dưới 255 kí tự");
        }
        else if (vtitle !== "" || vtitle.length < 201 && vtag.length < 256 && vsummary !== "" || vsummary.length < 401) {
            $("#validateTitleNews").text("");
            $("#validateTagNews").text("");
            $("#validateSummaryNews").text("");
            $.ajax({
                type: "POST",
                url: '/Open24Api/PostAPI/' + "ImageUploadFolder",
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (response) {
                    //if (response !== "") {
                    var id = $("#idIDNews").val();
                    var title = $("#idTitleNews").val();//self.valTitleNews();
                    //var image = $("#idUrlImageNews").val();//self.valImageNews();
                    var tag = $("#idTagNews").val();//self.valTagNews();
                    var summary = $("#idSummaryNews").val();//self.valSummaryNews();
                    var status = self.chkStatusNews();//$("input[name='optStatusNews']:checked").val();
                    var content = CKEDITOR.instances['idContentNews'].getData();
                    var category = $("#idCateTypeNews").val();
                    var model = {
                        ID: id,
                        Title: title,
                        Summary: summary,
                        UrlImage: response,
                        Content: content,
                        Tag: tag,
                        CategoryID: category,
                        UpdateDate: getDate(),
                        Status: status,
                        Url: self.Url(),
                        DatePost: $('#datetimepicker').find("input").val()
                    };
                    $.ajax({
                        data: model,
                        url: '/Open24Api/PostAPI/' + "AddArticle",
                        type: 'POST',
                        async: true,
                        dataType: 'json',
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                        success: function (item) {
                            if (item.res) {
                                if (item.DataSoure.timeout) {
                                    AlertError("Phiên làm việc đã hết vui lòng tải lại trang");
                                }
                                else {
                                    AlertNotice("Cập nhật thành công.");
                                }
                                FilterGrid();
                                resetUpdateArticleNews();
                                $('#myModal2').modal('hide');
                            }
                            else {
                                AlertError(item.mess);
                            }
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            console.log('err');
                        }
                    })
                    //} else {
                    //    alert("Chưa chọn ảnh đại diện!");
                    //}
                    //error: function(error) {
                    //    alert("errror");
                    //}
                }
            });
        }
    }

    self.updateArticleRecruit = function () {
        var id = $("#idIDRecruit").val();
        var title = $("#idTitleRecruit").val();//self.valTitleNews();
        var image = $("#idImageRecruit").val();//self.valImageNews();
        var salary = $("#idSalaryRecruit").val();//self.valTagNews();
        var address = $("#idAddressRecruit").val();//self.valSummaryNews();
        var experience = $("#idExperienceRecruit").val();//self.chkStatusNews();
        var position = $("#idPositionRecruit").val();
        var degree = $("#idDegreeRecruit").val();
        var trades = $("#idTradesRecruit").val();
        var expirationDate = $("#idExpirationDateRecruit").val();
        var numberOfRecruits = $("#idNumberOfRecruitsRecruit").val();
        var summary = $("#idSummaryRecruit").val();
        var gender = $("input[name='optGenderRecruitUp']:checked").val();
        var status = $("input[name='optStatusRecruitUp']:checked").val();
        var content = CKEDITOR.instances['idContentRecruit'].getData();
        var category = $("#idCateTypeRecruit").val();

        var model = {
            ID: id,
            Title: title,
            UrlImage: image,
            Salary: salary,
            Address: address,
            Experience: experience,
            Position: position,
            Degree: degree,
            Trades: trades,
            ExpirationDate: expirationDate,
            NumberOfRecruits: numberOfRecruits,
            Summary: summary,
            Gender: gender,
            Status: status,
            Content: content,
            CategoryID: category,
            UpdatedBy: 1,
            UpdateDate: getDate()
        };
        $.ajax({
            data: model,
            url: '/Open24Api/PostAPI/' + "AddArticle",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                alert("Update successful!");
                getAllArticle();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log('err');
            }
        })
    }

    function getAllCategoriesNews() {
        $.ajax({
            url: "/Open24Api/ApiGroupPost/GetGroupchilden",
            type: 'GET',
            async: true,
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                if (result.res === true) {
                    self.GroupCateNews(result.DataSoure);
                    self.GroupCateNewsUd(result.DataSoure);
                }
                else {
                    alert(result.mess);
                }
            }
        });

    }
    getAllCategoriesNews();

    function getAllCategoriesRecruit() {
        ajaxHelper('/Open24Api/PostAPI/' + "GetAllCategories?cateTypeID=2", 'GET').done(function (data) {
            var a = JSON.stringify(data);
            self.GroupCateRecruit(JSON.parse(a));
            self.GroupCateRecruitUp(JSON.parse(a));
        });
    }
    getAllCategoriesRecruit();

    self.deleteArticle = function () {
        //if ($("input[name='optradio']:checked").val()) {
        //    var id = $("input[name='optradio']:checked").val();
        if (confirm('Bạn có chắc chắc chắn muốn xóa bài viết này không?')) {
            ajaxHelper('/Open24Api/PostAPI/' + "DeleteArticle?id=" + this.ID, 'GET').done(function (data) {
                //alert("Deleted!");
                getAllArticle();
            });
        } else {
            return;
        }
        //} else {
        //    alert("Select item!");
        //}
    }

    function getAllCategoryTypeNews() {
        ajaxHelper('/Open24Api/PostAPI/' + "GetAllCategoryType?postionID=1", 'GET').done(function (data) {
            var a = JSON.stringify(data);
            self.GroupCateTypeNews(JSON.parse(a));
        });
    }
    getAllCategoryTypeNews();

    function getAllCategoryTypeRecruit() {
        ajaxHelper('/Open24Api/PostAPI/' + "GetAllCategoryType?postionID=2", 'GET').done(function (data) {
            var a = JSON.stringify(data);
            self.GroupCateTypeRecruit(JSON.parse(a));
        });
    }
    getAllCategoryTypeRecruit();

    self.popupInsertCateTypeNews = function () {
        self.valCategoryNameNews("");
        $('#myModal5').modal('show');
    }

    self.popupInsertCateTypeRecruit = function () {
        $('#myModal1').modal('show');
    }

    self.insertCateTypeNews = function () {
        //var categoryTypeID = $("#selCateTypeNews").val();
        var categoryName = self.valCategoryNameNews();
        var cate = $("#valCateNews").val();
        if (cate === "") {
            $("#validateCateNews").text("Nhập thể loại");
        } else {
            var model = {
                //CategoryTypeID: categoryTypeID,
                Name: categoryName
            };

            $.ajax({
                data: model,
                url: '/Open24Api/PostAPI/' + "InsertCategory",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (item) {
                    alert("Success");
                    $('#myModal5').modal('hide');
                    getAllCategoriesNews();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log('err');
                }
            })
        }
    }

    self.insertCateTypeRecruit = function () {
        var categoryTypeID = $("#selCateTypeRecruit").val();
        var categoryTypeName = self.valCategoryTypeNameRecruit();

        var model = {
            CategoryTypeID: categoryTypeID,
            Name: categoryTypeName
        };

        $.ajax({
            data: model,
            url: '/Open24Api/PostAPI/' + "InsertCategory",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                alert("Success");
                getAllCategoriesNews();
                $('#myModal5').modal('Hide');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log('err');
            }
        })
    }



};
ko.applyBindings(new View());


function ajaxHelper(uri, method, data) {

    return $.ajax({
        type: method,
        url: uri,
        dataType: 'json',
        contentType: 'application/json',
        data: data ? JSON.stringify(data) : null,
        async: false,
        statusCode: {
            404: function () {
                console.log("Page not found");
            },
        }
    })
        .fail(function (jqXHR, textStatus, errorThrown) {
            console.log("1");
            console.log(textStatus + ": " + errorThrown + ": " + jqXHR.responseText);
        });
}

function getDate() {
    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();
    var output = d.getFullYear() + '-' + (month < 10 ? '0' : '') + month + '-' + (day < 10 ? '0' : '') + day;
    return output;
}
