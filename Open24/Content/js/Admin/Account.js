function AccountLogin() {
    var self = this;
    self.userName = ko.observable()
    self.password = ko.observable();
    self.myMessage = ko.observable();
    self.LoginAccount = function () {
        var user = self.userName();
        var pass = self.password();
        if (user != undefined
            && user.replace(/\s+/g, '') != ""
            && pass != undefined
            && pass.replace(/\s+/g, '') != "")
        {
            var userLogin = {
                UserName: user,
                UserPassword: pass,
            };
            $.ajax({
                url: '/AdminPage/Account/Login',
                type: 'POST',
                data: ko.toJSON({ UserLogin: userLogin }),
                contentType: 'application/json',
                success: function (result) {
                    if (result.res === true) {
                        self.myMessage();
                        location.href = "/Home/Index";
                    }
                    else {
                        self.myMessage(result.mess);
                    }
                },
                error: function () {
                    alert("Đã xảy ra lỗi.");
                }
            });
        }
        else
        {
            self.myMessage("Vui lòng điền đầy đủ thông tin tài khoản và mật khẩu.");
        }
    };
};
ko.applyBindings(new AccountLogin());