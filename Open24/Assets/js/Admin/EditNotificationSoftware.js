function View() {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.TitleForm = ko.observable("Thêm mới thông báo phần mềm");
    self.koTitle = ko.observable();
    self.koStatusNews = ko.observable(true);
    self.kosubdomain = ko.observable();
    function Ready() {
        if (!localValidate.CheckNull($('#KeyNotificationID').val())) {
            self.TitleForm("Cập nhật thông báo phần mềm");
            $.ajax({
                url: '/Open24Api/ApiHome/GetNotificationSoftwareById/' + $('#KeyNotificationID').val(),
                type: 'GET',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {
                    if (result.res === true) {
                        CKEDITOR.instances['txtbodycontent'].setData(result.DataSoure.BodyContent);
                        if (result.DataSoure.ApplyDate !== null && result.DataSoure.ApplyDate !== undefined) {
                            var date = moment(result.DataSoure.ApplyDate).format('MM/DD/YYYY');
                            $("#datetimepicker").find("input").val(date);
                        }
                        $('#selecttype').val(result.DataSoure.Type);
                        anableSubdomain($('#selecttype').val());
                        self.kosubdomain(result.DataSoure.Subdomain);
                        self.koStatusNews(result.DataSoure.Status);
                        self.koTitle(result.DataSoure.Title);
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
    }
    Ready();

    function anableSubdomain(value)
    {
        if ($('#NotificationSoftware').val() !== value) {
            $('.adressSubdomain').show();
        }
        else {
            $('.adressSubdomain').hide();
        }
    }
    $('#selecttype').change(function () {
        anableSubdomain($(this).val());
       
    });
    //===============================
    // Cập nhật thông báo
    //===============================
    self.EditNotification = function (model) {
        var mota = CKEDITOR.instances['txtbodycontent'].getData();
        if (localValidate.CheckNull(self.koTitle())) {

            AlertError("Vui lòng nhập tên thông báo.");
        }
        else if (localValidate.CheckNull(mota)) {

            AlertError("Vui lòng nhập mô tả thông báo");
        }
        else if ($('.adressSubdomain').css('display') !== 'none' && localValidate.CheckNull(self.kosubdomain())) {
            AlertError("Vui lòng nhập địa chỉ cửa hàng cần thông báo.");
        }

        else {
            model = {
                ID: $('#KeyNotificationID').val(),
                BodyContent: mota,
                ApplyDate: $("#datetimepicker").find("input").val(),
                Type: $('#selecttype').val(),
                Subdomain: self.kosubdomain(),
                Status: self.koStatusNews(),
                Title: self.koTitle()
            };
            $.ajax({
                data: model,
                url: '/Open24Api/ApiHome/EditNotificationSoftware',
                type: 'POST',
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                success: function (result) {

                    if (result.res === true) {
                        AlertNotice(result.mess);
                        location.href = "/AdminPage/Home/NotificationSoftware";
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

    }
};
ko.applyBindings(new View());
