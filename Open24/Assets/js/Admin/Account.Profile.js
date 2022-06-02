var ProfileUser = function () {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.UserGroupList = ko.observableArray();
    self.koUserName = ko.observable();
    self.koPasswordNew = ko.observable();
    self.koPasswordOld = ko.observable();
    self.koPasswordconfluent = ko.observable();
    self.koName = ko.observable();
    self.koAddress = ko.observable();
    self.koEmail = ko.observable();
    self.koPhone = ko.observable();
    //===============================
    // Thêm mới người dùng
    //===============================
    self.insertProfileUser = function () {
        if (self.koUserName() === undefined
            || self.koUserName().replace(/\s+/g, '') === "") {
            AlertError("Vui lòng nhập tài khoản.");
            return; 

        }
        if (self.koPasswordOld() === undefined
            || self.koPasswordOld().replace(/\s+/g, '') === "") {

            if (self.koPasswordNew() !== undefined
                && self.koPasswordNew().replace(/\s+/g, '') !== "") {
                AlertError("Vui lòng nhập mật khẩu cũ.");
                return;
            }
            else if (self.koPasswordconfluent() !== undefined
                && self.koPasswordconfluent().replace(/\s+/g, '') !== "") {
                AlertError("Vui lòng nhập mật khẩu cũ.");
                return;
            }
        }
        if (self.koPasswordNew() === undefined
            || self.koPasswordNew().replace(/\s+/g, '') === "") {
            if (self.koPasswordconfluent() !== undefined
                && self.koPasswordconfluent().replace(/\s+/g, '') !== "") {
                AlertError("Vui lòng nhập mật khẩu mới.");
                return;
            }
        }
        if (self.koPasswordconfluent() === undefined
            || self.koPasswordconfluent().replace(/\s+/g, '') === "") {

            if (self.koPasswordNew() !== undefined
                && self.koPasswordNew().replace(/\s+/g, '') !== "") {
                AlertError("Vui lòng nhập lại mật khẩu mới.");
                return;
            }
        }
        else if (!validateEmail(self.koEmail())) {
            AlertError("Địa chỉ Email không hợp lệ.");
            return;
        }
        else if (!validatePhone(self.koPhone())) {
            AlertError("Số điện thoại không hợp lệ.");
            return;
        }

        var object = {
            UserName: self.koUserName(),
            PasswordNew: self.koPasswordNew(),
            PasswordOld: self.koPasswordOld(),
            Passwordconfluent: self.koPasswordconfluent(),
            Name: self.koName(),
            BirthDay: $('#datetimepicker').val(),
            Address: self.koAddress(),
            Email: self.koEmail(),
            Phone: self.koPhone(),
        }
        $.ajax({
            url: '/AdminPage/Account/UpdateProfile',
            type: 'POST',
            data: ko.toJSON({ model: object }),
            contentType: 'application/json',
            success: function (result) {
               
                if (result.res === true) {
                    AlertNotice(result.mess);
                }
                else {
                    AlertError(result.mess);
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi.");
            }
        });


    };
    self.ShowUserGroup = function () {

    }
    //===============================
    // Load dữ liệu khi lên form
    //===============================
    function GetGroupUser() {
        $.ajax({
            url: '/AdminPage/News_User/GetGroupUser',
            type: 'POST',
            contentType: 'application/json',
            success: function (result) {
                if (result.res === true) {
                    self.UserGroupList(result.DataSoure);
                    $('#selectUserGroup').val(result.DataSoure.GroupID);
                }
                else {
                    AlertError(result.mess)
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi.");
            }
        });

    }
    GetGroupUser();
    function GetProfileUser() {
        $.ajax({
            url: '/AdminPage/Account/GetProfileUser',
            type: 'POST',
            contentType: 'application/json',
            success: function (result) {
                if (result.res === true) {
                    self.koUserName(result.DataSoure.UserName);
                    self.koName(result.DataSoure.Name);
                    var $datepicker = $('#datetimepicker');
                    $datepicker.datetimepicker();
                    $datepicker.datetimepicker({ dateFormat: 'MM/DD/YYYY' });
                    if (result.DataSoure.BirthDay !== undefined
                        && result.DataSoure.BirthDay !== null
                        && result.DataSoure.BirthDay.replace(/\s+/g, '') !== "") {
                        var date = moment(result.DataSoure.BirthDay).format('MM/DD/YYYY');

                        $datepicker.val(date);
                    }
                    $('#selectUserGroup').val(result.DataSoure.GroupID);
                    self.koAddress(result.DataSoure.Address);
                    self.koEmail(result.DataSoure.Email);
                    self.koPhone(result.DataSoure.Phone);
                }
                else {
                    AlertError(result.mess)
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi.");
            }
        });

    }
    GetProfileUser();
}
ko.applyBindings(new ProfileUser());

