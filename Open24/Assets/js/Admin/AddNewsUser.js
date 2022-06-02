var AddNewsUser = function () {
    var self = this;
    //===============================
    // Khai báo chung
    //===============================
    self.UserGroupList = ko.observableArray();
    self.koUserName = ko.observable();
    self.koPassword = ko.observable();
    self.koName = ko.observable();
    self.koAddress = ko.observable();
    self.koEmail = ko.observable();
    self.koPhone = ko.observable();
    self.koStatus = ko.observable(true);
    self.koConfluentPassword = ko.observable();
    //===============================
    // Thêm mới người dùng
    //===============================
    self.insertUser = function () {
        if (self.koUserName() === undefined
            || self.koUserName().replace(/\s+/g, '') === "")
        {
            AlertError("Vui lòng nhập tài khoản."); 

        }
        else if (self.koUserName().replace(/\s+/g, '').length<3) {
            AlertError("Tài khoản chứa ít nhất 3 ký tự");

        } 
        else if (self.koPassword() === undefined
            || self.koPassword().replace(/\s+/g, '') === "") {
            AlertError("Vui lòng nhập mật khẩu.");
        }
        else if (self.koPassword().replace(/\s+/g, '').length < 6) {
            AlertError("Mật khẩu chứa ít nhất 6 ký tự");

        } 
        else if (self.koName() === undefined
            || self.koName() === null
            || self.koName().replace(/\s+/g, '') === "") {
            AlertError("Vui lòng nhập họ tên.");
        }
        else if (self.koConfluentPassword() === undefined
            || self.koConfluentPassword().replace(/\s+/g, '') === "") {
            AlertError("Vui lòng nhập lại mật khẩu khớp .");
        }
        else if (self.koConfluentPassword().replace(/\s+/g, '') !== self.koPassword().replace(/\s+/g, '')) {
            AlertError("mật khẩu nhập lại không khớp với mật khẩu trên.");
        }
        else if (!validateEmail(self.koEmail())) {
            AlertError("Địa chỉ Email không hợp lệ.");
        }
        else if (!validatePhone(self.koPhone()))
        {
            AlertError("Số điện thoại không hợp lệ.");
        }
        else if (validatespecialcharacters(self.koUserName())) {
            AlertError("Tài khoản không được chứa ký tự đặc biệt.");
        }
        else if (validatespecialcharacters(self.koName())) {
            AlertError("Họ tên không được chứa ký tự đặc biệt.");
        }
        else
        {
            var object = {
                UserName: self.koUserName().replace(/\s+/g, ''),
                Password: self.koPassword().replace(/\s+/g, ''),
                Name: self.koName(),
                BirthDay: $('#datetimepicker').val(),
                GroupID: $('#selCategoryNews').val(),
                Address: self.koAddress(),
                Email: self.koEmail(),
                Phone: self.koPhone(),
                Status: self.koStatus(),
            }
            $.ajax({
                url: '/AdminPage/News_User/CreateUser',
                type: 'POST',
                data: ko.toJSON({ model: object, ConfluentPassword:self.koConfluentPassword().replace(/\s+/g, '') }),
                contentType: 'application/json',
                success: function (result) {
                    if (result.res === true) {
                        location.href = "/AdminPage/News_User/";
                    }
                    else {
                        AlertError(result.mess);
                    }
                },
                error: function () {
                    alert("Đã xảy ra lỗi.");
                }
            });

        }
    };
    self.ShowUserGroup = function ()
    {

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
}
ko.applyBindings(new AddNewsUser());

